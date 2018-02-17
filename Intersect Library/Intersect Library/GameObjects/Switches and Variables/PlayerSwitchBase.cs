using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class PlayerSwitchBase : DatabaseObject<PlayerSwitchBase>
    {
        [JsonConstructor]
        public PlayerSwitchBase(int index) : base(index)
        {
            Name = "New Player Switch";
        }

        public override byte[] BinaryData => Data();

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
    }
}