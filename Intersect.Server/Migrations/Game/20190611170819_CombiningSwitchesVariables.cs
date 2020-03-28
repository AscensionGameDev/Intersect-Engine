using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{

    public partial class CombiningSwitchesVariables : Migration
    {

        private static void RenameOldTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("PlayerVariables", newName: "PlayerVariables_Old");
            migrationBuilder.RenameTable("ServerVariables", newName: "ServerVariables_Old");
        }

        private static void CreateNewTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerVariables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true),
                    Type = table.Column<byte>(nullable: false, defaultValue: (byte) 0),
                    TimeCreated = table.Column<long>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_PlayerVariables", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "ServerVariables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true),
                    Type = table.Column<byte>(nullable: false, defaultValue: (byte) 0),
                    Value = table.Column<string>(nullable: true),
                    TimeCreated = table.Column<long>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_ServerVariables", x => x.Id); }
            );
        }

        private static void TransferDataUp(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO PlayerVariables (Id, Name, TextId, TimeCreated) " +
                "SELECT Id, Name, TextId, TimeCreated " +
                "FROM PlayerVariables_Old;"
            );

            migrationBuilder.Sql("UPDATE PlayerVariables Set Type = 2 WHERE Type = 0;");

            migrationBuilder.Sql(
                "INSERT INTO PlayerVariables (Id, Name, TextId, TimeCreated) " +
                "SELECT Id, Name, TextId, TimeCreated " +
                "FROM PlayerSwitches;"
            );

            migrationBuilder.Sql("UPDATE PlayerVariables Set Type = 1 WHERE Type = 0;");

            migrationBuilder.Sql(
                "INSERT INTO ServerVariables (Id, Name, TextId, Value, TimeCreated) " +
                "SELECT Id, Name, TextId, Value, TimeCreated " +
                "FROM ServerVariables_Old;"
            );

            migrationBuilder.Sql("UPDATE ServerVariables Set Type = 2 WHERE Type = 0;");

            if (migrationBuilder.ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.Sql(
                    "UPDATE ServerVariables Set Value=('{\"Type\":2,\"Value\":' || Value || '}') WHERE Type = 2;"
                );
            }
            else
            {
                migrationBuilder.Sql(
                    "UPDATE ServerVariables Set Value=CONCAT('{\"Type\":2,\"Value\":', Value, '}') WHERE Type = 2;"
                );
            }

            migrationBuilder.Sql(
                "INSERT INTO ServerVariables (Id, Name, TextId, Value, TimeCreated) " +
                "SELECT Id, Name, TextId, Value, TimeCreated " +
                "FROM ServerSwitches;"
            );

            migrationBuilder.Sql("UPDATE ServerVariables Set Type = 1 WHERE Type = 0;");

            if (migrationBuilder.ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.Sql(
                    "UPDATE ServerVariables Set Value=('{\"Type\":1,\"Value\": true}') WHERE Type = 1 AND Value = 1;"
                );

                migrationBuilder.Sql(
                    "UPDATE ServerVariables Set Value=('{\"Type\":1,\"Value\": false}') WHERE Type = 1 AND Value = 0;"
                );
            }
            else
            {
                migrationBuilder.Sql(
                    "UPDATE ServerVariables Set Value=CONCAT('{\"Type\":1,\"Value\": true}') WHERE Type = 1 AND Value = 1;"
                );

                migrationBuilder.Sql(
                    "UPDATE ServerVariables Set Value=CONCAT('{\"Type\":1,\"Value\": false}') WHERE Type = 1 AND Value = 0;"
                );
            }
        }

        private static void DeleteOldTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PlayerSwitches");

            migrationBuilder.DropTable(name: "ServerSwitches");

            migrationBuilder.DropTable(name: "PlayerVariables_Old");

            migrationBuilder.DropTable(name: "ServerVariables_Old");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            RenameOldTables(migrationBuilder);

            CreateNewTables(migrationBuilder);

            TransferDataUp(migrationBuilder);

            DeleteOldTables(migrationBuilder);
        }

        private static void RenameNewTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("PlayerVariables", newName: "PlayerVariables_Old");
            migrationBuilder.RenameTable("ServerVariables", newName: "ServerVariables_Old");
        }

        private static void CreateOldTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerSwitches", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true),
                    Value = table.Column<bool>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_ServerSwitches", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "ServerVariables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true),
                    Value = table.Column<long>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_ServerVariables", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "PlayerSwitches", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_PlayerSwitches", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "PlayerVariables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_PlayerVariables", x => x.Id); }
            );
        }

        private static void TransferDataDown(MigrationBuilder migrationBuilder)
        {
            //PlayerVariables_Old -> PlayerVariables and PlayerSwitches
            migrationBuilder.Sql(
                "INSERT INTO PlayerVariables (Id, Name, TextId, TimeCreated) " +
                "SELECT Id, Name, TextId, TimeCreated " +
                "FROM PlayerVariables_Old WHERE Type = 2;"
            );

            migrationBuilder.Sql(
                "INSERT INTO PlayerSwitches (Id, Name, TextId, TimeCreated) " +
                "SELECT Id, Name, TextId, TimeCreated " +
                "FROM PlayerVariables_Old WHERE Type = 1;"
            );

            //ServerVariables_Old -> ServerVariables and ServerSwitches
            migrationBuilder.Sql(
                "SELECT Value REPLACE(Value, '{\"Type\":1,\"Value\":', Value) FROM ServerVariables_Old WHERE Type = 1;"
            );

            migrationBuilder.Sql("SELECT Value REPLACE(Value, '}', Value) FROM ServerVariables_Old WHERE Type = 1;");

            migrationBuilder.Sql(
                "SELECT Value REPLACE(Value, '{\"Type\":2,\"Value\":', Value) FROM ServerVariables_Old WHERE Type = 2;"
            );

            migrationBuilder.Sql("SELECT Value REPLACE(Value, '}', Value) FROM ServerVariables_Old WHERE Type = 2;");

            migrationBuilder.Sql(
                "INSERT INTO ServerVariables (Id, Name, TextId, Value, TimeCreated) " +
                "SELECT Id, Name, TextId, Value, TimeCreated " +
                "FROM ServerVariables_Old WHERE Type = 2;"
            );

            migrationBuilder.Sql(
                "INSERT INTO ServerSwitches (Id, Name, TextId, Value, TimeCreated) " +
                "SELECT Id, Name, TextId, Value, TimeCreated " +
                "FROM ServerVariables_Old WHERE Type = 1;"
            );
        }

        private static void DeleteNewTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PlayerVariables");

            migrationBuilder.DropTable(name: "ServerVariables");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            RenameNewTables(migrationBuilder);

            CreateOldTables(migrationBuilder);

            TransferDataDown(migrationBuilder);

            DeleteNewTables(migrationBuilder);
        }

    }

}
