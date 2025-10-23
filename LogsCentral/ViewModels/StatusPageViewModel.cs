using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace LogsCentral.ViewModels
{
    public class StatusPageViewModel
    {
        private readonly LogsDbContext _db;

        public StatusPageViewModel(LogsDbContext db)
        {
            _db = db;
        }

        public int LastDayCount { get; private set; }
        public int LastWeekCount { get; private set; }
        public int LastMonthCount { get; private set; }
        public int SelectedDays { get; private set; }
        public int SelectedPeriodCount { get; private set; }
        public Dictionary<string, int> LogsByLevel { get; private set; } = new();

        public async Task LoadAsync(int? days)
        {
            var now = DateTimeOffset.UtcNow;

            LastDayCount = await _db.SerilogEvents.CountAsync(l => l.Timestamp >= now.AddDays(-1));
            LastWeekCount = await _db.SerilogEvents.CountAsync(l => l.Timestamp >= now.AddDays(-7));
            LastMonthCount = await _db.SerilogEvents.CountAsync(l => l.Timestamp >= now.AddMonths(-1));

            SelectedDays = days ?? 7;
            var fromDate = now.AddDays(-SelectedDays);

            var logs = await _db.SerilogEvents
                                .Where(l => l.Timestamp >= fromDate)
                                .ToListAsync();

            LogsByLevel = logs.GroupBy(l => l.Level).ToDictionary(g => g.Key, g => g.Count());

            SelectedPeriodCount = logs.Count;
        }
    }
}
