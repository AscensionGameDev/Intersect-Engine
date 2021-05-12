using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class BoundItemExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanBag",
                table: "Items",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanBank",
                table: "Items",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "DropChanceOnDeath",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CanSell",
                table: "Items",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanTrade",
                table: "Items",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanBag",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CanBank",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DropChanceOnDeath",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CanSell",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CanTrade",
                table: "Items");
        }
    }
}
