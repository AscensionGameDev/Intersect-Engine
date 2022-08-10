using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Reflection;

using Intersect.Config;
using Intersect.Framework;
using Intersect.Server.Database.Converters;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database
{
    /// <summary>
    /// Abstract DbContext class for all Intersect database contexts.
    /// </summary>
    /// <inheritdoc cref="DbContext" />
    /// <inheritdoc cref="ISeedableContext" />
    public abstract partial class IntersectDbContext<T> : DbContext, ISeedableContext where T : IntersectDbContext<T>
    {
        private static readonly IDictionary<Type, ConstructorInfo> constructorCache =
            new ConcurrentDictionary<Type, ConstructorInfo>();

        private static DbConnectionStringBuilder configuredConnectionStringBuilder;

        private static DatabaseOptions.DatabaseType configuredDatabaseType = DatabaseOptions.DatabaseType.SQLite;

        private static ILoggerFactory loggerFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStringBuilder"></param>
        /// <param name="databaseType"></param>
        /// <inheritdoc />
        protected IntersectDbContext(
            DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType = DatabaseOptions.DatabaseType.SQLite,
            Intersect.Logging.Logger logger = null,
            Intersect.Logging.LogLevel logLevel = Intersect.Logging.LogLevel.None,
            bool readOnly = false,
            bool explicitLoad = false,
            bool autoDetectChanges = true,
            bool lazyLoading = false,
            QueryTrackingBehavior? queryTrackingBehavior = default
        )
        {
            ConnectionStringBuilder = connectionStringBuilder;
            DatabaseType = databaseType;

            //Translate Intersect.Logging.LogLevel into LoggerFactory Log Level
            if (loggerFactory == null && logger != null && logLevel > Intersect.Logging.LogLevel.None)
            {
                var efLogLevel = LogLevel.None;
                switch (logLevel)
                {
                    case Intersect.Logging.LogLevel.None:
                        break;

                    case Intersect.Logging.LogLevel.Error:
                        efLogLevel = LogLevel.Error;

                        break;

                    case Intersect.Logging.LogLevel.Warn:
                        efLogLevel = LogLevel.Warning;

                        break;

                    case Intersect.Logging.LogLevel.Info:
                        efLogLevel = LogLevel.Information;

                        break;

                    case Intersect.Logging.LogLevel.Trace:
                        efLogLevel = LogLevel.Trace;

                        break;

                    case Intersect.Logging.LogLevel.Verbose:
                        efLogLevel = LogLevel.Trace;

                        break;

                    case Intersect.Logging.LogLevel.Debug:
                        efLogLevel = LogLevel.Debug;

                        break;

                    case Intersect.Logging.LogLevel.Diagnostic:
                        efLogLevel = LogLevel.Trace;

                        break;

                    case Intersect.Logging.LogLevel.All:
                        efLogLevel = LogLevel.Trace;

                        break;
                }

                loggerFactory = LoggerFactory.Create(
                    builder =>
                    {
                        builder.AddFilter((level) => level >= efLogLevel).AddProvider(new DbLoggerProvider(logger));
                    }
                );
            }

            ReadOnly = readOnly;

            if (queryTrackingBehavior != default)
            {
                ChangeTracker.QueryTrackingBehavior = queryTrackingBehavior.Value;
            }

            ChangeTracker.AutoDetectChangesEnabled = autoDetectChanges || ReadOnly;

            if (ReadOnly && !explicitLoad)
            {
                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }

            ChangeTracker.LazyLoadingEnabled = lazyLoading || !ReadOnly;
        }

        private static ILoggerFactory MsExtLoggerFactory { get; } =
            LoggerFactory.Create(builder => builder.AddConsole());

        /// <summary>
        /// 
        /// </summary>
        public DatabaseOptions.DatabaseType DatabaseType { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool ReadOnly { get; }

        /// <summary>
        /// 
        /// </summary>
        public DbConnectionStringBuilder ConnectionStringBuilder { get; }

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

        public static void Configure(
            DatabaseOptions.DatabaseType databaseType = DatabaseOptions.DatabaseType.SQLite,
            DbConnectionStringBuilder connectionStringBuilder = null
        )
        {
            configuredDatabaseType = databaseType;
            configuredConnectionStringBuilder = connectionStringBuilder;
        }

        public static T Create(
            DatabaseOptions.DatabaseType? databaseType = null,
            DbConnectionStringBuilder connectionStringBuilder = null
        )
        {
            var type = typeof(T);
            if (!constructorCache.TryGetValue(type, out var constructorInfo))
            {
                constructorInfo = type.GetConstructor(
                    new[] {typeof(DbConnectionStringBuilder), typeof(DatabaseOptions.DatabaseType)}
                );

                constructorCache[type] = constructorInfo;
            }

            if (constructorInfo == null)
            {
                throw new InvalidOperationException(@"Missing IntersectDbContext constructor.");
            }

            if (constructorInfo.Invoke(
                new object[]
                {
                    connectionStringBuilder ?? configuredConnectionStringBuilder,
                    databaseType ?? configuredDatabaseType
                }
            ) is not T contextInstance)
            {
                throw new InvalidOperationException();
            }

            return contextInstance;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var connectionString = ConnectionStringBuilder.ToString();

            //optionsBuilder.UseLoggerFactory(MsExtLoggerFactory);

            optionsBuilder.EnableSensitiveDataLogging(true);
            switch (DatabaseType)
            {
                case DatabaseOptions.DatabaseType.SQLite:
                    //optionsBuilder.UseLoggerFactory(loggerFactory);
                    optionsBuilder.UseSqlite(connectionString).UseQueryTrackingBehavior(ReadOnly ? QueryTrackingBehavior.NoTracking : QueryTrackingBehavior.TrackAll);

                    break;

                case DatabaseOptions.DatabaseType.MySQL:
                    optionsBuilder.UseLoggerFactory(loggerFactory).UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options => options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(12), null)).UseQueryTrackingBehavior(ReadOnly ? QueryTrackingBehavior.NoTracking : QueryTrackingBehavior.TrackAll);

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(DatabaseType));
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            var idConverterGenericType = typeof(IdGuidConverter<>);

            switch (DatabaseType)
            {
                case DatabaseOptions.DatabaseType.SQLite:
                    configurationBuilder
                        .Properties<Guid>()
                        .HaveConversion<GuidBinaryConverter>();
                    idConverterGenericType = typeof(IdBinaryConverter<>);
                    break;
            }

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
                    case DatabaseOptions.DatabaseType.SQLite:
                        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";

                        break;

                    case DatabaseOptions.DatabaseType.MySQL:
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
            if (ReadOnly)
                throw new InvalidOperationException("Cannot save changes on a read only context!");

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            if (ReadOnly)
                throw new InvalidOperationException("Cannot save changes on a read only context!");

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            if (ReadOnly)
                throw new InvalidOperationException("Cannot save changes on a read only context!");
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (ReadOnly)
                throw new InvalidOperationException("Cannot save changes on a read only context!");

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
