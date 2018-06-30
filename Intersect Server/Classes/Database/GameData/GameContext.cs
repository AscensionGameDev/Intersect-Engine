using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Classes.Database.GameData
{
    public class GameContext : DbContext
    {
        public static GameContext Current { get; private set; }
        
        //Animations
        public DbSet<AnimationBase> Animations { get; set; }

        //Crafting
        public DbSet<CraftBase> Crafts { get; set; }
        public DbSet<CraftingTableBase> CraftingTables { get; set; }

        //Classes
        public DbSet<ClassBase> Classes { get; set; }

        //Items
        public DbSet<ItemBase> Items { get; set; }

        //NPCs
        public DbSet<NpcBase> Npcs { get; set; }

        //Projectiles
        public DbSet<ProjectileBase> Projectiles { get; set; }

        //Resources
        public DbSet<ResourceBase> Resources { get; set; }

        //Shops
        public DbSet<ShopBase> Shops { get; set; }

        private DatabaseUtils.DbProvider mConnection = DatabaseUtils.DbProvider.Sqlite;
        private string mConnectionString = @"Data Source=resources/gamedata.db";

        public GameContext()
        {
            Current = this;
        }

        public GameContext(DatabaseUtils.DbProvider connection, string connectionString)
        {
            mConnection = connection;
            mConnectionString = connectionString;
            Current = this;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (mConnection)
            {
                case DatabaseUtils.DbProvider.Sqlite:
                    optionsBuilder.UseSqlite(mConnectionString);
                    break;
                case DatabaseUtils.DbProvider.MySql:
                    optionsBuilder.UseMySql(mConnectionString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnimationBase>().OwnsOne(p => p.Upper);
            modelBuilder.Entity<AnimationBase>().OwnsOne(p => p.Lower);
            modelBuilder.Entity<ResourceBase>().OwnsOne(p => p.Initial);
            modelBuilder.Entity<ResourceBase>().OwnsOne(p => p.Exhausted);
        }
    }
}
