using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_6.Intersect_Convert_Lib.GameObjects
{
    public class TilesetBase : DatabaseObject
    {
        //Core info
        public new const string DATABASE_TABLE = "tilesets";

        public new const GameObject OBJECT_TYPE = GameObject.Tileset;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public string Value = "";

        public TilesetBase(int id) : base(id)
        {
        }

        public void SetValue(string filename)
        {
            Value = filename.Trim();
        }

        public string GetValue()
        {
            return Value;
        }

        public override void Load(byte[] packet)
        {
            Value = Encoding.ASCII.GetString(packet, 0, packet.Length);
        }

        public byte[] Data()
        {
            return Encoding.ASCII.GetBytes(Value);
        }

        public static TilesetBase GetTileset(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (TilesetBase) Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((TilesetBase) Objects[index]).Value;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return Data();
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

        public static Dictionary<int, TilesetBase> GetObjects()
        {
            Dictionary<int, TilesetBase> objects = Objects.ToDictionary(k => k.Key, v => (TilesetBase) v.Value);
            return objects;
        }
    }
}