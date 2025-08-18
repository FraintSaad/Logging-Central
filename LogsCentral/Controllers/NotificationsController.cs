using Data.Context;
using Data.Entities;
using LogsCentral.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogsCentral.Controllers
{
    [Route("status")]
    public class NotificationsController : Controller
    {
        private readonly LogsDbContext _dbContext;
        public NotificationsController(LogsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("notifications")]
        public async Task<IActionResult> IndexAsync()
        {
            var configs = await _dbContext.Notifications.ToListAsync();
            var models = configs.Select(c => new NotificationsViewModel
            {
                Id = c.Id,
                Period = c.Period,
                CreatedAt = c.CreatedAt,
                ThrashHold = c.ThrashHold,
                LogLevels = c.LogLevels,
                Email = c.Email
            }).ToList();
           

            return View(models);
        }
        [HttpPost("add")]
        public async Task<IActionResult> Add(NotificationsViewModel model, string[] selectedLevels)
        {
            var entity = new NotificationEntity
            {
                Period = model.Period,
                CreatedAt = DateTime.Now,
                ThrashHold = model.ThrashHold,
                LogLevels = string.Join(",", selectedLevels),
                Email = model.Email
            };

            _dbContext.Notifications.Add(entity);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var item = await _dbContext.Notifications.FindAsync(id);
            if (item != null)
            {
                _dbContext.Notifications.Remove(item);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
