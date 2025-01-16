using Microsoft.AspNetCore.Mvc;

namespace Intersect.Server.Web.Controllers.Api
{
    [Route("api/info")]
    public partial class RootInfoController : Controller
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
            DiscoveredVersions.ToString();
        }

        private static string[] DiscoveredVersions { get; }

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

        [HttpGet("versions")]
        public IEnumerable<string> Versions()
        {
            return DiscoveredVersions;
        }
    }
}
