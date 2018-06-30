using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Intersect.Server.Migrations.Game
{
    public partial class Classes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Lower_Lights = table.Column<string>(nullable: true),
                    Upper_Lights = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Sound = table.Column<string>(nullable: true),
                    Lower_DisableRotations = table.Column<bool>(nullable: false),
                    Lower_FrameCount = table.Column<int>(nullable: false),
                    Lower_FrameSpeed = table.Column<int>(nullable: false),
                    Lower_LoopCount = table.Column<int>(nullable: false),
                    Lower_Sprite = table.Column<string>(nullable: true),
                    Lower_XFrames = table.Column<int>(nullable: false),
                    Lower_YFrames = table.Column<int>(nullable: false),
                    Upper_DisableRotations = table.Column<bool>(nullable: false),
                    Upper_FrameCount = table.Column<int>(nullable: false),
                    Upper_FrameSpeed = table.Column<int>(nullable: false),
                    Upper_LoopCount = table.Column<int>(nullable: false),
                    Upper_Sprite = table.Column<string>(nullable: true),
                    Upper_XFrames = table.Column<int>(nullable: false),
                    Upper_YFrames = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AttackAnimation = table.Column<int>(nullable: false),
                    BaseExp = table.Column<long>(nullable: false),
                    BasePoints = table.Column<int>(nullable: false),
                    CritChance = table.Column<int>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    ExpIncrease = table.Column<long>(nullable: false),
                    IncreasePercentage = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    BaseStats = table.Column<string>(nullable: true),
                    BaseVitals = table.Column<string>(nullable: true),
                    Items = table.Column<string>(nullable: true),
                    Spells = table.Column<string>(nullable: true),
                    Sprites = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PointIncrease = table.Column<int>(nullable: false),
                    VitalRegen = table.Column<string>(nullable: true),
                    Scaling = table.Column<int>(nullable: false),
                    ScalingStat = table.Column<int>(nullable: false),
                    SpawnDir = table.Column<int>(nullable: false),
                    SpawnMap = table.Column<int>(nullable: false),
                    SpawnX = table.Column<int>(nullable: false),
                    SpawnY = table.Column<int>(nullable: false),
                    StatIncreases = table.Column<string>(nullable: true),
                    VitalIncreases = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CraftingTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Crafts = table.Column<string>(nullable: true),
                    Index = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraftingTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Crafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Ingredients = table.Column<string>(nullable: true),
                    Item = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Time = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crafts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Animation = table.Column<int>(nullable: false),
                    AttackAnimation = table.Column<int>(nullable: false),
                    Bound = table.Column<bool>(nullable: false),
                    CritChance = table.Column<int>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    Data1 = table.Column<int>(nullable: false),
                    Data2 = table.Column<int>(nullable: false),
                    Data3 = table.Column<int>(nullable: false),
                    Data4 = table.Column<int>(nullable: false),
                    Desc = table.Column<string>(nullable: true),
                    FemalePaperdoll = table.Column<string>(nullable: true),
                    Index = table.Column<int>(nullable: false),
                    ItemType = table.Column<int>(nullable: false),
                    UsageRequirements = table.Column<string>(nullable: true),
                    MalePaperdoll = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Pic = table.Column<string>(nullable: true),
                    Price = table.Column<int>(nullable: false),
                    Projectile = table.Column<int>(nullable: false),
                    Scaling = table.Column<int>(nullable: false),
                    ScalingStat = table.Column<int>(nullable: false),
                    Speed = table.Column<int>(nullable: false),
                    Stackable = table.Column<bool>(nullable: false),
                    StatGrowth = table.Column<int>(nullable: false),
                    StatsGiven = table.Column<string>(nullable: true),
                    Tool = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Npcs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AttackAllies = table.Column<bool>(nullable: false),
                    AttackAnimation = table.Column<int>(nullable: false),
                    Behavior = table.Column<byte>(nullable: false),
                    Spells = table.Column<string>(nullable: true),
                    CritChance = table.Column<int>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    Experience = table.Column<long>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    AggroList = table.Column<string>(nullable: true),
                    Drops = table.Column<string>(nullable: true),
                    MaxVital = table.Column<string>(nullable: true),
                    Stats = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NpcVsNpcEnabled = table.Column<bool>(nullable: false),
                    Scaling = table.Column<int>(nullable: false),
                    ScalingStat = table.Column<int>(nullable: false),
                    SightRange = table.Column<int>(nullable: false),
                    SpawnDuration = table.Column<int>(nullable: false),
                    SpellFrequency = table.Column<int>(nullable: false),
                    Sprite = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Npcs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projectiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Ammo = table.Column<int>(nullable: false),
                    AmmoRequired = table.Column<int>(nullable: false),
                    Animations = table.Column<string>(nullable: true),
                    Delay = table.Column<int>(nullable: false),
                    GrappleHook = table.Column<bool>(nullable: false),
                    Homing = table.Column<bool>(nullable: false),
                    IgnoreActiveResources = table.Column<bool>(nullable: false),
                    IgnoreExhaustedResources = table.Column<bool>(nullable: false),
                    IgnoreMapBlocks = table.Column<bool>(nullable: false),
                    IgnoreZDimension = table.Column<bool>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Knockback = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Range = table.Column<int>(nullable: false),
                    SpawnLocations = table.Column<string>(nullable: true),
                    Speed = table.Column<int>(nullable: false),
                    Spell = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projectiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Animation = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Drops = table.Column<string>(nullable: true),
                    HarvestingRequirements = table.Column<string>(nullable: true),
                    MaxHp = table.Column<int>(nullable: false),
                    MinHp = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SpawnDuration = table.Column<int>(nullable: false),
                    Tool = table.Column<int>(nullable: false),
                    WalkableAfter = table.Column<bool>(nullable: false),
                    WalkableBefore = table.Column<bool>(nullable: false),
                    Exhausted_Graphic = table.Column<string>(nullable: true),
                    Exhausted_GraphicFromTileset = table.Column<bool>(nullable: false),
                    Exhausted_Height = table.Column<int>(nullable: false),
                    Exhausted_Width = table.Column<int>(nullable: false),
                    Exhausted_X = table.Column<int>(nullable: false),
                    Exhausted_Y = table.Column<int>(nullable: false),
                    Initial_Graphic = table.Column<string>(nullable: true),
                    Initial_GraphicFromTileset = table.Column<bool>(nullable: false),
                    Initial_Height = table.Column<int>(nullable: false),
                    Initial_Width = table.Column<int>(nullable: false),
                    Initial_X = table.Column<int>(nullable: false),
                    Initial_Y = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animations");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "CraftingTables");

            migrationBuilder.DropTable(
                name: "Crafts");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Npcs");

            migrationBuilder.DropTable(
                name: "Projectiles");

            migrationBuilder.DropTable(
                name: "Resources");
        }
    }
}
