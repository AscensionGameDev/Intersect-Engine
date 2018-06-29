using Intersect.Enums;
using Intersect.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Utilities;

namespace Intersect.GameObjects
{
    public class NpcBase : DatabaseObject<NpcBase>
    {
        [Column("AggroList")]
        [JsonIgnore]
        public string JsonAggroList
        {
            get => JsonConvert.SerializeObject(AggroList);
            set => AggroList = JsonConvert.DeserializeObject<List<int>>(value);
        }
        [NotMapped]
        public List<int> AggroList { get; set; } = new List<int>();


        public bool AttackAllies { get; set; }

        [Column("AttackAnimation")]
        public int AttackAnimationId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public AnimationBase AttackAnimation
        {
            get => AnimationBase.Lookup.Get<AnimationBase>(AttackAnimationId);
            set => AttackAnimationId = value?.Index ?? -1;
        }
        
        public byte Behavior { get; set; }
        public int CritChance { get; set; }

        //Combat
        public int Damage { get; set; } = 1;
        public int DamageType { get; set; }

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

        [JsonConstructor]
        public NpcBase(int index) : base(index)
        {
            Name = "New Npc";
        }

        //Parameterless constructor for EF
        public NpcBase()
        {
            Name = "New Npc";
        }

        public static NpcBase Get(int index)
        {
            return NpcBase.Lookup.Get<NpcBase>(index);
        }
    }

    public class NpcDrop
    {
        public int Amount;
        public double Chance;
        public int ItemNum;
    }
}