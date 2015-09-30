using Intersect_Editor.Classes;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect_Editor.Forms
{
    public partial class frmMapEditor : DockContent
    {
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
            picMap.Width = (Globals.MapWidth + 2) * Globals.TileWidth;
            picMap.Height = (Globals.MapHeight + 2) * Globals.TileHeight;
        }
        private void frmMapEditor_DockStateChanged(object sender, EventArgs e)
        {
            if (Graphics.RenderWindow != null && !Globals.MapEditorWindow.picMap.IsDisposed)
            {
                Graphics.RenderWindow.Close();
                Graphics.RenderWindow.Dispose();
                Graphics.RenderWindow = new RenderWindow(Globals.MapEditorWindow.picMap.Handle);
            }
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
                CurrentMapState = tmpMap.Save();
            }
        }
        public void AddUndoState()
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (CurrentMapState != null)
            {
                MapUndoStates.Add(CurrentMapState);
                MapRedoStates.Clear();
                CurrentMapState = tmpMap.Save();
            }
            MapChanged = false;
        }

        //PicMap Functions
        public void picMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (Globals.EditingLight != null) { return; }
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];

            if (e.X >= picMap.Width - Globals.TileWidth || e.Y >= picMap.Height - Globals.TileHeight) { return; }
            if (e.X < Globals.TileWidth || e.Y < Globals.TileHeight) { return; }

            if (CurrentMapState == null)
            {
                CurrentMapState = tmpMap.Save();
            }

            switch (e.Button)
            {
                case MouseButtons.Left:
                    Globals.MouseButton = 0;

                    switch (Globals.CurrentLayer)
                    {
                        case Constants.LayerCount:
                            Globals.MapLayersWindow.PlaceAttribute(Globals.GameMaps[Globals.CurrentMap]);
                            MapChanged = true;
                            break;
                        case Constants.LayerCount + 1:
                            Light tmpLight;
                            if ((tmpLight = Globals.GameMaps[Globals.CurrentMap].FindLightAt(Globals.CurTileX, Globals.CurTileY)) == null)
                            {
                                tmpLight = new Light(Globals.CurTileX, Globals.CurTileY);
                                Globals.MapLayersWindow.tabControl.SelectedTab = Globals.MapLayersWindow.tabLights;
                                Globals.MapLayersWindow.pnlLight.Show();
                                Globals.GameMaps[Globals.CurrentMap].Lights.Add(tmpLight);
                                Graphics.LightsChanged = true;
                                Globals.BackupLight = new Light(tmpLight.TileX, tmpLight.TileY)
                                {
                                    OffsetX = tmpLight.OffsetX
                                };
                                Globals.BackupLight.OffsetX = tmpLight.OffsetX;
                                Globals.BackupLight.Intensity = tmpLight.Intensity;
                                Globals.BackupLight.Range = tmpLight.Range;
                                Globals.MapLayersWindow.txtLightIntensity.Text = "" + tmpLight.Intensity;
                                Globals.MapLayersWindow.txtLightRange.Text = "" + tmpLight.Range;
                                Globals.MapLayersWindow.txtLightOffsetX.Text = "" + tmpLight.OffsetX;
                                Globals.MapLayersWindow.txtLightOffsetY.Text = "" + tmpLight.OffsetY;
                                Globals.MapLayersWindow.scrlLightIntensity.Value = (int)(tmpLight.Intensity * 10000.0);
                                Globals.MapLayersWindow.scrlLightRange.Value = tmpLight.Range;
                                Globals.EditingLight = tmpLight;
                                MapChanged = true;
                            }
                            else
                            {
                                Globals.MapLayersWindow.tabControl.SelectedTab = Globals.MapLayersWindow.tabLights;
                                Globals.MapLayersWindow.pnlLight.Show();
                                Globals.BackupLight = new Light(tmpLight.TileX, tmpLight.TileY)
                                {
                                    OffsetX = tmpLight.OffsetX
                                };
                                Globals.BackupLight.OffsetX = tmpLight.OffsetX;
                                Globals.BackupLight.Intensity = tmpLight.Intensity;
                                Globals.BackupLight.Range = tmpLight.Range;
                                Globals.MapLayersWindow.txtLightIntensity.Text = "" + tmpLight.Intensity;
                                Globals.MapLayersWindow.txtLightRange.Text = "" + tmpLight.Range;
                                Globals.MapLayersWindow.txtLightOffsetX.Text = "" + tmpLight.OffsetX;
                                Globals.MapLayersWindow.txtLightOffsetY.Text = "" + tmpLight.OffsetY;
                                Globals.MapLayersWindow.scrlLightIntensity.Value = (int)(tmpLight.Intensity * 10000.0);
                                Globals.MapLayersWindow.scrlLightRange.Value = tmpLight.Range;
                                Globals.EditingLight = tmpLight;
                            }
                            break;
                        case Constants.LayerCount + 2:
                            EventStruct tmpEvent;
                            FrmEvent tmpEventEditor;
                            if ((tmpEvent = Globals.GameMaps[Globals.CurrentMap].FindEventAt(Globals.CurTileX, Globals.CurTileY)) == null)
                            {
                                tmpEvent = new EventStruct(Globals.CurTileX, Globals.CurTileY);
                                Globals.GameMaps[Globals.CurrentMap].Events.Add(tmpEvent);
                                tmpEventEditor = new FrmEvent
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
                                tmpEventEditor = new FrmEvent { MyEvent = tmpEvent };
                                tmpEventEditor.InitEditor();
                                tmpEventEditor.Show();
                            }
                            break;
                        case Constants.LayerCount + 3:
                            if (Globals.MapLayersWindow.lstMapNpcs.SelectedIndex > -1 && Globals.MapLayersWindow.rbDeclared.Checked == true)
                            {
                                Globals.GameMaps[Globals.CurrentMap].Spawns[Globals.MapLayersWindow.lstMapNpcs.SelectedIndex].X = Globals.CurTileX;
                                Globals.GameMaps[Globals.CurrentMap].Spawns[Globals.MapLayersWindow.lstMapNpcs.SelectedIndex].Y = Globals.CurTileY;
                                MapChanged = true;
                            }
                            break;
                        default:
                            if (Globals.Autotilemode == 0)
                            {
                                for (var x = 0; x <= Globals.CurSelW; x++)
                                {
                                    for (var y = 0; y <= Globals.CurSelH; y++)
                                    {
                                        if (Globals.CurTileX + x >= 0 && Globals.CurTileX + x < Globals.MapWidth && Globals.CurTileY + y >= 0 && Globals.CurTileY + y < Globals.MapHeight)
                                        {
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].TilesetIndex = Globals.CurrentTileset;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].X = Globals.CurSelX + x;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].Y = Globals.CurSelY + y;
                                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].Autotile = 0;
                                            tmpMap.Autotiles.InitAutotiles();
                                        }
                                    }
                                }
                                MapChanged = true;
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = Globals.CurrentTileset;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = (byte)Globals.Autotilemode;
                                tmpMap.Autotiles.InitAutotiles();
                                MapChanged = true;
                            }
                            break;
                    }
                    break;
                case MouseButtons.Right:
                    Globals.MouseButton = 1;

                    switch (Globals.CurrentLayer)
                    {
                        case Constants.LayerCount:
                            if (Globals.MapLayersWindow.RemoveAttribute()) { MapChanged = true; }
                            break;
                        case Constants.LayerCount + 1:
                            Light tmpLight;
                            if ((tmpLight = Globals.GameMaps[Globals.CurrentMap].FindLightAt(Globals.CurTileX, Globals.CurTileY)) != null)
                            {
                                Globals.GameMaps[Globals.CurrentMap].Lights.Remove(tmpLight);
                                Graphics.LightsChanged = true;
                                MapChanged = true;
                            }
                            break;
                        case Constants.LayerCount + 2:
                            EventStruct tmpEvent;
                            if ((tmpEvent = Globals.GameMaps[Globals.CurrentMap].FindEventAt(Globals.CurTileX, Globals.CurTileY)) != null)
                            {
                                Globals.GameMaps[Globals.CurrentMap].Events.Remove(tmpEvent);
                                tmpEvent.Deleted = 1;
                                MapChanged = true;
                            }
                            break;
                        case Constants.LayerCount + 3:
                            break;
                        default:
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                            tmpMap.Autotiles.InitAutotiles();
                            MapChanged = true;
                            break;
                    }
                    break;
            }
            if (Globals.CurTileX == 0)
            {
                if (tmpMap.Left > -1)
                {
                    if (Globals.GameMaps[tmpMap.Left] != null)
                    {
                        Globals.GameMaps[tmpMap.Left].Autotiles.InitAutotiles();
                    }
                }
            }
            if (Globals.CurTileY == 0)
            {
                if (tmpMap.Up > -1)
                {
                    if (Globals.GameMaps[tmpMap.Up] != null)
                    {
                        Globals.GameMaps[tmpMap.Up].Autotiles.InitAutotiles();
                    }
                }
            }
            if (Globals.CurTileX == Globals.MapWidth - 1)
            {
                if (tmpMap.Right > -1)
                {
                    if (Globals.GameMaps[tmpMap.Right] != null)
                    {
                        Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles();
                    }
                }
            }
            if (Globals.CurTileY != Globals.MapHeight - 1) return;
            if (tmpMap.Down <= -1) return;
            if (Globals.GameMaps[tmpMap.Down] != null)
            {
                Globals.GameMaps[tmpMap.Down].Autotiles.InitAutotiles();
            }
        }
        private void picMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (Globals.EditingLight != null) { return; }
            Globals.MouseX = e.X;
            Globals.MouseY = e.Y;
            if (e.X >= picMap.Width - Globals.TileWidth || e.Y >= picMap.Height - Globals.TileHeight) { return; }
            if (e.X < Globals.TileWidth || e.Y < Globals.TileHeight) { return; }
            int oldx = Globals.CurTileX;
            int oldy = Globals.CurTileY;
            Globals.CurTileX = (int)Math.Floor((double)(e.X - Globals.TileWidth) / Globals.TileWidth);
            Globals.CurTileY = (int)Math.Floor((double)(e.Y - Globals.TileHeight) / Globals.TileHeight);
            if (Globals.CurTileX < 0) { Globals.CurTileX = 0; }
            if (Globals.CurTileY < 0) { Globals.CurTileY = 0; }

            if (oldx != Globals.CurTileX || oldy != Globals.CurTileY)
            {
                Graphics.TilePreviewUpdated = true;
            }

            if (Globals.MouseButton > -1)
            {
                var tmpMap = Globals.GameMaps[Globals.CurrentMap];
                if (Globals.MouseButton == 0)
                {
                    if (Globals.CurrentLayer == Constants.LayerCount)
                    {
                        Globals.MapLayersWindow.PlaceAttribute(tmpMap);
                    }
                    else if (Globals.CurrentLayer == Constants.LayerCount + 1)
                    {

                    }
                    else if (Globals.CurrentLayer == Constants.LayerCount + 2)
                    {

                    }
                    else
                    {

                        // Check for adding NPC Spawns in map properties
                        if (Globals.CurrentLayer == Constants.LayerCount + 3)
                        {
                            if (Globals.MapLayersWindow.rbDeclared.Checked == true && Globals.MapLayersWindow.lstMapNpcs.Items.Count > 0)
                            {
                                Globals.GameMaps[Globals.CurrentMap].Spawns[Globals.MapLayersWindow.lstMapNpcs.SelectedIndex].X = Globals.CurTileX;
                                Globals.GameMaps[Globals.CurrentMap].Spawns[Globals.MapLayersWindow.lstMapNpcs.SelectedIndex].Y = Globals.CurTileY;
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
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].TilesetIndex = Globals.CurrentTileset;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].X = Globals.CurSelX + x;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].Y = Globals.CurSelY + y;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].Autotile = 0;
                                        tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX + x, Globals.CurTileY + y, Globals.CurrentLayer);
                                    }
                                }
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = Globals.CurrentTileset;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = (byte)Globals.Autotilemode;
                                tmpMap.Autotiles.UpdateAutoTiles(Globals.CurTileX, Globals.CurTileY, Globals.CurrentLayer);
                            }
                        }
                    }
                    if (Globals.CurTileX == 0)
                    {
                        if (tmpMap.Left > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Left] != null)
                            {
                                Globals.GameMaps[tmpMap.Left].Autotiles.InitAutotiles();
                            }
                        }
                    }
                    if (Globals.CurTileY == 0)
                    {
                        if (tmpMap.Up > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Up] != null)
                            {
                                Globals.GameMaps[tmpMap.Up].Autotiles.InitAutotiles();
                            }
                        }
                    }
                    if (Globals.CurTileX == Globals.MapWidth - 1)
                    {
                        if (tmpMap.Right > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Right] != null)
                            {
                                Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles();
                            }
                        }
                    }
                    if (Globals.CurTileY == Globals.MapHeight - 1)
                    {
                        if (tmpMap.Down > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Down] != null)
                            {
                                Globals.GameMaps[tmpMap.Down].Autotiles.InitAutotiles();
                            }
                        }
                    }
                }
                else if (Globals.MouseButton == 1)
                {
                    if (Globals.CurrentLayer == Constants.LayerCount)
                    {
                        Globals.MapLayersWindow.RemoveAttribute();
                    }
                    else if (Globals.CurrentLayer == Constants.LayerCount + 1)
                    {

                    }
                    else if (Globals.CurrentLayer == Constants.LayerCount + 2)
                    {

                    }
                    else if (Globals.CurrentLayer == Constants.LayerCount + 3) //Npcs
                    {

                    }
                    else
                    {
                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = -1;
                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                        tmpMap.Autotiles.InitAutotiles();
                    }
                    if (Globals.CurTileX == 0)
                    {
                        if (tmpMap.Left > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Left] != null)
                            {
                                Globals.GameMaps[tmpMap.Left].Autotiles.InitAutotiles();
                            }
                        }
                    }
                    if (Globals.CurTileY == 0)
                    {
                        if (tmpMap.Up > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Up] != null)
                            {
                                Globals.GameMaps[tmpMap.Up].Autotiles.InitAutotiles();
                            }
                        }
                    }
                    if (Globals.CurTileX == Globals.MapWidth - 1)
                    {
                        if (tmpMap.Right > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Right] != null)
                            {
                                Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles();
                            }
                        }
                    }
                    if (Globals.CurTileY == Globals.MapHeight - 1)
                    {
                        if (tmpMap.Down > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Down] != null)
                            {
                                Globals.GameMaps[tmpMap.Down].Autotiles.InitAutotiles();
                            }
                        }
                    }
                }
            }
        }
        public void picMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (Globals.EditingLight != null) { return; }
            Globals.MouseButton = -1;
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (MapChanged)
            {
                MapUndoStates.Add(CurrentMapState);
                MapRedoStates.Clear();
                CurrentMapState = tmpMap.Save();
                MapChanged = false;
            }

        }
        private void picMap_DoubleClick(object sender, EventArgs e)
        {
            if (Globals.MouseX >= Globals.TileWidth && Globals.MouseX <= (Globals.MapWidth + 2) * Globals.TileWidth - Globals.TileWidth)
            {
                if (Globals.MouseY >= 0 && Globals.MouseY <= Globals.TileHeight)
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
                else if (Globals.MouseY >= (Globals.MapHeight + 2) * Globals.TileHeight - Globals.TileHeight && Globals.MouseY <= (Globals.MapHeight + 2) * Globals.TileHeight)
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
            if (Globals.MouseY < Globals.TileHeight || Globals.MouseY > (Globals.MapHeight + 2) * Globals.TileHeight - Globals.TileHeight) return;
            if (Globals.MouseX >= 0 & Globals.MouseX <= Globals.TileWidth)
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
            else if (Globals.MouseX >= (Globals.MapWidth + 2) * Globals.TileWidth - Globals.TileWidth && Globals.MouseX <= (Globals.MapWidth + 2) * Globals.TileWidth)
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
        }
        private void picMap_MouseEnter(object sender, EventArgs e)
        {
            pnlMapContainer.Focus();
        }

        //Fill/Erase Functions
        public void FillLayer()
        {
            var oldCurSelX = Globals.CurTileX;
            var oldCurSelY = Globals.CurTileY;
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (CurrentMapState == null)
            {
                CurrentMapState = tmpMap.Save();
            }
            if (MessageBox.Show(@"Are you sure you want to fill this layer?", @"Fill Layer", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        Globals.CurTileX = x;
                        Globals.CurTileY = y;

                        if (Globals.CurrentLayer == Constants.LayerCount)
                        {
                            Globals.MapLayersWindow.PlaceAttribute(tmpMap);
                        }
                        else if (Globals.CurrentLayer < Constants.LayerCount)
                        {
                            if (Globals.Autotilemode == 0)
                            {
                                for (var x1 = 0; x1 <= Globals.CurSelW; x1++)
                                {
                                    for (var y1 = 0; y1 <= Globals.CurSelH; y1++)
                                    {
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x1, Globals.CurTileY + y1].TilesetIndex = Globals.CurrentTileset;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x1, Globals.CurTileY + y1].X = Globals.CurSelX + x1;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x1, Globals.CurTileY + y1].Y = Globals.CurSelY + y1;
                                        tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x1, Globals.CurTileY + y1].Autotile = 0;
                                    }
                                }
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
                tmpMap.Autotiles.InitAutotiles();
                Globals.CurTileX = oldCurSelX;
                Globals.CurTileY = oldCurSelY;
                if (tmpMap.Left > -1)
                {
                    if (Globals.GameMaps[tmpMap.Left] != null)
                    {
                        Globals.GameMaps[tmpMap.Left].Autotiles.InitAutotiles();
                    }
                }
                if (tmpMap.Up > -1)
                {
                    if (Globals.GameMaps[tmpMap.Up] != null)
                    {
                        Globals.GameMaps[tmpMap.Up].Autotiles.InitAutotiles();
                    }
                }
                if (tmpMap.Right > -1)
                {
                    if (Globals.GameMaps[tmpMap.Right] != null)
                    {
                        Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles();
                    }
                }
                if (tmpMap.Down > -1)
                {
                    if (Globals.GameMaps[tmpMap.Down] != null)
                    {
                        Globals.GameMaps[tmpMap.Down].Autotiles.InitAutotiles();
                    }
                }

                if (!CurrentMapState.SequenceEqual(tmpMap.Save()))
                {
                    MapUndoStates.Add(CurrentMapState);
                    MapRedoStates.Clear();
                    CurrentMapState = tmpMap.Save();
                }
            }
        }
        public void EraseLayer()
        {
            var oldCurSelX = Globals.CurTileX;
            var oldCurSelY = Globals.CurTileY;
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (CurrentMapState == null)
            {
                CurrentMapState = tmpMap.Save();
            }
            if (MessageBox.Show(@"Are you sure you want to erase this layer?", @"Fill Layer", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        Globals.CurTileX = x;
                        Globals.CurTileY = y;

                        if (Globals.CurrentLayer == Constants.LayerCount)
                        {
                            Globals.MapLayersWindow.RemoveAttribute();
                        }
                        else if (Globals.CurrentLayer < Constants.LayerCount)
                        {
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                        }
                    }
                }
                tmpMap.Autotiles.InitAutotiles();
                Globals.CurTileX = oldCurSelX;
                Globals.CurTileY = oldCurSelY;
                if (tmpMap.Left > -1)
                {
                    if (Globals.GameMaps[tmpMap.Left] != null)
                    {
                        Globals.GameMaps[tmpMap.Left].Autotiles.InitAutotiles();
                    }
                }
                if (tmpMap.Up > -1)
                {
                    if (Globals.GameMaps[tmpMap.Up] != null)
                    {
                        Globals.GameMaps[tmpMap.Up].Autotiles.InitAutotiles();
                    }
                }
                if (tmpMap.Right > -1)
                {
                    if (Globals.GameMaps[tmpMap.Right] != null)
                    {
                        Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles();
                    }
                }
                if (tmpMap.Down > -1)
                {
                    if (Globals.GameMaps[tmpMap.Down] != null)
                    {
                        Globals.GameMaps[tmpMap.Down].Autotiles.InitAutotiles();
                    }
                }

                if (!CurrentMapState.SequenceEqual(tmpMap.Save()))
                {
                    MapUndoStates.Add(CurrentMapState);
                    MapRedoStates.Clear();
                    CurrentMapState = tmpMap.Save();
                }
            }
        }

    }
}
