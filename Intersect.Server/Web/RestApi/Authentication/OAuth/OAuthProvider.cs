using Intersect.Server.Web.RestApi.Authentication.OAuth.Providers;
using Intersect.Server.Web.RestApi.Configuration;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Owin.Security.AesDataProtectorProvider;
using System;
using System.Collections.Generic;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth
{

    using RequestMap = Dictionary<(PathString, string, string), RequestMapFunc>;

    internal class OAuthProvider : AuthenticationProvider
    {

        [NotNull]
        private OAuthAuthorizationServerProvider OAuthAuthorizationServerProvider { get; }

        [NotNull]
        private AuthenticationTokenProvider RefreshTokenProvider { get; }

        public OAuthProvider([NotNull] ApiConfiguration configuration) : base(configuration)
        {
            OAuthAuthorizationServerProvider = new GrantProvider(Configuration);
            RefreshTokenProvider = new RefreshTokenProvider(Configuration);
        }

        public override void Configure([NotNull] IAppBuilder appBuilder)
        {
            appBuilder.UseAesDataProtectorProvider();

            appBuilder.UseContentTypeMappingMiddleware(
                new RequestMap
                {
                    {
                        (new PathString("/api/oauth/token"), "POST", "application/json"),
                        async owinContext =>
                            await (owinContext?.ConvertFromJsonToFormBody() ??
                                   throw new InvalidOperationException(@"Task is null"))
                    }
                }
            );

            appBuilder.UseOAuthAuthorizationServer(
                new OAuthAuthorizationServerOptions
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
