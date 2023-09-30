using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.MySql.Game
{
    public partial class TickAnimationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
               name: "TickAnimation",
               table: "Spells",
               defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
               nullable: false);
        }
    }
}
