using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddCraftingProfessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(name: "Profession", table: "Crafts", type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>(name: "SkillOrange", table: "Crafts", type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>(name: "SkillYellow", table: "Crafts", type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>(name: "SkillGreen", table: "Crafts", type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>(name: "SkillGray", table: "Crafts", type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<bool>(name: "Trainable", table: "Crafts", type: "INTEGER", nullable: false, defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Profession", table: "Crafts");
            migrationBuilder.DropColumn(name: "SkillOrange", table: "Crafts");
            migrationBuilder.DropColumn(name: "SkillYellow", table: "Crafts");
            migrationBuilder.DropColumn(name: "SkillGreen", table: "Crafts");
            migrationBuilder.DropColumn(name: "SkillGray", table: "Crafts");
            migrationBuilder.DropColumn(name: "Trainable", table: "Crafts");
        }
    }
}
