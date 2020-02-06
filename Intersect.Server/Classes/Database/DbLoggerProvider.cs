using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Classes.Database
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
