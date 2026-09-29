using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class UnlockedAchievementsPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public UnlockedAchievementsPacket()
    {
    }

    public UnlockedAchievementsPacket(Dictionary<Guid, long> unlockedAchievements)
    {
        UnlockedAchievements = unlockedAchievements;
    }

    /// <summary>
    /// Achievement id -> the UTC unix time (ms) it was unlocked at. The client resolves names/descriptions
    /// against the <c>AchievementDescriptor</c> lookup it receives with the rest of the game data.
    /// </summary>
    [Key(0)]
    public Dictionary<Guid, long> UnlockedAchievements { get; set; } = [];
}
