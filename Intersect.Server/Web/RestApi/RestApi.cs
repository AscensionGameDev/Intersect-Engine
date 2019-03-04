using Intersect.Server.Web.RestApi.Authentication;
using Intersect.Server.Web.RestApi.Authentication.OAuth;
using Intersect.Server.Web.RestApi.RouteProviders;
using JetBrains.Annotations;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Intersect.Server.Web.RestApi
{
    internal sealed class RestApi : IDisposable, IAppConfigurationProvider
    {
        [NotNull]
        public static StartOptions DefaultStartOptions => new StartOptions(
#if DEBUG
            "http://localhost:5401/"
#endif
        );

        public bool Disposing { get; private set; }

        public bool Disposed { get; private set; }

        [CanBeNull] private IDisposable mWebAppHandle;

        [NotNull]
        public StartOptions StartOptions { get; }

        [NotNull] private AuthenticationProvider AuthenticationProvider { get; }

        public RestApi() : this(DefaultStartOptions)
        {
        }

        public RestApi([NotNull] StartOptions startOptions)
        {
            StartOptions = startOptions;
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

            AuthenticationProvider.Configure(appBuilder);

            // Map routes
            config.MapHttpAttributeRoutes(new VersionedRouteProvider());
            config.EnsureInitialized();

            Func<IHttpRoute, object> map = null;
            map = route =>
            {
                var flagInternal = BindingFlags.NonPublic | BindingFlags.Instance;
                var subroutes =
                    route?.GetType().GetProperty("SubRoutes", flagInternal)?.GetValue(route) as IReadOnlyCollection<IHttpRoute>;

                //var parsedRoute = route?.GetType().GetProperty("ParsedRoute", flagInternal)?.GetValue(route) as 

                return new
                {
                    routeTemplate = route?.RouteTemplate,
                    subroutes = subroutes?.Select(map).ToList()
                };
            };

            var routeNames = config.Routes?.Select(map).ToList();

            // Make JSON the default response type for browsers
            config.Formatters?.JsonFormatter?.Map("accept", "text/html", "application/json");

            appBuilder.UseWebApi(config);

            appBuilder.ToString();
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
