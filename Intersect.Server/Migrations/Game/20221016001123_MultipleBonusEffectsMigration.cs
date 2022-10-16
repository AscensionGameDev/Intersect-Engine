using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class MultipleBonusEffectsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Effects",
                table: "Items",
                nullable: true);

            switch (migrationBuilder.ActiveProvider)
            {
                case "Microsoft.EntityFrameworkCore.Sqlite":
                    _ = migrationBuilder.Sql("UPDATE Items SET Effects = \"[{\"\"Type\"\":\" || Effect_Type || \",\"\"Percentage\"\":\" || Effect_Percentage || \"}]\" WHERE Effect_Percentage <> 0;");
                    // Drop columns doesn't work, and even the roundabout way of doing it just isn't working even in SQLite Studio
                    break;

                case "Pomelo.EntityFrameworkCore.MySql":
                    _ = migrationBuilder.Sql("UPDATE `Items` SET Effects = CONCAT(\"[{\\\"Type\\\":\", Effect_Type, \",\\\"Percentage\\\":\", Effect_Percentage, \"}]\") WHERE Effect_Percentage <> 0;");

                    migrationBuilder.DropColumn(
                        name: "Effect_Percentage",
                        table: "Items");

                    migrationBuilder.DropColumn(
                        name: "Effect_Type",
                        table: "Items");
                    break;

                default:
                    throw new NotSupportedException(migrationBuilder.ActiveProvider);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Effect_Percentage",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "Effect_Type",
                table: "Items",
                nullable: false,
                defaultValue: (byte)0);

            switch (migrationBuilder.ActiveProvider)
            {
                case "Microsoft.EntityFrameworkCore.Sqlite":
                    throw new NotImplementedException(migrationBuilder.ActiveProvider);

                case "Pomelo.EntityFrameworkCore.MySql":
                    _ = migrationBuilder.Sql("UPDATE `Items` SET Effect_Type = JSON_VALUE(Effects, \"$[0].Type\"), Effect_Percentage = JSON_VALUE(Effects, \"$[0].Percentage\") WHERE CHAR_LENGTH(`Effects`) > 2;");
                    break;

                default:
                    throw new NotSupportedException(migrationBuilder.ActiveProvider);
            }

            migrationBuilder.DropColumn(
                name: "Effects",
                table: "Items");
        }
    }
}
