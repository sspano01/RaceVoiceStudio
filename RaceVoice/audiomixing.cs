using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaceVoice
{
    public partial class audiomixing : Form
    {
        int iracing_vol = 0;
        int racevoice_vol = 0;
        int iracing_pid = 0;
        int current_pid = 0;
        bool ready = false;
        CarMetadata metadata;
        public audiomixing()
        {
            InitializeComponent();

            current_pid = AudioManager.GetProcessID("racevoice");
            iracing_pid=AudioManager.GetProcessID("iracingsim");

            ready = true;

        }

        public void UpdateVolumes(CarMetadata _metadata)
        {
            // apply defaults
            if (_metadata.HardwareData.iRacingVolume == 0) _metadata.HardwareData.iRacingVolume = 80;
            if (_metadata.HardwareData.SimVolume == 0) _metadata.HardwareData.SimVolume = 100;
            if (iracing_pid!=0) AudioManager.SetApplicationVolume(iracing_pid, (float)_metadata.HardwareData.iRacingVolume);
            if (current_pid != 0) AudioManager.SetApplicationVolume(current_pid, (float)_metadata.HardwareData.SimVolume);


        }

        public void SetData(CarMetadata _metadata)
        {
            metadata = _metadata;
            racevoice_vol = (int)AudioManager.GetApplicationVolume(current_pid);
            iracing_vol = (int)AudioManager.GetApplicationVolume(iracing_pid);

            iracing_trackbar.Value = iracing_vol;
            RaceVoice_trackbar.Value = racevoice_vol;
            globals.WriteLine("Mixer:Iracing Volume=" + iracing_vol + " RaceVoice Volume=" + racevoice_vol);

        }
        private void close_Click(object sender, EventArgs e)
        {
            Close();

        }

        private void iracing_trackbar_Scroll(object sender, EventArgs e)
        {
            if (!ready) return;
            iracing_vol = iracing_trackbar.Value;
            metadata.HardwareData.iRacingVolume = iracing_vol;
            if (iracing_pid == 0) return;
            AudioManager.SetApplicationVolume(iracing_pid, (float)iracing_vol);

        }

        private void RaceVoice_trackbar_Scroll(object sender, EventArgs e)
        {
            if (!ready) return;
            racevoice_vol = RaceVoice_trackbar.Value;
            metadata.HardwareData.SimVolume = racevoice_vol;
            if (current_pid == 0) return;
            AudioManager.SetApplicationVolume(current_pid, (float)racevoice_vol);
        }
    }
}
