using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Intersect.Logging
{
    public class Logger
    {
        public Logger(string tag = null)
        {
            InternalOutputs = new List<ILogOutput>();

#if DEBUG
            LogLevel = LogLevel.All;
#else
            LogLevel = LogLevel.Info;
#endif

            Tag = (string.IsNullOrEmpty(tag) ? null : tag);
        }

        [NotNull]
        protected IList<ILogOutput> InternalOutputs { get; }

        [NotNull]
        public virtual IList<ILogOutput> Outputs => InternalOutputs.ToImmutableList() ?? throw new InvalidOperationException();

        public LogLevel LogLevel { get; set; }

        public string Tag { get; set; }

        public virtual bool AddOutput([NotNull] ILogOutput output)
        {
            if (InternalOutputs.Contains(output))
            {
                return false;
            }

            InternalOutputs.Add(output);
            return true;

        }

        public virtual bool RemoveOutput([NotNull] ILogOutput output)
        {
            return InternalOutputs.Contains(output) && InternalOutputs.Remove(output);
        }

        public virtual void Write(LogLevel logLevel, string message)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            foreach (var output in Outputs)
            {
                output.Write(Tag, logLevel, message);
            }
        }

        public virtual void Write(LogLevel logLevel, string format, params object[] args)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            foreach (var output in Outputs)
            {
                output.Write(Tag, logLevel, format, args);
            }
        }

        public virtual void Write(LogLevel logLevel, Exception exception, string message = null)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            foreach (var output in Outputs)
            {
                output.Write(Tag, logLevel, exception, message);
            }
        }

        public virtual void Write(string message)
        {
            Write(LogLevel, message);
        }

        public virtual void Write(string format, params object[] args)
        {
            Write(LogLevel, format, args);
        }

        public virtual void Write(Exception exception, string message = null)
        {
            Write(LogLevel, exception, message);
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