using Intersect_Editor.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Intersect_Editor.Classes.General;
using Intersect_Editor.Classes.Maps;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;
using Microsoft.Xna.Framework.Graphics;
using WeifenLuo.WinFormsUI.Docking;
using EditorGraphics = Intersect_Editor.Classes.EditorGraphics;

namespace Intersect_Editor.Forms
{
    public partial class frmMapEditor : DockContent
    {
        //MonoGame Swap Chain
        private SwapChainRenderTarget _chain;

        //Map States
        public List<byte[]> MapUndoStates = new List<byte[]>();
        public List<byte[]> MapRedoStates = new List<byte[]>();
        public byte[] CurrentMapState;
        private bool MapChanged = false;

        //Init/Form Functions
        public frmMapEditor()
        {
            InitializeComponent();
        }
        private void frmMapEditor_Load(object sender, EventArgs e)
        {
            PacketHandler.MapUpdatedDelegate += InitMapEditor;
            picMap.Size = pnlMapContainer.ClientSize;
            picMap.MinimumSize = new Size((Options.MapWidth + 2) * Options.TileWidth, (Options.MapHeight + 2) * Options.TileHeight);
            EditorGraphics.CurrentView = new Rectangle((picMap.Size.Width - (Options.MapWidth) * Options.TileWidth) / 2, (picMap.Size.Height - (Options.MapHeight) * Options.TileHeight) / 2, picMap.Size.Width, picMap.Size.Height);
            CreateSwapChain();
        }

