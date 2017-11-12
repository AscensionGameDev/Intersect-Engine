namespace Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.GameObjects.Switches_and_Variables
{
    public class PlayerVariableBase : DatabaseObject<PlayerVariableBase>
    {
        public PlayerVariableBase(int id) : base(id)
        {
            Name = "New Player Variable";
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