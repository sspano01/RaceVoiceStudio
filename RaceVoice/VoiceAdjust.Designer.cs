namespace RaceVoice
{
    partial class VoiceAdjust
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
            this.components = new System.ComponentModel.Container();
            this.sendVoice = new System.Windows.Forms.Button();
            this.testVoice = new System.Windows.Forms.Button();
            this.volumeBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.sldPitch = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.setDefaults = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.sldPitch)).BeginInit();
            this.SuspendLayout();
            // 
            // sendVoice
            // 
            this.sendVoice.Location = new System.Drawing.Point(12, 141);
            this.sendVoice.Name = "sendVoice";
            this.sendVoice.Size = new System.Drawing.Size(260, 45);
            this.sendVoice.TabIndex = 1;
            this.sendVoice.Text = "Save Settings";
            this.sendVoice.UseVisualStyleBackColor = true;
            this.sendVoice.Click += new System.EventHandler(this.sendVoice_Click);
            // 
            // testVoice
            // 
            this.testVoice.Location = new System.Drawing.Point(12, 90);
            this.testVoice.Name = "testVoice";
            this.testVoice.Size = new System.Drawing.Size(260, 45);
            this.testVoice.TabIndex = 2;
            this.testVoice.Text = "Test Voice";
            this.testVoice.UseVisualStyleBackColor = true;
            this.testVoice.Click += new System.EventHandler(this.testVoice_Click);
            // 
            // volumeBox
            // 
            this.volumeBox.FormattingEnabled = true;
            this.volumeBox.Location = new System.Drawing.Point(60, 12);
            this.volumeBox.Name = "volumeBox";
            this.volumeBox.Size = new System.Drawing.Size(212, 21);
            this.volumeBox.TabIndex = 3;
            this.volumeBox.SelectedIndexChanged += new System.EventHandler(this.volumeBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Volume";
            // 
            // sldPitch
            // 
            this.sldPitch.LargeChange = 10;
            this.sldPitch.Location = new System.Drawing.Point(60, 39);
            this.sldPitch.Maximum = 100;
            this.sldPitch.Name = "sldPitch";
            this.sldPitch.Size = new System.Drawing.Size(212, 45);
            this.sldPitch.TabIndex = 5;
            this.sldPitch.TickFrequency = 5;
            this.sldPitch.Value = 50;
            this.sldPitch.Scroll += new System.EventHandler(this.sldPitch_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Pitch";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // setDefaults
            // 
            this.setDefaults.Location = new System.Drawing.Point(12, 192);
            this.setDefaults.Name = "setDefaults";
            this.setDefaults.Size = new System.Drawing.Size(260, 45);
            this.setDefaults.TabIndex = 7;
            this.setDefaults.Text = "Restore Defaults";
            this.setDefaults.UseVisualStyleBackColor = true;
            this.setDefaults.Click += new System.EventHandler(this.setDefaults_Click);
            // 
            // VoiceAdjust
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(284, 252);
            this.Controls.Add(this.setDefaults);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.volumeBox);
            this.Controls.Add(this.testVoice);
            this.Controls.Add(this.sendVoice);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sldPitch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VoiceAdjust";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Voice Settings";
            this.Load += new System.EventHandler(this.VoiceAdjust_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sldPitch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendVoice;
        private System.Windows.Forms.Button testVoice;
        private System.Windows.Forms.ComboBox volumeBox;
        private System.Windows.Forms.Label label1;
        private System.IO.Ports.SerialPort _serialPort;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TrackBar sldPitch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button setDefaults;
    }
}