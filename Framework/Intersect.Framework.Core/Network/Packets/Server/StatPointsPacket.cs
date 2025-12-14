using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class StatPointsPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public StatPointsPacket()
    {
    }

    public StatPointsPacket(int points, int skillPoints)
    {
        Points = points;
        SkillPoints = skillPoints;
    }

    [Key(0)]
    public int Points { get; set; }

    [Key(1)]
    public int SkillPoints { get; set; }

}
