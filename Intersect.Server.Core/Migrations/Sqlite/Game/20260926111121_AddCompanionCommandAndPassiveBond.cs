using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddCompanionCommandAndPassiveBond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CompanionCommand_Retreat",
                table: "Spells",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PassiveBond_BonusDamagePercent",
                table: "Spells",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassiveBond_WindowMs",
                table: "Spells",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanionCommand_Retreat",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "PassiveBond_BonusDamagePercent",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "PassiveBond_WindowMs",
                table: "Spells");
        }
    }
}
