using System;
using System.Collections.Generic;
using Intersect;
using Intersect_Migration_Tool.UpgradeInstructions.Upgrade_2.Intersect_Convert_Lib.GameObjects.Events;

namespace Intersect_Migration_Tool.UpgradeInstructions.Upgrade_2.Intersect_Convert_Lib.GameObjects.Maps
{
    public class MapBase : DatabaseObject
    {
        public new const string DATABASE_TABLE = "maps";
        public new const GameObject OBJECT_TYPE = GameObject.Map;
        protected static Dictionary<int, MapBase> Objects = new Dictionary<int, MapBase>();

        //SyncLock
        protected Object _mapLock = new Object();

        //Client/Editor Only
        public MapAutotiles Autotiles;

        //Server/Editor Only
        public int EventIndex = 0;

        //Temporary Values
        public bool IsClient = false;

        //Core Data
        public TileArray[] Layers = new TileArray[Options.LayerCount];

        public MapBase(int mapNum, bool isClient) : base(mapNum)
        {
            MyMapNum = mapNum;
            IsClient = isClient;
            for (var i = 0; i < Options.LayerCount; i++)
            {
                Layers[i].Tiles = new Tile[Options.MapWidth, Options.MapHeight];
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y].TilesetIndex = -1;
                        if (i == 0)
                        {
                            Attributes[x, y] = new Attribute();
                        }
                    }
                }
            }
        }

        public MapBase(MapBase mapcopy) : base(mapcopy.MyMapNum)
        {
            lock (_mapLock)
            {
                ByteBuffer bf = new ByteBuffer();
                MyName = mapcopy.MyName;
                Brightness = mapcopy.Brightness;
                IsIndoors = mapcopy.IsIndoors;
                for (var i = 0; i < Options.LayerCount; i++)
                {
                    Layers[i].Tiles = new Tile[Options.MapWidth, Options.MapHeight];
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            Layers[i].Tiles[x, y] = new Tile()
                            {
                                TilesetIndex = mapcopy.Layers[i].Tiles[x, y].TilesetIndex,
                                X = mapcopy.Layers[i].Tiles[x, y].X,
                                Y = mapcopy.Layers[i].Tiles[x, y].Y,
                                Autotile = mapcopy.Layers[i].Tiles[x, y].Autotile
                            };
                            if (i == 0 && mapcopy.Attributes[x, y] != null)
                            {
                                Attributes[x, y] = new Attribute()
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

        public string MyName { get; set; } = "New Map";
        public int Up { get; set; } = -1;
        public int Down { get; set; } = -1;
        public int Left { get; set; } = -1;
        public int Right { get; set; } = -1;
        public int MyMapNum { get; set; }
        public int Revision { get; set; }
        public Attribute[,] Attributes { get; set; } = new Attribute[Options.MapWidth, Options.MapHeight];
        public List<LightBase> Lights { get; set; } = new List<LightBase>();
        public Dictionary<int, EventBase> Events { get; set; } = new Dictionary<int, EventBase>();
        public List<NpcSpawn> Spawns { get; set; } = new List<NpcSpawn>();
        public List<ResourceSpawn> ResourceSpawns { get; set; } = new List<ResourceSpawn>();

        //Properties
        public string Music { get; set; } = "None";
        public string Sound { get; set; } = "None";
        public bool IsIndoors { get; set; }
        public string Panorama { get; set; } = "None";
        public string Fog { get; set; } = "None";
        public int FogXSpeed { get; set; } = 0;
        public int FogYSpeed { get; set; } = 0;
        public int FogTransparency { get; set; } = 0;
        public int RHue { get; set; } = 0;
        public int GHue { get; set; } = 0;
        public int BHue { get; set; } = 0;
        public int AHue { get; set; } = 0;
        public int Brightness { get; set; } = 100;
        public byte ZoneType { get; set; } = 0; //Everything goes, 1 is safe, add more later
        public int PlayerLightSize { get; set; } = 300;
        public byte PlayerLightIntensity { get; set; } = 255;
        public float PlayerLightExpand { get; set; } = 0f;
        public Color PlayerLightColor { get; set; } = Color.White;
        public string OverlayGraphic { get; set; } = "None";

        public override void Load(byte[] packet)
        {
            lock (_mapLock)
            {
                var bf = new ByteBuffer();
                bf.WriteBytes(packet);
                MyName = bf.ReadString();
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
                ZoneType = bf.ReadByte();
                OverlayGraphic = bf.ReadString();
                PlayerLightSize = bf.ReadInteger();
                PlayerLightExpand = (float) bf.ReadDouble();
                PlayerLightIntensity = bf.ReadByte();
                PlayerLightColor = Color.FromArgb(bf.ReadByte(), bf.ReadByte(), bf.ReadByte());

                for (var i = 0; i < Options.LayerCount; i++)
                {
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            Layers[i].Tiles[x, y].TilesetIndex = bf.ReadInteger();
                            Layers[i].Tiles[x, y].X = bf.ReadInteger();
                            Layers[i].Tiles[x, y].Y = bf.ReadInteger();
                            Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                        }
                    }
                }

                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        int attributeType = bf.ReadInteger();
                        if (attributeType > 0)
                        {
                            Attributes[x, y] = new Attribute()
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
                        var TempNpc = new NpcSpawn()
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
            bf.WriteString(MyName);
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
            bf.WriteByte(ZoneType);
            bf.WriteString(OverlayGraphic);
            bf.WriteInteger(PlayerLightSize);
            bf.WriteDouble(PlayerLightExpand);
            bf.WriteByte(PlayerLightIntensity);
            bf.WriteByte(PlayerLightColor.R);
            bf.WriteByte(PlayerLightColor.G);
            bf.WriteByte(PlayerLightColor.B);

            for (var i = 0; i < Options.LayerCount; i++)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        bf.WriteInteger(Layers[i].Tiles[x, y].TilesetIndex);
                        bf.WriteInteger(Layers[i].Tiles[x, y].X);
                        bf.WriteInteger(Layers[i].Tiles[x, y].Y);
                        bf.WriteByte(Layers[i].Tiles[x, y].Autotile);
                    }
                }
            }
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

        public static MapBase GetMap(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (MapBase) Objects[index];
            }
            return null;
        }

        public static string GetName(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return ((MapBase) Objects[index]).MyName;
            }
            return "Deleted";
        }

        public override byte[] GetData()
        {
            return GetMapData(false);
        }

        public override string GetTable()
        {
            return DATABASE_TABLE;
        }

        public override GameObject GetGameObjectType()
        {
            return OBJECT_TYPE;
        }

        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }

        public override void Delete()
        {
            Objects.Remove(GetId());
        }

        public static void ClearObjects()
        {
            Objects.Clear();
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Add(index, (MapBase) obj);
        }

        public static int ObjectCount()
        {
            return Objects.Count;
        }

        public static Dictionary<int, MapBase> GetObjects()
        {
            return Objects;
        }
    }
}