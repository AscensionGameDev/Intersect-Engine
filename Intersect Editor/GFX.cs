using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML;
using SFML.Graphics;
using System.IO;
using SFML.Window;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Intersect_Editor
{
    public static class GFX
    {
        //Main render area - map window
        private static SFML.Graphics.RenderWindow renderwindow;
        //Tileset render widow
        private static SFML.Graphics.RenderWindow tilesetwindow;

        //Game Graphics
        static bool tilesetsLoaded;
        //Tileset Textures
        static Texture[] tilesetTex;

        //Basic Editor Textures
        private static Texture transH;
        private static Texture transV;
        //Texture for blocked tiles
        private static Texture blockedTex;
        //Texture for events
        private static Texture eventTex;
        //Single tile texture for light placement
        private static Texture lightTex;

        //Setup and Loading
        public static void InitSFML(frmMain myForm)
        {
            try
            {
                Stream s = new MemoryStream();

                renderwindow = new SFML.Graphics.RenderWindow(myForm.picMap.Handle); // creates our SFML RenderWindow on our surface control
                tilesetwindow = new SFML.Graphics.RenderWindow(myForm.picTileset.Handle);
                Intersect_Editor.Properties.Resources.transV.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                transV = new Texture(s);
                s.Dispose();
                s = new MemoryStream();
                Intersect_Editor.Properties.Resources.transH.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                transH = new Texture(s);
                s.Dispose();
                s = new MemoryStream();
                Intersect_Editor.Properties.Resources.jcb.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                blockedTex = new Texture(s);
                s.Dispose();
                s = new MemoryStream();
                Intersect_Editor.Properties.Resources.jce.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                eventTex = new Texture(s);
                s.Dispose();
                s = new MemoryStream();
                Intersect_Editor.Properties.Resources.jcl.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                lightTex = new Texture(s);
                s.Dispose();
                LoadGraphics(myForm);

            }
            catch (Exception)
            {

            }

        }
        private static void LoadGraphics(frmMain myForm)
        {
            LoadTilesets(myForm);
        }
        private static void LoadTilesets(frmMain myForm)
        {
            if (!Directory.Exists("Resources/Tilesets")) { Directory.CreateDirectory("Resources/Tilesets"); }
            string[] tilesets = Directory.GetFiles("Resources/Tilesets", "*.png");
            bool tilesetsUpdated = false;
            if (tilesets.Length > 0)
            {
                for (int i = 0; i < tilesets.Length; i++)
                {
                    tilesets[i] = tilesets[i].Replace("Resources/Tilesets\\", "");
                    if (Globals.tilesets != null)
                    {
                        if (Globals.tilesets.Length > 0)
                        {
                            for (int x = 0; x < Globals.tilesets.Length; x++)
                            {
                                if (Globals.tilesets[x] == tilesets[i])
                                {
                                    break;
                                }
                                else
                                {
                                    if (x == Globals.tilesets.Length - 1)
                                    {
                                        string[] newTilesets = new string[Globals.tilesets.Length + 1];
                                        Globals.tilesets.CopyTo(newTilesets, 0);
                                        newTilesets[Globals.tilesets.Length] = tilesets[i];
                                        Globals.tilesets = newTilesets;
                                        tilesetsUpdated = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            string[] newTilesets = new string[1];
                            newTilesets[0] = tilesets[i];
                            Globals.tilesets = newTilesets;
                            tilesetsUpdated = true;
                        }
                    }
                    else
                    {
                        string[] newTilesets = new string[1];
                        newTilesets[0] = tilesets[i];
                        Globals.tilesets = newTilesets;
                        tilesetsUpdated = true;
                    }
                }

                if (tilesetsUpdated)
                {
                    PacketSender.SendTilesets();
                }

                myForm.cmbTilesets.Items.Clear();
                for (int x = 0; x < Globals.tilesets.Length; x++)
                {
                    if (File.Exists("Resources/Tilesets/" + Globals.tilesets[x]))
                    {
                        myForm.cmbTilesets.Items.Add(Globals.tilesets[x]);
                    }
                    else
                    {
                        myForm.cmbTilesets.Items.Add(Globals.tilesets[x] + " - [MISSING]");
                    }
                }
                myForm.cmbTilesets.SelectedIndex = 0;
                Globals.currentTileset = 0;

                tilesetTex = new Texture[Globals.tilesets.Length];
                for (int i = 0; i < Globals.tilesets.Length; i++)
                {
                    if (File.Exists("Resources/Tilesets/" + Globals.tilesets[i]))
                    {
                        tilesetTex[i] = new Texture(new SFML.Graphics.Image("Resources/Tilesets/" + Globals.tilesets[i]));
                    }
                }
                tilesetsLoaded = true;
                InitTileset(0, myForm);
            }
        }

        //Rendering
        public static void Render()
        {
            renderwindow.DispatchEvents(); // handle SFML events - NOTE this is still required when SFML is hosted in another window
            renderwindow.Clear(SFML.Graphics.Color.Black); // clear our SFML RenderWindow
            tilesetwindow.DispatchEvents();
            tilesetwindow.Clear(SFML.Graphics.Color.Black);
            DrawTileset();

            //Draw Current Map
            DrawTransparentBorders();
            DrawMap();
            DrawMapUp();
            DrawMapDown();
            DrawMapLeft();
            DrawMapRight();
            DrawMapBorders();
            if (Globals.nightEnabled || Globals.currentLayer == Constants.LAYER_COUNT + 1) { DrawNight(); }
            renderwindow.Display(); // display what SFML has drawn to the screen
            tilesetwindow.Display();
        }
        private static void DrawTransparentBorders()
        {
            SFML.Graphics.Sprite tmpSprite;
            tmpSprite = new Sprite(transH);
            renderwindow.Draw(tmpSprite);
            tmpSprite.Position = new Vector2f(0, 1024 - 32);
            renderwindow.Draw(tmpSprite);
            tmpSprite = new Sprite(transV);
            tmpSprite.Position = new Vector2f(0, 0);
            renderwindow.Draw(tmpSprite);
            tmpSprite.Position = new Vector2f(1024 - 32, 0);
            renderwindow.Draw(tmpSprite);
        }
        private static void DrawMap()
        {
            SFML.Graphics.Sprite tmpSprite;
            Map tmpMap = Globals.GameMaps[Globals.currentMap];
            if (tmpMap == null) { return; }
            for (int x = 0; x < Constants.MAP_WIDTH; x++)
            {
                for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                {
                    for (int z = 0; z < Constants.LAYER_COUNT; z++)
                    {
                        if (tmpMap.Layers[z].Tiles[x, y].tilesetIndex > -1)
                        {
                            if (tilesetTex[tmpMap.Layers[z].Tiles[x, y].tilesetIndex] != null)
                            {
                                if (tmpMap.autotiles.Autotile[x, y].Layer[z].renderState == Constants.RENDER_STATE_NORMAL)
                                {
                                    tmpSprite = new Sprite(tilesetTex[tmpMap.Layers[z].Tiles[x, y].tilesetIndex]);
                                    tmpSprite.TextureRect = new IntRect(tmpMap.Layers[z].Tiles[x, y].x * 32, tmpMap.Layers[z].Tiles[x, y].y * 32, 32, 32);
                                    tmpSprite.Position = new Vector2f(x * 32 + 32, y * 32 + 32);
                                    renderwindow.Draw(tmpSprite);
                                }
                                else if (tmpMap.autotiles.Autotile[x, y].Layer[z].renderState == Constants.RENDER_STATE_AUTOTILE)
                                {
                                    DrawAutoTile(z, x * 32 + 32, y * 32 + 32, 1, x, y, tmpMap);
                                    DrawAutoTile(z, x * 32 + 16 + 32, y * 32 + 32, 2, x, y, tmpMap);
                                    DrawAutoTile(z, x * 32 + 32, y * 32 + 16 + 32, 3, x, y, tmpMap);
                                    DrawAutoTile(z, x * 32 + 16 + 32, y * 32 + 16 + 32, 4, x, y, tmpMap);
                                }
                            }
                        }
                    }
                }

            }
            RectangleShape line1;
            if (Globals.currentLayer == Constants.LAYER_COUNT)
            {
                //Draw Blocks
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        if (tmpMap.blocked[x, y] == 1)
                        {
                            tmpSprite = new Sprite(blockedTex);
                            tmpSprite.TextureRect = new IntRect(0, 0, 32, 32);
                            tmpSprite.Position = new Vector2f(x * 32 + 32, y * 32 + 32);
                            renderwindow.Draw(tmpSprite);
                        }
                    }

                }
                line1 = new RectangleShape(new Vector2f(32, 32));
                line1.Position = new Vector2f(Globals.curTileX * 32 + 32, Globals.curTileY * 32 + 32);
                line1.FillColor = SFML.Graphics.Color.White;
                line1.FillColor = SFML.Graphics.Color.Transparent;
                line1.OutlineColor = SFML.Graphics.Color.White;
                line1.OutlineThickness = 1;
                renderwindow.Draw(line1);
            }
            else if (Globals.currentLayer == Constants.LAYER_COUNT + 1)
            {

            }
            else if (Globals.currentLayer == Constants.LAYER_COUNT + 2)
            {
                //Draw Blocks
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        if (Globals.GameMaps[Globals.currentMap].FindEventAt(x, y) != null)
                        {
                            tmpSprite = new Sprite(eventTex);
                            tmpSprite.TextureRect = new IntRect(0, 0, 32, 32);
                            tmpSprite.Position = new Vector2f(x * 32 + 32, y * 32 + 32);
                            renderwindow.Draw(tmpSprite);
                        }
                    }

                }
                line1 = new RectangleShape(new Vector2f(32, 32));
                line1.Position = new Vector2f(Globals.curTileX * 32 + 32, Globals.curTileY * 32 + 32);
                line1.FillColor = SFML.Graphics.Color.White;
                line1.FillColor = SFML.Graphics.Color.Transparent;
                line1.OutlineColor = SFML.Graphics.Color.White;
                line1.OutlineThickness = 1;
                renderwindow.Draw(line1);
            }
            else
            {
                line1 = new RectangleShape(new Vector2f(32, 32));
                if (Globals.autotilemode == 0) { line1 = new RectangleShape(new Vector2f(32 + (Globals.curSelW * 32), 32 + (Globals.curSelH * 32))); }
                line1.Position = new Vector2f(Globals.curTileX * 32 + 32, Globals.curTileY * 32 + 32);
                line1.FillColor = SFML.Graphics.Color.White;
                line1.FillColor = SFML.Graphics.Color.Transparent;
                line1.OutlineColor = SFML.Graphics.Color.White;
                line1.OutlineThickness = 1;
                renderwindow.Draw(line1);
            }


        }
        private static void DrawAutoTile(int layerNum, int destX, int destY, int quarterNum, int x, int y, Map map)
        {
            int yOffset = 0, xOffset = 0;
            SFML.Graphics.Sprite tmpSprite;

            // calculate the offset
            switch (map.Layers[layerNum].Tiles[x, y].Autotile)
            {
                case Constants.AUTOTILE_WATERFALL:
                    yOffset = (Globals.waterfallFrame - 1) * 32;
                    break;
                case Constants.AUTOTILE_ANIM:
                    xOffset = Globals.autotileFrame * 64;
                    break;
                case Constants.AUTOTILE_CLIFF:
                    yOffset = -32;
                    break;
            }

            // Draw the quarter
            tmpSprite = new Sprite(tilesetTex[map.Layers[layerNum].Tiles[x, y].tilesetIndex]);
            tmpSprite.TextureRect = new IntRect((int)map.autotiles.Autotile[x, y].Layer[layerNum].srcX[quarterNum] + xOffset, (int)map.autotiles.Autotile[x, y].Layer[layerNum].srcY[quarterNum] + yOffset, 16, 16);
            tmpSprite.Position = new Vector2f(destX, destY);
            renderwindow.Draw(tmpSprite);
        }
        private static void DrawMapUp()
        {
            Map tmpMap = Globals.GameMaps[Globals.currentMap];
            SFML.Graphics.Sprite tmpSprite;
            if (tmpMap == null) { return; }
            if (tmpMap.up > -1)
            {
                tmpMap = Globals.GameMaps[tmpMap.up];
                if (tmpMap == null) { return; }
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = Constants.MAP_HEIGHT - 1; y < Constants.MAP_HEIGHT; y++)
                    {
                        for (int z = 0; z < Constants.LAYER_COUNT; z++)
                        {
                            if (tmpMap.Layers[z].Tiles[x, y].tilesetIndex > -1)
                            {
                                if (tilesetTex[tmpMap.Layers[z].Tiles[x, y].tilesetIndex] != null)
                                {
                                    if (tmpMap.autotiles.Autotile[x, y].Layer[z].renderState == Constants.RENDER_STATE_NORMAL)
                                    {
                                        tmpSprite = new Sprite(tilesetTex[tmpMap.Layers[z].Tiles[x, y].tilesetIndex]);
                                        tmpSprite.TextureRect = new IntRect(tmpMap.Layers[z].Tiles[x, y].x * 32, tmpMap.Layers[z].Tiles[x, y].y * 32, 32, 32);
                                        tmpSprite.Position = new Vector2f(x * 32 + 32, (y - Constants.MAP_HEIGHT + 1) * 32);
                                        renderwindow.Draw(tmpSprite);
                                    }
                                    else if (tmpMap.autotiles.Autotile[x, y].Layer[z].renderState == Constants.RENDER_STATE_AUTOTILE)
                                    {
                                        DrawAutoTile(z, x * 32 + 32, (y - Constants.MAP_HEIGHT + 1) * 32, 1, x, y, tmpMap);
                                        DrawAutoTile(z, x * 32 + 16 + 32, (y - Constants.MAP_HEIGHT + 1) * 32, 2, x, y, tmpMap);
                                        DrawAutoTile(z, x * 32 + 32, (y - Constants.MAP_HEIGHT + 1) * 32 + 16, 3, x, y, tmpMap);
                                        DrawAutoTile(z, x * 32 + 16 + 32, (y - Constants.MAP_HEIGHT + 1) * 32 + 16, 4, x, y, tmpMap);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        private static void DrawMapDown()
        {
            Map tmpMap = Globals.GameMaps[Globals.currentMap];
            SFML.Graphics.Sprite tmpSprite;
            if (tmpMap == null) { return; }
            if (tmpMap.down > -1)
            {
                tmpMap = Globals.GameMaps[tmpMap.down];
                if (tmpMap == null) { return; }
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < 1; y++)
                    {
                        for (int z = 0; z < Constants.LAYER_COUNT; z++)
                        {
                            if (tmpMap.Layers[z].Tiles[x, y].tilesetIndex > -1)
                            {
                                if (tilesetTex[tmpMap.Layers[z].Tiles[x, y].tilesetIndex] != null)
                                {
                                    if (tmpMap.autotiles.Autotile[x, y].Layer[z].renderState == Constants.RENDER_STATE_NORMAL)
                                    {
                                        tmpSprite = new Sprite(tilesetTex[tmpMap.Layers[z].Tiles[x, y].tilesetIndex]);
                                        tmpSprite.TextureRect = new IntRect(tmpMap.Layers[z].Tiles[x, y].x * 32, tmpMap.Layers[z].Tiles[x, y].y * 32, 32, 32);
                                        tmpSprite.Position = new Vector2f(x * 32 + 32, 32 + Constants.MAP_HEIGHT * 32);
                                        renderwindow.Draw(tmpSprite);
                                    }
                                    else if (tmpMap.autotiles.Autotile[x, y].Layer[z].renderState == Constants.RENDER_STATE_AUTOTILE)
                                    {
                                        DrawAutoTile(z, x * 32 + 32, 32 + Constants.MAP_HEIGHT * 32, 1, x, y, tmpMap);
                                        DrawAutoTile(z, x * 32 + 16 + 32, 32 + Constants.MAP_HEIGHT * 32, 2, x, y, tmpMap);
                                        DrawAutoTile(z, x * 32 + 32, 32 + Constants.MAP_HEIGHT * 32 + 16, 3, x, y, tmpMap);
                                        DrawAutoTile(z, x * 32 + 16 + 32, 32 + Constants.MAP_HEIGHT * 32 + 16, 4, x, y, tmpMap);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        private static void DrawMapLeft()
        {
            Map tmpMap = Globals.GameMaps[Globals.currentMap];
            SFML.Graphics.Sprite tmpSprite;
            if (tmpMap == null) { return; }
            if (tmpMap.left > -1)
            {
                tmpMap = Globals.GameMaps[tmpMap.left];
                if (tmpMap == null) { return; }
                for (int x = Constants.MAP_WIDTH - 1; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        for (int z = 0; z < Constants.LAYER_COUNT; z++)
                        {
                            if (tmpMap.Layers[z].Tiles[x, y].tilesetIndex > -1)
                            {
                                if (tilesetTex[tmpMap.Layers[z].Tiles[x, y].tilesetIndex] != null)
                                {
                                    if (tmpMap.autotiles.Autotile[x, y].Layer[z].renderState == Constants.RENDER_STATE_NORMAL)
                                    {
                                        tmpSprite = new Sprite(tilesetTex[tmpMap.Layers[z].Tiles[x, y].tilesetIndex]);
                                        tmpSprite.TextureRect = new IntRect(tmpMap.Layers[z].Tiles[x, y].x * 32, tmpMap.Layers[z].Tiles[x, y].y * 32, 32, 32);
                                        tmpSprite.Position = new Vector2f(0, y * 32 + 32);
                                        renderwindow.Draw(tmpSprite);
                                    }
                                    else if (tmpMap.autotiles.Autotile[x, y].Layer[z].renderState == Constants.RENDER_STATE_AUTOTILE)
                                    {
                                        DrawAutoTile(z, 0, y * 32 + 32, 1, x, y, tmpMap);
                                        DrawAutoTile(z, 0 + 16, y * 32 + 32, 2, x, y, tmpMap);
                                        DrawAutoTile(z, 0, y * 32 + 16 + 32, 3, x, y, tmpMap);
                                        DrawAutoTile(z, 0 + 16, y * 32 + 16 + 32, 4, x, y, tmpMap);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        private static void DrawMapRight()
        {
            Map tmpMap = Globals.GameMaps[Globals.currentMap];
            SFML.Graphics.Sprite tmpSprite;
            if (tmpMap == null) { return; }
            if (tmpMap.right > -1)
            {
                tmpMap = Globals.GameMaps[tmpMap.right];
                if (tmpMap == null) { return; }
                for (int x = 0; x < 1; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        for (int z = 0; z < Constants.LAYER_COUNT; z++)
                        {
                            if (tmpMap.Layers[z].Tiles[x, y].tilesetIndex > -1)
                            {
                                if (tilesetTex[tmpMap.Layers[z].Tiles[x, y].tilesetIndex] != null)
                                {
                                    if (tmpMap.autotiles.Autotile[x, y].Layer[z].renderState == Constants.RENDER_STATE_NORMAL)
                                    {
                                        tmpSprite = new Sprite(tilesetTex[tmpMap.Layers[z].Tiles[x, y].tilesetIndex]);
                                        tmpSprite.TextureRect = new IntRect(tmpMap.Layers[z].Tiles[x, y].x * 32, tmpMap.Layers[z].Tiles[x, y].y * 32, 32, 32);
                                        tmpSprite.Position = new Vector2f(32 + Constants.MAP_WIDTH * 32, 32 + y * 32);
                                        renderwindow.Draw(tmpSprite);
                                    }
                                    else if (tmpMap.autotiles.Autotile[x, y].Layer[z].renderState == Constants.RENDER_STATE_AUTOTILE)
                                    {
                                        DrawAutoTile(z, 32 + Constants.MAP_WIDTH * 32, 32 + y * 32, 1, x, y, tmpMap);
                                        DrawAutoTile(z, 32 + Constants.MAP_WIDTH * 32 + 16, 32 + y * 32, 2, x, y, tmpMap);
                                        DrawAutoTile(z, 32 + Constants.MAP_WIDTH * 32, 32 + y * 32 + 16, 3, x, y, tmpMap);
                                        DrawAutoTile(z, 32 + Constants.MAP_WIDTH * 32 + 16, 32 + y * 32 + 16, 4, x, y, tmpMap);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        private static void DrawTileset()
        {
            SFML.Graphics.Sprite tmpSprite;
            //Draw Current Tileset
            if (Globals.currentTileset > -1)
            {
                int selW;
                int selH;
                int selX;
                int selY;
                tmpSprite = new Sprite(tilesetTex[Globals.currentTileset]);
                tilesetwindow.Draw(tmpSprite);
                //renderwindow.
                selX = Globals.curSelX;
                selY = Globals.curSelY;
                selW = Globals.curSelW;
                selH = Globals.curSelH;
                if (selW < 0)
                {
                    selX -= Math.Abs(selW);
                    selW = Math.Abs(selW);
                }
                if (selH < 0)
                {
                    selY -= Math.Abs(selH);
                    selH = Math.Abs(selH);
                }
                RectangleShape line1 = new RectangleShape(new Vector2f(32 + (selW * 32), 32 + (selH * 32)));
                line1.Position = new Vector2f(selX * 32, selY * 32);
                line1.FillColor = SFML.Graphics.Color.White;
                line1.FillColor = SFML.Graphics.Color.Transparent;
                line1.OutlineColor = SFML.Graphics.Color.White;
                line1.OutlineThickness = 1;
                tilesetwindow.Draw(line1);
            }

        }
        private static void DrawMapBorders()
        {
            RectangleShape line1 = new RectangleShape(new Vector2f(1024, 1));
            line1.Position = new Vector2f(0, 32);
            line1.FillColor = SFML.Graphics.Color.White;
            renderwindow.Draw(line1);
            line1.Position = new Vector2f(0, 1024 - 32);
            renderwindow.Draw(line1);
            line1.Size = new Vector2f(1, 1024);
            line1.Position = new Vector2f(32, 0);
            renderwindow.Draw(line1);
            line1.Position = new Vector2f(1024 - 32, 0);
            renderwindow.Draw(line1);
            line1.Dispose();
        }
        private static void DrawNight()
        {
            if (Globals.lightsChanged) { GenerateLightTexture(); }
            Sprite tmpSprite;
            Shape line1;
            Sprite nightSprite = new Sprite(Globals.nightTex);
            nightSprite.Position = new Vector2f(-32 * 30, -32 * 30);
            nightSprite.Draw(renderwindow, RenderStates.Default);
            nightSprite.Dispose();
            if (Globals.currentLayer == Constants.LAYER_COUNT + 1)
            {
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        if (Globals.GameMaps[Globals.currentMap].FindLightAt(x, y) != null)
                        {
                            tmpSprite = new Sprite(lightTex);
                            tmpSprite.TextureRect = new IntRect(0, 0, 32, 32);
                            tmpSprite.Position = new Vector2f(x * 32 + 32, y * 32 + 32);
                            renderwindow.Draw(tmpSprite);
                        }
                    }

                }
                line1 = new RectangleShape(new Vector2f(32, 32));
                line1.Position = new Vector2f(Globals.curTileX * 32 + 32, Globals.curTileY * 32 + 32);
                line1.FillColor = SFML.Graphics.Color.White;
                line1.FillColor = SFML.Graphics.Color.Transparent;
                line1.OutlineColor = SFML.Graphics.Color.White;
                line1.OutlineThickness = 1;
                renderwindow.Draw(line1);
            }
        }

        //Lighting
        private static void GenerateLightTexture()
        {
            if (Globals.bnightColorArray == null)
            {
                Globals.bnightColorArray = new SFML.Graphics.Color[30 * 32 * 3, 30 * 32 * 3];
                for (int x = 0; x < 30 * 32 * 3; x++)
                {
                    for (int y = 0; y < 30 * 32 * 3; y++)
                    {
                        Globals.bnightColorArray[x, y] = new SFML.Graphics.Color(0, 0, 0, 180);
                    }
                }
            }
            Globals.nightColorArray = (SFML.Graphics.Color[,])Globals.bnightColorArray.Clone();
            double w = 1;

            for (int i = 0; i < Globals.GameMaps[Globals.currentMap].Lights.Count; i++)
            {
                w = CalcLightWidth(Globals.GameMaps[Globals.currentMap].Lights[i].range);
                int x = (30 * 32) + (Globals.GameMaps[Globals.currentMap].Lights[i].tileX * 32 + Globals.GameMaps[Globals.currentMap].Lights[i].offsetX) - (int)w / 2 + 32 + 16;
                int y = (int)(30 * 32) + (int)(Globals.GameMaps[Globals.currentMap].Lights[i].tileY * 32 + Globals.GameMaps[Globals.currentMap].Lights[i].offsetY) - (int)w / 2 + 32 + 16;
                AddLight(x, y, (int)w, Globals.GameMaps[Globals.currentMap].Lights[i].intensity, Globals.nightColorArray, Globals.GameMaps[Globals.currentMap].Lights[i]);
            }
            SFML.Graphics.Image img = new SFML.Graphics.Image(Globals.nightColorArray);
            Globals.nightTex = new Texture(img);
            Globals.lightsChanged = false;
        }
        private static int CalcLightWidth(int range)
        {
            int[] xVals = { 0, 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180 };
            int[] yVals = { 1, 8, 18, 34, 50, 72, 92, 114, 135, 162, 196, 230, 268, 320, 394, 500, 658, 976, 1234, 1600 };
            int w = 0;
            int x = 0;
            while (range >= xVals[x])
            {
                x++;
            }
            if (x > yVals.Length)
            {
                w = yVals[yVals.Length - 1];
            }
            else
            {
                w = yVals[x - 1];
                w += (int)((float)(range - xVals[x - 1]) / ((float)xVals[x] - xVals[x - 1])) * (yVals[x] - yVals[x - 1]);
            }
            return w;
        }
        private static void AddLight(int x1, int y1, int size, double intensity, SFML.Graphics.Color[,] colorArr, Light light)
        {
            Bitmap sp1;
            if (light.graphic == null)
            {
                sp1 = new Bitmap(size, size);
                Graphics g = Graphics.FromImage(sp1);
                GraphicsPath pth = new GraphicsPath();
                pth.AddEllipse(0, 0, size - 1, size - 1);
                PathGradientBrush pgb = new PathGradientBrush(pth);
                pgb.CenterColor = System.Drawing.Color.FromArgb((int)((double)(255 * intensity)), 0, 0, 0);
                pgb.SurroundColors = new System.Drawing.Color[] { System.Drawing.Color.Transparent };
                pgb.FocusScales = new PointF(0.8f, 0.8f);
                g.FillPath(pgb, pth);
                g.Dispose();
                light.graphic = sp1;
            }
            else
            {
                sp1 = light.graphic;
            }
            SFML.Graphics.Color emptyBlack;
            BitmapProcessing.FastBitmap FB1 = new BitmapProcessing.FastBitmap(sp1);
            FB1.LockImage();
            for (int x = x1; x < x1 + size; x++)
            {
                for (int y = y1; y < y1 + size; y++)
                {
                    if (y >= 0 && y < 30 * 32 * 3 && x >= 0 && x < 30 * 32 * 3)
                    {
                        emptyBlack = Globals.nightColorArray[y, x];
                        System.Drawing.Color tmpPixel = FB1.GetPixel(x - x1, y - y1);
                        int a = emptyBlack.A - tmpPixel.A;
                        int b = emptyBlack.R + tmpPixel.R;
                        int c = emptyBlack.G + tmpPixel.G;
                        int d = emptyBlack.B + tmpPixel.B;
                        if (a > 255) { a = 255; }
                        if (a < 0) { a = 0; }
                        if (b > 255) { b = 255; }
                        if (c > 255) { c = 255; }
                        if (d > 255) { d = 255; }
                        Globals.nightColorArray[y, x] = new SFML.Graphics.Color((byte)b, (byte)c, (byte)d, (byte)a);
                    }
                }
            }
            FB1.UnlockImage();
        }

        //Extra
        public static void InitTileset(int index,frmMain myForm)
        {
            if (!tilesetsLoaded) { return; }
            Globals.currentTileset = index;
            Globals.curSelX = 0;
            Globals.curSelY = 0;
            myForm.picTileset.Width = (int)tilesetTex[index].Size.X;
            myForm.picTileset.Height = (int)tilesetTex[index].Size.Y;
            tilesetwindow.SetView(new SFML.Graphics.View(new Vector2f(myForm.picTileset.Width / 2, myForm.picTileset.Height / 2), new Vector2f(myForm.picTileset.Width, myForm.picTileset.Height)));
        }
    }
}
