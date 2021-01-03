using System;
using System.Linq;

using Intersect.Server.Web.Utilities;

using Microsoft.Owin;

using WebApiThrottle;

namespace Intersect.Server.Web.RestApi.Middleware
{

    public class IntersectThrottlingMiddleware : ThrottlingMiddleware
    {

        private const string DefaultAuthorizedFallbackClientKey = "authorized";

        public const string DefaultFallbackClientKey = "test";

        public const string DefaultHeader = "X-Intersect-ApiKey";

        public static readonly IThrottleRepository DefaultRepository = new MemoryCacheRepository();

        private string mFallbackClientKey;

        private string mHeader;

        /// <inheritdoc />
        public IntersectThrottlingMiddleware(
            OwinMiddleware next,
            ThrottlePolicy policy,
            string header,
            string fallbackClientKey,
            IThrottleRepository throttleRepository
        ) : base(next, policy, null, throttleRepository ?? DefaultRepository, null, null)
        {
            mHeader = header ?? DefaultHeader;
            mFallbackClientKey = fallbackClientKey ?? DefaultFallbackClientKey;
        }

        public string Header
        {
            get => mHeader;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    mHeader = value;
                }
            }
        }

        public string FallbackClientKey
        {
            get => mFallbackClientKey;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    mFallbackClientKey = value;
                }
            }
        }

        protected override RequestIdentity SetIdentity(IOwinRequest request)
        {
            if (request.Headers == null)
            {
                throw new ArgumentNullException(nameof(request.Headers));
            }

            string clientKey = null;
            if (request.Headers.TryGetValue(Header, out var clientKeys))
            {
                clientKey = clientKeys.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(clientKey) &&
                request.Headers.TryGetValue("Authorization", out var authorizationHeaders))
            {
                if (authorizationHeaders.Any(HeaderHelper.IsValidAuthorizationBearerHeader))
                {
                    clientKey = DefaultAuthorizedFallbackClientKey;
                }
            }

            return new RequestIdentity
            {
                ClientKey = clientKey ?? FallbackClientKey,
                ClientIp = request.RemoteIpAddress,
                Endpoint = request.Uri?.AbsolutePath.ToLowerInvariant()
            };
        }

    }

}
