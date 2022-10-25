using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using Intersect.Config;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Server.Database.GameData.Migrations;
using Intersect.Server.Maps;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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
        ) : base(connectionStringBuilder, databaseType, logger, logLevel, readOnly, false)
        {

        }

        public static DbConnectionStringBuilder DefaultConnectionStringBuilder =>
            new SqliteConnectionStringBuilder(@"Data Source=resources/gamedata.db");

        //Animations
        public DbSet<AnimationBase> Animations { get; set; }

        //Crafting
        public DbSet<CraftBase> Crafts { get; set; }

        public DbSet<CraftingTableBase> CraftingTables { get; set; }

        //Classes
        public DbSet<ClassBase> Classes { get; set; }

        //Events
        public DbSet<EventBase> Events { get; set; }

        //Items
        public DbSet<ItemBase> Items { get; set; }

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

        public DbSet<UserVariableBase> UserVariables { get; set; }

        //Tilesets
        public DbSet<TilesetBase> Tilesets { get; set; }

        //Time
        public DbSet<TimeBase> Time { get; set; }

        public override void MigrationsProcessed(string[] migrations)
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
            internal static readonly Func<Guid, ServerVariableBase> ServerVariableById =
                (Guid id) => (ServerVariableBase)ServerVariableBase.Lookup.FirstOrDefault(variable => variable.Key == id).Value;

            internal static readonly Func<string, ServerVariableBase> ServerVariableByName =
                (string name) => (ServerVariableBase)ServerVariableBase.Lookup.FirstOrDefault(variable => string.Equals(variable.Value.Name, name, StringComparison.OrdinalIgnoreCase)).Value;

            internal static readonly Func<int, int, IEnumerable<ServerVariableBase>> ServerVariables =
                (int page, int count) => ServerVariableBase.Lookup.Select(v => (ServerVariableBase)v.Value).OrderBy(v => v.Id.ToString()).Skip(page * count).Take(count);
        }
    }
}
