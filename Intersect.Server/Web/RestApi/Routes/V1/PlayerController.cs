using System;
using System.Web.Http;
using Intersect.Server.General;
using Intersect.Server.Web.RestApi.Attributes;

namespace Intersect.Server.Web.RestApi.Routes.V1
{
    [RoutePrefix("players")]
    [ConfigurableAuthorize]
    public sealed class PlayerController : ApiController
    {
        [Route]
        [HttpGet]
        public object List()
        {
            // TODO: Implement player listing with pagination
            return new
            {
            };
        }

        [Route("online")]
        [HttpGet]
        public object Online()
        {
            // TODO: Implement user listing with pagination
            return new
            {
            };
        }

        [Route("online/count")]
        [HttpGet]
        public int OnlineCount()
        {
            return Globals.OnlineList?.Count ?? 0;
        }

        [Route("{playerName}")]
        [HttpGet]
        public object PlayerByName(string playerName)
        {
            return LegacyDatabase.GetCharacter(playerName);
        }

        [Route("{playerId:guid}")]
        [HttpGet]
        public object PlayerById(Guid playerId)
        {
            return LegacyDatabase.GetCharacter(playerId);
        }
    }
}