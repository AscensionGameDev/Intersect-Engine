using System.Collections.Generic;

namespace Intersect_Library.GameObjects.Events
{
    public class EventStruct
    {
        public string MyName { get; set; }
        public int MyIndex { get; set; }
        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
        public int Deleted { get; set; }
        public bool CommonEvent { get; set; }
        public byte IsGlobal { get; set; }
        public List<EventPage> MyPages { get; set; }

        public EventStruct(int index, int x, int y, bool isCommon = false, byte isGlobal = 0)
        {
            MyName = "";
            if (isCommon) MyName = "Common Event " + index;
            MyIndex = index;
            SpawnX = x;
            SpawnY = y;
            CommonEvent = isCommon;
            IsGlobal = isGlobal;
            MyPages = new List<EventPage>();
            MyPages.Add(new EventPage());
        }

        public EventStruct(int index, EventStruct copy)
        {
            MyName = "New Event";
            MyPages = new List<EventPage>();
            ByteBuffer myBuffer = new ByteBuffer();
            MyIndex = index;
            myBuffer.WriteBytes(copy.EventData());
            Deleted = myBuffer.ReadInteger();
            if (Deleted != 1)
            {
                MyName = myBuffer.ReadString();
                SpawnX = myBuffer.ReadInteger();
                SpawnY = myBuffer.ReadInteger();
                IsGlobal = myBuffer.ReadByte();
                int pageCount = myBuffer.ReadInteger();
                CommonEvent = copy.CommonEvent;
                for (var i = 0; i < pageCount; i++)
                {
                    MyPages.Add(new EventPage(myBuffer));
                }
            }
        }
        public EventStruct(int index, ByteBuffer myBuffer, bool isCommon = false)
        {
            MyName = "New Event";
            MyPages = new List<EventPage>();
            MyIndex = index;
            Deleted = myBuffer.ReadInteger();
            if (Deleted != 1)
            {
                MyName = myBuffer.ReadString();
                SpawnX = myBuffer.ReadInteger();
                SpawnY = myBuffer.ReadInteger();
                IsGlobal = myBuffer.ReadByte();
                int pageCount = myBuffer.ReadInteger();
                CommonEvent = isCommon;
                for (var i = 0; i < pageCount; i++)
                {
                    MyPages.Add(new EventPage(myBuffer));
                }
            }
        }
        public byte[] EventData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(Deleted);
            if (Deleted == 0)
            {
                myBuffer.WriteString(MyName);
                myBuffer.WriteInteger(SpawnX);
                myBuffer.WriteInteger(SpawnY);
                myBuffer.WriteByte(IsGlobal);
                myBuffer.WriteInteger(MyPages.Count);
                for (var i = 0; i < MyPages.Count; i++)
                {
                    MyPages[i].WriteBytes(myBuffer);
                }
            }
            return myBuffer.ToArray();
        }
    }
}
