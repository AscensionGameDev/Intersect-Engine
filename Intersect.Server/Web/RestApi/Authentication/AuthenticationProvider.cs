using Intersect.Server.Web.RestApi.Configuration;

using JetBrains.Annotations;

using Owin;

namespace Intersect.Server.Web.RestApi.Authentication
{

    internal abstract class AuthenticationProvider : IAppConfigurationProvider
    {

        protected AuthenticationProvider([NotNull] ApiConfiguration configuration)
        {
            Configuration = configuration;
        }

        [NotNull]
        protected ApiConfiguration Configuration { get; }

        public abstract void Configure(IAppBuilder appBuilder);

    }

}
