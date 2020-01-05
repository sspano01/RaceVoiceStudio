using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using IntelHexFormatReader;
using IntelHexFormatReader.Model;
using System.Security.Cryptography;

namespace RaceVoice
{
    class firmware
    {
        string fw_file_month = "";
        string fw_file_day = "";
        string fw_file_year = "";
        string fw_file_version = "";

        string fw_unit_month = "";
        string fw_unit_day = "";
        string fw_unit_year = "";
        string fw_unit_version = "";

        private void GenFWVersion(CarMetadata _metadata)
        {
            
            string[] vsplit = _metadata.HardwareData.Version.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
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


        }

        private int byteSearch(byte[] src, byte[] pattern)
        {
            int c = src.Length - pattern.Length + 1;
            int j;
            for (int i = 0; i < c; i++)
            {
                if (src[i] != pattern[0]) continue;
                for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
                if (j == 0) return i;
            }
            return -1;
        }

        private bool IsFirmwareLess(string date)
        {
            string mm = date.Substring(0, 2);
            string dd = date.Substring(2, 2);
            string yy = date.Substring(4, 4);
            if (fw_unit_year.Length == 0) return false;

            DateTime fw_file_date = new DateTime(Convert.ToInt32(yy), Convert.ToInt32(mm), Convert.ToInt32(dd));
            DateTime fw_unit_date = new DateTime(Convert.ToInt32(fw_unit_year), globals.FwMonth(fw_unit_month), Convert.ToInt32(fw_unit_day));

            if (DateTime.Compare(fw_unit_date, fw_file_date) < 0) return true; 

            return false;

        }


