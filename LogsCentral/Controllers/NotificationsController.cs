using LogsCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LogsCentral.Controllers
{
    [Route("notifications")]
    public class NotificationsController : Controller
    {
        private readonly NotificationsPageViewModel _viewModel;

        public NotificationsController(NotificationsPageViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await _viewModel.SeedTestLogsAsync();

            var models = await _viewModel.GetAllAsync();
            return View(models);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(NotificationRuleViewModel model, string[] selectedLevels)
        {
            await _viewModel.AddAsync(model, selectedLevels);
            return RedirectToAction("Index");
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _viewModel.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
