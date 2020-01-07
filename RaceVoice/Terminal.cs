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
    public partial class Terminal : Form
    {
        racevoicecom com = new racevoicecom();
        string inline = "";
        public Terminal()
        {

            InitializeComponent();
        }

        private void Terminal_Load(object sender, EventArgs e)
        {
            send.Enabled = false;
            openclose.Text = "OPEN";
        }

        private void openclose_Click(object sender, EventArgs e)
        {
            if (openclose.Text.ToUpper().Contains("OPEN"))
            {
                if (com.OpenSerial())
                {
                    timer1.Interval = 10;
                    timer1.Enabled = true;
                    openclose.Text = "CLOSE";
                    send.Enabled = true;
                }
            }
            else
            {
                timer1.Enabled = false;
                com.CloseSerial();
                send.Enabled = false;
                openclose.Text = "OPEN";

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool read = true;
           while (read)
            {
                Application.DoEvents();
                char bt = com.GetChar();
                if (bt == 0) break;

                if (bt == '\r')
                {
                    lb.Items.Add(inline);
                    inline = "";
                }
                else inline += bt;
            }
        }

        private void SendIt()
        {
            string os = cmd.Text;
            os = os.TrimEnd('\r', '\n');
            os += "\r"; // make sure we only have 1 CR
            com.WriteSingleCmd(os, false);

        }
        private void send_Click(object sender, EventArgs e)
        {
            SendIt();
        }

        private void cmd_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmd_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                SendIt();
            }
        }

        private void Terminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            com.CloseSerial();
        }
    }
}
