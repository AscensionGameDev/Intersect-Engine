using Intersect.Server.Models;
using JetBrains.Annotations;
using System;
using System.Security.Principal;

namespace Intersect.Server.WebApi.Authentication
{
    public class UserIdentity : IIdentity
    {
        [NotNull]
        public User User { get; }

        public string Name => User.Guid.ToString();

        public string AuthenticationType => "jwt";

        public bool IsAuthenticated
        {
            get => User.Power == 2;
        }

        public UserIdentity(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
        }
    }
}
