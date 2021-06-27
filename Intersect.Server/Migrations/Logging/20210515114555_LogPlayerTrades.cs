using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Logging
{
    public partial class LogPlayerTrades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TradeHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TradeId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    Ip = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    TargetId = table.Column<Guid>(nullable: false),
                    Items = table.Column<string>(nullable: true),
                    TargetItems = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeHistory");
        }
    }
}
