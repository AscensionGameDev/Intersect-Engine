using System;
using System.Collections.Generic;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Logging
{
    public class Logger
    {
        private List<ILogOutput> mOutputs;

        public Logger(string tag = null)
        {
            mOutputs = new List<ILogOutput>();

#if DEBUG
            LogLevel = LogLevel.All;
#else
            LogLevel = LogLevel.Info;
#endif

            Tag = (string.IsNullOrEmpty(tag) ? null : tag);
        }

        public List<ILogOutput> Outputs
        {
            get
            {
                if (mOutputs == null)
                {
                    mOutputs = new List<ILogOutput>();
                }

                return new List<ILogOutput>(mOutputs);
            }
        }

        public LogLevel LogLevel { get; set; }

        public string Tag { get; set; }

        public List<ILogOutput> GetOutputs()
        {
            return Outputs;
        }

        public bool AddOutput(ILogOutput output)
        {
            if (output != null && !mOutputs.Contains(output))
            {
                mOutputs.Add(output);
                return true;
            }

            return false;
        }

        public bool RemoveOutput(ILogOutput output)
        {
            if (output != null && mOutputs.Contains(output))
            {
                return mOutputs.Remove(output);
            }

            return false;
        }

        public void Write(LogLevel logLevel, string message)
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

        public void Write(LogLevel logLevel, string format, params object[] args)
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

        public void Write(LogLevel logLevel, Exception exception, string message = null)
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

        public void Write(string message)
        {
            Write(LogLevel, message);
        }

        public void Write(string format, params object[] args)
        {
            Write(LogLevel, format, args);
        }

        public void Write(Exception exception, string message = null)
        {
            Write(LogLevel, exception, message);
        }

        public void All(string message)
        {
            Write(LogLevel.All, message);
        }

        public void All(string format, params object[] args)
        {
            Write(LogLevel.All, format, args);
        }

        public void All(Exception exception, string message = null)
        {
            Write(LogLevel.All, exception, message);
        }

        public void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        public void Error(string format, params object[] args)
        {
            Write(LogLevel.Error, format, args);
        }

        public void Error(Exception exception, string message = null)
        {
            Write(LogLevel.Error, exception, message);
        }

        public void Warn(string message)
        {
            Write(LogLevel.Warn, message);
        }

        public void Warn(string format, params object[] args)
        {
            Write(LogLevel.Warn, format, args);
        }

        public void Warn(Exception exception, string message = null)
        {
            Write(LogLevel.Warn, exception, message);
        }

        public void Info(string message)
        {
            Write(LogLevel.Info, message);
        }

        public void Info(string format, params object[] args)
        {
            Write(LogLevel.Info, format, args);
        }

        public void Info(Exception exception, string message = null)
        {
            Write(LogLevel.Info, exception, message);
        }

        public void Trace(string message)
        {
            Write(LogLevel.Trace, message);
        }

        public void Trace(string format, params object[] args)
        {
            Write(LogLevel.Trace, format, args);
        }

        public void Trace(Exception exception, string message = null)
        {
            Write(LogLevel.Trace, exception, message);
        }

        public void Debug(string message)
        {
            Write(LogLevel.Debug, message);
        }

        public void Debug(string format, params object[] args)
        {
            Write(LogLevel.Debug, format, args);
        }

        public void Debug(Exception exception, string message = null)
        {
            Write(LogLevel.Debug, exception, message);
        }

        public void Diagnostic(string message)
        {
#if INTERSECT_DIAGNOSTIC
            Write(LogLevel.Diagnostic, message);
#endif
        }

        public void Diagnostic(string format, params object[] args)
        {
#if INTERSECT_DIAGNOSTIC
            Write(LogLevel.Diagnostic, format, args);
#endif
        }

        public void Diagnostic(Exception exception, string message = null)
        {
#if INTERSECT_DIAGNOSTIC
            Write(LogLevel.Diagnostic, exception, message);
#endif
        }

        public void Verbose(string message)
        {
            Write(LogLevel.Verbose, message);
        }

        public void Verbose(string format, params object[] args)
        {
            Write(LogLevel.Verbose, format, args);
        }

        public void Verbose(Exception exception, string message = null)
        {
            Write(LogLevel.Verbose, exception, message);
        }
    }
}