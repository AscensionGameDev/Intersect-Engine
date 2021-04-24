using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class AddIndividualizedLootOptionToNpcs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IndividualizedLoot",
                table: "Npcs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndividualizedLoot",
                table: "Npcs");
        }
    }
}
