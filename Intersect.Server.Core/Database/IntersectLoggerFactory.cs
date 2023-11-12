using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
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
    private static string ExecutableName => Path.GetFileNameWithoutExtension(
        Process.GetCurrentProcess().MainModule?.FileName ?? Assembly.GetExecutingAssembly().GetName().Name
    );

    private readonly DbLoggerProvider _loggerProvider;

    internal IntersectLoggerFactory()
    {
        var outputs = ImmutableList.Create<ILogOutput>(
            new FileOutput($"db-{ExecutableName}.log"),
            new FileOutput($"errors-{ExecutableName}.log", IntersectLogLevel.Error),
            new ConciseConsoleOutput(Debugger.IsAttached ? IntersectLogLevel.Warn : IntersectLogLevel.Error)
        );

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