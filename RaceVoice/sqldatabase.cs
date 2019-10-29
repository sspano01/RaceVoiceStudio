using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Data;
using System.Windows.Forms;

namespace RaceVoice
{
    internal class sqldatabase
    {


        public bool ValidateUUID(string uuid,bool set, CarMetadata cm)
        {
            string connetionString;
            bool val_status = false;
            string dbauth = "";
            string dbuuid = "";
            string email_address = "none";
            string user_name = "";
            int access_count = 0;
            bool all_stop = false;
            connetionString = "Data Source=" + globals.racevoice_sqlserver + "; Initial Catalog = racevoice;Integrated Security=False;User ID=root;Password=#RaceVoice01;connection timeout=30";

            //connectionString="Data Source=104.155.20.171;Initial Catalog=bookshelf;Integrated Security=False;User ID=dotnetapp;Password=test;MultipleActiveResultSets=True"

            //Server = myServerAddress; Database = myDataBase; User Id = myUsername;
            //Password = myPassword;
            //connetionString="<add name="YOUR_NAME_HERE" connectionString="Server=***.***.**.***;Database=YOUR_DATABASE_NAME;Uid=YOUR_USERNAME;Pwd=YOUR_USER_PASSWORD" providerName="MySql.Data.MySqlClient" />"
            // string conStr = "Server=xxx.xxx.xxx.xxx;Database=DB_NAME;Uid=USER_NAME;Password=PASSWORD;

            uuid = uuid.ToUpper().Trim();
            MySqlConnection sqlConnection1 = new MySqlConnection(connetionString);
            MySqlCommand cmd = new MySqlCommand();
            MySqlDataReader reader;

            try
            {
                sqlConnection1.Open();

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
                    string timedate = DateTime.Now.ToString("ddd, dd MMM yyy HH:mm:ss GMT");
                    cmd.CommandText = "UPDATE license SET lastaccess = @lastaccess, accesscount = @accesscount Where uuid like '%" + uuid + "%'";
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
                    if (dbauth.Contains("VALID"))
                    {
                        if (dbauth.Contains("LITE"))
                        {
                            globals.license_state = "VALID LITE";
                        }
                        else
                        {
                            globals.license_state = "VALID";
                        }
                        sqlConnection1.Close();
                        return true;
                    }
                    if (dbauth.Contains("DEMO"))
                    {
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
                    cmd.CommandText = "UPDATE license SET name = @name, uuid = @uuid, lastaccess = @lastaccess, accesscount = @accesscount Where email = @email";
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
                    MessageBox.Show("Your email was not found\r\nRaceVoice Studio will be activated in Demonstration Mode.\r\n", "License Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    int ac = 1;
                    string timedate = DateTime.Now.ToString("ddd, dd MMM yyy HH:mm:ss GMT");
                    cmd.CommandText = "INSERT INTO license VALUES (@name, @uuid, @lastaccess, @email, @authorization, @purchasedate, @accesscount, @swversion, @unitversion)";
                    dbauth = "demo";
                    cmd.Parameters.AddWithValue("@name", user_name);
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    cmd.Parameters.AddWithValue("@lastaccess", timedate);
                    cmd.Parameters.AddWithValue("@email", email_address);
                    cmd.Parameters.AddWithValue("@authorization", dbauth);
                    cmd.Parameters.AddWithValue("@purchasedate", timedate);
                    cmd.Parameters.AddWithValue("@accesscount", ac);
                    cmd.Parameters.AddWithValue("@swversion", globals.UIVersion);
                    cmd.Parameters.AddWithValue("@unitversion",dbauth);
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