using System.Collections.Generic;
using System.Linq;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_7.Intersect_Convert_Lib.GameObjects.
    Switches_and_Variables
{
    public class PlayerSwitchBase : DatabaseObject
    {
        //Core info
        public new const string DATABASE_TABLE = "player_switches";
        public new const GameObject OBJECT_TYPE = GameObject.PlayerSwitch;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public string Name = "New Player Switch";

        public PlayerSwitchBase(int id) : base(id)
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

        public static PlayerSwitchBase GetSwitch(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (PlayerSwitchBase) Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((PlayerSwitchBase) Objects[index]).Name;
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

        public static Dictionary<int, PlayerSwitchBase> GetObjects()
        {
            Dictionary<int, PlayerSwitchBase> objects = Objects.ToDictionary(k => k.Key, v => (PlayerSwitchBase) v.Value);
            return objects;
        }
    }
}