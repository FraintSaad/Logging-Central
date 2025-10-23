using Data.Context;

namespace LogsCentral.Services
{
    public class CleanUpService
    {
        private readonly ILogger<CleanUpService> _logger;
        private readonly LogsDbContext _dbContext;

        public CleanUpService(LogsDbContext dbContext, ILogger<CleanUpService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public void Run()
        {
            var cutoffDate = DateTimeOffset.UtcNow.AddDays(-180);

            var oldEntries = _dbContext.SerilogEvents
                .Where(x => x.Timestamp < cutoffDate)
                .ToList();

            if (!oldEntries.Any())
            {
                return;
            
            }

            _dbContext.SerilogEvents.RemoveRange(oldEntries);
            _logger.LogInformation("Deleted old log entries (> 180 days)");
        }
    }
}
