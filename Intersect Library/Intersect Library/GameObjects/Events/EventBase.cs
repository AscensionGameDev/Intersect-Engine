using System.Collections.Generic;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Events
{
    public class EventBase : DatabaseObject<EventBase>
    {
        internal EventBase(int id) : base(id)
        {
        }

        [JsonConstructor]
        public EventBase(int index, int mapIndex, int x, int y, bool isCommon = false, byte isGlobal = 0) : base(index)
        {
            Name = "";
            MapIndex = mapIndex;
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
            Load(copy.JsonData);
            CommonEvent = copy.CommonEvent;
        }

        public EventBase(int index, string json, bool isCommon = false) : base(index)
        {
            Name = "New Event";
            CommonEvent = isCommon;
            MyPages = new List<EventPage>();
            Load(json);
        }

        public int MapIndex { get; set; } //Used for assigning move routes and such.. will be replaced with guid soon.
        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
        public bool CommonEvent { get; set; }
        public byte IsGlobal { get; set; }
        public List<EventPage> MyPages { get; set; }
    }
}