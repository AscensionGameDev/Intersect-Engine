using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class AddingMailBox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hair",
                table: "Players",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Player_MailBox",
                columns: table => new
                {
                    ItemId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    StatBuffs = table.Column<string>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    SenderID = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player_MailBox", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_MailBox_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Player_MailBox_Players_SenderID",
                        column: x => x.SenderID,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
			
            migrationBuilder.CreateIndex(
                name: "IX_Player_MailBox_PlayerId",
                table: "Player_MailBox",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_MailBox_SenderID",
                table: "Player_MailBox",
                column: "SenderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Player_MailBox");

            migrationBuilder.DropColumn(
                name: "Hair",
                table: "Players");
        }
    }
}
