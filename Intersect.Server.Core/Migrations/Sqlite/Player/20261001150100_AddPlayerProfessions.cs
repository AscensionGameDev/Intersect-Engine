using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Player
{
    /// <inheritdoc />
    public partial class AddPlayerProfessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "Professions", table: "Players", type: "TEXT", nullable: true);
            migrationBuilder.AddColumn<string>(name: "KnownRecipes", table: "Players", type: "TEXT", nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Professions", table: "Players");
            migrationBuilder.DropColumn(name: "KnownRecipes", table: "Players");
        }
    }
}
