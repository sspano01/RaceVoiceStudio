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
    public partial class Preferences : Form
    {
        private CarMetadata _meta;
        

         public Preferences(CarMetadata meta)
        {
            InitializeComponent();
            _meta = meta;
            ReadAtStart.Checked = _meta.HardwareData.GetConfigAtStart;
            share.Checked = _meta.HardwareData.ShareNewTracks;
        }

        private void ReadAtStart_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void save_Click(object sender, EventArgs e)
        {
            _meta.HardwareData.GetConfigAtStart = ReadAtStart.Checked;
            _meta.HardwareData.ShareNewTracks = share.Checked;
            Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
