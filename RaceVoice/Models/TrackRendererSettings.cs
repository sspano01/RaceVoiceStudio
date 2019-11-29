using System.Drawing;

namespace RaceVoice
{
    public class TrackRendererSettings
    {
        public int ClusterSize { get; private set; }

        public float TrackThickness { get; set; }
        public Color TrackColor { get; set; }
        public Color BackgroundColor { get; set; }

        public int SegmentResizeHandleSize { get; set; }
        public int SegmentThickness { get; set; }
        public Color SegmentResizeHandleColor { get; set; }
        public Color SegmentLabelColor { get; set; }
        public Color SelectedSegmentColor { get; set; }
        public Color DefaultSegmentColor { get; set; }

        public int SplitIndicatorSize { get; set; }
        public Color SplitIndicatorColor { get; set; }
        public int SplitIndicatorThickness { get; set; }
        public Color InactiveColor { get; set; }

        public Color MouseIndicatorColor { get; set; }
        public int MouseIndicatorSize { get; set; }

        public string ChequeredFlagImage { get; set; }

        public bool UseCurveRendering { get; set; }

        public bool ShowGpsPoints { get; set; }
        public Color GpsPointColor { get; set; }
        public int GpsPointSize { get; set; }

#if !APP
        public Font SegmentFont { get; set; }
        public Font SplitFont { get; set; }
#endif
        public TrackRendererSettings(int clusterSize)
        {
            ClusterSize = clusterSize;
        }
    }
}