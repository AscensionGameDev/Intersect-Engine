using Microsoft.Extensions.Logging;

namespace Intersect.Server.Database
{
    public class DbLoggerProvider : ILoggerProvider
    {
        private DbLogger _logger;
        private Intersect.Logging.Logger _intersectLogger;

        public DbLoggerProvider(Intersect.Logging.Logger intersectLogger)
        {
            _intersectLogger = intersectLogger;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (_logger == null) _logger = new DbLogger(_intersectLogger);
            return _logger;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}
