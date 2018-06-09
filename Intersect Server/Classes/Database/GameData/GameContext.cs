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
        public DbSet<BenchBase> Benches { get; set; }
        public DbSet<Craft> Crafts { get; set; }
        public DbSet<CraftIngredient> Ingredients { get; set; }

        //Items
        public DbSet<ItemBase> Items { get; set; }

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
            modelBuilder.Entity<BenchBase>().HasMany(b => b.Crafts);
            modelBuilder.Entity<Craft>().HasMany(b => b.Ingredients);
            modelBuilder.Entity<Craft>().HasOne(b => b.Item);
            modelBuilder.Entity<ItemBase>().HasOne(b => b.Animation);
            modelBuilder.Entity<ItemBase>().HasOne(b => b.AttackAnimation);
        }
    }
}
