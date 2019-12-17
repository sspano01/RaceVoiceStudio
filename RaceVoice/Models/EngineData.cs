namespace RaceVoice
{
    public class EngineData
    {
        public EcuType EcuType { get; set; }
        public string EcuName { get; set; }

        public bool OverRevEnabled { get; set; }
        public int OverRev { get; set; }

        public ShiftNotificationType ShiftNotificationType { get; set; }

        public bool UpShiftEnabled { get; set; }
        public int UpShift { get; set; }

        public bool DownShiftEnabled { get; set; }
        public int DownShift { get; set; }

        public bool OilPressureEnabled { get; set; }
        public int OilPressurePsi { get; set; }
        public int OilPressureRpm { get; set; }
        
        public bool TemperatureEnabled { get; set; }
        public int Temperature { get; set; }

        public bool VoltageEnabled { get; set; }
        public double Voltage { get; set; }

        public int TrackSelectionIndex { get; set; }
        public string TrackSelectionName { get; set; }
        public bool FindTrackByName { get; set; }

        public EngineData()
        {
            OverRevEnabled = true;
            OverRev = 8000;

            ShiftNotificationType = ShiftNotificationType.Speech;
            UpShiftEnabled = true;
            UpShift = 6550;

            DownShiftEnabled = false;
            DownShift = 4000;

            OilPressureEnabled = true;
            OilPressurePsi = 10;
            OilPressureRpm = 1200;

            TemperatureEnabled = true;
            Temperature = 220;

            VoltageEnabled = true;
            Voltage = 11.5;

            TrackSelectionIndex = 0;
            TrackSelectionName = "";
            FindTrackByName = false;
        
        }
    }
}