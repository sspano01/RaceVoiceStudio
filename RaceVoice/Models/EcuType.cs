﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using JR.Utils.GUI.Forms;


namespace RaceVoice
{
    public enum EcuType
    {
        AIM,
        MoTec,
        SmartyCam1,
        SmartyCam2,
        VBOX
    }

    public class EcuData
    {
        public string Name { get; set; }
        public int Baud { get; set; }
        public bool Active { get; set; }
        public string TPS { get; set; }
        public string OILP { get; set; }
        public string ECT { get; set; }
        public string RPM { get; set; }
    }


    public class EcuMetadata
    {

        public void PopulateECU(ComboBox ecuType)
        {
            EcuData metadata = null;

            if (Directory.Exists(globals.ecu_folder))
            {
                var files = Directory.GetFiles(globals.ecu_folder, "*.json");
                foreach (var filepath in files)
                {
                    globals.WriteLine("Parse ECU File->" + filepath);
                    try
                    {
                        metadata = JsonConvert.DeserializeObject<EcuData>(File.ReadAllText(filepath));
                        ecuType.Items.Add(metadata.Name);

                    }
                    catch (FileNotFoundException)
                    {
                        metadata = null;
                    }
                }

            }
        }
    }



}