using Intersect.Server.Models;
using JetBrains.Annotations;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Intersect.Server.WebApi.Authentication
{
    public class UserIdentity : ClaimsIdentity
    {
        [NotNull]
        public User User { get; }

        public override string Name => User.Guid.ToString();

        public override string AuthenticationType => "jwt";

        public override bool IsAuthenticated => User.Rights.Api;

        public UserIdentity(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }
    }
}
