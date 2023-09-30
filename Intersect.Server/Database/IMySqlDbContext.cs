using Intersect.Config;

namespace Intersect.Server.Database;

public interface IMySqlDbContext : IDbContext
{
    DatabaseType DatabaseType => DatabaseType.MySql;
}