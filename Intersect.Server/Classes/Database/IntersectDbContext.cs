using Intersect.Config;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using Intersect.Logging.Microsoft.Extensions.Logging;
using Intersect.Server.Classes.Database;

using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database
{
    /// <summary>
    /// Abstract DbContext class for all Intersect database contexts.
    /// </summary>
    /// <inheritdoc cref="DbContext" />
    /// <inheritdoc cref="ISeedableContext" />
    public abstract class IntersectDbContext<T>: DbContext, ISeedableContext
        where T : IntersectDbContext<T>
    {
        public static T Current { get; private set; }

        private static ILoggerFactory MsExtLoggerFactory { get; } =
            LoggerFactory.Create(builder => builder.AddConsole());

        private static DatabaseOptions.DatabaseType configuredDatabaseType = DatabaseOptions.DatabaseType.SQLite;

        private static DbConnectionStringBuilder configuredConnectionStringBuilder;

        public static void Configure(
            DatabaseOptions.DatabaseType databaseType = DatabaseOptions.DatabaseType.SQLite,
            DbConnectionStringBuilder connectionStringBuilder = null
        )
        {
            configuredDatabaseType = databaseType;
            configuredConnectionStringBuilder = connectionStringBuilder;
        }

        [NotNull]
        private static readonly IDictionary<Type, ConstructorInfo> constructorCache =
            new ConcurrentDictionary<Type, ConstructorInfo>();

        [NotNull]
        public static T Create(
            DatabaseOptions.DatabaseType? databaseType = null,
            DbConnectionStringBuilder connectionStringBuilder = null
        )
        {
            var type = typeof(T);
            if (!constructorCache.TryGetValue(type, out var constructorInfo))
            {
                constructorInfo = type.GetConstructor(new [] { typeof(DbConnectionStringBuilder), typeof(DatabaseOptions.DatabaseType) });
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

        /// <summary>
        /// 
        /// </summary>
        public DatabaseOptions.DatabaseType DatabaseType { get; }

        /// <summary>
        /// 
        /// </summary>
        [NotNull] public DbConnectionStringBuilder ConnectionStringBuilder { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStringBuilder"></param>
        /// <param name="databaseType"></param>
        /// <inheritdoc />
        protected IntersectDbContext(
            [NotNull] DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType = DatabaseOptions.DatabaseType.SQLite,
            bool isTemporary = false
            )
        {
            ConnectionStringBuilder = connectionStringBuilder;
            DatabaseType = databaseType;

            if (!isTemporary)
            {
                Current = this as T;
            }
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
                    optionsBuilder.UseSqlite(connectionString);
                    break;

                case DatabaseOptions.DatabaseType.MySQL:
                    optionsBuilder.UseMySql(connectionString);
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

        public DbSet<TType> GetDbSet<TType>() where TType : class
        {
            var searchType = typeof(DbSet<TType>);
            var property = GetType()
                .GetProperties()
                .FirstOrDefault(propertyInfo => searchType == propertyInfo.PropertyType);
            return property?.GetValue(this) as DbSet<TType>;
        }

        [NotNull]
        public ICollection<string> PendingMigrations => Database?.GetPendingMigrations()?.ToList() ?? new List<string>();

        public virtual void MigrationsProcessed([NotNull] string[] migrations) { }

    }
}