using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using DarkUI.Forms;
using Intersect.Config;
using Intersect.Editor.Classes.Maps;
using Intersect.Editor.Core;
using Intersect.Editor.Forms.Editors.Events;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Maps;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Logging;

using Microsoft.Xna.Framework.Graphics;

using WeifenLuo.WinFormsUI.Docking;

namespace Intersect.Editor.Forms.DockingElements
{

    public partial class FrmMapEditor : DockContent
    {

        public MapSaveState CurrentMapState;

        public List<MapSaveState> MapRedoStates = new List<MapSaveState>();

        //Map States
        public List<MapSaveState> MapUndoStates = new List<MapSaveState>();

        //MonoGame Swap Chain
        private SwapChainRenderTarget mChain;

        private bool mMapChanged;

        //Init/Form Functions
        public FrmMapEditor()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon;
        }

        private void InitLocalization()
        {
            Text = Strings.Mapping.editortitle;
        }

        private void frmMapEditor_Load(object sender, EventArgs e)
        {
            PacketHandler.MapUpdatedDelegate += InitMapEditor;
            picMap.Size = pnlMapContainer.ClientSize;
            picMap.MinimumSize = new Size(
                (Options.MapWidth + 2) * Options.TileWidth, (Options.MapHeight + 2) * Options.TileHeight
            );

            Core.Graphics.CurrentView = new Rectangle(
                (picMap.Size.Width - Options.MapWidth * Options.TileWidth) / 2,
                (picMap.Size.Height - Options.MapHeight * Options.TileHeight) / 2, picMap.Size.Width, picMap.Size.Height
            );

            CreateSwapChain();
            InitLocalization();
        }

        public void InitMapEditor()
        {
            if (InvokeRequired)
            {
                BeginInvoke(PacketHandler.MapUpdatedDelegate);
            }
            else
            {
                pnlMapContainer.AutoScroll = true;
                picMap.Size = pnlMapContainer.ClientSize;
                picMap.MinimumSize = new Size(
                    (Options.MapWidth + 2) * Options.TileWidth, (Options.MapHeight + 2) * Options.TileHeight
                );

                Core.Graphics.CurrentView = new Rectangle(
                    (picMap.Size.Width - Options.MapWidth * Options.TileWidth) / 2,
                    (picMap.Size.Height - Options.MapHeight * Options.TileHeight) / 2, picMap.Size.Width,
                    picMap.Size.Height
                );

                CreateSwapChain();
                Globals.MapLayersWindow.RefreshNpcList();
                Globals.MapPropertiesWindow.Init(Globals.CurrentMap);
                Globals.CurrentMap.SaveStateAsUnchanged();
                if (Globals.MapEditorWindow.picMap.Visible)
                {
                    return;
                }

                Globals.MapEditorWindow.picMap.Visible = true;
            }
        }

        private void CreateSwapChain()
        {
            if (!Globals.ClosingEditor)
            {
                if (mChain != null)
                {
                    mChain.Dispose();
                }

                if (Core.Graphics.GetGraphicsDevice() != null)
                {
                    if (picMap.Width > 0 && picMap.Height > 0)
                    {
                        mChain = new SwapChainRenderTarget(
                            Core.Graphics.GetGraphicsDevice(), picMap.Handle, picMap.Width, picMap.Height, false,
                            SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents,
                            PresentInterval.Immediate
                        );

                        Core.Graphics.SetMapEditorChain(mChain);
                    }
                }
            }
        }

        private void frmMapEditor_DockStateChanged(object sender, EventArgs e)
        {
            CreateSwapChain();
        }

