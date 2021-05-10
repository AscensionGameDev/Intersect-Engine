using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class AddingQuestCategoriesAndOrderValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletedCategory",
                table: "Quests",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DoNotShowUnlessRequirementsMet",
                table: "Quests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InProgressCategory",
                table: "Quests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderValue",
                table: "Quests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UnstartedCategory",
                table: "Quests",
                nullable: true);
        }
    }
}
