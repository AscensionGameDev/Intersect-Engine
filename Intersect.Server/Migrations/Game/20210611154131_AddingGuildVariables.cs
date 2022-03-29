using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class AddingGuildVariables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildVariables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    Folder = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildVariables", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildVariables");
        }
    }
}
