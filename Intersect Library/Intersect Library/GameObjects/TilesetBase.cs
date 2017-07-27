using System.Text;
using Intersect.Models;

namespace Intersect.GameObjects
{
    public class TilesetBase : DatabaseObject<TilesetBase>
    {
        public TilesetBase(int id) : base(id)
        {
            Name = "";
        }

        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value?.Trim().ToLower(); }
        }

        public override void Load(byte[] packet) => Name = Encoding.ASCII.GetString(packet, 0, packet.Length);
        
        public override byte[] BinaryData => Encoding.ASCII.GetBytes(Name);
    }
}