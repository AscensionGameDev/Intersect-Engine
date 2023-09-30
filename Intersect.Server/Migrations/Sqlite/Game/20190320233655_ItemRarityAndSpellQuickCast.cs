using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{

    public partial class ItemRarityAndSpellQuickCast : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DestroySpell", table: "Items", nullable: false, defaultValue: false
            );

            migrationBuilder.AddColumn<bool>(name: "QuickCast", table: "Items", nullable: false, defaultValue: false);

            migrationBuilder.AddColumn<int>(name: "Rarity", table: "Items", nullable: false, defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DestroySpell", table: "Items");

            migrationBuilder.DropColumn(name: "QuickCast", table: "Items");

            migrationBuilder.DropColumn(name: "Rarity", table: "Items");
        }

    }

}
