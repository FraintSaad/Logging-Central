using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Logging_Central.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogDebug("Это тестовый DEBUG лог");
            _logger.LogInformation("Это тестовый INFO лог");
            _logger.LogWarning("Это тестовый WARNING лог");
            _logger.LogError("Это тестовый ERROR лог");
        }
    }
}
