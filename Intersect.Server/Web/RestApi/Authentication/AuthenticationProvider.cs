using JetBrains.Annotations;
using Owin;

namespace Intersect.Server.Web.RestApi.Authentication
{
    internal abstract class AuthenticationProvider : IAppConfigurationProvider
    {
        [NotNull] protected RestApi RestApi { get; }

        protected AuthenticationProvider([NotNull] RestApi restApi)
        {
            RestApi = restApi;
        }

        public abstract void Configure(IAppBuilder appBuilder);
    }
}
