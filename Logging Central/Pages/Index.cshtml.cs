using Microsoft.AspNetCore.Mvc;
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
            _logger.LogDebug("Это тестовый DEBUG лог в {Time}", DateTime.Now);
            _logger.LogInformation("Это тестовый INFO лог в {Time}", DateTime.Now);
            _logger.LogWarning("Это тестовый WARNING лог в {Time}", DateTime.Now);
            _logger.LogError("Это тестовый ERROR лог в {Time}", DateTime.Now);
        }
    }
}
