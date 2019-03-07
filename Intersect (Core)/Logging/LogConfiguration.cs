using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Intersect.Immutability;
using Intersect.Logging.Formatting;
using Intersect.Logging.Output;
using JetBrains.Annotations;

namespace Intersect.Logging
{
    public sealed class LogConfiguration
    {

        [NotNull]
        public static LogConfiguration Default => new LogConfiguration
        {
            Formatters = DefaultFormatters,

#if DEBUG
            LogLevel = LogLevel.All,
#else
            LogLevel = LogLevel.Info,
#endif

            Pretty = false,

            Tag = null
        };

        private static readonly ILogFormatter DefaultFormatter = new DefaultFormatter();

        private static readonly ImmutableList<ILogFormatter> DefaultFormatters = ImmutableList.Create<ILogFormatter>();
        private static readonly ImmutableList<ILogOutput> DefaultOutputs = ImmutableList.Create<ILogOutput>();

        private Immutable<IReadOnlyList<ILogFormatter>> mFormatters;
        private Immutable<LogLevel> mLogLevel;
        private Immutable<IReadOnlyList<ILogOutput>> mOutputs;
        private Immutable<bool> mPretty;
        private Immutable<string> mTag;

        [NotNull]
        public ILogFormatter Formatter => mFormatters.Value.FirstOrDefault() ?? DefaultFormatter;

        [NotNull]
        public IReadOnlyList<ILogFormatter> Formatters
        {
            get => mFormatters.Value ?? DefaultFormatters;
            set => mFormatters.Value = value;
        }

        public LogLevel LogLevel {
            get => mLogLevel;
            set => mLogLevel.Value = value;
        }

        [NotNull]
        public IReadOnlyList<ILogOutput> Outputs
        {
            get => mOutputs.Value ?? DefaultOutputs;
            set => mOutputs.Value = value;
        }

        public bool Pretty
        {
            get => mPretty;
            set => mPretty.Value = value;
        }

        public string Tag
        {
            get => mTag;
            set => mTag.Value = value;
        }

        [NotNull]
        internal LogConfiguration Clone()
        {
            return MemberwiseClone() as LogConfiguration ?? throw new InvalidOperationException();
        }
    }
}
