/*
    Intersect Game Engine (Editor)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using System.CodeDom;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Windows.Forms;
using System.Xml;
using Intersect_Editor.Classes.General;
using Intersect_Editor.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Color = System.Drawing.Color;

namespace Intersect_Editor.Classes
{
    public static class EditorGraphics
    {

        //MonoGame Setup/Device
        private static GraphicsDevice _graphicsDevice;
        private static PresentationParameters _presentationParams = new PresentationParameters();
        private static SwapChainRenderTarget _mapEditorChain;
        private static SwapChainRenderTarget _tilesetChain;

        //Game Graphics
        public static bool TilesetsLoaded;
        public static Texture2D[] TilesetTextures;
        public static List<string> ItemNames;
        public static Texture2D[] ItemTextures;
        public static List<string> EntityFileNames;
        public static Texture2D[] EntityTextures;
        public static List<string> SpellFileNames;
        public static Texture2D[] SpellTextures;
        public static List<string> AnimationFileNames;
        public static Texture2D[] AnimationTextures;
        public static List<string> ImageFileNames;
        public static Texture2D[] ImageTextures;
        public static List<string> FogFileNames;
        public static Texture2D[] FogTextures;
        public static List<string> ResourceFileNames;
        public static Texture2D[] ResourceTextures;
        public static List<string> PaperdollFileNames;
        public static Texture2D[] PaperdollTextures;

        //Face Textures
        public static List<string> FaceFileNames;
        public static Texture2D[] FaceTextures;

        //Basic Editor Textures
        private static Texture2D _trans;
        //Texture for attributes
        private static Texture2D _attributesTex;
        //Texture for events
        private static Texture2D _eventTex;
        //Single tile texture for light placement
        private static Texture2D _lightTex;
        //Texture for NPC Spawns
        private static Texture2D _spawnTex;
        private static RenderTarget2D _whiteTex;

        //Light Stuff
        public static byte CurrentBrightness = 100;
        public static bool HideDarkness = false;
        public static RenderTarget2D DarknessTexture;
        private static Effect RadialGradientShader;
        public static BlendState MultiplyState;
        private static List<KeyValuePair<Microsoft.Xna.Framework.Point, Light>> _lightQueue = new List<KeyValuePair<Microsoft.Xna.Framework.Point,Light>>();

        //Overlay Stuff
        public static Color OverlayColor = Color.Transparent;
        public static bool HideOverlay = false;

        //Fog Stuff
        public static bool HideFog = false;
        private static long _fogUpdateTime = Environment.TickCount;
        private static float _fogCurrentX = 0;
        private static float _fogCurrentY = 0;

        //Resources
        public static bool HideResources = false;

        //Advanced Editing Features
        public static bool HideTilePreview = false;
        public static bool HideGrid = true;
        public static bool TilePreviewUpdated = false;
        public static MapStruct TilePreviewStruct;

        //Rendering Variables
        private static SpriteBatch _spriteBatch;
        private static bool _spriteBatchBegan;
        private static BlendState _currentBlendmode = BlendState.AlphaBlend;
        private static Effect _currentShader = null;
        private static RenderTarget2D _currentTarget = null;

        //Setup and Loading
        public static void InitMonogame(frmMain myForm)
        {
            try
            {
                //Create the Graphics Device
                _presentationParams.IsFullScreen = false;
                _presentationParams.BackBufferWidth = (Options.TileWidth + 2) * Options.MapWidth;
                _presentationParams.BackBufferHeight = (Options.TileHeight + 2) * Options.MapHeight;
                _presentationParams.RenderTargetUsage = RenderTargetUsage.DiscardContents;

                // Create device
                _graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach,
                    _presentationParams);

                //We create a dummy game service so we can load up a content manager.
                var container = new GameServiceContainer();
                container.AddService(typeof(IGraphicsDeviceService), new DummyGraphicsDeviceManager(_graphicsDevice));
                ContentManager cm = new ContentManager(container,"");

                //Define our spritebatch :D
                _spriteBatch = new SpriteBatch(_graphicsDevice);

                //Load Various Editor Resources
                _trans = LoadTexture("Resources/transV.png");
                _attributesTex = LoadTexture("Resources/attributes.png");
                _lightTex = LoadTexture("Resources/jcl.png");
                _spawnTex = LoadTexture("Resources/MapSpawn.png");
                _eventTex = LoadTexture("Resources/jce.png");
                SetupWhiteTex();

                //Load the rest of the graphics and audio
                LoadGraphics(myForm);
                Audio.LoadAudio();

                //Load Shader
                RadialGradientShader = cm.Load<Effect>("Resources/Shaders/RadialGradient");

                //Create our multiplicative blending state.
                MultiplyState = new BlendState();
                MultiplyState.ColorBlendFunction = BlendFunction.Add;
                MultiplyState.ColorSourceBlend = Microsoft.Xna.Framework.Graphics.Blend.DestinationColor;
                MultiplyState.ColorDestinationBlend = Microsoft.Xna.Framework.Graphics.Blend.Zero;
            }
            catch (Exception ex)
            {
                // ignored
                MessageBox.Show("Failed to initialize MonoGame. Exception Info: " + ex.ToString() + "\nClosing Now");
                Application.Exit();
            }
        }
        public static GraphicsDevice GetGraphicsDevice()
        {
            return _graphicsDevice;
        }
        public static void SetMapEditorChain(SwapChainRenderTarget _chain)
        {
            _mapEditorChain = _chain;
        }
        public static void SetTilesetChain(SwapChainRenderTarget _chain)
        {
            _tilesetChain = _chain;
        }
        private static void LoadGraphics(frmMain myForm)
        {
            LoadTilesets(myForm);
            LoadItems();
            LoadEntities();
            LoadSpells();
            LoadAnimations();
            LoadFaces();
            LoadImages();
            LoadFogs();
            LoadResources();
            LoadPaperdolls();
        }

        //Resource Allocation
        private static void SetupWhiteTex()
        {
            _whiteTex = CreateRenderTexture(1, 1);
            _graphicsDevice.SetRenderTarget(_whiteTex);
            _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.White);
            _graphicsDevice.SetRenderTarget(null);
        }
        public static RenderTarget2D GetWhiteTex()
        {
            return _whiteTex;
        }
        private static Texture2D LoadTexture(string filename)
        {
            using (var fileStream = new FileStream(filename, FileMode.Open))
            {
                return Texture2D.FromStream(_graphicsDevice, fileStream);
            }
        }
        public static RenderTarget2D CreateRenderTexture(int width, int height)
        {
            return new RenderTarget2D(_graphicsDevice, width,height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
        }

        //Loading Game Resources
        private static void LoadTilesets(frmMain myForm)
        {
            if (!Directory.Exists("Resources/Tilesets")) { Directory.CreateDirectory("Resources/Tilesets"); }
            var tilesets = Directory.GetFiles("Resources/Tilesets", "*.png");
            Array.Sort(tilesets, new AlphanumComparatorFast());
            var tilesetsUpdated = false;
            if (tilesets.Length > 0)
            {
                for (var i = 0; i < tilesets.Length; i++)
                {
                    tilesets[i] = tilesets[i].Replace("Resources/Tilesets\\", "");
                    if (Globals.Tilesets != null)
                    {
                        if (Globals.Tilesets.Length > 0)
                        {
                            for (var x = 0; x < Globals.Tilesets.Length; x++)
                            {
                                if (Globals.Tilesets[x] == tilesets[i])
                                {
                                    break;
                                }
                                if (x != Globals.Tilesets.Length - 1) continue;
                                var newTilesets = new string[Globals.Tilesets.Length + 1];
                                Globals.Tilesets.CopyTo(newTilesets, 0);
                                newTilesets[Globals.Tilesets.Length] = tilesets[i];
                                Globals.Tilesets = newTilesets;
                                tilesetsUpdated = true;
                            }
                        }
                        else
                        {
                            var newTilesets = new string[1];
                            newTilesets[0] = tilesets[i];
                            Globals.Tilesets = newTilesets;
                            tilesetsUpdated = true;
                        }
                    }
                    else
                    {
                        var newTilesets = new string[1];
                        newTilesets[0] = tilesets[i];
                        Globals.Tilesets = newTilesets;
                        tilesetsUpdated = true;
                    }
                }

                if (tilesetsUpdated)
                {
                    PacketSender.SendTilesets();
                }

                Globals.MapLayersWindow.cmbTilesets.Items.Clear();
                foreach (var filename in Globals.Tilesets)
                {
                    if (File.Exists("Resources/Tilesets/" + filename))
                    {
                        Globals.MapLayersWindow.cmbTilesets.Items.Add(filename);
                    }
                    else
                    {
                        Globals.MapLayersWindow.cmbTilesets.Items.Add(filename + " - [MISSING]");
                    }
                }
                Globals.MapLayersWindow.cmbTilesets.SelectedIndex = 0;
                Globals.CurrentTileset = 0;

                TilesetTextures = new Texture2D[Globals.Tilesets.Length];
                for (var i = 0; i < Globals.Tilesets.Length; i++)
                {
                    if (File.Exists("Resources/Tilesets/" + Globals.Tilesets[i]))
                    {
                        TilesetTextures[i] = LoadTexture("Resources/Tilesets/" + Globals.Tilesets[i]);
                    }
                    else
                    {

                    }
                }
                TilesetsLoaded = true;
            }
        }
        private static void LoadItems()
        {
            if (!Directory.Exists("Resources/Items")) { Directory.CreateDirectory("Resources/Items"); }
            var items = Directory.GetFiles("Resources/Items", "*.png");
            Array.Sort(items, new AlphanumComparatorFast());
            ItemNames = new List<string>();
            ItemTextures = new Texture2D[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                ItemNames.Add(items[i].Replace("Resources/Items\\", ""));
                ItemTextures[i] = LoadTexture("Resources/Items/" + ItemNames[i]);
            }
        }
        private static void LoadEntities()
        {
            if (!Directory.Exists("Resources/Entities")) { Directory.CreateDirectory("Resources/Entities"); }
            var chars = Directory.GetFiles("Resources/Entities", "*.png");
            Array.Sort(chars, new AlphanumComparatorFast());
            EntityFileNames = new List<string>();
            EntityTextures = new Texture2D[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                EntityFileNames.Add(chars[i].Replace("Resources/Entities\\", ""));
                EntityTextures[i] = LoadTexture("Resources/Entities/" + EntityFileNames[i]);
            }
        }
        private static void LoadSpells()
        {
            if (!Directory.Exists("Resources/Spells")) { Directory.CreateDirectory("Resources/Spells"); }
            var spells = Directory.GetFiles("Resources/Spells", "*.png");
            Array.Sort(spells, new AlphanumComparatorFast());
            SpellFileNames = new List<string>();
            SpellTextures = new Texture2D[spells.Length];
            for (int i = 0; i < spells.Length; i++)
            {
                SpellFileNames.Add(spells[i].Replace("Resources/Spells\\", ""));
                SpellTextures[i] = LoadTexture("Resources/Spells/" + SpellFileNames[i]);
            }
        }
        private static void LoadAnimations()
        {
            if (!Directory.Exists("Resources/Animations")) { Directory.CreateDirectory("Resources/Animations"); }
            var animations = Directory.GetFiles("Resources/Animations", "*.png");
            Array.Sort(animations, new AlphanumComparatorFast());
            AnimationFileNames = new List<string>();
            AnimationTextures = new Texture2D[animations.Length];
            for (int i = 0; i < animations.Length; i++)
            {
                AnimationFileNames.Add(animations[i].Replace("Resources/Animations\\", ""));
                AnimationTextures[i] = LoadTexture("Resources/Animations/" + AnimationFileNames[i]);
            }
        }
        private static void LoadFaces()
        {
            if (!Directory.Exists("Resources/Faces")) { Directory.CreateDirectory("Resources/Faces"); }
            var faces = Directory.GetFiles("Resources/Faces", "*.png");
            Array.Sort(faces, new AlphanumComparatorFast());
            FaceFileNames = new List<string>();
            FaceTextures = new Texture2D[faces.Length];
            for (int i = 0; i < faces.Length; i++)
            {
                FaceFileNames.Add(faces[i].Replace("Resources/Faces\\", ""));
                FaceTextures[i] = LoadTexture("Resources/Faces/" + FaceFileNames[i]);
            }
        }
        private static void LoadImages()
        {
            if (!Directory.Exists("Resources/Images")) { Directory.CreateDirectory("Resources/Images"); }
            var images = Directory.GetFiles("Resources/Images", "*.png");
            Array.Sort(images, new AlphanumComparatorFast());
            ImageFileNames = new List<string>();
            ImageTextures = new Texture2D[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                ImageFileNames.Add(images[i].Replace("Resources/Images\\", ""));
                ImageTextures[i] = LoadTexture("Resources/Images/" + ImageFileNames[i]);
            }
        }
        private static void LoadFogs()
        {
            if (!Directory.Exists("Resources/Fogs")) { Directory.CreateDirectory("Resources/Fogs"); }
            var fogs = Directory.GetFiles("Resources/Fogs", "*.png");
            Array.Sort(fogs, new AlphanumComparatorFast());
            FogFileNames = new List<string>();
            FogTextures = new Texture2D[fogs.Length];
            for (int i = 0; i < fogs.Length; i++)
            {
                FogFileNames.Add(fogs[i].Replace("Resources/Fogs\\", ""));
                FogTextures[i] = LoadTexture("Resources/Fogs/" + FogFileNames[i]);
            }
        }
        private static void LoadResources()
        {
            if (!Directory.Exists("Resources/Resources")) { Directory.CreateDirectory("Resources/Resources"); }
            var resources = Directory.GetFiles("Resources/Resources", "*.png");
            Array.Sort(resources, new AlphanumComparatorFast());
            ResourceFileNames = new List<string>();
            ResourceTextures = new Texture2D[resources.Length];
            for (int i = 0; i < resources.Length; i++)
            {
                ResourceFileNames.Add(resources[i].Replace("Resources/Resources\\", ""));
                ResourceTextures[i] = LoadTexture("Resources/Resources/" + ResourceFileNames[i]);
            }
        }
        private static void LoadPaperdolls()
        {
            if (!Directory.Exists("Resources/Paperdolls")) { Directory.CreateDirectory("Resources/Paperdolls"); }
            var resources = Directory.GetFiles("Resources/Paperdolls", "*.png");
            Array.Sort(resources, new AlphanumComparatorFast());
            PaperdollFileNames = new List<string>();
            PaperdollTextures = new Texture2D[resources.Length];
            for (int i = 0; i < resources.Length; i++)
            {
                PaperdollFileNames.Add(resources[i].Replace("Resources/Paperdolls\\", ""));
                PaperdollTextures[i] = LoadTexture("Resources/Paperdolls/" + PaperdollFileNames[i]);
            }
        }

        //Rendering
        public static void Render()
        {
            if (_mapEditorChain != null)
            {
                _graphicsDevice.SetRenderTarget(_mapEditorChain);
                _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
                StartSpritebatch();
                ClearDarknessTexture();
                DrawTileset();

                //Draw Current Map
                if (Globals.MapEditorWindow.picMap.Visible && Globals.CurrentMap > -1 &&
                    Globals.GameMaps[Globals.CurrentMap] != null)
                {
                    DrawTransparentBorders();
                    for (int i = 0; i < 2; i++)
                    {
                        for (int z = -1; z < 4; z++)
                        {
                            DrawMap(z, false, i, null);
                        }
                        if (i == 0)
                        {
                            for (int z = -1; z < 4; z++)
                            {
                                DrawMapAttributes(z, false, null, false);
                            }
                        }
                    }
                    for (int z = -1; z < 4; z++)
                    {
                        DrawMapAttributes(z, false, null, true);
                    }
                    if (!HideFog)
                    {
                        DrawFog(null);
                    }
                    if (!HideOverlay)
                    {
                        DrawMapOverlay(null);
                    }
                    if (!HideDarkness || Globals.CurrentLayer == Options.LayerCount + 1)
                    {
                        OverlayDarkness(null);
                    }
                    if (!HideGrid) DrawMapGrid();

                    DrawMapBorders();
                    DrawSelectionRect();
                }
                EndSpriteBatch();
                _mapEditorChain.Present();
            }
        }
        private static void DrawMapGrid()
        {
            for (int x = 0; x < Options.MapWidth; x++)
            {
                DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x * Options.TileWidth + Options.TileWidth, Options.TileHeight, 1, Options.MapHeight * Options.TileHeight), null);
            }
            for (int y = 0; y < Options.MapHeight; y++)
            {
                DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(Options.TileWidth, y * Options.TileHeight + Options.TileHeight, Options.MapWidth * Options.TileWidth, 1), null);
            }
        }
        private static void DrawTransparentBorders()
        {
            for (int x = 0; x < Options.MapWidth + 2; x++)
            {
                DrawTexture(_trans, Options.TileWidth * x, 0, 0, 0, Options.TileWidth, Options.TileHeight, null);
                DrawTexture(_trans, Options.TileWidth * x, Options.TileHeight * (Options.MapHeight + 1), 0, 0, Options.TileWidth, Options.TileHeight, null);
            }
            for (int y = 1; y < Options.MapHeight + 1; y++)
            {
                DrawTexture(_trans, 0, Options.TileHeight * y, 0, 0, Options.TileWidth, Options.TileHeight, null);
                DrawTexture(_trans, Options.TileWidth * (Options.MapWidth + 1), Options.TileHeight * y, 0, 0, Options.TileWidth, Options.TileHeight, null);
            }
        }
        private static void DrawAutoTile(int layerNum, int destX, int destY, int quarterNum, int x, int y, MapStruct map, RenderTarget2D target)
        {
            int yOffset = 0, xOffset = 0;

            // calculate the offset
            switch (map.Layers[layerNum].Tiles[x, y].Autotile)
            {
                case MapAutotiles.AutotileWaterfall:
                    yOffset = (Globals.WaterfallFrame - 1) * Options.TileHeight;
                    break;
                case MapAutotiles.AutotileAnim:
                    xOffset = Globals.AutotileFrame * 64;
                    break;
                case MapAutotiles.AutotileCliff:
                    yOffset = -Options.TileHeight;
                    break;
            }
            DrawTexture(TilesetTextures[map.Layers[layerNum].Tiles[x, y].TilesetIndex],
                                destX, destY,
                                (int)map.Autotiles.Autotile[x, y].Layer[layerNum].SrcX[quarterNum] + xOffset,
                                (int)map.Autotiles.Autotile[x, y].Layer[layerNum].SrcY[quarterNum] + yOffset,
                                16, 16, target);

        }
        private static void DrawMap(int dir, bool screenShotting, int layer, RenderTarget2D RenderTarget2D)
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (tmpMap == null || tmpMap.Deleted == 1) { return; }
            int selX = Globals.CurMapSelX, selY = Globals.CurMapSelY, selW = Globals.CurMapSelW, selH = Globals.CurMapSelH;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0, z1 = 0, z2 = 3, xoffset = 0, yoffset = 0;
            int dragxoffset = 0, dragyoffset = 0;
            if (Globals.CurrentTool == (int)Enums.EdittingTool.Rectangle ||
            Globals.CurrentTool == (int)Enums.EdittingTool.Selection)
            {
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
            }
            if (layer == 1)
            {
                z1 = 3;
                z2 = 5;
            }
            switch (dir)
            {
                case -1:
                    x1 = 0;
                    x2 = Options.MapWidth;
                    y1 = 0;
                    y2 = Options.MapHeight;
                    xoffset = Options.TileWidth;
                    yoffset = Options.TileHeight;
                    break;
                case 0:
                    if (tmpMap.Up <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Up];
                    x1 = 0;
                    x2 = Options.MapWidth;
                    y1 = Options.MapHeight - 1;
                    y2 = Options.MapHeight;
                    xoffset = Options.TileWidth;
                    yoffset = -(Options.MapHeight - 1) * Options.TileHeight;
                    break;
                case 1:
                    if (tmpMap.Down <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Down];
                    x1 = 0;
                    x2 = Options.MapWidth;
                    y1 = 0;
                    y2 = 1;
                    xoffset = Options.TileWidth;
                    yoffset = Options.TileHeight + Options.MapHeight * Options.TileHeight;
                    break;
                case 2:
                    if (tmpMap.Left <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Left];
                    x1 = Options.MapWidth - 1;
                    x2 = Options.MapWidth;
                    y1 = 0;
                    y2 = Options.MapHeight;
                    xoffset = Options.TileWidth - Options.MapWidth * Options.TileWidth;
                    yoffset = Options.TileHeight;
                    break;
                case 3:
                    if (tmpMap.Right <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Right];
                    x1 = 0;
                    x2 = 1;
                    y1 = 0;
                    y2 = Options.MapHeight;
                    xoffset = Options.TileWidth + Options.MapWidth * Options.TileWidth;
                    yoffset = Options.TileHeight;
                    break;
            }
            if (tmpMap == null || tmpMap.Deleted == 1) { return; }
            if (screenShotting)
            {
                xoffset -= Options.TileWidth;
                yoffset -= Options.TileHeight;
            }

            if (dir == -1)
            {
                if (Globals.Dragging)
                {
                    if (Globals.MouseButton == 0)
                    {
                        dragxoffset = Globals.TotalTileDragX - (Globals.TileDragX - Globals.CurTileX);
                        dragyoffset = Globals.TotalTileDragY - (Globals.TileDragY - Globals.CurTileY);
                    }
                    else
                    {
                        dragxoffset = Globals.TotalTileDragX;
                        dragyoffset = Globals.TotalTileDragY;
                    }

                }
                if ((!HideTilePreview || Globals.Dragging) && !screenShotting)
                {
                    tmpMap = TilePreviewStruct;
                    if (TilePreviewUpdated || TilePreviewStruct == null)
                    {
                        TilePreviewStruct = new MapStruct(Globals.GameMaps[Globals.CurrentMap]);
                        //Lets Create the Preview
                        //Mimic Mouse Down
                        tmpMap = TilePreviewStruct;
                        if (Globals.CurrentTool == (int)Enums.EdittingTool.Selection && Globals.Dragging)
                        {
                            Globals.MapEditorWindow.ProcessSelectionMovement(tmpMap, false, true);
                        }
                        else
                        {
                            switch (Globals.CurrentLayer)
                            {
                                case Options.LayerCount:
                                    if (Globals.CurrentTool == (int)Enums.EdittingTool.Pen)
                                    {
                                        Globals.MapLayersWindow.PlaceAttribute(tmpMap, Globals.CurTileX,
                                            Globals.CurTileY);
                                    }
                                    else if (Globals.CurrentTool == (int)Enums.EdittingTool.Rectangle)
                                    {
                                        for (int x = selX; x < selX + selW + 1; x++)
                                        {
                                            for (int y = selY; y < selY + selH + 1; y++)
                                            {
                                                if (Globals.MouseButton == 0)
                                                {
                                                    Globals.MapLayersWindow.PlaceAttribute(tmpMap, x, y);
                                                }
                                                else if (Globals.MouseButton == 1)
                                                {
                                                    Globals.MapLayersWindow.RemoveAttribute(tmpMap, x, y);
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case Options.LayerCount + 1:
                                    break;
                                case Options.LayerCount + 2:
                                    break;
                                case Options.LayerCount + 3:
                                    break;
                                default:
                                    if (Globals.CurrentTool == (int)Enums.EdittingTool.Pen)
                                    {
                                        if (Globals.Autotilemode == 0)
                                        {
                                            for (var x = 0; x <= Globals.CurSelW; x++)
                                            {
                                                for (var y = 0; y <= Globals.CurSelH; y++)
                                                {
                                                    if (Globals.CurTileX + x >= 0 &&
                                                        Globals.CurTileX + x < Options.MapWidth &&
                                                        Globals.CurTileY + y >= 0 &&
                                                        Globals.CurTileY + y < Options.MapHeight)
                                                    {
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                            Globals.CurTileX + x, Globals.CurTileY + y].TilesetIndex =
                                                            Globals.CurrentTileset;
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                            Globals.CurTileX + x, Globals.CurTileY + y].X =
                                                            Globals.CurSelX + x;
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                            Globals.CurTileX + x, Globals.CurTileY + y].Y =
                                                            Globals.CurSelY + y;
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                            Globals.CurTileX + x, Globals.CurTileY + y].Autotile = 0;
                                                        tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX + x,
                                                            Globals.CurTileY + y, Globals.CurrentLayer);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY
                                                ].TilesetIndex = Globals.CurrentTileset;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY
                                                ].X = Globals.CurSelX;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY
                                                ].Y = Globals.CurSelY;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY
                                                ].Autotile = (byte)Globals.Autotilemode;
                                            tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX, Globals.CurTileY,
                                                Globals.CurrentLayer);
                                        }
                                    }
                                    else if (Globals.CurrentTool == (int)Enums.EdittingTool.Rectangle)
                                    {
                                        int x = 0;
                                        int y = 0;
                                        for (int x0 = selX; x0 < selX + selW + 1; x0++)
                                        {
                                            for (int y0 = selY; y0 < selY + selH + 1; y0++)
                                            {
                                                x = (x0 - selX) % (Globals.CurSelW + 1);
                                                y = (y0 - selY) % (Globals.CurSelH + 1);
                                                if (Globals.Autotilemode == 0)
                                                {
                                                    if (x0 >= 0 && x0 < Options.MapWidth && y0 >= 0 &&
                                                        y0 < Options.MapHeight && x0 < selX + selW + 1 &&
                                                        y0 < selY + selH + 1)
                                                    {
                                                        if (Globals.MouseButton == 0)
                                                        {
                                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                                x0, y0].TilesetIndex =
                                                                Globals.CurrentTileset;
                                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                                x0, y0].X = Globals.CurSelX + x;
                                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                                x0, y0].Y = Globals.CurSelY + y;
                                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                                x0, y0].Autotile =
                                                                (byte)Globals.Autotilemode;
                                                        }
                                                        else if (Globals.MouseButton == 1)
                                                        {
                                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                                x0, y0].TilesetIndex = -1;
                                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                                x0, y0].X = 0;
                                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                                x0, y0].Y = 0;
                                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                                x0, y0].Autotile = 0;
                                                        }
                                                        tmpMap.Autotiles.UpdateAutoTiles(x0, y0,
                                                            Globals.CurrentLayer);
                                                    }
                                                }
                                                else
                                                {
                                                    if (Globals.MouseButton == 0)
                                                    {
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].TilesetIndex =
                                                            Globals.CurrentTileset;
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].X =
                                                            Globals.CurSelX;
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Y =
                                                            Globals.CurSelY;
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Autotile =
                                                            (byte)Globals.Autotilemode;
                                                    }
                                                    else if (Globals.MouseButton == 1)
                                                    {
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].TilesetIndex =
                                                            -1;
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].X = 0;
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Y = 0;
                                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Autotile = 0;
                                                    }
                                                    tmpMap.Autotiles.UpdateAutoTiles(x0, y0,
                                                        Globals.CurrentLayer);
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        TilePreviewUpdated = false;
                    }
                }
                else
                {
                    tmpMap = Globals.GameMaps[Globals.CurrentMap];
                }
            }


            for (var x = x1; x < x2; x++)
            {
                for (var y = y1; y < y2; y++)
                {
                    for (var z = z1; z < z2; z++)
                    {
                        if (tmpMap.Layers[z].Tiles[x, y].TilesetIndex <= -1) continue;
                        if (TilesetTextures[tmpMap.Layers[z].Tiles[x, y].TilesetIndex] == null) continue;
                        if (tmpMap.Autotiles.Autotile[x, y].Layer[z].RenderState != MapAutotiles.RenderStateNormal)
                        {
                            if (tmpMap.Autotiles.Autotile[x, y].Layer[z].RenderState != MapAutotiles.RenderStateAutotile)
                                continue;
                            DrawAutoTile(z, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, 1, x, y, tmpMap, RenderTarget2D);
                            DrawAutoTile(z, x * Options.TileWidth + 16 + xoffset, y * Options.TileHeight + yoffset, 2, x, y, tmpMap, RenderTarget2D);
                            DrawAutoTile(z, x * Options.TileWidth + xoffset, y * Options.TileHeight + 16 + yoffset, 3, x, y, tmpMap, RenderTarget2D);
                            DrawAutoTile(z, x * Options.TileWidth + 16 + xoffset, y * Options.TileHeight + 16 + yoffset, 4, x, y, tmpMap, RenderTarget2D);
                        }
                        else
                        {
                            DrawTexture(TilesetTextures[tmpMap.Layers[z].Tiles[x, y].TilesetIndex],
                                x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset,
                                tmpMap.Layers[z].Tiles[x, y].X * Options.TileWidth, tmpMap.Layers[z].Tiles[x, y].Y * Options.TileHeight,
                                Options.TileWidth, Options.TileHeight, RenderTarget2D);
                        }
                    }
                }
            }

            foreach (var light in tmpMap.Lights)
            {
                double w = light.Size;
                var x = xoffset + (light.TileX * Options.TileWidth + light.OffsetX) + 16;
                var y = yoffset + (light.TileY * Options.TileHeight + light.OffsetY) + 16;
                if (!HideDarkness) AddLight(x,y,light,null);
            }
        }
        private static void DrawSelectionRect()
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (tmpMap == null || tmpMap.Deleted == 1) { return; }
            int selX = Globals.CurMapSelX, selY = Globals.CurMapSelY, selW = Globals.CurMapSelW, selH = Globals.CurMapSelH;
            int dragxoffset = 0, dragyoffset = 0;
            if (Globals.CurrentTool == (int)Enums.EdittingTool.Rectangle ||
            Globals.CurrentTool == (int)Enums.EdittingTool.Selection)
            {
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
            }
            if (Globals.Dragging)
            {
                if (Globals.MouseButton == 0)
                {
                    dragxoffset = Globals.TotalTileDragX - (Globals.TileDragX - Globals.CurTileX);
                    dragyoffset = Globals.TotalTileDragY - (Globals.TileDragY - Globals.CurTileY);
                }
                else
                {
                    dragxoffset = Globals.TotalTileDragX;
                    dragyoffset = Globals.TotalTileDragY;
                }

            }
            switch (Globals.CurrentLayer)
            {
                case Options.LayerCount:
                    //Draw attributes
                    for (int x = 0; x < Options.MapWidth; x++)
                    {
                        for (int y = 0; y < Options.MapHeight; y++)
                        {
                            if (tmpMap.Attributes[x, y].value > 0)
                            {
                                DrawTexture(_attributesTex,x * Options.TileWidth + Options.TileWidth,
                                y * Options.TileHeight + Options.TileHeight, 0, (tmpMap.Attributes[x, y].value - 1) * Options.TileHeight, Options.TileWidth,
                                Options.TileHeight, null);
                            }
                        }

                    }
                    break;
                case Options.LayerCount + 1:

                    break;
                case Options.LayerCount + 2:
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            if (tmpMap.FindEventAt(x, y) == null) continue;
                            DrawTexture(_eventTex, x * Options.TileWidth + Options.TileWidth,
                                y * Options.TileHeight + Options.TileHeight, 0, 0, Options.TileWidth,
                                Options.TileHeight, null);
                        }

                    }
                    break;
                case Options.LayerCount + 3:
                    for (int i = 0; i < tmpMap.Spawns.Count; i++)
                    {
                        if (tmpMap.Spawns[i].X >= 0 && tmpMap.Spawns[i].Y >= 0)
                        {
                            DrawTexture(_spawnTex, tmpMap.Spawns[i].X*Options.TileWidth + Options.TileWidth,
                                tmpMap.Spawns[i].Y*Options.TileHeight + Options.TileHeight, 0, 0, Options.TileWidth,
                                Options.TileHeight, null);
                        }
                    }
                    break;
                default:
                    break;
            }
            if (Globals.CurrentTool == (int)Enums.EdittingTool.Selection && Globals.Dragging)
            {
                DrawBoxOutline(Globals.CurTileX * Options.TileWidth + Options.TileWidth, Globals.CurTileY * Options.TileHeight + Options.TileHeight,
                        Options.TileWidth, Options.TileHeight,
                    Color.Blue, null);
            }
            else if (Globals.CurrentTool == (int)Enums.EdittingTool.Rectangle || Globals.CurrentTool == (int)Enums.EdittingTool.Selection)
            {
                DrawBoxOutline((selX + dragxoffset) * Options.TileWidth + Options.TileWidth,
                        (selY + dragyoffset) * Options.TileHeight + Options.TileHeight, (selW + 1) * Options.TileWidth, (selH + 1) * Options.TileHeight,
                    Color.White, null);
            }
            else
            {
                DrawBoxOutline(Globals.CurTileX * Options.TileWidth + Options.TileWidth,
                    Globals.CurTileY * Options.TileHeight + Options.TileHeight, Options.TileWidth, Options.TileHeight,
                    Color.White, null);
            }
        }

        private static void DrawBoxOutline(int x, int y, int w, int h, Color clr, RenderTarget2D target)
        {
            //Draw Top of Box
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x, y, w, 1), clr, target);
            //Bottom
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x, y + h, w, 1), clr, target);
            //Left
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x, y, 1, h), clr, target);
            //Right
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x + w, y, 1, h), clr, target);
        }

        public static void DrawTileset()
        {
            if (_tilesetChain == null) return;
            _graphicsDevice.SetRenderTarget(_tilesetChain);
            _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            if (Globals.CurrentTileset > -1 && TilesetTextures[Globals.CurrentTileset] != null)
            {
                DrawTexture(TilesetTextures[Globals.CurrentTileset], 0, 0, _tilesetChain);
                var selX = Globals.CurSelX;
                var selY = Globals.CurSelY;
                var selW = Globals.CurSelW;
                var selH = Globals.CurSelH;
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
                DrawBoxOutline(selX*Options.TileWidth, selY*Options.TileHeight,
                    Options.TileWidth + (selW*Options.TileWidth), Options.TileHeight + (selH*Options.TileHeight),
                    Color.White, _tilesetChain);
            }
            _tilesetChain.Present();
        }
        private static void DrawMapBorders()
        {
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, Options.TileHeight, (Options.MapWidth + 2) * Options.TileWidth, 1), null);
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, (Options.MapHeight + 2) * Options.TileHeight - Options.TileHeight, (Options.MapWidth + 2) * Options.TileWidth, 1), null);
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(Options.TileWidth, 0, 1, (Options.MapHeight + 2) * Options.TileHeight), null);
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF((Options.MapWidth + 2) * Options.TileWidth - Options.TileWidth, 0, 1, (Options.MapHeight + 2) * Options.TileHeight), null);
        }
        private static void DrawMapAttributes(int dir, bool screenShotting, RenderTarget2D renderTarget, bool upper)
        {
            if (HideResources) { return; }
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (tmpMap == null || tmpMap.Deleted == 1) { return; }
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0, xoffset = 0, yoffset = 0;
            switch (dir)
            {
                case -1:
                    if (TilePreviewStruct != null && !screenShotting)
                    {
                        tmpMap = TilePreviewStruct;
                    }
                    x1 = 0;
                    x2 = Options.MapWidth;
                    y1 = 0;
                    y2 = Options.MapHeight;
                    xoffset = Options.TileWidth;
                    yoffset = Options.TileHeight;
                    break;
                case 0:
                    if (tmpMap.Up <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Up];
                    x1 = 0;
                    x2 = Options.MapWidth;
                    y1 = Options.MapHeight - 8;
                    y2 = Options.MapHeight;
                    xoffset = Options.TileWidth;
                    yoffset = -(Options.MapHeight - 1) * Options.TileHeight;
                    break;
                case 1:
                    if (tmpMap.Down <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Down];
                    x1 = 0;
                    x2 = Options.MapWidth;
                    y1 = 0;
                    y2 = 8;
                    xoffset = Options.TileWidth;
                    yoffset = Options.TileHeight + Options.MapHeight * Options.TileHeight;
                    break;
                case 2:
                    if (tmpMap.Left <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Left];
                    x1 = Options.MapWidth - 8;
                    x2 = Options.MapWidth;
                    y1 = 0;
                    y2 = Options.MapHeight;
                    xoffset = Options.TileWidth - Options.MapWidth * Options.TileWidth;
                    yoffset = Options.TileHeight;
                    break;
                case 3:
                    if (tmpMap.Right <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Right];
                    x1 = 0;
                    x2 = 8;
                    y1 = 0;
                    y2 = Options.MapHeight;
                    xoffset = Options.TileWidth + Options.MapWidth * Options.TileWidth;
                    yoffset = Options.TileHeight;
                    break;
            }
            if (tmpMap == null || tmpMap.Deleted == 1) { return; }
            Texture2D res;
            for (var x = x1; x < x2; x++)
            {
                for (var y = y1; y < y2; y++)
                {
                    if (tmpMap.Attributes[x, y].value == (int)Enums.MapAttributes.Resource && !upper)
                    {
                        int resourcenum = tmpMap.Attributes[x, y].data1;
                        if (resourcenum >= 0 && resourcenum < Options.MaxResources)
                        {
                            if (Globals.GameResources[resourcenum].Name != "" & Globals.GameResources[resourcenum].InitialGraphic != "None")
                            {
                                if (EditorGraphics.ResourceFileNames.IndexOf(Globals.GameResources[resourcenum].InitialGraphic) > -1)
                                {
                                    res = EditorGraphics.ResourceTextures[EditorGraphics.ResourceFileNames.IndexOf(Globals.GameResources[resourcenum].InitialGraphic)];
                                    float xpos = x * Options.TileWidth + xoffset;
                                    float ypos = y * Options.TileHeight + yoffset;
                                    if (res.Width > Options.TileHeight)
                                    {
                                        ypos -= ((int)res.Height - Options.TileHeight);
                                    }
                                    if (res.Height > Options.TileWidth)
                                    {
                                        xpos -= (res.Width - 32) / 2;
                                    }
                                    DrawTexture(res, xpos, ypos,
                                    0, 0, (int)res.Width, (int)res.Height, renderTarget);
                                }
                            }
                        }
                    }
                    else if (tmpMap.Attributes[x, y].value == (int)Enums.MapAttributes.Animation)
                    {
                        int animationNum = tmpMap.Attributes[x, y].data1;


                        if (animationNum >= 0 && animationNum < Options.MaxAnimations)
                        {
                            float xpos = x * Options.TileWidth + xoffset + 16;
                            float ypos = y * Options.TileHeight + yoffset + 16;
                            var tmpMapOld = tmpMap;
                            if (tmpMap == TilePreviewStruct)
                            {
                                tmpMap = Globals.GameMaps[Globals.CurrentMap];
                            }

                            if (tmpMap.Attributes[x, y].animInstance == null)
                            {
                                tmpMap.Attributes[x, y].animInstance =
                                    new AnimationInstance(Globals.GameAnimations[animationNum], true);
                            }
                            else
                            {
                                //Update if the animation isn't right!
                                if (tmpMap.Attributes[x, y].animInstance.myBase != Globals.GameAnimations[animationNum])
                                {
                                    tmpMap.Attributes[x, y].animInstance =
                                    new AnimationInstance(Globals.GameAnimations[animationNum], true);
                                }
                            }
                            tmpMap.Attributes[x, y].animInstance.Update();
                            tmpMap.Attributes[x, y].animInstance.SetPosition((int)xpos, (int)ypos, 0);
                            tmpMap.Attributes[x, y].animInstance.Draw(renderTarget, upper);
                            tmpMap = tmpMapOld;
                        }
                    }
                }
            }


        }
        private static void DrawMapOverlay(RenderTarget2D target)
        {
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, 512, 512), Color.FromArgb(Globals.GameMaps[Globals.CurrentMap].AHue, Globals.GameMaps[Globals.CurrentMap].RHue, Globals.GameMaps[Globals.CurrentMap].GHue, Globals.GameMaps[Globals.CurrentMap].BHue), target);
        }
        public static RenderTarget2D ScreenShotMap(bool bland = false)
        {
            RenderTarget2D screenShot = CreateRenderTexture((Options.MapWidth) * Options.TileWidth, (Options.MapHeight) * Options.TileHeight);
            _graphicsDevice.SetRenderTarget(screenShot);
            _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);
            for (int i = 0; i < 2; i++)
            {
                for (int z = -1; z < 4; z++)
                {
                    DrawMap(z, true, i, screenShot);
                }
                if (i == 0)
                {
                    for (int z = -1; z < 4; z++)
                    {
                        DrawMapAttributes(z, true, screenShot, false);
                    }
                }
            }
            for (int z = -1; z < 4; z++)
            {
                DrawMapAttributes(z, true, screenShot, true);
            }
            if (!HideFog && !bland) { DrawFog(screenShot); }
            if (!HideOverlay && !bland) { DrawMapOverlay(screenShot); }
            if ((!HideDarkness || Globals.CurrentLayer == Options.LayerCount + 1) && !bland) { OverlayDarkness(screenShot, true); }
            return screenShot;
        }

        //Fogs
        private static void DrawFog(RenderTarget2D target)
        {
            float ecTime = Environment.TickCount - _fogUpdateTime;
            _fogUpdateTime = Environment.TickCount;
            if (Globals.GameMaps[Globals.CurrentMap].Fog.Length > 0)
            {
                if (FogFileNames.IndexOf(Globals.GameMaps[Globals.CurrentMap].Fog) > -1)
                {
                    int fogIndex = FogFileNames.IndexOf(Globals.GameMaps[Globals.CurrentMap].Fog);
                    int xCount = (int)(Globals.MapEditorWindow.picMap.Width / FogTextures[fogIndex].Width) + 1;
                    int yCount = (int)(Globals.MapEditorWindow.picMap.Height / FogTextures[fogIndex].Height) + 1;

                    _fogCurrentX += (ecTime / 1000f) * Globals.GameMaps[Globals.CurrentMap].FogXSpeed * 2;
                    _fogCurrentY += (ecTime / 1000f) * Globals.GameMaps[Globals.CurrentMap].FogYSpeed * 2;

                    if (_fogCurrentX < FogTextures[fogIndex].Width) { _fogCurrentX += FogTextures[fogIndex].Width; }
                    if (_fogCurrentX > FogTextures[fogIndex].Width) { _fogCurrentX -= FogTextures[fogIndex].Width; }
                    if (_fogCurrentY < FogTextures[fogIndex].Height) { _fogCurrentY += FogTextures[fogIndex].Height; }
                    if (_fogCurrentY > FogTextures[fogIndex].Height) { _fogCurrentY -= FogTextures[fogIndex].Height; }

                    for (int x = -1; x < xCount; x++)
                    {
                        for (int y = -1; y < yCount; y++)
                        {
                            DrawTexture(FogTextures[fogIndex],
                                new RectangleF(0, 0, FogTextures[fogIndex].Width, FogTextures[fogIndex].Height),
                                new RectangleF(x*FogTextures[fogIndex].Width + _fogCurrentX,
                                    y*FogTextures[fogIndex].Height + _fogCurrentY, FogTextures[fogIndex].Width,
                                    FogTextures[fogIndex].Height),
                                Color.FromArgb(Globals.GameMaps[Globals.CurrentMap].FogTransparency, 255, 255, 255),target);
                        }
                    }
                }
            }
        }

        //Lighting
        private static void ClearDarknessTexture()
        {
            if (DarknessTexture == null)
            {
                DarknessTexture = CreateRenderTexture((Options.TileWidth * Options.MapWidth * 3), (Options.TileHeight * Options.MapHeight * 3));
            }
            _graphicsDevice.SetRenderTarget(DarknessTexture);
            _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            _graphicsDevice.SetRenderTarget(null);

            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1),new RectangleF(0,0,Options.MapWidth * Options.TileWidth * 3, Options.MapHeight * Options.TileHeight * 3),
                Color.FromArgb((byte)(((float)tmpMap.Brightness / 100f) * 255f), 255, 255, 255), DarknessTexture, BlendState.Additive);

            DrawLights();
        }
        private static void OverlayDarkness(RenderTarget2D target, bool screenShotting = false)
        {
            if (DarknessTexture == null) { return; }

            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (TilePreviewStruct != null)
            {
                tmpMap = TilePreviewStruct;
            }


            DrawTexture(DarknessTexture, new RectangleF(0, 0, Options.MapWidth * Options.TileWidth * 3, Options.MapHeight * Options.TileHeight * 3),
                new RectangleF(-Options.MapWidth * Options.TileWidth, -Options.MapHeight * Options.TileHeight,
                    Options.MapWidth * Options.TileWidth * 3, Options.MapHeight * Options.TileHeight * 3),
                Color.FromArgb((byte)(((float)tmpMap.Brightness / 100f) * 255f), 255, 255, 255), target,MultiplyState);


            ////Draw Light Attribute Icons
            if (Globals.CurrentLayer != Options.LayerCount + 1) return;
            if (!screenShotting)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        if (tmpMap.FindLightAt(x, y) == null) continue;
                        DrawTexture(_lightTex, x*Options.TileWidth + Options.TileWidth,
                            y*Options.TileHeight + Options.TileHeight, 0, 0, Options.TileWidth, Options.TileHeight,
                            target);
                    }

                }
                DrawBoxOutline(Globals.CurTileX*Options.TileWidth + Options.TileWidth,
                    Globals.CurTileY*Options.TileHeight + Options.TileHeight, Options.TileWidth, Options.TileHeight,
                    Color.White, target);
            }
        }
        private static void DrawLights(RenderTarget2D target = null)
        {
            foreach (KeyValuePair<Microsoft.Xna.Framework.Point, Light> light in _lightQueue)
            {
                var x = light.Key.X;
                var y = light.Key.Y;
                DrawLight(x,y,light.Value,DarknessTexture);
            }
            _lightQueue.Clear();
        }
        public static void DrawLight(int x, int y, Light light, RenderTarget2D target)
        {
            RadialGradientShader.Parameters["_Color"].SetValue(new Vector4(light.Color.R / 255f,
                    light.Color.G / 255f, light.Color.B / 255f, light.Intensity / 255f));
            RadialGradientShader.Parameters["_Expand"].SetValue(light.Expand / 100f);
            y -= light.Size;
            x -= light.Size;
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x, y, light.Size * 2, light.Size * 2), Color.Transparent,
                target, BlendState.Additive, RadialGradientShader);
        }
        public static void AddLight(int x, int y, Light light, RenderTarget2D target = null)
        {
            if (target == null)
            {
                target = DarknessTexture;
                x += Options.TileWidth * Options.MapWidth;
                y += Options.TileHeight * Options.MapHeight;
            }
            _lightQueue.Add(new KeyValuePair<Microsoft.Xna.Framework.Point, Light>(new Microsoft.Xna.Framework.Point(x,y),light));
        }

        //Rendering
        //Rendering Functions
        public static void DrawTexture(Texture2D tex, float x, float y, RenderTarget2D RenderTarget2D)
        {
            var destRectangle = new RectangleF(x, y, (int)tex.Width, (int)tex.Height);
            var srcRectangle = new RectangleF(0, 0, (int)tex.Width, (int)tex.Height);
            DrawTexture(tex, srcRectangle, destRectangle, RenderTarget2D);
        }
        public static void DrawTexture(Texture2D tex, float x, float y, RenderTarget2D RenderTarget2D, BlendState blendMode)
        {
            var destRectangle = new RectangleF(x, y, (int)tex.Width, (int)tex.Height);
            var srcRectangle = new RectangleF(0, 0, (int)tex.Width, (int)tex.Height);
            DrawTexture(tex, srcRectangle, destRectangle, Color.White, RenderTarget2D, blendMode);
        }
        public static void DrawTexture(Texture2D tex, float dx, float dy, float sx, float sy, float w, float h, RenderTarget2D RenderTarget2D)
        {
            var destRectangle = new RectangleF(dx, dy, w, h);
            var srcRectangle = new RectangleF(sx, sy, w, h);
            DrawTexture(tex, srcRectangle, destRectangle, RenderTarget2D);
        }
        public static void DrawTexture(Texture2D tex, RectangleF srcRectangle, RectangleF targetRect, RenderTarget2D RenderTarget2D)
        {
            DrawTexture(tex, srcRectangle, targetRect, Color.White, RenderTarget2D, BlendState.AlphaBlend);
        }
        public static void DrawTexture(Texture2D tex, RectangleF srcRectangle, RectangleF targetRect, Color renderColor, RenderTarget2D renderTarget = null, BlendState blendMode = null, Effect shader = null, float rotationDegrees = 0)
        {
            if (tex == null) return;
            if (blendMode == null) blendMode = BlendState.NonPremultiplied;
            if (renderTarget == null)
            {
                StartSpritebatch(blendMode, shader, null, false, null);
                _spriteBatch.Draw(tex, null,
                    new Microsoft.Xna.Framework.Rectangle((int)targetRect.X, (int)targetRect.Y, (int)targetRect.Width,
                        (int)targetRect.Height),
                    new Microsoft.Xna.Framework.Rectangle((int)srcRectangle.X, (int)srcRectangle.Y,
                        (int)srcRectangle.Width, (int)srcRectangle.Height),
                    null, rotationDegrees, null, ConvertColor(renderColor), SpriteEffects.None, 0);
            }
            else
            {
                StartSpritebatch(blendMode, shader, renderTarget, false, null);
                _spriteBatch.Draw(tex, null,
                        new Microsoft.Xna.Framework.Rectangle((int)targetRect.X, (int)targetRect.Y, (int)targetRect.Width,
                            (int)targetRect.Height),
                        new Microsoft.Xna.Framework.Rectangle((int)srcRectangle.X, (int)srcRectangle.Y,
                            (int)srcRectangle.Width, (int)srcRectangle.Height),
                        null, rotationDegrees, null, ConvertColor(renderColor), SpriteEffects.None, 0);
            }
        }

        //Extra MonoGame Stuff
        public static Microsoft.Xna.Framework.Color ConvertColor(Color clr)
        {
            return new Microsoft.Xna.Framework.Color(new Vector4(clr.R / 255f, clr.G / 255f, clr.B / 255f, clr.A / 255f));
        }
        private static void StartSpritebatch(BlendState mode = null, Effect shader = null, RenderTarget2D target = null, bool forced = false, RasterizerState rs = null)
        {
            if (_spriteBatch.GraphicsDevice == null) return;
            bool viewsDiff = false;
            if (mode == null) mode = BlendState.NonPremultiplied;
            if (mode != _currentBlendmode || shader != _currentShader || target != _currentTarget || viewsDiff || forced || !_spriteBatchBegan)
            {
                if (_spriteBatchBegan) _spriteBatch.End();
                if (target == null)
                {
                    _graphicsDevice.SetRenderTarget(_mapEditorChain);
                }
                else
                {
                    _graphicsDevice.SetRenderTarget(target);
                }
                _spriteBatch.Begin(SpriteSortMode.Immediate, mode, null, null, rs, shader);
                _currentBlendmode = mode;
                _currentShader = shader;
                _currentTarget = target;
                _spriteBatchBegan = true;
            }
        }
        private static void EndSpriteBatch()
        {
            _spriteBatch.End();
            _spriteBatchBegan = false;
        }
    }

    internal class DummyGraphicsDeviceManager : IGraphicsDeviceService
    {
        public GraphicsDevice GraphicsDevice { get; private set; }

        // Not used:
        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public DummyGraphicsDeviceManager(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }
    }
}
