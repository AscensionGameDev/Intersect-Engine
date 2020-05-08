using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class Game_HDV_ItemListed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemListed",
                table: "HDVs",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isWhiteList",
                table: "HDVs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemListed",
                table: "HDVs");

            migrationBuilder.DropColumn(
                name: "isWhiteList",
                table: "HDVs");
        }
    }
}
