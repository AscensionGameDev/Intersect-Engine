using System.Web.Http;
using System.Web.Http.Controllers;

using Intersect.Server.Web.RestApi.Serialization;

namespace Intersect.Server.Web.RestApi
{

    public abstract class IntersectApiController : ApiController
    {

        public const int PAGE_SIZE_MAX = 100;

        public const int PAGE_SIZE_MIN = 5;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            if (RequestContext != null && Configuration?.Formatters?.JsonFormatter?.SerializerSettings != null)
            {
                Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                    new ApiVisibilityContractResolver(RequestContext);
            }
        }

    }

}
