using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class NpcResourceRegens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VitalRegen",
                table: "Resources",
                nullable: false,
                defaultValue: 10);

            migrationBuilder.AddColumn<string>(
                name: "VitalRegen",
                table: "Npcs",
                nullable: true,
                defaultValue: "[10,10]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VitalRegen",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "VitalRegen",
                table: "Npcs");
        }
    }
}
