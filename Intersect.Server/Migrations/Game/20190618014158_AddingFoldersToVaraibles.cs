using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{

    public partial class AddingFoldersToVaraibles : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "Folder", table: "ServerVariables", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "PlayerVariables", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Folder", table: "ServerVariables");

            migrationBuilder.DropColumn(name: "Folder", table: "PlayerVariables");
        }

    }

}
