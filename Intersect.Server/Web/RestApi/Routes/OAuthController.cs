using System;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Web.RestApi.Attributes;

namespace Intersect.Server.Web.RestApi.Routes
{

    [RoutePrefix("oauth")]
    [ConfigurableAuthorize]
    public sealed partial class OAuthController : IntersectApiController
    {

        private class UsernameAndTokenResponse
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public Guid TokenId { get; set; } = default;

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string Username { get; set; } = default;
        }

        [HttpDelete]
        [Route("tokens/{tokenId:guid}")]
        public async Task<IHttpActionResult> DeleteTokenById(Guid tokenId)
        {
            var actor = IntersectUser;
            if (actor == default)
            {
                return Unauthorized();
            }

            if (!RefreshToken.TryFind(tokenId, out var refreshToken))
            {
                return Unauthorized();
            }

            if (refreshToken.Id != tokenId)
            {
                return InternalServerError();
            }

            if (refreshToken.UserId != actor.Id && !actor.Power.ApiRoles.UserManage)
            {
                return Unauthorized();
            }

            if (!RefreshToken.Remove(refreshToken))
            {
                return InternalServerError();
            }

            return Ok(new UsernameAndTokenResponse { TokenId = tokenId });
        }

        [Authorize]
        [HttpDelete]
        [Route("tokens/{username}")]
        public async Task<IHttpActionResult> DeleteTokensForUsername(string username, CancellationToken cancellationToken)
        {
            var user = Database.PlayerData.User.Find(username);

            if (user == null)
            {
                return Unauthorized();
            }

            var actor = IntersectUser;
            if (actor == default)
            {
                return Unauthorized();
            }

            if (!actor.Power.ApiRoles.UserManage && actor.Id != user.Id)
            {
                return Unauthorized();
            }

            if (!RefreshToken.HasTokens(user))
            {
                return StatusCode(HttpStatusCode.Gone);
            }

            var success = await RefreshToken.RemoveForUserAsync(user.Id, cancellationToken).ConfigureAwait(false);
            return success ? (IHttpActionResult)Ok(new { Username = username }) : StatusCode(HttpStatusCode.Unauthorized);
        }

        [HttpDelete]
        [Route("tokens/{username}/{tokenId:guid}")]
        public async Task<IHttpActionResult> DeleteTokenForUsernameById(string username, Guid tokenId)
        {
            var user = Database.PlayerData.User.Find(username);

            if (user == null)
            {
                return Unauthorized();
            }

            if (IntersectUser?.Id != user.Id && !IntersectUser.Power.ApiRoles.UserManage)
            {
                return Unauthorized();
            }

            if (!RefreshToken.TryFind(tokenId, out _))
            {
                return StatusCode(HttpStatusCode.Gone);
            }

            if (!RefreshToken.Remove(tokenId))
            {
                return InternalServerError();
            }

            return Ok(new UsernameAndTokenResponse { TokenId = tokenId, Username = username });
        }

    }

}
