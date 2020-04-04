using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{

    public partial class Initial : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animations", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Lower_Sprite = table.Column<string>(nullable: true),
                    Lower_FrameCount = table.Column<int>(nullable: false),
                    Lower_XFrames = table.Column<int>(nullable: false),
                    Lower_YFrames = table.Column<int>(nullable: false),
                    Lower_FrameSpeed = table.Column<int>(nullable: false),
                    Lower_LoopCount = table.Column<int>(nullable: false),
                    Lower_DisableRotations = table.Column<bool>(nullable: false),
                    Lower_AlternateRenderLayer = table.Column<bool>(nullable: false),
                    Lower_Light = table.Column<string>(nullable: true),
                    Upper_Sprite = table.Column<string>(nullable: true),
                    Upper_FrameCount = table.Column<int>(nullable: false),
                    Upper_XFrames = table.Column<int>(nullable: false),
                    Upper_YFrames = table.Column<int>(nullable: false),
                    Upper_FrameSpeed = table.Column<int>(nullable: false),
                    Upper_LoopCount = table.Column<int>(nullable: false),
                    Upper_DisableRotations = table.Column<bool>(nullable: false),
                    Upper_AlternateRenderLayer = table.Column<bool>(nullable: false),
                    Upper_Light = table.Column<string>(nullable: true),
                    Sound = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_Animations", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Classes", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    AttackAnimation = table.Column<Guid>(nullable: false),
                    BasePoints = table.Column<int>(nullable: false),
                    CritChance = table.Column<int>(nullable: false),
                    CritMultiplier = table.Column<double>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    BaseExp = table.Column<long>(nullable: false),
                    ExpIncrease = table.Column<long>(nullable: false),
                    IncreasePercentage = table.Column<int>(nullable: false),
                    Locked = table.Column<bool>(nullable: false),
                    PointIncrease = table.Column<int>(nullable: false),
                    Scaling = table.Column<int>(nullable: false),
                    ScalingStat = table.Column<int>(nullable: false),
                    SpawnMap = table.Column<Guid>(nullable: false),
                    SpawnX = table.Column<int>(nullable: false),
                    SpawnY = table.Column<int>(nullable: false),
                    SpawnDir = table.Column<int>(nullable: false),
                    BaseStats = table.Column<string>(nullable: true),
                    BaseVitals = table.Column<string>(nullable: true),
                    Items = table.Column<string>(nullable: true),
                    Spells = table.Column<string>(nullable: true),
                    Sprites = table.Column<string>(nullable: true),
                    StatIncreases = table.Column<string>(nullable: true),
                    VitalIncreases = table.Column<string>(nullable: true),
                    VitalRegen = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_Classes", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "CraftingTables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Crafts = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_CraftingTables", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Crafts", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Ingredients = table.Column<string>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Time = table.Column<int>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_Crafts", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Events", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MapId = table.Column<Guid>(nullable: false),
                    SpawnX = table.Column<int>(nullable: false),
                    SpawnY = table.Column<int>(nullable: false),
                    CommonEvent = table.Column<bool>(nullable: false),
                    Global = table.Column<bool>(nullable: false),
                    Pages = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_Events", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Items", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Animation = table.Column<Guid>(nullable: false),
                    AttackAnimation = table.Column<Guid>(nullable: false),
                    EquipmentAnimation = table.Column<Guid>(nullable: false),
                    Bound = table.Column<bool>(nullable: false),
                    CritChance = table.Column<int>(nullable: false),
                    CritMultiplier = table.Column<double>(nullable: false),
                    Cooldown = table.Column<int>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    AttackSpeedModifier = table.Column<int>(nullable: false),
                    AttackSpeedValue = table.Column<int>(nullable: false),
                    Consumable_Type = table.Column<byte>(nullable: false),
                    Consumable_Value = table.Column<int>(nullable: false),
                    EquipmentSlot = table.Column<int>(nullable: false),
                    TwoHanded = table.Column<bool>(nullable: false),
                    Effect_Type = table.Column<byte>(nullable: false),
                    Effect_Percentage = table.Column<int>(nullable: false),
                    SlotCount = table.Column<int>(nullable: false),
                    Spell = table.Column<Guid>(nullable: false),
                    Event = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FemalePaperdoll = table.Column<string>(nullable: true),
                    ItemType = table.Column<int>(nullable: false),
                    MalePaperdoll = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Price = table.Column<int>(nullable: false),
                    Projectile = table.Column<Guid>(nullable: false),
                    Scaling = table.Column<int>(nullable: false),
                    ScalingStat = table.Column<int>(nullable: false),
                    Speed = table.Column<int>(nullable: false),
                    Stackable = table.Column<bool>(nullable: false),
                    StatGrowth = table.Column<int>(nullable: false),
                    Tool = table.Column<int>(nullable: false),
                    VitalsGiven = table.Column<string>(nullable: true),
                    StatsGiven = table.Column<string>(nullable: true),
                    UsageRequirements = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_Items", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "MapFolders", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    JsonData = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_MapFolders", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Maps", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TileData = table.Column<byte[]>(nullable: true),
                    Up = table.Column<Guid>(nullable: false),
                    Down = table.Column<Guid>(nullable: false),
                    Left = table.Column<Guid>(nullable: false),
                    Right = table.Column<Guid>(nullable: false),
                    Revision = table.Column<int>(nullable: false),
                    Attributes = table.Column<byte[]>(nullable: true),
                    Lights = table.Column<string>(nullable: true),
                    Events = table.Column<string>(nullable: true),
                    NpcSpawns = table.Column<string>(nullable: true),
                    Music = table.Column<string>(nullable: true),
                    Sound = table.Column<string>(nullable: true),
                    IsIndoors = table.Column<bool>(nullable: false),
                    Panorama = table.Column<string>(nullable: true),
                    Fog = table.Column<string>(nullable: true),
                    FogXSpeed = table.Column<int>(nullable: false),
                    FogYSpeed = table.Column<int>(nullable: false),
                    FogTransparency = table.Column<int>(nullable: false),
                    RHue = table.Column<int>(nullable: false),
                    GHue = table.Column<int>(nullable: false),
                    BHue = table.Column<int>(nullable: false),
                    AHue = table.Column<int>(nullable: false),
                    Brightness = table.Column<int>(nullable: false),
                    ZoneType = table.Column<int>(nullable: false),
                    PlayerLightSize = table.Column<int>(nullable: false),
                    PlayerLightIntensity = table.Column<byte>(nullable: false),
                    PlayerLightExpand = table.Column<float>(nullable: false),
                    PlayerLightColor = table.Column<string>(nullable: true),
                    OverlayGraphic = table.Column<string>(nullable: true),
                    WeatherAnimation = table.Column<Guid>(nullable: false),
                    WeatherXSpeed = table.Column<int>(nullable: false),
                    WeatherYSpeed = table.Column<int>(nullable: false),
                    WeatherIntensity = table.Column<int>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_Maps", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Npcs", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    AggroList = table.Column<string>(nullable: true),
                    AttackAllies = table.Column<bool>(nullable: false),
                    AttackAnimation = table.Column<Guid>(nullable: false),
                    Aggressive = table.Column<bool>(nullable: false),
                    Movement = table.Column<byte>(nullable: false),
                    Swarm = table.Column<bool>(nullable: false),
                    FleeHealthPercentage = table.Column<byte>(nullable: false),
                    FocusHighestDamageDealer = table.Column<bool>(nullable: false),
                    PlayerFriendConditions = table.Column<string>(nullable: true),
                    AttackOnSightConditions = table.Column<string>(nullable: true),
                    PlayerCanAttackConditions = table.Column<string>(nullable: true),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    CritChance = table.Column<int>(nullable: false),
                    CritMultiplier = table.Column<double>(nullable: false),
                    OnDeathEvent = table.Column<Guid>(nullable: false),
                    OnDeathPartyEvent = table.Column<Guid>(nullable: false),
                    Drops = table.Column<string>(nullable: true),
                    Experience = table.Column<long>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    MaxVital = table.Column<string>(nullable: true),
                    NpcVsNpcEnabled = table.Column<bool>(nullable: false),
                    Scaling = table.Column<int>(nullable: false),
                    ScalingStat = table.Column<int>(nullable: false),
                    SightRange = table.Column<int>(nullable: false),
                    SpawnDuration = table.Column<int>(nullable: false),
                    SpellFrequency = table.Column<int>(nullable: false),
                    Spells = table.Column<string>(nullable: true),
                    Sprite = table.Column<string>(nullable: true),
                    Stats = table.Column<string>(nullable: true),
                    VitalRegen = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_Npcs", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "PlayerSwitches", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_PlayerSwitches", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "PlayerVariables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_PlayerVariables", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Projectiles", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Ammo = table.Column<Guid>(nullable: false),
                    AmmoRequired = table.Column<int>(nullable: false),
                    Animations = table.Column<string>(nullable: true),
                    Delay = table.Column<int>(nullable: false),
                    GrappleHook = table.Column<bool>(nullable: false),
                    IgnoreActiveResources = table.Column<bool>(nullable: false),
                    IgnoreExhaustedResources = table.Column<bool>(nullable: false),
                    IgnoreMapBlocks = table.Column<bool>(nullable: false),
                    IgnoreZDimension = table.Column<bool>(nullable: false),
                    Knockback = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Range = table.Column<int>(nullable: false),
                    SpawnLocations = table.Column<string>(nullable: true),
                    Speed = table.Column<int>(nullable: false),
                    Spell = table.Column<Guid>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_Projectiles", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Quests", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    StartDescription = table.Column<string>(nullable: true),
                    BeforeDescription = table.Column<string>(nullable: true),
                    EndDescription = table.Column<string>(nullable: true),
                    InProgressDescription = table.Column<string>(nullable: true),
                    LogAfterComplete = table.Column<bool>(nullable: false),
                    LogBeforeOffer = table.Column<bool>(nullable: false),
                    Quitable = table.Column<bool>(nullable: false),
                    Repeatable = table.Column<bool>(nullable: false),
                    Requirements = table.Column<string>(nullable: true),
                    StartEvent = table.Column<Guid>(nullable: false),
                    EndEvent = table.Column<Guid>(nullable: false),
                    Tasks = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_Quests", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Resources", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Initial_Graphic = table.Column<string>(nullable: true),
                    Initial_RenderBelowEntities = table.Column<bool>(nullable: false),
                    Initial_GraphicFromTileset = table.Column<bool>(nullable: false),
                    Initial_X = table.Column<int>(nullable: false),
                    Initial_Y = table.Column<int>(nullable: false),
                    Initial_Width = table.Column<int>(nullable: false),
                    Initial_Height = table.Column<int>(nullable: false),
                    Exhausted_Graphic = table.Column<string>(nullable: true),
                    Exhausted_RenderBelowEntities = table.Column<bool>(nullable: false),
                    Exhausted_GraphicFromTileset = table.Column<bool>(nullable: false),
                    Exhausted_X = table.Column<int>(nullable: false),
                    Exhausted_Y = table.Column<int>(nullable: false),
                    Exhausted_Width = table.Column<int>(nullable: false),
                    Exhausted_Height = table.Column<int>(nullable: false),
                    Animation = table.Column<Guid>(nullable: false),
                    Drops = table.Column<string>(nullable: true),
                    HarvestingRequirements = table.Column<string>(nullable: true),
                    Event = table.Column<Guid>(nullable: false),
                    VitalRegen = table.Column<int>(nullable: false),
                    MaxHp = table.Column<int>(nullable: false),
                    MinHp = table.Column<int>(nullable: false),
                    SpawnDuration = table.Column<int>(nullable: false),
                    Tool = table.Column<int>(nullable: false),
                    WalkableAfter = table.Column<bool>(nullable: false),
                    WalkableBefore = table.Column<bool>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_Resources", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "ServerSwitches", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true),
                    Value = table.Column<bool>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_ServerSwitches", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "ServerVariables", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TextId = table.Column<string>(nullable: true),
                    Value = table.Column<long>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_ServerVariables", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Shops", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    BuyingWhitelist = table.Column<bool>(nullable: false),
                    DefaultCurrency = table.Column<Guid>(nullable: false),
                    BuyingItems = table.Column<string>(nullable: true),
                    SellingItems = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_Shops", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Spells", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SpellType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    CastAnimation = table.Column<Guid>(nullable: false),
                    HitAnimation = table.Column<Guid>(nullable: false),
                    CastDuration = table.Column<int>(nullable: false),
                    CooldownDuration = table.Column<int>(nullable: false),
                    CastRequirements = table.Column<string>(nullable: true),
                    Combat_CritChance = table.Column<int>(nullable: false),
                    Combat_CritMultiplier = table.Column<double>(nullable: false),
                    Combat_DamageType = table.Column<int>(nullable: false),
                    Combat_HitRadius = table.Column<int>(nullable: false),
                    Combat_Friendly = table.Column<bool>(nullable: false),
                    Combat_CastRange = table.Column<int>(nullable: false),
                    Projectile = table.Column<Guid>(nullable: false),
                    VitalDiff = table.Column<string>(nullable: true),
                    StatDiff = table.Column<string>(nullable: true),
                    Combat_Scaling = table.Column<int>(nullable: false),
                    Combat_ScalingStat = table.Column<int>(nullable: false),
                    Combat_TargetType = table.Column<int>(nullable: false),
                    Combat_HoTDoT = table.Column<bool>(nullable: false),
                    Combat_HotDotInterval = table.Column<int>(nullable: false),
                    Combat_Duration = table.Column<int>(nullable: false),
                    Combat_Effect = table.Column<int>(nullable: false),
                    Combat_TransformSprite = table.Column<string>(nullable: true),
                    Warp_MapId = table.Column<Guid>(nullable: false),
                    Warp_X = table.Column<int>(nullable: false),
                    Warp_Y = table.Column<int>(nullable: false),
                    Warp_Dir = table.Column<byte>(nullable: false),
                    Dash_IgnoreMapBlocks = table.Column<bool>(nullable: false),
                    Dash_IgnoreActiveResources = table.Column<bool>(nullable: false),
                    Dash_IgnoreInactiveResources = table.Column<bool>(nullable: false),
                    Dash_IgnoreZDimensionAttributes = table.Column<bool>(nullable: false),
                    Event = table.Column<Guid>(nullable: false),
                    VitalCost = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_Spells", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Tilesets", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TimeCreated = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                }, constraints: table => { table.PrimaryKey("PK_Tilesets", x => x.Id); }
            );

            migrationBuilder.CreateTable(
                name: "Time", columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DaylightHues = table.Column<string>(nullable: true),
                    RangeInterval = table.Column<int>(nullable: false),
                    Rate = table.Column<float>(nullable: false),
                    SyncTime = table.Column<bool>(nullable: false)
                }, constraints: table => { table.PrimaryKey("PK_Time", x => x.Id); }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Animations");

            migrationBuilder.DropTable(name: "Classes");

            migrationBuilder.DropTable(name: "CraftingTables");

            migrationBuilder.DropTable(name: "Crafts");

            migrationBuilder.DropTable(name: "Events");

            migrationBuilder.DropTable(name: "Items");

            migrationBuilder.DropTable(name: "MapFolders");

            migrationBuilder.DropTable(name: "Maps");

            migrationBuilder.DropTable(name: "Npcs");

            migrationBuilder.DropTable(name: "PlayerSwitches");

            migrationBuilder.DropTable(name: "PlayerVariables");

            migrationBuilder.DropTable(name: "Projectiles");

            migrationBuilder.DropTable(name: "Quests");

            migrationBuilder.DropTable(name: "Resources");

            migrationBuilder.DropTable(name: "ServerSwitches");

            migrationBuilder.DropTable(name: "ServerVariables");

            migrationBuilder.DropTable(name: "Shops");

            migrationBuilder.DropTable(name: "Spells");

            migrationBuilder.DropTable(name: "Tilesets");

            migrationBuilder.DropTable(name: "Time");
        }

    }

}
