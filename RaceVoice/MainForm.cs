using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.CodeDom.Compiler;
#if (!APP)
using System.Windows.Forms;
using JR.Utils.GUI.Forms;
#endif

namespace RaceVoice
{
#if (!APP)
    public partial class MainForm : Form
#else
    public class RaceVoicePCMain
#endif
    {
#if (!APP)
        private iracing irace = new iracing();
#endif
        private const string METADATA_EXTENSION = ".json";

        private TrackRenderer _renderer;
        public TrackModel _trackModel;

        private TrackMetadata _trackMetadata;
        public string _trackMetafile;

        public string _trackFile;
        private racevoicecom rvcom = null;

#if APP
        private string _carMetafile = "";
        public CarMetadata _carMetadata;
        public RaceVoicePCMain()
        {
            bool ping_good = false;
            sqldatabase sql = new sqldatabase();
            _carMetafile = globals.LocalFolder() + "//car.json";
            _carMetadata = CarMetadata.Load(_carMetafile);
            globals.theUUID = HardwareInfo.GenerateUID();
            ping_good = globals.IsOnlineTest();
            if (ping_good)
            {
                globals.network_ok = true;
                sql.ValidateSystem(_carMetadata);
                _carMetadata.Save(_carMetafile);
            }


                rvcom = new racevoicecom();

        }

        public int GetComBarValue()
        {
            if (rvcom != null)
            {
                return rvcom.barval;
            }
            return 0;
        }
        public void SaveCarMetaData()
        {
            _carMetadata.Save(_carMetafile);

        }
#endif

#if (!APP)

        private const string _carMetafile = "car.json";
        private Bitmap _renderTarget;
        private CarMetadata _carMetadata;


        private CheckBox[] _dataCheckboxes;
        private bool _engineValuesUpdating;
        private bool _dynamicsValuesUpdating;
        private bool _dataCheckboxesUpdating;



      

        public MainForm()
        {
            //import aim_import = new import();
            //string file = globals.LocalFolder() + "\\aim\\mugello.ztracks";
            //string oname ="mug";
            //aim_import.ImportZTRACK(file,oname);
            //aim_import.Parse(true);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(globals.HandleException);
            InitializeComponent();

            if (globals.disabled_charts)
            {
                tabMain.TabPages.Remove(tabCharts);
                menuStrip1.Items.Remove(chartsMenuItem);
            }

            toolStripSeparator3.Visible = false;
            installUSBDriversToolStripMenuItem.Visible = false;
        }


        private void PopulateTracks()
        {
            if (Directory.Exists(globals.track_folder))
            {
                cmbTracks.Items.Clear();
                var files = Directory.GetFiles(globals.track_folder, "*.csv");
                foreach (var f in files)
                {
                    var trackModel = TrackModelParser.LoadFile(f);
                    var metaFile = GetMetaFileForCsv(f);
                    var metadata = TrackMetadata.Load(metaFile);
                    metadata = CheckTrackMetaData(f, metaFile, metadata, trackModel);
                }
            }

            try
            {
                if (_carMetadata.EngineData.TrackSelectionIndex >= cmbTracks.Items.Count)
                {
                    cmbTracks.SelectedIndex = 0;
                }
                else
                {
                    cmbTracks.SelectedIndex = _carMetadata.EngineData.TrackSelectionIndex;
                }
            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
            }
        }


    
        private void QueueTrackForSending(string name)
        {
            if (_carMetadata.HardwareData.ShareNewTracks)
            {
                string ufile = globals.LocalFolder() + "\\uploads.txt";
                name = name.Replace(' ', '_');
                StreamWriter stream = new StreamWriter(ufile, true);
                stream.WriteLine(name);
                stream.Close();
            }
        }

        private void SendNewTracksToServer()
        {
            if (_carMetadata.HardwareData.ShareNewTracks && globals.network_ok)
            {
                // are there any tracks that have been queded for upload?
                // Read the file and display it line by line. 
                email em = new email();
                string line;
                try
                {
                    string ufile = globals.LocalFolder() + "\\uploads.txt";
                    System.IO.StreamReader file = new System.IO.StreamReader(ufile);
                    if (file != null)
                        while ((line = file.ReadLine()) != null)
                        {
                            em.EmailTrackFile(line);
                        }
                    file.Close();
                    File.Delete(ufile);
                }
                catch (Exception ee)
                {
                    globals.WriteLine(ee.Message);
                }
            }
        }


        private void CheckNewUI()
        {

            FileDownloader fd = new FileDownloader();
            string[] rui = new string[2000];
            string path = globals.LocalFolder() + "\\";
            string line;
            int ri = 0;

            try
            {
                // download the ecu files
                fd.DownloadFile("ui.php", "remoteui.php");
                // now load the data into an array
                path = globals.LocalFolder() + "\\remoteui.php";
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                while ((line = file.ReadLine()) != null)
                {
                    globals.WriteLine(line);
                    rui[ri] = line;
                    ri++;
                }
                file.Close();
                System.IO.File.Delete(path);

                // now find the latest file on the server
                string[] ours = globals.UIVersion.Split('-');
                string mm = ours[0];
                string dd = ours[1];
                string yy = ours[2];
                DateTime ui_date = new DateTime(Convert.ToInt32(yy), Convert.ToInt32(mm), Convert.ToInt32(dd));
                for (int i =0;i<ri;i++)
                {
                    string[] rs = rui[i].Split('_');

                    mm = rs[1];
                    dd = rs[2];
                    yy = rs[3];
                    if (yy.Length == 2) yy = "20" + yy;
                    // any letters?
                    bool skip = false;
                    for (int j=0;j<yy.Length;j++)
                    {
                        if (yy[j] < '0' || yy[j] > '9') skip = true;
                    }
                    if (yy.Length > 4) skip = true;
                    if (skip) continue;

                    //yy = "2020";
                    //mm = "06";
                    DateTime rem_date = new DateTime(Convert.ToInt32(yy), Convert.ToInt32(mm), Convert.ToInt32(dd));

                    Console.WriteLine("UI Date=" + ui_date.ToString() +" VS Remote Date="+rem_date.ToString());


                    if (DateTime.Compare(rem_date,ui_date)>0)
                    {

                        DialogResult dr = MessageBox.Show("A new version of RaceVoiceStudio is available\r\nWould you like to update to the latest one?\r\n", "New Software", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (dr==DialogResult.Yes)
                        {

                            MessageBox.Show("Great, the RaceVoice.com webpage will open and you can download the installer from there.\r\n", "New Software", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            System.Diagnostics.Process.Start("https://www.racevoice.com/download/");
                            Environment.Exit(0);
                        }

                        return;

                    }
                }

            }
            catch (Exception e)
            {
                globals.WriteLine(e.Message);
                return;
            }


        }
        private void CheckNewEcus(splash isplash)
        {
            FileDownloader fd = new FileDownloader();
            string[] remoteecus = new string[2000];
            string[] localecus = new string[2000];
            string path = globals.LocalFolder() + "\\";
            string line = "";
            string emsg = "";
            int ri = 0;
            int li = 0;

            try
            {
                // download the ecu files
                fd.DownloadFile("ecus.php", "remoteecus.php");
                // now load the data into an array
                path = globals.LocalFolder() + "\\remoteecus.php";
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                while ((line = file.ReadLine()) != null)
                {
                    globals.WriteLine(line);
                    remoteecus[ri] = line;
                    ri++;
                }
                file.Close();
                System.IO.File.Delete(path);

            }
            catch (Exception e)
            {
                globals.WriteLine(e.Message);
                return;
            }

            path = globals.LocalFolder() + "\\ecus";
            // MessageBox.Show("!");
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
                Thread.Sleep(1000);
            }
            // Take a snapshot of the file system.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);

            // This method assumes that the application has discovery permissions
            // for all folders under the specified path.
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            //Create the query
            IEnumerable<System.IO.FileInfo> fileQuery =
            from file in fileList
            where file.Extension == ".json"
            orderby file.Name
            select file;

            //Execute the query. This might write out a lot of files!
            foreach (System.IO.FileInfo fi in fileQuery)
            {
                string hash = globals.CalculateMD5(fi.FullName);
                DateTime lastModified = System.IO.File.GetLastWriteTime(fi.FullName);
                string tn = fi.Name + "," + hash + "," + lastModified.ToString("MM/dd/yyyy");
                localecus[li] = tn;
                li++;
                globals.WriteLine("Local Scan->" + tn);
            }


            // now compare the files
            for (int j = 0; j < ri; j++)
            {
                bool found = false;
                string[] remecu = remoteecus[j].Split(',');
                for (int k = 0; k < li; k++)
                {
                    string[] ecuseg = localecus[k].Split(',');
                    globals.WriteLine("ECU Compare " + remecu[0] + " vs " + ecuseg[0]);
                    if (ecuseg[0].ToUpper().Equals(remecu[0].ToUpper()))
                    {
                        globals.WriteLine("FOUND!");
                        found = true;
                        break;
                    }
                }

                // found = false;
                if (!found)
                {
                    globals.WriteLine(remecu[0] + "-->** NOT FOUND!");
                    string remotefile = "\\ecus\\" + remecu[0];
                    string localfile = "\\ecus\\" + remecu[0];
                    globals.WriteLine("Downlad " + remotefile + "--->" + localfile);

                    double pct = Convert.ToDouble(j) / Convert.ToDouble(ri);
                    pct *= Convert.ToDouble(100);
                    isplash.setbar(Convert.ToInt32(pct));
                    isplash.setlabel("Downloading ECU map " + remecu[0]);
                    string[] nice = remecu[0].Split('.');
                    emsg += nice[0].Replace('_', ' ') + "\r\n";
                    fd.DownloadFile(remotefile, localfile);

                }
            }

            if (emsg.Length > 0)
            {
                DialogResult dr = FlexibleMessageBox.Show(emsg, "New ECUs Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CheckNewTracks(bool pushversions, bool force, string download_name)
        {
            sqldatabase sql = new sqldatabase();
            FileDownloader fd = new FileDownloader();
            string path = globals.LocalFolder() + "\\";
            string line;
            int ri = 0;
            int li = 0;
            int changes = 0;
            bool ping_good = false;
            string[] remotetracks = new string[2000];
            string[] localtracks = new string[2000];
            string[] updates = new string[2000];
            splash isplash = new splash(1);
            isplash.Show();

            globals.theUUID = HardwareInfo.GenerateUID();
            isplash.setbar(30);
            isplash.setlabel("Communicating ....");
            ping_good = globals.IsOnlineTest();
            if (ping_good)
            {
                globals.network_ok = true;
                isplash.setbar(60);
                sql.ValidateSystem(_carMetadata);
                _carMetadata.Save(_carMetafile);

                if (pushversions)
                {
                    isplash.Close();
                    return;
                }
                if (!globals.no_track_check)
                {
                    isplash.setbar(80);
                    isplash.setlabel("Checking For Updates ....");
                    CheckNewUI();
                    CheckNewEcus(isplash);

                    fd.DownloadFile("tracks.php", "remotetracks.php");
                    //MessageBox.Show("hi");

                    // now load the data into an array
                    try
                    {
                        path += "remotetracks.php";
                        System.IO.StreamReader file = new System.IO.StreamReader(path);
                        while ((line = file.ReadLine()) != null)
                        {
                            globals.WriteLine(line);
                            remotetracks[ri] = line;
                            ri++;
                        }
                        file.Close();
                        System.IO.File.Delete(path);

                    }
                    catch (Exception e)
                    {
                        globals.WriteLine(e.Message);
                        isplash.Close();
                        return;
                    }

                    path = globals.LocalFolder() + "\\tracks";
                    // MessageBox.Show("!");
                    if (Directory.Exists(path) == false)
                    {
                        Directory.CreateDirectory(path);
                        Thread.Sleep(1000);
                    }
                    // Take a snapshot of the file system.
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);

                    // This method assumes that the application has discovery permissions
                    // for all folders under the specified path.
                    IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

                    //Create the query
                    IEnumerable<System.IO.FileInfo> fileQuery =
                    from file in fileList
                    where file.Extension == ".csv"
                    orderby file.Name
                    select file;

                    //Execute the query. This might write out a lot of files!
                    foreach (System.IO.FileInfo fi in fileQuery)
                    {
                        string hash = globals.CalculateMD5(fi.FullName);
                        DateTime lastModified = System.IO.File.GetLastWriteTime(fi.FullName);
                        string tn = fi.Name + "," + hash + "," + lastModified.ToString("MM/dd/yyyy");
                        localtracks[li] = tn;
                        li++;
                        globals.WriteLine("Local Scan->" + tn);
                    }


                }
                else
                {
                    isplash.Close();
                }


            }
            else
            {
                // ping fail...
                isplash.Close();
                return;

            }

            string[] remotes;
            string[] locals;
            bool found = false;
            bool newer = false;
            // any changes or new tracks??
            string track_stat = "";
            string track_msg = "Updates Are Available - Press YES to Update RaceVoice Studio";
            found = newer = false;
            for (int i = 0; i < ri; i++)
            {
                remotes = remotetracks[i].Split(',');
                found = false;
                newer = false;
                for (int j = 0; j < li; j++)
                {
                    locals = localtracks[j].Split(',');
                    if (locals[0].ToUpper() == remotes[0].ToUpper())
                    {
                        found = true;
                        // first time registering, we load everything
                        if (globals.virgin_load == false)
                        {
                            // is local newer?
                            if (!globals.FileModifiedNewer(locals[0], locals[2], remotes[2], locals[1], remotes[1]))
                            {
                                newer = true;
                            }
                        }
                    }
                }

                if (!found || newer)
                {
                    track_stat += "New Track: " + remotes[0] + "\r\n";
                    updates[changes] = remotes[0];
                    changes++;

                }

            }



            if (force)
            {
                track_stat = "";
                track_msg = "Synchronize Tracks";
                updates = new string[100];
                changes = 0;

                for (int j = 0; j < ri; j++)
                {

                    remotes = remotetracks[j].Split(',');
                    if (download_name.Length > 0)
                    {
                        if (download_name.ToUpper().Contains(remotes[0].ToUpper()))
                        {
                            updates[0] = remotes[0];
                            track_stat += "Restore: " + remotes[0] + "\r\n";

                            changes++;
                            break;

                        }
                    }
                }

                if (track_stat.Length == 0)
                {
                    MessageBox.Show("The currently selected track is not available on the server", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


            if (track_stat.Length > 0)
            {
                DialogResult dr = DialogResult.Yes;
                if (globals.virgin_load == false)
                {
                   dr = FlexibleMessageBox.Show(track_stat, track_msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (dr == DialogResult.Yes)
                {
                    isplash.setbar(0);
                    for (int i = 0; i < changes; i++)
                    {
                        string remotefile = "";
                        string localfile = "";
                        isplash.setlabel("Downloading " + updates[i]);
                        if (updates[i].ToUpper().Contains(".EXE") || updates[i].ToUpper().Contains(".HEX"))
                        {
                            remotefile = "tracks/" + updates[i];
                            if (updates[i].ToUpper().Contains("EXE"))
                            {
                                localfile = updates[i] + ".new";
                            }
                            else
                            {
                                localfile = updates[i];
                            }
                        }
                        else
                        {
                            remotefile = "tracks/" + updates[i];
                            localfile = remotefile;
                        }
                        //if (updates[i].ToUpper().Contains("EXE")) continue;
                        globals.WriteLine("Updating -> " + remotefile);
                        fd.DownloadFile(remotefile, localfile);
                        double pct = Convert.ToDouble(i) / Convert.ToDouble(changes);
                        pct *= Convert.ToDouble(100);
                        isplash.setbar(Convert.ToInt32(pct));
                    }
                    //isplash.setbar(Convert.ToInt32(100));
                    isplash.Close();

                }
            }
            //MessageBox.Show("HI");
            isplash.Close();
        }

        private void UpdateTitle()
        {
            string debug = "";
            string MainFormText = "RaceVoice Studio ";

            MainFormText += "Version " + globals.UIVersion;
            if (globals.no_track_check) debug += " DEBUG:NO TRACK CHECKING";
            if (globals.no_unit_check) debug += " DEBUG:NO UNIT COMMUNICATION";
            if (globals.fake_connection)
            {
                this.Text = MainFormText + " : Unit [A9771341]  Version [April 4, 2019]";

            }
            else
            {
                if (globals.thePort.Length == 0)
                {
                    this.Text = MainFormText + " : No Connection : " + debug;

                }
                else
                {
                    this.Text = MainFormText + " : Unit [" + _carMetadata.HardwareData.Name + "]  Version [" + _carMetadata.HardwareData.Version + "]" + debug;
                }
            }
        }

    
        private void InitRaceVoiceHW(bool atboot)
        {
            firmware updater = new firmware();
            int update_stat = 0;
            _carMetadata.HardwareData.SWVersion = globals.UIVersion;

            if (!atboot)
            {
                if (!globals.IsRaceVoiceConnected()) return;
            }

            if (globals.thePort.Length == 0)
            {

                _carMetadata.Save(_carMetafile); // save what we read
                // no unit was found .. so do nothing
            }
            else
            {
                globals.first_connected = true;
                if (_carMetadata.HardwareData.GetConfigAtStart && atboot)
                {
                    ReadDataFromRaceVoice(false);
                    _carMetadata.Save(_carMetafile); // save what we read
                                                     // WriteDataToRaceVoice(); // update based on what we read BEFORE we performed firmware update cycle                         

                    if (updater.ComparePackagedFirmwareVersion(_carMetadata) || globals.force_firmware_update != 0)
                    {
                        update_stat = updater.UpdateFirmware();
                        if (update_stat == 1)
                        {
                            WriteDataToRaceVoice(); // update based on what we read BEFORE we performed firmware update cycle   
                            ReadDataFromRaceVoice(true); // get the new version
                        }
                        if (update_stat < 0)
                        {
                            globals.Terminate();

                        }
                    }
                    globals.WriteLine("done");

                }
                else
                {
                    ReadDataFromRaceVoice(true);  // get the version
                    _carMetadata.Save(_carMetafile); // save what we read ... should jsut be version and name
                    if (updater.ComparePackagedFirmwareVersion(_carMetadata) || globals.force_firmware_update != 0)
                    {
                        update_stat = updater.UpdateFirmware();
                        if (update_stat < 0)
                        {
                            globals.Terminate();

                        }
                        if (update_stat == 1)
                        {
                            ReadDataFromRaceVoice(true); // get the new version

                        }
                    }

                }
            }
            CheckNewTracks(true, false, "");
            UpdateTitle();
        }

        public class WindowStateInfo
        {
            public FormWindowState? WindowState { get; set; }

            public Point? WindowLocation { get; set; }
            public int width;
            public int height;
        }

        private void SaveWindowState(Form form)
        {
            string fn = globals.LocalFolder() + "\\appsetting.json";
            var state = new WindowStateInfo
            {
                WindowLocation = form.Location,
                WindowState = form.WindowState,
                width = form.Width,
                height = form.Height
            };

            File.WriteAllText(fn, JsonConvert.SerializeObject(state));
        }

        private void LoadWindowState(Form form)
        {
            try
            {
                string fn = globals.LocalFolder() + "\\appsetting.json";
                if (!File.Exists(fn)) return;

                var state = JsonConvert.DeserializeObject<WindowStateInfo>(File.ReadAllText(fn));

                    if (state.height>0 && state.width>0)
                 {
                        form.Width = state.width;
                        form.Height = state.height;
                 }
               if (state.WindowState.HasValue) form.WindowState = state.WindowState.Value;
                if (state.WindowLocation.HasValue) form.Location = state.WindowLocation.Value;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadWindowState(this);
            EcuMetadata ecuMetadata = new EcuMetadata();
            _dataCheckboxes = new CheckBox[]
            {
                chkMinimumSpeed,
                chkEntrySpeed,
                chkExitSpeed,
                chkTurnInSpeed,
                chkMaxLateralG,
                chkMaxLinearG,
                chkLateralG,
                chKsegmentRollingMph
            };

            _carMetadata = CarMetadata.Load(_carMetafile);
            if (_carMetadata.HardwareData.Trace >= 1) globals.trace = true;
            if (_carMetadata.HardwareData.HideWarnings.Contains("YES"))
            {
                licenseHideWarnings.Checked = true;
                globals.license_hide_warnings = true;
            }

            CheckNewTracks(false, false, "");

            PopulateTracks();
            SendNewTracksToServer();
            ecuMetadata.PopulateECU(cmbEcuType);

            rvcom = new racevoicecom();
            rvcom.SetBar(progressBar1);
            InitRaceVoiceHW(true);

            AdjustUIForFeatures();

            var chartUrl = "file:///" + globals.LocalFolder() + "/charts/index.html";
            var tablesUrl = "file:///" + globals.LocalFolder() + "/charts/tables.html";

            webCharts.Navigate(chartUrl);
            webTables.Navigate(tablesUrl);

            heartbeat.Enabled = true;

        }

        private void UpdateSplits()
        {
            lstSplits.Items.Clear();
            foreach (var t in _trackModel.Splits)
            {
                Console.WriteLine("split=" + t.Text + " val=" + t.Hidden.ToString());
                lstSplits.Items.Add(t.Text, !t.Hidden);
            }

            addSplitToolStripMenuItem.Enabled = _trackModel.Splits.Count < globals.MAX_SPLITS;
        }

        private void UpdateSpeechTags()
        {
            lstSpeechTags.Items.Clear();
            foreach (var t in _trackModel.SpeechTags)
            {
                Console.WriteLine("speech tag=" + t.Name + " val=" + t.Hidden.ToString());
                lstSpeechTags.Items.Add(t.Name, !t.Hidden);
            }

            addSpeechTagToolStripMenuItem.Enabled = _trackModel.SpeechTags.Count < globals.MAX_SPEECH_TAGS;
        }

        private void UpdateSegments()
        {
            lstSegments.Items.Clear();
            foreach (var t in _trackModel.Segments)
            {
                lstSegments.Items.Add(t.Name);
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (_renderTarget == null) return;

            _renderTarget.Dispose();

            _renderTarget = new Bitmap(Math.Max(256, TrackView.Width), Math.Max(256, TrackView.Height));

            ReRender();
        }

        private void TrackView_Click(object sender, EventArgs e)
        {
            if (_renderer == null) return;

            if (_renderer.HandleClick(sender, e))
            {
                if (_renderer.SelectedSegment == null)
                {
                    lstSegments.ClearSelected();
                }
                else
                {
                    lstSegments.SelectedIndex = _trackModel.Segments.IndexOf(_renderer.SelectedSegment);
                }
            }
        }

        private void UpdateDataCheckboxes(bool from_load)
        {
            UInt16 selectedBits;
            _dataCheckboxesUpdating = true;
          
                selectedBits = _renderer.SelectedSegment != null ? _renderer.SelectedSegment.DataBits : (UInt16)0;
                for (int i = 0; i < _dataCheckboxes.Length; i++)
                {
                    _dataCheckboxes[i].Checked = (selectedBits & (1 << i)) != 0;
                }
            AdjustUIForFeatures();
            _dataCheckboxesUpdating = false;
        }

        private void TrackView_MouseDown(object sender, MouseEventArgs e)
        {
            if (_renderer == null) return;

            if (_renderer.HandleMouseDown(sender, e))
            {
                ReRender();
            }
        }

        private void TrackView_MouseUp(object sender, MouseEventArgs e)
        {
            if (_renderer == null) return;

            if (_renderer.HandleMouseUp(sender, e))
            {
                ReRender();
            }
        }

        private void TrackView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_renderer == null) return;

            if (_renderer.HandleMouseMove(sender, e))
            {
                ReRender();
            }
        }

        private void rotation_Scroll(object sender, EventArgs e)
        {
            if (_renderer == null) return;

            _trackMetadata.Rotation = rotation.Value;
            _trackMetadata.Save(_trackMetafile);

            ReRender();
        }

        private void zoom_Scroll(object sender, EventArgs e)
        {
            if (_renderer == null) return;

            _trackMetadata.Zoom = zoom.Value;
            _trackMetadata.Save(_trackMetafile);

            ReRender();
        }

        private void SetZoomRotateOffsetValues()
        {
            _renderer.Zoom = (float)zoom.Value / 100;
            _renderer.Rotation = rotation.Value * 0.017453292f;
            _renderer.CenterOffset = new PointF((hScroll.Value / 100f) * TrackView.Width, (vScroll.Value / -100f) * TrackView.Height);
        }

        private void rendererRightClickMenu_Opening(object sender, CancelEventArgs e)
        {
            deleteSelectedSegmentToolStripMenuItem.Visible = _renderer.SelectedSegment != null;
        }

        private void addSegmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_trackModel.Segments.Count >= globals.MAX_SEGMENTS)
            {
                MessageBox.Show("Sorry, the maximum number of segments allowed is " + globals.MAX_SEGMENTS + "\r\nTry changing the map to reduce the number of segments.", "Too Many Segments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            String name = Interaction.InputBox("Segment Name");
            if (name.Length > 0)
            {
                if (_renderer.CanWeMakeASegmentHere())
                {
                    _trackModel = _renderer.CreateSegmentAtHighlightedPosition(name, TrackColor.LightBlue, TrackColor.LightBlue);

                    ReRender();
                    UpdateSegments();
                    SaveTrack();
                }
                else
                {
                    MessageBox.Show("You cannot create a new segment that overlaps an existing segment.\r\n", "Segment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void addSplitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String text = Interaction.InputBox("Enter name for new split point");
            if (text.Length > 0)
            {
                _trackModel = _renderer.CreateSplitAtHighlightedPosition(text);

                ReRender();
                UpdateSplits();
                ResaveSplitEnabledStates();
                SaveTrack();
            }
        }

        private void btnSplitDelete_Click(object sender, EventArgs e)
        {
            if (lstSplits.SelectedIndex > -1)
            {
                _trackModel = _renderer.DeleteSplit(_trackModel.Splits[lstSplits.SelectedIndex]);
                ReRender();
                UpdateSplits();
                ResaveSplitEnabledStates();
                SaveTrack();
            }
        }


        private void btnSpeechTagDelete_Click(object sender, EventArgs e)
        {
            if (lstSpeechTags.SelectedIndex > -1)
            {
                _trackModel = _renderer.DeleteSpeechTag(_trackModel.SpeechTags[lstSpeechTags.SelectedIndex]);
                ReRender();
                UpdateSpeechTags();
                ResaveSpeechTagEnabledStates();
                SaveTrack();
            }
        }

        private void btnSegmentDelete_Click(object sender, EventArgs e)
        {
            if (lstSegments.SelectedIndex > -1)
            {
                _trackModel = _renderer.DeleteSegment(_trackModel.Segments[lstSegments.SelectedIndex]);
                ReRender();
                UpdateSegments();
                SaveTrack();
            }
        }

        private void deleteSelectedSegmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_renderer.SelectedSegment != null)
            {
                _renderer.DeleteSegment(_renderer.SelectedSegment);
                ReRender();
                UpdateSegments();
                SaveTrack();
            }
        }
        private void TrackScroll(object sender, EventArgs e)
        {
            _trackMetadata.XPan = hScroll.Value;
            _trackMetadata.YPan = vScroll.Value;
            _trackMetadata.Save(_trackMetafile);
            ReRender();
        }

        private bool ShowSelectedTrack(bool from_index)
        {
            int index = 0;
            bool changed = false;
            bool got_name = false;
            try
            {
                if (from_index)
                {
                    // this is when we are reading data back from the unit itself
                    // find by name?
                    if (_carMetadata.EngineData.FindTrackByName)
                    {
                        _carMetadata.EngineData.FindTrackByName = false;
                        if (GotoTrackName(_carMetadata.EngineData.TrackSelectionName, true)) got_name = true;
                    }

                    if (!got_name)
                    { // find by index?
                        index = _carMetadata.EngineData.TrackSelectionIndex;
                        if (index > cmbTracks.Items.Count)
                        {
                            index = 0;
                        }
                        if (index != cmbTracks.SelectedIndex) changed = true;
                        cmbTracks.SelectedIndex = index;

                    }
                    else
                    {
                        changed = true;
                    }

                }
                else
                {
                    changed = true;
                    // otherwise, change the UI if the user picks a new track
                }

                if (changed)
                {

                    var selection = (ComboBoxItem<string>)cmbTracks.SelectedItem;
                    LoadTrackFile(selection.Value.ToString());

                    _carMetadata.EngineData.TrackSelectionIndex = cmbTracks.SelectedIndex;
                    _carMetadata.EngineData.TrackSelectionName = _trackMetadata.TrackName;
                    _carMetadata.Save(_carMetafile);
                }
            }
            catch (Exception ee)
            {
                // this may fail during a virgin startup
                globals.WriteLine(ee.Message);
            }

            AdjustUIForFeatures();

            return changed;

        }
        private void cmbTracks_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedTrack(false);
        }

        private void UpdateDynamicsDataValues()
        {
            if (_dynamicsValuesUpdating)
            {
                return;
            }

            _dynamicsValuesUpdating = true;

            numLateralGForce.Enabled = chkLateralGForce.Checked = _carMetadata.DynamicsData.AnnounceLateralGForce;
            numLinearGForce.Enabled = chkLinearGForce.Checked = _carMetadata.DynamicsData.AnnounceLinearGForce;
            numLateralGForce.Value = (decimal)_carMetadata.DynamicsData.LateralGForceThreshold;
            numLinearGForce.Value = (decimal)_carMetadata.DynamicsData.LinearGForceThreshold;

            numWheelSpeedDiff.Enabled = numBrakePSI.Enabled = chkActiveWheelLockDetection.Checked = _carMetadata.DynamicsData.ActiveWheelLockDetectionEnabled;
            numBrakePSI.Value = _carMetadata.DynamicsData.BrakeThresholdPsi;

            if (_carMetadata.DynamicsData.WheelSpeedPercentDifference < numWheelSpeedDiff.Maximum)
            {
                numWheelSpeedDiff.Value = _carMetadata.DynamicsData.WheelSpeedPercentDifference;
            }

            numAnnounceSpeed.Enabled = chkAnnounceSpeed.Checked = _carMetadata.DynamicsData.AnnounceSpeed;
            numAnnounceSpeed.Value = _carMetadata.DynamicsData.SpeedThreshold;

            cmbBrakeThreshold.SelectedIndex= _carMetadata.DynamicsData.AnnounceBrakeThresholdIndex;
            if (cmbBrakeThreshold.SelectedIndex==0)
            {
                numMaxBrakeHz.Enabled = false;
                numMaxBrakeThreshold.Enabled = false;
                numMinBrakeHz.Enabled = false;
                numMinBrakeThreshold.Enabled = false;
            }
            if (cmbBrakeThreshold.SelectedIndex == 1)
            {
                numMaxBrakeHz.Enabled = true;
                numMaxBrakeThreshold.Enabled = true;
                numMinBrakeHz.Enabled = false;
                numMinBrakeThreshold.Enabled = false;
            }
            if (cmbBrakeThreshold.SelectedIndex == 2)
            {
                numMaxBrakeHz.Enabled = true;
                numMaxBrakeThreshold.Enabled = true;
                numMinBrakeHz.Enabled = true;
                numMinBrakeThreshold.Enabled = true;
            }

            numMaxBrakeThreshold.Value = _carMetadata.DynamicsData.BrakeThresholdMax;
            numMinBrakeThreshold.Value = _carMetadata.DynamicsData.BrakeThresholdMin;
            numMaxBrakeHz.Value = _carMetadata.DynamicsData.BrakeMaxHz;
            numMinBrakeHz.Value = _carMetadata.DynamicsData.BrakeMinHz;

            chkAnnounceBestLap.Checked = _carMetadata.DynamicsData.AnnounceBestLap;
            chkAnnounceLapDelta.Checked = _carMetadata.DynamicsData.AnnounceLapDelta;

            _dynamicsValuesUpdating = false;
        }

        private void DynamicsDataValueChanged(object sender, EventArgs e)
        {
            if (_dynamicsValuesUpdating)
            {
                return;
            }

            _dynamicsValuesUpdating = true;

            if (numMinBrakeThreshold.Value > numMaxBrakeThreshold.Value)
            {
                var v = numMaxBrakeThreshold.Value;
                numMaxBrakeThreshold.Value = numMinBrakeThreshold.Value;
                numMinBrakeThreshold.Value = v;
            }
            if (numMinBrakeHz.Value > numMaxBrakeHz.Value)
            {
                var v = numMaxBrakeHz.Value;
                numMaxBrakeHz.Value = numMinBrakeHz.Value;
                numMinBrakeHz.Value = v;
            }

            numBrakePSI.Enabled = numWheelSpeedDiff.Enabled = _carMetadata.DynamicsData.ActiveWheelLockDetectionEnabled = chkActiveWheelLockDetection.Checked;
            _carMetadata.DynamicsData.AnnounceBestLap = chkAnnounceBestLap.Checked;
            _carMetadata.DynamicsData.AnnounceLapDelta = chkAnnounceLapDelta.Checked;
            numLateralGForce.Enabled = _carMetadata.DynamicsData.AnnounceLateralGForce = chkLateralGForce.Checked;
            numLinearGForce.Enabled = _carMetadata.DynamicsData.AnnounceLinearGForce = chkLinearGForce.Checked;
            numAnnounceSpeed.Enabled = _carMetadata.DynamicsData.AnnounceSpeed = chkAnnounceSpeed.Checked;
            _carMetadata.DynamicsData.BrakeThresholdPsi = (int)numBrakePSI.Value;
            _carMetadata.DynamicsData.LateralGForceThreshold = (double)numLateralGForce.Value;
            _carMetadata.DynamicsData.LinearGForceThreshold = (double)numLinearGForce.Value;
            _carMetadata.DynamicsData.SpeedThreshold = (int)numAnnounceSpeed.Value;
            _carMetadata.DynamicsData.WheelSpeedPercentDifference = (int)numWheelSpeedDiff.Value;
            _carMetadata.DynamicsData.BrakeThresholdMin = (int)numMinBrakeThreshold.Value;
            _carMetadata.DynamicsData.BrakeThresholdMax = (int)numMaxBrakeThreshold.Value;

            _carMetadata.DynamicsData.BrakeMinHz = (int)numMinBrakeHz.Value;
            _carMetadata.DynamicsData.BrakeMaxHz = (int)numMaxBrakeHz.Value;
            _carMetadata.DynamicsData.AnnounceBrakeThresholdIndex = cmbBrakeThreshold.SelectedIndex;

            if (cmbBrakeThreshold.SelectedIndex == 0)
            {
                numMaxBrakeHz.Enabled = false;
                numMaxBrakeThreshold.Enabled = false;
                numMinBrakeHz.Enabled = false;
                numMinBrakeThreshold.Enabled = false;
            }
            if (cmbBrakeThreshold.SelectedIndex == 1)
            {
                numMaxBrakeHz.Enabled = true;
                numMaxBrakeThreshold.Enabled = true;
                numMinBrakeHz.Enabled = false;
                numMinBrakeThreshold.Enabled = false;
            }
            if (cmbBrakeThreshold.SelectedIndex == 2)
            {
                numMaxBrakeHz.Enabled = true;
                numMaxBrakeThreshold.Enabled = true;
                numMinBrakeHz.Enabled = true;
                numMinBrakeThreshold.Enabled = true;
            }
            _carMetadata.Save(_carMetafile);

            _dynamicsValuesUpdating = false;
        }

        private void UpdateEngineDataValues()
        {
            if (_engineValuesUpdating)
            {
                return;
            }

            _engineValuesUpdating = true;

            chkOverRev.Checked = _carMetadata.EngineData.OverRevEnabled;
            chkUpShift.Checked = _carMetadata.EngineData.UpShiftEnabled;
            chkDownShift.Checked = _carMetadata.EngineData.DownShiftEnabled;
            chkOilPressure.Checked = _carMetadata.EngineData.OilPressureEnabled;
            chkTemperature.Checked = _carMetadata.EngineData.TemperatureEnabled;
            chkVoltage.Checked = _carMetadata.EngineData.VoltageEnabled;

            numOverRev.Enabled = chkOverRev.Checked;
            numUpShift.Enabled = chkUpShift.Checked;
            numDownShift.Enabled = chkDownShift.Checked;
            numOilPressurePsi.Enabled = chkOilPressure.Checked;
            numOilPressureRpm.Enabled = chkOilPressure.Checked;
            numTemperature.Enabled = chkTemperature.Checked;
            numVoltage.Enabled = chkVoltage.Checked;

            numOverRev.Value = _carMetadata.EngineData.OverRev;
            numUpShift.Value = _carMetadata.EngineData.UpShift;
            numDownShift.Value = _carMetadata.EngineData.DownShift;
            numOilPressurePsi.Value = _carMetadata.EngineData.OilPressurePsi;
            numOilPressureRpm.Value = _carMetadata.EngineData.OilPressureRpm;
            numTemperature.Value = _carMetadata.EngineData.Temperature;
            numVoltage.Value = (decimal)(_carMetadata.EngineData.Voltage);

            if (_carMetadata.EngineData.EcuName.Length > 2)
            {
                // find the ECU by name lookup
                bool found = false;
                for (int i = 0; i < cmbEcuType.Items.Count; i++)
                {
                    cmbEcuType.SelectedIndex = i;
                    string iname = cmbEcuType.SelectedItem.ToString();
                    iname = iname.ToUpper();
                    if (iname == _carMetadata.EngineData.EcuName.ToUpper())
                    {
                        found = true;
                        break;
                    }

                }

                if (!found)
                {
                    cmbEcuType.SelectedIndex = 0;
                }

            }
            else
            {
                if ((int)_carMetadata.EngineData.EcuType >= cmbEcuType.Items.Count)
                {
                    cmbEcuType.SelectedIndex = 0;
                }
                else
                {
                    cmbEcuType.SelectedIndex = (int)_carMetadata.EngineData.EcuType;
                }
            }
            rdoSpeechNotify.Checked = _carMetadata.EngineData.ShiftNotificationType == ShiftNotificationType.Speech;
            rdoToneNotify.Checked = _carMetadata.EngineData.ShiftNotificationType == ShiftNotificationType.Tone;

            _engineValuesUpdating = false;
        }

        private void EngineDataValueChanged(object sender, EventArgs e)
        {
            if (_engineValuesUpdating)
            {
                return;
            }


            _engineValuesUpdating = true;

            _carMetadata.EngineData.OverRevEnabled = numOverRev.Enabled = chkOverRev.Checked;
            _carMetadata.EngineData.UpShiftEnabled = numUpShift.Enabled = chkUpShift.Checked;
            _carMetadata.EngineData.DownShiftEnabled = numDownShift.Enabled = chkDownShift.Checked;
            _carMetadata.EngineData.OilPressureEnabled = numOilPressurePsi.Enabled = numOilPressureRpm.Enabled = chkOilPressure.Checked;
            _carMetadata.EngineData.TemperatureEnabled = numTemperature.Enabled = chkTemperature.Checked;
            _carMetadata.EngineData.VoltageEnabled = numVoltage.Enabled = chkVoltage.Checked;

            _carMetadata.EngineData.OverRev = (int)numOverRev.Value;
            _carMetadata.EngineData.UpShift = (int)numUpShift.Value;
            _carMetadata.EngineData.DownShift = (int)numDownShift.Value;
            _carMetadata.EngineData.OilPressurePsi = (int)numOilPressurePsi.Value;
            _carMetadata.EngineData.OilPressureRpm = (int)numOilPressureRpm.Value;
            _carMetadata.EngineData.Temperature = (int)numTemperature.Value;
            _carMetadata.EngineData.Voltage = (double)numVoltage.Value;

            _carMetadata.EngineData.EcuType = (EcuType)cmbEcuType.SelectedIndex;
            _carMetadata.EngineData.EcuName = cmbEcuType.SelectedItem.ToString();
            _carMetadata.EngineData.ShiftNotificationType = rdoSpeechNotify.Checked ? ShiftNotificationType.Speech : ShiftNotificationType.Tone;

            AdjustUIForFeatures();

            _carMetadata.Save(_carMetafile);


            _engineValuesUpdating = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splash about_page = new splash(2);
            about_page.setlabel("License# " + globals.theUUID);
            about_page.ShowDialog();
        }

        private void gotoRaceVoiceComToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.racevoice.com");
        }



        private void voiceSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VoiceAdjust va = new VoiceAdjust(_carMetadata);
            va.ShowDialog();
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences pa = new Preferences(_carMetadata);
            pa.ShowDialog();
            _carMetadata.Save(_carMetafile);

        }
        private void dataTraceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                RealTimeView rtv = new RealTimeView();
                rtv.ShowDialog();
            }
            catch
            {
            }
        }


        private void WriteDataToFwTrace()
        {
            try
            {
                rvcom.WriteDataToRaceVoice(_carMetadata, _trackModel, true);
            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
            }
        }




        private void sendConfigButton(object sender, EventArgs e)
        {
            if (globals.iracing_mode)
            {
                if (globals.AllowIracing())
                {
                    try
                    {
                        irace.configure(_carMetadata, _trackMetadata, _trackModel);
                        UpdateSimVolumes();
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.Message);
                    }
                }
                else
                {
                    irace.LicenseMessage(false);
                }
                return;
            }

            WriteDataToFwTrace();
            if (!globals.first_connected)
            {
                InitRaceVoiceHW(false);
            }
            WriteDataToRaceVoice();
        }

        private void getConfigButton(object sender, EventArgs e)
        {
            if (!globals.first_connected)
            {
                InitRaceVoiceHW(false);
            }
            else
            {
                ReadDataFromRaceVoice(false);
            }
        }

        private void lstSplits_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_renderTarget == null)
            {
                return;
            }

            _trackModel.Splits[e.Index].Hidden = e.NewValue != CheckState.Checked;

            ResaveSplitEnabledStates();

            ReRender();
        }


        private void lstSpeechTags_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_renderTarget == null)
            {
                return;
            }

            _trackModel.SpeechTags[e.Index].Hidden = e.NewValue != CheckState.Checked;

            ResaveSpeechTagEnabledStates();

            ReRender();
        }

