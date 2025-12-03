using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.MySql.Game
{
    public partial class AddSkills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MaxLevel = table.Column<int>(nullable: false),
                    BaseExperience = table.Column<long>(nullable: false),
                    ExperienceIncrease = table.Column<long>(nullable: false),
                    ExperienceOverrides = table.Column<string>(nullable: true),
                    ExperienceCurve = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Folder = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Skills");
        }
    }
}

