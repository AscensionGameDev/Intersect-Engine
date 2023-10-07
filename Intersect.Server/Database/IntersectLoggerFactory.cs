using Intersect.Logging;
using Intersect.Reflection;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Intersect.Server.Database;

internal sealed class IntersectLoggerFactory : ILoggerFactory
{
    private readonly DbLoggerProvider _loggerProvider = new(Log.Default);

    public void Dispose()
    {
    }

    public void AddProvider(ILoggerProvider provider)
    {
        Log.Warn($"Tried to add provider but this is not implemented: {provider.GetFullishName()}");
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggerProvider.CreateLogger(categoryName);
    }
}