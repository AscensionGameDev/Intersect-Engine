using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.MySql.Player
{
    /// <inheritdoc />
    public partial class GuildCorrections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Guilds_DbGuildId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "DbGuildId",
                table: "Players",
                newName: "GuildId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_DbGuildId",
                table: "Players",
                newName: "IX_Players_GuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Guilds_GuildId",
                table: "Players",
                column: "GuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Guilds_GuildId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "GuildId",
                table: "Players",
                newName: "DbGuildId");

            migrationBuilder.RenameIndex(
                name: "IX_Players_GuildId",
                table: "Players",
                newName: "IX_Players_DbGuildId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Guilds_DbGuildId",
                table: "Players",
                column: "DbGuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
