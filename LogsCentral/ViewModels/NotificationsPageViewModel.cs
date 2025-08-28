using Data.Context;
using Data.Entities;
using Data.Models;
using LogsCentral.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LogsCentral.ViewModels
{
    public class NotificationsPageViewModel
    {
        private readonly LogsDbContext _db;

        public NotificationsPageViewModel(LogsDbContext db)
        {
            _db = db;
        }

        public async Task<List<NotificationRuleViewModel>> GetAllAsync()
        {
            var notificationRules = await _db.NotificationRules.ToListAsync();

            return notificationRules.Select(c => new NotificationRuleViewModel
            {
                Id = c.Id,
                Period = c.Period,
                CreatedAt = c.CreatedAt,
                Threshold = c.Threshold,
                LogLevels = c.LogLevel,
                Email = c.Email
            }).ToList();
        }

        public async Task AddAsync(NotificationRuleViewModel model, string[] selectedLevels)
        {
            var entity = new NotificationsRuleEntity
            {
                Period = model.Period,
                CreatedAt = DateTime.Now,
                Threshold = model.Threshold,
                LogLevel = string.Join(",", selectedLevels),
                Email = model.Email
            };

            _db.NotificationRules.Add(entity);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var item = await _db.NotificationRules.FindAsync(id);
            if (item != null)
            {
                _db.NotificationRules.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task EditAsync(NotificationRuleViewModel model, string[] selectedLevels)
        {
            var entity = await _db.NotificationRules.FindAsync(model.Id);
            if (entity != null)
            {
                entity.Period = model.Period;
                entity.Threshold = model.Threshold;
                entity.LogLevel = string.Join(",", selectedLevels);
                entity.Email = model.Email;

                await _db.SaveChangesAsync();
            }
        }

        public async Task SeedTestLogsAsync()
        {
            _db.SerilogEvents.AddRange(new[]
            {
                new LogEntity { Timestamp = DateTime.Now, Level = "Warning", Message = "Warning log 1" },
                new LogEntity { Timestamp = DateTime.Now, Level = "Warning", Message = "Warning log 2" },
                new LogEntity { Timestamp = DateTime.Now, Level = "Warning", Message = "Warning log 3" },
                new LogEntity { Timestamp = DateTime.Now, Level = "Warning", Message = "Warning log 4" },
                new LogEntity { Timestamp = DateTime.Now, Level = "Warning", Message = "Warning log 5" }
            });

            await _db.SaveChangesAsync();
        }
    }
}
