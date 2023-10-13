using Intersect.Config;

namespace Intersect.Server.Database.Logging;

/// <summary>
/// SQLite-specific implementation of <see cref="LoggingContext"/>
/// </summary>
public sealed class SqliteLoggingContext : LoggingContext, ISqliteDbContext
{
    /// <inheritdoc />
    public SqliteLoggingContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    public override DatabaseType DatabaseType => DatabaseType.Sqlite;
}