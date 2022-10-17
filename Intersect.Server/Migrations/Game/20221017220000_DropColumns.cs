using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class DropOwnedEffectData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.Sqlite") {
                _ = migrationBuilder.Sql(
                    @"
                    CREATE TABLE Items__migration AS SELECT Id, TimeCreated, Name, Animation, AttackAnimation, EquipmentAnimation, Bound, CritChance, CritMultiplier, Cooldown, Damage, DamageType, AttackSpeedModifier, AttackSpeedValue, Consumable_Type, Consumable_Value, EquipmentSlot, TwoHanded, Effects, SlotCount, Spell, Event, Description, FemalePaperdoll, ItemType, MalePaperdoll, Icon, Price, Projectile, Scaling, ScalingStat, Speed, Stackable, StatGrowth, Tool, VitalsGiven, StatsGiven, UsageRequirements, DestroySpell, QuickCast, Rarity, Folder, Consumable_Percentage, PercentageStatsGiven, PercentageVitalsGiven, VitalsRegen, CooldownGroup, IgnoreGlobalCooldown, Color, IgnoreCooldownReduction, CanBag, CanBank, DropChanceOnDeath, CanSell, CanTrade, MaxBankStack, MaxInventoryStack, CanGuildBank, CannotUseMessage, BlockAbsorption, BlockAmount, BlockChance, WeaponSpriteOverride, DespawnTime FROM Items;
                    DROP TABLE Items;
                    CREATE TABLE Items AS SELECT * FROM Items__migration;
                    DROP TABLE Items__migration;
                    "
                );
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.Sqlite") {
                throw new NotImplementedException();
            }
        }
    }
}
