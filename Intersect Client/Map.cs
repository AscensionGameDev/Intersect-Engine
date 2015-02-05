using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace Intersect_Client
{
    public class Map
    {
        private const int layerCount = 5;
        public TileArray[] Layers = new TileArray[Constants.LAYER_COUNT];
        public int[,] blocked = new int[Constants.MAP_WIDTH, Constants.MAP_HEIGHT];
        public int myMapNum = 0;
        public string myName = "New Map";
        public int up = -1;
        public int down = -1;
        public int left = -1;
        public int right = -1;
        public int revision;
        public string BGM;
        public bool mapLoaded = false;

        //public Texture2D[] layerCache = new Texture2D[2];
        //public Sprite[,] mySprite = new Sprite[3, 2];

        public bool mapRendered = false;
        public bool mapRendering = false;
        public byte[] myPacket;
        public MapAutotiles autotiles;
        public bool cacheCleared = true;
        public List<LightObj> Lights = new List<LightObj>();
        public bool isIndoors;

        public RenderTexture[] lowerTextures = new RenderTexture[3];
        public RenderTexture[] upperTextures = new RenderTexture[3];

        public Map(int mapNum, byte[] mapPacket)
        {
            myMapNum = mapNum;
            for (int i = 0; i < Constants.LAYER_COUNT; i++)
            {
                Layers[i] = new TileArray();
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        Layers[i].Tiles[x, y] = new Tile();
                    }
                }
            }
            //cacheThread = new Thread (Load);
            //cacheThread.Start (mapPacket);
            myPacket = mapPacket;
            Load();
            mapLoaded = true;

            //CacheMap1 ();
            //CacheMapLayers();
        }

        private void Load()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(myPacket);
            myName = bf.ReadString();
            up = bf.ReadInteger();
            down = bf.ReadInteger();
            left = bf.ReadInteger();
            right = bf.ReadInteger();
            BGM = bf.ReadString();
            isIndoors = Convert.ToBoolean(bf.ReadInteger());
            for (int i = 0; i < Constants.LAYER_COUNT; i++)
            {
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        Layers[i].Tiles[x, y].tilesetIndex = bf.ReadInteger();
                        Layers[i].Tiles[x, y].x = bf.ReadInteger();
                        Layers[i].Tiles[x, y].y = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                    }
                }
            }
            for (int x = 0; x < Constants.MAP_WIDTH; x++)
            {
                for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                {
                    blocked[x, y] = bf.ReadInteger();
                }
            }
            int lCount = bf.ReadInteger();
            for (int i = 0; i < lCount; i++)
            {
                Lights.Add(new LightObj(bf));
            }
            revision = bf.ReadInteger();
            bf.ReadLong();
            mapLoaded = true;
            Globals.shouldUpdateLights = true;
            autotiles = new MapAutotiles(this);
            autotiles.initAutotiles();
            mapRendered = false;

            //Globals.mapRevision[myMapNum] = revision;
            //Database.SaveMapRevisions();
        }

        public void ClearCache()
        {
            //Map is no longer in bounds or in use, lets clear the stuff we simply do not need.
            if (cacheCleared == false) { return; }
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
            cacheCleared = true;
        }

        private void DrawAutoTile(int layerNum, int destX, int destY, int quarterNum, int x, int y, int forceFrame, RenderTexture tex)
        {
            int yOffset = 0, xOffset = 0;
            Sprite tmpSprite;

            // calculate the offset
            switch (Layers[layerNum].Tiles[x, y].Autotile)
            {
                case Constants.AUTOTILE_WATERFALL:
                    yOffset = (forceFrame - 1) * 32;
                    break;

                case Constants.AUTOTILE_ANIM:
                    xOffset = forceFrame * 64;
                    break;

                case Constants.AUTOTILE_CLIFF:
                    yOffset = -32;
                    break;
            }

            tmpSprite = new Sprite(Graphics.tilesets[Layers[layerNum].Tiles[x, y].tilesetIndex]);
            tmpSprite.TextureRect = new IntRect((int)autotiles.Autotile[x, y].Layer[layerNum].srcX[quarterNum] + xOffset, (int)autotiles.Autotile[x, y].Layer[layerNum].srcY[quarterNum] + yOffset, 16, 16);
            tmpSprite.Position = new SFML.Window.Vector2f(destX, destY);
            tex.Draw(tmpSprite);
        }

        private void PreRenderMap()
        {
            for (int i = 0; i < 3; i++)
            {
                if (lowerTextures[i] != null) { lowerTextures[i].Dispose(); }
                lowerTextures[i] = new RenderTexture(32 * Constants.MAP_WIDTH, 32 * Constants.MAP_HEIGHT);
                if (upperTextures[i] != null) { upperTextures[i].Dispose(); }
                upperTextures[i] = new RenderTexture(32 * Constants.MAP_WIDTH, 32 * Constants.MAP_HEIGHT);

                for (int l = 0; l < Constants.LAYER_COUNT; l++)
                {
                    if (l < 3)
                    {
                        DrawMapLayer(lowerTextures[i], l, i);
                    }
                    else
                    {
                        DrawMapLayer(upperTextures[i], l, i);
                    }
                }
                lowerTextures[i].Display();
                upperTextures[i].Display();
            }
            mapRendered = true;
            Graphics.lightsChanged = true;
        }

        private void DrawMapLayer(RenderTexture tex, int l, int z)
        {
            Sprite tmpSprite;
            for (int x = 0; x < Constants.MAP_WIDTH; x++)
            {
                for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                {
                    if (Globals.tilesets != null)
                    {
                        if (true)
                        {
                            if (Layers[l].Tiles[x, y].tilesetIndex >= 0)
                            {
                                if (Layers[l].Tiles[x, y].tilesetIndex < Globals.tilesets.Length)
                                {
                                    try
                                    {
                                        if (autotiles.Autotile[x, y].Layer[l].renderState == Constants.RENDER_STATE_NORMAL)
                                        {
                                            tmpSprite = new Sprite(Graphics.tilesets[Layers[l].Tiles[x, y].tilesetIndex]);
                                            tmpSprite.TextureRect = new IntRect(Layers[l].Tiles[x, y].x * 32, Layers[l].Tiles[x, y].y * 32, 32, 32);
                                            tmpSprite.Position = new SFML.Window.Vector2f(x * 32, y * 32);
                                            tex.Draw(tmpSprite);
                                        }
                                        else if (autotiles.Autotile[x, y].Layer[l].renderState == Constants.RENDER_STATE_AUTOTILE)
                                        {
                                            DrawAutoTile(l, x * 32, y * 32, 1, x, y, z, tex);
                                            DrawAutoTile(l, x * 32 + 16, y * 32, 2, x, y, z, tex);
                                            DrawAutoTile(l, x * 32, y * 32 + 16, 3, x, y, z, tex);
                                            DrawAutoTile(l, +x * 32 + 16, y * 32 + 16, 4, x, y, z, tex);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Draw(int xoffset, int yoffset, bool upper = false)
        {
            if (!mapRendered) { PreRenderMap(); }
            Sprite tmpSprite;
            if (!upper)
            {
                tmpSprite = new Sprite(lowerTextures[Globals.animFrame].Texture);
                tmpSprite.Position = new SFML.Window.Vector2f(xoffset, yoffset);
                Graphics.renderWindow.Draw(tmpSprite);
            }
            else
            {
                tmpSprite = new Sprite(upperTextures[Globals.animFrame].Texture);
                tmpSprite.Position = new SFML.Window.Vector2f(xoffset, yoffset);
                Graphics.renderWindow.Draw(tmpSprite);
            }
        }

        public bool shouldLoad(int index)
        {
            return true;
            if (Globals.myIndex > -1)
            {
                if (Globals.entities.Count > Globals.myIndex)
                {
                    switch (index)
                    {
                        case 0:
                            if (Globals.entities[Globals.myIndex].currentX < 18 && Globals.entities[Globals.myIndex].currentY < 18) { return true; }
                            break;

                        case 1:
                            if (Globals.entities[Globals.myIndex].currentY < 18) { return true; }
                            break;

                        case 2:
                            if (Globals.entities[Globals.myIndex].currentX > 11 && Globals.entities[Globals.myIndex].currentY < 18) { return true; }
                            break;

                        case 3:
                            if (Globals.entities[Globals.myIndex].currentX < 18) { return true; }
                            break;

                        case 4:
                            return true;

                        case 5:
                            if (Globals.entities[Globals.myIndex].currentX > 11) { return true; }
                            break;

                        case 6:
                            if (Globals.entities[Globals.myIndex].currentX < 18 && Globals.entities[Globals.myIndex].currentY > 11) { return true; }
                            break;

                        case 7:
                            if (Globals.entities[Globals.myIndex].currentY > 11) { return true; }
                            break;

                        case 8:
                            if (Globals.entities[Globals.myIndex].currentX > 11 && Globals.entities[Globals.myIndex].currentY > 11) { return true; }
                            break;
                    }
                }
            }
            return false;
        }
    }

    public class TileArray
    {
        public Tile[,] Tiles = new Tile[Constants.MAP_WIDTH, Constants.MAP_HEIGHT];
    }

    public class Tile
    {
        public int tilesetIndex = -1;
        public int x;
        public int y;
        public byte Autotile;
    }

    public class LightObj
    {
        public int offsetX = 0;
        public int offsetY = 0;
        public int tileX;
        public int tileY;
        public double intensity = 1;
        public int range = 20;
        public System.Drawing.Bitmap graphic;

        public LightObj(ByteBuffer myBuffer)
        {
            offsetX = myBuffer.ReadInteger();
            offsetY = myBuffer.ReadInteger();
            tileX = myBuffer.ReadInteger();
            tileY = myBuffer.ReadInteger();
            intensity = myBuffer.ReadDouble();
            range = myBuffer.ReadInteger();
        }
    }
}