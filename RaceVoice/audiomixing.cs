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
        public audiomixing()
        {
            InitializeComponent();

            current_pid = AudioManager.GetProcessID("racevoice");
            iracing_pid=AudioManager.GetProcessID("iracingsim");

            racevoice_vol = (int)AudioManager.GetApplicationVolume(current_pid);
            iracing_vol = (int)AudioManager.GetApplicationVolume(iracing_pid);

            iracing_trackbar.Value = iracing_vol;
            RaceVoice_trackbar.Value = racevoice_vol;
            globals.WriteLine("Mixer:Iracing Volume=" + iracing_vol + " RaceVoice Volume=" + racevoice_vol);
            ready = true;

        }

        private void close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void iracing_trackbar_Scroll(object sender, EventArgs e)
        {
            if (!ready) return;
            iracing_vol = iracing_trackbar.Value;
            AudioManager.SetApplicationVolume(iracing_pid, (float)iracing_vol);

        }

        private void RaceVoice_trackbar_Scroll(object sender, EventArgs e)
        {
            if (!ready) return;
            racevoice_vol = RaceVoice_trackbar.Value;
            AudioManager.SetApplicationVolume(current_pid, (float)racevoice_vol);
        }
    }
}
