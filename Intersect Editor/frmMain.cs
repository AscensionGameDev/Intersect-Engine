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


namespace Intersect_Editor
{
    public partial class frmMain : Form
    {
        //General Editting Variables
        bool tMouseDown;
        
        //Initialization & Setup Functions
        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            Database.InitDatabase();
            InitEditor();
            this.Show();
            EditorLoop.startLoop(this);
        }
        private void InitEditor()
        {
            EnterMap(0);
            GFX.InitSFML(this);
            UpdateScrollBars();
            InitLayerMenu();
            cmbAutotile.SelectedIndex = 0;
            Globals.inEditor = true;
            if (Globals.GameMaps[Globals.currentMap].up > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.currentMap].up); }
            if (Globals.GameMaps[Globals.currentMap].down > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.currentMap].down); }
            if (Globals.GameMaps[Globals.currentMap].left > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.currentMap].left); }
            if (Globals.GameMaps[Globals.currentMap].right > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.currentMap].right); }
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
            Globals.currentMap = mapNum;
            if (Globals.GameMaps[mapNum] != null)
            {
                this.Text = "Intersect Editor - Map# " + mapNum + " " + Globals.GameMaps[mapNum].myName + " Revision: " + Globals.GameMaps[mapNum].revision;
            }
            picMap.Visible = false;
            vScrollMap.Value = 0;
            hScrollMap.Value = 0;
            picMap.Left = 0;
            picMap.Top = 0;
            PacketSender.SendNeedMap(Globals.currentMap);
        }

        //MenuBar Functions
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.GameSocket.SendPacket(File.ReadAllBytes("c:\\tmp\\lastpacket.dat"));
        }
        private void HandleLayerClick(object sender, EventArgs e)
        {
            ToolStripItem tmpTsi = (ToolStripItem)sender;
            Globals.currentLayer = (int)tmpTsi.Tag;
            tmpTsi.Select();
            if (Globals.currentLayer == Constants.LAYER_COUNT)
            {
                lblCurLayer.Text = "Layer: Blocks";
            }
            else
            {
                lblCurLayer.Text = "Layer #" + (int)tmpTsi.Tag;
            }
        }
        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to create a new, unconnected map?", "New Map", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    PacketSender.SendMap(Globals.currentMap);
                }
                else
                {
                    PacketSender.SendCreateMap(-1, Globals.currentMap);
                }
            }
        }
        private void mapPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grpMapProperties.BringToFront();
            txtMapName.Text = Globals.GameMaps[Globals.currentMap].myName;
            chkIndoors.Checked = Globals.GameMaps[Globals.currentMap].isIndoors;
            grpMapProperties.Show();
        }
        private void nightTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.nightEnabled = !Globals.nightEnabled;
            nightTimeToolStripMenuItem.Checked = Globals.nightEnabled;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save this map?", "Save Map", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                PacketSender.SendMap(Globals.currentMap);
            }
        }
        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int oldCurSelX = Globals.curTileX;
            int oldCurSelY = Globals.curTileY;
            if (MessageBox.Show("Are you sure you want to fill this layer?", "Fill Layer", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        Globals.curTileX = x;
                        Globals.curTileY = y;
                        picMap_MouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, x * 32 + 32, y * 32 + 32, 0));
                        picMap_MouseUp(null, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
                    }
                }
                Globals.curTileX = oldCurSelX;
                Globals.curTileY = oldCurSelY;
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
                        Globals.curTileX = x;
                        Globals.curTileY = y;
                        picMap_MouseDown(null, new MouseEventArgs(MouseButtons.Right, 1, x * 32 + 32, y * 32 + 32, 0));
                        picMap_MouseUp(null, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
                    }
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

        //Tileset Area
        private void cmbTilesets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (File.Exists("Resources/Tilesets/" + Globals.tilesets[cmbTilesets.SelectedIndex]))
            {
                Globals.currentTileset = cmbTilesets.SelectedIndex;
                GFX.InitTileset(Globals.currentTileset, this);
            }
            else
            {
                cmbTilesets.SelectedIndex = Globals.currentTileset;
            }
        }
        private void picTileset_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > picTileset.Width || e.Y > picTileset.Height) { return; }
            tMouseDown = true;
            Globals.curSelX = (int)Math.Floor((double)e.X / 32);
            Globals.curSelY = (int)Math.Floor((double)e.Y / 32);
            Globals.curSelW = 0;
            Globals.curSelH = 0;
            if (Globals.curSelX < 0) { Globals.curSelX = 0; }
            if (Globals.curSelY < 0) { Globals.curSelY = 0; }
            switch (Globals.autotilemode)
            {
                case 1:
                case 5:
                    Globals.curSelW = 1;
                    Globals.curSelH = 2;
                    break;
                case 2:
                    Globals.curSelW = 0;
                    Globals.curSelH = 0;
                    break;
                case 3:
                    Globals.curSelW = 5;
                    Globals.curSelH = 2;
                    break;
                case 4:
                    Globals.curSelW = 1;
                    Globals.curSelH = 1;
                    break;
            }

        }
        private void picTileset_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > picTileset.Width || e.Y > picTileset.Height) { return; }
            if (tMouseDown && Globals.autotilemode == 0)
            {
                int tmpX = (int)Math.Floor((double)e.X / 32);
                int tmpY = (int)Math.Floor((double)e.Y / 32);
                Globals.curSelW = tmpX - Globals.curSelX;
                Globals.curSelH = tmpY - Globals.curSelY;
            }
        }
        private void picTileset_MouseUp(object sender, MouseEventArgs e)
        {

            int selW;
            int selH;
            int selX;
            int selY;
            selX = Globals.curSelX;
            selY = Globals.curSelY;
            selW = Globals.curSelW;
            selH = Globals.curSelH;
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
            Globals.curSelX = selX;
            Globals.curSelY = selY;
            Globals.curSelW = selW;
            Globals.curSelH = selH;
            tMouseDown = false;
        }
        private void cmbAutotile_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.autotilemode = cmbAutotile.SelectedIndex;
            switch (Globals.autotilemode)
            {
                case 1:
                case 5:
                    Globals.curSelW = 1;
                    Globals.curSelH = 2;
                    break;
                case 2:
                    Globals.curSelW = 0;
                    Globals.curSelH = 0;
                    break;
                case 3:
                    Globals.curSelW = 5;
                    Globals.curSelH = 2;
                    break;
                case 4:
                    Globals.curSelW = 1;
                    Globals.curSelH = 1;
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
            if (Globals.editingLight != null) { return; }
            Map tmpMap = Globals.GameMaps[Globals.currentMap];
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Globals.mouseButton = 0;
                if (Globals.currentLayer == Constants.LAYER_COUNT)
                {
                    tmpMap.blocked[Globals.curTileX, Globals.curTileY] = 1;
                }
                else if (Globals.currentLayer == Constants.LAYER_COUNT + 1)
                {
                    Light tmpLight;
                    if ((tmpLight = Globals.GameMaps[Globals.currentMap].FindLightAt(Globals.curTileX, Globals.curTileY)) == null)
                    {
                        tmpLight = new Light(Globals.curTileX, Globals.curTileY);
                        grpLightEditor.BringToFront();
                        grpLightEditor.Show();
                        Globals.GameMaps[Globals.currentMap].Lights.Add(tmpLight);
                        Globals.lightsChanged = true;
                        Globals.backupLight = new Light(tmpLight.tileX, tmpLight.tileY);
                        Globals.backupLight.offsetX = tmpLight.offsetX;
                        Globals.backupLight.offsetX = tmpLight.offsetX;
                        Globals.backupLight.intensity = tmpLight.intensity;
                        Globals.backupLight.range = tmpLight.range;
                        txtLightIntensity.Text = "" + tmpLight.intensity;
                        txtLightRange.Text = "" + tmpLight.range;
                        txtLightOffsetX.Text = "" + tmpLight.offsetX;
                        txtLightOffsetY.Text = "" + tmpLight.offsetY;
                        scrlLightIntensity.Value = (int)(tmpLight.intensity * 10000.0);
                        scrlLightRange.Value = tmpLight.range;
                        Globals.editingLight = tmpLight;
                    }
                    else
                    {
                        grpLightEditor.BringToFront();
                        grpLightEditor.Show();
                        Globals.backupLight = new Light(tmpLight.tileX, tmpLight.tileY);
                        Globals.backupLight.offsetX = tmpLight.offsetX;
                        Globals.backupLight.offsetX = tmpLight.offsetX;
                        Globals.backupLight.intensity = tmpLight.intensity;
                        Globals.backupLight.range = tmpLight.range;
                        txtLightIntensity.Text = "" + tmpLight.intensity;
                        txtLightRange.Text = "" + tmpLight.range;
                        txtLightOffsetX.Text = "" + tmpLight.offsetX;
                        txtLightOffsetY.Text = "" + tmpLight.offsetY;
                        scrlLightIntensity.Value = (int)(tmpLight.intensity * 10000.0);
                        scrlLightRange.Value = tmpLight.range;
                        Globals.editingLight = tmpLight;
                    }
                }
                else if (Globals.currentLayer == Constants.LAYER_COUNT + 2)
                {
                    Event tmpEvent;
                    frmEvent tmpEventEditor;
                    if ((tmpEvent = Globals.GameMaps[Globals.currentMap].FindEventAt(Globals.curTileX, Globals.curTileY)) == null)
                    {
                        tmpEvent = new Event(Globals.curTileX, Globals.curTileY);
                        Globals.GameMaps[Globals.currentMap].Events.Add(tmpEvent);
                        tmpEventEditor = new frmEvent();
                        tmpEventEditor.myEvent = tmpEvent;
                        tmpEventEditor.myMap = Globals.GameMaps[Globals.currentMap];
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
                    if (Globals.autotilemode == 0)
                    {
                        for (int x = 0; x <= Globals.curSelW; x++)
                        {
                            for (int y = 0; y <= Globals.curSelH; y++)
                            {
                                tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX + x, Globals.curTileY + y].tilesetIndex = Globals.currentTileset;
                                tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX + x, Globals.curTileY + y].x = Globals.curSelX + x;
                                tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX + x, Globals.curTileY + y].y = Globals.curSelY + y;
                                tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX + x, Globals.curTileY + y].Autotile = 0;
                                tmpMap.autotiles.initAutotiles();
                            }
                        }
                    }
                    else
                    {
                        tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].tilesetIndex = Globals.currentTileset;
                        tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].x = Globals.curSelX;
                        tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].y = Globals.curSelY;
                        tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].Autotile = (byte)Globals.autotilemode;
                        tmpMap.autotiles.initAutotiles();
                    }
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Globals.mouseButton = 1;
                if (Globals.currentLayer == Constants.LAYER_COUNT)
                {
                    tmpMap.blocked[Globals.curTileX, Globals.curTileY] = 0;
                }
                else if (Globals.currentLayer == Constants.LAYER_COUNT + 1)
                {
                    Light tmpLight;
                    if ((tmpLight = Globals.GameMaps[Globals.currentMap].FindLightAt(Globals.curTileX, Globals.curTileY)) != null)
                    {
                        Globals.GameMaps[Globals.currentMap].Lights.Remove(tmpLight);
                        Globals.lightsChanged = true;
                    }
                }
                else if (Globals.currentLayer == Constants.LAYER_COUNT + 2)
                {
                    Event tmpEvent;
                    if ((tmpEvent = Globals.GameMaps[Globals.currentMap].FindEventAt(Globals.curTileX, Globals.curTileY)) != null)
                    {
                        Globals.GameMaps[Globals.currentMap].Events.Remove(tmpEvent);
                        tmpEvent.deleted = 1;
                    }
                }
                else
                {
                    tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].tilesetIndex = -1;
                    tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].Autotile = 0;
                    tmpMap.autotiles.initAutotiles();
                }
            }
            if (Globals.curTileX == 0)
            {
                if (tmpMap.left > -1)
                {
                    if (Globals.GameMaps[tmpMap.left] != null)
                    {
                        Globals.GameMaps[tmpMap.left].autotiles.initAutotiles();
                    }
                }
            }
            if (Globals.curTileY == 0)
            {
                if (tmpMap.up > -1)
                {
                    if (Globals.GameMaps[tmpMap.up] != null)
                    {
                        Globals.GameMaps[tmpMap.up].autotiles.initAutotiles();
                    }
                }
            }
            if (Globals.curTileX == Constants.MAP_WIDTH - 1)
            {
                if (tmpMap.right > -1)
                {
                    if (Globals.GameMaps[tmpMap.right] != null)
                    {
                        Globals.GameMaps[tmpMap.right].autotiles.initAutotiles();
                    }
                }
            }
            if (Globals.curTileY == Constants.MAP_HEIGHT - 1)
            {
                if (tmpMap.down > -1)
                {
                    if (Globals.GameMaps[tmpMap.down] != null)
                    {
                        Globals.GameMaps[tmpMap.down].autotiles.initAutotiles();
                    }
                }
            }

        }
        private void picMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (Globals.editingLight != null) { return; }
            Globals.mouseX = e.X;
            Globals.mouseY = e.Y;
            if (e.X >= picMap.Width - 32 || e.Y >= picMap.Height - 32) { return; }
            if (e.X < 32 || e.Y < 32) { return; }
            Globals.curTileX = (int)Math.Floor((double)(e.X - 32) / 32);
            Globals.curTileY = (int)Math.Floor((double)(e.Y - 32) / 32);
            if (Globals.curTileX < 0) { Globals.curTileX = 0; }
            if (Globals.curTileY < 0) { Globals.curTileY = 0; }

            if (Globals.mouseButton > -1)
            {
                Map tmpMap = Globals.GameMaps[Globals.currentMap];
                if (Globals.mouseButton == 0)
                {
                    if (Globals.currentLayer == Constants.LAYER_COUNT)
                    {
                        tmpMap.blocked[Globals.curTileX, Globals.curTileY] = 1;
                    }
                    else if (Globals.currentLayer == Constants.LAYER_COUNT + 1)
                    {

                    }
                    else
                    {
                        if (Globals.autotilemode == 0)
                        {
                            for (int x = 0; x <= Globals.curSelW; x++)
                            {
                                for (int y = 0; y <= Globals.curSelH; y++)
                                {
                                    tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX + x, Globals.curTileY + y].tilesetIndex = Globals.currentTileset;
                                    tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX + x, Globals.curTileY + y].x = Globals.curSelX + x;
                                    tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX + x, Globals.curTileY + y].y = Globals.curSelY + y;
                                    tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX + x, Globals.curTileY + y].Autotile = 0;
                                    tmpMap.autotiles.initAutotiles();
                                }
                            }
                        }
                        else
                        {
                            tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].tilesetIndex = Globals.currentTileset;
                            tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].x = Globals.curSelX;
                            tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].y = Globals.curSelY;
                            tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].Autotile = (byte)Globals.autotilemode;
                            tmpMap.autotiles.initAutotiles();
                        }
                    }
                    if (Globals.curTileX == 0)
                    {
                        if (tmpMap.left > -1)
                        {
                            if (Globals.GameMaps[tmpMap.left] != null)
                            {
                                Globals.GameMaps[tmpMap.left].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (Globals.curTileY == 0)
                    {
                        if (tmpMap.up > -1)
                        {
                            if (Globals.GameMaps[tmpMap.up] != null)
                            {
                                Globals.GameMaps[tmpMap.up].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (Globals.curTileX == Constants.MAP_WIDTH - 1)
                    {
                        if (tmpMap.right > -1)
                        {
                            if (Globals.GameMaps[tmpMap.right] != null)
                            {
                                Globals.GameMaps[tmpMap.right].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (Globals.curTileY == Constants.MAP_HEIGHT - 1)
                    {
                        if (tmpMap.down > -1)
                        {
                            if (Globals.GameMaps[tmpMap.down] != null)
                            {
                                Globals.GameMaps[tmpMap.down].autotiles.initAutotiles();
                            }
                        }
                    }
                }
                else if (Globals.mouseButton == 1)
                {
                    if (Globals.currentLayer == Constants.LAYER_COUNT)
                    {
                        tmpMap.blocked[Globals.curTileX, Globals.curTileY] = 0;
                    }
                    else if (Globals.currentLayer == Constants.LAYER_COUNT + 1)
                    {

                    }
                    else if (Globals.currentLayer == Constants.LAYER_COUNT + 2)
                    {

                    }
                    else
                    {
                        tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].tilesetIndex = -1;
                        tmpMap.Layers[Globals.currentLayer].Tiles[Globals.curTileX, Globals.curTileY].Autotile = 0;
                        tmpMap.autotiles.initAutotiles();
                    }
                    if (Globals.curTileX == 0)
                    {
                        if (tmpMap.left > -1)
                        {
                            if (Globals.GameMaps[tmpMap.left] != null)
                            {
                                Globals.GameMaps[tmpMap.left].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (Globals.curTileY == 0)
                    {
                        if (tmpMap.up > -1)
                        {
                            if (Globals.GameMaps[tmpMap.up] != null)
                            {
                                Globals.GameMaps[tmpMap.up].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (Globals.curTileX == Constants.MAP_WIDTH - 1)
                    {
                        if (tmpMap.right > -1)
                        {
                            if (Globals.GameMaps[tmpMap.right] != null)
                            {
                                Globals.GameMaps[tmpMap.right].autotiles.initAutotiles();
                            }
                        }
                    }
                    if (Globals.curTileY == Constants.MAP_HEIGHT - 1)
                    {
                        if (tmpMap.down > -1)
                        {
                            if (Globals.GameMaps[tmpMap.down] != null)
                            {
                                Globals.GameMaps[tmpMap.down].autotiles.initAutotiles();
                            }
                        }
                    }
                }
            }
        }
        private void picMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (Globals.editingLight != null) { return; }
            Globals.mouseButton = -1;

        }
        private void picMap_DoubleClick(object sender, EventArgs e)
        {
            if (Globals.mouseX >= 32 && Globals.mouseX <= 1024 - 32)
            {
                if (Globals.mouseY >= 0 && Globals.mouseY <= 32)
                {
                    if (Globals.GameMaps[Globals.currentMap].up == -1)
                    {
                        if (MessageBox.Show("Do you want to create a map here?", "Create new map.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                PacketSender.SendMap(Globals.currentMap);
                            }
                            else
                            {
                                PacketSender.SendCreateMap(0, Globals.currentMap);
                            }
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            PacketSender.SendMap(Globals.currentMap);
                        }
                        EnterMap(Globals.GameMaps[Globals.currentMap].up);
                    }
                }
                else if (Globals.mouseY >= 1024 - 32 && Globals.mouseY <= 1024)
                {
                    if (Globals.GameMaps[Globals.currentMap].down == -1)
                    {
                        if (MessageBox.Show("Do you want to create a map here?", "Create new map.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                PacketSender.SendMap(Globals.currentMap);
                            }
                            else
                            {
                                PacketSender.SendCreateMap(1, Globals.currentMap);
                            }
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            PacketSender.SendMap(Globals.currentMap);
                        }
                        EnterMap(Globals.GameMaps[Globals.currentMap].down);
                    }
                }
            }
            if (Globals.mouseY >= 32 && Globals.mouseY <= 1024 - 32)
            {
                if (Globals.mouseX >= 0 & Globals.mouseX <= 32)
                {
                    if (Globals.GameMaps[Globals.currentMap].left == -1)
                    {
                        if (MessageBox.Show("Do you want to create a map here?", "Create new map.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                PacketSender.SendMap(Globals.currentMap);
                            }
                            else
                            {
                                PacketSender.SendCreateMap(2, Globals.currentMap);
                            }
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            PacketSender.SendMap(Globals.currentMap);
                        }
                        EnterMap(Globals.GameMaps[Globals.currentMap].left);
                    }
                }
                else if (Globals.mouseX >= 1024 - 32 && Globals.mouseX <= 1024)
                {
                    if (Globals.GameMaps[Globals.currentMap].right == -1)
                    {
                        if (MessageBox.Show("Do you want to create a map here?", "Create new map.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                PacketSender.SendMap(Globals.currentMap);
                            }
                            else
                            {
                                PacketSender.SendCreateMap(3, Globals.currentMap);
                            }
                        }
                    }
                    else
                    {
                        //Should ask if the user wants to save changes
                        if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            PacketSender.SendMap(Globals.currentMap);
                        }
                        EnterMap(Globals.GameMaps[Globals.currentMap].right);
                    }
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
            if (Globals.MapRefs != null)
            {
                for (int i = 0; i < Globals.MapRefs.Length; i++)
                {
                    if (Globals.MapRefs[i].deleted == 0)
                    {
                        lstGameMaps.Items.Add(i + ". " + Globals.MapRefs[i].MapName);
                    }
                }
            }
        }
        private void lstGameMaps_DoubleClick(object sender, EventArgs e)
        {
            if (lstGameMaps.SelectedIndex > -1)
            {
                int mapNum = Convert.ToInt32(((String)(lstGameMaps.Items[lstGameMaps.SelectedIndex])).Split('.')[0]);
                if (MessageBox.Show("Do you want to save your current map?", "Save current map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    PacketSender.SendMap(Globals.currentMap);
                }
                EnterMap(mapNum);
            }
        }

        //Map Properties Functions
        private void btnCloseProperties_Click(object sender, EventArgs e)
        {
            grpMapProperties.Hide();
        }
        private void txtMapName_TextChanged(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.currentMap].myName = txtMapName.Text;
        }
        private void chkIndoors_CheckedChanged(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.currentMap].isIndoors = chkIndoors.Checked;
        }

        //Light Editor
        private void txtLightOffsetY_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Globals.editingLight == null) { return; }
                int offsetY = Convert.ToInt32(txtLightOffsetY.Text);
                Globals.editingLight.offsetY = offsetY;
                Globals.editingLight.graphic = null;
                Globals.lightsChanged = true;
            }
            catch { }
        }
        private void txtLightOffsetX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Globals.editingLight == null) { return; }
                int offsetX = Convert.ToInt32(txtLightOffsetX.Text);
                Globals.editingLight.offsetX = offsetX;
                Globals.editingLight.graphic = null;
                Globals.lightsChanged = true;
            }
            catch { }
        }
        private void scrlLightRange_Scroll(object sender, ScrollEventArgs e)
        {
            if (Globals.editingLight == null) { return; }
            txtLightRange.Text = "" + Globals.editingLight.range;
            if (Globals.editingLight.graphic != null) { Globals.editingLight.graphic.Dispose(); }
            Globals.editingLight.graphic = null;
            Globals.editingLight.range = scrlLightRange.Value;
            Globals.lightsChanged = true;
        }
        private void scrlLightIntensity_Scroll(object sender, ScrollEventArgs e)
        {
            if (Globals.editingLight == null) { return; }
            Globals.editingLight.intensity = (double)scrlLightIntensity.Value / 10000.0;
            if (Globals.editingLight.graphic != null) { Globals.editingLight.graphic.Dispose(); }
            Globals.editingLight.graphic = null;
            txtLightIntensity.Text = "" + Globals.editingLight.intensity;
            Globals.lightsChanged = true;
        }
        private void txtLightIntensity_TextChanged(object sender, EventArgs e)
        {
            if (Globals.editingLight == null) { return; }
            try
            {
                double intensity = Convert.ToDouble(txtLightIntensity.Text);
                if (intensity < 0) { intensity = 0; }
                if (intensity > 1) { intensity = 1; }
                Globals.editingLight.intensity = intensity;
                Globals.editingLight.graphic = null;
                txtLightIntensity.Text = "" + Globals.editingLight.intensity;
                scrlLightIntensity.Value = (int)(intensity * 10000.0);
                Globals.lightsChanged = true;
            }
            catch { }
        }
        private void txtLightRange_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Globals.editingLight == null) { return; }
                int range = Convert.ToInt32(txtLightRange.Text);
                if (range < 2) { range = 2; }
                if (range > 179) { range = 179; }
                Globals.editingLight.range = range;
                Globals.editingLight.graphic = null;
                Globals.lightsChanged = true;
                txtLightRange.Text = "" + range;
            }
            catch { }
        }
        private void btnLightEditorClose_Click(object sender, EventArgs e)
        {
            grpLightEditor.Hide();
            Globals.editingLight = null;
        }
        private void btnLightEditorRevert_Click(object sender, EventArgs e)
        {
            Globals.editingLight.intensity = Globals.backupLight.intensity;
            Globals.editingLight.range = Globals.backupLight.range;
            Globals.editingLight.offsetX = Globals.backupLight.offsetX;
            Globals.editingLight.offsetY = Globals.backupLight.offsetY;
            Globals.editingLight.graphic.Dispose();
            Globals.editingLight.graphic = null;
            Globals.lightsChanged = false;
            grpLightEditor.Hide();
        }

        private void itemEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendItemEditor();
        }
    }
}
