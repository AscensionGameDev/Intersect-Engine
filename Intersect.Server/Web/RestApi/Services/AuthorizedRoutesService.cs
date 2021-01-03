using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Description;

using Intersect.Server.Web.RestApi.Configuration;

namespace Intersect.Server.Web.RestApi.Services
{

    internal sealed class AuthorizedRoutesService : IAuthorizedRoutesService
    {

        public AuthorizedRoutesService(
            ApiConfiguration apiConfiguration,
            HttpConfiguration httpConfiguration
        )
        {
            ApiExplorer = httpConfiguration.Services.GetApiExplorer() ?? throw new InvalidOperationException();
            Routes = apiConfiguration.RouteAuthorization;
        }

        private IReadOnlyDictionary<string, object> Routes { get; }

        public IApiExplorer ApiExplorer { get; }

        /// <inheritdoc />
        public bool RequiresAuthorization(string endpoint, string method)
        {
            if (Routes.TryGetValue(endpoint, out var endpointAuthorization))
            {
                return SegmentRequiresAuthorization(endpointAuthorization, method);
            }

            var segments = endpoint.Split('/');

            // TODO: Implement hierarchical scan

            return PartialEndpointRequiresAuthorization(segments, method);
        }

        private bool PartialEndpointRequiresAuthorization(
            IReadOnlyList<string> segments,
            string method
        )
        {
            if (segments.Count < 1)
            {
                return true;
            }

            var partialEndpoint = segments[0];
            var requiresAuthorization = true;
            for (var segmentIndex = 0; segmentIndex < segments.Count; ++segmentIndex)
            {
                if (segmentIndex > 0)
                {
                    partialEndpoint += '/' + segments[segmentIndex];
                }

                Debug.Assert(partialEndpoint != null, nameof(partialEndpoint) + " != null");
                if (!Routes.TryGetValue(partialEndpoint, out var endpointAuthorization))
                {
                    continue;
                }

                // More specific overrides initial/previous state
                requiresAuthorization = SegmentRequiresAuthorization(endpointAuthorization, method);
            }

            return requiresAuthorization;
        }

        private bool SegmentRequiresAuthorization(string segment, string method)
        {
            return !Routes.TryGetValue(segment, out var segmentAuthorization) ||
                   SegmentRequiresAuthorization(segmentAuthorization, method);
        }

        private static bool SegmentRequiresAuthorization(object segmentAuthorization, string method)
        {
            switch (segmentAuthorization)
            {
                case bool segmentAuthorized:
                    return segmentAuthorized;

                case Dictionary<string, object> endpointAuthorization:
                    if (!endpointAuthorization.TryGetValue(method, out var methodAuthorizationObject))
                    {
                        return true;
                    }

                    return !(methodAuthorizationObject is bool methodAuthorized) || methodAuthorized;
            }

            return true;
        }

    }

}
