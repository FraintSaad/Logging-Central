using Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LogsCentral.Models
{
    public class LogsPageModel
    {
        public List<LogEntity> Logs { get; set; } = new List<LogEntity>();

        public bool CurrentSortOrder { get; set; } = false;
        public bool LogLevelDebug { get; set; } = false;
        public bool LogLevelInfo { get; set; } = false;
        public bool LogLevelWarning { get; set; } = false;
        public bool LogLevelError { get; set; } = false;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public string? SelectedTimezoneId { get; set; }
        public List<SelectListItem> Timezones { get; set; } = new List<SelectListItem>();

        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public DateTime? StartLocalForInput { get; set; }
        public DateTime? EndLocalForInput { get; set; }
    }
}
