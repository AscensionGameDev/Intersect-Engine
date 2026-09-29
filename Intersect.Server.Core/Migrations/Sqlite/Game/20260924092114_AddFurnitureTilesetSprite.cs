using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddFurnitureTilesetSprite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TileX",
                table: "Furniture",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TileY",
                table: "Furniture",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TilesetId",
                table: "Furniture",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "UseTilesetSprite",
                table: "Furniture",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TileX",
                table: "Furniture");

            migrationBuilder.DropColumn(
                name: "TileY",
                table: "Furniture");

            migrationBuilder.DropColumn(
                name: "TilesetId",
                table: "Furniture");

            migrationBuilder.DropColumn(
                name: "UseTilesetSprite",
                table: "Furniture");
        }
    }
}
