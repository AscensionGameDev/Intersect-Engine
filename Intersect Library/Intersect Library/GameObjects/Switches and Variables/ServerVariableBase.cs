using Intersect.Models;

namespace Intersect.GameObjects
{
    public class ServerVariableBase : DatabaseObject<ServerVariableBase>
    {
        public int Value = 0;

        public ServerVariableBase(int id) : base(id)
        {
            Name = "New Global Variable";
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            Value = myBuffer.ReadInteger();
            myBuffer.Dispose();
        }

        public byte[] Data()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(Value);
            return myBuffer.ToArray();
        }

        public override byte[] BinaryData => Data();
    }
}