using System;
using System.Drawing;

namespace RaceVoice
{
    public struct TrackColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static TrackColor Lerp(TrackColor a, TrackColor b, float t)
        {
            int dr = b.R - a.R;
            int dg = b.G - a.G;
            int db = b.B - a.B;

            return new TrackColor()
            {
                R = Clamp(a.R + (dr * t)),
                G = Clamp(a.G + (dg * t)),
                B = Clamp(a.B + (db * t)),
            };
        }

        public Color ToDrawingColor()
        {
            return Color.FromArgb(255, R, G, B);
        }

        private static byte Clamp(float v)
        {
            return (byte)Math.Min(Math.Max(0, v), 255);
        }

        public static readonly TrackColor Red = new TrackColor() { R = 255, G = 0, B = 0 };
        public static readonly TrackColor Green = new TrackColor() { R = 0, G = 255, B = 0 };
        public static readonly TrackColor Blue = new TrackColor() { R = 0, G = 0, B = 255 };
        public static readonly TrackColor LightBlue = new TrackColor() { R = 0, G = 128, B = 255 };
        public static readonly TrackColor Black = new TrackColor() { R = 0, G = 0, B = 0 };
        public static readonly TrackColor White = new TrackColor() { R = 255, G = 255, B = 255 };
        public static readonly TrackColor Gray = new TrackColor() { R = 128, G = 128, B = 128 };
    }

    public static class TrackColorExtensions
    {
        public static TrackColor ToTrackColor(this Color c)
        {
            return new TrackColor() { R = c.R, G = c.G, B = c.B };
        }
    }
}