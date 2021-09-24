using RaceVoice;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaceVoice.Parser
{
    public interface IDataTraceSource
    {
        IList<LapDataTrace> GetDataTrace();
    }
}
