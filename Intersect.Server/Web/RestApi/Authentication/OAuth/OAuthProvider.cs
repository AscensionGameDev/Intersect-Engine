using System;
using System.Threading.Tasks;
using Intersect.Server.Classes.Database.PlayerData.Api;
using Intersect.Server.Web.RestApi.Authentication.OAuth.Providers;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth
{
    internal class OAuthProvider : AuthenticationProvider
    {
        [NotNull] private OAuthAuthorizationServerProvider OAuthAuthorizationServerProvider { get; }
        [NotNull] private AuthenticationTokenProvider AccessTokenProvider { get; }
        [NotNull] private AuthenticationTokenProvider RefreshTokenProvider { get; }

        public OAuthProvider([NotNull] RestApi restApi) : base(restApi)
        {
            OAuthAuthorizationServerProvider = new GrantProvider(this);
            AccessTokenProvider = new AccessTokenProvider(this);
            RefreshTokenProvider = new RefreshTokenProvider(this);
        }

        public override void Configure(IAppBuilder appBuilder)
        {
            var oauthOptions = new OAuthAuthorizationServerOptions
            {
                //AuthorizeEndpointPath = new PathString("/api/oauth/authorize"),
                TokenEndpointPath = new PathString("/api/oauth/token"),
                ApplicationCanDisplayErrors = true,
#if DEBUG
                AllowInsecureHttp = true,
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(5),
#else
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
#endif

                Provider = OAuthAuthorizationServerProvider,
                AuthorizationCodeProvider = AccessTokenProvider,
                RefreshTokenProvider = RefreshTokenProvider

            };

            appBuilder.UseOAuthAuthorizationServer(oauthOptions);
            appBuilder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        public async Task<bool> AddRefreshToken(RefreshToken refreshToken)
        {
            return false;
        }
    }
}