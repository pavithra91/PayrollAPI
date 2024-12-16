using System.Net.Mail;
using System.Net;
using PayrollAPI.Models;
using System.IO;

namespace PayrollAPI.Services
{
    public class EmailSender
    {
        public async Task<bool> SendEmail(string toEmail, string subject, string body)
        {
            try
            {

                //var client = new SmtpClient("smtp.office365.com", 587)
                //{
                //    Credentials = new NetworkCredential("rnd@cpstl.lk", "cgrflymtflttxbmy"),
                //    EnableSsl = true,
                //    UseDefaultCredentials = false
                //};

                //await client.SendMailAsync("rnd@cpstl.lk", "pavi.dsscst@gmail.com", "Subject", "Body");

                var message = new MailMessage();
                message.From = new MailAddress("rnd@cpstl.lk");
                message.To.Add(toEmail);
                message.Subject = subject;
                message.Body = body;

                // Attachment
                var attachment = new Attachment("C:\\Users\\17532\\source\\repos\\pavithra91\\PayrollAPI\\PayrollAPI\\output.txt");
                message.Attachments.Add(attachment);

                var smtpClient = new SmtpClient("smtp.office365.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("rnd@cpstl.lk", "cgrflymtflttxbmy");
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendEmail(IEnumerable<Sys_Properties> email_configurations)
        {
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(email_configurations.Where(o => o.variable_name == "Email_From").FirstOrDefault().variable_value);
                message.To.Add(email_configurations.Where(o => o.variable_name == "Email_To").FirstOrDefault().variable_value);
                message.Subject = email_configurations.Where(o => o.variable_name == "Email_Subject").FirstOrDefault().variable_value;
                message.Body = email_configurations.Where(o => o.variable_name == "Email_Body").FirstOrDefault().variable_value;

                // Attachment
                var attachment = new Attachment("C:\\Users\\17532\\source\\repos\\pavithra91\\PayrollAPI\\PayrollAPI\\output.txt");
                message.Attachments.Add(attachment);

                var smtpClient = new SmtpClient("smtp.office365.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(email_configurations.Where(o => o.variable_name == "Email_From").FirstOrDefault().variable_value, email_configurations.Where(o => o.variable_name == "Email_Password").FirstOrDefault().variable_value);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(message);
                
                Thread.Sleep(200);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendEmail(IEnumerable<Sys_Properties> email_configurations, byte[] dataArray)
        {
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(email_configurations.Where(o => o.variable_name == "Email_From").FirstOrDefault().variable_value);
                message.To.Add(email_configurations.Where(o => o.variable_name == "Email_To").FirstOrDefault().variable_value);
                message.Subject = email_configurations.Where(o => o.variable_name == "Email_Subject").FirstOrDefault().variable_value;
                message.Body = email_configurations.Where(o => o.variable_name == "Email_Body").FirstOrDefault().variable_value;

                // Attachment
                using var memoryStream = new MemoryStream(dataArray);
                var attachment = new Attachment(memoryStream, "AdvancePayment.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                message.Attachments.Add(attachment);

                var smtpClient = new SmtpClient("smtp.office365.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(email_configurations.Where(o => o.variable_name == "Email_From").FirstOrDefault().variable_value, email_configurations.Where(o => o.variable_name == "Email_Password").FirstOrDefault().variable_value);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(message);

                Thread.Sleep(200);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendEmail(IEnumerable<Sys_Properties> email_configurations, string msg)
        {
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(email_configurations.Where(o => o.variable_name == "Email_From").FirstOrDefault().variable_value);
                message.To.Add(email_configurations.Where(o => o.variable_name == "Email_To").FirstOrDefault().variable_value);
                message.Subject = email_configurations.Where(o => o.variable_name == "Email_Subject").FirstOrDefault().variable_value;
                message.Body = msg;

                var smtpClient = new SmtpClient("smtp.office365.com");
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(email_configurations.Where(o => o.variable_name == "Email_From").FirstOrDefault().variable_value, email_configurations.Where(o => o.variable_name == "Email_Password").FirstOrDefault().variable_value);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(message);

                Thread.Sleep(200);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
