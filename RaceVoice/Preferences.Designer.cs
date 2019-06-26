namespace RaceVoice
{
    partial class Preferences
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ReadAtStart = new System.Windows.Forms.CheckBox();
            this.save = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.share = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ReadAtStart
            // 
            this.ReadAtStart.AutoSize = true;
            this.ReadAtStart.Location = new System.Drawing.Point(12, 28);
            this.ReadAtStart.Name = "ReadAtStart";
            this.ReadAtStart.Size = new System.Drawing.Size(347, 17);
            this.ReadAtStart.TabIndex = 0;
            this.ReadAtStart.Text = "Download all RaceVoice settings when RaceVoice Studio first starts";
            this.ReadAtStart.UseVisualStyleBackColor = true;
            this.ReadAtStart.CheckedChanged += new System.EventHandler(this.ReadAtStart_CheckedChanged);
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(9, 114);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(260, 45);
            this.save.TabIndex = 4;
            this.save.Text = "Save Settings";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(294, 114);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(260, 45);
            this.cancel.TabIndex = 5;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // share
            // 
            this.share.AutoSize = true;
            this.share.Location = new System.Drawing.Point(12, 58);
            this.share.Name = "share";
            this.share.Size = new System.Drawing.Size(337, 17);
            this.share.TabIndex = 6;
            this.share.Text = "Allow newly imported tracks to be uploaded to RaceVoice\'s server";
            this.share.UseVisualStyleBackColor = true;
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(574, 184);
            this.Controls.Add(this.share);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.save);
            this.Controls.Add(this.ReadAtStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Preferences";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ReadAtStart;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.CheckBox share;
    }
}