        private void ResaveSplitEnabledStates()
        {
            _trackMetadata.SplitEnabledStates = new List<bool>(_trackModel.Splits.Count);
            foreach (var t in _trackModel.Splits)
            {
                _trackMetadata.SplitEnabledStates.Add(!t.Hidden);
            }

            _trackMetadata.Save(_trackMetafile);
        }

        private void ResaveSpeechTagEnabledStates()
        {
            _trackMetadata.SpeechTagEnabledStates = new List<bool>(_trackModel.SpeechTags.Count);
            foreach (var t in _trackModel.SpeechTags)
            {
                _trackMetadata.SpeechTagEnabledStates.Add(!t.Hidden);
            }

            _trackMetadata.Save(_trackMetafile);
        }

        private void lstSegments_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_renderTarget == null)
            {
                return;
            }

            _trackModel.Segments[e.Index].Hidden = e.NewValue != CheckState.Checked;
            ReRender();
        }

        private void DataCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (_dataCheckboxesUpdating || _renderer.SelectedSegment == null)
            {
                return;
            }

            UInt16 bitfield = 0;
            for (int i = 0; i < _dataCheckboxes.Length; i++)
            {
                if (_dataCheckboxes[i].CheckState == CheckState.Checked)
                {
                    bitfield = (ushort)(bitfield | 1 << i);
                }
            }

            _renderer.SelectedSegment.DataBits = bitfield;
            _renderer.SelectedSegment.Hidden = !_dataCheckboxes.Any(c => c.Checked);
            _trackMetadata.DataBitfields = _trackModel.Segments.Select(s => s.DataBits).ToList();
            _trackMetadata.Save(_trackMetafile);
            AdjustUIForFeatures();

            ReRender();
        }

        private void gPSSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new GpsSettings(_carMetadata).ShowDialog();
        }

        private void lstSegments_SelectedIndexChanged(object sender, EventArgs e)
        {
            var segment = lstSegments.SelectedIndex != -1 ? _trackModel.Segments[lstSegments.SelectedIndex] : null;
            _renderer.SelectSegment(segment);

            if (_renderer.SelectedSegment == null)
            {
                grpData.Text = "Data";
                grpData.Enabled = false;
            }
            else
            {
                grpData.Enabled = true;
                grpData.Text = "Data - " + _renderer.SelectedSegment.Name;
            }

            ReRender();
            UpdateDataCheckboxes(false);
        }

        private void loadCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                DisplayChart(openFileDialog.FileName);
            }
        }

        private void messageTriggersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new MessageTriggers(_carMetafile, _carMetadata).ShowDialog();
        }

        private void btnSaveTrack_Click(object sender, EventArgs e)
        {
            SaveTrack();
        }

        private void SaveTrack()
        {
            TrackModelParser.SaveFile(_trackFile, _trackModel);
            _trackMetadata.Save(_trackMetafile);
            btnSaveTrack.Enabled = false;
        }

        private void baudRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CanBusSettings(_carMetafile, _carMetadata).ShowDialog();
        }

        private void btnHideAllSplits_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSplits.Items.Count; i++)
            {
                lstSplits.SetItemChecked(i, false);
            }
        }

        private void btnHideAllSpeechTags_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSpeechTags.Items.Count; i++)
            {
                lstSpeechTags.SetItemChecked(i, false);
            }
        }

        private void btnClearAllData_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _dataCheckboxes.Length; i++)
            {
                _dataCheckboxes[i].Checked = false;
            }
            AdjustUIForFeatures();
        }

        private void btnHideAllSegments_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstSegments.Items.Count; i++)
            {
                //lstSegments.SetItemChecked(i, false);
            }
        }

        private void releaseNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splash about_page = new splash(3);
            about_page.setlabel("License# " + globals.theUUID);
            about_page.ShowDialog();
        }



        private void restoreAllTracksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Download all available tracks from the server?", "Restore All Tracks", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                CheckNewTracks(false, true, "");
            }
        }

        private bool GotoTrackName(string track, bool selectit)
        {
            int i;
            bool found = false;
            track = track.ToUpper().Trim(); ;
            for (i = 0; i < cmbTracks.Items.Count; i++)
            {
                string selitem = cmbTracks.Items[i].ToString().ToUpper().Trim();

                if (selitem.Equals(track))
                {
                    if (selectit) cmbTracks.SelectedIndex = i;
                    found = true;
                    break;
                }

            }

            return found;

        }

        private void importTrackMapsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool status = false;
            string track_name = "";
            import aim_import = new import();
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = globals.LocalFolder(),
                Title = "Import Track Data",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "csv",
                Filter = "Tracks (*.csv;*.ztracks)|*.csv;*.ztracks",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };



            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fn = openFileDialog1.FileName;
                string input = Interaction.InputBox("Enter a name for the Imported Track", "Set Track Name", "", -1, -1);
                if (globals.ValidTrackName(input))
                {
                    if (DoesTrackExist(input, true)) return;
                    if (fn.ToUpper().Contains(".ZTRACKS"))
                    {
                        track_name = aim_import.ImportZTRACK(fn, input);
                        if (track_name.Length > 0) status = true;
                    }
                    if (fn.ToUpper().Contains(".CSV"))
                    {
                        track_name = aim_import.ImportRawCSV(fn, input);
                        if (track_name.Length > 0) status = true;
                    }

                    if (status)
                    {
                        MessageBox.Show("Import Success!\r\n" + track_name, "Track Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PopulateTracks();
                        GotoTrackName(track_name, true);
                        QueueTrackForSending(track_name + ".csv");
                        SendNewTracksToServer();
                    }
                    else
                    {
                        MessageBox.Show("Import Failed!", "Track Import", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private bool DoesTrackExist(string track, bool show)
        {
            if (GotoTrackName(track, false))
            {
                if (show) MessageBox.Show("Sorry, that track name already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;

            }

            return false;
        }
        private void cloneCurrentTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string current_track = "";
            current_track = cmbTracks.Text;
            DialogResult dr = MessageBox.Show("Clone Track <" + current_track + "> ?", "Clone", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                string input = Interaction.InputBox("Enter a name for the Cloned Track", "Cloned Track Name", "", -1, -1);
                if (globals.ValidTrackName(input))
                {
                    if (!DoesTrackExist(input, true))
                    {
                        if (globals.CloneTrack(input, current_track))
                        {
                            if (globals.RenameTrackJson(input))
                            {
                                MessageBox.Show("Track has been cloned", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                PopulateTracks();
                                GotoTrackName(input, true);

                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry, that name is not valid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                }
            }
        }



        private void deleteTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string current_track = "";
            current_track = cmbTracks.Text;
            DialogResult dr = MessageBox.Show("Delete Track <" + current_track + "> ?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (DoesTrackExist(current_track, false))
                {
                    if (globals.DeleteTrack(current_track))
                    {
                        MessageBox.Show("Track has been deleted", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        PopulateTracks();
                    }
                    else
                    {
                        MessageBox.Show("Sorry, Delete has failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }

        }

        private void restoreCurrentTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string current_track = "";
            current_track = cmbTracks.Text;
            DialogResult dr = MessageBox.Show("Download Track <" + current_track + "> from the server?", "Restore A Track", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                CheckNewTracks(false, true, globals.ToTrackName(current_track));
            }
        }

     
        private void AdjustUIForFeatures()
        {
            if ((EcuType)cmbEcuType.SelectedIndex == EcuType.IRACING)
            {
                if (globals.AllowIracing())
                {
                    globals.iracing_mode = true;
                }
                else
                {
                    globals.iracing_mode = false;
                    irace.LicenseMessage(false);
                    cmbEcuType.SelectedIndex = (int)EcuType.STANDALONE;
                }
            }
            else globals.iracing_mode = false;

            if (globals.iracing_mode)
            {
                if (globals.last_dash >= 0)
                {
                    if (globals.last_dash!=cmbEcuType.SelectedIndex)
                    {
                        try
                        {
                            irace.configure(_carMetadata, _trackMetadata, _trackModel);
                        }
                        catch (Exception ee)
                        {
                            MessageBox.Show(ee.Message);
                        }
                    }

                }
            }
            globals.last_dash = cmbEcuType.SelectedIndex;


            if (globals.terminal) terminalToolStripMenuItem.Visible = true;
            switch (_carMetadata.EngineData.EcuType)
            {
                case EcuType.SmartyCam1:
                case EcuType.SmartyCam2:
                    splitsBox.Visible = false;
                    wheelLockbox.Visible = false;
                    braketonebox.Visible = false;
                    cmbBrakeThreshold.SelectedIndex = 0;
                    cmbBrakeThreshold.Enabled = false;
                    chkActiveWheelLockDetection.Enabled = false;
                    chkAnnounceBestLap.Enabled = chkAnnounceBestLap.Checked = false;
                    chkAnnounceLapDelta.Enabled = chkAnnounceLapDelta.Checked = false;

                    chkOverRev.Enabled = true;
                    chkTemperature.Enabled = true;
                    chkVoltage.Enabled = true;
                    chkOilPressure.Enabled = true;
                    chkUpShift.Enabled = true;
                    chkDownShift.Enabled = true;

                    break;

                case EcuType.VBOX:
                    splitsBox.Visible = false;
                    wheelLockbox.Visible = false;
                    braketonebox.Visible = false;
                    cmbBrakeThreshold.SelectedIndex = 0;
                    cmbBrakeThreshold.Enabled = false;
                    chkActiveWheelLockDetection.Enabled = false;
                    chkAnnounceBestLap.Enabled = chkAnnounceBestLap.Checked = false;
                    chkAnnounceLapDelta.Enabled = chkAnnounceLapDelta.Checked = false;
                    chkOverRev.Enabled = chkOverRev.Checked = false;
                    chkTemperature.Enabled = chkTemperature.Checked = false;
                    chkVoltage.Enabled = chkVoltage.Checked = false;
                    chkOilPressure.Enabled = chkOilPressure.Checked = false;
                    chkUpShift.Enabled = chkUpShift.Checked = false;
                    chkDownShift.Enabled = chkDownShift.Checked = false;

                    break;
                default:
                    splitsBox.Visible = true;
                    chkAnnounceBestLap.Enabled = true;
                    chkAnnounceLapDelta.Enabled = true;
                    wheelLockbox.Visible = true;
                    braketonebox.Visible = true;
                    cmbBrakeThreshold.Enabled= true;
                    chkActiveWheelLockDetection.Enabled = true;

                    chkOverRev.Enabled = true;
                    chkTemperature.Enabled = true;
                    chkVoltage.Enabled = true;
                    chkOilPressure.Enabled = true;
                    chkUpShift.Enabled = true;
                    chkDownShift.Enabled = true;


                    break;


            }

            if (globals.iracing_mode)
            {
                getConfig.Visible = false;
                DownloadData.Visible = false;
                sendConfig.Text = "Update iRacing";


                chkTemperature.Checked = false;
                chkTemperature.Enabled = false;
                numTemperature.Enabled = false;

                chkVoltage.Checked = false;
                chkVoltage.Enabled = false;
                numVoltage.Enabled = false;

                chkOilPressure.Enabled = false;
                chkOilPressure.Checked = false;
                numOilPressurePsi.Enabled = false;
                numOilPressureRpm.Enabled = false;

                chkActiveWheelLockDetection.Checked = false;
                chkActiveWheelLockDetection.Enabled = false;
                numBrakePSI.Enabled = false;
                numWheelSpeedDiff.Enabled = false;

                chkAnnounceLapDelta.Enabled = false;
                chkAnnounceLapDelta.Checked = false;
                rdoSpeechNotify.Checked = true;
                rdoToneNotify.Checked = false;
                rdoToneNotify.Enabled = false;


            }
            else
            {
                getConfig.Visible = true;
                DownloadData.Visible = true;
                sendConfig.Text = "Send Configuration";


                chkTemperature.Enabled = true;
                numTemperature.Enabled = true;
                chkVoltage.Enabled = true;
                numVoltage.Enabled = true;

                chkOilPressure.Enabled = true;
                numOilPressurePsi.Enabled = true;
                numOilPressureRpm.Enabled = true;

                chkActiveWheelLockDetection.Enabled = true;
                numBrakePSI.Enabled = true;
                numWheelSpeedDiff.Enabled = true;

                chkAnnounceLapDelta.Enabled = true;
                rdoToneNotify.Enabled = true;


            }


        }

        private void clearDataLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool valid = false;
            racevoicecom rvcom = new racevoicecom();
            if (!globals.IsRaceVoiceConnected())
            {
                return;
            }

            if (rvcom.OpenSerial())
            {
                if (rvcom.WriteSingleCmd("FLASH ERASE")) valid = true;
                rvcom.CloseSerial();
            }

            if (valid)
            {
                MessageBox.Show("Data Log Has Been Erased", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("Data Log Erase Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void enableDataLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool valid = false;
            racevoicecom rvcom = new racevoicecom();
            if (!globals.IsRaceVoiceConnected())
            {
                return;
            }

            if (rvcom.OpenSerial())
            {
                if (rvcom.WriteSingleCmd("FLASH RECORD ENABLE")) valid = true;
                rvcom.CloseSerial();
            }

            if (valid)
            {
                MessageBox.Show("Data Log Has Been Enabled", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                MessageBox.Show("Data Log Failed To Enable", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void readDataLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            racevoicecom rvcom = new racevoicecom();
            if (!globals.IsRaceVoiceConnected())
            {
                return;
            }

            if (rvcom.OpenSerial())
            {
                tracer trace = new tracer();
                trace.Show();
                Thread.Sleep(1000);
                rvcom.WriteSingleCmd("FLASH RESET");
                Thread.Sleep(1000);
                rvcom.WriteSingleCmd("FLASH READ");
                try
                {
                    while (true)
                    {
                        string line = rvcom.ReadLine();
                        globals.WriteLine(line);
                        Thread.Sleep(10);
                        Application.DoEvents();
                        trace.addline(line);
                        if (line.ToUpper().Contains("COMPLETE"))
                        {
                            MessageBox.Show("Data Log Has Been Read Back", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    }
                }
                catch (Exception ee)
                {
                    globals.WriteLine(ee.Message);
                    MessageBox.Show("Data Log Failed To Read", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                rvcom.CloseSerial();
            }

        }

        private void firmwareUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            firmware updater = new firmware();
            int update_stat = 0;
            if (ReadDataFromRaceVoice(true)) // get the version from the unit
            {
                _carMetadata.Save(_carMetafile); // save what we read ... should jsut be version and name
                updater.ComparePackagedFirmwareVersion(_carMetadata);
                update_stat = updater.UpdateFirmware();
                if (update_stat < 0)
                {
                    globals.Terminate();

                }
                if (update_stat == 1)
                {
                    ReadDataFromRaceVoice(true); // get the new version
                }
            }
        }


        private void DriverUpdate()
        {

        }

        bool DoesDriverExist(string drivername)
        {
            bool found = false;
            Console.WriteLine("Searching for driver...");

            System.Management.SelectQuery query = new System.Management.SelectQuery("Win32_SystemDriver");
            query.Condition = "Name = 'SomeDriverName'";
            System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(query);
            var drivers = searcher.Get();

            if (drivers.Count > 0) Console.WriteLine("Driver exists.");
            else Console.WriteLine("Driver could not be found.");

            Console.ReadLine();

            return found;
        }

        private void DriverInstall(string driverPath)
        {

            var process = new System.Diagnostics.Process();
            string infrun = "";
            string windows_path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).ToString();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = "cmd.exe";

            infrun = "/c C:\\Windows\\System32\\InfDefaultInstall.exe " + driverPath;
            globals.WriteLine("InstallDriver->" + infrun);
            process.StartInfo.Arguments = infrun;
            process.Start();
            process.WaitForExit();
            process.Dispose();

        }

        private void installUSBDriversToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = globals.LocalFolder() + "\\drivers\\ftdibus.inf";
            DriverInstall(path);
            path = globals.LocalFolder() + "\\drivers\\ftdiport.inf";
            DriverInstall(path);
            MessageBox.Show("Driver has been installed.\r\n Please disconnect and reconnect the RaceVoice USB\r\nYour PC should then find RaceVoice.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

#endif

#if APP
        public int GetRenderView(int mode)
        {
            return _renderer.GetRenderView(mode);
        }
        public string ReRender(SkiaSharp.SKCanvas canvas, SkiaSharp.SKImageInfo info)
        {
            _renderer.Render(canvas, info);
            return _renderer.debug;
        }

#else
        private void ReRender()
        {
            if (_renderer == null) return;
            btnSaveTrack.Enabled = true;

            SetZoomRotateOffsetValues();

            _renderer.Render(_renderTarget);

            TrackView.Image = _renderTarget;
        }
#endif

        private static string GetMetaFileForCsv(string filepath)
        {
            return filepath.Replace(".csv", METADATA_EXTENSION);
        }


        private TrackMetadata CheckTrackMetaData(string filename, string metaFile, TrackMetadata metadata, TrackModel trackModel)
        {
#if !APP
            bool no_json = false;
            if (metadata == null)
            {
                no_json = true;
                metadata = new TrackMetadata()
                {
                    TrackName = Path.GetFileNameWithoutExtension(filename).Replace('_', ' '),
                    DataBitfields = new List<UInt16>(trackModel.Segments.Count),
                    ClusterSize = trackModel.SampleRate == 0 ? 4 : trackModel.SampleRate
                };


                foreach (var segment in trackModel.Segments)
                {
                    metadata.DataBitfields.Add(0);
                }

                metadata.Save(metaFile);
            }


            // ensure we always have bitfields for the segments
            if (trackModel.Segments.Count() != metadata.DataBitfields.Count())
            {
                int i = 0;
                foreach (var segment in trackModel.Segments)
                {
                    if (i >= metadata.DataBitfields.Count())
                    {
                        metadata.DataBitfields.Add(0);
                    }

                    i++;

                }
                metadata.Save(metaFile);
            }


            if (metadata.SpeechTagEnabledStates == null || metadata.SplitEnabledStates.Count != trackModel.SpeechTags.Count)
            {
                int counts = 0;
                if (trackModel.SpeechTags.Count==0)
                {
                    metadata.SpeechTagEnabledStates = new List<bool>(1);
                    counts = 1;
                }
                else
                {
                    counts = trackModel.SpeechTags.Count();
                    if (metadata.SpeechTagEnabledStates == null)
                    {
                        metadata.SpeechTagEnabledStates = new List<bool>(counts);
                    }
                    else
                    {
                        if (metadata.SpeechTagEnabledStates.Count() == 0)
                        {
                            metadata.SpeechTagEnabledStates = new List<bool>(trackModel.SpeechTags.Count);

                        }
                    }
                }

                for (int i = 0; i < counts; i++)
                {
                    if (no_json || trackModel.SpeechTags.Count==0)
                    {

                        metadata.SpeechTagEnabledStates.Add(false);
                    }
                    if ((i+1)>trackModel.SpeechTags.Count)
                    {
                        continue;
                    }
                    trackModel.SpeechTags[i].Hidden = !metadata.SpeechTagEnabledStates[i];
                }

                metadata.Save(metaFile);
            }


            if (metadata.SplitEnabledStates == null || metadata.SplitEnabledStates.Count != trackModel.Splits.Count)
            {
                metadata.SplitEnabledStates = new List<bool>(trackModel.Splits.Count);
                for (int i = 0; i < trackModel.Splits.Count; i++)
                {
                    if (no_json)
                    {

                        metadata.SplitEnabledStates.Add(false);
                    }
                    else

                        metadata.SplitEnabledStates.Add(!trackModel.Splits[i].Hidden);
                }

                metadata.Save(metaFile);
            }

            cmbTracks.Items.Add(new ComboBoxItem<string>() { Text = globals.FixName(metadata.TrackName,false), Value = filename });
            return metadata;
#else
            //bool no_json = false;
            if (metadata == null)
            {
                //no_json = true;
                metadata = new TrackMetadata()
                {
                    //                         TrackName = Path.GetFileNameWithoutExtension(f).Replace('_', ' '),
                    //                            DataBitfields = new List<UInt16>(trackModel.Segments.Count),
                    //                           ClusterSize = trackModel.SampleRate == 0 ? 4 : trackModel.SampleRate
                };


                // foreach (var segment in trackModel.Segments)
                //{
                //   metadata.DataBitfields.Add(0);
                //}

                //metadata.Save(metaFile);
            }

            return metadata;
#endif
        }





        public void LoadTrackFile(string filename)
        {
            globals.WriteLine("Load Track:" + filename);

#if (!APP)
            grpData.Enabled = false;
            grpData.Text = "Data";
#endif
            _trackMetafile = GetMetaFileForCsv(filename);
            _trackMetadata = TrackMetadata.Load(_trackMetafile);
            _trackModel = TrackModelParser.LoadFile(filename);
#if (APP)
            _trackMetadata = CheckTrackMetaData(filename, _trackMetafile, _trackMetadata, _trackModel);
            // _trackMetadata=CheckTrackMetaData(_trackMetadata);
#endif
            TrackRenderer.SmoothTrack(_trackModel, 10);
#if (!APP)

            for (int i=0; i<_trackMetadata.SpeechTagEnabledStates.Count; i++)
            {
                if (_trackModel.SpeechTags.Count > 0)
                {
                    if ((i + 1) <= _trackModel.SpeechTags.Count)
                    {
                        _trackModel.SpeechTags[i].Hidden = !_trackMetadata.SpeechTagEnabledStates[i];
                    }
                }
            }
            for (int i = 0; i < _trackMetadata.SplitEnabledStates.Count; i++)
            {
                _trackModel.Splits[i].Hidden = !_trackMetadata.SplitEnabledStates[i];
            }

            for (int i = 0; i < _trackMetadata.DataBitfields.Count; i++)
            {
                UInt16 bits = _trackMetadata.DataBitfields[i];
                // you may have added extra segments and json control variables
                // if the track is restored and there are LESS segments, we need to handle that case
                if (i < _trackModel.Segments.Count())
                {
                    _trackModel.Segments[i].DataBits = bits;
                    _trackModel.Segments[i].Hidden = bits == 0;
                }
            }
            _trackModel.CalculateDistances(globals.MAX_SEGMENTS,globals.MAX_SPLITS,globals.MAX_SPEECH_TAGS);
#endif
            var rendererSettings = new TrackRendererSettings(_trackMetadata.ClusterSize)
            {
                BackgroundColor = Color.White,
                DefaultSegmentColor = Color.CornflowerBlue,
                MouseIndicatorColor = Color.Orange,
                MouseIndicatorSize = 10,
                SegmentResizeHandleColor = Color.DarkGreen,
                SegmentResizeHandleSize = 16,
                SelectedSegmentColor = Color.LawnGreen,
                SegmentThickness = 10,
#if (!APP)
                SegmentFont = new Font("Consolas", 16, FontStyle.Bold, GraphicsUnit.Pixel),
                SegmentLabelColor = Color.Black,
                SplitFont = new Font("Consolas", 16, GraphicsUnit.Pixel),
                SpeechTagFont = new Font("Consolas", 16, GraphicsUnit.Pixel),
                TrackThickness = 4,
#else
                TrackThickness = 5,
#endif
                SplitIndicatorColor = Color.Blue,
                SpeechTagIndicatorColor = Color.BlueViolet,
                InactiveColor = Color.Gray,
                SplitIndicatorSize = 20,
                SplitIndicatorThickness = 4,
                SpeechTagIndicatorSize = 20,
                SpeechTagIndicatorThickness = 4,
                TrackColor = Color.Black,
                ChequeredFlagImage = globals.LocalFolder() + "\\flag.png",
                carImage = globals.LocalFolder() + "\\car.png",

                UseCurveRendering = _trackMetadata.UseCurveRendering,
                ShowGpsPoints = true,
                GpsPointSize = 3,
                GpsPointColor = Color.Yellow
            };

            _renderer = new TrackRenderer(_trackModel, rendererSettings);

#if (!APP)
            _renderTarget = new Bitmap(TrackView.Width, TrackView.Height);
            UpdateSplits();
            UpdateSpeechTags();
            UpdateSegments();
            UpdateEngineDataValues();
            UpdateDynamicsDataValues();
            UpdateDataCheckboxes(true);

            zoom.Value = Math.Max(zoom.Minimum, Math.Min(zoom.Maximum, _trackMetadata.Zoom));
            rotation.Value = Math.Max(rotation.Minimum, Math.Min(rotation.Maximum, _trackMetadata.Rotation));
            hScroll.Value = Math.Max(hScroll.Minimum, Math.Min(hScroll.Maximum, _trackMetadata.XPan));
            vScroll.Value = Math.Max(vScroll.Minimum, Math.Min(vScroll.Maximum, _trackMetadata.YPan));
            _renderer.Zoom = (float)zoom.Value / 100;

            _trackFile = filename;
            ReRender();
            globals.last_dash = (int)_carMetadata.EngineData.EcuType;
            btnSaveTrack.Enabled = false;
            globals.AllowIracing();
            irace.LicenseMessage(false);
            if (globals.AllowIracing())
            { 
                try
                {
                    if (globals.iracing_mode)
                    {
                        irace.configure(_carMetadata, _trackMetadata, _trackModel);
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }

                try
                {
                    irace.startit();
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
            else
            {
                if ((EcuType)cmbEcuType.SelectedIndex == EcuType.IRACING)
                {
                    cmbEcuType.SelectedIndex = (int)EcuType.STANDALONE;
                }

            }


#endif
        }


        public void DisplayChart(string filename)
        {
            string dirname = "charts";

#if APP
            dirname = globals.LocalFolder() + "//charts";
#endif
            if (!Directory.Exists(dirname))
            {
                Directory.CreateDirectory(dirname);
            }
            IList<LapDataTrace> trace = null;
            string firstLine = null;
            using (StreamReader sr = new StreamReader(filename))
            {
                firstLine = sr.ReadLine().ToLower();
            }

            if (firstLine.Contains("aim csv file"))
            {
                trace = AimCsv.LoadCsvFile(filename).GetDataTrace();
            }
            else if (firstLine.Contains("motec csv file"))
            {
                trace = MoTecCsv.LoadCsvFile(filename).GetDataTrace();
            }
            else
            {
                trace = RaceVoiceCsv.LoadCsvFile(_trackModel, filename).GetDataTrace();
            }

            Charting.GenerateChartBundle(trace, _trackModel, globals.LocalFolder() + "//charts");

#if !APP
            webCharts.Refresh(WebBrowserRefreshOption.Completely);
            webTables.Refresh(WebBrowserRefreshOption.Completely);
#endif
        }

        public bool ReadDataFromRaceVoice(bool version_only)
        {
            if (!globals.IsRaceVoiceConnected())
            {
#if !APP
                AdjustUIForFeatures();
#endif
                return false;
            }

            rvcom.OpenSerial();
            // the code here will read data from the unit and it will need to populat the global structure
            // to configure the UI based on the values read back from the unit
            rvcom.Bar(0);
            rvcom.Bar(150);
            rvcom.SendSerial("\r");
            rvcom.SendSerial("\r");
            if (version_only)
            {
                rvcom.SendSerial("SHOW VERSION", _carMetadata, _trackModel);
                rvcom.CloseSerial();
                rvcom.Bar(0);
#if !APP
                UpdateTitle();
#endif
            }
            else
            {
                rvcom.SendSerial("show settings", _carMetadata, _trackModel);
#if !APP
                if (ShowSelectedTrack(true)) // track has changed, so reload the data one more time so we make sure we are in sync
                {
                    rvcom.SendSerial("show settings", _carMetadata, _trackModel);
                }
#endif
                rvcom.CloseSerial();
#if !APP
                UpdateEngineDataValues();
                UpdateDynamicsDataValues();
                UpdateTitle();
                UpdateSegments();
                UpdateSplits();
                UpdateSpeechTags();
                ReRender();
#endif
                rvcom.Bar(0);
            }
#if !APP
            AdjustUIForFeatures();
#endif
            return true;
        }

        public string DownloadDataFromRaceVoice()
        {
            string file = "";
            bool error = false;
            if (!globals.IsRaceVoiceConnected()) return "";
            rvcom.OpenSerial();
            rvcom.Bar(0);
            rvcom.Bar(200);
            rvcom.SendSerial("SHOW VERSION", _carMetadata, _trackModel);
#if (!APP)
            if (_carMetadata.HardwareData.Version.ToUpper().Contains("RACEVOICE-SA"))
            {
                file = rvcom.DownloadData();
                if (file.Contains("NODATA"))
                {
                    MessageBox.Show("No Data Is Available For Download", "Complete", MessageBoxButtons.OK, MessageBoxIcon.None);
                    error = true;

                }
                if (file.Length == 0)
                {
                    MessageBox.Show("Data Download Error", "Complete", MessageBoxButtons.OK, MessageBoxIcon.None);
                    error = true;

                }
                if (!error)
                {
                    MessageBox.Show("Success: Data Download Finished", "Complete", MessageBoxButtons.OK, MessageBoxIcon.None);
                }

            }
            else
            {
                MessageBox.Show("RaceVoice-DI does not support data logging.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#else
            if (_carMetadata.HardwareData.Version.ToUpper().Contains("RACEVOICE-SA"))
            {
                file = rvcom.DownloadData();
                if (file.Contains("NODATA"))
                {
                    //MessageBox.Show("No Data Is Available For Download", "Complete", MessageBoxButtons.OK, MessageBoxIcon.None);
                    error = true;
                }
            }

#endif
            rvcom.CloseSerial();
            rvcom.Bar(0);

            return file;
        }

        public bool WriteDataToRaceVoice()
        {
            bool valid = true;
            if (globals.no_unit_check) return true;

            if (!globals.IsRaceVoiceConnected()) return false;
            rvcom.OpenSerial();
            rvcom.Bar(0);
            rvcom.Bar(200);
            rvcom.SendSerial("SHOW VERSION", _carMetadata, _trackModel);
            valid = rvcom.WriteDataToRaceVoice(_carMetadata, _trackModel, false);
            rvcom.CloseSerial();
            rvcom.Bar(0);

#if !APP
            if (valid)
            {
                ReadDataFromRaceVoice(false); // read everything back
                MessageBox.Show("Success: RaceVoice has been updated", "Complete", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            else
            {
                MessageBox.Show("ERROR: RaceVoice update has failed\r\n", "Communication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
            return valid;
        }

        private void DownloadData_Click(object sender, EventArgs e)
        {
#if (!APP)
            if (!globals.first_connected)
            {
                InitRaceVoiceHW(false);
            }

#endif
            DownloadDataFromRaceVoice();
        }

#if (!APP)
        private void terminalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Terminal term = new Terminal();
            term.ShowDialog();
        }

        private void addSpeechTagMenuItem_Click(object sender, EventArgs e)
        {
            new SpeechTagDialogue((result) =>
            {
                if (result.Add)
                {
                    _trackModel = _renderer.CreateSpeechTagAtHighlightedPosition(result.Name, result.Speech);

                    ReRender();
                    UpdateSpeechTags();
                    ResaveSpeechTagEnabledStates();
                    SaveTrack();
                }
            }).ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveWindowState(this);
            Environment.Exit(0);
        }

        private void heartbeat_Tick(object sender, EventArgs e)
        {
            if (globals.AllowIracing())
            {
                if (globals.iracing_telemetry)
                {
                    if ((EcuType)cmbEcuType.SelectedIndex != EcuType.IRACING)
                    {
                        cmbEcuType.SelectedIndex = (int)EcuType.IRACING;
                    }
                }
                if (globals.iracing_mode)
                {
                    progressBar1.Value = globals.irace_hb;
                }
            }

            if (globals.irace_track_distance>0)
            {
                ReRender();
            }
        }

        private void licenseRenewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            irace.Renew();
        }

        private void licenseCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            irace.LicenseMessage(true);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void licenseHideWarnings_Click(object sender, EventArgs e)
        {
            if (licenseHideWarnings.Checked)
            {
                _carMetadata.HardwareData.HideWarnings = "YES";
                globals.license_hide_warnings = true;
                _carMetadata.Save(_carMetafile);

            }
            else
            {
                _carMetadata.HardwareData.HideWarnings = "NO";
                globals.license_hide_warnings = false;
                _carMetadata.Save(_carMetafile);

            }
        }

        private void CanCapture_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("For the CANBus capture to work, make sure the BLUE Led is flashing\r\nOn your RaceVoice Unit before pressing 'YES'\r\n", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    if (!globals.IsRaceVoiceConnected())
                    {
                        return;
                    }
                    if (rvcom.OpenSerial())
                    {
                        rvcom.CollectCanbus();
                        rvcom.CloseSerial();
                        MessageBox.Show("CANBus capture is complete\r\n", "Capture", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error During Capture");
            }

        }

        private void UpdateSimVolumes()
        {
            audiomixing mixer = new audiomixing();
            mixer.UpdateVolumes(_carMetadata);
        }
        
        private void audioMixerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            audiomixing mixer = new audiomixing();
            mixer.SetData(_carMetadata);
            mixer.Show();
            mixer.UpdateVolumes(_carMetadata);
            _carMetadata.Save(_carMetafile);
            UpdateSimVolumes();


        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckNewTracks(true, false, "");
        }
    }
#endif

    }
#if APP
}
#endif
