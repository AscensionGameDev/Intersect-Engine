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

        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value?.Trim(); }
        }

        public override void Load(byte[] packet) => Name = Encoding.ASCII.GetString(packet, 0, packet.Length);

        public byte[] Data() => Encoding.ASCII.GetBytes(Name);

        public override byte[] BinaryData => Data();

        public override string DatabaseTableName => DATABASE_TABLE;

        public override GameObject GameObjectType => OBJECT_TYPE;
    }
}