using LogsCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;

[Route("status")]
public class LogsController : Controller
{
    private readonly LogsViewModel _logsViewModel;

    public LogsController(LogsViewModel logsViewModel)
    {
        _logsViewModel = logsViewModel;
    }

    [HttpGet("logs")]
    public async Task<IActionResult> Index()
    {
        bool logLevelDebug = Request.Query.ContainsKey("logLevelDebug");
        bool logLevelInfo = Request.Query.ContainsKey("logLevelInfo");
        bool logLevelWarning = Request.Query.ContainsKey("logLevelWarning");
        bool logLevelError = Request.Query.ContainsKey("logLevelError");

        string? sortBy = Request.Query["sortBy"].FirstOrDefault();
        bool sortOrderAsc = bool.TryParse(Request.Query["sortOrder"], out var sort) && sort;
        int page = int.TryParse(Request.Query["page"], out var p) ? p : 1;

        var model = await _logsViewModel.LoadAsync(logLevelDebug, logLevelInfo, 
                                                   logLevelWarning, logLevelError,
                                                   sortBy, sortOrderAsc, page);
        return View(model);
    }
}
