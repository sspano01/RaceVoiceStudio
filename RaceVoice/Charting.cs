using Newtonsoft.Json;
using RaceVoice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RaceVoice
{
    public class Charting
    {
        public static void GenerateChartBundle(IList<LapDataTrace> lapData, TrackModel track,string outFolder)
        {
            var sampleList = new List<ICollection<DataTracePoint>>();
            var minTime = lapData.SelectMany(ld => ld.DataPoints).Min(d => d.Time);
            var maxTime = lapData.SelectMany(ld => ld.DataPoints).Max(d => d.Time);
            var mag = maxTime - minTime;
            var sampleCount = (int)mag;
            var bucketSize = mag / sampleCount;

            var buckets = Enumerable.Range(1, sampleCount).Select(i => bucketSize * i).ToList();

            for (int i = 1; i < lapData.Count - 1; i++)
            {
                DataTraceSampler sampler = new DataTraceSampler(lapData[i].DataPoints);
                var samples = sampler.SampleByTime(buckets);
                sampleList.Add(samples);
            }

            var json = JsonConvert.SerializeObject(sampleList);
            var js = "(function () { var data = " + json + "; onDataDownloaded(data); })();";
            File.WriteAllText(outFolder + "//data.js", js);

            var sessionData = GenerateSessionData(lapData, track);
            json = JsonConvert.SerializeObject(sessionData);
            js = "(function () { var data = " + json + "; onDataDownloaded(data); })();";
            File.WriteAllText(outFolder + "//tabledata.js", js);
        }

        private static SessionDataModel GenerateSessionData(IList<LapDataTrace> trace, TrackModel track)
        {
            Dictionary<int, LapSegmentsModel> laps = new Dictionary<int, LapSegmentsModel>();

            foreach (var lap in trace)
            {
                var entryDistances = new Dictionary<string, double>();
                var entryClosest = new Dictionary<string, int>();
                var exitDistances = new Dictionary<string, double>();
                var exitClosest = new Dictionary<string, int>();

                foreach (var seg in track.Segments)
                {
                    entryDistances[seg.Name] = double.MaxValue;
                    exitDistances[seg.Name] = double.MaxValue;
                }

                //Match up data trace points to the turn entry and exit GPS co-ordinates
                for (int i = 0; i < lap.DataPoints.Count; i++)
                {
                    var dp = lap.DataPoints[i];

                    foreach (var seg in track.Segments)
                    {
                        var firstPoint = track.Waypoints[seg.StartIndex];
                        var lastPoint = track.Waypoints[seg.EndIndex];
                        var entryDistance = SqrDistance(dp.Lat, dp.Lng, firstPoint.Latitude, firstPoint.Longitude);
                        if (entryDistance < entryDistances[seg.Name])
                        {
                            entryDistances[seg.Name] = entryDistance;
                            entryClosest[seg.Name] = i;
                        }

                        var exitDistance = SqrDistance(dp.Lat, dp.Lng, lastPoint.Latitude, lastPoint.Longitude);
                        if (exitDistance < exitDistances[seg.Name])
                        {
                            exitDistances[seg.Name] = exitDistance;
                            exitClosest[seg.Name] = i;
                        }
                    }
                }

                Dictionary<string, SegmentDataModel> segments = new Dictionary<string, SegmentDataModel>();
                foreach (var seg in track.Segments)
                {
                    var entryDp = lap.DataPoints[entryClosest[seg.Name]];
                    var exitDp = lap.DataPoints[exitClosest[seg.Name]];

                    double min = double.MaxValue;
                    double max = double.MinValue;
                    for (int i = entryClosest[seg.Name]; i <= exitClosest[seg.Name]; i++)
                    {
                        var dp = lap.DataPoints[i];
                        if (dp.Speed < min)
                        {
                            min = dp.Speed;
                        }

                        if (dp.Speed > max)
                        {
                            max = dp.Speed;
                        }
                    }
                    segments[seg.Name] = new SegmentDataModel()
                    {
                        EntrySpeed = entryDp.Speed,
                        ExitSpeed = exitDp.Speed,
                        MinSpeed = min,
                        MaxSpeed = max
                    };
                }

                laps[lap.LapNumber] = new LapSegmentsModel()
                {
                    Segments = segments
                };
            }

            return new SessionDataModel()
            {
                Laps = laps
            };
        }

        private static double SqrDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2);
        }
    }
}
