using System;
using System.Collections.Generic;
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
        protected object mMapLock = new object();

        //Client/Editor Only
        [JsonIgnore]
        public MapAutotiles Autotiles;

        //Server/Editor Only
        public int EventIndex;

        //Temporary Values
        [JsonIgnore]
        public bool IsClient;

        //Core Data
        [JsonIgnore]
        public TileArray[] Layers = new TileArray[Options.LayerCount];

        //For server only
        [JsonIgnore]
        public byte[] TileData;

        [JsonConstructor]
        public MapBase(int index, bool isClient) : base(index)
        {
            Name = "New Map";
            IsClient = isClient;
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
                    EventIndex = mapcopy.EventIndex;
                    foreach (var evt in mapcopy.Events)
                    {
                        Events.Add(evt.Key, new EventBase(evt.Key, evt.Value.JsonData));
                    }
                }
            }
        }

        public int Up { get; set; } = -1;
        public int Down { get; set; } = -1;
        public int Left { get; set; } = -1;
        public int Right { get; set; } = -1;
        public int Revision { get; set; }
        [JsonIgnore]
        public Attribute[,] Attributes { get; set; } = new Attribute[Options.MapWidth, Options.MapHeight];
        public List<LightBase> Lights { get; set; } = new List<LightBase>();
        public Dictionary<int, EventBase> Events { get; set; } = new Dictionary<int, EventBase>();
        public List<NpcSpawn> Spawns { get; set; } = new List<NpcSpawn>();
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
        public Color PlayerLightColor { get; set; } = Color.White;
        public string OverlayGraphic { get; set; } = null;

        //Weather
        public int Weather { get; set; } = -1;
        public int WeatherXSpeed { get; set;}
        public int WeatherYSpeed { get; set; }
        public int WeatherIntensity { get; set; }
        

        public object MapLock => mMapLock;

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