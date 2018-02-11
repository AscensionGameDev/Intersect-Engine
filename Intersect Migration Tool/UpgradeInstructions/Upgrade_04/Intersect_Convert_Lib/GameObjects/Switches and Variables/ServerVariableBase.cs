using System.Collections.Generic;
using System.Linq;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_4.Intersect_Convert_Lib.GameObjects.
    Switches_and_Variables
{
    public class ServerVariableBase : DatabaseObject
    {
        //Core info
        public new const string DATABASE_TABLE = "server_variables";

        public new const GameObject OBJECT_TYPE = GameObject.ServerVariable;
        protected static Dictionary<int, DatabaseObject> sObjects = new Dictionary<int, DatabaseObject>();

        public string Name = "New Global Variable";
        public int Value;

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
            if (sObjects.ContainsKey(index))
            {
                return (ServerVariableBase) sObjects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return ((ServerVariableBase) sObjects[index]).Name;
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
            if (sObjects.ContainsKey(index))
            {
                return sObjects[index];
            }
            return null;
        }

        public override void Delete()
        {
            sObjects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            sObjects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            sObjects.Remove(index);
            sObjects.Add(index, obj);
        }

        public static int ObjectCount()
        {
            return sObjects.Count;
        }

        public static Dictionary<int, ServerVariableBase> GetObjects()
        {
            Dictionary<int, ServerVariableBase> objects = sObjects.ToDictionary(k => k.Key,
                v => (ServerVariableBase) v.Value);
            return objects;
        }
    }
}