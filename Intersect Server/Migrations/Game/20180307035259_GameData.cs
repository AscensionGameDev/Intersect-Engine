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
                    LowerAnimFrameCount = table.Column<int>(nullable: false),
                    LowerAnimFrameSpeed = table.Column<int>(nullable: false),
                    LowerAnimLoopCount = table.Column<int>(nullable: false),
                    LowerAnimSprite = table.Column<string>(nullable: true),
                    LowerAnimXFrames = table.Column<int>(nullable: false),
                    LowerAnimYFrames = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Sound = table.Column<string>(nullable: true),
                    UpperAnimFrameCount = table.Column<int>(nullable: false),
                    UpperAnimFrameSpeed = table.Column<int>(nullable: false),
                    UpperAnimLoopCount = table.Column<int>(nullable: false),
                    UpperAnimSprite = table.Column<string>(nullable: true),
                    UpperAnimXFrames = table.Column<int>(nullable: false),
                    UpperAnimYFrames = table.Column<int>(nullable: false)
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
