using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class PlayerTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Players",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Players");
        }
    }
}
