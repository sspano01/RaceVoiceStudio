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

        public bool SendRegEmail(string str)
        {
            SendEmail(str,"");
            return true;
        }

        public bool EmailTrackFile(string path)
        {
            SendEmail("New Track Has Been Added", path);
            return true;
        }

        private  bool SendEmail(string subject,string path)
        {
            bool good = true;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            System.Net.Mail.Attachment attachment;
            try
            {
                if (path.Length > 0)
                { 
                path = globals.LocalFolder() + "\\tracks\\" + path;
                }
                mail.From = new MailAddress("racevoicestudio@racevoice.com");
                mail.To.Add("steve@racevoice.com");
                if (subject.ToUpper().Contains("REGISTER"))
                {

                    mail.To.Add("dave@racevoice.com");
                    mail.To.Add("jamesr@pointsw.com");

                }
                mail.Subject = subject +"Site="+globals.theUUID;
                if (path.Length > 0)
                {
                    mail.Body = subject + "\r\nmail with attachment";
                    attachment = new System.Net.Mail.Attachment(path);
                    mail.Attachments.Add(attachment);

                }
                else
                {
                    mail.Body = subject;
             
                }

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
