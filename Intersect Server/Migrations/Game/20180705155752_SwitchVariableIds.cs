using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class SwitchVariableIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextId",
                table: "ServerVariables",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextId",
                table: "ServerSwitches",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextId",
                table: "PlayerVariables",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextId",
                table: "PlayerSwitches",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextId",
                table: "ServerVariables");

            migrationBuilder.DropColumn(
                name: "TextId",
                table: "ServerSwitches");

            migrationBuilder.DropColumn(
                name: "TextId",
                table: "PlayerVariables");

            migrationBuilder.DropColumn(
                name: "TextId",
                table: "PlayerSwitches");
        }
    }
}
