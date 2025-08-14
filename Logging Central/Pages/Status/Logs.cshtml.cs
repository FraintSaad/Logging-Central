using Data.Context;
using Data.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Logging_Central.Pages.Status
{
    public class LogsModel : PageModel
    {
        private readonly LogsDbContext _dbContext;

        public LogsModel(LogsDbContext dbContext)
        {
            _dbContext = dbContext; 
        }

        public bool CurrentSortOrder { get; set; }
        public List<LogEntity> Logs { get; set; } = new();

        public async Task OnGetAsync(bool sortOrder)
        {
            CurrentSortOrder = sortOrder;

            if (sortOrder == true)
            {
                Logs = await _dbContext.Logs
                                   .OrderBy(log => log.Timestamp)
                                   .Take(100)
                                   .ToListAsync();
            }
            else
            {
                Logs = await _dbContext.Logs
                                   .OrderByDescending(log => log.Timestamp)
                                   .Take(100)
                                   .ToListAsync();
            }

        }
    }
}
