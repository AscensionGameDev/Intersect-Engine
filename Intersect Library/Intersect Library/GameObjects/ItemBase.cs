using System.Collections.Generic;
using Intersect.GameObjects.Conditions;

namespace Intersect.GameObjects
{
    public class ItemBase : DatabaseObject<ItemBase>
    {
        public new const string DATABASE_TABLE = "items";
        public new const GameObject OBJECT_TYPE = GameObject.Item;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
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

        public ItemBase(int id) : base(id)
        {
            Name = "New Item";
            Speed = 10; // Set to 10 by default.
            StatsGiven = new int[Options.MaxStats];
        }

        public override void Load(byte[] data)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(data);
            Name = myBuffer.ReadString();
            Desc = myBuffer.ReadString();
            ItemType = myBuffer.ReadInteger();
            Pic = myBuffer.ReadString();
            Price = myBuffer.ReadInteger();
            Bound = myBuffer.ReadInteger();
            Stackable = myBuffer.ReadInteger();
            Animation = myBuffer.ReadInteger();
            Projectile = myBuffer.ReadInteger();
            AttackAnimation = myBuffer.ReadInteger();

            UseReqs.Load(myBuffer);

            for (var i = 0; i < Options.MaxStats; i++)
            {
                StatsGiven[i] = myBuffer.ReadInteger();
            }

            StatGrowth = myBuffer.ReadInteger();
            Damage = myBuffer.ReadInteger();
            CritChance = myBuffer.ReadInteger();
            DamageType = myBuffer.ReadInteger();
            ScalingStat = myBuffer.ReadInteger();
            Scaling = myBuffer.ReadInteger();
            Speed = myBuffer.ReadInteger();
            MalePaperdoll = myBuffer.ReadString();
            FemalePaperdoll = myBuffer.ReadString();
            Tool = myBuffer.ReadInteger();
            Data1 = myBuffer.ReadInteger();
            Data2 = myBuffer.ReadInteger();
            Data3 = myBuffer.ReadInteger();
            Data4 = myBuffer.ReadInteger();
        }

        public byte[] ItemData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteString(Desc);
            myBuffer.WriteInteger(ItemType);
            myBuffer.WriteString(Pic);
            myBuffer.WriteInteger(Price);
            myBuffer.WriteInteger(Bound);
            myBuffer.WriteInteger(Stackable);
            myBuffer.WriteInteger(Animation);
            myBuffer.WriteInteger(Projectile);
            myBuffer.WriteInteger(AttackAnimation);

            UseReqs.Save(myBuffer);

            for (var i = 0; i < Options.MaxStats; i++)
            {
                myBuffer.WriteInteger(StatsGiven[i]);
            }

            myBuffer.WriteInteger(StatGrowth);
            myBuffer.WriteInteger(Damage);
            myBuffer.WriteInteger(CritChance);
            myBuffer.WriteInteger(DamageType);
            myBuffer.WriteInteger(ScalingStat);
            myBuffer.WriteInteger(Scaling);
            myBuffer.WriteInteger(Speed);
            myBuffer.WriteString(MalePaperdoll);
            myBuffer.WriteString(FemalePaperdoll);
            myBuffer.WriteInteger(Tool);
            myBuffer.WriteInteger(Data1);
            myBuffer.WriteInteger(Data2);
            myBuffer.WriteInteger(Data3);
            myBuffer.WriteInteger(Data4);
            return myBuffer.ToArray();
        }

        public bool IsStackable()
        {
            //Allow Stacking on Currency, Consumable, Spell, and item types of none.
            return (ItemType == (int) ItemTypes.Consumable ||
                    ItemType == (int) ItemTypes.Currency ||
                    ItemType == (int) ItemTypes.None ||
                    ItemType == (int) ItemTypes.Spell) && Stackable > 0;
        }

        public override byte[] BinaryData => ItemData();

        public override string DatabaseTableName
        {
            get { return DATABASE_TABLE; }
        }

        public override GameObject GameObjectType
        {
            get { return OBJECT_TYPE; }
        }
    }
}