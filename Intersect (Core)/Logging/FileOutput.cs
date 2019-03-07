using System;
using System.IO;
using System.Text;

using Intersect.IO.FileSystem;

using JetBrains.Annotations;

namespace Intersect.Logging
{

    public class FileOutput : ILogOutput
    {

        private const string TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        private static readonly string Spacer = Environment.NewLine + new string('-', 80) + Environment.NewLine;

        [NotNull]
        private string mFilename;

        private StreamWriter mWriter;

        public FileOutput(string filename = null, bool append = true)
            : this(filename, LogLevel.All, append)
        {
        }

        public FileOutput(string filename, LogLevel logLevel, bool append = true)
        {
            Filename = string.IsNullOrEmpty(filename) ? Log.SuggestFilename() : filename;
            LogLevel = logLevel;
            Append = append;
        }

        public bool Append { get; set; }

        [NotNull]
        public string Filename
        {
            get => mFilename;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Log.Warn("Cannot set FileOutput to an empty file name.");
                    return;
                }

                Close();

                mFilename = value;
            }
        }

        [NotNull]
        private StreamWriter Writer
        {
            get
            {
                if (mWriter != null)
                {
                    return mWriter;
                }

                var directory = Path.IsPathRooted(mFilename) ? Path.GetDirectoryName(mFilename) : null;
                directory = string.IsNullOrWhiteSpace(directory) ? "logs" : directory;

                if (!FileSystemHelper.EnsureDirectoryExists(directory))
                {
                    throw new InvalidOperationException("The logger directory could not be created or is a file.");
                }

                mWriter = new StreamWriter(Path.Combine(directory, mFilename), Append, Encoding.UTF8)
                {
                    AutoFlush = true
                };

                return mWriter;
            }
        }

        public LogLevel LogLevel { get; set; }

        private void InternalWrite(LogLevel logLevel, string message)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            Writer.WriteLine(message);
        }

        public void Write(LoggerConfiguration configuration, LogLevel logLevel, string message)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            var prefix = configuration.Pretty ? "" : $"{DateTime.UtcNow.ToString(TIMESTAMP_FORMAT)} [{logLevel}]";
            var line = string.IsNullOrEmpty(configuration.Tag)
                ? $"{prefix} {message}"
                : $"{prefix} {configuration.Tag}: {message}";

            InternalWrite(logLevel, line.TrimStart());
        }

        public void Write(LoggerConfiguration configuration, LogLevel logLevel, [NotNull] string format, [NotNull] params object[] args)
        {
            Write(configuration, logLevel, string.Format(format, args));
        }

        public void Write(LoggerConfiguration configuration, LogLevel logLevel, Exception exception, string message)
        {
            if (exception != null)
            {
                Write(configuration, logLevel, $"Message: {exception.Message}");

                if (exception.StackTrace != null)
                {
                    Write(configuration, logLevel, "Stack Trace:");
                    InternalWrite(logLevel, exception.StackTrace);
                }

                if (exception.InnerException != null)
                {
                    Write(configuration, logLevel, $"Caused by: {exception.InnerException.Message}");
                    Write(configuration, logLevel, "Stack Trace:");
                    InternalWrite(logLevel, exception.InnerException.StackTrace);
                }
            }

            Write(configuration, logLevel, $"Time: {DateTime.UtcNow}");
            if (!string.IsNullOrEmpty(message))
            {
                Write(configuration, logLevel, $"Note: {message}");
            }

            Writer.WriteLine(Spacer);
            Writer.Flush();
        }

        ~FileOutput()
        {
            Close();
        }

        public void Flush()
        {
            if (mWriter == null)
            {
                return;
            }

            try
            {
                mWriter.Flush();
            }
            catch (ObjectDisposedException)
            {
                /* Ignore this exception */
            }
        }

        public void Close()
        {
            Flush();

            if (mWriter == null)
            {
                return;
            }

            try
            {
                mWriter.Close();
            }
            catch (ObjectDisposedException)
            {
                /* Ignore this exception */
            }

            mWriter = null;
        }

    }

}
