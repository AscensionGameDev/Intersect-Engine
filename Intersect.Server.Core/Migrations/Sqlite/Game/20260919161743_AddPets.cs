using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddPets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Sprite = table.Column<string>(type: "TEXT", nullable: true),
                    Speed = table.Column<int>(type: "INTEGER", nullable: false),
                    PickupRange = table.Column<int>(type: "INTEGER", nullable: false),
                    PickupOwnerDrops = table.Column<bool>(type: "INTEGER", nullable: false),
                    PickupUnownedItems = table.Column<bool>(type: "INTEGER", nullable: false),
                    Folder = table.Column<string>(type: "TEXT", nullable: true),
                    TimeCreated = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pets");
        }
    }
}
