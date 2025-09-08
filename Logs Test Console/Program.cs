using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Logs_Test_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
                Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                 .Enrich.FromLogContext()
                 .WriteTo.MSSqlServer(
                       connectionString: "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LogsDB;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=True",
                       sinkOptions: new MSSqlServerSinkOptions
                       {
                           TableName = "SerilogEvents",
                           AutoCreateSqlTable = true,
                       },
                       columnOptions: new ColumnOptions()
                 )
                 .CreateLogger();

                Log.Information("Приложение запущено");
                Log.Debug("Отладочное сообщение");
                Log.Information("Информационное сообщение");
                Log.Warning("Warning Log");
                Log.Error("Error log");
                Log.Fatal("Фатальная ошибка!");
                Log.CloseAndFlush();
                Console.WriteLine("Логи отправились");
        }
    }
}
