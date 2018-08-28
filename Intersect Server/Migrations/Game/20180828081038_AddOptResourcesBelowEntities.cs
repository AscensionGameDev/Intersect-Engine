using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class AddOptResourcesBelowEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Exhausted_RenderBelowEntities",
                table: "Resources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Initial_RenderBelowEntities",
                table: "Resources",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Exhausted_RenderBelowEntities",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Initial_RenderBelowEntities",
                table: "Resources");
        }
    }
}
