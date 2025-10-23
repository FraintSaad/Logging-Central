namespace LogsCentral.Dto
{
    public sealed class LogFilters
    {
        // Log levels: presence of the key means "include"
        public string logLevelDebug { get; set; } = "off";
        public string logLevelInfo { get; set; } = "off";
        public string logLevelWarning { get; set; } = "off";
        public string logLevelError { get; set; } = "off";

        // Sorting & paging
        public string? sortBy { get; set; }
        public bool sortOrder { get; set; }
        public int page { get; set; } = 1;

        public string? startTime { get; set; }
        public string? endTime { get; set; }
        public string? timezoneId { get; set; }
    }
}
