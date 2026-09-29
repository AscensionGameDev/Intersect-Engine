using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class SetActiveMountPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public SetActiveMountPacket()
    {
    }

    /// <param name="mountId">The mount to select, or <see cref="Guid.Empty"/> to clear the selection.</param>
    public SetActiveMountPacket(Guid mountId)
    {
        MountId = mountId;
    }

    [Key(0)]
    public Guid MountId { get; set; }
}
