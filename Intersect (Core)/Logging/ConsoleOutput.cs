using System;

using JetBrains.Annotations;

namespace Intersect.Logging
{

    public class ConsoleOutput : ILogOutput
    {

        private const string TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public ConsoleOutput(LogLevel logLevel = LogLevel.All)
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

        public void Write(
            LoggerConfiguration configuration,
            LogLevel logLevel,
            [NotNull] string format,
            [NotNull] params object[] args
        ) => Write(configuration, logLevel, string.Format(format, args));

        public void Write(
            LoggerConfiguration configuration,
            LogLevel logLevel,
            Exception exception,
            string message = null
        )
        {
            Write(configuration, logLevel, $"Message: {exception?.Message}");
            Write(configuration, logLevel, $"Stack Trace: {exception?.StackTrace}");

            if (exception?.InnerException != null)
            {
                Write(configuration, logLevel, $"Stack Trace: {exception.InnerException?.StackTrace}");
            }

            Write(configuration, logLevel, $"Time: {DateTime.UtcNow}");

            if (!string.IsNullOrEmpty(message))
            {
                Write(configuration, logLevel, $"Note: {message}");
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
