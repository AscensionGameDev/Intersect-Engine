using System;
using System.Linq;
using System.Net.Http;

using JetBrains.Annotations;

using WebApiThrottle;

namespace Intersect.Server.Web.RestApi
{
    public class IntersectThrottlingHandler : ThrottlingHandler
    {

        [NotNull]
        public const string DefaultFallbackClientKey = "test";

        public string Header { get; set; }

        public string FallbackClientKey { get; set; }

        protected override RequestIdentity SetIdentity([NotNull] HttpRequestMessage request)
        {
            if (request.Headers == null)
            {
                throw new ArgumentNullException(nameof(request.Headers));
            }

            string clientKey = null;
            if (request.Headers.TryGetValues(Header, out var clientKeys))
            {
                clientKey = clientKeys.FirstOrDefault();
            }

            return new RequestIdentity
            {
                ClientKey = clientKey ?? FallbackClientKey ?? DefaultFallbackClientKey,
                ClientIp = GetClientIp(request)?.ToString(),
                Endpoint = request.RequestUri?.AbsolutePath
            };
        }
    }
}
