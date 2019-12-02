using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RaceVoice
{
    public class Charting
    {
        public static void GenerateChartBundle(IList<LapDataTrace> lapData, string outFile)
        {
            var sampleList = new List<ICollection<DataTracePoint>>();
            var minTime = lapData.SelectMany(ld => ld.DataPoints).Min(d => d.Time);
            var maxTime = lapData.SelectMany(ld => ld.DataPoints).Max(d => d.Time);
            var mag = maxTime - minTime;
            var sampleCount = (int)mag;
            var bucketSize = mag / sampleCount;

#if APP
            outFile = globals.LocalFolder() + "//" + outFile;
#endif
            var buckets = Enumerable.Range(1, sampleCount).Select(i => bucketSize * i).ToList();

            for (int i = 1; i < lapData.Count - 1; i++)
            {
                DataTraceSampler sampler = new DataTraceSampler(lapData[i].DataPoints);
                var samples = sampler.SampleByTime(buckets);
                sampleList.Add(samples);
            }

            var json = JsonConvert.SerializeObject(sampleList);
            var js = "(function () { var data = " + json + "; onDataDownloaded(data); })();";
            File.WriteAllText(outFile, js);
        }
    }
}
