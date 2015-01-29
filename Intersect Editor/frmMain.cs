using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFML.Graphics;
using SFML.Window;
using SFML;
using System.IO;
using System.Drawing.Drawing2D;


namespace IntersectEditor
{
    public partial class frmMain : Form
    {
        SFML.Graphics.RenderWindow renderwindow;
        SFML.Graphics.RenderWindow tilesetwindow;
        Texture transH;
        Texture transV;
        Texture blockedTex;
        Texture eventTex;
        Texture nightTex;
        Texture lightTex;
        int currentTileset = -1;
        public int currentMap;
        int curSelX;
        int curSelY;
        int curTileX;
        int curTileY;
        int curSelW;
        int curSelH;
        int mouseX;
        int mouseY;
        int currentLayer = 0;
        int mouseButton = -1;
        bool tMouseDown;
        bool tilesetsLoaded;
        int autotilemode = 0;
        int waterfallFrame = 0;
        int autotileFrame = 0;
        bool nightEnabled = false;
        public bool lightsChanged = true;
        SFML.Graphics.Color[,] nightColorArray;// = new SFML.Graphics.Color[bmp.Height, bmp.Width];
        SFML.Graphics.Color[,] bnightColorArray;
        Texture[] tilesetTex;
        Light backupLight;
        Light editingLight;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            long animationTimer = Environment.TickCount;
            long waterfallTimer = Environment.TickCount;
            EnterMap(0);
            InitSFML();
            InitTilesets();
            this.Show();
            UpdateScrollBars();
            InitLayerMenu();
            cmbAutotile.SelectedIndex = 0;
            GlobalVariables.inEditor = true;
            if (GlobalVariables.GameMaps[currentMap].up > -1) { PacketSender.SendNeedMap(GlobalVariables.GameMaps[currentMap].up); }
            if (GlobalVariables.GameMaps[currentMap].down > -1) { PacketSender.SendNeedMap(GlobalVariables.GameMaps[currentMap].down); }
            if (GlobalVariables.GameMaps[currentMap].left > -1) { PacketSender.SendNeedMap(GlobalVariables.GameMaps[currentMap].left); }
            if (GlobalVariables.GameMaps[currentMap].right > -1) { PacketSender.SendNeedMap(GlobalVariables.GameMaps[currentMap].right); }
            // drawing loop
            while (this.Visible) // loop while the window is open
            {
                if (GlobalVariables.GameMaps[currentMap] != null)
                {
                    if (this.Text != "Intersect Editor - Map# " + currentMap + " " + GlobalVariables.GameMaps[currentMap].myName + " Revision: " + GlobalVariables.GameMaps[currentMap].revision + " CurX: " + curTileX + " CurY: " + curTileY)
                    {
                        this.Text = "Intersect Editor - Map# " + currentMap + " " + GlobalVariables.GameMaps[currentMap].myName + " Revision: " + GlobalVariables.GameMaps[currentMap].revision + " CurX: " + curTileX + " CurY: " + curTileY;
                    }
                }
                if (waterfallTimer < Environment.TickCount)
                {
                    waterfallFrame++;
                    if (waterfallFrame == 3) { waterfallFrame = 0; }
                    waterfallTimer = Environment.TickCount + 500;
                }
                if (animationTimer < Environment.TickCount)
                {
                    autotileFrame++;
                    if (autotileFrame == 3) { autotileFrame = 0; }
                    animationTimer = Environment.TickCount + 600;
                }
                System.Windows.Forms.Application.DoEvents(); // handle form events
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
                if (nightEnabled || currentLayer == Constants.LAYER_COUNT + 1) { DrawNight(); }
                renderwindow.Display(); // display what SFML has drawn to the screen
                tilesetwindow.Display();
                System.Threading.Thread.Sleep(10);
            }
        }


        private void InitLayerMenu()
        {
            ToolStripItem tmpTsi;
            for (int i = 0; i < Constants.LAYER_COUNT + 3; i++)
            {
                tmpTsi = layerMenu.DropDownItems.Add("Layer " + (i + 1));
                tmpTsi.Click += new EventHandler(HandleLayerClick);
                tmpTsi.Tag = i;
                if (i == Constants.LAYER_COUNT)
                {
                    tmpTsi.Text = "Blocks";
                }
                if (i == Constants.LAYER_COUNT + 1)
                {
                    tmpTsi.Text = "Lights";
                }
                if (i == Constants.LAYER_COUNT + 2)
                {
                    tmpTsi.Text = "Events";
                }
            }
        }

