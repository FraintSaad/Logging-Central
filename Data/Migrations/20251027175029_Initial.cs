using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CooldownPeriod = table.Column<int>(type: "integer", nullable: false),
                    LookbackPeriod = table.Column<int>(type: "integer", nullable: false),
                    Threshold = table.Column<int>(type: "integer", nullable: false),
                    LogLevel = table.Column<string>(type: "text", nullable: false),
                    Recipients = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FiredAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RuleId = table.Column<int>(type: "integer", nullable: false),
                    LogsCount = table.Column<int>(type: "integer", nullable: false),
                    Sent = table.Column<bool>(type: "boolean", nullable: true),
                    SendError = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiredAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerilogEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Level = table.Column<string>(type: "text", nullable: true),
                    Exception = table.Column<string>(type: "text", nullable: true),
                    Environment = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerilogEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertRules_LogLevel",
                table: "AlertRules",
                column: "LogLevel");

            migrationBuilder.CreateIndex(
                name: "IX_FiredAlerts_RuleId_CreatedAt",
                table: "FiredAlerts",
                columns: new[] { "RuleId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertRules");

            migrationBuilder.DropTable(
                name: "FiredAlerts");

            migrationBuilder.DropTable(
                name: "SerilogEvents");
        }
    }
}
