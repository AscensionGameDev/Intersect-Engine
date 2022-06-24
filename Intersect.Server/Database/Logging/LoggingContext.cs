using Intersect.Config;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Database.Logging.Seed;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using System.Data.Common;

namespace Intersect.Server.Database.Logging
{
    internal sealed partial class LoggingContext : IntersectDbContext<LoggingContext>, ILoggingContext
    {
        public LoggingContext() : this(DefaultConnectionStringBuilder) { }

        /// <inheritdoc />
        public LoggingContext(
            DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType = DatabaseOptions.DatabaseType.SQLite
        ) : this(connectionStringBuilder, databaseType, false)
        {
        }

        /// <inheritdoc />
        public LoggingContext(
            DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType,
            bool readOnly = false
        ) : base(connectionStringBuilder, databaseType, null, Intersect.Logging.LogLevel.None, readOnly, true)
        {
        }

        public static DbConnectionStringBuilder DefaultConnectionStringBuilder =>
            new SqliteConnectionStringBuilder(@"Data Source=resources/logging.db");

        public DbSet<RequestLog> RequestLogs { get; set; }

        public DbSet<UserActivityHistory> UserActivityHistory { get; set; }

        public DbSet<ChatHistory> ChatHistory { get; set; }

        public DbSet<TradeHistory> TradeHistory { get; set; }

        public DbSet<GuildHistory> GuildHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RequestLog.Mapper());
        }

        public void Seed()
        {
#if DEBUG
            new SeedTrades().SeedIfEmpty(this);
            ChangeTracker.DetectChanges();
            SaveChanges();
#endif
        }
    }

    internal sealed partial class LoggingContextInterface : ContextInterface<ILoggingContext>, ILoggingContext
    {
        /// <inheritdoc />
        public LoggingContextInterface(ILoggingContext context) : base(context)
        {
        }

        #region Implementation of ILoggingContext

        /// <inheritdoc />
        public DbSet<RequestLog> RequestLogs => Context.RequestLogs;

        /// <inheritdoc />
        public DbSet<UserActivityHistory> UserActivityHistory => Context.UserActivityHistory;

        /// <inheritdoc />
        public DbSet<ChatHistory> ChatHistory => Context.ChatHistory;

        /// <inheritdoc />
        public DbSet<TradeHistory> TradeHistory => Context.TradeHistory;

        /// <inheritdoc />
        public DbSet<GuildHistory> GuildHistory => Context.GuildHistory;

        /// <inheritdoc />
        public void Seed() => Context.Seed();

        #endregion
    }
}
