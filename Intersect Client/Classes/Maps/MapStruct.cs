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
using System.Drawing;
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
            //cacheThread = new Thread (Load);
            //cacheThread.Start (mapPacket);
            MyPacket = mapPacket;
            Load();
            MapLoaded = true;

            //CacheMap1 ();
            //CacheMapLayers();
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
            CreateMapSounds();
            MapRendered = false;
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
        private void PreRenderMap()
        {
            for (var i = 0; i < 3; i++)
            {
                if (LowerTextures[i] != null) { LowerTextures[i].Dispose(); }
                LowerTextures[i] = new RenderTexture((uint)Globals.TileWidth * (uint)Globals.MapWidth, (uint)Globals.TileHeight * (uint)Globals.MapHeight);
                LowerTextures[i].Clear(Color.Transparent);
                if (UpperTextures[i] != null) { UpperTextures[i].Dispose(); }
                UpperTextures[i] = new RenderTexture((uint)Globals.TileWidth * (uint)Globals.MapWidth, (uint)Globals.TileHeight * (uint)Globals.MapHeight);
                if (PeakTextures[i] != null) { PeakTextures[i].Dispose(); }
                PeakTextures[i] = new RenderTexture((uint)Globals.TileWidth * (uint)Globals.MapWidth, (uint)Globals.TileHeight * (uint)Globals.MapHeight);
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
                LowerTextures[i].Display();
                UpperTextures[i].Display();
                PeakTextures[i].Display();
            }
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
                                Graphics.RenderTexture(Graphics.Tilesets[Layers[l].Tiles[x, y].TilesetIndex], x * Globals.TileWidth + xoffset, y * Globals.TileHeight + yoffset, Layers[l].Tiles[x, y].X * Globals.TileWidth, Layers[l].Tiles[x, y].Y * Globals.TileHeight, Globals.TileWidth, Globals.TileHeight, tex);
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
        public void Update()
        {
            if (BackgroundSound == null && Sound != "None" && Sound !="")
            {
                BackgroundSound = Sounds.AddMapSound(Sound, -1, -1, MyMapNum, true, 10);
            }
        }
        public void Draw(float xoffset, float yoffset, int layer = 0)
        {
            if (!MapRendered) { PreRenderMap(); }
            if (layer == 0)
            {
                Graphics.RenderTexture(LowerTextures[Globals.AnimFrame].Texture,xoffset,yoffset,Graphics.RenderWindow);
                
                //Draw Map Items
                for (int i = 0; i < MapItems.Count; i++)
                {
                    if (Graphics.ItemFileNames.IndexOf(Globals.GameItems[MapItems[i].ItemNum].Pic) > -1)
                    {
                        Graphics.RenderTexture(Graphics.ItemTextures[Graphics.ItemFileNames.IndexOf(Globals.GameItems[MapItems[i].ItemNum].Pic)], xoffset + MapItems[i].X * Globals.TileWidth, yoffset + MapItems[i].Y * Globals.TileHeight, Graphics.RenderWindow);
                    }
                }
            }
            else if (layer == 1)
            {
                Graphics.RenderTexture(UpperTextures[Globals.AnimFrame].Texture, xoffset, yoffset, Graphics.RenderWindow);
            }
            else
            {
                Graphics.RenderTexture(PeakTextures[Globals.AnimFrame].Texture, xoffset, yoffset, Graphics.RenderWindow);
                DrawFog();
            }

        }

        public void DrawNew(float xoffset, float yoffset, int layer = 0)
        {
            //if (!MapRendered) { PreRenderMap(); }
            //return;
            if (layer == 0)
            {
                for (var l = 0; l < 3; l++)
                {
                        DrawMapLayer(Graphics.RenderWindow, l, Globals.AnimFrame,xoffset,yoffset);
                }

                //Draw Map Items
                for (int i = 0; i < MapItems.Count; i++)
                {
                    if (Graphics.ItemFileNames.IndexOf(Globals.GameItems[MapItems[i].ItemNum].Pic) > -1)
                    {
                        Graphics.RenderTexture(Graphics.ItemTextures[Graphics.ItemFileNames.IndexOf(Globals.GameItems[MapItems[i].ItemNum].Pic)], xoffset + MapItems[i].X * Globals.TileWidth, yoffset + MapItems[i].Y * Globals.TileHeight, Graphics.RenderWindow);
                    }
                }
            }
            else if (layer == 1)
            {
                for (var l = 3; l <= 3; l++)
                {
                    DrawMapLayer(Graphics.RenderWindow, l, Globals.AnimFrame, xoffset, yoffset);
                }
            }
            else
            {
                for (var l = 4; l < Constants.LayerCount; l++)
                {
                    DrawMapLayer(Graphics.RenderWindow, l, Globals.AnimFrame, xoffset, yoffset);
                }
                DrawFog();
            }

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
                    int xCount = (int)(Graphics.RenderWindow.Size.X / Graphics.FogTextures[fogIndex].Size.X) + 1;
                    int yCount = (int)(Graphics.RenderWindow.Size.Y / Graphics.FogTextures[fogIndex].Size.Y) + 1;

                    _fogCurrentX += (ecTime / 1000f) * Globals.GameMaps[MyMapNum].FogXSpeed * -6;
                    _fogCurrentY += (ecTime / 1000f) * Globals.GameMaps[MyMapNum].FogYSpeed * 2;
                    float deltaX = _lastFogX - Graphics.CalcMapOffsetX(0) - Graphics.FogOffsetX;
                    _fogCurrentX -= deltaX;
                    float deltaY = _lastFogY - Graphics.CalcMapOffsetY(0) - Graphics.FogOffsetY;
                    _fogCurrentY -= deltaY;

                    if (_fogCurrentX < Graphics.FogTextures[fogIndex].Size.X) { _fogCurrentX += Graphics.FogTextures[fogIndex].Size.X; }
                    if (_fogCurrentX > Graphics.FogTextures[fogIndex].Size.X) { _fogCurrentX -= Graphics.FogTextures[fogIndex].Size.X; }
                    if (_fogCurrentY < Graphics.FogTextures[fogIndex].Size.Y) { _fogCurrentY += Graphics.FogTextures[fogIndex].Size.Y; }
                    if (_fogCurrentY > Graphics.FogTextures[fogIndex].Size.Y) { _fogCurrentY -= Graphics.FogTextures[fogIndex].Size.Y; }

                    for (int x = -1; x < xCount; x++)
                    {
                        for (int y = -1; y < yCount; y++)
                        {
                            var fogSprite = new Sprite(Graphics.FogTextures[fogIndex]) { Position = new Vector2f(x * Graphics.FogTextures[fogIndex].Size.X + _fogCurrentX, y * Graphics.FogTextures[fogIndex].Size.Y + _fogCurrentY) };
                            fogSprite.Color = new Color(255, 255, 255, (byte)(Globals.GameMaps[MyMapNum].FogTransaprency * _curFogIntensity));
                            Graphics.RenderWindow.Draw(fogSprite);
                        }
                    }
                    _lastFogX = Graphics.CalcMapOffsetX(0);
                    _lastFogY = Graphics.CalcMapOffsetY(0);
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
        public void Dispose()
        {
            //Clean up all map stuff.
            for (int i = 0; i < 3; i++)
            {
                if (LowerTextures[i] != null) { LowerTextures[i].Dispose(); }
                if (UpperTextures[i] != null) { UpperTextures[i].Dispose(); }
                if (PeakTextures[i] != null) { PeakTextures[i].Dispose(); }
            }
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