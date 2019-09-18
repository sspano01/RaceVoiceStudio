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
    public partial class CanBusSettings : Form
    {
        private CarMetadata _metadata;
        private string _metafile;

        public CanBusSettings(string metafile, CarMetadata metadata)
        {
            InitializeComponent();

            _metadata = metadata;
            _metafile = metafile;
        }

        private void cmbBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            _metadata.HardwareData.BaudRate = cmbBaudRate.SelectedIndex;
            _metadata.Save(_metafile);
        }

        private void CanBusSettings_Load(object sender, EventArgs e)
        {
            cmbBaudRate.SelectedIndex = _metadata.HardwareData.BaudRate;
        }

        private void saveSettings_Click(object sender, EventArgs e)
        {
            bool valid = false;
            if (globals.IsDemoMode(true)) return;
            racevoicecom rvcom = new racevoicecom();
            if (!globals.IsRaceVoiceConnected())
            {
                return;
            }

            string baud_string = "250";
            if (_metadata.HardwareData.BaudRate == 0) baud_string = "125";
            if (_metadata.HardwareData.BaudRate == 1) baud_string = "250";
            if (_metadata.HardwareData.BaudRate == 2) baud_string = "500";
            if (_metadata.HardwareData.BaudRate == 3) baud_string = "1000";

            if (rvcom.OpenSerial())
            {
                if (rvcom.WriteSingleCmd("SET BAUD RATE " + baud_string)) valid = true;
                rvcom.CloseSerial();
            }
            if (valid)
            {
                MessageBox.Show("CANBus Settings Haves Been Updated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("CANBus Settings Could Not Be Updated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
