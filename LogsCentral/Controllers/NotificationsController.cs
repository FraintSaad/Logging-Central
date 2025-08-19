using Data.Context;
using Data.Entities;
using Data.Models;
using LogsCentral.Models;
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
        public NotificationsController(LogsDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public IActionResult TestEmail()
        {
            var from = "gtbartbrsynts7@gmail.com";  
            var to = "islamqwertyu@mail.ru"; 
            var password = "ocnm sjcb jfho jlly";

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential(from, password);
                client.EnableSsl = true;

                var mail = new MailMessage(from, to)
                {
                    Subject = "Тестовое письмо",
                    Body = "Привет! Это тест отправки логов :)"
                };

                client.Send(mail);
            }

            return Ok("Письмо отправлено!");
        }
    }
}
