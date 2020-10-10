using System;

namespace Intersect.Network.Packets.Client
{

    public class PickupItemPacket : CerasPacket
    {

        public PickupItemPacket(Point location, Guid uniqueId)
        {
            UniqueId = uniqueId;
            Location = location;
        }

        public Guid UniqueId { get; set; }

        public Point Location { get; set; }

    }

}
