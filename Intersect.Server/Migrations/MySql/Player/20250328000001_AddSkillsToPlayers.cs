using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.MySql.Player
{
    public partial class AddSkillsToPlayers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "Players",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Players");
        }
    }
}

