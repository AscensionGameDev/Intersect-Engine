using System.Collections.Generic;

using Intersect.Logging;
using Intersect.Logging.Formatting;

namespace Intersect.Plugins.Interfaces
{
    /// <summary>
    /// Defines the API for accessing the logging system.
    /// </summary>
    public interface ILoggingHelper
    {
        /// <summary>
        /// The <see cref="Logger"/> instance for the entire application.
        /// </summary>
        Logger Application { get; }

        /// <summary>
        /// The <see cref="Logger"/> instance for the active plugin.
        /// </summary>
        Logger Plugin { get; }

        /// <summary>
        /// Creates specialized <see cref="Logger"/>s for the active plugin.
        /// </summary>
        /// <param name="createLoggerOptions">options to configure the <see cref="Logger"/></param>
        /// <returns>a specialized <see cref="Logger"/> instance</returns>
        Logger CreateLogger(CreateLoggerOptions createLoggerOptions);
    }

    /// <summary>
    /// Configuration options for creating <see cref="Logger"/>s.
    /// </summary>
    public struct CreateLoggerOptions
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
        /// The custom formatters to use for output from the created <see cref="Logger"/>.
        /// </summary>
        public IReadOnlyList<ILogFormatter> Formatters { get; set; }

        /// <summary>
        /// The name of the created <see cref="Logger"/>.
        /// </summary>
        public string Name { get; set; }
    }
}
