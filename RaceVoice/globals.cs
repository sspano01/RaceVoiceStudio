﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
#if !APP
using System.IO.Ports;
using System.Windows.Forms;
#endif

namespace RaceVoice
{
    internal static class globals
    {

#if APP
        public static string selected_track = "";
        public static RaceVoicePCMain pcmain;
        public static bool check_track_server = true;

#endif
        public static string thePort = "";
        public static string theSerialNumber = "";
        public static string theUUID = "";

        public static string UIVersion = "12-17-2019-A1";

        //public static string racevoice_http = "racevoice.servep2p.com";
       
        public static string racevoice_http = "racevoice.serveftp.com";

        public static string racevoice_sqlserver = "racevoicesql.servep2p.com";
        public static bool NO_ADIM_FILES = false;

        public static int force_firmware_update = 0;
        public static bool first_connected = false;
        public static bool no_unit_check = false;
        public static string forcePort = "";
        public static bool no_track_check = true;
        public static bool no_license_check = true;

        public static bool disabled_charts = false;

        public static bool virgin_load = false;
        public static string license_state = "UNKNOWN";
        public static int license_feature = 0;

        public const string track_folder = "Tracks";
        public const string ecu_folder = "ECUs";

        public static bool fake_connection = false;
        public static bool trace = false; 


        public static string last_rx = "";

        public static int MAX_SEGMENTS = 10;
        public static int MAX_SPLITS = 10;
        public static int ENABLE_BEST_LAP = 1;
        public static int ENABLE_GAIN_LAP = 2;
        public static bool network_ok = false;
        public static int MAX_TRACK_NAME = 48;

        public static bool all_stop = false;

        public enum FeatureState
        {
            NONE,
            FULL,
            LITE,
            DEMO
        }

        public static string ToTrackName(string ascii_name)
        {
            ascii_name = ascii_name.Replace(' ', '_');
            ascii_name += ".csv";
            return ascii_name;
        }
        public static string ToTrackJsonName(string ascii_name)
        {
            ascii_name = ascii_name.Replace(' ', '_');
            ascii_name += ".json";
            return ascii_name;
        }
        public static bool RenameTrackJson(string new_track)
        {
            bool good = true;
            string new_track_json = globals.LocalFolder() + "\\tracks\\" + globals.ToTrackJsonName(new_track);
            try
            {
                if (File.Exists(new_track_json))
                {
                    var metadata = TrackMetadata.Load(new_track_json);
                    if (metadata!=null)
                    {
                        metadata.TrackName=Path.GetFileNameWithoutExtension(new_track).Replace('_', ' ');
                        metadata.Save(new_track_json);
                    }

                }
            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
                good = false;
            }

            return good;
        }
        public static bool CloneTrack(string new_track,string current_track)
        {
            bool good = true;
            string new_track_json = "";
            string current_track_json = "";
            new_track_json = globals.LocalFolder() + "\\tracks\\" + globals.ToTrackJsonName(new_track);
            new_track = globals.LocalFolder() + "\\tracks\\" + globals.ToTrackName(new_track);

            current_track_json = globals.LocalFolder() + "\\tracks\\" + globals.ToTrackJsonName(current_track);
            current_track = globals.LocalFolder() + "\\tracks\\" + globals.ToTrackName(current_track);

            try
            {
                if (File.Exists(current_track) && File.Exists(current_track_json))
                {
                    File.Copy(current_track, new_track);
                    File.Copy(current_track_json, new_track_json);

                    if (!File.Exists(new_track) || !File.Exists(new_track_json)) good = false;
                }
            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
                good = false;
            }

            return good;
        }

        public static bool DeleteTrack(string track)
        {
            bool good = true;
            string track_json = "";
            track_json = globals.LocalFolder() + "\\tracks\\" + globals.ToTrackJsonName(track);
            track = globals.LocalFolder() + "\\tracks\\" + globals.ToTrackName(track);

            try
            {
                if (File.Exists(track) && File.Exists(track_json))
                {
                    File.Delete(track);
                    File.Delete(track_json);

                    if (File.Exists(track) || File.Exists(track_json)) good = false;
                }
            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
                good = false;
            }

            return good;
        }

        public static bool ValidTrackName(string name)
        {
            bool good = true;
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            if (name.Length == 0) good = false;
            if (name.Length >= MAX_TRACK_NAME) good = false;
            if (name.IndexOfAny(invalidFileNameChars) >= 0) good = false;
            return good;
        }
        public static void fwtrace(string str,bool clear)
        {
            bool append = true;
            if (clear) append = false;
            string fn = globals.LocalFolder() + "\\fwtrace.txt";
            try
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(fn, append);
                str = str + ";";
                file.WriteLine(str);
                file.Close();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
       

    }

