using Intersect_Editor.Classes;
using System;
using System.Collections.Generic;
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
            picMap.Width = (Options.MapWidth + 2) * Options.TileWidth;
            picMap.Height = (Options.MapHeight + 2) * Options.TileHeight;
            CreateSwapChain();
        }

        public void InitMapEditor()
        {
            CreateSwapChain();
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
                    _chain = new SwapChainRenderTarget(EditorGraphics.GetGraphicsDevice(), this.picMap.Handle,
                        this.picMap.Width, this.picMap.Height);
                    EditorGraphics.SetMapEditorChain(_chain);
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
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (CurrentMapState == null)
            {
                CurrentMapState = tmpMap.GetMapData(false);
            }
        }
        public void AddUndoState()
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
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
            Globals.MapEditorWindow.DockPanel.Focus();
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
            if (Globals.EditingLight != null) { return; }
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];

            if (e.X >= picMap.Width - Options.TileWidth || e.Y >= picMap.Height - Options.TileHeight) { return; }
            if (e.X < Options.TileWidth || e.Y < Options.TileHeight) { return; }

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
                                    tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex);
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
                                        tmpMap.Layers[i].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex);
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
                                ProcessSelectionMovement(Globals.GameMaps[Globals.CurrentMap], true);
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
                                Globals.SelectionSource = Globals.GameMaps[Globals.CurrentMap];
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
                            Globals.MapLayersWindow.PlaceAttribute(Globals.GameMaps[Globals.CurrentMap],
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
                                                Globals.CurrentTileset;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].X =
                                                Globals.CurSelX + x;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].Y =
                                                Globals.CurSelY + y;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].Autotile = 0;
                                            tmpMap.Autotiles.InitAutotiles(gameMaps);
                                        }
                                    }
                                }
                                MapChanged = true;
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY]
                                    .TilesetIndex = Globals.CurrentTileset;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X
                                    = Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y
                                    = Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY]
                                    .Autotile = (byte)Globals.Autotilemode;
                                tmpMap.Autotiles.InitAutotiles(gameMaps);
                                MapChanged = true;
                            }
                        }
                    }
                    break;
                case MouseButtons.Right:
                    Globals.MouseButton = 1;


                    if (Globals.CurrentTool == (int) EdittingTool.Selection)
                    {
                        if (Globals.Dragging)
                        {
                            //Place the change, we done!
                            ProcessSelectionMovement(Globals.GameMaps[Globals.CurrentMap], true);
                            PlaceSelection();
                        }
                    }
                    if (Globals.CurrentLayer == Options.LayerCount) //Attributes
                    {
                        if (Globals.CurrentTool == (int)EdittingTool.Pen)
                        {
                            if (Globals.MapLayersWindow.RemoveAttribute(Globals.GameMaps[Globals.CurrentMap], Globals.CurTileX, Globals.CurTileY)) { MapChanged = true; }
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
                        Light tmpLight;
                        if ((tmpLight = Globals.GameMaps[Globals.CurrentMap].FindLightAt(Globals.CurTileX, Globals.CurTileY)) != null)
                        {
                            Globals.GameMaps[Globals.CurrentMap].Lights.Remove(tmpLight);
                            EditorGraphics.TilePreviewUpdated = true;
                            MapChanged = true;
                        }
                    }
                    else if (Globals.CurrentLayer == Options.LayerCount + 2) //Events
                    {
                        EventStruct tmpEvent;
                        if ((tmpEvent = Globals.GameMaps[Globals.CurrentMap].FindEventAt(Globals.CurTileX, Globals.CurTileY)) != null)
                        {
                            tmpEvent.Deleted = 1;
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
                            tmpMap.Autotiles.InitAutotiles(gameMaps);
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
                if (tmpMap.Left > -1)
                {
                    if (Globals.GameMaps[tmpMap.Left] != null)
                    {
                        Globals.GameMaps[tmpMap.Left].Autotiles.InitAutotiles(gameMaps);
                    }
                }
            }
            if (Globals.CurTileY == 0)
            {
                if (tmpMap.Up > -1)
                {
                    if (Globals.GameMaps[tmpMap.Up] != null)
                    {
                        Globals.GameMaps[tmpMap.Up].Autotiles.InitAutotiles(gameMaps);
                    }
                }
            }
            if (Globals.CurTileX == Options.MapWidth - 1)
            {
                if (tmpMap.Right > -1)
                {
                    if (Globals.GameMaps[tmpMap.Right] != null)
                    {
                        Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles(gameMaps);
                    }
                }
            }
            if (Globals.CurTileY != Options.MapHeight - 1) return;
            if (tmpMap.Down <= -1) return;
            if (Globals.GameMaps[tmpMap.Down] != null)
            {
                Globals.GameMaps[tmpMap.Down].Autotiles.InitAutotiles(gameMaps);
            }
        }
        private void picMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (Globals.EditingLight != null) { return; }
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
            Globals.MouseX = e.X;
            Globals.MouseY = e.Y;
            if (e.X >= picMap.Width - Options.TileWidth || e.Y >= picMap.Height - Options.TileHeight) { return; }
            if (e.X < Options.TileWidth || e.Y < Options.TileHeight) { return; }
            int oldx = Globals.CurTileX;
            int oldy = Globals.CurTileY;
            Globals.CurTileX = (int)Math.Floor((double)(e.X - Options.TileWidth) / Options.TileWidth);
            Globals.CurTileY = (int)Math.Floor((double)(e.Y - Options.TileHeight) / Options.TileHeight);
            if (Globals.CurTileX < 0) { Globals.CurTileX = 0; }
            if (Globals.CurTileY < 0) { Globals.CurTileY = 0; }

            if (oldx != Globals.CurTileX || oldy != Globals.CurTileY)
            {
                EditorGraphics.TilePreviewUpdated = true;
            }

            if (Globals.MouseButton > -1)
            {
                var tmpMap = Globals.GameMaps[Globals.CurrentMap];
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
                                Globals.GameMaps[Globals.CurrentMap].Spawns[
                                    Globals.MapLayersWindow.lstMapNpcs.SelectedIndex
                                    ].X = Globals.CurTileX;
                                Globals.GameMaps[Globals.CurrentMap].Spawns[
                                    Globals.MapLayersWindow.lstMapNpcs.SelectedIndex
                                    ].Y = Globals.CurTileY;
                            }
                        }
                        else
                        {
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
                                                Globals.CurrentTileset;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].X = Globals.CurSelX + x;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].Y = Globals.CurSelY + y;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[
                                                Globals.CurTileX + x, Globals.CurTileY + y].Autotile = 0;
                                            tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX + x, Globals.CurTileY + y,
                                                Globals.CurrentLayer, gameMaps);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY]
                                    .TilesetIndex = Globals.CurrentTileset;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X =
                                    Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y =
                                    Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY]
                                    .Autotile =
                                    (byte)Globals.Autotilemode;
                                tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX, Globals.CurTileY,
                                    Globals.CurrentLayer, gameMaps);
                            }
                        }
                        if (Globals.CurTileX == 0)
                        {
                            if (tmpMap.Left > -1)
                            {
                                if (Globals.GameMaps[tmpMap.Left] != null)
                                {
                                    Globals.GameMaps[tmpMap.Left].Autotiles.InitAutotiles(gameMaps);
                                }
                            }
                        }
                        if (Globals.CurTileY == 0)
                        {
                            if (tmpMap.Up > -1)
                            {
                                if (Globals.GameMaps[tmpMap.Up] != null)
                                {
                                    Globals.GameMaps[tmpMap.Up].Autotiles.InitAutotiles(gameMaps);
                                }
                            }
                        }
                        if (Globals.CurTileX == Options.MapWidth - 1)
                        {
                            if (tmpMap.Right > -1)
                            {
                                if (Globals.GameMaps[tmpMap.Right] != null)
                                {
                                    Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles(gameMaps);
                                }
                            }
                        }
                        if (Globals.CurTileY == Options.MapHeight - 1)
                        {
                            if (tmpMap.Down > -1)
                            {
                                if (Globals.GameMaps[tmpMap.Down] != null)
                                {
                                    Globals.GameMaps[tmpMap.Down].Autotiles.InitAutotiles(gameMaps);
                                }
                            }
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
                                tmpMap.Autotiles.InitAutotiles(gameMaps);
                            }
                        }
                    }
                    if (Globals.CurTileX == 0)
                    {
                        if (tmpMap.Left > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Left] != null)
                            {
                                Globals.GameMaps[tmpMap.Left].Autotiles.InitAutotiles(gameMaps);
                            }
                        }
                    }
                    if (Globals.CurTileY == 0)
                    {
                        if (tmpMap.Up > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Up] != null)
                            {
                                Globals.GameMaps[tmpMap.Up].Autotiles.InitAutotiles(gameMaps);
                            }
                        }
                    }
                    if (Globals.CurTileX == Options.MapWidth - 1)
                    {
                        if (tmpMap.Right > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Right] != null)
                            {
                                Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles(gameMaps);
                            }
                        }
                    }
                    if (Globals.CurTileY == Options.MapHeight - 1)
                    {
                        if (tmpMap.Down > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Down] != null)
                            {
                                Globals.GameMaps[tmpMap.Down].Autotiles.InitAutotiles(gameMaps);
                            }
                        }
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
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
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
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].TilesetIndex = Globals.CurrentTileset;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].X = Globals.CurSelX + x1;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Y = Globals.CurSelY + y1;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Autotile = (byte)Globals.Autotilemode;
                                    }
                                    else if (Globals.MouseButton == 1)
                                    {
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].TilesetIndex = -1;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].X = 0;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Y = 0;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].Autotile = 0;
                                    }
                                    tmpMap.Autotiles.UpdateAutoTiles(x0, y0, Globals.CurrentLayer, gameMaps);
                                }
                            }
                            else
                            {
                                if (Globals.MouseButton == 0)
                                {
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[x0, y0].TilesetIndex = Globals.CurrentTileset;
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
                                    Globals.CurrentLayer, gameMaps);
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
            if (Globals.Dragging) { 
            Globals.TotalTileDragX -= (Globals.TileDragX - Globals.CurTileX);
            Globals.TotalTileDragY -= (Globals.TileDragY - Globals.CurTileY);
            Globals.TileDragX = 0;
            Globals.TileDragY = 0;
            }
            EditorGraphics.TilePreviewUpdated = true;

        }
        private void picMap_DoubleClick(object sender, EventArgs e)
        {
            if (Globals.MouseX >= Options.TileWidth && Globals.MouseX <= (Options.MapWidth + 2) * Options.TileWidth - Options.TileWidth)
            {
                if (Globals.MouseY >= 0 && Globals.MouseY <= Options.TileHeight)
                {
                    if (Globals.GameMaps[Globals.CurrentMap].Up == -1)
                    {
                        if (MessageBox.Show(@"Do you want to create a map here?", @"Create new map.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            if (MapUndoStates.Count > 0 && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                PacketSender.SendMap(Globals.CurrentMap);
                            }
                            PacketSender.SendCreateMap(0, Globals.CurrentMap, null);
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MapUndoStates.Count > 0 && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            PacketSender.SendMap(Globals.CurrentMap);
                        }
                        Globals.MainForm.EnterMap(Globals.GameMaps[Globals.CurrentMap].Up);
                    }
                }
                else if (Globals.MouseY >= (Options.MapHeight + 2) * Options.TileHeight - Options.TileHeight && Globals.MouseY <= (Options.MapHeight + 2) * Options.TileHeight)
                {
                    if (Globals.GameMaps[Globals.CurrentMap].Down == -1)
                    {
                        if (MessageBox.Show(@"Do you want to create a map here?", @"Create new map.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            if (MapUndoStates.Count > 0 && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                PacketSender.SendMap(Globals.CurrentMap);
                            }
                            PacketSender.SendCreateMap(1, Globals.CurrentMap, null);
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MapUndoStates.Count > 0 && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            PacketSender.SendMap(Globals.CurrentMap);
                        }
                        Globals.MainForm.EnterMap(Globals.GameMaps[Globals.CurrentMap].Down);
                    }
                }
            }
            if (Globals.MouseY < Options.TileHeight || Globals.MouseY > (Options.MapHeight + 2) * Options.TileHeight - Options.TileHeight) return;
            if (Globals.MouseX >= 0 & Globals.MouseX <= Options.TileWidth)
            {
                if (Globals.GameMaps[Globals.CurrentMap].Left == -1)
                {
                    if (
                        MessageBox.Show(@"Do you want to create a map here?", @"Create new map.",
                            MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    if (MapUndoStates.Count > 0 && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        PacketSender.SendMap(Globals.CurrentMap);
                    }
                    PacketSender.SendCreateMap(2, Globals.CurrentMap, null);
                }
                else
                {
                    //Should ask if the user wants to save changes
                    if (MapUndoStates.Count > 0 && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        PacketSender.SendMap(Globals.CurrentMap);
                    }
                    Globals.MainForm.EnterMap(Globals.GameMaps[Globals.CurrentMap].Left);
                }
            }
            else if (Globals.MouseX >= (Options.MapWidth + 2) * Options.TileWidth - Options.TileWidth && Globals.MouseX <= (Options.MapWidth + 2) * Options.TileWidth)
            {
                if (Globals.GameMaps[Globals.CurrentMap].Right == -1)
                {
                    if (
                        MessageBox.Show(@"Do you want to create a map here?", @"Create new map.", MessageBoxButtons.YesNo) !=
                        DialogResult.Yes) return;
                    if (MapUndoStates.Count > 0 && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        PacketSender.SendMap(Globals.CurrentMap);
                    }
                    PacketSender.SendCreateMap(3, Globals.CurrentMap, null);
                }
                else
                {
                    //Should ask if the user wants to save changes
                    if (MapUndoStates.Count > 0 && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        PacketSender.SendMap(Globals.CurrentMap);
                    }
                    Globals.MainForm.EnterMap(Globals.GameMaps[Globals.CurrentMap].Right);
                }
            }

            //See if we should edit an event, light, npc, etc
            if (Globals.CurrentLayer == Options.LayerCount) //Attributes
            {

            }
            else if (Globals.CurrentLayer == Options.LayerCount + 1) //Lights
            {
                Light tmpLight;
                if (
                    (tmpLight =
                        Globals.GameMaps[Globals.CurrentMap].FindLightAt(Globals.CurTileX,
                            Globals.CurTileY)) == null)
                {
                    tmpLight = new Light(Globals.CurTileX, Globals.CurTileY);
                    tmpLight.Size = 50;
                    Globals.GameMaps[Globals.CurrentMap].Lights.Add(tmpLight);
                }
                Globals.MapLayersWindow.tabControl.SelectedTab = Globals.MapLayersWindow.tabLights;
                Globals.MapLayersWindow.lightEditor.Show();
                Globals.BackupLight = new Light(tmpLight);
                Globals.MapLayersWindow.lightEditor.LoadEditor(tmpLight);
                Globals.EditingLight = tmpLight;
                MapChanged = true;
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 2) //Events
            {
                EventStruct tmpEvent;
                FrmEvent tmpEventEditor;
                if (
                    (tmpEvent =
                        Globals.GameMaps[Globals.CurrentMap].FindEventAt(Globals.CurTileX,
                            Globals.CurTileY)) == null)
                {
                    tmpEvent = new EventStruct(Globals.GameMaps[Globals.CurrentMap].Events.Count, Globals.CurTileX, Globals.CurTileY);
                    Globals.GameMaps[Globals.CurrentMap].Events.Add(tmpEvent);
                    tmpEventEditor = new FrmEvent(Globals.GameMaps[Globals.CurrentMap])
                    {
                        MyEvent = tmpEvent,
                        MyMap = Globals.GameMaps[Globals.CurrentMap],
                        NewEvent = true
                    };
                    tmpEventEditor.InitEditor();
                    tmpEventEditor.Show();
                    MapChanged = true;
                }
                else
                {
                    tmpEventEditor = new FrmEvent(Globals.GameMaps[Globals.CurrentMap]) { MyEvent = tmpEvent, MyMap = Globals.GameMaps[Globals.CurrentMap] };
                    tmpEventEditor.InitEditor();
                    tmpEventEditor.Show();
                }
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 3) //NPCS
            {
                if (Globals.MapLayersWindow.lstMapNpcs.SelectedIndex > -1 &&
                        Globals.MapLayersWindow.rbDeclared.Checked == true)
                {
                    Globals.GameMaps[Globals.CurrentMap].Spawns[
                        Globals.MapLayersWindow.lstMapNpcs.SelectedIndex].X = Globals.CurTileX;
                    Globals.GameMaps[Globals.CurrentMap].Spawns[
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

        //Fill/Erase Functions
        public void FillLayer()
        {
            var oldCurSelX = Globals.CurTileX;
            var oldCurSelY = Globals.CurTileY;
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
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
                            Globals.MapLayersWindow.PlaceAttribute(Globals.GameMaps[Globals.CurrentMap], Globals.CurTileX, Globals.CurTileY);
                        }
                        else if (Globals.CurrentLayer < Options.LayerCount)
                        {
                            if (Globals.Autotilemode == 0)
                            {
                                x1 = (x) % (Globals.CurSelW + 1);
                                y1 = (y) % (Globals.CurSelH + 1);
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = Globals.CurrentTileset;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX + x1;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY + y1;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = Globals.CurrentTileset;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = (byte)Globals.Autotilemode;
                            }
                        }
                    }
                }
                tmpMap.Autotiles.InitAutotiles(gameMaps);
                Globals.CurTileX = oldCurSelX;
                Globals.CurTileY = oldCurSelY;
                if (tmpMap.Left > -1)
                {
                    if (Globals.GameMaps[tmpMap.Left] != null)
                    {
                        Globals.GameMaps[tmpMap.Left].Autotiles.InitAutotiles(gameMaps);
                    }
                }
                if (tmpMap.Up > -1)
                {
                    if (Globals.GameMaps[tmpMap.Up] != null)
                    {
                        Globals.GameMaps[tmpMap.Up].Autotiles.InitAutotiles(gameMaps);
                    }
                }
                if (tmpMap.Right > -1)
                {
                    if (Globals.GameMaps[tmpMap.Right] != null)
                    {
                        Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles(gameMaps);
                    }
                }
                if (tmpMap.Down > -1)
                {
                    if (Globals.GameMaps[tmpMap.Down] != null)
                    {
                        Globals.GameMaps[tmpMap.Down].Autotiles.InitAutotiles(gameMaps);
                    }
                }

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
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
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
                            Globals.MapLayersWindow.RemoveAttribute(Globals.GameMaps[Globals.CurrentMap], Globals.CurTileX, Globals.CurTileY);
                        }
                        else if (Globals.CurrentLayer < Options.LayerCount)
                        {
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                        }
                    }
                }
                tmpMap.Autotiles.InitAutotiles(gameMaps);
                Globals.CurTileX = oldCurSelX;
                Globals.CurTileY = oldCurSelY;
                if (tmpMap.Left > -1)
                {
                    if (Globals.GameMaps[tmpMap.Left] != null)
                    {
                        Globals.GameMaps[tmpMap.Left].Autotiles.InitAutotiles(gameMaps);
                    }
                }
                if (tmpMap.Up > -1)
                {
                    if (Globals.GameMaps[tmpMap.Up] != null)
                    {
                        Globals.GameMaps[tmpMap.Up].Autotiles.InitAutotiles(gameMaps);
                    }
                }
                if (tmpMap.Right > -1)
                {
                    if (Globals.GameMaps[tmpMap.Right] != null)
                    {
                        Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles(gameMaps);
                    }
                }
                if (tmpMap.Down > -1)
                {
                    if (Globals.GameMaps[tmpMap.Down] != null)
                    {
                        Globals.GameMaps[tmpMap.Down].Autotiles.InitAutotiles(gameMaps);
                    }
                }

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
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
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
            if (Globals.GameMaps[Globals.CurrentMap] == tmpMap && Globals.SelectionSource == tmpMap)
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
                    WipeCurrentSelection(Globals.GameMaps[Globals.CurrentMap]);
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
                            l,gameMaps);
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
                            tmpMap.Attributes[x0, y0].value = Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].value;
                            tmpMap.Attributes[x0, y0].data1 = Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].data1;
                            tmpMap.Attributes[x0, y0].data2 = Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].data2;
                            tmpMap.Attributes[x0, y0].data3 = Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].data3;
                            tmpMap.Attributes[x0, y0].data4 = Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].data4;
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
                        Light lightCopy;
                        if (Globals.SelectionType != (int)SelectionTypes.CurrentLayer || Globals.CurrentLayer == Options.LayerCount + 1)
                        {
                            if (Globals.SelectionSource.FindLightAt(x0 - dragxoffset, y0 - dragyoffset) != null)
                            {
                                if (tmpMap.FindLightAt(x0, y0) != null)
                                {
                                    tmpMap.Lights.Remove(tmpMap.FindLightAt(x0, y0));
                                }
                                lightCopy = new Light(Globals.SelectionSource.FindLightAt(x0 - dragxoffset, y0 - dragyoffset));
                                lightCopy.TileX = x0;
                                lightCopy.TileY = y0;
                                tmpMap.Lights.Add(lightCopy);
                            }
                        }

                        //Events
                        EventStruct eventCopy;
                        if (Globals.SelectionType != (int)SelectionTypes.CurrentLayer || Globals.CurrentLayer == Options.LayerCount + 2)
                        {
                            if (Globals.SelectionSource.FindEventAt(x0 - dragxoffset, y0 - dragyoffset) != null)
                            {
                                if (tmpMap.FindEventAt(x0, y0) != null)
                                {
                                    tmpMap.FindEventAt(x0, y0).Deleted = 1;
                                }
                                eventCopy = new EventStruct(tmpMap.Events.Count, Globals.SelectionSource.FindEventAt(x0 - dragxoffset, y0 - dragyoffset));
                                eventCopy.SpawnX = x0;
                                eventCopy.SpawnY = y0;
                                tmpMap.Events.Add(eventCopy);
                            }
                        }
                    }
                }
            }
        }

        private void WipeCurrentSelection(MapStruct tmpMap)
        {
            int selX = Globals.CurMapSelX, selY = Globals.CurMapSelY, selW = Globals.CurMapSelW, selH = Globals.CurMapSelH;
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
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
                            l,gameMaps);
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
                            tmpMap.Attributes[x0, y0].value = 0;
                            tmpMap.Attributes[x0, y0].data1 = 0;
                            tmpMap.Attributes[x0, y0].data2 = 0;
                            tmpMap.Attributes[x0, y0].data3 = 0;
                            tmpMap.Attributes[x0, y0].data4 = "";
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
                            for (int w = 0; w < tmpMap.Events.Count; w++)
                            {
                                if (tmpMap.Events[w].SpawnX == x0 &&
                                    tmpMap.Events[w].SpawnY == y0)
                                {
                                    tmpMap.Events[w].Deleted = 1;
                                }
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
            WipeCurrentSelection(Globals.GameMaps[Globals.CurrentMap]);
            EditorGraphics.TilePreviewUpdated = true;
            if (CurrentMapState != null) MapUndoStates.Add(CurrentMapState);
            MapRedoStates.Clear();
            CurrentMapState = Globals.GameMaps[Globals.CurrentMap].GetMapData(false);
            MapChanged = false;
        }
        public void Copy()
        {
            if (Globals.CurrentTool == (int)EdittingTool.Selection)
            {
                Globals.CopySource = new MapInstance(Globals.GameMaps[Globals.CurrentMap]);
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
    }
}
