namespace Intersect.Client.Entities;

public class ClientSkillData
{
    public long Experience { get; set; }
    public int Level { get; set; } = 1;
    public long ExperienceToNextLevel { get; set; }

    public ClientSkillData()
    {
    }

    public ClientSkillData(long experience, int level, long experienceToNextLevel)
    {
        Experience = experience;
        Level = level;
        ExperienceToNextLevel = experienceToNextLevel;
    }
}

