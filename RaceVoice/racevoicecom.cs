using System;
using System.IO;
#if !APP
using System.IO.Ports;
using System.Windows.Forms;
#endif
using System.Threading;
using System.Linq;



namespace RaceVoice
{
#if APP

    internal class splash
    {
        public splash(int mode)
        {

        }

        public void Show()
        {

        }

        public void setlabel(string label)
        {

        }

        public void setbar(int pos)
        {

        }

        public void Close()
        {

        }

    }
    internal class SerialPort
    {
        public bool OpenSerial()
        {
            return true;
        }

        public bool CloseSerial()
        {
            return true;
        }
        public string ReadLine()
        {
            string line = "";
            bool read = true;
            char[] inl = new char[512];
            int idx = 0;
            int to = 0;
            while (read)
            {
                line = globals.bluetooth.Read();
                if (line.Length!=0)
                {
                    line = line.Trim();
                    return line;
                }
                else
                {
                    Thread.Sleep(1);
                }
                to++;
                if (to >= 10000) return "";

            }
            return line;
        }


        public string xReadLine()
        {
            string line = "";
            bool read = true;
            char[] inl = new char[512];
            int idx = 0;
            int to = 0;
            while(read)
            {
                int ch = globals.bluetooth.ReadCh();
                if (ch >= 0)
                {
                    if (ch == 0 || ch == '\r')
                    {
                        line = new string(inl);
                        line = line.Substring(0, idx);
                        break;
                    }
                    inl[idx] = (char)ch;
                    idx++;
                }
                else
                {
                    Thread.Sleep(1);
                }
                to++;
                if (to >= 10000) return "";

            }

            line = line.Trim();
            return line;
        }

        public string ReadChar()
        {
            string c = "";
            // used for flash erase ... need to work on this
            return c;
        }

        public bool Write(byte[] ch,int mode, int size)
        {
            /// not used this is for sending audio
            return true;
        }


        public bool Write(string d)
        {
            int to = 0;
            bool sendit = true;
            while (sendit)
            {
                if (globals.bluetooth.Write(d))
                {
                    return true;
                }
                else
                {
                    Thread.Sleep(1);
                    to++;
                    if (to >= 10000) return false;
                }
            }

            return false;
        }


    }
#endif

    internal class racevoicecom
    {
        private static SerialPort _serialPort = null;


#if APP
        private bool do_firmware_update = false;
        public int barval = 0;
        public void Bar(int val)
        {
            if (val == 0) barval = 0;
            if (val == 1)
            {
                if (barval < 100) barval++;
            }
            if (val > 1 && val <= 100) barval = val;
        }

        public bool OpenSerial()
        {
            _serialPort = new SerialPort();
            _serialPort.OpenSerial();
            return true;
        }

        public bool CloseSerial()
        {

            _serialPort.CloseSerial();
            return true;
        }
        private bool CtsHolding()
        {

            // cts is not set, so we are CLEAR TO SEND
            return false; 
        }

#else
        private static ProgressBar pBar = null;
        private static bool do_firmware_update = false;

        public void SetBar(ProgressBar topbar)
        {
            pBar = topbar;
        }

        private bool CtsHolding()
        {
            return _serialPort.CtsHolding;
        }

        
        public bool OpenSerial()
        {
            if (!globals.thePort.Contains("COM"))
            {
                return false;
            }
            if (_serialPort == null)
            {
                try
                {
                    // Create a new SerialPort object with default settings.
                    _serialPort = new SerialPort();

                    // Allow the user to set the appropriate properties.
                    _serialPort.PortName = globals.thePort;
                    _serialPort.BaudRate = 115200;
                    _serialPort.Parity = Parity.None;
                    _serialPort.DataBits = 8;
                    _serialPort.StopBits = StopBits.One;
                    _serialPort.Handshake = Handshake.RequestToSend;

                    // Set the read/write timeouts
                    _serialPort.ReadTimeout = 5000;
                    _serialPort.WriteTimeout = 5000;

                    _serialPort.Open();

                    return true;
                }
                catch (Exception ee)
                {
                    globals.WriteLine(ee.Message);
                    _serialPort = null;
                    globals.thePort = "";
                }
            }

            return false;
        }

