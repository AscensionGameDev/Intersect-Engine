using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class AddingGuildVariables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guild_Variables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    VariableId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    GuildId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guild_Variables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guild_Variables_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guild_Variables_GuildId",
                table: "Guild_Variables",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Guild_Variables_VariableId_GuildId",
                table: "Guild_Variables",
                columns: new[] { "VariableId", "GuildId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guild_Variables");
        }
    }
}
