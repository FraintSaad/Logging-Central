using Data.Context;
using Data.Entities;
using Data.Models;
using LogsCentral.Models;
using LogsCentral.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.Net;
using System.Net.Mail;

namespace LogsCentral.Controllers
{
    [Route("status")]
    public class NotificationsController : Controller
    {
        private readonly LogsDbContext _dbContext;
        private readonly EmailService _emailService;

        public NotificationsController(LogsDbContext dbContext, EmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }
        [HttpGet("notifications")]
        public async Task<IActionResult> Index()
        {
            _dbContext.Logs.AddRange(new[]
            {
                new LogEntity { Timestamp = DateTime.Now, Level = "Warning", Message = "Warning log 1" },
                new LogEntity { Timestamp = DateTime.Now, Level = "Warning", Message = "Warning log 2" },
                new LogEntity { Timestamp = DateTime.Now, Level = "Warning", Message = "Warning log 3" },
                new LogEntity { Timestamp = DateTime.Now, Level = "Warning", Message = "Warning log 4" },
                new LogEntity { Timestamp = DateTime.Now, Level = "Warning", Message = "Warning log 5" }
            });
            _dbContext.SaveChanges();

            var configs = await _dbContext.Notifications.ToListAsync();
            var models = configs.Select(c => new NotificationsViewModel
            {
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