        public bool CloseSerial()
        {
            if (_serialPort != null)
            {
                try
                {
                    _serialPort.Close();
                    _serialPort = null;
                    return true;
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }

            return false;
        }


        public void ReadTimeout(int to)
        {
            if (_serialPort != null)
            {

                _serialPort.ReadTimeout = to;
            }
        }

        public char GetChar()
        {
            if (_serialPort.BytesToRead == 0) return (char)0;
            return (char)_serialPort.ReadChar();
        }
        public string ReadLine()
        {
            string line = "";
            if (_serialPort != null)
            {
                line = _serialPort.ReadLine();
            }
            return line;

        }

        public void Bar(int val)
        {
            if (pBar == null) return;

            if (val == 0) pBar.Value = 0;
            if (val == 1)
            {
                if (pBar.Maximum > (pBar.Value + pBar.Step))
                {
                    pBar.Value += pBar.Step;
                }
            }
            if (val > 1)
            {
                pBar.Maximum = val;
            }

        }
#endif

        public bool WriteSingleCmd(string cmd,bool reply=true)
        {
            return WriteSingleCmd(_serialPort, cmd,reply);
        }

        public bool WriteSingleCmd(SerialPort _sp, string cmd,bool reply=true)
        {
            string message = "";
            int steps = 0;
            globals.WriteLine("SENDING--->" + cmd);
            _sp.Write(cmd + "\r");
            if (cmd.ToUpper().Contains("VERSION TALK")) return true;
            if (cmd.ToUpper().Contains("FLASH RESET")) return true;
            if (cmd.ToUpper().Contains("FLASH READ")) return true;
            if (cmd.ToUpper().Contains("FLASH ERASE"))
            {
                splash isplash = new splash(1);
                isplash.Show();
                try
                {
                    isplash.setlabel("Erasing data log memory....");
                    while (true)
                    {
                        message = Convert.ToChar(_sp.ReadChar()).ToString();
                        Thread.Sleep(10);
                        message = message.ToUpper();
                        globals.WriteLine(message);
                        globals.last_rx = message;
                        steps++;
                        double pct = Convert.ToDouble(steps) / Convert.ToDouble(70);
                        pct *= Convert.ToDouble(100);
                        isplash.setbar(Convert.ToInt32(pct));
                        if (message.Contains("G"))
                        {
                            isplash.Close();
                            return true;
                        }
                    }
                }
                catch (Exception ee)
                {
                    globals.WriteLine(ee.Message);
                    globals.WriteLine(message);

                }

                isplash.Close();
                return true;

            }

            if (reply == false) return true;

            try
            {
                while (true)
                {
                    message = _sp.ReadLine(); // this should be a reply
                    Thread.Sleep(10);
                    message = message.ToUpper();
                    globals.WriteLine(message);
                    globals.last_rx = message;
                    if (message.Contains("*EOM*")) return true;
                    if (message.Contains("SAVED")) return true;
                    if (message.Contains("ERROR")) break;
                }
            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);

            }
            return false;
        }

        private string CheckString(bool ischecked)
        {
            if (ischecked) return "1"; else return "0";
        }

#if(!APP)
        private void DownloadCustomCan(CarMetadata carMetadata)
        {
            // find the custom can file for the ecu type
            EcuMetadata ecu = new EcuMetadata();
            EcuData ecudata = null;
            int slot = 0;
            ecudata=ecu.FindECUByName(carMetadata.EngineData.EcuName);
            if (ecudata!=null)
            {
                globals.WriteLine("Found the ECU Data...");
                // now build the string
                string cfgline = "";
                string pline = "";
                string[] psplit;
                int i;

                for (slot = 0; slot < 4; slot++)
                {

                    cfgline = "CAN CONFIG "+slot.ToString()+" ";
                    if (slot == 0) pline = ecudata.RPM;
                    if (slot == 1) pline = ecudata.TPS;
                    if (slot == 2) pline = ecudata.OILP;
                    if (slot == 3) pline = ecudata.ECT;

                    psplit = pline.Split(',');

                    cfgline += psplit[0] + " "; // canbus message id
                    for (i = 0; i < 8; i++)
                    {
                        cfgline += psplit[1 + i]; // these are the masks
                    }

                    cfgline += " " + psplit[9]+" "; // resource id


                    string type = "";
                    string offset = "";
                    string mult = "";
                    type = psplit[10].ToUpper();
                    offset = psplit[11].ToUpper();
                    mult = psplit[12].ToUpper();

                    if (type.Equals("TPS")) cfgline += "0 ";
                    if (type.Equals("RPM")) cfgline += "1 ";
                    if (type.Equals("OIL")) cfgline += "2 ";
                    if (type.Equals("ECT")) cfgline += "3 ";
                    if (type.Equals("VOLT")) cfgline += "4 ";

                    cfgline += offset + " ";
                    cfgline += mult + " ";
                    globals.WriteLine("CustomCan >" + cfgline + "<");

                    SendSerial(cfgline);
                }

                cfgline = "SET BAUD RATE " + ecudata.Baud+" "+ecudata.Listen;
                SendSerial(cfgline);
                cfgline = ecudata.Name.Replace(' ', '_');
                SendSerial("SET DASH CUSTOM "+cfgline);



            }
            else
            {
                /// some error!

            }
        }

#endif
        private string FinishDownload(System.IO.StreamWriter file,int data_count,string datafile,string final_name)
        {
            string finalfile = "";
            try
            {
                file.Close();
                if (data_count > 0)
                {
#if APP
                finalfile= globals.LocalFolder() + "//data//" + final_name;
#else
                    finalfile = globals.LocalFolder() + "\\data\\" + final_name;
#endif

                    if (File.Exists(finalfile))
                    {
                        File.Delete(finalfile);
                    }
                    System.IO.File.Move(datafile, finalfile);
                }
                if (data_count == 0) finalfile = "NODATA";
            }
            catch (Exception ee)
            {
                finalfile = "NODATA";
                globals.WriteLine(ee.Message);
            }
            globals.WriteLine("-->DATA DOWNLOADED TO -->" + finalfile + "<--");
            return finalfile;
        }

