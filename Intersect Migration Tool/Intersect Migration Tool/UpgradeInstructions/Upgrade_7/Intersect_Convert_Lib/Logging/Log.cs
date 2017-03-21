using System;
using System.Diagnostics;
using System.IO;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_7.Intersect_Convert_Lib.Logging
{
    public static class Log
    {
        public static string SuggestFilename(DateTime? time = null)
        {
            return string.Format("{0}-{1}.log", Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName), (time.HasValue ? time.Value : DateTime.Now).ToString("yyyy_MM_dd-HH_mm_ss_fff"));
        }

        #region Global
        private static Logger sGlobal;
        public static Logger Global
        {
            get
            {
                if (sGlobal == null)
                {
                    sGlobal = new Logger();
                    sGlobal.AddOutput(new FileOutput());
                    sGlobal.AddOutput(new FileOutput("errors.log", LogLevel.Error));
                    // TODO: Add console output
                }

                return sGlobal;
            }
        }

        public static void Write(LogLevel logLevel, string message)
        {
            Global.Write(logLevel, message);
        }

        public static void Write(LogLevel logLevel, string format, params object[] args)
        {
            Global.Write(logLevel, format, args);
        }

        public static void Write(LogLevel logLevel, Exception exception)
        {
            Global.Write(logLevel, exception);
        }

        public static void Write(string message)
        {
            Global.Write(message);
        }

        public static void Write(string format, params object[] args)
        {
            Global.Write(format, args);
        }

        public static void Write(Exception exception)
        {
            Global.Write(exception);
        }

        public static void All(string message)
        {
            Global.All(message);
        }

        public static void All(string format, params object[] args)
        {
            Global.All(format, args);
        }

        public static void All(Exception exception)
        {
            Global.All(exception);
        }

        public static void Error(string message)
        {
            Global.Error(message);
        }

        public static void Error(string format, params object[] args)
        {
            Global.Error(format, args);
        }

        public static void Error(Exception exception)
        {
            Global.Error(exception);
        }

        public static void Warn(string message)
        {
            Global.Warn(message);
        }

        public static void Warn(string format, params object[] args)
        {
            Global.Warn(format, args);
        }

        public static void Warn(Exception exception)
        {
            Global.Warn(exception);
        }

        public static void Info(string message)
        {
            Global.Info(message);
        }

        public static void Info(string format, params object[] args)
        {
            Global.Info(format, args);
        }

        public static void Info(Exception exception)
        {
            Global.Info(exception);
        }

        public static void Trace(string message)
        {
            Global.Trace(message);
        }

        public static void Trace(string format, params object[] args)
        {
            Global.Trace(format, args);
        }

        public static void Trace(Exception exception)
        {
            Global.Trace(exception);
        }

        public static void Debug(string message)
        {
            Global.Debug(message);
        }

        public static void Debug(string format, params object[] args)
        {
            Global.Debug(format, args);
        }

        public static void Debug(Exception exception)
        {
            Global.Debug(exception);
        }

        public static void Verbose(string message)
        {
            Global.Verbose(message);
        }

        public static void Verbose(string format, params object[] args)
        {
            Global.Verbose(format, args);
        }

        public static void Verbose(Exception exception)
        {
            Global.Verbose(exception);
        }
        #endregion
    }
}
