using Intersect.Server.Classes.Database.PlayerData.Api;
using System;
using System.Net;
using System.Web.Http;

using Intersect.Server.Web.RestApi.Attributes;

namespace Intersect.Server.Web.RestApi.Routes
{
    [RoutePrefix("oauth")]
    [ConfigurableAuthorize]
    public sealed class OAuthController : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("token")]
        public IHttpActionResult Token(
            [FromBody] string grant_type,
            [FromBody] string username,
            [FromBody] string password
#if DEBUG
            ,
            [FromBody] string prehash
#endif
        )
        {
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("token")]
        public IHttpActionResult Token(
            [FromBody] string grant_type,
            [FromBody] string refresh_token
        )
        {
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [Route("token/{tokenId:guid}")]
        public IHttpActionResult DeleteToken(Guid tokenId)
        {
            if (User?.Identity == null)
            {
                return Unauthorized();
            }

            if (RefreshToken.Remove(tokenId, true).GetAwaiter().GetResult())
            {
                return Ok(new
                {
                    revoked_refresh_token = tokenId
                });
            }

            return StatusCode(HttpStatusCode.Gone);
        }
    }
}
