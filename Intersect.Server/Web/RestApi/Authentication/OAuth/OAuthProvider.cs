using System;
using Intersect.Server.Web.RestApi.Authentication.OAuth.Providers;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Owin.Security.AesDataProtectorProvider;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth
{
    internal class OAuthProvider : AuthenticationProvider
    {
        [NotNull] private OAuthAuthorizationServerProvider OAuthAuthorizationServerProvider { get; }
        [NotNull] private AuthenticationTokenProvider RefreshTokenProvider { get; }

        public OAuthProvider([NotNull] RestApi restApi) : base(restApi)
        {
            OAuthAuthorizationServerProvider = new GrantProvider(this);
            RefreshTokenProvider = new RefreshTokenProvider(this);
        }

        public override void Configure(IAppBuilder appBuilder)
        {
            appBuilder.UseAesDataProtectorProvider();

            appBuilder.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/oauth/token"),
                ApplicationCanDisplayErrors = true,
#if DEBUG
                AllowInsecureHttp = true,
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(5),
#else
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60),
#endif
                Provider = OAuthAuthorizationServerProvider,
                RefreshTokenProvider = RefreshTokenProvider
            });

            appBuilder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                Provider = new BearerAuthenticationProvider()
            });
        }
    }
}