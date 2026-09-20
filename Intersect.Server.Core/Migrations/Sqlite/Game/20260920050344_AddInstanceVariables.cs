using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddInstanceVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstanceVariables",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TimeCreated = table.Column<long>(type: "INTEGER", nullable: false),
                    DataType = table.Column<byte>(type: "INTEGER", nullable: false),
                    Folder = table.Column<string>(type: "TEXT", nullable: true),
                    TextId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstanceVariables", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstanceVariables");
        }
    }
}
