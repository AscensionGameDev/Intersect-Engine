using System;

namespace Intersect.Network.Packets.Client
{

    public class PickupItemPacket : CerasPacket
    {

        public PickupItemPacket(Guid uniqueId)
        {
            UniqueId = uniqueId;
        }

        public Guid UniqueId { get; set; }

    }

}
