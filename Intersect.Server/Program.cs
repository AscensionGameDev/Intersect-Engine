using System.Data.Common;
using System.Globalization;
using System.Reflection;
using Intersect.Config;
using Intersect.Framework.Reflection;
using Intersect.Network;
using Intersect.Server.Core;
using Intersect.Server.Database;
using Intersect.Server.Networking;
using Intersect.Server.Networking.LiteNetLib;
using Microsoft.Data.Sqlite;
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
            var executingAssembly = Assembly.GetExecutingAssembly();
            Console.WriteLine($"Starting {executingAssembly.GetMetadataName()}...");

            ServerContext.ServerContextFactory = (options, logger, packetHelper) =>
                new FullServerContext(executingAssembly, options, logger, packetHelper);

            ServerContext.NetworkFactory = (context, parameters, handlePacket, shouldProcessPacket) =>
            {
                var config = new NetworkConfiguration(Options.Instance.ServerPort);
                return new ServerNetwork(context as IServerContext, context, config, parameters)
                {
                    Handler = handlePacket,
                    PreProcessHandler = shouldProcessPacket,
                };
            };

            Client.EnqueueNetworkTask = action => ServerNetwork.Pool.QueueWorkItem(action);

            Bootstrapper.Start(executingAssembly, args, ExtractWwwroot);
        }
        catch (Exception exception)
        {
            ServerContext.DispatchUnhandledException(exception.InnerException ?? exception);
        }
    }

    private static void ExtractWwwroot(ServerCommandLineOptions options)
    {
        var assembly = typeof(Program).Assembly;
        var wwwrootResourceNames = assembly.FindMatchingResources("wwwroot");
        var workingDirectory = options.WorkingDirectory;
        if (string.IsNullOrWhiteSpace(workingDirectory))
        {
            workingDirectory = Environment.CurrentDirectory;
        }

        foreach (var wwwrootResourceName in wwwrootResourceNames)
        {
            var resolvedOutputPath = Path.Combine(workingDirectory, wwwrootResourceName);
            FileInfo outputFileInfo = new(resolvedOutputPath);
            if (outputFileInfo.Exists)
            {
                // Don't overwrite existing files
                continue;
            }

            using var manifestResourceStream = assembly.GetManifestResourceStream(wwwrootResourceName);
            if (manifestResourceStream == null)
            {
                // TODO: Pre-context logging
                continue;
            }

            DirectoryInfo outputFileDirectoryInfo = outputFileInfo.Directory;
            if (outputFileDirectoryInfo is { Exists: false })
            {
                outputFileDirectoryInfo.Create();
            }

            using var outputFileStream = outputFileInfo.OpenWrite();
            manifestResourceStream.CopyTo(outputFileStream);
        }
    }

    /// <summary>
    /// Host builder method for Entity Framework Design Tools to use when generating migrations.
    /// </summary>
    public static IHostBuilder CreateHostBuilder(string[] args) => Host
        .CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            var rawDatabaseType = hostContext.Configuration.GetValue(
                "DatabaseType",
                DatabaseType.Sqlite.ToString()
            );
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