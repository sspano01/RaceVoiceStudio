using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using RaceVoiceLib.Parser;
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
            _carMetafile=globals.LocalFolder()+"//car.json";
            _carMetadata = CarMetadata.Load(_carMetafile);
            rvcom = new racevoicecom();

        }

        public int GetComBarValue()
        {
            if (rvcom!=null)
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
            //string oname ="mugello";
            //aim_import.ImportZTRACK(file,oname);
            //aim_import.Parse(true);

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
                    metadata = CheckTrackMetaData(f,metaFile,metadata,trackModel);
/*
                    if (metadata == null)
                    {
                        no_json = true;
                        metadata = new TrackMetadata()
                        {
                            TrackName = Path.GetFileNameWithoutExtension(f).Replace('_', ' '),
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
                    if (trackModel.Segments.Count()!=metadata.DataBitfields.Count())
                    {
                        int i = 0;
                        foreach (var segment in trackModel.Segments)
                        {
                            if (i>=metadata.DataBitfields.Count())
                            {
                             metadata.DataBitfields.Add(0);
                            }
                            
                            i++;
                            
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

                    cmbTracks.Items.Add(new ComboBoxItem<string>() { Text = metadata.TrackName, Value = f });
                    */
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


        private bool DecodeLicense()
        {
            bool valid = false;
            string local_license = EncodeLicense(true);
            string local_key = _carMetadata.HardwareData.LicenseState.Trim().ToUpper();
            local_license = local_license.Trim().ToUpper();
            if (local_license == local_key) valid = true;
            return valid;
        }

        private string EncodeFeature(string feature)
        {
            return HardwareInfo.GenerateUID(feature);
        }

        private void DecodeFeature()
        {
            string key_lite = EncodeFeature("VALID LITE");
            string key_full = EncodeFeature("VALID");
            string key_demo = EncodeFeature("DEMO");
            globals.license_feature = 0;
            string local_key = _carMetadata.HardwareData.FeatureCode.Trim().ToUpper();

            if (local_key.Contains("NONE"))
            {
                globals.license_feature = (int)globals.FeatureState.FULL; // assume we have full version
                
            }
            if (local_key.Contains(key_demo)) globals.license_feature = (int)globals.FeatureState.DEMO; // demo version
            if (local_key.Contains(key_full)) globals.license_feature = (int)globals.FeatureState.FULL; // full version
            if (local_key.Contains(key_lite)) globals.license_feature = (int)globals.FeatureState.LITE; // lite version


        }
        private string EncodeLicense(bool state)
        {
            string enc = "";
            if (state == false) return "*NOLICENSE*";
            enc= HardwareInfo.GenerateUID("#VALID#LICENSE#");
            return enc;
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

        private void CheckNewTracks(bool pushversions,bool force,string download_name)
        {

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

            isplash.setbar(30);
            isplash.setlabel("Communicating ....");
            ping_good = globals.IsOnlineTest();
            if (ping_good || globals.no_license_check)
            {
                globals.network_ok = true;
                isplash.setbar(60);
                isplash.setlabel("Checking License ....");
                if (!CheckLicensedUser() && !DecodeLicense())
                {
                    _carMetadata.HardwareData.LicenseState = EncodeLicense(false);
                    _carMetadata.Save(_carMetafile);
                    isplash.Close();
                    globals.Terminate();
                    
                    return;
                }

                _carMetadata.HardwareData.LicenseState = EncodeLicense(true);
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
                    // now also add firmware and exe local hashes into this mix
                    //path = globals.LocalFolder() + "\\racevoice.exe";
                    //localtracks[li] = "racevoice.exe,"+globals.CalculateMD5(path);
                    //li++;
                    //path = globals.LocalFolder() + "\\firmware.hex";
                    //localtracks[li] = "firmware.hex," + globals.CalculateMD5(path);
                    //li++;
                }
                else
                {
                    isplash.Close();
                }


            }
            else
            {
                globals.network_ok = false;
                // ping test failed, so check to see if we are licensed locally
                if (!DecodeLicense())
                {
                    MessageBox.Show("License Registration Failed.\r\nThis PC is not valid.\r\nPlease contact support@racevoice.com", "License Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    _carMetadata.Save(_carMetafile);
                    globals.Terminate();


                }
            }

            DecodeFeature(); // regardless of how we got the license, we need to now decode the feature code in the license file


            string[] remotes;
            string[] locals;
            bool found = false;
            bool newer = false;
           // any changes or new tracks??
            string track_stat = "";
            string track_msg = "Updates Are Available - Press YES to Update RaceVoice Studio";
            found = newer = false;
            for (int i=0;i<ri;i++)
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
                            if (!globals.FileModifiedNewer(locals[0], locals[2], remotes[2],locals[1],remotes[1]))
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

                if (track_stat.Length==0)
                {
                    MessageBox.Show("The currently selected track is not available on the server", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


            if (track_stat.Length > 0)
            {
                DialogResult dr = FlexibleMessageBox.Show(track_stat, track_msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // DialogResult dr = MessageBox.Show(track_stat, track_msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            if (globals.license_feature == (int)globals.FeatureState.LITE) MainFormText += "Lite ";
            else if (globals.license_feature == (int)globals.FeatureState.DEMO) MainFormText += "Demo Mode "; else MainFormText += "Pro ";

            MainFormText += "Version "+ globals.UIVersion;
            if (globals.no_track_check) debug += " DEBUG:NO TRACK CHECKING";
            if (globals.no_unit_check) debug += " DEBUG:NO UNIT COMMUNICATION";
            /*
            if (uuidlabel.Text.Length == 0)
            {
                uuidlabel.Text = "SN:" + HardwareInfo.GenerateUID("RACEVOICE");
                uuidlabel.ReadOnly = true;
                uuidlabel.BorderStyle = 0;
                uuidlabel.BackColor = this.BackColor;
                uuidlabel.TabStop = false;
            }
            */

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


        private bool CheckLicensedUser()
        {
            bool state = false;
            if (globals.all_stop) return false;

            globals.theUUID = HardwareInfo.GenerateUID("RACEVOICE");
            globals.WriteLine("UUID==" + globals.theUUID);
            sqldatabase sql = new sqldatabase();
            if (globals.no_license_check)
            {
                globals.license_state = "VALID SUPER USER";
                return true;
            }

           state=sql.ValidateUUID(globals.theUUID,false,_carMetadata);
            if (state)
            {
                if (globals.license_state.Contains("RELOAD"))
                {
                    state = sql.ValidateUUID(globals.theUUID, false, _carMetadata); // re-read the license state after the first registration
                }
                sql.ValidateUUID(globals.theUUID, true,_carMetadata);
                _carMetadata.HardwareData.FeatureCode=EncodeFeature(globals.license_state);
            }
            return state;

            //MessageBox.Show(local_uuid);

            // return isgood;
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

            if (globals.thePort.Length == 0 || globals.IsDemoMode(false))
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
                        if (update_stat==1)
                        {
                            ReadDataFromRaceVoice(true); // get the new version

                        }
                    }

                }
            }
            CheckNewTracks(true, false, "");
            UpdateTitle();

        }

      
        private void MainForm_Load(object sender, EventArgs e)
        {
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

            ecuMetadata.PopulateECU(cmbEcuType);
            _carMetadata = CarMetadata.Load(_carMetafile);
            if (_carMetadata.HardwareData.Trace >= 1) globals.trace = true;

            CheckNewTracks(false,false, "");
            PopulateTracks();
            SendNewTracksToServer();

            rvcom = new racevoicecom();
            rvcom.SetBar(progressBar1);
            InitRaceVoiceHW(true);

            AdjustUIForFeatures();

            var chartUrl = "file:///" + globals.LocalFolder() + "/charts/index.html";
            var tablesUrl = "file:///" + globals.LocalFolder() + "/charts/tables.html";

            webCharts.Navigate(chartUrl);
            webTables.Navigate(tablesUrl);
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

        private void UpdateDataCheckboxes()
        {
            _dataCheckboxesUpdating = true;
            UInt16 selectedBits = _renderer.SelectedSegment != null ? _renderer.SelectedSegment.DataBits : (UInt16)0;
            for (int i = 0; i < _dataCheckboxes.Length; i++)
            {
                _dataCheckboxes[i].Checked = (selectedBits & (1 << i)) != 0;
            }
            AdjustUIForFeatures();
            _dataCheckboxesUpdating = false;
        }

      

        private void TrackView_MouseDown(object sender, MouseEventArgs e)
        {
            if (!FeatureAllowed()) return;
            if (_renderer == null) return;

            if (_renderer.HandleMouseDown(sender, e))
            {
                ReRender();
            }
        }

        private void TrackView_MouseUp(object sender, MouseEventArgs e)
        {
            if (!FeatureAllowed()) return;
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
                MessageBox.Show("Sorry, the maximum number of segments allowed is " + globals.MAX_SEGMENTS+"\r\nTry changing the map to reduce the number of segments.", "Too Many Segments", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                }
                else
                {
                    MessageBox.Show("You cannot create a new segment that overlaps an existing segment.\r\n","Segment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            }
        }

        private void btnSegmentDelete_Click(object sender, EventArgs e)
        {
            if (lstSegments.SelectedIndex > -1)
            {
                _trackModel = _renderer.DeleteSegment(_trackModel.Segments[lstSegments.SelectedIndex]);
                ReRender();
                UpdateSegments();
            }
        }


        private void deleteSelectedSegmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_renderer.SelectedSegment != null)
            {
                _renderer.DeleteSegment(_renderer.SelectedSegment);
                ReRender();
                UpdateSegments();
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
            if (from_index)
            {
                // this is when we are reading data back from the unit itself
                // find by name?
                if (_carMetadata.EngineData.FindTrackByName)
                {
                    _carMetadata.EngineData.FindTrackByName = false;
                    if (GotoTrackName(_carMetadata.EngineData.TrackSelectionName,true)) got_name = true;
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

            numMaxBrakeThreshold.Enabled = numMinBrakeThreshold.Enabled = chkBrakeThreshold.Checked = _carMetadata.DynamicsData.AnnounceBrakeThreshold;
            numMaxBrakeThreshold.Value = _carMetadata.DynamicsData.BrakeThresholdMax;
            numMinBrakeThreshold.Value = _carMetadata.DynamicsData.BrakeThresholdMin;

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

            numBrakePSI.Enabled = numWheelSpeedDiff.Enabled = _carMetadata.DynamicsData.ActiveWheelLockDetectionEnabled = chkActiveWheelLockDetection.Checked;
            _carMetadata.DynamicsData.AnnounceBestLap = chkAnnounceBestLap.Checked;
            _carMetadata.DynamicsData.AnnounceLapDelta = chkAnnounceLapDelta.Checked;
            numLateralGForce.Enabled = _carMetadata.DynamicsData.AnnounceLateralGForce = chkLateralGForce.Checked;
            numLinearGForce.Enabled = _carMetadata.DynamicsData.AnnounceLinearGForce = chkLinearGForce.Checked;
            numAnnounceSpeed.Enabled = _carMetadata.DynamicsData.AnnounceSpeed = chkAnnounceSpeed.Checked;
            numMinBrakeThreshold.Enabled = numMaxBrakeThreshold.Enabled = _carMetadata.DynamicsData.AnnounceBrakeThreshold = chkBrakeThreshold.Checked;
            _carMetadata.DynamicsData.BrakeThresholdPsi = (int)numBrakePSI.Value;
            _carMetadata.DynamicsData.LateralGForceThreshold = (double)numLateralGForce.Value;
            _carMetadata.DynamicsData.LinearGForceThreshold = (double)numLinearGForce.Value;
            _carMetadata.DynamicsData.SpeedThreshold = (int)numAnnounceSpeed.Value;
            _carMetadata.DynamicsData.WheelSpeedPercentDifference = (int)numWheelSpeedDiff.Value;
            _carMetadata.DynamicsData.BrakeThresholdMin = (int)numMinBrakeThreshold.Value;
            _carMetadata.DynamicsData.BrakeThresholdMax = (int)numMaxBrakeThreshold.Value;


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
            numTemperature.Value =_carMetadata.EngineData.Temperature;
            numVoltage.Value = (decimal)(_carMetadata.EngineData.Voltage);

            if (_carMetadata.EngineData.EcuName.Length > 2)
            {
                // find the ECU by name lookup
                bool found = false;
                for (int i=0;i<cmbEcuType.Items.Count;i++)
                {
                    cmbEcuType.SelectedIndex = i;
                    string iname = cmbEcuType.SelectedItem.ToString();
                    iname = iname.ToUpper();
                    if (iname==_carMetadata.EngineData.EcuName.ToUpper())
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
            if (globals.IsDemoMode(true)) return;

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
            if (globals.IsDemoMode(true)) return;
                
            WriteDataToFwTrace();
            if (!globals.first_connected)
            {
                InitRaceVoiceHW(false);
            }
            WriteDataToRaceVoice();
        }

        private void getConfigButton(object sender, EventArgs e)
        {
            if (globals.IsDemoMode(true)) return;
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

        private void ResaveSplitEnabledStates()
        {
            _trackMetadata.SplitEnabledStates = new List<bool>(_trackModel.Splits.Count);
            foreach (var t in _trackModel.Splits)
            {
                _trackMetadata.SplitEnabledStates.Add(!t.Hidden);
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
            UpdateDataCheckboxes();
        }

        private void loadCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();

            if(result == DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
            {
                DisplayChart(openFileDialog.FileName);
            }
        }
        
        private void messageTriggersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!FeatureAllowed()) return;
            new MessageTriggers(_carMetafile, _carMetadata).ShowDialog();
        }

        private void btnSaveTrack_Click(object sender, EventArgs e)
        {
            TrackModelParser.SaveFile(_trackFile, _trackModel);
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
                CheckNewTracks(false,true,"");
            }
        }

        private bool GotoTrackName(string track,bool selectit)
        {
            int i;
            bool found = false;
            track = track.ToUpper().Trim(); ;
            for (i=0; i<cmbTracks.Items.Count;i++)
            {
                string selitem = cmbTracks.Items[i].ToString().ToUpper().Trim();

                if (selitem.Equals(track))
                {
                    if(selectit) cmbTracks.SelectedIndex = i;
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
            if (!FeatureAllowed()) return;
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
                        track_name = aim_import.ImportZTRACK(fn,input);
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

        private bool DoesTrackExist(string track,bool show)
        {
            if (GotoTrackName(track,false))
            {
                if (show) MessageBox.Show("Sorry, that track name already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;

            }

            return false;
        }
        private void cloneCurrentTrackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string current_track = "";
            if (!FeatureAllowed()) return;
            current_track = cmbTracks.Text;
            DialogResult dr = MessageBox.Show("Clone Track <" + current_track + "> ?", "Clone", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                string input = Interaction.InputBox("Enter a name for the Cloned Track", "Cloned Track Name", "", -1, -1);
                if (globals.ValidTrackName(input))
                {
                    if (!DoesTrackExist(input,true))
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
            if (!FeatureAllowed()) return;
            DialogResult dr = MessageBox.Show("Delete Track <" + current_track + "> ?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (DoesTrackExist(current_track,false))
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
                CheckNewTracks(false,true, globals.ToTrackName(current_track));
            }
        }

        private bool FeatureAllowed()
        {
            if (globals.no_license_check) return true;
            if (globals.license_feature == (int)globals.FeatureState.FULL) return true;
            if (globals.license_feature == (int)globals.FeatureState.DEMO) return true;
            MessageBox.Show("Sorry, this feature is not available in RaceVoice Lite\r\nContact RaceVoice to upgrade your license", "License Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;

        }

        private void AdjustUIForFeatures()
        {
            if (globals.license_feature==(int)globals.FeatureState.LITE)
            {

                chkActiveWheelLockDetection.Checked = false;
                chkAnnounceSpeed.Checked = false;
                chkAnnounceBestLap.Checked = false;
                chkAnnounceLapDelta.Checked = false;
                chkBrakeThreshold.Checked = false;
                chkLinearGForce.Checked = false;

                chkActiveWheelLockDetection.Enabled = false;
                chkAnnounceSpeed.Enabled = false;
                chkAnnounceBestLap.Enabled = false;
                chkAnnounceLapDelta.Enabled = false;
                chkBrakeThreshold.Enabled = false;
                chkLinearGForce.Enabled = false;

                btnSaveTrack.Enabled = false;
                btnSaveTrack.Visible = false;
                btnSegmentDelete.Enabled = false;
                btnSplitDelete.Enabled = false;
                btnSegmentDelete.Visible = false;
                btnSplitDelete.Visible = false;

                chkEntrySpeed.Enabled = chkEntrySpeed.Checked = false;
                chkExitSpeed.Enabled = chkExitSpeed.Checked = false;
                chkTurnInSpeed.Enabled = chkTurnInSpeed.Checked = false;
                chkMaxLinearG.Enabled = chkMaxLinearG.Checked = false;

                chkOverRev.Checked = chkOverRev.Enabled = false;
                chkDownShift.Checked = chkDownShift.Enabled = false;
                chkVoltage.Checked = chkVoltage.Enabled = false;

                rdoSpeechNotify.Enabled = false;
                rdoToneNotify.Enabled = false;
                rdoSpeechNotify.Checked = true;

            }

            switch(_carMetadata.EngineData.EcuType)
            {
                case EcuType.SmartyCam1:
                case EcuType.SmartyCam2:
                    splitsBox.Visible = false;
                    wheelLockbox.Visible = false;
                    braketonebox.Visible = false;
                    chkBrakeThreshold.Enabled = false;
                    chkActiveWheelLockDetection.Enabled = false;
                    chkAnnounceBestLap.Enabled = chkAnnounceBestLap.Checked = false;
                    chkAnnounceLapDelta.Enabled = chkAnnounceLapDelta.Checked = false;

                    chkOverRev.Enabled = true;
                    chkTemperature.Enabled  = true;
                    chkVoltage.Enabled  = true;
                    chkOilPressure.Enabled  = true;
                    chkUpShift.Enabled  = true;
                    chkDownShift.Enabled =  true;

                    break;

                case EcuType.VBOX:
                    splitsBox.Visible = false;
                    wheelLockbox.Visible = false;
                    braketonebox.Visible = false;
                    chkBrakeThreshold.Enabled = false;
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
                    wheelLockbox.Visible =true;
                    braketonebox.Visible = true;
                    chkBrakeThreshold.Enabled = true;
                    chkActiveWheelLockDetection.Enabled = true;

                    chkOverRev.Enabled =  true;
                    chkTemperature.Enabled =  true;
                    chkVoltage.Enabled =  true;
                    chkOilPressure.Enabled = true;
                    chkUpShift.Enabled =  true;
                    chkDownShift.Enabled =  true;


                    break;


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
                    while(true)
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
            if (globals.IsDemoMode(true)) return;
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

        bool  DoesDriverExist(string drivername)
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
        public void ReRender(SkiaSharp.SKCanvas canvas, SkiaSharp.SKImageInfo info)
        {
            _renderer.Render(canvas,info);
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


        private TrackMetadata CheckTrackMetaData(string filename, string metaFile, TrackMetadata metadata,TrackModel trackModel)
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

            cmbTracks.Items.Add(new ComboBoxItem<string>() { Text = metadata.TrackName, Value = filename });
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
           _trackMetadata= CheckTrackMetaData(filename, _trackMetafile, _trackMetadata, _trackModel);
            // _trackMetadata=CheckTrackMetaData(_trackMetadata);
#endif
            TrackRenderer.SmoothTrack(_trackModel, 10);
#if (!APP)
            
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
#endif
                SplitIndicatorColor = Color.Blue,
                InactiveColor = Color.Gray,
                SplitIndicatorSize = 20,
                SplitIndicatorThickness = 4,
                TrackColor = Color.Black,
                TrackThickness = 4,
                ChequeredFlagImage = globals.LocalFolder() + "\\flag.png",

                UseCurveRendering = _trackMetadata.UseCurveRendering,
                ShowGpsPoints = true,
                GpsPointSize = 3,
                GpsPointColor = Color.Yellow
            };

            _renderer = new TrackRenderer(_trackModel, rendererSettings);

#if (!APP)
            _renderTarget = new Bitmap(TrackView.Width, TrackView.Height);
            UpdateSplits();
            UpdateSegments();
            UpdateEngineDataValues();
            UpdateDynamicsDataValues();
            UpdateDataCheckboxes();

            zoom.Value = Math.Max(zoom.Minimum, Math.Min(zoom.Maximum, _trackMetadata.Zoom));
            rotation.Value = Math.Max(rotation.Minimum, Math.Min(rotation.Maximum, _trackMetadata.Rotation));
            hScroll.Value = Math.Max(hScroll.Minimum, Math.Min(hScroll.Maximum, _trackMetadata.XPan));
            vScroll.Value = Math.Max(vScroll.Minimum, Math.Min(vScroll.Maximum, _trackMetadata.YPan));
            _renderer.Zoom = (float)zoom.Value / 100;

            _trackFile = filename;
            ReRender();

            btnSaveTrack.Enabled = false;
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
                firstLine = sr.ReadLine();
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
                trace = RaceVoiceCsv.LoadCsvFile(filename).GetDataTrace();
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
                ReRender();
#endif
                rvcom.Bar(0);
            }
#if !APP
            AdjustUIForFeatures();
#endif
            return true;


        }


        
        public bool WriteDataToRaceVoice()
        {
            bool valid = true;
            if (!globals.IsRaceVoiceConnected()) return false;
            rvcom.OpenSerial();
            rvcom.Bar(0);
            rvcom.Bar(200);
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
    }
}
