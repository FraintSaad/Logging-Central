using Data.Context;
using Data.Entities;
using LogsCentral.Services;
using Microsoft.EntityFrameworkCore;

public class NotificationsSenderService
{
    private readonly LogsDbContext _db;
    private readonly EmailService _emailService;

    public NotificationsSenderService(LogsDbContext db, EmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task ProcessNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var notifications = await _db.Notifications.ToListAsync(cancellationToken);

        foreach (var config in notifications)
        {
            var since = DateTime.Now.AddMinutes(-config.Period);
            var logLevels = config.LogLevels.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var alreadySentIds = _db.SentNotifications
                                   .Where(ln => ln.NotificationId == config.Id)
                                   .Select(ln => ln.LogId)
                                   .ToHashSet();

            var logs = await _db.Logs
                               .Where(l => l.Timestamp >= since &&
                                      logLevels.Contains(l.Level) &&
                                      !alreadySentIds.Contains(l.Id)).ToListAsync(cancellationToken);

            if (logs.Count >= config.ThrashHold)
            {
                var body = string.Join("\n", logs.Select(l => $"{l.Timestamp}: {l.Level} - {l.Message}"));
                _emailService.Send(config.Email,
                    $"Логи за последние {config.Period} минут",
                    $"Количество логов: {logs.Count}\n\n{body}");

                _db.SentNotifications.AddRange(
                    logs.Select(l => new SentNotificationEntity
                    {
                        LogId = l.Id,
                        NotificationId = config.Id,
                        SentAt = DateTime.Now
                    })
                );

                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
