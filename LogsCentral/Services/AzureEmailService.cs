using Azure;
using Azure.Communication.Email;
using LogsCentral.Interfaces;
using LogsCentral.Models;

namespace LogsCentral.Services
{
    public class AzureEmailService : IEmailService
    {
        private readonly EmailClient _emailClient;
        private readonly ILogger<AzureEmailService> _logger;
        private readonly string _sender;

        public AzureEmailService(AppSettings appSettings, ILogger<AzureEmailService> logger)
        {
            _emailClient = new EmailClient(appSettings.AzureEmailConnectionString);
            _logger = logger;
            _sender = appSettings.AzureEmailSender;
        }
        public async Task SendMailAsync(string recipients, string subject, string htmlBody, string plainBody)
        {
            var emailRecepients = new EmailRecipients();
            foreach (var email in recipients.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                emailRecepients.To.Add(new EmailAddress(email.Trim()));
            }

            var emailMessage = new EmailMessage(
                senderAddress: _sender,
                content: new EmailContent(subject)
                {
                    Html = htmlBody,
                    PlainText = plainBody
                },
                recipients: emailRecepients
            );

            EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage);

            _logger.LogInformation("Email sent. Operation Id: {OperationId}", emailSendOperation.Id);
        }
    }
}
