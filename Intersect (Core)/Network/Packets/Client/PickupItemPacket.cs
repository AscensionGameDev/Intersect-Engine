using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class PickupItemPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PickupItemPacket()
        {
        }

        public PickupItemPacket(Point location, Guid uniqueId)
        {
            UniqueId = uniqueId;
            Location = location;
        }

        [Key(0)]
        public Guid UniqueId { get; set; }

        [Key(1)]
        public Point Location { get; set; }

    }

}
