using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace RaceVoice
{
    public partial class VoiceAdjust : Form
    {
        private CarMetadata _meta;

        public VoiceAdjust(CarMetadata meta)
        {
            InitializeComponent();
            _meta = meta;

            volumeBox.Items.Clear();
            volumeBox.Items.Add("1 (Low)");
            volumeBox.Items.Add("2");
            volumeBox.Items.Add("3");
            volumeBox.Items.Add("4");
            volumeBox.Items.Add("5 (Default)");
            volumeBox.Items.Add("6");
            volumeBox.Items.Add("7");
            volumeBox.Items.Add("8");
            volumeBox.Items.Add("9");
            volumeBox.Items.Add("10 (Max)");
            volumeBox.SelectedIndex = _meta.HardwareData.Volume;

            // fix me
            sldPitch.Value = globals.RangeFix(_meta.HardwareData.Pitch, sldPitch.Minimum, sldPitch.Maximum);
        }

        private void volumeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _meta.HardwareData.Volume = volumeBox.SelectedIndex;
        }

       

        private void SaveVoice()
        {

            bool valid = true;
            racevoicecom rvcom = new racevoicecom();
            if (!globals.IsRaceVoiceConnected())
            {
                return;
            }
            if (rvcom.OpenSerial())
            {
                string txt = "SET VOICE VOLUME " + volumeBox.SelectedIndex.ToString();
                if (!rvcom.WriteSingleCmd(txt))
                {
                    MessageBox.Show("Voice Volume Could Not Be Updated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    valid = false;
                }
                else
                {
                    _meta.HardwareData.Volume = volumeBox.SelectedIndex;
                }
                txt = "SET VOICE PITCH " + sldPitch.Value.ToString();
                if (!rvcom.WriteSingleCmd(txt))
                {
                    MessageBox.Show("Voice Pitch Could Not Be Updated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    valid = false;
                }
                else
                {
                    _meta.HardwareData.Pitch = sldPitch.Value;
                }
 
                if (valid)
                {
                    rvcom.CloseSerial();
                    MessageBox.Show("Voice Has Been Updated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {
                Close();
            }
        }
        private void sendVoice_Click(object sender, EventArgs e)
        {
            if (globals.IsDemoMode(true)) return;

            SaveVoice();
        }

        private void testVoice_Click(object sender, EventArgs e)
        {
            if (globals.IsDemoMode(true)) return;

            racevoicecom rvcom = new racevoicecom();
            if (!globals.IsRaceVoiceConnected()) this.Close();
            if (rvcom.OpenSerial())
            {
                string txt = "VERSION TALK";
                rvcom.WriteSingleCmd(txt);
                rvcom.CloseSerial();
            }
           
        }

        private void sldPitch_Scroll(object sender, EventArgs e)
        {
            _meta.HardwareData.Pitch = sldPitch.Value;
        }

        private void VoiceAdjust_Load(object sender, EventArgs e)
        {

        }

        private void setDefaults_Click(object sender, EventArgs e)
        {

            _meta.HardwareData.Pitch=sldPitch.Value = 60;
            _meta.HardwareData.Volume = volumeBox.SelectedIndex = 5;
            if (globals.IsDemoMode(true)) return;
            SaveVoice();
        }
    }
}