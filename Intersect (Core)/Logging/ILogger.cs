using System;

using JetBrains.Annotations;

namespace Intersect.Logging
{

    public interface ILogger
    {

        [NotNull]
        LogConfiguration Configuration { get; set; }

        void All([NotNull] string message);

        void All([NotNull] string format, params object[] args);

        void All(Exception exception, string message = null);

        void Debug([NotNull] string message);

        void Debug([NotNull] string format, params object[] args);

        void Debug(Exception exception, string message = null);

        void Diagnostic([NotNull] string message);

        void Diagnostic([NotNull] string format, params object[] args);

        void Diagnostic(Exception exception, string message = null);

        void Error([NotNull] string message);

        void Error([NotNull] string format, params object[] args);

        void Error(Exception exception, string message = null);

        void Info([NotNull] string message);

        void Info([NotNull] string format, params object[] args);

        void Info(Exception exception, string message = null);

        void Trace([NotNull] string message);

        void Trace([NotNull] string format, params object[] args);

        void Trace(Exception exception, string message = null);

        void Verbose([NotNull] string message);

        void Verbose([NotNull] string format, params object[] args);

        void Verbose(Exception exception, string message = null);

        void Warn([NotNull] string message);

        void Warn([NotNull] string format, params object[] args);

        void Warn(Exception exception, string message = null);

        void Write(LogLevel logLevel, [NotNull] string message);

        void Write(LogLevel logLevel, [NotNull] string format, params object[] args);

        void Write(LogLevel logLevel, Exception exception, string message = null);

        void Write([NotNull] string message);

        void Write([NotNull] string format, params object[] args);

        void Write(Exception exception, string message = null);

    }

}
