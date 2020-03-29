using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{

    public partial class Spell_Upgrades_Traps_Item_Upgrades_Effects_Event_Upgrades_Labels : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(name: "Trap", table: "Spells", nullable: false, defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PierceTarget", table: "Projectiles", nullable: false, defaultValue: false
            );

            migrationBuilder.AddColumn<string>(name: "VitalsRegen", table: "Items", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Trap", table: "Spells");

            migrationBuilder.DropColumn(name: "PierceTarget", table: "Projectiles");

            migrationBuilder.DropColumn(name: "VitalsRegen", table: "Items");
        }

    }

}
