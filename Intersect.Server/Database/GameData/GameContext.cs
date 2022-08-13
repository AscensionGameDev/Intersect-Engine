using System.Data.Common;
using System.Reflection;

using Intersect.Config;
using Intersect.Enums;
using Intersect.Framework.Reflection;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Models;
using Intersect.Server.Database.GameData.Migrations;
using Intersect.Server.Database.GameData.Seeds;
using Intersect.Server.Maps;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database.GameData;

public partial class GameContext : IntersectDbContext<GameContext>, IGameContext
{
    private readonly MethodInfo _descriptorOnModelCreating = typeof(GameContext).GetMethod(
        nameof(OnModelCreating),
        1,
        BindingFlags.NonPublic | BindingFlags.Instance,
        default,
        new[] { typeof(ModelBuilder) },
        default
    ) ?? throw new InvalidOperationException();

    public GameContext() : this(
        DefaultConnectionStringBuilder,
        DatabaseOptions.DatabaseType.SQLite
    ) { }

    public GameContext(
        DbConnectionStringBuilder connectionStringBuilder,
        DatabaseOptions.DatabaseType? databaseType,
        bool autoDetectChanges = false,
        bool explicitLoad = false,
        bool lazyLoading = false,
        bool readOnly = false,
        ILoggerFactory? loggerFactory = default,
        QueryTrackingBehavior? queryTrackingBehavior = default
    ) : base(
        autoDetectChanges: autoDetectChanges,
        connectionStringBuilder: connectionStringBuilder ?? DefaultConnectionStringBuilder,
        databaseType: databaseType ?? DatabaseOptions.DatabaseType.SQLite,
        explicitLoad: explicitLoad,
        lazyLoading: lazyLoading,
        loggerFactory: loggerFactory,
        readOnly: readOnly,
        queryTrackingBehavior: queryTrackingBehavior
    ) { }

    public static DbConnectionStringBuilder DefaultConnectionStringBuilder =>
        new SqliteConnectionStringBuilder(@"Data Source=resources/gamedata.db");

    //Animations
    public DbSet<AnimationBase> Animations { get; set; }

    public DbSet<ContentString> ContentStrings { get; set; }

    //Crafting
    public DbSet<CraftBase> Crafts { get; set; }

    public DbSet<CraftingTableBase> CraftingTables { get; set; }

    //Classes
    public DbSet<ClassBase> Classes { get; set; }

    //Events
    public DbSet<EventBase> Events { get; set; }

    public DbSet<Folder> Folders { get; set; }

    //Items
    public DbSet<ItemBase> Items { get; set; }

    public DbSet<LocaleContentString> LocaleContentStrings { get; set; }

    //Maps
    public DbSet<MapController> Maps { get; set; }

    //NPCs
    public DbSet<NpcBase> Npcs { get; set; }

    //Projectiles
    public DbSet<ProjectileBase> Projectiles { get; set; }

    //Quests
    public DbSet<QuestBase> Quests { get; set; }

    //Resources
    public DbSet<ResourceBase> Resources { get; set; }

    //Shops
    public DbSet<ShopBase> Shops { get; set; }

    //Spells
    public DbSet<SpellBase> Spells { get; set; }

    //Variables
    public DbSet<PlayerVariableBase> PlayerVariables { get; set; }

    public DbSet<ServerVariableBase> ServerVariables { get; set; }

    public DbSet<GuildVariableBase> GuildVariables { get; set; }

    //Tilesets
    public DbSet<TilesetBase> Tilesets { get; set; }

    //Time
    public DbSet<TimeBase> Time { get; set; }

    private void OnModelCreating<TDescriptor>(ModelBuilder modelBuilder)
        where TDescriptor : Descriptor
    {
        modelBuilder.Entity<TDescriptor>()
            .HasOne(descriptor => descriptor.Parent)
            .WithMany(folder => (ICollection<TDescriptor>)folder.Children);

        modelBuilder.Entity<TDescriptor>()
            .Navigation(descriptor => descriptor.Parent)
            .AutoInclude();
    }

    private static Type CorrectDescriptorType(Type descriptorType)
    {
        if (descriptorType == typeof(MapBase))
        {
            return typeof(MapController);
        }

        return descriptorType;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContentString>()
            .HasMany(contentString => contentString.Localizations)
            .WithOne(localeContentString => localeContentString.ContentString);

        modelBuilder.Entity<ContentString>()
            .Navigation(contentString => contentString.Localizations)
            .AutoInclude();

        modelBuilder.Entity<LocaleContentString>()
            .HasKey(lcs => new { lcs.Id, lcs.Locale });

        modelBuilder.Entity<LocaleContentString>()
            .Navigation(lcs => lcs.ContentString)
            .AutoInclude();

        modelBuilder.Entity<Folder>()
            .HasOne(folder => folder.Name);

        modelBuilder.Entity<Folder>()
            .Navigation(folder => folder.Name)
            .AutoInclude();

        modelBuilder.Entity<Folder>()
            .HasMany(folder => (ICollection<Folder>)folder.Children)
            .WithOne(child => child.Parent);

        modelBuilder.Entity<Folder>()
            .Navigation(folder => folder.Parent)
            .AutoInclude();

        var descriptorTypes = Enum
            .GetValues<GameObjectType>()
            .Select(descriptorType => descriptorType.GetObjectType())
            .Where(descriptorType => descriptorType.Extends<Descriptor>())
            .OrderBy(descriptorType => descriptorType.Name);
        foreach (var descriptorType in descriptorTypes)
        {
            var correctedDescriptorType = CorrectDescriptorType(descriptorType);
            var methodInfoOnModelCreatingDesriptorType = _descriptorOnModelCreating.MakeGenericMethod(correctedDescriptorType);
            _ = methodInfoOnModelCreatingDesriptorType.Invoke(this, new[] { modelBuilder });
        }
    }

    public override void MigrationsProcessed(string[] migrations)
    {
        if (Array.IndexOf(migrations, "20190611170819_CombiningSwitchesVariables") > -1)
        {
            Beta6Migration.Run(this);
        }

        if (Array.IndexOf(migrations, "20201004032158_EnablingCerasVersionTolerance") > -1)
        {
            CerasVersionToleranceMigration.Run(this);
        }

        if (Array.IndexOf(migrations, "20210512071349_BoundItemExtension") > -1)
        {
            BoundItemExtensionMigration.Run(this);
        }

        if (Array.IndexOf(migrations, "20211031200145_FixQuestTaskCompletionEvents") > -1)
        {
            FixQuestTaskCompletionEventsMigration.Run(this);
        }
    }

    public override void Seed()
    {
#if DEBUG
        new SeedContentStrings().SeedIfEmpty(this);
        ChangeTracker.DetectChanges();
        SaveChanges();
#endif
    }

    internal static partial class Queries
    {
        internal static readonly Func<Guid, ServerVariableBase> ServerVariableById =
            (Guid id) => (ServerVariableBase)ServerVariableBase.Lookup.FirstOrDefault(variable => variable.Key == id).Value;

        internal static readonly Func<string, ServerVariableBase> ServerVariableByName =
            (string name) => (ServerVariableBase)ServerVariableBase.Lookup.FirstOrDefault(variable => string.Equals(variable.Value.Name, name, StringComparison.OrdinalIgnoreCase)).Value;

        internal static readonly Func<int, int, IEnumerable<ServerVariableBase>> ServerVariables =
            (int page, int count) => ServerVariableBase.Lookup.Select(v => (ServerVariableBase)v.Value).OrderBy(v => v.Id.ToString()).Skip(page * count).Take(count);
    }
}
