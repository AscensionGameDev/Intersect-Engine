using Intersect.Enums;
using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class UpdateGuildMemberPacket : IntersectPacket
    {
        /// <summary>
        /// Parameterless Constructor for MessagePack
        /// </summary>
        public UpdateGuildMemberPacket()
        {

        }

        public UpdateGuildMemberPacket(Guid id, string name, GuildMemberUpdateActions action, int rank = -1)
        {
            Id = id;
            Name = name;
            Action = action;
            Rank = rank;
        }

        [Key(0)]
        public Guid Id { get; set; }

        [Key(1)]
        public string Name { get; set; }

        [Key(2)]
        public GuildMemberUpdateActions Action { get; set; }

        [Key(3)]
        public int Rank { get; set; }
    }
}
