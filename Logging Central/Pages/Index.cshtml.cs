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
            _logger.LogDebug("��� �������� DEBUG ��� � {Time}", DateTime.Now);
            _logger.LogInformation("��� �������� INFO ��� � {Time}", DateTime.Now);
            _logger.LogWarning("��� �������� WARNING ��� � {Time}", DateTime.Now);
            _logger.LogError("��� �������� ERROR ��� � {Time}", DateTime.Now);
        }
    }
}
