using System.Data;
using System.Data.Common;
using Intersect.Config;
using Intersect.Framework;
using Intersect.Server.Database.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database;

/// <summary>
/// Abstract DbContext class for all Intersect database contexts.
/// </summary>
/// <inheritdoc cref="DbContext" />
/// <inheritdoc cref="ISeedableContext" />
public abstract partial class IntersectDbContext<TDbContext> : DbContext, ISeedableContext
    where TDbContext : IntersectDbContext<TDbContext>
{
    private static DatabaseContextOptions _fallbackContextOptions = new();

    internal Type ContextType => typeof(TDbContext);

    public DatabaseContextOptions ContextOptions { get; }

    public DbConnectionStringBuilder ConnectionStringBuilder => ContextOptions.ConnectionStringBuilder;

    public DatabaseType DatabaseType => ContextOptions.DatabaseType;

    public bool IsReadOnly => ContextOptions.ReadOnly;

    public IReadOnlyCollection<string> AllMigrations =>
        (Database?.GetMigrations()?.ToList() ?? new())
        .AsReadOnly();

    public IReadOnlyCollection<string> AppliedMigrations =>
        (Database?.GetAppliedMigrations()?.ToList() ?? new())
        .AsReadOnly();

    public IReadOnlyCollection<string> PendingMigrations =>
        (Database?.GetPendingMigrations()?.ToList() ?? new())
        .AsReadOnly();

    public DbSet<TType> GetDbSet<TType>() where TType : class
    {
        var searchType = typeof(DbSet<TType>);
        var property = GetType()
            .GetProperties()
            .FirstOrDefault(propertyInfo => searchType == propertyInfo.PropertyType);

        return property?.GetValue(this) as DbSet<TType>;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var queryTrackingBehavior = ContextOptions.QueryTrackingBehavior ??
                                    (ContextOptions.ReadOnly && !ContextOptions.ExplicitLoad
                                        ? QueryTrackingBehavior.NoTracking
                                        : QueryTrackingBehavior.TrackAll);
        _ = optionsBuilder
            .EnableDetailedErrors(ContextOptions.EnableDetailedErrors)
            .EnableSensitiveDataLogging(ContextOptions.EnableSensitiveDataLogging)
            .ReplaceService<IModelCacheKeyFactory, IntersectModelCacheKeyFactory>()
            .UseLoggerFactory(ContextOptions.LoggerFactory)
            .UseQueryTrackingBehavior(queryTrackingBehavior);

        var connectionString = ConnectionStringBuilder.ConnectionString;
        switch (DatabaseType)
        {
            case DatabaseType.SQLite:
                optionsBuilder.UseSqlite(connectionString);
                break;

            case DatabaseType.MySQL:
                optionsBuilder.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    options => options
                        .EnableRetryOnFailure(
                            5,
                            TimeSpan.FromSeconds(12),
                            default
                        )
                );
                break;

            default:
                throw new IndexOutOfRangeException($"Invalid DatabaseType: {DatabaseType}");
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        var idConverterGenericType = typeof(IdGuidConverter<>);

        var idTypes = Id<object>.FindDerivedTypes();
        foreach (var idType in idTypes)
        {
            configurationBuilder
                .Properties(idType)
                .HaveConversion(idConverterGenericType.MakeGenericType(idType.GenericTypeArguments));
        }
    }

    /// <summary>
    /// Checks if the database is empty by checking if there are any tables.
    /// </summary>
    /// <returns>if the database is empty</returns>
    public bool IsEmpty()
    {
        var connection = Database?.GetDbConnection();
        if (connection == null)
        {
            throw new InvalidOperationException("Cannot get connection to the database.");
        }

        using (var command = connection.CreateCommand())
        {
            switch (DatabaseType)
            {
                case DatabaseType.SQLite:
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";

                    break;

                case DatabaseType.MySQL:
                    command.CommandText = "show tables;";

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(DatabaseType));
            }

            command.CommandType = CommandType.Text;

            Database.OpenConnection();

            using (var result = command.ExecuteReader())
            {
                return !(result?.HasRows ?? false);
            }
        }
    }

    public virtual void MigrationsProcessed(string[] migrations) { }

    public virtual void Seed() { }

    public override int SaveChanges()
    {
        if (IsReadOnly)
            throw new InvalidOperationException("Cannot save changes on a read only context!");

        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        if (IsReadOnly)
            throw new InvalidOperationException("Cannot save changes on a read only context!");

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        if (IsReadOnly)
            throw new InvalidOperationException("Cannot save changes on a read only context!");
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (IsReadOnly)
            throw new InvalidOperationException("Cannot save changes on a read only context!");

        return base.SaveChangesAsync(cancellationToken);
    }
}
