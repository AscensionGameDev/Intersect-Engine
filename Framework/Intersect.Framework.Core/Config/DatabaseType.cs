using Intersect.Framework.Annotations;

namespace Intersect.Config;

public enum DatabaseType
{
    [Ignore]
    Unknown,

    SQLite,

    Sqlite = SQLite,

    sqlite = SQLite,

    MySQL,

    MySql = MySQL,

    Mysql = MySQL,

    mysql = MySQL,

    MariaDB = MySQL,

    mariadb = MariaDB,
}