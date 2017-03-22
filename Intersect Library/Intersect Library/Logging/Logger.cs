using System;
using System.Collections.Generic;

namespace Intersect_Library.Logging
{
    public class Logger
    {
        private List<ILogOutput> mOutputs;
        public List<ILogOutput> Outputs
        {
            get {
                if (mOutputs == null)
                {
                    mOutputs = new List<ILogOutput>();
                }

                return new List<ILogOutput>(mOutputs);
            }
        }

        public LogLevel LogLevel { get; set; }

        public string Tag { get; set; }

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

        public void Write(LogLevel logLevel, Exception exception)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            foreach (var output in Outputs)
            {
                output.Write(Tag, logLevel, exception);
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

        public void Write(Exception exception)
        {
            Write(LogLevel, exception);
        }

        public void All(string message)
        {
            Write(LogLevel.All, message);
        }

        public void All(string format, params object[] args)
        {
            Write(LogLevel.All, format, args);
        }

        public void All(Exception exception)
        {
            Write(LogLevel.All, exception);
        }

        public void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        public void Error(string format, params object[] args)
        {
            Write(LogLevel.Error, format, args);
        }

        public void Error(Exception exception)
        {
            Write(LogLevel.Error, exception);
        }

        public void Warn(string message)
        {
            Write(LogLevel.Warn, message);
        }

        public void Warn(string format, params object[] args)
        {
            Write(LogLevel.Warn, format, args);
        }

        public void Warn(Exception exception)
        {
            Write(LogLevel.Warn, exception);
        }

        public void Info(string message)
        {
            Write(LogLevel.Info, message);
        }

        public void Info(string format, params object[] args)
        {
            Write(LogLevel.Info, format, args);
        }

        public void Info(Exception exception)
        {
            Write(LogLevel.Info, exception);
        }

        public void Trace(string message)
        {
            Write(LogLevel.Trace, message);
        }

        public void Trace(string format, params object[] args)
        {
            Write(LogLevel.Trace, format, args);
        }

        public void Trace(Exception exception)
        {
            Write(LogLevel.Trace, exception);
        }

        public void Debug(string message)
        {
            Write(LogLevel.Debug, message);
        }

        public void Debug(string format, params object[] args)
        {
            Write(LogLevel.Debug, format, args);
        }

        public void Debug(Exception exception)
        {
            Write(LogLevel.Debug, exception);
        }

        public void Verbose(string message)
        {
            Write(LogLevel.Verbose, message);
        }

        public void Verbose(string format, params object[] args)
        {
            Write(LogLevel.Verbose, format, args);
        }

        public void Verbose(Exception exception)
        {
            Write(LogLevel.Verbose, exception);
        }
    }
}
