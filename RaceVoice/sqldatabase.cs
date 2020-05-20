using System;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

#if (!APP)
using System.Windows.Forms;
#else
using Xamarin.Forms;
#endif
using MySql.Data.MySqlClient;

namespace RaceVoice
{

    internal class sqldatabase
    {

        public static DateTime GetNetworkTime()
        {
            const string NtpServer = "pool.ntp.org";

            const int DaysTo1900 = 1900 * 365 + 95; // 95 = offset for leap-years etc.
            const long TicksPerSecond = 10000000L;
            const long TicksPerDay = 24 * 60 * 60 * TicksPerSecond;
            const long TicksTo1900 = DaysTo1900 * TicksPerDay;

            var ntpData = new byte[48];
            ntpData[0] = 0x1B; // LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(NtpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            long pingDuration = Stopwatch.GetTimestamp(); // temp access (JIT-Compiler need some time at first call)
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);
                socket.ReceiveTimeout = 5000;
                socket.Send(ntpData);
                pingDuration = Stopwatch.GetTimestamp(); // after Send-Method to reduce WinSocket API-Call time

                socket.Receive(ntpData);
                pingDuration = Stopwatch.GetTimestamp() - pingDuration;
            }

            long pingTicks = pingDuration * TicksPerSecond / Stopwatch.Frequency;

            // optional: display response-time
            // Console.WriteLine("{0:N2} ms", new TimeSpan(pingTicks).TotalMilliseconds);

            long intPart = (long)ntpData[40] << 24 | (long)ntpData[41] << 16 | (long)ntpData[42] << 8 | ntpData[43];
            long fractPart = (long)ntpData[44] << 24 | (long)ntpData[45] << 16 | (long)ntpData[46] << 8 | ntpData[47];
            long netTicks = intPart * TicksPerSecond + (fractPart * TicksPerSecond >> 32);

            var networkDateTime = new DateTime(TicksTo1900 + netTicks + pingTicks / 2);

