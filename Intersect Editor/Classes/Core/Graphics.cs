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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Windows.Forms;
using Intersect_Editor.Forms;
using Intersect_Editor.Properties;
using SFML.Graphics;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Image = SFML.Graphics.Image;
using SFML.System;
using View = SFML.Graphics.View;

namespace Intersect_Editor.Classes
{
    public static class Graphics
    {
        //Main render area - map window
        public static RenderWindow RenderWindow;
        //Tileset render widow
        public static RenderWindow TilesetWindow;

        //Game Graphics
        static bool _tilesetsLoaded;
        static Texture[] _tilesetTex;
        public static List<string> ItemNames;
        public static Texture[] ItemTextures;
        public static List<string> EntityFileNames;
        public static Texture[] EntityTextures;
        public static List<string> SpellFileNames;
        public static Texture[] SpellTextures;
        public static List<string> AnimationFileNames;
        public static Texture[] AnimationTextures;
        public static List<string> ImageFileNames;
        public static Texture[] ImageTextures;
        public static List<string> FogFileNames;
        public static Texture[] FogTextures;
        public static List<string> ResourceFileNames;
        public static Texture[] ResourceTextures;
        public static List<string> PaperdollFileNames;
        public static Texture[] PaperdollTextures;

        //Face Textures
        public static List<string> FaceFileNames;
        public static Texture[] FaceTextures;

        //Basic Editor Textures
        private static Texture _trans;
        //Texture for attributes
        private static Texture _attributesTex;
        //Texture for events
        private static Texture _eventTex;
        //Single tile texture for light placement
        private static Texture _lightTex;
        //Texture for NPC Spawns
        private static Texture _spawnTex;

        //Light Stuff
        public static byte CurrentBrightness = 100;
        public static bool HideDarkness = false;
        public static RenderTexture DarknessTexture;
        private static Shader RadialGradientShader;
        private static string LightingFragmentShader = @"
            uniform vec4 color;
            uniform float expand;
            uniform vec2 center;
            uniform float radius;
            uniform float windowHeight;
            void main()
            {
	            vec2 centerFromSfml = vec2(center.x, windowHeight - center.y);
	            vec2 p = (gl_FragCoord.xy - centerFromSfml) / radius;
                float r = sqrt(dot(p, p));
                if (r < 1.0)
                {
                    gl_FragColor = mix(color, gl_Color, (r - expand) / (1 - expand));
                }
                else
                {
                    gl_FragColor = gl_Color;
                }
            }";

        private static string LightingVertexShader = @"
            void main()
            {
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;
                gl_FrontColor = gl_Color;
            }";

        //Overlay Stuff
        public static Color OverlayColor = Color.Transparent;
        public static bool HideOverlay = false;
        public static RenderTexture OverlayTexture = new RenderTexture((uint)Globals.MapWidth * (uint)Globals.TileWidth * 3, (uint)Globals.MapHeight * (uint)Globals.TileHeight * 3);

        //Fog Stuff
        public static bool HideFog = false;
        private static long _fogUpdateTime = Environment.TickCount;
        private static float _fogCurrentX = 0;
        private static float _fogCurrentY = 0;

        //Resources
        public static bool HideResources = false;

        //Advanced Editing Features
        public static bool HideTilePreview = false;
        public static bool TilePreviewUpdated = false;
        public static MapStruct TilePreviewStruct;

        //Rendering Variables
        private static RenderStates _renderState = new RenderStates(BlendMode.Alpha);
        private static Vertex[] _vertexCache = new Vertex[1024];
        private static int _vertexCount = 0;
        private static Texture _curTexture;
        public static int DrawCalls = 0;
        public static int CacheLimit = 0;

