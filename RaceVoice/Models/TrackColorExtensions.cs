using System.Drawing;

namespace RaceVoice.Models
{
    public static class TrackColorExtensions
    {
        public static TrackColor ToTrackColor(this Color c)
        {
            return new TrackColor() { R = c.R, G = c.G, B = c.B };
        }

        public static Color ToDrawingColor(this TrackColor c)
        {
            return Color.FromArgb(255, c.R, c.G, c.B);
        }
    }
}
