/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using SFML.Graphics;
using Color = SFML.Graphics.Color;
using SFML.Window;
using SFML.System;

namespace Intersect_Client.Classes
{
    public class MapStruct
    {
        //Core
        public int MyMapNum;
        public string MyName = "New Map";
        public int Up = -1;
        public int Down = -1;
        public int Left = -1;
        public int Right = -1;
        public int Revision;

        //Camera Locking Variables
        public int HoldUp = 0;
        public int HoldDown = 0;
        public int HoldLeft = 0;
        public int HoldRight = 0;

        //World Position
        public int MapGridX = 0;
        public int MapGridY = 0;

        //Core Data
        public TileArray[] Layers = new TileArray[Constants.LayerCount];
        public Attribute[,] Attributes = new Attribute[Globals.MapWidth, Globals.MapHeight];
        public List<LightObj> Lights = new List<LightObj>();

        //Properties
        public string Music = "";
        public string Sound = "";
        public List<NpcSpawn> Spawns = new List<NpcSpawn>();
        public bool IsIndoors;

        //Visual Effect Properties
        public string Panorama = "None";
        public string Fog = "None";
        public int FogXSpeed = 0;
        public int FogYSpeed = 0;
        public int FogTransaprency = 0;
        public int RHue = 0;
        public int GHue = 0;
        public int BHue = 0;
        public int AHue = 0;
        public int Brightness = 0;

        //Temporary Values
        public bool MapLoaded;
        public bool MapRendered;
        public bool MapRendering = false;
        public byte[] MyPacket;
        public MapAutotiles Autotiles;
        public bool CacheCleared = true;
        public RenderTexture[] LowerTextures = new RenderTexture[3];
        public RenderTexture[] UpperTextures = new RenderTexture[3];
        public RenderTexture[] PeakTextures = new RenderTexture[3];
        public List<MapItemInstance> MapItems = new List<MapItemInstance>();
        public MapSound BackgroundSound;
        public List<MapSound> AttributeSounds = new List<MapSound>();
        private long _fogUpdateTime = Environment.TickCount;
        private float _curFogIntensity;
        private float _fogCurrentX;
        private float _fogCurrentY;
        private float _lastFogX;
        private float _lastFogY;
        private long _lastUpdateTime;

        private int _preRenderStage = 0;
        private int _preRenderLayer = 0;