        public void InitMapEditor()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(PacketHandler.MapUpdatedDelegate);
            }
            else
            {
                pnlMapContainer.AutoScroll = true;
                picMap.Size = pnlMapContainer.ClientSize;
                picMap.MinimumSize = new Size((Options.MapWidth + 2) * Options.TileWidth, (Options.MapHeight + 2) * Options.TileHeight);
                EditorGraphics.CurrentView = new Rectangle((picMap.Size.Width - (Options.MapWidth) * Options.TileWidth) / 2, (picMap.Size.Height - (Options.MapHeight) * Options.TileHeight) / 2, picMap.Size.Width, picMap.Size.Height);
                CreateSwapChain();
                Globals.MapLayersWindow.RefreshNpcList();
                Globals.MapPropertiesWindow.Init(Globals.CurrentMap);
                if (Globals.MapEditorWindow.picMap.Visible) return;
                Globals.MapEditorWindow.picMap.Visible = true;
            }
        }

        private void CreateSwapChain()
        {
            if (!Globals.ClosingEditor)
            {
                if (_chain != null)
                {
                    _chain.Dispose();
                }
                if (EditorGraphics.GetGraphicsDevice() != null)
                {
                    if (this.picMap.Width > 0 && this.picMap.Height > 0)
                    {
                        _chain = new SwapChainRenderTarget(EditorGraphics.GetGraphicsDevice(), this.picMap.Handle,
                            this.picMap.Width, this.picMap.Height, false, SurfaceFormat.Color, DepthFormat.Depth24, 0,
                            RenderTargetUsage.DiscardContents, PresentInterval.Immediate);
                        EditorGraphics.SetMapEditorChain(_chain);
                    }
                }
            }
        }

        private void frmMapEditor_DockStateChanged(object sender, EventArgs e)
        {
            CreateSwapChain();
        }

        //Undo/Redo Functions
        public void ResetUndoRedoStates()
        {
            MapUndoStates.Clear();
            MapRedoStates.Clear();
            CurrentMapState = null;
        }
        public void PrepUndoState()
        {
            var tmpMap = Globals.CurrentMap;
            if (CurrentMapState == null)
            {
                CurrentMapState = tmpMap.GetMapData(false);
            }
        }
        public void AddUndoState()
        {
            var tmpMap = Globals.CurrentMap;
            if (CurrentMapState != null)
            {
                MapUndoStates.Add(CurrentMapState);
                MapRedoStates.Clear();
                CurrentMapState = tmpMap.GetMapData(false);
            }
            MapChanged = false;
        }

        //PicMap Functions
        public void picMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Globals.MapEditorWindow.DockPanel.Focused)
            {
                Globals.MapEditorWindow.DockPanel.Focus();
                return;
            }
            if (Globals.EditingLight != null) { return; }
            var tmpMap = Globals.CurrentMap;

            if (e.X < EditorGraphics.CurrentView.Left || e.Y < EditorGraphics.CurrentView.Top ||
                e.X > EditorGraphics.CurrentView.Left + Options.MapWidth * Options.TileWidth ||
                e.Y > EditorGraphics.CurrentView.Top + Options.MapHeight * Options.TileHeight) return;

            if (CurrentMapState == null)
            {
                CurrentMapState = tmpMap.GetMapData(false);
            }



            switch (e.Button)
            {
                case MouseButtons.Left:
                    Globals.MouseButton = 0;
                    if (Globals.CurrentTool == (int)EdittingTool.Droppler)
                    {
                        for (int i = Globals.CurrentLayer; i >= 0; i--)
                        {
                            if (tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex > -1)
                            {
                                Globals.MapLayersWindow.SetTileset(
                                    TilesetBase.GetName(tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex));
                                Globals.MapLayersWindow.SetAutoTile(
                                    tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile);
                                Globals.CurSelX = tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].X;
                                Globals.CurSelY = tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].Y;
                                Globals.CurrentTool = (int)EdittingTool.Pen;
                                Globals.MapLayersWindow.SetLayer(i);
                                break;
                            }
                        }
                        if (Globals.CurrentTool == (int)EdittingTool.Droppler)
                        {
                            //Could not find a tile from the selected level downward, maybe they didnt opt for a specific layer?
                            for (int i = 4; i >= 0; i--)
                            {
                                if (tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex > -1)
                                {
                                    Globals.MapLayersWindow.SetTileset(
                                        TilesetBase.GetName(tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex));
                                    Globals.MapLayersWindow.SetAutoTile(
                                        tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile);
                                    Globals.CurSelX = tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].X;
                                    Globals.CurSelY = tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].Y;
                                    Globals.CurrentTool = (int)EdittingTool.Pen;
                                    Globals.MapLayersWindow.SetLayer(i);
                                    break;
                                }
                            }
                        }
                        return;
                    }
                    else if (Globals.CurrentTool == (int)EdittingTool.Selection)
                    {
                        if (Globals.Dragging == true)
                        {
                            if (Globals.CurTileX >= Globals.CurMapSelX + Globals.TotalTileDragX &&
                                Globals.CurTileX <= Globals.CurMapSelX + Globals.TotalTileDragX + Globals.CurMapSelW &&
                                Globals.CurTileY >= Globals.CurMapSelY + Globals.TotalTileDragY &&
                                Globals.CurTileY <= Globals.CurMapSelY + Globals.TotalTileDragY + Globals.CurMapSelH)
                            {
                                Globals.TileDragX = Globals.CurTileX;
                                Globals.TileDragY = Globals.CurTileY;
                                return;
                            }
                            else
                            {
                                //Place the change, we done!
                                ProcessSelectionMovement(Globals.CurrentMap, true);
                                PlaceSelection();
                            }

                        }
                        else
                        {
                            if (Globals.CurTileX >= Globals.CurMapSelX &&
                                Globals.CurTileX <= Globals.CurMapSelX + Globals.CurMapSelW &&
                                Globals.CurTileY >= Globals.CurMapSelY &&
                                Globals.CurTileY <= Globals.CurMapSelY + Globals.CurMapSelH)
                            {
                                Globals.Dragging = true;
                                Globals.TileDragX = Globals.CurTileX;
                                Globals.TileDragY = Globals.CurTileY;
                                Globals.TotalTileDragX = 0;
                                Globals.TotalTileDragY = 0;
                                Globals.SelectionSource = Globals.CurrentMap;
                                return;
                            }
                            else
                            {
                                Globals.CurMapSelX = Globals.CurTileX;
                                Globals.CurMapSelY = Globals.CurTileY;
                                Globals.CurMapSelW = 0;
                                Globals.CurMapSelH = 0;
                                return;
                            }
                        }
                    }
                    else if (Globals.CurrentTool == (int)EdittingTool.Rectangle)
                    {
                        Globals.CurMapSelX = Globals.CurTileX;
                        Globals.CurMapSelY = Globals.CurTileY;
                        Globals.CurMapSelW = 0;
                        Globals.CurMapSelH = 0;
                    }
                    else
                    {
                        if (Globals.CurrentLayer == Options.LayerCount) //Attributes
                        {
                            Globals.MapLayersWindow.PlaceAttribute(Globals.CurrentMap,
                                    Globals.CurTileX, Globals.CurTileY);
                            MapChanged = true;
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
                        else
                        {
                            if (Globals.CurrentTileset == null) return;
                            if (Globals.Autotilemode == 0)
                            {
                                for (var x = 0; x <= Globals.CurSelW; x++)
                                {
                                    for (var y = 0; y <= Globals.CurSelH; y++)
                                    {
                                        if (Globals.CurTileX + x >= 0 && Globals.CurTileX + x < Options.MapWidth &&
                                            Globals.CurTileY + y >= 0 &&
                                            Globals.CurTileY + y < Options.MapHeight)
                                        {
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].TilesetIndex =
                                                Globals.CurrentTileset.GetId();
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].X =
                                                Globals.CurSelX + x;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].Y =
                                                Globals.CurSelY + y;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].Autotile = 0;
                                            tmpMap.InitAutotiles();
                                        }
                                    }
                                }
                                MapChanged = true;
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY]
                                    .TilesetIndex = Globals.CurrentTileset.GetId();
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X
                                    = Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y
                                    = Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY]
                                    .Autotile = (byte)Globals.Autotilemode;
                                tmpMap.InitAutotiles();
                                MapChanged = true;
                            }
                        }
                    }
                    break;
                case MouseButtons.Right:
                    Globals.MouseButton = 1;


                    if (Globals.CurrentTool == (int)EdittingTool.Selection)
                    {
                        if (Globals.Dragging)
                        {
                            //Place the change, we done!
                            ProcessSelectionMovement(Globals.CurrentMap, true);
                            PlaceSelection();
                        }
                    }
                    if (Globals.CurrentLayer == Options.LayerCount) //Attributes
                    {
                        if (Globals.CurrentTool == (int)EdittingTool.Pen)
                        {
                            if (Globals.MapLayersWindow.RemoveAttribute(Globals.CurrentMap, Globals.CurTileX, Globals.CurTileY)) { MapChanged = true; }
                        }
                        else if (Globals.CurrentTool == (int)EdittingTool.Rectangle || Globals.CurrentTool == (int)EdittingTool.Selection)
                        {
                            Globals.CurMapSelX = Globals.CurTileX;
                            Globals.CurMapSelY = Globals.CurTileY;
                            Globals.CurMapSelW = 0;
                            Globals.CurMapSelH = 0;
                        }
                    }
                    else if (Globals.CurrentLayer == Options.LayerCount + 1) //Lights
                    {
                        LightBase tmpLight;
                        if ((tmpLight = Globals.CurrentMap.FindLightAt(Globals.CurTileX, Globals.CurTileY)) != null)
                        {
                            Globals.CurrentMap.Lights.Remove(tmpLight);
                            EditorGraphics.TilePreviewUpdated = true;
                            MapChanged = true;
                        }
                    }
                    else if (Globals.CurrentLayer == Options.LayerCount + 2) //Events
                    {
                        EventBase tmpEvent;
                        if ((tmpEvent = Globals.CurrentMap.FindEventAt(Globals.CurTileX, Globals.CurTileY)) != null)
                        {
                            Globals.CurrentMap.Events.Remove(tmpEvent.MyIndex);
                            MapChanged = true;
                        }
                    }
                    else if (Globals.CurrentLayer == Options.LayerCount + 3) //NPCS
                    {

                    }
                    else
                    {
                        if (Globals.CurrentTool == (int)EdittingTool.Pen)
                        {
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                            tmpMap.InitAutotiles();
                            MapChanged = true;
                        }
                        else if (Globals.CurrentTool == (int)EdittingTool.Rectangle || Globals.CurrentTool == (int)EdittingTool.Selection)
                        {
                            Globals.CurMapSelX = Globals.CurTileX;
                            Globals.CurMapSelY = Globals.CurTileY;
                            Globals.CurMapSelW = 0;
                            Globals.CurMapSelH = 0;
                        }
                    }
                    break;
            }
            if (Globals.CurTileX == 0)
            {
                if (MapInstance.GetMap(tmpMap.Left) != null)
                    MapInstance.GetMap(tmpMap.Left).InitAutotiles();
            }
            if (Globals.CurTileY == 0)
            {
                if (MapInstance.GetMap(tmpMap.Up) != null)
                    MapInstance.GetMap(tmpMap.Up).InitAutotiles();
            }
            if (Globals.CurTileX == Options.MapWidth - 1)
            {
                if (MapInstance.GetMap(tmpMap.Right) != null)
                    MapInstance.GetMap(tmpMap.Right).InitAutotiles();
            }
            if (Globals.CurTileY == Options.MapHeight - 1)
            {
                if (MapInstance.GetMap(tmpMap.Down) != null)
                    MapInstance.GetMap(tmpMap.Down).InitAutotiles();
            }
        }
        private void picMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (Globals.EditingLight != null) { return; }

            if (e.Button == MouseButtons.Middle)
            {
                EditorGraphics.CurrentView.X -= (Globals.MouseX - e.X);
                EditorGraphics.CurrentView.Y -= (Globals.MouseY - e.Y);
                if (EditorGraphics.CurrentView.X > Options.MapWidth*Options.TileWidth)
                    EditorGraphics.CurrentView.X = Options.MapWidth*Options.TileWidth;
                if (EditorGraphics.CurrentView.Y > Options.MapHeight * Options.TileHeight)
                    EditorGraphics.CurrentView.Y = Options.MapHeight * Options.TileHeight;
                if (EditorGraphics.CurrentView.X - picMap.Width < -Options.TileWidth*Options.MapWidth*2)
                {
                    EditorGraphics.CurrentView.X = -Options.TileWidth * Options.MapWidth * 2  + picMap.Width;
                }
                if (EditorGraphics.CurrentView.Y - picMap.Height < -Options.TileHeight * Options.MapHeight * 2)
                {
                    EditorGraphics.CurrentView.Y = -Options.TileHeight * Options.MapHeight * 2 + picMap.Height;
                }
            }

            Globals.MouseX = e.X;
            Globals.MouseY = e.Y;

            if (e.X < EditorGraphics.CurrentView.Left ||e.Y < EditorGraphics.CurrentView.Top ||
                e.X > EditorGraphics.CurrentView.Left + Options.MapWidth*Options.TileWidth ||
                e.Y > EditorGraphics.CurrentView.Top + Options.MapHeight*Options.TileHeight) return;
            int oldx = Globals.CurTileX;
            int oldy = Globals.CurTileY;
            Globals.CurTileX = (int)Math.Floor((double)(e.X - EditorGraphics.CurrentView.Left) / Options.TileWidth);
            Globals.CurTileY = (int)Math.Floor((double)(e.Y - EditorGraphics.CurrentView.Top) / Options.TileHeight);
            if (Globals.CurTileX < 0) { Globals.CurTileX = 0; }
            if (Globals.CurTileY < 0) { Globals.CurTileY = 0; }
            if (Globals.CurTileX >= Options.MapWidth) Globals.CurTileX = Options.MapWidth - 1;
            if (Globals.CurTileY >= Options.MapHeight) Globals.CurTileY = Options.MapHeight - 1;

            if (oldx != Globals.CurTileX || oldy != Globals.CurTileY)
            {
                EditorGraphics.TilePreviewUpdated = true;
            }

            if (Globals.MouseButton > -1)
            {
                var tmpMap = Globals.CurrentMap;
                if (Globals.MouseButton == 0)
                {
                    if (Globals.CurrentTool == (int)EdittingTool.Selection)
                    {
                        if (!Globals.Dragging)
                        {
                            Globals.CurMapSelW = Globals.CurTileX - Globals.CurMapSelX;
                            Globals.CurMapSelH = Globals.CurTileY - Globals.CurMapSelY;
                        }
                    }
                    else if (Globals.CurrentTool == (int)EdittingTool.Rectangle)
                    {
                        Globals.CurMapSelW = Globals.CurTileX - Globals.CurMapSelX;
                        Globals.CurMapSelH = Globals.CurTileY - Globals.CurMapSelY;
                    }
                    else
                    {
                        if (Globals.CurrentLayer == Options.LayerCount)
                        {
                            Globals.MapLayersWindow.PlaceAttribute(tmpMap, Globals.CurTileX, Globals.CurTileY);
                        }
                        else if (Globals.CurrentLayer == Options.LayerCount + 1)
                        {

                        }
                        else if (Globals.CurrentLayer == Options.LayerCount + 2)
                        {

                        }
                        else if (Globals.CurrentLayer == Options.LayerCount + 3)
                        {
                            if (Globals.MapLayersWindow.rbDeclared.Checked == true &&
                                Globals.MapLayersWindow.lstMapNpcs.Items.Count > 0)
                            {
                                Globals.CurrentMap.Spawns[
                                    Globals.MapLayersWindow.lstMapNpcs.SelectedIndex
                                    ].X = Globals.CurTileX;
                                Globals.CurrentMap.Spawns[
                                    Globals.MapLayersWindow.lstMapNpcs.SelectedIndex
                                    ].Y = Globals.CurTileY;
                            }
                        }
                        else
                        {
                            if (Globals.CurrentTileset == null) return;
                            if (Globals.Autotilemode == 0)
                            {
                                for (var x = 0; x <= Globals.CurSelW; x++)
                                {
                                    for (var y = 0; y <= Globals.CurSelH; y++)
                                    {
                                        if (Globals.CurTileX + x >= 0 && Globals.CurTileX + x < Options.MapWidth &&
                                                Globals.CurTileY + y >= 0 &&
                                                Globals.CurTileY + y < Options.MapHeight)
                                        {
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].TilesetIndex =
                                                Globals.CurrentTileset.GetId();
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].X = Globals.CurSelX + x;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].Y = Globals.CurSelY + y;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].Autotile = 0;
                                            tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX + x, Globals.CurTileY + y,
                                                Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY]
                                    .TilesetIndex = Globals.CurrentTileset.GetId();
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X =
                                    Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y =
                                    Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY]
                                    .Autotile =
                                    (byte)Globals.Autotilemode;
                                tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX, Globals.CurTileY,
                                    Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                            }
                        }
                        if (Globals.CurTileX == 0)
                        {
                            if (MapInstance.GetMap(tmpMap.Left) != null)
                                MapInstance.GetMap(tmpMap.Left).InitAutotiles();
                        }
                        if (Globals.CurTileY == 0)
                        {
                            if (MapInstance.GetMap(tmpMap.Up) != null)
                                MapInstance.GetMap(tmpMap.Up).InitAutotiles();
                        }
                        if (Globals.CurTileX == Options.MapWidth - 1)
                        {
                            if (MapInstance.GetMap(tmpMap.Right) != null)
                                MapInstance.GetMap(tmpMap.Right).InitAutotiles();
                        }
                        if (Globals.CurTileY == Options.MapHeight - 1)
                        {
                            if (MapInstance.GetMap(tmpMap.Down) != null)
                                MapInstance.GetMap(tmpMap.Down).InitAutotiles();
                        }
                    }
                }
                else if (Globals.MouseButton == 1)
                {
                    if (Globals.CurrentTool == (int)EdittingTool.Rectangle)
                    {
                        Globals.CurMapSelW = Globals.CurTileX - Globals.CurMapSelX;
                        Globals.CurMapSelH = Globals.CurTileY - Globals.CurMapSelY;
                    }
                    else if (Globals.CurrentTool == (int)EdittingTool.Pen)
                    {
                        if (Globals.CurrentLayer == Options.LayerCount)
                        {
                            Globals.MapLayersWindow.RemoveAttribute(tmpMap, Globals.CurTileX, Globals.CurTileY);
                        }
                        else if (Globals.CurrentLayer < Options.LayerCount)
                        {
                            if (Globals.CurrentTool == (int)EdittingTool.Pen)
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex =
                                    -1;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                                tmpMap.InitAutotiles();
                            }
                        }
                    }
                    if (Globals.CurTileX == 0)
                    {
                        if (MapInstance.GetMap(tmpMap.Left) != null)
                            MapInstance.GetMap(tmpMap.Left).InitAutotiles();
                    }
                    if (Globals.CurTileY == 0)
                    {
                        if (MapInstance.GetMap(tmpMap.Up) != null)
                            MapInstance.GetMap(tmpMap.Up).InitAutotiles();
                    }
                    if (Globals.CurTileX == Options.MapWidth - 1)
                    {
                        if (MapInstance.GetMap(tmpMap.Right) != null)
                            MapInstance.GetMap(tmpMap.Right).InitAutotiles();
                    }
                    if (Globals.CurTileY == Options.MapHeight - 1)
                    {
                        if (MapInstance.GetMap(tmpMap.Down) != null)
                            MapInstance.GetMap(tmpMap.Down).InitAutotiles();
                    }
                }
            }
            else
            {
                if (Globals.CurrentTool != (int)EdittingTool.Selection)
                {
                    Globals.CurMapSelX = Globals.CurTileX;
                    Globals.CurMapSelY = Globals.CurTileY;
                }
            }
        }
        public void picMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (Globals.EditingLight != null) { return; }
            var tmpMap = Globals.CurrentMap;
            if (Globals.CurrentTool == (int)EdittingTool.Rectangle)
            {
                int selX = Globals.CurMapSelX, selY = Globals.CurMapSelY, selW = Globals.CurMapSelW, selH = Globals.CurMapSelH;
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
                Globals.CurMapSelX = selX;
                Globals.CurMapSelY = selY;
                Globals.CurMapSelW = selW;
                Globals.CurMapSelH = selH;

                if (Globals.CurrentLayer == Options.LayerCount) //Attributes
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
                    MapChanged = true;
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
                else
                {
                    int x1 = 0;
                    int y1 = 0;
                    for (int x0 = selX; x0 < selX + selW + 1; x0++)
                    {
                        for (int y0 = selY; y0 < selY + selH + 1; y0++)
                        {
                            if (Globals.Autotilemode == 0)
                            {
                                x1 = (x0 - selX) % (Globals.CurSelW + 1);
                                y1 = (y0 - selY) % (Globals.CurSelH + 1);
                                if (x0 >= 0 && x0 < Options.MapWidth && y0 >= 0 && y0 < Options.MapHeight && x0 < selX + selW + 1 && y0 < selY + selH + 1)
                                {
                                    if (Globals.MouseButton == 0)
                                    {
                                        if (Globals.CurrentTileset != null)
                                        {
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].TilesetIndex =
                                                Globals.CurrentTileset.GetId();
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].X = Globals.CurSelX + x1;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Y = Globals.CurSelY + y1;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Autotile =
                                                (byte) Globals.Autotilemode;
                                        }
                                    }
                                    else if (Globals.MouseButton == 1)
                                    {
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].TilesetIndex = -1;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].X = 0;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Y = 0;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Autotile = 0;
                                    }
                                    tmpMap.Autotiles.UpdateAutoTiles(x0, y0, Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                                }
                            }
                            else
                            {
                                if (Globals.MouseButton == 0)
                                {
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].TilesetIndex = Globals.CurrentTileset.GetId();
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].X = Globals.CurSelX;
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Y = Globals.CurSelY;
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Autotile = (byte)Globals.Autotilemode;
                                }
                                else if (Globals.MouseButton == 1)
                                {
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].TilesetIndex = -1;
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].X = 0;
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Y = 0;
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Autotile = 0;
                                }
                                tmpMap.Autotiles.UpdateAutoTiles(x0, y0,
                                    Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                            }
                        }
                    }
                    MapChanged = true;
                }
            }
            Globals.MouseButton = -1;
            if (MapChanged)
            {
                if (CurrentMapState != null) MapUndoStates.Add(CurrentMapState);
                MapRedoStates.Clear();
                CurrentMapState = tmpMap.GetMapData(false);
                MapChanged = false;
            }
            if (Globals.CurrentTool != (int)EdittingTool.Selection)
            {
                Globals.CurMapSelX = Globals.CurTileX;
                Globals.CurMapSelY = Globals.CurTileY;
                Globals.CurMapSelW = 0;
                Globals.CurMapSelH = 0;
            }
            //Globals.Dragging = false;
            if (Globals.Dragging)
            {
                Globals.TotalTileDragX -= (Globals.TileDragX - Globals.CurTileX);
                Globals.TotalTileDragY -= (Globals.TileDragY - Globals.CurTileY);
                Globals.TileDragX = 0;
                Globals.TileDragY = 0;
            }
            EditorGraphics.TilePreviewUpdated = true;
        }
        private void picMap_DoubleClick(object sender, EventArgs e)
        {
            var dir = -1;
            var newMap = -1;

            if (Globals.MouseX < 0 || Globals.MouseX > picMap.Width || Globals.MouseY < 0 ||
                Globals.MouseY > picMap.Height) return;

            if (Globals.Dragging == true)
            {
                //Place the change, we done!
                Globals.MapEditorWindow.ProcessSelectionMovement(Globals.CurrentMap, true);
                Globals.MapEditorWindow.PlaceSelection();
            }

            //Check Left Column of Maps
            var gridX = 0;
            var gridY = 0;
            if (Globals.MouseX < EditorGraphics.CurrentView.Left) gridX = -1;
            if (Globals.MouseX > EditorGraphics.CurrentView.Left + Options.MapWidth*Options.TileWidth) gridX = 1;
            if (Globals.MouseY < EditorGraphics.CurrentView.Top) gridY = -1;
            if (Globals.MouseY > EditorGraphics.CurrentView.Top + Options.MapHeight*Options.TileHeight) gridY = 1;

            if (gridX != 0 || gridY != 0)
            {
                if (gridX == -1 && gridY == 0)
                {
                    dir = (int)Directions.Left;
                }
                else if (gridX == 1 && gridY == 0)
                {
                    dir = (int)Directions.Right;
                }
                else if (gridX == 0 && gridY == -1)
                {
                    dir = (int)Directions.Up;
                }
                else if (gridX == 0 && gridY == 1)
                {
                    dir = (int) Directions.Down;
                }
                if (dir != -1)
                {
                    if (Globals.MapGrid != null && Globals.MapGrid.Contains(Globals.CurrentMap.MyMapNum))
                    {
                        var x = gridX + Globals.CurrentMap.MapGridX;
                        var y = gridY + Globals.CurrentMap.MapGridY;
                        if (x >= 0 && y >= 0 && x < Globals.MapGrid.GridWidth && y < Globals.MapGrid.GridHeight)
                        {
                            if (Globals.MapGrid.Grid[x, y].mapnum > -1)
                            {
                                newMap = Globals.MapGrid.Grid[x, y].mapnum;
                            }
                        }
                    }
                }
                else
                {
                    if (Globals.MapGrid != null && Globals.MapGrid.Contains(Globals.CurrentMap.MyMapNum))
                    {
                        var x = gridX + Globals.CurrentMap.MapGridX;
                        var y = gridY + Globals.CurrentMap.MapGridY;
                        if (x >= 0 && y >= 0 && x < Globals.MapGrid.GridWidth && y < Globals.MapGrid.GridHeight)
                        {
                            if (Globals.MapGrid.Grid[x, y].mapnum > -1)
                            {
                                if (Globals.CurrentMap.Changed() && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    SaveMap();
                                }
                                Globals.MainForm.EnterMap(Globals.MapGrid.Grid[x, y].mapnum);
                            }
                            else
                            {
                                MessageBox.Show(@"Cannot create maps diagonally!", @"Create new map.",MessageBoxButtons.OK);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show(@"Cannot create maps diagonally!", @"Create new map.", MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
                if (dir > -1)
                {
                    if (newMap == -1)
                    {
                        if (
                            MessageBox.Show(@"Do you want to create a map here?", @"Create new map.", MessageBoxButtons.YesNo) !=
                            DialogResult.Yes) return;
                        if (Globals.CurrentMap.Changed() && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SaveMap();
                        }
                        PacketSender.SendCreateMap(dir, Globals.CurrentMap.GetId(), null);
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (Globals.CurrentMap.Changed() && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SaveMap();
                        }
                        Globals.MainForm.EnterMap(newMap);
                    }
                    return;
                }
                return;
            }

            //See if we should edit an event, light, npc, etc
            if (Globals.CurrentLayer == Options.LayerCount) //Attributes
            {

            }
            else if (Globals.CurrentLayer == Options.LayerCount + 1) //Lights
            {
                LightBase tmpLight;
                if (
                    (tmpLight =
                        Globals.CurrentMap.FindLightAt(Globals.CurTileX,
                            Globals.CurTileY)) == null)
                {
                    tmpLight = new LightBase(Globals.CurTileX, Globals.CurTileY);
                    tmpLight.Size = 50;
                    Globals.CurrentMap.Lights.Add(tmpLight);
                }
                Globals.MapLayersWindow.tabControl.SelectedTab = Globals.MapLayersWindow.tabLights;
                Globals.MapLayersWindow.lightEditor.Show();
                Globals.BackupLight = new LightBase(tmpLight);
                Globals.MapLayersWindow.lightEditor.LoadEditor(tmpLight);
                Globals.EditingLight = tmpLight;
                MapChanged = true;
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 2) //Events
            {
                EventBase tmpEvent;
                FrmEvent tmpEventEditor;
                if (
                    (tmpEvent =
                        Globals.CurrentMap.FindEventAt(Globals.CurTileX,
                            Globals.CurTileY)) == null)
                {
                    tmpEvent = new EventBase(Globals.CurrentMap.EventIndex, Globals.CurTileX, Globals.CurTileY);
                    Globals.CurrentMap.Events.Add(Globals.CurrentMap.EventIndex, tmpEvent);
                    Globals.CurrentMap.EventIndex++;
                    tmpEventEditor = new FrmEvent(Globals.CurrentMap)
                    {
                        MyEvent = tmpEvent,
                        MyMap = Globals.CurrentMap,
                        NewEvent = true
                    };
                    tmpEventEditor.InitEditor();
                    tmpEventEditor.Show();
                    MapChanged = true;
                }
                else
                {
                    tmpEventEditor = new FrmEvent(Globals.CurrentMap) { MyEvent = tmpEvent, MyMap = Globals.CurrentMap };
                    tmpEventEditor.InitEditor();
                    tmpEventEditor.Show();
                }
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 3) //NPCS
            {
                if (Globals.MapLayersWindow.lstMapNpcs.SelectedIndex > -1 &&
                        Globals.MapLayersWindow.rbDeclared.Checked == true)
                {
                    Globals.CurrentMap.Spawns[
                        Globals.MapLayersWindow.lstMapNpcs.SelectedIndex].X = Globals.CurTileX;
                    Globals.CurrentMap.Spawns[
                        Globals.MapLayersWindow.lstMapNpcs.SelectedIndex].Y = Globals.CurTileY;
                    MapChanged = true;
                }
            }
            else
            {

            }
            Globals.Dragging = false;
            Globals.TotalTileDragX = 0;
            Globals.TotalTileDragY = 0;
            Globals.MouseButton = -1;
            Globals.SelectionSource = null;
            EditorGraphics.TilePreviewUpdated = true;
        }
        private void picMap_MouseEnter(object sender, EventArgs e)
        {
            if (Globals.MainForm.Focused) { pnlMapContainer.Focus(); }
        }

        private void SaveMap()
        {
            if (Globals.CurrentTool == (int)EdittingTool.Selection)
            {
                if (Globals.Dragging == true)
                {
                    //Place the change, we done!
                    Globals.MapEditorWindow.ProcessSelectionMovement(Globals.CurrentMap, true);
                    Globals.MapEditorWindow.PlaceSelection();
                }
            }
            PacketSender.SendMap(Globals.CurrentMap);
        }

        //Fill/Erase Functions
        public void FillLayer()
        {
            var oldCurSelX = Globals.CurTileX;
            var oldCurSelY = Globals.CurTileY;
            var tmpMap = Globals.CurrentMap;
            if (CurrentMapState == null)
            {
                CurrentMapState = tmpMap.GetMapData(false);
            }
            if (MessageBox.Show(@"Are you sure you want to fill this layer?", @"Fill Layer", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int x1 = 0;
                int y1 = 0;
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        Globals.CurTileX = x;
                        Globals.CurTileY = y;

                        if (Globals.CurrentLayer == Options.LayerCount)
                        {
                            Globals.MapLayersWindow.PlaceAttribute(Globals.CurrentMap, Globals.CurTileX, Globals.CurTileY);
                        }
                        else if (Globals.CurrentLayer < Options.LayerCount)
                        {
                            if (Globals.Autotilemode == 0)
                            {
                                x1 = (x) % (Globals.CurSelW + 1);
                                y1 = (y) % (Globals.CurSelH + 1);
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = Globals.CurrentTileset.GetId();
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX + x1;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY + y1;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = Globals.CurrentTileset.GetId();
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = (byte)Globals.Autotilemode;
                            }
                        }
                    }
                }
                tmpMap.InitAutotiles();
                Globals.CurTileX = oldCurSelX;
                Globals.CurTileY = oldCurSelY;
                if (MapInstance.GetMap(tmpMap.Left) != null)
                    MapInstance.GetMap(tmpMap.Left).InitAutotiles();
                if (MapInstance.GetMap(tmpMap.Up) != null)
                    MapInstance.GetMap(tmpMap.Up).InitAutotiles();
                if (MapInstance.GetMap(tmpMap.Right) != null)
                    MapInstance.GetMap(tmpMap.Right).InitAutotiles();
                if (MapInstance.GetMap(tmpMap.Down) != null)
                    MapInstance.GetMap(tmpMap.Down).InitAutotiles();


                if (!CurrentMapState.SequenceEqual(tmpMap.GetMapData(false)))
                {
                    if (CurrentMapState != null) MapUndoStates.Add(CurrentMapState);
                    MapRedoStates.Clear();
                    CurrentMapState = tmpMap.GetMapData(false);
                }
            }
        }
        public void EraseLayer()
        {
            var oldCurSelX = Globals.CurTileX;
            var oldCurSelY = Globals.CurTileY;
            var tmpMap = Globals.CurrentMap;
            if (CurrentMapState == null)
            {
                CurrentMapState = tmpMap.GetMapData(false);
            }
            if (MessageBox.Show(@"Are you sure you want to erase this layer?", @"Fill Layer", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        Globals.CurTileX = x;
                        Globals.CurTileY = y;

                        if (Globals.CurrentLayer == Options.LayerCount)
                        {
                            Globals.MapLayersWindow.RemoveAttribute(Globals.CurrentMap, Globals.CurTileX, Globals.CurTileY);
                        }
                        else if (Globals.CurrentLayer < Options.LayerCount)
                        {
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = 0;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = 0;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                        }
                    }
                }
                tmpMap.InitAutotiles();
                Globals.CurTileX = oldCurSelX;
                Globals.CurTileY = oldCurSelY;
                if (MapInstance.GetMap(tmpMap.Left) != null)
                    MapInstance.GetMap(tmpMap.Left).InitAutotiles();
                if (MapInstance.GetMap(tmpMap.Up) != null)
                    MapInstance.GetMap(tmpMap.Up).InitAutotiles();
                if (MapInstance.GetMap(tmpMap.Right) != null)
                    MapInstance.GetMap(tmpMap.Right).InitAutotiles();
                if (MapInstance.GetMap(tmpMap.Down) != null)
                    MapInstance.GetMap(tmpMap.Down).InitAutotiles();

                if (!CurrentMapState.SequenceEqual(tmpMap.GetMapData(false)))
                {
                    if (CurrentMapState != null) MapUndoStates.Add(CurrentMapState);
                    MapRedoStates.Clear();
                    CurrentMapState = tmpMap.GetMapData(false);
                }
            }
        }

        //Selection/Movement Function
        public void ProcessSelectionMovement(MapInstance tmpMap, bool ignoreMouse = false, bool ispreview = false)
        {
            bool wipeSource = false;
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
                if (Globals.MouseButton == 0 && !ignoreMouse)
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
            //WE are moving tiles, this will be fun!
            if (Globals.CurrentMap == tmpMap && Globals.SelectionSource == tmpMap)
            {
                Globals.SelectionSource = new MapInstance(tmpMap);
                wipeSource = true;
            }
            if ((wipeSource || ispreview) && !Globals.IsPaste)
            {
                if (ispreview)
                {
                    WipeCurrentSelection(tmpMap);
                }
                else
                {
                    WipeCurrentSelection(Globals.CurrentMap);
                }
            }
            int z = 0, zf = Options.LayerCount;
            if (Globals.SelectionType == (int)SelectionTypes.CurrentLayer)
            {
                z = Globals.CurrentLayer;
                zf = z + 1;
                if (zf > Options.LayerCount)
                {
                    zf = Options.LayerCount;
                }
            }

            //Finish by copying the source tiles over
            for (int l = z; l < zf; l++)
            {
                for (int x0 = selX + dragxoffset; x0 < selX + selW + 1 + dragxoffset; x0++)
                {
                    for (int y0 = selY + dragyoffset; y0 < selY + selH + 1 + dragyoffset; y0++)
                    {
                        if (x0 >= 0 && x0 < Options.MapWidth && y0 >= 0 &&
                            y0 < Options.MapHeight)
                        {
                            tmpMap.Layers[l].Tiles[x0, y0].TilesetIndex = Globals.SelectionSource.Layers[l].Tiles[x0 - dragxoffset, y0 - dragyoffset].TilesetIndex;
                            tmpMap.Layers[l].Tiles[x0, y0].X = Globals.SelectionSource.Layers[l].Tiles[x0 - dragxoffset, y0 - dragyoffset].X;
                            tmpMap.Layers[l].Tiles[x0, y0].Y = Globals.SelectionSource.Layers[l].Tiles[x0 - dragxoffset, y0 - dragyoffset].Y;
                            tmpMap.Layers[l].Tiles[x0, y0].Autotile = Globals.SelectionSource.Layers[l].Tiles[x0 - dragxoffset, y0 - dragyoffset].Autotile;
                        }
                        tmpMap.Autotiles.UpdateAutoTiles(x0, y0,
                            l, tmpMap.GenerateAutotileGrid());
                    }
                }
            }

            for (int x0 = selX + dragxoffset; x0 < selX + selW + 1 + dragxoffset; x0++)
            {
                for (int y0 = selY + dragyoffset; y0 < selY + selH + 1 + dragyoffset; y0++)
                {
                    if (x0 >= 0 && x0 < Options.MapWidth && y0 >= 0 &&
                        y0 < Options.MapHeight)
                    {
                        //Attributes
                        if (Globals.SelectionType != (int)SelectionTypes.CurrentLayer || Globals.CurrentLayer == Options.LayerCount)
                        {
                            if (Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset] != null)
                            {
                                tmpMap.Attributes[x0, y0] = new Intersect_Library.GameObjects.Maps.Attribute();
                                tmpMap.Attributes[x0, y0].value =
                                    Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].value;
                                tmpMap.Attributes[x0, y0].data1 =
                                    Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].data1;
                                tmpMap.Attributes[x0, y0].data2 =
                                    Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].data2;
                                tmpMap.Attributes[x0, y0].data3 =
                                    Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].data3;
                                tmpMap.Attributes[x0, y0].data4 =
                                    Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].data4;
                            }
                            else
                            {
                                tmpMap.Attributes[x0, y0] = null;
                            }
                        }

                        //Spawns
                        NpcSpawn spawnCopy;
                        if (Globals.SelectionType != (int)SelectionTypes.CurrentLayer || Globals.CurrentLayer == Options.LayerCount + 3)
                        {
                            if (Globals.SelectionSource.FindSpawnAt(x0 - dragxoffset, y0 - dragyoffset) != null)
                            {
                                if (tmpMap.FindSpawnAt(x0, y0) != null)
                                {
                                    tmpMap.Spawns.Remove(tmpMap.FindSpawnAt(x0, y0));
                                }
                                spawnCopy = new NpcSpawn(Globals.SelectionSource.FindSpawnAt(x0 - dragxoffset, y0 - dragyoffset));
                                spawnCopy.X = x0;
                                spawnCopy.Y = y0;
                                tmpMap.Spawns.Add(spawnCopy);
                            }
                        }

                        //Lights
                        LightBase lightCopy;
                        if (Globals.SelectionType != (int)SelectionTypes.CurrentLayer || Globals.CurrentLayer == Options.LayerCount + 1)
                        {
                            if (Globals.SelectionSource.FindLightAt(x0 - dragxoffset, y0 - dragyoffset) != null)
                            {
                                if (tmpMap.FindLightAt(x0, y0) != null)
                                {
                                    tmpMap.Lights.Remove(tmpMap.FindLightAt(x0, y0));
                                }
                                lightCopy = new LightBase(Globals.SelectionSource.FindLightAt(x0 - dragxoffset, y0 - dragyoffset));
                                lightCopy.TileX = x0;
                                lightCopy.TileY = y0;
                                tmpMap.Lights.Add(lightCopy);
                            }
                        }

                        //Events
                        EventBase eventCopy;
                        if (Globals.SelectionType != (int)SelectionTypes.CurrentLayer || Globals.CurrentLayer == Options.LayerCount + 2)
                        {
                            if (Globals.SelectionSource.FindEventAt(x0 - dragxoffset, y0 - dragyoffset) != null)
                            {
                                if (tmpMap.FindEventAt(x0, y0) != null)
                                {
                                    tmpMap.Events.Remove(tmpMap.FindEventAt(x0, y0).MyIndex);
                                }
                                eventCopy = new EventBase(tmpMap.EventIndex, Globals.SelectionSource.FindEventAt(x0 - dragxoffset, y0 - dragyoffset));
                                eventCopy.SpawnX = x0;
                                eventCopy.SpawnY = y0;
                                tmpMap.Events.Add(tmpMap.EventIndex, eventCopy);
                                tmpMap.EventIndex++;
                            }
                        }
                    }
                }
            }
        }

        private void WipeCurrentSelection(MapBase tmpMap)
        {
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
                    dragxoffset = Globals.TotalTileDragX;
                    dragyoffset = Globals.TotalTileDragY;
                }
            }
            int z = 0, zf = Options.LayerCount;
            if (Globals.SelectionType == (int)SelectionTypes.CurrentLayer)
            {
                z = Globals.CurrentLayer;
                zf = z + 1;
                if (zf > Options.LayerCount)
                {
                    zf = Options.LayerCount;
                }
            }
            //start by deleting the source tiles
            for (int l = z; l < zf; l++)
            {
                for (int x0 = selX; x0 < selX + selW + 1; x0++)
                {
                    for (int y0 = selY; y0 < selY + selH + 1; y0++)
                    {
                        if (x0 >= 0 && x0 < Options.MapWidth && y0 >= 0 &&
                            y0 < Options.MapHeight && x0 < selX + selW + 1 &&
                            y0 < selY + selH + 1)
                        {
                            tmpMap.Layers[l].Tiles[
                                x0, y0].TilesetIndex = -1;
                            tmpMap.Layers[l].Tiles[
                                x0, y0].X = 0;
                            tmpMap.Layers[l].Tiles[
                                x0, y0].Y = 0;
                            tmpMap.Layers[l].Tiles[
                                x0, y0].Autotile = 0;
                        }
                        tmpMap.Autotiles.UpdateAutoTiles(x0, y0,
                            l, ((MapInstance)tmpMap).GenerateAutotileGrid());
                    }
                }
            }

            for (int x0 = selX; x0 < selX + selW + 1; x0++)
            {
                for (int y0 = selY; y0 < selY + selH + 1; y0++)
                {
                    if (x0 >= 0 && x0 < Options.MapWidth && y0 >= 0 &&
                        y0 < Options.MapHeight && x0 < selX + selW + 1 &&
                        y0 < selY + selH + 1)
                    {
                        //Attributes
                        if (Globals.SelectionType != (int)SelectionTypes.CurrentLayer || Globals.CurrentLayer == Options.LayerCount)
                        {
                            if (tmpMap.Attributes[x0, y0] != null)
                            {
                                tmpMap.Attributes[x0, y0].value = 0;
                                tmpMap.Attributes[x0, y0].data1 = 0;
                                tmpMap.Attributes[x0, y0].data2 = 0;
                                tmpMap.Attributes[x0, y0].data3 = 0;
                                tmpMap.Attributes[x0, y0].data4 = "";
                            }
                        }

                        //Spawns
                        if (Globals.SelectionType != (int)SelectionTypes.CurrentLayer || Globals.CurrentLayer == Options.LayerCount + 3)
                        {
                            for (int w = 0; w < tmpMap.Spawns.Count; w++)
                            {
                                if (tmpMap.Spawns[w].X == x0 &&
                                    tmpMap.Spawns[w].Y == y0)
                                {
                                    tmpMap.Spawns.Remove(tmpMap.Spawns[w]);
                                }
                            }
                        }

                        //Lights
                        if (Globals.SelectionType != (int)SelectionTypes.CurrentLayer || Globals.CurrentLayer == Options.LayerCount + 1)
                        {
                            for (int w = 0; w < tmpMap.Lights.Count; w++)
                            {
                                if (tmpMap.Lights[w].TileX == x0 &&
                                    tmpMap.Lights[w].TileY == y0)
                                {
                                    tmpMap.Lights.Remove(tmpMap.Lights[w]);
                                }
                            }
                        }

                        //Events
                        if (Globals.SelectionType != (int)SelectionTypes.CurrentLayer || Globals.CurrentLayer == Options.LayerCount + 2)
                        {
                            if (((MapInstance)tmpMap).FindEventAt(x0, y0) != null)
                            {
                                tmpMap.Events.Remove(((MapInstance) tmpMap).FindEventAt(x0, y0).MyIndex);
                            }
                        }
                    }
                }
            }
        }

        public void PlaceSelection()
        {
            if (!Globals.Dragging) { return; }
            Globals.Dragging = false;
            Globals.TotalTileDragX = 0;
            Globals.TotalTileDragY = 0;
            Globals.SelectionSource = null;
            EditorGraphics.TilePreviewUpdated = true;
            Globals.CurMapSelX = Globals.CurTileX;
            Globals.CurMapSelY = Globals.CurTileY;
            Globals.CurMapSelW = 0;
            Globals.CurMapSelH = 0;
            Globals.SelectionSource = null;
            Globals.IsPaste = false;
            MapChanged = true;
        }

        //Cut Copy Paste Functions
        public void Cut()
        {
            Copy();
            WipeCurrentSelection(Globals.CurrentMap);
            EditorGraphics.TilePreviewUpdated = true;
            if (CurrentMapState != null) MapUndoStates.Add(CurrentMapState);
            MapRedoStates.Clear();
            CurrentMapState = Globals.CurrentMap.GetMapData(false);
            MapChanged = false;
        }
        public void Copy()
        {
            if (Globals.CurrentTool == (int)EdittingTool.Selection)
            {
                Globals.CopySource = new MapInstance(Globals.CurrentMap);
                Globals.CopyMapSelH = Globals.CurMapSelH;
                Globals.CopyMapSelW = Globals.CurMapSelW;
                Globals.CopyMapSelX = Globals.CurMapSelX;
                Globals.CopyMapSelY = Globals.CurMapSelY;
                Globals.HasCopy = true;
            }
        }
        public void Paste()
        {
            if (Globals.HasCopy && Globals.CopySource != null)
            {
                Globals.CurrentTool = (int)EdittingTool.Selection;
                int selX1 = Globals.CurMapSelX, selY1 = Globals.CurMapSelY, selW1 = Globals.CurMapSelW, selH1 = Globals.CurMapSelH;
                if (selW1 < 0)
                {
                    selX1 -= Math.Abs(selW1);
                    selW1 = Math.Abs(selW1);
                }
                if (selH1 < 0)
                {
                    selY1 -= Math.Abs(selH1);
                    selH1 = Math.Abs(selH1);
                }
                Globals.SelectionSource = Globals.CopySource;
                Globals.CurMapSelH = Globals.CopyMapSelH;
                Globals.CurMapSelW = Globals.CopyMapSelW;
                Globals.CurMapSelX = Globals.CopyMapSelX;
                Globals.CurMapSelY = Globals.CopyMapSelY;
                Globals.Dragging = true;
                int selX = Globals.CurMapSelX, selY = Globals.CurMapSelY, selW = Globals.CurMapSelW, selH = Globals.CurMapSelH;
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
                //We just pasted, lets move it under the cursor:
                Globals.TotalTileDragX = -selX + selX1;
                Globals.TotalTileDragY = -selY + selY1;
                Globals.IsPaste = true;
                EditorGraphics.TilePreviewUpdated = true;
            }
        }

        private void pnlMapContainer_Scroll(object sender, ScrollEventArgs e)
        {
            EditorLoop.RunFrame();
        }

        private void picMap_Resize(object sender, EventArgs e)
        {
            
        }

        private void pnlMapContainer_Resize(object sender, EventArgs e)
        {
            picMap.Size = pnlMapContainer.ClientSize;
            picMap.MinimumSize = new Size((Options.MapWidth + 2) * Options.TileWidth, (Options.MapHeight + 2) * Options.TileHeight);
            EditorGraphics.CurrentView = new Rectangle((picMap.Size.Width-  (Options.MapWidth) * Options.TileWidth)/2, (picMap.Size.Height - (Options.MapHeight) * Options.TileHeight)/2, picMap.Size.Width, picMap.Size.Height);
            CreateSwapChain();
        }

        private void frmMapEditor_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
