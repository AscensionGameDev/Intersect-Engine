using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class Player_HDV : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HDV",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
					HDVId = table.Column<Guid>(nullable: false),
					SellerId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    StatBuffs = table.Column<string>(nullable: true),
                    Price = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HDV", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HDV");
        }
    }
}
