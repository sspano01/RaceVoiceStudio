namespace RaceVoice
{
    partial class RealTimeView
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
            this.label1 = new System.Windows.Forms.Label();
            this.throttlePos = new System.Windows.Forms.TextBox();
            this.rpm = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.temperature = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.oilpsi = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gps = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.brakePSI = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.voltage = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Throttle (%)";
            // 
            // throttlePos
            // 
            this.throttlePos.Location = new System.Drawing.Point(114, 13);
            this.throttlePos.Name = "throttlePos";
            this.throttlePos.Size = new System.Drawing.Size(100, 20);
            this.throttlePos.TabIndex = 1;
            this.throttlePos.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.throttlePos_KeyPress);
            // 
            // rpm
            // 
            this.rpm.Location = new System.Drawing.Point(114, 39);
            this.rpm.Name = "rpm";
            this.rpm.Size = new System.Drawing.Size(100, 20);
            this.rpm.TabIndex = 3;
            this.rpm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rpm_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "RPM";
            // 
            // temperature
            // 
            this.temperature.Location = new System.Drawing.Point(114, 65);
            this.temperature.Name = "temperature";
            this.temperature.Size = new System.Drawing.Size(100, 20);
            this.temperature.TabIndex = 5;
            this.temperature.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.temperature_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Temp (F)";
            // 
            // oilpsi
            // 
            this.oilpsi.Location = new System.Drawing.Point(114, 91);
            this.oilpsi.Name = "oilpsi";
            this.oilpsi.Size = new System.Drawing.Size(100, 20);
            this.oilpsi.TabIndex = 7;
            this.oilpsi.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.oilpsi_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(53, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Oil PSI";
            // 
            // gps
            // 
            this.gps.Location = new System.Drawing.Point(317, 13);
            this.gps.Name = "gps";
            this.gps.Size = new System.Drawing.Size(205, 20);
            this.gps.TabIndex = 9;
            this.gps.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.latlng_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(269, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "GPS ";
            // 
            // brakePSI
            // 
            this.brakePSI.Location = new System.Drawing.Point(114, 117);
            this.brakePSI.Name = "brakePSI";
            this.brakePSI.Size = new System.Drawing.Size(100, 20);
            this.brakePSI.TabIndex = 11;
            this.brakePSI.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.brakePSI_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(53, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Brake PSI";
            // 
            // voltage
            // 
            this.voltage.Location = new System.Drawing.Point(114, 143);
            this.voltage.Name = "voltage";
            this.voltage.Size = new System.Drawing.Size(100, 20);
            this.voltage.TabIndex = 13;
            this.voltage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(53, 146);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Voltage";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // RealTimeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(556, 188);
            this.Controls.Add(this.voltage);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.brakePSI);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.gps);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.oilpsi);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.temperature);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rpm);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.throttlePos);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RealTimeView";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RealTimeView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RealTimeView_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox throttlePos;
        private System.Windows.Forms.TextBox rpm;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox temperature;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox oilpsi;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox gps;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox brakePSI;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox voltage;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Timer timer1;
    }
}