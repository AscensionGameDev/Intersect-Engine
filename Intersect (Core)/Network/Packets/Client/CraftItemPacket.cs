using System;

namespace Intersect.Network.Packets.Client
{

    public class CraftItemPacket : CerasPacket
    {

        public CraftItemPacket(Guid craftId)
        {
            CraftId = craftId;
        }

        public Guid CraftId { get; set; }

    }

}
