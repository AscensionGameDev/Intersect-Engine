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
        public List<int> AggroList = new List<int>();

        [Column("AggroList")]
        public string JsonAggroList
        {
            get => JsonConvert.SerializeObject(AggroList);
            set => AggroList = JsonConvert.DeserializeObject<List<int>>(value);
        }

        public bool AttackAllies;
        public int AttackAnimation = -1;
        public byte Behavior;
        public int CritChance;

        //Combat
        public int Damage = 1;

        public int DamageType;

        //Drops
        public List<NpcDrop> Drops = new List<NpcDrop>();

        [Column("Drops")]
        public string JsonDrops
        {
            get => JsonConvert.SerializeObject(Drops);
            set => Drops = JsonConvert.DeserializeObject<List<NpcDrop>>(value);
        }

        public long Experience;
        public int Level = 1;

        //Vitals & Stats
        public int[] MaxVital = new int[(int) Vitals.VitalCount];

        [Column("MaxVital")]
        public string JsonMaxVital
        {
            get => DatabaseUtils.SaveIntArray(MaxVital, (int)Vitals.VitalCount);
            set => DatabaseUtils.LoadIntArray(ref MaxVital, value, (int)Vitals.VitalCount);
        }

        //NPC vs NPC Combat
        public bool NpcVsNpcEnabled;

        public int Scaling = 100;
        public int ScalingStat;
        public int SightRange;

        //Basic Info
        public int SpawnDuration;

        public int SpellFrequency = 2;

        //Spells
        public List<int> Spells = new List<int>();

        public string Sprite = "";

        public int[] Stat = new int[(int) Stats.StatCount];

        [Column("Stat")]
        public string JsonStat
        {
            get => DatabaseUtils.SaveIntArray(Stat, (int)Stats.StatCount);
            set => DatabaseUtils.LoadIntArray(ref Stat, value, (int)Stats.StatCount);
        }

        [JsonConstructor]
        public NpcBase(int index) : base(index)
        {
            Name = "New Npc";
        }
    }

    public class NpcDrop
    {
        public int Amount;
        public double Chance;
        public int ItemNum;
    }
}