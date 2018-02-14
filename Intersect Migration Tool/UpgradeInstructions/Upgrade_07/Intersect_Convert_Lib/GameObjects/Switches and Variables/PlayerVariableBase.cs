using System.Collections.Generic;
using System.Linq;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_7.Intersect_Convert_Lib.GameObjects.
    Switches_and_Variables
{
    public class PlayerVariableBase : DatabaseObject
    {
        //Core info
        public new const string DATABASE_TABLE = "player_variables";

        public new const GameObject OBJECT_TYPE = GameObject.PlayerVariable;
        protected static Dictionary<int, DatabaseObject> sObjects = new Dictionary<int, DatabaseObject>();

        public string Name = "New Player Variable";

        public PlayerVariableBase(int id) : base(id)
        {
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

        public static PlayerVariableBase GetVariable(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return (PlayerVariableBase) sObjects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (sObjects.ContainsKey(index))
            {
                return ((PlayerVariableBase) sObjects[index]).Name;
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

        public static Dictionary<int, PlayerVariableBase> GetObjects()
        {
            Dictionary<int, PlayerVariableBase> objects = sObjects.ToDictionary(k => k.Key,
                v => (PlayerVariableBase) v.Value);
            return objects;
        }
    }
}