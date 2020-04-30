using System;

namespace RaceVoice
{
    public class TrackSegment
    {
        public int StartDistance { get; set; }
        public int EndDistance { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string Name { get; set; }
        public TrackColor StartColor { get; set; }
        public TrackColor EndColor { get; set; }
        public UInt16 DataBits { get; set; }
        public bool Hidden { get; set; }
    }
}