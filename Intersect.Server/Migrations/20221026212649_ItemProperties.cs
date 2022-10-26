using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class ItemProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemProperties",
                table: "Player_Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemProperties",
                table: "Player_Bank",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemProperties",
                table: "Guild_Bank",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemProperties",
                table: "Bag_Items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemProperties",
                table: "Player_Items");

            migrationBuilder.DropColumn(
                name: "ItemProperties",
                table: "Player_Bank");

            migrationBuilder.DropColumn(
                name: "ItemProperties",
                table: "Guild_Bank");

            migrationBuilder.DropColumn(
                name: "ItemProperties",
                table: "Bag_Items");
        }
    }
}
