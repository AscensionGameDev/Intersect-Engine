using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Intersect.Config
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
