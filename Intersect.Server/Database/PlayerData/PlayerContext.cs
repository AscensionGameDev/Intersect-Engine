using System.Data.Common;
using Intersect.Config;
using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Database.PlayerData.Migrations;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.SeedData;
using Intersect.Server.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database.PlayerData;

/// <summary>
/// MySQL/MariaDB-specific implementation of <see cref="PlayerContext"/>
/// </summary>
public sealed class MySqlPlayerContext : PlayerContext, IMySqlDbContext
{
    /// <inheritdoc />
    public MySqlPlayerContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }
}

/// <summary>
/// SQLite-specific implementation of <see cref="PlayerContext"/>
/// </summary>
public sealed class SqlitePlayerContext : PlayerContext, ISqliteDbContext
{
    /// <inheritdoc />
    public SqlitePlayerContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }
}

public abstract partial class PlayerContext : IntersectDbContext<PlayerContext>, IPlayerContext
{
    /// <inheritdoc />
    protected PlayerContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    /// <inheritdoc />
    public DbSet<User> Users { get; set; }

    /// <inheritdoc />
    public DbSet<Mute> Mutes { get; set; }

    /// <inheritdoc />
    public DbSet<Ban> Bans { get; set; }

    /// <inheritdoc />
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    /// <inheritdoc />
    public DbSet<Player> Players { get; set; }

    /// <inheritdoc />
    public DbSet<BankSlot> Player_Bank { get; set; }

    /// <inheritdoc />
    public DbSet<Friend> Player_Friends { get; set; }

    /// <inheritdoc />
    public DbSet<HotbarSlot> Player_Hotbar { get; set; }

    /// <inheritdoc />
    public DbSet<InventorySlot> Player_Items { get; set; }

    /// <inheritdoc />
    public DbSet<Quest> Player_Quests { get; set; }

    /// <inheritdoc />
    public DbSet<SpellSlot> Player_Spells { get; set; }

    /// <inheritdoc />
    public DbSet<PlayerVariable> Player_Variables { get; set; }

    /// <inheritdoc />
    public DbSet<Bag> Bags { get; set; }

    /// <inheritdoc />
    public DbSet<BagSlot> Bag_Items { get; set; }

    /// <inheritdoc />
    public DbSet<Guild> Guilds { get; set; }

    /// <inheritdoc />
    public DbSet<GuildBankSlot> Guild_Bank { get; set; }

    /// <inheritdoc />
    public DbSet<GuildVariable> Guild_Variables { get; set; }

    internal async ValueTask Commit(bool commit = false, CancellationToken cancellationToken = default)
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
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
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
        modelBuilder.Entity<PlayerVariable>().HasIndex(p => new { p.VariableId, CharacterId = p.PlayerId }).IsUnique();

        modelBuilder.Entity<Player>().HasMany(b => b.Hotbar).WithOne(p => p.Player);

        modelBuilder.Entity<Player>().HasMany(b => b.Quests).WithOne(p => p.Player);
        modelBuilder.Entity<Quest>().HasIndex(p => new { p.QuestId, CharacterId = p.PlayerId }).IsUnique();

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

        modelBuilder.Entity<Guild>().HasMany(b => b.Variables).WithOne(p => p.Guild);
        modelBuilder.Entity<GuildVariable>().HasIndex(p => new { p.VariableId, GuildId = p.GuildId }).IsUnique();
    }

    /// <inheritdoc />
    public override void Seed()
    {
#if DEBUG
        new SeedUsers().SeedIfEmpty(this);
        ChangeTracker.DetectChanges();
        SaveChanges();
#endif
    }

    public override void MigrationsProcessed(string[] migrations)
    {
        if (Array.IndexOf(migrations, "20220331140427_GuildBankMaxSlotsMigration") > -1)
        {
            GuildBankMaxSlotMigration.Run(this);
        }
    }

    /// <inheritdoc />
    protected override void StopTracking(EntityEntry entityEntry)
    {
        base.StopTracking(entityEntry);

        if (entityEntry.Entity is Player player)
        {
            StopTrackingPlayer(player);
        }
    }

    private void StopTrackingPlayer(Player player)
    {
        foreach (var bankSlot in player.Bank)
        {
            Entry(bankSlot).State = EntityState.Detached;
        }

        foreach (var friend in player.Friends)
        {
            Entry(friend).State = EntityState.Detached;
        }

        foreach (var hotbarSlot in player.Hotbar)
        {
            Entry(hotbarSlot).State = EntityState.Detached;
        }

        foreach (var inventorySlot in player.Items)
        {
            Entry(inventorySlot).State = EntityState.Detached;
        }

        foreach (var quest in player.Quests)
        {
            Entry(quest).State = EntityState.Detached;
        }

        foreach (var spellSlot in player.Spells)
        {
            Entry(spellSlot).State = EntityState.Detached;
        }

        foreach (var playerVariable in player.Variables)
        {
            Entry(playerVariable).State = EntityState.Detached;
        }
    }

    public void StopTrackingUsersExcept(User user)
    {
        StopTrackingExcept(user);
        StopTrackingExcept(user.Players.ToArray());
    }
}
