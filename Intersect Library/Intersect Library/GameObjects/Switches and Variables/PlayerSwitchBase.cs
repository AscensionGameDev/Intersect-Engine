using Intersect.Models;

namespace Intersect.GameObjects
{
    public class PlayerSwitchBase : DatabaseObject<PlayerSwitchBase>
    {
        public PlayerSwitchBase(int id) : base(id)
        {
            Name = "New Player Switch";
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            myBuffer.Dispose();
        }

        public byte[] Data()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            return myBuffer.ToArray();
        }

        public override byte[] BinaryData => Data();
    }
}