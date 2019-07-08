using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceVoice
{
    public class LapDataTrace
    {
        public int LapNumber { get; private set; }
        public IList<DataTracePoint> DataPoints { get; private set; }

        public LapDataTrace(int lapNumber, IList<DataTracePoint> dataPoints)
        {
            LapNumber = lapNumber;
            DataPoints = dataPoints;
        }
    }
}
