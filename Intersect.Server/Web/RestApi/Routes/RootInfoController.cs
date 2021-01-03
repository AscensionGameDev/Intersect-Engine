using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Intersect.Server.Web.RestApi.Routes
{

    [RoutePrefix("info")]
    public class RootInfoController : ApiController
    {

        static RootInfoController()
        {
            var rootNamespace = typeof(RootInfoController).Namespace ?? throw new InvalidOperationException();
            DiscoveredVersions = typeof(RootInfoController).Assembly.GetTypes()
                .Select(type => type.Namespace)
                .Distinct()
                .Where(ns => ns?.StartsWith(rootNamespace) ?? false)
                .Select(ns => ns.Substring(Math.Min(ns.Length, rootNamespace.Length + 1)).ToLowerInvariant())
                .Where(ns => !string.IsNullOrWhiteSpace(ns))
                .ToArray();
        }

        private static string[] DiscoveredVersions { get; }

        [Route]
        [HttpGet]
        public object Default()
        {
            return new
            {
#if DEBUG
                debug = true,
#endif
                versions = Versions()
            };
        }

        [Route("versions")]
        [HttpGet]
        public IEnumerable<string> Versions()
        {
            return DiscoveredVersions;
        }

    }

}
