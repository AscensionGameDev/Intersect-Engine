using Intersect.Server.Web.RestApi.Authentication;
using Intersect.Server.Web.RestApi.Authentication.OAuth;
using Intersect.Server.Web.RestApi.Configuration;
using Intersect.Server.Web.RestApi.RouteProviders;
using Intersect.Server.Web.RestApi.Services;
using JetBrains.Annotations;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Linq;
using System.Web.Http;

using Intersect.Logging;

namespace Intersect.Server.Web.RestApi
{

    internal sealed class RestApi : IDisposable, IAppConfigurationProvider
    {

        [NotNull]
        public ApiConfiguration ApiConfiguration { get; }

        public bool Disposing { get; private set; }

        public bool Disposed { get; private set; }

        [CanBeNull] private IDisposable mWebAppHandle;

        [NotNull]
        public StartOptions StartOptions { get; }

        [NotNull]
        private AuthenticationProvider AuthenticationProvider { get; }

        public RestApi()
        {
            StartOptions = new StartOptions();

            ApiConfiguration = ApiConfiguration.Load() ?? throw new InvalidOperationException();
            ApiConfiguration.Hosts.ToList().ForEach(host => StartOptions.Urls?.Add(host));
            if (!ApiConfiguration.Save(ApiConfiguration))
            {
                Log.Warn("Failed to save API configuration to disk.");
            }

            AuthenticationProvider = new OAuthProvider(this);
        }

        public void Start()
        {
            mWebAppHandle = WebApp.Start(StartOptions, Configure);
        }

        public void Configure(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();

            var services = config.Services;
            if (services == null)
            {
                throw new InvalidOperationException();
            }

            AuthenticationProvider.Configure(appBuilder);

            // Map routes
            config.MapHttpAttributeRoutes(new VersionedRouteProvider());
            config.DependencyResolver = new IntersectServiceDependencyResolver(ApiConfiguration, config);

            // Make JSON the default response type for browsers
            config.Formatters?.JsonFormatter?.Map("accept", "text/html", "application/json");

            appBuilder.UseWebApi(config);
        }

        public void Dispose()
        {
            lock (this)
            {
                if (Disposed || Disposing)
                {
                    return;
                }

                Disposing = true;
            }

            mWebAppHandle?.Dispose();
            Disposed = true;
        }

    }

}
