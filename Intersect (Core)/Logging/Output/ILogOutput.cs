using System;

using JetBrains.Annotations;

namespace Intersect.Logging.Output
{

    public interface ILogOutput
    {

        LogLevel LogLevel { get; set; }

        void Write([NotNull] LogConfiguration configuration, LogLevel logLevel, [NotNull] string message);

        void Write(
            [NotNull] LogConfiguration configuration,
            LogLevel logLevel,
            [NotNull] string format,
            [NotNull] params object[] args
        );

        void Write(
            [NotNull] LogConfiguration configuration,
            LogLevel logLevel,
            [NotNull] Exception exception,
            [NotNull] string message
        );

        void Write(
            [NotNull] LogConfiguration configuration,
            LogLevel logLevel,
            [NotNull] Exception exception,
            [NotNull] string format,
            [NotNull] params object[] args
        );

    }

}
