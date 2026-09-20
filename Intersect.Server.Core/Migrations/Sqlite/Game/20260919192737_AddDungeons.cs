using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddDungeons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dungeons",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Icon = table.Column<string>(type: "TEXT", nullable: true),
                    AccessType = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryMapId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EntryX = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryY = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryDirection = table.Column<int>(type: "INTEGER", nullable: false),
                    MinPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxPlayers = table.Column<int>(type: "INTEGER", nullable: false),
                    RequiredLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Lives = table.Column<int>(type: "INTEGER", nullable: false),
                    CooldownSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    ExitDelaySeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    RewardExperience = table.Column<long>(type: "INTEGER", nullable: false),
                    RewardTitleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Rewards = table.Column<string>(type: "TEXT", nullable: true),
                    Folder = table.Column<string>(type: "TEXT", nullable: true),
                    TimeCreated = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dungeons", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dungeons");
        }
    }
}
