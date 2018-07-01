using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Intersect.Server.Migrations.Game
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
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
                    AttackAnimation = table.Column<Guid>(nullable: false),
                    BaseExp = table.Column<long>(nullable: false),
                    BasePoints = table.Column<int>(nullable: false),
                    CritChance = table.Column<int>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    ExpIncrease = table.Column<long>(nullable: false),
                    IncreasePercentage = table.Column<int>(nullable: false),
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
                    SpawnMap = table.Column<Guid>(nullable: false),
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
                    Ingredients = table.Column<string>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Time = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crafts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CommonEvent = table.Column<bool>(nullable: false),
                    IsGlobal = table.Column<byte>(nullable: false),
                    MapId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Pages = table.Column<string>(nullable: true),
                    SpawnX = table.Column<int>(nullable: false),
                    SpawnY = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Animation = table.Column<Guid>(nullable: false),
                    AttackAnimation = table.Column<Guid>(nullable: false),
                    Bound = table.Column<bool>(nullable: false),
                    CritChance = table.Column<int>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    Desc = table.Column<string>(nullable: true),
                    EquipmentSlot = table.Column<int>(nullable: false),
                    Event = table.Column<Guid>(nullable: false),
                    FemalePaperdoll = table.Column<string>(nullable: true),
                    ItemType = table.Column<int>(nullable: false),
                    UsageRequirements = table.Column<string>(nullable: true),
                    MalePaperdoll = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Pic = table.Column<string>(nullable: true),
                    Price = table.Column<int>(nullable: false),
                    Projectile = table.Column<Guid>(nullable: false),
                    Scaling = table.Column<int>(nullable: false),
                    ScalingStat = table.Column<int>(nullable: false),
                    SlotCount = table.Column<int>(nullable: false),
                    Speed = table.Column<int>(nullable: false),
                    Spell = table.Column<Guid>(nullable: false),
                    Stackable = table.Column<bool>(nullable: false),
                    StatGrowth = table.Column<int>(nullable: false),
                    StatsGiven = table.Column<string>(nullable: true),
                    Tool = table.Column<int>(nullable: false),
                    TwoHanded = table.Column<bool>(nullable: false),
                    Consumable_Type = table.Column<byte>(nullable: false),
                    Consumable_Value = table.Column<int>(nullable: false),
                    Effect_Percentage = table.Column<int>(nullable: false),
                    Effect_Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MapFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FoldersBlob = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AHue = table.Column<int>(nullable: false),
                    Attributes = table.Column<byte[]>(nullable: true),
                    BHue = table.Column<int>(nullable: false),
                    Brightness = table.Column<int>(nullable: false),
                    Down = table.Column<Guid>(nullable: false),
                    Events = table.Column<string>(nullable: true),
                    Fog = table.Column<string>(nullable: true),
                    FogTransparency = table.Column<int>(nullable: false),
                    FogXSpeed = table.Column<int>(nullable: false),
                    FogYSpeed = table.Column<int>(nullable: false),
                    GHue = table.Column<int>(nullable: false),
                    IsIndoors = table.Column<bool>(nullable: false),
                    Left = table.Column<Guid>(nullable: false),
                    Lights = table.Column<string>(nullable: true),
                    Music = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NpcSpawns = table.Column<string>(nullable: true),
                    OverlayGraphic = table.Column<string>(nullable: true),
                    Panorama = table.Column<string>(nullable: true),
                    PlayerLightColor = table.Column<string>(nullable: true),
                    PlayerLightExpand = table.Column<float>(nullable: false),
                    PlayerLightIntensity = table.Column<byte>(nullable: false),
                    PlayerLightSize = table.Column<int>(nullable: false),
                    RHue = table.Column<int>(nullable: false),
                    ResourceSpawns = table.Column<string>(nullable: true),
                    Revision = table.Column<int>(nullable: false),
                    Right = table.Column<Guid>(nullable: false),
                    Sound = table.Column<string>(nullable: true),
                    TileData = table.Column<byte[]>(nullable: true),
                    Up = table.Column<Guid>(nullable: false),
                    WeatherAnimation = table.Column<Guid>(nullable: false),
                    WeatherIntensity = table.Column<int>(nullable: false),
                    WeatherXSpeed = table.Column<int>(nullable: false),
                    WeatherYSpeed = table.Column<int>(nullable: false),
                    ZoneType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Npcs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AttackAllies = table.Column<bool>(nullable: false),
                    AttackAnimation = table.Column<Guid>(nullable: false),
                    Behavior = table.Column<byte>(nullable: false),
                    Spells = table.Column<string>(nullable: true),
                    CritChance = table.Column<int>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    Experience = table.Column<long>(nullable: false),
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
                name: "PlayerSwitches",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerSwitches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerVariables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerVariables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projectiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Ammo = table.Column<Guid>(nullable: false),
                    AmmoRequired = table.Column<int>(nullable: false),
                    Animations = table.Column<string>(nullable: true),
                    Delay = table.Column<int>(nullable: false),
                    GrappleHook = table.Column<bool>(nullable: false),
                    Homing = table.Column<bool>(nullable: false),
                    IgnoreActiveResources = table.Column<bool>(nullable: false),
                    IgnoreExhaustedResources = table.Column<bool>(nullable: false),
                    IgnoreMapBlocks = table.Column<bool>(nullable: false),
                    IgnoreZDimension = table.Column<bool>(nullable: false),
                    Knockback = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    Range = table.Column<int>(nullable: false),
                    SpawnLocations = table.Column<string>(nullable: true),
                    Speed = table.Column<int>(nullable: false),
                    Spell = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projectiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BeforeDesc = table.Column<string>(nullable: true),
                    EndDesc = table.Column<string>(nullable: true),
                    EndEvent = table.Column<Guid>(nullable: false),
                    InProgressDesc = table.Column<string>(nullable: true),
                    Requirements = table.Column<string>(nullable: true),
                    LogAfterComplete = table.Column<byte>(nullable: false),
                    LogBeforeOffer = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    NextTaskId = table.Column<int>(nullable: false),
                    Quitable = table.Column<byte>(nullable: false),
                    Repeatable = table.Column<byte>(nullable: false),
                    StartDesc = table.Column<string>(nullable: true),
                    StartEvent = table.Column<Guid>(nullable: false),
                    Tasks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Animation = table.Column<Guid>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "ServerSwitches",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSwitches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerVariables",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerVariables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BuyingWhitelist = table.Column<bool>(nullable: false),
                    DefaultCurrency = table.Column<Guid>(nullable: false),
                    BuyingItems = table.Column<string>(nullable: true),
                    SellingItems = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spells",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CastAnimation = table.Column<Guid>(nullable: false),
                    CastDuration = table.Column<int>(nullable: false),
                    CastRange = table.Column<int>(nullable: false),
                    CooldownDuration = table.Column<int>(nullable: false),
                    Cost = table.Column<int>(nullable: false),
                    CritChance = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    Data1 = table.Column<int>(nullable: false),
                    Data2 = table.Column<int>(nullable: false),
                    Data3 = table.Column<int>(nullable: false),
                    Data4 = table.Column<int>(nullable: false),
                    Data5 = table.Column<string>(nullable: true),
                    Desc = table.Column<string>(nullable: true),
                    Friendly = table.Column<int>(nullable: false),
                    Guid1 = table.Column<Guid>(nullable: false),
                    Guid2 = table.Column<Guid>(nullable: false),
                    Guid3 = table.Column<Guid>(nullable: false),
                    Guid4 = table.Column<Guid>(nullable: false),
                    HitAnimation = table.Column<Guid>(nullable: false),
                    HitRadius = table.Column<int>(nullable: false),
                    CastRequirements = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Pic = table.Column<string>(nullable: true),
                    Projectile = table.Column<Guid>(nullable: false),
                    Scaling = table.Column<int>(nullable: false),
                    ScalingStat = table.Column<int>(nullable: false),
                    SpellType = table.Column<byte>(nullable: false),
                    StatDiff = table.Column<string>(nullable: true),
                    TargetType = table.Column<int>(nullable: false),
                    VitalCost = table.Column<string>(nullable: true),
                    VitalDiff = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spells", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tilesets",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tilesets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Time",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RangeInterval = table.Column<int>(nullable: false),
                    Rate = table.Column<float>(nullable: false),
                    DaylightHues = table.Column<string>(nullable: true),
                    SyncTime = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Time", x => x.Id);
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
                name: "Events");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "MapFolders");

            migrationBuilder.DropTable(
                name: "Maps");

            migrationBuilder.DropTable(
                name: "Npcs");

            migrationBuilder.DropTable(
                name: "PlayerSwitches");

            migrationBuilder.DropTable(
                name: "PlayerVariables");

            migrationBuilder.DropTable(
                name: "Projectiles");

            migrationBuilder.DropTable(
                name: "Quests");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "ServerSwitches");

            migrationBuilder.DropTable(
                name: "ServerVariables");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropTable(
                name: "Spells");

            migrationBuilder.DropTable(
                name: "Tilesets");

            migrationBuilder.DropTable(
                name: "Time");
        }
    }
}
