namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Switches_and_Variables
{
    public class ServerVariableBase : DatabaseObject<ServerVariableBase>
    {
        public int Value;

        public ServerVariableBase(int id) : base(id)
        {
            Name = "New Global Variable";
        }

        public override byte[] BinaryData => Data();

        public override void Load(byte[] packet)
        {
            var myBuffer = new Upgrade_10.Intersect_Convert_Lib.ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            Value = myBuffer.ReadInteger();
            myBuffer.Dispose();
        }

        public byte[] Data()
        {
            var myBuffer = new Upgrade_10.Intersect_Convert_Lib.ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(Value);
            return myBuffer.ToArray();
        }
    }
}