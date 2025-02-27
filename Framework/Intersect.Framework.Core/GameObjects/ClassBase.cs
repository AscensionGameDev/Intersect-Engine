using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.PlayerClass;
using Intersect.GameObjects.Maps;
using Intersect.Models;
using Intersect.Server.Utilities;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects;

public partial class ClassBase : DatabaseObject<ClassBase>, IFolderable
{
    public const long DEFAULT_BASE_EXPERIENCE = 100;

    public const long DEFAULT_EXPERIENCE_INCREASE = 50;

    [NotMapped, JsonIgnore]
    public int[] BaseStat { get; set; } = new int[Enum.GetValues<Stat>().Length];

    [NotMapped, JsonIgnore]
    public long[] BaseVital { get; set; } = new long[Enum.GetValues<Vital>().Length];

    [JsonProperty(nameof(BaseVital)), NotMapped]
    public IReadOnlyDictionary<Vital, long> BaseVitalLookup
    {
        get =>
            BaseVital.Select((value, index) => (value, index))
                .ToDictionary(t => (Vital)t.index, t => t.value)
                .AsReadOnly();
        set
        {
            foreach (var (key, val) in value)
            {
                BaseVital[(int)key] = val;
            }
        }
    }

    [JsonProperty(nameof(VitalIncrease)), NotMapped]
    public IReadOnlyDictionary<Vital, long> VitalIncreaseLookup
    {
        get =>
            VitalIncrease.Select((value, index) => (value, index))
                .ToDictionary(t => (Vital)t.index, t => t.value)
                .AsReadOnly();
        set
        {
            foreach (var (key, val) in value)
            {
                VitalIncrease[(int)key] = val;
            }
        }
    }

    [JsonProperty(nameof(VitalRegen)), NotMapped]
    public IReadOnlyDictionary<Vital, long> VitalRegenLookup
    {
        get =>
            VitalRegen.Select((value, index) => (value, index))
                .ToDictionary(t => (Vital)t.index, t => t.value)
                .AsReadOnly();
        set
        {
            foreach (var (key, val) in value)
            {
                VitalRegen[(int)key] = val;
            }
        }
    }

    [JsonProperty(nameof(BaseStat)), NotMapped]
    public IReadOnlyDictionary<Stat, int> BaseStatLookup
    {
        get =>
            BaseStat.Select((statValue, index) => (statValue, index))
                .ToDictionary(t => (Stat)t.index, t => t.statValue)
                .AsReadOnly();
        set
        {
            foreach (var (key, val) in value)
            {
                BaseStat[(int)key] = val;
            }
        }
    }

    [JsonProperty(nameof(StatIncrease)), NotMapped]
    public IReadOnlyDictionary<Stat, int> StatIncreaseLookup
    {
        get =>
            StatIncrease.Select((statValue, index) => (statValue, index))
                .ToDictionary(t => (Stat)t.index, t => t.statValue)
                .AsReadOnly();
        set
        {
            foreach (var (key, val) in value)
            {
                StatIncrease[(int)key] = val;
            }
        }
    }

    [NotMapped] public Dictionary<int, long> ExperienceOverrides { get; set; } = [];

    [NotMapped]
    public List<ClassItem> Items { get; set; } = [];

    [JsonIgnore]
    private long mBaseExp;

    [JsonIgnore]
    private long mExpIncrease;

    [NotMapped]
    public List<ClassSpell> Spells { get; set; } = [];

    [NotMapped]
    public List<ClassSprite> Sprites { get; set; } = [];

    [NotMapped, JsonIgnore]
    public int[] StatIncrease { get; set; } = new int[Enum.GetValues<Stat>().Length];

    [NotMapped, JsonIgnore]
    public long[] VitalIncrease { get; set; } = new long[Enum.GetValues<Vital>().Length];

    [NotMapped, JsonIgnore]
    public long[] VitalRegen { get; set; } = new long[Enum.GetValues<Vital>().Length];

    [JsonConstructor]
    public ClassBase(Guid id) : base(id)
    {
        Name = "New Class";

        ExperienceCurve = new ExperienceCurve();
        ExperienceCurve.Calculate(1);
        BaseExp = DEFAULT_BASE_EXPERIENCE;
        ExpIncrease = DEFAULT_EXPERIENCE_INCREASE;
    }

    //Parameterless constructor for EF
    public ClassBase()
    {
        Name = "New Class";

        ExperienceCurve = new ExperienceCurve();
        ExperienceCurve.Calculate(1);
        BaseExp = DEFAULT_BASE_EXPERIENCE;
        ExpIncrease = DEFAULT_EXPERIENCE_INCREASE;
    }

    [Column("AttackAnimation")]
    [JsonProperty]
    public Guid AttackAnimationId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public AnimationDescriptor AttackAnimation
    {
        get => AnimationDescriptor.Get(AttackAnimationId);
        set => AttackAnimationId = value?.Id ?? Guid.Empty;
    }

