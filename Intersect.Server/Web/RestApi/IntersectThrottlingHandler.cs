using System;
using System.Linq;
using System.Net.Http;

using Intersect.Server.Web.Utilities;

using WebApiThrottle;

namespace Intersect.Server.Web.RestApi
{

    // TODO: Probably a good idea to remove this since it was replaced with middleware
    public class IntersectThrottlingHandler : ThrottlingHandler
    {

        public const string DefaultAuthorizedFallbackClientKey = "authorized";

        public const string DefaultFallbackClientKey = "test";

        public string Header { get; set; }

        public string FallbackClientKey { get; set; }

        protected override RequestIdentity SetIdentity(HttpRequestMessage request)
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

            if (string.IsNullOrWhiteSpace(clientKey) &&
                request.Headers.TryGetValues("Authorization", out var authorizationHeaders))
            {
                if (authorizationHeaders.Any(HeaderHelper.IsValidAuthorizationBearerHeader))
                {
                    clientKey = DefaultAuthorizedFallbackClientKey;
                }
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
