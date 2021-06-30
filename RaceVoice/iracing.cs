using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using iRacingSdkWrapper;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.Logging;

//https://stackoverflow.com/questions/4844581/how-do-i-make-a-udp-server-in-c
//https://stackoverflow.com/questions/20038943/simple-udp-example-to-send-and-receive-data-from-same-socket
//https://stackoverflow.com/questions/11425202/is-it-possible-to-call-a-c-function-from-c-net

namespace RaceVoice
{

    // these need to match the racevoicetop.cpp file in the DLL project
     enum CMD
        {
        RESET,
        SETUP_MPH_ANNOUNCE,
        SETUP_MPH,
        SETUP_UPSHIFT_ANNOUNCE,
        SETUP_RPM_HIGH,
        SETUP_DOWNSHIFT_ANNOUNCE,
        SETUP_RPM_LOW,
        SETUP_OVERREV_ANNOUNCE,
        SETUP_OVERREV,
        SETUP_LATERAL_ANNOUNCE,
        SETUP_LATERAL_THRESHOLD,


        SETUP_END
    };
        class iracing
    {
        private readonly SdkWrapper wrapper;
        private UdpClient Udp;
        private static System.Timers.Timer timer;
        private static IPEndPoint BroadcastEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 90);
        private IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private float[] va = new float[10];
        private SpeechSynthesizer voice = new SpeechSynthesizer();
        private bool sdk_on = false;