    public int BasePoints { get; set; }

    public int CritChance { get; set; }

    public double CritMultiplier { get; set; } = 1.5;

    //Combat
    public int Damage { get; set; } = 1;

    public int DamageType { get; set; }

    public int AttackSpeedModifier { get; set; }

    public int AttackSpeedValue { get; set; }

    public string AttackSpriteOverride { get; set; }

    public long BaseExp
    {
        get => mBaseExp;
        set
        {
            mBaseExp = Math.Max(0, value);
            ExperienceCurve.BaseExperience = Math.Max(1, mBaseExp);
        }
    }

    public long ExpIncrease
    {
        get => mExpIncrease;
        set
        {
            mExpIncrease = Math.Max(0, value);
            ExperienceCurve.Gain = 1 + value / 100.0;
        }
    }

    //Level Up Info
    public bool IncreasePercentage { get; set; }

    [JsonIgnore]
    [NotMapped]
    public ExperienceCurve ExperienceCurve { get; }

    //Locked - Can the partial class be chosen from character select?
    public bool Locked { get; set; }

    public int PointIncrease { get; set; }

    public int Scaling { get; set; } = 100;

    public int ScalingStat { get; set; }

    //Spawn Info
    [Column("SpawnMap")]
    [JsonProperty]
    public Guid SpawnMapId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public MapBase SpawnMap
    {
        get => MapBase.Get(SpawnMapId);
        set => SpawnMapId = value?.Id ?? Guid.Empty;
    }

    public int SpawnX { get; set; }

    public int SpawnY { get; set; }

    public int SpawnDir { get; set; }

    //Base Stats
    [Column("BaseStats")]
    [JsonIgnore]
    public string JsonBaseStats
    {
        get => DatabaseUtils.SaveIntArray(BaseStat, Enum.GetValues<Stat>().Length);
        set => BaseStat = DatabaseUtils.LoadIntArray(value, Enum.GetValues<Stat>().Length);
    }

    //Base Vitals
    [Column("BaseVitals")]
    [JsonIgnore]
    public string JsonBaseVitals
    {
        get => DatabaseUtils.SaveLongArray(BaseVital, Enum.GetValues<Vital>().Length);
        set => BaseVital = DatabaseUtils.LoadLongArray(value, Enum.GetValues<Vital>().Length);
    }

    //Starting Items
    [Column("Items")]
    [JsonIgnore]
    public string JsonItems
    {
        get => JsonConvert.SerializeObject(Items);
        protected set => Items = JsonConvert.DeserializeObject<List<ClassItem>>(value);
    }

    //Starting Spells
    [Column("Spells")]
    [JsonIgnore]
    public string JsonSpells
    {
        get => JsonConvert.SerializeObject(Spells);
        protected set => Spells = JsonConvert.DeserializeObject<List<ClassSpell>>(value);
    }

    //Sprites
    [JsonIgnore]
    [Column("Sprites")]
    public string JsonSprites
    {
        get => JsonConvert.SerializeObject(Sprites);
        protected set => Sprites = JsonConvert.DeserializeObject<List<ClassSprite>>(value);
    }

    //Stat Increases (per level)
    [JsonIgnore]
    [Column("StatIncreases")]
    public string StatIncreaseJson
    {
        get => DatabaseUtils.SaveIntArray(StatIncrease, Enum.GetValues<Stat>().Length);
        set => StatIncrease = DatabaseUtils.LoadIntArray(value, Enum.GetValues<Stat>().Length);
    }

    //Vital Increases (per level0
    [JsonIgnore]
    [Column("VitalIncreases")]
    public string VitalIncreaseJson
    {
        get => DatabaseUtils.SaveLongArray(VitalIncrease, Enum.GetValues<Vital>().Length);
        set => VitalIncrease = DatabaseUtils.LoadLongArray(value, Enum.GetValues<Vital>().Length);
    }

    //Vital Regen %
    [JsonIgnore]
    [Column("VitalRegen")]
    public string RegenJson
    {
        get => DatabaseUtils.SaveLongArray(VitalRegen, Enum.GetValues<Vital>().Length);
        set => VitalRegen = DatabaseUtils.LoadLongArray(value, Enum.GetValues<Vital>().Length);
    }

    [JsonIgnore]
    [Column("ExperienceOverrides")]
    public string ExpOverridesJson
    {
        get => JsonConvert.SerializeObject(ExperienceOverrides);
        set
        {
            ExperienceOverrides = JsonConvert.DeserializeObject<Dictionary<int, long>>(value ?? "");
            if (ExperienceOverrides == null)
            {
                ExperienceOverrides = new Dictionary<int, long>();
            }
        }
    }

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;

    public long ExperienceToNextLevel(int level)
    {
        if (ExperienceOverrides.ContainsKey(level))
        {
            return ExperienceOverrides[level];
        }

        return ExperienceCurve.Calculate(level);
    }
}