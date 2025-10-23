using Data.Context;
using Data.Models;
using LogsCentral.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LogsCentral.ViewModels
{
    public class LogsViewModel
    {
        private readonly LogsDbContext _dbContext;

        public LogsViewModel(LogsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Loads a page of logs with optional level filters, sorting, and UTC time range.
        /// All timestamps in the DB are assumed to be stored in UTC as DateTimeOffset.
        /// </summary>
        public async Task<LogsPageModel> LoadAsync(
            bool logLevelDebug,
            bool logLevelInfo,
            bool logLevelWarning,
            bool logLevelError,
            string? sortBy,
            bool sortOrderAsc,
            int page,
            DateTimeOffset? startTime = null,
            DateTimeOffset? endTime = null,
            int pageSize = 100)
        {
            var model = new LogsPageModel
            {
                LogLevelDebug = logLevelDebug,
                LogLevelInfo = logLevelInfo,
                LogLevelWarning = logLevelWarning,
                LogLevelError = logLevelError,
                CurrentSortOrder = sortOrderAsc,

                // expose the active time range (optional; helpful for UI)
                StartTime = startTime,
                EndTime = endTime
            };

            var selectedLevels = new List<string>(4);
            if (logLevelDebug) selectedLevels.Add("Debug");
            if (logLevelInfo) selectedLevels.Add("Information");
            if (logLevelWarning) selectedLevels.Add("Warning");
            if (logLevelError) selectedLevels.Add("Error");

            IQueryable<LogEntity> query = _dbContext.SerilogEvents.AsNoTracking();

            query = query.Where(l => l.Environment == "NS");

            // Level filtering (if none selected, show all)
            if (selectedLevels.Count > 0)
            {
                query = query.Where(l => l.Level != null && selectedLevels.Contains(l.Level));
            }
            else
            {
                query = query.Where(l => (l.Level == "Warning" || l.Level == "Error"));
            }

            // Time-window filtering in UTC
            if (startTime.HasValue)
            {
                // DB column is DateTimeOffset (UTC), so comparing to UTC boundary is correct.
                query = query.Where(l => l.Timestamp >= startTime.Value);
            }

            if (endTime.HasValue)
            {
                query = query.Where(l => l.Timestamp <= endTime.Value);
            }

            // Sorting
            // Default sort: Timestamp DESC (newest first) when sortBy is null/unknown.
            switch (sortBy?.ToLowerInvariant())
            {
                case "level":
                    // Preserve your explicit order for levels. EF translates the ternary chain to CASE.
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
                    query = sortOrderAsc
                        ? query.OrderBy(l => l.Message)
                        : query.OrderByDescending(l => l.Message);
                    break;

                case "exception":
                    query = sortOrderAsc
                        ? query.OrderBy(l => l.Exception)
                        : query.OrderByDescending(l => l.Exception);
                    break;

                case "timestamp":
                    query = sortOrderAsc
                        ? query.OrderBy(l => l.Timestamp)
                        : query.OrderByDescending(l => l.Timestamp);
                    break;

                default:
                    query = sortOrderAsc
                        ? query.OrderBy(l => l.Timestamp)
                        : query.OrderByDescending(l => l.Timestamp);
                    break;
            }

            // Override known non error issues
            query = query.Where(l => !(l.Level == "Error" && l.Exception != null && l.Exception.Contains("API calling limit exceeded")));
            query = query.Where(l => !(l.Level == "Error" && l.Message != null && l.Message.Contains("Unauthorized")));
            query = query.Where(l => !(l.Level == "Information" && l.Message != null && l.Message!.StartsWith("Item is not assigned to user")));

            // Pagination
            if (page < 1) page = 1;

            var totalLogs = await query.CountAsync();
            model.TotalPages = (int)Math.Ceiling(totalLogs / (double)pageSize);
            if (model.TotalPages == 0) model.TotalPages = 1;

            // clamp current page to [1..TotalPages]
            if (page > model.TotalPages) page = model.TotalPages;

            model.CurrentPage = page;

            model.Logs = await query
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            model.Logs.Select(l =>
            {
                var jsonStart = l.Message!.IndexOf("\"{");
                if (jsonStart != -1)
                {
                    var quotedJson = l.Message.Substring(jsonStart);

                    // Remove wrapping quotes if present
                    if (quotedJson.StartsWith("\"") && quotedJson.EndsWith("\""))
                    {
                        quotedJson = quotedJson.Substring(1, quotedJson.Length - 2);
                    }

                    // Unescape backslashes
                    l.Message = Regex.Unescape(quotedJson);
                }

                return new LogEntity
                {
                    Id = l.Id,
                    Level = l.Level,
                    Timestamp = l.Timestamp,
                    Message = l.Message,
                    Exception = l.Exception,
                    Environment = l.Environment,
                };
            }
            ).ToList();

            if (selectedLevels.Count == 0)
            {
                model.LogLevelDebug = false;
                model.LogLevelInfo = false;
                model.LogLevelWarning = true;
                model.LogLevelError = true;
            }

            return model;
        }
    }
}
