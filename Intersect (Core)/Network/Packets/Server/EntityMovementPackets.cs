using System;

using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class EntityMovementPackets : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public EntityMovementPackets()
        {
        }

        public EntityMovementPackets(EntityMovePacket[] globalMovements, EntityMovePacket[] localMovements)
        {
            GlobalMovements = globalMovements;
            LocalMovements = localMovements;
        }

        [Key(0)]
        public EntityMovePacket[] GlobalMovements { get; set; }

        [Key(1)]
        public EntityMovePacket[] LocalMovements { get; set; }

    }

}
