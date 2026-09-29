using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddDungeonMonsterSpawns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BossNpcId",
                table: "Dungeons",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "MaxNpcsPerRoom",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinNpcsPerRoom",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MonsterPoolNpcIds",
                table: "Dungeons",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BossNpcId",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "MaxNpcsPerRoom",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "MinNpcsPerRoom",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "MonsterPoolNpcIds",
                table: "Dungeons");
        }
    }
}
