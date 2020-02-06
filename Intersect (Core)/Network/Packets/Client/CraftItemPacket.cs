using System;

namespace Intersect.Network.Packets.Client
{
    public class CraftItemPacket : CerasPacket
    {
        public Guid CraftId { get; set; }

        public CraftItemPacket(Guid craftId)
        {
            CraftId = craftId;
        }
    }
}
