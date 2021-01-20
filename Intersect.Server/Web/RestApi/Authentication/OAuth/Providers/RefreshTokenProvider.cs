using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Intersect.Logging;
using Intersect.Security.Claims;
using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Web.RestApi.Configuration;

using Microsoft.Owin.Security.Infrastructure;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth.Providers
{

    internal class RefreshTokenProvider : AuthenticationTokenProvider
    {

        public RefreshTokenProvider(ApiConfiguration configuration)
        {
            Configuration = configuration;
        }

        private ApiConfiguration Configuration { get; }

        public override async Task CreateAsync(AuthenticationTokenCreateContext context)
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
                Log.Diagnostic(
                    "Received invalid client id '{0}'.", identity.FindFirst(IntersectClaimTypes.UserId)?.Value
                );
            }

            var identifier = identity.FindFirst(IntersectClaimTypes.UserId)?.Value;
            if (!Guid.TryParse(identifier, out var userId))
            {
                return;
            }

            var userName = identity.FindFirst(IntersectClaimTypes.UserName)?.Value;

            var issued = DateTime.UtcNow;
            var expires = issued.AddMinutes(Configuration.RefreshTokenLifetime);

            var ticketId = context.OwinContext.Get<Guid>("ticket_id");
            if (ticketId == Guid.Empty)
            {
                identity.FindAll(IntersectClaimTypes.TicketId)
                    ?.ToList()
                    .ForEach(
                        claim =>
                        {
                            if (!Guid.TryParse(claim?.Value, out var guid) || guid == Guid.Empty)
                            {
                                identity.TryRemoveClaim(claim);
                            }
                        }
                    );

                var ticketIdClaim = identity.FindFirst(IntersectClaimTypes.TicketId);
                if (ticketIdClaim == null || !Guid.TryParse(ticketIdClaim.Value, out ticketId))
                {
                    ticketId = Guid.NewGuid();
                    identity.AddClaim(new Claim(IntersectClaimTypes.TicketId, ticketId.ToString()));
                }
            }

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
            if (!Guid.TryParse(context?.Token, out var refreshTokenId))
            {
                return;
            }

            var refreshToken = RefreshToken.Find(refreshTokenId);

            if (refreshToken == null)
            {
                return;
            }

            // TODO: Require username

            context?.DeserializeTicket(refreshToken.Ticket);
        }

    }

}
