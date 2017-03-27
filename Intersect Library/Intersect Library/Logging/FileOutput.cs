using System;
using System.IO;
using System.Text;

namespace Intersect.Logging
{
    public class FileOutput : ILogOutput
    {
        private const string TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";
        private static readonly string SPACER =
            Environment.NewLine + new string('-', 80) + Environment.NewLine;

        private string mFilename;

        private StreamWriter mWriter;

        public FileOutput(string filename = null, bool append = true)
            : this(filename, LogLevel.All, append)
        {
        }

        public FileOutput(string filename, LogLevel logLevel,
            bool append = true)
        {
            Filename = string.IsNullOrEmpty(filename)
                ? Log.SuggestFilename() : filename;
            LogLevel = logLevel;
            Append = append;
        }

        public bool Append { get; set; }

        public string Filename
        {
            get { return mFilename; }
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

        private StreamWriter Writer
        {
            get
            {
                if (mWriter == null)
                {
                    var directory = Path.IsPathRooted(mFilename)
                        ? Path.GetDirectoryName(mFilename)
                        : "logs";
                    EnsureOutputDirectory(directory);
                    mWriter = new StreamWriter(
                        Path.Combine(directory, mFilename),
                        Append, Encoding.UTF8)
                    {
                        AutoFlush = true
                    };
                }

                return mWriter;
            }
        }

        public LogLevel LogLevel { get; set; }

        public void Write(string tag, LogLevel logLevel, string message)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            if (string.IsNullOrEmpty(tag))
            {
                Writer.WriteLine(
                    $"{{0:{TIMESTAMP_FORMAT}}} [{{1}}] {{2}}",
                    DateTime.UtcNow, logLevel, message);
            }
            else
            {
                Writer.WriteLine(
                    $"{{0:{TIMESTAMP_FORMAT}}} [{{1}}] {{2}}: {{3}}",
                    DateTime.UtcNow, logLevel, tag, message);
            }
        }

        public void Write(string tag, LogLevel logLevel, string format,
            params object[] args)
        {
            Write(tag, logLevel, string.Format(format, args));
        }

        public void Write(string tag, LogLevel logLevel, Exception exception)
        {
            Write(tag, logLevel, $"Message: {exception?.Message}");
            Write(tag, logLevel, $"Stack Trace: {exception?.StackTrace}");
            Write(tag, logLevel, $"Time: {DateTime.UtcNow}");
            Writer?.WriteLine(SPACER);
            Writer?.Flush();
        }

        private static void EnsureOutputDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        ~FileOutput()
        {
            Close();
        }

        public void Flush()
        {
            if (mWriter != null)
            {
                try
                {
                    mWriter.Flush();
                }
                catch (ObjectDisposedException)
                {
                    /* Ignore this exception */
                }
            }
        }

        public void Close()
        {
            Flush();

            if (mWriter != null)
            {
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
}