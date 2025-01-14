using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Player
{
    /// <inheritdoc />
    public partial class SqliteGuildForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Guilds_DbGuildId",
                table: "Players");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Guilds_DbGuildId",
                table: "Players",
                column: "DbGuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Do nothing, ideally this would have existed before
        }
    }
}
