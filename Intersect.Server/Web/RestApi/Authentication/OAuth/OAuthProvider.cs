using System;
using System.Collections.Generic;

using Intersect.Server.Web.RestApi.Authentication.OAuth.Providers;
using Intersect.Server.Web.RestApi.Configuration;
using Intersect.Server.Web.RestApi.Middleware;

using JetBrains.Annotations;

using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;

using Owin;
using Owin.Security.AesDataProtectorProvider;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth
{

    using RequestMap = Dictionary<(PathString, string, string), RequestMapFunc>;

    internal class OAuthProvider : AuthenticationProvider
    {

        public const string TokenEndpoint = "/api/oauth/token";

        public OAuthProvider([NotNull] ApiConfiguration configuration) : base(configuration)
        {
            OAuthAuthorizationServerProvider = new GrantProvider(Configuration);
            RefreshTokenProvider = new RefreshTokenProvider(Configuration);
        }

        [NotNull]
        private OAuthAuthorizationServerProvider OAuthAuthorizationServerProvider { get; }

        [NotNull]
        private AuthenticationTokenProvider RefreshTokenProvider { get; }

        public override void Configure([NotNull] IAppBuilder appBuilder)
        {
            appBuilder.UseAesDataProtectorProvider();

            appBuilder.UseContentTypeMappingMiddleware(
                new RequestMap
                {
                    {
                        (new PathString(TokenEndpoint), "POST", "application/json"),
                        async owinContext =>
                            await (owinContext?.ConvertFromJsonToFormBody() ??
                                   throw new InvalidOperationException(@"Task is null"))
                    }
                }
            );

            appBuilder.UseOAuthAuthorizationServer(
                new OAuthAuthorizationServerOptions
                {
                    TokenEndpointPath = new PathString(TokenEndpoint),
                    ApplicationCanDisplayErrors = true,
                    AllowInsecureHttp = true,
                    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(Configuration.RefreshTokenLifetime),
                    Provider = OAuthAuthorizationServerProvider,
                    RefreshTokenProvider = RefreshTokenProvider
                }
            );

            appBuilder.UseOAuthBearerAuthentication(
                new OAuthBearerAuthenticationOptions
                {
                    Provider = new BearerAuthenticationProvider()
                }
            );
        }

    }

}
