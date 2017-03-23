using System.Collections.Generic;
using System.Linq;
using Intersect;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib.GameObjects.
    Switches_and_Variables
{
    public class ServerVariableBase : DatabaseObject
    {
        //Core info
        public new const string DATABASE_TABLE = "server_variables";
        public new const GameObject OBJECT_TYPE = GameObject.ServerVariable;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public string Name = "New Global Variable";
        public int Value = 0;

        public ServerVariableBase(int id) : base(id)
        {
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

        public static ServerVariableBase GetVariable(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (ServerVariableBase) Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((ServerVariableBase) Objects[index]).Name;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return Data();
        }

        public override string GetTable()
        {
            return DATABASE_TABLE;
        }

        public override GameObject GetGameObjectType()
        {
            return OBJECT_TYPE;
        }

        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }

        public override void Delete()
        {
            Objects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            Objects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Remove(index);
            Objects.Add(index, obj);
        }

        public static int ObjectCount()
        {
            return Objects.Count;
        }

        public static Dictionary<int, ServerVariableBase> GetObjects()
        {
            Dictionary<int, ServerVariableBase> objects = Objects.ToDictionary(k => k.Key,
                v => (ServerVariableBase) v.Value);
            return objects;
        }
    }
}