namespace LogsCentral.Interfaces
{
    public interface IEmailService
    {
        public Task SendMailAsync(string recepients, string subject, string htmlBody, string plainBody);
    }
}
