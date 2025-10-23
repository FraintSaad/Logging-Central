using LogsCentral.Models;

namespace LogsCentral.Models
{
    public class SmtpEmailSettings
    {
        public string Sender { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Server { get; set; } = null!;
        public int Port { get; set; }
        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(Server))
            {
                throw new ArgumentException("Server cannot be null or empty.", nameof(Server));
            }
            if (Port <= 0 || Port > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(Port), "Port must be a valid TCP port");
            }
            if (string.IsNullOrWhiteSpace(Sender))
            {
                throw new ArgumentException("FromEmail cannot be null or empty.", nameof(Sender));
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(Password));
            }
        }
    }

    public class AppSettings
    {
        public string AppUrl { get; set; } = null!;
        public string SourceEnvironment { get; set; } = null!;
        public string AzureEmailSender { get; set; } = null!;
        public string AzureEmailConnectionString { get; set; } = null!;
        public string DatabaseConnectionString { get; set; } = null!;
        public SmtpEmailSettings? SmtpEmailSettings { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(SourceEnvironment))
            {
                throw new NullReferenceException(nameof(SourceEnvironment));
            }
            if (string.IsNullOrWhiteSpace(AppUrl))
            {
                throw new NullReferenceException(nameof(AppUrl));
            }
            if (string.IsNullOrWhiteSpace(DatabaseConnectionString))
            {
                throw new ArgumentException("Database connection string cannot be null or empty.", nameof(DatabaseConnectionString));
            }
            if (string.IsNullOrWhiteSpace(AzureEmailSender))
            {
                throw new ArgumentException("AzureEmailSender cannot be null or empty.", nameof(AzureEmailSender));
            }
            if (string.IsNullOrWhiteSpace(AzureEmailConnectionString))
            {
                throw new ArgumentException("AzureEmailConnectionString cannot be null or empty.", nameof(AzureEmailConnectionString));
            }

            // SmtpEmailSettings is optional
            if (SmtpEmailSettings != null)
            {
                SmtpEmailSettings.Validate();
            }
        }
    }
}
