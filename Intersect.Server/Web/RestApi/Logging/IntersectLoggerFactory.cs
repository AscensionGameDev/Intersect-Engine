
using Microsoft.Owin.Logging;
using ILogger = Microsoft.Owin.Logging.ILogger;
using ILoggerFactory = Microsoft.Owin.Logging.ILoggerFactory;

namespace Intersect.Server.Web.RestApi.Logging
{

    internal sealed partial class IntersectLoggerFactory : ILoggerFactory
    {

        /// <inheritdoc />
        public ILogger Create(string name)
        {
            return new IntersectLogger(name);
        }

    }

}
