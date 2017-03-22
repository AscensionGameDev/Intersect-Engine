using System.Collections.Generic;
using System.Linq;
using Intersect;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_1.Intersect_Convert_Lib.GameObjects.Events
{
    public class EventBase : DatabaseObject
    {
        public new const string DATABASE_TABLE = "events";
        public new const GameObject OBJECT_TYPE = GameObject.CommonEvent;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        public string MyName { get; set; }
        public int MyIndex { get; set; }
        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
        public bool CommonEvent { get; set; }
        public byte IsGlobal { get; set; }
        public List<EventPage> MyPages { get; set; }

        public EventBase(int index, int x, int y, bool isCommon = false, byte isGlobal = 0) : base (index)
        {
            MyName = "";
            if (isCommon) MyName = "Common Event " + index;
            MyIndex = index;
            SpawnX = x;
            SpawnY = y;
            CommonEvent = isCommon;
            IsGlobal = isGlobal;
            MyPages = new List<EventPage> {new EventPage()};
        }

        public EventBase(int index, EventBase copy) : base(index)
        {
            MyName = "New Event";
            MyPages = new List<EventPage>();
            MyIndex = index;
            Load(copy.EventData());
            CommonEvent = copy.CommonEvent;
        }
        public EventBase(int index, ByteBuffer myBuffer, bool isCommon = false) : base(index)
        {
            MyName = "New Event";
            MyPages = new List<EventPage>();
            MyIndex = index;
            Load(myBuffer.ToArray());
        }
        public byte[] EventData()
        {
            var myBuffer = new ByteBuffer();
                myBuffer.WriteString(MyName);
                myBuffer.WriteInteger(SpawnX);
                myBuffer.WriteInteger(SpawnY);
                myBuffer.WriteByte(IsGlobal);
                myBuffer.WriteInteger(MyPages.Count);
                for (var i = 0; i < MyPages.Count; i++)
                {
                    MyPages[i].WriteBytes(myBuffer);
                }
            return myBuffer.ToArray();
        }

        public override void Load(byte[] packet)
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteBytes(packet);
                MyName = myBuffer.ReadString();
                SpawnX = myBuffer.ReadInteger();
                SpawnY = myBuffer.ReadInteger();
                IsGlobal = myBuffer.ReadByte();
                int pageCount = myBuffer.ReadInteger();
                MyPages.Clear();
                for (var i = 0; i < pageCount; i++)
                {
                    MyPages.Add(new EventPage(myBuffer));
                }
        }

        public static EventBase GetEvent(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (EventBase)Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((EventBase)Objects[index]).MyName;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return EventData();
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
        public static Dictionary<int, EventBase> GetObjects()
        {
            Dictionary<int, EventBase> objects = Objects.ToDictionary(k => k.Key, v => (EventBase)v.Value);
            return objects;
        }
    }
}
