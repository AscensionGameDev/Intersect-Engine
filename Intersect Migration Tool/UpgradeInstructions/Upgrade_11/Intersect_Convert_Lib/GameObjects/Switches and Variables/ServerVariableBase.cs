using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Switches_and_Variables
{
    public class ServerVariableBase : DatabaseObject<ServerVariableBase>
    {
        public int Value;

        [JsonConstructor]
        public ServerVariableBase(int index) : base(index)
        {
            Name = "New Global Variable";
        }

        public override byte[] BinaryData => Data();

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
    }
}