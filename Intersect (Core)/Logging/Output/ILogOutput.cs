using System;

namespace Intersect.Logging.Output
{

    public interface ILogOutput
    {

        LogLevel LogLevel { get; set; }

        void Write(LogConfiguration configuration, LogLevel logLevel, string message);

        void Write(
            LogConfiguration configuration,
            LogLevel logLevel,
            string format,
            params object[] args
        );

        void Write(
            LogConfiguration configuration,
            LogLevel logLevel,
            Exception exception,
            string message
        );

        void Write(
            LogConfiguration configuration,
            LogLevel logLevel,
            Exception exception,
            string format,
            params object[] args
        );

    }

}
