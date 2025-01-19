using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.MySql.Player
{
    /// <inheritdoc />
    public partial class UserVariables_AddStandaloneIdPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User_Variables",
                table: "User_Variables");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "User_Variables",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_Variables",
                table: "User_Variables",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Variables_VariableId_UserId",
                table: "User_Variables",
                columns: new[] { "VariableId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User_Variables",
                table: "User_Variables");

            migrationBuilder.DropIndex(
                name: "IX_User_Variables_VariableId_UserId",
                table: "User_Variables");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "User_Variables");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_Variables",
                table: "User_Variables",
                columns: new[] { "VariableId", "UserId" });
        }
    }
}
