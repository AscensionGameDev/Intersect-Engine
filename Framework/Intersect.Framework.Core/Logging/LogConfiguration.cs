using System.Collections.Immutable;
using System.Diagnostics;

using Intersect.Logging.Formatting;
using Intersect.Logging.Output;
using Microsoft.Extensions.Logging;

namespace Intersect.Logging;

/// <summary>
/// Configuration class for <see cref="Logger{T}"/>.
/// </summary>
public sealed partial class LogConfiguration
{
    private static readonly ILogFormatter DefaultFormatter = new DefaultFormatter();

    private static readonly ImmutableList<ILogFormatter> DefaultFormatters =
        ImmutableList.Create<ILogFormatter>() ?? throw new InvalidOperationException();

    private static readonly ImmutableList<ILogOutput> DefaultOutputs =
        ImmutableList.Create<ILogOutput>() ?? throw new InvalidOperationException();

    private LogLevel mLogLevel;

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

    public ILogFormatter Formatter => Formatters.FirstOrDefault() ?? DefaultFormatter;

    public IReadOnlyList<ILogFormatter>? Formatters { get; init; }

    public LogLevel LogLevel
    {
        get => mLogLevel;
        set => mLogLevel = value;
    }

    public IReadOnlyList<ILogOutput> Outputs { get; init; }

    public string Tag { get; init; }

    internal LogConfiguration Clone() =>
        MemberwiseClone() as LogConfiguration ?? throw new InvalidOperationException();
}
