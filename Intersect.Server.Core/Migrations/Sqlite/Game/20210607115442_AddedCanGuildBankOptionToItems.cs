using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Sqlite.Game
{
    public partial class AddedCanGuildBankOptionToItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanGuildBank",
                table: "Items",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanGuildBank",
                table: "Items");
        }
    }
}
