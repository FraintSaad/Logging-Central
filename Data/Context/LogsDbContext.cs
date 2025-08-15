using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class LogsDbContext : DbContext
    {
        public LogsDbContext(DbContextOptions<LogsDbContext> options) : base(options) { }
        public DbSet<NotificationEntity> Notifications { get; set; }
        public DbSet<LogEntity> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntity>().ToTable("SerilogEvents");
        }
    }
}
