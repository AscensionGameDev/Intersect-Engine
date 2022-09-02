using Intersect.Config;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Database.Logging.Seed;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Intersect.Server.Database.Logging;

/// <summary>
/// MySQL/MariaDB-specific implementation of <see cref="LoggingContext"/>
/// </summary>
public sealed class MySqlLoggingContext : LoggingContext, IMySqlDbContext
{
    /// <inheritdoc />
    public MySqlLoggingContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }
}

/// <summary>
/// SQLite-specific implementation of <see cref="LoggingContext"/>
/// </summary>
public sealed class SqliteLoggingContext : LoggingContext, ISqliteDbContext
{
    /// <inheritdoc />
    public SqliteLoggingContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }
}

public abstract partial class LoggingContext : IntersectDbContext<LoggingContext>, ILoggingContext
{
    /// <inheritdoc />
    protected LoggingContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    /// <inheritdoc />
    public DbSet<RequestLog> RequestLogs { get; set; }

    /// <inheritdoc />
    public DbSet<UserActivityHistory> UserActivityHistory { get; set; }

    /// <inheritdoc />
    public DbSet<ChatHistory> ChatHistory { get; set; }

    /// <inheritdoc />
    public DbSet<TradeHistory> TradeHistory { get; set; }

    /// <inheritdoc />
    public DbSet<GuildHistory> GuildHistory { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new RequestLog.Mapper());
    }

    /// <inheritdoc />
    public override void Seed()
    {
#if DEBUG
        new SeedTrades().SeedIfEmpty(this);
        ChangeTracker.DetectChanges();
        SaveChanges();
#endif
    }
}
