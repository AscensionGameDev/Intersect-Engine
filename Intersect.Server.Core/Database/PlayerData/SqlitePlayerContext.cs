using Intersect.Config;

namespace Intersect.Server.Database.PlayerData;

/// <summary>
/// SQLite-specific implementation of <see cref="PlayerContext"/>
/// </summary>
public sealed class SqlitePlayerContext : PlayerContext, ISqliteDbContext
{
    /// <inheritdoc />
    public SqlitePlayerContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    public override DatabaseType DatabaseType => DatabaseType.Sqlite;
}