using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceVoice
{
    public class CarMetadata
    {
        public EngineData EngineData { get; set; }
        public DynamicsData DynamicsData { get; set; }
        public HardwareData HardwareData { get; set; }
        public IList<MessageTrigger> MessageTriggers { get; set; }

        public void Save(string filepath)
        {
            File.WriteAllText(filepath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static CarMetadata Load(string filepath)
        {
            CarMetadata metadata = null;
                     
            try
            {
                metadata = JsonConvert.DeserializeObject<CarMetadata>(File.ReadAllText(filepath));
               
            }
            catch (FileNotFoundException)
            {
                metadata = new CarMetadata()
                {
                    EngineData = new EngineData(),
                    DynamicsData = new DynamicsData(),
                    HardwareData = new HardwareData(),
                    MessageTriggers = new List<MessageTrigger>()
                };
            }

            // make sure we have some values to allow for new items in the structure

            if (metadata.EngineData == null) metadata.EngineData = new EngineData();
            if (metadata.DynamicsData == null) metadata.DynamicsData = new DynamicsData();
            if (metadata.HardwareData == null) metadata.HardwareData = new HardwareData();
            if (metadata.MessageTriggers == null) metadata.MessageTriggers = new List<MessageTrigger>();

            if (metadata.EngineData.EcuName == null) metadata.EngineData.EcuName = "";
          
            return metadata;
        }
    }
}
