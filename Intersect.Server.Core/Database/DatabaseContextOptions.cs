using System.Data.Common;
using Intersect.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database;

public record DatabaseContextOptions
{
    public bool AutoDetectChanges { get; init; }

    public DbConnectionStringBuilder ConnectionStringBuilder { get; init; }

    public DatabaseType DatabaseType { get; init; }

    public bool DisableAutoInclude { get; set; }

    public bool EnableDetailedErrors { get; init; }

    public bool EnableSensitiveDataLogging { get; init; }

    public bool ExplicitLoad { get; init; }

    public bool KillServerOnConcurrencyException { get; init; }

    public bool LazyLoading { get; init; }

    public ILoggerFactory? LoggerFactory { get; init; }

    public QueryTrackingBehavior? QueryTrackingBehavior { get; init; }

    public bool ReadOnly { get; init; }
}