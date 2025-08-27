namespace LogsCentral.Models
{
    public class StatusPageModel
    {

        public int LastDayCount { get; set; }
        public int LastWeekCount { get; set; }
        public int LastMonthCount { get; set; }

        public int SelectedPeriodCount { get; set; }

        public Dictionary<string, int> LogsByLevel { get; set; } = new Dictionary<string, int>();

        public int SelectedDays { get; set; } = 7;
    }
}
