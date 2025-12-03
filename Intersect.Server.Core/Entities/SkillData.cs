namespace Intersect.Server.Entities;

/// <summary>
/// Stores experience and level data for a skill
/// </summary>
public class SkillData
{
    public long Experience { get; set; } = 0;

    public int Level { get; set; } = 1;

    public SkillData()
    {
    }

    public SkillData(int level, long experience)
    {
        Level = level;
        Experience = experience;
    }
}

