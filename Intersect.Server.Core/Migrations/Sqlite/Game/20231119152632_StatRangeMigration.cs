using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class StatRangeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Item_EquipmentProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DescriptorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StatRange_Attack_LowRange = table.Column<int>(type: "INTEGER", nullable: false),
                    StatRange_Attack_HighRange = table.Column<int>(type: "INTEGER", nullable: false),
                    StatRange_Defense_LowRange = table.Column<int>(type: "INTEGER", nullable: false),
                    StatRange_Defense_HighRange = table.Column<int>(type: "INTEGER", nullable: false),
                    StatRange_Speed_LowRange = table.Column<int>(type: "INTEGER", nullable: false),
                    StatRange_Speed_HighRange = table.Column<int>(type: "INTEGER", nullable: false),
                    StatRange_AbilityPower_LowRange = table.Column<int>(type: "INTEGER", nullable: false),
                    StatRange_AbilityPower_HighRange = table.Column<int>(type: "INTEGER", nullable: false),
                    StatRange_MagicResist_LowRange = table.Column<int>(type: "INTEGER", nullable: false),
                    StatRange_MagicResist_HighRange = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item_EquipmentProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_EquipmentProperties_Items_DescriptorId",
                        column: x => x.DescriptorId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_EquipmentProperties_DescriptorId",
                table: "Item_EquipmentProperties",
                column: "DescriptorId",
                unique: true);

            migrationBuilder.DropColumn(
                name: "StatGrowth",
                table: "Items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemRange");

            migrationBuilder.DropTable(
                name: "Items_EquipmentProperties");

            migrationBuilder.AddColumn<int>(
                name: "StatGrowth",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
