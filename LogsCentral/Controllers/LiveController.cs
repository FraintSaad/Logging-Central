using LogsCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LogsCentral.Controllers
{
    [Route("status/logs/live")]
    public class LiveController : Controller
    {
        private readonly LivePageViewModel _viewModel;

        public LiveController(LivePageViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetLogs(string? level)
        {
            var logs = await _viewModel.GetLatestLogsAsync(level);
            return Json(logs);
        }

        [HttpPost("add-test-log")]
        public async Task<IActionResult> AddTestLog(string level, string message)
        {
            await _viewModel.AddTestLogAsync(level, message);
            return Ok(new { success = true });
        }
    }
}
