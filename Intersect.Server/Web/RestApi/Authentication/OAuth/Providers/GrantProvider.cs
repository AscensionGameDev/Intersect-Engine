using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using Intersect.Logging;
using Intersect.Security.Claims;
using Intersect.Server.Database.PlayerData;
using Microsoft.Owin.Security;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth.Providers
{

    internal class GrantProvider : OAuthAuthorizationServerProvider
    {
        [NotNull] private const string KEY_PREHASH = "intersect:prehash";

        [NotNull]
        private OAuthProvider OAuthProvider { get; }

        public GrantProvider([NotNull] OAuthProvider oAuthProvider)
        {
            OAuthProvider = oAuthProvider;
        }

        //public override async Task GrantClientCredentials([NotNull] OAuthGrantClientCredentialsContext context)
        //{
        //    var identity = new ClaimsIdentity(new GenericIdentity(context.ClientId, OAuthDefaults.AuthenticationType), context.Scope.Select(x => new Claim("urn:oauth:scope", x)));

        //    context.Validated(identity);
        //}

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

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            return base.GrantRefreshToken(context);
        }

        public override async Task GrantResourceOwnerCredentials(
            [NotNull] OAuthGrantResourceOwnerCredentialsContext context)
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
#if DEBUG
            if (!string.IsNullOrEmpty(password) && owinContext.Get<bool>(KEY_PREHASH))
            {
                using (var sha = new SHA256Managed())
                {
                    var digest = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                    password = BitConverter.ToString(digest).Replace("-", "");
                }
            }
#endif

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                context.SetError("credentials_missing");
                return;
            }

            username = username.Trim();

            var user = LegacyDatabase.GetUser(username);
            if (!user?.IsPasswordValid(password) ?? true)
            {
                context.SetError("credentials_invalid");
                return;
            }

            if (!user.Power?.Api ?? true)
            {
                context.SetError("insufficient_permissions");
                return;
            }

#if DEBUG
            owinContext.Set("as:clientRefreshTokenLifetime", 15);
#else
            owinContext.Set("as:clientRefreshTokenLifetime", 10080);
#endif

            if (!Guid.TryParse(context.ClientId, out var clientId))
            {
                Log.Diagnostic("Received invalid client id '{0}'.", context.ClientId);
            }

            var ticketId = Guid.NewGuid().ToString();
            owinContext.Set("ticket_id", ticketId);

            var identity = new ClaimsIdentity(options.AuthenticationType);
            identity.AddClaim(new Claim(IntersectClaimTypes.UserId, user.Id.ToString()));
            identity.AddClaim(new Claim(IntersectClaimTypes.UserName, user.Name));
            identity.AddClaim(new Claim(IntersectClaimTypes.Email, user.Email));
            identity.AddClaim(new Claim(IntersectClaimTypes.ClientId, clientId.ToString()));
            identity.AddClaim(new Claim(IntersectClaimTypes.TicketId, ticketId));

            if (user.Power != null)
            {
                identity.AddClaims(user.Power.Roles.Select(role => new Claim(IntersectClaimTypes.Role, role)));
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
            properties.ForEach(pair =>
            {
                if (!string.IsNullOrWhiteSpace(pair.Key))
                {
                    parameters.Add(pair.Key, pair.Value);
                }
            });

            return Task.FromResult(0);
        }

        public override Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            return base.ValidateAuthorizeRequest(context);
        }

        public override async Task ValidateClientAuthentication(
            [NotNull] OAuthValidateClientAuthenticationContext context)
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
#if DEBUG
                    if (parameters["raw"] != null)
                    {
                        context.OwinContext?.Set(KEY_PREHASH, true);
                    }
#endif
                    context.Validated();
                    return;

                default:
                    context.SetError("grant_type_invalid");
                    return;
            }
        }

        public override async Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            context?.Validated();
        }

        public override async Task ValidateTokenRequest([NotNull] OAuthValidateTokenRequestContext context)
        {
            context.Validated();
        }
    }
}