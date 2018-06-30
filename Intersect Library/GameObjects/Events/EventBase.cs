using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Maps;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Events
{
    public class EventBase : DatabaseObject<EventBase>
    {
        [Column("Map")]
        public int MapId { get; protected set; } = -1;
        [NotMapped]
        [JsonIgnore]
        public MapBase Map
        {
            get => MapBase.Lookup.Get<MapBase>(MapId);
            set => MapId = value?.Index ?? -1;
        }

        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
        public bool CommonEvent { get; set; }
        public byte IsGlobal { get; set; }

        [JsonIgnore]
        [Column("Pages")]
        public string PagesJson
        {
            get => JsonConvert.SerializeObject(Pages, Formatting.None);
            protected set => Pages = JsonConvert.DeserializeObject<List<EventPage>>(value);
        }
        [NotMapped]
        public List<EventPage> Pages { get; set; } = new List<EventPage>();

        public static EventBase Get(int index)
        {
            return EventBase.Lookup.Get<EventBase>(index);
        }

        public static EventBase Get(Guid id)
        {
            return EventBase.Lookup.Get<EventBase>(id);
        }

        internal EventBase(int id) : base(id)
        {
        }

        //EF Parameterless Constructor
        public EventBase()
        {
            
        }

        [JsonConstructor]
        public EventBase(int index, int mapIndex, int x, int y, bool isCommon = false, byte isGlobal = 0) : base(index)
        {
            Name = "";
            MapId = mapIndex;
            if (isCommon) Name = "Common Event " + index;
            SpawnX = x;
            SpawnY = y;
            CommonEvent = isCommon;
            IsGlobal = isGlobal;
            Pages = new List<EventPage> {new EventPage()};
        }

        public EventBase(int index,int mapIndex, EventBase copy) : base(index)
        {
            Name = "New Event";
            Pages = new List<EventPage>();
            Load(copy.JsonData);
            CommonEvent = copy.CommonEvent;
            MapId = mapIndex;
        }

        public EventBase(int index, string json, bool isCommon = false) : base(index)
        {
            Name = "New Event";
            CommonEvent = isCommon;
            Pages = new List<EventPage>();
            Load(json);
        }
    }
}