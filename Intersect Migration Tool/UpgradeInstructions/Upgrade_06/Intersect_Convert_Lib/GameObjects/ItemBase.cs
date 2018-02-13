using System.Collections.Generic;
using System.Linq;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_6.Intersect_Convert_Lib.GameObjects
{
    public class ItemBase : DatabaseObject
    {
        public new const string DATABASE_TABLE = "items";
        public new const GameObject OBJECT_TYPE = GameObject.Item;
        protected static Dictionary<int, DatabaseObject> sObjects = new Dictionary<int, DatabaseObject>();
        public int Animation;
        public int AttackAnimation = -1;
        public int Bound;
        public int ClassReq = -1;
        public int CritChance;
        public int Damage;
        public int DamageType;
        public int Data1;
        public int Data2;
        public int Data3;
        public int Data4;
        public string Desc = "";
        public string FemalePaperdoll = "";
        public int GenderReq;
        public int ItemType;
        public int LevelReq;
        public string MalePaperdoll = "";

        public string Name = "New Item";
        public string Pic = "";
        public int Price;
        public int Projectile = -1;
        public int Scaling;
        public int ScalingStat;
        public int Speed;
        public int StatGrowth;
        public int[] StatsGiven;
        public int[] StatsReq;
        public int Tool = -1;

        public ItemBase(int id) : base(id)
        {
            Speed = 10; // Set to 10 by default.
            StatsReq = new int[Options.MaxStats];
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
            Animation = myBuffer.ReadInteger();
            ClassReq = myBuffer.ReadInteger();
            LevelReq = myBuffer.ReadInteger();
            GenderReq = myBuffer.ReadInteger();
            Projectile = myBuffer.ReadInteger();
            AttackAnimation = myBuffer.ReadInteger();

            for (var i = 0; i < Options.MaxStats; i++)
            {
                StatsReq[i] = myBuffer.ReadInteger();
                StatsGiven[i] = myBuffer.ReadInteger();
            }

            StatGrowth = myBuffer.ReadInteger();
            Damage = myBuffer.ReadInteger();
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
            myBuffer.WriteInteger(Animation);
            myBuffer.WriteInteger(ClassReq);
            myBuffer.WriteInteger(LevelReq);
            myBuffer.WriteInteger(GenderReq);
            myBuffer.WriteInteger(Projectile);
            myBuffer.WriteInteger(AttackAnimation);

            for (var i = 0; i < Options.MaxStats; i++)
            {
                myBuffer.WriteInteger(StatsReq[i]);
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

        public static ItemBase GetItem(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return (ItemBase) sObjects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return ((ItemBase) sObjects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return ItemData();
        }

        public override string GetTable()
        {
            return DATABASE_TABLE;
        }

        public override GameObject GetGameObjectType()
        {
            return OBJECT_TYPE;
        }

        public static DatabaseObject Get(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return sObjects[index];
            }
            return null;
        }

        public override void Delete()
        {
            sObjects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            sObjects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            sObjects.Remove(index);
            sObjects.Add(index, obj);
        }

        public static int ObjectCount()
        {
            return sObjects.Count;
        }

        public static Dictionary<int, ItemBase> GetObjects()
        {
            Dictionary<int, ItemBase> objects = sObjects.ToDictionary(k => k.Key, v => (ItemBase) v.Value);
            return objects;
        }
    }
}