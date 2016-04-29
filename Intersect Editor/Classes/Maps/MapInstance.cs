using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect_Editor.Classes.Entities;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;

namespace Intersect_Editor.Classes.Maps
{
    public class MapInstance : MapStruct
    {
        public const string Version = "0.0.0.1";

        //Map Attributes
        private Dictionary<Intersect_Library.GameObjects.Maps.Attribute, AnimationInstance> _attributeAnimInstances = new Dictionary<Intersect_Library.GameObjects.Maps.Attribute, AnimationInstance>();


        public MapInstance(int mapNum) : base(mapNum)
        {
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
            Autotiles = new MapAutotiles(this);
            Autotiles.InitAutotiles(gameMaps);
        }

        public MapInstance(MapStruct mapStruct) : base(mapStruct)
        {
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
            Autotiles = new MapAutotiles(this);
            Autotiles.InitAutotiles(gameMaps);
        }

        //Legacy Saving/Loading
        //Saving/Loading
        public byte[] Save()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(0); //Never deleted
            bf.WriteString(Version);
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
            bf.WriteInteger(Lights.Count);
            foreach (var t in Lights)
            {
                bf.WriteBytes(t.LightData());
            }

            // Save Map Npcs
            bf.WriteInteger(Spawns.Count);
            for (var i = 0; i < Spawns.Count; i++)
            {
                bf.WriteInteger(Spawns[i].NpcNum);
                bf.WriteInteger(Spawns[i].X);
                bf.WriteInteger(Spawns[i].Y);
                bf.WriteInteger(Spawns[i].Dir);
            }

            bf.WriteInteger(Events.Count);
            foreach (var t in Events)
            {
                bf.WriteBytes(t.EventData());
            }
            return bf.ToArray();
        }

        public void Load(byte[] myArr, bool import = false)
        {
            var npcCount = 0;
            NpcSpawn TempNpc = new NpcSpawn();
            var bf = new ByteBuffer();
            bf.WriteBytes(myArr);
            Deleted = bf.ReadInteger();
            if (Deleted == 0)
            {
                string loadedVersion = bf.ReadString();
                if (loadedVersion != Version)
                    throw new Exception("Failed to load Map #" + MyMapNum + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);

                MyName = bf.ReadString();
                Revision = bf.ReadInteger();
                if (!import)
                {
                    Up = bf.ReadInteger();
                    Down = bf.ReadInteger();
                    Left = bf.ReadInteger();
                    Right = bf.ReadInteger();
                }
                else
                {
                    bf.ReadInteger();
                    bf.ReadInteger();
                    bf.ReadInteger();
                    bf.ReadInteger();
                }
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
                PlayerLightExpand = (float)bf.ReadDouble();
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
                            Attributes[x, y].value = attributeType;
                            Attributes[x, y].data1 = bf.ReadInteger();
                            Attributes[x, y].data2 = bf.ReadInteger();
                            Attributes[x, y].data3 = bf.ReadInteger();
                            Attributes[x, y].data4 = bf.ReadString();
                        }
                        else
                        {
                            Attributes[x, y].value = 0;
                        }
                    }
                }
                var lCount = bf.ReadInteger();
                Lights.Clear();
                for (var i = 0; i < lCount; i++)
                {
                    Lights.Add(new Light(bf));
                }

                // Load Map Npcs
                Spawns.Clear();
                npcCount = bf.ReadInteger();
                for (var i = 0; i < npcCount; i++)
                {
                    TempNpc = new NpcSpawn();
                    TempNpc.NpcNum = bf.ReadInteger();
                    TempNpc.X = bf.ReadInteger();
                    TempNpc.Y = bf.ReadInteger();
                    TempNpc.Dir = bf.ReadInteger();
                    Spawns.Add(TempNpc);
                }

                Events.Clear();
                var eCount = bf.ReadInteger();
                for (var i = 0; i < eCount; i++)
                {
                    Events.Add(new EventStruct(i, bf));
                }
                Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
                Autotiles.InitAutotiles(gameMaps);
                UpdateAdjacentAutotiles();
            }
        }

        //New Loading
        //public void Load(byte[] data, bool import)
        //{
        //    var up = Up;
        //    var down = Down;
        //    var left = Left;
        //    var right = Right;
        //    Load(data);
        //    if (import)
        //    {
        //        Up = up;
        //        Down = down;
        //        Left = left;
        //        Right = right;
        //    }
        //}

        //Attribute/Animations
        public AnimationInstance GetAttributeAnimation(Intersect_Library.GameObjects.Maps.Attribute attr, int animNum)
        {
            if (!_attributeAnimInstances.ContainsKey(attr))
            {
                _attributeAnimInstances.Add(attr,new AnimationInstance(Globals.GameAnimations[animNum],true));
            }
            return _attributeAnimInstances[attr];
        }
        public void SetAttributeAnimation(Intersect_Library.GameObjects.Maps.Attribute attribute, AnimationInstance animationInstance)
        {
            if (_attributeAnimInstances.ContainsKey(attribute))
            {
                _attributeAnimInstances[attribute] = animationInstance;
            }
        }

        //Helper Functions
        public void UpdateAdjacentAutotiles()
        {
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
            if (Up > -1 && Globals.GameMaps.ContainsKey(Up))
            {
                for (int x = 0; x < Options.MapWidth; x++)
                {
                    for (int y = Options.MapHeight - 1; y < Options.MapHeight; y++)
                    {
                        Globals.GameMaps[Up].Autotiles.UpdateAutoTiles(x, y, gameMaps);
                    }
                }
            }
            if (Down > -1 && Globals.GameMaps.ContainsKey(Down))
            {
                for (int x = 0; x < Options.MapWidth; x++)
                {
                    for (int y = 0; y < 1; y++)
                    {
                        Globals.GameMaps[Down].Autotiles.UpdateAutoTiles(x, y, gameMaps);
                    }
                }
            }
            if (Left > -1 && Globals.GameMaps.ContainsKey(Left))
            {
                for (int x = Options.MapWidth - 1; x < Options.MapWidth; x++)
                {
                    for (int y = 0; y < Options.MapHeight; y++)
                    {
                        Globals.GameMaps[Left].Autotiles.UpdateAutoTiles(x, y, gameMaps);
                    }
                }
            }
            if (Right > -1 && Globals.GameMaps.ContainsKey(Right))
            {
                for (int x = 0; x < 1; x++)
                {
                    for (int y = 0; y < Options.MapHeight; y++)
                    {
                        Globals.GameMaps[Right].Autotiles.UpdateAutoTiles(x, y, gameMaps);
                    }
                }
            }
        }
        public EventStruct FindEventAt(int x, int y)
        {
            if (Events.Count <= 0) return null;
            foreach (var t in Events)
            {
                if (t.Deleted == 1) continue;
                if (t.SpawnX == x && t.SpawnY == y)
                {
                    return t;
                }
            }
            return null;
        }
        public Light FindLightAt(int x, int y)
        {
            if (Lights.Count <= 0) return null;
            foreach (var t in Lights)
            {
                if (t.TileX == x && t.TileY == y)
                {
                    return t;
                }
            }
            return null;
        }
        public NpcSpawn FindSpawnAt(int x, int y)
        {
            if (Spawns.Count <= 0) return null;
            foreach (var t in Spawns)
            {
                if (t.X == x && t.Y == y)
                {
                    return t;
                }
            }
            return null;
        }

        public void Dispose()
        {

        }
    }
}
