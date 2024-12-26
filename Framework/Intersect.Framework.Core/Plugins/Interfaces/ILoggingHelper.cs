using Intersect.Logging;

namespace Intersect.Plugins.Interfaces;

/// <summary>
/// Defines the API for accessing the logging system.
/// </summary>
public interface ILoggingHelper
{
    /// <summary>
    /// The <see cref="Microsoft.Extensions.Logging.Logger{T}"/> instance for the entire application.
    /// </summary>
    ILogger Application { get; }

    /// <summary>
    /// The <see cref="Microsoft.Extensions.Logging.Logger{T}"/> instance for the active plugin.
    /// </summary>
    ILogger Plugin { get; }

    /// <summary>
    /// Creates specialized <see cref="Microsoft.Extensions.Logging.Logger{T}"/>s for the active plugin.
    /// </summary>
    /// <param name="createLoggerOptions">options to configure the <see cref="Microsoft.Extensions.Logging.Logger{T}"/></param>
    /// <returns>a specialized <see cref="Microsoft.Extensions.Logging.Logger{T}"/> instance</returns>
    ILogger CreateLogger(CreateLoggerOptions createLoggerOptions);
}