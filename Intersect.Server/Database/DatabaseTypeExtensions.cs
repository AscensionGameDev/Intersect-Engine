
using Intersect.Config;

using SqlKata.Compilers;

namespace Intersect.Server.Database;

internal static class DatabaseTypeExtensions
{
    public static Compiler CreateQueryCompiler(this DatabaseOptions.DatabaseType databaseType) =>
        databaseType switch
        {
            DatabaseOptions.DatabaseType.SQLite => new SqliteCompiler(),
            DatabaseOptions.DatabaseType.MySQL => new MySqlCompiler(),
            _ => throw new NotImplementedException(),
        };
}
