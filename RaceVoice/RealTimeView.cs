using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace RaceVoice
{
    public partial class RealTimeView : Form
    {
        private racevoicecom rvcom = new racevoicecom();
        public RealTimeView()
        {
            InitializeComponent();
            timer1.Interval = 1000;
            if (globals.fake_connection)
            {
                throttlePos.Text = "5%";
                rpm.Text = "1100";
                temperature.Text = "185 F";
                oilpsi.Text = "52 PSI";
                brakePSI.Text = "0 PSI";
                voltage.Text = "13.5";
                gps.Text = "42.341028, -76.928861";

            }
            else
            {
                if (!globals.IsRaceVoiceConnected())
                {
                    Close();
                    return;
                }
                if (OpenSerial())
                {
                    timer1.Enabled = true;
                }
                else
                {
                    Close();
                    return;
                }
            }
        }

        private bool OpenSerial()
        {
            if (!globals.IsRaceVoiceConnected())
            {
                return false;
            }
            if (!rvcom.OpenSerial())
            {

                return false;
            }
            else
            {
                return true;
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            rvcom.WriteSingleCmd("DATA TRACE");
            try
            {
                string[] fields = globals.last_rx.Split(',');
                if (fields[0].Contains("*SOM*"))
                {
                    throttlePos.Text = fields[1].ToString() + " %";
                    rpm.Text = fields[2].ToString();
                    temperature.Text = fields[3].ToString() + " F";
                    oilpsi.Text = fields[4].ToString() + " PSI";
                    brakePSI.Text = fields[5].ToString() + " PSI";
                    voltage.Text = fields[6].ToString();
                    gps.Text = fields[7].ToString() + "," + fields[8].ToString();
                    lastspeechcode.Text = "0x"+Convert.ToInt32(fields[11]).ToString("X");
                }
            }
            catch (Exception ee)
            {
                gps.Text = "Data Error";

            }
            //globals.WriteLine(fields);
            timer1.Enabled = true;
        }

        private void RealTimeView_FormClosing(object sender, FormClosingEventArgs e)
        {
            // e.Cancel = true;
            timer1.Enabled = false;
            rvcom.CloseSerial();
        }

        // prevent user entry
        private void throttlePos_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void rpm_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void temperature_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void oilpsi_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void brakePSI_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void latlng_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}