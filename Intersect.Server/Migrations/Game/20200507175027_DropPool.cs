using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class DropPool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DropPool",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ItemListed = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropPool", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DropPool");
        }
    }
}
