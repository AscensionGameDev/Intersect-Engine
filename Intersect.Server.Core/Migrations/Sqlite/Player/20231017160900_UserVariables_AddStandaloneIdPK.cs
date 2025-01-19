using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Player
{
    /// <inheritdoc />
    public partial class UserVariables_AddStandaloneIdPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 ALTER TABLE User_Variables ADD COLUMN Id TEXT DEFAULT NULL;
                                 UPDATE User_Variables SET Id = randomblob(16);
                                 UPDATE User_Variables SET Id = HEX(Id);
                                 UPDATE User_Variables SET Id = SUBSTR(Id,1,8)||'-'||SUBSTR(Id,9,4)||'-'||SUBSTR(Id,13,4)||'-'||SUBSTR(Id,17,4)||'-'||SUBSTR(Id,21,12);
                                 CREATE TABLE ef_temp_User_Variables (
                                     "Id" TEXT NOT NULL CONSTRAINT "PK_User_Variables" PRIMARY KEY,
                                     "UserId" TEXT NOT NULL,
                                     "Value" TEXT NULL,
                                     "VariableId" TEXT NOT NULL,
                                     CONSTRAINT "FK_User_Variables_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
                                 );
                                 CREATE UNIQUE INDEX IF NOT EXISTS IX_User_Variables_VariableId_UserId ON ef_temp_User_Variables (UserId COLLATE BINARY, VariableId COLLATE BINARY);
                                 INSERT INTO "ef_temp_User_Variables" ("Id", "UserId", "Value", "VariableId") SELECT "Id", "UserId", "Value", "VariableId" FROM "User_Variables";
                                 DROP TABLE User_Variables;
                                 ALTER TABLE "ef_temp_User_Variables" RENAME TO "User_Variables";
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User_Variables",
                table: "User_Variables");

            migrationBuilder.DropIndex(
                name: "IX_User_Variables_VariableId_UserId",
                table: "User_Variables");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "User_Variables");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_Variables",
                table: "User_Variables",
                columns: new[] { "VariableId", "UserId" });
        }
    }
}
