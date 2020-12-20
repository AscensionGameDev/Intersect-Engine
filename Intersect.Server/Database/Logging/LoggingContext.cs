using Intersect.Config;
using Intersect.Server.Database.Logging.Entities;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Intersect.Server.Database.Logging
{
    internal sealed class LoggingContext : IntersectDbContext<LoggingContext>, ILoggingContext
    {
        public LoggingContext() : this(DefaultConnectionStringBuilder) { }

        /// <inheritdoc />
        public LoggingContext(
            DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType = DatabaseOptions.DatabaseType.SQLite
        ) : base(connectionStringBuilder, databaseType, true)
        {
        }

        public static DbConnectionStringBuilder DefaultConnectionStringBuilder =>
            new SqliteConnectionStringBuilder(@"Data Source=resources/logging.db");

        public DbSet<RequestLog> RequestLogs { get; set; }

        public DbSet<UserActivityHistory> UserActivityHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RequestLog.Mapper());
        }
    }

    internal sealed class LoggingContextInterface : ContextInterface<ILoggingContext>, ILoggingContext
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

        #endregion
    }
}
