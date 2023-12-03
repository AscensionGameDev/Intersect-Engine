using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using Intersect.Config;
using Intersect.Logging;
using Intersect.Reflection;
using Intersect.Server.Core;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace Intersect.Server.Database;

/// <summary>
/// Abstract DbContext class for all Intersect database contexts.
/// </summary>
/// <inheritdoc cref="DbContext" />
/// <inheritdoc cref="ISeedableContext" />
public abstract partial class IntersectDbContext<TDbContext> : DbContext, IDbContext, ISeedableContext
    where TDbContext : IntersectDbContext<TDbContext>
{
    private IReadOnlyCollection<DataMigrationMetadata>? _dataMigrations;
    private IReadOnlyCollection<DataMigrationMetadata>? _pendingDataMigrations;

    public DatabaseContextOptions ContextOptions { get; }

    public DbConnectionStringBuilder ConnectionStringBuilder => ContextOptions.ConnectionStringBuilder;

    public abstract DatabaseType DatabaseType { get; }

    public bool IsReadOnly => ContextOptions.ReadOnly;

    public bool HasPendingMigrations => PendingSchemaMigrations.Any() || PendingDataMigrations.Any();

    public IReadOnlyCollection<DataMigrationMetadata> AllDataMigrations => _dataMigrations ??= FindAllDataMigrations();

    public IReadOnlyCollection<DataMigrationMetadata> PendingDataMigrations => _pendingDataMigrations =
        FilterPendingMigrations(
            _pendingDataMigrations ??= FindPendingDataMigrations(AllDataMigrations),
            this as TDbContext
        );

    public IReadOnlyCollection<string> PendingDataMigrationNames =>
        PendingDataMigrations.Select(dataMigration => dataMigration.Name).ToList().AsReadOnly();

    public IReadOnlyCollection<string> AllSchemaMigrations => Database.GetMigrations().ToList().AsReadOnly();

    public IReadOnlyCollection<string> AppliedSchemaMigrations => Database.GetAppliedMigrations().ToList().AsReadOnly();

    public IReadOnlyCollection<string> PendingSchemaMigrations => Database.GetPendingMigrations().ToList().AsReadOnly();

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

#if DEBUG
        optionsBuilder.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
        optionsBuilder.ConfigureWarnings(w => w.Throw(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning));
        optionsBuilder.ConfigureWarnings(w => w.Throw(CoreEventId.RowLimitingOperationWithoutOrderByWarning));
        optionsBuilder.ConfigureWarnings(w => w.Throw(CoreEventId.DistinctAfterOrderByWithoutRowLimitingOperatorWarning));
        optionsBuilder.ConfigureWarnings(w => w.Throw(CoreEventId.FirstWithoutOrderByAndFilterWarning));
#else
        optionsBuilder.ConfigureWarnings(w => w.Log(RelationalEventId.MultipleCollectionIncludeWarning));
        optionsBuilder.ConfigureWarnings(w => w.Log(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning));
        optionsBuilder.ConfigureWarnings(w => w.Log(CoreEventId.RowLimitingOperationWithoutOrderByWarning));
        optionsBuilder.ConfigureWarnings(w => w.Log(CoreEventId.DistinctAfterOrderByWithoutRowLimitingOperatorWarning));
        optionsBuilder.ConfigureWarnings(w => w.Log(CoreEventId.FirstWithoutOrderByAndFilterWarning));
#endif

        var loggerFactory = ContextOptions.LoggerFactory;
#if DIAGNOSTIC
        if (this is PlayerContext)
        {
            loggerFactory ??= new IntersectLoggerFactory();
        }
#endif

        var enableSensitiveDataLogging = ContextOptions.EnableSensitiveDataLogging;
#if DIAGNOSTIC
        enableSensitiveDataLogging = this is PlayerContext;
#endif

        var enableDetailedErrors = ContextOptions.EnableDetailedErrors;
#if DIAGNOSTIC
        enableDetailedErrors = this is PlayerContext;
#endif

        _ = optionsBuilder
            .EnableDetailedErrors(enableDetailedErrors)
            .EnableSensitiveDataLogging(enableSensitiveDataLogging)
            .ReplaceService<IModelCacheKeyFactory, IntersectModelCacheKeyFactory>()
            .UseLoggerFactory(loggerFactory)
            .UseQueryTrackingBehavior(queryTrackingBehavior);

        var connectionString = ConnectionStringBuilder.ConnectionString;
        switch (DatabaseType)
        {
            case DatabaseType.SQLite:
                optionsBuilder.UseSqlite(connectionString);
                break;

            case DatabaseType.Unknown:
                throw new NotSupportedException();

            case DatabaseType.MySQL:
            default:
                OnConfiguringProvider(optionsBuilder, connectionString);
                break;
        }
    }

    protected virtual void OnConfiguringProvider(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        throw new NotImplementedException($"Unimplemented DatabaseType: {DatabaseType}");
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        switch (DatabaseType)
        {
            case DatabaseType.MySql:
                configurationBuilder.Properties<Guid>().HaveColumnType("char(36)").UseCollation("ascii_general_ci");
                break;
            case DatabaseType.SQLite:
                break;
            case DatabaseType.Unknown:
                throw new DatabaseTypeInvalidException(DatabaseType);
            default:
                throw new UnreachableException();
        }
    }

    /// <summary>
    /// Checks if the database is empty by checking if there are any tables.
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            var connection = Database.GetDbConnection();
            using var command = connection.CreateCommand();
            command.CommandText = DatabaseType switch
            {
                DatabaseType.SQLite => "SELECT name FROM sqlite_master WHERE type='table';",
                DatabaseType.MySQL => "show tables;",
                DatabaseType.Unknown => throw new DatabaseTypeInvalidException(DatabaseType),
                _ => throw new UnreachableException(),
            };

            command.CommandType = CommandType.Text;

            Database.OpenConnection();

            using var result = command.ExecuteReader();
            return !result.HasRows;
        }
    }

    public virtual void OnSchemaMigrationsProcessed(string[] migrations) { }

    public virtual void Seed() { }

    public override int SaveChanges()
    {
        if (IsReadOnly)
        {

            throw new InvalidOperationException("Cannot save changes on a read only context!");
        }

        return base.SaveChanges();
    }

