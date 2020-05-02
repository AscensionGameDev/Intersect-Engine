using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class PlayerItemTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Player_MailBox",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Player_Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Player_Bank",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Bag_Items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Player_MailBox");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Player_Items");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Player_Bank");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Bag_Items");
        }
    }
}
