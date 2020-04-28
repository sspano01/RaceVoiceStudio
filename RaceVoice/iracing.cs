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

//https://stackoverflow.com/questions/4844581/how-do-i-make-a-udp-server-in-c
//https://stackoverflow.com/questions/20038943/simple-udp-example-to-send-and-receive-data-from-same-socket
//https://stackoverflow.com/questions/11425202/is-it-possible-to-call-a-c-function-from-c-net

namespace RaceVoice
{
    class iracing
    {
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
            Udp.Client.Bind(new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), 90));

            timer = new System.Timers.Timer();
            timer.Interval = 100;

            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;

            voice.Volume = 100;
            voice.Rate = -2;

            Thread thread = new Thread(new ThreadStart(ListenThreadFunction));
            thread.Start();


            // Create instance
            wrapper = new SdkWrapper();
            wrapper.TelemetryUpdateFrequency = 10;
            // Listen to events
            wrapper.TelemetryUpdated += OnTelemetryUpdated;
            wrapper.SessionInfoUpdated += OnSessionInfoUpdated;
            // Start it!

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

                    //voice.SpeakAsync(indata);

                    Console.WriteLine("This is the message you received " + indata);
                    Console.WriteLine("This message was sent from " +
                                                RemoteIpEndPoint.Address.ToString() +
                                                " on their port number " +
                                                RemoteIpEndPoint.Port.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(10);
            }
        }


        public static string GetLocalIPAddress()
        {
            bool isether = false;
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    Console.WriteLine(ni.Name);
                    if (ni.Name.ToUpper().Contains("ETHERNET")) isether = true;
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            Console.WriteLine(ip.Address.ToString());
                            if (isether) return ip.Address.ToString();
                        }
                    }
                }
            }
            return "255.255.255.255";
        }


        public void startit()
        {
            timer.Enabled = true;
            wrapper.Start();

        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            SendUDP(va[0], va[1], va[2], va[3]);
            va[0] += 100;
            if (va[0] > 7000) va[0] = 0;
           // Console.WriteLine("Raised: {0}", e.SignalTime);
        }


        private void OnSessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {

        }

        private void SendUDP(float rpm, float distance, float mph, float lapnum)
        {
            try
            {
                string telem = "";
                telem = Convert.ToString(rpm) + "," + Convert.ToString(distance) + "," + Convert.ToString(mph) + "," + Convert.ToString(lapnum);
                byte[] data = Encoding.ASCII.GetBytes(telem);
                int bt=Udp.Send(data, data.Length,BroadcastEP);
                Console.WriteLine("SEND->" + telem+" Bytes on wire="+bt);
            } catch (Exception ee)
            {

                Console.WriteLine(ee.Message);
            }

        }
        private void OnTelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {

            // Use live telemetry...
            float rpm = e.TelemetryInfo.RPM.Value;
            float distance = e.TelemetryInfo.LapDist.Value;
            float mph = e.TelemetryInfo.Speed.Value;
            float lapnum = e.TelemetryInfo.Lap.Value;
            SendUDP(rpm, distance, mph, lapnum);        }

    }
}
