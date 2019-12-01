using RaceVoice;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using CsvHelper;

namespace RaceVoiceLib.Parser
{
    public class AimCsv : IDataTraceSource
    {
        private enum FileSection
        {
            Metadata,
            Headings,
            Values
        }

        public IDictionary<string, IList<string>> Metadata { get; private set; }
        public IList<string> Headings { get; private set; }
        public IList<string> Units { get; private set; }
        public IList<IList<IList<double>>> LapValues { get; private set; }

        public AimCsv()
        {
            Metadata = new Dictionary<string, IList<string>>();
            Headings = new List<string>();
            Units = new List<string>();
            LapValues = new List<IList<IList<double>>>();
        }

        public IList<LapDataTrace> GetDataTrace()
        {
            List<LapDataTrace> dataTrace = new List<LapDataTrace>(LapValues.Count);

            foreach (var values in LapValues)
            {
                List<DataTracePoint> points = values.Select(v => new DataTracePoint()
                {
                    LapDistance = v[1],
                    Speed = v[11],
                    Time = v[0],
                    Lat = v[33],
                    Lng = v[34],
                    Rpm = (int)v[12],
                    ThrottlePosition = v[14]
                }).ToList();

                dataTrace.Add(new LapDataTrace(dataTrace.Count + 1, points));
            }

            return dataTrace;
        }

        public static AimCsv LoadCsvFile(string filename)
        {
            using (var sr = new StreamReader(File.OpenRead(filename)))
            {
                return LoadCsvFile(sr);
            }
        }

        public void SaveChartFile(string filepath, int maxDataPoints)
        {
            IList<IList<IList<double>>> decimatedValues = new List<IList<IList<double>>>();
            foreach (var list in LapValues)
            {
                double chk = -1;
                int stride = Math.Max(1, list.Count / maxDataPoints);
                var decimatedLap = new List<IList<double>>();
                for (int i = 0; i < list.Count; i += stride)
                {
                    double dist = list[i][11];
                    if (dist < chk)
                    {
                        continue;
                    }
                    decimatedLap.Add(list[i]);
                    chk = dist;
                }
                decimatedValues.Add(decimatedLap);
            }

            File.WriteAllText(filepath, JsonConvert.SerializeObject(new
            {
                Headings,
                Units,
                LapValues = decimatedValues
            }, Formatting.Indented));
        }

        public static AimCsv LoadCsvFile(StreamReader sr)
        {
            AimCsv aim = new AimCsv();

            FileSection section = FileSection.Metadata;

            double lastTime = double.MaxValue;

            using (var csv = new CsvReader(sr))
            {
                csv.Configuration.IgnoreBlankLines = false;
                while (csv.Read())
                {
                    var record = csv.Context.Record;

                    if (record.Length == 0)
                    {
                        if (section == FileSection.Metadata)
                        {
                            section = FileSection.Headings;

                            //The headings seem to appear twice in an aim csv file, so skip the first one
                            csv.Read();
                            record = csv.Context.Record;
                        }
                        else if (section == FileSection.Headings)
                        {
                            section = FileSection.Values;
                        }

                        csv.Read();
                        record = csv.Context.Record;
                    }

                    switch (section)
                    {
                        case FileSection.Metadata:
                            aim.Metadata.Add(record.First(), record.Skip(1).ToList());
                            break;

                        case FileSection.Headings:
                            if (aim.Headings.Count == 0)
                            {
                                aim.Headings = record.ToList();
                            }
                            else
                            {
                                aim.Units = record.ToList();

                                //Next there is some ordinal list. Not sure what this is for yet. Skip it!
                                csv.Read();
                                record = csv.Context.Record;
                            }
                            break;

                        case FileSection.Values:
                            try
                            {
                                var values = record.Select(double.Parse).ToList();
                                if (values[0] < lastTime)
                                {
                                    aim.LapValues.Add(new List<IList<double>>());
                                }
                                lastTime = values[0];

                                IList<IList<double>> lapList = aim.LapValues.Last();
                                lapList.Add(values);
                            }
                            catch
                            {
                                continue;
                            }
                            break;
                    }
                }
            }

            return aim;
        }
    }
}
