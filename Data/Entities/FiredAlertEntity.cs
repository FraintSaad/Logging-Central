namespace Data.Entities
{
    public class FiredAlertEntity
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public int LogsCount { get; set; }
        public bool? Sent { get; set; } = null;
        public string? SendError { get; set; } = null;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}