        public static string NormalizeLength(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static int FwMonth(string month)
        {
            month = month.ToLower();
            if (month.Contains("jan")) return 1;
            if (month.Contains("feb")) return 2;
            if (month.Contains("mar")) return 3;
            if (month.Contains("apr")) return 4;
            if (month.Contains("may")) return 5;
            if (month.Contains("jun")) return 6;
            if (month.Contains("jul")) return 7;
            if (month.Contains("aug")) return 8;
            if (month.Contains("sep")) return 9;
            if (month.Contains("oct")) return 10;
            if (month.Contains("nov")) return 11;
            if (month.Contains("dec")) return 12;

            return 0;
        }
         
        public static bool IsFirmwareAtLeast(string unit_version, string date)
        {
            string fw_unit_month = "";
            string fw_unit_day = "";
            string fw_unit_year = "";
            string fw_unit_version = "";
            string[] vsplit = unit_version.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < vsplit.Length; i++)
            {
                string ev = vsplit[i].Trim().ToUpper();
                //Global Read Step = 0 Val->RACE
                //Global Read Step = 1 Val->VOICE
                //Global Read Step = 2 Val->VERSION
                //Global Read Step = 3 Val->JANUARY
                //Global Read Step = 4 Val->17
                //Global Read Step = 5 Val->2019
                //Global Read Step = 6 Val->A1
                globals.WriteLine("FW Unit Reply Read Step = " + i + " Val->" + ev);
                switch (i)
                {
                    case 3: fw_unit_month = ev; break;
                    case 4: fw_unit_day = ev; break;
                    case 5: fw_unit_year = ev; break;
                    case 6: fw_unit_version = ev.Substring(0, 2); break;
                    default: break;
                }
            }
            string mm = date.Substring(0, 2);
            string dd = date.Substring(2, 2);
            string yy = date.Substring(4, 4);
            if (fw_unit_year.Length == 0) return false;

            DateTime fw_check_date = new DateTime(Convert.ToInt32(yy), Convert.ToInt32(mm), Convert.ToInt32(dd));
            DateTime fw_unit_date = new DateTime(Convert.ToInt32(fw_unit_year), globals.FwMonth(fw_unit_month), Convert.ToInt32(fw_unit_day));

            if (DateTime.Compare(fw_unit_date, fw_check_date) >= 0) return true;
            

            return false;
        }
        

        public static int RangeFix(int source, int min, int max)
        {
            if (source < min || source > max) source = max / 2;
            return source;
        }

        public static void FilelineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            int start = 0;
            for (int i = 0; i < arrLine.Length; i++)
            {
                if (arrLine[i].Contains('.'))
                {
                    start = i + 1;
                    break;
                }
            }
            if (start != 0)
            {
                int offset = line_to_edit + start;
                if (offset < arrLine.Length)
                {
                    arrLine[line_to_edit + start] = newText;
                    File.WriteAllLines(fileName, arrLine);
                }
            }
        }


        public static string LocalFolder()
        {
            string dir = "";
#if APP
            dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#else
            dir = Directory.GetCurrentDirectory();
#endif
            return dir;
        }

        public static bool IsNetworkAvailable()
        {
            return IsNetworkAvailable(0);
        }

        /// <summary>
        /// Indicates whether any network connection is available.
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable(long minimumSpeed)
        {
            bool valid = false;
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                Console.WriteLine("IsNetworkAvailable - Not Found Error 1");
                return false;
            }

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                globals.WriteLine("Checking Network Card " + ni.Description);
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                //if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                //    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                //    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                valid = true;
                break;
            }

            if (valid)
            {
                globals.WriteLine("Network Card Validated!");
            }
            else
            {
                globals.WriteLine("*** NO CARD AVAILABLE ***");

            }

            return valid;
        }

