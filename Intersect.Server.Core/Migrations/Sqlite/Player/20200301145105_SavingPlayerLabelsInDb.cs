﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Sqlite.Player
{

    public partial class SavingPlayerLabelsInDb : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "FooterLabel", table: "Players", nullable: true);

            migrationBuilder.AddColumn<string>(name: "HeaderLabel", table: "Players", nullable: true);
        }

    }

}
