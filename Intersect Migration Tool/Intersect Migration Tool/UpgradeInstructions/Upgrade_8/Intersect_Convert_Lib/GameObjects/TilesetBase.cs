using System.Text;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects
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
            set { base.Name = value?.Trim(); }
        }

        public override byte[] BinaryData => Encoding.ASCII.GetBytes(Name);

        public override void Load(byte[] packet) => Name = Encoding.ASCII.GetString(packet, 0, packet.Length);
    }
}