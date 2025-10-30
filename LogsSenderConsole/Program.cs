using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Host=localhost;Port=5432;Database=mydatabase;Username=postgres;Password=postgres";

        var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "Level", new LevelColumnWriter(true, NpgsqlTypes.NpgsqlDbType.Varchar) },
            { "Exception", new ExceptionColumnWriter() },
            { "Environment", new SinglePropertyColumnWriter("Environment", PropertyWriteMethod.Raw, NpgsqlDbType.Text) },

            { "Timestamp", new TimestampColumnWriter() }
        };

        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Environment", "NS")
            .WriteTo.PostgreSQL(
                connectionString: connectionString,
                tableName: "SerilogEvents",
                columnOptions: columnWriters,
                needAutoCreateTable: false
            )
            .MinimumLevel.Debug()
            .CreateLogger();

        Log.Information("Это информационное сообщение.");
        Log.Warning("Это предупреждающее сообщение.");

        Console.WriteLine("Логи отправлены.");

        Log.CloseAndFlush();
    }
}
