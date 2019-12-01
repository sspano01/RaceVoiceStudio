using RaceVoice;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaceVoiceLib.Parser
{
    public interface IDataTraceSource
    {
        IList<LapDataTrace> GetDataTrace();
    }
}
