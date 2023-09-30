using Intersect.Config;

namespace Intersect.Server.Database;

public interface ISqliteDbContext
{
    DatabaseType DatabaseType => DatabaseType.Sqlite;
}