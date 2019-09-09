using Intersect.Server.Web.RestApi.Configuration;

using JetBrains.Annotations;
using Owin;

namespace Intersect.Server.Web.RestApi.Authentication
{
    internal abstract class AuthenticationProvider : IAppConfigurationProvider
    {
        [NotNull] protected ApiConfiguration Configuration { get; }

        protected AuthenticationProvider([NotNull] ApiConfiguration configuration)
        {
            Configuration = configuration;
        }

        public abstract void Configure(IAppBuilder appBuilder);
    }
}