        public int UpdateFirmware()
        {
            string fwm = "";
            string message = "";
            string line = "";
            int reboot_good = -1;
            bool abort = false;
            DialogResult dr;
            bool flash_only = false ;
            int apply_patch = 0;
            string fwmsg = "CRITICAL ERROR";
            System.IO.StreamReader file = null;
            racevoicecom rvcom = new racevoicecom();


            if (IsFirmwareLess("01162019"))
            {
                fwm = "New firmware for your RaceVoice is available\r\n";
                fwm += "Current Firmware -> " + fw_unit_month + " " + fw_unit_day + " " + fw_unit_year + " " + fw_unit_version + "\r\n";
                fwm += "New Firmware -> " + fw_file_month + " " + fw_file_day + " " + fw_file_year + " " + fw_file_version + "\r\n";
                fwm += "However, this unit is not capable of an in-field firmware update\r\n";
                fwm += "Re-install your previous version of RaceVoice Studio to keep using RaceVoice\r\n";
                fwm += "You can continue to use RaceVoice Studio with your current unit.\r\n";
                fwm += "Please contact support@racevoice.com to arrainge to have your unit firmware updated\r\n";
                dr = MessageBox.Show(fwm, "Firmware Update", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return -1;
            }

            if (fw_unit_month.ToUpper().Equals("MAY") && fw_unit_year.ToUpper().Equals("2019") && fw_unit_version.ToUpper().Equals("A1"))
            {
                fwm = "New Firmware is available for your RaceVoice unit\r\n";
                fwm += "In order to update from your current May 8,2019 A1 Firmware\r\n";
                fwm += "A two-step update process is required.\r\n";
                fwm += "Press YES to Update or No to Abort\r\n";
                apply_patch = 1;
            }
            else
            {
                fwm = "New firmware for your RaceVoice is available\r\n";
                fwm += "Current Firmware -> " + fw_unit_month + " " + fw_unit_day + " " + fw_unit_year + " " + fw_unit_version + "\r\n";
                fwm += "New Firmware -> " + fw_file_month + " " + fw_file_day + " " + fw_file_year + " " + fw_file_version + "\r\n";
                fwm += "Press YES to Update or No to Abort\r\n";
                fwm += "Note: Some features of RaceVoice Studio may not be available if you do not update\r\n";
            }
            dr = MessageBox.Show(fwm, "Firmware Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                splash isplash = new splash(1);
                string fn = globals.LocalFolder() + "\\firmware.hex";
                if (apply_patch==1)
                {
                    fn = globals.LocalFolder() + "\\050819patch.hex";
                }
                isplash.Show();
                rvcom.OpenSerial();
                isplash.setlabel("Erasing memory....");
                rvcom.ReadTimeout(20000);
                message = rvcom.SendSerial("fwerase");
                Thread.Sleep(10);
                message = rvcom.ReadLine(); // this should be a reply
                message = message.ToUpper();

                globals.WriteLine("FWErase Reply Message->" + message);
                if (message.Contains("PASS") || (apply_patch==1 && message.Contains("DONE")))
                {
                    isplash.setlabel("Downloading....");
                    message = rvcom.SendSerial("fwupdate");
                    int sync = 0;
                    while (sync<20)
                    {
                        message = rvcom.ReadLine();
                        if (message.ToUpper().Contains("FWUPDATE"))
                        {
                            globals.WriteLine("Message=>" + message); // there is no reply here ....
                            break;
                        }
                        Thread.Sleep(10);
                        sync++;

                        if (sync>10)
                        {
                            MessageBox.Show("Error Writing Firmware - Line Sync Error(1)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            abort = true;
                            break;

                        }
                    }

                    // now read in all lines of the HEX file and send them down the unit
                    // Read the file and display it line by line.  

                    if (!flash_only)
                    {
                        int ii = 0;
                        int lineCount = File.ReadAllLines(fn).Length;
                        file = new System.IO.StreamReader(fn);
                        while ((line = file.ReadLine()) != null && abort == false)
                        {
                            ii++;
                            message = rvcom.SendSerial(line);
                            message = message.ToUpper();
                            sync = 0;
                            while (sync < 50)
                            {
                                if (message.Contains("SHELL>") && message.Contains(line.ToUpper()))
                                {
                                    // make sure we got the echo of the actual line we want to send
                                    // now read the actual reply to the firmware download response
                                    message = rvcom.ReadLine();
                                    message = message.ToUpper();
                                    break;
                                }
                                else
                                {
                                    sync++;
                                    message = rvcom.ReadLine();
                                    if (sync > 20)
                                    {
                                        MessageBox.Show("Error Writing Firmware - Line Sync Error(2)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        abort = true;
                                        break;
                                    }

                                }
                            }

                            if (message.Contains("TYPE=0x00"))
                            {
                                if (message.Contains("VALID=0"))
                                {
                                    MessageBox.Show("Error Writing Firmware", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    abort = true;
                                    break;
                                }
                            }
                            globals.WriteLine(ii + "/" + lineCount + " Reply->" + message);
                            double pct = Convert.ToDouble(ii) / Convert.ToDouble(lineCount);
                            pct *= Convert.ToDouble(100);
                            isplash.setbar(Convert.ToInt32(pct));
                        }
                    }

                    if (abort == false)
                    {
                        isplash.setlabel("Updating....DO NOT REMOVE POWER FROM RACEVOICE");
                        Thread.Sleep(500);
                        message = rvcom.SendSerial("fwcommit",null,null);
                        globals.WriteLine("Reply->" + message);

                        bool read_out = false;
                        rvcom.ReadTimeout(5000); // standard timeout
                        rvcom.Bar(0);
                        rvcom.Bar(50);
                        for (int j = 0; j < 50; j++)
                        {
                            if (reboot_good==1) break;
                            rvcom.Bar(j);
                            Thread.Sleep(1000); // wait for reboot
                            isplash.setlabel("Waiting for reboot.....DO NOT REMOVE POWER FROM RACEVOICE");
                            try
                            {
                                read_out = true;
                                while (read_out)
                                {
                                    fwmsg = rvcom.ReadLine().ToUpper();
                                    globals.WriteLine("Read--->" + fwmsg);
                                    if (fwmsg.Contains("VERSION"))
                                    {
                                        rvcom.FirmwareDone();
                                        reboot_good = 1;
                                        read_out = false;
                                        break;
                                    }

                                    Thread.Sleep(1);
                                }
                            }
                            catch (Exception ee)
                            {
                                // as racevoice is rebooting ... this readline will probably time out ... that's ok
                                Console.WriteLine(ee.Message);
                            }
                        }
                        if (reboot_good==1)
                        {
                            MessageBox.Show("Update Success\r\nNew Version\r\n"+fwmsg, "Firmware Update Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Firmware Update Failure - No Response from Unit\r\nPlease turn the power off and then back on to the RaceVoice\r\nReconnect and try again", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        isplash.Close();
                        if (file!=null)  file.Close();
                    }
                    else
                    {
                        MessageBox.Show("Please turn the power off and then back on to the RaceVoice\r\nReconnect and try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                else
                {
                    MessageBox.Show("Error Erasing Memory", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }



                rvcom.CloseSerial();
            }
            else
            {
                reboot_good = 0;
            }

            if (reboot_good==1)
            {
                if (apply_patch==1)
                {
                    MessageBox.Show("RaceVoice Studio will now close.\r\nPlease restart RaceVoice studio to complete the second step of the update process\r\nYou will be prompted for an additional firmware update.", "Step-Two", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    globals.Terminate();

                }
            }
            return reboot_good;
        }

        public bool xComparePackagedFirmwareVersion(CarMetadata _metadata)
        {
            return false;
        }
        public bool ComparePackagedFirmwareVersion(CarMetadata _metadata)
        {
            int offset = 0;

            try
            {
                // now, read in the firmware file that was packaged with the UI
                string fn = globals.LocalFolder() + "\\firmware.hex";
                HexFileReader reader = new HexFileReader(fn, 512 * 1024);
                MemoryBlock memoryRepresentation = reader.Parse();
                globals.WriteLine(memoryRepresentation.ToString());
                byte[] rawmem = new byte[1024 * 512];
                foreach (MemoryCell cell in memoryRepresentation.Cells)
                {
                    rawmem[cell.Address] = cell.Value;
                }
                byte[] pattern = new byte[8];
                pattern[0] = (byte)'V';
                pattern[1] = (byte)'e';
                pattern[2] = (byte)'r';
                pattern[3] = (byte)'s';
                pattern[4] = (byte)'i';
                pattern[5] = (byte)'o';
                pattern[6] = (byte)'n';
                pattern[7] = (byte)',';

                offset = byteSearch(rawmem, pattern);
                if (offset > 0)
                {
                    string vs = "";
                    offset -= 13; // backup so we get the "Race Voice," string
                    for (int i = offset; i < offset + 128; i++)
                    {
                        vs += (char)rawmem[i];
                    }
                    //clean up the string a bit
                    vs = vs.Replace("\0", string.Empty);
                    vs = vs.Replace("%s", string.Empty);
                    globals.WriteLine("VERSION-->" + vs);

                    // now split it apart with commas
                    //File Read Step = 0 Val->RACE
                    //File Read Step = 1 Val->VOICE
                    //File Read Step = 2 Val->VERSION  (BETAVERSION)
                    //File Read Step = 3 Val->JANUARY
                    //File Read Step = 4 Val->18
                    //File Read Step = 5 Val->2019
                    //File Read Step = 6 Val->A1RACE
                    string[] vsplit = vs.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < vsplit.Length; i++)
                    {
                        string ev = vsplit[i].Trim().ToUpper();
                        globals.WriteLine("File Read Step = " + i + " Val->" + ev);
                        switch (i)
                        {
                            case 3: fw_file_month = ev; break;
                            case 4: fw_file_day = ev; break;
                            case 5: fw_file_year = ev; break;
                            case 6: fw_file_version = ev.Substring(0, 2); break;
                            default: break;
                        }
                    }


                    GenFWVersion(_metadata);
                    // now compare against the firmware ID from the unit
                    bool delta = false;
                    if (fw_file_month != fw_unit_month) delta = true;
                    if (fw_file_day != fw_unit_day) delta = true;
                    if (fw_file_year != fw_unit_year) delta = true;
                    if (fw_file_version != fw_unit_version) delta = true;

                    if (fw_file_month.Contains("BETA"))
                    {
                        fw_file_month = fw_file_month.Substring(4);
                    }

                    delta = true;
                    if (delta)
                    {
                        delta = false;
                        if (fw_file_version.StartsWith("Z"))
                        {
                            // force the unit to update with any file version that starts with Z
                            return true;
                        }

                        // is the firmware packaged with the UI newer?
                        // new year?
                        if (Convert.ToInt32(fw_file_year) > Convert.ToInt32(fw_unit_year)) delta = true;

                        // same year
                        if (Convert.ToInt32(fw_file_year) == Convert.ToInt32(fw_unit_year))
                        {
                            // new month?
                            if (globals.FwMonth(fw_file_month) > globals.FwMonth(fw_unit_month)) delta = true;

                            // same month
                            if (globals.FwMonth(fw_file_month) == globals.FwMonth(fw_unit_month))
                            {
                                // new build day?
                                if (Convert.ToInt32(fw_file_day) > Convert.ToInt32(fw_unit_day)) delta = true;
                                // same day
                                if (Convert.ToInt32(fw_file_day) == Convert.ToInt32(fw_unit_day))
                                {
                                    // different build version?
                                    if (fw_file_version != fw_unit_version) delta = true;
                                }
                            }
                        }

                    }
                    return delta;
                }
            }
            catch (Exception ee)
            {
                globals.WriteLine("On Firmware Check " + ee.ToString());
            }
            return false;
        }

    }
}
