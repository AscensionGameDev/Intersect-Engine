using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.MySql.Game
{
    public partial class DropLegacyFolder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapFolders");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Tilesets");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "ServerVariables");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Projectiles");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "PlayerVariables");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Npcs");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "GuildVariables");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Crafts");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "CraftingTables");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Animations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Tilesets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Spells",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Shops",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "ServerVariables",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Resources",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Quests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Projectiles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "PlayerVariables",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Npcs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Maps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "GuildVariables",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Events",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Crafts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "CraftingTables",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Classes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Animations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MapFolders",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "BLOB", nullable: false),
                    JsonData = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapFolders", x => x.Id);
                });
        }
    }
}
