using System.Collections.Generic;
using Intersect.Enums;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ClassBase : DatabaseObject<ClassBase>
    {
        public int AttackAnimation = -1;

        //Exp Calculations
        public long BaseExp = 100;

        public int BasePoints;
        public int[] BaseStat = new int[(int) Stats.StatCount];

        //Starting Vitals & Stats
        public int[] BaseVital = new int[(int) Vitals.VitalCount];

        public int CritChance;

        //Combat
        public int Damage = 1;

        public int DamageType;
        public long ExpIncrease = 50;

        //Level Up Info
        public int IncreasePercentage;

        //Starting Items
        public List<ClassItem> Items = new List<ClassItem>();

        //Locked - Can the class be chosen from character select?
        public int Locked;

        public int PointIncrease;
        public int Scaling = 100;
        public int ScalingStat;
        public int SpawnDir;

        //Spawn Info
        public int SpawnMap;

        public int SpawnX;
        public int SpawnY;

        //Starting Spells
        public List<ClassSpell> Spells = new List<ClassSpell>();

        //Sprites
        public List<ClassSprite> Sprites = new List<ClassSprite>();

        public int[] StatIncrease = new int[(int) Stats.StatCount];
        public int[] VitalIncrease = new int[(int) Vitals.VitalCount];

        //Regen Percentages
        public int[] VitalRegen = new int[(int) Vitals.VitalCount];

        [JsonConstructor]
        public ClassBase(int index) : base(index)
        {
            Name = "New Class";
        }
    }

    public class ClassItem
    {
        public int Amount;
        public int ItemNum;
    }

    public class ClassSpell
    {
        public int Level;
        public int SpellNum;
    }

    public class ClassSprite
    {
        public string Face = "";
        public byte Gender;
        public string Sprite = "";
    }
}