using Data.Entities;
using LogsCentral.Models;
using System.Globalization;

namespace LogsCentral.Templates
{
    public class AlertEmailGenerator
    {
        private readonly string _logsBaseUrl;

        public AlertEmailGenerator(AppSettings appSettings)
        {
            _logsBaseUrl = appSettings.AppUrl + "/logs";
        }

        /// <summary>
        /// Builds an HTML email body for a fired alert based on an AlertRuleEntity.
        /// </summary>
        public string BuildAlertHtml(AlertRuleEntity rule, DateTimeOffset since, int logsCount, string? alertTitle = null, string? footerNote = null)
        {
            string title = string.IsNullOrWhiteSpace(alertTitle) ? "🚨 Alert Triggered" : alertTitle!;
            string level = rule.LogLevel?.Trim() ?? "INFO";
            string levelColor = GetLogLevelColor(level);

            var nowUtc = DateTimeOffset.UtcNow;
            string levelQueryKey = GetLogLevelQueryKey(level);
            string startIso = since.UtcDateTime.ToString("o", CultureInfo.InvariantCulture);
            string endIso = nowUtc.UtcDateTime.ToString("o", CultureInfo.InvariantCulture);
            string logsUrl = $"{_logsBaseUrl}?{levelQueryKey}=on&startTime={Uri.EscapeDataString(startIso)}&endTime={Uri.EscapeDataString(endIso)}";

            string html = $@"
                            <!DOCTYPE html>
                            <html lang=""en"">
                            <head>
                              <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
                              <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                              <title>{System.Net.WebUtility.HtmlEncode(title)}</title>
                            </head>
                            <body style=""margin:0;padding:0;background:#f6f7fb;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;color:#1f2937;"">
                              <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"">
                                <tr>
                                  <td align=""center"" style=""padding:24px;"">
                                    <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""600"" style=""max-width:600px;width:100%;background:#ffffff;border-radius:12px;box-shadow:0 2px 6px rgba(0,0,0,0.05);overflow:hidden;"">
                                      <!-- Header -->
                                      <tr>
                                        <td style=""padding:24px 24px 12px 24px;border-bottom:1px solid #eef2f7;"">
                                          <div style=""font-size:20px;line-height:28px;font-weight:700;"">{System.Net.WebUtility.HtmlEncode(title)}</div>
                                          <div style=""margin-top:6px;font-size:13px;line-height:18px;color:#6b7280;"">A rule condition has just fired.</div>
                                        </td>
                                      </tr>

                                      <!-- Meta row with level badge -->
                                      <tr>
                                        <td style=""padding:16px 24px 0 24px;"">
                                          <span style=""display:inline-block;padding:6px 10px;border-radius:999px;background:{levelColor};color:#ffffff;font-size:12px;line-height:1;font-weight:700;letter-spacing:.3px;text-transform:uppercase;"">
                                            {System.Net.WebUtility.HtmlEncode(level)}
                                          </span>
                                        </td>
                                      </tr>

                                      <!-- Details table -->
                                      <tr>
                                        <td style=""padding:16px 24px 24px 24px;"">
                                          <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" style=""border-collapse:separate;border-spacing:0 8px;"">
                                            <tr>
                                              <td style=""width:160px;color:#6b7280;font-size:13px;vertical-align:top;"">Rule ID</td>
                                              <td style=""font-size:14px;font-weight:600;vertical-align:top;"">{rule.Id}</td>
                                            </tr>
                                            <tr>
                                              <td style=""width:160px;color:#6b7280;font-size:13px;vertical-align:top;"">Period since</td>
                                              <td style=""font-size:14px;font-weight:600;vertical-align:top;"">{since:yyyy-MM-dd HH:mm:ss}</td>
                                            </tr>
                                            <tr>
                                              <td style=""width:160px;color:#6b7280;font-size:13px;vertical-align:top;"">Threshold</td>
                                              <td style=""font-size:14px;font-weight:600;vertical-align:top;"">{rule.Threshold}</td>
                                            </tr>
                                            <tr>
                                              <td style=""width:160px;color:#6b7280;font-size:13px;vertical-align:top;"">Count</td>
                                              <td style=""font-size:14px;font-weight:600;vertical-align:top;"">{logsCount}</td>
                                            </tr>
                                            <tr>
                                              <td style=""width:160px;color:#6b7280;font-size:13px;vertical-align:top;"">Fired At (UTC)</td>
                                              <td style=""font-size:14px;font-weight:600;vertical-align:top;"">{nowUtc:yyyy-MM-dd HH:mm:ss}</td>
                                            </tr>
                                          </table>

                                          <!-- Callout -->
                                          <div style=""margin-top:16px;padding:12px 14px;border:1px solid #e5e7eb;border-radius:10px;background:#fafafa;font-size:13px;color:#374151;"">
                                            This alert fired because the observed value met or exceeded the configured <strong>Threshold</strong> within the specified <strong>Period</strong>.
                                          </div>

                                          <!-- Logs Link -->
                                          <div style=""margin-top:20px;"">
                                            <a href=""{System.Net.WebUtility.HtmlEncode(logsUrl)}""
                                               style=""display:inline-block;text-decoration:none;background:#111827;color:#ffffff;padding:12px 16px;border-radius:10px;font-size:14px;font-weight:700;"">
                                              Open Logs (filtered)
                                            </a>
                                            <div style=""margin-top:8px;font-size:11px;color:#9ca3af;word-break:break-all;"">
                                              {System.Net.WebUtility.HtmlEncode(logsUrl)}
                                            </div>
                                          </div>
                                        </td>
                                      </tr>

                                      <!-- Footer -->
                                      <tr>
                                        <td style=""padding:16px 24px 24px 24px;border-top:1px solid #eef2f7;color:#6b7280;font-size:12px;"">
                                          {System.Net.WebUtility.HtmlEncode(footerNote ?? "This message was generated automatically by the monitoring system.")}
                                        </td>
                                      </tr>
                                    </table>

                                    <div style=""padding:16px 8px 0 8px;color:#9ca3af;font-size:11px;"">
                                      © {nowUtc:yyyy} eSkillz. All rights reserved.
                                    </div>
                                  </td>
                                </tr>
                              </table>
                            </body>
                            </html>";

            return html;
        }

