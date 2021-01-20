using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class ItemAndSpellCooldownImprovements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CooldownGroup",
                table: "Items",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<bool>(
                name: "IgnoreGlobalCooldown",
                table: "Items",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CooldownGroup",
                table: "Spells",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<bool>(
                name: "IgnoreGlobalCooldown",
                table: "Spells",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CooldownGroup",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IgnoreGlobalCooldown",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CooldownGroup",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "IgnoreGlobalCooldown",
                table: "Spells");
        }
    }
}
