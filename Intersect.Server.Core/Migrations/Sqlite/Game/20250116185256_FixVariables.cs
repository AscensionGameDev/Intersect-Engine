using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class FixVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ServerVariables",
                newName: "DataType");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "PlayerVariables",
                newName: "DataType");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "GuildVariables",
                newName: "DataType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataType",
                table: "ServerVariables",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "DataType",
                table: "PlayerVariables",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "DataType",
                table: "GuildVariables",
                newName: "Type");
        }
    }
}
