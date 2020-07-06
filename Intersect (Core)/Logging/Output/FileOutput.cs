using System;
using System.IO;
using System.Text;

using Intersect.IO.Files;

using JetBrains.Annotations;

namespace Intersect.Logging.Output
{

    public class FileOutput : ILogOutput
    {

        private static readonly string Spacer = Environment.NewLine + new string('-', 80) + Environment.NewLine;

        [NotNull] private string mFilename;

        private StreamWriter mWriter;

        public FileOutput(string filename = null, bool append = true) : this(filename, LogLevel.All, append)
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

        public void Write(LogConfiguration configuration, LogLevel logLevel, string message)
        {
            InternalWrite(configuration, logLevel, null, message);
        }

        public void Write(LogConfiguration configuration, LogLevel logLevel, string format, params object[] args)
        {
            InternalWrite(configuration, logLevel, null, format, args);
        }

        public void Write(LogConfiguration configuration, LogLevel logLevel, Exception exception, string message)
        {
            InternalWrite(configuration, logLevel, exception, message);
        }

        public void Write(
            LogConfiguration configuration,
            LogLevel logLevel,
            Exception exception,
            string format,
            params object[] args
        )
        {
            InternalWrite(configuration, logLevel, exception, format, args);
        }

        private void InternalWrite(
            [NotNull] LogConfiguration configuration,
            LogLevel logLevel,
            Exception exception,
            [NotNull] string format,
            params object[] args
        )
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            var line = configuration.Formatter.Format(
                configuration, logLevel, DateTime.UtcNow, exception, format, args
            );

            Writer.Write(line);
            Writer.Write(Spacer);
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
