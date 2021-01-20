using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

using Intersect.Logging.Formatting;
using Intersect.Logging.Output;

namespace Intersect.Logging
{
    public static class Log
    {
        internal static readonly DateTime Initial = DateTime.Now;

        private static string ExecutableName =>
            Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);

        public static string SuggestFilename(
            DateTime? time = null,
            string prefix = null,
            string extensionPrefix = null
        ) =>
            $"{prefix?.Trim() ?? ""}{ExecutableName}-{time ?? Initial:yyyy_MM_dd-HH_mm_ss_fff}{(string.IsNullOrWhiteSpace(extensionPrefix) ? "" : "." + extensionPrefix)}.log";

        #region Global

        static Log()
        {
            var outputs = ImmutableList.Create<ILogOutput>(
                              new FileOutput(), new FileOutput($"errors-{ExecutableName}.log", LogLevel.Error),
                              new ConciseConsoleOutput(Debugger.IsAttached ? LogLevel.All : LogLevel.Error)
                          ) ??
                          throw new InvalidOperationException();

            Pretty = new Logger(
                new LogConfiguration
                {
                    Formatters = ImmutableList.Create(new PrettyFormatter()) ?? throw new InvalidOperationException(),
                    LogLevel = LogConfiguration.Default.LogLevel,
                    Outputs = outputs
                }
            );

            Default = new Logger(
                new LogConfiguration
                {
                    Formatters = ImmutableList.Create(new DefaultFormatter()) ?? throw new InvalidOperationException(),
                    LogLevel = LogConfiguration.Default.LogLevel,
                    Outputs = outputs
                }
            );
        }

        public static Logger Pretty { get; internal set; }

        public static Logger Default { get; internal set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(LogLevel logLevel, string message)
        {
            Default.Write(logLevel, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(LogLevel logLevel, string format, params object[] args)
        {
            Default.Write(logLevel, format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(LogLevel logLevel, Exception exception, string message = null)
        {
            Default.Write(logLevel, exception);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(string message)
        {
            Default.Write(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(string format, params object[] args)
        {
            Default.Write(format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(Exception exception, string message = null)
        {
            Default.Write(exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void All(string message)
        {
            Default.All(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void All(string format, params object[] args)
        {
            Default.All(format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void All(Exception exception, string message = null)
        {
            Default.All(exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(string message)
        {
            Default.Error(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(string format, params object[] args)
        {
            Default.Error(format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(Exception exception, string message = null)
        {
            Default.Error(exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string message)
        {
            Default.Warn(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string format, params object[] args)
        {
            Default.Warn(format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(Exception exception, string message = null)
        {
            Default.Warn(exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(string message)
        {
            Default.Info(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(string format, params object[] args)
        {
            Default.Info(format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(Exception exception, string message = null)
        {
            Default.Info(exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Trace(string message)
        {
            Default.Trace(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Trace(string format, params object[] args)
        {
            Default.Trace(format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Trace(Exception exception, string message = null)
        {
            Default.Trace(exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(string message)
        {
            Default.Debug(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(string format, params object[] args)
        {
            Default.Debug(format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug(Exception exception, string message = null)
        {
            Default.Debug(exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Diagnostic(string message)
        {
            Default.Diagnostic(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Diagnostic(string format, params object[] args)
        {
            Default.Diagnostic(format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Diagnostic(Exception exception, string message = null)
        {
            Default.Diagnostic(exception, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Verbose(string message)
        {
            Default.Verbose(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Verbose(string format, params object[] args)
        {
            Default.Verbose(format, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Verbose(Exception exception, string message = null)
        {
            Default.Verbose(exception, message);
        }

        #endregion
    }
}
