using MessagePack;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class SkillDataPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public SkillDataPacket()
    {
        Skills = new Dictionary<Guid, SkillDataEntry>();
    }

    public SkillDataPacket(Dictionary<Guid, SkillDataEntry> skills)
    {
        Skills = skills ?? new Dictionary<Guid, SkillDataEntry>();
    }

    [Key(0)]
    public Dictionary<Guid, SkillDataEntry> Skills { get; set; }
}

[MessagePackObject]
public partial class SkillDataEntry
{
    //Parameterless Constructor for MessagePack
    public SkillDataEntry()
    {
    }

    public SkillDataEntry(long experience, int level, long experienceToNextLevel)
    {
        Experience = experience;
        Level = level;
        ExperienceToNextLevel = experienceToNextLevel;
    }

    [Key(0)]
    public long Experience { get; set; }

    [Key(1)]
    public int Level { get; set; }

    [Key(2)]
    public long ExperienceToNextLevel { get; set; }
}

