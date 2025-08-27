using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class LogsDbContext : DbContext
    {
        public LogsDbContext(DbContextOptions<LogsDbContext> options) : base(options) { }
        public DbSet<NotificationsRuleEntity> NotificationRules { get; set; }
        public DbSet<LogEntity> SerilogEvents { get; set; }
        public DbSet<SentNotificationEntity> SentNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntity>().ToTable("SerilogEvents");
        }
    }
}
