using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class OverTimeSpellAnimationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OverTimeAnimation",
                table: "Spells",
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverTimeAnimation",
                table: "Spells");
        }
    }
}
