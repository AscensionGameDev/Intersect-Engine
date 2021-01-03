using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

using Intersect.Immutability;
using Intersect.Logging.Formatting;
using Intersect.Logging.Output;

namespace Intersect.Logging
{
    /// <summary>
    /// Configuration class for <see cref="Logger"/>.
    /// </summary>
    public sealed class LogConfiguration
    {
        private static readonly ILogFormatter DefaultFormatter = new DefaultFormatter();

        private static readonly ImmutableList<ILogFormatter> DefaultFormatters =
            ImmutableList.Create<ILogFormatter>() ?? throw new InvalidOperationException();

        private static readonly ImmutableList<ILogOutput> DefaultOutputs =
            ImmutableList.Create<ILogOutput>() ?? throw new InvalidOperationException();

        private Immutable<IReadOnlyList<ILogFormatter>> mFormatters;

        private Immutable<LogLevel> mLogLevel;

        private Immutable<IReadOnlyList<ILogOutput>> mOutputs;

        private Immutable<string> mTag;

        public static LogConfiguration Default => new LogConfiguration
        {
            Formatters = DefaultFormatters,

#if DEBUG
            LogLevel = Debugger.IsAttached ? LogLevel.All : LogLevel.Debug,
#else
            LogLevel = Debugger.IsAttached ? LogLevel.All : LogLevel.Trace,
#endif

            Tag = null
        };

        public ILogFormatter Formatter => mFormatters.Value?[0] ?? DefaultFormatter;

        public IReadOnlyList<ILogFormatter> Formatters
        {
            get => mFormatters.Value ?? DefaultFormatters;
            set => mFormatters.Value = value;
        }

        public LogLevel LogLevel
        {
            get => mLogLevel;
            set => mLogLevel.Value = value;
        }

        public IReadOnlyList<ILogOutput> Outputs
        {
            get => mOutputs.Value ?? DefaultOutputs;
            set => mOutputs.Value = value;
        }

        public string Tag
        {
            get => mTag;
            set => mTag.Value = value;
        }

        internal LogConfiguration Clone() =>
            MemberwiseClone() as LogConfiguration ?? throw new InvalidOperationException();
    }
}
