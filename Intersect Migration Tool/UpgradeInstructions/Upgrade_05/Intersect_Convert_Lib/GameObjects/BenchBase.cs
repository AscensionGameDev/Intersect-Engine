using System.Collections.Generic;
using System.Linq;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_5.Intersect_Convert_Lib.GameObjects
{
    public class BenchBase : DatabaseObject
    {
        public new const string DATABASE_TABLE = "crafts";
        public new const GameObject OBJECT_TYPE = GameObject.Bench;
        protected static Dictionary<int, DatabaseObject> sObjects = new Dictionary<int, DatabaseObject>();
        public List<Bench> Crafts = new List<Bench>();

        public string Name = "New Bench";

        public BenchBase(int id) : base(id)
        {
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new Upgrade_10.Intersect_Convert_Lib.ByteBuffer();
            myBuffer.WriteBytes(packet);

            Name = myBuffer.ReadString();
            int count = myBuffer.ReadInteger();

            Crafts.Clear();

            for (int i = 0; i < count; i++)
            {
                Crafts.Add(new Bench());
                Crafts[i].Item = myBuffer.ReadInteger();
                Crafts[i].Time = myBuffer.ReadInteger();

                // Load Ingredients
                Crafts[i].Ingredients.Clear();
                var ingredientCount = myBuffer.ReadInteger();
                for (var n = 0; n < ingredientCount; n++)
                {
                    Crafts[i].Ingredients.Add(new CraftIngredient(myBuffer.ReadInteger(), myBuffer.ReadInteger()));
                }
            }

            myBuffer.Dispose();
        }

        public byte[] CraftData()
        {
            var myBuffer = new Upgrade_10.Intersect_Convert_Lib.ByteBuffer();

            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(Crafts.Count);
            foreach (var craft in Crafts)
            {
                myBuffer.WriteInteger(craft.Item);
                myBuffer.WriteInteger(craft.Time);

                myBuffer.WriteInteger(craft.Ingredients.Count);
                for (var i = 0; i < craft.Ingredients.Count; i++)
                {
                    myBuffer.WriteInteger(craft.Ingredients[i].Item);
                    myBuffer.WriteInteger(craft.Ingredients[i].Quantity);
                }
            }

            return myBuffer.ToArray();
        }

        public static BenchBase GetCraft(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return (BenchBase) sObjects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return ((BenchBase) sObjects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return CraftData();
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

        public static int ObjectCount()
        {
            return sObjects.Count;
        }

        public static Dictionary<int, BenchBase> GetObjects()
        {
            Dictionary<int, BenchBase> objects = sObjects.ToDictionary(k => k.Key, v => (BenchBase) v.Value);
            return objects;
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
    }

    public class Bench
    {
        public List<CraftIngredient> Ingredients = new List<CraftIngredient>();
        public int Item = -1;
        public int Time = 1;
    }

    public class CraftIngredient
    {
        public int Item = -1;
        public int Quantity = 1;

        public CraftIngredient(int item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }
}