using System;

namespace Intersect.Logging.Output
{

    public class ConsoleOutput : ILogOutput
    {

        public ConsoleOutput(LogLevel logLevel = LogLevel.All)
        {
            LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; set; }

        public void Write(LogConfiguration configuration, LogLevel logLevel, string message)
        {
            InternalWrite(configuration, logLevel, null, message);
        }

        public void Write(LogConfiguration configuration, LogLevel logLevel, string format, params object[] args)
        {
            InternalWrite(configuration, logLevel, null, format, args);
        }

        public void Write(LogConfiguration configuration, LogLevel logLevel, Exception exception, string message)
        {
            InternalWrite(configuration, logLevel, exception, message);
        }

        public void Write(
            LogConfiguration configuration,
            LogLevel logLevel,
            Exception exception,
            string format,
            params object[] args
        )
        {
            InternalWrite(configuration, logLevel, exception, format, args);
        }

        protected void InternalWrite(
            LogConfiguration configuration,
            LogLevel logLevel,
            Exception exception,
            string format,
            params object[] args
        )
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

            writer.Write(
                configuration.Formatter.Format(configuration, logLevel, DateTime.UtcNow, exception, format, args)
            );

            writer.Flush();
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
