using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.MySql.Player
{
    /// <inheritdoc />
    public partial class OfflineGuildInvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PendingGuildInviteFromId",
                table: "Players",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "PendingGuildInviteToId",
                table: "Players",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PendingGuildInviteFromId",
                table: "Players",
                column: "PendingGuildInviteFromId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PendingGuildInviteToId",
                table: "Players",
                column: "PendingGuildInviteToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Guilds_PendingGuildInviteToId",
                table: "Players",
                column: "PendingGuildInviteToId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Players_PendingGuildInviteFromId",
                table: "Players",
                column: "PendingGuildInviteFromId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Guilds_PendingGuildInviteToId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Players_PendingGuildInviteFromId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_PendingGuildInviteFromId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_PendingGuildInviteToId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PendingGuildInviteFromId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PendingGuildInviteToId",
                table: "Players");
        }
    }
}
