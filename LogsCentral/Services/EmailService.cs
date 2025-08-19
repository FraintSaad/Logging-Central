using System.Net;
using System.Net.Mail;

namespace LogsCentral.Services
{
    public class EmailService
    {
        private readonly string _fromEmail;
        private readonly string _password;

        public EmailService(string fromEmail, string password)
        {
            _fromEmail = fromEmail;
            _password = password;
        }

        public void Send(string to, string subject, string body)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential(_fromEmail, _password);
                client.EnableSsl = true;

                var mail = new MailMessage(_fromEmail, to)
                {
                    Subject = subject,
                    Body = body
                };

                client.Send(mail);
            }
        }
    }
}
