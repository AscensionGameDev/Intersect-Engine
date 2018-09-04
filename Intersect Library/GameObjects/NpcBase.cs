using System;
using Intersect.Enums;
using Intersect.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Utilities;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Conditions;

namespace Intersect.GameObjects
{
    public class NpcBase : DatabaseObject<NpcBase>
    {
        [Column("AggroList")]
        [JsonIgnore]
        public string JsonAggroList
        {
            get => JsonConvert.SerializeObject(AggroList);
            set => AggroList = JsonConvert.DeserializeObject<List<Guid>>(value);
        }
        [NotMapped]
        public List<Guid> AggroList { get; set; } = new List<Guid>();


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

        //Conditions
        [Column("PlayerFriendConditions")]
        [JsonIgnore]
        public string PlayerFriendConditionsJson
        {
            get => PlayerFriendConditions.Data();
            set => PlayerFriendConditions.Load(value);
        }
        [NotMapped]
        public ConditionLists PlayerFriendConditions = new ConditionLists();
        

        [Column("AttackOnSightConditions")]
        [JsonIgnore]
        public string AttackOnSightConditionsJson
        {
            get => AttackOnSightConditions.Data();
            set => AttackOnSightConditions.Load(value);
        }
        [NotMapped]
        public ConditionLists AttackOnSightConditions = new ConditionLists();

        [Column("PlayerCanAttackConditions")]
        [JsonIgnore]
        public string PlayerCanAttackConditionsJson
        {
            get => PlayerCanAttackConditions.Data();
            set => PlayerCanAttackConditions.Load(value);
        }
        [NotMapped]
        public ConditionLists PlayerCanAttackConditions = new ConditionLists();
        

        //Combat
        public int Damage { get; set; } = 1;
        public int DamageType { get; set; }
        public int CritChance { get; set; }
        public double CritMultiplier { get; set; } = 1.5;

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
            set => Drops = JsonConvert.DeserializeObject<List<NpcDrop>>(value);
        }
        [NotMapped]
        public List<NpcDrop> Drops = new List<NpcDrop>();
        
        public long Experience { get; set; }
        public int Level { get; set; } = 1;

        //Vitals & Stats
        [Column("MaxVital")]
        [JsonIgnore]
        public string JsonMaxVital
        {
            get => DatabaseUtils.SaveIntArray(MaxVital, (int)Vitals.VitalCount);
            set => DatabaseUtils.LoadIntArray(ref MaxVital, value, (int)Vitals.VitalCount);
        }
        [NotMapped]
        public int[] MaxVital = new int[(int)Vitals.VitalCount];

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
        public DbList<SpellBase> Spells { get; set; } = new DbList<SpellBase>();

        public string Sprite { get; set; } = "";

        [Column("Stats")]
        [JsonIgnore]
        public string JsonStat
        {
            get => DatabaseUtils.SaveIntArray(Stats, (int)Enums.Stats.StatCount);
            set => DatabaseUtils.LoadIntArray(ref Stats, value, (int)Enums.Stats.StatCount);
        }
        [NotMapped]
        public int[] Stats = new int[(int) Enums.Stats.StatCount];

        //Vital Regen %
        [JsonIgnore]
        [Column("VitalRegen")]
        public string RegenJson
        {
            get => DatabaseUtils.SaveIntArray(VitalRegen, (int)Vitals.VitalCount);
            set => VitalRegen = DatabaseUtils.LoadIntArray(value, (int)Vitals.VitalCount);
        }
        [NotMapped]
        public int[] VitalRegen = new int[(int)Vitals.VitalCount];

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
    }

    public class NpcDrop
    {
        public int Quantity;
        public double Chance;
        public Guid ItemId;
    }
}