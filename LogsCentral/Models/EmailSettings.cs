namespace LogsCentral.Models
{
    public class EmailSettings
    {
        public string FromEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; }

        internal void Validate()
        {
            if (string.IsNullOrWhiteSpace(FromEmail))
            {
                throw new ArgumentException("FromEmail cannot be null or empty.", nameof(FromEmail));
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(Password));
            }
            if (Port <= 0 || Port > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(Port), "Port must be a valid TCP port");
            }
        }
    }
}
