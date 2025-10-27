using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class LogsDbContext : DbContext
    {
        public DbSet<LogEntity> SerilogEvents { get; set; }
        public DbSet<AlertRuleEntity> AlertRules { get; set; }
        public DbSet<FiredAlertEntity> FiredAlerts { get; set; }
        public LogsDbContext(DbContextOptions<LogsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            modelBuilder.Entity<FiredAlertEntity>(entity =>
            {
                entity.ToTable("FiredAlerts");
                entity.HasIndex(e => new { e.RuleId, e.CreatedAt });
            });

            modelBuilder.Entity<AlertRuleEntity>(entity =>
            {
                entity.ToTable("AlertRules");
                entity.HasIndex(e => e.LogLevel);
            });

            modelBuilder.Entity<LogEntity>(entity =>
            {
                entity.ToTable("SerilogEvents");
                entity.HasKey(e => e.Id); 
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}