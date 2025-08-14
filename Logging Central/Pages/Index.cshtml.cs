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
            _logger.LogDebug("��� �������� DEBUG ���");
            _logger.LogInformation("��� �������� INFO ���");
            _logger.LogWarning("��� �������� WARNING ���");
            _logger.LogError("��� �������� ERROR ���");
        }
    }
}