        //Init
        public MapStruct(int mapNum, byte[] mapPacket)
        {
            MyMapNum = mapNum;
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                Layers[i] = new TileArray();
                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y] = new Tile();
                        if (i == 0) { Attributes[x, y] = new Attribute(); }
                    }
                }
            }
            MyPacket = mapPacket;
            Load();
            //Thread _renderThread = new Thread(new ThreadStart(PreRenderMap1));
            //_renderThread.Priority = ThreadPriority.Lowest;
            //_renderThread.Start();
            MapLoaded = true;

        }

        //Load
        private void Load()
        {
            var npcCount = 0;
            NpcSpawn TempNpc = new NpcSpawn();

            var bf = new ByteBuffer();
            bf.WriteBytes(MyPacket);
            MyName = bf.ReadString();
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
            FogTransaprency = bf.ReadInteger();
            RHue = bf.ReadInteger();
            GHue = bf.ReadInteger();
            BHue = bf.ReadInteger();
            AHue = bf.ReadInteger();
            Brightness = bf.ReadInteger();

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

            for (var i = 0; i < Constants.LayerCount; i++)
            {
                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y].TilesetIndex = bf.ReadInteger();
                        Layers[i].Tiles[x, y].X = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Y = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                    }
                }
            }
            for (var x = 0; x < Globals.MapWidth; x++)
            {
                for (var y = 0; y < Globals.MapHeight; y++)
                {
                    Attributes[x, y].value = bf.ReadInteger();
                    Attributes[x, y].data1 = bf.ReadInteger();
                    Attributes[x, y].data2 = bf.ReadInteger();
                    Attributes[x, y].data3 = bf.ReadInteger();
                    Attributes[x, y].data4 = bf.ReadString();
                }
            }
            var lCount = bf.ReadInteger();
            for (var i = 0; i < lCount; i++)
            {
                Lights.Add(new LightObj(bf));
            }
            Revision = bf.ReadInteger();
            bf.ReadLong();
            MapLoaded = true;
            Globals.ShouldUpdateLights = true;
            Autotiles = new MapAutotiles(this);
            Autotiles.InitAutotiles();
            UpdateAdjacentAutotiles();
            CreateMapSounds();
            MapRendered = false;
        }

        private void UpdateAdjacentAutotiles()
        {
            if (Up > -1 && Globals.GameMaps.ContainsKey(Up))
            {
                for (int x = 0; x < Globals.MapWidth; x++)
                {
                    for (int y = Globals.MapHeight - 1; y < Globals.MapHeight; y++)
                    {
                        Globals.GameMaps[Up].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
            if (Down > -1 && Globals.GameMaps.ContainsKey(Down))
            {
                for (int x = 0; x < Globals.MapWidth; x++)
                {
                    for (int y = 0; y < 1; y++)
                    {
                        Globals.GameMaps[Down].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
            if (Left > -1 && Globals.GameMaps.ContainsKey(Left))
            {
                for (int x = Globals.MapWidth - 1; x < Globals.MapWidth; x++)
                {
                    for (int y = 0; y < Globals.MapHeight; y++)
                    {
                        Globals.GameMaps[Left].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
            if (Right > -1 && Globals.GameMaps.ContainsKey(Right))
            {
                for (int x = 0; x < 1; x++)
                {
                    for (int y = 0; y < Globals.MapHeight; y++)
                    {
                        Globals.GameMaps[Right].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
        }
        public bool ShouldLoad(int index)
        {
            return true;
            if (Globals.MyIndex <= -1) return false;
            if (Globals.Entities.Count <= Globals.MyIndex) return false;
            switch (index)
            {
                case 0:
                    if (Globals.Entities[Globals.MyIndex].CurrentX < 18 && Globals.Entities[Globals.MyIndex].CurrentY < 18) { return true; }
                    break;

                case 1:
                    if (Globals.Entities[Globals.MyIndex].CurrentY < 18) { return true; }
                    break;

                case 2:
                    if (Globals.Entities[Globals.MyIndex].CurrentX > 11 && Globals.Entities[Globals.MyIndex].CurrentY < 18) { return true; }
                    break;

                case 3:
                    if (Globals.Entities[Globals.MyIndex].CurrentX < 18) { return true; }
                    break;

                case 4:
                    return true;

                case 5:
                    if (Globals.Entities[Globals.MyIndex].CurrentX > 11) { return true; }
                    break;

                case 6:
                    if (Globals.Entities[Globals.MyIndex].CurrentX < 18 && Globals.Entities[Globals.MyIndex].CurrentY > 11) { return true; }
                    break;

                case 7:
                    if (Globals.Entities[Globals.MyIndex].CurrentY > 11) { return true; }
                    break;

                case 8:
                    if (Globals.Entities[Globals.MyIndex].CurrentX > 11 && Globals.Entities[Globals.MyIndex].CurrentY > 11) { return true; }
                    break;
            }
            return false;
        }

        //Caching Functions
        public void PreRenderMap()
        {
            Graphics.PreRenderedMapLayer = true;
            if (_preRenderStage < 3)
            {
                int i = _preRenderStage;
                if (LowerTextures[i] == null)
                {
                    while (!Graphics.GetMapTexture(ref LowerTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                if (UpperTextures[i] == null)
                {
                    while (!Graphics.GetMapTexture(ref UpperTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                if (PeakTextures[i] == null)
                {
                    while (!Graphics.GetMapTexture(ref PeakTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                LowerTextures[i].Clear(Color.Transparent);
                UpperTextures[i].Clear(Color.Transparent);
                PeakTextures[i].Clear(Color.Transparent);

                _preRenderStage++;
                return;
            }
            else if (_preRenderStage >= 3 && _preRenderStage < 6)
            {
                int i = _preRenderStage - 3;
                int l = _preRenderLayer;
                _preRenderLayer++;
                if (l < 3)
                {
                    DrawMapLayer(LowerTextures[i], l, i);
                    LowerTextures[i].Display();
                    return;
                }
                else if (l == 3)
                {
                    DrawMapLayer(UpperTextures[i], l, i);
                    UpperTextures[i].Display();
                    return;
                }
                else
                {
                    DrawMapLayer(PeakTextures[i], l, i);
                    PeakTextures[i].Display();
                    _preRenderStage++;
                    _preRenderLayer = 0;
                    return;
                }
            }

            MapRendered = true;
            Graphics.LightsChanged = true;
        }
        public void PreRenderMap1()
        {
            long ectime = Environment.TickCount;
            long creationTime = 0;
            long renderingTime = 0;
            long displayTime = 0;
            for (var i = 0; i < 3; i++)
            {
                ectime = Environment.TickCount;
                if (LowerTextures[i] == null)
                {
                    while (!Graphics.GetMapTexture(ref LowerTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                if (UpperTextures[i] == null)
                {
                    while (!Graphics.GetMapTexture(ref UpperTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                if (PeakTextures[i] == null)
                {
                    while (!Graphics.GetMapTexture(ref PeakTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                creationTime += Environment.TickCount - ectime;
                LowerTextures[i].Clear(Color.Transparent);
                UpperTextures[i].Clear(Color.Transparent);
                PeakTextures[i].Clear(Color.Transparent);

                ectime = Environment.TickCount;
                for (var l = 0; l < Constants.LayerCount; l++)
                {
                    if (l < 3)
                    {
                        DrawMapLayer(LowerTextures[i], l, i);
                    }
                    else if (l == 3)
                    {
                        DrawMapLayer(UpperTextures[i], l, i);
                    }
                    else
                    {
                        DrawMapLayer(PeakTextures[i], l, i);
                    }
                }
                renderingTime += Environment.TickCount - ectime;
                ectime = Environment.TickCount;
                LowerTextures[i].Display();
                UpperTextures[i].Display();
                PeakTextures[i].Display();
                displayTime += Environment.TickCount - ectime;
                ectime = Environment.TickCount;
            }
            Debug.Print("Texture Creation Time:" + (creationTime));
            Debug.Print("Tile Rendering Time:" + (renderingTime));
            Debug.Print("Texture Display Time:" + (displayTime));
            MapRendered = true;
            Graphics.LightsChanged = true;
        }
        private void DrawAutoTile(int layerNum, float destX, float destY, int quarterNum, int x, int y, int forceFrame, RenderTarget tex)
        {
            int yOffset = 0, xOffset = 0;

            // calculate the offset
            switch (Layers[layerNum].Tiles[x, y].Autotile)
            {
                case Constants.AutotileWaterfall:
                    yOffset = (forceFrame - 1) * Globals.TileHeight;
                    break;

                case Constants.AutotileAnim:
                    xOffset = forceFrame * 64;
                    break;

                case Constants.AutotileCliff:
                    yOffset = -Globals.TileHeight;
                    break;
            }
            Graphics.RenderTexture(Graphics.Tilesets[Layers[layerNum].Tiles[x, y].TilesetIndex], destX, destY,
                (int)Autotiles.Autotile[x, y].Layer[layerNum].SrcX[quarterNum] + xOffset,
                (int)Autotiles.Autotile[x, y].Layer[layerNum].SrcY[quarterNum] + yOffset, 16, 16, tex);
        }
        private void DrawMapLayer(RenderTarget tex, int l, int z, float xoffset = 0, float yoffset = 0)
        {
            for (var x = 0; x < Globals.MapWidth; x++)
            {
                for (var y = 0; y < Globals.MapHeight; y++)
                {
                    if (Globals.Tilesets == null) continue;
                    if (Layers[l].Tiles[x, y].TilesetIndex < 0) continue;
                    if (Layers[l].Tiles[x, y].TilesetIndex >= Globals.Tilesets.Length) continue;
                    try
                    {
                        switch (Autotiles.Autotile[x, y].Layer[l].RenderState)
                        {
                            case Constants.RenderStateNormal:
                                Sprite tmpsprite = new Sprite(Graphics.Tilesets[Layers[l].Tiles[x, y].TilesetIndex]);
                                tmpsprite.Position = new Vector2f(x * Globals.TileWidth + xoffset,
                                    y * Globals.TileHeight + yoffset);
                                tmpsprite.TextureRect = new IntRect(Layers[l].Tiles[x, y].X * Globals.TileWidth,
                                    Layers[l].Tiles[x, y].Y * Globals.TileHeight, Globals.TileWidth, Globals.TileHeight);
                                tex.Draw(tmpsprite);
                                //Graphics.RenderTexture(Graphics.Tilesets[Layers[l].Tiles[x, y].TilesetIndex], x * Globals.TileWidth + xoffset, y * Globals.TileHeight + yoffset, Layers[l].Tiles[x, y].X * Globals.TileWidth, Layers[l].Tiles[x, y].Y * Globals.TileHeight, Globals.TileWidth, Globals.TileHeight, tex);
                                break;
                            case Constants.RenderStateAutotile:
                                DrawAutoTile(l, x * Globals.TileWidth + xoffset, y * Globals.TileHeight + yoffset, 1, x, y, z, tex);
                                DrawAutoTile(l, x * Globals.TileWidth + 16 + xoffset, y * Globals.TileHeight + yoffset, 2, x, y, z, tex);
                                DrawAutoTile(l, x * Globals.TileWidth + xoffset, y * Globals.TileHeight + 16 + yoffset, 3, x, y, z, tex);
                                DrawAutoTile(l, +x * Globals.TileWidth + 16 + xoffset, y * Globals.TileHeight + 16 + yoffset, 4, x, y, z, tex);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        //Rendering Functions
        public void Update(bool isLocal)
        {
            if (isLocal)
            {
                _lastUpdateTime = Environment.TickCount + 10000;
                if (BackgroundSound == null && Sound != "None" && Sound != "")
                {
                    BackgroundSound = Sounds.AddMapSound(Sound, -1, -1, MyMapNum, true, 10);
                }
            }
            else
            {
                if (Environment.TickCount > _lastUpdateTime || Graphics.FreeMapTextures.Count < 27)
                {
                    Dispose();
                }
            }
        }
        public void Draw(int layer = 0)
        {
            if (!MapRendered)
            {
                return;
            }
            if (layer == 0)
            {
                Graphics.RenderTexture(LowerTextures[Globals.AnimFrame].Texture, GetX(), GetY(), Graphics.RenderWindow);

                //Draw Map Items
                for (int i = 0; i < MapItems.Count; i++)
                {
                    if (Graphics.ItemFileNames.IndexOf(Globals.GameItems[MapItems[i].ItemNum].Pic) > -1)
                    {
                        Graphics.RenderTexture(Graphics.ItemTextures[Graphics.ItemFileNames.IndexOf(Globals.GameItems[MapItems[i].ItemNum].Pic)], GetX() + MapItems[i].X * Globals.TileWidth, GetY() + MapItems[i].Y * Globals.TileHeight, Graphics.RenderWindow);
                    }
                }
            }
            else if (layer == 1)
            {
                Graphics.RenderTexture(UpperTextures[Globals.AnimFrame].Texture, GetX(), GetY(), Graphics.RenderWindow);
            }
            else
            {
                Graphics.RenderTexture(PeakTextures[Globals.AnimFrame].Texture, GetX(), GetY(), Graphics.RenderWindow);
                DrawFog();
            }

        }
        public float GetX()
        {
            return MapGridX * Globals.MapWidth * Globals.TileWidth;
        }
        public float GetY()
        {
            return MapGridY * Globals.MapHeight * Globals.TileHeight;
        }

        //Fogs
        private void DrawFog()
        {
            float ecTime = Environment.TickCount - _fogUpdateTime;
            _fogUpdateTime = Environment.TickCount;
            if (MyMapNum == Globals.LocalMaps[4])
            {
                if (_curFogIntensity != 1)
                {
                    _curFogIntensity += (ecTime / 2000f);
                    if (_curFogIntensity > 1) { _curFogIntensity = 1; }
                }
            }
            else
            {
                if (_curFogIntensity != 0)
                {
                    _curFogIntensity -= (ecTime / 2000f);
                    if (_curFogIntensity < 0) { _curFogIntensity = 0; }
                }
            }
            if (Globals.GameMaps[MyMapNum].Fog.Length > 0)
            {
                if (Graphics.FogFileNames.IndexOf(Globals.GameMaps[MyMapNum].Fog) > -1)
                {
                    int fogIndex = Graphics.FogFileNames.IndexOf(Globals.GameMaps[MyMapNum].Fog);
                    int xCount = (int)(Graphics.ScreenWidth / Graphics.FogTextures[fogIndex].Size.X) + 2;
                    int yCount = (int)(Graphics.ScreenHeight / Graphics.FogTextures[fogIndex].Size.Y) + 2;

                    _fogCurrentX += (ecTime / 1000f) * Globals.GameMaps[MyMapNum].FogXSpeed * -6;
                    _fogCurrentY += (ecTime / 1000f) * Globals.GameMaps[MyMapNum].FogYSpeed * 2;
                    float deltaX = 0;///= _lastFogX - Graphics.CurrentView.Left - Graphics.FogOffsetX;
                    _fogCurrentX -= deltaX;
                    float deltaY = 0;// _lastFogY - Graphics.CurrentView.Top - Graphics.FogOffsetY;
                    _fogCurrentY -= deltaY;

                    if (_fogCurrentX < Graphics.FogTextures[fogIndex].Size.X) { _fogCurrentX += Graphics.FogTextures[fogIndex].Size.X; }
                    if (_fogCurrentX > Graphics.FogTextures[fogIndex].Size.X) { _fogCurrentX -= Graphics.FogTextures[fogIndex].Size.X; }
                    if (_fogCurrentY < Graphics.FogTextures[fogIndex].Size.Y) { _fogCurrentY += Graphics.FogTextures[fogIndex].Size.Y; }
                    if (_fogCurrentY > Graphics.FogTextures[fogIndex].Size.Y) { _fogCurrentY -= Graphics.FogTextures[fogIndex].Size.Y; }

                    for (int x = -1; x < xCount; x++)
                    {
                        for (int y = -1; y < yCount; y++)
                        {
                            var fogSprite = new Sprite(Graphics.FogTextures[fogIndex]) { Position = new Vector2f((GetX() + Globals.MapWidth * Globals.TileWidth / 2) - (xCount * Graphics.FogTextures[fogIndex].Size.X / 2) + x * Graphics.FogTextures[fogIndex].Size.X + _fogCurrentX, (GetY() + Globals.MapHeight * Globals.TileHeight / 2) - (yCount * Graphics.FogTextures[fogIndex].Size.Y / 2) + y * Graphics.FogTextures[fogIndex].Size.Y + _fogCurrentY) };
                            fogSprite.Color = new Color(255, 255, 255, (byte)(Globals.GameMaps[MyMapNum].FogTransaprency * _curFogIntensity));
                            Graphics.RenderWindow.Draw(fogSprite);
                        }
                    }
                }
            }
        }

        //Sound Functions
        private void CreateMapSounds()
        {
            ClearAttributeSounds();
            for (int x = 0; x < Globals.MapWidth; x++)
            {
                for (int y = 0; y < Globals.MapHeight; y++)
                {
                    if (Attributes[x, y].value == (int)Enums.MapAttributes.Sound)
                    {
                        if (Attributes[x, y].data4 != "None" && Attributes[x, y].data4 != "")
                        {
                            AttributeSounds.Add(Sounds.AddMapSound(Attributes[x, y].data4, x, y, MyMapNum, true, Attributes[x, y].data1));
                        }
                    }
                }
            }
        }
        private void ClearAttributeSounds()
        {
            for (int i = 0; i < AttributeSounds.Count; i++)
            {
                Sounds.StopSound(AttributeSounds[i]);
            }
            AttributeSounds.Clear();
        }

        //Dispose
        public void Dispose(bool prep = true)
        {
            if (prep)
            {
                Globals.MapsToRemove.Add(MyMapNum);
                return;
            }
            //Clean up all map stuff.
            for (int i = 0; i < 3; i++)
            {
                if (LowerTextures[i] != null)
                {
                    Graphics.ReleaseMapTexture(LowerTextures[i]);
                }
                if (UpperTextures[i] != null)
                {
                    Graphics.ReleaseMapTexture(UpperTextures[i]);
                }
                if (PeakTextures[i] != null)
                {
                    Graphics.ReleaseMapTexture(PeakTextures[i]);
                }
            }
            Globals.GameMaps.Remove(MyMapNum);
        }
    }

    public class Attribute
    {
        public int value;
        public int data1;
        public int data2;
        public int data3;
        public string data4 = "";
    }

    public class TileArray
    {
        public Tile[,] Tiles = new Tile[Globals.MapWidth, Globals.MapHeight];
    }

    public class Tile
    {
        public int TilesetIndex = -1;
        public int X;
        public int Y;
        public byte Autotile;
    }

    public class LightObj
    {
        public int OffsetX;
        public int OffsetY;
        public int TileX;
        public int TileY;
        public double Intensity;
        public int Range;
        public Bitmap Graphic;

        public LightObj(ByteBuffer myBuffer)
        {
            OffsetX = myBuffer.ReadInteger();
            OffsetY = myBuffer.ReadInteger();
            TileX = myBuffer.ReadInteger();
            TileY = myBuffer.ReadInteger();
            Intensity = myBuffer.ReadDouble();
            Range = myBuffer.ReadInteger();
        }
    }

    public class NpcSpawn
    {
        public int NpcNum;
        public int X;
        public int Y;
        public int Dir;
    }
}