using Intersect.Models;

namespace Intersect.GameObjects
{
    public class PlayerVariableBase : DatabaseObject<PlayerVariableBase>
    {
        public PlayerVariableBase(int id) : base(id)
        {
            Name = "New Player Variable";
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