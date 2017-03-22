
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Intersect_Editor.Classes.Core;
using Intersect_Editor.Classes.Entities;
using Intersect_Editor.Classes.General;
using Intersect_Editor.Classes.Maps;
using Intersect_Editor.Forms;
using Intersect_Library;
using Intersect_Library.Localization;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = System.Drawing.Color;
using static Intersect_Editor.Classes.Core.GameContentManager;

namespace Intersect_Editor.Classes
{
    public static class EditorGraphics
    {

        //MonoGame Setup/Device
        private static GraphicsDevice _graphicsDevice;
        private static PresentationParameters _presentationParams = new PresentationParameters();
        private static SwapChainRenderTarget _mapEditorChain;
        private static SwapChainRenderTarget _tilesetChain;
        private static SwapChainRenderTarget _mapGridChain;
        private static RenderTarget2D _whiteTex;

        //Light Stuff
        public static byte CurrentBrightness = 100;
        public static Intersect_Library.Color LightColor = null;
        public static bool HideDarkness = false;
        public static RenderTarget2D DarknessTexture;
        public static BlendState MultiplyState;
        private static List<KeyValuePair<Microsoft.Xna.Framework.Point, LightBase>> _lightQueue = new List<KeyValuePair<Microsoft.Xna.Framework.Point, LightBase>>();

        //Overlay Stuff
        public static Color OverlayColor = Color.Transparent;
        public static bool HideOverlay = false;

        //Fog Stuff
        public static bool HideFog = false;
        private static long _fogUpdateTime = Globals.System.GetTimeMs();
        private static float _fogCurrentX = 0;
        private static float _fogCurrentY = 0;

        //Resources
        public static bool HideResources = false;

        //Advanced Editing Features
        public static bool HideTilePreview = false;
        public static bool HideGrid = true;
        public static bool TilePreviewUpdated = false;
        public static MapInstance TilePreviewStruct;

        //Rendering Variables
        private static SpriteBatch _spriteBatch;
        private static bool _spriteBatchBegan;
        private static BlendState _currentBlendmode = BlendState.AlphaBlend;
        private static Effect _currentShader = null;
        private static RenderTarget2D _currentTarget = null;

        //Editor Viewing Rect
        public static System.Drawing.Rectangle CurrentView;
        public static object GraphicsLock = new object();

        //Setup and Loading
        public static void InitMonogame()
        {
            try
            {
                //Create the Graphics Device
                _presentationParams.IsFullScreen = false;
                _presentationParams.BackBufferWidth = (Options.TileWidth + 2) * Options.MapWidth;
                _presentationParams.BackBufferHeight = (Options.TileHeight + 2) * Options.MapHeight;
                _presentationParams.RenderTargetUsage = RenderTargetUsage.DiscardContents;
                _presentationParams.PresentationInterval = PresentInterval.Immediate;

                // Create device
                _graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef,
                    _presentationParams);

                //Define our spritebatch :D
                _spriteBatch = new SpriteBatch(_graphicsDevice);

                SetupWhiteTex();

                //Load the rest of the graphics and audio
                LoadEditorContent();

                //Create our multiplicative blending state.
                MultiplyState = new BlendState()
                {
                    ColorBlendFunction = BlendFunction.Add,
                    ColorSourceBlend = Blend.DestinationColor,
                    ColorDestinationBlend = Blend.Zero
                };
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

        public static void SetMapGridChain(SwapChainRenderTarget _chain)
        {
            _mapGridChain = _chain;
        }
        public static void SetMapEditorChain(SwapChainRenderTarget _chain)
        {
            _mapEditorChain = _chain;
        }
        public static void SetTilesetChain(SwapChainRenderTarget _chain)
        {
            _tilesetChain = _chain;
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
        public static RenderTarget2D CreateRenderTexture(int width, int height)
        {
            return new RenderTarget2D(_graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
        }

        //Rendering
        public static void Render()
        {
            if (_mapEditorChain != null && !_mapEditorChain.IsContentLost && !_mapEditorChain.IsDisposed)
            {
                lock (GraphicsLock)
                {
                    _graphicsDevice.SetRenderTarget(_mapEditorChain);
                    _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
                    StartSpritebatch();
                    ClearDarknessTexture(null);
                    DrawTileset();
                    DrawMapGrid();

                    //Draw Current Map
                    if (Globals.MapEditorWindow.picMap.Visible &&
                        Globals.MapEditorWindow.DockPanel.ActiveDocument == Globals.MapEditorWindow &&
                        Globals.CurrentMap != null && Globals.MapGrid.Loaded)
                    {

                        //Draw The lower maps
                        for (int y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                        {
                            for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    var map = MapInstance.GetMap(Globals.MapGrid.Grid[x, y].mapnum);
                                    if (map != null)
                                    {
                                        //Draw this map
                                        DrawMap(map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                            false,
                                            0, null);
                                    }
                                    else
                                    {
                                        DrawTransparentBorders(x - Globals.CurrentMap.MapGridX,
                                            y - Globals.CurrentMap.MapGridY);
                                    }
                                }
                                else
                                {
                                    DrawTransparentBorders(x - Globals.CurrentMap.MapGridX,
                                        y - Globals.CurrentMap.MapGridY);
                                }
                            }
                        }

                        //Draw the lower resources/animations
                        for (int y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                        {
                            for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    var map = MapInstance.GetMap(Globals.MapGrid.Grid[x, y].mapnum);
                                    if (map != null)
                                    {
                                        DrawMapAttributes(map, x - Globals.CurrentMap.MapGridX,
                                            y - Globals.CurrentMap.MapGridY, false, null, false);
                                    }
                                }
                            }
                        }

                        //Draw The upper maps
                        for (int y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                        {
                            for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    var map = MapInstance.GetMap(Globals.MapGrid.Grid[x, y].mapnum);
                                    if (map != null)
                                    {
                                        //Draw this map
                                        DrawMap(map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                            false, 1, null);
                                    }
                                }
                            }
                        }

                        //Draw the upper resources/animations
                        for (int y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                        {
                            for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    var map = MapInstance.GetMap(Globals.MapGrid.Grid[x, y].mapnum);
                                    if (map != null)
                                    {
                                        DrawMapAttributes(map, x - Globals.CurrentMap.MapGridX,
                                            y - Globals.CurrentMap.MapGridY, false, null, true);
                                    }
                                }
                            }
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
                        if (!HideGrid) DrawGridOverlay();

                        DrawMapBorders();
                        DrawSelectionRect();
                    }
                    EndSpriteBatch();
                    _mapEditorChain.Present();
                }
            }
        }

