using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class AddingShopSoundEffects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuySound",
                table: "Shops",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellSound",
                table: "Shops",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuySound",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "SellSound",
                table: "Shops");
        }
    }
}
