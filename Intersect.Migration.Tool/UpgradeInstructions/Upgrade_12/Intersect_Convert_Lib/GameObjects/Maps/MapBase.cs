using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Collections;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Config;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Enums;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Models;
using Newtonsoft.Json;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.GameObjects.Maps
{
    public class MapBase : DatabaseObject<MapBase>
    {
        //SyncLock
        [JsonIgnore]
        [NotMapped]
        protected object mMapLock = new object();

        //Client/Editor Only
        [JsonIgnore]
        [NotMapped]
        public MapAutotiles Autotiles;

        //Temporary Values
        [JsonIgnore]
        [NotMapped]
        public bool IsClient;

        //Core Data
        [JsonIgnore]
        [NotMapped]
        public TileArray[] Layers = new TileArray[Options.LayerCount];

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
            get => Compression.CompressPacket(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Attributes, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, ObjectCreationHandling = ObjectCreationHandling.Replace }).Replace("Intersect Migration Tool", "Intersect Core").Replace("Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib", "Intersect")));
            set => Attributes = JsonConvert.DeserializeObject<MapAttribute[,]>(System.Text.Encoding.UTF8.GetString(Compression.DecompressPacket(value)), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, ObjectCreationHandling = ObjectCreationHandling.Replace });
        }

        [NotMapped]
        [JsonIgnore]
        public MapAttribute[,] Attributes { get; set; } = new MapAttribute[Options.MapWidth, Options.MapHeight];

        [Column("Lights")]
        [JsonIgnore]
        public string LightsJson
        {
            get => JsonConvert.SerializeObject(Lights);
            set => Lights = JsonConvert.DeserializeObject<List<LightBase>>(value);
        }
        [NotMapped]
        public List<LightBase> Lights { get; set; } = new List<LightBase>();

        [Column("Events")]
        [JsonIgnore]
        public string EventIdsJson
        {
            get => JsonConvert.SerializeObject(EventIds);
            set => EventIds = JsonConvert.DeserializeObject<List<Guid>>(value);
        }
        [NotMapped]
        public List<Guid> EventIds = new List<Guid>();

        [NotMapped]
        public string LocalEventsJson
        {
            get => JsonConvert.SerializeObject(LocalEvents, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, ObjectCreationHandling = ObjectCreationHandling.Replace });
            set => JsonConvert.PopulateObject(value, LocalEvents, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, ObjectCreationHandling = ObjectCreationHandling.Replace });
        }
        [NotMapped]
        [JsonIgnore]
        public Dictionary<Guid, EventBase> LocalEvents = new Dictionary<Guid, EventBase>();

        [Column("NpcSpawns")]
        [JsonIgnore]
        public string NpcSpawnsJson
        {
            get => JsonConvert.SerializeObject(Spawns);
            set => Spawns = JsonConvert.DeserializeObject<List<NpcSpawn>>(value);
        }
        [NotMapped]
        public List<NpcSpawn> Spawns { get; set; } = new List<NpcSpawn>();

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
        public int WeatherXSpeed { get; set;}
        public int WeatherYSpeed { get; set; }
        public int WeatherIntensity { get; set; }
        
        [NotMapped]
        [JsonIgnore]
        public object MapLock => mMapLock;
        
        [JsonConstructor]
        public MapBase(Guid id, bool isClient) : base(id)
        {
            Name = "New Map";
            IsClient = isClient;
        }

        //EF Constructor
        public MapBase()
        {
            Name = "New Map";
            IsClient = false;
        }

        public MapBase(MapBase mapcopy) : base(mapcopy.Id)
        {
            lock (MapLock)
            {
                lock (mapcopy.MapLock)
                {
                    ByteBuffer bf = new ByteBuffer();
                    Name = mapcopy.Name;
                    Brightness = mapcopy.Brightness;
                    IsIndoors = mapcopy.IsIndoors;
                    if (Layers != null && mapcopy.Layers != null)
                    {
                        if (Layers.Length < Options.LayerCount) Layers = new TileArray[Options.LayerCount];
                        for (var i = 0; i < Options.LayerCount; i++)
                        {
                            Layers[i].Tiles = new Tile[Options.MapWidth, Options.MapHeight];
                            for (var x = 0; x < Options.MapWidth; x++)
                            {
                                for (var y = 0; y < Options.MapHeight; y++)
                                {
                                    Layers[i].Tiles[x, y] = new Tile
                                    {
                                        TilesetId = mapcopy.Layers[i].Tiles[x, y].TilesetId,
                                        X = mapcopy.Layers[i].Tiles[x, y].X,
                                        Y = mapcopy.Layers[i].Tiles[x, y].Y,
                                        Autotile = mapcopy.Layers[i].Tiles[x, y].Autotile
                                    };
                                }
                            }
                        }
                    }
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            if (mapcopy.Attributes[x, y] != null)
                            {
                                Attributes[x, y] = MapAttribute.CloneJson(mapcopy.Attributes[x, y]);
                            }
                        }
                    }
                    for (var i = 0; i < mapcopy.Spawns.Count; i++)
                    {
                        Spawns.Add(new NpcSpawn(mapcopy.Spawns[i]));
                    }
                    for (var i = 0; i < mapcopy.Lights.Count; i++)
                    {
                        Lights.Add(new LightBase(mapcopy.Lights[i]));
                    }
                    foreach (var record in mapcopy.LocalEvents)
                    {
                        var evt = new EventBase(record.Key, record.Value.JsonData);
                        LocalEvents.Add(record.Key,evt);
                    }
                    EventIds.Clear();
                    EventIds.AddRange(mapcopy.EventIds.ToArray());
                }
            }
        }

        public class MapInstances : DatabaseObjectLookup
        {
            private readonly DatabaseObjectLookup mBaseLookup;

            public MapInstances(DatabaseObjectLookup baseLookup)
            {
                if (baseLookup == null) throw new ArgumentNullException();
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