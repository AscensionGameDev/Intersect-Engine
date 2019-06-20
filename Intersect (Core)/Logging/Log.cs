using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using Intersect.Logging.Output;
using JetBrains.Annotations;

namespace Intersect.Logging
{

    public static class Log
    {

        [NotNull]
        private static string ExecutableName =>
            Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);

        [NotNull]
        public static string SuggestFilename(DateTime? time = null, [CanBeNull] string prefix = null, [CanBeNull] string extensionPrefix = null) =>
            $"{prefix?.Trim() ?? ""}{ExecutableName}-{time ?? DateTime.Now:yyyy_MM_dd-HH_mm_ss_fff}{(string.IsNullOrWhiteSpace(extensionPrefix) ? "" : "." + extensionPrefix)}.log";

        #region Global

        static Log()
        {
            var outputs = ImmutableList.Create<ILogOutput>(
                new FileOutput(),
                new FileOutput($"errors-{ExecutableName}.log", LogLevel.Error),
                new ConciseConsoleOutput(Debugger.IsAttached ? LogLevel.All : LogLevel.Error)
            ) ?? throw new InvalidOperationException();

            Pretty = new Logger(new LogConfiguration
            {
                Pretty = true,
                LogLevel = LogConfiguration.Default.LogLevel,
                Outputs = outputs
            });

            Default = new Logger(new LogConfiguration
            {
                Pretty = false,
                LogLevel = LogConfiguration.Default.LogLevel,
                Outputs = outputs
            });
        }

        [NotNull]
        public static Logger Pretty { get; }

        [NotNull]
        public static Logger Default { get; }

        public static void Write(LogLevel logLevel, string message)
        {
            Default.Write(logLevel, message);
        }

        public static void Write(LogLevel logLevel, string format, params object[] args)
        {
            Default.Write(logLevel, format, args);
        }

        public static void Write(LogLevel logLevel, Exception exception, string message = null)
        {
            Default.Write(logLevel, exception);
        }

        public static void Write(string message)
        {
            Default.Write(message);
        }

        public static void Write(string format, params object[] args)
        {
            Default.Write(format, args);
        }

        public static void Write(Exception exception, string message = null)
        {
            Default.Write(exception, message);
        }

        public static void All(string message)
        {
            Default.All(message);
        }

        public static void All(string format, params object[] args)
        {
            Default.All(format, args);
        }

        public static void All(Exception exception, string message = null)
        {
            Default.All(exception, message);
        }

        public static void Error(string message)
        {
            Default.Error(message);
        }

        public static void Error(string format, params object[] args)
        {
            Default.Error(format, args);
        }

        public static void Error(Exception exception, string message = null)
        {
            Default.Error(exception, message);
        }

        public static void Warn(string message)
        {
            Default.Warn(message);
        }

        public static void Warn(string format, params object[] args)
        {
            Default.Warn(format, args);
        }

        public static void Warn(Exception exception, string message = null)
        {
            Default.Warn(exception, message);
        }

        public static void Info(string message)
        {
            Default.Info(message);
        }

        public static void Info(string format, params object[] args)
        {
            Default.Info(format, args);
        }

        public static void Info(Exception exception, string message = null)
        {
            Default.Info(exception, message);
        }

        public static void Trace(string message)
        {
            Default.Trace(message);
        }

        public static void Trace(string format, params object[] args)
        {
            Default.Trace(format, args);
        }

        public static void Trace(Exception exception, string message = null)
        {
            Default.Trace(exception, message);
        }

        public static void Debug(string message)
        {
            Default.Debug(message);
        }

        public static void Debug(string format, params object[] args)
        {
            Default.Debug(format, args);
        }

        public static void Debug(Exception exception, string message = null)
        {
            Default.Debug(exception, message);
        }

        public static void Diagnostic(string message)
        {
            Default.Diagnostic(message);
        }

        public static void Diagnostic(string format, params object[] args)
        {
            Default.Diagnostic(format, args);
        }

        public static void Diagnostic(Exception exception, string message = null)
        {
            Default.Diagnostic(exception, message);
        }

        public static void Verbose(string message)
        {
            Default.Verbose(message);
        }

        public static void Verbose(string format, params object[] args)
        {
            Default.Verbose(format, args);
        }

        public static void Verbose(Exception exception, string message = null)
        {
            Default.Verbose(exception, message);
        }

        #endregion

    }

}
