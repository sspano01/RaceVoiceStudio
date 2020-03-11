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
    public partial class SpeechTagDialogue : Form
    {
        public class SpeechTagDialogueResult
        {
            public string Name { get; set; }
            public string Speech { get; set; }
            public bool Add { get; set; }
        }

        private bool _add;

        private Action<SpeechTagDialogueResult> _callback;

        public SpeechTagDialogue(Action<SpeechTagDialogueResult> callback)
        {
            InitializeComponent();
            _callback = callback;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("You must specify a name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (string.IsNullOrEmpty(txtPhrase.Text))
            {
                MessageBox.Show("You must specify a phrase", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _add = true;
                Close();
            }
        }

        private void SpeechTagDialogue_FormClosing(object sender, FormClosingEventArgs e)
        {
            _callback?.Invoke(new SpeechTagDialogueResult()
            {
                Add = _add,
                Name = txtName.Text,
                Speech = txtPhrase.Text
            });
        }
    }
}
