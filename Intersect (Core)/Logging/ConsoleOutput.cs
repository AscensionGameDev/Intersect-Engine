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

        public void Write(string tag, LogLevel logLevel, string message)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            var line = string.IsNullOrEmpty(tag)
                ? $"{DateTime.UtcNow.ToString(TIMESTAMP_FORMAT)} [{logLevel}] {message}"
                : $"{DateTime.UtcNow.ToString(TIMESTAMP_FORMAT)} [{logLevel}] {tag}: {message}";

            if (LogLevel < LogLevel.Info)
            {
                Console.Error.WriteLine(line);
            }
            else
            {
                Console.Out.WriteLine(line);
            }
        }

        public void Write(string tag, LogLevel logLevel, [NotNull] string format,
             [NotNull] params object[] args)
            => Write(tag, logLevel, string.Format(format, args));

        public void Write(string tag, LogLevel logLevel, Exception exception, string message = null)
        {
            Write(tag, logLevel, $"Message: {exception?.Message}");
            Write(tag, logLevel, $"Stack Trace: {exception?.StackTrace}");

            if (exception?.InnerException != null)
            {
                Write(tag, logLevel, $"Stack Trace: {exception.InnerException?.StackTrace}");
            }

            Write(tag, logLevel, $"Time: {DateTime.UtcNow}");

            if (!string.IsNullOrEmpty(message))
            {
                Write(tag, logLevel, $"Note: {message}");
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