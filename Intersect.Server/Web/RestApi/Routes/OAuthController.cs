using Intersect.Server.Classes.Database.PlayerData.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

using Intersect.Server.Database.PlayerData;
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
        [Route("token/{username}")]
        public async Task<IHttpActionResult> DeleteToken(string username)
        {
            User user;

            var context = PlayerContext.Current;
            user = Database.PlayerData.User.Find(username, context);

            if (user == null)
            {
                return Unauthorized();
            }

            var refreshToken = (await RefreshToken.FindForUser(user)).FirstOrDefault();

            if (refreshToken == null)
            {
                return StatusCode(HttpStatusCode.Gone);
            }

            if (await RefreshToken.Remove(refreshToken, true))
            {
                return Ok(new
                {
                    username
                });
            }

            return StatusCode(HttpStatusCode.Gone);
        }

        [AllowAnonymous]
        [HttpDelete]
        [Route("token/{username}/{tokenId:guid}")]
        public async Task<IHttpActionResult> DeleteToken(string username, Guid tokenId)
        {
            User user;

            var context = PlayerContext.Current;
            user = Database.PlayerData.User.Find(username, context);

            if (user == null)
            {
                return Unauthorized();
            }

            var refreshToken = (await RefreshToken.FindForUser(user)).FirstOrDefault();

            if (refreshToken?.Id != tokenId)
            {
                return Unauthorized();
            }

            if (await RefreshToken.Remove(refreshToken, true))
            {
                return Ok(new
                {
                    username,
                    tokenId
                });
            }

            return StatusCode(HttpStatusCode.Gone);
        }
    }
}
