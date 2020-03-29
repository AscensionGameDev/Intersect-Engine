using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{

    public partial class MoreGraularAttackSpeedOptions : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttackSpeedModifier", table: "Npcs", nullable: false, defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(name: "AttackSpeedValue", table: "Npcs", nullable: false, defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AttackSpeedModifier", table: "Classes", nullable: false, defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "AttackSpeedValue", table: "Classes", nullable: false, defaultValue: 0
            );
        }

    }

}
