using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class CustomAnimations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CastSpriteOverride",
                table: "Spells",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeaponSpriteOverride",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttackSpriteOverride",
                table: "Classes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CastSpriteOverride",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "WeaponSpriteOverride",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AttackSpriteOverride",
                table: "Classes");
        }
    }
}
