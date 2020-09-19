using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    /// <summary>
    /// The definition of the GuildPacket sent to a player containing the online and offline members of their guilds.
    /// </summary>
    public class GuildPacket : CerasPacket
    {

        /// <summary>
        /// Create a new instance of this class and define its contents.
        /// </summary>
        /// <param name="onlineMembers">A dictionary containing the online members of a guild and their rank.</param>
        /// <param name="offlineMembers">A list of offline member names.</param>
        public GuildPacket(Dictionary<string, string> onlineMembers, Dictionary<string, string> offlineMembers)
        {
            OnlineMembers = onlineMembers;
            OfflineMembers = offlineMembers;
        }

        public Dictionary<string, string> OnlineMembers { get; set; }

        public Dictionary<string, string> OfflineMembers { get; set; }

    }

}