        //Setup and Loading
        public static void InitSfml(frmMain myForm)
        {
            try
            {
                Stream s = new MemoryStream();

                RenderWindow = new RenderWindow(Globals.MapEditorWindow.picMap.Handle); // creates our SFML RenderWindow on our surface control
                TilesetWindow = new RenderWindow(Globals.MapLayersWindow.picTileset.Handle);
                Resources.trans.Save(s, ImageFormat.Png);
                _trans = new Texture(s);
                s.Dispose();
                s = new MemoryStream();
                Resources.attributes.Save(s, ImageFormat.Png);
                _attributesTex = new Texture(s);
                s.Dispose();
                s = new MemoryStream();
                Resources.jcl.Save(s, ImageFormat.Png);
                _lightTex = new Texture(s);
                s.Dispose();
                s = new MemoryStream();
                Resources.MapSpawn.Save(s, ImageFormat.Png);
                _spawnTex = new Texture(s);
                s.Dispose();
                s = new MemoryStream();
                Resources.jce.Save(s, ImageFormat.Png);
                _eventTex = new Texture(s);
                s.Dispose();
                LoadGraphics(myForm);
                Audio.LoadAudio();

                //Load Lighting Shader
                if (Shader.IsAvailable)
                {
                    RadialGradientShader = Shader.FromString(LightingVertexShader, LightingFragmentShader);
                }
                else
                {
                    MessageBox.Show(@"This machine is not able to load SFML Shaders and cannot run this application.");
                    Application.Exit();
                }

            }
            catch (Exception)
            {
                // ignored
            }
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

                _tilesetTex = new Texture[Globals.Tilesets.Length];
                for (var i = 0; i < Globals.Tilesets.Length; i++)
                {
                    if (File.Exists("Resources/Tilesets/" + Globals.Tilesets[i]))
                    {
                        _tilesetTex[i] = new Texture(new Image("Resources/Tilesets/" + Globals.Tilesets[i]));
                    }
                }
                _tilesetsLoaded = true;
                InitTileset(0, myForm);
            }
        }
        private static void LoadItems()
        {
            if (!Directory.Exists("Resources/Items")) { Directory.CreateDirectory("Resources/Items"); }
            var items = Directory.GetFiles("Resources/Items", "*.png");
            Array.Sort(items, new AlphanumComparatorFast());
            ItemNames = new List<string>();
            ItemTextures = new Texture[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                ItemNames.Add(items[i].Replace("Resources/Items\\", ""));
                ItemTextures[i] = new Texture(new Image("Resources/Items/" + ItemNames[i]));
            }
        }
        private static void LoadEntities()
        {
            if (!Directory.Exists("Resources/Entities")) { Directory.CreateDirectory("Resources/Entities"); }
            var chars = Directory.GetFiles("Resources/Entities", "*.png");
            Array.Sort(chars, new AlphanumComparatorFast());
            EntityFileNames = new List<string>();
            EntityTextures = new Texture[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                EntityFileNames.Add(chars[i].Replace("Resources/Entities\\", ""));
                EntityTextures[i] = new Texture(new Image("Resources/Entities/" + EntityFileNames[i]));
            }
        }
        private static void LoadSpells()
        {
            if (!Directory.Exists("Resources/Spells")) { Directory.CreateDirectory("Resources/Spells"); }
            var spells = Directory.GetFiles("Resources/Spells", "*.png");
            Array.Sort(spells, new AlphanumComparatorFast());
            SpellFileNames = new List<string>();
            SpellTextures = new Texture[spells.Length];
            for (int i = 0; i < spells.Length; i++)
            {
                SpellFileNames.Add(spells[i].Replace("Resources/Spells\\", ""));
                SpellTextures[i] = new Texture(new Image("Resources/Spells/" + SpellFileNames[i]));
            }
        }
        private static void LoadAnimations()
        {
            if (!Directory.Exists("Resources/Animations")) { Directory.CreateDirectory("Resources/Animations"); }
            var animations = Directory.GetFiles("Resources/Animations", "*.png");
            Array.Sort(animations, new AlphanumComparatorFast());
            AnimationFileNames = new List<string>();
            AnimationTextures = new Texture[animations.Length];
            for (int i = 0; i < animations.Length; i++)
            {
                AnimationFileNames.Add(animations[i].Replace("Resources/Animations\\", ""));
                AnimationTextures[i] = new Texture(new Image("Resources/Animations/" + AnimationFileNames[i]));
            }
        }
        private static void LoadFaces()
        {
            if (!Directory.Exists("Resources/Faces")) { Directory.CreateDirectory("Resources/Faces"); }
            var faces = Directory.GetFiles("Resources/Faces", "*.png");
            Array.Sort(faces, new AlphanumComparatorFast());
            FaceFileNames = new List<string>();
            FaceTextures = new Texture[faces.Length];
            for (int i = 0; i < faces.Length; i++)
            {
                FaceFileNames.Add(faces[i].Replace("Resources/Faces\\", ""));
                FaceTextures[i] = new Texture(new Image("Resources/Faces/" + FaceFileNames[i]));
            }
        }
        private static void LoadImages()
        {
            if (!Directory.Exists("Resources/Images")) { Directory.CreateDirectory("Resources/Images"); }
            var images = Directory.GetFiles("Resources/Images", "*.png");
            Array.Sort(images, new AlphanumComparatorFast());
            ImageFileNames = new List<string>();
            ImageTextures = new Texture[images.Length];
            for (int i = 0; i < images.Length; i++)
            {
                ImageFileNames.Add(images[i].Replace("Resources/Images\\", ""));
                ImageTextures[i] = new Texture(new Image("Resources/Images/" + ImageFileNames[i]));
            }
        }
        private static void LoadFogs()
        {
            if (!Directory.Exists("Resources/Fogs")) { Directory.CreateDirectory("Resources/Fogs"); }
            var fogs = Directory.GetFiles("Resources/Fogs", "*.png");
            Array.Sort(fogs, new AlphanumComparatorFast());
            FogFileNames = new List<string>();
            FogTextures = new Texture[fogs.Length];
            for (int i = 0; i < fogs.Length; i++)
            {
                FogFileNames.Add(fogs[i].Replace("Resources/Fogs\\", ""));
                FogTextures[i] = new Texture(new Image("Resources/Fogs/" + FogFileNames[i]));
            }
        }
        private static void LoadResources()
        {
            if (!Directory.Exists("Resources/Resources")) { Directory.CreateDirectory("Resources/Resources"); }
            var resources = Directory.GetFiles("Resources/Resources", "*.png");
            Array.Sort(resources, new AlphanumComparatorFast());
            ResourceFileNames = new List<string>();
            ResourceTextures = new Texture[resources.Length];
            for (int i = 0; i < resources.Length; i++)
            {
                ResourceFileNames.Add(resources[i].Replace("Resources/Resources\\", ""));
                ResourceTextures[i] = new Texture(new Image("Resources/Resources/" + ResourceFileNames[i]));
            }
        }
        private static void LoadPaperdolls()
        {
            if (!Directory.Exists("Resources/Paperdolls")) { Directory.CreateDirectory("Resources/Paperdolls"); }
            var resources = Directory.GetFiles("Resources/Paperdolls", "*.png");
            Array.Sort(resources, new AlphanumComparatorFast());
            PaperdollFileNames = new List<string>();
            PaperdollTextures = new Texture[resources.Length];
            for (int i = 0; i < resources.Length; i++)
            {
                PaperdollFileNames.Add(resources[i].Replace("Resources/Paperdolls\\", ""));
                PaperdollTextures[i] = new Texture(new Image("Resources/Paperdolls/" + PaperdollFileNames[i]));
            }
        }

