using System.Net.Mail;
using System.Net;

namespace PayrollAPI.Data
{
    public class EmailSender
    {
        public async Task<bool> SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress("bhagyaj@cpstl.lk");
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;

                // Attachment
                var attachment = new Attachment("C:\\Users\\17532\\source\\repos\\pavithra91\\PayrollAPI\\PayrollAPI\\output.txt");
                message.Attachments.Add(attachment);

                var smtpClient = new SmtpClient("smtp.office365.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("bhagyaj@cpstl.lk", "");
                smtpClient.EnableSsl = true;


                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
