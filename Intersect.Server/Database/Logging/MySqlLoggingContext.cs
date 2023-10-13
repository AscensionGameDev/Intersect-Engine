using Intersect.Config;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.Logging;

/// <summary>
/// MySQL/MariaDB-specific implementation of <see cref="LoggingContext"/>
/// </summary>
public sealed class MySqlLoggingContext : LoggingContext, IMySqlDbContext
{
    /// <inheritdoc />
    public MySqlLoggingContext(DatabaseContextOptions databaseContextOptions) : base(databaseContextOptions) { }

    public override DatabaseType DatabaseType => DatabaseType.MySql;

    protected override void OnConfiguringProvider(DbContextOptionsBuilder optionsBuilder, string connectionString) =>
        this.ConfigureOptionsBuilder(optionsBuilder, connectionString);
}