        /// <summary>
        /// Builds a plain-text fallback body for the same alert.
        /// </summary>
        public string BuildAlertPlain(AlertRuleEntity rule, DateTimeOffset since, int logsCount, string? alertTitle = null)
        {
            var nowUtc = DateTimeOffset.UtcNow;
            string title = string.IsNullOrWhiteSpace(alertTitle) ? "ALERT TRIGGERED" : alertTitle!;
            string level = rule.LogLevel?.Trim() ?? "INFO";

            string levelQueryKey = GetLogLevelQueryKey(level);
            string startIso = since.UtcDateTime.ToString("o", CultureInfo.InvariantCulture);
            string endIso = nowUtc.UtcDateTime.ToString("o", CultureInfo.InvariantCulture);
            string logsUrl = $"{_logsBaseUrl}?{levelQueryKey}=true&startTime={Uri.EscapeDataString(startIso)}&endTime={Uri.EscapeDataString(endIso)}";

            return $@"
                    {title}
                    ==============================

                    Log Level: {level}
                    Fired At (UTC): {nowUtc:yyyy-MM-dd HH:mm:ss}
                    Logs Count (actual): {logsCount}
                    Period since: {since:yyyy-MM-dd HH:mm:ss}

                    Rule Details:
                    - Rule ID: {rule.Id}
                    - Threshold: {rule.Threshold}

                    Open Logs (filtered):
                    {logsUrl}

                    This alert fired because {logsCount} events matched {level} within the specified period.

                    -- 
                    This message was generated automatically by the monitoring system.
                    © {nowUtc:yyyy} eSkillz. All rights reserved.
                    ".Trim();
        }

        private static string GetLogLevelQueryKey(string logLevel)
        {
            switch (logLevel.ToUpperInvariant())
            {
                case "CRITICAL":
                case "FATAL":
                case "ERROR": return "logLevelErro";
                case "WARN":
                case "WARNING": return "logLevelWarning";
                case "INFO": return "logLevelInfo";
                case "DEBUG": return "logLevelDebug";
                // If TRACE has its own key later, add it here; default to Info:
                default: return "logLevelInfo";
            }
        }

        private static string GetLogLevelColor(string logLevel)
        {
            switch (logLevel.ToUpperInvariant())
            {
                case "CRITICAL":
                case "FATAL":
                case "ERROR": return "#dc2626"; // red-600
                case "WARN":
                case "WARNING": return "#d97706"; // amber-600
                case "INFO": return "#2563eb"; // blue-600
                case "DEBUG": return "#6b7280"; // gray-500
                case "TRACE": return "#10b981"; // emerald-500
                default: return "#374151"; // gray-700
            }
        }
    }
}
