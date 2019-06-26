using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceVoice
{
    public class Charting
    {
        public static void GenerateChartBundleFromMoTecCsv(string inFile, string outFile)
        {
            MoTecCsv csv = MoTecCsv.LoadCsvFile("data.csv");
            var trace = csv.ToDataTrace();

            GenerateChartData(trace, outFile);
        }

        public static void GenerateChartData(IList<LapDataTrace> lapData, string outFile)
        {
            var sampleList = new List<IList<DataTracePoint>>();
            for (int i = 1; i < lapData.Count - 1; i++)
            {
                DataTraceSampler sampler = new DataTraceSampler(lapData[i].DataPoints);
                var samples = sampler.SampleByDistance(250);
                sampleList.Add(samples);
            }

            var json = JsonConvert.SerializeObject(sampleList);
            var js = "(function () { var data = " + json + "; onDataDownloaded(data); })();";
            File.WriteAllText(outFile, js);
        }
    }
}
