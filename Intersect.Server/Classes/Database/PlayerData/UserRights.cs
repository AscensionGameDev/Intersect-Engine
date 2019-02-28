using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intersect.Enums;
using JetBrains.Annotations;

namespace Intersect.Server.Database.PlayerData
{
    public class UserRights
    {
        private readonly int mHashCode = new Random().Next();

        public bool Editor { get; set; }

        public bool Ban { get; set; }

        public bool Kick { get; set; }

        public bool Mute { get; set; }

        public bool Api { get; set; }

        public bool IsModerator => Ban || Mute || Kick;

        [NotNull]
        public static UserRights None => new UserRights();

        [NotNull]
        public static UserRights Moderation => new UserRights
        {
            Ban = true,
            Kick = true,
            Mute = true
        };

        [NotNull]
        public static UserRights Admin => new UserRights
        {
            Editor = true,
            Ban = true,
            Kick = true,
            Mute = true
        };

        public static bool operator ==(UserRights b1, UserRights b2)
        {
            if (b1 is null || b2 is null)
            {
                return b1 is null && b2 is null;
            }

            return b1.Equals(b2);
        }

        public static bool operator !=(UserRights b1, UserRights b2)
        {
            return !(b1 == b2);
        }

        public override bool Equals(object obj)
        {
            return obj is UserRights userRights && Equals(userRights);
        }

        protected bool Equals([NotNull] UserRights other)
        {
            return Editor == other.Editor && Ban == other.Ban && Kick == other.Kick && Mute == other.Mute && Api == other.Api;
        }

        public override int GetHashCode()
        {
            return mHashCode;
        }
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
    }

    public static class AccessExtensions
    {
        [NotNull]
        public static UserRights AsUserRights(this Access access)
        {
            switch (access)
            {
                case Access.Admin:
                    return UserRights.Admin;

                case Access.Moderator:
                    return UserRights.Moderation;

                case Access.None:
                    return UserRights.None;

                default:
                    throw new ArgumentOutOfRangeException(nameof(access), access, null);
            }
        }
    }
}
