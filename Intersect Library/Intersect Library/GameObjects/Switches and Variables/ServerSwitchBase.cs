using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.GameObjects
{
    public class ServerSwitchBase : DatabaseObject<ServerSwitchBase>
    {
        //Core info
        public new const string DATABASE_TABLE = "server_switches";
        public new const GameObject OBJECT_TYPE = GameObject.ServerSwitch;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public bool Value = false;

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
        public override string DatabaseTableName => DATABASE_TABLE;
        public override GameObject GameObjectType => OBJECT_TYPE;
    }
}