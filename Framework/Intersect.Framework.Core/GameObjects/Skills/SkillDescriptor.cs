using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Models;
using Intersect.Server.Utilities;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Skills;

public partial class SkillDescriptor : DatabaseObject<SkillDescriptor>, IFolderable
{
    public const long DEFAULT_BASE_EXPERIENCE = 100;

    public const long DEFAULT_EXPERIENCE_INCREASE = 50;

    [JsonIgnore]
    private long mBaseExperience;

    [JsonIgnore]
    private long mExperienceIncrease;

    [JsonConstructor]
    public SkillDescriptor(Guid id) : base(id)
    {
        Name = "New Skill";
        ExperienceCurve = new ExperienceCurve();
        ExperienceCurve.Calculate(1);
        BaseExperience = DEFAULT_BASE_EXPERIENCE;
        ExperienceIncrease = DEFAULT_EXPERIENCE_INCREASE;
    }

    //EF wants NO PARAMETERS!!!!!
    public SkillDescriptor()
    {
        Name = "New Skill";
        ExperienceCurve = new ExperienceCurve();
        ExperienceCurve.Calculate(1);
        BaseExperience = DEFAULT_BASE_EXPERIENCE;
        ExperienceIncrease = DEFAULT_EXPERIENCE_INCREASE;
    }

    /// <summary>
    /// Maximum level for this skill
    /// </summary>
    public int MaxLevel { get; set; } = 99;

    /// <summary>
    /// Base experience required for level 2
    /// </summary>
    public long BaseExperience
    {
        get => mBaseExperience;
        set
        {
            mBaseExperience = Math.Max(0, value);
            ExperienceCurve.BaseExperience = Math.Max(1, mBaseExperience);
        }
    }

    /// <summary>
    /// Experience increase per level
    /// </summary>
    public long ExperienceIncrease
    {
        get => mExperienceIncrease;
        set
        {
            mExperienceIncrease = Math.Max(0, value);
            ExperienceCurve.Gain = 1 + value / 100.0;
        }
    }

    /// <summary>
    /// Experience curve configuration
    /// </summary>
    [NotMapped, JsonIgnore]
    public ExperienceCurve ExperienceCurve { get; set; } = new();

    [Column("ExperienceCurve")]
    [JsonIgnore]
    public string ExperienceCurveJson
    {
        get => JsonConvert.SerializeObject(ExperienceCurve);
        set => ExperienceCurve = JsonConvert.DeserializeObject<ExperienceCurve>(value ?? "{}") ?? new ExperienceCurve();
    }

    /// <summary>
    /// Custom experience overrides for specific levels (level -> experience required)
    /// </summary>
    [NotMapped, JsonIgnore]
    public Dictionary<int, long> ExperienceOverrides { get; set; } = new();

    [JsonIgnore]
    [Column("ExperienceOverrides")]
    public string ExpOverridesJson
    {
        get => JsonConvert.SerializeObject(ExperienceOverrides);
        set
        {
            ExperienceOverrides = JsonConvert.DeserializeObject<Dictionary<int, long>>(value ?? "") ?? new Dictionary<int, long>();
        }
    }

    /// <summary>
    /// Icon texture for this skill (optional)
    /// </summary>
    [Column("Icon")]
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Description of the skill
    /// </summary>
    [Column("Description")]
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;

    /// <summary>
    /// Calculates the experience required to reach the next level from the given level
    /// </summary>
    public long ExperienceToNextLevel(int level)
    {
        if (ExperienceOverrides.ContainsKey(level))
        {
            return ExperienceOverrides[level];
        }

        return ExperienceCurve.Calculate(level);
    }
}

