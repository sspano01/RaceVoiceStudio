using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceVoice
{
    public class DataTraceSampler
    {
        private IList<DataTracePoint> _timeSorted;
        private IList<DataTracePoint> _distanceSorted;

        public DataTraceSampler(IList<DataTracePoint> data)
        {
            _timeSorted = data.OrderBy(d => d.Time).ToList();
            _distanceSorted = data.OrderBy(d => d.LapDistance).ToList();
        }


        public IList<DataTracePoint> SampleByDistance(int numberOfSamples)
        {
            List<DataTracePoint> samples = new List<DataTracePoint>(numberOfSamples);
            var mag = _distanceSorted.Last().LapDistance - _distanceSorted.First().LapDistance;
            var chunkSize = mag / (numberOfSamples + 1);

            var lastIdx = 0;
            var takeUpTo = _distanceSorted.First().LapDistance + chunkSize;
            for (int n = 0; n < numberOfSamples; n++)
            {
                List<DataTracePoint> chunkPoints = new List<DataTracePoint>();

                while (_distanceSorted[lastIdx].LapDistance <= takeUpTo)
                {
                    chunkPoints.Add(_distanceSorted[lastIdx]);
                    lastIdx++;
                }

                takeUpTo += chunkSize;

                samples.Add(new DataTracePoint()
                {
                    LapDistance = samples.Count * chunkSize,
                    Speed = chunkPoints.Average(p => p.Speed),
                    Time = chunkPoints.Average(p => p.Time),
                    Rpm = (int)chunkPoints.Average(p => p.Rpm),
                    //BrakePressureFront = chunkPoints.Max(p => p.BrakePressureFront),
                    //BrakePressureRear = chunkPoints.Max(p => p.BrakePressureRear),
                    ThrottlePosition = chunkPoints.Average(p => p.ThrottlePosition),
                    LateralG = chunkPoints.Average(p => p.LateralG),
                    LinearG = chunkPoints.Average(p => p.LinearG),
                });
            }

            return samples;
        }

        public ICollection<DataTracePoint> SampleByTime(IList<double> buckets)
        {
            LinkedList<DataTracePoint> samples = new LinkedList<DataTracePoint>();

            var lastIdx = 0;
            for (int i = 0; i < _timeSorted.Count; i++)
            {
                lastIdx = i;
                if (_timeSorted[i].LapDistance < 0.5)
                {
                    //Skip past data points that start at 99% at the start of the lap
                    break;
                }
            }
            for (int n = 0; n < buckets.Count; n++)
            {
                List<DataTracePoint> chunkPoints = new List<DataTracePoint>();

                while (lastIdx < _timeSorted.Count && _timeSorted[lastIdx].Time <= buckets[n])
                {
                    chunkPoints.Add(_timeSorted[lastIdx]);
                    lastIdx++;
                }

                var time = Math.Round(buckets[n], 3, MidpointRounding.AwayFromZero);
                if (chunkPoints.Any())
                {
                    samples.AddLast(new DataTracePoint()
                    {
                        LapDistance = chunkPoints.Average(p => p.LapDistance),
                        Speed = chunkPoints.Average(p => p.Speed),
                        Time = time,
                        Rpm = (int)chunkPoints.Average(p => p.Rpm),
                        //BrakePressureFront = chunkPoints.Max(p => p.BrakePressureFront),
                        //BrakePressureRear = chunkPoints.Max(p => p.BrakePressureRear),
                        ThrottlePosition = chunkPoints.Max(p => p.ThrottlePosition),
                        LinearG = chunkPoints.Average(p => p.LinearG),
                        LateralG = chunkPoints.Max(p => p.LateralG),
                    });
                }
                else
                {
                    samples.AddLast(new DataTracePoint()
                    {
                        Time = time
                    });
                }
            }

            foreach (var node in samples.Reverse())
            {
                if (node.Speed == 0)
                {
                    samples.RemoveLast();
                }
            }

            return samples;
        }
    }
}
