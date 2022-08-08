using System.Data.Common;

using Intersect.Config;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Models;
using Intersect.Server.Database.GameData.Migrations;
using Intersect.Server.Database.GameData.Seeds;
using Intersect.Server.Maps;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.GameData
{

    public partial class GameContext : IntersectDbContext<GameContext>, IGameContext
    {

        public GameContext() : base(DefaultConnectionStringBuilder)
        {

        }

        public GameContext(
            DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType,
            bool readOnly = false,
            Intersect.Logging.Logger logger = null,
            Intersect.Logging.LogLevel logLevel = Intersect.Logging.LogLevel.None
        ) : base(connectionStringBuilder, databaseType, logger, logLevel, readOnly, autoDetectChanges: false)
        {

        }

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

        public DbSet<MapList> MapFolders { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContentString>()
                .HasMany(contentString => contentString.Localizations)
                .WithOne(localeContentString => localeContentString.ContentString);

            modelBuilder.Entity<LocaleContentString>()
                .HasKey(lcs => new { lcs.Id, lcs.Locale });

            var descriptorTypes = Enum.GetValues<GameObjectType>().Select(descriptorType => descriptorType.GetObjectType());
            foreach (var descriptorType in descriptorTypes)
            {
                modelBuilder.Entity<Folder>()
                    .HasMany(typeof(ICollection<>).MakeGenericType(descriptorType), nameof(Folder.Children))
                    .WithOne(nameof(IFolderable.Parent));
            }

            modelBuilder.Entity<Folder>()
                .HasMany(folder => (ICollection<Folder>)folder.Children)
                .WithOne(child => child.Parent);

            modelBuilder.Entity<Folder>()
                .HasOne(folder => folder.Name);
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
}
