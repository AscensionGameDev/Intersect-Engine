using Intersect.Config;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.PlayerData;

/// <summary>
/// MySQL/MariaDB-specific implementation of <see cref="PlayerContext"/>
/// </summary>
public sealed class MySqlPlayerContext : PlayerContext, IMySqlDbContext
{
    /// <inheritdoc />
    public MySqlPlayerContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    public override DatabaseType DatabaseType => DatabaseType.MySql;

    protected override void OnConfiguringProvider(DbContextOptionsBuilder optionsBuilder, string connectionString) =>
        this.ConfigureOptionsBuilder(optionsBuilder, connectionString);
}