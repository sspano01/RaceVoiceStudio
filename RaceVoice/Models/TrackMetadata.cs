using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace RaceVoice
{
    public class TrackMetadata
    {
        public string TrackName { get; set; }
        public IList<UInt16> DataBitfields { get; set; }
        public int Rotation { get; set; }
        public int Zoom { get; set; }
        public int XPan { get; set; }
        public int YPan { get; set; }
        public int ClusterSize { get; set; }
        public bool UseCurveRendering { get; set; }

        public IList<bool> SplitEnabledStates { get; set; }
        public IList<bool> SpeechTagEnabledStates { get; set; }

        public TrackMetadata()
        {
            Zoom = 90;
            ClusterSize = 4;
        }

        public void Save(string filepath)
        {
            File.WriteAllText(filepath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static TrackMetadata Load(string filepath)
        {
            try
            {
                return JsonConvert.DeserializeObject<TrackMetadata>(File.ReadAllText(filepath));
            }
            catch
            {
                globals.WriteLine("NO JSON "+filepath);
            }

            return null;
        }
    }
}