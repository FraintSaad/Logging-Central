using LogsCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LogsCentral.Controllers
{
    [Route("status")]
    public class StatusController : Controller
    {
        private readonly StatusPageViewModel _viewModel;

        public StatusController(StatusPageViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? days)
        {
            await _viewModel.LoadAsync(days);
            return View(_viewModel);
        }
    }
}
