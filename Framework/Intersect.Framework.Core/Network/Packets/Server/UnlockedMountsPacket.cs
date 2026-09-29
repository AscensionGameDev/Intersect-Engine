using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class UnlockedMountsPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public UnlockedMountsPacket()
    {
    }

    public UnlockedMountsPacket(List<Guid> unlockedMountIds, Guid activeMountId)
    {
        UnlockedMountIds = unlockedMountIds;
        ActiveMountId = activeMountId;
    }

    /// <summary>
    /// Ids of the mounts the player has unlocked. The client resolves them against the
    /// <c>MountDescriptor</c> lookup it receives with the rest of the game data.
    /// </summary>
    [Key(0)]
    public List<Guid> UnlockedMountIds { get; set; } = [];

    /// <summary>The mount that will be ridden the next time the player mounts up.</summary>
    [Key(1)]
    public Guid ActiveMountId { get; set; }
}