        //Rendering
        public static void Render()
        {
            RenderWindow.DispatchEvents(); // handle SFML events - NOTE this is still required when SFML is hosted in another window
            RenderWindow.Clear(Color.Black); // clear our SFML RenderWindow
            TilesetWindow.DispatchEvents();
            TilesetWindow.Clear(Color.Black);
            ClearDarknessTexture();
            DrawTileset();
            

            //Draw Current Map
            if (Globals.MapEditorWindow.picMap.Visible && Globals.CurrentMap > -1 && Globals.GameMaps[Globals.CurrentMap] != null)
            {
                DrawTransparentBorders();
                for (int i = 0; i < 2; i++)
                {
                    for (int z = -1; z < 4; z++)
                    {
                        DrawMap(z, false, i, RenderWindow);
                    }
                    if (i == 0)
                    {
                        for (int z = -1; z < 4; z++)
                        {
                            DrawMapResources(z, false, RenderWindow);
                        }
                    }
                }
                if (_vertexCount > 0)
                {
                    RenderWindow.Draw(_vertexCache, 0, (uint)_vertexCount, PrimitiveType.Quads, _renderState);
                    RenderWindow.ResetGLStates();
                    _vertexCount = 0;
                }
                if (!HideFog) { DrawFog(RenderWindow); }
                if (!HideOverlay) { DrawMapOverlay(RenderWindow); }
                if (!HideDarkness || Globals.CurrentLayer == Constants.LayerCount + 1) { OverlayDarkness(RenderWindow); }

                DrawMapBorders();
                DrawSelectionRect();
            }
            Globals.MainForm.toolStripLabelDebug.Text = @"Draw Calls: " + DrawCalls + @" Cache Limit: " + CacheLimit;
            DrawCalls = 0;
            CacheLimit = 0;
            RenderCurrentBatch();
            RenderWindow.Display(); // display what SFML has drawn to the screen
            TilesetWindow.Display();
        }
        private static void DrawTransparentBorders()
        {
            var tmpSprite = new Sprite(_trans);
            RenderWindow.Draw(tmpSprite);
            for (int x = 0; x < Globals.MapWidth + 2; x++)
            {
                tmpSprite.Position = new Vector2f(Globals.TileWidth * x, 0);
                RenderWindow.Draw(tmpSprite);
                tmpSprite.Position = new Vector2f(Globals.TileWidth * x, Globals.TileHeight * (Globals.MapHeight + 1));
                RenderWindow.Draw(tmpSprite);
            }

            for (int y = 1; y < Globals.MapHeight + 1; y++)
            {
                tmpSprite.Position = new Vector2f(0, Globals.TileHeight * y);
                RenderWindow.Draw(tmpSprite);
                tmpSprite.Position = new Vector2f(Globals.TileWidth * (Globals.MapWidth + 1), Globals.TileHeight * y);
                RenderWindow.Draw(tmpSprite);
            }
        }
        private static void DrawAutoTile(int layerNum, int destX, int destY, int quarterNum, int x, int y, MapStruct map, RenderTarget target)
        {
            int yOffset = 0, xOffset = 0;

            // calculate the offset
            switch (map.Layers[layerNum].Tiles[x, y].Autotile)
            {
                case Constants.AutotileWaterfall:
                    yOffset = (Globals.WaterfallFrame - 1) * Globals.TileHeight;
                    break;
                case Constants.AutotileAnim:
                    xOffset = Globals.AutotileFrame * 64;
                    break;
                case Constants.AutotileCliff:
                    yOffset = -Globals.TileHeight;
                    break;
            }
            RenderTexture(_tilesetTex[map.Layers[layerNum].Tiles[x, y].TilesetIndex],
                                destX, destY,
                                (int)map.Autotiles.Autotile[x, y].Layer[layerNum].SrcX[quarterNum] + xOffset,
                                (int)map.Autotiles.Autotile[x, y].Layer[layerNum].SrcY[quarterNum] + yOffset,
                                16, 16, target);

        }
        private static void DrawMap(int dir, bool screenShotting, int layer, RenderTarget renderTarget)
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (tmpMap == null || tmpMap.Deleted == 1) { return; }
            RectangleShape tileSelectionRect;
            Sprite tmpSprite;
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
                    x2 = Globals.MapWidth;
                    y1 = 0;
                    y2 = Globals.MapHeight;
                    xoffset = Globals.TileWidth;
                    yoffset = Globals.TileHeight;
                    break;
                case 0:
                    if (tmpMap.Up <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Up];
                    x1 = 0;
                    x2 = Globals.MapWidth;
                    y1 = Globals.MapHeight - 1;
                    y2 = Globals.MapHeight;
                    xoffset = Globals.TileWidth;
                    yoffset = -(Globals.MapHeight - 1) * Globals.TileHeight;
                    break;
                case 1:
                    if (tmpMap.Down <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Down];
                    x1 = 0;
                    x2 = Globals.MapWidth;
                    y1 = 0;
                    y2 = 1;
                    xoffset = Globals.TileWidth;
                    yoffset = Globals.TileHeight + Globals.MapHeight * Globals.TileHeight;
                    break;
                case 2:
                    if (tmpMap.Left <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Left];
                    x1 = Globals.MapWidth - 1;
                    x2 = Globals.MapWidth;
                    y1 = 0;
                    y2 = Globals.MapHeight;
                    xoffset = Globals.TileWidth - Globals.MapWidth * Globals.TileWidth;
                    yoffset = Globals.TileHeight;
                    break;
                case 3:
                    if (tmpMap.Right <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Right];
                    x1 = 0;
                    x2 = 1;
                    y1 = 0;
                    y2 = Globals.MapHeight;
                    xoffset = Globals.TileWidth + Globals.MapWidth * Globals.TileWidth;
                    yoffset = Globals.TileHeight;
                    break;
            }
            if (tmpMap == null || tmpMap.Deleted == 1) { return; }
            if (screenShotting)
            {
                xoffset -= Globals.TileWidth;
                yoffset -= Globals.TileHeight;
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
                                case Constants.LayerCount:
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
                                case Constants.LayerCount + 1:
                                    break;
                                case Constants.LayerCount + 2:
                                    break;
                                case Constants.LayerCount + 3:
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
                                                        Globals.CurTileX + x < Globals.MapWidth &&
                                                        Globals.CurTileY + y >= 0 &&
                                                        Globals.CurTileY + y < Globals.MapHeight)
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
                                                    if (x0 >= 0 && x0 < Globals.MapWidth && y0 >= 0 &&
                                                        y0 < Globals.MapHeight && x0  < selX + selW + 1 &&
                                                        y0  < selY + selH + 1)
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
                        if (_tilesetTex[tmpMap.Layers[z].Tiles[x, y].TilesetIndex] == null) continue;
                        if (tmpMap.Autotiles.Autotile[x, y].Layer[z].RenderState != Constants.RenderStateNormal)
                        {
                            if (tmpMap.Autotiles.Autotile[x, y].Layer[z].RenderState != Constants.RenderStateAutotile)
                                continue;
                            DrawAutoTile(z, x * Globals.TileWidth + xoffset, y * Globals.TileHeight + yoffset, 1, x, y, tmpMap, renderTarget);
                            DrawAutoTile(z, x * Globals.TileWidth + 16 + xoffset, y * Globals.TileHeight + yoffset, 2, x, y, tmpMap, renderTarget);
                            DrawAutoTile(z, x * Globals.TileWidth + xoffset, y * Globals.TileHeight + 16 + yoffset, 3, x, y, tmpMap, renderTarget);
                            DrawAutoTile(z, x * Globals.TileWidth + 16 + xoffset, y * Globals.TileHeight + 16 + yoffset, 4, x, y, tmpMap, renderTarget);
                        }
                        else
                        {
                            RenderTexture(_tilesetTex[tmpMap.Layers[z].Tiles[x, y].TilesetIndex],
                                x * Globals.TileWidth + xoffset, y * Globals.TileHeight + yoffset,
                                tmpMap.Layers[z].Tiles[x, y].X * Globals.TileWidth, tmpMap.Layers[z].Tiles[x, y].Y * Globals.TileHeight,
                                Globals.TileWidth, Globals.TileHeight, renderTarget);
                        }
                    }
                }
            }

            foreach (var light in tmpMap.Lights)
            {
                double w = light.Size;
                var x = xoffset + (light.TileX * Globals.TileWidth + light.OffsetX) - (int)w / 2 + 16;
                var y = yoffset + (light.TileY * Globals.TileHeight + light.OffsetY) - (int)w / 2 + 16;
                Graphics.DrawLight((int)x, (int)y, (int)w, light.Intensity,light.Expand, light.Color);
            }
        }
        private static void DrawSelectionRect()
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (tmpMap == null || tmpMap.Deleted == 1) { return; }
            RectangleShape tileSelectionRect;
            Sprite tmpSprite;
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
            x1 = 0;
            x2 = Globals.MapWidth;
            y1 = 0;
            y2 = Globals.MapHeight;
            xoffset = Globals.TileWidth;
            yoffset = Globals.TileHeight;
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
                case Constants.LayerCount:
                    //Draw attributes
                    for (int x = 0; x < Globals.MapWidth; x++)
                    {
                        for (int y = 0; y < Globals.MapHeight; y++)
                        {
                            if (tmpMap.Attributes[x, y].value > 0)
                            {
                                tmpSprite = new Sprite(_attributesTex);
                                tmpSprite.TextureRect = new IntRect(0, (tmpMap.Attributes[x, y].value - 1) * Globals.TileWidth, Globals.TileWidth, Globals.TileHeight);
                                tmpSprite.Position = new Vector2f(x * Globals.TileWidth + Globals.TileWidth, y * Globals.TileHeight + Globals.TileHeight);
                                tmpSprite.Color = new Color(255, 255, 255, 150);
                                RenderWindow.Draw(tmpSprite);
                            }
                        }

                    }
                    break;
                case Constants.LayerCount + 1:

                    break;
                case Constants.LayerCount + 2:
                    for (var x = 0; x < Globals.MapWidth; x++)
                    {
                        for (var y = 0; y < Globals.MapHeight; y++)
                        {
                            if (tmpMap.FindEventAt(x, y) == null) continue;
                            tmpSprite = new Sprite(_eventTex)
                            {
                                TextureRect = new IntRect(0, 0, Globals.TileWidth, Globals.TileHeight),
                                Position = new Vector2f(x * Globals.TileWidth + Globals.TileWidth, y * Globals.TileHeight + Globals.TileHeight)
                            };
                            RenderWindow.Draw(tmpSprite);
                        }

                    }
                    break;
                case Constants.LayerCount + 3:
                    for (int i = 0; i < tmpMap.Spawns.Count; i++)
                    {
                        if (tmpMap.Spawns[i].X >= 0 && tmpMap.Spawns[i].Y >= 0)
                        {
                            tmpSprite = new Sprite(_spawnTex);
                            tmpSprite.TextureRect = new IntRect(0, 0, Globals.TileWidth, Globals.TileHeight);
                            tmpSprite.Position = new Vector2f(tmpMap.Spawns[i].X * Globals.TileWidth + Globals.TileWidth, tmpMap.Spawns[i].Y * Globals.TileHeight + Globals.TileHeight);
                            RenderWindow.Draw(tmpSprite);
                        }
                    }
                    break;
                default:
                    break;
            }
            if (Globals.CurrentTool == (int)Enums.EdittingTool.Rectangle ||
Globals.CurrentTool == (int)Enums.EdittingTool.Selection)
            {
                tileSelectionRect = new RectangleShape(new Vector2f((selW + 1) * Globals.TileWidth, (selH + 1) * Globals.TileHeight))
                {
                    Position = new Vector2f((selX + dragxoffset) * Globals.TileWidth + Globals.TileWidth, (selY + dragyoffset) * Globals.TileHeight + Globals.TileHeight),
                    FillColor = Color.White,
                    OutlineColor = Color.White
                };
            }
            else
            {
                tileSelectionRect = new RectangleShape(new Vector2f(Globals.TileWidth, Globals.TileHeight))
                {
                    Position = new Vector2f(Globals.CurTileX * Globals.TileWidth + Globals.TileWidth, Globals.CurTileY * Globals.TileHeight + Globals.TileHeight),
                    FillColor = Color.White,
                    OutlineColor = Color.White
                };
            }
            tileSelectionRect.FillColor = Color.Transparent;
            tileSelectionRect.OutlineColor = Color.White;
            if (Globals.CurrentTool == (int)Enums.EdittingTool.Selection && Globals.Dragging)
            {
                tileSelectionRect.OutlineColor = Color.Blue;
            }
            tileSelectionRect.OutlineThickness = 1;
            RenderWindow.Draw(tileSelectionRect);

        }
        private static void DrawTileset()
        {
            //Draw Current Tileset
            if (Globals.CurrentTileset <= -1) return;
            var tmpSprite = new Sprite(_tilesetTex[Globals.CurrentTileset]);
            TilesetWindow.Draw(tmpSprite);
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
            var tileSelectionRect = new RectangleShape(new Vector2f(Globals.TileWidth + (selW * Globals.TileWidth), Globals.TileHeight + (selH * Globals.TileHeight)))
            {
                Position = new Vector2f(selX * Globals.TileWidth, selY * Globals.TileHeight),
                FillColor = Color.White
            };
            tileSelectionRect.FillColor = Color.Transparent;
            tileSelectionRect.OutlineColor = Color.White;
            tileSelectionRect.OutlineThickness = 1;
            TilesetWindow.Draw(tileSelectionRect);
        }
        private static void DrawMapBorders()
        {
            var mapBorderLine = new RectangleShape(new Vector2f((Globals.MapWidth + 2) * Globals.TileWidth, 1))
            {
                Position = new Vector2f(0, Globals.TileHeight),
                FillColor = Color.White
            };
            RenderWindow.Draw(mapBorderLine);
            mapBorderLine.Position = new Vector2f(0, (Globals.MapHeight + 2) * Globals.TileHeight - Globals.TileHeight);
            RenderWindow.Draw(mapBorderLine);
            mapBorderLine.Size = new Vector2f(1, (Globals.MapHeight + 2) * Globals.TileHeight);
            mapBorderLine.Position = new Vector2f(Globals.TileWidth, 0);
            RenderWindow.Draw(mapBorderLine);
            mapBorderLine.Position = new Vector2f((Globals.MapWidth + 2) * Globals.TileWidth - Globals.TileWidth, 0);
            RenderWindow.Draw(mapBorderLine);
            mapBorderLine.Dispose();
        }
        private static void DrawMapResources(int dir, bool screenShotting, RenderTarget renderTarget)
        {
            if (HideResources) { return; }
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (tmpMap == null || tmpMap.Deleted == 1) { return; }
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0, z1 = 0, z2 = 3, xoffset = 0, yoffset = 0;
            switch (dir)
            {
                case -1:
                    if (TilePreviewStruct != null && !screenShotting)
                    {
                        tmpMap = TilePreviewStruct;
                    }
                    x1 = 0;
                    x2 = Globals.MapWidth;
                    y1 = 0;
                    y2 = Globals.MapHeight;
                    xoffset = Globals.TileWidth;
                    yoffset = Globals.TileHeight;
                    break;
                case 0:
                    if (tmpMap.Up <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Up];
                    x1 = 0;
                    x2 = Globals.MapWidth;
                    y1 = Globals.MapHeight - 8;
                    y2 = Globals.MapHeight;
                    xoffset = Globals.TileWidth;
                    yoffset = -(Globals.MapHeight - 1) * Globals.TileHeight;
                    break;
                case 1:
                    if (tmpMap.Down <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Down];
                    x1 = 0;
                    x2 = Globals.MapWidth;
                    y1 = 0;
                    y2 = 8;
                    xoffset = Globals.TileWidth;
                    yoffset = Globals.TileHeight + Globals.MapHeight * Globals.TileHeight;
                    break;
                case 2:
                    if (tmpMap.Left <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Left];
                    x1 = Globals.MapWidth - 8;
                    x2 = Globals.MapWidth;
                    y1 = 0;
                    y2 = Globals.MapHeight;
                    xoffset = Globals.TileWidth - Globals.MapWidth * Globals.TileWidth;
                    yoffset = Globals.TileHeight;
                    break;
                case 3:
                    if (tmpMap.Right <= -1) return;
                    tmpMap = Globals.GameMaps[tmpMap.Right];
                    x1 = 0;
                    x2 = 8;
                    y1 = 0;
                    y2 = Globals.MapHeight;
                    xoffset = Globals.TileWidth + Globals.MapWidth * Globals.TileWidth;
                    yoffset = Globals.TileHeight;
                    break;
            }
            if (tmpMap == null || tmpMap.Deleted == 1) { return; }
            Texture res;
            for (var x = x1; x < x2; x++)
            {
                for (var y = y1; y < y2; y++)
                {
                    if (tmpMap.Attributes[x, y].value == (int)Enums.MapAttributes.Resource)
                    {
                        int resourcenum = tmpMap.Attributes[x, y].data1;
                        if (resourcenum >= 0 && resourcenum < Constants.MaxResources)
                        {
                            if (Globals.GameResources[resourcenum].Name != "" & Globals.GameResources[resourcenum].InitialGraphic != "None")
                            {
                                if (Graphics.ResourceFileNames.IndexOf(Globals.GameResources[resourcenum].InitialGraphic) > -1)
                                {
                                    res = Graphics.ResourceTextures[Graphics.ResourceFileNames.IndexOf(Globals.GameResources[resourcenum].InitialGraphic)];
                                    float xpos = x * Globals.TileWidth + xoffset;
                                    float ypos = y * Globals.TileHeight + yoffset;
                                    if (res.Size.Y > Globals.TileHeight)
                                    {
                                        ypos -= ((int)res.Size.Y - Globals.TileHeight);
                                    }
                                    if (res.Size.X > Globals.TileWidth)
                                    {
                                        xpos -= (res.Size.X - 32) / 2;
                                    }
                                    RenderTexture(res, xpos, ypos,
                                    0, 0, (int)res.Size.X, (int)res.Size.Y, renderTarget);
                                }
                            }
                        }
                    };
                }
            }


        }
        private static void DrawMapOverlay(RenderTarget target)
        {
            if (OverlayColor.A != Globals.GameMaps[Globals.CurrentMap].AHue || OverlayColor.R != Globals.GameMaps[Globals.CurrentMap].RHue || OverlayColor.G != Globals.GameMaps[Globals.CurrentMap].GHue || OverlayColor.B != Globals.GameMaps[Globals.CurrentMap].BHue)
            {
                OverlayColor = new Color((byte)Globals.GameMaps[Globals.CurrentMap].RHue, (byte)Globals.GameMaps[Globals.CurrentMap].GHue, (byte)Globals.GameMaps[Globals.CurrentMap].BHue, (byte)(Globals.GameMaps[Globals.CurrentMap].AHue));
                OverlayTexture.Clear(OverlayColor);
                OverlayTexture.Display();
            }
            var overlaySprite = new Sprite(OverlayTexture.Texture) { Position = new Vector2f(-Globals.TileWidth * 30, -Globals.TileHeight * 30) };
            target.Draw(overlaySprite);
            overlaySprite.Dispose();
        }
        public static Image ScreenShotMap(bool bland = false)
        {
            RenderTexture screenShot = new RenderTexture((uint)((Globals.MapWidth) * Globals.TileWidth), (uint)((Globals.MapHeight) * Globals.TileHeight));
            screenShot.Clear(Color.Transparent);
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
                        DrawMapResources(z, true, screenShot);
                    }
                }
            }
            if (!HideFog && !bland) { DrawFog(screenShot); }
            if (!HideOverlay && !bland) { DrawMapOverlay(screenShot); }
            if ((!HideDarkness || Globals.CurrentLayer == Constants.LayerCount + 1) && !bland) { OverlayDarkness(screenShot, true); }
            screenShot.Display();
            Image img = screenShot.Texture.CopyToImage();
            screenShot.Dispose();
            return img;
        }

        //Fogs
        private static void DrawFog(RenderTarget target)
        {
            float ecTime = Environment.TickCount - _fogUpdateTime;
            _fogUpdateTime = Environment.TickCount;
            if (Globals.GameMaps[Globals.CurrentMap].Fog.Length > 0)
            {
                if (FogFileNames.IndexOf(Globals.GameMaps[Globals.CurrentMap].Fog) > -1)
                {
                    int fogIndex = FogFileNames.IndexOf(Globals.GameMaps[Globals.CurrentMap].Fog);
                    int xCount = (int)(RenderWindow.Size.X / FogTextures[fogIndex].Size.X) + 1;
                    int yCount = (int)(RenderWindow.Size.Y / FogTextures[fogIndex].Size.Y) + 1;

                    _fogCurrentX += (ecTime / 1000f) * Globals.GameMaps[Globals.CurrentMap].FogXSpeed * 2;
                    _fogCurrentY += (ecTime / 1000f) * Globals.GameMaps[Globals.CurrentMap].FogYSpeed * 2;

                    if (_fogCurrentX < FogTextures[fogIndex].Size.X) { _fogCurrentX += FogTextures[fogIndex].Size.X; }
                    if (_fogCurrentX > FogTextures[fogIndex].Size.X) { _fogCurrentX -= FogTextures[fogIndex].Size.X; }
                    if (_fogCurrentY < FogTextures[fogIndex].Size.Y) { _fogCurrentY += FogTextures[fogIndex].Size.Y; }
                    if (_fogCurrentY > FogTextures[fogIndex].Size.Y) { _fogCurrentY -= FogTextures[fogIndex].Size.Y; }

                    for (int x = -1; x < xCount; x++)
                    {
                        for (int y = -1; y < yCount; y++)
                        {
                            var fogSprite = new Sprite(FogTextures[fogIndex]) { Position = new Vector2f(x * FogTextures[fogIndex].Size.X + _fogCurrentX, y * FogTextures[fogIndex].Size.Y + _fogCurrentY) };
                            fogSprite.Color = new Color(255, 255, 255, (byte)Globals.GameMaps[Globals.CurrentMap].FogTransparency);
                            target.Draw(fogSprite);
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
                DarknessTexture = new RenderTexture((uint)(Globals.TileWidth * Globals.MapWidth * 3),(uint)(Globals.TileHeight * Globals.MapHeight * 3));
            }
            DarknessTexture.Clear(Color.Black);
        }
        private static void OverlayDarkness(RenderTarget target, bool screenShotting = false)
        {
            if (DarknessTexture == null) { return; }
            var rs = new RectangleShape(new Vector2f(DarknessTexture.Size.X, DarknessTexture.Size.Y));

            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (TilePreviewStruct != null)
            {
                tmpMap = TilePreviewStruct;
            }
            rs.FillColor = new Color(255, 255, 255, (byte)(((float)tmpMap.Brightness/100f) * 255f));    //Draw a rectangle, the opacity indicates if it is day or night.
            DarknessTexture.Draw(rs);
            DarknessTexture.Display();
            RenderCurrentBatch();
            RenderTexture(DarknessTexture.Texture,-Globals.MapWidth * Globals.TileWidth,-Globals.MapHeight * Globals.TileHeight,target,BlendMode.Multiply);
            RenderCurrentBatch();

            //Draw Light Attribute Icons
            if (Globals.CurrentLayer != Constants.LayerCount + 1) return;
            if (!screenShotting)
            {
                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        if (tmpMap.FindLightAt(x, y) == null) continue;
                        var tmpSprite = new Sprite(_lightTex)
                        {
                            TextureRect = new IntRect(0, 0, Globals.TileWidth, Globals.TileHeight),
                            Position = new Vector2f(x * Globals.TileWidth + Globals.TileWidth, y * Globals.TileHeight + Globals.TileHeight)
                        };
                        target.Draw(tmpSprite);
                    }

                }
                Shape tileSelectionRect = new RectangleShape(new Vector2f(Globals.TileWidth, Globals.TileHeight));
                tileSelectionRect.Position = new Vector2f(Globals.CurTileX * Globals.TileWidth + Globals.TileWidth, Globals.CurTileY * Globals.TileHeight + Globals.TileHeight);
                tileSelectionRect.FillColor = Color.White;
                tileSelectionRect.FillColor = Color.Transparent;
                tileSelectionRect.OutlineColor = Color.White;
                tileSelectionRect.OutlineThickness = 1;
                target.Draw(tileSelectionRect);
            }
        }
        public static void DrawLight(int x, int y, int size, byte intensity, float expand, System.Drawing.Color color, RenderTarget target = null)
        {
            if (target == null)
            {
                target = DarknessTexture;
                x += Globals.TileWidth * Globals.MapWidth;
                y += Globals.TileHeight * Globals.MapHeight;
            }
            RectangleShape rect = new RectangleShape(new Vector2f(size* 2,size* 2));
            rect.Origin = new Vector2f(size, size);
            rect.Position = new Vector2f(x + size / 2,
                y + size / 2);
            rect.FillColor = Color.Transparent;
            RadialGradientShader.SetParameter("color", new Color(color.R, color.G, color.B, intensity));
            RadialGradientShader.SetParameter("center", rect.Position);
            RadialGradientShader.SetParameter("radius", size);
            RadialGradientShader.SetParameter("expand", expand/100f);
            RadialGradientShader.SetParameter("windowHeight", target.Size.Y); // this must be set, but only needs to be set once (or whenever the size of the window changes)
            target.Draw(rect, new RenderStates(BlendMode.Add, Transform.Identity, null, RadialGradientShader));
        }
        private static Texture TexFromBitmap(Bitmap bmp)
        {
            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return new Texture(ms);
        }

        //Extra
        public static void InitTileset(int index, frmMain myForm)
        {
            if (!_tilesetsLoaded) { return; }
            Globals.CurrentTileset = index;
            Globals.CurSelX = 0;
            Globals.CurSelY = 0;
            Globals.MapLayersWindow.picTileset.Width = (int)_tilesetTex[index].Size.X;
            Globals.MapLayersWindow.picTileset.Height = (int)_tilesetTex[index].Size.Y;
            TilesetWindow.SetView(new View(new Vector2f(Globals.MapLayersWindow.picTileset.Width / 2f, Globals.MapLayersWindow.picTileset.Height / 2f), new Vector2f(Globals.MapLayersWindow.picTileset.Width, Globals.MapLayersWindow.picTileset.Height)));
        }

        //Rendering
        //Rendering Functions
        public static void RenderTexture(Texture tex, float x, float y, RenderTarget renderTarget)
        {
            var destRectangle = new RectangleF(x, y, (int)tex.Size.X, (int)tex.Size.Y);
            var srcRectangle = new RectangleF(0, 0, (int)tex.Size.X, (int)tex.Size.Y);
            RenderTexture(tex, srcRectangle, destRectangle, renderTarget);
        }
        public static void RenderTexture(Texture tex, float x, float y, RenderTarget renderTarget, BlendMode blendMode)
        {
            var destRectangle = new RectangleF(x, y, (int)tex.Size.X, (int)tex.Size.Y);
            var srcRectangle = new RectangleF(0, 0, (int)tex.Size.X, (int)tex.Size.Y);
            RenderTexture(tex, srcRectangle, destRectangle, renderTarget, blendMode);
        }
        public static void RenderTexture(Texture tex, float dx, float dy, float sx, float sy, float w, float h, RenderTarget renderTarget)
        {
            var destRectangle = new RectangleF(dx, dy, w, h);
            var srcRectangle = new RectangleF(sx, sy, w, h);
            RenderTexture(tex, srcRectangle, destRectangle, renderTarget);
        }
        public static void RenderTexture(Texture tex, RectangleF srcRectangle, RectangleF targetRect, RenderTarget renderTarget)
        {
            RenderTexture(tex, srcRectangle, targetRect, renderTarget, BlendMode.Alpha);
        }
        public static void RenderTexture(Texture tex, RectangleF srcRectangle, RectangleF targetRect, RenderTarget renderTarget, BlendMode blendMode)
        {
            var u1 = (float)srcRectangle.X / tex.Size.X;
            var v1 = (float)srcRectangle.Y / tex.Size.Y;
            var u2 = (float)srcRectangle.Right / tex.Size.X;
            var v2 = (float)srcRectangle.Bottom / tex.Size.Y;


            u1 *= tex.Size.X;
            v1 *= tex.Size.Y;
            u2 *= tex.Size.X;
            v2 *= tex.Size.Y;

            _renderState.BlendMode = blendMode;

            if (renderTarget == RenderWindow)
            {
                if (!targetRect.IntersectsWith(new RectangleF(Globals.MapEditorWindow.pnlMapContainer.HorizontalScroll.Value, Globals.MapEditorWindow.pnlMapContainer.VerticalScroll.Value, Globals.MapEditorWindow.pnlMapContainer.Width, Globals.MapEditorWindow.pnlMapContainer.Height)

                    ))
                {
                    return;
                }
                if (_renderState.Texture == null || _renderState.Texture != tex || _vertexCount >= 1024 - 4)
                {
                    // enable the new texture
                    if (_vertexCount > 0)
                    {
                        if (_vertexCount > CacheLimit)
                        {
                            CacheLimit = _vertexCount;
                        }
                        renderTarget.Draw(_vertexCache, 0, (uint)_vertexCount, PrimitiveType.Quads, _renderState);
                        DrawCalls++;
                        renderTarget.ResetGLStates();
                        _vertexCount = 0;
                    }

                    _renderState.Texture = tex;
                }


                var right = targetRect.X + targetRect.Width;
                var bottom = targetRect.Y + targetRect.Height;


                _vertexCache[_vertexCount++] = new Vertex(new Vector2f(targetRect.X, targetRect.Y), new Vector2f(u1, v1));
                _vertexCache[_vertexCount++] = new Vertex(new Vector2f(right, targetRect.Y), new Vector2f(u2, v1));
                _vertexCache[_vertexCount++] = new Vertex(new Vector2f(right, bottom), new Vector2f(u2, v2));
                _vertexCache[_vertexCount++] = new Vertex(new Vector2f(targetRect.X, bottom), new Vector2f(u1, v2));
            }
            else
            {
                if (_renderState.Texture == null || _renderState.Texture != tex)
                {
                    // enable the new texture

                    _renderState.Texture = tex;
                }
                var right = targetRect.X + targetRect.Width;
                var bottom = targetRect.Y + targetRect.Height;
                var vertexCache = new Vertex[4];
                vertexCache[0] = new Vertex(new Vector2f(targetRect.X, targetRect.Y), new Vector2f(u1, v1));
                vertexCache[1] = new Vertex(new Vector2f(right, targetRect.Y), new Vector2f(u2, v1));
                vertexCache[2] = new Vertex(new Vector2f(right, bottom), new Vector2f(u2, v2));
                vertexCache[3] = new Vertex(new Vector2f(targetRect.X, bottom), new Vector2f(u1, v2));
                DrawCalls++;
                renderTarget.Draw(vertexCache, 0, 4, PrimitiveType.Quads, _renderState);
                renderTarget.ResetGLStates();
            }


        }
        private static void RenderCurrentBatch()
        {
            if (_vertexCount > 0)
            {
                RenderWindow.Draw(_vertexCache, 0, (uint)_vertexCount, PrimitiveType.Quads, _renderState);
                DrawCalls++;
                RenderWindow.ResetGLStates();
                _vertexCount = 0;
            }
        }
    }
}
