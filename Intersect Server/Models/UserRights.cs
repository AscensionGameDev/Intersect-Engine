using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Intersect.Enums;
using JetBrains.Annotations;

namespace Intersect.Server.Models
{
    public struct UserRights
    {
        public bool Editor { get; set; }

        public bool Ban { get; set; }

        public bool Kick { get; set; }

        public bool Mute { get; set; }

        public bool Api { get; set; }

        public static UserRights None => new UserRights();

        public static UserRights Moderation => new UserRights
        {
            Ban = true,
            Kick = true,
            Mute = true
        };

        public static UserRights All => new UserRights
        {
            Editor = true,
            Ban = true,
            Kick = true,
            Mute = true,
            Api = true
        };
    }

    public static class UserRightsHelper
    {
        [NotNull]
        private static readonly Type UserRightsType = typeof(UserRights);

        [NotNull]
        private static readonly List<PropertyInfo> UserRightsPermissions = UserRightsType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

        public static List<string> EnumeratePermissions(this UserRights userRights, bool permitted = true)
        {
            return UserRightsPermissions
                .FindAll(property => (bool) (property?.GetValue(userRights, null) ?? false) == permitted)
                .Select(property => property?.Name).ToList();
        }

        public static UserRights FromLegacyPowers(Access access)
        {
            switch (access)
            {
                case Access.Moderator:
                    return UserRights.Moderation;

                case Access.Admin:
                    return UserRights.All;

                default:
                    return UserRights.None;
            }
        }
    }
}
