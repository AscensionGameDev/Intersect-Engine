using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Intersect.Server.Migrations.Game
{
    public partial class GameData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisableLowerRotations = table.Column<bool>(nullable: false),
                    DisableUpperRotations = table.Column<bool>(nullable: false),
                    LowerFrameCount = table.Column<int>(nullable: false),
                    LowerFrameSpeed = table.Column<int>(nullable: false),
                    LowerLoopCount = table.Column<int>(nullable: false),
                    LowerSprite = table.Column<string>(nullable: true),
                    LowerXFrames = table.Column<int>(nullable: false),
                    LowerYFrames = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Sound = table.Column<string>(nullable: true),
                    UpperFrameCount = table.Column<int>(nullable: false),
                    UpperFrameSpeed = table.Column<int>(nullable: false),
                    UpperLoopCount = table.Column<int>(nullable: false),
                    UpperSprite = table.Column<string>(nullable: true),
                    UpperXFrames = table.Column<int>(nullable: false),
                    UpperYFrames = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animations");
        }
    }
}
