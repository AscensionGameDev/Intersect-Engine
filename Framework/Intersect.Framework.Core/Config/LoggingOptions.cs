using Intersect.Framework.Core.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Extensions.Logging;

namespace Intersect.Config;

public partial class LoggingOptions
{
    private const LogLevel DefaultLogLevel = LogLevel.Information;

    public static readonly LoggingLevelSwitch LoggingLevelSwitch = new(LevelConvert.ToSerilogLevel(DefaultLogLevel));

    private LogLevel _level = DefaultLogLevel;
    private bool _showSensitiveData;

    public bool ShowSensitiveData
    {
        get => _showSensitiveData;
        set => _showSensitiveData = value;
    }

    [JsonConverter(typeof(SafeStringEnumConverter))]
    public LogLevel Level
    {
        get => _level;
        set
        {
            if (value == _level)
            {
                return;
            }

            _level = value;
            LoggingLevelSwitch.MinimumLevel = LevelConvert.ToSerilogLevel(_level);
        }
    }

    /// <summary>
    /// Determines whether chat logs should be written into the logging database
    /// </summary>
    public bool Chat { get; set; } = true;

    /// <summary>
    /// Logs guild activity (creations, disbands, joins, leaves, kicks, promotions, transfers, etc
    /// </summary>
    public bool GuildActivity { get; set; } = true;

    /// <summary>
    /// Determines whether trades should be written into the logging database
    /// </summary>
    public bool Trade { get; set; } = true;

    /// <summary>
    /// Determines if general user activity (logins, logouts, character creations/deletions, etc) should be written into the logging database
    /// </summary>
    public bool UserActivity { get; set; } = true;
}
