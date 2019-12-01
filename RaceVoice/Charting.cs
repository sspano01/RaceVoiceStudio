using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace RaceVoice
{
    public class Charting
    {
        public static void GenerateChartBundle(IList<LapDataTrace> lapData, string outFile)
        {
            var sampleList = new List<IList<DataTracePoint>>();
            for (int i = 1; i < lapData.Count - 1; i++)
            {
                DataTraceSampler sampler = new DataTraceSampler(lapData[i].DataPoints);
                var samples = sampler.SampleByTime(20);
                sampleList.Add(samples);
            }

            var json = JsonConvert.SerializeObject(sampleList);
            var js = "(function () { var data = " + json + "; onDataDownloaded(data); })();";
            File.WriteAllText(outFile, js);
        }
    }
}
