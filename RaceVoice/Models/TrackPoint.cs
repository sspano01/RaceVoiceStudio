namespace RaceVoice
{
    public struct TrackPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public TrackPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}