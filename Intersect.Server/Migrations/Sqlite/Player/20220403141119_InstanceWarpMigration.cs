using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class InstanceWarpMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstanceType",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "LastOverworldMapId",
                table: "Players",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "LastOverworldX",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastOverworldY",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PersonalMapInstanceId",
                table: "Players",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "SharedInstanceRespawnDir",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SharedInstanceRespawnId",
                table: "Players",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "SharedInstanceRespawnX",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SharedInstanceRespawnY",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SharedMapInstanceId",
                table: "Players",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "GuildInstanceId",
                table: "Guilds",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstanceType",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LastOverworldMapId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LastOverworldX",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LastOverworldY",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PersonalMapInstanceId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SharedInstanceRespawnDir",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SharedInstanceRespawnId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SharedInstanceRespawnX",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SharedInstanceRespawnY",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SharedMapInstanceId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GuildInstanceId",
                table: "Guilds");
        }
    }
}