        private void HandleLayerClick(object sender, EventArgs e)
        {
            ToolStripItem tmpTsi = (ToolStripItem)sender;
            currentLayer = (int)tmpTsi.Tag;
            tmpTsi.Select();
            if (currentLayer == Constants.LAYER_COUNT)
            {
                lblCurLayer.Text = "Layer: Blocks";
            }
            else
            {
                lblCurLayer.Text = "Layer #" + (int)tmpTsi.Tag;
            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalVariables.GameSocket.SendPacket(File.ReadAllBytes("c:\\tmp\\lastpacket.dat"));
        }

        private void InitSFML()
        {
            try
            {
                Stream s = new MemoryStream();

                renderwindow = new SFML.Graphics.RenderWindow(picMap.Handle); // creates our SFML RenderWindow on our surface control
                tilesetwindow = new SFML.Graphics.RenderWindow(picTileset.Handle);
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

            }
            catch (Exception)
            {

            }

        }

        private void DrawNight()
        {
            if (lightsChanged) { GenerateLightTexture(); }
            Sprite tmpSprite;
            Shape line1;
            Sprite nightSprite = new Sprite(nightTex);
            nightSprite.Position = new Vector2f(-32 * 30, -32 * 30);
            nightSprite.Draw(renderwindow, RenderStates.Default);
            nightSprite.Dispose();
            if (currentLayer == Constants.LAYER_COUNT + 1)
            {
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        if (GlobalVariables.GameMaps[currentMap].FindLightAt(x, y) != null)
                        {
                            tmpSprite = new Sprite(lightTex);
                            tmpSprite.TextureRect = new IntRect(0, 0, 32, 32);
                            tmpSprite.Position = new Vector2f(x * 32 + 32, y * 32 + 32);
                            renderwindow.Draw(tmpSprite);
                        }
                    }

                }
                line1 = new RectangleShape(new Vector2f(32, 32));
                line1.Position = new Vector2f(curTileX * 32 + 32, curTileY * 32 + 32);
                line1.FillColor = SFML.Graphics.Color.White;
                line1.FillColor = SFML.Graphics.Color.Transparent;
                line1.OutlineColor = SFML.Graphics.Color.White;
                line1.OutlineThickness = 1;
                renderwindow.Draw(line1);
            }
        }

        private void GenerateLightTexture()
        {
            if (bnightColorArray == null)
            {
                bnightColorArray = new SFML.Graphics.Color[30 * 32 * 3, 30 * 32 * 3];
                for (int x = 0; x < 30 * 32 * 3; x++)
                {
                    for (int y = 0; y < 30 * 32 * 3; y++)
                    {
                        bnightColorArray[x, y] = new SFML.Graphics.Color(0, 0, 0,  180);
                    }
                }
            }
            nightColorArray = (SFML.Graphics.Color[,])bnightColorArray.Clone();
            double w = 1;

            for (int i = 0; i < GlobalVariables.GameMaps[currentMap].Lights.Count; i++)
            {
                w = CalcLightWidth(GlobalVariables.GameMaps[currentMap].Lights[i].range);
                int x = (30 * 32) + (GlobalVariables.GameMaps[currentMap].Lights[i].tileX * 32 + GlobalVariables.GameMaps[currentMap].Lights[i].offsetX) - (int)w / 2 + 32 + 16;
                int y = (int)(30 * 32) + (int)(GlobalVariables.GameMaps[currentMap].Lights[i].tileY * 32 + GlobalVariables.GameMaps[currentMap].Lights[i].offsetY) - (int)w / 2 + 32 + 16;
                AddLight(x, y, (int)w, GlobalVariables.GameMaps[currentMap].Lights[i].intensity, nightColorArray, GlobalVariables.GameMaps[currentMap].Lights[i]);
            }
            SFML.Graphics.Image img = new SFML.Graphics.Image(nightColorArray);
            nightTex = new Texture(img);
            lightsChanged = false;
        }

        private int CalcLightWidth(int range)
        {
            int[] xVals = {0,5,10,20,30,40,50,60,70,80,90,100,110,120,130,140,150,160,170,180};
            int[] yVals = { 1, 8, 18, 34, 50, 72, 92, 114, 135, 162, 196, 230, 268, 320, 394, 500, 658, 976, 1234 ,1600};
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
                w += (int)((float)(range - xVals[x - 1]) / ((float)xVals[x] -xVals[x - 1])) * (yVals[x] - yVals[x - 1]);
            }
            return w;
        }

        private void AddLight(int x1, int y1, int size, double intensity, SFML.Graphics.Color[,] colorArr, Light light)
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
                        emptyBlack = nightColorArray[y, x];
                        System.Drawing.Color tmpPixel = FB1.GetPixel(x - x1,y - y1);
                        int a = emptyBlack.A - tmpPixel.A;
                        int b = emptyBlack.R + tmpPixel.R;
                        int c = emptyBlack.G + tmpPixel.G;
                        int d = emptyBlack.B + tmpPixel.B;
                        if (a > 255) { a = 255; }
                        if (a < 0) { a = 0; }
                        if (b > 255) { b = 255; }
                        if (c > 255) { c = 255; }
                        if (d > 255) { d = 255; }
                        nightColorArray[y, x] = new SFML.Graphics.Color((byte)b, (byte)c, (byte)d, (byte)a);
                    }
                }
            }
            FB1.UnlockImage();
        }

        private void InitTilesets()
        {
            if (!Directory.Exists("Resources/Tilesets")) { Directory.CreateDirectory("Resources/Tilesets"); }
            string[] tilesets = Directory.GetFiles("Resources/Tilesets", "*.png");
            bool tilesetsUpdated = false;
            if (tilesets.Length > 0)
            {
                for (int i = 0; i < tilesets.Length; i++)
                {
                    tilesets[i] = tilesets[i].Replace("Resources/Tilesets\\", "");
                    if (GlobalVariables.tilesets != null)
                    {
                        if (GlobalVariables.tilesets.Length > 0)
                        {
                            for (int x = 0; x < GlobalVariables.tilesets.Length; x++)
                            {
                                if (GlobalVariables.tilesets[x] == tilesets[i])
                                {
                                    break;
                                }
                                else
                                {
                                    if (x == GlobalVariables.tilesets.Length - 1)
                                    {
                                        string[] newTilesets = new string[GlobalVariables.tilesets.Length + 1];
                                        GlobalVariables.tilesets.CopyTo(newTilesets, 0);
                                        newTilesets[GlobalVariables.tilesets.Length] = tilesets[i];
                                        GlobalVariables.tilesets = newTilesets;
                                        tilesetsUpdated = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            string[] newTilesets = new string[1];
                            newTilesets[0] = tilesets[i];
                            GlobalVariables.tilesets = newTilesets;
                            tilesetsUpdated = true;
                        }
                    }
                    else
                    {
                        string[] newTilesets = new string[1];
                        newTilesets[0] = tilesets[i];
                        GlobalVariables.tilesets = newTilesets;
                        tilesetsUpdated = true;
                    }
                }

                if (tilesetsUpdated)
                {
                    PacketSender.SendTilesets();
                }

                cmbTilesets.Items.Clear();
                for (int x = 0; x < GlobalVariables.tilesets.Length; x++)
                {
                    if (File.Exists("Resources/Tilesets/" + GlobalVariables.tilesets[x]))
                    {
                        cmbTilesets.Items.Add(GlobalVariables.tilesets[x]);
                    }
                    else
                    {
                        cmbTilesets.Items.Add(GlobalVariables.tilesets[x] + " - [MISSING]");
                    }
                }
                cmbTilesets.SelectedIndex = 0;
                currentTileset = 0;

                tilesetTex = new Texture[GlobalVariables.tilesets.Length];
                for (int i = 0; i < GlobalVariables.tilesets.Length; i++)
                {
                    if (File.Exists("Resources/Tilesets/" + GlobalVariables.tilesets[i]))
                    {
                        tilesetTex[i] = new Texture(new SFML.Graphics.Image("Resources/Tilesets/" + GlobalVariables.tilesets[i]));
                    }
                }
                tilesetsLoaded = true;
                InitTileset(0);
            }
        }

        private void InitTileset(int index)
        {
            if (!tilesetsLoaded) { return; }
            currentTileset = index;
            curSelX = 0;
            curSelY = 0;
            picTileset.Width = (int)tilesetTex[index].Size.X;
            picTileset.Height = (int)tilesetTex[index].Size.Y;
            tilesetwindow.SetView(new SFML.Graphics.View(new Vector2f(picTileset.Width / 2, picTileset.Height / 2), new Vector2f(picTileset.Width, picTileset.Height)));
        }

        private void UpdateScrollBars()
        {
            vScrollMap.Minimum = 0;
            vScrollMap.Maximum = 1;
            vScrollMap.Value = 0;
            hScrollMap.Minimum = 0;
            hScrollMap.Maximum = 1;
            hScrollMap.Value = 0;
            vScrollTileset.Minimum = 0;
            vScrollTileset.Maximum = 1;
            vScrollTileset.Value = 0;
            hScrollTileset.Minimum = 0;
            hScrollTileset.Maximum = 1;
            hScrollTileset.Value = 0;
            picMap.Left = 0;
            picMap.Top = 0;
            picTileset.Left = 0;
            picTileset.Top = 0;
            if (picMap.Height > grpMap.Height)
            {
                vScrollMap.Enabled = true;
                vScrollMap.Maximum = picMap.Height - grpMap.Height;
            }
            else
            {
                vScrollMap.Enabled = false;
            }
            if (picMap.Width > grpMap.Width)
            {
                hScrollMap.Enabled = true;
                hScrollMap.Maximum = picMap.Width - grpMap.Width;
            }
            else
            {
                hScrollMap.Enabled = false;
            }
            if (picTileset.Width > grpTileset.Width)
            {
                hScrollTileset.Enabled = true;
                hScrollTileset.Maximum = picTileset.Width - grpTileset.Width;
            }
            else
            {
                hScrollTileset.Enabled = false;
            }
            if (picTileset.Height > grpTileset.Height)
            {
                vScrollTileset.Enabled = true;
                vScrollTileset.Maximum = picTileset.Height - grpTileset.Height;
            }
            else
            {
                vScrollTileset.Enabled = false;
            }
        }

        private void DrawTransparentBorders()
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

        private void DrawMap()
        {
            SFML.Graphics.Sprite tmpSprite;
            Map tmpMap = GlobalVariables.GameMaps[currentMap];
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
            if (currentLayer == Constants.LAYER_COUNT)
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
                line1.Position = new Vector2f(curTileX * 32 + 32, curTileY * 32 + 32);
                line1.FillColor = SFML.Graphics.Color.White;
                line1.FillColor = SFML.Graphics.Color.Transparent;
                line1.OutlineColor = SFML.Graphics.Color.White;
                line1.OutlineThickness = 1;
                renderwindow.Draw(line1);
            }
            else if (currentLayer == Constants.LAYER_COUNT + 1)
            {

            }
            else if (currentLayer == Constants.LAYER_COUNT + 2)
            {
                //Draw Blocks
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        if (GlobalVariables.GameMaps[currentMap].FindEventAt(x, y) != null)
                        {
                            tmpSprite = new Sprite(eventTex);
                            tmpSprite.TextureRect = new IntRect(0, 0, 32, 32);
                            tmpSprite.Position = new Vector2f(x * 32 + 32, y * 32 + 32);
                            renderwindow.Draw(tmpSprite);
                        }
                    }

                }
                line1 = new RectangleShape(new Vector2f(32, 32));
                line1.Position = new Vector2f(curTileX * 32 + 32, curTileY * 32 + 32);
                line1.FillColor = SFML.Graphics.Color.White;
                line1.FillColor = SFML.Graphics.Color.Transparent;
                line1.OutlineColor = SFML.Graphics.Color.White;
                line1.OutlineThickness = 1;
                renderwindow.Draw(line1);
            }
            else
            {
                line1 = new RectangleShape(new Vector2f(32, 32));
                if (autotilemode == 0) { line1 = new RectangleShape(new Vector2f(32 + (curSelW * 32), 32 + (curSelH * 32))); }
                line1.Position = new Vector2f(curTileX * 32 + 32, curTileY * 32 + 32);
                line1.FillColor = SFML.Graphics.Color.White;
                line1.FillColor = SFML.Graphics.Color.Transparent;
                line1.OutlineColor = SFML.Graphics.Color.White;
                line1.OutlineThickness = 1;
                renderwindow.Draw(line1);
            }


        }

        private void DrawAutoTile(int layerNum, int destX, int destY, int quarterNum, int x, int y, Map map)
        {
            int yOffset = 0, xOffset = 0;
            SFML.Graphics.Sprite tmpSprite;

            // calculate the offset
            switch (map.Layers[layerNum].Tiles[x, y].Autotile)
            {
                case Constants.AUTOTILE_WATERFALL:
                    yOffset = (waterfallFrame - 1) * 32;
                    break;
                case Constants.AUTOTILE_ANIM:
                    xOffset = autotileFrame * 64;
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

        private void DrawMapUp()
        {
            Map tmpMap = GlobalVariables.GameMaps[currentMap];
            SFML.Graphics.Sprite tmpSprite;
            if (tmpMap == null) { return; }
            if (tmpMap.up > -1)
            {
                tmpMap = GlobalVariables.GameMaps[tmpMap.up];
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

        private void DrawMapDown()
        {
            Map tmpMap = GlobalVariables.GameMaps[currentMap];
            SFML.Graphics.Sprite tmpSprite;
            if (tmpMap == null) { return; }
            if (tmpMap.down > -1)
            {
                tmpMap = GlobalVariables.GameMaps[tmpMap.down];
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

        private void DrawMapLeft()
        {
            Map tmpMap = GlobalVariables.GameMaps[currentMap];
            SFML.Graphics.Sprite tmpSprite;
            if (tmpMap == null) { return; }
            if (tmpMap.left > -1)
            {
                tmpMap = GlobalVariables.GameMaps[tmpMap.left];
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

        private void DrawMapRight()
        {
            Map tmpMap = GlobalVariables.GameMaps[currentMap];
            SFML.Graphics.Sprite tmpSprite;
            if (tmpMap == null) { return; }
            if (tmpMap.right > -1)
            {
                tmpMap = GlobalVariables.GameMaps[tmpMap.right];
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

        private void DrawTileset()
        {
            SFML.Graphics.Sprite tmpSprite;
            //Draw Current Tileset
            if (currentTileset > -1)
            {
                int selW;
                int selH;
                int selX;
                int selY;
                tmpSprite = new Sprite(tilesetTex[currentTileset]);
                tilesetwindow.Draw(tmpSprite);
                //renderwindow.
                selX = curSelX;
                selY = curSelY;
                selW = curSelW;
                selH = curSelH;
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

        private void DrawMapBorders()
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateScrollBars();

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }

        private void hScrollMap_Scroll(object sender, ScrollEventArgs e)
        {
            picMap.Left = -hScrollMap.Value;

        }

        private void vScrollMap_Scroll(object sender, ScrollEventArgs e)
        {
            picMap.Top = -vScrollMap.Value;
        }

        private void vScrollTileset_Scroll(object sender, ScrollEventArgs e)
        {
            picTileset.Top = -vScrollTileset.Value;
        }

        private void hScrollTileset_Scroll(object sender, ScrollEventArgs e)
        {
            picTileset.Left = -hScrollTileset.Value;
        }

        private void picTileset_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > picTileset.Width || e.Y > picTileset.Height) { return; }
            tMouseDown = true;
            curSelX = (int)Math.Floor((double)e.X / 32);
            curSelY = (int)Math.Floor((double)e.Y / 32);
            curSelW = 0;
            curSelH = 0;
            if (curSelX < 0) { curSelX = 0; }
            if (curSelY < 0) { curSelY = 0; }
            switch (autotilemode)
            {
                case 1:
                case 5:
                    curSelW = 1;
                    curSelH = 2;
                    break;
                case 2:
                    curSelW = 0;
                    curSelH = 0;
                    break;
                case 3:
                    curSelW = 5;
                    curSelH = 2;
                    break;
                case 4:
                    curSelW = 1;
                    curSelH = 1;
                    break;
            }

        }

        private void picTileset_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > picTileset.Width || e.Y > picTileset.Height) { return; }
            if (tMouseDown && autotilemode == 0)
            {
                int tmpX = (int)Math.Floor((double)e.X / 32);
                int tmpY = (int)Math.Floor((double)e.Y / 32);
                curSelW = tmpX - curSelX;
                curSelH = tmpY - curSelY;
            }
        }

        private void picTileset_MouseUp(object sender, MouseEventArgs e)
        {

            int selW;
            int selH;
            int selX;
            int selY;
            selX = curSelX;
            selY = curSelY;
            selW = curSelW;
            selH = curSelH;
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
            curSelX = selX;
            curSelY = selY;
            curSelW = selW;
            curSelH = selH;
            tMouseDown = false;
        }

        private void picMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (editingLight != null) { return; }
            Map tmpMap = GlobalVariables.GameMaps[currentMap];
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mouseButton = 0;
                if (currentLayer == Constants.LAYER_COUNT)
                {
                    tmpMap.blocked[curTileX, curTileY] = 1;
                }
                else if (currentLayer == Constants.LAYER_COUNT + 1)
                {
                    Light tmpLight;
                    if ((tmpLight = GlobalVariables.GameMaps[currentMap].FindLightAt(curTileX, curTileY)) == null)
                    {
                        tmpLight = new Light(curTileX, curTileY);
                        grpLightEditor.BringToFront();
                        grpLightEditor.Show();
                        GlobalVariables.GameMaps[currentMap].Lights.Add(tmpLight);
                        lightsChanged = true;
                        backupLight = new Light(tmpLight.tileX, tmpLight.tileY);
                        backupLight.offsetX = tmpLight.offsetX;
                        backupLight.offsetX = tmpLight.offsetX;
                        backupLight.intensity = tmpLight.intensity;
                        backupLight.range = tmpLight.range;
                        txtLightIntensity.Text = "" + tmpLight.intensity;
                        txtLightRange.Text = "" + tmpLight.range;
                        txtLightOffsetX.Text = "" + tmpLight.offsetX;
                        txtLightOffsetY.Text = "" + tmpLight.offsetY;
                        scrlLightIntensity.Value = (int)(tmpLight.intensity * 10000.0);
                        scrlLightRange.Value = tmpLight.range;
                        editingLight = tmpLight;
                    }
                    else
                    {
                        grpLightEditor.BringToFront();
                        grpLightEditor.Show();
                        backupLight = new Light(tmpLight.tileX, tmpLight.tileY);
                        backupLight.offsetX = tmpLight.offsetX;
                        backupLight.offsetX = tmpLight.offsetX;
                        backupLight.intensity = tmpLight.intensity;
                        backupLight.range = tmpLight.range;
                        txtLightIntensity.Text = "" + tmpLight.intensity;
                        txtLightRange.Text = "" + tmpLight.range;
                        txtLightOffsetX.Text = "" + tmpLight.offsetX;
                        txtLightOffsetY.Text = "" + tmpLight.offsetY;
                        scrlLightIntensity.Value = (int)(tmpLight.intensity * 10000.0);
                        scrlLightRange.Value = tmpLight.range;
                        editingLight = tmpLight;
                    }
                }
                else if (currentLayer == Constants.LAYER_COUNT + 2)
                {
                    Event tmpEvent;
                    frmEvent tmpEventEditor;
                    if ((tmpEvent = GlobalVariables.GameMaps[currentMap].FindEventAt(curTileX, curTileY)) == null)
                    {
                        tmpEvent = new Event(curTileX, curTileY);
                        GlobalVariables.GameMaps[currentMap].Events.Add(tmpEvent);
                        tmpEventEditor = new frmEvent();
                        tmpEventEditor.myEvent = tmpEvent;
                        tmpEventEditor.myMap = GlobalVariables.GameMaps[currentMap];
                        tmpEventEditor.newEvent = true;
                        tmpEventEditor.initEditor();
                        tmpEventEditor.Show();
                    }
                    else
                    {
                        tmpEventEditor = new frmEvent();
                        tmpEventEditor.myEvent = tmpEvent;
                        tmpEventEditor.initEditor();
                        tmpEventEditor.Show();
                    }


                }
                else
                {
                    if (autotilemode == 0)
                    {
                        for (int x = 0; x <= curSelW; x++)
                        {
                            for (int y = 0; y <= curSelH; y++)
                            {
                                tmpMap.Layers[currentLayer].Tiles[curTileX + x, curTileY + y].tilesetIndex = currentTileset;
                                tmpMap.Layers[currentLayer].Tiles[curTileX + x, curTileY + y].x = curSelX + x;
                                tmpMap.Layers[currentLayer].Tiles[curTileX + x, curTileY + y].y = curSelY + y;
                                tmpMap.Layers[currentLayer].Tiles[curTileX + x, curTileY + y].Autotile = 0;
                                tmpMap.autotiles.initAutotiles();
                            }
                        }
                    }
                    else
                    {
                        tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].tilesetIndex = currentTileset;
                        tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].x = curSelX;
                        tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].y = curSelY;
                        tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].Autotile = (byte)autotilemode;
                        tmpMap.autotiles.initAutotiles();
                    }
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                mouseButton = 1;
                if (currentLayer == Constants.LAYER_COUNT)
                {
                    tmpMap.blocked[curTileX, curTileY] = 0;
                }
                else if (currentLayer == Constants.LAYER_COUNT + 1)
                {
                    Light tmpLight;
                    if ((tmpLight = GlobalVariables.GameMaps[currentMap].FindLightAt(curTileX, curTileY)) != null)
                    {
                        GlobalVariables.GameMaps[currentMap].Lights.Remove(tmpLight);
                        lightsChanged = true;
                    }
                }
                else if (currentLayer == Constants.LAYER_COUNT + 2)
                {
                    Event tmpEvent;
                    if ((tmpEvent = GlobalVariables.GameMaps[currentMap].FindEventAt(curTileX, curTileY)) != null)
                    {
                        GlobalVariables.GameMaps[currentMap].Events.Remove(tmpEvent);
                        tmpEvent.deleted = 1;
                    }
                }
                else
                {
                    tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].tilesetIndex = -1;
                    tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].Autotile = 0;
                    tmpMap.autotiles.initAutotiles();
                }
            }
            if (curTileX == 0)
            {
                if (tmpMap.left > -1)
                {
                    if (GlobalVariables.GameMaps[tmpMap.left] != null)
                    {
                        GlobalVariables.GameMaps[tmpMap.left].autotiles.initAutotiles();
                    }
                }
            }
            if (curTileY == 0)
            {
                if (tmpMap.up > -1)
                {
                    if (GlobalVariables.GameMaps[tmpMap.up] != null)
                    {
                        GlobalVariables.GameMaps[tmpMap.up].autotiles.initAutotiles();
                    }
                }
            }
            if (curTileX == Constants.MAP_WIDTH - 1)
            {
                if (tmpMap.right > -1)
                {
                    if (GlobalVariables.GameMaps[tmpMap.right] != null)
                    {
                        GlobalVariables.GameMaps[tmpMap.right].autotiles.initAutotiles();
                    }
                }
            }
            if (curTileY == Constants.MAP_HEIGHT - 1)
            {
                if (tmpMap.down > -1)
                {
                    if (GlobalVariables.GameMaps[tmpMap.down] != null)
                    {
                        GlobalVariables.GameMaps[tmpMap.down].autotiles.initAutotiles();
                    }
                }
            }

        }

        private void picMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (editingLight != null) { return; }
            mouseX = e.X;
            mouseY = e.Y;
            if (e.X >= picMap.Width - 32 || e.Y >= picMap.Height - 32) { return; }
            if (e.X < 32 || e.Y < 32) { return; }
            curTileX = (int)Math.Floor((double)(e.X - 32) / 32);
            curTileY = (int)Math.Floor((double)(e.Y - 32) / 32);
            if (curTileX < 0) { curTileX = 0; }
            if (curTileY < 0) { curTileY = 0; }

            if (mouseButton > -1)
            {
                Map tmpMap = GlobalVariables.GameMaps[currentMap];
                if (mouseButton == 0)
                {
                    if (currentLayer == Constants.LAYER_COUNT)
                    {
                        tmpMap.blocked[curTileX, curTileY] = 1;
                    }
                    else if (currentLayer == Constants.LAYER_COUNT + 1)
                    {

                    }
                    else
                    {
                        if (autotilemode == 0)
                        {
                            for (int x = 0; x <= curSelW; x++)
                            {
                                for (int y = 0; y <= curSelH; y++)
                                {
                                    tmpMap.Layers[currentLayer].Tiles[curTileX + x, curTileY + y].tilesetIndex = currentTileset;
                                    tmpMap.Layers[currentLayer].Tiles[curTileX + x, curTileY + y].x = curSelX + x;
                                    tmpMap.Layers[currentLayer].Tiles[curTileX + x, curTileY + y].y = curSelY + y;
                                    tmpMap.Layers[currentLayer].Tiles[curTileX + x, curTileY + y].Autotile = 0;
                                    tmpMap.autotiles.initAutotiles();
                                }
                            }
                        }
                        else
                        {
                            tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].tilesetIndex = currentTileset;
                            tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].x = curSelX;
                            tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].y = curSelY;
                            tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].Autotile = (byte)autotilemode;
                            tmpMap.autotiles.initAutotiles();
                        }
                    }
                    if (curTileX == 0)
                    {
                        if (tmpMap.left > -1)
                        {
                            if (GlobalVariables.GameMaps[tmpMap.left] != null)
                            {
                                GlobalVariables.GameMaps[tmpMap.left].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (curTileY == 0)
                    {
                        if (tmpMap.up > -1)
                        {
                            if (GlobalVariables.GameMaps[tmpMap.up] != null)
                            {
                                GlobalVariables.GameMaps[tmpMap.up].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (curTileX == Constants.MAP_WIDTH - 1)
                    {
                        if (tmpMap.right > -1)
                        {
                            if (GlobalVariables.GameMaps[tmpMap.right] != null)
                            {
                                GlobalVariables.GameMaps[tmpMap.right].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (curTileY == Constants.MAP_HEIGHT - 1)
                    {
                        if (tmpMap.down > -1)
                        {
                            if (GlobalVariables.GameMaps[tmpMap.down] != null)
                            {
                                GlobalVariables.GameMaps[tmpMap.down].autotiles.initAutotiles();
                            }
                        }
                    }
                }
                else if (mouseButton == 1)
                {
                    if (currentLayer == Constants.LAYER_COUNT)
                    {
                        tmpMap.blocked[curTileX, curTileY] = 0;
                    }
                    else if (currentLayer == Constants.LAYER_COUNT + 1)
                    {

                    }
                    else if (currentLayer == Constants.LAYER_COUNT + 2)
                    {

                    }
                    else
                    {
                        tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].tilesetIndex = -1;
                        tmpMap.Layers[currentLayer].Tiles[curTileX, curTileY].Autotile = 0;
                        tmpMap.autotiles.initAutotiles();
                    }
                    if (curTileX == 0)
                    {
                        if (tmpMap.left > -1)
                        {
                            if (GlobalVariables.GameMaps[tmpMap.left] != null)
                            {
                                GlobalVariables.GameMaps[tmpMap.left].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (curTileY == 0)
                    {
                        if (tmpMap.up > -1)
                        {
                            if (GlobalVariables.GameMaps[tmpMap.up] != null)
                            {
                                GlobalVariables.GameMaps[tmpMap.up].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (curTileX == Constants.MAP_WIDTH - 1)
                    {
                        if (tmpMap.right > -1)
                        {
                            if (GlobalVariables.GameMaps[tmpMap.right] != null)
                            {
                                GlobalVariables.GameMaps[tmpMap.right].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (curTileY == Constants.MAP_HEIGHT - 1)
                    {
                        if (tmpMap.down > -1)
                        {
                            if (GlobalVariables.GameMaps[tmpMap.down] != null)
                            {
                                GlobalVariables.GameMaps[tmpMap.down].autotiles.initAutotiles();
                            }
                        }
                    }
                }
            }
        }

        private void picMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (editingLight != null) { return; }
            mouseButton = -1;

        }

        private void cmbTilesets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (File.Exists("Resources/Tilesets/" + GlobalVariables.tilesets[cmbTilesets.SelectedIndex]))
            {
                currentTileset = cmbTilesets.SelectedIndex;
                InitTileset(currentTileset);
            }
            else
            {
                cmbTilesets.SelectedIndex = currentTileset;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save this map?", "Save Map", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                PacketSender.SendMap(currentMap);
            }
        }

        private void picMap_DoubleClick(object sender, EventArgs e)
        {
            if (mouseX >= 32 && mouseX <= 1024 - 32)
            {
                if (mouseY >= 0 && mouseY <= 32)
                {
                    if (GlobalVariables.GameMaps[currentMap].up == -1)
                    {
                        if (MessageBox.Show("Do you want to create a map here?", "Create new map.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                PacketSender.SendMap(currentMap);
                            }
                            else
                            {
                                PacketSender.SendCreateMap(0, currentMap);
                            }
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            PacketSender.SendMap(currentMap);
                        }
                        EnterMap(GlobalVariables.GameMaps[currentMap].up);
                    }
                }
                else if (mouseY >= 1024 - 32 && mouseY <= 1024)
                {
                    if (GlobalVariables.GameMaps[currentMap].down == -1)
                    {
                        if (MessageBox.Show("Do you want to create a map here?", "Create new map.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                PacketSender.SendMap(currentMap);
                            }
                            else
                            {
                                PacketSender.SendCreateMap(1, currentMap);
                            }
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            PacketSender.SendMap(currentMap);
                        }
                        EnterMap(GlobalVariables.GameMaps[currentMap].down);
                    }
                }
            }
            if (mouseY >= 32 && mouseY <= 1024 - 32)
            {
                if (mouseX >= 0 & mouseX <= 32)
                {
                    if (GlobalVariables.GameMaps[currentMap].left == -1)
                    {
                        if (MessageBox.Show("Do you want to create a map here?", "Create new map.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                PacketSender.SendMap(currentMap);
                            }
                            else
                            {
                                PacketSender.SendCreateMap(2, currentMap);
                            }
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            PacketSender.SendMap(currentMap);
                        }
                        EnterMap(GlobalVariables.GameMaps[currentMap].left);
                    }
                }
                else if (mouseX >= 1024 - 32 && mouseX <= 1024)
                {
                    if (GlobalVariables.GameMaps[currentMap].right == -1)
                    {
                        if (MessageBox.Show("Do you want to create a map here?", "Create new map.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                PacketSender.SendMap(currentMap);
                            }
                            else
                            {
                                PacketSender.SendCreateMap(3, currentMap);
                            }
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            PacketSender.SendMap(currentMap);
                        }
                        EnterMap(GlobalVariables.GameMaps[currentMap].right);
                    }
                }

            }
        }

        public void EnterMap(int mapNum)
        {
            currentMap = mapNum;
            if (GlobalVariables.GameMaps[mapNum] != null)
            {
                this.Text = "Intersect Editor - Map# " + mapNum + " " + GlobalVariables.GameMaps[mapNum].myName + " Revision: " + GlobalVariables.GameMaps[mapNum].revision;
            }
            picMap.Visible = false;
            vScrollMap.Value = 0;
            hScrollMap.Value = 0;
            picMap.Left = 0;
            picMap.Top = 0;
            PacketSender.SendNeedMap(currentMap);
        }

        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int oldCurSelX = curTileX;
            int oldCurSelY = curTileY;
            if (MessageBox.Show("Are you sure you want to fill this layer?", "Fill Layer", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        curTileX = x;
                        curTileY = y;
                        picMap_MouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, x * 32 + 32, y * 32 + 32, 0));
                        picMap_MouseUp(null, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
                    }
                }
                curTileX = oldCurSelX;
                curTileY = oldCurSelY;
            }
        }

        private void eraseLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to erase this layer?", "Erase Layer", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        curTileX = x;
                        curTileY = y;
                        picMap_MouseDown(null, new MouseEventArgs(MouseButtons.Right, 1, x * 32 + 32, y * 32 + 32, 0));
                        picMap_MouseUp(null, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
                    }
                }
            }
        }

        private void cmbAutotile_SelectedIndexChanged(object sender, EventArgs e)
        {
            autotilemode = cmbAutotile.SelectedIndex;
            switch (autotilemode)
            {
                case 1:
                case 5:
                    curSelW = 1;
                    curSelH = 2;
                    break;
                case 2:
                    curSelW = 0;
                    curSelH = 0;
                    break;
                case 3:
                    curSelW = 5;
                    curSelH = 2;
                    break;
                case 4:
                    curSelW = 1;
                    curSelH = 1;
                    break;

            }
        }

        private void mapListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grpMapList.Visible = !grpMapList.Visible;
            mapListToolStripMenuItem.Checked = grpMapList.Visible;
            if (grpMapList.Visible)
            {
                UpdateMapList();
            }
        }

        private void lblCloseMapList_Click(object sender, EventArgs e)
        {
            grpMapList.Hide();
            mapListToolStripMenuItem.Checked = grpMapList.Visible;
        }

        private void UpdateMapList()
        {
            lstGameMaps.Items.Clear();
            if (GlobalVariables.MapRefs != null)
            {
                for (int i = 0; i < GlobalVariables.MapRefs.Length; i++)
                {
                    if (GlobalVariables.MapRefs[i].deleted == 0)
                    {
                        lstGameMaps.Items.Add(i + ". " + GlobalVariables.MapRefs[i].MapName);
                    }
                }
            }
        }

        private void btnCloseProperties_Click(object sender, EventArgs e)
        {
            grpMapProperties.Hide();
        }

        private void mapPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grpMapProperties.BringToFront();
            txtMapName.Text = GlobalVariables.GameMaps[currentMap].myName;
            chkIndoors.Checked = GlobalVariables.GameMaps[currentMap].isIndoors;
            grpMapProperties.Show();
        }

        private void txtMapName_TextChanged(object sender, EventArgs e)
        {
            GlobalVariables.GameMaps[currentMap].myName = txtMapName.Text;
        }

        private void lstGameMaps_DoubleClick(object sender, EventArgs e)
        {
            if (lstGameMaps.SelectedIndex > -1)
            {
                int mapNum = Convert.ToInt32(((String)(lstGameMaps.Items[lstGameMaps.SelectedIndex])).Split('.')[0]);
                if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    PacketSender.SendMap(currentMap);
                }
                EnterMap(mapNum);
            }
        }

        private void picMap_Click(object sender, EventArgs e)
        {

        }

        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to create a new, unconnected map?", "New Map", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    PacketSender.SendMap(currentMap);
                }
                else
                {
                    PacketSender.SendCreateMap(-1, currentMap);
                }
            }
        }

        private void nightTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nightEnabled = !nightEnabled;
            nightTimeToolStripMenuItem.Checked = nightEnabled;
        }

        private void scrlLightRange_Scroll(object sender, ScrollEventArgs e)
        {
            if (editingLight == null) { return; }
            txtLightRange.Text = "" + editingLight.range;
            if (editingLight.graphic != null) { editingLight.graphic.Dispose(); }
            editingLight.graphic = null;
            editingLight.range = scrlLightRange.Value;
            lightsChanged = true;
        }

        private void scrlLightIntensity_Scroll(object sender, ScrollEventArgs e)
        {
            if (editingLight == null) { return; }
            editingLight.intensity = (double)scrlLightIntensity.Value / 10000.0;
            if (editingLight.graphic != null) { editingLight.graphic.Dispose(); }
            editingLight.graphic = null;
            txtLightIntensity.Text = "" + editingLight.intensity;
            lightsChanged = true;
        }

        private void txtLightIntensity_TextChanged(object sender, EventArgs e)
        {
            if (editingLight == null) { return; }
            try
            {
                double intensity = Convert.ToDouble(txtLightIntensity.Text);
                if (intensity < 0) { intensity = 0; }
                if (intensity > 1) { intensity = 1; }
                editingLight.intensity = intensity;
                editingLight.graphic = null;
                txtLightIntensity.Text = "" + editingLight.intensity;
                scrlLightIntensity.Value = (int)(intensity * 10000.0);
                lightsChanged = true;
            }
            catch { }
        }

        private void txtLightRange_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (editingLight == null) { return; }
                int range = Convert.ToInt32(txtLightRange.Text);
                if (range < 2) { range = 2; }
                if (range > 179) { range = 179; }
                editingLight.range = range;
                editingLight.graphic = null;
                lightsChanged = true;
                txtLightRange.Text = "" + range;
            }
            catch { }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            grpLightEditor.Hide();
            editingLight = null;
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {
            editingLight.intensity = backupLight.intensity;
            editingLight.range = backupLight.range;
            editingLight.offsetX = backupLight.offsetX;
            editingLight.offsetY = backupLight.offsetY;
            editingLight.graphic.Dispose();
            editingLight.graphic = null;
            lightsChanged = false;
            grpLightEditor.Hide();
        }

        private void chkIndoors_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVariables.GameMaps[currentMap].isIndoors = chkIndoors.Checked;
        }

        private void txtLightOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (editingLight == null) { return; }
                int offsetY = Convert.ToInt32(txtLightOffsetY.Text);
                editingLight.offsetY = offsetY;
                editingLight.graphic = null;
                lightsChanged = true;
            }
            catch { }
        }

        private void txtLightOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (editingLight == null) { return; }
                int offsetX = Convert.ToInt32(txtLightOffsetX.Text);
                editingLight.offsetX = offsetX;
                editingLight.graphic = null;
                lightsChanged = true;
            }
            catch { }
        }

        private void hihiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap sp1 = new Bitmap(200,200);
            Graphics g = Graphics.FromImage(sp1);
            GraphicsPath pth = new GraphicsPath();
            pth.AddEllipse(0, 0, 200 - 1, 200 - 1);
            PathGradientBrush pgb = new PathGradientBrush(pth);
            pgb.CenterColor = System.Drawing.Color.FromArgb((int)((double)(255 * .8)), 255, 255, 255);
            pgb.SurroundColors = new System.Drawing.Color[] { System.Drawing.Color.Transparent };
            pgb.FocusScales = new PointF(0.5f, 0.5f);
            g.FillPath(pgb, pth);
            g.Dispose();
            sp1.Save("c:\\tmp\\playerlight.png", System.Drawing.Imaging.ImageFormat.Png);
        }


    }
}
