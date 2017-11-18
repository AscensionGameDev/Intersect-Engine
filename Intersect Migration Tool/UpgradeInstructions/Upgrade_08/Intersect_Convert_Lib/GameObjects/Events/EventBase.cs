using System.Collections.Generic;
using Intersect.Migration.UpgradeInstructions.Upgrade_10.Intersect_Convert_Lib;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.GameObjects.Events
{
    public class EventBase : DatabaseObject<EventBase>
    {
        internal EventBase(int id) : base(id)
        {
        }

        public EventBase(int index, int x, int y, bool isCommon = false, byte isGlobal = 0) : base(index)
        {
            Name = "";
            if (isCommon) Name = "Common Event " + index;
            SpawnX = x;
            SpawnY = y;
            CommonEvent = isCommon;
            IsGlobal = isGlobal;
            MyPages = new List<EventPage> {new EventPage()};
        }

        public EventBase(int index, EventBase copy) : base(index)
        {
            Name = "New Event";
            MyPages = new List<EventPage>();
            Load(copy.EventData());
            CommonEvent = copy.CommonEvent;
        }

        public EventBase(int index, ByteBuffer myBuffer, bool isCommon = false) : base(index)
        {
            Name = "New Event";
            MyPages = new List<EventPage>();
            Load(myBuffer.ToArray());
        }

        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
        public bool CommonEvent { get; set; }
        public byte IsGlobal { get; set; }
        public List<EventPage> MyPages { get; set; }

        public override byte[] BinaryData => EventData();

        public byte[] EventData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteString(Name);
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
            Name = myBuffer.ReadString();
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
    }
}