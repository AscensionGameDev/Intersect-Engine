using System;
using System.IO;
using System.Windows.Forms;
using Intersect_Editor.Classes;

namespace Intersect_Editor.Forms
{
    public partial class FrmMain : Form
    {
        //General Editting Variables
        bool _tMouseDown;

        //Initialization & Setup Functions
        public FrmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            InitEditor();
            Show();
            EditorLoop.StartLoop(this);
        }
        private void InitEditor()
        {
            EnterMap(0);
            Graphics.InitSfml(this);
            UpdateScrollBars();
            InitLayerMenu();
            cmbAutotile.SelectedIndex = 0;
            Globals.InEditor = true;
            if (Globals.GameMaps[Globals.CurrentMap].Up > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.CurrentMap].Up); }
            if (Globals.GameMaps[Globals.CurrentMap].Down > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.CurrentMap].Down); }
            if (Globals.GameMaps[Globals.CurrentMap].Left > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.CurrentMap].Left); }
            if (Globals.GameMaps[Globals.CurrentMap].Right > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.CurrentMap].Right); }
        }
        private void InitLayerMenu()
        {
            for (var i = 0; i < Constants.LayerCount + 3; i++)
            {
                var tmpTsi = layerMenu.DropDownItems.Add("Layer " + (i + 1));
                tmpTsi.Click += HandleLayerClick;
                tmpTsi.Tag = i;
                if (i == Constants.LayerCount)
                {
                    tmpTsi.Text = @"Attributes";
                }
                if (i == Constants.LayerCount + 1)
                {
                    tmpTsi.Text = @"Lights";
                }
                if (i == Constants.LayerCount + 2)
                {
                    tmpTsi.Text = @"Events";
                }
            }
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
        public void EnterMap(int mapNum)
        {
            Globals.CurrentMap = mapNum;
            if (Globals.GameMaps[mapNum] != null)
            {
                Text = @"Intersect Editor - Map# " + mapNum + @" " + Globals.GameMaps[mapNum].MyName + @" Revision: " + Globals.GameMaps[mapNum].Revision;
            }
            picMap.Visible = false;
            vScrollMap.Value = 0;
            hScrollMap.Value = 0;
            picMap.Left = 0;
            picMap.Top = 0;
            PacketSender.SendNeedMap(Globals.CurrentMap);
        }

        //MenuBar Functions
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private void HandleLayerClick(object sender, EventArgs e)
        {
            var tmpTsi = (ToolStripItem)sender;
            Globals.CurrentLayer = (int)tmpTsi.Tag;
            tmpTsi.Select();
            grpAttributes.Hide();
            if (Globals.CurrentLayer == Constants.LayerCount)
            {
                lblCurLayer.Text = @"Layer: Attributes";
                grpAttributes.Show();
            }
            else
            {
                lblCurLayer.Text = @"Layer #" + (int)tmpTsi.Tag;
            }
        }
        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(@"Are you sure you want to create a new, unconnected map?", @"New Map",
                    MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PacketSender.SendMap(Globals.CurrentMap);
            }
            else
            {
                PacketSender.SendCreateMap(-1, Globals.CurrentMap);
            }
        }

        private void mapPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grpMapProperties.BringToFront();
            txtMapName.Text = Globals.GameMaps[Globals.CurrentMap].MyName;
            chkIndoors.Checked = Globals.GameMaps[Globals.CurrentMap].IsIndoors;
            grpMapProperties.Show();
        }
        private void nightTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics.NightEnabled = !Graphics.NightEnabled;
            nightTimeToolStripMenuItem.Checked = Graphics.NightEnabled;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Are you sure you want to save this map?", @"Save Map", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PacketSender.SendMap(Globals.CurrentMap);
            }
        }
        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var oldCurSelX = Globals.CurTileX;
            var oldCurSelY = Globals.CurTileY;
            if (MessageBox.Show(@"Are you sure you want to fill this layer?", @"Fill Layer", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (var x = 0; x < Constants.MapWidth; x++)
                {
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        Globals.CurTileX = x;
                        Globals.CurTileY = y;
                        picMap_MouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, x * 32 + 32, y * 32 + 32, 0));
                        picMap_MouseUp(null, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
                    }
                }
                Globals.CurTileX = oldCurSelX;
                Globals.CurTileY = oldCurSelY;
            }
        }
        private void eraseLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(@"Are you sure you want to erase this layer?", @"Erase Layer", MessageBoxButtons.YesNo) !=
                DialogResult.Yes) return;
            for (var x = 0; x < Constants.MapWidth; x++)
            {
                for (var y = 0; y < Constants.MapHeight; y++)
                {
                    Globals.CurTileX = x;
                    Globals.CurTileY = y;
                    picMap_MouseDown(null, new MouseEventArgs(MouseButtons.Right, 1, x * 32 + 32, y * 32 + 32, 0));
                    picMap_MouseUp(null, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
                }
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
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void itemEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendItemEditor();
        }
        private void npcEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendNpcEditor();
        }
        private void spellEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendSpellEditor();
        }
        private void animationEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendAnimationEditor();
        }

        //Tileset Area
        private void cmbTilesets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (File.Exists("Resources/Tilesets/" + Globals.Tilesets[cmbTilesets.SelectedIndex]))
            {
                Globals.CurrentTileset = cmbTilesets.SelectedIndex;
                Graphics.InitTileset(Globals.CurrentTileset, this);
            }
            else
            {
                cmbTilesets.SelectedIndex = Globals.CurrentTileset;
            }
        }
        private void picTileset_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > picTileset.Width || e.Y > picTileset.Height) { return; }
            _tMouseDown = true;
            Globals.CurSelX = (int)Math.Floor((double)e.X / 32);
            Globals.CurSelY = (int)Math.Floor((double)e.Y / 32);
            Globals.CurSelW = 0;
            Globals.CurSelH = 0;
            if (Globals.CurSelX < 0) { Globals.CurSelX = 0; }
            if (Globals.CurSelY < 0) { Globals.CurSelY = 0; }
            switch (Globals.Autotilemode)
            {
                case 1:
                case 5:
                    Globals.CurSelW = 1;
                    Globals.CurSelH = 2;
                    break;
                case 2:
                    Globals.CurSelW = 0;
                    Globals.CurSelH = 0;
                    break;
                case 3:
                    Globals.CurSelW = 5;
                    Globals.CurSelH = 2;
                    break;
                case 4:
                    Globals.CurSelW = 1;
                    Globals.CurSelH = 1;
                    break;
            }

        }
        private void picTileset_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > picTileset.Width || e.Y > picTileset.Height) { return; }
            if (_tMouseDown && Globals.Autotilemode == 0)
            {
                var tmpX = (int)Math.Floor((double)e.X / 32);
                var tmpY = (int)Math.Floor((double)e.Y / 32);
                Globals.CurSelW = tmpX - Globals.CurSelX;
                Globals.CurSelH = tmpY - Globals.CurSelY;
            }
        }
        private void picTileset_MouseUp(object sender, MouseEventArgs e)
        {
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
            Globals.CurSelX = selX;
            Globals.CurSelY = selY;
            Globals.CurSelW = selW;
            Globals.CurSelH = selH;
            _tMouseDown = false;
        }
        private void cmbAutotile_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Autotilemode = cmbAutotile.SelectedIndex;
            switch (Globals.Autotilemode)
            {
                case 1:
                case 5:
                    Globals.CurSelW = 1;
                    Globals.CurSelH = 2;
                    break;
                case 2:
                    Globals.CurSelW = 0;
                    Globals.CurSelH = 0;
                    break;
                case 3:
                    Globals.CurSelW = 5;
                    Globals.CurSelH = 2;
                    break;
                case 4:
                    Globals.CurSelW = 1;
                    Globals.CurSelH = 1;
                    break;

            }
        }
        private void vScrollTileset_Scroll(object sender, ScrollEventArgs e)
        {
            picTileset.Top = -vScrollTileset.Value;
        }
        private void hScrollTileset_Scroll(object sender, ScrollEventArgs e)
        {
            picTileset.Left = -hScrollTileset.Value;
        }

        //Map Area
        private void picMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (Globals.EditingLight != null) { return; }
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Globals.MouseButton = 0;
                    switch (Globals.CurrentLayer)
                    {
                        case Constants.LayerCount:
                            PlaceAttribute();
                            break;
                        case Constants.LayerCount + 1:
                            Light tmpLight;
                            if ((tmpLight = Globals.GameMaps[Globals.CurrentMap].FindLightAt(Globals.CurTileX, Globals.CurTileY)) == null)
                            {
                                tmpLight = new Light(Globals.CurTileX, Globals.CurTileY);
                                grpLightEditor.BringToFront();
                                grpLightEditor.Show();
                                Globals.GameMaps[Globals.CurrentMap].Lights.Add(tmpLight);
                                Graphics.LightsChanged = true;
                                Globals.BackupLight = new Light(tmpLight.TileX, tmpLight.TileY)
                                {
                                    OffsetX = tmpLight.OffsetX
                                };
                                Globals.BackupLight.OffsetX = tmpLight.OffsetX;
                                Globals.BackupLight.Intensity = tmpLight.Intensity;
                                Globals.BackupLight.Range = tmpLight.Range;
                                txtLightIntensity.Text = "" + tmpLight.Intensity;
                                txtLightRange.Text = "" + tmpLight.Range;
                                txtLightOffsetX.Text = "" + tmpLight.OffsetX;
                                txtLightOffsetY.Text = "" + tmpLight.OffsetY;
                                scrlLightIntensity.Value = (int)(tmpLight.Intensity * 10000.0);
                                scrlLightRange.Value = tmpLight.Range;
                                Globals.EditingLight = tmpLight;
                            }
                            else
                            {
                                grpLightEditor.BringToFront();
                                grpLightEditor.Show();
                                Globals.BackupLight = new Light(tmpLight.TileX, tmpLight.TileY)
                                {
                                    OffsetX = tmpLight.OffsetX
                                };
                                Globals.BackupLight.OffsetX = tmpLight.OffsetX;
                                Globals.BackupLight.Intensity = tmpLight.Intensity;
                                Globals.BackupLight.Range = tmpLight.Range;
                                txtLightIntensity.Text = "" + tmpLight.Intensity;
                                txtLightRange.Text = "" + tmpLight.Range;
                                txtLightOffsetX.Text = "" + tmpLight.OffsetX;
                                txtLightOffsetY.Text = "" + tmpLight.OffsetY;
                                scrlLightIntensity.Value = (int)(tmpLight.Intensity * 10000.0);
                                scrlLightRange.Value = tmpLight.Range;
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
                            }
                            else
                            {
                                tmpEventEditor = new FrmEvent { MyEvent = tmpEvent };
                                tmpEventEditor.InitEditor();
                                tmpEventEditor.Show();
                            }
                            break;
                        default:
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
                                        tmpMap.Autotiles.InitAutotiles();
                                    }
                                }
                            }
                            else
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = Globals.CurrentTileset;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = (byte)Globals.Autotilemode;
                                tmpMap.Autotiles.InitAutotiles();
                            }
                            break;
                    }
                    break;
                case MouseButtons.Right:
                    Globals.MouseButton = 1;
                    switch (Globals.CurrentLayer)
                    {
                        case Constants.LayerCount:
                            RemoveAttribute();
                            break;
                        case Constants.LayerCount + 1:
                            Light tmpLight;
                            if ((tmpLight = Globals.GameMaps[Globals.CurrentMap].FindLightAt(Globals.CurTileX, Globals.CurTileY)) != null)
                            {
                                Globals.GameMaps[Globals.CurrentMap].Lights.Remove(tmpLight);
                                Graphics.LightsChanged = true;
                            }
                            break;
                        case Constants.LayerCount + 2:
                            EventStruct tmpEvent;
                            if ((tmpEvent = Globals.GameMaps[Globals.CurrentMap].FindEventAt(Globals.CurTileX, Globals.CurTileY)) != null)
                            {
                                Globals.GameMaps[Globals.CurrentMap].Events.Remove(tmpEvent);
                                tmpEvent.Deleted = 1;
                            }
                            break;
                        default:
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                            tmpMap.Autotiles.InitAutotiles();
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
            if (Globals.CurTileX == Constants.MapWidth - 1)
            {
                if (tmpMap.Right > -1)
                {
                    if (Globals.GameMaps[tmpMap.Right] != null)
                    {
                        Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles();
                    }
                }
            }
            if (Globals.CurTileY != Constants.MapHeight - 1) return;
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
            if (e.X >= picMap.Width - 32 || e.Y >= picMap.Height - 32) { return; }
            if (e.X < 32 || e.Y < 32) { return; }
            Globals.CurTileX = (int)Math.Floor((double)(e.X - 32) / 32);
            Globals.CurTileY = (int)Math.Floor((double)(e.Y - 32) / 32);
            if (Globals.CurTileX < 0) { Globals.CurTileX = 0; }
            if (Globals.CurTileY < 0) { Globals.CurTileY = 0; }

            if (Globals.MouseButton > -1)
            {
                var tmpMap = Globals.GameMaps[Globals.CurrentMap];
                if (Globals.MouseButton == 0)
                {
                    if (Globals.CurrentLayer == Constants.LayerCount)
                    {
                        PlaceAttribute();
                    }
                    else if (Globals.CurrentLayer == Constants.LayerCount + 1)
                    {

                    }
                    else if (Globals.CurrentLayer == Constants.LayerCount + 2)
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
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].TilesetIndex = Globals.CurrentTileset;
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].X = Globals.CurSelX + x;
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].Y = Globals.CurSelY + y;
                                    tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX + x, Globals.CurTileY + y].Autotile = 0;
                                    tmpMap.Autotiles.InitAutotiles();
                                }
                            }
                        }
                        else
                        {
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = Globals.CurrentTileset;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].X = Globals.CurSelX;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Y = Globals.CurSelY;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = (byte)Globals.Autotilemode;
                            tmpMap.Autotiles.InitAutotiles();
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
                    if (Globals.CurTileX == Constants.MapWidth - 1)
                    {
                        if (tmpMap.Right > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Right] != null)
                            {
                                Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles();
                            }
                        }
                    }
                    if (Globals.CurTileY == Constants.MapHeight - 1)
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
                        RemoveAttribute();
                    }
                    else if (Globals.CurrentLayer == Constants.LayerCount + 1)
                    {

                    }
                    else if (Globals.CurrentLayer == Constants.LayerCount + 2)
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
                    if (Globals.CurTileX == Constants.MapWidth - 1)
                    {
                        if (tmpMap.Right > -1)
                        {
                            if (Globals.GameMaps[tmpMap.Right] != null)
                            {
                                Globals.GameMaps[tmpMap.Right].Autotiles.InitAutotiles();
                            }
                        }
                    }
                    if (Globals.CurTileY == Constants.MapHeight - 1)
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
        private void picMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (Globals.EditingLight != null) { return; }
            Globals.MouseButton = -1;

        }
        private void picMap_DoubleClick(object sender, EventArgs e)
        {
            if (Globals.MouseX >= 32 && Globals.MouseX <= 1024 - 32)
            {
                if (Globals.MouseY >= 0 && Globals.MouseY <= 32)
                {
                    if (Globals.GameMaps[Globals.CurrentMap].Up == -1)
                    {
                        if (MessageBox.Show(@"Do you want to create a map here?", @"Create new map.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                PacketSender.SendMap(Globals.CurrentMap);
                            }
                            else
                            {
                                PacketSender.SendCreateMap(0, Globals.CurrentMap);
                            }
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            PacketSender.SendMap(Globals.CurrentMap);
                        }
                        EnterMap(Globals.GameMaps[Globals.CurrentMap].Up);
                    }
                }
                else if (Globals.MouseY >= 1024 - 32 && Globals.MouseY <= 1024)
                {
                    if (Globals.GameMaps[Globals.CurrentMap].Down == -1)
                    {
                        if (MessageBox.Show(@"Do you want to create a map here?", @"Create new map.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                PacketSender.SendMap(Globals.CurrentMap);
                            }
                            else
                            {
                                PacketSender.SendCreateMap(1, Globals.CurrentMap);
                            }
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            PacketSender.SendMap(Globals.CurrentMap);
                        }
                        EnterMap(Globals.GameMaps[Globals.CurrentMap].Down);
                    }
                }
            }
            if (Globals.MouseY < 32 || Globals.MouseY > 1024 - 32) return;
            if (Globals.MouseX >= 0 & Globals.MouseX <= 32)
            {
                if (Globals.GameMaps[Globals.CurrentMap].Left == -1)
                {
                    if (
                        MessageBox.Show(@"Do you want to create a map here?", @"Create new map.",
                            MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                    if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        PacketSender.SendMap(Globals.CurrentMap);
                    }
                    else
                    {
                        PacketSender.SendCreateMap(2, Globals.CurrentMap);
                    }
                }
                else
                {
                    //Should ask if the user wants to save changes
                    if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        PacketSender.SendMap(Globals.CurrentMap);
                    }
                    EnterMap(Globals.GameMaps[Globals.CurrentMap].Left);
                }
            }
            else if (Globals.MouseX >= 1024 - 32 && Globals.MouseX <= 1024)
            {
                if (Globals.GameMaps[Globals.CurrentMap].Right == -1)
                {
                    if (
                        MessageBox.Show(@"Do you want to create a map here?", @"Create new map.", MessageBoxButtons.YesNo) !=
                        DialogResult.Yes) return;
                    if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        PacketSender.SendMap(Globals.CurrentMap);
                    }
                    else
                    {
                        PacketSender.SendCreateMap(3, Globals.CurrentMap);
                    }
                }
                else
                {
                    //Should ask if the user wants to save changes
                    if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        PacketSender.SendMap(Globals.CurrentMap);
                    }
                    EnterMap(Globals.GameMaps[Globals.CurrentMap].Right);
                }
            }
        }
        private void picMap_Click(object sender, EventArgs e)
        {

        }
        private void hScrollMap_Scroll(object sender, ScrollEventArgs e)
        {
            picMap.Left = -hScrollMap.Value;

        }
        private void vScrollMap_Scroll(object sender, ScrollEventArgs e)
        {
            picMap.Top = -vScrollMap.Value;
        }

        //Map List Functions
        private void lblCloseMapList_Click(object sender, EventArgs e)
        {
            grpMapList.Hide();
            mapListToolStripMenuItem.Checked = grpMapList.Visible;
        }
        private void UpdateMapList()
        {
            lstGameMaps.Items.Clear();
            if (Globals.MapRefs == null) return;
            for (var i = 0; i < Globals.MapRefs.Length; i++)
            {
                if (Globals.MapRefs[i].Deleted == 0)
                {
                    lstGameMaps.Items.Add(i + ". " + Globals.MapRefs[i].MapName);
                }
            }
        }
        private void lstGameMaps_DoubleClick(object sender, EventArgs e)
        {
            if (lstGameMaps.SelectedIndex <= -1) return;
            var mapNum = Convert.ToInt32(((String)(lstGameMaps.Items[lstGameMaps.SelectedIndex])).Split('.')[0]);
            if (MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PacketSender.SendMap(Globals.CurrentMap);
            }
            EnterMap(mapNum);
        }

        //Map Properties Functions
        private void btnCloseProperties_Click(object sender, EventArgs e)
        {
            grpMapProperties.Hide();
        }
        private void txtMapName_TextChanged(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.CurrentMap].MyName = txtMapName.Text;
        }
        private void chkIndoors_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.CurrentMap].IsIndoors = chkIndoors.Checked;
        }

        //Light Editor
        private void txtLightOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Globals.EditingLight == null)
                {
                    return;
                }
                var offsetY = Convert.ToInt32(txtLightOffsetY.Text);
                Globals.EditingLight.OffsetY = offsetY;
                Globals.EditingLight.Graphic = null;
                Graphics.LightsChanged = true;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void txtLightOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Globals.EditingLight == null)
                {
                    return;
                }
                var offsetX = Convert.ToInt32(txtLightOffsetX.Text);
                Globals.EditingLight.OffsetX = offsetX;
                Globals.EditingLight.Graphic = null;
                Graphics.LightsChanged = true;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void scrlLightRange_Scroll(object sender, ScrollEventArgs e)
        {
            if (Globals.EditingLight == null) { return; }
            txtLightRange.Text = "" + Globals.EditingLight.Range;
            if (Globals.EditingLight.Graphic != null) { Globals.EditingLight.Graphic.Dispose(); }
            Globals.EditingLight.Graphic = null;
            Globals.EditingLight.Range = scrlLightRange.Value;
            Graphics.LightsChanged = true;
        }
        private void scrlLightIntensity_Scroll(object sender, ScrollEventArgs e)
        {
            if (Globals.EditingLight == null) { return; }
            Globals.EditingLight.Intensity = scrlLightIntensity.Value / 10000.0;
            if (Globals.EditingLight.Graphic != null) { Globals.EditingLight.Graphic.Dispose(); }
            Globals.EditingLight.Graphic = null;
            txtLightIntensity.Text = "" + Globals.EditingLight.Intensity;
            Graphics.LightsChanged = true;
        }
        private void txtLightIntensity_TextChanged(object sender, EventArgs e)
        {
            if (Globals.EditingLight == null) { return; }
            try
            {
                var intensity = Convert.ToDouble(txtLightIntensity.Text);
                if (intensity < 0)
                {
                    intensity = 0;
                }
                if (intensity > 1)
                {
                    intensity = 1;
                }
                Globals.EditingLight.Intensity = intensity;
                Globals.EditingLight.Graphic = null;
                txtLightIntensity.Text = "" + Globals.EditingLight.Intensity;
                scrlLightIntensity.Value = (int)(intensity * 10000.0);
                Graphics.LightsChanged = true;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void txtLightRange_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Globals.EditingLight == null)
                {
                    return;
                }
                var range = Convert.ToInt32(txtLightRange.Text);
                if (range < 2)
                {
                    range = 2;
                }
                if (range > 179)
                {
                    range = 179;
                }
                Globals.EditingLight.Range = range;
                Globals.EditingLight.Graphic = null;
                Graphics.LightsChanged = true;
                txtLightRange.Text = "" + range;
            }
            catch (Exception)
            {
                //ignore
            }
        }
        private void btnLightEditorClose_Click(object sender, EventArgs e)
        {
            grpLightEditor.Hide();
            Globals.EditingLight = null;
        }
        private void btnLightEditorRevert_Click(object sender, EventArgs e)
        {
            Globals.EditingLight.Intensity = Globals.BackupLight.Intensity;
            Globals.EditingLight.Range = Globals.BackupLight.Range;
            Globals.EditingLight.OffsetX = Globals.BackupLight.OffsetX;
            Globals.EditingLight.OffsetY = Globals.BackupLight.OffsetY;
            Globals.EditingLight.Graphic.Dispose();
            Globals.EditingLight.Graphic = null;
            Graphics.LightsChanged = false;
            grpLightEditor.Hide();
        }



        //Mapping Attribute Functions
        /// <summary>
        /// A method that hides all of the extra group boxes for tile data related to the map attributes.
        /// </summary>
        private void hideAttributeMenus()
        {
            grpItem.Visible = false;
        }
        private void rbItem_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpItem.Visible = true;
        }
        private void rbBlocked_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
        }
        private void scrlMapItem_Scroll(object sender, ScrollEventArgs e)
        {
            lblMapItem.Text = "Item: " + scrlMapItem.Value + " " + Globals.GameItems[scrlMapItem.Value - 1].Name;
        }
        private void scrlMaxItemVal_Scroll(object sender, ScrollEventArgs e)
        {
            lblMaxItemAmount.Text = "Quantity: x" + scrlMaxItemVal.Value;
        }
        private void PlaceAttribute()
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (rbBlocked.Checked == true)
            {
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].value = (int)Enums.MapAttributes.Blocked;
            }
            else if (rbItem.Checked == true)
            {
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].value = (int)Enums.MapAttributes.Item;
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data1 = scrlMapItem.Value - 1;
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data2 = scrlMaxItemVal.Value;
            }
        }
        private void RemoveAttribute()
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].value = 0;
            tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data1 = 0;
            tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data2 = 0;
            tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data3 = 0;
        }



        
    }
}
