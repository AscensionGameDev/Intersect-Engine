using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class MultipleBonusEffectsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Effect_Percentage",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Effect_Type",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "Effects",
                table: "Items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Effects",
                table: "Items");

            migrationBuilder.AddColumn<int>(
                name: "Effect_Percentage",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "Effect_Type",
                table: "Items",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
