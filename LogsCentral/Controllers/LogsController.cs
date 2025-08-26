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
            
            _dbContext.SaveChanges();
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

            var sortBy = Request.Query["sortBy"].FirstOrDefault();

            if (sortBy == "Level")
            {
                query = logsViewModel.CurrentSortOrder
                                     ? query.OrderBy(l => l.Level == "Debug" ? 1 :
                                                          l.Level == "Information" ? 2 :
                                                          l.Level == "Warning" ? 3 :
                                                          l.Level == "Error" ? 4 : 5)
                                     : query.OrderByDescending(l => l.Level == "Debug" ? 1 :
                                                          l.Level == "Information" ? 2 :
                                                          l.Level == "Warning" ? 3 :
                                                          l.Level == "Error" ? 4 : 5);
            }
            else if (sortBy == "Message")
            {
                query = logsViewModel.CurrentSortOrder
                    ? query.OrderBy(l => l.Message)
                    : query.OrderByDescending(l => l.Message);
            }
            else if (sortBy == "Exception")
            {
                query = logsViewModel.CurrentSortOrder ? query.OrderBy(l => l.Exception) : query.OrderByDescending(l => l.Exception);
            }
            else
            {
                query = logsViewModel.CurrentSortOrder ? query.OrderBy(l => l.Timestamp) : query.OrderByDescending(l => l.Timestamp);
            }

            int pageSize = 100;
            int page = int.TryParse(Request.Query["page"], out var p) ? p : 1;
            if (page < 1)
            {
                page = 1;
            }
            int totalLogs = await query.CountAsync();
            logsViewModel.TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize);
            logsViewModel.CurrentPage = page;
            logsViewModel.Logs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return View(logsViewModel);
        }
    }
}
