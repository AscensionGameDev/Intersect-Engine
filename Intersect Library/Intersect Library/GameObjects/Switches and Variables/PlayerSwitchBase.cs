using System.Collections.Generic;

namespace Intersect.GameObjects
{
    public class PlayerSwitchBase : DatabaseObject<PlayerSwitchBase>
    {
        //Core info
        public new const string DATABASE_TABLE = "player_switches";
        public new const GameObject OBJECT_TYPE = GameObject.PlayerSwitch;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

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
        public override string DatabaseTableName => DATABASE_TABLE;
        public override GameObject GameObjectType => OBJECT_TYPE;
    }
}