        public void UnloadMap()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker) delegate { UnloadMap(); });

                return;
            }

            picMap.Visible = false;
            ResetUndoRedoStates();
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
                CurrentMapState = tmpMap.SaveInternal();
            }
        }

        public void AddUndoState()
        {
            var tmpMap = Globals.CurrentMap;
            if (CurrentMapState != null)
            {
                MapUndoStates.Add(CurrentMapState);
                MapRedoStates.Clear();
                CurrentMapState = tmpMap.SaveInternal();
            }

            mMapChanged = false;
        }

        //PicMap Functions
        public void picMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Globals.MapEditorWindow.DockPanel.Focused)
            {
                Globals.MapEditorWindow.DockPanel.Focus();

                return;
            }

            if (Globals.EditingLight != null)
            {
                return;
            }

            var tmpMap = Globals.CurrentMap;

            if (e.X < Core.Graphics.CurrentView.Left ||
                e.Y < Core.Graphics.CurrentView.Top ||
                e.X > Core.Graphics.CurrentView.Left + Options.MapWidth * Options.TileWidth ||
                e.Y > Core.Graphics.CurrentView.Top + Options.MapHeight * Options.TileHeight)
            {
                if (Globals.Dragging)
                {
                    //Place the change, we done!
                    Globals.MapEditorWindow.ProcessSelectionMovement(Globals.CurrentMap, true);
                    Globals.MapEditorWindow.PlaceSelection();
                }

                return;
            }

            if (CurrentMapState == null)
            {
                CurrentMapState = tmpMap.SaveInternal();
            }

            switch (e.Button)
            {
                case MouseButtons.Left:
                    Globals.MouseButton = 0;
                    if (Globals.CurrentTool == (int) EditingTool.Droppler)
                    {
                        foreach (var layer in Enumerable.Reverse(Options.Instance.MapOpts.Layers.All))
                        {
                            if (tmpMap.Layers[layer][Globals.CurTileX, Globals.CurTileY].TilesetId != Guid.Empty)
                            {
                                Globals.MapLayersWindow.SetTileset(TilesetBase.GetName(tmpMap.Layers[layer][Globals.CurTileX, Globals.CurTileY].TilesetId));

                                Globals.CurSelW = 0;
                                Globals.CurSelH = 0;
                                Globals.MapLayersWindow.SetAutoTile(tmpMap.Layers[layer][Globals.CurTileX, Globals.CurTileY].Autotile);

                                Globals.CurSelX = tmpMap.Layers[layer][Globals.CurTileX, Globals.CurTileY].X;
                                Globals.CurSelY = tmpMap.Layers[layer][Globals.CurTileX, Globals.CurTileY].Y;
                                Globals.CurrentTool = (int)EditingTool.Pen;
                                Globals.MapLayersWindow.SetLayer(layer);

                                break;
                            }
                        }
                        return;
                    }
                    else if (Globals.CurrentTool == (int) EditingTool.Selection)
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
                    else if (Globals.CurrentTool == (int) EditingTool.Rectangle)
                    {
                        Globals.CurMapSelX = Globals.CurTileX;
                        Globals.CurMapSelY = Globals.CurTileY;
                        Globals.CurMapSelW = 0;
                        Globals.CurMapSelH = 0;
                    }
                    else if (Globals.CurrentTool == (int) EditingTool.Fill)
                    {
                        if (Globals.CurrentLayer == LayerOptions.Attributes)
                        {
                            Globals.MapEditorWindow.SmartFillAttributes(Globals.CurTileX, Globals.CurTileY);
                        }
                        else if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer))
                        {
                            Globals.MapEditorWindow.SmartFillLayer(Globals.CurTileX, Globals.CurTileY);
                        }

                        Globals.MouseButton = -1;
                    }
                    else if (Globals.CurrentTool == (int) EditingTool.Erase)
                    {
                        if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer))
                        {
                            if (Globals.CurrentLayer == LayerOptions.Attributes)
                            {
                                Globals.MapEditorWindow.SmartEraseAttributes(Globals.CurTileX, Globals.CurTileY);
                            }
                            else if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer))
                            {
                                Globals.MapEditorWindow.SmartEraseLayer(Globals.CurTileX, Globals.CurTileY);
                            }
                        }

                        Globals.MouseButton = -1;
                    }
                    else
                    {
                        if (Globals.CurrentLayer == LayerOptions.Attributes) //Attributes
                        {
                            Globals.MapLayersWindow.PlaceAttribute(
                                Globals.CurrentMap, Globals.CurTileX, Globals.CurTileY
                            );

                            mMapChanged = true;
                        }
                        else if (Globals.CurrentLayer == LayerOptions.Lights) //Lights
                        {
                        }
                        else if (Globals.CurrentLayer == LayerOptions.Events) //Events
                        {
                        }
                        else if (Globals.CurrentLayer == LayerOptions.Npcs) //NPCS
                        {
                        }
                        else
                        {
                            if (Globals.CurrentTileset == null)
                            {
                                return;
                            }

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

                                            tmpMap.InitAutotiles();
                                        }
                                    }
                                }

                                mMapChanged = true;
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].TilesetId = Globals.CurrentTileset.Id;
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Autotile = (byte) Globals.Autotilemode;

                                tmpMap.InitAutotiles();
                                mMapChanged = true;
                            }
                        }
                    }

                    break;
                case MouseButtons.Right:
                    Globals.MouseButton = 1;

                    if (Globals.CurrentTool == (int) EditingTool.Selection)
                    {
                        if (Globals.Dragging)
                        {
                            //Place the change, we done!
                            ProcessSelectionMovement(Globals.CurrentMap, true);
                            PlaceSelection();
                        }
                    }

                    if (Globals.CurrentTool == (int) EditingTool.Fill)
                    {
                        if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer))
                        {
                            Globals.MapEditorWindow.FillLayer();
                        }

                        Globals.MouseButton = -1;
                    }
                    else if (Globals.CurrentTool == (int) EditingTool.Erase)
                    {
                        if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer))
                        {
                            Globals.MapEditorWindow.EraseLayer();
                        }

                        Globals.MouseButton = -1;
                    }
                    else if (Globals.CurrentLayer == LayerOptions.Attributes)
                    {
                        if (Globals.CurrentTool == (int) EditingTool.Pen)
                        {
                            if (Globals.MapLayersWindow.RemoveAttribute(
                                Globals.CurrentMap, Globals.CurTileX, Globals.CurTileY
                            ))
                            {
                                mMapChanged = true;
                            }
                        }
                        else if (Globals.CurrentTool == (int) EditingTool.Rectangle ||
                                 Globals.CurrentTool == (int) EditingTool.Selection)
                        {
                            Globals.CurMapSelX = Globals.CurTileX;
                            Globals.CurMapSelY = Globals.CurTileY;
                            Globals.CurMapSelW = 0;
                            Globals.CurMapSelH = 0;
                        }
                    }
                    else if (Globals.CurrentLayer == LayerOptions.Lights)
                    {
                        LightBase tmpLight;
                        if ((tmpLight = Globals.CurrentMap.FindLightAt(Globals.CurTileX, Globals.CurTileY)) != null)
                        {
                            Globals.CurrentMap.Lights.Remove(tmpLight);
                            Core.Graphics.TilePreviewUpdated = true;
                            mMapChanged = true;
                        }
                    }
                    else if (Globals.CurrentLayer == LayerOptions.Events)
                    {
                        EventBase tmpEvent;
                        if ((tmpEvent = Globals.CurrentMap.FindEventAt(Globals.CurTileX, Globals.CurTileY)) != null)
                        {
                            Globals.CurrentMap.LocalEvents.Remove(tmpEvent.Id);
                            mMapChanged = true;
                        }
                    }
                    else if (Globals.CurrentLayer == LayerOptions.Npcs)
                    {
                    }
                    else
                    {
                        if (Globals.CurrentTool == (int) EditingTool.Pen)
                        {
                            tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].TilesetId = Guid.Empty;
                            tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                            tmpMap.InitAutotiles();
                            mMapChanged = true;
                        }
                        else if (Globals.CurrentTool == (int) EditingTool.Rectangle ||
                                 Globals.CurrentTool == (int) EditingTool.Selection)
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
                if (MapInstance.Get(tmpMap.Left) != null)
                {
                    MapInstance.Get(tmpMap.Left).InitAutotiles();
                }
            }

            if (Globals.CurTileY == 0)
            {
                if (MapInstance.Get(tmpMap.Up) != null)
                {
                    MapInstance.Get(tmpMap.Up).InitAutotiles();
                }
            }

            if (Globals.CurTileX == Options.MapWidth - 1)
            {
                if (MapInstance.Get(tmpMap.Right) != null)
                {
                    MapInstance.Get(tmpMap.Right).InitAutotiles();
                }
            }

            if (Globals.CurTileY == Options.MapHeight - 1)
            {
                if (MapInstance.Get(tmpMap.Down) != null)
                {
                    MapInstance.Get(tmpMap.Down).InitAutotiles();
                }
            }
        }

        private void picMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (Globals.EditingLight != null)
            {
                return;
            }

            if (e.Button == MouseButtons.Middle)
            {
                Core.Graphics.CurrentView.X -= Globals.MouseX - e.X;
                Core.Graphics.CurrentView.Y -= Globals.MouseY - e.Y;
                if (Core.Graphics.CurrentView.X > Options.MapWidth * Options.TileWidth)
                {
                    Core.Graphics.CurrentView.X = Options.MapWidth * Options.TileWidth;
                }

                if (Core.Graphics.CurrentView.Y > Options.MapHeight * Options.TileHeight)
                {
                    Core.Graphics.CurrentView.Y = Options.MapHeight * Options.TileHeight;
                }

                if (Core.Graphics.CurrentView.X - picMap.Width < -Options.TileWidth * Options.MapWidth * 2)
                {
                    Core.Graphics.CurrentView.X = -Options.TileWidth * Options.MapWidth * 2 + picMap.Width;
                }

                if (Core.Graphics.CurrentView.Y - picMap.Height < -Options.TileHeight * Options.MapHeight * 2)
                {
                    Core.Graphics.CurrentView.Y = -Options.TileHeight * Options.MapHeight * 2 + picMap.Height;
                }
            }

            Globals.MouseX = e.X;
            Globals.MouseY = e.Y;

            if (e.X < Core.Graphics.CurrentView.Left ||
                e.Y < Core.Graphics.CurrentView.Top ||
                e.X > Core.Graphics.CurrentView.Left + Options.MapWidth * Options.TileWidth ||
                e.Y > Core.Graphics.CurrentView.Top + Options.MapHeight * Options.TileHeight)
            {
                return;
            }

            var oldx = Globals.CurTileX;
            var oldy = Globals.CurTileY;
            Globals.CurTileX = (int) Math.Floor((double) (e.X - Core.Graphics.CurrentView.Left) / Options.TileWidth);
            Globals.CurTileY = (int) Math.Floor((double) (e.Y - Core.Graphics.CurrentView.Top) / Options.TileHeight);
            if (Globals.CurTileX < 0)
            {
                Globals.CurTileX = 0;
            }

            if (Globals.CurTileY < 0)
            {
                Globals.CurTileY = 0;
            }

            if (Globals.CurTileX >= Options.MapWidth)
            {
                Globals.CurTileX = Options.MapWidth - 1;
            }

            if (Globals.CurTileY >= Options.MapHeight)
            {
                Globals.CurTileY = Options.MapHeight - 1;
            }

            if (e.Button == MouseButtons.Middle)
            {
                return;
            }

            if (oldx != Globals.CurTileX || oldy != Globals.CurTileY)
            {
                Core.Graphics.TilePreviewUpdated = true;
            }

            if (Globals.CurrentTool == (int) EditingTool.Erase ||
                Globals.CurrentTool == (int) EditingTool.Fill ||
                Globals.CurrentTool == (int) EditingTool.Droppler)
            {
                return; //No click/drag with fill, erase, or droppler tools
            }

            if (Globals.MouseButton > -1)
            {
                var tmpMap = Globals.CurrentMap;
                if (Globals.MouseButton == 0)
                {
                    if (Globals.CurrentTool == (int) EditingTool.Selection)
                    {
                        if (!Globals.Dragging)
                        {
                            Globals.CurMapSelW = Globals.CurTileX - Globals.CurMapSelX;
                            Globals.CurMapSelH = Globals.CurTileY - Globals.CurMapSelY;
                        }
                    }
                    else if (Globals.CurrentTool == (int) EditingTool.Rectangle)
                    {
                        Globals.CurMapSelW = Globals.CurTileX - Globals.CurMapSelX;
                        Globals.CurMapSelH = Globals.CurTileY - Globals.CurMapSelY;
                    }
                    else
                    {
                        if (Globals.CurrentLayer == LayerOptions.Attributes)
                        {
                            Globals.MapLayersWindow.PlaceAttribute(tmpMap, Globals.CurTileX, Globals.CurTileY);
                        }
                        else if (Globals.CurrentLayer == LayerOptions.Lights)
                        {
                        }
                        else if (Globals.CurrentLayer == LayerOptions.Events)
                        {
                        }
                        else if (Globals.CurrentLayer == LayerOptions.Npcs)
                        {
                            if (Globals.MapLayersWindow.rbDeclared.Checked == true && Globals.MapLayersWindow.lstMapNpcs.Items.Count > 0)
                            {
                                Globals.CurrentMap.Spawns[Globals.MapLayersWindow.lstMapNpcs.SelectedIndex].X = Globals.CurTileX;

                                Globals.CurrentMap.Spawns[Globals.MapLayersWindow.lstMapNpcs.SelectedIndex].Y = Globals.CurTileY;
                            }
                        }
                        else
                        {
                            if (Globals.CurrentTileset == null)
                            {
                                return;
                            }

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

                                tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX, Globals.CurTileY, Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                            }

                            tmpMap.Autotiles.UpdateCliffAutotiles(tmpMap, Globals.CurrentLayer);
                        }

                        if (Globals.CurTileX == 0)
                        {
                            if (MapInstance.Get(tmpMap.Left) != null)
                            {
                                MapInstance.Get(tmpMap.Left).InitAutotiles();
                            }
                        }

                        if (Globals.CurTileY == 0)
                        {
                            if (MapInstance.Get(tmpMap.Up) != null)
                            {
                                MapInstance.Get(tmpMap.Up).InitAutotiles();
                            }
                        }

                        if (Globals.CurTileX == Options.MapWidth - 1)
                        {
                            if (MapInstance.Get(tmpMap.Right) != null)
                            {
                                MapInstance.Get(tmpMap.Right).InitAutotiles();
                            }
                        }

                        if (Globals.CurTileY == Options.MapHeight - 1)
                        {
                            if (MapInstance.Get(tmpMap.Down) != null)
                            {
                                MapInstance.Get(tmpMap.Down).InitAutotiles();
                            }
                        }
                    }
                }
                else if (Globals.MouseButton == 1)
                {
                    if (Globals.CurrentTool == (int) EditingTool.Rectangle)
                    {
                        Globals.CurMapSelW = Globals.CurTileX - Globals.CurMapSelX;
                        Globals.CurMapSelH = Globals.CurTileY - Globals.CurMapSelY;
                    }
                    else if (Globals.CurrentTool == (int) EditingTool.Pen)
                    {
                        if (Globals.CurrentLayer == LayerOptions.Attributes)
                        {
                            Globals.MapLayersWindow.RemoveAttribute(tmpMap, Globals.CurTileX, Globals.CurTileY);
                        }
                        else if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer))
                        {
                            if (Globals.CurrentTool == (int) EditingTool.Pen)
                            {
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].TilesetId = Guid.Empty;

                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Autotile = 0;

                                tmpMap.InitAutotiles();
                            }
                        }
                    }

                    if (Globals.CurTileX == 0)
                    {
                        if (MapInstance.Get(tmpMap.Left) != null)
                        {
                            MapInstance.Get(tmpMap.Left).InitAutotiles();
                        }
                    }

                    if (Globals.CurTileY == 0)
                    {
                        if (MapInstance.Get(tmpMap.Up) != null)
                        {
                            MapInstance.Get(tmpMap.Up).InitAutotiles();
                        }
                    }

                    if (Globals.CurTileX == Options.MapWidth - 1)
                    {
                        if (MapInstance.Get(tmpMap.Right) != null)
                        {
                            MapInstance.Get(tmpMap.Right).InitAutotiles();
                        }
                    }

                    if (Globals.CurTileY == Options.MapHeight - 1)
                    {
                        if (MapInstance.Get(tmpMap.Down) != null)
                        {
                            MapInstance.Get(tmpMap.Down).InitAutotiles();
                        }
                    }
                }
            }
            else
            {
                if (Globals.CurrentTool != (int) EditingTool.Selection)
                {
                    Globals.CurMapSelX = Globals.CurTileX;
                    Globals.CurMapSelY = Globals.CurTileY;
                }
            }
        }

        public void picMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (Globals.EditingLight != null || e.Button == MouseButtons.Middle)
            {
                return;
            }

            var tmpMap = Globals.CurrentMap;
            if (Globals.CurrentTool == (int) EditingTool.Rectangle)
            {
                int selX = Globals.CurMapSelX,
                    selY = Globals.CurMapSelY,
                    selW = Globals.CurMapSelW,
                    selH = Globals.CurMapSelH;

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

                if (Globals.CurrentLayer == LayerOptions.Attributes)
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

                    mMapChanged = true;
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
                else
                {
                    var x1 = 0;
                    var y1 = 0;
                    for (var x0 = selX; x0 < selX + selW + 1; x0++)
                    {
                        for (var y0 = selY; y0 < selY + selH + 1; y0++)
                        {
                            if (Globals.Autotilemode == 0)
                            {
                                x1 = (x0 - selX) % (Globals.CurSelW + 1);
                                y1 = (y0 - selY) % (Globals.CurSelH + 1);
                                if (x0 >= 0 &&
                                    x0 < Options.MapWidth &&
                                    y0 >= 0 &&
                                    y0 < Options.MapHeight &&
                                    x0 < selX + selW + 1 &&
                                    y0 < selY + selH + 1)
                                {
                                    if (Globals.MouseButton == 0)
                                    {
                                        if (Globals.CurrentTileset != null)
                                        {
                                            tmpMap.Layers[Globals.CurrentLayer][x0, y0].TilesetId = Globals.CurrentTileset.Id;
                                            tmpMap.Layers[Globals.CurrentLayer][x0, y0].X = Globals.CurSelX + x1;
                                            tmpMap.Layers[Globals.CurrentLayer][x0, y0].Y = Globals.CurSelY + y1;
                                            tmpMap.Layers[Globals.CurrentLayer][x0, y0].Autotile = (byte) Globals.Autotilemode;
                                        }
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

                                tmpMap.Autotiles.UpdateAutoTiles(x0, y0, Globals.CurrentLayer, tmpMap.GenerateAutotileGrid());
                            }

                            tmpMap.Autotiles.UpdateCliffAutotiles(tmpMap, Globals.CurrentLayer);
                        }
                    }

                    mMapChanged = true;
                }
            }

            Globals.MouseButton = -1;
            if (mMapChanged)
            {
                if (CurrentMapState != null)
                {
                    MapUndoStates.Add(CurrentMapState);
                }

                MapRedoStates.Clear();
                CurrentMapState = tmpMap.SaveInternal();
                mMapChanged = false;
            }

            if (Globals.CurrentTool != (int) EditingTool.Selection)
            {
                Globals.CurMapSelX = Globals.CurTileX;
                Globals.CurMapSelY = Globals.CurTileY;
                Globals.CurMapSelW = 0;
                Globals.CurMapSelH = 0;
            }

            //Globals.Dragging = false;
            if (Globals.Dragging)
            {
                Globals.TotalTileDragX -= Globals.TileDragX - Globals.CurTileX;
                Globals.TotalTileDragY -= Globals.TileDragY - Globals.CurTileY;
                Globals.TileDragX = 0;
                Globals.TileDragY = 0;
            }

            Core.Graphics.TilePreviewUpdated = true;
        }

        private void picMap_DoubleClick(object sender, EventArgs e)
        {
            var currentMap = Globals.CurrentMap;

            if (currentMap == null)
            {
                Log.Error(new ArgumentNullException(nameof(currentMap)));

                return;
            }

            var dir = -1;
            var newMapId = Guid.Empty;

            if (Globals.MouseX < 0 ||
                Globals.MouseX > picMap.Width ||
                Globals.MouseY < 0 ||
                Globals.MouseY > picMap.Height)
            {
                return;
            }

            if (Globals.Dragging)
            {
                //Place the change, we done!
                Globals.MapEditorWindow.ProcessSelectionMovement(Globals.CurrentMap, true);
                Globals.MapEditorWindow.PlaceSelection();
            }

            //Check Left Column of Maps
            var gridX = 0;
            var gridY = 0;
            if (Globals.MouseX < Core.Graphics.CurrentView.Left)
            {
                gridX = -1;
            }

            if (Globals.MouseX > Core.Graphics.CurrentView.Left + Options.MapWidth * Options.TileWidth)
            {
                gridX = 1;
            }

            if (Globals.MouseY < Core.Graphics.CurrentView.Top)
            {
                gridY = -1;
            }

            if (Globals.MouseY > Core.Graphics.CurrentView.Top + Options.MapHeight * Options.TileHeight)
            {
                gridY = 1;
            }

            if (gridX != 0 || gridY != 0)
            {
                if (gridX == -1 && gridY == 0)
                {
                    dir = (int) Directions.Left;
                }
                else if (gridX == 1 && gridY == 0)
                {
                    dir = (int) Directions.Right;
                }
                else if (gridX == 0 && gridY == -1)
                {
                    dir = (int) Directions.Up;
                }
                else if (gridX == 0 && gridY == 1)
                {
                    dir = (int) Directions.Down;
                }

                if (dir != -1)
                {
                    if (Globals.MapGrid != null && Globals.MapGrid.Contains(Globals.CurrentMap.Id))
                    {
                        var x = gridX + Globals.CurrentMap.MapGridX;
                        var y = gridY + Globals.CurrentMap.MapGridY;
                        if (x >= 0 && y >= 0 && x < Globals.MapGrid.GridWidth && y < Globals.MapGrid.GridHeight)
                        {
                            if (Globals.MapGrid.Grid[x, y].MapId != Guid.Empty)
                            {
                                newMapId = Globals.MapGrid.Grid[x, y].MapId;
                            }
                        }
                    }
                }
                else
                {
                    if (Globals.MapGrid != null && Globals.MapGrid.Contains(Globals.CurrentMap.Id))
                    {
                        var x = gridX + Globals.CurrentMap.MapGridX;
                        var y = gridY + Globals.CurrentMap.MapGridY;
                        if (x >= 0 && y >= 0 && x < Globals.MapGrid.GridWidth && y < Globals.MapGrid.GridHeight)
                        {
                            if (Globals.MapGrid.Grid[x, y].MapId != Guid.Empty)
                            {
                                if (Globals.CurrentMap.Changed() &&
                                    DarkMessageBox.ShowInformation(
                                        Strings.Mapping.savemapdialogue, Strings.Mapping.savemap,
                                        DarkDialogButton.YesNo, Properties.Resources.Icon
                                    ) ==
                                    DialogResult.Yes)
                                {
                                    SaveMap();
                                }

                                Globals.MainForm.EnterMap(Globals.MapGrid.Grid[x, y].MapId);
                            }
                            else
                            {
                                DarkMessageBox.ShowError(
                                    Strings.Mapping.diagonalwarning, Strings.Mapping.createmap, DarkDialogButton.Ok,
                                    Properties.Resources.Icon
                                );

                                return;
                            }
                        }
                        else
                        {
                            DarkMessageBox.ShowError(
                                Strings.Mapping.diagonalwarning, Strings.Mapping.createmap, DarkDialogButton.Ok,
                                Properties.Resources.Icon
                            );

                            return;
                        }
                    }
                }

                if (dir > -1)
                {
                    if (newMapId == Guid.Empty)
                    {
                        if (DarkMessageBox.ShowInformation(
                                Strings.Mapping.createmapdialogue, Strings.Mapping.createmap, DarkDialogButton.YesNo,
                                Properties.Resources.Icon
                            ) !=
                            DialogResult.Yes)
                        {
                            return;
                        }

                        if (Globals.CurrentMap.Changed() &&
                            DarkMessageBox.ShowWarning(
                                Strings.Mapping.savemapdialogue, Strings.Mapping.savemap, DarkDialogButton.YesNo,
                                Properties.Resources.Icon
                            ) ==
                            DialogResult.Yes)
                        {
                            SaveMap();
                        }

                        PacketSender.SendCreateMap(dir, Globals.CurrentMap.Id, null);
                    }
                    else
                    {
                        if (Globals.CurrentMap.Changed() &&
                            DarkMessageBox.ShowWarning(
                                Strings.Mapping.savemapdialogue, Strings.Mapping.savemap, DarkDialogButton.YesNo,
                                Properties.Resources.Icon
                            ) ==
                            DialogResult.Yes)
                        {
                            SaveMap();
                        }

                        Globals.MainForm.EnterMap(newMapId);
                    }

                    return;
                }

                return;
            }

            switch (Globals.CurrentLayer)
            {
                //See if we should edit an event, light, npc, etc
                //Attributes
                case LayerOptions.Attributes:
                    break;

                //Lights
                case LayerOptions.Lights:
                {
                    LightBase tmpLight;
                    if ((tmpLight = Globals.CurrentMap.FindLightAt(Globals.CurTileX, Globals.CurTileY)) == null)
                    {
                        tmpLight = new LightBase(Globals.CurTileX, Globals.CurTileY)
                        {
                            Size = 50
                        };

                        Globals.CurrentMap.Lights.Add(tmpLight);
                    }

                    Globals.MapLayersWindow.btnLightsHeader_Click(null, null);
                    Globals.MapLayersWindow.lightEditor.Show();
                    Globals.BackupLight = new LightBase(tmpLight);
                    Globals.MapLayersWindow.lightEditor.LoadEditor(tmpLight);
                    Globals.EditingLight = tmpLight;
                    mMapChanged = true;

                    break;
                }

                //Events
                case LayerOptions.Events:
                {
                    var tmpEvent = currentMap.FindEventAt(Globals.CurTileX, Globals.CurTileY);
                    FrmEvent tmpEventEditor;
                    if (tmpEvent == null)
                    {
                        tmpEvent = new EventBase(
                            Guid.NewGuid(), Globals.CurrentMap.Id, Globals.CurTileX, Globals.CurTileY
                        );

                        Globals.CurrentMap.LocalEvents.Add(tmpEvent.Id, tmpEvent);
                        tmpEventEditor = new FrmEvent(Globals.CurrentMap)
                        {
                            MyEvent = tmpEvent,
                            MyMap = Globals.CurrentMap,
                            NewEvent = true
                        };

                        tmpEventEditor.InitEditor(false, false, false);
                        tmpEventEditor.ShowDialog();
                        mMapChanged = true;
                    }
                    else
                    {
                        tmpEventEditor = new FrmEvent(Globals.CurrentMap)
                            {MyEvent = tmpEvent, MyMap = Globals.CurrentMap};

                        tmpEventEditor.InitEditor(false, false, false);
                        tmpEventEditor.ShowDialog();
                    }

                    break;
                }

                //NPCS
                case LayerOptions.Npcs:
                {
                    var spawnIndex = Globals.MapLayersWindow.lstMapNpcs.SelectedIndex;
                    var spawn = -1 < spawnIndex && spawnIndex < currentMap.Spawns.Count
                        ? currentMap.Spawns[spawnIndex]
                        : null;

                    if (spawn != null && Globals.MapLayersWindow.rbDeclared.Checked)
                    {
                        spawn.X = Globals.CurTileX;
                        spawn.Y = Globals.CurTileY;
                        mMapChanged = true;
                    }

                    break;
                }

                default:
                    break;
            }

            Globals.Dragging = false;
            Globals.TotalTileDragX = 0;
            Globals.TotalTileDragY = 0;
            Globals.MouseButton = -1;
            Globals.SelectionSource = null;
            Core.Graphics.TilePreviewUpdated = true;
        }

        private void picMap_MouseEnter(object sender, EventArgs e)
        {
            if (Globals.CurrentEditor == -1)
            {
                Globals.MapEditorWindow.DockPanel.Focus();
            }
        }

        private void SaveMap()
        {
            if (Globals.CurrentTool == (int) EditingTool.Selection)
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
                CurrentMapState = tmpMap.SaveInternal();
            }

            if (DarkMessageBox.ShowWarning(
                    Strings.Mapping.filllayerdialogue, Strings.Mapping.filllayer, DarkDialogButton.YesNo,
                    Properties.Resources.Icon
                ) ==
                DialogResult.Yes)
            {
                var x1 = 0;
                var y1 = 0;
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        Globals.CurTileX = x;
                        Globals.CurTileY = y;

                        if (Globals.CurrentLayer == LayerOptions.Attributes)
                        {
                            Globals.MapLayersWindow.PlaceAttribute(
                                Globals.CurrentMap, Globals.CurTileX, Globals.CurTileY
                            );
                        }
                        else if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer))
                        {
                            if (Globals.Autotilemode == 0)
                            {
                                x1 = x % (Globals.CurSelW + 1);
                                y1 = y % (Globals.CurSelH + 1);
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].TilesetId = Globals.CurrentTileset.Id;
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX + x1;
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY + y1;
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].TilesetId = Globals.CurrentTileset.Id;
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Autotile = (byte) Globals.Autotilemode;
                            }
                        }
                    }
                }

                tmpMap.InitAutotiles();
                Globals.CurTileX = oldCurSelX;
                Globals.CurTileY = oldCurSelY;
                if (MapInstance.Get(tmpMap.Left) != null)
                {
                    MapInstance.Get(tmpMap.Left).InitAutotiles();
                }

                if (MapInstance.Get(tmpMap.Up) != null)
                {
                    MapInstance.Get(tmpMap.Up).InitAutotiles();
                }

                if (MapInstance.Get(tmpMap.Right) != null)
                {
                    MapInstance.Get(tmpMap.Right).InitAutotiles();
                }

                if (MapInstance.Get(tmpMap.Down) != null)
                {
                    MapInstance.Get(tmpMap.Down).InitAutotiles();
                }

                if (!CurrentMapState.Matches(tmpMap.SaveInternal()))
                {
                    if (CurrentMapState != null)
                    {
                        MapUndoStates.Add(CurrentMapState);
                    }

                    MapRedoStates.Clear();
                    CurrentMapState = tmpMap.SaveInternal();
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
                CurrentMapState = tmpMap.SaveInternal();
            }

            if (DarkMessageBox.ShowWarning(
                    Strings.Mapping.eraselayerdialogue, Strings.Mapping.eraselayer, DarkDialogButton.YesNo,
                    Properties.Resources.Icon
                ) ==
                DialogResult.Yes)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        Globals.CurTileX = x;
                        Globals.CurTileY = y;

                        if (Globals.CurrentLayer == LayerOptions.Attributes)
                        {
                            Globals.MapLayersWindow.RemoveAttribute(
                                Globals.CurrentMap, Globals.CurTileX, Globals.CurTileY
                            );
                        }
                        else if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer))
                        {
                            tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].TilesetId =
                                Guid.Empty;

                            tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].X = 0;
                            tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Y = 0;
                            tmpMap.Layers[Globals.CurrentLayer][Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                        }
                    }
                }

                tmpMap.InitAutotiles();
                Globals.CurTileX = oldCurSelX;
                Globals.CurTileY = oldCurSelY;
                if (MapInstance.Get(tmpMap.Left) != null)
                {
                    MapInstance.Get(tmpMap.Left).InitAutotiles();
                }

                if (MapInstance.Get(tmpMap.Up) != null)
                {
                    MapInstance.Get(tmpMap.Up).InitAutotiles();
                }

                if (MapInstance.Get(tmpMap.Right) != null)
                {
                    MapInstance.Get(tmpMap.Right).InitAutotiles();
                }

                if (MapInstance.Get(tmpMap.Down) != null)
                {
                    MapInstance.Get(tmpMap.Down).InitAutotiles();
                }

                if (!CurrentMapState.Matches(tmpMap.SaveInternal()))
                {
                    if (CurrentMapState != null)
                    {
                        MapUndoStates.Add(CurrentMapState);
                    }

                    MapRedoStates.Clear();
                    CurrentMapState = tmpMap.SaveInternal();
                }
            }
        }

        private void SmartFillTile(int x, int y, Tile target)
        {
            int x1, y1;

            if (x < 0 || x >= Options.MapWidth || y < 0 || y >= Options.MapHeight)
            {
                return;
            }

            var selected = Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y];

            if (selected.TilesetId == target.TilesetId &&
                selected.X == target.X &&
                selected.Y == target.Y &&
                selected.Autotile == target.Autotile)
            {
                if (Globals.Autotilemode == 0)
                {
                    x1 = x % (Globals.CurSelW + 1);
                    y1 = y % (Globals.CurSelH + 1);
                    Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].TilesetId = Globals.CurrentTileset.Id;
                    Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].X = Globals.CurSelX + x1;
                    Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].Y = Globals.CurSelY + y1;
                    Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].Autotile = 0;
                }
                else
                {
                    Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].TilesetId = Globals.CurrentTileset.Id;
                    Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].X = Globals.CurSelX;
                    Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].Y = Globals.CurSelY;
                    Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].Autotile = (byte) Globals.Autotilemode;
                }

                SmartFillTile(x, y - 1, target);
                SmartFillTile(x, y + 1, target);
                SmartFillTile(x - 1, y, target);
                SmartFillTile(x + 1, y, target);
            }
        }

        public void SmartFillLayer(int x, int y)
        {
            var target = Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y];

            //if target tile != selected tile then we should smart fill...
            if (target.TilesetId != Globals.CurrentTileset.Id ||
                target.X != Globals.CurSelX ||
                target.Y != Globals.CurSelY ||
                target.Autotile != (byte) Globals.Autotilemode)
            {
                SmartFillTile(x, y, target);

                Globals.CurrentMap.InitAutotiles();
                if (MapInstance.Get(Globals.CurrentMap.Left) != null)
                {
                    MapInstance.Get(Globals.CurrentMap.Left).InitAutotiles();
                }

                if (MapInstance.Get(Globals.CurrentMap.Up) != null)
                {
                    MapInstance.Get(Globals.CurrentMap.Up).InitAutotiles();
                }

                if (MapInstance.Get(Globals.CurrentMap.Right) != null)
                {
                    MapInstance.Get(Globals.CurrentMap.Right).InitAutotiles();
                }

                if (MapInstance.Get(Globals.CurrentMap.Down) != null)
                {
                    MapInstance.Get(Globals.CurrentMap.Down).InitAutotiles();
                }

                if (!CurrentMapState.Matches(Globals.CurrentMap.SaveInternal()))
                {
                    if (CurrentMapState != null)
                    {
                        MapUndoStates.Add(CurrentMapState);
                    }

                    MapRedoStates.Clear();
                    CurrentMapState = Globals.CurrentMap.SaveInternal();
                }
            }
        }

        private void SmartFillAttribute(int x, int y, string data  = null, MapAttribute newAttribute = null)
        {
            if (x < 0 || x >= Options.MapWidth || y < 0 || y >= Options.MapHeight)
            {
                return;
            }

            if (newAttribute == null)
            {
                newAttribute = Globals.MapLayersWindow.CreateAttribute();
            }

            var attributeAtPoint = Globals.CurrentMap.Attributes[x, y];
            var thisData = attributeAtPoint?.Data();

            if (string.Equals(newAttribute.Data(), thisData, StringComparison.Ordinal))
            {
                return;
            }

            if (!string.Equals(data, thisData, StringComparison.Ordinal))
            {
                return;
            }

            Globals.MapLayersWindow.PlaceAttribute(Globals.CurrentMap, x, y);

            SmartFillAttribute(x, y - 1, thisData);
            SmartFillAttribute(x, y + 1, thisData);
            SmartFillAttribute(x - 1, y, thisData);
            SmartFillAttribute(x + 1, y, thisData);
        }

        public void SmartFillAttributes(int x, int y)
        {
            var attribute = Globals.CurrentMap.Attributes[x, y];
            SmartFillAttribute(x, y);

            if (!CurrentMapState.Matches(Globals.CurrentMap.SaveInternal()))
            {
                if (CurrentMapState != null)
                {
                    MapUndoStates.Add(CurrentMapState);
                }

                MapRedoStates.Clear();
                CurrentMapState = Globals.CurrentMap.SaveInternal();
            }
        }

        private void SmartEraseTile(int x, int y, Tile target)
        {
            if (x < 0 || x >= Options.MapWidth || y < 0 || y >= Options.MapHeight)
            {
                return;
            }

            var selected = Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y];

            if (selected.TilesetId == target.TilesetId &&
                selected.X == target.X &&
                selected.Y == target.Y &&
                selected.Autotile == target.Autotile)
            {
                Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].TilesetId = Guid.Empty;
                Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].X = 0;
                Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].Y = 0;
                Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y].Autotile = 0;

                SmartEraseTile(x, y - 1, target);
                SmartEraseTile(x, y + 1, target);
                SmartEraseTile(x - 1, y, target);
                SmartEraseTile(x + 1, y, target);
            }
        }

        public void SmartEraseLayer(int x, int y)
        {
            var target = Globals.CurrentMap.Layers[Globals.CurrentLayer][x, y];

            if (target.TilesetId != Guid.Empty)
            {
                SmartEraseTile(x, y, target);

                Globals.CurrentMap.InitAutotiles();
                if (MapInstance.Get(Globals.CurrentMap.Left) != null)
                {
                    MapInstance.Get(Globals.CurrentMap.Left).InitAutotiles();
                }

                if (MapInstance.Get(Globals.CurrentMap.Up) != null)
                {
                    MapInstance.Get(Globals.CurrentMap.Up).InitAutotiles();
                }

                if (MapInstance.Get(Globals.CurrentMap.Right) != null)
                {
                    MapInstance.Get(Globals.CurrentMap.Right).InitAutotiles();
                }

                if (MapInstance.Get(Globals.CurrentMap.Down) != null)
                {
                    MapInstance.Get(Globals.CurrentMap.Down).InitAutotiles();
                }

                if (!CurrentMapState.Matches(Globals.CurrentMap.SaveInternal()))
                {
                    if (CurrentMapState != null)
                    {
                        MapUndoStates.Add(CurrentMapState);
                    }

                    MapRedoStates.Clear();
                    CurrentMapState = Globals.CurrentMap.SaveInternal();
                }
            }
        }

        private void SmartEraseAttribute(int x, int y, MapAttributes attribute)
        {
            var a = MapAttributes.Walkable;

            if (x < 0 || x >= Options.MapWidth || y < 0 || y >= Options.MapHeight)
            {
                return;
            }

            if (Globals.CurrentMap.Attributes[x, y] != null)
            {
                a = Globals.CurrentMap.Attributes[x, y].Type;
            }

            if (a == attribute)
            {
                Globals.MapLayersWindow.RemoveAttribute(Globals.CurrentMap, x, y);

                SmartEraseAttribute(x, y - 1, attribute);
                SmartEraseAttribute(x, y + 1, attribute);
                SmartEraseAttribute(x - 1, y, attribute);
                SmartEraseAttribute(x + 1, y, attribute);
            }
        }

        public void SmartEraseAttributes(int x, int y)
        {
            var attribute = MapAttributes.Walkable;

            if (Globals.CurrentMap.Attributes[x, y] != null)
            {
                attribute = Globals.CurrentMap.Attributes[x, y].Type;
            }

            if (attribute > 0)
            {
                SmartEraseAttribute(x, y, attribute);

                if (!CurrentMapState.Matches(Globals.CurrentMap.SaveInternal()))
                {
                    if (CurrentMapState != null)
                    {
                        MapUndoStates.Add(CurrentMapState);
                    }

                    MapRedoStates.Clear();
                    CurrentMapState = Globals.CurrentMap.SaveInternal();
                }
            }
        }

        //Selection/Movement Function
        public void ProcessSelectionMovement(MapInstance tmpMap, bool ignoreMouse = false, bool ispreview = false)
        {
            var wipeSource = false;
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

                if (dragxoffset == 0 && dragyoffset == 0 && Globals.SelectionSource == Globals.CurrentMap)
                {
                    return;
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

            var layers = Options.Instance.MapOpts.Layers.All;
            if (Globals.SelectionType == (int) SelectionTypes.CurrentLayer)
            {
                layers = Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer) ? new List<string>() { Globals.CurrentLayer } : new List<string>();
            }

            //Finish by copying the source tiles over
            foreach (var layer in layers)
            {
                for (var x0 = selX + dragxoffset; x0 < selX + selW + 1 + dragxoffset; x0++)
                {
                    for (var y0 = selY + dragyoffset; y0 < selY + selH + 1 + dragyoffset; y0++)
                    {
                        if (x0 >= 0 && x0 < Options.MapWidth && y0 >= 0 && y0 < Options.MapHeight)
                        {
                            tmpMap.Layers[layer][x0, y0].TilesetId = Globals.SelectionSource.Layers[layer][x0 - dragxoffset, y0 - dragyoffset].TilesetId;
                            tmpMap.Layers[layer][x0, y0].X = Globals.SelectionSource.Layers[layer][x0 - dragxoffset, y0 - dragyoffset].X;
                            tmpMap.Layers[layer][x0, y0].Y = Globals.SelectionSource.Layers[layer][x0 - dragxoffset, y0 - dragyoffset].Y;
                            tmpMap.Layers[layer][x0, y0].Autotile = Globals.SelectionSource.Layers[layer][x0 - dragxoffset, y0 - dragyoffset].Autotile;
                        }

                        tmpMap.Autotiles.UpdateAutoTiles(x0, y0, layer, tmpMap.GenerateAutotileGrid());
                    }
                }
            }

            tmpMap.Autotiles.UpdateCliffAutotiles(tmpMap, Globals.CurrentLayer);

            for (var x0 = selX + dragxoffset; x0 < selX + selW + 1 + dragxoffset; x0++)
            {
                for (var y0 = selY + dragyoffset; y0 < selY + selH + 1 + dragyoffset; y0++)
                {
                    if (x0 >= 0 && x0 < Options.MapWidth && y0 >= 0 && y0 < Options.MapHeight)
                    {
                        //Attributes
                        if (Globals.SelectionType != (int) SelectionTypes.CurrentLayer ||
                            Globals.CurrentLayer == LayerOptions.Attributes)
                        {
                            if (Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset] != null)
                            {
                                tmpMap.Attributes[x0, y0] = Globals.SelectionSource.Attributes[x0 - dragxoffset, y0 - dragyoffset].Clone();
                            }
                            else
                            {
                                tmpMap.Attributes[x0, y0] = null;
                            }
                        }

                        //Spawns
                        NpcSpawn spawnCopy;
                        if (Globals.SelectionType != (int) SelectionTypes.CurrentLayer ||
                            Globals.CurrentLayer == LayerOptions.Npcs)
                        {
                            if (Globals.SelectionSource.FindSpawnAt(x0 - dragxoffset, y0 - dragyoffset) != null)
                            {
                                if (tmpMap.FindSpawnAt(x0, y0) != null)
                                {
                                    tmpMap.Spawns.Remove(tmpMap.FindSpawnAt(x0, y0));
                                }

                                spawnCopy = new NpcSpawn(Globals.SelectionSource.FindSpawnAt(x0 - dragxoffset, y0 - dragyoffset))
                                {
                                    X = x0,
                                    Y = y0
                                };

                                tmpMap.Spawns.Add(spawnCopy);
                            }
                        }

                        //Lights
                        LightBase lightCopy;
                        if (Globals.SelectionType != (int) SelectionTypes.CurrentLayer ||
                            Globals.CurrentLayer == LayerOptions.Lights)
                        {
                            if (Globals.SelectionSource.FindLightAt(x0 - dragxoffset, y0 - dragyoffset) != null)
                            {
                                if (tmpMap.FindLightAt(x0, y0) != null)
                                {
                                    tmpMap.Lights.Remove(tmpMap.FindLightAt(x0, y0));
                                }

                                lightCopy = new LightBase(Globals.SelectionSource.FindLightAt(x0 - dragxoffset, y0 - dragyoffset))
                                {
                                    TileX = x0,
                                    TileY = y0
                                };

                                tmpMap.Lights.Add(lightCopy);
                            }
                        }

                        //Events
                        EventBase eventCopy;
                        if (Globals.SelectionType != (int) SelectionTypes.CurrentLayer ||
                            Globals.CurrentLayer == LayerOptions.Events)
                        {
                            if (Globals.SelectionSource.FindEventAt(x0 - dragxoffset, y0 - dragyoffset) != null)
                            {
                                if (tmpMap.FindEventAt(x0, y0) != null)
                                {
                                    tmpMap.LocalEvents.Remove(tmpMap.FindEventAt(x0, y0).Id);
                                }

                                eventCopy = new EventBase(Guid.NewGuid(), Globals.SelectionSource.FindEventAt(x0 - dragxoffset, y0 - dragyoffset)
                                )
                                {
                                    SpawnX = x0,
                                    SpawnY = y0
                                };

                                tmpMap.LocalEvents.Add(eventCopy.Id, eventCopy);
                            }
                        }
                    }
                }
            }
        }

        private void WipeCurrentSelection(MapBase tmpMap)
        {
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
                    dragxoffset = Globals.TotalTileDragX;
                    dragyoffset = Globals.TotalTileDragY;
                }
            }

            var layers = Options.Instance.MapOpts.Layers.All;
            if (Globals.SelectionType == (int)SelectionTypes.CurrentLayer)
            {
                layers = Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer) ? new List<string>() { Globals.CurrentLayer } : new List<string>();
            }

            //start by deleting the source tiles
            foreach (var layer in layers)
            {
                for (var x0 = selX; x0 < selX + selW + 1; x0++)
                {
                    for (var y0 = selY; y0 < selY + selH + 1; y0++)
                    {
                        if (x0 >= 0 &&
                            x0 < Options.MapWidth &&
                            y0 >= 0 &&
                            y0 < Options.MapHeight &&
                            x0 < selX + selW + 1 &&
                            y0 < selY + selH + 1)
                        {
                            tmpMap.Layers[layer][x0, y0].TilesetId = Guid.Empty;
                            tmpMap.Layers[layer][x0, y0].X = 0;
                            tmpMap.Layers[layer][x0, y0].Y = 0;
                            tmpMap.Layers[layer][x0, y0].Autotile = 0;
                        }

                        tmpMap.Autotiles.UpdateAutoTiles(x0, y0, layer, ((MapInstance) tmpMap).GenerateAutotileGrid());
                    }
                }
            }

            tmpMap.Autotiles.UpdateCliffAutotiles(tmpMap, Globals.CurrentLayer);

            for (var x0 = selX; x0 < selX + selW + 1; x0++)
            {
                for (var y0 = selY; y0 < selY + selH + 1; y0++)
                {
                    if (x0 >= 0 &&
                        x0 < Options.MapWidth &&
                        y0 >= 0 &&
                        y0 < Options.MapHeight &&
                        x0 < selX + selW + 1 &&
                        y0 < selY + selH + 1)
                    {
                        //Attributes
                        if (Globals.SelectionType != (int) SelectionTypes.CurrentLayer ||
                            Globals.CurrentLayer == LayerOptions.Attributes)
                        {
                            if (tmpMap.Attributes[x0, y0] != null)
                            {
                                tmpMap.Attributes[x0, y0] = null;
                            }
                        }

                        //Spawns
                        if (Globals.SelectionType != (int) SelectionTypes.CurrentLayer ||
                            Globals.CurrentLayer == LayerOptions.Npcs)
                        {
                            for (var w = 0; w < tmpMap.Spawns.Count; w++)
                            {
                                if (tmpMap.Spawns[w].X == x0 && tmpMap.Spawns[w].Y == y0)
                                {
                                    tmpMap.Spawns.Remove(tmpMap.Spawns[w]);
                                }
                            }
                        }

                        //Lights
                        if (Globals.SelectionType != (int) SelectionTypes.CurrentLayer ||
                            Globals.CurrentLayer == LayerOptions.Lights)
                        {
                            for (var w = 0; w < tmpMap.Lights.Count; w++)
                            {
                                if (tmpMap.Lights[w].TileX == x0 && tmpMap.Lights[w].TileY == y0)
                                {
                                    tmpMap.Lights.Remove(tmpMap.Lights[w]);
                                }
                            }
                        }

                        //Events
                        if (Globals.SelectionType != (int) SelectionTypes.CurrentLayer ||
                            Globals.CurrentLayer == LayerOptions.Events)
                        {
                            if (((MapInstance) tmpMap).FindEventAt(x0, y0) != null)
                            {
                                tmpMap.LocalEvents.Remove(((MapInstance) tmpMap).FindEventAt(x0, y0).Id);
                            }
                        }
                    }
                }
            }
        }

        public void PlaceSelection()
        {
            if (!Globals.Dragging)
            {
                return;
            }

            Globals.Dragging = false;
            Globals.TotalTileDragX = 0;
            Globals.TotalTileDragY = 0;
            Globals.SelectionSource = null;
            Core.Graphics.TilePreviewUpdated = true;
            Globals.CurMapSelX = Globals.CurTileX;
            Globals.CurMapSelY = Globals.CurTileY;
            Globals.CurMapSelW = 0;
            Globals.CurMapSelH = 0;
            Globals.SelectionSource = null;
            Globals.IsPaste = false;
            mMapChanged = true;
        }

        //Flip Functions
        public void FlipVertical()
        {
            var tmpMap = new MapInstance(Globals.CurrentMap);
            int selX = Globals.CurMapSelX,
                selY = Globals.CurMapSelY,
                selW = Globals.CurMapSelW,
                selH = Globals.CurMapSelH;

            MapUndoStates.Add(CurrentMapState);

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

                for (var x = 0; x <= selW; x++)
                {
                    for (var y = 0; y <= selH; y++)
                    {
                        foreach (var layer in Options.Instance.MapOpts.Layers.All)
                        {
                            Globals.CurrentMap.Layers[layer][selX + x, selY + y] = tmpMap.Layers[layer][selX + x, selY + selH - y];
                        }

                        Globals.CurrentMap.Attributes[selX + x, selY + y] = tmpMap.Attributes[selX + x, selY + selH - y];

                        //Copy npc spawns over
                        var tmpSpawn = tmpMap.FindSpawnAt(selX + x, selY + y);
                        if (tmpSpawn != null)
                        {
                            var newSpawn = Globals.CurrentMap.FindSpawnAt(selX + x, selY + y);
                            if (newSpawn != null)
                            {
                                newSpawn.X = selX + x;
                                newSpawn.Y = selY + selH - y;
                            }
                        }

                        //Copy lights over
                        var tmpLight = tmpMap.FindLightAt(selX + x, selY + y);
                        if (tmpLight != null)
                        {
                            var newLight = Globals.CurrentMap.FindLightAt(selX + x, selY + y);
                            if (newLight != null)
                            {
                                newLight.TileX = selX + x;
                                newLight.TileY = selY + selH - y;
                            }
                        }

                        //Copy events over
                        var tmpEvent = tmpMap.FindEventAt(selX + x, selY + y);
                        if (tmpEvent != null)
                        {
                            Globals.CurrentMap.LocalEvents[tmpEvent.Id].SpawnX = selX + x;
                            Globals.CurrentMap.LocalEvents[tmpEvent.Id].SpawnY = selY + selH - y;
                        }
                    }
                }

                Core.Graphics.TilePreviewUpdated = true;
            }
        }

        public void FlipHorizontal()
        {
            var tmpMap = new MapInstance(Globals.CurrentMap);
            int selX = Globals.CurMapSelX,
                selY = Globals.CurMapSelY,
                selW = Globals.CurMapSelW,
                selH = Globals.CurMapSelH;

            MapUndoStates.Add(CurrentMapState);

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

                for (var x = 0; x <= selW; x++)
                {
                    for (var y = 0; y <= selH; y++)
                    {
                        foreach (var layer in Options.Instance.MapOpts.Layers.All)
                        {
                            Globals.CurrentMap.Layers[layer][selX + x, selY + y] = tmpMap.Layers[layer][selX + selW - x, selY + y];
                        }

                        Globals.CurrentMap.Attributes[selX + x, selY + y] = tmpMap.Attributes[selX + selW - x, selY + y];

                        //Copy npc spawns over
                        var tmpSpawn = tmpMap.FindSpawnAt(selX + x, selY + y);
                        if (tmpSpawn != null)
                        {
                            var newSpawn = Globals.CurrentMap.FindSpawnAt(selX + x, selY + y);
                            if (newSpawn != null)
                            {
                                newSpawn.X = selX + selW - x;
                                newSpawn.Y = selY + y;
                            }
                        }

                        //Copy lights over
                        var tmpLight = tmpMap.FindLightAt(selX + x, selY + y);
                        if (tmpLight != null)
                        {
                            var newLight = Globals.CurrentMap.FindLightAt(selX + x, selY + y);
                            if (newLight != null)
                            {
                                newLight.TileX = selX + selW - x;
                                newLight.TileY = selY + y;
                            }
                        }

                        //Copy events over
                        var tmpEvent = tmpMap.FindEventAt(selX + x, selY + y);
                        if (tmpEvent != null)
                        {
                            Globals.CurrentMap.LocalEvents[tmpEvent.Id].SpawnX = selX + selW - x;
                            Globals.CurrentMap.LocalEvents[tmpEvent.Id].SpawnY = selY + y;
                        }
                    }
                }

                Core.Graphics.TilePreviewUpdated = true;
            }
        }

        //Cut Copy Paste Functions
        public void Cut()
        {
            Copy();
            WipeCurrentSelection(Globals.CurrentMap);
            Core.Graphics.TilePreviewUpdated = true;
            if (CurrentMapState != null)
            {
                MapUndoStates.Add(CurrentMapState);
            }

            MapRedoStates.Clear();
            CurrentMapState = Globals.CurrentMap.SaveInternal();
            mMapChanged = false;
        }

        public void Copy()
        {
            if (Globals.CurrentTool == (int) EditingTool.Selection)
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
                Globals.CurrentTool = (int) EditingTool.Selection;
                int selX1 = Globals.CurMapSelX,
                    selY1 = Globals.CurMapSelY,
                    selW1 = Globals.CurMapSelW,
                    selH1 = Globals.CurMapSelH;

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
                int selX = Globals.CurMapSelX,
                    selY = Globals.CurMapSelY,
                    selW = Globals.CurMapSelW,
                    selH = Globals.CurMapSelH;

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
                Core.Graphics.TilePreviewUpdated = true;
            }
        }

        private void pnlMapContainer_Scroll(object sender, ScrollEventArgs e)
        {
            Main.DrawFrame();
        }

        private void picMap_Resize(object sender, EventArgs e)
        {
        }

        private void pnlMapContainer_Resize(object sender, EventArgs e)
        {
            if (!Options.Loaded)
            {
                return;
            }

            picMap.Size = pnlMapContainer.ClientSize;
            picMap.MinimumSize = new Size(
                (Options.MapWidth + 2) * Options.TileWidth, (Options.MapHeight + 2) * Options.TileHeight
            );

            Core.Graphics.CurrentView = new Rectangle(
                (picMap.Size.Width - Options.MapWidth * Options.TileWidth) / 2,
                (picMap.Size.Height - Options.MapHeight * Options.TileHeight) / 2, picMap.Size.Width, picMap.Size.Height
            );

            CreateSwapChain();
        }

        private void frmMapEditor_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void picMap_Paint(object sender, PaintEventArgs e)
        {
        }

    }

}
