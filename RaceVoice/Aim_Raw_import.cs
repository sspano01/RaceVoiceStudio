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
using Ionic.Zip;
using RaceVoice;



namespace RaceVoice
{
    class import
    {



        //        https://sourceforge.net/projects/geographiclib/files/distrib/
        // needs to be changed to used actual fractional interpolation method


        public string ImportRawCSV(string file, string trackname)
        {
            string tn = trackname;
            string line = "";
            string outname = "";
            Console.WriteLine("Generating->" + file + "--->" + trackname);
            System.IO.StreamReader ifile = new System.IO.StreamReader(file);
            outname= globals.LocalFolder() + "//tracks//" + trackname.Replace(' ', '_') + ".csv";
            System.IO.StreamWriter ofile = new System.IO.StreamWriter(outname);
            bool readin = false;
            if (ifile!=null && ofile!=null)
            {
                ofile.WriteLine("Lattitude,Longitude,Misc,Misc");
                ofile.WriteLine("samplerate,1");
                while ((line = ifile.ReadLine()) != null)
                {
                    readin = true;
                    string[] segs = line.Split(',');
                    if (segs.Count()!=2)
                    {
                        tn = "";
                        break;
                    }
                    try
                    {
                        double lat = Convert.ToDouble(segs[0]);
                        double lng = Convert.ToDouble(segs[1]);
                        string otxt = lat.ToString() + "," + lng.ToString() + ",0,0";
                        ofile.WriteLine(otxt);

                    }
                    catch (Exception ee)
                    {
                        globals.WriteLine(ee.Message);
                        tn = "";
                        break;
                    }

                }
                ifile.Close();
                ofile.Close();
            }

            if (!readin) tn = "";
            if (tn.Length == 0) File.Delete(outname);
            return tn;
        }

        public string ImportZTRACK(string file,string outfile)
        {
            bool valid = true;
            string track_name = "";
            MemoryStream tempS = new MemoryStream();
            byte[] fdata;
            try
            {
                globals.WriteLine("Aim ZTRACK IMPORT:" + file);
                ZipFile zip = ZipFile.Read(file);
                ZipEntry e = zip.First();
                e.Extract(tempS);
                fdata = tempS.ToArray();
                track_name=TKKtoCSV(fdata,outfile);

              

            }
            catch (Exception ee)
            {
                valid = false;
                globals.WriteLine("Ztrack Import Error:" + ee.Message);
           }
            
            if (valid)
            {
                return track_name;
            }


            return "";

        }

        private string toGPS(byte[] array, int offset)
        {
            string txt = "";
            int lat = 0;
            int lng = 0;
            double dlat = 0;
            double dlng = 0;
            byte[] lgps = new byte[8];
            for (int i = 0; i < 8; i++) lgps[i] = array[offset + i];
            lat = (lgps[3] << 24) | (lgps[2] << 16) | (lgps[1] << 8) | (lgps[0]);
            lng = (lgps[7] << 24) | (lgps[6] << 16) | (lgps[5] << 8) | (lgps[4]);
            dlat = Convert.ToDouble(lat) / 1e7;
            dlng = Convert.ToDouble(lng) / 1e7;
            txt = dlat.ToString() + "," + dlng.ToString() + ",0,0";

            if (dlat > 100 && dlng > 100) txt = "";
            //Console.WriteLine("GPS->" + txt);
            return txt;
        }

        private string TKKtoCSV(byte[] fdata,string trackname)
        {
            bool valid = true;
            string ztrackname = "";
            int offset = 16;

            try
            {
                for (int i = 0; i < 64; i++)
                {
                    if (fdata[offset + i] == 0) break;
                    ztrackname += Convert.ToChar(fdata[offset + i]);
                }

                Console.WriteLine("Generating->" + ztrackname+"--->"+trackname);
                string oname = globals.LocalFolder() + "\\tracks\\" + trackname.Replace(' ', '_') + ".csv";
                System.IO.StreamWriter ofile = new System.IO.StreamWriter(oname);


                offset = 0x448;
                bool first = true;
                string wl = "";
                int ct = 0;
                double last_lat, last_lng = 0;
                double lat, lng = 0;

                last_lat = 0;
                lat = 0;
                lng = 0;
                last_lng = 0;

                for (int i = offset; i < fdata.Length; i = i + 12)
                {
                            globals.WriteLine("Mark @ " + i);

                    if (first)
                    {
                        if (fdata[i] == 0 && fdata[i + 1] == 0 && fdata[i + 2] == 0 && fdata[i + 3] == 0x3e)
                        {
                            //globals.WriteLine("Start Mark @ " + i);
                            ofile.WriteLine("Lattitude,Longitude,Misc,Misc," + trackname);
                            ofile.WriteLine("samplerate,1");
                            wl = toGPS(fdata, (i + 4));
                            first = false;

                        }
                    }
                    else
                    {
                        wl = toGPS(fdata, i+4);
                        string[] wls = wl.Split(',');
                        if (wls.Length >= 4)
                        {
                            lat = Convert.ToDouble(wls[0]);
                            lng = Convert.ToDouble(wls[1]);

                            if (last_lat != 0 && last_lng != 0)
                            {
                                if (Math.Abs(lat-last_lat)>1 || Math.Abs(lng-last_lng)>1) break;
                            }
                            last_lat = lat;
                            last_lng = lng;
                        }
                        else
                        {

                            break;
                        }
                        if (wl.Length > 0)
                        {

                            globals.WriteLine("Writing @ " + i + " " + wl);
                            ofile.WriteLine(wl);
                        }
                    }

                    ct++;
                    //first = false;
                }


                /*
                for (int i = offset; i < fdata.Length; i = i + 12)
                {
                    if (first)
                    {
                        if (fdata[i] == 0 && fdata[i + 1] == 0 && fdata[i + 2] == 0 && fdata[i + 3] == 0x3e)
                        {
                            //globals.WriteLine("Start Mark @ " + i);
                            ofile.WriteLine("Lattitude,Longitude,Misc,Misc," + trackname);
                            ofile.WriteLine("samplerate,1");
                            wl = toGPS(fdata, (i + 4));

                        }
                    }
                    else
                    {
                        if (fdata[i] == 0 && fdata[i + 1] == 0 && fdata[i + 2] == 0 && fdata[i + 3] == 0)
                        {
                            globals.WriteLine("Mark @ " + i);

                            wl = toGPS(fdata, (i + 4));
                            if (wl.Length > 0)
                            {

                                globals.WriteLine("Writing @ " + i);
                                ofile.WriteLine(wl);
                            }


                        }
                        else
                        {
                            globals.WriteLine("Skipping @ " + i);
                            //break;
                        }

                    }
                    ct++;
                    first = false;
                }
                */
                ofile.Close();
            }
            catch (Exception ee)
            {
                globals.WriteLine("TKK Convert Error:" + ee.Message);
                valid = false;
            }


            if (valid) return trackname;
            return "";
        }

