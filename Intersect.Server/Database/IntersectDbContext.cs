using System.Data;
using System.Data.Common;

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
    public abstract partial class IntersectDbContext<TDbContext> : DbContext, ISeedableContext where TDbContext : IntersectDbContext<TDbContext>
    {
        private static DbConnectionStringBuilder _fallbackConnectionStringBuilder;

        private static DatabaseOptions.DatabaseType _fallbackDatabaseType = DatabaseOptions.DatabaseType.SQLite;

        private static ILoggerFactory _loggerFactory;

        /// <summary>
        /// 
        /// </summary>
        public DbConnectionStringBuilder ConnectionStringBuilder { get; }

        /// <summary>
        /// 
        /// </summary>
        public DatabaseOptions.DatabaseType DatabaseType { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool ReadOnly { get; }

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
            DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType
        )
        {
            _fallbackConnectionStringBuilder = connectionStringBuilder;
            _fallbackDatabaseType = databaseType;
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
                    optionsBuilder.UseLoggerFactory(_loggerFactory).UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options => options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(12), null)).UseQueryTrackingBehavior(ReadOnly ? QueryTrackingBehavior.NoTracking : QueryTrackingBehavior.TrackAll);

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
