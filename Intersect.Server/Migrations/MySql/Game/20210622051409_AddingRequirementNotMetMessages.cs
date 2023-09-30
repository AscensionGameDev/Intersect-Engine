using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class AddingRequirementNotMetMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CannotCastMessage",
                table: "Spells",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CannotHarvestMessage",
                table: "Resources",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CannotUseMessage",
                table: "Items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CannotCastMessage",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "CannotHarvestMessage",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "CannotUseMessage",
                table: "Items");
        }
    }
}
