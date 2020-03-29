using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{

    public partial class ItemUpgrades_Percentage_Based_Stats : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Consumable_Percentage", table: "Items", nullable: false, defaultValue: 0
            );

            migrationBuilder.AddColumn<string>(name: "PercentageStatsGiven", table: "Items", nullable: true);

            migrationBuilder.AddColumn<string>(name: "PercentageVitalsGiven", table: "Items", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Consumable_Percentage", table: "Items");

            migrationBuilder.DropColumn(name: "PercentageStatsGiven", table: "Items");

            migrationBuilder.DropColumn(name: "PercentageVitalsGiven", table: "Items");
        }

    }

}
