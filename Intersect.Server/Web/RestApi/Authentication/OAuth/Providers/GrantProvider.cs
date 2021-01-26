using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Intersect.Logging;
using Intersect.Reflection;
using Intersect.Security.Claims;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Web.RestApi.Configuration;

using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth.Providers
{

    internal class GrantProvider : OAuthAuthorizationServerProvider
    {

        public GrantProvider(ApiConfiguration configuration)
        {
            Configuration = configuration;
        }

        private ApiConfiguration Configuration { get; }

        public override Task AuthorizationEndpointResponse(OAuthAuthorizationEndpointResponseContext context)
        {
            return base.AuthorizationEndpointResponse(context);
        }

        public override Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            return base.AuthorizeEndpoint(context);
        }

        public override Task GrantAuthorizationCode(OAuthGrantAuthorizationCodeContext context)
        {
            return base.GrantAuthorizationCode(context);
        }

        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            return base.GrantClientCredentials(context);
        }

        public override Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            return base.GrantCustomExtension(context);
        }

        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            context?.Validated(context.Ticket);
        }

        public override async Task GrantResourceOwnerCredentials(
            OAuthGrantResourceOwnerCredentialsContext context
        )
        {
            var owinContext = context.OwinContext;
            var options = context.Options;

            if (owinContext == null || options == null)
            {
                context.SetError("server_error");
                if (context.Response != null)
                {
                    context.Response.StatusCode = 500;
                }

                return;
            }

            var username = context.UserName;
            var password = context.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                context.SetError("credentials_missing");

                return;
            }

            username = username.Trim();

            var user = User.Find(username);
            if (!user?.IsPasswordValid(password.ToUpper().Trim()) ?? true)
            {
                context.SetError("credentials_invalid");

                return;
            }

            if (!user.Power?.Api ?? true)
            {
                context.SetError("insufficient_permissions");

                return;
            }

            if (!Guid.TryParse(context.ClientId, out var clientId))
            {
                Log.Diagnostic("Received invalid client id '{0}'.", context.ClientId);
            }

            var ticketId = Guid.NewGuid();
            owinContext.Set("ticket_id", ticketId);

            var identity = new ClaimsIdentity(options.AuthenticationType);
            identity.AddClaim(new Claim(IntersectClaimTypes.UserId, user.Id.ToString()));
            identity.AddClaim(new Claim(IntersectClaimTypes.UserName, user.Name));
            identity.AddClaim(new Claim(IntersectClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(IntersectClaimTypes.ClientId, clientId.ToString()));
            identity.AddClaim(new Claim(IntersectClaimTypes.TicketId, ticketId.ToString()));

            if (user.Power != null)
            {
                identity.AddClaims(user.Power.Roles.Select(role => new Claim(IntersectClaimTypes.Role, role)));
                if (user.Power.ApiRoles?.UserQuery ?? false)
                {
                    identity.AddClaim(new Claim(IntersectClaimTypes.AccessRead, typeof(User).FullName));
                    identity.AddClaim(
                        new Claim(
                            IntersectClaimTypes.AccessRead, typeof(User).GetProperty(nameof(User.Ban))?.GetFullName()
                        )
                    );

                    identity.AddClaim(
                        new Claim(
                            IntersectClaimTypes.AccessRead, typeof(User).GetProperty(nameof(User.Mute))?.GetFullName()
                        )
                    );

                    identity.AddClaim(
                        new Claim(
                            IntersectClaimTypes.AccessRead,
                            typeof(User).GetProperty(nameof(User.IsBanned))?.GetFullName()
                        )
                    );

                    identity.AddClaim(
                        new Claim(
                            IntersectClaimTypes.AccessRead,
                            typeof(User).GetProperty(nameof(User.IsMuted))?.GetFullName()
                        )
                    );
                }
            }

            var ticketProperties = new AuthenticationProperties();
            var ticket = new AuthenticationTicket(identity, ticketProperties);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            if (context?.Properties?.Dictionary == null || context.AdditionalResponseParameters == null)
            {
                return Task.FromResult(0);
            }

            var properties = context.Properties.Dictionary.ToList();
            var parameters = context.AdditionalResponseParameters;
            properties.ForEach(
                pair =>
                {
                    if (!string.IsNullOrWhiteSpace(pair.Key))
                    {
                        parameters.Add(pair.Key, pair.Value);
                    }
                }
            );

            return Task.FromResult(0);
        }

        public override async Task ValidateClientAuthentication(
            OAuthValidateClientAuthenticationContext context
        )
        {
            var parameters = context.Parameters;
            var owinContext = context.OwinContext;

            if (parameters == null || owinContext == null)
            {
                context.SetError("server_error");

                if (context.Response != null)
                {
                    context.Response.StatusCode = 500;
                }

                return;
            }

            var grantType = parameters["grant_type"]?.Trim();

            if (string.IsNullOrWhiteSpace(grantType))
            {
                context.SetError("grant_type_missing");

                return;
            }

            switch (grantType)
            {
                case "password":
                    context.Validated();

                    return;

                case "refresh_token":
                    context.Validated();

                    return;

                default:
                    context.SetError("grant_type_invalid");

                    return;
            }
        }

    }

}
