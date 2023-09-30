using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{

    public partial class CombiningSwitchesVariables : Migration
    {

        private static void RenameOldTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable("Player_Switches", newName: "Player_Switches_Old");
            if (!migrationBuilder.ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.DropForeignKey("FK_Player_Switches_Players_PlayerId", "Player_Switches_Old");
            }

            migrationBuilder.RenameTable("Player_Variables", newName: "Player_Variables_Old");
            if (!migrationBuilder.ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.DropForeignKey("FK_Player_Variables_Players_PlayerId", "Player_Variables_Old");
            }
        }

        private static void CreateNewTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Player_Variables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    VariableId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Variables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Variables_Players_PlayerId", column: x => x.PlayerId,
                        principalTable: "Players", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );
        }

        private static void TransferDataUp(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO Player_Variables (Id, PlayerId, VariableId, Value) " +
                "SELECT Id, PlayerId, VariableId, Value " +
                "FROM Player_Variables_Old;"
            );

            if (migrationBuilder.ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.Sql("UPDATE Player_Variables Set Value=('{\"Type\":2,\"Value\":' || Value || '}');");
            }
            else
            {
                migrationBuilder.Sql("UPDATE Player_Variables Set Value=CONCAT('{\"Type\":2,\"Value\":', Value, '}');");
            }

            if (migrationBuilder.ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.Sql(
                    "INSERT INTO Player_Variables (Id, PlayerId, VariableId, Value) " +
                    "SELECT Id, PlayerId, SwitchId, Value " +
                    "FROM Player_Switches_Old;"
                );

                migrationBuilder.Sql(
                    "UPDATE Player_Variables Set Value=('{\"Type\":1,\"Value\": false}') WHERE Value = '0';"
                );

                migrationBuilder.Sql(
                    "UPDATE Player_Variables Set Value=('{\"Type\":1,\"Value\": true}') WHERE Value = '1';"
                );
            }
            else
            {
                migrationBuilder.Sql(
                    "INSERT INTO Player_Variables (Id, PlayerId, VariableId) " +
                    "SELECT Id, PlayerId, SwitchId " +
                    "FROM Player_Switches_Old WHERE Value = 0 OR Value = false;"
                );

                migrationBuilder.Sql(
                    "UPDATE Player_Variables Set Value=('{\"Type\":1,\"Value\": false}') WHERE Value = '' OR Value IS NULL;"
                );

                migrationBuilder.Sql(
                    "INSERT INTO Player_Variables (Id, PlayerId, VariableId) " +
                    "SELECT Id, PlayerId, SwitchId " +
                    "FROM Player_Switches_Old WHERE Value = 1 OR Value = true;"
                );

                migrationBuilder.Sql(
                    "UPDATE Player_Variables Set Value=('{\"Type\":1,\"Value\": true}') WHERE Value = '' OR Value IS NULL;"
                );
            }
        }

        private static void DeleteOldTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Player_Switches_Old");

            migrationBuilder.DropTable(name: "Player_Variables_Old");
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
            migrationBuilder.RenameTable("Player_Variables", newName: "Player_Variables_Old");
        }

        private static void CreateOldTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Player_Switches", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    SwitchId = table.Column<Guid>(nullable: false),
                    Value = table.Column<bool>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Switches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Switches_Players_PlayerId", column: x => x.PlayerId, principalTable: "Players",
                        principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Player_Variables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerId = table.Column<Guid>(nullable: false),
                    VariableId = table.Column<Guid>(nullable: false),
                    Value = table.Column<long>(nullable: false)
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Player_Variables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Variables_Players_PlayerId", column: x => x.PlayerId,
                        principalTable: "Players", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                    );
                }
            );
        }

        private static void TransferDataDown(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Player_Variables_Old Set Value=0 WHERE Value LIKE %alse%;");
            migrationBuilder.Sql("UPDATE Player_Variables_Old Set Value=0 WHERE Value LIKE %rue%;");

            migrationBuilder.Sql(
                "SELECT Value REPLACE(Value, '{\"Type\":2,\"Value\":', Value) FROM Player_Variables_Old WHERE Type = 2;"
            );

            migrationBuilder.Sql("SELECT Value REPLACE(Value, '}', Value) FROM Player_Variables_Old WHERE Type = 2;");

            migrationBuilder.Sql(
                "INSERT INTO Player_Variables (Id, PlayerId, VariableId, Value) " +
                "SELECT Id, PlayerId, VariableId, Value " +
                "FROM Player_Variables_Old;"
            );

            migrationBuilder.Sql(
                "INSERT INTO Player_Switches (Id, PlayerId, SwitchId, Value) " +
                "SELECT Id, PlayerId, VariableId, Value " +
                "FROM Player_Switches_Old;"
            );
        }

        private static void DeleteNewTables(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Player_Variables_Old");
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
