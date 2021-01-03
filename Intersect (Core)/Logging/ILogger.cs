using System;

namespace Intersect.Logging
{

    public interface ILogger
    {

        LogConfiguration Configuration { get; set; }

        void All(string message);

        void All(string format, params object[] args);

        void All(Exception exception, string message = null);

        void Debug(string message);

        void Debug(string format, params object[] args);

        void Debug(Exception exception, string message = null);

        void Diagnostic(string message);

        void Diagnostic(string format, params object[] args);

        void Diagnostic(Exception exception, string message = null);

        void Error(string message);

        void Error(string format, params object[] args);

        void Error(Exception exception, string message = null);

        void Info(string message);

        void Info(string format, params object[] args);

        void Info(Exception exception, string message = null);

        void Trace(string message);

        void Trace(string format, params object[] args);

        void Trace(Exception exception, string message = null);

        void Verbose(string message);

        void Verbose(string format, params object[] args);

        void Verbose(Exception exception, string message = null);

        void Warn(string message);

        void Warn(string format, params object[] args);

        void Warn(Exception exception, string message = null);

        void Write(LogLevel logLevel, string message);

        void Write(LogLevel logLevel, string format, params object[] args);

        void Write(LogLevel logLevel, Exception exception, string message = null);

        void Write(string message);

        void Write(string format, params object[] args);

        void Write(Exception exception, string message = null);

    }

}
