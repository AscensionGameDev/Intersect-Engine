using System.Collections.Generic;
using System.Linq;
using Intersect;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_2.Intersect_Convert_Lib.GameObjects
{
    public class ItemBase : DatabaseObject
    {
        public new const string DATABASE_TABLE = "items";
        public new const GameObject OBJECT_TYPE = GameObject.Item;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
        public int Animation;
        public int Bound;
        public int ClassReq = -1;
        public int Damage;
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
        public int Speed;
        public int StatGrowth;
        public int[] StatsGiven;
        public int[] StatsReq;
        public int Tool;

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

            for (var i = 0; i < Options.MaxStats; i++)
            {
                myBuffer.WriteInteger(StatsReq[i]);
                myBuffer.WriteInteger(StatsGiven[i]);
            }

            myBuffer.WriteInteger(StatGrowth);
            myBuffer.WriteInteger(Damage);
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
            if (Objects.ContainsKey(index))
            {
                return (ItemBase) Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((ItemBase) Objects[index]).Name;
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
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }

        public override void Delete()
        {
            Objects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            Objects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Remove(index);
            Objects.Add(index, obj);
        }

        public static int ObjectCount()
        {
            return Objects.Count;
        }

        public static Dictionary<int, ItemBase> GetObjects()
        {
            Dictionary<int, ItemBase> objects = Objects.ToDictionary(k => k.Key, v => (ItemBase) v.Value);
            return objects;
        }
    }
}