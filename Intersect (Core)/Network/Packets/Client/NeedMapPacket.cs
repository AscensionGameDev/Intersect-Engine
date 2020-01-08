using System;

namespace Intersect.Network.Packets.Client
{
    public class NeedMapPacket : CerasPacket
    {
        public Guid MapId { get; set; }

        public NeedMapPacket(Guid mapId)
        {
            MapId = mapId;
        }
    }
}
