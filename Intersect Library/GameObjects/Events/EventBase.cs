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
        public int Map { get; set; } = -1;
        public int SpawnX { get; set; } = -1;
        public int SpawnY { get; set; } = -1;
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
        public EventBase(Guid id, int map, int x, int y, bool isCommon = false, byte isGlobal = 0) : base(id)
        {
            Name = "";
            Map = map;
            if (isCommon) Name = "Common Event " + id;
            SpawnX = x;
            SpawnY = y;
            CommonEvent = isCommon;
            IsGlobal = isGlobal;
            Pages = new List<EventPage> {new EventPage()};
        }

        public EventBase(Guid id, bool isCommon = false) : base(id)
        {
            Name = "New Event";
            Pages = new List<EventPage>();
            CommonEvent = isCommon;
        }

        public EventBase(Guid id, EventBase copy) : base(id)
        {
            Name = "New Event";
            Pages = new List<EventPage>();
            Load(copy.JsonData);
            CommonEvent = copy.CommonEvent;
        }

        public EventBase(Guid id, string json, bool isCommon = false) : base(id)
        {
            Name = "New Event";
            CommonEvent = isCommon;
            Pages = new List<EventPage>();
            Load(json);
        }
    }
}