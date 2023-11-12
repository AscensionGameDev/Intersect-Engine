using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database;

public sealed partial class DbLoggerProvider : ILoggerProvider
{
    private readonly Intersect.Logging.Logger _intersectLogger;

    private DbLogger? _logger;

    public DbLoggerProvider(Intersect.Logging.Logger intersectLogger)
    {
        _intersectLogger = intersectLogger;
    }

    public ILogger CreateLogger(string categoryName) => _logger ??= new DbLogger(_intersectLogger);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _logger = default;
    }
}