namespace Data.Entities
{
    public class NotificationsRuleEntity
    {
        public int Id { get; set; }
        public int Period { get; set; }
        public int Threshold { get; set; }
        public string LogLevel { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
