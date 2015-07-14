using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace frauddetect.common.core.email
{
    public class EmailManager
    {
        public EmailManager(string fromAddress, string toAddress, string emailSubject, string emailBody)
        {
            FromAddress = fromAddress;
            ToAddress = toAddress;
            EmailSubject = emailSubject;
            EmailBody = emailBody;
        }
        public string ToAddress { get; set; }
        public string FromAddress { get; set; }

        public string MailServer { get; set; }

        public string EmailBody { get; set; }

        public string EmailSubject { get; set; }

        public int SmtpPort { get; set; }

        public NetworkCredential EmailAccountCredential { get; set;}

        public bool EnableEmailSSL { get; set; }

        public void Send()
        {
            try
            {
                MailMessage msg = new MailMessage(FromAddress, ToAddress, EmailSubject, EmailBody);
                //send the message
                SmtpClient smtp = new SmtpClient(MailServer);
                smtp.Port = SmtpPort;
                smtp.EnableSsl = EnableEmailSSL;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = EmailAccountCredential;
                smtp.Send(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
