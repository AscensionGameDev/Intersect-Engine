using Intersect.Logging;
using Intersect.Logging.Formatting;

namespace Intersect.Plugins.Interfaces;

/// <summary>
/// Configuration options for creating <see cref="Microsoft.Extensions.Logging.Logger{T}"/>s.
/// </summary>
public partial struct CreateLoggerOptions
{
    /// <summary>
    /// The minimum <see cref="LogLevel"/> for console output, set to <see cref="LogLevel.None"/> to disable.
    /// </summary>
    public LogLevel Console { get; set; }

    /// <summary>
    /// The minimum <see cref="LogLevel"/> for file output, set to <see cref="LogLevel.None"/> to disable.
    /// </summary>
    public LogLevel File { get; set; }

    /// <summary>
    /// The custom formatters to use for output from the created <see cref="Microsoft.Extensions.Logging.Logger{T}"/>.
    /// </summary>
    public IReadOnlyList<ILogFormatter> Formatters { get; set; }

    /// <summary>
    /// The name of the created <see cref="Microsoft.Extensions.Logging.Logger{T}"/>.
    /// </summary>
    public string Name { get; set; }
}