#if !APP
        public static bool IsDemoMode(bool nag)
        {

            if (globals.license_feature == (int)globals.FeatureState.DEMO)
            {
                if (nag)
                {
                    MessageBox.Show("RaceVoice Studio is in Demonstration Mode\r\n\r\nGoto www.RaceVoice.com\r\nto purchase a unit for full access to RaceVoice Studio", "RaceVoice Demo Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                return true;
            }
            return false;

        }

        
        public static bool IsRaceVoiceConnected()
        {
            if (globals.thePort.Length == 0)
            {
                splash sp = new splash(0);
                sp.ShowDialog(); // look for a new racevoice unit
                if (globals.thePort.Length == 0)
                {
                    // didn't find it?
                    sp.Close();
                    MessageBox.Show("RaceVoice is not connected.\r\nMake sure USB is connected.\r\nMake sure that RaceVoice has power applied.\r\nCheck that the USB Drivers have been installed.\r\n", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return false;
                }
                sp.Close();
                return true;
            }
            else
            {

                racevoicecom rv = new racevoicecom(); // we have a racevoice port, check to make sure its still there
                if (rv.OpenSerial() == false)
                {
                    globals.thePort = "";  // nope so find it again
                    splash sp = new splash(0);
                    sp.ShowDialog();
                    if (globals.thePort.Length == 0)
                    {
                        sp.Close();
                        MessageBox.Show("RaceVoice is not connected.\r\nMake sure USB is connected.\r\nMake sure that RaceVoice has power applied.\r\nCheck that the USB Drivers have been installed.\r\n", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    else
                    {
                        sp.Close();
                        return true;
                    }
                }
                else
                {
                    // still there!
                    rv.CloseSerial();
                    return true;
                }
            }
            return false;
        }

#endif

#if APP
        public static bool IsRaceVoiceConnected()
        {
            return true;
        }

#endif
        public static void Terminate()
        {
            Environment.Exit(0);
        }

       

        public static  bool IsOnlineTest()
        {
            globals.WriteLine("Starting OnlineTest Test..\r\n");

            if (globals.no_license_check || globals.no_track_check)
            {
                globals.WriteLine("Bypassed for debug!\r\n");
                return true;
            }

            System.Net.WebRequest req = System.Net.WebRequest.Create("https://www.google.com");
            System.Net.WebResponse resp = default(System.Net.WebResponse);
            try
            {
                resp = req.GetResponse();
                resp.Close();
                req = null;
                globals.WriteLine("PASS..\r\n");
                return true;
            }
            catch (Exception ex)
            {
                globals.WriteLine("FAIL.."+ex.Message);
                req = null;
                return false;
            }
        }


        public static bool OnlinePingTest()
        {
            try
            {
                globals.WriteLine("Starting PING Test..\r\n");
                if (!IsNetworkAvailable())
                {
                    globals.WriteLine("Network was not available");
                    return false;
                }

                Ping myPing = new Ping();
                String host = "www.google.com";
                byte[] buffer = new byte[32];
                int i;
                for (i = 0; i < 5; i++)
                {
                    int timeout = 1000;
                    PingOptions pingOptions = new PingOptions();
                    PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                    globals.WriteLine("Ping " + i);
                    if (reply.Status == IPStatus.Success)
                    {
                        globals.WriteLine("SUCCESS!");
                        return true;
                    }

                }
                globals.WriteLine("Fail!");
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // is the local one newer than the server one?
        public static bool FileModifiedNewer(string file, string localdate, string remotedate,string localmd5,string remotemd5)
        {
            bool newer = false;
            string mm = "";
            string dy = "";
            string yy = "";

            if (file.ToLower().Contains("shen"))
            {
                Console.Write("!");
            }
            remotedate = remotedate.ToUpper();
            string[] rsplit = remotedate.Split(' ');
            if (rsplit[0].Contains("JAN")) mm = "01";
            if (rsplit[0].Contains("FEB")) mm = "02";
            if (rsplit[0].Contains("MAR")) mm = "03";
            if (rsplit[0].Contains("APR")) mm = "04";
            if (rsplit[0].Contains("MAY")) mm = "05";
            if (rsplit[0].Contains("JUN")) mm = "06";
            if (rsplit[0].Contains("JUL")) mm = "07";
            if (rsplit[0].Contains("AUG")) mm = "08";
            if (rsplit[0].Contains("SEP")) mm = "09";
            if (rsplit[0].Contains("OCT")) mm = "10";
            if (rsplit[0].Contains("NOV")) mm = "11";
            if (rsplit[0].Contains("DEC")) mm = "12";

            dy = rsplit[1];
            yy = rsplit[2];

            string remdate = mm + "/" + dy + "/" + yy;

            DateTime r1 = new DateTime(int.Parse(yy), int.Parse(mm), int.Parse(dy));
            DateTime r2 = Convert.ToDateTime(localdate);

            if (r2 == r1)
            {
                // same date? what about same md5
                //if (localmd5.Trim().ToUpper().Contains(remotemd5.Trim().ToUpper())) newer = true;

                newer = true;
            }
            else
            {
                if (r2 > r1) newer = true;
            }
            globals.WriteLine(file + " DATE Local=" + localdate + " vs " + remdate + " Local Is Newer=" + newer.ToString());

            return newer;
        }

        public static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }


        public static void WriteLine(string ln)
        {
            Console.WriteLine(ln);

            if (globals.trace)
            {
                string fn = globals.LocalFolder() + "\\trace.txt";
                try
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter(fn, true);
                    file.WriteLine(DateTime.Now.ToString() + ":" + ln);
                    file.Close();
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.Message);
                }
            }
        }

        
    }
}