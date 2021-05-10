using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class AddingGuildsWithGuildBanks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DbGuildId",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GuildJoinDate",
                table: "Players",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GuildRank",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    FoundingDate = table.Column<DateTime>(nullable: false),
                    BankSlotsCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guild_Bank",
                columns: table => new
                {
                    BagId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    StatBuffs = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    GuildId = table.Column<Guid>(nullable: false),
                    Slot = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild_Bank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guild_Bank_Bags_BagId",
                        column: x => x.BagId,
                        principalTable: "Bags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Guild_Bank_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_DbGuildId",
                table: "Players",
                column: "DbGuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_Bank_BagId",
                table: "Guild_Bank",
                column: "BagId");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_Bank_GuildId",
                table: "Guild_Bank",
                column: "GuildId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Players_Guilds_DbGuildId",
            //    table: "Players",
            //    column: "DbGuildId",
            //    principalTable: "Guilds",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
