using System;

using JetBrains.Annotations;

namespace Intersect.Logging
{
    public sealed class LoggerConfiguration
    {

        // TODO: Add formatters and outputs to LoggerConfiguration

        [NotNull]
        public static LoggerConfiguration Default => new LoggerConfiguration
        {
#if DEBUG
            LogLevel = LogLevel.All,
#else
            LogLevel = LogLevel.Info,
#endif
            Pretty = false,

            Tag = null
        };

        public LogLevel LogLevel { get; set; }

        public bool Pretty { get; set; }

        public string Tag { get; set; }

        public LoggerConfiguration()
        {
            Pretty = false;
            Tag = null;
        }

        [NotNull]
        internal LoggerConfiguration Clone()
        {
            return MemberwiseClone() as LoggerConfiguration ?? throw new InvalidOperationException();
        }
    }
}
