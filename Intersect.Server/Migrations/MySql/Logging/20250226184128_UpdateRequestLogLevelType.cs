using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.MySql.Logging
{
    /// <inheritdoc />
    public partial class UpdateRequestLogLevelType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Level",
                table: "RequestLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Level",
                table: "RequestLogs",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
