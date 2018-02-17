using System.Text;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class TilesetBase : DatabaseObject<TilesetBase>
    {
        [JsonConstructor]
        public TilesetBase(int index) : base(index)
        {
            Name = "";
        }

        public new string Name
        {
            get { return base.Name; }
            set { base.Name = value?.Trim().ToLower(); }
        }

        public override byte[] BinaryData => Encoding.UTF8.GetBytes(Name);

        public override void Load(byte[] packet) => Name = Encoding.UTF8.GetString(packet, 0, packet.Length);
    }
}