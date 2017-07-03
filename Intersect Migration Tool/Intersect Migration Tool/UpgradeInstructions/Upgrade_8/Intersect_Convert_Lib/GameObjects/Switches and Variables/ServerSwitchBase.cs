using System;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Switches_and_Variables
{
    public class ServerSwitchBase : DatabaseObject<ServerSwitchBase>
    {
        public bool Value;

        public ServerSwitchBase(int id) : base(id)
        {
            Name = "New Global Switch";
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
            Name = myBuffer.ReadString();
            Value = Convert.ToBoolean(myBuffer.ReadInteger());
            myBuffer.Dispose();
        }

        public byte[] Data()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
            myBuffer.WriteInteger(Convert.ToInt32(Value));
            return myBuffer.ToArray();
        }

        public override byte[] BinaryData => Data();
    }
}