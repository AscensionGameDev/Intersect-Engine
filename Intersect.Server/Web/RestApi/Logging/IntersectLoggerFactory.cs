
using Microsoft.Owin.Logging;

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
