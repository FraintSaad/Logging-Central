using Data.Context;
using LogsCentral.Models;
using Microsoft.AspNetCore.Mvc;

namespace LogsCentral.Controllers
{
    [Route("status")]
    public class NotificationsController : Controller
    {
        private readonly NotificationsDbContext _dbContext;
        public NotificationsController(NotificationsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        private static List<NotificationsViewModel> _notifications = new();
        private static int _idCounter = 1;
        [HttpGet("notifications")]
        public IActionResult Index()
        {
            return View(_notifications);
        }
        [HttpPost("add")]
        public IActionResult Add(NotificationsViewModel model, string[] selectedLevels)
        {
            model.Id = _idCounter++;
            model.CreatedAt = DateTime.Now;
            model.LogLevels = string.Join(",", selectedLevels);
            _notifications.Add(model);

            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var item = _notifications.FirstOrDefault(n => n.Id == id);
            if (item != null)
                _notifications.Remove(item);

            return RedirectToAction("Index");
        }
    }
}
