using System;
using System.Diagnostics;
using System.IO;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Logging
{
    public static class Log
    {
        private static string ExecutableName =>
            Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);

        public static string SuggestFilename(DateTime? time = null)
            => $"{ExecutableName}-{time ?? DateTime.Now:yyyy_MM_dd-HH_mm_ss_fff}.log";

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
                    sGlobal.AddOutput(new FileOutput($"errors-{ExecutableName}.log", LogLevel.Error));
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

        public static void Write(LogLevel logLevel, Exception exception, string message = null)
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

        public static void Write(Exception exception, string message = null)
        {
            Global.Write(exception, message);
        }

        public static void All(string message)
        {
            Global.All(message);
        }

        public static void All(string format, params object[] args)
        {
            Global.All(format, args);
        }

        public static void All(Exception exception, string message = null)
        {
            Global.All(exception, message);
        }

        public static void Error(string message)
        {
            Global.Error(message);
        }

        public static void Error(string format, params object[] args)
        {
            Global.Error(format, args);
        }

        public static void Error(Exception exception, string message = null)
        {
            Global.Error(exception, message);
        }

        public static void Warn(string message)
        {
            Global.Warn(message);
        }

        public static void Warn(string format, params object[] args)
        {
            Global.Warn(format, args);
        }

        public static void Warn(Exception exception, string message = null)
        {
            Global.Warn(exception, message);
        }

        public static void Info(string message)
        {
            Global.Info(message);
        }

        public static void Info(string format, params object[] args)
        {
            Global.Info(format, args);
        }

        public static void Info(Exception exception, string message = null)
        {
            Global.Info(exception, message);
        }

        public static void Trace(string message)
        {
            Global.Trace(message);
        }

        public static void Trace(string format, params object[] args)
        {
            Global.Trace(format, args);
        }

        public static void Trace(Exception exception, string message = null)
        {
            Global.Trace(exception, message);
        }

        public static void Debug(string message)
        {
            Global.Debug(message);
        }

        public static void Debug(string format, params object[] args)
        {
            Global.Debug(format, args);
        }

        public static void Debug(Exception exception, string message = null)
        {
            Global.Debug(exception, message);
        }

        public static void Diagnostic(string message)
        {
            Global.Diagnostic(message);
        }

        public static void Diagnostic(string format, params object[] args)
        {
            Global.Diagnostic(format, args);
        }

        public static void Diagnostic(Exception exception, string message = null)
        {
            Global.Diagnostic(exception, message);
        }

        public static void Verbose(string message)
        {
            Global.Verbose(message);
        }

        public static void Verbose(string format, params object[] args)
        {
            Global.Verbose(format, args);
        }

        public static void Verbose(Exception exception, string message = null)
        {
            Global.Verbose(exception, message);
        }

        #endregion
    }
}