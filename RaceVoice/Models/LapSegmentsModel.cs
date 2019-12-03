using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceVoice.Models
{
    public class LapSegmentsModel
    {
        public IDictionary<string, SegmentDataModel> Segments { get; set; }
    }
}
