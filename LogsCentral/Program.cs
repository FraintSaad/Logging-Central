using Data.Context;
using LogsCentral.Models;
using LogsCentral.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace LogsCentral
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var emailSettings = new EmailSettings();
            builder.Configuration.GetSection("EmailSettings").Bind(emailSettings);
            emailSettings.Validate();

            builder.Services.AddSingleton(emailSettings);
            builder.Services.AddSingleton<EmailService>();


            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .WriteTo.Console()
              .Enrich.FromLogContext()
              .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        TableName = "SerilogEvents",
                        AutoCreateSqlTable = true,
                    },
                    columnOptions: new ColumnOptions(),
                    appConfiguration: builder.Configuration
              )
              .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog();


            builder.Services.AddDbContext<LogsDbContext>(options => options.UseSqlServer(connectionString));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
                dbContext.Database.Migrate();
            }
            app.Run();
        }
    }
}
