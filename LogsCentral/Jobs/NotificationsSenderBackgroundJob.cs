using Data.Context;
using Data.Entities;
using LogsCentral.Services;
using Microsoft.EntityFrameworkCore;

namespace LogsCentral.Jobs
{
    public class NotificationsSenderBackgroundJob : BackgroundService
    {
        private readonly IServiceProvider _services;

        public NotificationsSenderBackgroundJob(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var NotificationsSenderService = scope.ServiceProvider.GetRequiredService<NotificationsSenderService>();

                    await NotificationsSenderService.ProcessNotificationsAsync(stoppingToken);
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
