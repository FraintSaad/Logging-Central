using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LogLevel",
                table: "AlertRules",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_FiredAlerts_RuleId_CreatedAt",
                table: "FiredAlerts",
                columns: new[] { "RuleId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AlertRules_LogLevel",
                table: "AlertRules",
                column: "LogLevel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FiredAlerts_RuleId_CreatedAt",
                table: "FiredAlerts");

            migrationBuilder.DropIndex(
                name: "IX_AlertRules_LogLevel",
                table: "AlertRules");

            migrationBuilder.AlterColumn<string>(
                name: "LogLevel",
                table: "AlertRules",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
