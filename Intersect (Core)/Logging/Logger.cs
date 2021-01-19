using System;
using System.Runtime.CompilerServices;

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

        public Logger(LogConfiguration configuration)
        {
            Configuration = configuration;
        }

        public LogConfiguration Configuration { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Write(LogLevel logLevel, string message)
        {
            if (Configuration.LogLevel < logLevel)
            {
                return;
            }

            string trace = null;
            if (logLevel == LogLevel.Trace)
            {
                trace = Environment.StackTrace;
            }

            foreach (var output in Configuration.Outputs)
            {
                output.Write(Configuration, logLevel, message);
                if (trace != null)
                {
                    output.Write(Configuration, logLevel, trace);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Write(LogLevel logLevel, string format, params object[] args)
        {
            if (Configuration.LogLevel < logLevel)
            {
                return;
            }

            string trace = null;
            if (logLevel == LogLevel.Trace)
            {
                trace = Environment.StackTrace;
            }

            foreach (var output in Configuration.Outputs)
            {
                output.Write(Configuration, logLevel, format, args);
                if (trace != null)
                {
                    output.Write(Configuration, logLevel, trace);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Write(LogLevel logLevel, Exception exception, string message = null)
        {
            if (Configuration.LogLevel < logLevel)
            {
                return;
            }

            string trace = null;
            if (logLevel == LogLevel.Trace)
            {
                trace = Environment.StackTrace;
            }

            foreach (var output in Configuration.Outputs)
            {
                output.Write(Configuration, logLevel, exception, message);
                if (trace != null)
                {
                    output.Write(Configuration, logLevel, trace);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Write(string message)
        {
            Write(Configuration.LogLevel, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Write(string format, params object[] args)
        {
            Write(Configuration.LogLevel, format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Write(Exception exception, string message = null)
        {
            Write(Configuration.LogLevel, exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void All(string message)
        {
            Write(LogLevel.All, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void All(string format, params object[] args)
        {
            Write(LogLevel.All, format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void All(Exception exception, string message = null)
        {
            Write(LogLevel.All, exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Error(string format, params object[] args)
        {
            Write(LogLevel.Error, format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Error(Exception exception, string message = null)
        {
            Write(LogLevel.Error, exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Warn(string message)
        {
            Write(LogLevel.Warn, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Warn(string format, params object[] args)
        {
            Write(LogLevel.Warn, format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Warn(Exception exception, string message = null)
        {
            Write(LogLevel.Warn, exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Info(string message)
        {
            Write(LogLevel.Info, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Info(string format, params object[] args)
        {
            Write(LogLevel.Info, format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Info(Exception exception, string message = null)
        {
            Write(LogLevel.Info, exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Trace(string message)
        {
            Write(LogLevel.Trace, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Trace(string format, params object[] args)
        {
            Write(LogLevel.Trace, format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Trace(Exception exception, string message = null)
        {
            Write(LogLevel.Trace, exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Debug(string message)
        {
            Write(LogLevel.Debug, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Debug(string format, params object[] args)
        {
            Write(LogLevel.Debug, format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Debug(Exception exception, string message = null)
        {
            Write(LogLevel.Debug, exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Diagnostic(string message)
        {
#if INTERSECT_DIAGNOSTIC
            Write(LogLevel.Diagnostic, message);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Diagnostic(string format, params object[] args)
        {
#if INTERSECT_DIAGNOSTIC
            Write(LogLevel.Diagnostic, format, args);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Diagnostic(Exception exception, string message = null)
        {
#if INTERSECT_DIAGNOSTIC
            Write(LogLevel.Diagnostic, exception, message);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Verbose(string message)
        {
            Write(LogLevel.Verbose, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Verbose(string format, params object[] args)
        {
            Write(LogLevel.Verbose, format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Verbose(Exception exception, string message = null)
        {
            Write(LogLevel.Verbose, exception, message);
        }

    }

}
