using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Intersect.Models;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Events
{

    public class EventBase : DatabaseObject<EventBase>, IFolderable
    {

        //Cached Pages Data
        private string mCachedPagesData = null;

        //Event Pages
        private List<EventPage> mPages = new List<EventPage>();

        //EF Parameterless Constructor
        public EventBase()
        {
        }

        [JsonConstructor]
        public EventBase(Guid id, Guid mapId, int x, int y, bool isCommon = false, bool isGlobal = false) : base(id)
        {
            Name = "New Event";
            MapId = mapId;
            if (isCommon)
            {
                Name = "New Common Event";
            }

            SpawnX = x;
            SpawnY = y;
            CommonEvent = isCommon;
            Global = isGlobal;
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

        public Guid MapId { get; set; }

        public int SpawnX { get; set; } = -1;

        public int SpawnY { get; set; } = -1;

        public bool CommonEvent { get; set; }

        public bool Global { get; set; }

        [JsonIgnore]
        [Column("Pages")]
        public string PagesJson
        {
            get => mCachedPagesData;
            protected set => Pages = JsonConvert.DeserializeObject<List<EventPage>>(
                value,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
        }

        [NotMapped]
        public List<EventPage> Pages
        {
            get => mPages;
            set
            {
                mPages = value;
                mCachedPagesData = JsonConvert.SerializeObject(
                    Pages,
                    new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                        ObjectCreationHandling = ObjectCreationHandling.Replace
                    }
                );
            }
        }

        public new static string[] Names => Lookup.Where(pair => ((EventBase) pair.Value)?.CommonEvent ?? false)
            .OrderBy(p => p.Value?.TimeCreated)
            .Select(pair => pair.Value?.Name ?? Deleted)
            .ToArray();

        public new static KeyValuePair<Guid, string>[] ItemPairs => Lookup
            .Where(pair => ((EventBase) pair.Value)?.CommonEvent ?? false)
            .OrderBy(p => p.Value?.TimeCreated)
            .Select(pair => new KeyValuePair<Guid, string>(pair.Key, pair.Value?.Name ?? Deleted))
            .ToArray();

        [JsonIgnore]
        [NotMapped]
        public override string JsonData => JsonConvert.SerializeObject(
            this, Formatting.Indented,
            new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            }
        );

        /// <inheritdoc />
        public string Folder { get; set; } = "";

        public new static Guid IdFromList(int listIndex)
        {
            if (listIndex < 0)
            {
                return Guid.Empty;
            }

            var commonEvents = Lookup.Where(pair => ((EventBase) pair.Value)?.CommonEvent ?? false)
                .OrderBy(p => p.Value?.TimeCreated)
                .ToArray();

            if (listIndex > commonEvents.Length)
            {
                return Guid.Empty;
            }

            return commonEvents[listIndex].Value?.Id ?? Guid.Empty;
        }

        public new static EventBase FromList(int listIndex)
        {
            return Get(IdFromList(listIndex));
        }

        public new static int ListIndex(Guid id)
        {
            var commonEvents = Lookup.Where(pair => ((EventBase) pair.Value)?.CommonEvent ?? false)
                .OrderBy(p => p.Value?.TimeCreated)
                .ToArray();

            for (var i = 0; i < commonEvents.Length; i++)
            {
                if (commonEvents[i].Key == id)
                {
                    return i;
                }
            }

            return -1;
        }

        public new int ListIndex()
        {
            return ListIndex(Id);
        }

        public override void Load(string json, bool keepCreationTime = false)
        {
            var oldTime = TimeCreated;
            JsonConvert.PopulateObject(
                json, this,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );

            if (keepCreationTime)
            {
                TimeCreated = oldTime;
            }
        }

    }

}
