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
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Spells",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Shops",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "ServerVariables",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Resources",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Quests",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Projectiles",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "PlayerVariables",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Npcs",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Maps",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Items",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "GuildVariables",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Events",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Crafts",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "CraftingTables",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Classes",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Animations",
                type: "longtext",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MapFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    JsonData = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapFolders", x => x.Id);
                });
        }
    }
}
