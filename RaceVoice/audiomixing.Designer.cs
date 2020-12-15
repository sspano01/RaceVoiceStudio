namespace RaceVoice
{
    partial class audiomixing
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(audiomixing));
            this.iracing_trackbar = new System.Windows.Forms.TrackBar();
            this.RaceVoice_trackbar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.iracing_trackbar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RaceVoice_trackbar)).BeginInit();
            this.SuspendLayout();
            // 
            // iracing_trackbar
            // 
            this.iracing_trackbar.LargeChange = 10;
            this.iracing_trackbar.Location = new System.Drawing.Point(62, 29);
            this.iracing_trackbar.Maximum = 100;
            this.iracing_trackbar.Name = "iracing_trackbar";
            this.iracing_trackbar.Size = new System.Drawing.Size(523, 45);
            this.iracing_trackbar.TabIndex = 0;
            this.iracing_trackbar.Scroll += new System.EventHandler(this.iracing_trackbar_Scroll);
            // 
            // RaceVoice_trackbar
            // 
            this.RaceVoice_trackbar.LargeChange = 10;
            this.RaceVoice_trackbar.Location = new System.Drawing.Point(62, 119);
            this.RaceVoice_trackbar.Maximum = 100;
            this.RaceVoice_trackbar.Name = "RaceVoice_trackbar";
            this.RaceVoice_trackbar.Size = new System.Drawing.Size(523, 45);
            this.RaceVoice_trackbar.TabIndex = 1;
            this.RaceVoice_trackbar.Scroll += new System.EventHandler(this.RaceVoice_trackbar_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(73, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "iRacing Master Volume";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(73, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "RaceVoiceSIM Master Volume";
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(62, 179);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(96, 43);
            this.close.TabIndex = 4;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // audiomixing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(630, 234);
            this.Controls.Add(this.close);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RaceVoice_trackbar);
            this.Controls.Add(this.iracing_trackbar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "audiomixing";
            this.Text = "RaceVoiceSIM Master Volume Adjustment";
            ((System.ComponentModel.ISupportInitialize)(this.iracing_trackbar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RaceVoice_trackbar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar iracing_trackbar;
        private System.Windows.Forms.TrackBar RaceVoice_trackbar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button close;
    }
}