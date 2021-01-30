using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Intersect.Collections;
using Intersect.Compression;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Models;

using Newtonsoft.Json;

namespace Intersect.GameObjects.Maps
{

    public class MapBase : DatabaseObject<MapBase>
    {
        [NotMapped]
        [JsonIgnore]
        protected JsonSerializerSettings mJsonSerializerSettings { get; } = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        [NotMapped] [JsonIgnore]        public readonly Dictionary<Guid, EventBase> LocalEvents = new Dictionary<Guid, EventBase>();

        //Client/Editor Only
        [JsonIgnore] [NotMapped] public MapAutotiles Autotiles;

        [NotMapped] public List<Guid> EventIds = new List<Guid>();

        [NotMapped] [JsonIgnore] public List<EventBase> EventsCache = new List<EventBase>();

        //Core Data
        [JsonIgnore] [NotMapped] public Dictionary<string, Tile[,]> Layers = new Dictionary<string, Tile[,]>();

        //Map Attributes
        private MapAttribute[,] mAttributes = new MapAttribute[Options.MapWidth, Options.MapHeight];

        //Cached Att Data
        private byte[] mCachedAttributeData = null;

        //SyncLock
        [JsonIgnore] [NotMapped] protected object mMapLock = new object();

        [JsonConstructor]
        public MapBase(Guid id) : base(id)
        {
            Name = "New Map";

            //Create empty tile array and then compress it down
            if (Layers == null)
            {
                Layers = new Dictionary<string,Tile[,]>();
                TileData = LZ4.PickleString(JsonConvert.SerializeObject(Layers, Formatting.None, mJsonSerializerSettings));
                Layers = null;
            }
            else
            {
                TileData = LZ4.PickleString(JsonConvert.SerializeObject(Layers, Formatting.None, mJsonSerializerSettings));
            }

            mCachedAttributeData = LZ4.PickleString(JsonConvert.SerializeObject(Attributes, Formatting.None, mJsonSerializerSettings));
        }

        //EF Constructor
        public MapBase()
        {
            Name = "New Map";
        }

