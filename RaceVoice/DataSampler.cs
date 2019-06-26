﻿using System;
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
            var chunkSize = mag / (numberOfSamples+1);

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
                    BrakePressureFront = chunkPoints.Max(p => p.BrakePressureFront),
                    BrakePressureRear = chunkPoints.Max(p => p.BrakePressureRear),
                    Throttle = chunkPoints.Max(p => p.Throttle),
                });
            }

            return samples;
        }
    }
}
