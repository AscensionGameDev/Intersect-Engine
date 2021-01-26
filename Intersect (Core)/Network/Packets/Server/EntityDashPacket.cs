using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class EntityDashPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public EntityDashPacket()
        {
        }

        public EntityDashPacket(Guid entityId, Guid endMapId, byte endX, byte endY, int dashTime, sbyte direction)
        {
            EntityId = entityId;
            EndMapId = endMapId;
            EndX = endX;
            EndY = endY;
            DashTime = dashTime;
            Direction = direction;
        }

        [Key(0)]
        public Guid EntityId { get; set; }

        [Key(1)]
        public Guid EndMapId { get; set; }

        [Key(2)]
        public byte EndX { get; set; }

        [Key(3)]
        public byte EndY { get; set; }

        [Key(4)]
        public int DashTime { get; set; }

        [Key(5)]
        public sbyte Direction { get; set; }

    }

}
