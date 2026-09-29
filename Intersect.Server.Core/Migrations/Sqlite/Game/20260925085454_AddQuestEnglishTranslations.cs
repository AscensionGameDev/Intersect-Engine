using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddQuestEnglishTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BeforeDescriptionEn",
                table: "Quests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndDescriptionEn",
                table: "Quests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InProgressDescriptionEn",
                table: "Quests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "Quests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartDescriptionEn",
                table: "Quests",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeforeDescriptionEn",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "EndDescriptionEn",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "InProgressDescriptionEn",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "StartDescriptionEn",
                table: "Quests");
        }
    }
}
