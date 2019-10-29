using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EmailValidation;


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

        private bool IsName(string name)
        {
          
            if (name.Length>3)
            {
                string[] name_parts = name.Split(' ');
                if (name_parts.Length >= 2) return true;

            }

            return false;

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

            if (!EmailValidator.Validate(user_email))
            {
                MessageBox.Show("Email address format is incorrect", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            this.Close();
        }
    }
}