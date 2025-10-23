using LogsCentral.Services;

namespace LogsCentral.Jobs
{
    public class CleanUpBackgroundJob : BackgroundService
    {
        private readonly IServiceProvider _services;

        public CleanUpBackgroundJob(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<CleanUpService>();
                    service.Run();
                }
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
