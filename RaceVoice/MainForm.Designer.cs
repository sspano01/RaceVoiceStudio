namespace RaceVoice
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.rendererRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addSegmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedSegmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSplitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSpeechTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitsBox = new System.Windows.Forms.GroupBox();
            this.btnHideAllSplits = new System.Windows.Forms.Button();
            this.lstSplits = new System.Windows.Forms.CheckedListBox();
            this.btnSplitDelete = new System.Windows.Forms.Button();
            this.segmentGroupBox = new System.Windows.Forms.GroupBox();
            this.lstSegments = new System.Windows.Forms.ListBox();
            this.btnSegmentDelete = new System.Windows.Forms.Button();
            this.grpData = new System.Windows.Forms.GroupBox();
            this.chKsegmentRollingMph = new System.Windows.Forms.CheckBox();
            this.btnClearAllData = new System.Windows.Forms.Button();
            this.chkLateralG = new System.Windows.Forms.CheckBox();
            this.chkMaxLinearG = new System.Windows.Forms.CheckBox();
            this.chkMaxLateralG = new System.Windows.Forms.CheckBox();
            this.chkTurnInSpeed = new System.Windows.Forms.CheckBox();
            this.chkExitSpeed = new System.Windows.Forms.CheckBox();
            this.chkEntrySpeed = new System.Windows.Forms.CheckBox();
            this.chkMinimumSpeed = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.updatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTrackMapsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cloneCurrentTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.restoreAllTracksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreCurrentTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chartsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMoTecCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.voiceSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataTraceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gPSSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.messageTriggersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.baudRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.firmwareUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.installUSBDriversToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.terminalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoRaceVoiceComToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.releaseNotesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmbTracks = new System.Windows.Forms.ComboBox();
            this.sendConfig = new System.Windows.Forms.Button();
            this.getConfig = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabEngine = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.rdoToneNotify = new System.Windows.Forms.RadioButton();
            this.rdoSpeechNotify = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numOilPressureRpm = new System.Windows.Forms.NumericUpDown();
            this.numDownShift = new System.Windows.Forms.NumericUpDown();
            this.numVoltage = new System.Windows.Forms.NumericUpDown();
            this.numTemperature = new System.Windows.Forms.NumericUpDown();
            this.numOilPressurePsi = new System.Windows.Forms.NumericUpDown();
            this.numUpShift = new System.Windows.Forms.NumericUpDown();
            this.numOverRev = new System.Windows.Forms.NumericUpDown();
            this.chkVoltage = new System.Windows.Forms.CheckBox();
            this.chkTemperature = new System.Windows.Forms.CheckBox();
            this.chkOilPressure = new System.Windows.Forms.CheckBox();
            this.chkDownShift = new System.Windows.Forms.CheckBox();
            this.chkUpShift = new System.Windows.Forms.CheckBox();
            this.chkOverRev = new System.Windows.Forms.CheckBox();
            this.cmbEcuType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabDynamics = new System.Windows.Forms.TabPage();
            this.braketonebox = new System.Windows.Forms.GroupBox();
            this.numMaxBrakeThreshold = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numMinBrakeThreshold = new System.Windows.Forms.NumericUpDown();
            this.chkBrakeThreshold = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.chkAnnounceLapDelta = new System.Windows.Forms.CheckBox();
            this.chkAnnounceBestLap = new System.Windows.Forms.CheckBox();
            this.numAnnounceSpeed = new System.Windows.Forms.NumericUpDown();
            this.chkAnnounceSpeed = new System.Windows.Forms.CheckBox();
            this.wheelLockbox = new System.Windows.Forms.GroupBox();
            this.numWheelSpeedDiff = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numBrakePSI = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.chkActiveWheelLockDetection = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numLinearGForce = new System.Windows.Forms.NumericUpDown();
            this.numLateralGForce = new System.Windows.Forms.NumericUpDown();
            this.chkLinearGForce = new System.Windows.Forms.CheckBox();
            this.chkLateralGForce = new System.Windows.Forms.CheckBox();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabTrack = new System.Windows.Forms.TabPage();
            this.speechTagBox = new System.Windows.Forms.GroupBox();
            this.btnHideAllSpeechTags = new System.Windows.Forms.Button();
            this.lstSpeechTags = new System.Windows.Forms.CheckedListBox();
            this.btnSpeechTagDelete = new System.Windows.Forms.Button();
            this.btnSaveTrack = new System.Windows.Forms.Button();
            this.TrackView = new System.Windows.Forms.PictureBox();
            this.zoom = new System.Windows.Forms.TrackBar();
            this.rotation = new System.Windows.Forms.TrackBar();
            this.vScroll = new System.Windows.Forms.TrackBar();
            this.hScroll = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabCharts = new System.Windows.Forms.TabPage();
            this.webCharts = new System.Windows.Forms.WebBrowser();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.webTables = new System.Windows.Forms.WebBrowser();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.DownloadData = new System.Windows.Forms.Button();
            this.rendererRightClickMenu.SuspendLayout();
            this.splitsBox.SuspendLayout();
            this.segmentGroupBox.SuspendLayout();
            this.grpData.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabEngine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOilPressureRpm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDownShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVoltage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTemperature)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOilPressurePsi)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpShift)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOverRev)).BeginInit();
            this.tabDynamics.SuspendLayout();
            this.braketonebox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxBrakeThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinBrakeThreshold)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAnnounceSpeed)).BeginInit();
            this.wheelLockbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWheelSpeedDiff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBrakePSI)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLinearGForce)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLateralGForce)).BeginInit();
            this.tabMain.SuspendLayout();
            this.tabTrack.SuspendLayout();
            this.speechTagBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vScroll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hScroll)).BeginInit();
            this.tabCharts.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rendererRightClickMenu
            // 
            this.rendererRightClickMenu.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.rendererRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSegmentToolStripMenuItem,
            this.deleteSelectedSegmentToolStripMenuItem,
            this.addSplitToolStripMenuItem,
            this.addSpeechTagToolStripMenuItem});
            this.rendererRightClickMenu.Name = "rendererRightClickMenu";
            this.rendererRightClickMenu.Size = new System.Drawing.Size(205, 92);
            this.rendererRightClickMenu.Opening += new System.ComponentModel.CancelEventHandler(this.rendererRightClickMenu_Opening);
            // 
            // addSegmentToolStripMenuItem
            // 
            this.addSegmentToolStripMenuItem.Name = "addSegmentToolStripMenuItem";
            this.addSegmentToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.addSegmentToolStripMenuItem.Text = "Add Segment";
            this.addSegmentToolStripMenuItem.Click += new System.EventHandler(this.addSegmentToolStripMenuItem_Click);
            // 
            // deleteSelectedSegmentToolStripMenuItem
            // 
            this.deleteSelectedSegmentToolStripMenuItem.Name = "deleteSelectedSegmentToolStripMenuItem";
            this.deleteSelectedSegmentToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.deleteSelectedSegmentToolStripMenuItem.Text = "Delete Selected Segment";
            this.deleteSelectedSegmentToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedSegmentToolStripMenuItem_Click);
            // 
            // addSplitToolStripMenuItem
            // 
            this.addSplitToolStripMenuItem.Name = "addSplitToolStripMenuItem";
            this.addSplitToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.addSplitToolStripMenuItem.Text = "Add Split";
            this.addSplitToolStripMenuItem.Click += new System.EventHandler(this.addSplitToolStripMenuItem_Click);
            // 
            // addSpeechTagToolStripMenuItem
            // 
            this.addSpeechTagToolStripMenuItem.Name = "addSpeechTagToolStripMenuItem";
            this.addSpeechTagToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.addSpeechTagToolStripMenuItem.Text = "Add Speech Tag";
            this.addSpeechTagToolStripMenuItem.Visible = false;
            this.addSpeechTagToolStripMenuItem.Click += new System.EventHandler(this.addSpeechTagMenuItem_Click);
            // 
            // splitsBox
            // 
            this.splitsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.splitsBox.Controls.Add(this.btnHideAllSplits);
            this.splitsBox.Controls.Add(this.lstSplits);
            this.splitsBox.Controls.Add(this.btnSplitDelete);
            this.splitsBox.Location = new System.Drawing.Point(820, 2);
            this.splitsBox.Margin = new System.Windows.Forms.Padding(1);
            this.splitsBox.Name = "splitsBox";
            this.splitsBox.Padding = new System.Windows.Forms.Padding(1);
            this.splitsBox.Size = new System.Drawing.Size(183, 174);
            this.splitsBox.TabIndex = 7;
            this.splitsBox.TabStop = false;
            this.splitsBox.Text = "Splits";
            // 
            // btnHideAllSplits
            // 
            this.btnHideAllSplits.Location = new System.Drawing.Point(8, 115);
            this.btnHideAllSplits.Margin = new System.Windows.Forms.Padding(1);
            this.btnHideAllSplits.Name = "btnHideAllSplits";
            this.btnHideAllSplits.Size = new System.Drawing.Size(167, 23);
            this.btnHideAllSplits.TabIndex = 3;
            this.btnHideAllSplits.Text = "Clear All";
            this.btnHideAllSplits.UseVisualStyleBackColor = true;
            this.btnHideAllSplits.Click += new System.EventHandler(this.btnHideAllSplits_Click);
            // 
            // lstSplits
            // 
            this.lstSplits.FormattingEnabled = true;
            this.lstSplits.Location = new System.Drawing.Point(9, 17);
            this.lstSplits.Name = "lstSplits";
            this.lstSplits.Size = new System.Drawing.Size(167, 94);
            this.lstSplits.TabIndex = 2;
            this.lstSplits.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstSplits_ItemCheck);
            // 
            // btnSplitDelete
            // 
            this.btnSplitDelete.Location = new System.Drawing.Point(8, 140);
            this.btnSplitDelete.Margin = new System.Windows.Forms.Padding(1);
            this.btnSplitDelete.Name = "btnSplitDelete";
            this.btnSplitDelete.Size = new System.Drawing.Size(167, 23);
            this.btnSplitDelete.TabIndex = 1;
            this.btnSplitDelete.Text = "Delete";
            this.btnSplitDelete.UseVisualStyleBackColor = true;
            this.btnSplitDelete.Click += new System.EventHandler(this.btnSplitDelete_Click);
            // 
            // segmentGroupBox
            // 
            this.segmentGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.segmentGroupBox.Controls.Add(this.lstSegments);
            this.segmentGroupBox.Controls.Add(this.btnSegmentDelete);
            this.segmentGroupBox.Location = new System.Drawing.Point(820, 178);
            this.segmentGroupBox.Margin = new System.Windows.Forms.Padding(1);
            this.segmentGroupBox.Name = "segmentGroupBox";
            this.segmentGroupBox.Padding = new System.Windows.Forms.Padding(1);
            this.segmentGroupBox.Size = new System.Drawing.Size(183, 150);
            this.segmentGroupBox.TabIndex = 8;
            this.segmentGroupBox.TabStop = false;
            this.segmentGroupBox.Text = "Segments";
            // 
            // lstSegments
            // 
            this.lstSegments.FormattingEnabled = true;
            this.lstSegments.Location = new System.Drawing.Point(9, 16);
            this.lstSegments.Name = "lstSegments";
            this.lstSegments.Size = new System.Drawing.Size(166, 95);
            this.lstSegments.TabIndex = 4;
            this.lstSegments.SelectedIndexChanged += new System.EventHandler(this.lstSegments_SelectedIndexChanged);
            // 
            // btnSegmentDelete
            // 
            this.btnSegmentDelete.Location = new System.Drawing.Point(8, 115);
            this.btnSegmentDelete.Margin = new System.Windows.Forms.Padding(1);
            this.btnSegmentDelete.Name = "btnSegmentDelete";
            this.btnSegmentDelete.Size = new System.Drawing.Size(168, 23);
            this.btnSegmentDelete.TabIndex = 1;
            this.btnSegmentDelete.Text = "Delete";
            this.btnSegmentDelete.UseVisualStyleBackColor = true;
            this.btnSegmentDelete.Click += new System.EventHandler(this.btnSegmentDelete_Click);
            // 
            // grpData
            // 
            this.grpData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpData.Controls.Add(this.chKsegmentRollingMph);
            this.grpData.Controls.Add(this.btnClearAllData);
            this.grpData.Controls.Add(this.chkLateralG);
            this.grpData.Controls.Add(this.chkMaxLinearG);
            this.grpData.Controls.Add(this.chkMaxLateralG);
            this.grpData.Controls.Add(this.chkTurnInSpeed);
            this.grpData.Controls.Add(this.chkExitSpeed);
            this.grpData.Controls.Add(this.chkEntrySpeed);
            this.grpData.Controls.Add(this.chkMinimumSpeed);
            this.grpData.Location = new System.Drawing.Point(820, 330);
            this.grpData.Margin = new System.Windows.Forms.Padding(1);
            this.grpData.Name = "grpData";
            this.grpData.Padding = new System.Windows.Forms.Padding(1);
            this.grpData.Size = new System.Drawing.Size(183, 204);
            this.grpData.TabIndex = 9;
            this.grpData.TabStop = false;
            this.grpData.Text = "Data";
            // 
            // chKsegmentRollingMph
            // 
            this.chKsegmentRollingMph.AutoSize = true;
            this.chKsegmentRollingMph.Location = new System.Drawing.Point(9, 139);
            this.chKsegmentRollingMph.Margin = new System.Windows.Forms.Padding(1);
            this.chKsegmentRollingMph.Name = "chKsegmentRollingMph";
            this.chKsegmentRollingMph.Size = new System.Drawing.Size(130, 17);
            this.chKsegmentRollingMph.TabIndex = 9;
            this.chKsegmentRollingMph.Text = "Segment Rolling MPH";
            this.chKsegmentRollingMph.UseVisualStyleBackColor = true;
            this.chKsegmentRollingMph.CheckedChanged += new System.EventHandler(this.DataCheckbox_CheckedChanged);
            // 
            // btnClearAllData
            // 
            this.btnClearAllData.Location = new System.Drawing.Point(7, 170);
            this.btnClearAllData.Margin = new System.Windows.Forms.Padding(1);
            this.btnClearAllData.Name = "btnClearAllData";
            this.btnClearAllData.Size = new System.Drawing.Size(168, 23);
            this.btnClearAllData.TabIndex = 8;
            this.btnClearAllData.Text = "Clear All";
            this.btnClearAllData.UseVisualStyleBackColor = true;
            this.btnClearAllData.Click += new System.EventHandler(this.btnClearAllData_Click);
            // 
            // chkLateralG
            // 
            this.chkLateralG.AutoSize = true;
            this.chkLateralG.Location = new System.Drawing.Point(9, 120);
            this.chkLateralG.Margin = new System.Windows.Forms.Padding(1);
            this.chkLateralG.Name = "chkLateralG";
            this.chkLateralG.Size = new System.Drawing.Size(69, 17);
            this.chkLateralG.TabIndex = 6;
            this.chkLateralG.Text = "Lateral-G";
            this.chkLateralG.UseVisualStyleBackColor = true;
            this.chkLateralG.CheckedChanged += new System.EventHandler(this.DataCheckbox_CheckedChanged);
            // 
            // chkMaxLinearG
            // 
            this.chkMaxLinearG.AutoSize = true;
            this.chkMaxLinearG.Location = new System.Drawing.Point(9, 103);
            this.chkMaxLinearG.Margin = new System.Windows.Forms.Padding(1);
            this.chkMaxLinearG.Name = "chkMaxLinearG";
            this.chkMaxLinearG.Size = new System.Drawing.Size(89, 17);
            this.chkMaxLinearG.TabIndex = 5;
            this.chkMaxLinearG.Text = "Max Linear-G";
            this.chkMaxLinearG.UseVisualStyleBackColor = true;
            this.chkMaxLinearG.CheckedChanged += new System.EventHandler(this.DataCheckbox_CheckedChanged);
            // 
            // chkMaxLateralG
            // 
            this.chkMaxLateralG.AutoSize = true;
            this.chkMaxLateralG.Location = new System.Drawing.Point(9, 85);
            this.chkMaxLateralG.Margin = new System.Windows.Forms.Padding(1);
            this.chkMaxLateralG.Name = "chkMaxLateralG";
            this.chkMaxLateralG.Size = new System.Drawing.Size(92, 17);
            this.chkMaxLateralG.TabIndex = 4;
            this.chkMaxLateralG.Text = "Max Lateral-G";
            this.chkMaxLateralG.UseVisualStyleBackColor = true;
            this.chkMaxLateralG.CheckedChanged += new System.EventHandler(this.DataCheckbox_CheckedChanged);
            // 
            // chkTurnInSpeed
            // 
            this.chkTurnInSpeed.AutoSize = true;
            this.chkTurnInSpeed.Location = new System.Drawing.Point(9, 68);
            this.chkTurnInSpeed.Margin = new System.Windows.Forms.Padding(1);
            this.chkTurnInSpeed.Name = "chkTurnInSpeed";
            this.chkTurnInSpeed.Size = new System.Drawing.Size(93, 17);
            this.chkTurnInSpeed.TabIndex = 3;
            this.chkTurnInSpeed.Text = "Turn-in Speed";
            this.chkTurnInSpeed.UseVisualStyleBackColor = true;
            this.chkTurnInSpeed.CheckedChanged += new System.EventHandler(this.DataCheckbox_CheckedChanged);
            // 
            // chkExitSpeed
            // 
            this.chkExitSpeed.AutoSize = true;
            this.chkExitSpeed.Location = new System.Drawing.Point(9, 50);
            this.chkExitSpeed.Margin = new System.Windows.Forms.Padding(1);
            this.chkExitSpeed.Name = "chkExitSpeed";
            this.chkExitSpeed.Size = new System.Drawing.Size(77, 17);
            this.chkExitSpeed.TabIndex = 2;
            this.chkExitSpeed.Text = "Exit Speed";
            this.chkExitSpeed.UseVisualStyleBackColor = true;
            this.chkExitSpeed.CheckedChanged += new System.EventHandler(this.DataCheckbox_CheckedChanged);
            // 
            // chkEntrySpeed
            // 
            this.chkEntrySpeed.AutoSize = true;
            this.chkEntrySpeed.Location = new System.Drawing.Point(9, 33);
            this.chkEntrySpeed.Margin = new System.Windows.Forms.Padding(1);
            this.chkEntrySpeed.Name = "chkEntrySpeed";
            this.chkEntrySpeed.Size = new System.Drawing.Size(84, 17);
            this.chkEntrySpeed.TabIndex = 1;
            this.chkEntrySpeed.Text = "Entry Speed";
            this.chkEntrySpeed.UseVisualStyleBackColor = true;
            this.chkEntrySpeed.CheckedChanged += new System.EventHandler(this.DataCheckbox_CheckedChanged);
            // 
            // chkMinimumSpeed
            // 
            this.chkMinimumSpeed.AutoSize = true;
            this.chkMinimumSpeed.Location = new System.Drawing.Point(9, 15);
            this.chkMinimumSpeed.Margin = new System.Windows.Forms.Padding(1);
            this.chkMinimumSpeed.Name = "chkMinimumSpeed";
            this.chkMinimumSpeed.Size = new System.Drawing.Size(101, 17);
            this.chkMinimumSpeed.TabIndex = 0;
            this.chkMinimumSpeed.Text = "Minimum Speed";
            this.chkMinimumSpeed.UseVisualStyleBackColor = true;
            this.chkMinimumSpeed.CheckedChanged += new System.EventHandler(this.DataCheckbox_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updatesToolStripMenuItem,
            this.chartsMenuItem,
            this.advancedToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(3, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(1257, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // updatesToolStripMenuItem
            // 
            this.updatesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importTrackMapsToolStripMenuItem1,
            this.toolStripSeparator1,
            this.cloneCurrentTrackToolStripMenuItem,
            this.deleteTrackToolStripMenuItem,
            this.toolStripSeparator2,
            this.restoreAllTracksToolStripMenuItem,
            this.restoreCurrentTrackToolStripMenuItem});
            this.updatesToolStripMenuItem.Name = "updatesToolStripMenuItem";
            this.updatesToolStripMenuItem.Size = new System.Drawing.Size(51, 22);
            this.updatesToolStripMenuItem.Text = "Tracks";
            // 
            // importTrackMapsToolStripMenuItem1
            // 
            this.importTrackMapsToolStripMenuItem1.Name = "importTrackMapsToolStripMenuItem1";
            this.importTrackMapsToolStripMenuItem1.Size = new System.Drawing.Size(186, 22);
            this.importTrackMapsToolStripMenuItem1.Text = "Import Track Maps";
            this.importTrackMapsToolStripMenuItem1.Click += new System.EventHandler(this.importTrackMapsToolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            // 
            // cloneCurrentTrackToolStripMenuItem
            // 
            this.cloneCurrentTrackToolStripMenuItem.Name = "cloneCurrentTrackToolStripMenuItem";
            this.cloneCurrentTrackToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.cloneCurrentTrackToolStripMenuItem.Text = "Clone Track";
            this.cloneCurrentTrackToolStripMenuItem.Click += new System.EventHandler(this.cloneCurrentTrackToolStripMenuItem_Click);
            // 
            // deleteTrackToolStripMenuItem
            // 
            this.deleteTrackToolStripMenuItem.Name = "deleteTrackToolStripMenuItem";
            this.deleteTrackToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.deleteTrackToolStripMenuItem.Text = "Delete Track";
            this.deleteTrackToolStripMenuItem.Click += new System.EventHandler(this.deleteTrackToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(183, 6);
            // 
            // restoreAllTracksToolStripMenuItem
            // 
            this.restoreAllTracksToolStripMenuItem.Name = "restoreAllTracksToolStripMenuItem";
            this.restoreAllTracksToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.restoreAllTracksToolStripMenuItem.Text = "Restore All Tracks";
            this.restoreAllTracksToolStripMenuItem.Click += new System.EventHandler(this.restoreAllTracksToolStripMenuItem_Click);
            // 
            // restoreCurrentTrackToolStripMenuItem
            // 
            this.restoreCurrentTrackToolStripMenuItem.Name = "restoreCurrentTrackToolStripMenuItem";
            this.restoreCurrentTrackToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.restoreCurrentTrackToolStripMenuItem.Text = "Restore Current Track";
            this.restoreCurrentTrackToolStripMenuItem.Click += new System.EventHandler(this.restoreCurrentTrackToolStripMenuItem_Click);
            // 
            // chartsMenuItem
            // 
            this.chartsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadMoTecCSVToolStripMenuItem});
            this.chartsMenuItem.Name = "chartsMenuItem";
            this.chartsMenuItem.Size = new System.Drawing.Size(111, 22);
            this.chartsMenuItem.Text = "Charts and Tables";
            // 
            // loadMoTecCSVToolStripMenuItem
            // 
            this.loadMoTecCSVToolStripMenuItem.Name = "loadMoTecCSVToolStripMenuItem";
            this.loadMoTecCSVToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.loadMoTecCSVToolStripMenuItem.Text = "Load CSV";
            this.loadMoTecCSVToolStripMenuItem.Click += new System.EventHandler(this.loadCSVToolStripMenuItem_Click);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.voiceSettingsToolStripMenuItem,
            this.dataTraceToolStripMenuItem,
            this.gPSSettingsToolStripMenuItem,
            this.messageTriggersToolStripMenuItem,
            this.baudRateToolStripMenuItem,
            this.preferencesToolStripMenuItem,
            this.toolStripSeparator4,
            this.firmwareUpdateToolStripMenuItem,
            this.toolStripSeparator3,
            this.installUSBDriversToolStripMenuItem,
            this.toolStripSeparator5,
            this.terminalToolStripMenuItem});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(72, 22);
            this.advancedToolStripMenuItem.Text = "Advanced";
            // 
            // voiceSettingsToolStripMenuItem
            // 
            this.voiceSettingsToolStripMenuItem.Name = "voiceSettingsToolStripMenuItem";
            this.voiceSettingsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.voiceSettingsToolStripMenuItem.Text = "Voice Settings";
            this.voiceSettingsToolStripMenuItem.Click += new System.EventHandler(this.voiceSettingsToolStripMenuItem_Click);
            // 
            // dataTraceToolStripMenuItem
            // 
            this.dataTraceToolStripMenuItem.Name = "dataTraceToolStripMenuItem";
            this.dataTraceToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.dataTraceToolStripMenuItem.Text = "Data Trace";
            this.dataTraceToolStripMenuItem.Click += new System.EventHandler(this.dataTraceToolStripMenuItem_Click);
            // 
            // gPSSettingsToolStripMenuItem
            // 
            this.gPSSettingsToolStripMenuItem.Name = "gPSSettingsToolStripMenuItem";
            this.gPSSettingsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.gPSSettingsToolStripMenuItem.Text = "GPS Settings";
            this.gPSSettingsToolStripMenuItem.Click += new System.EventHandler(this.gPSSettingsToolStripMenuItem_Click);
            // 
            // messageTriggersToolStripMenuItem
            // 
            this.messageTriggersToolStripMenuItem.Name = "messageTriggersToolStripMenuItem";
            this.messageTriggersToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.messageTriggersToolStripMenuItem.Text = "Message Triggers";
            this.messageTriggersToolStripMenuItem.Click += new System.EventHandler(this.messageTriggersToolStripMenuItem_Click);
            // 
            // baudRateToolStripMenuItem
            // 
            this.baudRateToolStripMenuItem.Name = "baudRateToolStripMenuItem";
            this.baudRateToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.baudRateToolStripMenuItem.Text = "Baud Rate";
            this.baudRateToolStripMenuItem.Click += new System.EventHandler(this.baudRateToolStripMenuItem_Click);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(165, 6);
            // 
            // firmwareUpdateToolStripMenuItem
            // 
            this.firmwareUpdateToolStripMenuItem.Name = "firmwareUpdateToolStripMenuItem";
            this.firmwareUpdateToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.firmwareUpdateToolStripMenuItem.Text = "Firmware Update";
            this.firmwareUpdateToolStripMenuItem.Click += new System.EventHandler(this.firmwareUpdateToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(165, 6);
            // 
            // installUSBDriversToolStripMenuItem
            // 
            this.installUSBDriversToolStripMenuItem.Name = "installUSBDriversToolStripMenuItem";
            this.installUSBDriversToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.installUSBDriversToolStripMenuItem.Text = "Install USB Drivers";
            this.installUSBDriversToolStripMenuItem.Click += new System.EventHandler(this.installUSBDriversToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(165, 6);
            // 
            // terminalToolStripMenuItem
            // 
            this.terminalToolStripMenuItem.Name = "terminalToolStripMenuItem";
            this.terminalToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.terminalToolStripMenuItem.Text = "Terminal";
            this.terminalToolStripMenuItem.Visible = false;
            this.terminalToolStripMenuItem.Click += new System.EventHandler(this.terminalToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.gotoRaceVoiceComToolStripMenuItem,
            this.releaseNotesToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // gotoRaceVoiceComToolStripMenuItem
            // 
            this.gotoRaceVoiceComToolStripMenuItem.Name = "gotoRaceVoiceComToolStripMenuItem";
            this.gotoRaceVoiceComToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.gotoRaceVoiceComToolStripMenuItem.Text = "Goto RaceVoice.Com";
            this.gotoRaceVoiceComToolStripMenuItem.Click += new System.EventHandler(this.gotoRaceVoiceComToolStripMenuItem_Click);
            // 
            // releaseNotesToolStripMenuItem
            // 
            this.releaseNotesToolStripMenuItem.Name = "releaseNotesToolStripMenuItem";
            this.releaseNotesToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.releaseNotesToolStripMenuItem.Text = "Release Notes";
            this.releaseNotesToolStripMenuItem.Click += new System.EventHandler(this.releaseNotesToolStripMenuItem_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmbTracks);
            this.groupBox4.Location = new System.Drawing.Point(5, 26);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(1);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(1);
            this.groupBox4.Size = new System.Drawing.Size(222, 41);
            this.groupBox4.TabIndex = 12;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Track Selection";
            // 
            // cmbTracks
            // 
            this.cmbTracks.FormattingEnabled = true;
            this.cmbTracks.Location = new System.Drawing.Point(7, 15);
            this.cmbTracks.Margin = new System.Windows.Forms.Padding(1);
            this.cmbTracks.Name = "cmbTracks";
            this.cmbTracks.Size = new System.Drawing.Size(213, 21);
            this.cmbTracks.TabIndex = 13;
            this.cmbTracks.SelectedIndexChanged += new System.EventHandler(this.cmbTracks_SelectedIndexChanged);
            // 
            // sendConfig
            // 
            this.sendConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendConfig.Location = new System.Drawing.Point(5, 728);
            this.sendConfig.Margin = new System.Windows.Forms.Padding(1);
            this.sendConfig.Name = "sendConfig";
            this.sendConfig.Size = new System.Drawing.Size(222, 30);
            this.sendConfig.TabIndex = 13;
            this.sendConfig.Text = "Send Configuration";
            this.sendConfig.UseVisualStyleBackColor = true;
            this.sendConfig.Click += new System.EventHandler(this.sendConfigButton);
            // 
            // getConfig
            // 
            this.getConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.getConfig.Location = new System.Drawing.Point(5, 760);
            this.getConfig.Margin = new System.Windows.Forms.Padding(1);
            this.getConfig.Name = "getConfig";
            this.getConfig.Size = new System.Drawing.Size(222, 30);
            this.getConfig.TabIndex = 14;
            this.getConfig.Text = "Get Configuration";
            this.getConfig.UseVisualStyleBackColor = true;
            this.getConfig.Click += new System.EventHandler(this.getConfigButton);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar1.Location = new System.Drawing.Point(5, 831);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(1);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(222, 30);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 15;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tabEngine);
            this.tabControl1.Controls.Add(this.tabDynamics);
            this.tabControl1.Location = new System.Drawing.Point(5, 70);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(222, 642);
            this.tabControl1.TabIndex = 16;
            // 
            // tabEngine
            // 
            this.tabEngine.Controls.Add(this.label8);
            this.tabEngine.Controls.Add(this.rdoToneNotify);
            this.tabEngine.Controls.Add(this.rdoSpeechNotify);
            this.tabEngine.Controls.Add(this.label7);
            this.tabEngine.Controls.Add(this.label6);
            this.tabEngine.Controls.Add(this.numOilPressureRpm);
            this.tabEngine.Controls.Add(this.numDownShift);
            this.tabEngine.Controls.Add(this.numVoltage);
            this.tabEngine.Controls.Add(this.numTemperature);
            this.tabEngine.Controls.Add(this.numOilPressurePsi);
            this.tabEngine.Controls.Add(this.numUpShift);
            this.tabEngine.Controls.Add(this.numOverRev);
            this.tabEngine.Controls.Add(this.chkVoltage);
            this.tabEngine.Controls.Add(this.chkTemperature);
            this.tabEngine.Controls.Add(this.chkOilPressure);
            this.tabEngine.Controls.Add(this.chkDownShift);
            this.tabEngine.Controls.Add(this.chkUpShift);
            this.tabEngine.Controls.Add(this.chkOverRev);
            this.tabEngine.Controls.Add(this.cmbEcuType);
            this.tabEngine.Controls.Add(this.label3);
            this.tabEngine.Location = new System.Drawing.Point(4, 22);
            this.tabEngine.Margin = new System.Windows.Forms.Padding(1);
            this.tabEngine.Name = "tabEngine";
            this.tabEngine.Padding = new System.Windows.Forms.Padding(1);
            this.tabEngine.Size = new System.Drawing.Size(214, 616);
            this.tabEngine.TabIndex = 0;
            this.tabEngine.Text = "Engine";
            this.tabEngine.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(2, 253);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Notification Type";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // rdoToneNotify
            // 
            this.rdoToneNotify.AutoSize = true;
            this.rdoToneNotify.Location = new System.Drawing.Point(166, 251);
            this.rdoToneNotify.Name = "rdoToneNotify";
            this.rdoToneNotify.Size = new System.Drawing.Size(50, 17);
            this.rdoToneNotify.TabIndex = 18;
            this.rdoToneNotify.Text = "Tone";
            this.rdoToneNotify.UseVisualStyleBackColor = true;
            this.rdoToneNotify.CheckedChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // rdoSpeechNotify
            // 
            this.rdoSpeechNotify.AutoSize = true;
            this.rdoSpeechNotify.Checked = true;
            this.rdoSpeechNotify.Location = new System.Drawing.Point(104, 251);
            this.rdoSpeechNotify.Name = "rdoSpeechNotify";
            this.rdoSpeechNotify.Size = new System.Drawing.Size(62, 17);
            this.rdoSpeechNotify.TabIndex = 18;
            this.rdoSpeechNotify.TabStop = true;
            this.rdoSpeechNotify.Text = "Speech";
            this.rdoSpeechNotify.UseVisualStyleBackColor = true;
            this.rdoSpeechNotify.CheckedChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 170);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "RPM";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 148);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "PSI";
            // 
            // numOilPressureRpm
            // 
            this.numOilPressureRpm.Location = new System.Drawing.Point(54, 168);
            this.numOilPressureRpm.Margin = new System.Windows.Forms.Padding(1);
            this.numOilPressureRpm.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numOilPressureRpm.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numOilPressureRpm.Name = "numOilPressureRpm";
            this.numOilPressureRpm.Size = new System.Drawing.Size(159, 20);
            this.numOilPressureRpm.TabIndex = 20;
            this.numOilPressureRpm.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numOilPressureRpm.ValueChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // numDownShift
            // 
            this.numDownShift.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numDownShift.Location = new System.Drawing.Point(110, 227);
            this.numDownShift.Margin = new System.Windows.Forms.Padding(1);
            this.numDownShift.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numDownShift.Name = "numDownShift";
            this.numDownShift.Size = new System.Drawing.Size(102, 20);
            this.numDownShift.TabIndex = 19;
            this.numDownShift.ValueChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // numVoltage
            // 
            this.numVoltage.DecimalPlaces = 2;
            this.numVoltage.Location = new System.Drawing.Point(110, 87);
            this.numVoltage.Margin = new System.Windows.Forms.Padding(1);
            this.numVoltage.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numVoltage.Name = "numVoltage";
            this.numVoltage.Size = new System.Drawing.Size(102, 20);
            this.numVoltage.TabIndex = 18;
            this.numVoltage.ValueChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // numTemperature
            // 
            this.numTemperature.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numTemperature.Location = new System.Drawing.Point(110, 65);
            this.numTemperature.Margin = new System.Windows.Forms.Padding(1);
            this.numTemperature.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numTemperature.Name = "numTemperature";
            this.numTemperature.Size = new System.Drawing.Size(102, 20);
            this.numTemperature.TabIndex = 17;
            this.numTemperature.ValueChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // numOilPressurePsi
            // 
            this.numOilPressurePsi.Location = new System.Drawing.Point(53, 146);
            this.numOilPressurePsi.Margin = new System.Windows.Forms.Padding(1);
            this.numOilPressurePsi.Maximum = new decimal(new int[] {
            1004,
            0,
            0,
            0});
            this.numOilPressurePsi.Name = "numOilPressurePsi";
            this.numOilPressurePsi.Size = new System.Drawing.Size(159, 20);
            this.numOilPressurePsi.TabIndex = 16;
            this.numOilPressurePsi.ValueChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // numUpShift
            // 
            this.numUpShift.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numUpShift.Location = new System.Drawing.Point(110, 205);
            this.numUpShift.Margin = new System.Windows.Forms.Padding(1);
            this.numUpShift.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numUpShift.Name = "numUpShift";
            this.numUpShift.Size = new System.Drawing.Size(102, 20);
            this.numUpShift.TabIndex = 15;
            this.numUpShift.ValueChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // numOverRev
            // 
            this.numOverRev.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numOverRev.Location = new System.Drawing.Point(110, 43);
            this.numOverRev.Margin = new System.Windows.Forms.Padding(1);
            this.numOverRev.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numOverRev.Name = "numOverRev";
            this.numOverRev.Size = new System.Drawing.Size(102, 20);
            this.numOverRev.TabIndex = 14;
            this.numOverRev.ValueChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // chkVoltage
            // 
            this.chkVoltage.AutoSize = true;
            this.chkVoltage.Location = new System.Drawing.Point(5, 88);
            this.chkVoltage.Margin = new System.Windows.Forms.Padding(1);
            this.chkVoltage.Name = "chkVoltage";
            this.chkVoltage.Size = new System.Drawing.Size(62, 17);
            this.chkVoltage.TabIndex = 7;
            this.chkVoltage.Text = "Voltage";
            this.chkVoltage.UseVisualStyleBackColor = true;
            this.chkVoltage.CheckStateChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // chkTemperature
            // 
            this.chkTemperature.AutoSize = true;
            this.chkTemperature.Location = new System.Drawing.Point(5, 66);
            this.chkTemperature.Margin = new System.Windows.Forms.Padding(1);
            this.chkTemperature.Name = "chkTemperature";
            this.chkTemperature.Size = new System.Drawing.Size(101, 17);
            this.chkTemperature.TabIndex = 6;
            this.chkTemperature.Text = "Temperature (F)";
            this.chkTemperature.UseVisualStyleBackColor = true;
            this.chkTemperature.CheckStateChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // chkOilPressure
            // 
            this.chkOilPressure.AutoSize = true;
            this.chkOilPressure.Location = new System.Drawing.Point(5, 127);
            this.chkOilPressure.Margin = new System.Windows.Forms.Padding(1);
            this.chkOilPressure.Name = "chkOilPressure";
            this.chkOilPressure.Size = new System.Drawing.Size(82, 17);
            this.chkOilPressure.TabIndex = 5;
            this.chkOilPressure.Text = "Oil Pressure";
            this.chkOilPressure.UseVisualStyleBackColor = true;
            this.chkOilPressure.CheckStateChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // chkDownShift
            // 
            this.chkDownShift.AutoSize = true;
            this.chkDownShift.Location = new System.Drawing.Point(5, 228);
            this.chkDownShift.Margin = new System.Windows.Forms.Padding(1);
            this.chkDownShift.Name = "chkDownShift";
            this.chkDownShift.Size = new System.Drawing.Size(75, 17);
            this.chkDownShift.TabIndex = 4;
            this.chkDownShift.Text = "DownShift";
            this.chkDownShift.UseVisualStyleBackColor = true;
            this.chkDownShift.CheckStateChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // chkUpShift
            // 
            this.chkUpShift.AutoSize = true;
            this.chkUpShift.Location = new System.Drawing.Point(5, 206);
            this.chkUpShift.Margin = new System.Windows.Forms.Padding(1);
            this.chkUpShift.Name = "chkUpShift";
            this.chkUpShift.Size = new System.Drawing.Size(61, 17);
            this.chkUpShift.TabIndex = 3;
            this.chkUpShift.Text = "UpShift";
            this.chkUpShift.UseVisualStyleBackColor = true;
            this.chkUpShift.CheckStateChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // chkOverRev
            // 
            this.chkOverRev.AutoSize = true;
            this.chkOverRev.Location = new System.Drawing.Point(5, 44);
            this.chkOverRev.Margin = new System.Windows.Forms.Padding(1);
            this.chkOverRev.Name = "chkOverRev";
            this.chkOverRev.Size = new System.Drawing.Size(69, 17);
            this.chkOverRev.TabIndex = 2;
            this.chkOverRev.Text = "OverRev";
            this.chkOverRev.UseVisualStyleBackColor = true;
            this.chkOverRev.CheckStateChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // cmbEcuType
            // 
            this.cmbEcuType.FormattingEnabled = true;
            this.cmbEcuType.Items.AddRange(new object[] {
            "AIM EVO5,MXL2,MXP/G/S,DL2",
            "MoTec C1XX",
            "AIM SmartyCam-Mode1",
            "AIM SmartyCam-Mode2",
            "RaceLogic VBOX",
            "Autosport Labs",
            "Standalone Mode",
            "OBD-II  (ISO-15765)"});
            this.cmbEcuType.Location = new System.Drawing.Point(5, 16);
            this.cmbEcuType.Margin = new System.Windows.Forms.Padding(1);
            this.cmbEcuType.Name = "cmbEcuType";
            this.cmbEcuType.Size = new System.Drawing.Size(209, 21);
            this.cmbEcuType.TabIndex = 1;
            this.cmbEcuType.SelectedIndexChanged += new System.EventHandler(this.EngineDataValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 1);
            this.label3.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Dashboard / ECU Type";
            // 
            // tabDynamics
            // 
            this.tabDynamics.Controls.Add(this.braketonebox);
            this.tabDynamics.Controls.Add(this.groupBox6);
            this.tabDynamics.Controls.Add(this.wheelLockbox);
            this.tabDynamics.Controls.Add(this.groupBox3);
            this.tabDynamics.Location = new System.Drawing.Point(4, 22);
            this.tabDynamics.Margin = new System.Windows.Forms.Padding(1);
            this.tabDynamics.Name = "tabDynamics";
            this.tabDynamics.Padding = new System.Windows.Forms.Padding(1);
            this.tabDynamics.Size = new System.Drawing.Size(214, 616);
            this.tabDynamics.TabIndex = 1;
            this.tabDynamics.Text = "Dynamics";
            this.tabDynamics.UseVisualStyleBackColor = true;
            // 
            // braketonebox
            // 
            this.braketonebox.Controls.Add(this.numMaxBrakeThreshold);
            this.braketonebox.Controls.Add(this.label10);
            this.braketonebox.Controls.Add(this.label9);
            this.braketonebox.Controls.Add(this.numMinBrakeThreshold);
            this.braketonebox.Controls.Add(this.chkBrakeThreshold);
            this.braketonebox.Location = new System.Drawing.Point(5, 340);
            this.braketonebox.Name = "braketonebox";
            this.braketonebox.Size = new System.Drawing.Size(205, 99);
            this.braketonebox.TabIndex = 18;
            this.braketonebox.TabStop = false;
            this.braketonebox.Text = "Brake Threshold Tone";
            // 
            // numMaxBrakeThreshold
            // 
            this.numMaxBrakeThreshold.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMaxBrakeThreshold.Location = new System.Drawing.Point(105, 61);
            this.numMaxBrakeThreshold.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numMaxBrakeThreshold.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMaxBrakeThreshold.Name = "numMaxBrakeThreshold";
            this.numMaxBrakeThreshold.Size = new System.Drawing.Size(94, 20);
            this.numMaxBrakeThreshold.TabIndex = 20;
            this.numMaxBrakeThreshold.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMaxBrakeThreshold.ValueChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(102, 45);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "Max (PSI)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(2, 45);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Min (PSI)";
            // 
            // numMinBrakeThreshold
            // 
            this.numMinBrakeThreshold.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMinBrakeThreshold.Location = new System.Drawing.Point(5, 61);
            this.numMinBrakeThreshold.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numMinBrakeThreshold.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMinBrakeThreshold.Name = "numMinBrakeThreshold";
            this.numMinBrakeThreshold.Size = new System.Drawing.Size(94, 20);
            this.numMinBrakeThreshold.TabIndex = 18;
            this.numMinBrakeThreshold.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numMinBrakeThreshold.ValueChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // chkBrakeThreshold
            // 
            this.chkBrakeThreshold.AutoSize = true;
            this.chkBrakeThreshold.Location = new System.Drawing.Point(6, 19);
            this.chkBrakeThreshold.Name = "chkBrakeThreshold";
            this.chkBrakeThreshold.Size = new System.Drawing.Size(65, 17);
            this.chkBrakeThreshold.TabIndex = 18;
            this.chkBrakeThreshold.Text = "Enabled";
            this.chkBrakeThreshold.UseVisualStyleBackColor = true;
            this.chkBrakeThreshold.CheckedChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.chkAnnounceLapDelta);
            this.groupBox6.Controls.Add(this.chkAnnounceBestLap);
            this.groupBox6.Controls.Add(this.numAnnounceSpeed);
            this.groupBox6.Controls.Add(this.chkAnnounceSpeed);
            this.groupBox6.Location = new System.Drawing.Point(5, 237);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(205, 97);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Lap Announce";
            // 
            // chkAnnounceLapDelta
            // 
            this.chkAnnounceLapDelta.AutoSize = true;
            this.chkAnnounceLapDelta.Location = new System.Drawing.Point(6, 68);
            this.chkAnnounceLapDelta.Name = "chkAnnounceLapDelta";
            this.chkAnnounceLapDelta.Size = new System.Drawing.Size(92, 17);
            this.chkAnnounceLapDelta.TabIndex = 3;
            this.chkAnnounceLapDelta.Text = "Announce +/-";
            this.chkAnnounceLapDelta.UseVisualStyleBackColor = true;
            this.chkAnnounceLapDelta.CheckedChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // chkAnnounceBestLap
            // 
            this.chkAnnounceBestLap.AutoSize = true;
            this.chkAnnounceBestLap.Location = new System.Drawing.Point(6, 45);
            this.chkAnnounceBestLap.Name = "chkAnnounceBestLap";
            this.chkAnnounceBestLap.Size = new System.Drawing.Size(127, 17);
            this.chkAnnounceBestLap.TabIndex = 2;
            this.chkAnnounceBestLap.Text = "Announce BEST Lap";
            this.chkAnnounceBestLap.UseVisualStyleBackColor = true;
            this.chkAnnounceBestLap.CheckedChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // numAnnounceSpeed
            // 
            this.numAnnounceSpeed.Location = new System.Drawing.Point(121, 19);
            this.numAnnounceSpeed.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numAnnounceSpeed.Name = "numAnnounceSpeed";
            this.numAnnounceSpeed.Size = new System.Drawing.Size(78, 20);
            this.numAnnounceSpeed.TabIndex = 1;
            this.numAnnounceSpeed.ValueChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // chkAnnounceSpeed
            // 
            this.chkAnnounceSpeed.AutoSize = true;
            this.chkAnnounceSpeed.Location = new System.Drawing.Point(6, 20);
            this.chkAnnounceSpeed.Name = "chkAnnounceSpeed";
            this.chkAnnounceSpeed.Size = new System.Drawing.Size(109, 17);
            this.chkAnnounceSpeed.TabIndex = 0;
            this.chkAnnounceSpeed.Text = "Announce Speed";
            this.chkAnnounceSpeed.UseVisualStyleBackColor = true;
            this.chkAnnounceSpeed.CheckedChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // wheelLockbox
            // 
            this.wheelLockbox.Controls.Add(this.numWheelSpeedDiff);
            this.wheelLockbox.Controls.Add(this.label5);
            this.wheelLockbox.Controls.Add(this.numBrakePSI);
            this.wheelLockbox.Controls.Add(this.label4);
            this.wheelLockbox.Controls.Add(this.chkActiveWheelLockDetection);
            this.wheelLockbox.Location = new System.Drawing.Point(5, 89);
            this.wheelLockbox.Name = "wheelLockbox";
            this.wheelLockbox.Size = new System.Drawing.Size(206, 142);
            this.wheelLockbox.TabIndex = 1;
            this.wheelLockbox.TabStop = false;
            this.wheelLockbox.Text = "Wheel Lockup Announce";
            // 
            // numWheelSpeedDiff
            // 
            this.numWheelSpeedDiff.Location = new System.Drawing.Point(9, 106);
            this.numWheelSpeedDiff.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numWheelSpeedDiff.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numWheelSpeedDiff.Name = "numWheelSpeedDiff";
            this.numWheelSpeedDiff.Size = new System.Drawing.Size(190, 20);
            this.numWheelSpeedDiff.TabIndex = 4;
            this.numWheelSpeedDiff.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numWheelSpeedDiff.ValueChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Wheel Speed Percent Difference";
            // 
            // numBrakePSI
            // 
            this.numBrakePSI.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numBrakePSI.Location = new System.Drawing.Point(9, 58);
            this.numBrakePSI.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numBrakePSI.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numBrakePSI.Name = "numBrakePSI";
            this.numBrakePSI.Size = new System.Drawing.Size(191, 20);
            this.numBrakePSI.TabIndex = 2;
            this.numBrakePSI.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numBrakePSI.ValueChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Brake Threshold PSI";
            // 
            // chkActiveWheelLockDetection
            // 
            this.chkActiveWheelLockDetection.AutoSize = true;
            this.chkActiveWheelLockDetection.Location = new System.Drawing.Point(6, 19);
            this.chkActiveWheelLockDetection.Name = "chkActiveWheelLockDetection";
            this.chkActiveWheelLockDetection.Size = new System.Drawing.Size(166, 17);
            this.chkActiveWheelLockDetection.TabIndex = 0;
            this.chkActiveWheelLockDetection.Text = "Active Wheel Lock Detection";
            this.chkActiveWheelLockDetection.UseVisualStyleBackColor = true;
            this.chkActiveWheelLockDetection.CheckedChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numLinearGForce);
            this.groupBox3.Controls.Add(this.numLateralGForce);
            this.groupBox3.Controls.Add(this.chkLinearGForce);
            this.groupBox3.Controls.Add(this.chkLateralGForce);
            this.groupBox3.Location = new System.Drawing.Point(4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(206, 79);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "G-Force Announce";
            // 
            // numLinearGForce
            // 
            this.numLinearGForce.DecimalPlaces = 1;
            this.numLinearGForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numLinearGForce.Location = new System.Drawing.Point(111, 42);
            this.numLinearGForce.Name = "numLinearGForce";
            this.numLinearGForce.Size = new System.Drawing.Size(89, 20);
            this.numLinearGForce.TabIndex = 3;
            this.numLinearGForce.ValueChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // numLateralGForce
            // 
            this.numLateralGForce.DecimalPlaces = 1;
            this.numLateralGForce.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numLateralGForce.Location = new System.Drawing.Point(111, 19);
            this.numLateralGForce.Name = "numLateralGForce";
            this.numLateralGForce.Size = new System.Drawing.Size(89, 20);
            this.numLateralGForce.TabIndex = 2;
            this.numLateralGForce.ValueChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // chkLinearGForce
            // 
            this.chkLinearGForce.AutoSize = true;
            this.chkLinearGForce.Location = new System.Drawing.Point(6, 43);
            this.chkLinearGForce.Name = "chkLinearGForce";
            this.chkLinearGForce.Size = new System.Drawing.Size(96, 17);
            this.chkLinearGForce.TabIndex = 1;
            this.chkLinearGForce.Text = "Linear G-Force";
            this.chkLinearGForce.UseVisualStyleBackColor = true;
            this.chkLinearGForce.CheckedChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // chkLateralGForce
            // 
            this.chkLateralGForce.AutoSize = true;
            this.chkLateralGForce.Location = new System.Drawing.Point(6, 20);
            this.chkLateralGForce.Name = "chkLateralGForce";
            this.chkLateralGForce.Size = new System.Drawing.Size(99, 17);
            this.chkLateralGForce.TabIndex = 0;
            this.chkLateralGForce.Text = "Lateral G-Force";
            this.chkLateralGForce.UseVisualStyleBackColor = true;
            this.chkLateralGForce.CheckedChanged += new System.EventHandler(this.DynamicsDataValueChanged);
            // 
            // tabMain
            // 
            this.tabMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabMain.Controls.Add(this.tabTrack);
            this.tabMain.Controls.Add(this.tabCharts);
            this.tabMain.Controls.Add(this.tabPage1);
            this.tabMain.Location = new System.Drawing.Point(230, 22);
            this.tabMain.Margin = new System.Windows.Forms.Padding(1);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1017, 839);
            this.tabMain.TabIndex = 17;
            // 
            // tabTrack
            // 
            this.tabTrack.Controls.Add(this.speechTagBox);
            this.tabTrack.Controls.Add(this.btnSaveTrack);
            this.tabTrack.Controls.Add(this.TrackView);
            this.tabTrack.Controls.Add(this.zoom);
            this.tabTrack.Controls.Add(this.rotation);
            this.tabTrack.Controls.Add(this.vScroll);
            this.tabTrack.Controls.Add(this.hScroll);
            this.tabTrack.Controls.Add(this.label2);
            this.tabTrack.Controls.Add(this.label1);
            this.tabTrack.Controls.Add(this.splitsBox);
            this.tabTrack.Controls.Add(this.grpData);
            this.tabTrack.Controls.Add(this.segmentGroupBox);
            this.tabTrack.Location = new System.Drawing.Point(4, 22);
            this.tabTrack.Margin = new System.Windows.Forms.Padding(1);
            this.tabTrack.Name = "tabTrack";
            this.tabTrack.Padding = new System.Windows.Forms.Padding(1);
            this.tabTrack.Size = new System.Drawing.Size(1009, 813);
            this.tabTrack.TabIndex = 0;
            this.tabTrack.Text = "Track";
            this.tabTrack.UseVisualStyleBackColor = true;
            // 
            // speechTagBox
            // 
            this.speechTagBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.speechTagBox.Controls.Add(this.btnHideAllSpeechTags);
            this.speechTagBox.Controls.Add(this.lstSpeechTags);
            this.speechTagBox.Controls.Add(this.btnSpeechTagDelete);
            this.speechTagBox.Location = new System.Drawing.Point(820, 572);
            this.speechTagBox.Margin = new System.Windows.Forms.Padding(1);
            this.speechTagBox.Name = "speechTagBox";
            this.speechTagBox.Padding = new System.Windows.Forms.Padding(1);
            this.speechTagBox.Size = new System.Drawing.Size(183, 174);
            this.speechTagBox.TabIndex = 8;
            this.speechTagBox.TabStop = false;
            this.speechTagBox.Text = "Speech Tags";
            this.speechTagBox.Visible = false;
            // 
            // btnHideAllSpeechTags
            // 
            this.btnHideAllSpeechTags.Location = new System.Drawing.Point(8, 115);
            this.btnHideAllSpeechTags.Margin = new System.Windows.Forms.Padding(1);
            this.btnHideAllSpeechTags.Name = "btnHideAllSpeechTags";
            this.btnHideAllSpeechTags.Size = new System.Drawing.Size(167, 23);
            this.btnHideAllSpeechTags.TabIndex = 3;
            this.btnHideAllSpeechTags.Text = "Clear All";
            this.btnHideAllSpeechTags.UseVisualStyleBackColor = true;
            this.btnHideAllSpeechTags.Click += new System.EventHandler(this.btnHideAllSpeechTags_Click);
            // 
            // lstSpeechTags
            // 
            this.lstSpeechTags.FormattingEnabled = true;
            this.lstSpeechTags.Location = new System.Drawing.Point(9, 17);
            this.lstSpeechTags.Name = "lstSpeechTags";
            this.lstSpeechTags.Size = new System.Drawing.Size(167, 94);
            this.lstSpeechTags.TabIndex = 2;
            this.lstSpeechTags.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstSpeechTags_ItemCheck);
            // 
            // btnSpeechTagDelete
            // 
            this.btnSpeechTagDelete.Location = new System.Drawing.Point(8, 140);
            this.btnSpeechTagDelete.Margin = new System.Windows.Forms.Padding(1);
            this.btnSpeechTagDelete.Name = "btnSpeechTagDelete";
            this.btnSpeechTagDelete.Size = new System.Drawing.Size(167, 23);
            this.btnSpeechTagDelete.TabIndex = 1;
            this.btnSpeechTagDelete.Text = "Delete";
            this.btnSpeechTagDelete.UseVisualStyleBackColor = true;
            this.btnSpeechTagDelete.Click += new System.EventHandler(this.btnSpeechTagDelete_Click);
            // 
            // btnSaveTrack
            // 
            this.btnSaveTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveTrack.Enabled = false;
            this.btnSaveTrack.Location = new System.Drawing.Point(820, 538);
            this.btnSaveTrack.Name = "btnSaveTrack";
            this.btnSaveTrack.Size = new System.Drawing.Size(183, 30);
            this.btnSaveTrack.TabIndex = 18;
            this.btnSaveTrack.Text = "Save Track";
            this.btnSaveTrack.UseVisualStyleBackColor = true;
            this.btnSaveTrack.Click += new System.EventHandler(this.btnSaveTrack_Click);
            // 
            // TrackView
            // 
            this.TrackView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TrackView.BackColor = System.Drawing.Color.DarkGray;
            this.TrackView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TrackView.ContextMenuStrip = this.rendererRightClickMenu;
            this.TrackView.Location = new System.Drawing.Point(2, 61);
            this.TrackView.Margin = new System.Windows.Forms.Padding(1);
            this.TrackView.Name = "TrackView";
            this.TrackView.Size = new System.Drawing.Size(769, 717);
            this.TrackView.TabIndex = 11;
            this.TrackView.TabStop = false;
            this.TrackView.Click += new System.EventHandler(this.TrackView_Click);
            this.TrackView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrackView_MouseDown);
            this.TrackView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TrackView_MouseMove);
            this.TrackView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TrackView_MouseUp);
            // 
            // zoom
            // 
            this.zoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zoom.BackColor = System.Drawing.Color.White;
            this.zoom.Location = new System.Drawing.Point(48, 25);
            this.zoom.Margin = new System.Windows.Forms.Padding(1);
            this.zoom.Maximum = 200;
            this.zoom.Minimum = 50;
            this.zoom.Name = "zoom";
            this.zoom.Size = new System.Drawing.Size(723, 45);
            this.zoom.TabIndex = 14;
            this.zoom.Value = 90;
            this.zoom.Scroll += new System.EventHandler(this.zoom_Scroll);
            // 
            // rotation
            // 
            this.rotation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rotation.BackColor = System.Drawing.Color.White;
            this.rotation.Location = new System.Drawing.Point(48, 2);
            this.rotation.Margin = new System.Windows.Forms.Padding(1);
            this.rotation.Maximum = 360;
            this.rotation.Name = "rotation";
            this.rotation.Size = new System.Drawing.Size(723, 45);
            this.rotation.TabIndex = 12;
            this.rotation.Scroll += new System.EventHandler(this.rotation_Scroll);
            // 
            // vScroll
            // 
            this.vScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScroll.BackColor = System.Drawing.Color.White;
            this.vScroll.Location = new System.Drawing.Point(773, 61);
            this.vScroll.Margin = new System.Windows.Forms.Padding(1);
            this.vScroll.Maximum = 100;
            this.vScroll.Minimum = -100;
            this.vScroll.Name = "vScroll";
            this.vScroll.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.vScroll.Size = new System.Drawing.Size(45, 717);
            this.vScroll.TabIndex = 17;
            this.vScroll.Value = 1;
            this.vScroll.Scroll += new System.EventHandler(this.TrackScroll);
            // 
            // hScroll
            // 
            this.hScroll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScroll.BackColor = System.Drawing.Color.White;
            this.hScroll.Location = new System.Drawing.Point(3, 780);
            this.hScroll.Margin = new System.Windows.Forms.Padding(1);
            this.hScroll.Maximum = 100;
            this.hScroll.Minimum = -100;
            this.hScroll.Name = "hScroll";
            this.hScroll.Size = new System.Drawing.Size(768, 45);
            this.hScroll.TabIndex = 16;
            this.hScroll.Scroll += new System.EventHandler(this.TrackScroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Zoom";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Rotation";
            // 
            // tabCharts
            // 
            this.tabCharts.Controls.Add(this.webCharts);
            this.tabCharts.Location = new System.Drawing.Point(4, 22);
            this.tabCharts.Margin = new System.Windows.Forms.Padding(1);
            this.tabCharts.Name = "tabCharts";
            this.tabCharts.Padding = new System.Windows.Forms.Padding(1);
            this.tabCharts.Size = new System.Drawing.Size(1009, 813);
            this.tabCharts.TabIndex = 1;
            this.tabCharts.Text = "Charts";
            this.tabCharts.UseVisualStyleBackColor = true;
            // 
            // webCharts
            // 
            this.webCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webCharts.Location = new System.Drawing.Point(1, 1);
            this.webCharts.MinimumSize = new System.Drawing.Size(20, 20);
            this.webCharts.Name = "webCharts";
            this.webCharts.Size = new System.Drawing.Size(1007, 811);
            this.webCharts.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.webTables);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1009, 813);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Tables";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // webTables
            // 
            this.webTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webTables.Location = new System.Drawing.Point(0, 0);
            this.webTables.MinimumSize = new System.Drawing.Size(20, 20);
            this.webTables.Name = "webTables";
            this.webTables.Size = new System.Drawing.Size(1009, 813);
            this.webTables.TabIndex = 1;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "CSV Files|*.csv";
            this.openFileDialog.InitialDirectory = ".";
            // 
            // DownloadData
            // 
            this.DownloadData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DownloadData.Location = new System.Drawing.Point(6, 792);
            this.DownloadData.Margin = new System.Windows.Forms.Padding(1);
            this.DownloadData.Name = "DownloadData";
            this.DownloadData.Size = new System.Drawing.Size(222, 30);
            this.DownloadData.TabIndex = 18;
            this.DownloadData.Text = "Download Data";
            this.DownloadData.UseVisualStyleBackColor = true;
            this.DownloadData.Click += new System.EventHandler(this.DownloadData_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1257, 869);
            this.Controls.Add(this.DownloadData);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.getConfig);
            this.Controls.Add(this.sendConfig);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RaceVoice Studio";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.rendererRightClickMenu.ResumeLayout(false);
            this.splitsBox.ResumeLayout(false);
            this.segmentGroupBox.ResumeLayout(false);
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabEngine.ResumeLayout(false);
            this.tabEngine.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOilPressureRpm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDownShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVoltage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTemperature)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOilPressurePsi)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpShift)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOverRev)).EndInit();
            this.tabDynamics.ResumeLayout(false);
            this.braketonebox.ResumeLayout(false);
            this.braketonebox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxBrakeThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinBrakeThreshold)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAnnounceSpeed)).EndInit();
            this.wheelLockbox.ResumeLayout(false);
            this.wheelLockbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWheelSpeedDiff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBrakePSI)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLinearGForce)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLateralGForce)).EndInit();
            this.tabMain.ResumeLayout(false);
            this.tabTrack.ResumeLayout(false);
            this.tabTrack.PerformLayout();
            this.speechTagBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TrackView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vScroll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hScroll)).EndInit();
            this.tabCharts.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip rendererRightClickMenu;
        private System.Windows.Forms.ToolStripMenuItem addSegmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedSegmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSplitToolStripMenuItem;
        private System.Windows.Forms.GroupBox splitsBox;
        private System.Windows.Forms.Button btnSplitDelete;
        private System.Windows.Forms.GroupBox segmentGroupBox;
        private System.Windows.Forms.Button btnSegmentDelete;
        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.CheckBox chkLateralG;
        private System.Windows.Forms.CheckBox chkMaxLinearG;
        private System.Windows.Forms.CheckBox chkMaxLateralG;
        private System.Windows.Forms.CheckBox chkTurnInSpeed;
        private System.Windows.Forms.CheckBox chkExitSpeed;
        private System.Windows.Forms.CheckBox chkEntrySpeed;
        private System.Windows.Forms.CheckBox chkMinimumSpeed;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cmbTracks;
        private System.Windows.Forms.Button sendConfig;
        private System.Windows.Forms.Button getConfig;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabEngine;
        private System.Windows.Forms.TabPage tabDynamics;
        private System.Windows.Forms.CheckBox chkVoltage;
        private System.Windows.Forms.CheckBox chkTemperature;
        private System.Windows.Forms.CheckBox chkOilPressure;
        private System.Windows.Forms.CheckBox chkDownShift;
        private System.Windows.Forms.CheckBox chkUpShift;
        private System.Windows.Forms.CheckBox chkOverRev;
        private System.Windows.Forms.ComboBox cmbEcuType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numDownShift;
        private System.Windows.Forms.NumericUpDown numVoltage;
        private System.Windows.Forms.NumericUpDown numTemperature;
        private System.Windows.Forms.NumericUpDown numOilPressurePsi;
        private System.Windows.Forms.NumericUpDown numUpShift;
        private System.Windows.Forms.NumericUpDown numOverRev;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabTrack;
        private System.Windows.Forms.TrackBar vScroll;
        private System.Windows.Forms.TrackBar hScroll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox TrackView;
        private System.Windows.Forms.TrackBar zoom;
        private System.Windows.Forms.TrackBar rotation;
        private System.Windows.Forms.TabPage tabCharts;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gotoRaceVoiceComToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreAllTracksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem voiceSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataTraceToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox lstSplits;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox wheelLockbox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkAnnounceBestLap;
        private System.Windows.Forms.NumericUpDown numAnnounceSpeed;
        private System.Windows.Forms.CheckBox chkAnnounceSpeed;
        private System.Windows.Forms.NumericUpDown numWheelSpeedDiff;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numBrakePSI;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkActiveWheelLockDetection;
        private System.Windows.Forms.NumericUpDown numLinearGForce;
        private System.Windows.Forms.NumericUpDown numLateralGForce;
        private System.Windows.Forms.CheckBox chkLinearGForce;
        private System.Windows.Forms.CheckBox chkLateralGForce;
        private System.Windows.Forms.WebBrowser webCharts;
        private System.Windows.Forms.ToolStripMenuItem gPSSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chartsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMoTecCSVToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton rdoToneNotify;
        private System.Windows.Forms.RadioButton rdoSpeechNotify;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numOilPressureRpm;
        private System.Windows.Forms.GroupBox braketonebox;
        private System.Windows.Forms.NumericUpDown numMaxBrakeThreshold;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numMinBrakeThreshold;
        private System.Windows.Forms.CheckBox chkBrakeThreshold;
        private System.Windows.Forms.CheckBox chkAnnounceLapDelta;
        private System.Windows.Forms.ToolStripMenuItem messageTriggersToolStripMenuItem;
        private System.Windows.Forms.Button btnSaveTrack;
        private System.Windows.Forms.ToolStripMenuItem baudRateToolStripMenuItem;
        private System.Windows.Forms.Button btnHideAllSplits;
        private System.Windows.Forms.Button btnClearAllData;
        private System.Windows.Forms.ListBox lstSegments;
        private System.Windows.Forms.ToolStripMenuItem releaseNotesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreCurrentTrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importTrackMapsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cloneCurrentTrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteTrackToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.CheckBox chKsegmentRollingMph;
        private System.Windows.Forms.ToolStripMenuItem firmwareUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installUSBDriversToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.WebBrowser webTables;
        private System.Windows.Forms.Button DownloadData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem terminalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSpeechTagToolStripMenuItem;
        private System.Windows.Forms.GroupBox speechTagBox;
        private System.Windows.Forms.Button btnHideAllSpeechTags;
        private System.Windows.Forms.CheckedListBox lstSpeechTags;
        private System.Windows.Forms.Button btnSpeechTagDelete;
    }
}

