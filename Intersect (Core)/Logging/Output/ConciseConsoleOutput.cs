using System;

namespace Intersect.Logging.Output
{
    // TODO: Figure out what doesn't need to be duplicated between this and ConsoleOutput
    public class ConciseConsoleOutput : ILogOutput
    {

        public ConciseConsoleOutput(LogLevel logLevel = LogLevel.All)
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

        private void InternalWrite(
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

            if (string.IsNullOrEmpty(format))
            {
                writer.Write(configuration.Formatter.Format(configuration, logLevel, DateTime.UtcNow, exception, null));
            }
            else
            {
                writer.Write(
                    configuration.Formatter.Format(configuration, logLevel, DateTime.UtcNow, null, format, args)
                );

                if (exception != null)
                {
                    writer.Write(
                        configuration.Formatter.Format(
                            configuration, logLevel, DateTime.UtcNow, null, @"See logs for more information."
                        )
                    );
                }
            }

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