            return networkDateTime.ToLocalTime(); // without ToLocalTime() = faster
        }



        public bool ValidateUUID(string uuid,bool set, CarMetadata cm)
        {
            string connetionString;
            bool val_status = false;
            string dbauth = "";
            string dbuuid = "";
            string email_address = "none";
            string user_name = "";
            int access_count = 0;
            string this_pc_uuid = "";
            bool all_stop = false;

           // if (globals.network_time.Length==0)
           // {
            //    globals.network_time = GetNetworkTime().ToString();
           // }
            connetionString = "Data Source=" + globals.racevoice_sqlserver + "; Initial Catalog = racevoice;Integrated Security=False;User ID=root;Password=#RaceVoice01;connection timeout=30";

            //connectionString="Data Source=104.155.20.171;Initial Catalog=bookshelf;Integrated Security=False;User ID=dotnetapp;Password=test;MultipleActiveResultSets=True"

            //Server = myServerAddress; Database = myDataBase; User Id = myUsername;
            //Password = myPassword;
            //connetionString="<add name="YOUR_NAME_HERE" connectionString="Server=***.***.**.***;Database=YOUR_DATABASE_NAME;Uid=YOUR_USERNAME;Pwd=YOUR_USER_PASSWORD" providerName="MySql.Data.MySqlClient" />"
            // string conStr = "Server=xxx.xxx.xxx.xxx;Database=DB_NAME;Uid=USER_NAME;Password=PASSWORD;

            uuid = uuid.ToUpper().Trim();
            this_pc_uuid = uuid;
            MySqlConnection sqlConnection1 = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            MySqlDataReader reader;

            try
            {
                sqlConnection1.Open();

                cmd.CommandText = "SELECT CURDATE()";
                cmd.Connection = sqlConnection1;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    globals.network_time= reader.GetValue(0).ToString();
                }
                reader.Close();


                cmd.CommandText = "SELECT * FROM license where UUID like '%" + uuid + "%'";
                //cmd.CommandType = Command
                cmd.Connection = sqlConnection1;

                reader = cmd.ExecuteReader();
                bool match_uuid = false;
                bool match_email = false;
                // Data is accessible through the DataReader object here.
                while (reader.Read())
                {
                    string dbname = reader.GetValue(0).ToString();
                    dbuuid = reader.GetValue(1).ToString().ToUpper().Trim();
                    string dblastaccess = reader.GetValue(2).ToString();
                    string dbemail = reader.GetValue(3).ToString();
                    dbauth = reader.GetValue(4).ToString().ToUpper().Trim();
                    string purchasedate = reader.GetValue(5).ToString();
                    access_count = Convert.ToInt32(reader.GetValue(6));
                    globals.expire_time = reader.GetValue(9).ToString();
                    globals.iracing_node = reader.GetValue(10).ToString();
                    if (dbuuid.Contains(uuid))
                    {
                        match_uuid = true;
                    }
                }
                reader.Close();
                sqlConnection1.Close();


                try
                {
                    if (!match_uuid)
                    {
                        globals.WriteLine("UUID was not found");
                        // try by email address
                        licenseemail getemail = new licenseemail();
                        getemail.ShowDialog();
                        email_address = getemail.user_email;
                        user_name = getemail.user_name;
                        if (user_name.Contains("end"))
                        {
                            globals.Terminate();

                        }
                        //email_address = "steve@fl-eng.com";
                        //email_address = "jamesr@pointsw.com";
                        sqlConnection1.Open();
                        cmd.CommandText = "SELECT * FROM license where email = '" + email_address + "'";
                        //cmd.CommandType = Command
                        cmd.Connection = sqlConnection1;

                        reader = cmd.ExecuteReader();
                        // Data is accessible through the DataReader object here.
                        while (reader.Read())
                        {
                            string dbname = reader.GetValue(0).ToString();
                            dbuuid = reader.GetValue(1).ToString().ToUpper().Trim();
                            string dblastaccess = reader.GetValue(2).ToString();
                            string dbemail = reader.GetValue(3).ToString().ToUpper().Trim();
                            dbauth = reader.GetValue(4).ToString().ToUpper().Trim();
                            if (dbemail == email_address.ToUpper().Trim())
                            {
                                match_email = true;
                            }
                        }
                    }
                    reader.Close();

                   
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                    globals.WriteLine(ee.Message);
                }

                if (sqlConnection1.State != ConnectionState.Open)
                {
                    sqlConnection1.Close();
                    sqlConnection1.Open();
                }

                if (match_uuid && set)
                {
                    string combined = cm.HardwareData.Name+"["+cm.HardwareData.Version + "]";
                    // update the access date and count
                    cmd.CommandText = "UPDATE license SET swversion = @swversion, unitversion = @unitversion Where uuid like '%" + uuid + "%'";
                    cmd.Parameters.AddWithValue("@unitversion", combined);
                    cmd.Parameters.AddWithValue("@swversion", globals.UIVersion);
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ee)
                    {
                        globals.WriteLine(ee.Message);
                        return false;
                    }
                    sqlConnection1.Close();
                    return true;
                }
                if (match_uuid)
                {
                    // update the access date and count
                    dbauth = dbauth.ToUpper();
                    bool valid_lic = false;
                    bool valid_irace = false;
                    bool demo_only = false;
                    bool doreload = false;
                    if (dbauth.Contains("VALID")) valid_lic = true;
                    if (dbauth.Contains("IRACING")) valid_irace = true;
                    if (dbauth.Contains("DEMO")) demo_only = true;
                    string timedate = DateTime.Now.ToString("ddd, dd MMM yyy HH:mm:ss GMT");
                    if (globals.iracing_node.Length < 2)
                    {
                        cmd.CommandText = "UPDATE license SET lastaccess = @lastaccess, accesscount = @accesscount, iracingnode = @iracingnode, authorization = @authorization, expire = @expire Where uuid like '%" + uuid + "%'";
                        cmd.Parameters.AddWithValue("@iracingnode", this_pc_uuid);

                        string authfield = "DEMO-IRACING";
                        if (demo_only == false)
                        {
                            authfield = "VALID-IRACING";
                        }
                        cmd.Parameters.AddWithValue("@authorization", authfield);

                        DateTime tnow = Convert.ToDateTime(globals.network_time);
                        tnow = tnow.AddDays(10);
                        string expire_time = tnow.ToString("MM/dd/yyyy");
                        globals.expire_time = expire_time;
                        cmd.Parameters.AddWithValue("@expire", expire_time);

                        doreload = true;


                    }
                    else
                    {
                        cmd.CommandText = "UPDATE license SET lastaccess = @lastaccess, accesscount = @accesscount Where uuid like '%" + uuid + "%'";
                    }
                    cmd.Parameters.AddWithValue("@lastaccess", timedate);
                    cmd.Parameters.AddWithValue("@accesscount", access_count + 1);
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ee)
                    {
                        globals.WriteLine(ee.Message);
                    }

                    if (doreload)
                    {
                        globals.license_state = "RELOAD";
                        sqlConnection1.Close();
                        return true;
                    }
                    if (!demo_only)
                    {
                        if (valid_lic)
                        {
                            if (dbauth.Contains("LITE"))
                            {
                                globals.license_state = "VALID-LITE";
                                sqlConnection1.Close();
                                return true;
                            }
                        }

                        if (valid_irace && !valid_lic)
                        {

                            globals.license_state = "IRACING";
                            sqlConnection1.Close();
                            return true;
                        }

                        if (valid_lic && valid_irace)
                        {
                            globals.license_state = "VALID-IRACING";
                            sqlConnection1.Close();
                            return true;
                        }

                        if (valid_lic && !valid_irace)
                        {
                            globals.license_state = "VALID";
                            sqlConnection1.Close();
                            return true;
                        }
                    }
                    else
                    {
                            if (valid_irace) globals.license_state = "DEMO-IRACING"; else
                            globals.license_state = "DEMO";
                            sqlConnection1.Close();
                            return true;
                    }
                    if (dbauth.Contains("REVOKE") || dbauth.Contains("NONE"))
                    {
                        MessageBox.Show("License Registration Failed.\r\nThis PC is not valid.\r\nPlease contact support@racevoice.com", "License Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        globals.license_state = "REVOKE";
                        sqlConnection1.Close();
                        globals.Terminate();

                    }

                    globals.license_state = "EXPIRED";
                   MessageBox.Show("Your RaceVoice License has expired.\r\nYou can still continue to use RaceVoice\r\nbut you will no longer get track updates or new features.\r\nPlease goto RaceVoice.com to renew your license.", "License Expired", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                   sqlConnection1.Close();
                   return true;
                    
                }

                if (match_email && !match_uuid)
                {
                    if (dbuuid.Length > 0 && dbuuid.Contains("NONE") == false)
                    {
                        uuid = dbuuid + "," + uuid; // append a computer ID
                        //string str = "Expected=" + uuid + "\r\nFound=" + dbuuid;
                        //MessageBox.Show("This RaceVoice is already Licensed to another PC.\r\nPlease contact support@racevoice.com for assistance.\r\n"+str, "License Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        //sqlConnection1.Close();
                        //Application.Exit();
                    }
                    // tell the user we are going to license them for the first time
                    MessageBox.Show("Thank you for Purchasing Your RaceVoice!\r\nWe will now license your PC for use of RaceVoice Studio.\r\nThe license entitles you to free track map updates, feature requests, feature updates, and support@racevoice.com.", "License Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    int ac = 1;
                    string timedate = DateTime.Now.ToString("ddd, dd MMM yyy HH:mm:ss GMT");
                    cmd.CommandText = "UPDATE license SET name = @name, uuid = @uuid, lastaccess = @lastaccess, accesscount = @accesscount  Where email = @email";
                    cmd.Parameters.AddWithValue("@name", user_name);
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    cmd.Parameters.AddWithValue("@lastaccess", timedate);
                    cmd.Parameters.AddWithValue("@accesscount", ac);
                    cmd.Parameters.AddWithValue("@email", email_address);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        val_status = true;
                        string trackPath = globals.LocalFolder() + "\\tracks";
                        try
                        {
                            System.IO.DirectoryInfo di = new DirectoryInfo(trackPath);

                            foreach (FileInfo file in di.GetFiles())
                            {
                                globals.WriteLine("DELETE->" + file.Name);
                                file.Delete();
                            }
                        }
                        catch (Exception ee)
                        {
                            globals.WriteLine(ee.Message);
                        }
                        globals.virgin_load = true;
                        globals.license_state = "RELOAD";
                        email em = new email();
                        string demostring = "User:[" + user_name + "] Email=[" + email_address + "] Has Registered RaceVoice";
                        em.SendRegEmail(demostring);
                        MessageBox.Show("Successfully Licensed - Thank You", "License Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    }
                    catch (Exception ee)
                    {
                        globals.WriteLine(ee.Message);
                        MessageBox.Show("License Registration Failed. Plese contact support@racevoice.com", "License Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        all_stop = true;
                        val_status = false;
                    }
                }

                if (!match_email && !match_uuid)
                {
                    if (dbuuid.Length > 0 && dbuuid.Contains("NONE") == false)
                    {
                        uuid = dbuuid + "," + uuid; // append a computer ID
                    }
                    // tell the user we are going to license them for the first time
                    MessageBox.Show("Your email was not found\r\nRaceVoice Studio will be activated in Demonstration Mode.\r\nThis includes a 10-day trial to RaceVoiceSIM", "License Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    int ac = 1;
                    string timedate = DateTime.Now.ToString("ddd, dd MMM yyy HH:mm:ss GMT");
                    cmd.CommandText = "INSERT INTO license VALUES (@name, @uuid, @lastaccess, @email, @authorization, @purchasedate, @accesscount, @swversion, @unitversion, @expire, @iracingnode)";
                    dbauth = "DEMO-IRACING";
                    DateTime tnow= Convert.ToDateTime(globals.network_time);
                    tnow = tnow.AddDays(10);
                    string expire_time = tnow.ToString("MM/dd/yyyy");
                    cmd.Parameters.AddWithValue("@name", user_name);
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    cmd.Parameters.AddWithValue("@lastaccess", timedate);
                    cmd.Parameters.AddWithValue("@email", email_address);
                    cmd.Parameters.AddWithValue("@authorization", dbauth);
                    cmd.Parameters.AddWithValue("@purchasedate", timedate);
                    cmd.Parameters.AddWithValue("@accesscount", ac);
                    cmd.Parameters.AddWithValue("@swversion", globals.UIVersion);
                    cmd.Parameters.AddWithValue("@unitversion",dbauth);
                    cmd.Parameters.AddWithValue("@expire", expire_time);
                    cmd.Parameters.AddWithValue("@iracingnode", this_pc_uuid);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        val_status = true;
                        string trackPath = globals.LocalFolder() + "\\tracks";
                        try
                        {
                            System.IO.DirectoryInfo di = new DirectoryInfo(trackPath);

                            foreach (FileInfo file in di.GetFiles())
                            {
                                globals.WriteLine("DELETE->" + file.Name);
                                // file.Delete();
                            }
                        }
                        catch (Exception ee)
                        {
                            globals.WriteLine(ee.Message);
                        }
                        globals.virgin_load = true;
                        globals.license_state = "RELOAD";
                        email em = new email();
                        string demostring = "User:[" + user_name + "] Email=[" + email_address + "] Has Registered RaceVoice *Demo*";
                        em.SendRegEmail(demostring);
                        MessageBox.Show("Demo Mode Enabled - Thank You", "License Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch (Exception ee)
                    {
                        globals.WriteLine(ee.Message);
                        MessageBox.Show("License Registration Failed. Plese contact support@racevoice.com", "License Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        all_stop = true;
                        val_status = false;
                    }
                }
            }
            catch (Exception ee)
            {
                globals.WriteLine(ee.Message);
            }

            sqlConnection1.Close();

            if (all_stop)
            {
                globals.all_stop = true;
                globals.Terminate();

            }
            return val_status;
        }
    }
}