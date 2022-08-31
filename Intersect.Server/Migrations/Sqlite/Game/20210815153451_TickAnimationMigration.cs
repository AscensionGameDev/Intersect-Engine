using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Intersect.Server.Migrations.Sqlite.Game
{
    public partial class TickAnimationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
               name: "TickAnimation",
               table: "Spells",
               defaultValue: migrationBuilder.IsSqlite() ? Guid.Empty.ToByteArray(): Guid.Empty,
               nullable: false);
        }
    }
}
