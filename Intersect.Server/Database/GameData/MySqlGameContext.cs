using Intersect.Config;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.GameData;

/// <summary>
/// MySQL/MariaDB-specific implementation of <see cref="GameContext"/>
/// </summary>
public sealed class MySqlGameContext : GameContext, IMySqlDbContext
{
    /// <inheritdoc />
    public MySqlGameContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    public override DatabaseType DatabaseType => DatabaseType.MySql;

    protected override void OnConfiguringProvider(DbContextOptionsBuilder optionsBuilder, string connectionString) =>
        this.ConfigureOptionsBuilder(optionsBuilder, connectionString);
}