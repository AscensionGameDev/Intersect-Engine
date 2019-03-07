using System;
using JetBrains.Annotations;

namespace Intersect.Logging
{
    public class ConciseConsoleOutput : ILogOutput
    {
        private const string TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public ConciseConsoleOutput(LogLevel logLevel = LogLevel.All)
        {
            LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; set; }

        public void Write(LoggerConfiguration configuration, LogLevel logLevel, string message)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            var prefix = configuration.Pretty ? "" : $"{DateTime.UtcNow.ToString(TIMESTAMP_FORMAT)} [{logLevel}]";
            var line = string.IsNullOrEmpty(configuration.Tag)
                ? $"{prefix} {message}"
                : $"{prefix} {configuration.Tag}: {message}";

            if (LogLevel < LogLevel.Info)
            {
                Console.Error.WriteLine(line.TrimStart());
            }
            else
            {
                Console.Out.WriteLine(line.TrimStart());
            }
        }

        public void Write(LoggerConfiguration configuration, LogLevel logLevel, [NotNull] string format,
             [NotNull] params object[] args)
            => Write(configuration, logLevel, string.Format(format, args));

        public void Write(LoggerConfiguration configuration, LogLevel logLevel, Exception exception, string message = null)
        {
            Write(configuration, logLevel, $"Time: {DateTime.UtcNow}");

            if (string.IsNullOrWhiteSpace(message))
            {
                Write(configuration, logLevel, $"Message: {exception?.Message}");
                Write(configuration, logLevel, $"Stack Trace: {exception?.StackTrace}");

                if (exception?.InnerException != null)
                {
                    Write(configuration, logLevel, $"Stack Trace: {exception.InnerException?.StackTrace}");
                }
            }
            else
            {
                Write(configuration, logLevel, $"Note: {message}");
                Write(configuration, logLevel, @"See logs for more information.");
            }

            Flush();
        }

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