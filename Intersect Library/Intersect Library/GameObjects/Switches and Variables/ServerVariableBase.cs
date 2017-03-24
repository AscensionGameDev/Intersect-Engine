using System.Collections.Generic;

namespace Intersect.GameObjects
{
    public class ServerVariableBase : DatabaseObject<ServerVariableBase>
    {
        //Core info
        public new const string DATABASE_TABLE = "server_variables";
        public new const GameObject OBJECT_TYPE = GameObject.ServerVariable;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

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
        public override string DatabaseTableName => DATABASE_TABLE;
        public override GameObject GameObjectType => OBJECT_TYPE;
    }
}