using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Intersect.Config;
using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.SeedData;
using Intersect.Server.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.PlayerData
{

    public class PlayerContext : IntersectDbContext<PlayerContext>, IPlayerContext
    {

        public PlayerContext() : base(DefaultConnectionStringBuilder)
        {
        }

        public PlayerContext(
            DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType,
            bool readOnly = false,
            Intersect.Logging.Logger logger = null,
            Intersect.Logging.LogLevel logLevel = Intersect.Logging.LogLevel.None
        ) : base(connectionStringBuilder, databaseType, logger, logLevel, readOnly, false)
        {
        }

        public static DbConnectionStringBuilder DefaultConnectionStringBuilder =>
            new SqliteConnectionStringBuilder(@"Data Source=resources/playerdata.db");

        public DbSet<User> Users { get; set; }

        public DbSet<Mute> Mutes { get; set; }

        public DbSet<Ban> Bans { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<BankSlot> Player_Bank { get; set; }

        public DbSet<Friend> Player_Friends { get; set; }

        public DbSet<HotbarSlot> Player_Hotbar { get; set; }

        public DbSet<InventorySlot> Player_Items { get; set; }

        public DbSet<Quest> Player_Quests { get; set; }

        public DbSet<SpellSlot> Player_Spells { get; set; }

        public DbSet<Variable> Player_Variables { get; set; }

        public DbSet<Bag> Bags { get; set; }

        public DbSet<BagSlot> Bag_Items { get; set; }

        public DbSet<Guild> Guilds { get; set; }

        public DbSet<GuildBankSlot> Guild_Bank { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
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

            modelBuilder.Entity<Guild>().HasMany(b => b.Bank).WithOne(p => p.Guild);
            modelBuilder.Entity<GuildBankSlot>().HasOne(b => b.Bag);
        }

        public void Seed()
        {
#if DEBUG
            new SeedUsers().SeedIfEmpty(this);
            ChangeTracker.DetectChanges();
            SaveChanges();
#endif
        }

        public override void MigrationsProcessed(string[] migrations)
        {
        }

        public void StopTrackingExcept(object obj)
        {
            foreach (var trackingState in ChangeTracker.Entries().ToArray())
            {
                if (trackingState.Entity != obj)
                {
                    trackingState.State = EntityState.Detached;
                }
            }
        }

        public void StopTrackingUsersExcept(User user)
        {
            //We don't want to be saving this players friends or anything....
            var otherUsers = ChangeTracker.Entries().Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted) && (e.Entity is User && e.Entity != user)).ToList();
            foreach (var otherUser in otherUsers)
            {
                if (otherUser.Entity != null)
                {
                    Entry(otherUser.Entity).State = EntityState.Detached;
                }
            }

            StopTrackingPlayersExcept(user.Players.ToArray());

        }

        public void StopTrackingPlayersExcept(Player[] players, bool trackUsers = true)
        {
            //We don't want to be saving this players friends or anything....
            var otherPlayers = ChangeTracker.Entries().Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted) && (e.Entity is Player && !players.Contains(e.Entity))).ToList();
            foreach (var otherPlayer in otherPlayers)
            {
                if (otherPlayer.Entity != null)
                {
                    DetachPlayer((Player)otherPlayer.Entity);
                }
            }
        }

        private void DetachPlayer(Player player)
        {
            Entry(player).State = EntityState.Detached;

            foreach (var itm in player.Friends)
                Entry(itm).State = EntityState.Detached;

            foreach (var itm in player.Spells)
                Entry(itm).State = EntityState.Detached;

            foreach (var itm in player.Items)
                Entry(itm).State = EntityState.Detached;

            foreach (var itm in player.Variables)
                Entry(itm).State = EntityState.Detached;

            foreach (var itm in player.Hotbar)
                Entry(itm).State = EntityState.Detached;

            foreach (var itm in player.Quests)
                Entry(itm).State = EntityState.Detached;

            foreach (var itm in player.Bank)
                Entry(itm).State = EntityState.Detached;
        }

    }

}
