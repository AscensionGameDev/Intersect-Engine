using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{

    public partial class AddingFoldersToGameObjects : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "Folder", table: "Spells", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "Shops", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "Resources", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "Quests", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "Projectiles", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "Npcs", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "Items", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "Events", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "Crafts", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "CraftingTables", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "Classes", nullable: true);

            migrationBuilder.AddColumn<string>(name: "Folder", table: "Animations", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Folder", table: "Spells");

            migrationBuilder.DropColumn(name: "Folder", table: "Shops");

            migrationBuilder.DropColumn(name: "Folder", table: "Resources");

            migrationBuilder.DropColumn(name: "Folder", table: "Quests");

            migrationBuilder.DropColumn(name: "Folder", table: "Projectiles");

            migrationBuilder.DropColumn(name: "Folder", table: "Npcs");

            migrationBuilder.DropColumn(name: "Folder", table: "Items");

            migrationBuilder.DropColumn(name: "Folder", table: "Events");

            migrationBuilder.DropColumn(name: "Folder", table: "Crafts");

            migrationBuilder.DropColumn(name: "Folder", table: "CraftingTables");

            migrationBuilder.DropColumn(name: "Folder", table: "Classes");

            migrationBuilder.DropColumn(name: "Folder", table: "Animations");
        }

    }

}
