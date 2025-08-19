using LogsCentral.Models;
using System.Net;
using System.Net.Mail;

namespace LogsCentral.Services
{
    public class EmailService
    {
        EmailSettings _settings;

        public EmailService(EmailSettings settings)
        {
            _settings = settings;
        }

        public void Send(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password);
                client.EnableSsl = true;

                var mail = new MailMessage(_settings.FromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body
                };

                client.Send(mail);
            }
        }


    }
}
