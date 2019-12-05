using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceVoice.Models
{
    public class SessionDataModel
    {
        public IDictionary<int, LapSegmentsModel> Laps { get; set; }
        public IDictionary<int, double> LapTimes { get; set; }
    }
}
