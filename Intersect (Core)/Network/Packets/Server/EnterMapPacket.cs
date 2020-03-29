using System;

namespace Intersect.Network.Packets.Server
{

    public class EnterMapPacket : CerasPacket
    {

        public EnterMapPacket(Guid mapId)
        {
            MapId = mapId;
        }

        public Guid MapId { get; set; }

    }

}
