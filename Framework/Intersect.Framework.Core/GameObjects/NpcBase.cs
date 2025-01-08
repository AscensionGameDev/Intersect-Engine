using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events;
using Intersect.Models;
using Intersect.Utilities;

using Newtonsoft.Json;

namespace Intersect.GameObjects;

public partial class NpcBase : DatabaseObject<NpcBase>, IFolderable
{
    private long[] _maxVital = new long[Enum.GetValues<Vital>().Length];
    private int[] _stats = new int[Enum.GetValues<Stat>().Length];
    private long[] _vitalRegen = new long[Enum.GetValues<Vital>().Length];

    [NotMapped]
    public ConditionLists AttackOnSightConditions { get; set; } = new();

    [NotMapped]
    public List<Drop> Drops { get; set; }= [];

    [NotMapped]
    public long[] MaxVital
    {
        get => _maxVital;
        set => _maxVital = value;
    }

    [NotMapped]
    public ConditionLists PlayerCanAttackConditions { get; set; } = new();

    [NotMapped]
    public ConditionLists PlayerFriendConditions { get; set; } = new();

    [NotMapped]
    public int[] Stats
    {
        get => _stats;
        set => _stats = value;
    }

    [NotMapped]
    public long[] VitalRegen
    {
        get => _vitalRegen;
        set => _vitalRegen = value;
    }

    [NotMapped]
    public List<SpellEffect> Immunities { get; set; } = [];

    [JsonIgnore]
    [Column("Immunities")]
    public string ImmunitiesJson
    {
        get => JsonConvert.SerializeObject(Immunities);
        set
        {
            Immunities = JsonConvert.DeserializeObject<List<SpellEffect>>(value ?? "") ?? [];
        }
    }

    [JsonConstructor]
    public NpcBase(Guid id) : base(id)
    {
        Name = "New Npc";
    }

    //Parameterless constructor for EF
    public NpcBase()
    {
        Name = "New Npc";
    }

    [Column("AggroList")]
    [JsonIgnore]
    public string JsonAggroList
    {
        get => JsonConvert.SerializeObject(AggroList);
        set => AggroList = JsonConvert.DeserializeObject<List<Guid>>(value);
    }

    [NotMapped]
    public List<Guid> AggroList { get; set; } = [];

    public bool AttackAllies { get; set; }

    [Column("AttackAnimation")]
    public Guid AttackAnimationId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public AnimationBase AttackAnimation
    {
        get => AnimationBase.Get(AttackAnimationId);
        set => AttackAnimationId = value?.Id ?? Guid.Empty;
    }

    //Behavior
    public bool Aggressive { get; set; }

    public byte Movement { get; set; }

    public bool Swarm { get; set; }

    public byte FleeHealthPercentage { get; set; }

    public bool FocusHighestDamageDealer { get; set; } = true;

    public int ResetRadius { get; set; }

    //Conditions
    [Column("PlayerFriendConditions")]
    [JsonIgnore]
    public string PlayerFriendConditionsJson
    {
        get => PlayerFriendConditions.Data();
        set => PlayerFriendConditions.Load(value);
    }

    [Column("AttackOnSightConditions")]
    [JsonIgnore]
    public string AttackOnSightConditionsJson
    {
        get => AttackOnSightConditions.Data();
        set => AttackOnSightConditions.Load(value);
    }

    [Column("PlayerCanAttackConditions")]
    [JsonIgnore]
    public string PlayerCanAttackConditionsJson
    {
        get => PlayerCanAttackConditions.Data();
        set => PlayerCanAttackConditions.Load(value);
    }

    //Combat
    public int Damage { get; set; } = 1;

    public int DamageType { get; set; }

    public int CritChance { get; set; }

    public double CritMultiplier { get; set; } = 1.5;

    public double Tenacity { get; set; } = 0.0;

    public int AttackSpeedModifier { get; set; }

    public int AttackSpeedValue { get; set; }

    //Common Events
    [Column("OnDeathEvent")]
    public Guid OnDeathEventId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public EventBase OnDeathEvent
    {
        get => EventBase.Get(OnDeathEventId);
        set => OnDeathEventId = value?.Id ?? Guid.Empty;
    }

    [Column("OnDeathPartyEvent")]
    public Guid OnDeathPartyEventId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public EventBase OnDeathPartyEvent
    {
        get => EventBase.Get(OnDeathPartyEventId);
        set => OnDeathPartyEventId = value?.Id ?? Guid.Empty;
    }

    //Drops
    [Column("Drops")]
    [JsonIgnore]
    public string JsonDrops
    {
        get => JsonConvert.SerializeObject(Drops);
        set => Drops = JsonConvert.DeserializeObject<List<Drop>>(value);
    }

    /// <summary>
    /// If true this npc will drop individual loot for all of those who helped slay it.
    /// </summary>
    public bool IndividualizedLoot { get; set; }

    public long Experience { get; set; }

    public int Level { get; set; } = 1;

    //Vitals & Stats
    [Column("MaxVital")]
    [JsonIgnore]
    public string JsonMaxVital
    {
        get => DatabaseUtils.SaveLongArray(_maxVital, Enum.GetValues<Vital>().Length);
        set => DatabaseUtils.LoadLongArray(ref _maxVital, value, Enum.GetValues<Vital>().Length);
    }

    //NPC vs NPC Combat
    public bool NpcVsNpcEnabled { get; set; }

    public int Scaling { get; set; } = 100;

    public int ScalingStat { get; set; }

    public int SightRange { get; set; }

    //Basic Info
    public int SpawnDuration { get; set; }

    public int SpellFrequency { get; set; } = 2;

    //Spells
    [JsonIgnore]
    [Column("Spells")]
    public string CraftsJson
    {
        get => JsonConvert.SerializeObject(Spells, Formatting.None);
        protected set => Spells = JsonConvert.DeserializeObject<DbList<SpellBase>>(value);
    }

    [NotMapped]
    public DbList<SpellBase> Spells { get; set; } = [];

    public string Sprite { get; set; } = string.Empty;

    /// <summary>
    /// The database compatible version of <see cref="Color"/>
    /// </summary>
    [Column("Color")]
    [JsonIgnore]
    public string JsonColor
    {
        get => JsonConvert.SerializeObject(Color);
        set => Color = !string.IsNullOrWhiteSpace(value) ? JsonConvert.DeserializeObject<Color>(value) : Color.White;
    }

    /// <summary>
    /// Defines the ARGB color settings for this Npc.
    /// </summary>
    [NotMapped]
    public Color Color { get; set; } = new(255, 255, 255, 255);

    [Column("Stats")]
    [JsonIgnore]
    public string JsonStat
    {
        get => DatabaseUtils.SaveIntArray(_stats, Enum.GetValues<Stat>().Length);
        set => DatabaseUtils.LoadIntArray(ref _stats, value, Enum.GetValues<Stat>().Length);
    }

    //Vital Regen %
    [JsonIgnore]
    [Column("VitalRegen")]
    public string RegenJson
    {
        get => DatabaseUtils.SaveLongArray(_vitalRegen, Enum.GetValues<Vital>().Length);
        set => DatabaseUtils.LoadLongArray(ref _vitalRegen, value, Enum.GetValues<Vital>().Length);
    }

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;

    public SpellBase GetRandomSpell(Random random)
    {
        if (Spells == null || Spells.Count == 0)
        {
            return null;
        }

        var spellIndex = random.Next(0, Spells.Count);
        var spellId = Spells[spellIndex];

        return SpellBase.Get(spellId);
    }
}
