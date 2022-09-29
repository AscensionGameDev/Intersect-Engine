using System;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

using Intersect.Security.Claims;
using Intersect.Server.Web.RestApi.Serialization;

using IntersectUser = Intersect.Server.Database.PlayerData.User;

namespace Intersect.Server.Web.RestApi
{

    public abstract partial class IntersectApiController : ApiController
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

        private IntersectUser _intersectUser;

        public IntersectUser IntersectUser
        {
            get
            {
                if (_intersectUser != default)
                {
                    return _intersectUser;
                }

                var identity = User.Identity as ClaimsIdentity;
                var idString = identity?.FindFirst(IntersectClaimTypes.UserId)?.Value;
                if (string.IsNullOrWhiteSpace(idString) || !Guid.TryParse(idString, out var id))
                {
                    return default;
                }

                _intersectUser = IntersectUser.FindOnline(id) ?? IntersectUser.Find(id);

                return _intersectUser;
            }
        }

    }

}
