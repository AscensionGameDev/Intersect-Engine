using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PlayAnimationPackets : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PlayAnimationPackets()
        {
        }

        public PlayAnimationPackets(PlayAnimationPacket[] packets)
        {
            Packets = packets;
        }

        [Key(0)]
        public PlayAnimationPacket[] Packets { get; set; }

    }

}
