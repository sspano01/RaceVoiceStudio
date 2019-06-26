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
    public partial class GpsSettings : Form
    {
        private CarMetadata _carMetadata;

        public GpsSettings(CarMetadata carMetadata)
        {
            InitializeComponent();

            _carMetadata = carMetadata;
            cmbGpsWindow.SelectedIndex = _carMetadata.HardwareData.GPSWindow;
        }

        private void GpsWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            _carMetadata.HardwareData.GPSWindow = cmbGpsWindow.SelectedIndex;
        }

        private void saveSettings_Click(object sender, EventArgs e)
        {
            bool valid = false;
            racevoicecom rvcom = new racevoicecom();
            if (!globals.IsRaceVoiceConnected())
            {
                this.Close();
            }
            if (rvcom.OpenSerial())
            {
                string txt ="SET GPS WINDOW " + _carMetadata.HardwareData.GPSWindow.ToString();
                if (rvcom.WriteSingleCmd(txt)) valid = true;
               
               
                rvcom.CloseSerial();

                if (valid)
                {
                    MessageBox.Show("GPS Settings Haves Been Updated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    MessageBox.Show("GPS Settings Could Not Be Updated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Close();
            }
        }
    }
}
