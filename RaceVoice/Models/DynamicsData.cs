using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceVoice
{
    
    public class DynamicsData
    {
        public bool AnnounceLateralGForce { get; set; }
        public double LateralGForceThreshold { get; set; }

        public bool AnnounceLinearGForce { get; set; }
        public double LinearGForceThreshold { get; set; }

        public bool ActiveWheelLockDetectionEnabled { get; set; }
        public int BrakeThresholdPsi { get; set; }
        public int WheelSpeedPercentDifference { get; set; }

        public bool AnnounceSpeed { get; set; }
        public int SpeedThreshold { get; set; }

        public bool AnnounceBestLap { get; set; }
        public bool AnnounceLapDelta { get; set; }

        public int AnnounceBrakeThresholdIndex { get; set; }
        public int BrakeThresholdMin { get; set; }
        public int BrakeThresholdMax { get; set; }
        public int BrakeMinHz { get; set; }
        public int BrakeMaxHz { get; set; }
        public int BrakeToneDuration { get; set; }


        //no longer used, just for backward compatability with JSON
        public bool AnnounceBrakeThreshold { get; set; }


        public DynamicsData()
        {
            AnnounceLateralGForce = false;
            LateralGForceThreshold = 1.0;
            AnnounceLinearGForce = false;
            LinearGForceThreshold = 1.0;

            ActiveWheelLockDetectionEnabled = false;
            WheelSpeedPercentDifference = 1;
            BrakeThresholdPsi = 100;

            AnnounceBestLap = false;
            AnnounceLapDelta = false;
            AnnounceSpeed = false;

            AnnounceBrakeThresholdIndex = 0;

            AnnounceBrakeThreshold = false;
            BrakeThresholdMin = 100;
            BrakeThresholdMax = 100;
            BrakeMinHz = 900;
            BrakeMaxHz = 900;
            BrakeToneDuration = 50;
        }
    }
}
