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
                    _ = migrationBuilder.Sql("UPDATE Items SET Effects = \"[{\"\"EffectType\"\":\" || Effect_Type || \",\"\"EffectPercentage\"\":\" || Effect_Percentage || \"}]\" WHERE Effect_Percentage <> 0;");
                    break;

                case "Pomelo.EntityFrameworkCore.MySql":
                    _ = migrationBuilder.Sql("UPDATE `Items` SET Effects = CONCAT(\"[{\\\"EffectType\\\":\", Effect_Type, \",\\\"EffectPercentage\\\":\", Effect_Percentage, \"}]\") WHERE Effect_Percentage <> 0;");
                    break;
            }

            migrationBuilder.DropColumn(
                name: "Effect_Percentage",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Effect_Type",
                table: "Items");
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
                    throw new NotSupportedException();

                case "Pomelo.EntityFrameworkCore.MySql":
                    _ = migrationBuilder.Sql("UPDATE `Items` SET Effect_Type = JSON_VALUE(Effects, \"$[0].EffectType\"), Effect_Percentage = JSON_VALUE(Effects, \"$[0].EffectPercentage\") WHERE CHAR_LENGTH(`Effects`) > 2;");
                    break;
            }

            migrationBuilder.DropColumn(
                name: "Effects",
                table: "Items");
        }
    }
}
