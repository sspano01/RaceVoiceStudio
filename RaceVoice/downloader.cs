using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace RaceVoice
{
    internal class FileDownloader
    {
        private readonly string _url;
        private readonly string _fullPathWhereToSave;
        private bool _result = false;
        private bool rundl = true;

        public FileDownloader(string url, string fullPathWhereToSave)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(fullPathWhereToSave)) throw new ArgumentNullException("fullPathWhereToSave");

            this._url = url;
            this._fullPathWhereToSave = fullPathWhereToSave;
        }

        public bool StartDownload(int timeout)
        {
            try
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(_fullPathWhereToSave));

                if (File.Exists(_fullPathWhereToSave))
                {
                    File.Delete(_fullPathWhereToSave);
                }
                using (WebClient client = new WebClient())
                {
                    rundl = true;
                    int to = 0;
                    var ur = new Uri(_url);
                    // client.Credentials = new NetworkCredential("username", "password");
                    client.DownloadProgressChanged += WebClientDownloadProgressChanged;
                    client.DownloadFileCompleted += WebClientDownloadCompleted;
                    globals.WriteLine(@"Downloading file:");
                    client.DownloadFileAsync(ur, _fullPathWhereToSave);
                    while (rundl)
                    {
                        Thread.Sleep(1000);
                        Application.DoEvents();
                        to += 1000;
                        if (to > timeout) break;
                        if (client.IsBusy == false) break;
                    }
                    return _result && File.Exists(_fullPathWhereToSave);
                }
            }
            catch (Exception e)
            {
                globals.WriteLine("Was not able to download file!");
                Console.Write(e);
                return false;
            }
        }

        private void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Write("\r     -->    {0}%.", e.ProgressPercentage);
        }

        private void WebClientDownloadCompleted(object sender, AsyncCompletedEventArgs args)
        {
            _result = !args.Cancelled;
            if (!_result)
            {
                Console.Write(args.Error.ToString());
            }
            globals.WriteLine(Environment.NewLine + "Download finished!");
            rundl = false;
        }

        public static bool xDownloadFile(string url, string fullPathWhereToSave, int timeoutInMilliSec)
        {
            return new FileDownloader(url, fullPathWhereToSave).StartDownload(timeoutInMilliSec);
        }

        public static bool xDownloadFile(string url, string fullPathWhereToSave)
        {
            return new FileDownloader(url, fullPathWhereToSave).StartDownload(60000);
        }

        public bool DownloadFile(string filename, string localname)
        {
            if (localname.Length == 0) localname = filename;

            string fileUrl = "http://" + globals.racevoice_http + "/" + filename;
            string savefile = globals.LocalFolder() + "\\" + localname;
            var success = xDownloadFile(fileUrl, savefile);
            globals.WriteLine("Done  - success: " + success);
            return (Convert.ToBoolean(success));
        }
    }
}