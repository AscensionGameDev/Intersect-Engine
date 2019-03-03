using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Server.Classes.Database.PlayerData.Api;
using Intersect.Server.Database.PlayerData.Characters;
using Intersect.Server.Entities;
using Intersect.Utilities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.PlayerData
{
    public class PlayerContext : DbContext
    {
        public static PlayerContext Current { get; private set; }

        [NotNull] public DbSet<User> Users { get; set; }
        [NotNull] public DbSet<Player> Characters { get; set; }
        [NotNull] public DbSet<Friend> Character_Friends { get; set; }
        [NotNull] public DbSet<SpellSlot> Character_Spells { get; set; }
        [NotNull] public DbSet<Switch> Character_Switches { get; set; }
        [NotNull] public DbSet<Variable> Character_Variables { get; set; }
        [NotNull] public DbSet<HotbarSlot> Character_Hotbar { get; set; }
        [NotNull] public DbSet<Quest> Character_Quests { get; set; }
        [NotNull] public DbSet<Bag> Bags { get; set; }
        [NotNull] public DbSet<InventorySlot> Character_Items { get; set; }
        [NotNull] public DbSet<BankSlot> Character_Bank { get; set; }
        [NotNull] public DbSet<BagSlot> Bag_Items { get; set; }
        [NotNull] public DbSet<Mute> Mutes { get; set; }
        [NotNull] public DbSet<Ban> Bans { get; set; }
        [NotNull] public DbSet<RefreshToken> RefreshTokens { get; set; }

        private DatabaseUtils.DbProvider mConnection = DatabaseUtils.DbProvider.Sqlite;
        private string mConnectionString = @"Data Source=resources/playerdata.db";

        public PlayerContext()
        {
            Current = this;
        }

        public PlayerContext(DatabaseUtils.DbProvider connection,string connectionString)
        {
            mConnection = connection;
            mConnectionString = connectionString;
            Current = this;
        }

        internal async ValueTask Commit(bool commit = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!commit)
            {
                return;
            }

            var task = SaveChangesAsync(cancellationToken);
            if (task != null)
            {
                await task;
            }
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
            modelBuilder.Entity<User>()
                .HasMany(b => b.Characters)
                .WithOne(p => p.Account);

            modelBuilder.Entity<Player>().HasMany(b => b.Friends).WithOne(p => p.Owner);

            modelBuilder.Entity<Player>().HasMany(b => b.Spells).WithOne(p => p.Character);

            modelBuilder.Entity<Player>().HasMany(b => b.Items).WithOne(p => p.Character);

            modelBuilder.Entity<Player>().HasMany(b => b.Switches).WithOne(p => p.Character);
            modelBuilder.Entity<Switch>().HasIndex(p => new { p.SwitchId, p.CharacterId }).IsUnique();

            modelBuilder.Entity<Player>().HasMany(b => b.Variables).WithOne(p => p.Character);
            modelBuilder.Entity<Variable>().HasIndex(p => new { p.VariableId, p.CharacterId }).IsUnique();

            modelBuilder.Entity<Player>().HasMany(b => b.Hotbar).WithOne(p => p.Character);

            modelBuilder.Entity<Player>().HasMany(b => b.Quests).WithOne(p => p.Character);
            modelBuilder.Entity<Quest>().HasIndex(p => new { p.QuestId, p.CharacterId }).IsUnique();

            modelBuilder.Entity<Player>().HasMany(b => b.Bank).WithOne(p => p.Character);

            modelBuilder.Entity<Bag>().HasMany(b => b.Slots).WithOne(p => p.ParentBag).HasForeignKey(p => p.ParentBagId);

            modelBuilder.Entity<InventorySlot>().HasOne(b => b.Bag);
            modelBuilder.Entity<BagSlot>().HasOne(b => b.Bag);
            modelBuilder.Entity<BankSlot>().HasOne(b => b.Bag);

            modelBuilder.Entity<Ban>().HasOne(b => b.Player);
            modelBuilder.Entity<Mute>().HasOne(b => b.Player);

        }

        public bool IsEmpty()
        {
            using (var command = Database.GetDbConnection().CreateCommand())
            {
                if (mConnection == DatabaseUtils.DbProvider.MySql)
                {
                    command.CommandText = "show tables;";
                }
                else if (mConnection == DatabaseUtils.DbProvider.Sqlite)
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
                }
                command.CommandType = CommandType.Text;

                Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    return !result.HasRows;
                }
            }
        }

    }
}
