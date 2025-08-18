using Data.Context;
using Data.Models;
using LogsCentral.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogsCentral.Controllers
{
    [Route("status")]
    public class LogsController : Controller
    {
        private readonly LogsDbContext _dbContext;
        public LogsController(LogsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("logs")]
        public async Task<IActionResult> Index()
        {
            var logsViewModel = new LogsViewModel();
            logsViewModel.CurrentSortOrder = Convert.ToBoolean(Request.Query["sortOrder"].FirstOrDefault());

            logsViewModel.LogLevelDebug = Request.Query["logLevelDebug"].FirstOrDefault() == "on";

            logsViewModel.CurrentSortOrder = bool.TryParse(Request.Query["sortOrder"], out var sort) && sort;

            logsViewModel.LogLevelDebug = Request.Query.ContainsKey("logLevelDebug");
            logsViewModel.LogLevelInfo = Request.Query.ContainsKey("logLevelInfo");
            logsViewModel.LogLevelWarning = Request.Query.ContainsKey("logLevelWarning");
            logsViewModel.LogLevelError = Request.Query.ContainsKey("logLevelError");
            var selectedLevels = new List<string>();
            if (logsViewModel.LogLevelDebug)
            {
                selectedLevels.Add("Debug");
            }
            if (logsViewModel.LogLevelInfo)
            {
                selectedLevels.Add("Information"); 
            }
            if (logsViewModel.LogLevelWarning)
            {
                selectedLevels.Add("Warning");
            }
            if (logsViewModel.LogLevelError)
            {
                selectedLevels.Add("Error"); 
            }

            var query = _dbContext.Logs.AsQueryable();

            if (selectedLevels.Count > 0)
            {
                query = query.Where(l => l.Level != null && selectedLevels.Contains(l.Level));
            }
            if (logsViewModel.CurrentSortOrder == true)
            {
                query = query.OrderBy(l => l.Timestamp);
            }
            else
            {
                query = query.OrderByDescending(l => l.Timestamp);
            }

            logsViewModel.Logs = await query.Take(100).ToListAsync();

            return View(logsViewModel);
        }
    }
}
