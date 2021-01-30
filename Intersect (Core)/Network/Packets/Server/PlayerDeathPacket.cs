using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PlayerDeathPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PlayerDeathPacket()
        {
        }

        public PlayerDeathPacket(Guid playerId)
        {
            PlayerId = playerId;
        }

        [Key(0)]
        public Guid PlayerId { get; set; }

    }

}
