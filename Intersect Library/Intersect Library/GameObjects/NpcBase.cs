using System;
using System.Collections.Generic;
using Intersect.Enums;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class NpcBase : DatabaseObject<NpcBase>
    {
        public List<int> AggroList = new List<int>();
        public bool AttackAllies;
        public int AttackAnimation = -1;
        public byte Behavior;
        public int CritChance;

        //Combat
        public int Damage;

        public int DamageType;

        //Drops
        public List<NpcDrop> Drops = new List<NpcDrop>();

        public long Experience;
        public int Level = 1;

        //Vitals & Stats
        public int[] MaxVital = new int[(int) Vitals.VitalCount];

        //NPC vs NPC Combat
        public bool NpcVsNpcEnabled;

        public int Scaling;
        public int ScalingStat;
        public int SightRange;

        //Basic Info
        public int SpawnDuration;

        public int SpellFrequency = 2;

        //Spells
        public List<int> Spells = new List<int>();

        public string Sprite = "";
        public int[] Stat = new int[(int) Stats.StatCount];

        [JsonConstructor]
        public NpcBase(int index) : base(index)
        {
            Name = "New Npc";
            for (int i = 0; i < Options.MaxNpcDrops; i++)
            {
                Drops.Add(new NpcDrop());
            }
        }
    }

    public class NpcDrop
    {
        public int Amount;
        public int Chance;
        public int ItemNum;
    }
}