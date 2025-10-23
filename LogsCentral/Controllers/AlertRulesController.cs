using System.ComponentModel.DataAnnotations;
using LogsCentral.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LogsCentral.Controllers
{
    [Route("alert-rules")]
    public class AlertRulesController : Controller
    {
        private readonly AlertRulesPageViewModel _viewModel;

        public AlertRulesController(AlertRulesPageViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            var models = await _viewModel.GetAllAsync();
            return View(models);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("add")]
        public async Task<IActionResult> Add(AlertRuleViewModel model, string selectedLevel)
        {
            // Convert input string to list first
            model.Recipients = model.Email.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            model.LookbackPeriod = NormalizePeriod(model.LookbackPeriod);
            model.LogLevel = selectedLevel;

            if (!ValidateEmails(model.Recipients))
            {
                ModelState.AddModelError("", "One or more recipient emails are invalid.");
                return View("Index", await _viewModel.GetAllAsync());
            }

            await _viewModel.AddAsync(model, selectedLevel);
            return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("edit/{id:int}")]
        public async Task<IActionResult> EditAsync([FromRoute] int id, AlertRuleViewModel model, string selectedLevel)
        {
            if (model.Id == 0 || model.Id != id) model.Id = id;
            model.Recipients = model.Email.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            model.LookbackPeriod = NormalizePeriod(model.LookbackPeriod);

            if (!ValidateEmails(model.Recipients))
            {
                ModelState.AddModelError("", "One or more recipient emails are invalid.");
                return View("Index", await _viewModel.GetAllAsync());
            }

            await _viewModel.EditAsync(model, selectedLevel);
            return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("delete/{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _viewModel.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Ensure period is within 5–168 minutes
        private static int NormalizePeriod(int period)
        {
            if (period < 5) return 5;
            if (period > 168) return 168;
            return period;
        }

        // Validate each recipient email
        private static bool ValidateEmails(List<string> emails)
        {
            var emailValidator = new EmailAddressAttribute();
            return emails.All(email => emailValidator.IsValid(email));
        }
    }
}
