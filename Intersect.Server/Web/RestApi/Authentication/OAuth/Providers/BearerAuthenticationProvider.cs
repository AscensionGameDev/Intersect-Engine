using System;
using System.Threading.Tasks;

using Intersect.Security.Claims;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Api;

using Microsoft.Owin.Security.OAuth;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth.Providers
{

    public partial class BearerAuthenticationProvider : OAuthBearerAuthenticationProvider
    {

        public override async Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            var owinContext = context.OwinContext;

            var ticket = context.Ticket;
            var identity = ticket?.Identity;
            if (identity == null || owinContext == null)
            {
                context.Rejected();

                return;
            }

            var claimClientId = identity.FindFirst(IntersectClaimTypes.ClientId);
            if (!Guid.TryParse(claimClientId?.Value, out var clientId))
            {
                context.SetError("invalid_token_client");

                return;
            }

            var claimUserId = identity.FindFirst(IntersectClaimTypes.UserId);
            if (!Guid.TryParse(claimUserId?.Value, out var userId))
            {
                context.SetError("invalid_token_user");

                return;
            }

            var ban = Ban.Find(userId);
            if (ban != default)
            {
                context.Rejected();
                return;
            }

            var claimTicketId = identity.FindFirst(IntersectClaimTypes.TicketId);
            if (!Guid.TryParse(claimTicketId?.Value, out var ticketId))
            {
                context.SetError("invalid_ticket_id");

                return;
            }

            var refreshToken = RefreshToken.FindForTicket(ticketId);
            if (refreshToken == null)
            {
                context.Rejected();

                return;
            }

            if (ticket.Properties?.ExpiresUtc < DateTime.UtcNow)
            {
                context.SetError("access_token_expired");

                return;
            }

            if (refreshToken.ClientId != clientId || refreshToken.UserId != userId)
            {
                _ = RefreshToken.Remove(refreshToken);
                context.Rejected();

                return;
            }

            owinContext.Set("refresh_token", refreshToken);
            context.Validated();
        }

    }

}
