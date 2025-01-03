using System.Collections.Immutable;
using System.Diagnostics;
using Intersect.Framework.Reflection;
using Intersect.Logging;
using Intersect.Logging.Formatting;
using Intersect.Logging.Output;
using Intersect.Reflection;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using IntersectLogLevel = Intersect.Logging.LogLevel;

namespace Intersect.Server.Database;

internal sealed class IntersectLoggerFactory : ILoggerFactory
{
    private static readonly Dictionary<string, ImmutableArray<ILogOutput>> _cachedOutputs = new();

    private readonly DbLoggerProvider _loggerProvider;

    internal IntersectLoggerFactory(string name)
    {
        if (!_cachedOutputs.TryGetValue(name, out var outputs))
        {
            outputs = ImmutableArray.Create<ILogOutput>(
                new FileOutput(Log.SuggestFilename(prefix: $"db-{name}-")),
                new FileOutput(Log.SuggestFilename(prefix: $"db_errors-{name}-"), IntersectLogLevel.Error),
                new ConciseConsoleOutput(Debugger.IsAttached ? IntersectLogLevel.Warn : IntersectLogLevel.Error)
            );
            _cachedOutputs[name] = outputs;
        }

        _loggerProvider = new DbLoggerProvider(
            new Logger(
                new LogConfiguration
                {
                    Formatters = ImmutableList.Create(new DefaultFormatter()),
                    LogLevel = IntersectLogLevel.Debug,
                    Outputs = outputs,
                }
            )
        );
    }

    public void AddProvider(ILoggerProvider provider)
    {
        Log.Warn($"Tried to add provider but this is not implemented: {provider.GetFullishName()}");
    }

    public ILogger CreateLogger(string categoryName) => _loggerProvider.CreateLogger(categoryName);

    public void Dispose()
    {
    }
}