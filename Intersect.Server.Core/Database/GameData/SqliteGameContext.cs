using Intersect.Config;

namespace Intersect.Server.Database.GameData;

/// <summary>
/// SQLite-specific implementation of <see cref="GameContext"/>
/// </summary>
public sealed class SqliteGameContext : GameContext, ISqliteDbContext
{
    /// <inheritdoc />
    public SqliteGameContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    public override DatabaseType DatabaseType => DatabaseType.Sqlite;
}