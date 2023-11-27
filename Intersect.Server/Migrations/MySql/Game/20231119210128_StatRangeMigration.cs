using System;
using Intersect.Server.Database;
using Intersect.Server.Migrations.SqlOnlyDataMigrations;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.MySql.Game
{
    /// <inheritdoc />
    public partial class StatRangeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var statRangeMigrationSqlQueries = new StatRangeMigrationSqlGenerator().Generate(migrationBuilder.GetDatabaseType());

            migrationBuilder.CreateTable(
                name: "Items_EquipmentProperties",
                columns: table => new
                {
                    DescriptorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StatRange_Attack_LowRange = table.Column<int>(type: "int", nullable: true),
                    StatRange_Attack_HighRange = table.Column<int>(type: "int", nullable: true),
                    StatRange_AbilityPower_LowRange = table.Column<int>(type: "int", nullable: true),
                    StatRange_AbilityPower_HighRange = table.Column<int>(type: "int", nullable: true),
                    StatRange_Defense_LowRange = table.Column<int>(type: "int", nullable: true),
                    StatRange_Defense_HighRange = table.Column<int>(type: "int", nullable: true),
                    StatRange_MagicResist_LowRange = table.Column<int>(type: "int", nullable: true),
                    StatRange_MagicResist_HighRange = table.Column<int>(type: "int", nullable: true),
                    StatRange_Speed_LowRange = table.Column<int>(type: "int", nullable: true),
                    StatRange_Speed_HighRange = table.Column<int>(type: "int", nullable: true)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            foreach (var query in statRangeMigrationSqlQueries)
            {
                migrationBuilder.Sql(query);
            }

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
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
