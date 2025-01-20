using System.Diagnostics;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Intersect.Server.Database;

internal sealed class IntersectLoggerFactory : ILoggerFactory
{
    private static readonly Dictionary<string, Logger> LoggersByName = new();

    private readonly ILoggerFactory _loggerFactory;

    internal IntersectLoggerFactory(string name)
    {
        if (!LoggersByName.TryGetValue(name, out var logger))
        {
            var configuration = new LoggerConfiguration().Enrich.FromLogContext().WriteTo
                .Console(restrictedToMinimumLevel: Debugger.IsAttached ? LogEventLevel.Warning : LogEventLevel.Error)
                .WriteTo.File(path: $"db-{name}.log").WriteTo.File(
                    path: $"db-errors-{name}.log",
                    restrictedToMinimumLevel: LogEventLevel.Error
                );
            LoggersByName[name] = configuration.CreateLogger();
        }

        _loggerFactory = new SerilogLoggerFactory(logger);
    }

    public void AddProvider(ILoggerProvider provider)
    {
        ApplicationContext.Context.Value?.Logger.LogWarning($"Tried to add provider but this is not implemented: {provider.GetFullishName()}");
    }

    public ILogger<T> CreateLogger<T>() => _loggerFactory.CreateLogger<T>();

    public ILogger CreateLogger(string categoryName) => _loggerFactory.CreateLogger(categoryName);

    public void Dispose()
    {
    }
}