        public MapBase(MapBase mapBase) : base(mapBase?.Id ?? Guid.Empty)
        {
            if (mapBase == null)
            {
                return;
            }

            lock (MapLock ?? throw new ArgumentNullException(nameof(MapLock), @"this"))
            {
                lock (mapBase.MapLock ?? throw new ArgumentNullException(nameof(mapBase.MapLock), nameof(mapBase)))
                {
                    Name = mapBase.Name;
                    Brightness = mapBase.Brightness;
                    IsIndoors = mapBase.IsIndoors;
                    if (Layers != null && mapBase.Layers != null)
                    {

                        Layers.Clear();

                        foreach (var layer in mapBase.Layers)
                        {
                            var tiles = new Tile[Options.MapWidth, Options.MapHeight];
                            for (var x = 0; x < Options.MapWidth; x++)
                            {
                                for (var y = 0; y < Options.MapHeight; y++)
                                {
                                    tiles[x, y] = new Tile
                                    {
                                        TilesetId = layer.Value[x, y].TilesetId,
                                        X = layer.Value[x, y].X,
                                        Y = layer.Value[x, y].Y,
                                        Autotile = layer.Value[x, y].Autotile
                                    };
                                }
                            }
                            Layers.Add(layer.Key, tiles);
                        }
                    }

                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            if (Attributes == null)
                            {
                                continue;
                            }

                            if (mapBase.Attributes?[x, y] == null)
                            {
                                Attributes[x, y] = null;
                            }
                            else
                            {
                                Attributes[x, y] = mapBase.Attributes[x, y].Clone();
                            }
                        }
                    }

                    for (var i = 0; i < mapBase.Spawns?.Count; i++)
                    {
                        Spawns.Add(new NpcSpawn(mapBase.Spawns[i]));
                    }

                    for (var i = 0; i < mapBase.Lights?.Count; i++)
                    {
                        Lights.Add(new LightBase(mapBase.Lights[i]));
                    }

                    foreach (var record in mapBase.LocalEvents)
                    {
                        var evt = new EventBase(record.Key, record.Value?.JsonData);
                        LocalEvents?.Add(record.Key, evt);
                    }

                    EventIds?.Clear();
                    EventIds?.AddRange(mapBase.EventIds?.ToArray() ?? new Guid[] { });
                }
            }
        }

        //For server only
        [JsonIgnore]
        public byte[] TileData { get; set; }

        public Guid Up { get; set; }

        public Guid Down { get; set; }

        public Guid Left { get; set; }

        public Guid Right { get; set; }

        public int Revision { get; set; }

        [Column("Attributes")]
        [JsonIgnore]
        public byte[] AttributeData
        {
            get => GetAttributeData();
            set
            {
                var str = LZ4.UnPickleString(value);
                mAttributes = JsonConvert.DeserializeObject<MapAttribute[,]>(LZ4.UnPickleString(value), mJsonSerializerSettings);
                mCachedAttributeData = value;
            }
        }

        [NotMapped]
        [JsonIgnore]
        public MapAttribute[,] Attributes
        {
            get => mAttributes ?? (mAttributes = new MapAttribute[Options.MapWidth, Options.MapHeight]);

            set
            {
                mAttributes = value;
                mCachedAttributeData = LZ4.PickleString(JsonConvert.SerializeObject(mAttributes, Formatting.None, mJsonSerializerSettings));
            }
        }

        [Column("Lights")]
        [JsonIgnore]
        public string LightsJson
        {
            get => JsonConvert.SerializeObject(Lights);
            set
            {
                Lights.Clear();
                var lights = JsonConvert.DeserializeObject<List<LightBase>>(value);
                if (lights != null)
                {
                    Lights.AddRange(lights);
                }
            }
        }

        [NotMapped]
        [JsonProperty]
        public List<LightBase> Lights { get; private set; } = new List<LightBase>();

        [Column("Events")]
        [JsonIgnore]
        public string EventIdsJson
        {
            get => JsonConvert.SerializeObject(EventIds);
            set => EventIds = JsonConvert.DeserializeObject<List<Guid>>(value);
        }

        [NotMapped]
        public string LocalEventsJson
        {
            get => JsonConvert.SerializeObject(
                LocalEvents,
                new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
            set => JsonConvert.PopulateObject(
                value, LocalEvents,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                }
            );
        }

        [Column("NpcSpawns")]
        [JsonIgnore]
        public string NpcSpawnsJson
        {
            get => JsonConvert.SerializeObject(Spawns);
            set
            {
                Spawns.Clear();

                var spawns = JsonConvert.DeserializeObject<List<NpcSpawn>>(value);
                if (spawns != null)
                {
                    Spawns.AddRange(spawns);
                }
            }
        }

        [NotMapped]
        [JsonProperty]
        public List<NpcSpawn> Spawns { get; private set; } = new List<NpcSpawn>();

        //Properties
        public string Music { get; set; } = null;

        public string Sound { get; set; } = null;

        public bool IsIndoors { get; set; }

        public string Panorama { get; set; } = null;

        public string Fog { get; set; } = null;

        public int FogXSpeed { get; set; }

        public int FogYSpeed { get; set; }

        public int FogTransparency { get; set; }

        public int RHue { get; set; }

        public int GHue { get; set; }

        public int BHue { get; set; }

        public int AHue { get; set; }

        public int Brightness { get; set; } = 100;

        public MapZones ZoneType { get; set; } = MapZones.Normal;

        public int PlayerLightSize { get; set; } = 300;

        public byte PlayerLightIntensity { get; set; } = 255;

        public float PlayerLightExpand { get; set; }

        [Column("PlayerLightColor")]
        [JsonIgnore]
        public string PlayerLightColorJson
        {
            get => JsonConvert.SerializeObject(PlayerLightColor);
            set => PlayerLightColor = JsonConvert.DeserializeObject<Color>(value);
        }

        [NotMapped]
        public Color PlayerLightColor { get; set; } = Color.White;

        public string OverlayGraphic { get; set; } = null;

        //Weather
        [Column("WeatherAnimation")]
        [JsonProperty]
        public Guid WeatherAnimationId { get; protected set; }

        [NotMapped]
        [JsonIgnore]
        public AnimationBase WeatherAnimation
        {
            get => AnimationBase.Get(WeatherAnimationId);
            set => WeatherAnimationId = value?.Id ?? Guid.Empty;
        }

        public int WeatherXSpeed { get; set; }

        public int WeatherYSpeed { get; set; }

        public int WeatherIntensity { get; set; }

        [NotMapped]
        [JsonIgnore]
        public object MapLock => mMapLock;

        public virtual MapBase[,] GenerateAutotileGrid()
        {
            return null;
        }

        public virtual byte[] GetAttributeData()
        {
            return mCachedAttributeData;
        }

        public class MapInstances : DatabaseObjectLookup
        {

            private readonly DatabaseObjectLookup mBaseLookup;

            public MapInstances(DatabaseObjectLookup baseLookup) : base(baseLookup.StoredType)
            {
                if (baseLookup == null)
                {
                    throw new ArgumentNullException();
                }

                mBaseLookup = baseLookup;
            }

            internal override bool InternalSet(IDatabaseObject value, bool overwrite)
            {
                mBaseLookup?.InternalSet(value, overwrite);

                return base.InternalSet(value, overwrite);
            }

            public override bool Delete(IDatabaseObject value)
            {
                mBaseLookup?.Delete(value);

                return base.Delete(value);
            }

            public override void Clear()
            {
                mBaseLookup?.Clear();
                base.Clear();
            }

        }

    }

}
