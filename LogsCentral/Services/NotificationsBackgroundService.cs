
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogsCentral.Services
{
    public class NotificationsBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly EmailService _emailService;

        public NotificationsBackgroundService(IServiceProvider services, EmailService emailService)
        {
            _services = services;
            _emailService = emailService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
                    var notifications = await db.Notifications.ToListAsync(stoppingToken);
                    foreach (var config in notifications)
                    {
                        var since = DateTime.Now.AddMinutes(-config.Period);
                        var logLevels = config.LogLevels.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        var alreadySentIds = db.SentNotifications
     .Where(ln => ln.NotificationId == config.Id)
     .Select(ln => ln.LogId)
     .ToHashSet();

                        var logs = await db.Logs
                            .Where(l => l.Timestamp >= since &&
                                        logLevels.Contains(l.Level) &&
                                        !alreadySentIds.Contains(l.Id))
                            .ToListAsync(stoppingToken);
                        if (logs.Count >= config.ThrashHold)
                        {
                            var body = string.Join("\n", logs.Select(l => $"{l.Timestamp}: {l.Level} - {l.Message}"));
                            _emailService.Send(config.Email,
                                $" Логи за последние {config.Period} минут",
                                $"Количество логов: {logs.Count}\n\n{body}");
                        }
                        db.SentNotifications.AddRange(logs.Select(l => new SentNotificationEntity
                        {
                                LogId = l.Id,
                                NotificationId = config.Id,
                                SentAt = DateTime.Now
                        }));
                        await db.SaveChangesAsync(stoppingToken);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
