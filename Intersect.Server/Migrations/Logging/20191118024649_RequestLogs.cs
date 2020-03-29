using System;

using Intersect.Utilities;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Logging
{

    public partial class RequestLogs : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestLogs", columns: table => new
                {
                    Id = table.IsNotNull().Column<Guid>(nullable: false),
                    Time = table.IsNotNull().Column<DateTime>(nullable: false),
                    Level = table.IsNotNull().Column<byte>(nullable: false),
                    Method = table.IsNotNull().Column<string>(nullable: true),
                    StatusCode = table.IsNotNull().Column<int>(nullable: false),
                    StatusMessage = table.IsNotNull().Column<string>(nullable: true),
                    Uri = table.IsNotNull().Column<string>(nullable: true),
                    RequestHeaders = table.IsNotNull().Column<string>(nullable: true),
                    ResponseHeaders = table.IsNotNull().Column<string>(nullable: true)
                }, constraints: table => { table.IsNotNull().PrimaryKey("PK_RequestLogs", x => x.Id); }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RequestLogs");
        }

    }

}
