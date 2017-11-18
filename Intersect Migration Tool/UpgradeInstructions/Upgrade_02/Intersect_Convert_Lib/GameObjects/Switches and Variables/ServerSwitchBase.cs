using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_2.Intersect_Convert_Lib.GameObjects.
    Switches_and_Variables
{
    public class ServerSwitchBase : DatabaseObject
    {
        //Core info
        public new const string DATABASE_TABLE = "server_switches";

        public new const GameObject OBJECT_TYPE = GameObject.ServerSwitch;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public string Name = "New Global Switch";
        public bool Value;

        public ServerSwitchBase(int id) : base(id)
        {
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

        public static ServerSwitchBase GetSwitch(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (ServerSwitchBase) Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((ServerSwitchBase) Objects[index]).Name;
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

        public static Dictionary<int, ServerSwitchBase> GetObjects()
        {
            Dictionary<int, ServerSwitchBase> objects =
                Objects.ToDictionary(k => k.Key, v => (ServerSwitchBase) v.Value);
            return objects;
        }
    }
}