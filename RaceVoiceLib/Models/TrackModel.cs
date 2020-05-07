using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace RaceVoice
{
    public class TrackModel
    {
        public Waypoint StartLinePosition { get; set; }
        public IList<Waypoint> Waypoints { get; set; }
        public IList<TrackSegment> Segments { get; set; }
        public IList<TrackSplit> Splits { get; set; }
        public IList<TrackSpeechTag> SpeechTags { get; set; }
        public int SampleRate { get; set; }

        public int FlagPosition { get; set; }

        public bool IsStraightTrack { get; set; }

        public TrackModel()
        {
            StartLinePosition = new Waypoint();
            Waypoints = new List<Waypoint>();
            Segments = new List<TrackSegment>();
            Splits = new List<TrackSplit>();
            SpeechTags = new List<TrackSpeechTag>();
        }


        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //:::                                                                         :::
        //:::  This routine calculates the distance between two points (given the     :::
        //:::  latitude/longitude of those points). It is being used to calculate     :::
        //:::  the distance between two locations using GeoDataSource(TM) products    :::
        //:::                                                                         :::
        //:::  Definitions:                                                           :::
        //:::    South latitudes are negative, east longitudes are positive           :::
        //:::                                                                         :::
        //:::  Passed to function:                                                    :::
        //:::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
        //:::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
        //:::    unit = the unit you desire for results                               :::
        //:::           where: 'M' is statute miles (default)                         :::
        //:::                  'K' is kilometers                                      :::
        //:::                  'N' is nautical miles                                  :::
        //:::                                                                         :::
        //:::  Worldwide cities and other features databases with latitude longitude  :::
        //:::  are available at https://www.geodatasource.com                         :::
        //:::                                                                         :::
        //:::  For enquiries, please contact sales@geodatasource.com                  :::
        //:::                                                                         :::
        //:::  Official Web site: https://www.geodatasource.com                       :::
        //:::                                                                         :::
        //:::           GeoDataSource.com (C) All Rights Reserved 2018                :::
        //:::                                                                         :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                else if (unit =='F')
                {
                    dist = dist * 5280; //feet
                }
                return (dist);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }


        private int CalcDistance(int posidx)
        {
            int dist = 0;
            int loop = 0;
            int track_index_length = Waypoints.Count();
            int start_line_index = FlagPosition;
            bool first = true;
            int last_index = 0;
            Waypoint wa = new Waypoint();
            Waypoint wb = new Waypoint();
            // sweeep across all of the points
            for (loop=0;loop<track_index_length;loop++)
            {
                if (!first)
                {
                    wb = Waypoints[start_line_index];
                    //Console.WriteLine("Last=" + last_index + " Now=" + start_line_index);
                    dist += (int)distance(wa.Latitude, wa.Longitude, wb.Latitude, wb.Longitude,'F');
                }

                last_index = start_line_index;
                wa = Waypoints[last_index];

                if (start_line_index >= posidx) break;

                start_line_index++;
                if (start_line_index >= track_index_length) start_line_index = 0; // account for a repositioned start/finish

                first = false;
            }

            int pct = Convert.ToInt32((double)start_line_index/ (double)(track_index_length) * (double)100);
            Console.WriteLine("Distance from Start-Finish to Index" + posidx + " Is " + dist +" Percent="+pct);
            return pct;
        }
        public void CalculateDistances(int maxseg,int maxsplit,int maxtags)
        {

            for (int i = 0; i < maxseg; i++)
            {
                Waypoint segmentStart = new Waypoint();
                Waypoint segmentStop = new Waypoint();

                if (Segments.Count > i)
                {
                    var segment = Segments[i];
                    var waypoints = GetSegmentWaypoints(segment);
                    segmentStart = waypoints.First();
                    segmentStop = waypoints.Last();
                    Segments[i].StartDistance=CalcDistance(segment.StartIndex);
                    Segments[i].EndDistance = CalcDistance(segment.EndIndex);
                    Console.WriteLine("Segment " + i.ToString() + " Start=" + Segments[i].StartDistance + " Stop="+Segments[i].EndDistance);
                }
            }

            for (int i=0;i<maxsplit;i++)
            {
                Waypoint waypoint = new Waypoint();
                if (i < Splits.Count)
                {
                    var split = Splits[i];
                    waypoint = GetSplitWaypoint(split);
                    Console.WriteLine("Split " + i.ToString() + " Distace=" + split.Distance);
                    Splits[i].Distance = CalcDistance(split.Index);
                }


            }

            for (int i = 0; i < maxtags; i++)
            {
                Waypoint waypoint = new Waypoint();
                if (i<SpeechTags.Count)
                {
                    var split = SpeechTags[i];
                    waypoint = GetSpeechWaypoint(split);
                    Console.WriteLine("Speech " + i.ToString() + " Distace=" + split.Distance+" Tag="+split.Phrase);
                    SpeechTags[i].Distance = CalcDistance(split.Index);
                }


            }

        }


        public IList<Waypoint> GetSegmentWaypoints(TrackSegment segment)
        {
            return Waypoints.Skip(segment.StartIndex)
                .Take(segment.EndIndex - segment.StartIndex)
                .ToList();
        }

        public Waypoint GetSplitWaypoint(TrackSplit split)
        {
            return Waypoints[split.Index];
        }

        public Waypoint GetSpeechWaypoint(TrackSpeechTag tag)
        {
            return Waypoints[tag.Index];
        }
    }
}