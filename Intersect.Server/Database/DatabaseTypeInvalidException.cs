using Intersect.Config;

namespace Intersect.Server.Database;

public class DatabaseTypeInvalidException : ArgumentOutOfRangeException
{
    public DatabaseTypeInvalidException(DatabaseType databaseType) : base(
        nameof(databaseType),
        databaseType,
        $"{nameof(DatabaseType.Unknown)} is not a valid {nameof(DatabaseType)}."
    )
    {
    }
}