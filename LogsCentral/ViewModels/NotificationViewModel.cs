namespace LogsCentral.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public int Period { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Threshold { get; set; }
        public string LogLevels { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
