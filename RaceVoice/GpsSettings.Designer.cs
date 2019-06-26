namespace RaceVoice
{
    partial class GpsSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbGpsWindow = new System.Windows.Forms.ComboBox();
            this.saveSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "GPS Window";
            // 
            // cmbGpsWindow
            // 
            this.cmbGpsWindow.FormattingEnabled = true;
            this.cmbGpsWindow.Items.AddRange(new object[] {
            "30 feet",
            "60 feet",
            "90 feet",
            "120 feet"});
            this.cmbGpsWindow.Location = new System.Drawing.Point(89, 12);
            this.cmbGpsWindow.Name = "cmbGpsWindow";
            this.cmbGpsWindow.Size = new System.Drawing.Size(201, 21);
            this.cmbGpsWindow.TabIndex = 1;
            this.cmbGpsWindow.SelectedIndexChanged += new System.EventHandler(this.GpsWindow_SelectedIndexChanged);
            // 
            // saveSettings
            // 
            this.saveSettings.Location = new System.Drawing.Point(30, 62);
            this.saveSettings.Name = "saveSettings";
            this.saveSettings.Size = new System.Drawing.Size(260, 45);
            this.saveSettings.TabIndex = 2;
            this.saveSettings.Text = "Save Settings";
            this.saveSettings.UseVisualStyleBackColor = true;
            this.saveSettings.Click += new System.EventHandler(this.saveSettings_Click);
            // 
            // GpsSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(318, 136);
            this.Controls.Add(this.saveSettings);
            this.Controls.Add(this.cmbGpsWindow);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GpsSettings";
            this.ShowIcon = false;
            this.Text = "GPS Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbGpsWindow;
        private System.Windows.Forms.Button saveSettings;
    }
}