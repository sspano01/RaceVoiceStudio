using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace RaceVoice
{
    public partial class splash : Form
    {
        private int portscan = 0;
        private int timeleft = 0;
        private static SerialPort _serialPort = null;
        private string[] ports = SerialPort.GetPortNames();
        private static int isplash = 0;
        private static bool first_start = true;
        private static bool first_poll = true;
        public splash(int splash)
        {
            isplash = splash;
            InitializeComponent();
            try
            {
                string debug_mode = globals.LocalFolder() + "\\admin.txt";
                string line;

                if (first_start)
                {
                    string cf = globals.LocalFolder() + "\\car.json";
                    if (File.Exists(cf))
                    {
                        System.IO.StreamReader file = new System.IO.StreamReader(cf);
                        while ((line = file.ReadLine()) != null)
                        {
                            line = line.ToUpper();
                            if (line.Contains("PC MODE"))
                            {
                                globals.quick_start = true;
                            }
                        }
                        file.Close();
                    }
                }
                if (File.Exists(debug_mode))
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(debug_mode);
                    while ((line = file.ReadLine()) != null)
                    {
                        line = line.ToUpper();
                        if (line.Equals("QUICK-START")) globals.quick_start = true;
                        if (line.Equals("NO-UNIT-CHECK")) globals.no_unit_check = true;
                        if (line.Equals("NO-TRACK-CHECK")) globals.no_track_check = true;
                        if (line.Equals("NO-LICENSE-CHECK")) globals.no_license_check = true;
                        if (line.Equals("IRACE-UDP-SEND")) globals.irace_udp_send = true;
                        if (line.Equals("IRACE-UDP-RCV")) globals.irace_udp_recv = true;
                        if (line.Equals("IRACE-DATA-LOG")) globals.irace_data_log = true;
                        if (line.Equals("TRACE")) globals.trace = true;
                        if (line.Equals("TERMINAL")) globals.terminal = true;
                    }

                    //globals.allow_track_edit = true;
                    file.Close();
                }

                if (first_start)
                {
                    if (globals.trace)
                    {
                        for (int i = 0; i < 20; i++) globals.WriteLine(" ");
                    }
                    globals.WriteLine("****** STARTUP");
                    first_start = false;
                }
            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
                // no problem, just move along
            }
        }

        public void setlabel(string l)
        {
            updatelabel.Text = l;
            updatelabel.Invalidate();
            updatelabel.Update();
            updatelabel.Refresh();
            Application.DoEvents();
        }

        public void setbar(int pct)
        {
            progressBar1.Value = pct;
            Application.DoEvents();
        }

        private void splash_Load(object sender, EventArgs e)
        {
            if (isplash==0)
            {

                if (first_poll)
                {
                    first_poll = false;
                    if (globals.quick_start)
                    {
                        this.Close();
                        return;
                    }
                }
            }
            if (isplash == 0 || isplash==1)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Size = new Size(this.Width, 270); // crop the bottom of the form
            }
            if (isplash == 0)
            {
                updatelabel.Visible = true;
                setlabel("Searching for RaceVoice....");

                if (globals.forcePort.Length > 0)
                {
                    globals.thePort = globals.forcePort;
                    this.Close();
                    return;
                }

                timer1.Start(); // 100ms
                progressBar1.Value = 0;
                // Get a list of serial port names.

                globals.WriteLine("The following serial ports were found:");

                // Display each port name to the console.
                foreach (string port in ports)
                {
                    globals.WriteLine(port);
                }
            }

            if (isplash == 1)
            {
                updatelabel.Visible = true;
                progressBar1.Maximum = 100;
            }

            if (isplash == 2)
            {
                updatelabel.Visible = false;
                progressBar1.Visible = false;

                int trackCount = Directory.GetFiles(globals.track_folder, "*.csv").Length;

                splashbox.Items.Add("Local UUID: " + globals.theUUID);
                splashbox.Items.Add("License State: " + globals.license_state + " Feature State: " + globals.license_feature);
                if (globals.iracing_node_error)
                {
                    splashbox.Items.Add("RaceVoiceSIM Is NOT LICENSED for this computer");
                    splashbox.Items.Add("License Exists for: "+globals.iracing_node);

                }
                else
                {
                    if (globals.expire_time.Length > 2)
                    {
                        splashbox.Items.Add("RaceVoiceSIM License Expires on: " + globals.expire_time);
                    }
                }
                splashbox.Items.Add("Total Available Tracks: " + trackCount);
                for (int i = 0; i < 2; i++) splashbox.Items.Add("\r\n");
                splashbox.Items.Add("Warning: ALL RACING AND MOTORSPORTS ARE DANGEROUS!");
                splashbox.Items.Add("Warning: IT IS THE DRIVER'S RESPONSIBILITY TO MAINTAIN CONTROL!");
                splashbox.Items.Add("Warning: UNDER ALL ROAD/WEATHER/SURFACE/COMPETITION CONDTIONS!");
                for (int i = 0; i < 2; i++) splashbox.Items.Add("\r\n");
                splashbox.Items.Add("Copyright 2020, RaceVoice LLC, Patent Pending, www.RaceVoice.com");
            }

            if (isplash == 3)
            {
                this.Size = new Size(this.Width + 200, this.Height);
                splashbox.Width += 200;
                close.Location = new Point(close.Location.X + 200, close.Location.Y);

                updatelabel.Visible = false;
                progressBar1.Visible = false;
                string rel_file = globals.LocalFolder() + "\\release.txt";
                try
                {
                    string line = "";
                    System.IO.StreamReader file =new System.IO.StreamReader(rel_file);
                    while ((line = file.ReadLine()) != null)
                    {
                        splashbox.Items.Add(line + "\r\n");

                    }

                    file.Close();
                }
                catch(Exception ee)
                {
                    globals.WriteLine(ee.Message);
                }

                splashbox.Items.Add(" ");
                splashbox.Items.Add(" ");
                splashbox.Items.Add("Copyright 2020, RaceVoice LLC, Patent Pending, www.RaceVoice.com");
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string port = "";
            int scan_time = 60;
            timeleft++;
            timer1.Stop();

            progressBar1.Maximum = scan_time;
            if (globals.no_unit_check)
            {
                this.Close();
                return;
            }
            if (ports.Length > 0)
            {
                port = ports[portscan];
                if (!globals.thePort.Contains("COM"))
                {
                    globals.WriteLine("CHECKING--->" + port);
                    if (FindComPort(port))
                    {
                        globals.thePort = port;
                        timer1.Stop();
                        this.Close();
                    }
                }
                portscan++;
                if (portscan >= ports.Length) portscan = 0;         
            }

            if (timeleft>=scan_time)
            {
                //if (globals.thePort.Length == 0)
                //{
                //    MessageBox.Show("RaceVoice has not been found.\r\nRaceVoice Studio will operate in standalone mode.", "Communication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //
                // }
                timer1.Stop();
                this.Close();
            }
            else
            {
                progressBar1.Value = timeleft;
                timer1.Start();
            }
        }

        private bool FindComPort(string port)
        {
            bool found = false;
            ftdi ftx = new ftdi();
            try
            {
                // Create a new SerialPort object with default settings.
                _serialPort = new SerialPort();

                // Allow the user to set the appropriate properties.
                _serialPort.PortName = port;
                _serialPort.BaudRate = 115200;
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.RequestToSend;

                // Set the read/write timeouts
                _serialPort.ReadTimeout = 100;
                _serialPort.WriteTimeout = 100;

                _serialPort.Open();

                // send some return keys
                int i = 0;
                string message = "";
                for (i = 0; i < 10; i++)
                {
                    _serialPort.WriteLine("\r");
                    message = _serialPort.ReadLine();
                    if (message.Contains(">> Invalid Command"))
                    {
                        _serialPort.Close();
                        found = true;
                        globals.theSerialNumber = ftx.GetSerialNumber(port);
                        break;
                    }
                    System.Threading.Thread.Sleep(250);
                }

                _serialPort.Close();
            }
            catch
            {
                // MessageBox.Show(ee.Message);
                if (_serialPort != null) _serialPort.Close();
                _serialPort = null;
            }

            return found;
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}