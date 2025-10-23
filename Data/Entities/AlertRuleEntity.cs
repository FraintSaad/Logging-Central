using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class AlertRuleEntity
    {
        public int Id { get; set; }
        // This is in minutes, time to wait before sending another alert for the same rule
        public int CooldownPeriod { get; set; } = 5;
        // This is in minutes, how far back to look for recent logs
        public int LookbackPeriod { get; set; }
        // Number of logs in the lookback period to trigger the alert
        public int Threshold { get; set; }
        public string LogLevel { get; set; } = null!;
        public string Recipients { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
