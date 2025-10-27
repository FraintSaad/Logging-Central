using Data.Context;
using LogsCentral.Interfaces;
using LogsCentral.Jobs;
using LogsCentral.Models;
using LogsCentral.Services;
using LogsCentral.Templates;
using LogsCentral.ViewModels;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;
using System.Text;

namespace LogsCentral
{
    /*
     * TODO: 
     * Add cleanup service for old logs and fired alerts
     * Move Auth middleware to its own class
     * Add YesNo popup for deleting alert rules
     */
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();

            var appSettings = new AppSettings();
            builder.Configuration.Bind(appSettings);
            //appSettings.Validate();

            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton(appSettings);

            builder.Services.AddScoped<CleanUpService>();
            builder.Services.AddScoped<AlertService>();
            builder.Services.AddScoped<AlertEmailGenerator>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<LogsViewModel>();
            builder.Services.AddScoped<AlertRulesPageViewModel>();
            builder.Services.AddScoped<StatusPageViewModel>();
            builder.Services.AddScoped<LivePageViewModel>();

            builder.Services.AddHostedService<AlertsBackgroundJob>();
            builder.Services.AddHostedService<CleanUpBackgroundJob>();

            builder.Services.AddDbContext<LogsDbContext>(options => options
                .UseNpgsql(appSettings.DatabaseConnectionString)
            );

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console();

            builder.Logging.ClearProviders();
            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                loggerConfig.WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day);

                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();


                // ===== Basic Auth Middleware =====
                app.Use(async (context, next) =>
                {
                    const string authHeader = "Authorization";
                    string username = builder.Configuration.GetValue<string>("AuthUsername")!;
                    string password = builder.Configuration.GetValue<string>("AuthPassword")!;

                    if (!context.Request.Headers.ContainsKey(authHeader))
                    {
                        context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"LogsCentral\"";
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Authentication required.");
                        return;
                    }

                    var auth = context.Request.Headers[authHeader].ToString();
                    if (auth.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                    {
                        var encodedCredentials = auth["Basic ".Length..].Trim();
                        var decodedBytes = Convert.FromBase64String(encodedCredentials);
                        var decodedCredentials = Encoding.UTF8.GetString(decodedBytes).Split(':', 2);

                        if (decodedCredentials.Length == 2 &&
                            decodedCredentials[0] == username &&
                            decodedCredentials[1] == password)
                        {
                            await next.Invoke(); // Authenticated
                            return;
                        }
                    }

                    // Unauthorized
                    context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"LogsCentral\"";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Authentication required.");
                });
                // ===== End Basic Auth =====
            }
            Log.Logger = loggerConfig.CreateLogger();

            app.UseRouting();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.MapControllers();
            app.MapGet("/", () => Results.Redirect("logs", true));

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
                dbContext.Database.Migrate();
            }

            app.Run();
        }
    }
}