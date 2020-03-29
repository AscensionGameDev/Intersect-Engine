using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using Intersect.Config;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database
{

    /// <summary>
    /// Abstract DbContext class for all Intersect database contexts.
    /// </summary>
    /// <inheritdoc cref="DbContext" />
    /// <inheritdoc cref="ISeedableContext" />
    public abstract class IntersectDbContext<T> : DbContext, ISeedableContext where T : IntersectDbContext<T>
    {

        [NotNull] private static readonly IDictionary<Type, ConstructorInfo> constructorCache =
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
            [NotNull] DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType = DatabaseOptions.DatabaseType.SQLite,
            bool isTemporary = false,
            Intersect.Logging.Logger dbLogger = null,
            Intersect.Logging.LogLevel logLevel = Intersect.Logging.LogLevel.None
        )
        {
            ConnectionStringBuilder = connectionStringBuilder;
            DatabaseType = databaseType;

            //Translate Intersect.Logging.LogLevel into LoggerFactory Log Level
            if (dbLogger != null && logLevel > Intersect.Logging.LogLevel.None)
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
                        builder.AddFilter((level) => level >= efLogLevel).AddProvider(new DbLoggerProvider(dbLogger));
                    }
                );
            }

            if (!isTemporary)
            {
                Current = this as T;
            }
        }

        public static T Current { get; private set; }

        private static ILoggerFactory MsExtLoggerFactory { get; } =
            LoggerFactory.Create(builder => builder.AddConsole());

        /// <summary>
        /// 
        /// </summary>
        public DatabaseOptions.DatabaseType DatabaseType { get; }

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public DbConnectionStringBuilder ConnectionStringBuilder { get; }

        [NotNull]
        public ICollection<string> PendingMigrations =>
            Database?.GetPendingMigrations()?.ToList() ?? new List<string>();

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

        [NotNull]
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

            if (!(constructorInfo.Invoke(
                new object[]
                {
                    connectionStringBuilder ?? configuredConnectionStringBuilder,
                    databaseType ?? configuredDatabaseType
                }
            ) is T contextInstance))
            {
                throw new InvalidOperationException();
            }

            return contextInstance;
        }

        protected override void OnConfiguring([NotNull] DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var connectionString = ConnectionStringBuilder.ToString();

            //optionsBuilder.UseLoggerFactory(MsExtLoggerFactory);

            optionsBuilder.EnableSensitiveDataLogging(true);
            switch (DatabaseType)
            {
                case DatabaseOptions.DatabaseType.SQLite:
                    optionsBuilder.UseLoggerFactory(loggerFactory).UseSqlite(connectionString);

                    break;

                case DatabaseOptions.DatabaseType.MySQL:
                    optionsBuilder.UseLoggerFactory(loggerFactory).UseMySql(connectionString);

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(DatabaseType));
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

        public virtual void MigrationsProcessed([NotNull] string[] migrations) { }

    }

}
