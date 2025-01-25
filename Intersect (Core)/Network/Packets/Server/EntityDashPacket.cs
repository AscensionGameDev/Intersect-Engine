using MessagePack;
using Intersect.Enums;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class EntityDashPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public EntityDashPacket()
    {
    }

    public EntityDashPacket(Guid entityId, Guid endMapId, int endX, int endY, long dashEndMilliseconds, int dashLengthMilliseconds, Direction direction)
    {
        EntityId = entityId;
        EndMapId = endMapId;
        EndX = (byte)endX;
        EndY = (byte)endY;
        DashEndMilliseconds = dashEndMilliseconds;
        DashLengthMilliseconds = dashLengthMilliseconds;
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
    public long DashEndMilliseconds { get; set; }

    [Key(5)]
    public int DashLengthMilliseconds { get; set; }

    [Key(6)]
    public Direction Direction { get; set; }

}
