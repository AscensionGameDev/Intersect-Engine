using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Intersect.Config;
using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.SeedData;
using Intersect.Server.Entities;

using JetBrains.Annotations;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.PlayerData
{

    public class PlayerContext : IntersectDbContext<PlayerContext>
    {

        public PlayerContext() : base(DefaultConnectionStringBuilder)
        {
        }

        public PlayerContext(
            [NotNull] DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType,
            Intersect.Logging.Logger logger = null,
            Intersect.Logging.LogLevel logLevel = Intersect.Logging.LogLevel.None
        ) : base(connectionStringBuilder, databaseType, false, logger, logLevel)
        {
        }

        [NotNull]
        public static DbConnectionStringBuilder DefaultConnectionStringBuilder =>
            new SqliteConnectionStringBuilder(@"Data Source=resources/playerdata.db");

        [NotNull]
        public DbSet<User> Users { get; set; }

        [NotNull]
        public DbSet<Mute> Mutes { get; set; }

        [NotNull]
        public DbSet<Ban> Bans { get; set; }

        [NotNull]
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        [NotNull]
        public DbSet<Player> Players { get; set; }

        [NotNull]
        public DbSet<BankSlot> Player_Bank { get; set; }

        [NotNull]
        public DbSet<Friend> Player_Friends { get; set; }

        [NotNull]
        public DbSet<HotbarSlot> Player_Hotbar { get; set; }

        [NotNull]
        public DbSet<InventorySlot> Player_Items { get; set; }

        [NotNull]
        public DbSet<Quest> Player_Quests { get; set; }

        [NotNull]
        public DbSet<SpellSlot> Player_Spells { get; set; }

        [NotNull]
        public DbSet<Variable> Player_Variables { get; set; }

        [NotNull]
        public DbSet<Bag> Bags { get; set; }

        [NotNull]
        public DbSet<BagSlot> Bag_Items { get; set; }

        internal async ValueTask Commit(
            bool commit = false,
            CancellationToken cancellationToken = default(CancellationToken)
        )
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

        protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>().HasOne(token => token.User);

            modelBuilder.Entity<Ban>().HasOne(b => b.User);
            modelBuilder.Entity<Mute>().HasOne(b => b.User);

            modelBuilder.Entity<User>().HasMany(b => b.Players).WithOne(p => p.User);

            modelBuilder.Entity<Player>()
                .HasMany(b => b.Friends)
                .WithOne(p => p.Owner)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Friend>().HasOne(b => b.Target).WithMany().OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>().HasMany(b => b.Spells).WithOne(p => p.Player);

            modelBuilder.Entity<Player>().HasMany(b => b.Items).WithOne(p => p.Player);

            modelBuilder.Entity<Player>().HasMany(b => b.Variables).WithOne(p => p.Player);
            modelBuilder.Entity<Variable>().HasIndex(p => new {p.VariableId, CharacterId = p.PlayerId}).IsUnique();

            modelBuilder.Entity<Player>().HasMany(b => b.Hotbar).WithOne(p => p.Player);

            modelBuilder.Entity<Player>().HasMany(b => b.Quests).WithOne(p => p.Player);
            modelBuilder.Entity<Quest>().HasIndex(p => new {p.QuestId, CharacterId = p.PlayerId}).IsUnique();

            modelBuilder.Entity<Player>().HasMany(b => b.Bank).WithOne(p => p.Player);

            modelBuilder.Entity<Bag>()
                .HasMany(b => b.Slots)
                .WithOne(p => p.ParentBag)
                .HasForeignKey(p => p.ParentBagId);

            modelBuilder.Entity<InventorySlot>().HasOne(b => b.Bag);
            modelBuilder.Entity<BagSlot>().HasOne(b => b.Bag);
            modelBuilder.Entity<BankSlot>().HasOne(b => b.Bag);
        }

        public void Seed()
        {
#if DEBUG
            new SeedUsers().SeedIfEmpty(this);

            SaveChanges();
#endif
        }

        public override void MigrationsProcessed(string[] migrations)
        {
        }

    }

}
