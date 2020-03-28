using System;

namespace Intersect.Network.Packets.Server
{

    public class PlayAnimationPacket : CerasPacket
    {

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

        public Guid AnimationId { get; set; }

        public int TargetType { get; set; } //TODO: Enum this!

        public Guid EntityId { get; set; }

        public Guid MapId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public sbyte Direction { get; set; }

    }

}
