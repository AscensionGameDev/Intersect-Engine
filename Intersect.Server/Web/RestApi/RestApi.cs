using Intersect.Configuration;
using Intersect.Enums;
using Intersect.Logging;
using Intersect.Server.Localization;
using Intersect.Server.Web.RestApi.Authentication;
using Intersect.Server.Web.RestApi.Authentication.OAuth;
using Intersect.Server.Web.RestApi.Configuration;
using Intersect.Server.Web.RestApi.Constraints;
using Intersect.Server.Web.RestApi.Logging;
using Intersect.Server.Web.RestApi.RouteProviders;
using Intersect.Server.Web.RestApi.Services;
using Intersect.Server.Web.RestApi.Payloads;
using JetBrains.Annotations;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Owin;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Intersect.Server.Web.RestApi
{

    internal sealed class RestApi : IAppConfigurationProvider, IConfigurable<ApiConfiguration>, IDisposable
    {

        [NotNull]
        public ApiConfiguration Configuration { get; }

        public bool Disposing { get; private set; }

        public bool Disposed { get; private set; }

        public bool IsStarted => mWebAppHandle != null;

        [CanBeNull] private IDisposable mWebAppHandle;

        [NotNull]
        public StartOptions StartOptions { get; }

        [NotNull]
        private AuthenticationProvider AuthenticationProvider { get; }

        public RestApi(ushort apiPort)
        {
            StartOptions = new StartOptions();

            Configuration = ApiConfiguration.Create();

            Configuration.Hosts.ToList().ForEach(host => StartOptions.Urls?.Add(host));

            if (apiPort > 0)
            {
                StartOptions.Urls.Clear();
                StartOptions.Urls.Add("http://*:" + apiPort + "/");
            }

            AuthenticationProvider = new OAuthProvider(Configuration);
        }

        public void Start()
        {
            if (!Configuration.Enabled)
            {
                return;
            }

            try
            {
                mWebAppHandle = WebApp.Start(StartOptions, Configure);
                System.Diagnostics.Trace.Listeners.Remove("HostingTraceListener");
                StartOptions.Urls.ToList().ForEach(host => Console.WriteLine(Strings.Intro.api.ToString(host)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Intro.apifailed);
                Log.Error(Strings.Intro.apifailed + Environment.NewLine + ex.ToString());
            }
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

            Configuration.Cors
                .Select(configuration => configuration.AsCorsOptions())?
                .ToList()
                .ForEach(corsOptions => appBuilder.UseCors(corsOptions));

            AuthenticationProvider.Configure(appBuilder);

            var constraintResolver = new DefaultInlineConstraintResolver();
            constraintResolver.ConstraintMap?.Add(nameof(AdminActions), typeof(AdminActionsConstraint));
            constraintResolver.ConstraintMap?.Add(nameof(LookupKey), typeof(LookupKey.Constraint));
            constraintResolver.ConstraintMap?.Add(nameof(ChatMessage), typeof(ChatMessage.Constraint));

            // Map routes
            config.MapHttpAttributeRoutes(constraintResolver, new VersionedRouteProvider());
            config.DependencyResolver = new IntersectServiceDependencyResolver(Configuration, config);

            // Make JSON the default response type for browsers
            config.Formatters?.JsonFormatter?.Map("accept", "text/html", "application/json");

            if (Configuration.DebugMode)
            {
                appBuilder.SetLoggerFactory(new IntersectLoggerFactory());
            }

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