        public bool Parse(bool onefile)
        {
            bool makecsv = false;
            string path = "c:\\aim_sport\\racestudio3\\aim_tracks";

            int cnt = 0;

            if (onefile)
            {
                path = globals.LocalFolder() + "\\imports";
            }
            // Take a snapshot of the file system.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);

            // This method assumes that the application has discovery permissions
            // for all folders under the specified path.
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            //Create the query
            IEnumerable<System.IO.FileInfo> fileQuery =
                from file in fileList
                where file.Extension == ".tkk"
                orderby file.Name
                select file;

            System.IO.StreamWriter lfile = new System.IO.StreamWriter(globals.LocalFolder() + "//imports//import.txt");

            string trackname = "";
            //Execute the query. This might write out a lot of files!
            foreach (System.IO.FileInfo fi in fileQuery)
            {
                //globals.WriteLine(fi.FullName);

                cnt++;
               // if (cnt > 20) break;
                // now open each file and read the first line to extract the name
                // Read the file and display it line by line.  
                try
                {
                  //  string fn = "C:\\AIM_SPORT\\RaceStudio3\\aim_tracks\\05b5a8sd.tkk";
                    string fn = fi.FullName;
                    byte[] fdata = File.ReadAllBytes(fn);
                    trackname = "";
                    int offset = 16;
                    for (int i = 0; i < 64; i++)
                    {
                        if (fdata[offset + i] == 0) break;
                        trackname += Convert.ToChar(fdata[offset + i]);
                    }

                 //   makecsv=false;
                 //   if (trackname.ToLower().Contains("barber")) makecsv = true;
                 //   if (trackname.ToLower().Contains("blackhawk")) makecsv = true;
                 //   if (trackname.ToLower().Contains("iowa")) makecsv = true;
                  //  if (trackname.ToLower().Contains("plains")) makecsv = true;
                 //   if (trackname.ToLower().Contains("heartland")) makecsv = true;
                  //  if (trackname.ToLower().Contains("charlotte")) makecsv = true;
                  //  if (trackname.ToLower().Contains("homestead")) makecsv = true;
                  //  if (trackname.ToLower().Contains("auto club")) makecsv = true;
                   // if (trackname.ToLower().Contains("willow")) makecsv = true;
                   // if (trackname.ToLower().Contains("sonoma")) makecsv = true;
                  //  if (trackname.ToLower().Contains("thunderhill")) makecsv = true;
                   // if (trackname.ToLower().Contains("portland")) makecsv = true;
                   // if (trackname.ToLower().Contains("roebling")) makecsv = true;
                    //makecsv = true;

                    makecsv = false;
                    //if (trackname.ToLower().Contains("fontana")) makecsv = true;
                    //if (trackname.ToLower().Contains("inde")) makecsv = true;
                    //if (trackname.ToLower().Contains("firebird")) makecsv = true;
                    //if (trackname.ToLower().Contains("arizona")) makecsv = true;
                    //if (trackname.ToLower().Contains("auto")) makecsv = true;

                    // if (trackname.ToLower().Contains("brainerd")) makecsv=true;
                    // if (trackname.ToLower().Contains("laguna")) makecsv=true;
                    //if (trackname.ToLower().Contains("nola")) makecsv = true;
                    // if (trackname.ToLower().Contains("pueblo")) makecsv = true;
                    //  if (trackname.ToLower().Contains("safety")) makecsv = true;

                    if (onefile) makecsv = true;
                    if (makecsv)
                    {

                    }
                        
                }
                catch (Exception ee)
                {
                   MessageBox.Show("ERROR: Cannot load track files\r\n" + ee, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    globals.Terminate();
                }

                //trackList.SelectedIndex = 0;
                string txt = fi.Name + "-->" + trackname;
                lfile.WriteLine(txt);
                //globals.WriteLine(txt);

            }


            lfile.Close();
            MessageBox.Show("DONE");
            return true;

        }
    }
}
