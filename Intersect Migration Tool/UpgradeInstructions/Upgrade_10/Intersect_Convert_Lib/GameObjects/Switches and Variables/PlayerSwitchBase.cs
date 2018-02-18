using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib.GameObjects.Switches_and_Variables
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