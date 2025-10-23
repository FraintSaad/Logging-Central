using LogsCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LogsCentral.Controllers
{
    [Route("live")]
    public class LiveController : Controller
    {
        private readonly LivePageViewModel _viewModel;

        public LiveController(LivePageViewModel viewModel)
        {
            _viewModel = viewModel;
        }

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
    }
}
