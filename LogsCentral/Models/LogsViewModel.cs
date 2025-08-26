using Data.Models;

namespace LogsCentral.Models
{
    public class LogsViewModel
    {

        public List<LogEntity> Logs { get; set; } = new List<LogEntity>();
        public bool CurrentSortOrder { get; set; } = false;
        public bool LogLevelDebug { get; set; } = true;
        public bool LogLevelInfo { get; set; } = true;
        public bool LogLevelWarning { get; set; } = true;
        public bool LogLevelError { get; set; } = true;
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
