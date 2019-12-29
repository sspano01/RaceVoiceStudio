using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using RaceVoice;

namespace RaceVoiceLib.Parser
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

        public static RaceVoiceCsv LoadCsvFile(string filename)
        {
            using (var sr = new StreamReader(File.OpenRead(filename)))
            {
                return LoadCsvFile(sr);
            }
        }

        public static RaceVoiceCsv LoadCsvFile(StreamReader sr)
        {
            RaceVoiceCsv rv = new RaceVoiceCsv();

            using (var csv = new CsvReader(sr))
            {
                while (csv.Read())
                {
                    var record = csv.Context.Record;
                    if (record[0].StartsWith("LOG:0X55"))
                    {
                        int lapNumber = int.Parse(record[2]);
                        lapNumber++;
                        double lat = double.Parse(record[3]);
                        double lng = double.Parse(record[4]);
                        double time = double.Parse(record[5]);
                        double speed = double.Parse(record[6]);
                        int rpm = int.Parse(record[7]);
                        double throttlePosition = int.Parse(record[8]);

                        List<DataTracePoint> list = null;
                        if (!rv._laps.TryGetValue(lapNumber, out list))
                        {
                            list = new List<DataTracePoint>(100);
                            rv._laps[lapNumber] = list;
                        }

                        list.Add(new DataTracePoint()
                        {
                            Lat = lat,
                            Lng = lng,
                            Rpm = rpm,
                            Speed = speed,
                            ThrottlePosition = throttlePosition,
                            Time = time
                        });
                    }
                }
            }

            return rv;
        }
    }
}
