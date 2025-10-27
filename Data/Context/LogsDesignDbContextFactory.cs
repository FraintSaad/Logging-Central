using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.Context
{
    internal class LogsDesignDbContextFactory : IDesignTimeDbContextFactory<LogsDbContext>
    {
        public LogsDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Host=localhost;Port=5432;Database=mydatabase;Username=postgres;Password=postgres";

            var optionsBuilder = new DbContextOptionsBuilder<LogsDbContext>();
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            optionsBuilder.UseNpgsql(connectionString);
            return new LogsDbContext(optionsBuilder.Options);
        }
    }
}
