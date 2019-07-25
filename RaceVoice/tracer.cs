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
    public partial class tracer : Form
    {
        public tracer()
        {
            InitializeComponent();
        }

        public void addline(string line)
        {
            box.Items.Add(line);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s1 = "";
            foreach (object item in box.Items) s1 += item.ToString() + "\r\n";
            Clipboard.SetText(s1);
        }
    }
}
