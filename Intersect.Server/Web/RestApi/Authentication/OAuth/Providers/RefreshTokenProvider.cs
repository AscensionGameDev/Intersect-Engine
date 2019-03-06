using Intersect.Logging;
using Intersect.Security.Claims;
using Intersect.Server.Classes.Database.PlayerData.Api;
using JetBrains.Annotations;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth.Providers
{

    internal class RefreshTokenProvider : AuthenticationTokenProvider
    {
        [NotNull] private OAuthProvider OAuthProvider { get; }

        public RefreshTokenProvider([NotNull] OAuthProvider oAuthProvider)
        {
            OAuthProvider = oAuthProvider;
        }

        public override async Task CreateAsync([NotNull] AuthenticationTokenCreateContext context)
        {
            if (context.OwinContext == null)
            {
                return;
            }

            var properties = context.Ticket?.Properties;
            if (properties == null)
            {
                return;
            }

            var identity = context.Ticket.Identity;
            if (identity == null)
            {
                return;
            }

            if (!Guid.TryParse(identity.FindFirst(IntersectClaimTypes.ClientId)?.Value, out var clientId))
            {
                Log.Diagnostic("Received invalid client id '{0}'.", identity.FindFirst(IntersectClaimTypes.UserId)?.Value);
            }

            var identifier = identity.FindFirst(IntersectClaimTypes.UserId)?.Value;
            if (!Guid.TryParse(identifier, out var userId))
            {
                return;
            }

            var userName = identity.FindFirst(IntersectClaimTypes.UserName)?.Value;

            var issued = DateTime.UtcNow;
            var tokenLifeTime = context.OwinContext.Get<int>("as:clientRefreshTokenLifetime");
            var ticketId = context.OwinContext.Get<Guid>("ticket_id");
            var expires = issued.AddMinutes(tokenLifeTime);

            var token = new RefreshToken
            {
                UserId = userId,
                ClientId = clientId,
                Subject = userName,
                Issued = issued,
                Expires = expires,
                TicketId = ticketId
            };

            properties.IssuedUtc = issued;
            properties.ExpiresUtc = expires;

            token.Ticket = context.SerializeTicket();

            if (await RefreshToken.Add(token, true))
            {
                context.SetToken(token.Id.ToString());
            }
        }

        public override async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            if (!Guid.TryParse(context.Token, out var refreshTokenId))
            {
                return;
            }

            var refreshToken = await RefreshToken.Find(refreshTokenId);

            if (refreshToken == null)
            {
                return;
            }

            context.DeserializeTicket(refreshToken.Ticket);

            context.Ticket.ToString();
        }
    }
}
