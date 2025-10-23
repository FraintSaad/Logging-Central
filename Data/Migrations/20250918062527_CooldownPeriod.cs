using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class CooldownPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Period",
                table: "AlertRules",
                newName: "LookbackPeriod");

            migrationBuilder.AddColumn<int>(
                name: "CooldownPeriod",
                table: "AlertRules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CooldownPeriod",
                table: "AlertRules");

            migrationBuilder.RenameColumn(
                name: "LookbackPeriod",
                table: "AlertRules",
                newName: "Period");
        }
    }
}
