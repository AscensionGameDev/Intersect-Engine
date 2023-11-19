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
                name: "Items_EquipmentProperties",
                columns: table => new
                {
                    DescriptorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StatRange_Attack_LowRange = table.Column<int>(type: "INTEGER", nullable: true),
                    StatRange_Attack_HighRange = table.Column<int>(type: "INTEGER", nullable: true),
                    StatRange_AbilityPower_LowRange = table.Column<int>(type: "INTEGER", nullable: true),
                    StatRange_AbilityPower_HighRange = table.Column<int>(type: "INTEGER", nullable: true),
                    StatRange_Defense_LowRange = table.Column<int>(type: "INTEGER", nullable: true),
                    StatRange_Defense_HighRange = table.Column<int>(type: "INTEGER", nullable: true),
                    StatRange_MagicResist_LowRange = table.Column<int>(type: "INTEGER", nullable: true),
                    StatRange_MagicResist_HighRange = table.Column<int>(type: "INTEGER", nullable: true),
                    StatRange_Speed_LowRange = table.Column<int>(type: "INTEGER", nullable: true),
                    StatRange_Speed_HighRange = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items_EquipmentProperties", x => x.DescriptorId);
                    table.ForeignKey(
                        name: "FK_Items_EquipmentProperties_Items_DescriptorId",
                        column: x => x.DescriptorId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // TODO: Copy StatGrowth data, you should be able to use SqlKata to generate the query instead of directly writing raw SQL
            // The upside to this is that you can put the actual SqlKata code in a reusable class and use the same code for both SQLite and MySQL since you just have to tell it which provider it is generating SQL for
            // (you don't need to use the executor, but you can see SqlKata being used in SqliteNetCoreGuidPatch and SqliteUserVariablePopulateNewColumnId)

            migrationBuilder.DropColumn(
                name: "StatGrowth",
                table: "Items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
