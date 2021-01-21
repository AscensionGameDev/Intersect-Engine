using Intersect.Logging;
using Intersect.Logging.Output;
using Intersect.Plugins.Interfaces;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Intersect.Plugins.Helpers
{
    /// <inheritdoc />
    internal sealed class LoggingHelper : ILoggingHelper
    {
        private static readonly string BasePluginLogPath = Path.Combine(
            "plugins", $"{Log.Initial:yyyy_MM_dd-HH_mm_ss_fff}"
        );

        private static Logger CreateLogger(IManifestHelper manifest, CreateLoggerOptions createLoggerOptions)
        {
            var logName = string.IsNullOrWhiteSpace(createLoggerOptions.Name)
                ? manifest.Key
                : $"{manifest.Key}.{createLoggerOptions.Name}";

            var outputs = new List<ILogOutput>();

            if (createLoggerOptions.File > LogLevel.None)
            {
                outputs.Add(
                    new FileOutput(Path.Combine(BasePluginLogPath, $"{logName}.log"), createLoggerOptions.File)
                );
            }

            if (createLoggerOptions.Console > LogLevel.None)
            {
                outputs.Add(new ConciseConsoleOutput(createLoggerOptions.Console));
            }

            var immutableOutputs = outputs.ToImmutableList();
            Debug.Assert(immutableOutputs != null, $"{nameof(immutableOutputs)} != null");

            return new Logger(
                new LogConfiguration
                {
                    LogLevel = LogConfiguration.Default.LogLevel,
                    Outputs = immutableOutputs
                }
            );
        }

        private readonly IManifestHelper mManifest;

        /// <inheritdoc />
        public Logger Application { get; }

        public Logger Plugin { get; }

        internal LoggingHelper(Logger applicationLogger, IManifestHelper manifest)
        {
            mManifest = manifest;

            Application = applicationLogger;
            Plugin = CreateLogger(
                manifest, new CreateLoggerOptions
                {
                    Console = Debugger.IsAttached ? LogLevel.Debug : LogLevel.None,
                    File = LogLevel.Info
                }
            );
        }

        /// <inheritdoc />
        public Logger CreateLogger(CreateLoggerOptions createLoggerOptions) =>
            CreateLogger(mManifest, createLoggerOptions);
    }
}
