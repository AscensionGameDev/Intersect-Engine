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
        public bool Editor { get; set; }

        public bool Ban { get; set; }

        public bool Kick { get; set; }

        public bool Mute { get; set; }

        public bool Api { get; set; }

        public bool IsModerator => Ban || Mute || Kick;

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

        public static bool operator ==(UserRights b1, UserRights b2)
        {
            if (null == b1)
                return (null == b2);

            return b1.Equals(b2);
        }

        public static bool operator !=(UserRights b1, UserRights b2)
        {
            return !(b1 == b2);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(UserRights))
            {
                var rights = (UserRights) obj;
                return (Editor == rights.Editor && Ban == rights.Ban && Kick == rights.Kick && Mute == rights.Mute && Api == rights.Api);
            }
            return false;
        }
        
    }
}
