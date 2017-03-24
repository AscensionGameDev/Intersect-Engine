using System.Collections.Generic;
using System.Linq;

namespace Intersect.GameObjects
{
    public class PlayerVariableBase : DatabaseObject<PlayerVariableBase>
    {
        //Core info
        public new const string DATABASE_TABLE = "player_variables";
        public new const GameObject OBJECT_TYPE = GameObject.PlayerVariable;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public PlayerVariableBase(int id) : base(id)
        {
            Name = "New Player Variable";
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