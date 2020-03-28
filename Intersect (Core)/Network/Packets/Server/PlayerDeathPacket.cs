using System;

namespace Intersect.Network.Packets.Server
{

    public class PlayerDeathPacket : CerasPacket
    {

        public PlayerDeathPacket(Guid playerId)
        {
            PlayerId = playerId;
        }

        public Guid PlayerId { get; set; }

    }

}
