using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class UnlockedTitlesPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public UnlockedTitlesPacket()
    {
    }

    public UnlockedTitlesPacket(List<Guid> unlockedTitleIds, Guid equippedTitleId)
    {
        UnlockedTitleIds = unlockedTitleIds;
        EquippedTitleId = equippedTitleId;
    }

    /// <summary>
    /// Ids of the titles the player has unlocked. The client resolves them against the
    /// <c>TitleDescriptor</c> lookup it receives with the rest of the game data.
    /// </summary>
    [Key(0)]
    public List<Guid> UnlockedTitleIds { get; set; } = [];

    [Key(1)]
    public Guid EquippedTitleId { get; set; }
}
