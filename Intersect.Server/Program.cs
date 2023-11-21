using System.Data.Common;
using System.Globalization;
using Intersect.Config;
using Intersect.Network;
using Intersect.Server.Core;
using Intersect.Server.Database;
using Intersect.Server.Networking;
using Intersect.Server.Networking.LiteNetLib;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySqlConnector;

namespace Intersect.Server;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

        try
        {
            ServerContext.ServerContextFactory = (options, logger, packetHelper) =>
                new FullServerContext(options, logger, packetHelper);

            ServerContext.NetworkFactory = (context, parameters, handlePacket, shouldProcessPacket) =>
            {
                var config = new NetworkConfiguration(Options.ServerPort);
                return new ServerNetwork(context as IServerContext, context, config, parameters)
                {
                    Handler = handlePacket,
                    PreProcessHandler = shouldProcessPacket,
                };
            };

            Client.EnqueueNetworkTask = action => ServerNetwork.Pool.QueueWorkItem(action);

            Bootstrapper.Start(args);
        }
        catch (Exception exception)
        {
            ServerContext.DispatchUnhandledException(exception.InnerException ?? exception);
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