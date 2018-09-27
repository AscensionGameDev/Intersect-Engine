using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects
{
    public class TilesetBase : DatabaseObject
    {
        //Core info
        public new const string DATABASE_TABLE = "tilesets";

        public new const GameObject OBJECT_TYPE = GameObject.Tileset;
        protected static Dictionary<int, DatabaseObject> sObjects = new Dictionary<int, DatabaseObject>();

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
            if (sObjects.ContainsKey(index))
            {
                return (TilesetBase) sObjects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return ((TilesetBase) sObjects[index]).Value;
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

        public static Dictionary<int, TilesetBase> GetObjects()
        {
            Dictionary<int, TilesetBase> objects = sObjects.ToDictionary(k => k.Key, v => (TilesetBase) v.Value);
            return objects;
        }
    }
}