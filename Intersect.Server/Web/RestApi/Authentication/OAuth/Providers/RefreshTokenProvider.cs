using Intersect.Security.Claims;
using Intersect.Server.Classes.Database.PlayerData.Api;
using JetBrains.Annotations;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Threading.Tasks;

using Intersect.Logging;

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

            var properties = context.Ticket?.Properties?.Dictionary;
            if (properties == null)
            {
                return;
            }

            if (!Guid.TryParse(properties["client_id"], out var clientId))
            {
                Log.Diagnostic("Received invalid client id '{0}'.", properties["as:client_id"]);
            }

            var identity = context.Ticket.Identity;
            if (identity == null)
            {
                return;
            }

            var identifier = identity.FindFirst(IntersectClaimTypes.UserId)?.Value;
            if (!Guid.TryParse(identifier, out var userId))
            {
                return;
            }

            var userName = identity.FindFirst(IntersectClaimTypes.UserName)?.Value;

            var issued = DateTime.UtcNow;
            var tokenLifeTime = context.OwinContext.Get<int>("as:clientRefreshTokenLifeTime");
            var expires = issued.AddMinutes(tokenLifeTime);

            var token = new RefreshToken
            {
                UserId = userId,
                ClientId = clientId,
                Subject = userName,
                Issued = issued,
                Expires = expires
            };

            context.Ticket.Properties.IssuedUtc = issued;
            context.Ticket.Properties.ExpiresUtc = expires;

            token.Ticket = context.SerializeTicket();

            if (await RefreshToken.Add(token, true))
            {
                context.SetToken(token.Id.ToString());
            }
        }

        public override Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            return base.ReceiveAsync(context);
        }

        public override void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}