#if DEBUG
        private int _saveCounter = 0;
#endif

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("Cannot save changes on a read only context!");
        }

#if DEBUG
            var currentExecutionId = _saveCounter++;
#endif

        try
        {
#if DEBUG
            Log.Debug($"DBOP-A SaveChanges({acceptAllChangesOnSuccess}) #{currentExecutionId}");
#endif

            var rowsChanged = base.SaveChanges(acceptAllChangesOnSuccess);

#if DEBUG
            Log.Debug($"DBOP-B SaveChanges({acceptAllChangesOnSuccess}) #{currentExecutionId}");
#endif

            return rowsChanged;
        }
        catch (DbUpdateConcurrencyException concurrencyException)
        {
            var concurrencyErrors = new StringBuilder();
            var userIds = concurrencyException.Entries.Select(entry => GetUserIdFrom(entry.Entity))
                .Where(id => id != default)
                .Distinct()
                .ToArray();
            if (userIds.Length == 1)
            {
                concurrencyErrors.AppendLine($"This failure was wholly caused by user {userIds.First()}");
            }
            else
            {
                concurrencyErrors.AppendLine($"Involved users: {string.Join(", ", userIds)}");
            }

            foreach (var entry in concurrencyException.Entries)
            {
                concurrencyErrors.AppendLine(
                    $"[{entry.State}] {entry.Entity.GetFullishName()} ({entry.GetFullishName()})"
                );

                var userId = GetUserIdFrom(entry.Entity);
                if (userId != default)
                {
                    concurrencyErrors.AppendLine($"User Id {userId}");
                }

                var proposedValues = entry.CurrentValues;
                var databaseValues = entry.GetDatabaseValues();

                foreach (var property in proposedValues.Properties)
                {
                    concurrencyErrors.AppendLine(
                        string.Join(
                            ' ',
                            $"\t{property.Name:propertyNameColumnSize} (Token: {property.IsConcurrencyToken}):",
                            $"Proposed: {proposedValues[property]}",
                            $"Original Value: {entry.OriginalValues[property]}",
                            $"Database Value: {(databaseValues?[property] ?? "null")}"
                        )
                    );
                }

                concurrencyErrors.AppendLine();
            }

#if DEBUG
            var suffix = $"#{currentExecutionId}";
#else
            var suffix = string.Empty;
#endif
            var entityTypeNames = ChangeTracker.Entries()
                .Select(entry => entry.Entity.GetType().Name)
                .Distinct()
                .ToArray();

            concurrencyErrors.AppendLine(Environment.StackTrace);

            Log.Error(concurrencyException, $"Jackpot! Concurrency Bug For {string.Join(", ", entityTypeNames)} {suffix}");
            Log.Error(concurrencyErrors.ToString());

#if DEBUG
            Log.Debug($"DBOP-C SaveChanges({acceptAllChangesOnSuccess}) #{currentExecutionId}");
#endif

            if (ContextOptions.KillServerOnConcurrencyException)
            {
                ServerContext.DispatchUnhandledException(
                    new Exception($"Failed to save {this.GetFullishName()}, shutting down to prevent rollbacks!")
                );
            }
            else
            {
                Log.Error($"{nameof(DbUpdateConcurrencyException)} occurred, please review the logs for more information.");
            }

            return -1;

            static Guid GetUserIdFrom(object entry)
            {
                return entry switch
                {
                    IPlayerOwned playerOwned => playerOwned.Player?.User?.Id ?? playerOwned.Player?.UserId ?? default,
                    Player player => player.User?.Id ?? player.UserId,
                    _ => default,
                };
            }
        }
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

    protected virtual void StopTracking(EntityEntry entityEntry)
    {
        entityEntry.State = EntityState.Detached;
    }

    public void StopTrackingExcept<TEntity>(params TEntity[] entities)
    {
        foreach (var entityEntry in ChangeTracker.Entries().ToArray())
        {
            if (entityEntry.State == EntityState.Detached)
            {
                continue;
            }

            if (entityEntry.Entity is not TEntity other)
            {
                continue;
            }

            if (entities.Any(entity => entity.Equals(other)))
            {
                continue;
            }

            StopTracking(entityEntry);
        }
    }
}
