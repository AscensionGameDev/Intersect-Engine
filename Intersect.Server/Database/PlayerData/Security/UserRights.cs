using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using Intersect.Enums;

using Newtonsoft.Json;

namespace Intersect.Server.Database.PlayerData.Security
{
    public sealed partial class UserRights : IComparable<UserRights>, IEquatable<UserRights>
    {
        private static readonly Type ApiRolesType = typeof(ApiRoles);

        private static readonly List<PropertyInfo> ApiRolesPermissions = ApiRolesType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(propertyInfo => propertyInfo.CanWrite)
            .Where(propertyInfo => propertyInfo.PropertyType == typeof(bool))
            .ToList();

        private static readonly Type UserRightsType = typeof(UserRights);

        private static readonly List<PropertyInfo> UserRightsPermissions = UserRightsType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(propertyInfo => propertyInfo.CanWrite)
            .Where(propertyInfo => propertyInfo.PropertyType == typeof(bool))
            .ToList();

        private readonly int mHashCode = new Random().Next();

        public bool Editor { get; set; }

        public bool Ban { get; set; }

        public bool Kick { get; set; }

        public bool Mute { get; set; }

        public bool Api { get; set; }

        public ApiRoles ApiRoles { get; set; } = new ApiRoles();

        [JsonIgnore]
        public bool IsModerator => Ban || Mute || Kick;

        [JsonIgnore]
        public bool IsAdmin => Ban && Mute && Kick && Editor;

        public static UserRights None => new UserRights();

        public static UserRights Moderation => new UserRights
        {
            Ban = true,
            Kick = true,
            Mute = true
        };

        public static UserRights Admin => new UserRights
        {
            Editor = true,
            Ban = true,
            Kick = true,
            Mute = true
        };

        [JsonIgnore]
        public ImmutableList<string> Roles => EnumeratePermissions();

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

        protected int ComputePowerScore()
        {
            var score = 0;
            score |= Convert.ToInt32(Editor) << 0;
            score |= Convert.ToInt32(Mute) << 1;
            score |= Convert.ToInt32(Kick) << 2;
            score |= Convert.ToInt32(Ban) << 3;

            return score;
        }

        public int CompareTo(UserRights other)
        {
            if (other == default)
            {
                return 1;
            }

            return ComputePowerScore() - other.ComputePowerScore();
        }

        public override bool Equals(object obj)
        {
            return obj is UserRights userRights && Equals(userRights);
        }

        public bool Equals(UserRights other)
        {
            var permissions = EnumeratePermissions();
            var otherPermissions = other.EnumeratePermissions();

            if (permissions.Count != otherPermissions.Count)
            {
                return false;
            }

            return permissions.IsEmpty || otherPermissions.All(permission => permissions.Contains(permission));
        }

        public override int GetHashCode()
        {
            return mHashCode;
        }

        internal ImmutableList<string> EnumeratePermissions(bool permitted = true)
        {
            var userRights = UserRightsPermissions
                                 .Where(property => permitted == (bool) (property?.GetValue(this, null) ?? false))
                                 .Select(property => property?.Name)
                                 .ToList() ??
                             throw new InvalidOperationException();

            var apiRoles = ApiRolesPermissions
                               .Where(
                                   property => permitted == (bool) (property?.GetValue(this.ApiRoles, null) ?? false)
                               )
                               .Select(property => property?.Name)
                               .ToList() ??
                           throw new InvalidOperationException();

            userRights.AddRange(apiRoles);

            return userRights.ToImmutableList();
        }
    }

    public static partial class AccessExtensions
    {
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

    public partial class ApiRoles
    {
        public bool UserQuery { get; set; }

        public bool UserManage { get; set; }
    }
}
