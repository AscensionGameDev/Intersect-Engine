using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PlayAnimationPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PlayAnimationPacket()
        {
        }

        public PlayAnimationPacket(
            Guid animId,
            int targetType,
            Guid entityId,
            Guid mapId,
            int x,
            int y,
            sbyte direction
        )
        {
            AnimationId = animId;
            TargetType = targetType;
            EntityId = entityId;
            MapId = mapId;
            X = x;
            Y = y;
            Direction = direction;
        }

        [Key(0)]
        public Guid AnimationId { get; set; }

        [Key(1)]
        public int TargetType { get; set; } //TODO: Enum this!

        [Key(2)]
        public Guid EntityId { get; set; }

        [Key(3)]
        public Guid MapId { get; set; }

        [Key(4)]
        public int X { get; set; }

        [Key(5)]
        public int Y { get; set; }

        [Key(6)]
        public sbyte Direction { get; set; }

    }

}
