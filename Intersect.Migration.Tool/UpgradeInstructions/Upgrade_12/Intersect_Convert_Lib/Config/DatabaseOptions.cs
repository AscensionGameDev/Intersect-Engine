using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Config
{
    public class DatabaseOptions
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum DatabaseType
        {
            sqlite,
            mysql,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public DatabaseType Type { get; set; } = DatabaseType.sqlite;
        public string Server { get; set; } = "localhost";
        public int Port { get; set; } = 3306;
        public string Database { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
