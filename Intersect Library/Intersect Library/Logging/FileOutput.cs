using System;
using System.IO;
using System.Text;

namespace Intersect_Library.Logging
{
    public class FileOutput : ILogOutput
    {
        private void EnsureOutputDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public bool Append { get; set; }

        private string mFilename;
        public string Filename
        {
            get
            {
                return mFilename;
            }
            set
            {
                if (TextUtils.IsEmpty(value))
                {
                    Log.Warn("Cannot set FileOutput to an empty file name.");
                    return;
                }

                Close();

                mFilename = value;
            }
        }

        private StreamWriter mWriter;
        private StreamWriter Writer
        {
            get
            {
                if (mWriter == null)
                {
                    var directory = Path.IsPathRooted(mFilename) ? Path.GetDirectoryName(mFilename) : Path.Combine("resources", "logs");
                    EnsureOutputDirectory(directory);
                    mWriter = new StreamWriter(Path.Combine(directory, mFilename), Append, Encoding.UTF8);
                    mWriter.AutoFlush = true;
                }

                return mWriter;
            }
        }

        public LogLevel LogLevel { get; set; }

        private const string TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public FileOutput(string filename = null, bool append = true) : this(filename, LogLevel.All, append)
        {
        }

        public FileOutput(string filename, LogLevel logLevel, bool append = true)
        {
            Filename = TextUtils.IsEmpty(filename) ? Log.SuggestFilename() : filename;
            LogLevel = logLevel;
            Append = append;
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
                } catch (ObjectDisposedException)
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

        public void Write(string tag, LogLevel logLevel, string message)
        {
            if (LogLevel < logLevel)
            {
                return;
            }

            if (TextUtils.IsEmpty(tag))
            {
                Writer.WriteLine("{0} [{1}] {2}", DateTime.UtcNow.ToString(TIMESTAMP_FORMAT), logLevel, message);
            } else
            {
                Writer.WriteLine("{0} [{1}] {2}: {3}", DateTime.UtcNow.ToString(TIMESTAMP_FORMAT), logLevel, tag, message);
            }
        }

        public void Write(string tag, LogLevel logLevel, string format, params object[] args)
        {
            Write(tag, logLevel, string.Format(format, args));
        }

        public void Write(string tag, LogLevel logLevel, Exception exception)
        {
            Write(tag, logLevel, string.Format("Message: {0}", exception.Message));
            Write(tag, logLevel, string.Format("Stack Trace: {0}", exception.StackTrace));
            Write(tag, logLevel, string.Format("Time: {0}", DateTime.UtcNow.ToString()));
            Writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            Writer.Flush();
        }
    }
}
