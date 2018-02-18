using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ItemBase : DatabaseObject<ItemBase>
    {
        public int Animation;
        public int AttackAnimation = -1;
        public int Bound;
        public int CritChance;
        public int Damage;
        public int DamageType;
        public int Data1;
        public int Data2;
        public int Data3;
        public int Data4;

        public string Desc = "";
        public string FemalePaperdoll = "";
        public int ItemType;
        public string MalePaperdoll = "";
        public string Pic = "";
        public int Price;
        public int Projectile = -1;
        public int Scaling;
        public int ScalingStat;
        public int Speed;
        public int Stackable;
        public int StatGrowth;
        public int[] StatsGiven;
        public int Tool = -1;
        public ConditionLists UseReqs = new ConditionLists();

        [JsonConstructor]
        public ItemBase(int index) : base(index)
        {
            Name = "New Item";
            Speed = 10; // Set to 10 by default.
            StatsGiven = new int[(int)Stats.StatCount];
        }

        public bool IsStackable()
        {
            return (ItemType == (int) ItemTypes.Currency || Stackable > 0) && ItemType != (int)ItemTypes.Equipment && ItemType != (int)ItemTypes.Bag;
        }
    }
}