using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Sqlite.Game
{
    public partial class NpcImmunityMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Immunities",
                table: "Npcs",
                nullable: true);
            migrationBuilder.AddColumn<double>(
                name: "Tenacity",
                table: "Npcs",
                defaultValue: "0.0",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Immunities",
                table: "Npcs");
            migrationBuilder.DropColumn(
                name: "Tenacity",
                table: "Npcs");
        }
    }
}
