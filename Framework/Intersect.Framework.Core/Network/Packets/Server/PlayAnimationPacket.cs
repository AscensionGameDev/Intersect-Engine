using MessagePack;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Animations;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class PlayAnimationPacket : IntersectPacket
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
        Direction direction,
        AnimationSourceType sourceType = AnimationSourceType.Any,
        Guid sourceId = default
    )
    {
        AnimationId = animId;
        TargetType = targetType;
        EntityId = entityId;
        MapId = mapId;
        X = x;
        Y = y;
        Direction = direction;
        SourceType = sourceType;
        SourceId = sourceId;
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
    public Direction Direction { get; set; }

    [Key(7)]
    public AnimationSourceType SourceType { get; set; }

    [Key(8)]
    public Guid SourceId { get; set; }

}
