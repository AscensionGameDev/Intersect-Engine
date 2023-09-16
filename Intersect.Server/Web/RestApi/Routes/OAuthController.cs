using Intersect.Server.Database.PlayerData.Api;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;

namespace Intersect.Server.Web.RestApi.Routes
{
    [Route("api/oauth")]
    // [ConfigurableAuthorize]
    public sealed partial class OAuthController : IntersectController
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
        public async Task<IActionResult> DeleteTokenById(Guid tokenId)
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

            return RefreshToken.Remove(refreshToken) ? Ok(new UsernameAndTokenResponse { TokenId = tokenId }) : InternalServerError();
        }

        [Authorize]
        [HttpDelete]
        [Route("tokens/{username}")]
        public async Task<IActionResult> DeleteTokensForUsername(string username, CancellationToken cancellationToken)
        {
            var actor = IntersectUser;
            if (actor == default)
            {
                return Unauthorized();
            }

            var user = Database.PlayerData.User.Find(username);

            if (user == null)
            {
                return Unauthorized();
            }

            if (!actor.Power.ApiRoles.UserManage && actor.Id != user.Id)
            {
                return Unauthorized();
            }

            if (!RefreshToken.HasTokens(user))
            {
                return Gone();
            }

            var success = await RefreshToken.RemoveForUserAsync(user.Id, cancellationToken).ConfigureAwait(false);
            return success ? Ok(new { Username = username }) : Unauthorized();
        }

        [HttpDelete]
        [Route("tokens/{username}/{tokenId:guid}")]
        public async Task<IActionResult> DeleteTokenForUsernameById(string username, Guid tokenId)
        {
            var intersectUser = IntersectUser;
            if (intersectUser == default)
            {
                return Unauthorized();
            }

            var user = Database.PlayerData.User.Find(username);

            if (user == null)
            {
                return Unauthorized();
            }

            if (intersectUser.Id != user.Id && !intersectUser.Power.ApiRoles.UserManage)
            {
                return Unauthorized();
            }

            if (!RefreshToken.TryFind(tokenId, out _))
            {
                return Gone();
            }

            return !RefreshToken.Remove(tokenId) ? InternalServerError() : Ok(new UsernameAndTokenResponse { TokenId = tokenId, Username = username });
        }
    }
}
