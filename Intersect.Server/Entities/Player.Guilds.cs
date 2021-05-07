using Intersect.Enums;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Intersect.Server.Entities
{
    public partial class Player
    {

        /// <summary>
        /// References the in-memory copy of the guild for this player, reference this instead of the Guild property below.
        /// </summary>
        [NotMapped] [JsonIgnore] public Guild Guild { get; set; }

        /// <summary>
        /// This field is used for EF database fields only and should never be assigned to or used, instead the guild instance will be assigned to CachedGuild above
        /// </summary>
        [JsonIgnore] public Guild DbGuild { get; set; }

        [NotMapped][JsonIgnore]
        public Tuple<Player, Guild> GuildInvite { get; set; }

        public int GuildRank { get; set; }

        public DateTime GuildJoinDate { get; set; }

        /// <summary>
        /// Used to determine whether the player is operating in the guild bank vs player bank
        /// </summary>
        [NotMapped] public bool GuildBank;


        public void LoadGuild()
        {
            using (var context = DbInterface.CreatePlayerContext())
            {
                var guildId = context.Players.Where(p => p.Id == Id && p.DbGuild.Id != null && p.DbGuild.Id != Guid.Empty).Select(p => p.DbGuild.Id).FirstOrDefault();
                if (guildId != default)
                {
                    Guild = Guild.LoadGuild(guildId);
                }
            }

            if (GuildRank > Options.Instance.Guild.Ranks.Length -1)
            {
                GuildRank = Options.Instance.Guild.Ranks.Length - 1;
            }
        }

        public void SendGuildInvite(Player from)
        {
            // Are we already in a guild? or have a pending invite?
            if (DbGuild == null && GuildInvite == null)
            {
                // Thank god, we can FINALLY get started!
                // Set our invite and send our players the relevant messages.
                GuildInvite = new Tuple<Player, Guild>(from, from.Guild);

                PacketSender.SendChatMsg(from, Strings.Guilds.InviteSent.ToString(Name, from.Guild.Name), ChatMessageType.Guild, CustomColors.Alerts.Info);

                PacketSender.SendGuildInvite(this, from);
            }
            else
            {
                PacketSender.SendChatMsg(from, Strings.Guilds.InviteAlreadyInGuild, ChatMessageType.Guild, CustomColors.Alerts.Error);
            }
        }
    }
}
