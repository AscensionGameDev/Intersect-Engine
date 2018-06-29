using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class SpellBase : DatabaseObject<SpellBase>
    {
        //Animations
        public int CastAnimation = -1;

        //Spell Times
        public int CastDuration;

        //Requirements
        public ConditionLists CastingReqs = new ConditionLists();

        public int CastRange;
        public int CooldownDuration;
        public int Cost;

        //Damage
        public int CritChance;

        public int DamageType = 1;
        public int Data1;
        public int Data2;
        public int Data3;
        public int Data4;
        public string Data5 = "";

        public string Desc = "";
        public int Friendly;
        public int HitAnimation = -1;
        public int HitRadius;
        public string Pic = "";

        //Extra Data, Teleport Coords, Custom Spells, Etc
        public int Projectile;

        public int Scaling = 100;
        public int ScalingStat;
        public byte SpellType;

        //Buff/Debuff Data
        public int[] StatDiff = new int[(int) Stats.StatCount];

        //Targetting Stuff
        public int TargetType;

        //Costs
        public int[] VitalCost = new int[(int) Vitals.VitalCount];

        //Heal/Damage
        public int[] VitalDiff = new int[(int) Vitals.VitalCount];

        [JsonConstructor]
        public SpellBase(int index) : base(index)
        {
            Name = "New Spell";
        }

        public static SpellBase Get(int index)
        {
            return SpellBase.Lookup.Get<SpellBase>(index);
        }
    }
}