using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class CritAndItemChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Combat_CritMultiplier",
                table: "Spells",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CritMultiplier",
                table: "Npcs",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "AttackSpeedModifier",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AttackSpeedValue",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Cooldown",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "CritMultiplier",
                table: "Items",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "EquipmentAnimation",
                table: "Items",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "VitalsGiven",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CritMultiplier",
                table: "Classes",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Combat_CritMultiplier",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "CritMultiplier",
                table: "Npcs");

            migrationBuilder.DropColumn(
                name: "AttackSpeedModifier",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AttackSpeedValue",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Cooldown",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CritMultiplier",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "EquipmentAnimation",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "VitalsGiven",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CritMultiplier",
                table: "Classes");
        }
    }
}
