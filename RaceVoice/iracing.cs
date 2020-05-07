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
        SETUP_LATERAL_ANNOUNCE,
        SETUP_LATERAL_THRESHOLD,


        SETUP_END
    };
        class iracing
    {
        private readonly SdkWrapper wrapper;
#if (UDP)
        private UdpClient Udp;
#endif
        private static System.Timers.Timer timer;
        private static IPEndPoint BroadcastEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 90);
        private IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private float[] va = new float[10];
        private SpeechSynthesizer voice = new SpeechSynthesizer();
        //private StreamWriter sw;
        //private StreamReader sr;
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

        //private string xpath = @"c:\\temp\\telemetry.txt";
        
        //private string xplaypath = @"c:\\temp\\short_course.txt";

        private bool play = false;
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

            DLLSetup((int)CMD.SETUP_LATERAL_ANNOUNCE, BTOI(carMetadata.DynamicsData.AnnounceLateralGForce));
            DLLSetup((int)CMD.SETUP_LATERAL_THRESHOLD, (int)(carMetadata.DynamicsData.LateralGForceThreshold*10));

            for (int slot = 0; slot < track.Segments.Count(); slot++)
            {
                track.Segments[0].DataBits = 1;
                DLLSetupSegment(slot, NBTOI(track.Segments[slot].Hidden), track.Segments[slot].StartDistance, track.Segments[slot].EndDistance,track.Segments[slot].DataBits);
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
                DLLSetupSpeechTags(slot, NBTOI(track.SpeechTags[slot].Hidden), track.SpeechTags[slot].Distance,sb);
            }

            configured = true;
            voice.Rate = 0;
            voice.Speak(trackdata.TrackName+ " is configured");

        }

        public iracing()
        {
            //string msg= "10,11,12,13,14,15,16,17,18,19,20";
            //byte[] db = Encoding.ASCII.GetBytes(msg);
            //DLLPushMessage(db,db.Length);
            //DLLCloseFiles();


#if (UDP)
            Udp = new UdpClient();
            Udp.ExclusiveAddressUse = false;
            Udp.EnableBroadcast = true;
            Udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Udp.Client.Bind(new IPEndPoint(IPAddress.Parse(globals.GetLocalIPAddress()), 90));
#endif
            timer = new System.Timers.Timer();
            timer.Interval = 30;

            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;

            voice.Volume = 100;
            voice.Rate = speech_rate;
            Thread threadS = new Thread(new ThreadStart(SpeechIt));
            threadS.Start();
            if (!play)
            {
                Thread thread = new Thread(new ThreadStart(ListenThreadFunction));
                //thread.Start();
                // Start it!
                //sw = File.CreateText(path);
                //sw.Close();
            }
            else
            {
               // sr = File.OpenText(playpath);
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


        private void speakdone(object sender, SpeakCompletedEventArgs sp)
        {
        }

        private void process(string indata,bool asis)
        {
            try
            {
                string ln = "";
                int dist = 0;
                double mph;
                double tps;
                string[] sp = indata.Split(',');

                //sp[2] = "50";
                //sp[3] = "1";
                //sp[0] = "5000";

                ln = "distance=" + sp[1];
               // Console.WriteLine(ln);
                if (!asis)
                {
                    dist = Convert.ToInt32(Convert.ToDouble(sp[1])*100);
                    //dist = Convert.ToInt32(Convert.ToDouble(sp[1]) * 3.4);
                    mph = Convert.ToDouble(sp[2]);
                    mph *= 2.25; // wtf??
                    tps = Convert.ToDouble(sp[3]) * 100;
                }
                else
                {
                    dist = Convert.ToInt32(sp[1]);
                    mph = Convert.ToDouble(sp[2]);
                    tps = Convert.ToDouble(sp[3]);

                }
                //if (dist == last_dist) return;
                last_dist = dist;
                indata = sp[0] + "," + dist + "," + Convert.ToInt32(mph) + "," + Convert.ToInt32(tps) + "," + sp[4];

                ln = "RPM=" + sp[0] + "  DISTANCE=" + dist + "  MPH=" + mph + "  TPS=" + Convert.ToInt32(tps) + "  LAP=" + sp[4];
                if (!play)
                {
                     Console.WriteLine(ln);
                    //sw = File.AppendText(path);
                    //sw.WriteLine(ln);
                    //sw.Close();
                }
                else
                {
                    // Console.WriteLine(ln);
                }

               // Console.WriteLine(ln);
                byte[] db = Encoding.ASCII.GetBytes(indata);
                if (configured)
                {
                    DLLPushMessage(db, db.Length);
                    StringBuilder speech_msg = new StringBuilder(200);
                    if (DLLGetSpeech(speech_msg) != 0)
                    {
                        Console.WriteLine("WILL SPEAK=[" + speech_msg + "]");
                        string msg = speech_msg.ToString();
                        if (msg.Contains("MPH:"))
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
                globals.WriteLine(ee.Message);
            }
        }

        public void SpeechIt()
        {

            while(true)
            {
                if (speech_queue.Count>0)
                {
                    string ele = speech_queue.Peek();
                    
                    if (ele.Contains("MPH:"))
                    {
                        string[] msplit = ele.Split(':');
                        ele = msplit[1];
                    }

                    voice.Rate = speech_rate;
                    voice.Speak(ele);
                    speech_queue.Dequeue();
                    Console.WriteLine(ele);
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
                    process(indata,false);
                    
                    //voice.SpeakAsync("HELLO!!!");
                    //voice.SpeakAsync(indata);

                    //Console.WriteLine("This is the message you received " + indata);
                    //Console.WriteLine("This message was sent from " +
                                      //          RemoteIpEndPoint.Address.ToString() +
                                        //        " on their port number " +
                                          //      RemoteIpEndPoint.Port.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(10);
            }
        }




        public void startit()
        {

            if (running) return;
            running = true;
            //voice.Speak("Started");
            // timer.Enabled = true;
            wrapper.Start();
            if (play) timer.Enabled = true;

        }


        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            timer.Enabled = false;
            //string lt = sr.ReadLine();
            //string[] lts = lt.Split(new Char[] { '=', ' ' });

            //lt=lts[1]+","+lts[4] + "," + lts[7] + "," + lts[10] + "," + lts[13];
            //process(lt, true);
//            Console.WriteLine(lt);
            //if (sr.EndOfStream)
           // {
            //    sr.Close();
            //     sr = File.OpenText(playpath);
            //}
            timer.Enabled = true;

        }


        private void OnSessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {
            sdk_on = true;
        }

        private void SendUDP(float rpm, float distance, float mph, float tps,float  lapnum)
        {
            try
            {
                string telem = "";
                telem = Convert.ToString(rpm) + "," + Convert.ToString(distance) + "," + Convert.ToString(mph) + "," + Convert.ToString(tps) + "," + Convert.ToString(lapnum);
                process(telem,false);
                globals.iracing_telemetry = true;
                globals.irace_hb++;
                if (globals.irace_hb >= 100) globals.irace_hb = 0;
                return;

                //sw = File.AppendText(path);
                //sw.WriteLine(telem);
                //process(telem);
                //globals.WriteLine(telem);
                //byte[] data = Encoding.ASCII.GetBytes(telem);
                //int bt=Udp.Send(data, data.Length,BroadcastEP);
                //globals.WriteLine("SEND->" + telem+" Bytes on wire="+bt);
            }
            catch (Exception ee)
            {

                globals.WriteLine(ee.Message);
            }

        }

        public void Renew()
        {
            System.Diagnostics.Process.Start("https://www.racevoice.com/product/ear-buds/");
        }
        public void LicenseMessage(bool check)
        {
            string msg;
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
            if (globals.license_days_left < 15)
            {
                if (globals.license_days_left >= 1)
                {
                    msg = "Your RaceVoiceSIM License will expire in " + globals.license_days_left + " days\r\nWould you like to renew?";
                }
                if (MessageBox.Show(msg, "License Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Hand)==DialogResult.Yes)
                {
                    Renew();
                }

            }

        }
        private void OnTelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            if (!sdk_on)
            {
               voice.SpeakAsync("Telemetry Running");
            }
            sdk_on = true;
            try
            {

                // Use live telemetry...
                float rpm = e.TelemetryInfo.RPM.Value;
                float distance = e.TelemetryInfo.LapDistPct.Value;
//                float distance = e.TelemetryInfo.LapDist.Value;
                float mph = e.TelemetryInfo.Speed.Value;
                float lapnum = e.TelemetryInfo.Lap.Value;
                float tps = e.TelemetryInfo.Throttle.Value;

                SendUDP(rpm, distance, mph, tps, lapnum);
               // if (!gottel) voice.Speak("Telemtry Running");

            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
            }

        }

    }


}
