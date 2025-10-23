using System.ComponentModel.DataAnnotations;

namespace LogsCentral.ViewModels
{
    public class AlertRuleViewModel
    {
        public int Id { get; set; }
        public string LogLevel { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        // Raw string for recipients input
        public string Email { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Threshold must be positive.")]
        public int Threshold { get; set; }

        [Range(5, 168, ErrorMessage = "Lookback period must be between 5 and 168 minutes.")]
        public int LookbackPeriod { get; set; }
        
        [Required(ErrorMessage = "At least one recipient must be specified.")]
        public List<string> Recipients { get; set; } = new();
    }
}
