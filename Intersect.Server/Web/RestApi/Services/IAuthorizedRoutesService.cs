using System.Web.Http.Dependencies;

namespace Intersect.Server.Web.RestApi.Services
{

    internal interface IAuthorizedRoutesService
    {

        bool RequiresAuthorization(string endpoint, string method = "GET");

    }

    internal static class ServiceCollectionExtensions
    {

        public static IAuthorizedRoutesService GetAuthorizedRoutes(
            this IDependencyResolver dependencyResolver
        )
        {
            return dependencyResolver.GetService(typeof(IAuthorizedRoutesService)) as IAuthorizedRoutesService;
        }

    }

}
