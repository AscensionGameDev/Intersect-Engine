using Intersect.Config;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Database.Logging.Seed;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Intersect.Server.Database.Logging;

public sealed class MySqlLoggingContext : LoggingContext, IMySqlDbContext
{
    public MySqlLoggingContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}

public sealed class SqliteLoggingContext : LoggingContext, ISqliteDbContext
{
    public static readonly DatabaseContextOptions DefaultContextOptions = new()
    {
        ConnectionStringBuilder = new SqliteConnectionStringBuilder($@"Data Source={DbInterface.LoggingDbFilename}"),
        DatabaseType = DatabaseType.Sqlite
    };

    public SqliteLoggingContext() : base(DefaultContextOptions) { }

    public SqliteLoggingContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}

public abstract partial class LoggingContext : IntersectDbContext<LoggingContext>, ILoggingContext
{
    /// <inheritdoc />
    protected LoggingContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    public static DbConnectionStringBuilder DefaultConnectionStringBuilder =>
        new SqliteConnectionStringBuilder(@"Data Source=resources/logging.db");

    public DbSet<RequestLog> RequestLogs { get; set; }

    public DbSet<UserActivityHistory> UserActivityHistory { get; set; }

    public DbSet<ChatHistory> ChatHistory { get; set; }

    public DbSet<TradeHistory> TradeHistory { get; set; }

    public DbSet<GuildHistory> GuildHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new RequestLog.Mapper());
    }

    public override void Seed()
    {
#if DEBUG
        new SeedTrades().SeedIfEmpty(this);
        ChangeTracker.DetectChanges();
        SaveChanges();
#endif
    }
}

internal sealed partial class LoggingContextInterface : ContextInterface<ILoggingContext>, ILoggingContext
{
    /// <inheritdoc />
    public LoggingContextInterface(ILoggingContext context) : base(context)
    {
    }

    #region Implementation of ILoggingContext

    /// <inheritdoc />
    public DbSet<RequestLog> RequestLogs => Context.RequestLogs;

    /// <inheritdoc />
    public DbSet<UserActivityHistory> UserActivityHistory => Context.UserActivityHistory;

    /// <inheritdoc />
    public DbSet<ChatHistory> ChatHistory => Context.ChatHistory;

    /// <inheritdoc />
    public DbSet<TradeHistory> TradeHistory => Context.TradeHistory;

    /// <inheritdoc />
    public DbSet<GuildHistory> GuildHistory => Context.GuildHistory;

    /// <inheritdoc />
    public void Seed() => Context.Seed();

    #endregion
}
