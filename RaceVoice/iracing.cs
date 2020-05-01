﻿using System;
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

#if (APP)

    class iracing
    {
        private readonly SdkWrapper wrapper;
        private UdpClient Udp;
        private static IPEndPoint BroadcastEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 90);
        private IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);


        public void configure(CarMetadata carMetadata, TrackModel track)
        {

        }

        public iracing()
        {
            // Create instance
            wrapper = new SdkWrapper();
            wrapper.TelemetryUpdateFrequency = 30;
            // Listen to events
            wrapper.TelemetryUpdated += OnTelemetryUpdated;
            wrapper.SessionInfoUpdated += OnSessionInfoUpdated;

            Udp = new UdpClient();
            Udp.ExclusiveAddressUse = false;
            Udp.EnableBroadcast = true;
            Udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Udp.Client.Bind(new IPEndPoint(IPAddress.Parse(globals.GetLocalIPAddress()), 90));


        }
        private void OnSessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {

        }

        private void OnTelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            try
            {



                // Use live telemetry...
                float rpm = e.TelemetryInfo.RPM.Value;
                float distance = e.TelemetryInfo.LapDist.Value;
                float mph = e.TelemetryInfo.Speed.Value;
                float lapnum = e.TelemetryInfo.Lap.Value;
                float tps = e.TelemetryInfo.Throttle.Value;
                globals.WriteLine("TPS=" + tps.ToString());
                SendUDP(rpm, distance, mph, tps, lapnum);

            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
            }
        }

        private void SendUDP(float rpm, float distance, float mph, float lapnum, float tps)
        {
            try
            {
                string telem = "";
                telem = Convert.ToString(rpm) + "," + Convert.ToString(distance) + "," + Convert.ToString(mph) + "," + Convert.ToString(tps) + "," + Convert.ToString(lapnum);
                globals.WriteLine(telem);
                byte[] data = Encoding.ASCII.GetBytes(telem);
                int bt = Udp.Send(data, data.Length, BroadcastEP);
                //globals.WriteLine("SEND->" + telem+" Bytes on wire="+bt);
            }
            catch (Exception ee)
            {

                globals.WriteLine(ee.Message);
            }

        }


        public void startit()
        {
            wrapper.Start();

        }

    }

#else

    class iracing
    {
        private bool speaking = false;
        private readonly SdkWrapper wrapper;
        private UdpClient Udp;
        private static System.Timers.Timer timer;
        private static IPEndPoint BroadcastEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 90);
        private IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private float[] va = new float[10];
        private SpeechSynthesizer voice = new SpeechSynthesizer();

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

        private bool configured = false;
        private int BTOI(bool val)
        {
            if (val) return 1; else return 0;
        }
        private int NBTOI(bool val)
        {
            if (val) return 0; else return 1;
        }

        private void SendConfig()
        {


        }
        public void configure(CarMetadata carMetadata, TrackModel track)
        {
            byte[] db = new byte[20];
            if (!configured)
            {
                db[0] = 0;
                DLLPushMessage(db, db.Length);

            }
            DLLSetup((int)CMD.RESET, 0);

            carMetadata.DynamicsData.AnnounceSpeed = true;
            carMetadata.DynamicsData.SpeedThreshold = 10;
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
                DLLSetupSegment(slot, NBTOI(track.Segments[slot].Hidden), track.Segments[slot].StartDistance, track.Segments[slot].EndDistance,track.Segments[slot].DataBits);
            }

            for (int slot = 0; slot < track.Splits.Count(); slot++)
            {
                DLLSetupSplit(slot, NBTOI(track.Splits[slot].Hidden), track.Splits[slot].Distance);
            }

            configured = true;

        }

        public iracing()
        {
            //string msg= "10,11,12,13,14,15,16,17,18,19,20";
            //byte[] db = Encoding.ASCII.GetBytes(msg);
            //DLLPushMessage(db,db.Length);
            //DLLCloseFiles();

            Udp = new UdpClient();
            Udp.ExclusiveAddressUse = false;
            Udp.EnableBroadcast = true;
            Udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Udp.Client.Bind(new IPEndPoint(IPAddress.Parse(globals.GetLocalIPAddress()), 90));

            timer = new System.Timers.Timer();
            timer.Interval = 100;

            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;

            voice.Volume = 100;
            ////voice.Rate = 2;

            Thread thread = new Thread(new ThreadStart(ListenThreadFunction));
            //thread.Start();


            // Create instance
            wrapper = new SdkWrapper();
            wrapper.TelemetryUpdateFrequency = 30;
            // Listen to events
            wrapper.TelemetryUpdated += OnTelemetryUpdated;
            wrapper.SessionInfoUpdated += OnSessionInfoUpdated;
            // Start it!

            voice.Speak("RaceVoice Ready");

            voice.SpeakCompleted += speakdone;

        }


        private void speakdone(object sender, SpeakCompletedEventArgs sp)
        {
            speaking = false;
        }

        private void process(string indata)
        {
            try
            {

                string[] sp = indata.Split(',');

                //sp[2] = "50";
                sp[3] = "1";
                //sp[0] = "5000";
                double mph = Convert.ToDouble(sp[2]);
                mph *= 2.25; // wtf??

                indata = sp[0] + "," + sp[1] + "," + Convert.ToInt32(mph) + "," + Convert.ToInt32(Convert.ToDouble(sp[3]) * 100) + "," + sp[4];
                Console.WriteLine(indata);
                byte[] db = Encoding.ASCII.GetBytes(indata);
                if (configured)
                {
                    DLLPushMessage(db, db.Length);
                    StringBuilder speech_msg = new StringBuilder(200);
                    if (DLLGetSpeech(speech_msg) != 0)
                    {
                        Console.WriteLine("WILL SPEAK=[" + speech_msg + "]");
                        string msg = speech_msg.ToString();
                        if (!speaking)
                        {
                            speaking = true;
                            voice.SpeakAsync(msg);
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
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
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(10);
            }
        }




        public void startit()
        {
            // timer.Enabled = true;
            wrapper.Start();
            //globals.WriteLine("Start!!");

        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            SendUDP(va[0], va[1], va[2], va[3],va[4]);
            va[0] += 100;
            if (va[0] > 7000) va[0] = 0;
           // Console.WriteLine("Raised: {0}", e.SignalTime);
        }


        private void OnSessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {

        }

        private void SendUDP(float rpm, float distance, float mph, float lapnum,float tps)
        {
            try
            {
                string telem = "";
                telem = Convert.ToString(rpm) + "," + Convert.ToString(distance) + "," + Convert.ToString(mph) + "," + Convert.ToString(tps) + "," + Convert.ToString(lapnum);
                process(telem);
                globals.WriteLine(telem);
                byte[] data = Encoding.ASCII.GetBytes(telem);
                int bt=Udp.Send(data, data.Length,BroadcastEP);
                //globals.WriteLine("SEND->" + telem+" Bytes on wire="+bt);
            }
            catch (Exception ee)
            {

                globals.WriteLine(ee.Message);
            }

        }
        private void OnTelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            try
            {


                // Use live telemetry...
                float rpm = e.TelemetryInfo.RPM.Value;
                float distance = e.TelemetryInfo.LapDist.Value;
                float mph = e.TelemetryInfo.Speed.Value;
                float lapnum = e.TelemetryInfo.Lap.Value;
                float tps = e.TelemetryInfo.Throttle.Value;
                SendUDP(rpm, distance, mph, tps, lapnum);
            } 
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
            }
        }

    }


#endif
}
