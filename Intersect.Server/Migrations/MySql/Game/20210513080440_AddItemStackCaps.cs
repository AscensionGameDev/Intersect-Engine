using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.MySql.Game
{
    public partial class AddItemStackCaps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxBankStack",
                table: "Items",
                nullable: false,
                defaultValue: 2147483647);

            migrationBuilder.AddColumn<int>(
                name: "MaxInventoryStack",
                table: "Items",
                nullable: false,
                defaultValue: 2147483647);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxBankStack",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "MaxInventoryStack",
                table: "Items");
        }
    }
}
