using Azure.Core;
using Data.Context;
using LogsCentral.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogsCentral.ViewModels
{
    public class LogsViewModel
    {
        private readonly LogsDbContext _dbContext;
        public LogsViewModel(LogsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LogsPageModel> LoadAsync(bool logLevelDebug, bool logLevelInfo, bool logLevelWarning,
                                                   bool logLevelError, string? sortBy, bool sortOrderAsc, int page, int pageSize = 100)
        {
            var model = new LogsPageModel
            {
                LogLevelDebug = logLevelDebug,
                LogLevelInfo = logLevelInfo,
                LogLevelWarning = logLevelWarning,
                LogLevelError = logLevelError,
                CurrentSortOrder = sortOrderAsc
            };

            var selectedLevels = new List<string>();
            if (logLevelDebug) selectedLevels.Add("Debug");
            if (logLevelInfo) selectedLevels.Add("Information");
            if (logLevelWarning) selectedLevels.Add("Warning");
            if (logLevelError) selectedLevels.Add("Error");

            var query = _dbContext.SerilogEvents.AsQueryable();

            if (selectedLevels.Count > 0)
            {
                query = query.Where(l => l.Level != null && selectedLevels.Contains(l.Level));
            }

            switch (sortBy?.ToLower())
            {
                case "level":
                    query = sortOrderAsc
                        ? query.OrderBy(l => l.Level == "Debug" ? 1 :
                                             l.Level == "Information" ? 2 :
                                             l.Level == "Warning" ? 3 :
                                             l.Level == "Error" ? 4 : 5)
                        : query.OrderByDescending(l => l.Level == "Debug" ? 1 :
                                                       l.Level == "Information" ? 2 :
                                                       l.Level == "Warning" ? 3 :
                                                       l.Level == "Error" ? 4 : 5);
                    break;

                case "message":
                    query = sortOrderAsc ? query.OrderBy(l => l.Message) : query.OrderByDescending(l => l.Message);
                    break;

                case "exception":
                    query = sortOrderAsc ? query.OrderBy(l => l.Exception) : query.OrderByDescending(l => l.Exception);
                    break;

                default:
                    query = sortOrderAsc ? query.OrderBy(l => l.Timestamp) : query.OrderByDescending(l => l.Timestamp);
                    break;
            }

            //pagination
            if (page < 1) page = 1;
            int totalLogs = await query.CountAsync();
            model.TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize);
            model.CurrentPage = page;
            model.Logs = await query.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return model;
        }
    }
}

