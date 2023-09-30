using System.Data.Common;
using Intersect.Config;
using Intersect.Server.Localization;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using SqlKata.Compilers;

namespace Intersect.Server.Database;

internal static class DatabaseTypeExtensions
{
    public static DbConnectionStringBuilder CreateConnectionStringBuilder(
        this DatabaseType databaseType,
        DatabaseOptions databaseOptions,
        string filename
    ) => databaseType switch
    {
        DatabaseType.SQLite => new SqliteConnectionStringBuilder($"Data Source={filename}"),
        DatabaseType.MySQL => new MySqlConnectionStringBuilder
        {
            Server = databaseOptions.Server,
            Port = databaseOptions.Port,
            Database = databaseOptions.Database,
            UserID = databaseOptions.Username,
            Password = databaseOptions.Password
        },
        _ => throw new ArgumentOutOfRangeException(nameof(databaseType)),
    };

    public static Compiler CreateQueryCompiler(this DatabaseType databaseType) =>
        databaseType switch
        {
            DatabaseType.SQLite => new SqliteCompiler(),
            DatabaseType.MySQL => new MySqlCompiler(),
            _ => throw new NotImplementedException(),
        };


    public static string GetName(this DatabaseType databaseType) =>
        Strings.Database.DatabaseTypes.TryGetValue(databaseType, out var name)
            ? name
            : throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
}