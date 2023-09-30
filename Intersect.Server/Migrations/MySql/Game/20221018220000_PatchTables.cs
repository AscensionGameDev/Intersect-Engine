using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
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
                                          FROM Items;

DROP TABLE Items;

CREATE TABLE Items (
    Id                      BLOB    NOT NULL
                                    CONSTRAINT PK_Items PRIMARY KEY,
    TimeCreated             INTEGER NOT NULL,
    Name                    TEXT,
    Animation               BLOB    NOT NULL,
    AttackAnimation         BLOB    NOT NULL,
    EquipmentAnimation      BLOB    NOT NULL,
    Bound                   INTEGER NOT NULL,
    CritChance              INTEGER NOT NULL,
    CritMultiplier          REAL    NOT NULL,
    Cooldown                INTEGER NOT NULL,
    Damage                  INTEGER NOT NULL,
    DamageType              INTEGER NOT NULL,
    AttackSpeedModifier     INTEGER NOT NULL,
    AttackSpeedValue        INTEGER NOT NULL,
    Consumable_Type         INTEGER NOT NULL,
    Consumable_Value        INTEGER NOT NULL,
    EquipmentSlot           INTEGER NOT NULL,
    TwoHanded               INTEGER NOT NULL,
    Effects                 TEXT,
    SlotCount               INTEGER NOT NULL,
    Spell                   BLOB    NOT NULL,
    Event                   BLOB    NOT NULL,
    Description             TEXT,
    FemalePaperdoll         TEXT,
    ItemType                INTEGER NOT NULL,
    MalePaperdoll           TEXT,
    Icon                    TEXT,
    Price                   INTEGER NOT NULL,
    Projectile              BLOB    NOT NULL,
    Scaling                 INTEGER NOT NULL,
    ScalingStat             INTEGER NOT NULL,
    Speed                   INTEGER NOT NULL,
    Stackable               INTEGER NOT NULL,
    StatGrowth              INTEGER NOT NULL,
    Tool                    INTEGER NOT NULL,
    VitalsGiven             TEXT,
    StatsGiven              TEXT,
    UsageRequirements       TEXT,
    DestroySpell            INTEGER NOT NULL
                                    DEFAULT 0,
    QuickCast               INTEGER NOT NULL
                                    DEFAULT 0,
    Rarity                  INTEGER NOT NULL
                                    DEFAULT 0,
    Folder                  TEXT,
    Consumable_Percentage   INTEGER NOT NULL
                                    DEFAULT 0,
    PercentageStatsGiven    TEXT,
    PercentageVitalsGiven   TEXT,
    VitalsRegen             TEXT,
    CooldownGroup           TEXT    NOT NULL
                                    DEFAULT '',
    IgnoreGlobalCooldown    INTEGER NOT NULL
                                    DEFAULT 0,
    Color                   TEXT,
    IgnoreCooldownReduction INTEGER NOT NULL
                                    DEFAULT 0,
    CanBag                  INTEGER NOT NULL
                                    DEFAULT 1,
    CanBank                 INTEGER NOT NULL
                                    DEFAULT 1,
    DropChanceOnDeath       INTEGER NOT NULL
                                    DEFAULT 0,
    CanSell                 INTEGER NOT NULL
                                    DEFAULT 1,
    CanTrade                INTEGER NOT NULL
                                    DEFAULT 1,
    MaxBankStack            INTEGER NOT NULL
                                    DEFAULT 2147483647,
    MaxInventoryStack       INTEGER NOT NULL
                                    DEFAULT 2147483647,
    CanGuildBank            INTEGER NOT NULL
                                    DEFAULT 0,
    CannotUseMessage        TEXT,
    BlockAbsorption         INTEGER NOT NULL
                                    DEFAULT 0,
    BlockAmount             INTEGER NOT NULL
                                    DEFAULT 0,
    BlockChance             INTEGER NOT NULL
                                    DEFAULT 0,
    WeaponSpriteOverride    TEXT,
    DespawnTime             INTEGER NOT NULL
                                    DEFAULT 0
);

INSERT INTO Items (
                      Id,
                      TimeCreated,
                      Name,
                      Animation,
                      AttackAnimation,
                      EquipmentAnimation,
                      Bound,
                      CritChance,
                      CritMultiplier,
                      Cooldown,
                      Damage,
                      DamageType,
                      AttackSpeedModifier,
                      AttackSpeedValue,
                      Consumable_Type,
                      Consumable_Value,
                      EquipmentSlot,
                      TwoHanded,
                      Effects,
                      SlotCount,
                      Spell,
                      Event,
                      Description,
                      FemalePaperdoll,
                      ItemType,
                      MalePaperdoll,
                      Icon,
                      Price,
                      Projectile,
                      Scaling,
                      ScalingStat,
                      Speed,
                      Stackable,
                      StatGrowth,
                      Tool,
                      VitalsGiven,
                      StatsGiven,
                      UsageRequirements,
                      DestroySpell,
                      QuickCast,
                      Rarity,
                      Folder,
                      Consumable_Percentage,
                      PercentageStatsGiven,
                      PercentageVitalsGiven,
                      VitalsRegen,
                      CooldownGroup,
                      IgnoreGlobalCooldown,
                      Color,
                      IgnoreCooldownReduction,
                      CanBag,
                      CanBank,
                      DropChanceOnDeath,
                      CanSell,
                      CanTrade,
                      MaxBankStack,
                      MaxInventoryStack,
                      CanGuildBank,
                      CannotUseMessage,
                      BlockAbsorption,
                      BlockAmount,
                      BlockChance,
                      WeaponSpriteOverride,
                      DespawnTime
                  )
                  SELECT Id,
                         TimeCreated,
                         Name,
                         Animation,
                         AttackAnimation,
                         EquipmentAnimation,
                         Bound,
                         CritChance,
                         CritMultiplier,
                         Cooldown,
                         Damage,
                         DamageType,
                         AttackSpeedModifier,
                         AttackSpeedValue,
                         Consumable_Type,
                         Consumable_Value,
                         EquipmentSlot,
                         TwoHanded,
                         Effects,
                         SlotCount,
                         Spell,
                         Event,
                         Description,
                         FemalePaperdoll,
                         ItemType,
                         MalePaperdoll,
                         Icon,
                         Price,
                         Projectile,
                         Scaling,
                         ScalingStat,
                         Speed,
                         Stackable,
                         StatGrowth,
                         Tool,
                         VitalsGiven,
                         StatsGiven,
                         UsageRequirements,
                         DestroySpell,
                         QuickCast,
                         Rarity,
                         Folder,
                         Consumable_Percentage,
                         PercentageStatsGiven,
                         PercentageVitalsGiven,
                         VitalsRegen,
                         CooldownGroup,
                         IgnoreGlobalCooldown,
                         Color,
                         IgnoreCooldownReduction,
                         CanBag,
                         CanBank,
                         DropChanceOnDeath,
                         CanSell,
                         CanTrade,
                         MaxBankStack,
                         MaxInventoryStack,
                         CanGuildBank,
                         CannotUseMessage,
                         BlockAbsorption,
                         BlockAmount,
                         BlockChance,
                         WeaponSpriteOverride,
                         DespawnTime
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
