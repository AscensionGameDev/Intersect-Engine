using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.MySql.Game
{
    public partial class AddGenericFolders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Tilesets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Tilesets",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Spells",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Shops",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "ServerVariables",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Resources",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Quests",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Projectiles",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "PlayerVariables",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Npcs",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<string>(
                name: "Folder",
                table: "Maps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Maps",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Items",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "GuildVariables",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Events",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Crafts",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "CraftingTables",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Classes",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<byte[]>(
                name: "ParentId",
                table: "Animations",
                type: "BLOB",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "BLOB", nullable: false),
                    DescriptorType = table.Column<int>(type: "INTEGER", nullable: false),
                    NameId = table.Column<byte[]>(type: "BLOB", nullable: false),
                    ParentId = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folders_ContentStrings_NameId",
                        column: x => x.NameId,
                        principalTable: "ContentStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Folders_Folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tilesets_ParentId",
                table: "Tilesets",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Spells_ParentId",
                table: "Spells",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shops_ParentId",
                table: "Shops",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerVariables_ParentId",
                table: "ServerVariables",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ParentId",
                table: "Resources",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_ParentId",
                table: "Quests",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Projectiles_ParentId",
                table: "Projectiles",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerVariables_ParentId",
                table: "PlayerVariables",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Npcs_ParentId",
                table: "Npcs",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Maps_ParentId",
                table: "Maps",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ParentId",
                table: "Items",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildVariables_ParentId",
                table: "GuildVariables",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ParentId",
                table: "Events",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Crafts_ParentId",
                table: "Crafts",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CraftingTables_ParentId",
                table: "CraftingTables",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ParentId",
                table: "Classes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Animations_ParentId",
                table: "Animations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_NameId",
                table: "Folders",
                column: "NameId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ParentId",
                table: "Folders",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animations_Folders_ParentId",
                table: "Animations",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Folders_ParentId",
                table: "Classes",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CraftingTables_Folders_ParentId",
                table: "CraftingTables",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Crafts_Folders_ParentId",
                table: "Crafts",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Folders_ParentId",
                table: "Events",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuildVariables_Folders_ParentId",
                table: "GuildVariables",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Folders_ParentId",
                table: "Items",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maps_Folders_ParentId",
                table: "Maps",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Npcs_Folders_ParentId",
                table: "Npcs",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerVariables_Folders_ParentId",
                table: "PlayerVariables",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projectiles_Folders_ParentId",
                table: "Projectiles",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Folders_ParentId",
                table: "Quests",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_Folders_ParentId",
                table: "Resources",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerVariables_Folders_ParentId",
                table: "ServerVariables",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Folders_ParentId",
                table: "Shops",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Spells_Folders_ParentId",
                table: "Spells",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tilesets_Folders_ParentId",
                table: "Tilesets",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animations_Folders_ParentId",
                table: "Animations");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Folders_ParentId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_CraftingTables_Folders_ParentId",
                table: "CraftingTables");

            migrationBuilder.DropForeignKey(
                name: "FK_Crafts_Folders_ParentId",
                table: "Crafts");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Folders_ParentId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_GuildVariables_Folders_ParentId",
                table: "GuildVariables");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Folders_ParentId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Maps_Folders_ParentId",
                table: "Maps");

            migrationBuilder.DropForeignKey(
                name: "FK_Npcs_Folders_ParentId",
                table: "Npcs");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerVariables_Folders_ParentId",
                table: "PlayerVariables");

            migrationBuilder.DropForeignKey(
                name: "FK_Projectiles_Folders_ParentId",
                table: "Projectiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Folders_ParentId",
                table: "Quests");

            migrationBuilder.DropForeignKey(
                name: "FK_Resources_Folders_ParentId",
                table: "Resources");

            migrationBuilder.DropForeignKey(
                name: "FK_ServerVariables_Folders_ParentId",
                table: "ServerVariables");

            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Folders_ParentId",
                table: "Shops");

            migrationBuilder.DropForeignKey(
                name: "FK_Spells_Folders_ParentId",
                table: "Spells");

            migrationBuilder.DropForeignKey(
                name: "FK_Tilesets_Folders_ParentId",
                table: "Tilesets");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropIndex(
                name: "IX_Tilesets_ParentId",
                table: "Tilesets");

            migrationBuilder.DropIndex(
                name: "IX_Spells_ParentId",
                table: "Spells");

            migrationBuilder.DropIndex(
                name: "IX_Shops_ParentId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_ServerVariables_ParentId",
                table: "ServerVariables");

            migrationBuilder.DropIndex(
                name: "IX_Resources_ParentId",
                table: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Quests_ParentId",
                table: "Quests");

            migrationBuilder.DropIndex(
                name: "IX_Projectiles_ParentId",
                table: "Projectiles");

            migrationBuilder.DropIndex(
                name: "IX_PlayerVariables_ParentId",
                table: "PlayerVariables");

            migrationBuilder.DropIndex(
                name: "IX_Npcs_ParentId",
                table: "Npcs");

            migrationBuilder.DropIndex(
                name: "IX_Maps_ParentId",
                table: "Maps");

            migrationBuilder.DropIndex(
                name: "IX_Items_ParentId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_GuildVariables_ParentId",
                table: "GuildVariables");

            migrationBuilder.DropIndex(
                name: "IX_Events_ParentId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Crafts_ParentId",
                table: "Crafts");

            migrationBuilder.DropIndex(
                name: "IX_CraftingTables_ParentId",
                table: "CraftingTables");

            migrationBuilder.DropIndex(
                name: "IX_Classes_ParentId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Animations_ParentId",
                table: "Animations");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Tilesets");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Tilesets");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ServerVariables");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Projectiles");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "PlayerVariables");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Npcs");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "GuildVariables");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Crafts");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "CraftingTables");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Animations");
        }
    }
}
