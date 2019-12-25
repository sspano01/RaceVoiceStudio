using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Text;
#if (!APP)
using System.Windows.Forms;
#endif

#if (APP)
namespace RaceVoice
#else
namespace RaceVoice
#endif
{
    internal class FileDownloader
    {

        private void iDownloadFile(string src, string dst)
        {

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                var response = client.GetAsync(src).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;

                   // Console.WriteLine(responseString);
                    byte[] s_bytes = Encoding.ASCII.GetBytes(responseString);
                    File.WriteAllBytes(dst, s_bytes);
                    return;
                }
            }

        }

        public bool DownloadFile(string filename, string localname)
        {
            bool success = true;
            if (localname.Length == 0) localname = filename;

            string fileUrl = "http://" + globals.racevoice_http + "/" + filename;
#if APP
            string savefile = globals.LocalFolder() + "/" + localname;
#else
            string savefile = globals.LocalFolder() + "\\" + localname;
#endif
            globals.WriteLine("Read [" + fileUrl + "] and save to [" + savefile + "]");
            iDownloadFile(fileUrl, savefile);
            globals.WriteLine("Done  - success: " + success);
            
            return (Convert.ToBoolean(success));
        }
    }
}