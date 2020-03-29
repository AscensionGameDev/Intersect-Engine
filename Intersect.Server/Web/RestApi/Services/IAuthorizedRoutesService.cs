using System.Web.Http.Dependencies;

using JetBrains.Annotations;

namespace Intersect.Server.Web.RestApi.Services
{

    internal interface IAuthorizedRoutesService
    {

        bool RequiresAuthorization([NotNull] string endpoint, [NotNull] string method = "GET");

    }

    internal static class ServiceCollectionExtensions
    {

        public static IAuthorizedRoutesService GetAuthorizedRoutes(
            [NotNull] this IDependencyResolver dependencyResolver
        )
        {
            return dependencyResolver.GetService(typeof(IAuthorizedRoutesService)) as IAuthorizedRoutesService;
        }

    }

}
