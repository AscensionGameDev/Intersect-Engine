using System.Data.Common;

using Intersect.Config;

using JetBrains.Annotations;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.Logging
{

    public class LoggingContext : IntersectDbContext<LoggingContext>
    {

        public LoggingContext() : this(DefaultConnectionStringBuilder) { }

        /// <inheritdoc />
        public LoggingContext(
            [NotNull] DbConnectionStringBuilder connectionStringBuilder,
            DatabaseOptions.DatabaseType databaseType = DatabaseOptions.DatabaseType.SQLite
        ) : base(connectionStringBuilder, databaseType, true)
        {
        }

        [NotNull]
        public static DbConnectionStringBuilder DefaultConnectionStringBuilder =>
            new SqliteConnectionStringBuilder(@"Data Source=resources/logging.db");

        [NotNull]
        public DbSet<RequestLog> RequestLogs { get; set; }

        protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RequestLog.Mapper());
        }

    }

}
