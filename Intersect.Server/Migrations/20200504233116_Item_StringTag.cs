using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class Item_StringTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StringTags",
                table: "Player_MailBox",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringTags",
                table: "Player_Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringTags",
                table: "Player_Bank",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringTags",
                table: "HDV",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringTags",
                table: "Bag_Items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StringTags",
                table: "Player_MailBox");

            migrationBuilder.DropColumn(
                name: "StringTags",
                table: "Player_Items");

            migrationBuilder.DropColumn(
                name: "StringTags",
                table: "Player_Bank");
			
            migrationBuilder.DropColumn(
                name: "StringTags",
                table: "HDV");

            migrationBuilder.DropColumn(
                name: "StringTags",
                table: "Bag_Items");
        }
    }
}
