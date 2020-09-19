using Intersect.Enums;

namespace Intersect.Network.Packets.Client
{

    public class UpdateGuildMemberPacket : CerasPacket
    {

        public UpdateGuildMemberPacket(string name, GuildMemberUpdateActions action)
        {
            Name = name;
            Action = action;
        }

        public string Name { get; set; }

        public GuildMemberUpdateActions Action { get; set; }

    }

}
