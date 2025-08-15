using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.Context
{
    internal class LogsDesignDbContextFactory : IDesignTimeDbContextFactory<LogsDbContext>
    {
        public LogsDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LogsDB;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<LogsDbContext>();
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            optionsBuilder.UseSqlServer(connectionString);
            return new LogsDbContext(optionsBuilder.Options);
        }
    }
}
