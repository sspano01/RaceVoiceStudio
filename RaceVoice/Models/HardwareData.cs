namespace RaceVoice
{
    public class HardwareData
    {
        public string LicenseState { get; set; }
        public string Name { get; set; }
        public string HideWarnings { get; set; }
        public string FeatureCode { get; set; }
        public string Version { get; set; }
        public int Volume { get; set; }
        public string SWVersion { get; set; }
        public int Pitch { get; set; }
        public int GPSWindow { get; set; }

        public int iRacingVolume { get; set; }
        public int SimVolume { get; set; }

        //0      , 1      , 2      , 3
        //125Kb/s, 250Kb/s, 500Kb/s, 1Mb/s
        public int BaudRate { get; set; }
        public int Trace { get; set; }
        public bool GetConfigAtStart { get; set; }
        public bool ShareNewTracks { get; set; }

        public HardwareData()
        {
            LicenseState = "NONE";
            HideWarnings = "NONE";
            Name = "NONE";
            BaudRate = 1;
            FeatureCode = "NONE";
            Version = "NONE";
            SWVersion = "NONE";
            Volume = 5;
            GPSWindow = 1;
            Trace = 0;
            Pitch = 50; // fixed
            GetConfigAtStart = true;
            ShareNewTracks = true;
            iRacingVolume = 0;
            SimVolume = 0;
        }
    }
}