using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using RaceVoice.Parser;

namespace RaceVoice
{
    public class RaceVoiceCsv : IDataTraceSource
    {
        private Dictionary<int, List<DataTracePoint>> _laps = new Dictionary<int, List<DataTracePoint>>();

        public IList<LapDataTrace> GetDataTrace()
        {
            return _laps.Select(kvp => new LapDataTrace(kvp.Key, kvp.Value))
                .OrderBy(dt => dt.LapNumber)
                .ToList();
        }

        public static RaceVoiceCsv LoadCsvFile(TrackModel track, string filename)
        {
            using (var sr = new StreamReader(File.OpenRead(filename)))
            {
                return LoadCsvFile(track, sr);
            }
        }

        public static double DistanceAlongLine(Waypoint a, Waypoint b, Waypoint p)
        {
            if (a.Latitude == b.Latitude && a.Longitude == b.Longitude)
            {
                return 0;
            }

            var dir = new Waypoint()
            {
                Latitude = b.Latitude - a.Latitude,
                Longitude = b.Longitude - a.Longitude
            };
            var len = Math.Sqrt((dir.Latitude * dir.Latitude) + (dir.Longitude * dir.Longitude));
            dir.Latitude /= len;
            dir.Longitude /= len;

            var dist = new Waypoint()
            {
                Latitude = p.Latitude - a.Latitude,
                Longitude = p.Longitude - a.Longitude
            };

            var ld = (dist.Latitude * dir.Latitude) + (dist.Longitude * dir.Longitude);

            return ld;
        }

        private static double Distance(Waypoint a, Waypoint b)
        {
            return Math.Sqrt(Math.Pow(b.Latitude - a.Latitude, 2) + Math.Pow(b.Longitude - a.Longitude, 2));
        }

        private static double[] GetWaypointDistances(TrackModel track)
        {
            double[] distances = new double[track.Waypoints.Count];

            double distance = 0;
            for (int i = 1; i < track.Waypoints.Count; i++)
            {
                var wpA = track.Waypoints[i-1];
                var wpB = track.Waypoints[i];
                distance += Distance(wpA, wpB);
                distances[i] = distance;
            }

            return distances;
        }

        private static double CalculateDistance(Waypoint wp, TrackModel track, double[] distances)
        {
            int closestIdx = 0;
            double closestDistance = double.MaxValue;

            for (int i = 0; i < track.Waypoints.Count; i++)
            {
                var twp = track.Waypoints[i];
                var d = Distance(wp, twp);
                if (d < closestDistance)
                {
                    closestIdx = i;
                    closestDistance = d;
                }
            }

            int leftIdx = closestIdx - 1;
            if (leftIdx < 0)
            {
                leftIdx = track.Waypoints.Count - 1;
            }
            int rightIdx = (closestIdx + 1) % track.Waypoints.Count;

            int nextClosestIdx = Distance(wp, track.Waypoints[leftIdx]) > Distance(wp, track.Waypoints[rightIdx]) ? rightIdx : leftIdx;

            int aIdx = -1;
            int bIdx = -1;
            if (distances[closestIdx] < distances[nextClosestIdx])
            {
                aIdx = closestIdx;
                bIdx = nextClosestIdx;
            }
            else
            {
                aIdx = nextClosestIdx;
                bIdx = closestIdx;
            }

            var p = distances[aIdx] + DistanceAlongLine(track.Waypoints[aIdx], track.Waypoints[bIdx], wp);
            return p / distances.Last();
        }

        public static RaceVoiceCsv LoadCsvFile(TrackModel track, StreamReader sr)
        {
            // Current RaceVoice Log format is
            // LOG:0X[RECORDTYPE]
            // Sample#
            // Lapnumber
            // Lattitude
            // Longitude
            // Running Lap Time at the Latt/Longitude
            // Miles Per Hour
            // Engine RPM
            // Engine Throttle Position (TPS)
            // Linear-G Force
            // Lateral-G Force
            // Number of Satellites in use
            // Download percentage, this is just a calculation of how much of the data has been downloaded. Mostly used to make a progress bar move

            RaceVoiceCsv rv = new RaceVoiceCsv();
            double[] distances = GetWaypointDistances(track);

            using (var csv = new CsvReader(sr))
            {
                while (csv.Read())
                {
                    var record = csv.Context.Record;
                    if (record[0].ToUpper().StartsWith("LOG:0X55"))
                    {
                        int lapNumber = int.Parse(record[2]);
                        lapNumber++;
                        double lat = double.Parse(record[3]);
                        double lng = double.Parse(record[4]);
                        double time = double.Parse(record[5]);
                        double speed = double.Parse(record[6]);
                        int rpm = int.Parse(record[7]);
                        double throttlePosition = int.Parse(record[8]);
                        double linearG = double.Parse(record[9]);
                        double lateralG = double.Parse(record[10]);

                        List<DataTracePoint> list = null;
                        if (!rv._laps.TryGetValue(lapNumber, out list))
                        {
                            list = new List<DataTracePoint>(100);
                            rv._laps[lapNumber] = list;
                        }

                        double lapDistance = CalculateDistance(new Waypoint()
                        {
                            Latitude = lat,
                            Longitude = lng
                        }, track, distances);

                        list.Add(new DataTracePoint()
                        {
                            Lat = lat,
                            Lng = lng,
                            Rpm = rpm,
                            Speed = speed,
                            ThrottlePosition = throttlePosition,
                            Time = time,
                            LinearG = linearG,
                            LateralG = lateralG,
                            LapDistance = lapDistance
                        });
                    }
                }
            }

            return rv;
        }
    }
}
