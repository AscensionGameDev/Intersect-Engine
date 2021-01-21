using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Intersect.Config;
using Intersect.Editor.Content;
using Intersect.Editor.Entities;
using Intersect.Editor.Forms.DockingElements;
using Intersect.Editor.General;
using Intersect.Editor.Maps;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Utilities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect.Editor.Core
{

    public static class Graphics
    {

        //Light Stuff
        public static byte CurrentBrightness = 100;

        //Editor Viewing Rect
        public static System.Drawing.Rectangle CurrentView;

        public static RenderTarget2D DarknessTexture;

        public static object GraphicsLock = new object();

        public static bool HideDarkness = false;

        //Fog Stuff
        public static bool HideFog = false;

        public static bool HideGrid = true;

        public static bool HideOverlay = false;

        //Resources
        public static bool HideResources = false;

        // Events
        public static bool HideEvents = false;

        //Advanced Editing Features
        public static bool HideTilePreview = false;

        public static Color LightColor = null;

        public static BlendState MultiplyState;

        //Overlay Stuff
        public static System.Drawing.Color OverlayColor = System.Drawing.Color.Transparent;

        private static BlendState sCurrentBlendmode = BlendState.NonPremultiplied;

        private static Effect sCurrentShader;

        private static RenderTarget2D sCurrentTarget;

        private static float sFogCurrentX;

        private static float sFogCurrentY;

        private static long sFogUpdateTime = Globals.System.GetTimeMs();

        //MonoGame Setup/Device
        private static GraphicsDevice sGraphicsDevice;

        private static List<KeyValuePair<Microsoft.Xna.Framework.Point, LightBase>> sLightQueue =
            new List<KeyValuePair<Microsoft.Xna.Framework.Point, LightBase>>();

        private static SwapChainRenderTarget sMapEditorChain;

        private static SwapChainRenderTarget sMapGridChain;

        private static PresentationParameters sPresentationParams = new PresentationParameters();

        //Screenshot RT
        private static RenderTarget2D sScreenShotRenderTexture;

        //Rendering Variables
        private static SpriteBatch sSpriteBatch;

        private static bool sSpriteBatchBegan;

        private static SwapChainRenderTarget sTilesetChain;

        private static RenderTarget2D sWhiteTex;

        public static MapInstance TilePreviewStruct;

        public static bool TilePreviewUpdated;

        //Setup and Loading
        public static void InitMonogame()
        {
            try
            {
                //Create the Graphics Device
                sPresentationParams.IsFullScreen = false;
                sPresentationParams.BackBufferWidth = (Options.TileWidth + 2) * Options.MapWidth;
                sPresentationParams.BackBufferHeight = (Options.TileHeight + 2) * Options.MapHeight;
                sPresentationParams.RenderTargetUsage = RenderTargetUsage.DiscardContents;
                sPresentationParams.PresentationInterval = PresentInterval.Immediate;

                // Create device
                sGraphicsDevice = new GraphicsDevice(
                    GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, sPresentationParams
                );

                //Define our spritebatch :D
                sSpriteBatch = new SpriteBatch(sGraphicsDevice);

                SetupWhiteTex();

                //Load the rest of the graphics and audio
                GameContentManager.LoadEditorContent();

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
                MessageBox.Show("Failed to initialize MonoGame. Exception Info: " + ex + "\nClosing Now");
                Application.Exit();
            }
        }

        public static GraphicsDevice GetGraphicsDevice()
        {
            return sGraphicsDevice;
        }

        public static void SetMapGridChain(SwapChainRenderTarget chain)
        {
            sMapGridChain = chain;
        }

        public static void SetMapEditorChain(SwapChainRenderTarget chain)
        {
            sMapEditorChain = chain;
        }

        public static void SetTilesetChain(SwapChainRenderTarget chain)
        {
            sTilesetChain = chain;
        }

        //Resource Allocation
        private static void SetupWhiteTex()
        {
            sWhiteTex = CreateRenderTexture(1, 1);
            SetRenderTarget(sWhiteTex);
            sGraphicsDevice.Clear(Microsoft.Xna.Framework.Color.White);
            SetRenderTarget(null);
        }

        public static RenderTarget2D GetWhiteTex()
        {
            return sWhiteTex;
        }

        public static RenderTarget2D CreateRenderTexture(int width, int height)
        {
            return new RenderTarget2D(
                sGraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0,
                RenderTargetUsage.PreserveContents
            );
        }

        //Rendering
        public static void Render()
        {
            if (sMapEditorChain != null && !sMapEditorChain.IsContentLost && !sMapEditorChain.IsDisposed)
            {
                lock (GraphicsLock)
                {
                    SetRenderTarget(sMapEditorChain);
                    sGraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
                    StartSpritebatch();
                    ClearDarknessTexture(null);
                    DrawTileset();
                    DrawMapGrid();

                    //Draw Current Map
                    if (Globals.MapEditorWindow.picMap.Visible &&
                        Globals.MapEditorWindow.DockPanel.ActiveDocument == Globals.MapEditorWindow &&
                        Globals.CurrentMap != null &&
                        Globals.MapGrid.Loaded)
                    {
                        //Draw The lower maps
                        for (var y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                        {
                            for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    MapInstance map = null;
                                    try
                                    {
                                        map = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                                    }
                                    catch (Exception exception)
                                    {
                                        Log.Error(
                                            $"{Globals.MapGrid.Grid.GetLength(0)}x{Globals.MapGrid.Grid.GetLength(1)} -- {x},{y}"
                                        );

                                        Log.Error(exception);
                                    }

                                    if (map != null)
                                    {
                                        lock (map.MapLock)
                                        {
                                            //Draw this map
                                            DrawMap(
                                                map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                                false, 0, null
                                            );
                                        }
                                    }
                                    else
                                    {
                                        DrawTransparentBorders(
                                            x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY
                                        );
                                    }
                                }
                                else
                                {
                                    DrawTransparentBorders(
                                        x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY
                                    );
                                }
                            }
                        }

                        //Draw the lower resources/animations
                        for (var y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                        {
                            for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    var mapGridItem = Globals.MapGrid.Grid[x, y];
                                    var map = MapInstance.Get(mapGridItem.MapId);
                                    if (map == null)
                                    {
                                        continue;
                                    }

                                    lock (map.MapLock)
                                    {
                                        DrawMapAttributes(
                                            map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                            false, null, false, false
                                        );

                                        DrawMapAttributes(
                                            map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                            false, null, false, true
                                        );

                                        DrawMapAttributes(
                                            map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                            false, null, true, true
                                        );
                                    }
                                }
                            }
                        }

                        // Draw events
                        for (var y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                        {
                            for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    var map = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                                    if (map != null)
                                    {
                                        lock (map.MapLock)
                                        {
                                            DrawMapEvents(
                                                map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                                false, null
                                            );
                                        }
                                    }
                                }
                            }
                        }

                        //Draw The upper maps
                        for (var y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                        {
                            for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    var map = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                                    if (map != null)
                                    {
                                        lock (map.MapLock)
                                        {
                                            //Draw this map
                                            DrawMap(
                                                map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                                false, 1, null
                                            );
                                        }
                                    }
                                }
                            }
                        }

                        //Draw the upper resources/animations
                        for (var y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                        {
                            for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    var map = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                                    if (map != null)
                                    {
                                        lock (map.MapLock)
                                        {
                                            DrawMapAttributes(
                                                map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                                false, null, true, false
                                            );
                                        }
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

                        if (!HideDarkness || Globals.CurrentLayer == LayerOptions.Lights)
                        {
                            OverlayDarkness(null);
                        }

                        if (!HideGrid)
                        {
                            DrawGridOverlay();
                        }

                        DrawMapBorders();
                        DrawSelectionRect();
                    }

                    EndSpriteBatch();
                    sMapEditorChain.Present();
                }
            }
        }

        private static void DrawGridOverlay()
        {
            for (var x = 0; x < Options.MapWidth; x++)
            {
                DrawTexture(
                    sWhiteTex, new RectangleF(0, 0, 1, 1),
                    new RectangleF(
                        CurrentView.Left + x * Options.TileWidth, CurrentView.Top, 1,
                        Options.MapHeight * Options.TileHeight
                    ), null
                );
            }

            for (var y = 0; y < Options.MapHeight; y++)
            {
                DrawTexture(
                    sWhiteTex, new RectangleF(0, 0, 1, 1),
                    new RectangleF(
                        CurrentView.Left, CurrentView.Top + y * Options.TileHeight,
                        Options.MapWidth * Options.TileWidth, 1
                    ), null
                );
            }
        }

        private static void DrawTransparentBorders(int gridX, int gridY)
        {
            var transTex = GameContentManager.GetTexture(GameContentManager.TextureType.Misc, "transtile.png");
            if (transTex == null)
            {
                return;
            }

            var xoffset = CurrentView.Left + gridX * Options.TileWidth * Options.MapWidth;
            var yoffset = CurrentView.Top + gridY * Options.TileHeight * Options.MapHeight;
            for (var x = 0; x < Options.MapWidth; x++)
            {
                for (var y = 0; y < Options.MapHeight; y++)
                {
                    if (new System.Drawing.Rectangle(
                        x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, Options.TileWidth,
                        Options.TileHeight
                    ).IntersectsWith(new System.Drawing.Rectangle(0, 0, CurrentView.Width, CurrentView.Height)))
                    {
                        DrawTexture(
                            transTex, new RectangleF(0, 0, transTex.Width, transTex.Height),
                            new RectangleF(
                                xoffset + x * Options.TileWidth, yoffset + y * Options.TileHeight, Options.TileWidth,
                                Options.TileHeight
                            ), System.Drawing.Color.White, null
                        );
                    }
                }
            }
        }

        private static void DrawAutoTile(
            Texture2D texture,
            string layerName,
            int destX,
            int destY,
            int quarterNum,
            int x,
            int y,
            MapBase map,
            RenderTarget2D target
        )
        {
            int yOffset = 0, xOffset = 0;

            // calculate the offset
            switch (map.Layers[layerName][x, y].Autotile)
            {
                case MapAutotiles.AUTOTILE_WATERFALL:
                    yOffset = (Globals.WaterfallFrame - 1) * Options.TileHeight;

                    break;
                case MapAutotiles.AUTOTILE_ANIM:
                    xOffset = Globals.AutotileFrame * Options.TileWidth * 2;

                    break;
                case MapAutotiles.AUTOTILE_ANIM_XP:
                    xOffset = Globals.AutotileFrame * Options.TileWidth * 3;

                    break;
                case MapAutotiles.AUTOTILE_CLIFF:
                    yOffset = -Options.TileHeight;

                    break;
            }

            DrawTexture(
                texture, destX, destY,
                (int) map.Autotiles.Layers[layerName][x, y].QuarterTile[quarterNum].X + xOffset,
                (int) map.Autotiles.Layers[layerName][x, y].QuarterTile[quarterNum].Y + yOffset,
                Options.TileWidth / 2, Options.TileHeight / 2, target
            );
        }

        private static void DrawMap(
            MapInstance map,
            int gridX,
            int gridY,
            bool screenShotting,
            int layer,
            RenderTarget2D renderTarget2D
        )
        {
            var tmpMap = Globals.CurrentMap;
            if (tmpMap == null)
            {
                return;
            }

            int selX = Globals.CurMapSelX,
                selY = Globals.CurMapSelY,
                selW = Globals.CurMapSelW,
                selH = Globals.CurMapSelH;

            int x1 = 0, y1 = 0, x2 = 0, y2 = 0, xoffset = 0, yoffset = 0;
            int dragxoffset = 0, dragyoffset = 0;
            if (Globals.CurrentTool == (int) EditingTool.Rectangle ||
                Globals.CurrentTool == (int) EditingTool.Selection)
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

            var drawLayers = new List<string>();
            if (layer == 0)
            {
                drawLayers.AddRange(Options.Instance.MapOpts.Layers.LowerLayers);
            }
            else
            {
                drawLayers.AddRange(Options.Instance.MapOpts.Layers.MiddleLayers);
                drawLayers.AddRange(Options.Instance.MapOpts.Layers.UpperLayers);
            }

            x1 = 0;
            x2 = Options.MapWidth;
            y1 = 0;
            y2 = Options.MapHeight;
            xoffset = CurrentView.Left + gridX * Options.TileWidth * Options.MapWidth;
            yoffset = CurrentView.Top + gridY * Options.TileHeight * Options.MapHeight;
            if (gridX != 0 || gridY != 0)
            {
                tmpMap = map;
            }

            if (tmpMap == null)
            {
                return;
            }

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
                        if (Globals.CurrentMap != null)
                        {
                            lock (Globals.CurrentMap.MapLock)
                            {
                                TilePreviewStruct = new MapInstance(Globals.CurrentMap);
                            }
                        }

                        //Lets Create the Preview
                        //Mimic Mouse Down
                        tmpMap = TilePreviewStruct;
                        if (Globals.CurrentTool == (int) EditingTool.Selection && Globals.Dragging)
                        {
                            Globals.MapEditorWindow.ProcessSelectionMovement(tmpMap, false, true);
                        }
                        else
                        {
                            if (Globals.CurrentLayer == LayerOptions.Attributes)
                            {
                                if (Globals.CurrentTool == (int) EditingTool.Pen)
                                {
                                    Globals.MapLayersWindow.PlaceAttribute(tmpMap, Globals.CurTileX, Globals.CurTileY);
                                }
                                else if (Globals.CurrentTool == (int) EditingTool.Rectangle)
                                {
                                    for (var x = selX; x < selX + selW + 1; x++)
                                    {
                                        for (var y = selY; y < selY + selH + 1; y++)
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
                            else if (Globals.CurrentLayer == LayerOptions.Lights)
                            {
                            }
                            else if (Globals.CurrentLayer == LayerOptions.Events) 
                            {
                            }
                            else if (Globals.CurrentLayer == LayerOptions.Npcs)
                            {
                            }
                            else if (Globals.CurrentTileset != null)
                            {
                                if (Globals.CurrentTool == (int) EditingTool.Pen)
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
                                                    tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX + x, Globals.CurTileY + y].TilesetId = Globals.CurrentTileset.Id;
                                                    tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX + x, Globals.CurTileY + y].X = Globals.CurSelX + x;
                                                    tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX + x, Globals.CurTileY + y].Y = Globals.CurSelY + y;
                                                    tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX + x, Globals.CurTileY + y].Autotile = 0;
                                                    tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX + x, Globals.CurTileY + y, Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].TilesetId = Globals.CurrentTileset.Id;
                                        tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX;
                                        tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY;
                                        tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Autotile = (byte) Globals.Autotilemode;
                                        tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX, Globals.CurTileY, Globals.CurrentLayer,tmpMap.GenerateAutotileGrid());
                                    }

                                    tmpMap.Autotiles.UpdateCliffAutotiles(tmpMap, Globals.CurrentLayer);
                                }
                                else if (Globals.CurrentTool == (int) EditingTool.Rectangle)
                                {
                                    var x = 0;
                                    var y = 0;
                                    for (var x0 = selX; x0 < selX + selW + 1; x0++)
                                    {
                                        for (var y0 = selY; y0 < selY + selH + 1; y0++)
                                        {
                                            x = (x0 - selX) % (Globals.CurSelW + 1);
                                            y = (y0 - selY) % (Globals.CurSelH + 1);
                                            if (Globals.Autotilemode == 0)
                                            {
                                                if (x0 >= 0 &&
                                                    x0 < Options.MapWidth &&
                                                    y0 >= 0 &&
                                                    y0 < Options.MapHeight &&
                                                    x0 < selX + selW + 1 &&
                                                    y0 < selY + selH + 1)
                                                {
                                                    if (Globals.MouseButton == 0)
                                                    {
                                                        tmpMap.Layers[Globals.CurrentLayer][x0, y0].TilesetId = Globals.CurrentTileset.Id;
                                                        tmpMap.Layers[Globals.CurrentLayer][x0, y0].X = Globals.CurSelX + x;
                                                        tmpMap.Layers[Globals.CurrentLayer][x0, y0].Y = Globals.CurSelY + y;
                                                        tmpMap.Layers[Globals.CurrentLayer][x0, y0].Autotile = (byte) Globals.Autotilemode;
                                                    }
                                                    else if (Globals.MouseButton == 1)
                                                    {
                                                        tmpMap.Layers[Globals.CurrentLayer][x0, y0].TilesetId = Guid.Empty;
                                                        tmpMap.Layers[Globals.CurrentLayer][x0, y0].X = 0;
                                                        tmpMap.Layers[Globals.CurrentLayer][x0, y0].Y = 0;
                                                        tmpMap.Layers[Globals.CurrentLayer][x0, y0].Autotile = 0;
                                                    }

                                                    tmpMap.Autotiles.UpdateAutoTiles(
                                                        x0, y0, Globals.CurrentLayer, tmpMap.GenerateAutotileGrid()
                                                    );
                                                }
                                            }
                                            else
                                            {
                                                if (Globals.MouseButton == 0)
                                                {
                                                    tmpMap.Layers[Globals.CurrentLayer][x0, y0].TilesetId = Globals.CurrentTileset.Id;
                                                    tmpMap.Layers[Globals.CurrentLayer][x0, y0].X = Globals.CurSelX;
                                                    tmpMap.Layers[Globals.CurrentLayer][x0, y0].Y = Globals.CurSelY;
                                                    tmpMap.Layers[Globals.CurrentLayer][x0, y0].Autotile = (byte) Globals.Autotilemode;
                                                }
                                                else if (Globals.MouseButton == 1)
                                                {
                                                    tmpMap.Layers[Globals.CurrentLayer][x0, y0].TilesetId = Guid.Empty;
                                                    tmpMap.Layers[Globals.CurrentLayer][x0, y0].X = 0;
                                                    tmpMap.Layers[Globals.CurrentLayer][x0, y0].Y = 0;
                                                    tmpMap.Layers[Globals.CurrentLayer][x0, y0].Autotile = 0;
                                                }

                                                tmpMap.Autotiles.UpdateAutoTiles(
                                                    x0, y0, Globals.CurrentLayer, tmpMap.GenerateAutotileGrid()
                                                );
                                            }

                                            tmpMap.Autotiles.UpdateCliffAutotiles(tmpMap, Globals.CurrentLayer);
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
                    foreach (var drawLayer in drawLayers)
                    {
                        if (screenShotting || Globals.MapLayersWindow.LayerVisibility[drawLayer])
                        {
                            if (new System.Drawing.Rectangle(
                                x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset, Options.TileWidth,
                                Options.TileHeight
                            ).IntersectsWith(new System.Drawing.Rectangle(0, 0, CurrentView.Width, CurrentView.Height)))
                            {
                                var tilesetObj = TilesetBase.Get(tmpMap.Layers[drawLayer][x, y].TilesetId);
                                try
                                {
                                    if (tilesetObj == null)
                                    {
                                        continue;
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Log.Error($"map={tmpMap != null},layer{drawLayer}.tiles={tmpMap.Layers[drawLayer] != null}");
                                    Log.Error(exception);

                                    continue;
                                }

                                var tilesetTex = GameContentManager.GetTexture(
                                    GameContentManager.TextureType.Tileset, tilesetObj.Name
                                );

                                if (tilesetTex == null || tmpMap.Autotiles == null || tmpMap.Autotiles.Layers == null)
                                {
                                    continue;
                                }

                                if (tmpMap.Autotiles.Layers[drawLayer][x, y].RenderState !=
                                    MapAutotiles.RENDER_STATE_NORMAL)
                                {
                                    if (tmpMap.Autotiles.Layers[drawLayer][x, y].RenderState !=
                                        MapAutotiles.RENDER_STATE_AUTOTILE)
                                    {
                                        continue;
                                    }

                                    DrawAutoTile(
                                        tilesetTex, drawLayer, x * Options.TileWidth + xoffset,
                                        y * Options.TileHeight + yoffset, 1, x, y, tmpMap, renderTarget2D
                                    );

                                    DrawAutoTile(
                                        tilesetTex, drawLayer, x * Options.TileWidth + Options.TileWidth / 2 + xoffset,
                                        y * Options.TileHeight + yoffset, 2, x, y, tmpMap, renderTarget2D
                                    );

                                    DrawAutoTile(
                                        tilesetTex, drawLayer, x * Options.TileWidth + xoffset,
                                        y * Options.TileHeight + Options.TileHeight / 2 + yoffset, 3, x, y, tmpMap,
                                        renderTarget2D
                                    );

                                    DrawAutoTile(
                                        tilesetTex, drawLayer, x * Options.TileWidth + Options.TileWidth / 2 + xoffset,
                                        y * Options.TileHeight + Options.TileHeight / 2 + yoffset, 4, x, y, tmpMap,
                                        renderTarget2D
                                    );
                                }
                                else
                                {
                                    DrawTexture(
                                        tilesetTex, x * Options.TileWidth + xoffset, y * Options.TileHeight + yoffset,
                                        tmpMap.Layers[drawLayer][x, y].X * Options.TileWidth,
                                        tmpMap.Layers[drawLayer][x, y].Y * Options.TileHeight, Options.TileWidth,
                                        Options.TileHeight, renderTarget2D
                                    );
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
                    var x = xoffset +
                            Options.MapWidth * Options.TileWidth -
                            CurrentView.Left +
                            light.TileX * Options.TileWidth +
                            light.OffsetX +
                            Options.TileWidth / 2;

                    var y = yoffset +
                            Options.MapHeight * Options.TileHeight -
                            CurrentView.Top +
                            light.TileY * Options.TileHeight +
                            light.OffsetY +
                            Options.TileHeight / 2;

                    if (!HideDarkness)
                    {
                        AddLight(x, y, light, null);
                    }
                }
            }
        }

        private static void DrawSelectionRect()
        {
            var tmpMap = Globals.CurrentMap;
            if (tmpMap == null)
            {
                return;
            }

            int selX = Globals.CurMapSelX,
                selY = Globals.CurMapSelY,
                selW = Globals.CurMapSelW,
                selH = Globals.CurMapSelH;

            int dragxoffset = 0, dragyoffset = 0;
            if (Globals.CurrentTool == (int) EditingTool.Rectangle ||
                Globals.CurrentTool == (int) EditingTool.Selection)
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

            if (!HideTilePreview || Globals.Dragging)
            {
                tmpMap = TilePreviewStruct;
                if (Globals.CurrentLayer == LayerOptions.Attributes) //Attributes
                {
                    //Draw attributes
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            if (tmpMap.Attributes[x, y] != null)
                            {
                                if (tmpMap.Attributes[x, y].Type > 0)
                                {
                                    var attributesTex = GameContentManager.GetTexture(
                                        GameContentManager.TextureType.Misc, "attributes.png"
                                    );

                                    if (attributesTex != null)
                                    {
                                        DrawTexture(
                                            attributesTex,
                                            new RectangleF(
                                                0, ((int) tmpMap.Attributes[x, y].Type - 1) * attributesTex.Width,
                                                attributesTex.Width, attributesTex.Width
                                            ),
                                            new RectangleF(
                                                CurrentView.Left + x * Options.TileWidth,
                                                CurrentView.Top + y * Options.TileHeight, Options.TileWidth,
                                                Options.TileHeight
                                            ), System.Drawing.Color.FromArgb(150, 255, 255, 255), null
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Globals.CurrentLayer == LayerOptions.Lights) //Lights
                {
                }
                else if (Globals.CurrentLayer == LayerOptions.Events) //Events
                {
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            if (tmpMap.FindEventAt(x, y) == null)
                            {
                                continue;
                            }

                            var eventTex = GameContentManager.GetTexture(
                                GameContentManager.TextureType.Misc, "eventicon.png"
                            );

                            if (eventTex != null)
                            {
                                DrawTexture(
                                    eventTex, new RectangleF(0, 0, eventTex.Width, eventTex.Height),
                                    new RectangleF(
                                        CurrentView.Left + x * Options.TileWidth,
                                        CurrentView.Top + y * Options.TileHeight, Options.TileWidth, Options.TileHeight
                                    ), System.Drawing.Color.White, null
                                );
                            }
                        }
                    }
                }
                else if (Globals.CurrentLayer == LayerOptions.Npcs) //NPCS
                {
                    for (var i = 0; i < tmpMap.Spawns.Count; i++)
                    {
                        if (tmpMap.Spawns[i].X >= 0 && tmpMap.Spawns[i].Y >= 0)
                        {
                            var spawnTex = GameContentManager.GetTexture(
                                GameContentManager.TextureType.Misc, "spawnicon.png"
                            );

                            if (spawnTex != null)
                            {
                                DrawTexture(
                                    spawnTex, new RectangleF(0, 0, spawnTex.Width, spawnTex.Height),
                                    new RectangleF(
                                        CurrentView.Left + tmpMap.Spawns[i].X * Options.TileWidth,
                                        CurrentView.Top + tmpMap.Spawns[i].Y * Options.TileHeight, Options.TileWidth,
                                        Options.TileHeight
                                    ), System.Drawing.Color.White, null
                                );
                            }
                        }
                    }
                }
                else
                {
                }
            }

            if (Globals.CurrentTool == (int) EditingTool.Selection && Globals.Dragging)
            {
                DrawBoxOutline(
                    CurrentView.Left + Globals.CurTileX * Options.TileWidth,
                    CurrentView.Top + Globals.CurTileY * Options.TileHeight, Options.TileWidth, Options.TileHeight,
                    System.Drawing.Color.White, null
                );
            }

            if (Globals.CurrentTool == (int) EditingTool.Rectangle ||
                Globals.CurrentTool == (int) EditingTool.Selection)
            {
                DrawBoxOutline(
                    CurrentView.Left + (selX + dragxoffset) * Options.TileWidth,
                    CurrentView.Top + (selY + dragyoffset) * Options.TileHeight, (selW + 1) * Options.TileWidth,
                    (selH + 1) * Options.TileHeight, System.Drawing.Color.Blue, null
                );
            }
            else
            {
                DrawBoxOutline(
                    CurrentView.Left + Globals.CurTileX * Options.TileWidth,
                    CurrentView.Top + Globals.CurTileY * Options.TileHeight, Options.TileWidth, Options.TileHeight,
                    System.Drawing.Color.White, null
                );
            }
        }

        private static void DrawBoxOutline(int x, int y, int w, int h, System.Drawing.Color clr, RenderTarget2D target)
        {
            //Draw Top of Box
            DrawTexture(sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x, y, w, 1), clr, target);

            //Bottom
            DrawTexture(sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x, y + h, w, 1), clr, target);

            //Left
            DrawTexture(sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x, y, 1, h), clr, target);

            //Right
            DrawTexture(sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x + w, y, 1, h), clr, target);
        }

        public static void DrawMapGrid()
        {
            if (sMapGridChain == null ||
                sMapGridChain.IsContentLost ||
                sMapGridChain.IsDisposed ||
                Globals.MapGridWindowNew.DockPanel.ActiveDocument != Globals.MapGridWindowNew)
            {
                return;
            }

            EndSpriteBatch();
            SetRenderTarget(sMapGridChain);
            sGraphicsDevice.Clear(Microsoft.Xna.Framework.Color.FromNonPremultiplied(60, 63, 65, 255));
            var rand = new Random();
            var grid = Globals.MapGrid;
            lock (Globals.MapGrid.GetMapGridLock())
            {
                grid.Update(sMapGridChain.Bounds);
                for (var x = 0; x < grid.GridWidth + 2; x++)
                {
                    for (var y = 0; y < grid.GridHeight + 2; y++)
                    {
                        var renderRect = new System.Drawing.Rectangle(
                            grid.ContentRect.X + x * grid.TileWidth, grid.ContentRect.Y + y * grid.TileHeight,
                            grid.TileWidth, grid.TileHeight
                        );

                        if (grid.ViewRect.IntersectsWith(renderRect))
                        {
                            if (x == 0 ||
                                y == 0 ||
                                x == grid.GridWidth + 1 ||
                                y == grid.GridHeight + 1 ||
                                grid.Grid[x - 1, y - 1].MapId == Guid.Empty)
                            {
                                DrawTexture(
                                    GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                    new RectangleF(
                                        grid.ContentRect.X + x * grid.TileWidth,
                                        grid.ContentRect.Y + y * grid.TileHeight, grid.TileWidth, grid.TileHeight
                                    ), System.Drawing.Color.FromArgb(45, 45, 48), sMapGridChain
                                );
                            }
                            else
                            {
                                if (grid.Grid[x - 1, y - 1].MapId != Guid.Empty)
                                {
                                    if (grid.Grid[x - 1, y - 1].Tex != null &&
                                        grid.Grid[x - 1, y - 1].Tex.Width == grid.TileWidth &&
                                        grid.Grid[x - 1, y - 1].Tex.Height == grid.TileHeight)
                                    {
                                        DrawTexture(
                                            grid.Grid[x - 1, y - 1].Tex, grid.ContentRect.X + x * grid.TileWidth,
                                            grid.ContentRect.Y + y * grid.TileHeight, sMapGridChain
                                        );
                                    }
                                    else
                                    {
                                        DrawTexture(
                                            GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                            new RectangleF(
                                                grid.ContentRect.X + x * grid.TileWidth,
                                                grid.ContentRect.Y + y * grid.TileHeight, grid.TileWidth,
                                                grid.TileHeight
                                            ), System.Drawing.Color.Green, sMapGridChain
                                        );
                                    }
                                }
                                else
                                {
                                    DrawTexture(
                                        GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                        new RectangleF(
                                            grid.ContentRect.X + x * grid.TileWidth,
                                            grid.ContentRect.Y + y * grid.TileHeight, grid.TileWidth, grid.TileHeight
                                        ), System.Drawing.Color.Gray, sMapGridChain
                                    );
                                }
                            }

                            if (Globals.MapGrid.ShowLines)
                            {
                                DrawTexture(
                                    GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                    new RectangleF(
                                        grid.ContentRect.X + x * grid.TileWidth,
                                        grid.ContentRect.Y + y * grid.TileHeight, grid.TileWidth, 1
                                    ), System.Drawing.Color.DarkGray, sMapGridChain
                                );

                                DrawTexture(
                                    GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                    new RectangleF(
                                        grid.ContentRect.X + x * grid.TileWidth,
                                        grid.ContentRect.Y + y * grid.TileHeight, 1, grid.TileHeight
                                    ), System.Drawing.Color.DarkGray, sMapGridChain
                                );

                                DrawTexture(
                                    GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                    new RectangleF(
                                        grid.ContentRect.X + x * grid.TileWidth + grid.TileWidth,
                                        grid.ContentRect.Y + y * grid.TileHeight, 1, grid.TileHeight
                                    ), System.Drawing.Color.DarkGray, sMapGridChain
                                );

                                DrawTexture(
                                    GetWhiteTex(), new RectangleF(0, 0, 1, 1),
                                    new RectangleF(
                                        grid.ContentRect.X + x * grid.TileWidth,
                                        grid.ContentRect.Y + y * grid.TileHeight + grid.TileHeight, grid.TileWidth, 1
                                    ), System.Drawing.Color.DarkGray, sMapGridChain
                                );
                            }
                        }
                    }
                }
            }

            EndSpriteBatch();
            sMapGridChain.Present();
        }

        private static void SetRenderTarget(RenderTarget2D target)
        {
            EndSpriteBatch();
            sGraphicsDevice.SetRenderTarget(target);
        }

        public static void DrawTileset()
        {
            if (sTilesetChain == null ||
                sTilesetChain.IsContentLost ||
                sTilesetChain.IsDisposed ||
                Globals.MapLayersWindow.CurrentTab != LayerTabs.Tiles)
            {
                return;
            }

            SetRenderTarget(sTilesetChain);
            sGraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            if (Globals.CurrentTileset != null)
            {
                var tilesetTex = GameContentManager.GetTexture(
                    GameContentManager.TextureType.Tileset, Globals.CurrentTileset.Name
                );

                if (tilesetTex != null)
                {
                    DrawTexture(tilesetTex, 0, 0, sTilesetChain);
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

                    DrawBoxOutline(
                        selX * Options.TileWidth, selY * Options.TileHeight,
                        Options.TileWidth + selW * Options.TileWidth, Options.TileHeight + selH * Options.TileHeight,
                        System.Drawing.Color.White, sTilesetChain
                    );
                }
            }

            EndSpriteBatch();
            sTilesetChain.Present();
        }

        private static void DrawMapBorders()
        {
            //Horizontal Top
            DrawTexture(
                sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, CurrentView.Y - 1, CurrentView.Width, 3),
                System.Drawing.Color.DimGray
            );

            //Horizontal Buttom
            DrawTexture(
                sWhiteTex, new RectangleF(0, 0, 1, 1),
                new RectangleF(0, CurrentView.Y + Options.TileHeight * Options.MapHeight - 1, CurrentView.Width, 3),
                System.Drawing.Color.DimGray
            );

            //Vertical Left
            DrawTexture(
                sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(CurrentView.Left - 1, 0, 3, CurrentView.Height),
                System.Drawing.Color.DimGray
            );

            //Vertical Right
            DrawTexture(
                sWhiteTex, new RectangleF(0, 0, 1, 1),
                new RectangleF(CurrentView.Left + Options.TileWidth * Options.MapWidth - 1, 0, 3, CurrentView.Height),
                System.Drawing.Color.DimGray
            );

            //Horizontal Top
            DrawTexture(
                sWhiteTex, new RectangleF(0, 0, 1, 1),
                new RectangleF(CurrentView.Left, CurrentView.Y - 1, Options.TileWidth * Options.MapWidth, 3),
                System.Drawing.Color.DimGray
            );

            //Horizontal Buttom
            DrawTexture(
                sWhiteTex, new RectangleF(0, 0, 1, 1),
                new RectangleF(
                    CurrentView.Left, CurrentView.Y + Options.TileHeight * Options.MapHeight - 1,
                    Options.TileWidth * Options.MapWidth, 3
                ), System.Drawing.Color.DimGray
            );

            //Vertical Left
            DrawTexture(
                sWhiteTex, new RectangleF(0, 0, 1, 1),
                new RectangleF(CurrentView.Left - 1, CurrentView.Y, 3, Options.MapHeight * Options.TileHeight),
                System.Drawing.Color.DimGray
            );

            //Vertical Right
            DrawTexture(
                sWhiteTex, new RectangleF(0, 0, 1, 1),
                new RectangleF(
                    CurrentView.Left + Options.TileWidth * Options.MapWidth - 1, CurrentView.Y, 3,
                    Options.MapHeight * Options.TileHeight
                ), System.Drawing.Color.DimGray
            );
        }

        private static void DrawMapAttributes(
            MapInstance map,
            int gridX,
            int gridY,
            bool screenShotting,
            RenderTarget2D renderTarget,
            bool upper,
            bool alternate
        )
        {
            if (!screenShotting && HideResources)
            {
                return;
            }

            if (screenShotting && Database.GridHideResources)
            {
                return;
            }

            var tmpMap = Globals.CurrentMap;
            if (tmpMap == null)
            {
                return;
            }

            int x1 = 0, y1 = 0, x2 = 0, y2 = 0, xoffset = 0, yoffset = 0;

            x1 = 0;
            x2 = Options.MapWidth;
            y1 = 0;
            y2 = Options.MapHeight;
            xoffset = CurrentView.Left + gridX * Options.TileWidth * Options.MapWidth;
            yoffset = CurrentView.Top + gridY * Options.TileHeight * Options.MapHeight;
            if (gridX != 0 || gridY != 0)
            {
                tmpMap = map;
            }

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

            if (tmpMap == null)
            {
                return;
            }

            for (var y = y1; y < y2; y++)
            {
                for (var x = x1; x < x2; x++)
                {
                    if (tmpMap.Attributes[x, y] == null)
                    {
                        continue;
                    }

                    if (tmpMap.Attributes[x, y].Type == MapAttributes.Resource && !upper && !alternate)
                    {
                        var resource = ResourceBase.Get(((MapResourceAttribute) tmpMap.Attributes[x, y]).ResourceId);
                        if (resource == null)
                        {
                            continue;
                        }

                        if (TextUtils.IsNone(resource.Initial.Graphic))
                        {
                            continue;
                        }

                        if (resource.Initial.GraphicFromTileset)
                        {
                            var res = GameContentManager.GetTexture(
                                GameContentManager.TextureType.Tileset, resource.Initial.Graphic
                            );

                            if (res == null)
                            {
                                continue;
                            }

                            float xpos = x * Options.TileWidth + xoffset;
                            float ypos = y * Options.TileHeight + yoffset;
                            if ((resource.Initial.Height + 1) * Options.TileHeight > Options.TileHeight)
                            {
                                ypos -= (int) (resource.Initial.Height + 1) * Options.TileHeight - Options.TileHeight;
                            }

                            if ((resource.Initial.Width + 1) * Options.TileWidth > Options.TileWidth)
                            {
                                xpos -= ((resource.Initial.Width + 1) * Options.TileWidth - Options.TileWidth) / 2;
                            }

                            DrawTexture(
                                res, xpos, ypos, resource.Initial.X * Options.TileWidth,
                                resource.Initial.Y * Options.TileHeight,
                                (int) (resource.Initial.Width + 1) * Options.TileWidth,
                                (int) (resource.Initial.Height + 1) * Options.TileHeight, renderTarget
                            );
                        }
                        else
                        {
                            var res = GameContentManager.GetTexture(
                                GameContentManager.TextureType.Resource, resource.Initial.Graphic
                            );

                            if (res == null)
                            {
                                continue;
                            }

                            float xpos = x * Options.TileWidth + xoffset;
                            float ypos = y * Options.TileHeight + yoffset;
                            if (res.Height > Options.TileHeight)
                            {
                                ypos -= (int) res.Height - Options.TileHeight;
                            }

                            if (res.Width > Options.TileWidth)
                            {
                                xpos -= (res.Width - Options.TileWidth) / 2;
                            }

                            DrawTexture(res, xpos, ypos, 0, 0, (int) res.Width, (int) res.Height, renderTarget);
                        }
                    }
                    else if (tmpMap.Attributes[x, y].Type == MapAttributes.Animation)
                    {
                        var animation =
                            AnimationBase.Get(((MapAnimationAttribute) tmpMap.Attributes[x, y]).AnimationId);

                        if (animation != null)
                        {
                            float xpos = x * Options.TileWidth + xoffset + Options.TileWidth / 2;
                            float ypos = y * Options.TileHeight + yoffset + Options.TileHeight / 2;
                            if (tmpMap.Attributes[x, y] != null)
                            {
                                var animInstance = tmpMap.GetAttributeAnimation(tmpMap.Attributes[x, y], animation.Id);

                                //Update if the animation isn't right!
                                if (animInstance == null || animInstance.MyBase != animation)
                                {
                                    tmpMap.SetAttributeAnimation(
                                        tmpMap.Attributes[x, y], new Animation(animation, true)
                                    );
                                }

                                animInstance.Update();
                                animInstance.SetPosition((int) xpos, (int) ypos, 0);
                                animInstance.Draw(renderTarget, upper, alternate);
                            }
                        }
                    }
                }
            }
        }

        private static void DrawMapEvents(
            MapInstance map,
            int gridX,
            int gridY,
            bool screenShotting,
            RenderTarget2D renderTarget
        )
        {
            if (!screenShotting && HideEvents)
            {
                return;
            }

            if (screenShotting && Database.GridHideEvents)
            {
                return;
            }

            var tmpMap = Globals.CurrentMap;
            if (tmpMap == null)
            {
                return;
            }

            int x1 = 0, y1 = 0, x2 = 0, y2 = 0, xoffset = 0, yoffset = 0;

            x1 = 0;
            x2 = Options.MapWidth;
            y1 = 0;
            y2 = Options.MapHeight;
            xoffset = CurrentView.Left + gridX * Options.TileWidth * Options.MapWidth;
            yoffset = CurrentView.Top + gridY * Options.TileHeight * Options.MapHeight;
            if (gridX != 0 || gridY != 0)
            {
                tmpMap = map;
            }

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

            if (tmpMap == null)
            {
                return;
            }

            for (var y = y1; y < y2; y++)
            {
                for (var x = x1; x < x2; x++)
                {
                    var tmpEvent = tmpMap.FindEventAt(x, y);
                    if (tmpEvent == null)
                    {
                        continue;
                    }

                    if (tmpEvent.Pages[0].Graphic == null)
                    {
                        continue;
                    }

                    var tmpGraphic = tmpEvent.Pages[0].Graphic;
                    if (TextUtils.IsNone(tmpGraphic.Filename))
                    {
                        continue;
                    }

                    Texture2D eventTex = null;
                    var destinationX = x * Options.TileWidth + xoffset;
                    var destinationY = y * Options.TileHeight + yoffset;
                    var sourceX = 0;
                    var sourceY = 0;
                    var width = 0;
                    var height = 0;

                    switch (tmpGraphic.Type)
                    {
                        case EventGraphicType.Sprite: //Sprite
                            eventTex = GameContentManager.GetTexture(
                                GameContentManager.TextureType.Entity, tmpGraphic.Filename
                            );
                            if (eventTex == null)
                            {
                                continue;
                            }

                            sourceX = (int)tmpGraphic.X * (eventTex.Width / Options.Instance.Sprites.NormalFrames);
                            sourceY = (int)tmpGraphic.Y * (eventTex.Height / Options.Instance.Sprites.Directions);
                            width = (eventTex.Width / Options.Instance.Sprites.NormalFrames);
                            height = (eventTex.Height / Options.Instance.Sprites.Directions);

                            break;
                        case EventGraphicType.Tileset: //Tile
                            eventTex = GameContentManager.GetTexture(
                                GameContentManager.TextureType.Tileset, tmpGraphic.Filename
                            );
                            if (eventTex == null)
                            {
                                continue;
                            }

                            sourceX = (int)tmpGraphic.X * Options.TileWidth;
                            sourceY = (int)tmpGraphic.Y * Options.TileHeight;
                            width = (tmpGraphic.Width + 1) * Options.TileWidth;
                            height = (tmpGraphic.Height + 1) * Options.TileHeight;

                            break;
                    }

                    if (height > Options.TileHeight)
                    {
                        destinationY -= (height - Options.TileHeight);
                    }

                    if (width > Options.TileWidth)
                    {
                        destinationX -= (width  - Options.TileWidth) / 2;
                    }

                    DrawTexture(eventTex, destinationX, destinationY, sourceX, sourceY, width, height, renderTarget);
                }
            }
        }
        private static void DrawMapOverlay(RenderTarget2D target)
        {
            DrawTexture(
                sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, CurrentView.Width, CurrentView.Height),
                System.Drawing.Color.FromArgb(
                    Globals.CurrentMap.AHue, Globals.CurrentMap.RHue, Globals.CurrentMap.GHue, Globals.CurrentMap.BHue
                ), target
            );
        }

        public static Bitmap ScreenShotMap()
        {
            if (sScreenShotRenderTexture == null)
            {
                sScreenShotRenderTexture = CreateRenderTexture(
                    Options.MapWidth * Options.TileWidth, Options.MapHeight * Options.TileHeight
                );
            }

            SetRenderTarget(sScreenShotRenderTexture);
            sGraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);

            if (Globals.MapGrid.Contains(Globals.CurrentMap.Id))
            {
                //Draw The lower maps
                for (var y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; ++y)
                {
                    for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; ++x)
                    {
                        if (x < 0 || x >= Globals.MapGrid.GridWidth)
                        {
                            continue;
                        }

                        if (y < 0 || y >= Globals.MapGrid.GridHeight)
                        {
                            continue;
                        }

                        var map = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                        if (map == null)
                        {
                            continue;
                        }

                        lock (map.MapLock)
                        {
                            //Draw this map
                            DrawMap(
                                map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY, true, 0,
                                sScreenShotRenderTexture
                            );
                        }
                    }
                }

                //Draw the lower resources/animations
                for (var y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                {
                    for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                    {
                        if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                        {
                            var map = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                            if (map != null)
                            {
                                lock (map.MapLock)
                                {
                                    DrawMapAttributes(
                                        map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY, true,
                                        sScreenShotRenderTexture, false, false
                                    );

                                    DrawMapAttributes(
                                        map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY, true,
                                        sScreenShotRenderTexture, false, true
                                    );

                                    DrawMapAttributes(
                                        map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY, true,
                                        sScreenShotRenderTexture, true, true
                                    );
                                }
                            }
                        }
                    }
                }

                // Draw events
                for (var y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                {
                    for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                    {
                        if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                        {
                            var map = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                            if (map != null)
                            {
                                lock (map.MapLock)
                                {
                                    DrawMapEvents(
                                        map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY,
                                        true, sScreenShotRenderTexture
                                    );
                                }
                            }
                        }
                    }
                }

                //Draw The upper maps
                for (var y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                {
                    for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                    {
                        if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                        {
                            var map = MapInstance.Get(Globals.MapGrid.Grid[x, y].MapId);
                            if (map != null)
                            {
                                lock (map.MapLock)
                                {
                                    //Draw this map
                                    DrawMap(
                                        map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY, true, 1,
                                        sScreenShotRenderTexture
                                    );
                                }
                            }
                        }
                    }
                }

                //Draw the upper resources/animations
                for (var y = Globals.CurrentMap.MapGridY - 1; y <= Globals.CurrentMap.MapGridY + 1; y++)
                {
                    for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                    {
                        if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                        {
                            var map = MapInstance.Get(Globals.MapGrid.Grid?[x, y]?.MapId ?? Guid.Empty);
                            if (map != null)
                            {
                                lock (map.MapLock)
                                {
                                    DrawMapAttributes(
                                        map, x - Globals.CurrentMap.MapGridX, y - Globals.CurrentMap.MapGridY, true,
                                        sScreenShotRenderTexture, true, false
                                    );
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                lock (Globals.CurrentMap.MapLock)
                {
                    //Draw this map
                    DrawMap(Globals.CurrentMap, 0, 0, true, 0, sScreenShotRenderTexture);
                    DrawMapAttributes(Globals.CurrentMap, 0, 0, true, sScreenShotRenderTexture, false, false);
                    DrawMapAttributes(Globals.CurrentMap, 0, 0, true, sScreenShotRenderTexture, false, true);
                    DrawMapAttributes(Globals.CurrentMap, 0, 0, true, sScreenShotRenderTexture, true, true);

                    //Draw this map
                    DrawMap(Globals.CurrentMap, 0, 0, true, 1, sScreenShotRenderTexture);
                    DrawMapAttributes(Globals.CurrentMap, 0, 0, true, sScreenShotRenderTexture, true, false);
                }
            }

            if (!Database.GridHideFog)
            {
                DrawFog(sScreenShotRenderTexture);
            }

            if (!Database.GridHideOverlay)
            {
                DrawMapOverlay(sScreenShotRenderTexture);
            }

            if (!Database.GridHideDarkness || Globals.CurrentLayer == LayerOptions.Lights)
            {
                ClearDarknessTexture(sScreenShotRenderTexture, true);
                OverlayDarkness(sScreenShotRenderTexture, true);
            }

            EndSpriteBatch();
            var data = new int[sScreenShotRenderTexture.Width * sScreenShotRenderTexture.Height];
            sScreenShotRenderTexture.GetData(
                0,
                new Microsoft.Xna.Framework.Rectangle(
                    0, 0, sScreenShotRenderTexture.Width, sScreenShotRenderTexture.Height
                ), data, 0, sScreenShotRenderTexture.Width * sScreenShotRenderTexture.Height
            );

            var bitmap = new Bitmap(sScreenShotRenderTexture.Width, sScreenShotRenderTexture.Height);
            var bits = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb
            );

            unsafe
            {
                var dstPointer = (byte*) bits.Scan0;
                for (var y = 0; y < sScreenShotRenderTexture.Height; ++y)
                {
                    for (var x = 0; x < sScreenShotRenderTexture.Width; ++x)
                    {
                        var bitmapColor = System.Drawing.Color.FromArgb(data[y * sScreenShotRenderTexture.Width + x]);
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
            float ecTime = Globals.System.GetTimeMs() - sFogUpdateTime;
            sFogUpdateTime = Globals.System.GetTimeMs();
            if (string.IsNullOrWhiteSpace(Globals.CurrentMap.Fog))
            {
                return;
            }

            var fogTex = GameContentManager.GetTexture(GameContentManager.TextureType.Fog, Globals.CurrentMap.Fog);
            if (fogTex == null)
            {
                return;
            }

            var xCount = Globals.MapEditorWindow.picMap.Width / fogTex.Width + 1;
            var yCount = Globals.MapEditorWindow.picMap.Height / fogTex.Height + 1;

            sFogCurrentX += ecTime / 1000f * Globals.CurrentMap.FogXSpeed * 2;
            sFogCurrentY += ecTime / 1000f * Globals.CurrentMap.FogYSpeed * 2;

            if (sFogCurrentX < fogTex.Width)
            {
                sFogCurrentX += fogTex.Width;
            }

            if (sFogCurrentX > fogTex.Width)
            {
                sFogCurrentX -= fogTex.Width;
            }

            if (sFogCurrentY < fogTex.Height)
            {
                sFogCurrentY += fogTex.Height;
            }

            if (sFogCurrentY > fogTex.Height)
            {
                sFogCurrentY -= fogTex.Height;
            }

            var drawX = (float) Math.Round(sFogCurrentX);
            var drawY = (float) Math.Round(sFogCurrentY);

            for (var x = -1; x < xCount; x++)
            {
                for (var y = -1; y < yCount; y++)
                {
                    DrawTexture(
                        fogTex, new RectangleF(0, 0, fogTex.Width, fogTex.Height),
                        new RectangleF(
                            x * fogTex.Width + drawX, y * fogTex.Height + drawY, fogTex.Width, fogTex.Height
                        ), System.Drawing.Color.FromArgb(Globals.CurrentMap.FogTransparency, 255, 255, 255), target
                    );
                }
            }
        }

        //Lighting
        private static void ClearDarknessTexture(RenderTarget2D target, bool screenShotting = false)
        {
            if (DarknessTexture == null)
            {
                DarknessTexture = CreateRenderTexture(
                    Options.TileWidth * Options.MapWidth * 3, Options.TileHeight * Options.MapHeight * 3
                );
            }

            SetRenderTarget(DarknessTexture);
            sGraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
            SetRenderTarget(null);

            if (Globals.CurrentMap == null)
            {
                return;
            }

            var tmpMap = Globals.CurrentMap;
            if (tmpMap == null)
            {
                return;
            }

            if (screenShotting)
            {
                var gridLightColor = System.Drawing.Color.FromArgb(Database.GridLightColor);
                if (!tmpMap.IsIndoors)
                {
                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1),
                        new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
                        System.Drawing.Color.FromArgb(255, 255, 255, 255), DarknessTexture, BlendState.Additive
                    );

                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1),
                        new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
                        System.Drawing.Color.FromArgb(
                            gridLightColor.A, gridLightColor.R, gridLightColor.G, gridLightColor.B
                        ), DarknessTexture, BlendState.NonPremultiplied
                    );
                }
                else
                {
                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1),
                        new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
                        System.Drawing.Color.FromArgb((byte) ((float) tmpMap.Brightness / 100f * 255f), 255, 255, 255),
                        DarknessTexture, BlendState.Additive
                    );
                }
            }
            else
            {
                if (!tmpMap.IsIndoors && LightColor != null)
                {
                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1),
                        new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
                        System.Drawing.Color.FromArgb(255, 255, 255, 255), DarknessTexture, BlendState.Additive
                    );

                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1),
                        new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
                        System.Drawing.Color.FromArgb(LightColor.A, LightColor.R, LightColor.G, LightColor.B),
                        DarknessTexture, BlendState.NonPremultiplied
                    );

                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, 32, 32),
                        System.Drawing.Color.FromArgb(255, 255, 0, 0), DarknessTexture, BlendState.NonPremultiplied
                    );

                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(0, DarknessTexture.Height - 32, 32, 32),
                        System.Drawing.Color.FromArgb(255, 255, 0, 0), DarknessTexture, BlendState.NonPremultiplied
                    );

                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(DarknessTexture.Width - 32, 0, 32, 32),
                        System.Drawing.Color.FromArgb(255, 255, 0, 0), DarknessTexture, BlendState.NonPremultiplied
                    );

                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1),
                        new RectangleF(DarknessTexture.Width - 32, DarknessTexture.Height - 32, 32, 32),
                        System.Drawing.Color.FromArgb(255, 255, 0, 0), DarknessTexture, BlendState.NonPremultiplied
                    );
                }
                else if (tmpMap.IsIndoors)
                {
                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1),
                        new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
                        System.Drawing.Color.FromArgb((byte) ((float) tmpMap.Brightness / 100f * 255f), 255, 255, 255),
                        DarknessTexture, BlendState.Additive
                    );
                }
                else
                {
                    DrawTexture(
                        sWhiteTex, new RectangleF(0, 0, 1, 1),
                        new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
                        System.Drawing.Color.FromArgb(255, 255, 255, 255), DarknessTexture, BlendState.Additive
                    );
                }
            }

            DrawLights(target);
        }

        private static void OverlayDarkness(RenderTarget2D target, bool screenShotting = false)
        {
            if (DarknessTexture == null)
            {
                return;
            }

            var tmpMap = Globals.CurrentMap;
            if (TilePreviewStruct != null)
            {
                tmpMap = TilePreviewStruct;
            }

            DrawTexture(
                DarknessTexture, new RectangleF(0, 0, DarknessTexture.Width, DarknessTexture.Height),
                new RectangleF(
                    CurrentView.Left - Options.MapWidth * Options.TileWidth,
                    CurrentView.Top - Options.MapHeight * Options.TileHeight, DarknessTexture.Width,
                    DarknessTexture.Height
                ), System.Drawing.Color.FromArgb(255, 255, 255, 255), target, MultiplyState
            );

            ////Draw Light Attribute Icons
            if (Globals.CurrentLayer != LayerOptions.Lights)
            {
                return;
            }

            if (!screenShotting)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        if (tmpMap.FindLightAt(x, y) == null)
                        {
                            continue;
                        }

                        var lightTex = GameContentManager.GetTexture(
                            GameContentManager.TextureType.Misc, "lighticon.png"
                        );

                        if (lightTex != null)
                        {
                            DrawTexture(
                                lightTex, new RectangleF(0, 0, lightTex.Width, lightTex.Height),
                                new RectangleF(
                                    x * Options.TileWidth + Options.MapWidth * Options.TileWidth * 0 + CurrentView.Left,
                                    y * Options.TileHeight +
                                    Options.MapHeight * Options.TileHeight * 0 +
                                    CurrentView.Top, Options.TileWidth, Options.TileHeight
                                ), System.Drawing.Color.White, target
                            );
                        }
                    }
                }

                DrawBoxOutline(
                    CurrentView.Left + Globals.CurTileX * Options.TileWidth,
                    CurrentView.Top + Globals.CurTileY * Options.TileHeight, Options.TileWidth, Options.TileHeight,
                    System.Drawing.Color.White, target
                );
            }
        }

        private static void DrawLights(RenderTarget2D target = null)
        {
            foreach (var light in sLightQueue)
            {
                var x = light.Key.X;
                var y = light.Key.Y;
                DrawLight(x, y, light.Value, DarknessTexture);
            }

            sLightQueue.Clear();
        }

        public static void DrawLight(int x, int y, LightBase light, RenderTarget2D target)
        {
            var shader = GameContentManager.GetShader("radialgradient_editor.xnb");
            var vec = new Vector4(
                light.Color.R / 255f, light.Color.G / 255f, light.Color.B / 255f, light.Intensity / 255f
            );

            shader.Parameters["LightColor"].SetValue(vec);
            shader.Parameters["Expand"].SetValue((float) (light.Expand / 100f));
            y -= light.Size;
            x -= light.Size;
            DrawTexture(
                sWhiteTex, new RectangleF(0, 0, 1, 1), new RectangleF(x, y, light.Size * 2, light.Size * 2),
                System.Drawing.Color.White, target, BlendState.Additive, shader
            );
        }

        public static void AddLight(int x, int y, LightBase light, RenderTarget2D target = null)
        {
            if (target == null)
            {
                target = DarknessTexture;
            }

            sLightQueue.Add(
                new KeyValuePair<Microsoft.Xna.Framework.Point, LightBase>(
                    new Microsoft.Xna.Framework.Point(x, y), light
                )
            );
        }

        //Rendering
        //Rendering Functions
        public static void DrawTexture(Texture2D tex, float x, float y, RenderTarget2D renderTarget2D)
        {
            var destRectangle = new RectangleF(x, y, (int) tex.Width, (int) tex.Height);
            var srcRectangle = new RectangleF(0, 0, (int) tex.Width, (int) tex.Height);
            DrawTexture(tex, srcRectangle, destRectangle, renderTarget2D);
        }

        public static void DrawTexture(
            Texture2D tex,
            float x,
            float y,
            RenderTarget2D renderTarget2D,
            BlendState blendMode
        )
        {
            var destRectangle = new RectangleF(x, y, (int) tex.Width, (int) tex.Height);
            var srcRectangle = new RectangleF(0, 0, (int) tex.Width, (int) tex.Height);
            DrawTexture(tex, srcRectangle, destRectangle, System.Drawing.Color.White, renderTarget2D, blendMode);
        }

        public static void DrawTexture(
            Texture2D tex,
            float dx,
            float dy,
            float sx,
            float sy,
            float w,
            float h,
            RenderTarget2D renderTarget2D
        )
        {
            var destRectangle = new RectangleF(dx, dy, w, h);
            var srcRectangle = new RectangleF(sx, sy, w, h);
            DrawTexture(tex, srcRectangle, destRectangle, renderTarget2D);
        }

        public static void DrawTexture(
            Texture2D tex,
            RectangleF srcRectangle,
            RectangleF targetRect,
            RenderTarget2D renderTarget2D
        )
        {
            DrawTexture(
                tex, srcRectangle, targetRect, System.Drawing.Color.White, renderTarget2D, BlendState.NonPremultiplied
            );
        }

        public static void DrawTexture(
            Texture2D tex,
            RectangleF srcRectangle,
            RectangleF targetRect,
            System.Drawing.Color renderColor,
            RenderTarget2D renderTarget = null,
            BlendState blendMode = null,
            Effect shader = null,
            float rotationDegrees = 0
        )
        {
            if (tex == null)
            {
                return;
            }

            if (blendMode == null)
            {
                blendMode = BlendState.NonPremultiplied;
            }

            if (renderTarget == null)
            {
                StartSpritebatch(blendMode, shader, null, false, null);
                sSpriteBatch.Draw(
                    tex,
                    new Microsoft.Xna.Framework.Rectangle(
                        (int) targetRect.X, (int) targetRect.Y, (int) targetRect.Width, (int) targetRect.Height
                    ),
                    new Microsoft.Xna.Framework.Rectangle(
                        (int) srcRectangle.X, (int) srcRectangle.Y, (int) srcRectangle.Width, (int) srcRectangle.Height
                    ), ConvertColor(renderColor), rotationDegrees, Vector2.Zero, SpriteEffects.None, 0
                );
            }
            else
            {
                StartSpritebatch(blendMode, shader, renderTarget, false, null);
                sSpriteBatch.Draw(
                    tex,
                    new Microsoft.Xna.Framework.Rectangle(
                        (int) targetRect.X, (int) targetRect.Y, (int) targetRect.Width, (int) targetRect.Height
                    ),
                    new Microsoft.Xna.Framework.Rectangle(
                        (int) srcRectangle.X, (int) srcRectangle.Y, (int) srcRectangle.Width, (int) srcRectangle.Height
                    ), ConvertColor(renderColor), rotationDegrees, Vector2.Zero, SpriteEffects.None, 0
                );
            }
        }

        //Extra MonoGame Stuff
        public static Microsoft.Xna.Framework.Color ConvertColor(System.Drawing.Color clr)
        {
            return new Microsoft.Xna.Framework.Color(
                new Vector4(clr.R / 255f, clr.G / 255f, clr.B / 255f, clr.A / 255f)
            );
        }

        private static void StartSpritebatch(
            BlendState mode = null,
            Effect shader = null,
            RenderTarget2D target = null,
            bool forced = false,
            RasterizerState rs = null
        )
        {
            if (sSpriteBatch.GraphicsDevice == null)
            {
                return;
            }

            var viewsDiff = false;
            if (mode == null)
            {
                mode = BlendState.NonPremultiplied;
            }

            if (mode != sCurrentBlendmode ||
                shader != sCurrentShader ||
                target != sCurrentTarget ||
                viewsDiff ||
                forced ||
                !sSpriteBatchBegan)
            {
                if (sSpriteBatchBegan)
                {
                    EndSpriteBatch();
                }

                if (target == null)
                {
                    SetRenderTarget(sMapEditorChain);
                }
                else
                {
                    SetRenderTarget(target);
                }

                if (shader != null)
                {
                    sSpriteBatch.Begin(SpriteSortMode.Immediate, mode, null, null, rs, shader);
                }
                else
                {
                    sSpriteBatch.Begin(SpriteSortMode.Deferred, mode, null, null, rs, shader);
                }

                sCurrentBlendmode = mode;
                sCurrentShader = shader;
                sCurrentTarget = target;
                sSpriteBatchBegan = true;
            }
        }

        public static void EndSpriteBatch()
        {
            if (!sSpriteBatchBegan)
            {
                return;
            }

            sSpriteBatch.End();
            sSpriteBatchBegan = false;
        }

    }

}
