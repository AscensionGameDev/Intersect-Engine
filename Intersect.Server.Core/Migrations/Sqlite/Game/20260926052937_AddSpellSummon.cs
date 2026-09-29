using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddSpellSummon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Summon_Amount",
                table: "Spells",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Summon_DespawnOnOwnerDeath",
                table: "Spells",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Summon_Duration",
                table: "Spells",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Summon_NpcId",
                table: "Spells",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Summon_ReplaceExisting",
                table: "Spells",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summon_Amount",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "Summon_DespawnOnOwnerDeath",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "Summon_Duration",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "Summon_NpcId",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "Summon_ReplaceExisting",
                table: "Spells");
        }
    }
}
