using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class RefactoringCooldowns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Mutes_PlayerId",
                table: "Mutes");

            migrationBuilder.DropIndex(
                name: "IX_Bans_PlayerId",
                table: "Bans");

            //Sqlite doesn't support drop column...
            //migrationBuilder.DropColumn(
            //    name: "SpellCd",
            //    table: "Player_Spells");

            migrationBuilder.AddColumn<string>(
                name: "ItemCooldowns",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpellCooldowns",
                table: "Players",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mutes_PlayerId",
                table: "Mutes",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bans_PlayerId",
                table: "Bans",
                column: "PlayerId",
                unique: true);
        }
    }
}
