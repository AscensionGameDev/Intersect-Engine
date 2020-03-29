using System;

namespace Intersect.Network.Packets.Client
{

    public class NeedMapPacket : CerasPacket
    {

        public NeedMapPacket(Guid mapId)
        {
            MapId = mapId;
        }

        public Guid MapId { get; set; }

    }

}
