using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class TrackPlaytimeAsULongsInSeconds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Sqlite does not support dropping columns right now... will handle this once we update to EF Core 5 where it does
            //migrationBuilder.DropColumn(
            //    name: "PlayTime",
            //    table: "Users");

            //migrationBuilder.DropColumn(
            //    name: "PlayTime",
            //    table: "Players");

            migrationBuilder.AddColumn<ulong>(
                name: "PlayTimeSeconds",
                table: "Users",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "PlayTimeSeconds",
                table: "Players",
                nullable: false,
                defaultValue: 0ul);
        }
    }
}
