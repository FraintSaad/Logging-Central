using LogsCentral.Dto;
using LogsCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Route("logs")]
public class LogsController : Controller
{
    private readonly LogsViewModel _logsViewModel;
    private const string TimezoneCookieName = "logs.timezone";

    public LogsController(LogsViewModel logsViewModel)
    {
        _logsViewModel = logsViewModel;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] LogFilters q)
    {
        // Normalize page
        var currentPage = q.page <= 0 ? 1 : q.page;

        // Determine timezone id (priority: query -> cookie -> UTC)
        string selectedTzId = q.timezoneId;
        if (string.IsNullOrEmpty(selectedTzId))
        {
            if (Request.Cookies.TryGetValue(TimezoneCookieName, out var cookieTz))
                selectedTzId = cookieTz;
        }
        if (string.IsNullOrEmpty(selectedTzId))
            selectedTzId = "UTC";

        // Build timezone list for view
        var tzList = TimeZoneInfo.GetSystemTimeZones()
            .Select(tz => new SelectListItem
            {
                Value = tz.Id,
                Text = tz.DisplayName + (tz.Id == TimeZoneInfo.Local.Id ? " (local)" : ""),
                Selected = tz.Id == selectedTzId
            })
            .ToList();

        // Parse selected timezone
        TimeZoneInfo selectedTz;
        try
        {
            selectedTz = TimeZoneInfo.FindSystemTimeZoneById(selectedTzId);
        }
        catch
        {
            selectedTz = TimeZoneInfo.Utc;
            selectedTzId = "UTC";
        }

        // Convert submitted start/end from selected timezone -> UTC (if present)
        DateTimeOffset? startTimeUtc = null;
        DateTimeOffset? endTimeUtc = null;

        if (!string.IsNullOrWhiteSpace(q.startTime))
        {
            if (DateTime.TryParse(q.startTime, null, System.Globalization.DateTimeStyles.None, out var dt))
            {
                // dt is unspecified (browser datetime-local); interpret in selectedTz then convert to UTC
                var utc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dt, DateTimeKind.Unspecified), selectedTz);
                startTimeUtc = new DateTimeOffset(utc, TimeSpan.Zero);
            }
        }

        if (!string.IsNullOrWhiteSpace(q.endTime))
        {
            if (DateTime.TryParse(q.endTime, null, System.Globalization.DateTimeStyles.None, out var dt))
            {
                var utc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dt, DateTimeKind.Unspecified), selectedTz);
                endTimeUtc = new DateTimeOffset(utc, TimeSpan.Zero);
            }
        }

        // Provide sensible defaults (last 24 hours) if missing
        var defaultEndUtc = DateTimeOffset.UtcNow;
        var defaultStartUtc = defaultEndUtc.AddHours(-24);

        if (!startTimeUtc.HasValue)
            startTimeUtc = defaultStartUtc;
        if (!endTimeUtc.HasValue)
            endTimeUtc = defaultEndUtc;

        // Ensure start <= end
        if (startTimeUtc.HasValue && endTimeUtc.HasValue && endTimeUtc < startTimeUtc)
        {
            var tmp = startTimeUtc;
            startTimeUtc = endTimeUtc;
            endTimeUtc = tmp;
        }

        // Load logs (pass UTC times)
        var model = await _logsViewModel.LoadAsync(
            logLevelDebug: q.logLevelDebug == "on",
            logLevelInfo: q.logLevelInfo == "on",
            logLevelWarning: q.logLevelWarning == "on",
            logLevelError: q.logLevelError == "on",
            sortBy: q.sortBy,
            sortOrderAsc: q.sortOrder,
            page: currentPage,
            startTime: startTimeUtc,
            endTime: endTimeUtc
        );

        // Populate timezone list + selected id
        model.Timezones = tzList;
        model.SelectedTimezoneId = selectedTzId;

        // Compute values appropriate for <input type="datetime-local"> in the selected timezone
        DateTime? ToLocalForInput(DateTimeOffset? utc)
        {
            if (!utc.HasValue) return null;
            // Convert UTC -> selected timezone
            var local = TimeZoneInfo.ConvertTimeFromUtc(utc.Value.UtcDateTime, selectedTz);
            return DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        }

        model.StartLocalForInput = ToLocalForInput(model.StartTime);
        model.EndLocalForInput = ToLocalForInput(model.EndTime);

        // Persist user's tz choice to cookie (1 year)
        Response.Cookies.Append(TimezoneCookieName, selectedTzId, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });

        return View(model);
    }
}
