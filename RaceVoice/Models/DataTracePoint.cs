using Newtonsoft.Json;

namespace RaceVoice
{
    public class DataTracePoint
    {   
        //0      1                2              3              4               5             6           7             8              9              10     11             12         13                      14           15          16                   17                   18              19                 20                21           22            23                    24                       25              26            
        //"Time","Steering Angle","GPS Altitude","GPS Latitude","GPS Longitude","GPS Heading","GPS Speed","G Force Lat","G Force Long","G Force Vert","Gear","Lap Distance","Lap Time","Lap Gain/Loss Running","Lap Number","Lap Speed","Lap Time Predicted","Reference Lap Time","GPS Sats Used","Brake Pres Front","Brake Pres Rear","Engine RPM","Engine Temp","Engine Oil Pressure","Engine Oil Temperature","Fuel Pressure","Throttle Pos"
        //"s",   "deg",           "m",           "deg",         "deg",          "deg",        "mph",      "G",          "G",           "G",           "",    "m",           "s",       "s",                    "",          "km/h",     "s",                 "s",                 "",             "kPa",             "kPa",            "rpm",       "C",          "kPa",                "C",                     "kPa",          "%"`

        [JsonProperty("ti")]
        public double Time { get; set; }

        [JsonProperty("ld")]
        public double LapDistance { get; set; }

        [JsonProperty("s")]
        public double Speed { get; set; }

        [JsonProperty("th")]
        public double Throttle { get; set; }

        [JsonProperty("bf")]
        public double BrakePressureFront { get; set; }

        [JsonProperty("br")]
        public double BrakePressureRear { get; set; }
    }
}
