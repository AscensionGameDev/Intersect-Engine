using System;
using System.Collections.Generic;
using System.Drawing;
using SFML.Graphics;
using Color = SFML.Graphics.Color;

namespace Intersect_Client.Classes
{
    public class Map
    {
        public TileArray[] Layers = new TileArray[Constants.LayerCount];
        public Attribute[,] Attributes = new Attribute[Constants.MapWidth, Constants.MapHeight];
        public int MyMapNum;
        public string MyName = "New Map";
        public int Up = -1;
        public int Down = -1;
        public int Left = -1;
        public int Right = -1;
        public int Revision;
        public string Bgm;
        public bool MapLoaded;

        //public Texture2D[] layerCache = new Texture2D[2];
        //public Sprite[,] mySprite = new Sprite[3, 2];

        public bool MapRendered;
        public bool MapRendering = false;
        public byte[] MyPacket;
        public MapAutotiles Autotiles;
        public bool CacheCleared = true;
        public List<LightObj> Lights = new List<LightObj>();
        public bool IsIndoors;

        public RenderTexture[] LowerTextures = new RenderTexture[3];
        public RenderTexture[] UpperTextures = new RenderTexture[3];

        public Map(int mapNum, byte[] mapPacket)
        {
            MyMapNum = mapNum;
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                Layers[i] = new TileArray();
                for (var x = 0; x < Constants.MapWidth; x++)
                {
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y] = new Tile();
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

        private void Load()
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(MyPacket);
            MyName = bf.ReadString();
            Up = bf.ReadInteger();
            Down = bf.ReadInteger();
            Left = bf.ReadInteger();
            Right = bf.ReadInteger();
            Bgm = bf.ReadString();
            IsIndoors = Convert.ToBoolean(bf.ReadInteger());
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                for (var x = 0; x < Constants.MapWidth; x++)
                {
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y].TilesetIndex = bf.ReadInteger();
                        Layers[i].Tiles[x, y].X = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Y = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                    }
                }
            }
            for (var x = 0; x < Constants.MapWidth; x++)
            {
                for (var y = 0; y < Constants.MapHeight; y++)
                {
                    Attributes[x, y].value = bf.ReadInteger();
                    Attributes[x, y].data1 = bf.ReadInteger();
                    Attributes[x, y].data2 = bf.ReadInteger();
                    Attributes[x, y].data3 = bf.ReadInteger();
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
            MapRendered = false;

            //Globals.mapRevision[myMapNum] = revision;
            //Database.SaveMapRevisions();
        }

        public void ClearCache()
        {
            //Map is no longer in bounds or in use, lets clear the stuff we simply do not need.
            if (CacheCleared == false) { return; }
            /*for (int z = 0; z < 3; z++)
            {
                for (int i = 0; i < 1; i++)
                {
                    if (layerCache[i] != null)
                    {
                        try
                        {
                            Texture2D.Destroy(layerCache[i]);
                        }
                        catch (Exception)
                        {
                        }
                        layerCache[i] = null;
                    }
                    if (mySprite[z, i] != null)
                    {
                        try
                        {
                            Sprite.Destroy(mySprite[z, i]);
                        }
                        catch (Exception)
                        {
                        }
                        mySprite[z, i] = null;
                    }
                }
            }*/
            CacheCleared = true;
        }

        private void DrawAutoTile(int layerNum, int destX, int destY, int quarterNum, int x, int y, int forceFrame, RenderTexture tex)
        {
            int yOffset = 0, xOffset = 0;

            // calculate the offset
            switch (Layers[layerNum].Tiles[x, y].Autotile)
            {
                case Constants.AutotileWaterfall:
                    yOffset = (forceFrame - 1) * 32;
                    break;

                case Constants.AutotileAnim:
                    xOffset = forceFrame * 64;
                    break;

                case Constants.AutotileCliff:
                    yOffset = -32;
                    break;
            }
            Graphics.RenderTexture(Graphics.Tilesets[Layers[layerNum].Tiles[x, y].TilesetIndex], destX, destY,
                (int)Autotiles.Autotile[x, y].Layer[layerNum].SrcX[quarterNum] + xOffset,
                (int)Autotiles.Autotile[x, y].Layer[layerNum].SrcY[quarterNum] + yOffset, 16, 16, tex);
        }

        private void PreRenderMap()
        {
            for (var i = 0; i < 3; i++)
            {
                if (LowerTextures[i] != null) { LowerTextures[i].Dispose(); }
                LowerTextures[i] = new RenderTexture(32 * Constants.MapWidth, 32 * Constants.MapHeight);
                LowerTextures[i].Clear(Color.Transparent);
                if (UpperTextures[i] != null) { UpperTextures[i].Dispose(); }
                UpperTextures[i] = new RenderTexture(32 * Constants.MapWidth, 32 * Constants.MapHeight);
                for (var l = 0; l < Constants.LayerCount; l++)
                {
                    if (l < 3)
                    {
                        DrawMapLayer(LowerTextures[i], l, i);
                    }
                    else
                    {
                        DrawMapLayer(UpperTextures[i], l, i);
                    }
                }
                LowerTextures[i].Display();
                UpperTextures[i].Display();
            }
            MapRendered = true;
            Graphics.LightsChanged = true;
        }

        private void DrawMapLayer(RenderTexture tex, int l, int z)
        {
            for (var x = 0; x < Constants.MapWidth; x++)
            {
                for (var y = 0; y < Constants.MapHeight; y++)
                {
                    if (Globals.Tilesets == null) continue;
                    if (Layers[l].Tiles[x, y].TilesetIndex < 0) continue;
                    if (Layers[l].Tiles[x, y].TilesetIndex >= Globals.Tilesets.Length) continue;
                    try
                    {
                        switch (Autotiles.Autotile[x, y].Layer[l].RenderState)
                        {
                            case Constants.RenderStateNormal:
                                Graphics.RenderTexture(Graphics.Tilesets[Layers[l].Tiles[x, y].TilesetIndex], x * 32, y * 32, Layers[l].Tiles[x, y].X * 32, Layers[l].Tiles[x, y].Y * 32,32,32,tex);
                                break;
                            case Constants.RenderStateAutotile:
                                DrawAutoTile(l, x * 32, y * 32, 1, x, y, z, tex);
                                DrawAutoTile(l, x * 32 + 16, y * 32, 2, x, y, z, tex);
                                DrawAutoTile(l, x * 32, y * 32 + 16, 3, x, y, z, tex);
                                DrawAutoTile(l, +x * 32 + 16, y * 32 + 16, 4, x, y, z, tex);
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

        public void Draw(int xoffset, int yoffset, bool upper = false)
        {
            if (!MapRendered) { PreRenderMap(); }
            if (!upper)
            {
                Graphics.RenderTexture(LowerTextures[Globals.AnimFrame].Texture,xoffset,yoffset,Graphics.RenderWindow);
            }
            else
            {
                Graphics.RenderTexture(UpperTextures[Globals.AnimFrame].Texture, xoffset, yoffset, Graphics.RenderWindow);
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
    }

    public class Attribute
    {
        public int value;
        public int data1;
        public int data2;
        public int data3;
    }

    public class TileArray
    {
        public Tile[,] Tiles = new Tile[Constants.MapWidth, Constants.MapHeight];
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
}