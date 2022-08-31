using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Intersect.Config;
using Intersect.Server.Database.GameData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Intersect.Server.Core
{
    /// <summary>
    /// Please do not modify this without JC's approval! If namespaces are referenced that are not SYSTEM.* then the server won't run cross platform.
    /// If you want to add startup instructions see Classes/ServerStart.cs
    /// </summary>
    public static partial class Program
    {

        [STAThread]
        public static void Main(string[] args)
        {
            Debugger.Launch();
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            try
            {
                Type.GetType("Intersect.Server.Core.Bootstrapper")
                    ?.GetMethod("Start")
                    ?.Invoke(null, new object[] { args });
            }
            catch (Exception exception)
            {
                var type = Type.GetType("Intersect.Server.Core.ServerContext", true);
                Debug.Assert(type != null, "type != null");

                var staticMethodInfo = type.GetMethod("DispatchUnhandledException",
                    BindingFlags.Static | BindingFlags.NonPublic);
                Debug.Assert(staticMethodInfo != null, nameof(staticMethodInfo) + " != null");

                staticMethodInfo.Invoke(null, new object[] { exception.InnerException ?? exception, true });
            }
        }

        //public class BloggingContextFactory : IDesignTimeDbContextFactory<GameContext>
        //{
        //    public GameContext CreateDbContext(string[] args)
        //    {
        //        var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
        //        var connectionStringBuilder = new MySqlConnectionStringBuilder("server=127.0.0.1;user id=root;password=Password12!;port=3306;");
        //        var serverVersion = ServerVersion.AutoDetect(connectionStringBuilder.ConnectionString);
        //        optionsBuilder.UseMySql(serverVersion);

        //        return new GameContext(optionsBuilder.Options);
        //    }
        //}

        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(
                (hostContext, services) =>
                {
                    // Debugger.Break();
                    // Debugger.Launch();
                    // services.AddDbContext<MySqlGameContext>();
                    services.AddDbContext<SqliteGameContext>(
                        // options =>
                        // {
                        //     Console.WriteLine(databaseType switch
                        //     {
                        //         DatabaseType.Sqlite => "SQLite",
                        //         DatabaseType.MySql => "MySQL",
                        //         _ => throw new NotSupportedException($"Unsupported provider: {databaseTypeString}")
                        //     });
                        //     _ = databaseType switch
                        //     {
                        //         DatabaseType.Sqlite => options.UseSqlite(
                        //             optionsBuilder => optionsBuilder.MigrationsAssembly($"Intersect.Server.Migrations.{databaseType}")
                        //         ),
                        //         DatabaseType.MySql => options.UseMySql(
                        //             new MySqlServerVersion(new Version(8, 0, 31)),
                        //             optionsBuilder => optionsBuilder.MigrationsAssembly($"Intersect.Server.Migrations.{databaseType}")
                        //         ),
                        //         _ => throw new NotSupportedException($"Unsupported provider: {databaseTypeString}")
                        //     };
                        // }
                    );
                    // services.AddDbContext<GameContext, SqliteGameContext>();
                }
            );
        //     .ConfigureServices(
        //         (hostContext, services) =>
        //         {
        //             services.AddDbContext<SqliteGameContext>();
        //             // Console.WriteLine("ConfigureServices");
        //             // // Set the active provider via configuration
        //             // var configuration = hostContext.Configuration;
        //             // var databaseTypeString = configuration.GetValue("DatabaseType", "SQLite");
        //             // if (!Enum.TryParse<DatabaseType>(databaseTypeString, out var databaseType))
        //             // {
        //             //     throw new Exception($"{databaseTypeString} is not a valid DatabaseType.");
        //             // }
        //             // Console.WriteLine(databaseType);
        //             // DbConnectionStringBuilder dbConnectionStringBuilder;
        //             // DbContextOptionsBuilder OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //             // {
        //             //     Console.WriteLine(databaseType switch
        //             //     {
        //             //         DatabaseType.Sqlite => "SQLite",
        //             //         DatabaseType.MySql => "MySQL",
        //             //         _ => throw new NotSupportedException($"Unsupported provider: {databaseTypeString}")
        //             //     });
        //             //     return databaseType switch
        //             //     {
        //             //         DatabaseType.Sqlite => optionsBuilder.UseSqlite(
        //             //             optionsBuilder => optionsBuilder.MigrationsAssembly($"Intersect.Server.Migrations.{databaseType}")
        //             //         ),
        //             //         DatabaseType.MySql => optionsBuilder.UseMySql(
        //             //             new MySqlServerVersion(new Version(8, 0, 31))//,
        //             //             //optionsBuilder => optionsBuilder.MigrationsAssembly($"Intersect.Server.Migrations.{databaseType}")
        //             //         ),
        //             //         _ => throw new NotSupportedException($"Unsupported provider: {databaseTypeString}")
        //             //     };
        //             // }
        //             // GameContext.Configure(OnConfiguring);
        //             //services.AddDbContext<GameContext>(
        //             //    options =>
        //             //    {
        //             //        Console.WriteLine(databaseType switch
        //             //        {
        //             //            DatabaseType.Sqlite => "SQLite",
        //             //            DatabaseType.MySql => "MySQL",
        //             //            _ => throw new NotSupportedException($"Unsupported provider: {databaseTypeString}")
        //             //        });
        //             //        _ = databaseType switch
        //             //        {
        //             //            DatabaseType.Sqlite => options.UseSqlite(
        //             //                optionsBuilder => optionsBuilder.MigrationsAssembly($"Intersect.Server.Migrations.{databaseType}")
        //             //            ),
        //             //            DatabaseType.MySql => options.UseMySql(
        //             //                new MySqlServerVersion(new Version(8, 0, 31)),
        //             //                optionsBuilder => optionsBuilder.MigrationsAssembly($"Intersect.Server.Migrations.{databaseType}")
        //             //            ),
        //             //            _ => throw new NotSupportedException($"Unsupported provider: {databaseTypeString}")
        //             //        };
        //             //    }
        //             //);
        //         }
        //     );
    }
}
