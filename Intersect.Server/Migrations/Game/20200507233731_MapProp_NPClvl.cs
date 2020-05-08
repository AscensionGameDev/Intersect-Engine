using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class MapProp_NPClvl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NpcLevelMax",
                table: "Maps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NpcLevelMin",
                table: "Maps",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NpcLevelMax",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "NpcLevelMin",
                table: "Maps");
        }
    }
}
