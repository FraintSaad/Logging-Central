using Data.Context;
using Data.Entities;
using LogsCentral.Interfaces;
using LogsCentral.Models;
using LogsCentral.Templates;
using Microsoft.EntityFrameworkCore;

public class AlertService
{
    private readonly LogsDbContext _db;
    private readonly IEmailService _emailService;
    private readonly ILogger<AlertService> _logger;
    private readonly AlertEmailGenerator _alertEmailGenerator;
    private readonly AppSettings _appSettings;

    public AlertService(
        LogsDbContext db,
        IEmailService emailService,
        ILogger<AlertService> logger,
        AppSettings appSettings,
        AlertEmailGenerator alertEmailGenerator)
    {
        _db = db;
        _emailService = emailService;
        _logger = logger;
        _alertEmailGenerator = alertEmailGenerator;
        _appSettings = appSettings;
    }

    public async Task ProcessAlertRulesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;

        // Read all rules once (AsNoTracking for read-only)
        var rules = await _db.AlertRules
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var rule in rules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(rule.LogLevel))
            {
                _logger.LogWarning("Alert rule {RuleId} has no LogLevel set; skipping", rule.Id);
                continue;
            }

            // Compute lookback window start
            var since = now.AddMinutes(-rule.LookbackPeriod);

            // Effective cooldown: explicit CooldownPeriod > 0 else fallback to LookbackPeriod
            var effectiveCooldown = rule.CooldownPeriod > 0 ? rule.CooldownPeriod : rule.LookbackPeriod;
            var cooldownSince = now.AddMinutes(-effectiveCooldown);

            var logLevels = rule.LogLevel
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Base query for matching events
            var baseQuery = _db.SerilogEvents
                .AsNoTracking()
                .Where(l => l.Environment == _appSettings.SourceEnvironment)
                // Ignore NS non-failing exceptions
                .Where(l => !(l.Level == "Error" && l.Exception != null && l.Exception.Contains("API calling limit exceeded")))
                .Where(l => l.Timestamp >= since)
                .Where(l => logLevels.Contains(l.Level));

            // Efficient count
            var matchingCount = await baseQuery.CountAsync(cancellationToken);
            if (matchingCount < rule.Threshold)
            {
                _logger.LogDebug("Rule {RuleId} not triggered: count={Count}, threshold={Threshold}", rule.Id, matchingCount, rule.Threshold);
                continue;
            }

            // Duplicate suppression: skip if there is a fired alert in the cooldown window
            var lastFired = await _db.FiredAlerts
                .AsNoTracking()
                .Where(f => f.RuleId == rule.Id)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => (DateTimeOffset?)f.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastFired.HasValue && lastFired.Value >= cooldownSince)
            {
                _logger.LogInformation("Skipping alert for Rule {RuleId}: last fired at {LastFired} (cooldown until {CooldownUntil})", rule.Id, lastFired.Value, cooldownSince);
                continue;
            }                    

            // build email bodies (assume generator accepts sample logs signature)
            var html = _alertEmailGenerator.BuildAlertHtml(rule, since, matchingCount, "🚨 Alert Triggered");
            var plain = _alertEmailGenerator.BuildAlertPlain(rule, since, matchingCount, "🚨 Alert Triggered");

            try
            {
                await _emailService.SendMailAsync(rule.Recipients, $"Alert — rule {rule.Id} triggered", html, plain);

                // persist fired alert AFTER successful send (single-process host -> safe)
                var fired = new FiredAlertEntity
                {
                    RuleId = rule.Id,
                    CreatedAt = DateTimeOffset.UtcNow,
                    LogsCount = matchingCount,
                    Sent = true                    
                };

                _db.FiredAlerts.Add(fired);
                await _db.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Alert sent for Rule {RuleId} — count={Count}", rule.Id, matchingCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send alert for Rule {RuleId}. Will retry on next run.", rule.Id);

                // Persist attempt row (not marking Sent = true) so operator can see failures:
                try
                {
                    var failedRecord = new FiredAlertEntity
                    {
                        RuleId = rule.Id,
                        CreatedAt = DateTimeOffset.UtcNow,
                        LogsCount = matchingCount,
                        Sent = false,
                        SendError = ex.Message.Length > 1000 ? ex.Message.Substring(0, 1000) : ex.Message
                    };

                    _db.FiredAlerts.Add(failedRecord);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                catch (Exception innerEx)
                {
                    // if logging to DB also fails, at least log locally
                    _logger.LogError(innerEx, "Failed to persist failed fired alert for Rule {RuleId}", rule.Id);
                }
            }
        }
    }
}
