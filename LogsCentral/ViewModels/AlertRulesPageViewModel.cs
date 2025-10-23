using Data.Context;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LogsCentral.ViewModels
{
    public class AlertRulesPageViewModel
    {
        private readonly LogsDbContext _db;

        public AlertRulesPageViewModel(LogsDbContext db)
        {
            _db = db;
        }

        public async Task<List<AlertRuleViewModel>> GetAllAsync()
        {
            var notificationRules = await _db.AlertRules.ToListAsync();

            return notificationRules.Select(c => new AlertRuleViewModel
            {
                Id = c.Id,
                LookbackPeriod = c.LookbackPeriod,
                CreatedAt = c.CreatedAt,
                Threshold = c.Threshold,
                LogLevel = c.LogLevel,
                Recipients = c.Recipients.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
            }).ToList();
        }

        public async Task AddAsync(AlertRuleViewModel model, string selectedLevel)
        {
            var entity = new AlertRuleEntity
            {
                LookbackPeriod = model.LookbackPeriod,
                CreatedAt = DateTimeOffset.UtcNow,
                Threshold = model.Threshold,
                LogLevel = selectedLevel,
                Recipients = string.Join(",", model.Recipients)
            };

            _db.AlertRules.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task EditAsync(AlertRuleViewModel model, string selectedLevel)
        {
            var entity = await _db.AlertRules.FindAsync(model.Id);
            if (entity is null) return;

            entity.LookbackPeriod = model.LookbackPeriod;
            entity.Threshold = model.Threshold;
            entity.LogLevel = selectedLevel;
            entity.Recipients = string.Join(",", model.Recipients);

            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var item = await _db.AlertRules.FindAsync(id);
            if (item != null)
            {
                _db.AlertRules.Remove(item);
                await _db.SaveChangesAsync();
            }
        }
    }
}
