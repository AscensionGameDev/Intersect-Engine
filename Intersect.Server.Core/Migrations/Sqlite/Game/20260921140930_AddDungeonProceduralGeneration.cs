using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddDungeonProceduralGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FloorTileX",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FloorTileY",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "FloorTilesetId",
                table: "Dungeons",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "MaxRoomSize",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxRooms",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinRoomSize",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinRooms",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ProceduralGeneration",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WallTileX",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WallTileY",
                table: "Dungeons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "WallTilesetId",
                table: "Dungeons",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FloorTileX",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "FloorTileY",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "FloorTilesetId",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "MaxRoomSize",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "MaxRooms",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "MinRoomSize",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "MinRooms",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "ProceduralGeneration",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "WallTileX",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "WallTileY",
                table: "Dungeons");

            migrationBuilder.DropColumn(
                name: "WallTilesetId",
                table: "Dungeons");
        }
    }
}
