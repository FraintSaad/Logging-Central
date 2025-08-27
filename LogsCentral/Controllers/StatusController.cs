using Data.Context;
using LogsCentral.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogsCentral.Controllers
{
    [Route("status")]
    public class StatusController : Controller
    {
        private readonly LogsDbContext _db;

        public StatusController(LogsDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? days)
        {
            var vm = new StatusPageModel();

            var now = DateTime.Now;

            vm.LastDayCount = await _db.SerilogEvents.CountAsync(l => l.Timestamp >= now.AddDays(-1));
            vm.LastWeekCount = await _db.SerilogEvents.CountAsync(l => l.Timestamp >= now.AddDays(-7));
            vm.LastMonthCount = await _db.SerilogEvents.CountAsync(l => l.Timestamp >= now.AddMonths(-1));

            vm.SelectedDays = days ?? 7;
            var fromDate = now.AddDays(-vm.SelectedDays);

            var logs = await _db.SerilogEvents.Where(l => l.Timestamp >= fromDate).ToListAsync();

            vm.LogsByLevel = logs.GroupBy(l => l.Level).ToDictionary(g => g.Key, g => g.Count());

            vm.SelectedPeriodCount = logs.Count;

            return View(vm);
        }
    }
}
