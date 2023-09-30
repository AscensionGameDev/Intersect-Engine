using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class PatchTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            switch (migrationBuilder.ActiveProvider)
            {
                case "Microsoft.EntityFrameworkCore.Sqlite":
                    _ = migrationBuilder.Sql(
                        @"
PRAGMA foreign_keys = 0;

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Player_Spells;

DROP TABLE Player_Spells;

CREATE TABLE Player_Spells (
    Id       BLOB    NOT NULL
                     CONSTRAINT PK_Player_Spells PRIMARY KEY,
    PlayerId BLOB    NOT NULL,
    Slot     INTEGER NOT NULL,
    SpellId  BLOB    NOT NULL,
    CONSTRAINT FK_Player_Spells_Players_PlayerId FOREIGN KEY (
        PlayerId
    )
    REFERENCES Players (Id) ON DELETE CASCADE
);

INSERT INTO Player_Spells (
                              Id,
                              PlayerId,
                              Slot,
                              SpellId
                          )
                          SELECT Id,
                                 PlayerId,
                                 Slot,
                                 SpellId
                            FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

CREATE INDEX IX_Player_Spells_PlayerId ON Player_Spells (
    ""PlayerId""
);

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Players;

DROP TABLE Players;

CREATE TABLE Players (Id BLOB NOT NULL CONSTRAINT PK_Players PRIMARY KEY, UserId BLOB NOT NULL, Name TEXT, LastOnline TEXT, MapId BLOB NOT NULL, X INTEGER NOT NULL, Y INTEGER NOT NULL, Z INTEGER NOT NULL, Dir INTEGER NOT NULL, Gender INTEGER NOT NULL, Sprite TEXT, Face TEXT, Level INTEGER NOT NULL, Exp INTEGER NOT NULL, ClassId BLOB NOT NULL, Vitals TEXT, StatPoints INTEGER NOT NULL, BaseStats TEXT, StatPointAllocations TEXT, Equipment TEXT, NameColor TEXT, ItemCooldowns TEXT, SpellCooldowns TEXT, FooterLabel TEXT, HeaderLabel TEXT, Color TEXT, CreationDate TEXT, DbGuildId BLOB, GuildJoinDate TEXT NOT NULL DEFAULT '0001-01-01 00:00:00', GuildRank INTEGER NOT NULL DEFAULT 0, PlayTimeSeconds INTEGER NOT NULL DEFAULT 0, InstanceType INTEGER NOT NULL DEFAULT 0, LastOverworldMapId BLOB NOT NULL DEFAULT X'00000000000000000000000000000000', LastOverworldX INTEGER NOT NULL DEFAULT 0, LastOverworldY INTEGER NOT NULL DEFAULT 0, PersonalMapInstanceId BLOB NOT NULL DEFAULT X'00000000000000000000000000000000', SharedInstanceRespawnDir INTEGER NOT NULL DEFAULT 0, SharedInstanceRespawnId BLOB NOT NULL DEFAULT X'00000000000000000000000000000000', SharedInstanceRespawnX INTEGER NOT NULL DEFAULT 0, SharedInstanceRespawnY INTEGER NOT NULL DEFAULT 0, SharedMapInstanceId BLOB NOT NULL DEFAULT X'00000000000000000000000000000000', CONSTRAINT FK_Players_Users_UserId FOREIGN KEY (UserId) REFERENCES Users (Id) ON DELETE RESTRICT);
INSERT INTO Players (
                        Id,
                        UserId,
                        Name,
                        LastOnline,
                        MapId,
                        X,
                        Y,
                        Z,
                        Dir,
                        Gender,
                        Sprite,
                        Face,
                        Level,
                        Exp,
                        ClassId,
                        Vitals,
                        StatPoints,
                        BaseStats,
                        StatPointAllocations,
                        Equipment,
                        NameColor,
                        ItemCooldowns,
                        SpellCooldowns,
                        FooterLabel,
                        HeaderLabel,
                        Color,
                        CreationDate,
                        DbGuildId,
                        GuildJoinDate,
                        GuildRank,
                        PlayTimeSeconds,
                        InstanceType,
                        LastOverworldMapId,
                        LastOverworldX,
                        LastOverworldY,
                        PersonalMapInstanceId,
                        SharedInstanceRespawnDir,
                        SharedInstanceRespawnId,
                        SharedInstanceRespawnX,
                        SharedInstanceRespawnY,
                        SharedMapInstanceId
                    )
                    SELECT Id,
                           UserId,
                           Name,
                           LastOnline,
                           MapId,
                           X,
                           Y,
                           Z,
                           Dir,
                           Gender,
                           Sprite,
                           Face,
                           Level,
                           Exp,
                           ClassId,
                           Vitals,
                           StatPoints,
                           BaseStats,
                           StatPointAllocations,
                           Equipment,
                           NameColor,
                           ItemCooldowns,
                           SpellCooldowns,
                           FooterLabel,
                           HeaderLabel,
                           Color,
                           CreationDate,
                           DbGuildId,
                           GuildJoinDate,
                           GuildRank,
                           PlayTimeSeconds,
                           InstanceType,
                           LastOverworldMapId,
                           LastOverworldX,
                           LastOverworldY,
                           PersonalMapInstanceId,
                           SharedInstanceRespawnDir,
                           SharedInstanceRespawnId,
                           SharedInstanceRespawnX,
                           SharedInstanceRespawnY,
                           SharedMapInstanceId
                      FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

CREATE INDEX IX_Players_DbGuildId ON Players (
    ""DbGuildId""
);

CREATE INDEX IX_Players_UserId ON Players (
    ""UserId""
);

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Users;

DROP TABLE Users;

CREATE TABLE Users (
    Id                BLOB    NOT NULL
                              CONSTRAINT PK_Users PRIMARY KEY,
    Name              TEXT,
    Salt              TEXT,
    Password          TEXT,
    Email             TEXT,
    Power             TEXT,
    PasswordResetCode TEXT,
    PasswordResetTime TEXT,
    LastIp            TEXT,
    RegistrationDate  TEXT,
    PlayTimeSeconds   INTEGER NOT NULL
                              DEFAULT 0
);

INSERT INTO Users (
                      Id,
                      Name,
                      Salt,
                      Password,
                      Email,
                      Power,
                      PasswordResetCode,
                      PasswordResetTime,
                      LastIp,
                      RegistrationDate,
                      PlayTimeSeconds
                  )
                  SELECT Id,
                         Name,
                         Salt,
                         Password,
                         Email,
                         Power,
                         PasswordResetCode,
                         PasswordResetTime,
                         LastIp,
                         RegistrationDate,
                         PlayTimeSeconds
                    FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

PRAGMA foreign_keys = 1;
                        ",
                        suppressTransaction: true
                    );
                    break;

                case "Pomelo.EntityFrameworkCore.MySql":
                    // Do nothing, wasn't broken by DropColumns
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
