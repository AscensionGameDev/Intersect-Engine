using JetBrains.Annotations;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Intersect.Server.Web.RestApi.Authentication.OAuth.Providers
{
    internal class AccessTokenProvider : AuthenticationTokenProvider
    {
        [NotNull] private OAuthProvider OAuthProvider { get; }

        public AccessTokenProvider([NotNull] OAuthProvider oAuthProvider)
        {
            OAuthProvider = oAuthProvider;
        }

        public override void Create(AuthenticationTokenCreateContext context)
        {
            base.Create(context);
        }

        public override Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            return base.CreateAsync(context);
        }

        public override void Receive(AuthenticationTokenReceiveContext context)
        {
            base.Receive(context);
        }

        public override Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            return base.ReceiveAsync(context);
        }
    }
}
