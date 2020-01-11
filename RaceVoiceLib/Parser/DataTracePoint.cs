using Newtonsoft.Json;

namespace RaceVoice
{
    public class DataTracePoint
    {   
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng{ get; set; }

        [JsonProperty("ti")]
        public double Time { get; set; }

        [JsonProperty("ld")]
        public double LapDistance { get; set; }

        [JsonProperty("s")]
        public double Speed { get; set; }

        [JsonProperty("rpm")]
        public int Rpm { get; set; }

        [JsonProperty("tp")]
        public double ThrottlePosition { get; set; }

        [JsonProperty("liG")]
        public double LinearG { get; set; }

        [JsonProperty("laG")]
        public double LateralG { get; set; }
    }
}
