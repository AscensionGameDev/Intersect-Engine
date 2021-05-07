using Intersect.Client.General;
using Intersect.Client.Interface.Game;
using Intersect.Client.Localization;
using Intersect.Network;
using Intersect.Network.Packets.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intersect.Client.Entities.Player;

namespace Intersect.Client.Networking
{
    internal sealed partial class PacketHandler
    {
        //GuildPacket
        public void HandlePacket(IPacketSender packetSender, GuildPacket packet)
        {
            if (Globals.Me == null || Globals.Me.Guild == null)
            {
                return;
            }

            Globals.Me.GuildMembers = packet.Members.OrderByDescending(m => m.Online).ThenBy(m => m.Rank).ThenBy(m => m.Name).ToArray();

            Interface.Interface.GameUi.NotifyUpdateGuildList();
        }


        //GuildInvitePacket
        public void HandlePacket(IPacketSender packetSender, GuildInvitePacket packet)
        {
            var iBox = new InputBox(
                Strings.Guilds.InviteRequestTitle, Strings.Guilds.InviteRequestPrompt.ToString(packet.Inviter, packet.GuildName), true,
                InputBox.InputType.YesNo, PacketSender.SendGuildInviteAccept, PacketSender.SendGuildInviteDecline,
                null
            );
        }

    }
}
