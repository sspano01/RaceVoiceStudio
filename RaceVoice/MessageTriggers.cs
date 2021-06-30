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
    public partial class MessageTriggers : Form
    {
        private CarMetadata _metadata;
        private string _metafile;

        public MessageTriggers(string metafile, CarMetadata metadata)
        {
            InitializeComponent();
            _metadata = metadata;
            _metafile = metafile;


            for (int i = 0; i < metadata.MessageTriggers.Count; i++)
            {
                TextBox phrase = (TextBox)Controls["txtPhrase" + i];
                ComboBox repeat = (ComboBox)Controls["cmbRepeat" + i];

                if (phrase != null)
                {
                    phrase.Text = metadata.MessageTriggers[i].Phrase;
                }

                if (repeat != null)
                {
                    repeat.SelectedIndex = metadata.MessageTriggers[i].Repeat;

                    if (metadata.MessageTriggers[i].Repeat==0)
                    {
                        if (phrase != null) phrase.Enabled = false; else phrase.Enabled = true;

                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            MessageTrigger[] triggers = new MessageTrigger[8];
            _metadata.MessageTriggers = triggers;
            for (int i = 0; i < 8; i++)
            {
                TextBox phrase = (TextBox)Controls["txtPhrase" + i];
                ComboBox repeat = (ComboBox)Controls["cmbRepeat" + i];
                triggers[i] = new MessageTrigger()
                {
                    Phrase = phrase.Text,
                    Repeat = repeat.SelectedIndex
                };
            }

            _metadata.Save(_metafile);
            Close();
        }

        private void cmbRepeat0_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                ComboBox repeat = (ComboBox)Controls["cmbRepeat" + i];
                TextBox phrase = (TextBox)Controls["txtPhrase" + i];
                if (repeat.SelectedIndex==0)
                {
                    phrase.Enabled = false;  
                }
                else
                {
                    phrase.Enabled = true;
                }
            }
        }
    }
}
