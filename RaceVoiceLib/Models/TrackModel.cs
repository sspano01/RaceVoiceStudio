using System;
using System.Linq;
using System.Collections.Generic;

namespace RaceVoice
{
    public class TrackModel
    {
        public Waypoint StartLinePosition { get; set; }
        public IList<Waypoint> Waypoints { get; set; }
        public IList<TrackSegment> Segments { get; set; }
        public IList<TrackSplit> Splits { get; set; }
        public IList<TrackSpeechTag> SpeechTags { get; set; }
        public int SampleRate { get; set; }

        public int FlagPosition { get; set; }

        public bool IsStraightTrack { get; set; }

        public TrackModel()
        {
            StartLinePosition = new Waypoint();
            Waypoints = new List<Waypoint>();
            Segments = new List<TrackSegment>();
            Splits = new List<TrackSplit>();
            SpeechTags = new List<TrackSpeechTag>();
        }

        public IList<Waypoint> GetSegmentWaypoints(TrackSegment segment)
        {
            return Waypoints.Skip(segment.StartIndex)
                .Take(segment.EndIndex - segment.StartIndex)
                .ToList();
        }

        public Waypoint GetSplitWaypoint(TrackSplit split)
        {
            return Waypoints[split.Index];
        }
    }
}