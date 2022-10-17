using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class DropColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            switch (migrationBuilder.ActiveProvider)
            {
                case "Microsoft.EntityFrameworkCore.Sqlite":
                    _ = migrationBuilder.Sql(
                        @"
                        CREATE TABLE Player_Spells__migration AS SELECT Id, PlayerId, Slot, SpellId FROM Player_Spells;
                        DROP TABLE Player_Spells;
                        CREATE TABLE Player_Spells AS SELECT * FROM Player_Spells__migration;
                        DROP TABLE Player_Spells__migration;
                        "
                    );
                    _ = migrationBuilder.Sql(
                        @"
                        CREATE TABLE Players__migration AS SELECT Id, UserId, Name, LastOnline, MapId, X, Y, Z, Dir, Gender, Sprite, Face, Level, Exp, ClassId, Vitals, StatPoints, BaseStats, StatPointAllocations, Equipment, NameColor, ItemCooldowns, SpellCooldowns, FooterLabel, HeaderLabel, Color, CreationDate, DbGuildId, GuildJoinDate, GuildRank, PlayTimeSeconds, InstanceType, LastOverworldMapId, LastOverworldX, LastOverworldY, PersonalMapInstanceId, SharedInstanceRespawnDir, SharedInstanceRespawnId, SharedInstanceRespawnX, SharedInstanceRespawnY, SharedMapInstanceId FROM Players;
                        DROP TABLE Players;
                        CREATE TABLE Players AS SELECT * FROM Players__migration;
                        DROP TABLE Players__migration;
                        "
                    );
                    _ = migrationBuilder.Sql(
                        @"
                        CREATE TABLE Users__migration AS SELECT Id, Name, Salt, Password, Email, Power, PasswordResetCode, PasswordResetTime, LastIp, RegistrationDate, PlayTimeSeconds FROM Users;
                        DROP TABLE Users;
                        CREATE TABLE Users AS SELECT * FROM Users__migration;
                        DROP TABLE Users__migration;
                        "
                    );
                    break;

                case "Pomelo.EntityFrameworkCore.MySql":
                    migrationBuilder.DropColumn(
                        name: "SpellCd",
                        table: "Player_Spells");

                    migrationBuilder.DropColumn(
                        name: "PlayTime",
                        table: "Players");

                    migrationBuilder.DropColumn(
                        name: "PlayTime",
                        table: "Users");
                    break;

                default:
                    throw new NotSupportedException(migrationBuilder.ActiveProvider);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
