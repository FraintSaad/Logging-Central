using LogsCentral.Models;
using System.Net;
using System.Net.Mail;

namespace LogsCentral.Services
{
    public class EmailService
    {
        private readonly IServiceProvider _services;
        EmailSettings _settings;

        public EmailService(EmailSettings settings, IServiceProvider services)
        {
            _settings = settings;
            _services = services;
        }

        public void Send(string toEmails, string subject, string body)
        {
            using (var scope = _services.CreateScope())
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password);
                    client.EnableSsl = true;

                    var mail = new MailMessage()
                    {
                        From = new MailAddress(_settings.FromEmail),
                        Subject = subject,
                        Body = body
                    };

                    foreach (var email in toEmails.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        mail.To.Add(email.Trim());
                    }

                    client.Send(mail);
                }
            }
        }
    }
}
