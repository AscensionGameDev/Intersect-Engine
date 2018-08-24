using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class AnimationLayersUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Lower_AlternateRenderLayer",
                table: "Animations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Upper_AlternateRenderLayer",
                table: "Animations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lower_AlternateRenderLayer",
                table: "Animations");

            migrationBuilder.DropColumn(
                name: "Upper_AlternateRenderLayer",
                table: "Animations");
        }
    }
}
