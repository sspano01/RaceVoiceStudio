using System;
using System.Windows.Forms;

namespace RaceVoice
{
    public partial class licenseemail : Form
    {
        public string user_email = "";
        public string user_name = "";

        public licenseemail()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            user_name = username.Text.Trim();
            user_email = emailaddr.Text.Trim();
            this.Close();
        }
    }
}