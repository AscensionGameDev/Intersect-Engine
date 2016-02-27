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
using System.Threading;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using Intersect_Client.Classes.Core;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Items;
using Intersect_Client.Classes.Misc;

namespace Intersect_Client.Classes.Maps
{
    public class MapStruct
    {
        //Core
        public const string Version = "0.0.0.1";
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
        public Attribute[,] Attributes = new Attribute[Globals.Database.MapWidth, Globals.Database.MapHeight];
        public List<Light> Lights = new List<Light>();

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
        public GameRenderTexture[] LowerTextures = new GameRenderTexture[3];
        public GameRenderTexture[] UpperTextures = new GameRenderTexture[3];
        public GameRenderTexture[] PeakTextures = new GameRenderTexture[3];
        public List<MapItemInstance> MapItems = new List<MapItemInstance>();
        public MapSound BackgroundSound;
        public List<MapSound> AttributeSounds = new List<MapSound>();
        private long _fogUpdateTime = Globals.System.GetTimeMS();
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
                for (var x = 0; x < Globals.Database.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.Database.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y] = new Tile();
                        if (i == 0) { Attributes[x, y] = new Attribute(); }
                    }
                }
            }
            MyPacket = mapPacket;
            Load();
            MapLoaded = true;

        }

        //Load
        private void Load()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(MyPacket);
            if (bf.ReadInteger() == 1)
            {
                //Deleted
                throw new Exception("Server sent a deleted or corrupted map!");
            }
            else
            {
                string loadedVersion = bf.ReadString();
                if (loadedVersion != Version)
                    throw new Exception("Failed to load Map #" + MyMapNum + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);
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
                FogTransaprency = bf.ReadInteger();
                RHue = bf.ReadInteger();
                GHue = bf.ReadInteger();
                BHue = bf.ReadInteger();
                AHue = bf.ReadInteger();
                Brightness = bf.ReadInteger();

                for (var i = 0; i < Constants.LayerCount; i++)
                {
                    for (var x = 0; x < Globals.Database.MapWidth; x++)
                    {
                        for (var y = 0; y < Globals.Database.MapHeight; y++)
                        {
                            Layers[i].Tiles[x, y].TilesetIndex = bf.ReadInteger();
                            Layers[i].Tiles[x, y].X = bf.ReadInteger();
                            Layers[i].Tiles[x, y].Y = bf.ReadInteger();
                            Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                        }
                    }
                }
                for (var x = 0; x < Globals.Database.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.Database.MapHeight; y++)
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
                MapLoaded = true;
                Globals.ShouldUpdateLights = true;
                Autotiles = new MapAutotiles(this);
                Autotiles.InitAutotiles();
                UpdateAdjacentAutotiles();
                CreateMapSounds();
                MapRendered = false;
            }
        }

        private void UpdateAdjacentAutotiles()
        {
            if (Up > -1 && Globals.GameMaps.ContainsKey(Up))
            {
                for (int x = 0; x < Globals.Database.MapWidth; x++)
                {
                    for (int y = Globals.Database.MapHeight - 1; y < Globals.Database.MapHeight; y++)
                    {
                        Globals.GameMaps[Up].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
            if (Down > -1 && Globals.GameMaps.ContainsKey(Down))
            {
                for (int x = 0; x < Globals.Database.MapWidth; x++)
                {
                    for (int y = 0; y < 1; y++)
                    {
                        Globals.GameMaps[Down].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
            if (Left > -1 && Globals.GameMaps.ContainsKey(Left))
            {
                for (int x = Globals.Database.MapWidth - 1; x < Globals.Database.MapWidth; x++)
                {
                    for (int y = 0; y < Globals.Database.MapHeight; y++)
                    {
                        Globals.GameMaps[Left].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
            if (Right > -1 && Globals.GameMaps.ContainsKey(Right))
            {
                for (int x = 0; x < 1; x++)
                {
                    for (int y = 0; y < Globals.Database.MapHeight; y++)
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
            GameGraphics.PreRenderedMapLayer = true;
            if (_preRenderStage < 3)
            {
                int i = _preRenderStage;
                if (LowerTextures[i] == null)
                {
                    while (!GameGraphics.GetMapTexture(ref LowerTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                if (UpperTextures[i] == null)
                {
                    while (!GameGraphics.GetMapTexture(ref UpperTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                if (PeakTextures[i] == null)
                {
                    while (!GameGraphics.GetMapTexture(ref PeakTextures[i]))
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
                    LowerTextures[i].Begin();
                    DrawMapLayer(LowerTextures[i], l, i);
                    LowerTextures[i].End();
                    return;
                }
                else if (l == 3)
                {
                    UpperTextures[i].Begin();
                    DrawMapLayer(UpperTextures[i], l, i);
                    UpperTextures[i].End();
                    return;
                }
                else
                {
                    PeakTextures[i].Begin();
                    DrawMapLayer(PeakTextures[i], l, i);
                    PeakTextures[i].End();
                    _preRenderStage++;
                    _preRenderLayer = 0;
                    return;
                }
            }

            MapRendered = true;
        }
        public void PreRenderMap1()
        {
            long ectime = Globals.System.GetTimeMS();
            long creationTime = 0;
            long renderingTime = 0;
            long displayTime = 0;
            for (var i = 0; i < 3; i++)
            {
                ectime = Globals.System.GetTimeMS();
                if (LowerTextures[i] == null)
                {
                    while (!GameGraphics.GetMapTexture(ref LowerTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                if (UpperTextures[i] == null)
                {
                    while (!GameGraphics.GetMapTexture(ref UpperTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                if (PeakTextures[i] == null)
                {
                    while (!GameGraphics.GetMapTexture(ref PeakTextures[i]))
                    {
                        Thread.Sleep(10);
                    }
                }
                creationTime += Globals.System.GetTimeMS() - ectime;
                LowerTextures[i].Begin();
                UpperTextures[i].Begin();
                PeakTextures[i].Begin();
                LowerTextures[i].Clear(Color.Transparent);
                UpperTextures[i].Clear(Color.Transparent);
                PeakTextures[i].Clear(Color.Transparent);

                ectime = Globals.System.GetTimeMS();
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
                renderingTime += Globals.System.GetTimeMS() - ectime;
                ectime = Globals.System.GetTimeMS();
                LowerTextures[i].End();
                UpperTextures[i].End();
                PeakTextures[i].End();
                displayTime += Globals.System.GetTimeMS() - ectime;
                ectime = Globals.System.GetTimeMS();
            }
            MapRendered = true;
        }
        private void DrawAutoTile(int layerNum, float destX, float destY, int quarterNum, int x, int y, int forceFrame, GameRenderTexture tex)
        {
            int yOffset = 0, xOffset = 0;

            // calculate the offset
            switch (Layers[layerNum].Tiles[x, y].Autotile)
            {
                case Constants.AutotileWaterfall:
                    yOffset = (forceFrame - 1) * Globals.Database.TileHeight;
                    break;

                case Constants.AutotileAnim:
                    xOffset = forceFrame * 64;
                    break;

                case Constants.AutotileCliff:
                    yOffset = -Globals.Database.TileHeight;
                    break;
            }
            GameGraphics.DrawGameTexture(GameGraphics.Tilesets[Layers[layerNum].Tiles[x, y].TilesetIndex], destX, destY,
                (int)Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X + xOffset,
                (int)Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y + yOffset, 16, 16, tex);
        }
        private void DrawMapLayer(GameRenderTexture tex, int l, int z, float xoffset = 0, float yoffset = 0)
        {
            int xmin = 0;
            int xmax = Globals.Database.MapWidth;
            int ymin = 0;
            int ymax = Globals.Database.MapHeight;
            if (!Globals.Database.RenderCaching)
            {
                if (GetX() < GameGraphics.CurrentView.Left)
                {
                    xmin += (int)(Math.Floor((GameGraphics.CurrentView.Left - GetX()) / 32));
                }
                if (GetX() + (xmax * 32) > GameGraphics.CurrentView.Left + GameGraphics.CurrentView.Width)
                {
                    xmax -= (int)(Math.Floor(((GetX() + (xmax * 32)) - (GameGraphics.CurrentView.Left + GameGraphics.CurrentView.Width)) / 32));
                }
                if (GetY() < GameGraphics.CurrentView.Top)
                {
                    ymin += (int)(Math.Floor((GameGraphics.CurrentView.Top - GetY()) / 32));
                }
                if (GetY() + (ymax * 32) > GameGraphics.CurrentView.Top + GameGraphics.CurrentView.Height)
                {
                    ymax -= (int)(Math.Floor(((GetY() + (ymax * 32)) - (GameGraphics.CurrentView.Top + GameGraphics.CurrentView.Height)) / 32));
                }
                xmin -= 2;
                if (xmin < 0) xmin = 0;
                xmax += 2;
                if (xmax > Globals.Database.MapWidth) xmax = Globals.Database.MapWidth;
                ymin -= 2;
                if (ymin < 0) ymin = 0;
                ymax += 2;
                if (ymax > Globals.Database.MapHeight) ymax = Globals.Database.MapHeight;
            }
            for (var x = xmin; x < xmax; x++)
            {
                for (var y = ymin; y < ymax; y++)
                {
                    if (Globals.Tilesets == null) continue;
                    if (Layers[l].Tiles[x, y].TilesetIndex < 0) continue;
                    if (Layers[l].Tiles[x, y].TilesetIndex >= Globals.Tilesets.Length) continue;
                    try
                    {
                        switch (Autotiles.Autotile[x, y].Layer[l].RenderState)
                        {
                            case Constants.RenderStateNormal:
                                GameGraphics.DrawGameTexture(GameGraphics.Tilesets[Layers[l].Tiles[x, y].TilesetIndex], x * Globals.Database.TileWidth + xoffset, y * Globals.Database.TileHeight + yoffset, Layers[l].Tiles[x, y].X * Globals.Database.TileWidth, Layers[l].Tiles[x, y].Y * Globals.Database.TileHeight, Globals.Database.TileWidth, Globals.Database.TileHeight, tex);
                                break;
                            case Constants.RenderStateAutotile:
                                DrawAutoTile(l, x * Globals.Database.TileWidth + xoffset, y * Globals.Database.TileHeight + yoffset, 1, x, y, z, tex);
                                DrawAutoTile(l, x * Globals.Database.TileWidth + 16 + xoffset, y * Globals.Database.TileHeight + yoffset, 2, x, y, z, tex);
                                DrawAutoTile(l, x * Globals.Database.TileWidth + xoffset, y * Globals.Database.TileHeight + 16 + yoffset, 3, x, y, z, tex);
                                DrawAutoTile(l, +x * Globals.Database.TileWidth + 16 + xoffset, y * Globals.Database.TileHeight + 16 + yoffset, 4, x, y, z, tex);
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
                _lastUpdateTime = Globals.System.GetTimeMS() + 10000;
                if (BackgroundSound == null && Sound != "None" && Sound != "")
                {
                    BackgroundSound = GameAudio.AddMapSound(Sound, -1, -1, MyMapNum, true, 10);
                }
            }
            else
            {
                if (Globals.System.GetTimeMS() > _lastUpdateTime || GameGraphics.FreeMapTextures.Count < 27)
                {
                    Dispose();
                }
            }
        }
        public void Draw(int layer = 0)
        {
            if (!MapRendered)
            {
                if (Globals.Database.RenderCaching) return;
                switch (layer)
                {
                    case 0:
                        for (int i = 0; i < 3; i++)
                        {
                            DrawMapLayer(null, i, Globals.AnimFrame, GetX(), GetY());
                        }
                        break;

                    case 1:
                        DrawMapLayer(null, 3, Globals.AnimFrame, GetX(), GetY());
                        break;

                    case 2:
                        DrawMapLayer(null, 4, Globals.AnimFrame, GetX(), GetY());
                        break;
                }

            }
            else
            {
                if (layer == 0)
                {
                    GameGraphics.DrawGameTexture(LowerTextures[Globals.AnimFrame], GetX(), GetY());
                }
                else if (layer == 1)
                {
                    GameGraphics.DrawGameTexture(UpperTextures[Globals.AnimFrame], GetX(), GetY());
                }
                else
                {
                    GameGraphics.DrawGameTexture(PeakTextures[Globals.AnimFrame], GetX(), GetY());
                }
            }
            if (layer == 0)
            {
                //Draw Map Items
                for (int i = 0; i < MapItems.Count; i++)
                {
                    if (GameGraphics.ItemFileNames.IndexOf(Globals.GameItems[MapItems[i].ItemNum].Pic) > -1)
                    {
                        GameGraphics.DrawGameTexture(GameGraphics.ItemTextures[GameGraphics.ItemFileNames.IndexOf(Globals.GameItems[MapItems[i].ItemNum].Pic)], GetX() + MapItems[i].X * Globals.Database.TileWidth, GetY() + MapItems[i].Y * Globals.Database.TileHeight);
                    }
                }

                //Add lights to our darkness texture
                foreach (var light in Lights)
                {
                    double w = light.Size;
                    var x = GetX() + (light.TileX * Globals.Database.TileWidth + light.OffsetX) - (int)w / 2 + 16;
                    var y = GetY() + (light.TileY * Globals.Database.TileHeight + light.OffsetY) - (int)w / 2 + 16;
                    GameGraphics.DrawLight((int)x, (int)y, (int)w, light.Intensity, light.Expand, light.Color);
                }
            }
            if (layer == 2)
            {
                DrawFog();
            }

        }
        public float GetX()
        {
            return MapGridX * Globals.Database.MapWidth * Globals.Database.TileWidth;
        }
        public float GetY()
        {
            return MapGridY * Globals.Database.MapHeight * Globals.Database.TileHeight;
        }

        //Fogs
        private void DrawFog()
        {
            float ecTime = Globals.System.GetTimeMS() - _fogUpdateTime;
            _fogUpdateTime = Globals.System.GetTimeMS();
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
                if (GameGraphics.FogFileNames.IndexOf(Globals.GameMaps[MyMapNum].Fog) > -1)
                {
                    int fogIndex = GameGraphics.FogFileNames.IndexOf(Globals.GameMaps[MyMapNum].Fog);
                    int xCount = (int)(GameGraphics.Renderer.GetScreenWidth() / GameGraphics.FogTextures[fogIndex].GetWidth()) + 2;
                    int yCount = (int)(GameGraphics.Renderer.GetScreenHeight() / GameGraphics.FogTextures[fogIndex].GetHeight()) + 2;

                    _fogCurrentX += (ecTime / 1000f) * Globals.GameMaps[MyMapNum].FogXSpeed * -6;
                    _fogCurrentY += (ecTime / 1000f) * Globals.GameMaps[MyMapNum].FogYSpeed * 2;
                    float deltaX = 0;///= _lastFogX - Graphics.CurrentView.Left - Graphics.FogOffsetX;
                    _fogCurrentX -= deltaX;
                    float deltaY = 0;// _lastFogY - Graphics.CurrentView.Top - Graphics.FogOffsetY;
                    _fogCurrentY -= deltaY;

                    if (_fogCurrentX < GameGraphics.FogTextures[fogIndex].GetWidth()) { _fogCurrentX += GameGraphics.FogTextures[fogIndex].GetWidth(); }
                    if (_fogCurrentX > GameGraphics.FogTextures[fogIndex].GetWidth()) { _fogCurrentX -= GameGraphics.FogTextures[fogIndex].GetWidth(); }
                    if (_fogCurrentY < GameGraphics.FogTextures[fogIndex].GetHeight()) { _fogCurrentY += GameGraphics.FogTextures[fogIndex].GetHeight(); }
                    if (_fogCurrentY > GameGraphics.FogTextures[fogIndex].GetHeight()) { _fogCurrentY -= GameGraphics.FogTextures[fogIndex].GetHeight(); }

                    for (int x = -1; x < xCount; x++)
                    {
                        for (int y = -1; y < yCount; y++)
                        {
                            int fogW = GameGraphics.FogTextures[fogIndex].GetWidth();
                            int fogH = GameGraphics.FogTextures[fogIndex].GetHeight();
                            GameGraphics.DrawGameTexture(GameGraphics.FogTextures[fogIndex],
                                new FloatRect(0, 0, fogW, fogH),
                                new FloatRect(
                                    (GetX() + Globals.Database.MapWidth*Globals.Database.TileWidth/2) - (xCount*fogW/2) + x*fogW +
                                    _fogCurrentX,
                                    (GetY() + Globals.Database.MapHeight*Globals.Database.TileHeight/2) - (yCount*fogH/2) + y*fogH +
                                    _fogCurrentY, fogW, fogH),
                                new Color((byte)(Globals.GameMaps[MyMapNum].FogTransaprency * _curFogIntensity),255, 255, 255
                                    ));
                        }
                    }
                }
            }
        }

        //Sound Functions
        private void CreateMapSounds()
        {
            ClearAttributeSounds();
            for (int x = 0; x < Globals.Database.MapWidth; x++)
            {
                for (int y = 0; y < Globals.Database.MapHeight; y++)
                {
                    if (Attributes[x, y].value == (int)Enums.MapAttributes.Sound)
                    {
                        if (Attributes[x, y].data4 != "None" && Attributes[x, y].data4 != "")
                        {
                            AttributeSounds.Add(GameAudio.AddMapSound(Attributes[x, y].data4, x, y, MyMapNum, true, Attributes[x, y].data1));
                        }
                    }
                }
            }
        }
        private void ClearAttributeSounds()
        {
            Globals.System.Log("Clearing Attribute Sounds");
            for (int i = 0; i < AttributeSounds.Count; i++)
            {
                GameAudio.StopSound(AttributeSounds[i]);
            }
            AttributeSounds.Clear();
        }

        //Dispose
        public void Dispose(bool prep = true, bool killentities = true)
        {
            if (prep)
            {
                Globals.MapsToRemove.Add(MyMapNum);
                return;
            }
            if (Globals.Database.RenderCaching)
            {
                //Clean up all map stuff.
                for (int i = 0; i < 3; i++)
                {
                    if (LowerTextures[i] != null)
                    {
                        GameGraphics.ReleaseMapTexture(LowerTextures[i]);
                    }
                    if (UpperTextures[i] != null)
                    {
                        GameGraphics.ReleaseMapTexture(UpperTextures[i]);
                    }
                    if (PeakTextures[i] != null)
                    {
                        GameGraphics.ReleaseMapTexture(PeakTextures[i]);
                    }
                }
            }
            if (killentities)
            {
                foreach (var en in Globals.Entities)
                {
                    if (en.Value.CurrentMap == MyMapNum)
                    {
                        Globals.EntitiesToDispose.Add(en.Key);
                    }
                }
                foreach (var en in Globals.LocalEntities)
                {
                    if (en.Value.CurrentMap == MyMapNum)
                    {
                        Globals.LocalEntitiesToDispose.Add(en.Key);
                    }
                }
            }
            MapRendered = false;
            MapLoaded = false;
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
        public Tile[,] Tiles = new Tile[Globals.Database.MapWidth, Globals.Database.MapHeight];
    }

    public class Tile
    {
        public int TilesetIndex = -1;
        public int X;
        public int Y;
        public byte Autotile;
    }

    public class Light
    {
        public int OffsetX;
        public int OffsetY;
        public int TileX;
        public int TileY;
        public byte Intensity = 255;
        public int Size = 20;
        public float Expand = 0f;
        public Color Color = Color.White;

        public Light(ByteBuffer myBuffer)
        {
            OffsetX = myBuffer.ReadInteger();
            OffsetY = myBuffer.ReadInteger();
            TileX = myBuffer.ReadInteger();
            TileY = myBuffer.ReadInteger();
            Intensity = myBuffer.ReadByte();
            Size = myBuffer.ReadInteger();
            Expand = (float)myBuffer.ReadDouble();
            Color = new Color(myBuffer.ReadByte(), myBuffer.ReadByte(), myBuffer.ReadByte());
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