using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{

    public partial class RefactoringCooldowns : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Sqlite doesn't support drop column...
            //migrationBuilder.DropColumn(
            //    name: "SpellCd",
            //    table: "Player_Spells");

            migrationBuilder.AddColumn<string>(name: "ItemCooldowns", table: "Players", nullable: true);

            migrationBuilder.AddColumn<string>(name: "SpellCooldowns", table: "Players", nullable: true);
        }

    }

}