        [DllImport("RaceVoiceDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void DLLDebug(int mode);
        [DllImport("RaceVoiceDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void DLLPushMessage(byte[] msg,int ln);

        [DllImport("RaceVoiceDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void DLLCloseFiles();

        [DllImport("RaceVoiceDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int DLLGetSpeech(StringBuilder msg);

        [DllImport("RaceVoiceDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void DLLSetup(int type, int value);

        [DllImport("RaceVoiceDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void DLLSetupSegment(int slot, int enable, int start_distance, int stop_distance,uint bits);

        [DllImport("RaceVoiceDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void DLLSetupSplit(int slot, int enable, int distance);

        [DllImport("RaceVoiceDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void DLLSetupSpeechTags(int slot, int enable, int distance,byte[] tag);

        private bool configured = false;

        private bool logging = false;
        private string log_path = @"c:\\temp\\irace_data_log.txt";
        private StreamWriter logfile;

 //       private string playpath = @"c:\\temp\\overrev.txt";

        private bool play = false;
        private string playpath = @"c:\\temp\\irace_limerock.txt";
        private StreamReader sr;
        
        private bool running = false;
        private Queue<string> speech_queue = new Queue<string>();

        private int last_dist = 0;
        private int speech_rate = 2;
        private int BTOI(bool val)
        {
            if (val) return 1; else return 0;
        }
        private int NBTOI(bool val)
        {
            if (val) return 0; else return 1;
        }

        
        public void configure(CarMetadata carMetadata, TrackMetadata trackdata, TrackModel track)
        {
            byte[] db = new byte[20];

            try
            {
                if (!configured)
                {
                    db[0] = 0;
                    DLLPushMessage(db, db.Length);

                }
                //DLLDebug(1);
                DLLSetup((int)CMD.RESET, 0);

                //carMetadata.DynamicsData.AnnounceSpeed = true;
                //carMetadata.DynamicsData.SpeedThreshold = 105;
                DLLSetup((int)CMD.SETUP_MPH_ANNOUNCE, BTOI(carMetadata.DynamicsData.AnnounceSpeed));
                DLLSetup((int)CMD.SETUP_MPH, carMetadata.DynamicsData.SpeedThreshold);

                DLLSetup((int)CMD.SETUP_UPSHIFT_ANNOUNCE, BTOI(carMetadata.EngineData.UpShiftEnabled));
                DLLSetup((int)CMD.SETUP_RPM_HIGH, carMetadata.EngineData.UpShift);

                DLLSetup((int)CMD.SETUP_DOWNSHIFT_ANNOUNCE, BTOI(carMetadata.EngineData.DownShiftEnabled));
                DLLSetup((int)CMD.SETUP_RPM_LOW, carMetadata.EngineData.DownShift);

                DLLSetup((int)CMD.SETUP_OVERREV_ANNOUNCE, BTOI(carMetadata.EngineData.OverRevEnabled));
                DLLSetup((int)CMD.SETUP_OVERREV, carMetadata.EngineData.OverRev);


                DLLSetup((int)CMD.SETUP_LATERAL_ANNOUNCE, BTOI(carMetadata.DynamicsData.AnnounceLateralGForce));
                DLLSetup((int)CMD.SETUP_LATERAL_THRESHOLD, (int)(carMetadata.DynamicsData.LateralGForceThreshold * 10));

                for (int slot = 0; slot < track.Segments.Count(); slot++)
                {
                    DLLSetupSegment(slot, NBTOI(track.Segments[slot].Hidden), track.Segments[slot].StartDistance, track.Segments[slot].EndDistance, track.Segments[slot].DataBits);
                }

                for (int slot = 0; slot < track.Splits.Count(); slot++)
                {
                    DLLSetupSplit(slot, NBTOI(track.Splits[slot].Hidden), track.Splits[slot].Distance);
                }

                for (int slot = 0; slot < track.Splits.Count(); slot++)
                {
                    DLLSetupSplit(slot, NBTOI(track.Splits[slot].Hidden), track.Splits[slot].Distance);
                }

                for (int slot = 0; slot < track.SpeechTags.Count(); slot++)
                {
                    byte[] sb = Encoding.ASCII.GetBytes(track.SpeechTags[slot].Phrase);
                    DLLSetupSpeechTags(slot, NBTOI(track.SpeechTags[slot].Hidden), track.SpeechTags[slot].Distance, sb);
                }

                configured = true;
                voice.Rate = 0;
                voice.Speak(globals.FixName(trackdata.TrackName, true) + " is configured");
            } 
            catch (Exception ee)
            {
                globals.WriteLine("Iracing:Configure Fail "+ee.Message);
            }

        }

        public iracing()
        {
            //string msg= "10,11,12,13,14,15,16,17,18,19,20";
            //byte[] db = Encoding.ASCII.GetBytes(msg);
            //DLLPushMessage(db,db.Length);
            //DLLCloseFiles();

            try
            {
                if (globals.irace_data_log)
                {
                    try
                    {
                        logfile = new StreamWriter(log_path, true);
                        logging = true;
                    }
                    catch (Exception ee)
                    {
                        globals.WriteLine(ee.Message);
                    }

                }
                if (globals.irace_udp_send)
                {
                    Udp = new UdpClient();
                    Udp.ExclusiveAddressUse = false;
                    Udp.EnableBroadcast = true;
                    Udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    Udp.Client.Bind(new IPEndPoint(IPAddress.Parse(globals.GetLocalIPAddress()), 90));
                }

                timer = new System.Timers.Timer();
                timer.Interval = 30;

                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;

                voice.Volume = 100;
                voice.Rate = speech_rate;
                Thread threadS = new Thread(new ThreadStart(SpeechIt));
                threadS.Start();

                if (globals.irace_udp_recv)
                {
                    Thread thread = new Thread(new ThreadStart(ListenThreadFunction));
                    thread.Start();
                }

                // Create instance
                wrapper = new SdkWrapper();
                wrapper.TelemetryUpdateFrequency = 30;
                // Listen to events
                wrapper.TelemetryUpdated += OnTelemetryUpdated;
                wrapper.SessionInfoUpdated += OnSessionInfoUpdated;

                //speech_queue.Enqueue("RaceVoice Is Ready");

                voice.SpeakCompleted += speakdone;
                voice.Volume = 100;
            }
            catch (Exception ee)
            {
                globals.WriteLine("Iracing:Initialization Fail " + ee.Message);
            }

        }

    


        private void speakdone(object sender, SpeakCompletedEventArgs sp)
        {
        }

        private void process(string indata)
        {
            try
            {
                string ln = "";
                int dist = 0;
                double mph;
                double tps;
                double latg;
                double psi;
                string[] sp = indata.Split(',');

                if (logging)
                {
                    logfile.WriteLine(indata);
                }
                //sp[2] = "50";
                //sp[3] = "1";
                //sp[0] = "5000";

                ln = "distance=" + sp[1];
                dist = Convert.ToInt32(Convert.ToDouble(sp[1])*100);
                mph = Convert.ToDouble(sp[2]) * 2.23694; // scale from m/s to mph
                tps = Convert.ToDouble(sp[3]) * 100; // scale from 0 to 100%
                latg = Convert.ToDouble(sp[5]) * 0.101 *100; // this is m/s2 so covert to g and then scale up 100x so its 3-digit signed integer
                psi = Convert.ToDouble(sp[6])*1000; // scale all PSI from 0 to 1000
 
                //if (dist == last_dist) return;
                last_dist = dist;
                indata = sp[0] + "," + dist + "," + Convert.ToInt32(mph) + "," + Convert.ToInt32(tps) + "," + sp[4] + "," + Convert.ToInt32(latg) + "," + Convert.ToInt32(psi); ;

                ln = "RPM=" + sp[0] + "  DISTANCE=" + dist + "  MPH=" + mph + "  TPS=" + Convert.ToInt32(tps) + "  LAP=" + sp[4] + "  Latg=" + sp[5] + " " + "Brake=" + sp[6];
                globals.irace_track_distance = dist;
               // if (play) Console.WriteLine(ln);
                  
                byte[] db = Encoding.ASCII.GetBytes(indata);
                if (configured)
                {
                    DLLPushMessage(db, db.Length);
                    StringBuilder speech_msg = new StringBuilder(200);
                    if (DLLGetSpeech(speech_msg) != 0)
                    {
                        Console.WriteLine("WILL SPEAK=[" + speech_msg + "]");
                        string msg = speech_msg.ToString();
                        if (msg.Contains(":") || msg.Contains("OVER"))
                        {
                            // don't stack up the MPH announcements
                            if (speech_queue.Count==0)
                            {
                                speech_queue.Enqueue(msg);
                            }
                        }
                        else
                        {
                            speech_queue.Enqueue(msg);
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                globals.WriteLine("IracingProcess Fail:"+ee.Message);
            }
        }

        public void SpeechIt()
        {

        while (true)
        {
            if (speech_queue.Count > 0)
            {
                try
                {
                    string ele = speech_queue.Peek();

                    if (ele.Contains(":"))
                    {
                        string[] msplit = ele.Split(':');
                        ele = msplit[1];
                    }

                    voice.Rate = speech_rate;
                    voice.Speak(ele);
                    speech_queue.Dequeue();
                    Console.WriteLine(ele);
                }
                catch (Exception ee)
                {
                    globals.WriteLine("Iracing:Speech Out Fail " + ee.Message);
                }
            }

            Thread.Sleep(100);
        }
        }
        public void ListenThreadFunction()
        {
            bool runit = true;
            //Creates a UdpClient for reading incoming data.
            UdpClient receivingUdpClient = new UdpClient(90);

            //Creates an IPEndPoint to record the IP Address and port number of the sender.
            // The IPEndPoint will allow you to read datagrams sent from any source.
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (runit)
            {
                try
                {

                    if (sdk_on)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    // Blocks until a message returns on this socket from a remote host.
                    Byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                    string indata = Encoding.ASCII.GetString(receiveBytes);
                    process(indata);
                    
                    //voice.SpeakAsync("HELLO!!!");
                    //voice.SpeakAsync(indata);

                    //Console.WriteLine("This is the message you received " + indata);
                    //Console.WriteLine("This message was sent from " +
                                      //          RemoteIpEndPoint.Address.ToString() +
                                        //        " on their port number " +
                                          //      RemoteIpEndPoint.Port.ToString());
                }
              catch (Exception ee)
                {
                    globals.WriteLine("Iracing:Listen Fail " + ee.Message);
                }

            }
        }




        public void startit()
        {

            if (running) return;
            running = true;
            //voice.Speak("Started");
            // timer.Enabled = true;
            wrapper.Start();
            if (play)
            {
                sr = File.OpenText(playpath);
                timer.Enabled = true;
            }
        }


        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            timer.Enabled = false;
            string lt = sr.ReadLine();
            process(lt);
            if (sr.EndOfStream)
            {
                sr.Close();
                sr = File.OpenText(playpath);
            }
            timer.Enabled = true;

        }


        private void OnSessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {
            sdk_on = true;
        }

        private void SendUDP(float rpm, float distance, float mph, float tps,float  lapnum,float latg, float psi)
        {
            try
            {
                string telem = "";
                telem = Convert.ToString(rpm) + "," + Convert.ToString(distance) + "," + Convert.ToString(mph) + "," + Convert.ToString(tps) + "," + Convert.ToString(lapnum)+","+Convert.ToString(latg)+","+Convert.ToString(psi);

                if (globals.irace_udp_send)
                {

                    byte[] data = Encoding.ASCII.GetBytes(telem);
                    int bt=Udp.Send(data, data.Length,BroadcastEP);
                    //globals.WriteLine("SEND->" + telem+" Bytes on wire="+bt);

                }
                else
                {
                    process(telem);
                    globals.iracing_telemetry = true;
                    globals.irace_hb++;
                    if (globals.irace_hb >= 100) globals.irace_hb = 0;
                }
            }
            catch (Exception ee)
            {

                globals.WriteLine(ee.Message);
            }

        }

        public void Renew()
        {
            System.Diagnostics.Process.Start("https://www.racevoice.com/sim/");
        }
        public void LicenseMessage(bool check)
        {
            string msg;
            int days_to_go = 5;
            if (!globals.network_ok)
            {
                if (check)
                {
                    msg = "Sorry, there is no internet connection.\r\nLicense cannot be validated";
                    MessageBox.Show(msg, "License Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);

                }
                return;
            }
            if (globals.iracing_node_error)
            {
                msg = "Sorry, RaceVoiceSIM is not Licensed for this computer.\r\n";
                msg += "Current Computer = " + globals.theUUID+"\r\n";
                msg +=" Licensed Computer = "+globals.iracing_node + "\r\n"; ;
                msg += "Please contact RaceVoice to discuss licensing an additional computer or moving a license.";
                // skip it at boot, so we show only the second time
                if (globals.license_hide_warnings == false || check)
                {
                    MessageBox.Show(msg, "License Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                return;

            }


            if (globals.license_days_left >= 15)
            {
                if (check)
                {
                    msg = "You Have " + globals.license_days_left + " days remaining on your RaceVoiceSIM License";
                    MessageBox.Show(msg, "License Check", MessageBoxButtons.OK, MessageBoxIcon.Hand);

                }
                return;
            }
            if (check == false)
            { 
                if (globals.license_days_left <= (-5)) return;
            }


             msg = "Your RaceVoiceSIM License has expired\r\nWould you like to renew?";
            if (globals.license_days_left < days_to_go)
            {
                if (globals.license_days_left >= 1)
                {
                    msg = "Your RaceVoiceSIM License will expire in " + globals.license_days_left + " days\r\nWould you like to renew?";
                }
                if (check)
                {
                    if (MessageBox.Show(msg, "License Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
                    {
                        Renew();
                    }
                }
            }

        }

// – Add lap information to telemetry. (LapBestLap, LapBestLapTime, LapCurrentLapTime, LapLastLapTime) 
//– Add delta time information to telemetry. (LapDeltaToBestLap, LapDeltaToBestLap_DD, 
 //       LapDeltaToBestLap_OK, LapDeltaToOptimalLap, LapDeltaToOptimalLap_DD, LapDeltaToOptimalLap_OK, 
  //      LapDeltaToSessionBestLap, LapDeltaToSessionBestLap_DD, LapDeltaToSessionBestLap_OK, LapDeltaToSessionOptimalLap,
   //     LapDeltaToSessionOptimalLap_DD, LapDeltaToSessionOptimalLap_OK) 
        private void OnTelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            if (!sdk_on)
            {
               voice.SpeakAsync("iRacing Link is Running");
            }
            sdk_on = true;
            try
            {

                // Use live telemetry...
                float rpm = e.TelemetryInfo.RPM.Value;
                float distance = e.TelemetryInfo.LapDistPct.Value;
                float mph = e.TelemetryInfo.Speed.Value;
                float lapnum = e.TelemetryInfo.Lap.Value;
                float tps = e.TelemetryInfo.Throttle.Value;
                float latg = e.TelemetryInfo.LatAccel.Value;
                var tpsi = wrapper.GetTelemetryValue<float>("BrakeRaw");
                float psi = tpsi.Value * (float)1000;
                SendUDP(rpm, distance, mph, tps, lapnum,latg,psi);
               // if (!gottel) voice.Speak("Telemtry Running");

            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
            }

        }

    }


}
