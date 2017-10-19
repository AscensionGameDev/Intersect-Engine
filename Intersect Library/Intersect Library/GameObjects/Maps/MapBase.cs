using System;
using System.Collections.Generic;
using Intersect.Collections;
using Intersect.Enums;
using Intersect.GameObjects.Events;
using Intersect.Localization;
using Intersect.Models;

namespace Intersect.GameObjects.Maps
{
    public class MapBase : DatabaseObject<MapBase>
    {
        //SyncLock
        protected object _mapLock = new object();

        //Client/Editor Only
        public MapAutotiles Autotiles;

        //Server/Editor Only
        public int EventIndex;

        //Temporary Values
        public bool IsClient;

        //Core Data
        public TileArray[] Layers = new TileArray[Options.LayerCount];

        //For server only
        public byte[] TileData;

        public MapBase(int mapNum, bool isClient) : base(mapNum)
        {
            Name = "New Map";
            IsClient = isClient;
        }

        public MapBase(MapBase mapcopy) : base(mapcopy.Index)
        {
            lock (GetMapLock())
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
                                value = mapcopy.Attributes[x, y].value,
                                data1 = mapcopy.Attributes[x, y].data1,
                                data2 = mapcopy.Attributes[x, y].data2,
                                data3 = mapcopy.Attributes[x, y].data3,
                                data4 = mapcopy.Attributes[x, y].data4
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
                    bf.WriteBytes(evt.Value.EventData());
                    Events.Add(evt.Key, new EventBase(evt.Key, bf));
                    bf.Clear();
                }
            }
        }

        public int Up { get; set; } = -1;
        public int Down { get; set; } = -1;
        public int Left { get; set; } = -1;
        public int Right { get; set; } = -1;
        public int Revision { get; set; }
        public Attribute[,] Attributes { get; set; } = new Attribute[Options.MapWidth, Options.MapHeight];
        public List<LightBase> Lights { get; set; } = new List<LightBase>();
        public Dictionary<int, EventBase> Events { get; set; } = new Dictionary<int, EventBase>();
        public List<NpcSpawn> Spawns { get; set; } = new List<NpcSpawn>();
        public List<ResourceSpawn> ResourceSpawns { get; set; } = new List<ResourceSpawn>();

        //Properties
        public string Music { get; set; } = Strings.Get("general", "none");

        public string Sound { get; set; } = Strings.Get("general", "none");
        public bool IsIndoors { get; set; }
        public string Panorama { get; set; } = Strings.Get("general", "none");
        public string Fog { get; set; } = Strings.Get("general", "none");
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
        public string OverlayGraphic { get; set; } = Strings.Get("general", "none");

        public override byte[] BinaryData => GetMapData(false);

        public object GetMapLock()
        {
            return _mapLock;
        }

        public override void Load(byte[] packet)
        {
            lock (GetMapLock())
            {
                var bf = new ByteBuffer();
                bf.WriteBytes(packet);
                Name = bf.ReadString();
                Revision = bf.ReadInteger();
                Up = bf.ReadInteger();
                Down = bf.ReadInteger();
                Left = bf.ReadInteger();
                Right = bf.ReadInteger();
                Music = bf.ReadString();
                Sound = bf.ReadString();
                IsIndoors = Convert.ToBoolean(bf.ReadInteger());
                Panorama = bf.ReadString();
                Fog = bf.ReadString();
                FogXSpeed = bf.ReadInteger();
                FogYSpeed = bf.ReadInteger();
                FogTransparency = bf.ReadInteger();
                RHue = bf.ReadInteger();
                GHue = bf.ReadInteger();
                BHue = bf.ReadInteger();
                AHue = bf.ReadInteger();
                Brightness = bf.ReadInteger();
                ZoneType = (MapZones) bf.ReadByte();
                OverlayGraphic = bf.ReadString();
                PlayerLightSize = bf.ReadInteger();
                PlayerLightExpand = (float) bf.ReadDouble();
                PlayerLightIntensity = bf.ReadByte();
                PlayerLightColor = Color.FromArgb(bf.ReadByte(), bf.ReadByte(), bf.ReadByte());

                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        int attributeType = bf.ReadInteger();
                        if (attributeType > 0)
                        {
                            Attributes[x, y] = new Attribute
                            {
                                value = attributeType,
                                data1 = bf.ReadInteger(),
                                data2 = bf.ReadInteger(),
                                data3 = bf.ReadInteger(),
                                data4 = bf.ReadString()
                            };
                        }
                        else
                        {
                            Attributes[x, y] = null;
                        }
                    }
                }
                var lCount = bf.ReadInteger();
                Lights.Clear();
                for (var i = 0; i < lCount; i++)
                {
                    Lights.Add(new LightBase(bf));
                }

                if (!IsClient)
                {
                    // Load Map Npcs
                    Spawns.Clear();
                    var npcCount = bf.ReadInteger();
                    for (var i = 0; i < npcCount; i++)
                    {
                        var TempNpc = new NpcSpawn
                        {
                            NpcNum = bf.ReadInteger(),
                            X = bf.ReadInteger(),
                            Y = bf.ReadInteger(),
                            Dir = bf.ReadInteger()
                        };
                        Spawns.Add(TempNpc);
                    }
                    Events.Clear();
                    EventIndex = bf.ReadInteger();
                    var ecount = bf.ReadInteger();
                    for (var i = 0; i < ecount; i++)
                    {
                        var eventIndex = bf.ReadInteger();
                        var evtDataLen = bf.ReadLong();
                        var evtBuffer = new ByteBuffer();
                        evtBuffer.WriteBytes(bf.ReadBytes((int) evtDataLen));
                        Events.Add(eventIndex, new EventBase(eventIndex, evtBuffer));
                        evtBuffer.Dispose();
                    }
                }
            }
        }

        public virtual byte[] GetMapData(bool forClient)
        {
            var bf = new ByteBuffer();
            bf.WriteString(Name);
            bf.WriteInteger(Revision);
            bf.WriteInteger(Up);
            bf.WriteInteger(Down);
            bf.WriteInteger(Left);
            bf.WriteInteger(Right);
            bf.WriteString(Music);
            bf.WriteString(Sound);
            bf.WriteInteger(Convert.ToInt32(IsIndoors));
            bf.WriteString(Panorama);
            bf.WriteString(Fog);
            bf.WriteInteger(FogXSpeed);
            bf.WriteInteger(FogYSpeed);
            bf.WriteInteger(FogTransparency);
            bf.WriteInteger(RHue);
            bf.WriteInteger(GHue);
            bf.WriteInteger(BHue);
            bf.WriteInteger(AHue);
            bf.WriteInteger(Brightness);
            bf.WriteByte((byte) ZoneType);
            bf.WriteString(OverlayGraphic);
            bf.WriteInteger(PlayerLightSize);
            bf.WriteDouble(PlayerLightExpand);
            bf.WriteByte(PlayerLightIntensity);
            bf.WriteByte(PlayerLightColor.R);
            bf.WriteByte(PlayerLightColor.G);
            bf.WriteByte(PlayerLightColor.B);

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
                        bf.WriteInteger(Attributes[x, y].value);
                        if (Attributes[x, y].value > 0)
                        {
                            bf.WriteInteger(Attributes[x, y].data1);
                            bf.WriteInteger(Attributes[x, y].data2);
                            bf.WriteInteger(Attributes[x, y].data3);
                            bf.WriteString(Attributes[x, y].data4);
                        }
                    }
                }
            }
            bf.WriteInteger(Lights.Count);
            foreach (var t in Lights)
            {
                bf.WriteBytes(t.LightData());
            }

            if (!forClient)
            {
                // Save Map Npcs
                bf.WriteInteger(Spawns.Count);
                for (var i = 0; i < Spawns.Count; i++)
                {
                    bf.WriteInteger(Spawns[i].NpcNum);
                    bf.WriteInteger(Spawns[i].X);
                    bf.WriteInteger(Spawns[i].Y);
                    bf.WriteInteger(Spawns[i].Dir);
                }

                bf.WriteInteger(EventIndex);
                bf.WriteInteger(Events.Count);
                foreach (var t in Events)
                {
                    bf.WriteInteger(t.Key);
                    var evtData = t.Value.EventData();
                    bf.WriteLong(evtData.Length);
                    bf.WriteBytes(evtData);
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