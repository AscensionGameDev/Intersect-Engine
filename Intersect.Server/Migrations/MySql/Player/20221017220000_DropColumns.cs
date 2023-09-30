using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class DropColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            switch (migrationBuilder.ActiveProvider)
            {
                case "Microsoft.EntityFrameworkCore.Sqlite":
                    // Removed because this will wipe out anything fkey reliant
                    break;

                case "Pomelo.EntityFrameworkCore.MySql":
                    migrationBuilder.DropColumn(
                        name: "SpellCd",
                        table: "Player_Spells");

                    migrationBuilder.DropColumn(
                        name: "PlayTime",
                        table: "Players");

                    migrationBuilder.DropColumn(
                        name: "PlayTime",
                        table: "Users");
                    break;

                default:
                    throw new NotSupportedException(migrationBuilder.ActiveProvider);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
