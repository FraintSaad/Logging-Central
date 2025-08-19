using Data.Context;
using Data.Entities;
using Data.Models;
using LogsCentral.Models;
using LogsCentral.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            _dbContext.Logs.Add(new LogEntity
            {
                Timestamp = DateTime.Now,
                Level = "Error",
                Message = "Test error log"
            });
            _dbContext.SaveChanges();

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

            TestEmail();
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

        [HttpGet("test-email")]
        public IActionResult TestEmail()
        {
            _emailService.Send(
                "islamqwertyu@mail.ru",
                "Тестовое уведомление",
                "Если ты читаешь это письмо, значит EmailService работает!"
            );

            return Ok("Письмо отправлено!");
        }
    }
}
