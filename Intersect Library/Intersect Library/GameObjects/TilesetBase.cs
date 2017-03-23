using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect.GameObjects
{
    public class TilesetBase : DatabaseObject<TilesetBase>
    {
        //Core info
        public new const string DATABASE_TABLE = "tilesets";
        public new const GameObject OBJECT_TYPE = GameObject.Tileset;

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
            return Lookup.Get(index);
        }

        public override byte[] BinaryData => Data();

        public override string DatabaseTableName
        {
            get { return DATABASE_TABLE; }
        }

        public override GameObject GameObjectType
        {
            get { return OBJECT_TYPE; }
        }

        public static void ClearObjects()
        {
            Lookup.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            Lookup.Add(index, (TilesetBase)obj);
        }
    }
}