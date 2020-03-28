using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{

    public partial class FixingPlayerDeleteFriendsForeignKeyContraintFail : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Cannot drop indexes and recreate below without first removing the foreign keys in mysql....
            if (!migrationBuilder.ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.DropForeignKey(name: "FK_Bans_Users_PlayerId", table: "Bans");

                migrationBuilder.DropForeignKey(name: "FK_Mutes_Users_PlayerId", table: "Mutes");
            }

            migrationBuilder.DropIndex(name: "IX_Mutes_PlayerId", table: "Mutes");

            migrationBuilder.DropIndex(name: "IX_Bans_PlayerId", table: "Bans");

            migrationBuilder.CreateIndex(name: "IX_Mutes_PlayerId", table: "Mutes", column: "PlayerId", unique: true);

            migrationBuilder.CreateIndex(name: "IX_Bans_PlayerId", table: "Bans", column: "PlayerId", unique: true);

            //Now that indexes are recreated we can recreate the foreign keys.. sheesh
            if (!migrationBuilder.ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.AddForeignKey(
                    name: "FK_Bans_Users_PlayerId", table: "Bans", column: "PlayerId", principalTable: "Users",
                    principalColumn: "Id", onDelete: ReferentialAction.Cascade
                );

                migrationBuilder.AddForeignKey(
                    name: "FK_Mutes_Users_PlayerId", table: "Mutes", column: "PlayerId", principalTable: "Users",
                    principalColumn: "Id", onDelete: ReferentialAction.Cascade
                );
            }

            //Sqlite doesn't support dropping foreign keys.. so here is a custom sql migration...
            if (migrationBuilder.ActiveProvider.Contains("Sqlite"))
            {
                migrationBuilder.Sql(
                    "CREATE TABLE `Player_Friends1` (`Id` BLOB NOT NULL, `OwnerId` BLOB, `TargetId` BLOB, PRIMARY KEY(`Id`), FOREIGN KEY(`OwnerId`) REFERENCES `Players`(`Id`) ON DELETE CASCADE, FOREIGN KEY(`TargetId`) REFERENCES `Players`(`Id`) ON DELETE CASCADE);"
                );

                migrationBuilder.Sql(
                    "INSERT INTO Player_Friends1 (Id, OwnerId, TargetId) SELECT Id, OwnerId, TargetId FROM Player_Friends;"
                );

                migrationBuilder.Sql("DROP TABLE Player_Friends;");
                migrationBuilder.Sql("ALTER TABLE Player_Friends1 RENAME TO Player_Friends;");
            }
            else
            {
                migrationBuilder.DropForeignKey(name: "FK_Player_Friends_Players_OwnerId", table: "Player_Friends");

                migrationBuilder.DropForeignKey(name: "FK_Player_Friends_Players_TargetId", table: "Player_Friends");

                migrationBuilder.AddForeignKey(
                    name: "FK_Player_Friends_Players_OwnerId", table: "Player_Friends", column: "OwnerId",
                    principalTable: "Players", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                );

                migrationBuilder.AddForeignKey(
                    name: "FK_Player_Friends_Players_TargetId", table: "Player_Friends", column: "TargetId",
                    principalTable: "Players", principalColumn: "Id", onDelete: ReferentialAction.Cascade
                );
            }
        }

    }

}
