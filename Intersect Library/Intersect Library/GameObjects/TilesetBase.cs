using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Library.GameObjects
{
    public class TilesetBase : DatabaseObject
    {
        //Core info
        public new const string DatabaseTable = "tilesets";
        public new const GameObject Type = GameObject.Tileset;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public TilesetBase(int id) : base(id)
        {
            Name = "";
        }

        public void SetValue(string filename)
        {
            Name = filename.Trim();
        }

        public string GetValue()
        {
            return Name;
        }

        public override void Load(byte[] packet)
        {
            Name = Encoding.ASCII.GetString(packet, 0, packet.Length);
        }

        public byte[] Data()
        {
            return Encoding.ASCII.GetBytes(Name);
        }

        public static TilesetBase GetTileset(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (TilesetBase)Objects[index];
            }
            return null;
        }


        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((TilesetBase)Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return Data();
        }

        public override string GetTable()
        {
            return DatabaseTable;
        }

        public override GameObject GetGameObjectType()
        {
            return Type;
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
        public static int ObjectCount()
        {
            return Objects.Count;
        }
        public static Dictionary<int, TilesetBase> GetObjects()
        {
            Dictionary<int, TilesetBase> objects = Objects.ToDictionary(k => k.Key, v => (TilesetBase)v.Value);
            return objects;
        }
    }
}
