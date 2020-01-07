namespace RaceVoice
{
    partial class Terminal
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
            this.send = new System.Windows.Forms.Button();
            this.cmd = new System.Windows.Forms.TextBox();
            this.lb = new System.Windows.Forms.ListBox();
            this.openclose = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // send
            // 
            this.send.Location = new System.Drawing.Point(158, 439);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(194, 35);
            this.send.TabIndex = 0;
            this.send.Text = "Send";
            this.send.UseVisualStyleBackColor = true;
            this.send.Click += new System.EventHandler(this.send_Click);
            // 
            // cmd
            // 
            this.cmd.Location = new System.Drawing.Point(358, 445);
            this.cmd.Name = "cmd";
            this.cmd.Size = new System.Drawing.Size(673, 20);
            this.cmd.TabIndex = 1;
            this.cmd.TextChanged += new System.EventHandler(this.cmd_TextChanged);
            this.cmd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmd_KeyDown);
            // 
            // lb
            // 
            this.lb.FormattingEnabled = true;
            this.lb.Location = new System.Drawing.Point(42, 13);
            this.lb.Name = "lb";
            this.lb.ScrollAlwaysVisible = true;
            this.lb.Size = new System.Drawing.Size(989, 420);
            this.lb.TabIndex = 2;
            // 
            // openclose
            // 
            this.openclose.Location = new System.Drawing.Point(42, 439);
            this.openclose.Name = "openclose";
            this.openclose.Size = new System.Drawing.Size(110, 35);
            this.openclose.TabIndex = 3;
            this.openclose.Text = "Open";
            this.openclose.UseVisualStyleBackColor = true;
            this.openclose.Click += new System.EventHandler(this.openclose_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Terminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1042, 509);
            this.Controls.Add(this.openclose);
            this.Controls.Add(this.lb);
            this.Controls.Add(this.cmd);
            this.Controls.Add(this.send);
            this.Name = "Terminal";
            this.Text = "Terminal";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Terminal_FormClosing);
            this.Load += new System.EventHandler(this.Terminal_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button send;
        private System.Windows.Forms.TextBox cmd;
        private System.Windows.Forms.ListBox lb;
        private System.Windows.Forms.Button openclose;
        private System.Windows.Forms.Timer timer1;
    }
}