using System;

namespace Intersect.Extensions
{

    public static class LogLevelExtensions
    {

        public static Logging.LogLevel AsIntersectLogLevel(this Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                    return Logging.LogLevel.Diagnostic;

                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    return Logging.LogLevel.Debug;

                case Microsoft.Extensions.Logging.LogLevel.Information:
                    return Logging.LogLevel.Info;

                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    return Logging.LogLevel.Warn;

                case Microsoft.Extensions.Logging.LogLevel.Error:
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    return Logging.LogLevel.Error;

                case Microsoft.Extensions.Logging.LogLevel.None:
                    return Logging.LogLevel.None;

                default:

                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

    }

}
