using System;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Intersect.Config;
using Intersect.Server.Database;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;

namespace Intersect.Server.Core
{

    /// <summary>
    /// Please do not modify this without JC's approval! If namespaces are referenced that are not SYSTEM.* then the server won't run cross platform.
    /// If you want to add startup instructions see Classes/ServerStart.cs
    /// </summary>
    public static partial class Program
    {
        private const string LdLibraryPath = "LD_LIBRARY_PATH";

        [STAThread]
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            try
            {
                Type.GetType("Intersect.Server.Core.Bootstrapper")
                    ?.GetMethod("Start")
                    ?.Invoke(null, new object[] {args});
            }
            catch (Exception exception)
            {
                var type = Type.GetType("Intersect.Server.Core.ServerContext", true);
                Debug.Assert(type != null, "type != null");

                var staticMethodInfo = type.GetMethod("DispatchUnhandledException", BindingFlags.Static | BindingFlags.NonPublic);
                Debug.Assert(staticMethodInfo != null, nameof(staticMethodInfo) + " != null");

                staticMethodInfo.Invoke(null, new object[] { exception.InnerException ?? exception, true });
            }
        }

        /// <summary>
        /// Host builder method for Entity Framework Design Tools to use when generating migrations.
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var rawDatabaseType = hostContext.Configuration.GetValue<string>("DatabaseType") ??
                                      DatabaseType.Sqlite.ToString();
                if (!Enum.TryParse(rawDatabaseType, out DatabaseType databaseType) || databaseType == DatabaseType.Unknown)
                {
                    throw new InvalidOperationException($"Invalid database type: {rawDatabaseType}");
                }

                var connectionString = hostContext.Configuration.GetValue<string>("ConnectionString");
                DbConnectionStringBuilder connectionStringBuilder = databaseType switch
                {
                    DatabaseType.MySql => new MySqlConnectionStringBuilder(connectionString),
                    DatabaseType.Sqlite => new SqliteConnectionStringBuilder(connectionString),
                    DatabaseType.Unknown => throw new DatabaseTypeInvalidException(databaseType),
                    _ => throw new IndexOutOfRangeException($"Unsupported database type: {databaseType}"),
                };

                DatabaseContextOptions databaseContextOptions = new()
                {
                    ConnectionStringBuilder = connectionStringBuilder,
                    DatabaseType = databaseType,
                };

                services.AddSingleton(databaseContextOptions);

                Console.WriteLine($"Generating migration for DatabaseType={databaseType}, ConnectionString=\"{connectionString}\"...");
            });
    }
}
