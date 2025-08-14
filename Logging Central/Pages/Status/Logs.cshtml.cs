using Data.Context;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
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

        public async Task OnGetAsync(bool sortOrder, string? level)
        {
            CurrentSortOrder = sortOrder;
            var query = _dbContext.Logs.AsQueryable();
            query = CurrentSortOrder ? query.OrderBy(l => l.Timestamp) : query.OrderByDescending(l => l.Timestamp);
            
        }
    }
}
