using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Collections;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects.Maps
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

        public int Up { get; set; } = -1;
        public int Down { get; set; } = -1;
        public int Left { get; set; } = -1;
        public int Right { get; set; } = -1;
        public int Revision { get; set; }

        [Column("Attributes")]
        [JsonIgnore]
        public byte[] AttributeData
        {
            get => AttributesData();
            set => LoadAttributes(value);
        }
        [NotMapped]
        [JsonIgnore]
        public Attribute[,] Attributes { get; set; } = new Attribute[Options.MapWidth, Options.MapHeight];

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

        [Column("ResourceSpawns")]
        [JsonIgnore]
        public string ResourceSpawnsJson
        {
            get => JsonConvert.SerializeObject(ResourceSpawns);
            set => ResourceSpawns = JsonConvert.DeserializeObject<List<ResourceSpawn>>(value);
        }
        [NotMapped]
        public List<ResourceSpawn> ResourceSpawns { get; set; } = new List<ResourceSpawn>();

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
        public int Weather { get; set; } = -1;
        public int WeatherXSpeed { get; set;}
        public int WeatherYSpeed { get; set; }
        public int WeatherIntensity { get; set; }
        
        [NotMapped]
        [JsonIgnore]
        public object MapLock => mMapLock;
        
        [JsonConstructor]
        public MapBase(int index, bool isClient) : base(index)
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

        public MapBase(MapBase mapcopy) : base(mapcopy.Index)
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
                                        TilesetIndex = mapcopy.Layers[i].Tiles[x, y].TilesetIndex,
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
                                Attributes[x, y] = new Attribute
                                {
                                    Value = mapcopy.Attributes[x, y].Value,
                                    Data1 = mapcopy.Attributes[x, y].Data1,
                                    Data2 = mapcopy.Attributes[x, y].Data2,
                                    Data3 = mapcopy.Attributes[x, y].Data3,
                                    Data4 = mapcopy.Attributes[x, y].Data4
                                };
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

        public void LoadAttributes(byte[] data)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(data);
            for (var x = 0; x < Options.MapWidth; x++)
            {
                for (var y = 0; y < Options.MapHeight; y++)
                {
                    int attributeType = bf.ReadInteger();
                    if (attributeType > 0)
                    {
                        Attributes[x, y] = new Attribute
                        {
                            Value = attributeType,
                            Data1 = bf.ReadInteger(),
                            Data2 = bf.ReadInteger(),
                            Data3 = bf.ReadInteger(),
                            Data4 = bf.ReadString()
                        };
                    }
                    else
                    {
                        Attributes[x, y] = null;
                    }
                }
            }
            bf.Dispose();
        }

        public byte[] AttributesData()
        {
            var bf = new ByteBuffer();
            for (var x = 0; x < Options.MapWidth; x++)
            {
                for (var y = 0; y < Options.MapHeight; y++)
                {
                    if (Attributes[x, y] == null)
                    {
                        bf.WriteInteger(0);
                    }
                    else
                    {
                        bf.WriteInteger(Attributes[x, y].Value);
                        if (Attributes[x, y].Value > 0)
                        {
                            bf.WriteInteger(Attributes[x, y].Data1);
                            bf.WriteInteger(Attributes[x, y].Data2);
                            bf.WriteInteger(Attributes[x, y].Data3);
                            bf.WriteString(Attributes[x, y].Data4);
                        }
                    }
                }
            }
            return bf.ToArray();
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