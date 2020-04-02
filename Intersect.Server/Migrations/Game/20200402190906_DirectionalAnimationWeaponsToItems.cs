using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class DirectionalAnimationWeaponsToItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AttackAnimationDown",
                table: "Items",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AttackAnimationLeft",
                table: "Items",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AttackAnimationRight",
                table: "Items",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AttackAnimationUp",
                table: "Items",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "DirectionalAnimation",
                table: "Items",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttackAnimationDown",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AttackAnimationLeft",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AttackAnimationRight",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AttackAnimationUp",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DirectionalAnimation",
                table: "Items");
        }
    }
}
