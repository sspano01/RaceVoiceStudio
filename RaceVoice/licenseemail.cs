using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
            user_name = "end";
            globals.Terminate();

        }

        private bool IsEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            // MUST CONTAIN ONE AND ONLY ONE @
            int i;
            int atCount = 0;
            for (i=0;i<input.Length;i++)
            {
                if (input[i] == '@') atCount++;
            }
            if (atCount != 1) return false;

            // MUST CONTAIN PERIOD
            if (!input.Contains(".")) return false;

            // @ MUST OCCUR BEFORE LAST PERIOD
            var indexOfAt = input.IndexOf("@", StringComparison.Ordinal);
            var lastIndexOfPeriod = input.LastIndexOf(".", StringComparison.Ordinal);
            var atBeforeLastPeriod = lastIndexOfPeriod > indexOfAt;
            if (!atBeforeLastPeriod) return false;

            // CODE FROM COGWHEEL'S ANSWER: https://stackoverflow.com/a/1374644/388267 
            try
            {
                var addr = new System.Net.Mail.MailAddress(input);
                return addr.Address == input;
            }
            catch
            {
                return false;
            }
        }

        private bool IsName(string name)
        {

            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            user_name = username.Text.Trim();
            user_email = emailaddr.Text.Trim();
            
            /*
            user_name = "steve spano";
            user_email = "s@s.com";
            */
            if (!IsName(user_name))
            {
                MessageBox.Show("Your name is not in the correct format", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            if (!IsEmail(user_email))
            {
                MessageBox.Show("Email address format is incorrect", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            this.Close();
        }
    }
}