using Intersect.Config;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.PlayerData;

/// <summary>
/// SQLite-specific implementation of <see cref="PlayerContext"/>
/// </summary>
public sealed class SqlitePlayerContext : PlayerContext, ISqliteDbContext
{
    /// <inheritdoc />
    public SqlitePlayerContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    public override DatabaseType DatabaseType => DatabaseType.Sqlite;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().Property(u => u.Name).UseCollation("NOCASE");
        modelBuilder.Entity<User>().Property(u => u.Email).UseCollation("NOCASE");
        modelBuilder.Entity<Player>().Property(u => u.Name).UseCollation("NOCASE");
        modelBuilder.Entity<Guild>().Property(u => u.Name).UseCollation("NOCASE");
    }
}