        private static void DrawGridOverlay()
        {
            for (int x = 0; x < Options.MapWidth; x++)
            {
                DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(CurrentView.Left + x * Options.TileWidth, CurrentView.Top, 1, Options.MapHeight * Options.TileHeight), null);
            }
            for (int y = 0; y < Options.MapHeight; y++)
            {
                DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(CurrentView.Left, CurrentView.Top + y * Options.TileHeight, Options.MapWidth * Options.TileWidth, 1), null);
            }
        }
        private static void DrawTransparentBorders(int gridX, int gridY)
        {
            var transTex = GetTexture(TextureType.Misc, "transtile.png");
            var xoffset = CurrentView.Left + (gridX) * (Options.TileWidth * Options.MapWidth);
            var yoffset = CurrentView.Top + (gridY) * (Options.TileHeight * Options.MapHeight);
            for (int x = 0; x < Options.MapWidth; x++)
            {
                for (int y = 0; y < Options.MapHeight; y++)
                {
                    if (new System.Drawing.Rectangle(x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, Options.TileWidth, Options.TileHeight)
                                .IntersectsWith(new System.Drawing.Rectangle(0, 0, CurrentView.Width, CurrentView.Height)))
                        DrawTexture(transTex, new RectangleF(0, 0, transTex.Width, transTex.Height), new RectangleF(xoffset + x * Options.TileWidth, yoffset + y * Options.TileHeight, Options.TileWidth, Options.TileHeight), Color.White, null);
                }
            }
        }
        private static void DrawAutoTile(int layerNum, int destX, int destY, int quarterNum, int x, int y, MapBase map, RenderTarget2D target)
        {
            int yOffset = 0, xOffset = 0;

            // calculate the offset
            switch (map.Layers[layerNum].Tiles[x, y].Autotile)
            {
                case MapAutotiles.AutotileWaterfall:
                    yOffset = (Globals.WaterfallFrame - 1) * Options.TileHeight;
                    break;
                case MapAutotiles.AutotileAnim:
                    xOffset = Globals.AutotileFrame * Options.TileWidth * 2;
                    break;
                case MapAutotiles.AutotileCliff:
                    yOffset = -Options.TileHeight;
                    break;
            }
            DrawTexture(GetTexture(TextureType.Tileset, TilesetBase.GetTileset(map.Layers[layerNum].Tiles[x, y].TilesetIndex).Name),
                                destX, destY,
                                (int)map.Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].X + xOffset,
                                (int)map.Autotiles.Autotile[x, y].Layer[layerNum].QuarterTile[quarterNum].Y + yOffset,
                                Options.TileWidth / 2, Options.TileHeight / 2, target);

        }
        private static void DrawMap(MapInstance map, int gridX, int gridY, bool screenShotting, int layer, RenderTarget2D RenderTarget2D)
        {
            var tmpMap = Globals.CurrentMap;
            if (tmpMap == null) { return; }
            int selX = Globals.CurMapSelX, selY = Globals.CurMapSelY, selW = Globals.CurMapSelW, selH = Globals.CurMapSelH;
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0, z1 = 0, z2 = 3, xoffset = 0, yoffset = 0;
            int dragxoffset = 0, dragyoffset = 0;
            if (Globals.CurrentTool == (int)EdittingTool.Rectangle ||
            Globals.CurrentTool == (int)EdittingTool.Selection)
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

            x1 = 0;
            x2 = Options.MapWidth;
            y1 = 0;
            y2 = Options.MapHeight;
            xoffset = CurrentView.Left + (gridX) * (Options.TileWidth * Options.MapWidth);
            yoffset = CurrentView.Top + (gridY) * (Options.TileHeight * Options.MapHeight);
            if (gridX != 0 || gridY != 0) tmpMap = map;
            if (tmpMap == null) { return; }
            if (screenShotting)
            {
                xoffset -= CurrentView.Left;
                yoffset -= CurrentView.Top;
            }

            if (gridX == 0 && gridY == 0)
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
                        TilePreviewStruct = new MapInstance(Globals.CurrentMap);
                        //Lets Create the Preview
                        //Mimic Mouse Down
                        tmpMap = TilePreviewStruct;
                        if (Globals.CurrentTool == (int)EdittingTool.Selection && Globals.Dragging)
                        {
                            Globals.MapEditorWindow.ProcessSelectionMovement(tmpMap, false, true);
                        }
                        else
                        {
                            if (Globals.CurrentLayer == Options.LayerCount) //Attributes
                            {
                                if (Globals.CurrentTool == (int)EdittingTool.Pen)
                                {
                                    Globals.MapLayersWindow.PlaceAttribute(tmpMap, Globals.CurTileX,
                                        Globals.CurTileY);
                                }
                                else if (Globals.CurrentTool == (int)EdittingTool.Rectangle)
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
                            }
                            else if (Globals.CurrentLayer == Options.LayerCount + 1) //Lights
                            {

                            }
                            else if (Globals.CurrentLayer == Options.LayerCount + 2) //Events
                            {

                            }
                            else if (Globals.CurrentLayer == Options.LayerCount + 3) //NPCS
                            {

                            }
                            else if (Globals.CurrentTileset != null)
                            {
                                if (Globals.CurrentTool == (int)EdittingTool.Pen)
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
                                                        Globals.CurrentTileset.Id;
                                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                        Globals.CurTileX + x, Globals.CurTileY + y].X =
                                                        Globals.CurSelX + x;
                                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                        Globals.CurTileX + x, Globals.CurTileY + y].Y =
                                                        Globals.CurSelY + y;
                                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                        Globals.CurTileX + x, Globals.CurTileY + y].Autotile = 0;
                                                    tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX + x,
                                                        Globals.CurTileY + y, Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY
                                            ].TilesetIndex = Globals.CurrentTileset.Id;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY
                                            ].X = Globals.CurSelX;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY
                                            ].Y = Globals.CurSelY;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY
                                            ].Autotile = (byte)Globals.Autotilemode;
                                        tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX, Globals.CurTileY,
                                            Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                                    }
                                }
                                else if (Globals.CurrentTool == (int)EdittingTool.Rectangle)
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
                                                            Globals.CurrentTileset.Id;
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
                                                        Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                                                }
                                            }
                                            else
                                            {
                                                if (Globals.MouseButton == 0)
                                                {
                                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].TilesetIndex =
                                                        Globals.CurrentTileset.Id;
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
                                                    Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        TilePreviewUpdated = false;
                    }
                }
                else
                {
                    tmpMap = Globals.CurrentMap;
                }
            }


            for (var x = x1; x < x2; x++)
            {
                for (var y = y1; y < y2; y++)
                {
                    for (var z = z1; z < z2; z++)
                    {
                        if (screenShotting || Globals.MapLayersWindow.LayerVisibility[z])
                        {
                            if (
                                new System.Drawing.Rectangle(x*Options.TileWidth + xoffset,
                                    y*Options.TileHeight + yoffset, Options.TileWidth, Options.TileHeight)
                                    .IntersectsWith(new System.Drawing.Rectangle(0, 0, CurrentView.Width,
                                        CurrentView.Height)))
                            {
                                if (TilesetBase.GetTileset(tmpMap.Layers[z].Tiles[x, y].TilesetIndex) == null) continue;
                                Texture2D tilesetTex = GetTexture(TextureType.Tileset,
                                    TilesetBase.GetTileset(tmpMap.Layers[z].Tiles[x, y].TilesetIndex).Name);
                                if (tilesetTex == null) continue;
                                if (tmpMap.Autotiles.Autotile[x, y].Layer[z].RenderState !=
                                    MapAutotiles.RenderStateNormal)
                                {
                                    if (tmpMap.Autotiles.Autotile[x, y].Layer[z].RenderState !=
                                        MapAutotiles.RenderStateAutotile)
                                        continue;
                                    DrawAutoTile(z, x*Options.TileWidth + xoffset, y*Options.TileHeight + yoffset, 1, x,
                                        y, tmpMap, RenderTarget2D);
                                    DrawAutoTile(z, x*Options.TileWidth + (Options.TileWidth/2) + xoffset,
                                        y*Options.TileHeight + yoffset, 2, x, y, tmpMap, RenderTarget2D);
                                    DrawAutoTile(z, x*Options.TileWidth + xoffset,
                                        y*Options.TileHeight + (Options.TileHeight/2) + yoffset, 3, x, y, tmpMap,
                                        RenderTarget2D);
                                    DrawAutoTile(z, x*Options.TileWidth + (Options.TileWidth/2) + xoffset,
                                        y*Options.TileHeight + (Options.TileHeight/2) + yoffset, 4, x, y, tmpMap,
                                        RenderTarget2D);
                                }
                                else
                                {
                                    DrawTexture(tilesetTex,
                                        x*Options.TileWidth + xoffset, y*Options.TileHeight + yoffset,
                                        tmpMap.Layers[z].Tiles[x, y].X*Options.TileWidth,
                                        tmpMap.Layers[z].Tiles[x, y].Y*Options.TileHeight,
                                        Options.TileWidth, Options.TileHeight, RenderTarget2D);
                                }
                            }
                        }
                    }
                }
            }

            if (layer == 1)
            {
                foreach (var light in tmpMap.Lights)
                {
                    double w = light.Size;
                    var x = xoffset + Options.MapWidth * Options.TileWidth + (light.TileX * Options.TileWidth + light.OffsetX) + Options.TileWidth /2;
                    var y = yoffset + Options.MapHeight * Options.TileHeight + (light.TileY * Options.TileHeight + light.OffsetY) + Options.TileHeight /2;
                    if (!HideDarkness) AddLight(x, y, light, null);
                }
            }
        }
        private static void DrawSelectionRect()
        {
            var tmpMap = Globals.CurrentMap;
            if (tmpMap == null) { return; }
            int selX = Globals.CurMapSelX, selY = Globals.CurMapSelY, selW = Globals.CurMapSelW, selH = Globals.CurMapSelH;
            int dragxoffset = 0, dragyoffset = 0;
            if (Globals.CurrentTool == (int)EdittingTool.Rectangle ||
            Globals.CurrentTool == (int)EdittingTool.Selection)
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
            if ((!HideTilePreview || Globals.Dragging))
            {
                tmpMap = TilePreviewStruct;
                if (Globals.CurrentLayer == Options.LayerCount) //Attributes
                {
                    //Draw attributes
                    for (int x = 0; x < Options.MapWidth; x++)
                    {
                        for (int y = 0; y < Options.MapHeight; y++)
                        {
                            if (tmpMap.Attributes[x, y] != null)
                            {
                                if (tmpMap.Attributes[x, y].value > 0)
                                {
                                    var attributesTex = GetTexture(TextureType.Misc, "attributes.png");
                                    if (attributesTex != null)
                                    {
                                        DrawTexture(attributesTex, new RectangleF(0, (tmpMap.Attributes[x, y].value - 1) * attributesTex.Width, attributesTex.Width, attributesTex.Width),new RectangleF(CurrentView.Left + x * Options.TileWidth, 
                                            CurrentView.Top + y * Options.TileHeight, Options.TileWidth, Options.TileHeight), Color.FromArgb(150, 255, 255, 255), null);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Globals.CurrentLayer == Options.LayerCount + 1) //Lights
                {

                }
                else if (Globals.CurrentLayer == Options.LayerCount + 2) //Events
                {
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            if (tmpMap.FindEventAt(x, y) == null) continue;
                            var eventTex = GetTexture(TextureType.Misc, "eventicon.png");
                            if (eventTex != null)
                            {
                                DrawTexture(eventTex, new RectangleF(0, 0, eventTex.Width, eventTex.Height), new RectangleF(CurrentView.Left + x * Options.TileWidth,
                                CurrentView.Top + y * Options.TileHeight, Options.TileWidth, Options.TileHeight), Color.White, null);
                            }
                        }

                    }
                }
                else if (Globals.CurrentLayer == Options.LayerCount + 3) //NPCS
                {
                    for (int i = 0; i < tmpMap.Spawns.Count; i++)
                    {
                        if (tmpMap.Spawns[i].X >= 0 && tmpMap.Spawns[i].Y >= 0)
                        {
                            var spawnTex = GetTexture(TextureType.Misc, "spawnicon.png");
                            if (spawnTex != null)
                            {
                                DrawTexture(spawnTex, new RectangleF(0,0,spawnTex.Width,spawnTex.Height),new RectangleF(CurrentView.Left + tmpMap.Spawns[i].X * Options.TileWidth,
                                CurrentView.Top + tmpMap.Spawns[i].Y * Options.TileHeight,Options.TileWidth, Options.TileHeight),Color.White, null);
                            }
                        }
                    }
                }
                else
                {

                }
            }
            if (Globals.CurrentTool == (int)EdittingTool.Selection && Globals.Dragging)
            {
                DrawBoxOutline(CurrentView.Left + Globals.CurTileX * Options.TileWidth, CurrentView.Top + Globals.CurTileY * Options.TileHeight, Options.TileWidth, Options.TileHeight,
                    Color.White, null);
            }
            if (Globals.CurrentTool == (int)EdittingTool.Rectangle || Globals.CurrentTool == (int)EdittingTool.Selection)
            {
                DrawBoxOutline(CurrentView.Left + (selX + dragxoffset) * Options.TileWidth, CurrentView.Top + (selY + dragyoffset) * Options.TileHeight, (selW + 1) * Options.TileWidth, (selH + 1) * Options.TileHeight,
                    Color.Blue, null);
            }
            else
            {
                DrawBoxOutline(CurrentView.Left + Globals.CurTileX * Options.TileWidth,
                    CurrentView.Top + Globals.CurTileY * Options.TileHeight, Options.TileWidth, Options.TileHeight,
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

        public static void DrawMapGrid()
        {
            if (_mapGridChain == null || _mapGridChain.IsContentLost || _mapGridChain.IsDisposed || Globals.MapGridWindowNew.DockPanel.ActiveDocument != Globals.MapGridWindowNew) return;
            _graphicsDevice.SetRenderTarget(_mapGridChain);
            _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.FromNonPremultiplied(60,63,65,255));
            var rand = new Random();
            var grid = Globals.MapGrid;
            grid.Update(_mapGridChain.Bounds);
            for (int x = 0; x < grid.GridWidth + 2; x++)
            {
                for (int y = 0; y < grid.GridHeight + 2; y++)
                {
                    var renderRect = new System.Drawing.Rectangle(grid.ContentRect.X + x * grid.TileWidth, grid.ContentRect.Y + y * grid.TileHeight, grid.TileWidth, grid.TileHeight);
                    if (grid.ViewRect.IntersectsWith(renderRect))
                    {
                        if (x == 0 || y == 0 || x == grid.GridWidth + 1 || y == grid.GridHeight + 1 || grid.Grid[x - 1, y - 1].mapnum == -1)
                        {
                            DrawTexture(GetWhiteTex(), new RectangleF(0, 0, 1, 1), new RectangleF(grid.ContentRect.X + x * grid.TileWidth, grid.ContentRect.Y + y * grid.TileHeight, grid.TileWidth, grid.TileHeight), Color.FromArgb(45,45,48), _mapGridChain);
                        }
                        else
                        {
                            if (grid.Grid[x - 1, y - 1].mapnum > -1)
                            {
                                if (grid.Grid[x - 1, y - 1].tex != null &&
                                    grid.Grid[x - 1, y - 1].tex.Width == grid.TileWidth &&
                                    grid.Grid[x - 1, y - 1].tex.Height == grid.TileHeight)
                                {
                                    DrawTexture(grid.Grid[x - 1, y - 1].tex, grid.ContentRect.X + x * grid.TileWidth,
                                        grid.ContentRect.Y + y * grid.TileHeight, _mapGridChain);
                                }
                                else
                                {
                                    DrawTexture(GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                        new RectangleF(grid.ContentRect.X + x * grid.TileWidth,
                                            grid.ContentRect.Y + y * grid.TileHeight, grid.TileWidth, grid.TileHeight),
                                        Color.Green, _mapGridChain);
                                }
                            }
                            else
                            {
                                DrawTexture(GetWhiteTex(), new RectangleF(0, 0, 1, 1), new RectangleF(grid.ContentRect.X + x * grid.TileWidth, grid.ContentRect.Y + y * grid.TileHeight, grid.TileWidth, grid.TileHeight), Color.Gray, _mapGridChain);
                            }
                        }
                        if (Globals.MapGrid.ShowLines)
                        {
                            DrawTexture(GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                new RectangleF(grid.ContentRect.X + x * grid.TileWidth,
                                    grid.ContentRect.Y + y * grid.TileHeight, grid.TileWidth, 1), Color.DarkGray,
                                _mapGridChain);
                            DrawTexture(GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                new RectangleF(grid.ContentRect.X + x * grid.TileWidth,
                                    grid.ContentRect.Y + y * grid.TileHeight, 1, grid.TileHeight), Color.DarkGray,
                                _mapGridChain);
                            DrawTexture(GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                new RectangleF(grid.ContentRect.X + x * grid.TileWidth + grid.TileWidth,
                                    grid.ContentRect.Y + y * grid.TileHeight, 1, grid.TileHeight), Color.DarkGray,
                                _mapGridChain);
                            DrawTexture(GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                new RectangleF(grid.ContentRect.X + x * grid.TileWidth,
                                    grid.ContentRect.Y + y * grid.TileHeight + grid.TileHeight, grid.TileWidth, 1),
                                Color.DarkGray, _mapGridChain);
                        }
                    }
                }
            }
            _mapGridChain.Present();
        }

        public static void DrawTileset()
        {
            if (_tilesetChain == null || _tilesetChain.IsContentLost || _tilesetChain.IsDisposed || Globals.MapLayersWindow.CurrentTab != LayerTabs.Tiles) return;
            _graphicsDevice.SetRenderTarget(_tilesetChain);
            _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            if (Globals.CurrentTileset != null)
            {
                Texture2D tilesetTex = GetTexture(TextureType.Tileset, Globals.CurrentTileset.Name);
                if (tilesetTex != null)
                {
                    DrawTexture(tilesetTex, 0, 0, _tilesetChain);
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
                    DrawBoxOutline(selX * Options.TileWidth, selY * Options.TileHeight,
                        Options.TileWidth + (selW * Options.TileWidth), Options.TileHeight + (selH * Options.TileHeight),
                        Color.White, _tilesetChain);
                }
            }
            _tilesetChain.Present();
        }
        private static void DrawMapBorders()
        {
            //Horizontal Top
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, CurrentView.Y - 1, CurrentView.Width, 3), Color.DimGray);
            //Horizontal Buttom
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, CurrentView.Y + (Options.TileHeight * Options.MapHeight) - 1, CurrentView.Width, 3), Color.DimGray);
            //Vertical Left
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(CurrentView.Left - 1, 0, 3, CurrentView.Height), Color.DimGray);
            //Vertical Right
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(CurrentView.Left + (Options.TileWidth * Options.MapWidth) - 1, 0, 3, CurrentView.Height), Color.DimGray);

            //Horizontal Top
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(CurrentView.Left, CurrentView.Y - 1, Options.TileWidth * Options.MapWidth, 3), Color.DimGray);
            //Horizontal Buttom
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(CurrentView.Left, CurrentView.Y + (Options.TileHeight * Options.MapHeight) - 1, Options.TileWidth * Options.MapWidth, 3), Color.DimGray);
            //Vertical Left
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(CurrentView.Left - 1, CurrentView.Y, 3, Options.MapHeight * Options.TileHeight), Color.DimGray);
            //Vertical Right
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(CurrentView.Left + (Options.TileWidth * Options.MapWidth) - 1, CurrentView.Y, 3, Options.MapHeight * Options.TileHeight), Color.DimGray);

        }
        private static void DrawMapAttributes(MapInstance map, int gridX, int gridY, bool screenShotting, RenderTarget2D renderTarget, bool upper)
        {
            if (!screenShotting && HideResources) { return; }
            if (screenShotting && Database.GridHideResources) { return; }
            var tmpMap = Globals.CurrentMap;
            if (tmpMap == null) { return; }
            int x1 = 0, y1 = 0, x2 = 0, y2 = 0, xoffset = 0, yoffset = 0;

            x1 = 0;
            x2 = Options.MapWidth;
            y1 = 0;
            y2 = Options.MapHeight;
            xoffset = CurrentView.Left + (gridX) * (Options.TileWidth * Options.MapWidth);
            yoffset = CurrentView.Top + (gridY) * (Options.TileHeight * Options.MapHeight);
            if (gridX != 0 || gridY != 0) tmpMap = map;

            if (screenShotting)
            {
                xoffset -= CurrentView.Left;
                yoffset -= CurrentView.Top;
            }

            if (gridX == 0 && gridY == 0)
            {
                if ((!HideTilePreview || Globals.Dragging) && !screenShotting)
                {
                    tmpMap = TilePreviewStruct;
                }
            }

            if (tmpMap == null) { return; }
            for (var y = y1; y < y2; y++)
            {
                for (var x = x1; x < x2; x++)
                {
                    if (tmpMap.Attributes[x, y] != null)
                    {
                        if (tmpMap.Attributes[x, y].value == (int)MapAttributes.Resource && !upper)
                        {
                            var resource = ResourceBase.GetResource(tmpMap.Attributes[x, y].data1);
                            if (resource != null)
                            {
                                if (resource.Name != "" & resource.InitialGraphic != Strings.Get("general","none"))
                                {
                                    Texture2D res = GetTexture(TextureType.Resource, resource.InitialGraphic);
                                    if (res != null)
                                    {
                                        float xpos = x * Options.TileWidth + xoffset;
                                        float ypos = y * Options.TileHeight + yoffset;
                                        if (res.Height > Options.TileHeight)
                                        {
                                            ypos -= ((int)res.Height - Options.TileHeight);
                                        }
                                        if (res.Width > Options.TileWidth)
                                        {
                                            xpos -= (res.Width - 32) / 2;
                                        }
                                        DrawTexture(res, xpos, ypos,
                                            0, 0, (int)res.Width, (int)res.Height, renderTarget);
                                    }
                                }
                            }
                        }
                        else if (tmpMap.Attributes[x, y].value == (int)MapAttributes.Animation)
                        {
                            var animation = AnimationBase.GetAnim(tmpMap.Attributes[x, y].data1);
                            if (animation != null)
                            {
                                float xpos = x * Options.TileWidth + xoffset + Options.TileWidth/2;
                                float ypos = y * Options.TileHeight + yoffset + Options.TileHeight/2;
                                var tmpMapOld = tmpMap;
                                if (tmpMap == TilePreviewStruct)
                                {
                                    tmpMap = Globals.CurrentMap;
                                }
                                if (tmpMap.Attributes[x, y] != null)
                                {

                                    var animInstance = tmpMap.GetAttributeAnimation(tmpMap.Attributes[x, y],
                                        animation.Id);
                                    //Update if the animation isn't right!
                                    if (animInstance == null || animInstance.myBase != animation)
                                    {
                                        tmpMap.SetAttributeAnimation(tmpMap.Attributes[x, y],
                                            new AnimationInstance(animation, true));
                                    }
                                    animInstance.Update();
                                    animInstance.SetPosition((int)xpos, (int)ypos, 0);
                                    animInstance.Draw(renderTarget, upper);
                                }
                                tmpMap = tmpMapOld;
                            }
                        }
                    }
                }
            }


        }
        private static void DrawMapOverlay(RenderTarget2D target)
        {
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, CurrentView.Width, CurrentView.Height), Color.FromArgb(Globals.CurrentMap.AHue, Globals.CurrentMap.RHue, Globals.CurrentMap.GHue, Globals.CurrentMap.BHue), target);
        }
        public static Bitmap ScreenShotMap()
        {
            RenderTarget2D screenShot = CreateRenderTexture((Options.MapWidth) * Options.TileWidth, (Options.MapHeight) * Options.TileHeight);
            _graphicsDevice.SetRenderTarget(screenShot);
            _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);

            if (Globals.MapGrid.Contains(((DatabaseObject) Globals.CurrentMap).Id))
            {
                //Draw The lower maps
                for (int y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                {
                    for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                    {
                        if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                        {
                            var map = MapInstance.GetMap(Globals.MapGrid.Grid[x, y].mapnum);
                            if (map != null)
                            {
                                lock (map.GetMapLock())
                                {
                                    //Draw this map
                                    DrawMap(map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY, true, 0,
                                        screenShot);
                                }
                            }
                        }
                    }
                }

                //Draw the lower resources/animations
                for (int y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                {
                    for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                    {
                        if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                        {
                            var map = MapInstance.GetMap(Globals.MapGrid.Grid[x, y].mapnum);
                            if (map != null)
                            {
                                lock (map.GetMapLock())
                                {
                                    DrawMapAttributes(map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                        true, screenShot, false);
                                }
                            }
                        }
                    }
                }

                //Draw The upper maps
                for (int y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                {
                    for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                    {
                        if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                        {
                            var map = MapInstance.GetMap(Globals.MapGrid.Grid[x, y].mapnum);
                            if (map != null)
                            {
                                lock (map.GetMapLock())
                                {
                                    //Draw this map
                                    DrawMap(map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY, true, 1,
                                        screenShot);
                                }
                            }
                        }
                    }
                }

                //Draw the upper resources/animations
                for (int y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                {
                    for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                    {
                        if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                        {
                            var map = MapInstance.GetMap(Globals.MapGrid.Grid[x, y].mapnum);
                            if (map != null)
                            {
                                lock (map.GetMapLock())
                                {
                                    DrawMapAttributes(map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                        true, screenShot, true);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                lock (Globals.CurrentMap.GetMapLock())
                {
                    //Draw this map
                    DrawMap(Globals.CurrentMap, 0,0, true, 0,
                        screenShot);
                    DrawMapAttributes(Globals.CurrentMap, 0,0,
                                        true, screenShot, false);
                    //Draw this map
                    DrawMap(Globals.CurrentMap, 0,0, true, 1,
                        screenShot);
                    DrawMapAttributes(Globals.CurrentMap, 0,0,
                                        true, screenShot, true);
                }
            }
            if (!Database.GridHideFog) { DrawFog(screenShot); }
            if (!Database.GridHideOverlay) { DrawMapOverlay(screenShot); }
            if ((!Database.GridHideDarkness || Globals.CurrentLayer == Options.LayerCount + 1))
            {
                ClearDarknessTexture(screenShot, true);
                OverlayDarkness(screenShot, true);
            }
            int[] data = new int[screenShot.Width * screenShot.Height];
            screenShot.GetData(0, new Microsoft.Xna.Framework.Rectangle(0, 0, screenShot.Width, screenShot.Height), data, 0, screenShot.Width * screenShot.Height);
            Bitmap bitmap = new Bitmap(screenShot.Width, screenShot.Height);
            var bits = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe
            {
                byte* dstPointer = (byte*)bits.Scan0;
                for (int y = 0; y < screenShot.Height; ++y)
                {
                    for (int x = 0; x < screenShot.Width; ++x)
                    {
                        Color bitmapColor =
                            Color.FromArgb(data[y * screenShot.Width + x]);
                        dstPointer[0] = bitmapColor.R;
                        dstPointer[1] = bitmapColor.G;
                        dstPointer[2] = bitmapColor.B;
                        dstPointer[3] = bitmapColor.A;

                        dstPointer += 4;
                    }
                }
            }
            bitmap.UnlockBits(bits);


            return bitmap;
        }

        //Fogs
        private static void DrawFog(RenderTarget2D target)
        {
            float ecTime = Globals.System.GetTimeMs() - _fogUpdateTime;
            _fogUpdateTime = Globals.System.GetTimeMs();
            if (Globals.CurrentMap.Fog.Length > 0)
            {
                Texture2D fogTex = GetTexture(TextureType.Fog, Globals.CurrentMap.Fog);
                if (fogTex != null)
                {
                    int xCount = (int)(Globals.MapEditorWindow.picMap.Width / fogTex.Width) + 1;
                    int yCount = (int)(Globals.MapEditorWindow.picMap.Height / fogTex.Height) + 1;

                    _fogCurrentX += (ecTime / 1000f) * Globals.CurrentMap.FogXSpeed * 2;
                    _fogCurrentY += (ecTime / 1000f) * Globals.CurrentMap.FogYSpeed * 2;

                    if (_fogCurrentX < fogTex.Width) { _fogCurrentX += fogTex.Width; }
                    if (_fogCurrentX > fogTex.Width) { _fogCurrentX -= fogTex.Width; }
                    if (_fogCurrentY < fogTex.Height) { _fogCurrentY += fogTex.Height; }
                    if (_fogCurrentY > fogTex.Height) { _fogCurrentY -= fogTex.Height; }

                    var drawX = (float)Math.Round(_fogCurrentX);
                    var drawY = (float)Math.Round(_fogCurrentY);

                    for (int x = -1; x < xCount; x++)
                    {
                        for (int y = -1; y < yCount; y++)
                        {
                            DrawTexture(fogTex,
                                new RectangleF(0, 0, fogTex.Width, fogTex.Height),
                                new RectangleF(x * fogTex.Width + drawX,
                                    y * fogTex.Height + drawY, fogTex.Width,
                                    fogTex.Height),
                                Color.FromArgb(Globals.CurrentMap.FogTransparency, 255, 255, 255), target);
                        }
                    }
                }
            }
        }

        //Lighting
        private static void ClearDarknessTexture(RenderTarget2D target, bool screenShotting = false)
        {
            if (DarknessTexture == null)
            {
                DarknessTexture = CreateRenderTexture(Options.TileWidth * Options.MapWidth * 3, Options.TileHeight * Options.MapHeight * 3);
            }
            _graphicsDevice.SetRenderTarget(DarknessTexture);
            _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            _graphicsDevice.SetRenderTarget(null);

            if (Globals.CurrentMap == null) return;
            var tmpMap = Globals.CurrentMap;
            if (tmpMap == null) return;
            if (screenShotting)
            {
                Color gridLightColor = Color.FromArgb(Database.GridLightColor);
                if (!tmpMap.IsIndoors && gridLightColor != null)
                {
                    DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
    Color.FromArgb(255, 255, 255, 255), DarknessTexture, BlendState.Additive);
                    DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
        Color.FromArgb(gridLightColor.A, gridLightColor.R, gridLightColor.G, gridLightColor.B), DarknessTexture, BlendState.NonPremultiplied);
                }
                else
                {
                    DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
        Color.FromArgb((byte)(((float)tmpMap.Brightness / 100f) * 255f), 255, 255, 255), DarknessTexture, BlendState.Additive);
                }
            }
            else
            {
                if (!tmpMap.IsIndoors && LightColor != null)
                {
                    DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
    Color.FromArgb(255, 255, 255, 255), DarknessTexture, BlendState.Additive);
                    DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
        Color.FromArgb(LightColor.A, LightColor.R, LightColor.G, LightColor.B), DarknessTexture, BlendState.NonPremultiplied);
                }
                else if (tmpMap.IsIndoors)
                {
                    DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1),
                        new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
                        Color.FromArgb((byte)(((float)tmpMap.Brightness / 100f) * 255f), 255, 255, 255), DarknessTexture,
                        BlendState.Additive);
                }
                else
                {
                    DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1),
                        new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
                        Color.FromArgb(255, 255, 255, 255), DarknessTexture,
                        BlendState.Additive);
                }
            }


            DrawLights(target);
        }
        private static void OverlayDarkness(RenderTarget2D target, bool screenShotting = false)
        {
            if (DarknessTexture == null) { return; }

            var tmpMap = Globals.CurrentMap;
            if (TilePreviewStruct != null)
            {
                tmpMap = TilePreviewStruct;
            }


            DrawTexture(DarknessTexture, new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),new RectangleF(-Options.MapWidth * Options.TileWidth, -Options.MapHeight * Options.TileHeight,DarknessTexture.Width, DarknessTexture.Height),Color.FromArgb(255, 255, 255, 255), target, MultiplyState);

            ////Draw Light Attribute Icons
            if (Globals.CurrentLayer != Options.LayerCount + 1) return;
            if (!screenShotting)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        if (tmpMap.FindLightAt(x, y) == null) continue;
                        var lightTex = GetTexture(TextureType.Misc, "lighticon.png");
                        if (lightTex != null)
                        {
                            DrawTexture(lightTex, new RectangleF(0,0,lightTex.Width,lightTex.Height),new RectangleF(CurrentView.Left + x * Options.TileWidth,
                            CurrentView.Top + y * Options.TileHeight, Options.TileWidth,Options.TileHeight),Color.White, target);
                        }
                    }

                }
                DrawBoxOutline(CurrentView.Left + Globals.CurTileX * Options.TileWidth,
                    CurrentView.Top + Globals.CurTileY * Options.TileHeight, Options.TileWidth, Options.TileHeight,
                    Color.White, target);
            }
        }
        private static void DrawLights(RenderTarget2D target = null)
        {
            foreach (KeyValuePair<Microsoft.Xna.Framework.Point, LightBase> light in _lightQueue)
            {
                var x = light.Key.X;
                var y = light.Key.Y;
                DrawLight(x, y, light.Value, DarknessTexture);
            }
            _lightQueue.Clear();
        }
        public static void DrawLight(int x, int y, LightBase light, RenderTarget2D target)
        {
            Effect shader = GetShader("radialgradient_editor.xnb");
            var vec = new Vector4(light.Color.R / 255f,
                light.Color.G / 255f, light.Color.B / 255f, light.Intensity / 255f);
            shader.Parameters["LightColor"].SetValue(vec);
            shader.Parameters["Expand"].SetValue((float)(light.Expand / 100f));
            y -= light.Size;
            x -= light.Size;
            DrawTexture(_whiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x, y, light.Size * 2, light.Size * 2), Color.White,
                target, BlendState.Additive, shader);
        }
        public static void AddLight(int x, int y, LightBase light, RenderTarget2D target = null)
        {
            if (target == null)
            {
                target = DarknessTexture;
            }
            _lightQueue.Add(new KeyValuePair<Microsoft.Xna.Framework.Point, LightBase>(new Microsoft.Xna.Framework.Point(x, y), light));
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
}
