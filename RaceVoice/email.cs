using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Mail;


namespace RaceVoice
{
    class email
    {

        public Thread xEmailTrackFile(string path)
        {
            var t = new Thread(() => SendEmail("New Track Has Been Added", path));
            t.Start();
            return t;
        }

        public bool EmailTrackFile(string path)
        {
            SendEmail("New Track Has Been Added", path);
            return true;
        }

        private bool SendEmail(string subject,string path)
        {
            bool good = true;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            System.Net.Mail.Attachment attachment;
            try
            {
                path=globals.LocalFolder()+"\\tracks\\" + path;
                mail.From = new MailAddress("racevoicestudio@racevoice.com");
                mail.To.Add("steve@racevoice.com");
                mail.Subject = subject +"Site="+globals.theUUID;
                mail.Body = subject+"\r\nmail with attachment";

                attachment = new System.Net.Mail.Attachment(path);
                mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("racevoicestudio@racevoice.com", "#InFieldUpdate#");
                SmtpServer.EnableSsl = true;
                SmtpServer.SendCompleted += (s, e) => {
                    SmtpServer.Dispose();
                    mail.Dispose();
                };
                SmtpServer.SendAsync(mail, mail);
            }
            catch (Exception ex)
            {
                globals.WriteLine(ex.ToString());
                good = false;
            }

            if (good) globals.WriteLine("Emailed Track-->" + path);
            return good;
        }
    }
}
