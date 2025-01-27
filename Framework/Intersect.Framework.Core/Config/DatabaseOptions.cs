using System.ComponentModel;
using Intersect.Framework.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Intersect.Config;

[RequiresRestart]
public partial class DatabaseOptions
{
#if DEBUG
    private const bool DefaultKillServerOnConcurrencyException = true;
#else
    private const bool DefaultKillServerOnConcurrencyException = false;
#endif

    [JsonConverter(typeof(StringEnumConverter))]
    public DatabaseType Type { get; set; } = DatabaseType.SQLite;

    /// <summary>
    /// The database hostname, assumed to be <c>localhost</c> if not specified.
    /// </summary>
    public string? Server { get; set; }

    public ushort Port { get; set; } = 3306;

    public string? Database { get; set; }

    /// <summary>
    /// The username to connect to the database with, assumed to be <c>root</c> if not specified.
    /// </summary>
    public string? Username { get; set; }

    [Password]
    [PasswordPropertyText(password: true)]
    public string? Password { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Populate
    )]
    [DefaultValue(Microsoft.Extensions.Logging.LogLevel.Error)]
    public Microsoft.Extensions.Logging.LogLevel LogLevel { get; set; } = Microsoft.Extensions.Logging.LogLevel.Error;

    public bool KillServerOnConcurrencyException { get; set; } = DefaultKillServerOnConcurrencyException;
}
