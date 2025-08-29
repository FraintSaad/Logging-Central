using Data.Context;
using Data.Entities;
using Data.Models;
using LogsCentral.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

public class NotificationsSenderServiceTests
{
    private NotificationsSenderService CreateService(string dbName, out LogsDbContext context, out Mock<EmailService> emailMock)
    {
        var options = new DbContextOptionsBuilder<LogsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        context = new LogsDbContext(options);
        emailMock = new Mock<EmailService>(null);

        return new NotificationsSenderService(context, emailMock.Object);
    }

    [Fact]
    public async Task ProcessNotificationsAsync_ShouldSendEmail_WhenThresholdExceeded()
    {
        var service = CreateService(nameof(ProcessNotificationsAsync_ShouldSendEmail_WhenThresholdExceeded),
            out var context, out var emailMock);

        context.NotificationRules.Add(new NotificationsRuleEntity
        {
            Id = 1,
            Period = 60,
            Threshold = 2,
            LogLevel = "Error",
            Email = "test@mail.com"
        });

        context.SerilogEvents.Add(new LogEntity { Id = 1, Timestamp = DateTime.Now, Level = "Error", Message = "Error 1" });
        context.SerilogEvents.Add(new LogEntity { Id = 2, Timestamp = DateTime.Now, Level = "Error", Message = "Error 2" });

        await context.SaveChangesAsync();

        await service.ProcessNotificationsAsync();

        emailMock.Verify(e => e.Send("test@mail.com",
            It.Is<string>(s => s.Contains("Logs for the last")),
            It.Is<string>(s => s.Contains("Error 1") && s.Contains("Error 2"))),
            Times.Once);

        Assert.Equal(2, context.SentNotifications.Count());
    }

    [Fact]
    public async Task ProcessNotificationsAsync_ShouldNotSendEmail_WhenThresholdNotReached()
    {
        var service = CreateService(nameof(ProcessNotificationsAsync_ShouldNotSendEmail_WhenThresholdNotReached),
            out var context, out var emailMock);

        context.NotificationRules.Add(new NotificationsRuleEntity
        {
            Id = 1,
            Period = 60,
            Threshold = 3,
            LogLevel = "Error",
            Email = "test@mail.com"
        });

        context.SerilogEvents.Add(new LogEntity { Id = 1, Timestamp = DateTime.Now, Level = "Error", Message = "Only one error" });
        await context.SaveChangesAsync();

        await service.ProcessNotificationsAsync();

        emailMock.Verify(e => e.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        Assert.Empty(context.SentNotifications);
    }

    [Fact]
    public async Task ProcessNotificationsAsync_ShouldSkipAlreadySentLogs()
    {
        var service = CreateService(nameof(ProcessNotificationsAsync_ShouldSkipAlreadySentLogs),
            out var context, out var emailMock);

        context.NotificationRules.Add(new NotificationsRuleEntity
        {
            Id = 1,
            Period = 60,
            Threshold = 1,
            LogLevel = "Error",
            Email = "test@mail.com"
        });

        context.SerilogEvents.Add(new LogEntity { Id = 1, Timestamp = DateTime.Now, Level = "Error", Message = "Old error" });
        context.SentNotifications.Add(new SentNotificationEntity { LogId = 1, RuleId = 1, SentAt = DateTime.Now });
        await context.SaveChangesAsync();

        await service.ProcessNotificationsAsync();

        emailMock.Verify(e => e.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}