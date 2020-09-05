using System;

namespace Intersect.Config
{
    /// <summary>
    /// Contains all options pertaining to guilds
    /// </summary>
    public class GuildOptions
    {

        /// <summary>
        /// Denotes the minimum amount of characters a guild name must contain before being accepted.
        /// </summary>
        public int MinimumGuildNameSize = 3;

        /// <summary>
        /// Denotes the maximum amount of characters a guild name can contain before being rejected.
        /// </summary>
        public int MaximumGuildNameSize = 25;

    }
}
