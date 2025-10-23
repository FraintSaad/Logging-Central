namespace LogsCentral.Jobs
{
    public class AlertsBackgroundJob : BackgroundService
    {
        private readonly IServiceProvider _services;

        public AlertsBackgroundJob(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var alertsService = scope.ServiceProvider.GetRequiredService<AlertService>();
                    await alertsService.ProcessAlertRulesAsync(stoppingToken);
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
