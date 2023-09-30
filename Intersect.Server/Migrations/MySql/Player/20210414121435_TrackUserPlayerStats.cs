using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class TrackUserPlayerStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastIp",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PlayTime",
                table: "Users",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Players",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PlayTime",
                table: "Players",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastIp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PlayTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayTime",
                table: "Players");
        }
    }
}
