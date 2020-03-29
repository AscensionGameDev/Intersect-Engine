using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Web.RestApi.Attributes;

namespace Intersect.Server.Web.RestApi.Routes
{

    [RoutePrefix("oauth")]
    [ConfigurableAuthorize]
    public sealed class OAuthController : ApiController
    {

        [Authorize]
        [HttpDelete]
        [Route("token/{username}")]
        public async Task<IHttpActionResult> DeleteToken(string username)
        {
            User user;

            user = Database.PlayerData.User.Find(username);

            if (user == null)
            {
                return Unauthorized();
            }

            var refreshToken = RefreshToken.FindForUser(user).FirstOrDefault();

            if (refreshToken == null)
            {
                return StatusCode(HttpStatusCode.Gone);
            }

            if (RefreshToken.Remove(refreshToken, true))
            {
                return Ok(
                    new
                    {
                        username
                    }
                );
            }

            return StatusCode(HttpStatusCode.Gone);
        }

        [AllowAnonymous]
        [HttpDelete]
        [Route("token/{username}/{tokenId:guid}")]
        public async Task<IHttpActionResult> DeleteToken(string username, Guid tokenId)
        {
            User user;

            user = Database.PlayerData.User.Find(username);

            if (user == null)
            {
                return Unauthorized();
            }

            var refreshToken = RefreshToken.FindForUser(user).FirstOrDefault();

            if (refreshToken?.Id != tokenId)
            {
                return Unauthorized();
            }

            if (RefreshToken.Remove(refreshToken, true))
            {
                return Ok(
                    new
                    {
                        username,
                        tokenId
                    }
                );
            }

            return StatusCode(HttpStatusCode.Gone);
        }

    }

}
