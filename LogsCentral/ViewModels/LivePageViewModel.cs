using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LogsCentral.ViewModels
{
    public class LivePageViewModel
    {
        private readonly LogsDbContext _db;

        public LivePageViewModel(LogsDbContext db)
        {
            _db = db;
        }

        public async Task<List<LiveLogDto>> GetLatestLogsAsync(string? levelFilter = null, int take = 100)
        {
            var query = _db.SerilogEvents.AsQueryable();

            if (!string.IsNullOrEmpty(levelFilter))
                query = query.Where(l => l.Level == levelFilter);

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Take(take)
                .ToListAsync();

            return logs.Select(l => new LiveLogDto
            {
                Timestamp = l.Timestamp,
                Level = l.Level,
                Message = l.Message
            }).ToList();
        }
    }
}
