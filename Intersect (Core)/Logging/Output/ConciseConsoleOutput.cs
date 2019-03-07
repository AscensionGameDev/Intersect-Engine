using System;
using JetBrains.Annotations;

namespace Intersect.Logging.Output
{
    public class ConciseConsoleOutput : ILogOutput
    {
        private const string TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public ConciseConsoleOutput(LogLevel logLevel = LogLevel.All)
        {
            LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; set; }

        private void InternalWrite(LogConfiguration configuration, LogLevel logLevel, Exception exception, [NotNull] string format, params object[] args)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            var writer = Console.Out;
            if (LogLevel < LogLevel.Info)
            {
                writer = Console.Error;
            }

            if (string.IsNullOrEmpty(format))
            {
                writer.WriteLine(configuration.Formatter.Format(configuration, logLevel, DateTime.UtcNow, null, exception.Message));
            }
            else
            {
                writer.WriteLine(configuration.Formatter.Format(configuration, logLevel, DateTime.UtcNow, null, format, args));
                writer.WriteLine(configuration.Formatter.Format(configuration, logLevel, DateTime.UtcNow, null, @"See logs for more information."));
            }

            writer.Flush();
        }

        public void Write(LogConfiguration configuration, LogLevel logLevel, string message)
            => InternalWrite(configuration, LogLevel, null, message);

        public void Write(LogConfiguration configuration, LogLevel logLevel, string format, params object[] args)
            => InternalWrite(configuration, LogLevel, null, format, args);

        public void Write(LogConfiguration configuration, LogLevel logLevel, Exception exception, string message)
            => InternalWrite(configuration, LogLevel, exception, message);

        public void Write(LogConfiguration configuration, LogLevel logLevel, Exception exception, string format, params object[] args)
            => InternalWrite(configuration, LogLevel, exception, format, args);

        private static void Flush()
        {
            Console.Error.Flush();
            Console.Out.Flush();
        }

        public void Close()
        {
            Flush();
        }
    }
}