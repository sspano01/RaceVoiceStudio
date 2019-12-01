using CsvHelper;
using Newtonsoft.Json;
using RaceVoiceLib.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceVoice
{
    public class MoTecCsv : IDataTraceSource
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

        public MoTecCsv()
        {
            Metadata = new Dictionary<string, IList<string>>();
            Headings = new List<string>();
            Units = new List<string>();
            LapValues = new List<IList<IList<double>>>();
        }

        public IList<LapDataTrace> GetDataTrace()
        {
            List<LapDataTrace> dataTrace = new List<LapDataTrace>(LapValues.Count);

            double timeOffset = 0;
            foreach (var values in LapValues)
            {
                List<DataTracePoint> points = values.Select(v => new DataTracePoint()
                {
                    LapDistance = v[11],
                    Speed = v[6],
                    Time = v[0] - timeOffset,
                    Lat = v[3],
                    Lng = v[4],
                    Rpm = (int)v[21],
                    ThrottlePosition = v[26]
                }).ToList();

                dataTrace.Add(new LapDataTrace(dataTrace.Count + 1, points));

                timeOffset = values.Last()[0];
            }

            return dataTrace;
        }

        public static MoTecCsv LoadCsvFile(string filename)
        {
            using (var sr = new StreamReader(File.OpenRead(filename)))
            {
                return LoadCsvFile(sr);
            }
        }

        public static MoTecCsv LoadCsvFile(StreamReader sr)
        {
            MoTecCsv motec = new MoTecCsv();

            FileSection section = FileSection.Metadata;

            int lapIndex = -1;

            using (var csv = new CsvReader(sr))
            {
                csv.Configuration.IgnoreBlankLines = false;
                while (csv.Read())
                {
                    var record = csv.Context.Record;

                    if (record.Length == 0)
                    {
                        csv.Read();
                        record = csv.Context.Record;
                    }

                    if (record.Length == 0)
                    {
                        if (section == FileSection.Metadata)
                        {
                            section = FileSection.Headings;
                        }
                        else if (section == FileSection.Headings)
                        {
                            lapIndex = motec.Headings.IndexOf("Lap Number");
                            if (lapIndex == -1)
                            {
                                throw new InvalidOperationException("Invalid MoTec csv file. Couldn't find Lap Number heading.");
                            }
                            section = FileSection.Values;
                        }

                        csv.Read();
                        record = csv.Context.Record;
                    }

                    switch (section)
                    {
                        case FileSection.Metadata:
                            motec.Metadata.Add(record.First(), record.Skip(1).ToList());
                            break;

                        case FileSection.Headings:
                            if (motec.Headings.Count == 0)
                            {
                                motec.Headings = record.ToList();
                            }
                            else
                            {
                                motec.Units = record.ToList();
                            }
                            break;

                        case FileSection.Values:
                            try
                            {
                                var lapNumber = (int)double.Parse(record[lapIndex]);
                                if (motec.LapValues.Count == lapNumber)
                                {
                                    motec.LapValues.Add(new List<IList<double>>());
                                }

                                IList<IList<double>> lapList = motec.LapValues.Last();
                                lapList.Add(record.Select(double.Parse).ToList());
                            }
                            catch
                            {
                                continue;
                            }
                            break;
                    }
                }
            }

            return motec;
        }
    }
}
