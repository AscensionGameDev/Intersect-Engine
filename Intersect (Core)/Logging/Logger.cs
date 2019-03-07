using System;

using JetBrains.Annotations;

namespace Intersect.Logging
{

    public class Logger : ILogger
    {

        public Logger() : this(LogConfiguration.Default) { }

        public Logger(string tag = null) : this(
            new LogConfiguration {Tag = string.IsNullOrEmpty(tag?.Trim()) ? null : tag}
        )
        {
        }

        public Logger([NotNull] LogConfiguration configuration)
        {
            Configuration = configuration;
        }

        [NotNull]
        public LogConfiguration Configuration { get; set; }

        public virtual void Write(LogLevel logLevel, string message)
        {
            if (Configuration.LogLevel < logLevel)
            {
                return;
            }

            foreach (var output in Configuration.Outputs)
            {
                output.Write(Configuration, logLevel, message);
            }
        }

        public virtual void Write(LogLevel logLevel, string format, params object[] args)
        {
            if (Configuration.LogLevel < logLevel)
            {
                return;
            }

            foreach (var output in Configuration.Outputs)
            {
                output.Write(Configuration, logLevel, format, args);
            }
        }

        public virtual void Write(LogLevel logLevel, Exception exception, string message = null)
        {
            if (Configuration.LogLevel < logLevel)
            {
                return;
            }

            foreach (var output in Configuration.Outputs)
            {
                output.Write(Configuration, logLevel, exception, message);
            }
        }

        public virtual void Write(string message)
        {
            Write(Configuration.LogLevel, message);
        }

        public virtual void Write(string format, params object[] args)
        {
            Write(Configuration.LogLevel, format, args);
        }

        public virtual void Write(Exception exception, string message = null)
        {
            Write(Configuration.LogLevel, exception, message);
        }

        public virtual void All(string message)
        {
            Write(LogLevel.All, message);
        }

        public virtual void All(string format, params object[] args)
        {
            Write(LogLevel.All, format, args);
        }

        public virtual void All(Exception exception, string message = null)
        {
            Write(LogLevel.All, exception, message);
        }

        public virtual void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        public virtual void Error(string format, params object[] args)
        {
            Write(LogLevel.Error, format, args);
        }

        public virtual void Error(Exception exception, string message = null)
        {
            Write(LogLevel.Error, exception, message);
        }

        public virtual void Warn(string message)
        {
            Write(LogLevel.Warn, message);
        }

        public virtual void Warn(string format, params object[] args)
        {
            Write(LogLevel.Warn, format, args);
        }

        public virtual void Warn(Exception exception, string message = null)
        {
            Write(LogLevel.Warn, exception, message);
        }

        public virtual void Info(string message)
        {
            Write(LogLevel.Info, message);
        }

        public virtual void Info(string format, params object[] args)
        {
            Write(LogLevel.Info, format, args);
        }

        public virtual void Info(Exception exception, string message = null)
        {
            Write(LogLevel.Info, exception, message);
        }

        public virtual void Trace(string message)
        {
            Write(LogLevel.Trace, message);
        }

        public virtual void Trace(string format, params object[] args)
        {
            Write(LogLevel.Trace, format, args);
        }

        public virtual void Trace(Exception exception, string message = null)
        {
            Write(LogLevel.Trace, exception, message);
        }

        public virtual void Debug(string message)
        {
            Write(LogLevel.Debug, message);
        }

        public virtual void Debug(string format, params object[] args)
        {
            Write(LogLevel.Debug, format, args);
        }

        public virtual void Debug(Exception exception, string message = null)
        {
            Write(LogLevel.Debug, exception, message);
        }

        public virtual void Diagnostic(string message)
        {
#if INTERSECT_DIAGNOSTIC
            Write(LogLevel.Diagnostic, message);
#endif
        }

        public virtual void Diagnostic(string format, params object[] args)
        {
#if INTERSECT_DIAGNOSTIC
            Write(LogLevel.Diagnostic, format, args);
#endif
        }

        public virtual void Diagnostic(Exception exception, string message = null)
        {
#if INTERSECT_DIAGNOSTIC
            Write(LogLevel.Diagnostic, exception, message);
#endif
        }

        public virtual void Verbose(string message)
        {
            Write(LogLevel.Verbose, message);
        }

        public virtual void Verbose(string format, params object[] args)
        {
            Write(LogLevel.Verbose, format, args);
        }

        public virtual void Verbose(Exception exception, string message = null)
        {
            Write(LogLevel.Verbose, exception, message);
        }

    }

}
