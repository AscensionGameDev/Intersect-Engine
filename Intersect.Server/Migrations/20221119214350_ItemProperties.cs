using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations
{
    public partial class ItemProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemProperties",
                table: "Player_Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemProperties",
                table: "Player_Bank",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemProperties",
                table: "Guild_Bank",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemProperties",
                table: "Bag_Items",
                nullable: true);

            switch (migrationBuilder.ActiveProvider)
            {
                case "Microsoft.EntityFrameworkCore.Sqlite":
                    _ = migrationBuilder.Sql("UPDATE Bag_Items SET ItemProperties = \"{\"\"StatModifiers\"\":\" || StatBuffs || \"}\" WHERE StatBuffs <> \"[0,0,0,0,0]\";");
                    _ = migrationBuilder.Sql("UPDATE Guild_Bank SET ItemProperties = \"{\"\"StatModifiers\"\":\" || StatBuffs || \"}\" WHERE StatBuffs <> \"[0,0,0,0,0]\";");
                    _ = migrationBuilder.Sql("UPDATE Player_Bank SET ItemProperties = \"{\"\"StatModifiers\"\":\" || StatBuffs || \"}\" WHERE StatBuffs <> \"[0,0,0,0,0]\";");
                    _ = migrationBuilder.Sql("UPDATE Player_Items SET ItemProperties = \"{\"\"StatModifiers\"\":\" || StatBuffs || \"}\" WHERE StatBuffs <> \"[0,0,0,0,0]\";");
                    // Drop columns doesn't work, and even the roundabout way of doing it just isn't working even in SQLite Studio
                    _ = migrationBuilder.Sql(@"
PRAGMA foreign_keys = 0;

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Bag_Items;

DROP TABLE Bag_Items;

CREATE TABLE Bag_Items (
    Id             BLOB    NOT NULL
                           CONSTRAINT PK_Bag_Items PRIMARY KEY,
    ParentBagId    BLOB    NOT NULL,
    Slot           INTEGER NOT NULL,
    BagId          BLOB,
    ItemId         BLOB    NOT NULL,
    Quantity       INTEGER NOT NULL,
    ItemProperties TEXT,
    CONSTRAINT FK_Bag_Items_Bags_BagId FOREIGN KEY (
        BagId
    )
    REFERENCES Bags (Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Bag_Items_Bags_ParentBagId FOREIGN KEY (
        ParentBagId
    )
    REFERENCES Bags (Id) ON DELETE CASCADE
);

INSERT INTO Bag_Items (
                          Id,
                          ParentBagId,
                          Slot,
                          BagId,
                          ItemId,
                          Quantity,
                          ItemProperties
                      )
                      SELECT Id,
                             ParentBagId,
                             Slot,
                             BagId,
                             ItemId,
                             Quantity,
                             ItemProperties
                        FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

CREATE INDEX IX_Bag_Items_BagId ON Bag_Items (
    ""BagId""
);

CREATE INDEX IX_Bag_Items_ParentBagId ON Bag_Items (
    ""ParentBagId""
);

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Guild_Bank;

DROP TABLE Guild_Bank;

CREATE TABLE Guild_Bank (
    Id             BLOB    NOT NULL
                           CONSTRAINT PK_Guild_Bank PRIMARY KEY,
    GuildId        BLOB    NOT NULL,
    Slot           INTEGER NOT NULL,
    BagId          BLOB,
    ItemId         BLOB    NOT NULL,
    Quantity       INTEGER NOT NULL,
    ItemProperties TEXT,
    CONSTRAINT FK_Guild_Bank_Bags_BagId FOREIGN KEY (
        BagId
    )
    REFERENCES Bags (Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Guild_Bank_Guilds_GuildId FOREIGN KEY (
        GuildId
    )
    REFERENCES Guilds (Id) ON DELETE CASCADE
);

INSERT INTO Guild_Bank (
                           Id,
                           GuildId,
                           Slot,
                           BagId,
                           ItemId,
                           Quantity,
                           ItemProperties
                       )
                       SELECT Id,
                              GuildId,
                              Slot,
                              BagId,
                              ItemId,
                              Quantity,
                              ItemProperties
                         FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

CREATE INDEX IX_Guild_Bank_BagId ON Guild_Bank (
    ""BagId""
);

CREATE INDEX IX_Guild_Bank_GuildId ON Guild_Bank (
    ""GuildId""
);

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Player_Bank;

DROP TABLE Player_Bank;

CREATE TABLE Player_Bank (
    Id             BLOB    NOT NULL
                           CONSTRAINT PK_Player_Bank PRIMARY KEY,
    PlayerId       BLOB    NOT NULL,
    Slot           INTEGER NOT NULL,
    BagId          BLOB,
    ItemId         BLOB    NOT NULL,
    Quantity       INTEGER NOT NULL,
    ItemProperties TEXT,
    CONSTRAINT FK_Player_Bank_Bags_BagId FOREIGN KEY (
        BagId
    )
    REFERENCES Bags (Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Player_Bank_Players_PlayerId FOREIGN KEY (
        PlayerId
    )
    REFERENCES Players (Id) ON DELETE CASCADE
);

INSERT INTO Player_Bank (
                            Id,
                            PlayerId,
                            Slot,
                            BagId,
                            ItemId,
                            Quantity,
                            ItemProperties
                        )
                        SELECT Id,
                               PlayerId,
                               Slot,
                               BagId,
                               ItemId,
                               Quantity,
                               ItemProperties
                          FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

CREATE INDEX IX_Player_Bank_BagId ON Player_Bank (
    ""BagId""
);

CREATE INDEX IX_Player_Bank_PlayerId ON Player_Bank (
    ""PlayerId""
);

CREATE TABLE sqlitestudio_temp_table AS SELECT *
                                          FROM Player_Items;

DROP TABLE Player_Items;

CREATE TABLE Player_Items (
    Id             BLOB    NOT NULL
                           CONSTRAINT PK_Player_Items PRIMARY KEY,
    PlayerId       BLOB    NOT NULL,
    Slot           INTEGER NOT NULL,
    BagId          BLOB,
    ItemId         BLOB    NOT NULL,
    Quantity       INTEGER NOT NULL,
    ItemProperties TEXT,
    CONSTRAINT FK_Player_Items_Bags_BagId FOREIGN KEY (
        BagId
    )
    REFERENCES Bags (Id) ON DELETE RESTRICT,
    CONSTRAINT FK_Player_Items_Players_PlayerId FOREIGN KEY (
        PlayerId
    )
    REFERENCES Players (Id) ON DELETE CASCADE
);

INSERT INTO Player_Items (
                             Id,
                             PlayerId,
                             Slot,
                             BagId,
                             ItemId,
                             Quantity,
                             ItemProperties
                         )
                         SELECT Id,
                                PlayerId,
                                Slot,
                                BagId,
                                ItemId,
                                Quantity,
                                ItemProperties
                           FROM sqlitestudio_temp_table;

DROP TABLE sqlitestudio_temp_table;

CREATE INDEX IX_Player_Items_BagId ON Player_Items (
    ""BagId""
);

CREATE INDEX IX_Player_Items_PlayerId ON Player_Items (
    ""PlayerId""
);

PRAGMA foreign_keys = 1;
");
                    break;

                case "Pomelo.EntityFrameworkCore.MySql":
                    _ = migrationBuilder.Sql("UPDATE `Bag_Items` SET ItemProperties = CONCAT(\"{\\\"StatModifiers\\\":\", StatBuffs, \"}\") WHERE StatBuffs <> \"[0,0,0,0,0]\";");
                    _ = migrationBuilder.Sql("UPDATE `Guild_Bank` SET ItemProperties = CONCAT(\"{\\\"StatModifiers\\\":\", StatBuffs, \"}\") WHERE StatBuffs <> \"[0,0,0,0,0]\";");
                    _ = migrationBuilder.Sql("UPDATE `Player_Bank` SET ItemProperties = CONCAT(\"{\\\"StatModifiers\\\":\", StatBuffs, \"}\") WHERE StatBuffs <> \"[0,0,0,0,0]\";");
                    _ = migrationBuilder.Sql("UPDATE `Player_Items` SET ItemProperties = CONCAT(\"{\\\"StatModifiers\\\":\", StatBuffs, \"}\") WHERE StatBuffs <> \"[0,0,0,0,0]\";");

                    migrationBuilder.DropColumn(
                        name: "StatBuffs",
                        table: "Player_Items");

                    migrationBuilder.DropColumn(
                        name: "StatBuffs",
                        table: "Player_Bank");

                    migrationBuilder.DropColumn(
                        name: "StatBuffs",
                        table: "Guild_Bank");

                    migrationBuilder.DropColumn(
                        name: "StatBuffs",
                        table: "Bag_Items");
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
