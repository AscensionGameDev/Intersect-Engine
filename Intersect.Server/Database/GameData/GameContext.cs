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

namespace Intersect.Server.Database.GameData;

/// <summary>
/// MySQL/MariaDB-specific implementation of <see cref="GameContext"/>
/// </summary>
public sealed class MySqlGameContext : GameContext, IMySqlDbContext
{
    /// <inheritdoc />
    public MySqlGameContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }
}

/// <summary>
/// SQLite-specific implementation of <see cref="GameContext"/>
/// </summary>
public sealed class SqliteGameContext : GameContext, ISqliteDbContext
{
    /// <inheritdoc />
    public SqliteGameContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }
}

/// <summary>
/// <see cref="DbContext"/> implementation that contains static game content descriptors.
/// </summary>
public abstract partial class GameContext : IntersectDbContext<GameContext>, IGameContext
{
    private readonly MethodInfo _descriptorOnModelCreating = typeof(GameContext).GetMethod(
        nameof(OnModelCreating),
        1,
        BindingFlags.NonPublic | BindingFlags.Instance,
        default,
        new[] { typeof(ModelBuilder) },
        default
    ) ?? throw new InvalidOperationException();

    /// <inheritdoc />
    protected GameContext(DatabaseContextOptions? databaseContextOptions) : base(databaseContextOptions) { }

    /// <inheritdoc />
    public DbSet<AnimationBase> Animations { get; set; }

    /// <inheritdoc />
    public DbSet<ContentString> ContentStrings { get; set; }

    /// <inheritdoc />
    public DbSet<CraftBase> Crafts { get; set; }

    /// <inheritdoc />
    public DbSet<CraftingTableBase> CraftingTables { get; set; }

    /// <inheritdoc />
    public DbSet<ClassBase> Classes { get; set; }

    /// <inheritdoc />
    public DbSet<EventBase> Events { get; set; }

    /// <inheritdoc />
    public DbSet<Folder> Folders { get; set; }

    /// <inheritdoc />
    public DbSet<ItemBase> Items { get; set; }

    /// <inheritdoc />
    public DbSet<LocaleContentString> LocaleContentStrings { get; set; }

    /// <inheritdoc />
    public DbSet<MapController> Maps { get; set; }

    /// <inheritdoc />
    public DbSet<NpcBase> Npcs { get; set; }

    /// <inheritdoc />
    public DbSet<ProjectileBase> Projectiles { get; set; }

    /// <inheritdoc />
    public DbSet<QuestBase> Quests { get; set; }

    /// <inheritdoc />
    public DbSet<ResourceBase> Resources { get; set; }

    /// <inheritdoc />
    public DbSet<ShopBase> Shops { get; set; }

    /// <inheritdoc />
    public DbSet<SpellBase> Spells { get; set; }

    /// <inheritdoc />
    public DbSet<PlayerVariableBase> PlayerVariables { get; set; }

    /// <inheritdoc />
    public DbSet<ServerVariableBase> ServerVariables { get; set; }

    /// <inheritdoc />
    public DbSet<GuildVariableBase> GuildVariables { get; set; }

    /// <inheritdoc />
    public DbSet<TilesetBase> Tilesets { get; set; }

    /// <inheritdoc />
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
            var methodInfoOnModelCreatingDesriptorType =
                _descriptorOnModelCreating.MakeGenericMethod(correctedDescriptorType);
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

    /// <inheritdoc />
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
            (Guid id) =>
                (ServerVariableBase)ServerVariableBase.Lookup.FirstOrDefault(variable => variable.Key == id).Value;

        internal static readonly Func<string, ServerVariableBase> ServerVariableByName =
            (string name) => (ServerVariableBase)ServerVariableBase.Lookup.FirstOrDefault(variable =>
                string.Equals(variable.Value.Name, name, StringComparison.OrdinalIgnoreCase)).Value;

        internal static readonly Func<int, int, IEnumerable<ServerVariableBase>> ServerVariables =
            (int page, int count) => ServerVariableBase.Lookup.Select(v => (ServerVariableBase)v.Value)
                .OrderBy(v => v.Id.ToString()).Skip(page * count).Take(count);
    }
}
