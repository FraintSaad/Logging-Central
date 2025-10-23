using LogsCentral.Interfaces;
using LogsCentral.Models;
using System.Net;
using System.Net.Mail;

namespace LogsCentral.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpEmailSettings? _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(AppSettings settings, ILogger<EmailService> logger)
        {
            _settings = settings.SmtpEmailSettings;
            _logger = logger;
        }

        public async Task SendMailAsync(string toEmails, string subject, string htmlBody, string plainBody)
        {
            if (_settings == null)
            {
                _logger.LogError("SMTP settings are not configured.");
                return;
            }

            using (var client = new SmtpClient(_settings.Server, _settings.Port))
            {
                client.Credentials = new NetworkCredential(_settings.Sender, _settings.Password);
                client.EnableSsl = true;

                var message = new MailMessage()
                {
                    From = new MailAddress(_settings.Sender),
                    Subject = subject,
                    Body = htmlBody
                };

                foreach (var email in toEmails.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(email.Trim());
                }

                await client.SendMailAsync(message);
            }
        }
    }
}
