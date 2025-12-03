using Intersect.Extensions;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.Core.GameObjects.Crafting;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Mapping.Tilesets;
using Intersect.Framework.Core.GameObjects.Maps.MapList;
using Intersect.Framework.Core.GameObjects.NPCs;
using Intersect.Framework.Core.GameObjects.PlayerClass;
using Intersect.Framework.Core.GameObjects.Resources;
using Intersect.Framework.Core.GameObjects.Skills;
using Intersect.Framework.Core.GameObjects.Variables;
using Intersect.GameObjects;
using Intersect.Server.Database.GameData.Migrations;
using Intersect.Server.Maps;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.GameData;

/// <summary>
/// <see cref="DbContext"/> implementation that contains static game content descriptors.
/// </summary>
public abstract partial class GameContext : IntersectDbContext<GameContext>, IGameContext
{
    /// <inheritdoc />
    protected GameContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    //Animations
    public DbSet<AnimationDescriptor> Animations { get; set; }

    //Crafting
    public DbSet<CraftingRecipeDescriptor> Crafts { get; set; }

    public DbSet<CraftingTableDescriptor> CraftingTables { get; set; }

    //Classes
    public DbSet<ClassDescriptor> Classes { get; set; }

    //Events
    public DbSet<EventDescriptor> Events { get; set; }

    //Items
    public DbSet<ItemDescriptor> Items { get; set; }

    //Equipment Properties of Items
    public DbSet<EquipmentProperties> Items_EquipmentProperties { get; set; }

    //Maps
    public DbSet<MapController> Maps { get; set; }

    public DbSet<MapList> MapFolders { get; set; }

    //NPCs
    public DbSet<NPCDescriptor> Npcs { get; set; }

    //Projectiles
    public DbSet<ProjectileDescriptor> Projectiles { get; set; }

    //Quests
    public DbSet<QuestDescriptor> Quests { get; set; }

    //Resources
    public DbSet<ResourceDescriptor> Resources { get; set; }

    //Shops
    public DbSet<ShopDescriptor> Shops { get; set; }

    //Spells
    public DbSet<SpellDescriptor> Spells { get; set; }

    //Variables
    public DbSet<PlayerVariableDescriptor> PlayerVariables { get; set; }

    public DbSet<ServerVariableDescriptor> ServerVariables { get; set; }

    public DbSet<GuildVariableDescriptor> GuildVariables { get; set; }

    public DbSet<UserVariableDescriptor> UserVariables { get; set; }

    //Tilesets
    public DbSet<TilesetDescriptor> Tilesets { get; set; }

    //Time
    public DbSet<DaylightCycleDescriptor> Time { get; set; }

    //Skills
    public DbSet<SkillDescriptor> Skills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EquipmentProperties>()
            .HasOne(property => property.Descriptor)
            .WithOne(item => item.EquipmentProperties)
            .HasForeignKey<EquipmentProperties>(property => property.DescriptorId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override void OnSchemaMigrationsProcessed(string[] migrations)
    {
        if (migrations.IndexOf("20190611170819_CombiningSwitchesVariables") > -1)
        {
            Beta6Migration.Run(this);
        }

        if (migrations.IndexOf("20201004032158_EnablingCerasVersionTolerance") > -1)
        {
            CerasVersionToleranceMigration.Run(this);
        }

        if (migrations.IndexOf("20210512071349_BoundItemExtension") > -1)
        {
            BoundItemExtensionMigration.Run(this);
        }

        if (migrations.IndexOf("20211031200145_FixQuestTaskCompletionEvents") > -1)
        {
            FixQuestTaskCompletionEventsMigration.Run(this);
        }
    }

    internal static partial class Queries
    {
        internal static readonly Func<Guid, ServerVariableDescriptor> ServerVariableById =
            (Guid id) => (ServerVariableDescriptor)ServerVariableDescriptor.Lookup.FirstOrDefault(variable => variable.Key == id).Value;

        internal static readonly Func<string, ServerVariableDescriptor> ServerVariableByName =
            (string name) => (ServerVariableDescriptor)ServerVariableDescriptor.Lookup.FirstOrDefault(variable => string.Equals(variable.Value.Name, name, StringComparison.OrdinalIgnoreCase)).Value;

        internal static readonly Func<int, int, IEnumerable<ServerVariableDescriptor>> ServerVariables =
            (int page, int count) => ServerVariableDescriptor.Lookup.Select(v => (ServerVariableDescriptor)v.Value).OrderBy(v => v.Id.ToString()).Skip(page * count).Take(count);
    }
}
