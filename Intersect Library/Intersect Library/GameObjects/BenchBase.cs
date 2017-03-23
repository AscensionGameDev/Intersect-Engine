using System.Collections.Generic;
using System.Linq;

namespace Intersect.GameObjects
{
    public class BenchBase : DatabaseObject<BenchBase>
    {
        public new const string DATABASE_TABLE = "crafts";
        public new const GameObject OBJECT_TYPE = GameObject.Bench;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public List<Craft> Crafts = new List<Craft>();

        public BenchBase(int id) : base(id)
        {
            Name = "New Bench";
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);

            Name = myBuffer.ReadString();
            int count = myBuffer.ReadInteger();

            Crafts.Clear();

            for (int i = 0; i < count; i++)
            {
                Crafts.Add(new Craft());
                Crafts[i].Load(myBuffer);
            }

            myBuffer.Dispose();
        }

        public byte[] CraftData()
        {
            using (var myBuffer = new ByteBuffer())
            {

                myBuffer.WriteString(Name);
                myBuffer.WriteInteger(Crafts.Count);
                foreach (var craft in Crafts)
                {
                    myBuffer.WriteBytes(craft.Data());
                }

                return myBuffer.ToArray();
            }
        }

        public static BenchBase GetCraft(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (BenchBase) Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((BenchBase) Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] BinaryData => CraftData();

        public override string DatabaseTableName
        {
            get { return DATABASE_TABLE; }
        }

        public override GameObject GameObjectType
        {
            get { return OBJECT_TYPE; }
        }

        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }

        public static int ObjectCount()
        {
            return Objects.Count;
        }

        public static Dictionary<int, BenchBase> GetObjects()
        {
            Dictionary<int, BenchBase> objects = Objects.ToDictionary(k => k.Key, v => (BenchBase) v.Value);
            return objects;
        }

        public override void Delete()
        {
            Objects.Remove(Id);
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
    }

    public class Craft
    {
        public List<CraftIngredient> Ingredients = new List<CraftIngredient>();
        public int Item = -1;
        public int Time = 1;

        public void Load(ByteBuffer bf)
        {
            Item = bf.ReadInteger();
            Time = bf.ReadInteger();
            Ingredients.Clear();
            var count = bf.ReadInteger();
            for (int i = 0; i < count; i++)
            {
                var craftIngredient = new CraftIngredient(bf.ReadInteger(), bf.ReadInteger());
                Ingredients.Add(craftIngredient);
            }
        }

        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(Item);
            bf.WriteInteger(Time);
            bf.WriteInteger(Ingredients.Count);
            for (int i = 0; i < Ingredients.Count; i++)
            {
                bf.WriteInteger(Ingredients[i].Item);
                bf.WriteInteger(Ingredients[i].Quantity);
            }
            return bf.ToArray();
        }
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