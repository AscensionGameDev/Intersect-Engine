
using Intersect.Config;

namespace Intersect.Server.Database;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class DbContextProviderAttribute : Attribute
{
    public DatabaseType DatabaseType { get; }

    public DbContextProviderAttribute(DatabaseType databaseType) =>
        DatabaseType = databaseType;
}
