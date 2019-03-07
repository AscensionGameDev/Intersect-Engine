using System;

using JetBrains.Annotations;

namespace Intersect.Logging
{
    public interface ILogOutput
    {
        LogLevel LogLevel { get; set; }

        void Write([NotNull] LoggerConfiguration configuration, LogLevel logLevel, string message);
        void Write([NotNull] LoggerConfiguration configuration, LogLevel logLevel, string format, params object[] args);
        void Write([NotNull] LoggerConfiguration configuration, LogLevel logLevel, Exception exception, string message);
    }
}