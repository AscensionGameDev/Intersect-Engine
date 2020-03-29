using Owin;

namespace Intersect.Server.Web.RestApi
{

    internal interface IAppConfigurationProvider
    {

        void Configure(IAppBuilder appBuilder);

    }

}