        public string DownloadData()
        {
            bool getit = true;
            string message = "";
            long count = 0;
            string localname = "";
            string final_name = "";
            string finalfile = "";
            string trackname = "";
            string trackindex = "";
            string timestamp = "";
            bool writing = false;
            int data_count = 0;
            

            localname = "datalog.csv";
#if APP
            string datafile = globals.LocalFolder() + "//data//";
#else
            splash isplash = new splash(1);
            string datafile = globals.LocalFolder() + "\\data\\";
#endif
            try
            {
                System.IO.Directory.CreateDirectory(datafile);
            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
                return "";
            }
            datafile += localname;

            WriteSingleCmd("FLASH RESET");
            WriteSingleCmd("FLASH READ");
            System.IO.StreamWriter file = new System.IO.StreamWriter(datafile);
            while (getit)
            {
#if(!APP)
                isplash.Show();
                isplash.setlabel("Downloading Data...."+final_name);
#endif
                try
                {
                    message = _serialPort.ReadLine(); // this should be a reply
                    if (message.Length < 5)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    if (message[0] == 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    if (message.Contains("COMPLETE"))
                    {
                        finalfile=FinishDownload(file, data_count, datafile, final_name);
                        break;
                    }

                    message = message.ToUpper();
                    string[] msplit = message.Split(new char[] { ',', '=',']', '[' });
                    if (message.StartsWith("LOG:0XA"))
                    {
                        //LOG:0xa0,[DRYDEN_INTERNATIONAL_RACEWAY]
                        //LOG:0xa1,[TRACK=20]
                        if (message.StartsWith("LOG:0XA0"))
                        {
                            trackname = msplit[2];
                            globals.WriteLine("TRACKNAME->" + trackname);
                            if (writing)
                            {
                                finalfile = FinishDownload(file, data_count, datafile, final_name);
                                file = new System.IO.StreamWriter(datafile);
                            }
                        }
                        if (message.StartsWith("LOG:0XA1"))
                        {
                            trackindex = msplit[3];
                            globals.WriteLine("TRACKINDEX->" + trackindex);
                        }
                        if (message.StartsWith("LOG:0XA2"))
                        {
                            //message = "LOG:0XA2,[14_38_44_12_29_2019]";
                            //msplit = message.Split(new char[] { ',', '=', ']', '[' });
                            timestamp = msplit[2];
                            if (timestamp.Length<2)
                            {
                                // use PC timestamp
                                timestamp= System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
                            }
                            globals.WriteLine("TIMESTAMP->" + timestamp);
                            final_name = trackname + "_" + trackindex + "_" + timestamp + ".csv";

                        }
                        continue;
                    }

                    if (message.Contains("LOG:0X55") == false) continue;
                    writing = true;
                    data_count++;
                    file.WriteLine(message.TrimEnd('\r', '\n'));
                    msplit = message.Split(',');
                    int position = Convert.ToInt32(msplit[msplit.Count() - 1]);
#if(!APP)
                    isplash.setbar(position);
#else
                    Bar(position);
#endif
                    count++;

                    globals.WriteLine("GOT" + count + "->" + message + "<-POS=" + position);
                }
                catch (Exception ee)
                {
                    globals.WriteLine(ee.Message);
                }

            }

            

          
      
#if (!APP)
            isplash.Close();
#endif
            return finalfile;

        }
        public bool WriteDataToRaceVoice(CarMetadata carMetadata, TrackModel track,bool fw_trace)
        {
            bool valid = true;
            string DASH = "AIM";
            int val = 0;

            if (fw_trace == false)
            {
                // configure the dash first
                switch(carMetadata.EngineData.EcuType)
                {
                    case EcuType.AIM: DASH = "AIM"; break;
                    case EcuType.MoTec: DASH = "MOTEC"; break;
                    case EcuType.SmartyCam1: DASH = "SMARTY 0"; break;
                    case EcuType.SmartyCam2: DASH = "SMARTY 1"; break;
                    case EcuType.VBOX: DASH = "VBOX"; break;
                    case EcuType.AUTOSPORT: DASH = "AUTOSPORT"; break;
                    case EcuType.STANDALONE: DASH = "STANDALONE"; break;
                    case EcuType.OBDII: DASH = "OBDII"; break;
                    default: DASH = "CUSTOM"; break;

                }

#if (!APP)

                bool dash_error = false;
                if (!carMetadata.HardwareData.Version.ToUpper().Contains("RACE VOICE-SA"))
                {
                    if (DASH.Equals("CUSTOM") || DASH.Equals("OBDII"))
                    {
                        dash_error = true;
                        MessageBox.Show("RaceVoice-DI does not support VCI/OBD-II formats.\r\nPress OK to continue configuration.", "Dash/ECU Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (DASH.Equals("STANDALONE"))
                    {
                        dash_error = true;
                        MessageBox.Show("RaceVoice-DI does not support STANDALONE operation.\r\nPress OK to continue configuration.", "Dash/ECU Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                if (!dash_error)
                {
                    if (DASH.Equals("CUSTOM"))
                    {
                        DownloadCustomCan(carMetadata);
                    }
                    else
                    {
                        SendSerial("SET DASH " + DASH);
                        if (carMetadata.EngineData.EcuType == EcuType.OBDII)
                        {
                            string baud_string = "500";
                            SendSerial("SET BAUD RATE " + baud_string);
                        }
                    }
                }
#endif
                // iterate and send all values
                SendSerial("SET RPM OVERREV THRESHOLD " + carMetadata.EngineData.OverRev.ToString());
                SendSerial("SET RPM HIGH THRESHOLD " + carMetadata.EngineData.UpShift.ToString());
                SendSerial("SET RPM LOW THRESHOLD " + carMetadata.EngineData.DownShift.ToString());
                SendSerial("SET RPM OIL THRESHOLD " + carMetadata.EngineData.OilPressureRpm.ToString());
                SendSerial("SET OIL THRESHOLD " + carMetadata.EngineData.OilPressurePsi.ToString());
                SendSerial("SET TEMPERATURE THRESHOLD " + carMetadata.EngineData.Temperature.ToString());
                SendSerial("SET VOLTAGE THRESHOLD " + carMetadata.EngineData.Voltage.ToString());
                SendSerial("SET LATERAL THRESHOLD " + carMetadata.DynamicsData.LateralGForceThreshold.ToString());
                SendSerial("SET LINEAR THRESHOLD " + carMetadata.DynamicsData.LinearGForceThreshold.ToString());
                SendSerial("SET MPH THRESHOLD " + carMetadata.DynamicsData.SpeedThreshold.ToString());
                SendSerial("SET ANNOUNCE OVERREV " + CheckString(carMetadata.EngineData.OverRevEnabled));

                val = 0;
                if (carMetadata.EngineData.UpShiftEnabled) val |= 1;
                if (carMetadata.EngineData.ShiftNotificationType == ShiftNotificationType.Tone) val |= 2;
                SendSerial("SET ANNOUNCE UPSHIFT " + val.ToString());
                SendSerial("SET ANNOUNCE DOWNSHIFT " + CheckString(carMetadata.EngineData.DownShiftEnabled));
                SendSerial("SET ANNOUNCE OIL " + CheckString(carMetadata.EngineData.OilPressureEnabled));
                SendSerial("SET ANNOUNCE TEMPERATURE " + CheckString(carMetadata.EngineData.TemperatureEnabled));
                SendSerial("SET ANNOUNCE VOLTAGE " + CheckString(carMetadata.EngineData.VoltageEnabled));
                SendSerial("SET ANNOUNCE LATGFORCE " + CheckString(carMetadata.DynamicsData.AnnounceLateralGForce));
                SendSerial("SET ANNOUNCE LINGFORCE " + CheckString(carMetadata.DynamicsData.AnnounceLinearGForce));
                SendSerial("SET ANNOUNCE MPH " + CheckString(carMetadata.DynamicsData.AnnounceSpeed));

                string wheel = "0";
                wheel = CheckString(carMetadata.DynamicsData.ActiveWheelLockDetectionEnabled);
                if (carMetadata.EngineData.EcuType == EcuType.SmartyCam1) wheel = "0";
                if (carMetadata.EngineData.EcuType == EcuType.SmartyCam2) wheel = "0";

                SendSerial("SET ANNOUNCE WHEELLOCK " +wheel);
                /*
                 * 
                SendSerial("SET GPS WINDOW " + carMetadata.HardwareData.GPSWindow.ToString());

                string baud_string = "250";
                if (carMetadata.HardwareData.BaudRate == 0) baud_string = "125";
                if (carMetadata.HardwareData.BaudRate == 1) baud_string = "250";
                if (carMetadata.HardwareData.BaudRate == 2) baud_string = "500";
                if (carMetadata.HardwareData.BaudRate == 3) baud_string = "1000";
                SendSerial("SET BAUD RATE " + baud_string);
                */

                val = 0;
                if (carMetadata.DynamicsData.AnnounceBestLap) val |= globals.ENABLE_BEST_LAP;
                if (carMetadata.DynamicsData.AnnounceLapDelta) val |= globals.ENABLE_GAIN_LAP;
                if (carMetadata.EngineData.EcuType == EcuType.SmartyCam1) val = 0;
                if (carMetadata.EngineData.EcuType == EcuType.SmartyCam2) val = 0;

                SendSerial("SET ANNOUNCE LAP " + val.ToString());
                SendSerial("SET WHEELLOCKSPEED THRESHOLD " + carMetadata.DynamicsData.WheelSpeedPercentDifference.ToString());
                SendSerial("SET WHEELLOCKBRAKE THRESHOLD " + carMetadata.DynamicsData.BrakeThresholdPsi.ToString());

                string sendout = "SET BRAKE TONE ";
                sendout += carMetadata.DynamicsData.BrakeThresholdMin.ToString() + " ";
                sendout += carMetadata.DynamicsData.BrakeThresholdMax.ToString() + " ";
                // force settings for now
                carMetadata.DynamicsData.BrakeToneHz = 900; // hz
                carMetadata.DynamicsData.BrakeToneDuration = 50; // half a second
                sendout += carMetadata.DynamicsData.BrakeToneHz.ToString() + " ";
                sendout += carMetadata.DynamicsData.BrakeToneDuration.ToString() + " ";

                string bt = "0";
                bt = CheckString(carMetadata.DynamicsData.AnnounceBrakeThreshold);
                sendout +=bt;
                SendSerial(sendout);




                string trackname = carMetadata.EngineData.TrackSelectionName;
                trackname = trackname.Replace(' ', '_');
                trackname = globals.NormalizeLength(trackname, 48);
                if (globals.IsFirmwareAtLeast(carMetadata.HardwareData.Version,"05082019"))
                    SendSerial("SET TRACK INDEX " + carMetadata.EngineData.TrackSelectionIndex.ToString() + " " + trackname);
                else
                    SendSerial("SET TRACK INDEX " + carMetadata.EngineData.TrackSelectionIndex.ToString());


#if (!APP)
                  bool allow_messages = true;
                if (carMetadata.EngineData.EcuType == EcuType.SmartyCam1) allow_messages = false;
                if (carMetadata.EngineData.EcuType == EcuType.SmartyCam2) allow_messages = false;
                if (carMetadata.MessageTriggers.Count != 0 && allow_messages)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        string current_phrase = "";
                        string new_phrase = "";
                        if (i >= carMetadata.MessageTriggers.Count)
                        {
                            carMetadata.MessageTriggers[i] = new MessageTrigger();
                        }
                        for (int j = 0; j < current_phrase.Length; j++)
                        {
                            if (current_phrase[j] == ' ') new_phrase += "_"; else new_phrase += current_phrase[j];
                        }
                        if (new_phrase.Length == 0) new_phrase = "NONE";
                        SendSerial("SET PHRASE " + (i + 1).ToString() + " " + new_phrase + " " + carMetadata.MessageTriggers[i].Repeat.ToString());
                    }
                }
#endif
            }
            // right now, we can support 10 segments and 10 splits, so the UI should not be able to allow the user
            // to create more than 10 segments/splits
            // all of these need to be available in the structure
            // even if the selected track only has 1 segment
            // the unused entries in the structure should all be set to "0"
            // that way all of the slots in the unit get updated
            // and we don't have any old slots from an old track hanging around in memory of the unit
            // similar process needs to happen on the readback

            // make sure we actually have a track ready, mostly an issue for app development/debug testing
            if (track != null)
            {
                // send segments
                for (int i = 0; i < globals.MAX_SEGMENTS; i++)
                {
                    bool svalid = false;
                    Waypoint segmentStart = new Waypoint();
                    Waypoint segmentStop = new Waypoint();

                    if (track.Segments.Count > i)
                    {
                        var segment = track.Segments[i];
                        var waypoints = track.GetSegmentWaypoints(segment);
                        segmentStart = waypoints.First();
                        segmentStop = waypoints.Last();
                        svalid = true;
                    }


                    if (fw_trace == false)
                    {

                        string segstr = "SET SEGMENT " + (i + 1) + " START ";
                        segstr += segmentStart.Latitude + " ";
                        segstr += segmentStart.Longitude + " ";
                        segstr += "STOP ";
                        segstr += segmentStop.Latitude + " ";
                        segstr += segmentStop.Longitude;
                        globals.WriteLine(segstr);
                        SendSerial(segstr);

                        if (svalid)
                        {
                            if (track.Segments[i].Hidden) segstr = "0";
                            else
                            {
                                segstr = track.Segments[i].DataBits.ToString();
                                // the unit will unpack the bit fields to determine what should be enabled on each corner
                            }
                        }
                        else segstr = "0";
                        segstr = "SET SEGMENT " + (i + 1) + " ENABLE " + segstr;
                        SendSerial(segstr);
                    }

                    if (fw_trace)
                    {
                        bool clearit = true;
                        if (i != 0) clearit = false;
                        globals.fwtrace("settings.segment_start_lat[" + (i) + "] = " + segmentStart.Latitude, clearit);
                        globals.fwtrace("settings.segment_start_lng[" + (i) + "] = " + segmentStart.Longitude, false);
                        globals.fwtrace("settings.segment_stop_lat[" + (i) + "] = " + segmentStop.Latitude, false);
                        globals.fwtrace("settings.segment_stop_lng[" + (i) + "] = " + segmentStop.Longitude, false);
                        if (svalid)
                            globals.fwtrace("settings.segment_enable[" + (i) + "] = " + track.Segments[i].DataBits.ToString(), false);
                        else
                            globals.fwtrace("settings.segment_enable[" + (i) + "] = 0", false);

                    }
                }

                globals.WriteLine("!!!");

                // send splits
                for (int i = 0; i < globals.MAX_SPLITS; i++)
                {
                    string yn = "0";
                    Waypoint waypoint = new Waypoint();
                    if (i < track.Splits.Count)
                    {
                        var split = track.Splits[i];
                        waypoint = track.GetSplitWaypoint(split);
                        yn = split.Hidden ? "0" : "1";
                    }

                    if (fw_trace == false)
                    {
                        string splitstr = "SET SPLIT " + (i + 1) + " START ";
                        splitstr += waypoint.Latitude + " ";
                        splitstr += waypoint.Longitude;
                        globals.WriteLine(splitstr);
                        SendSerial(splitstr);

                        if (carMetadata.EngineData.EcuType == EcuType.SmartyCam1) yn = "0"; // smarty cam  does not have timing on canbus
                        if (carMetadata.EngineData.EcuType == EcuType.SmartyCam2) yn = "0"; // smarty cam  does not have timing on canbus

                        string enabledstr = "SET SPLITS " + (i + 1) + " ENABLE " + yn;  // send down a "1" or "0"
                        SendSerial(enabledstr);
                    }
                    else
                    {
                        globals.fwtrace("settings.split_lat[" + (i) + "] = " + waypoint.Latitude, false);
                        globals.fwtrace("settings.split_lng[" + (i) + "] = " + waypoint.Longitude, false);
                        globals.fwtrace("settings.split_enable[" + (i) + "] = " + yn, false);

                    }
                }

                if (fw_trace == false)
                {
                    string sf = "SET CHECKER " + track.StartLinePosition.Latitude + " " + track.StartLinePosition.Longitude;
                    SendSerial(sf);
                }
                else
                {
                    globals.fwtrace("settings.checker_lattitude=" + track.StartLinePosition.Latitude, false);
                    globals.fwtrace("settings.checker_longitude=" + track.StartLinePosition.Longitude, false);
                }
            }
            return valid;
        }

        public string SendSerial(string cmd, CarMetadata carMetadata, TrackModel track)
        {
            return _SendSerial(cmd, carMetadata, track);
        }

        public string SendSerial(string cmd)
        {
            return _SendSerial(cmd, null, null);
        }

        public void FirmwareDone()
        {
            do_firmware_update = false; // restore send serial back to normal operation
        }

        public bool SendAudio(string file)
        {
            bool success = true;
            byte[] db = new byte[10];
            file = globals.LocalFolder() + "\\"+file;
            try
            {

                globals.WriteLine("Sending Audio File =["+file+"]");

                FileStream fs = new FileStream(file, FileMode.Open);

                for (int i=0;i<fs.Length;i++)
                {
                    db[0] = (byte)fs.ReadByte();

                    while (CtsHolding())
                    {
                        globals.WriteLine("Sending Audio...wait for CTS");
                        Thread.Sleep(1);
                    }
                    _serialPort.Write(db, 0, 1);



                }

                fs.Close();

            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
                success = false;
            }




            return success;
            
        }
        private string _SendSerial(string cmd, CarMetadata carMetadata, TrackModel track)
        {
            string message = "";
            string full = "";
            bool get_settings = false;
            bool save_settings = false;
            bool get_version = false;

            try
            {
                cmd = cmd.ToUpper();
                if (_serialPort != null)
                {
                    if (cmd.Contains("SHOW VERSION"))
                    {
                        cmd = "SHOW SETTINGS";
                        get_version = true;
                    }
                    if (cmd.ToUpper().Contains("FWERASE")) do_firmware_update = true;
                    globals.WriteLine("SENDING--->" + cmd);
                    int i;
                    cmd += "\r";

                    for (i = 0; i < cmd.Length; i++)
                    {
                        while (CtsHolding()) Thread.Sleep(1);
                        string sb = cmd[i].ToString();
                        _serialPort.Write(sb);
                    }
                    if (do_firmware_update)
                    {
                        Thread.Sleep(5);
                        message = _serialPort.ReadLine(); // this should now be the status reply for the firmware update process
                        //globals.WriteLine("COMLAYER_FW->" + message);
                        return message;
                    }

                    message = _serialPort.ReadLine(); // this should have an echo of the command we send
                    globals.WriteLine("COMLAYER->"+message);
                    Thread.Sleep(1);
                    if (cmd.StartsWith("SET")) save_settings = true;
                    if (cmd.StartsWith("CAN CONFIG")) save_settings = true;
                    if (cmd.Contains("SHOW SETTINGS") && !get_version) get_settings = true;
                    if (message.ToUpper().Contains("INVALID") || message.ToUpper().Contains("SHELL") || message[0]=='\r')
                    {
                        // trap on a null being sent, which is basically just a carriage return 
                        if (cmd[0] == 0)
                        {
                            return "OK";
                        }

                    }
                    while (save_settings)
                    {
                        Bar(1);
                        message = _serialPort.ReadLine(); // this should be a reply
                        Thread.Sleep(10);
                        message = message.ToUpper();
                        globals.WriteLine(message);

                        if (message.Contains("SAVED"))
                        {
                            break;
                        }
                        // allow older firmware to still work as it may reply as "invalid" with some commands
                        if (message.Contains("INVALID")) break;
       
                        if (message.Contains("ERROR")) break;
                    }

                    while (get_settings || get_version)
                    {
                        Bar(1);
                        message = _serialPort.ReadLine(); // this should be a reply
                        Thread.Sleep(10);
                        message = message.ToUpper();
                        message = message.TrimEnd('\r', '\n');
                        // strip any non ascii characters
                        string[] fields = message.Split(' ');

                        if (message.Contains("VERSION"))
                        {
                            carMetadata.HardwareData.Version = message.Trim();

                            continue;
                        }

                        if (fields[0].Contains("UNIT"))
                        {
                            if (fields[1].Contains("NAME"))
                            {
                                carMetadata.HardwareData.Name = fields[3].Trim();
                            }
                            continue;
                        }
                        if (get_version)
                        {
                            if (message.Contains("DONE")) break; else continue;
                        }


                        // serial txt scrape // all of these values need to be moved into a common upper structure
                        // that will be accessed in the UI, saved to JSON/etc
                        // these are values from the unit itself

                        if (fields[0].Contains("BAUD") && fields[1].Contains("RATE"))
                        {
                            if (fields[3].Contains("125")) carMetadata.HardwareData.BaudRate = 0;
                            if (fields[3].Contains("250")) carMetadata.HardwareData.BaudRate = 1;
                            if (fields[3].Contains("500")) carMetadata.HardwareData.BaudRate = 2;
                            if (fields[3].Contains("1000")) carMetadata.HardwareData.BaudRate = 3;
                        }
                        if (fields[0].Contains("VOICE"))
                        {
                            if (fields[1].Contains("VOLUME"))
                            {
                                carMetadata.HardwareData.Volume = Convert.ToInt32(fields[3]);
                            }

                            if (fields[1].Contains("PITCH"))
                            {
                                carMetadata.HardwareData.Pitch = globals.RangeFix(Convert.ToInt32(fields[3]),1,99);
                            }
                        }

                        if (fields[0].Contains("WHEELLOCKBRAKE"))
                        {
                            carMetadata.DynamicsData.BrakeThresholdPsi = Convert.ToInt32(fields[3]);
                            // fixup for code prior to 02-09-19
                            if (carMetadata.DynamicsData.BrakeThresholdPsi < 100)
                            {
                                carMetadata.DynamicsData.BrakeThresholdPsi = 100;
                            }


                        }
                        if (fields[0].Contains("WHEELLOCKSPEED"))
                        {
                            carMetadata.DynamicsData.WheelSpeedPercentDifference = 1 + Convert.ToInt32(fields[3]);
                            if (carMetadata.DynamicsData.WheelSpeedPercentDifference>=20)
                            {
                                carMetadata.DynamicsData.WheelSpeedPercentDifference = 20;
                            }
                        }

                        if (fields[0].Contains("BRAKE"))
                        {
                            if (fields[1].Contains("TONE"))
                            {
                                carMetadata.DynamicsData.BrakeThresholdMin = Convert.ToInt32(fields[3]);
                                carMetadata.DynamicsData.BrakeThresholdMax = Convert.ToInt32(fields[4]);
                                carMetadata.DynamicsData.BrakeToneHz = Convert.ToInt32(fields[5]);
                                carMetadata.DynamicsData.BrakeToneDuration = Convert.ToInt32(fields[6]);
                                if (fields[7].Contains("1"))
                                    carMetadata.DynamicsData.AnnounceBrakeThreshold = true;
                                else
                                    carMetadata.DynamicsData.AnnounceBrakeThreshold = false;

                            }

                        }

                       
                       
                        if (fields[0].Contains("DASH"))
                        {
                            string theDASH = fields[3].Trim().ToUpper();
                            carMetadata.EngineData.EcuName = "";
                            if (theDASH.Contains("AIM")) carMetadata.EngineData.EcuType = EcuType.AIM;
                            if (theDASH.Contains("MOTEC")) carMetadata.EngineData.EcuType = EcuType.MoTec;
                            if (theDASH.Contains("SMARTY"))
                            {
                                carMetadata.EngineData.EcuType = EcuType.SmartyCam1;
                                if (fields[4].Trim().ToUpper().Contains("1"))
                                {
                                    carMetadata.EngineData.EcuType = EcuType.SmartyCam2;
                                }
                            }
                            if (theDASH.Contains("VBOX")) carMetadata.EngineData.EcuType = EcuType.VBOX;
                            if (theDASH.Contains("AUTOSPORT")) carMetadata.EngineData.EcuType = EcuType.AUTOSPORT;
                            if (theDASH.Contains("OBD_II")) carMetadata.EngineData.EcuType = EcuType.OBDII;
                            if (theDASH.Contains("STANDALONE")) carMetadata.EngineData.EcuType = EcuType.STANDALONE;

                            if (theDASH.Contains("CUSTOM"))
                            {
                                theDASH = fields[4].Trim().ToUpper();
                                theDASH = theDASH.Replace('_', ' ');
                                carMetadata.EngineData.EcuName = theDASH;
                            }
                        }

                        if (fields[0].Contains("GPS"))
                        {
                            if (fields[1].Contains("WINDOW"))
                            {
                                if (fields[3].Contains("."))
                                {
                                    //02-09-19 code had a floating point number
                                    fields[3] = "1"; // set to defaults
                                }
                                carMetadata.HardwareData.GPSWindow = Convert.ToInt32(fields[3]);
                            }
                        }

                        if (fields[0].Contains("PHRASE"))
                        {
                            int segn = Convert.ToInt32(fields[1]);  // this is the index of the segment starts at 1 in the unit
                            segn = segn - 1;
                            if (carMetadata.MessageTriggers.Count != 8)
                            {
                                carMetadata.MessageTriggers = new MessageTrigger[8];
                                for (int j = 0; j < 8; j++) carMetadata.MessageTriggers[j] = new MessageTrigger();
                            }
                            else
                            {
                                for (int j=0;j<8;j++)
                                {
                                    if (carMetadata.MessageTriggers[j] == null) carMetadata.MessageTriggers[j] = new MessageTrigger();
                                }
                            }
                            if (segn>=0 && segn<=7)
                            {
                                // remove the underscores
                                string text_phrase = fields[2];
                                string new_phrase = "";
                                for (int j = 0; j < text_phrase.Length; j++) if (text_phrase[j] == '_') new_phrase += ' '; else new_phrase += text_phrase[j];
                                carMetadata.MessageTriggers[segn].Phrase = new_phrase;
                                carMetadata.MessageTriggers[segn].Repeat = Convert.ToInt32(fields[3]);
                            }


                        }
                        // the readback of the segments, we only need to set if the segment
                        // in the track map is enabled or disabled
                        // we don't need to worry about reading back any of the GPS values
                        // the UI also needs to change its track selection based on the "TRACK" reply below
                        if (fields[0].Contains("SEGMENT"))
                        {
                            if (fields[2].Contains("START") && track!=null)
                            {
                                int segn = Convert.ToInt32(fields[1]);  // this is the index of the segment starts at 1 in the unit
                                segn = segn - 1;
                                // make sure we don't add more than what's already defined
                                if (segn < track.Segments.Count)
                                {
                                    if (fields[10].Contains("DISABLED") || fields[10].Contains("ACTIVE"))
                                    {
                                            if (fields[10].Contains("DISABLED"))
                                            {
                                                track.Segments[segn].DataBits = 0;
                                            }
                                            else
                                            {
                                                track.Segments[segn].DataBits = 1;
                                            }
                                    }
                                    else
                                    {
                                        track.Segments[segn].DataBits = Convert.ToUInt16(fields[10]);
                                    }
                                    if (track.Segments[segn].DataBits == 0) track.Segments[segn].Hidden = true; else track.Segments[segn].Hidden = false;
                                }
                            }
                        }

                        if (fields[0].Contains("SPLIT") && track!=null)
                        {
                            int segn = Convert.ToInt32(fields[1]);  // this is the index of the split starts at 1 in the unit
                            segn = segn - 1;
                            bool hide = true;

                            if (fields[4].ToUpper().Contains("ACTIVE")) hide=false;
                            Console.WriteLine("From Unit Split=" + segn + " Hidden=" + hide.ToString());
                            if (segn < track.Splits.Count)
                            {
                                Console.WriteLine("Set!");
                                track.Splits[segn].Hidden = hide;
                            }
                        }

                        if (fields[0].Contains("TRACK"))
                        {
                            if (fields[1].Contains("INDEX"))
                            {
                                try
                                {
                                    int ti = Convert.ToInt32(fields[3]);
                                    carMetadata.EngineData.TrackSelectionIndex = ti;

                                    if (fields.Count()>4)
                                    {
                                        carMetadata.EngineData.TrackSelectionName = fields[4].Replace('_', ' ');
                                        carMetadata.EngineData.FindTrackByName = true;
                                    }
                                } 
                                catch (Exception ee)
                                {
                                    globals.WriteLine(ee.Message);
                                }
                                // convert ME so I can pick the selected track from the UI's drop down list
                            }
                        }






                        if (fields[0].Contains("RPM"))
                        {
                            if (fields[2].Contains("THRESHOLD"))
                            {
                                if (fields[1].Contains("OVERREV"))
                                {
                                    carMetadata.EngineData.OverRev = Convert.ToInt32(fields[4].Trim());
                                }
                                if (fields[1].Contains("HIGH"))
                                {
                                    carMetadata.EngineData.UpShift = Convert.ToInt32(fields[4].Trim());
                                }
                                if (fields[1].Contains("LOW"))
                                {
                                    carMetadata.EngineData.DownShift = Convert.ToInt32(fields[4].Trim());
                                }
                                if (fields[1].Contains("OIL"))
                                {
                                    carMetadata.EngineData.OilPressureRpm= Convert.ToInt32(fields[4].Trim());
                                }
                            }
                        }

                        if (fields[0].Contains("OIL") && fields[3].Contains("THRESHOLD"))
                        {
                            carMetadata.EngineData.OilPressurePsi = Convert.ToInt32(fields[5].Trim());
                        }

                        if (fields[0].Contains("TEMPERATURE") && fields[2].Contains("THRESHOLD"))
                        {
                            carMetadata.EngineData.Temperature = Convert.ToInt32(fields[4].Trim());
                        }

                        if (fields[0].Contains("VOLTAGE") && fields[2].Contains("THRESHOLD"))
                        {
                            carMetadata.EngineData.Voltage = (float)Convert.ToDouble(fields[4].Trim());
                        }

                        if (fields[0].Contains("MPH") && fields[2].Contains("THRESHOLD"))
                        {
                            carMetadata.DynamicsData.SpeedThreshold = Convert.ToInt32(fields[4].Trim());
                        }

                        if (fields[0].Contains("LATGFORCE") && fields[1].Contains("THRESHOLD"))
                        {
                            carMetadata.DynamicsData.LateralGForceThreshold = Convert.ToDouble(fields[3].Trim());
                        }

                        if (fields[0].Contains("LINGFORCE") && fields[1].Contains("THRESHOLD"))
                        {
                            carMetadata.DynamicsData.LinearGForceThreshold = Convert.ToDouble(fields[3].Trim());
                        }
                        if (fields.Length >= 4)
                        {
                            if (fields[1].Contains("ANNOUNCE"))
                            {
                                bool flag = fields[3].Contains("1");
                                int val = 0;

                                if (fields[0].Contains("LAP"))
                                {
                                    val=Convert.ToInt32(fields[3]) & globals.ENABLE_BEST_LAP;
                                    if (val != 0) flag = true; else flag = false;
                                    carMetadata.DynamicsData.AnnounceBestLap = flag;

                                    val = Convert.ToInt32(fields[3]) & globals.ENABLE_GAIN_LAP;
                                    if (val != 0) flag = true; else flag = false;
                                    carMetadata.DynamicsData.AnnounceLapDelta = flag;
                                }
                                if (fields[0].Contains("OVERREV")) carMetadata.EngineData.OverRevEnabled = flag;

                                if (fields[0].Contains("UPSHIFT"))
                                {
                                    val = Convert.ToInt32(fields[3]);
                                    flag = false;

                                    if (Convert.ToBoolean(val & 2))
                                        carMetadata.EngineData.ShiftNotificationType = ShiftNotificationType.Tone;
                                    else
                                        carMetadata.EngineData.ShiftNotificationType = ShiftNotificationType.Speech;
                                    if (Convert.ToBoolean(val & 1)) flag = true;
                                    carMetadata.EngineData.UpShiftEnabled = flag;
                                }

                                if (fields[0].Contains("DOWNSHIFT"))
                                {
                                    carMetadata.EngineData.DownShiftEnabled = flag;
                                }

                                if (fields[0].Contains("OIL")) carMetadata.EngineData.OilPressureEnabled = flag;
                                if (fields[0].Contains("TEMPERATURE")) carMetadata.EngineData.TemperatureEnabled = flag;
                                if (fields[0].Contains("VOLTAGE")) carMetadata.EngineData.VoltageEnabled = flag;
                                if (fields[0].Contains("LATGFORCE")) carMetadata.DynamicsData.AnnounceLateralGForce = flag;
                                if (fields[0].Contains("LINGFORCE")) carMetadata.DynamicsData.AnnounceLinearGForce = flag;
                                if (fields[0].Contains("MPH")) carMetadata.DynamicsData.AnnounceSpeed = flag;
                                if (fields[0].Contains("WHEELLOCK")) carMetadata.DynamicsData.ActiveWheelLockDetectionEnabled = flag;
                            }
                        }
                        full = full + message;
                        globals.WriteLine(message);
                        if (message.Contains("DONE")) break;
                    }
                }

                if (!save_settings)
                {
                    if (carMetadata != null)
                    {
                        if (carMetadata.HardwareData.Name.Contains("NONE"))
                        {
                            //NameBox nb = new NameBox();
                            //nb.ShowDialog();
                            if (globals.theSerialNumber.Length > 0)
                            {
                                string txt = "SET NAME " + globals.theSerialNumber;
                                if (WriteSingleCmd(_serialPort, txt))
                                {
                                    carMetadata.HardwareData.Name = globals.theSerialNumber;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                if (do_firmware_update == false)
                {
                    globals.WriteLine(ee.Message + "\r\n" + ">" + message + "<");
                   // MessageBox.Show();
                }
            }

            return message;
        }
    }
}