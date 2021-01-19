using Intersect.Server.Web.RestApi.Configuration;

using Owin;

namespace Intersect.Server.Web.RestApi.Authentication
{

    internal abstract class AuthenticationProvider : IAppConfigurationProvider
    {

        protected AuthenticationProvider(ApiConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected ApiConfiguration Configuration { get; }

        public abstract void Configure(IAppBuilder appBuilder);

    }

}
