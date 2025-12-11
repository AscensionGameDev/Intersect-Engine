using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class ConsolidateHealingEffects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Combat_PercentageEffect",
                table: "Spells",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Combat_PercentageEffect",
                table: "Spells");

            migrationBuilder.AddColumn<int>(
                name: "Combat_GrievousWoundsReduction",
                table: "Spells",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Combat_HealingBoostPercentage",
                table: "Spells",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
