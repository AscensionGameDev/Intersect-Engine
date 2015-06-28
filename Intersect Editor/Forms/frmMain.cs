/*
    Intersect Game Engine (Server)
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
            // Set form object properties based on constants to prevent user inputting invalid options.
            InitFormObjects();

            // Initilise the editor.
            InitEditor();
            Show();
            EditorLoop.StartLoop(this);
        }

        private void InitFormObjects()
        {
            scrlMap.Maximum = Globals.GameMaps.Length;
            scrlX.Maximum = Constants.MapWidth;
            scrlY.Maximum = Constants.MapHeight;
            scrlMapItem.Maximum = Constants.MaxItems;
        }

        private void InitEditor()
        {
            EnterMap(0);
            Graphics.InitSfml(this);
            Sounds.Init();
            UpdateSoundLists();
            UpdateFogLists();
            UpdateImageLists();
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
        private void UpdateSoundLists()
        {
            cmbMapMusic.Items.Clear();
            cmbMapMusic.Items.Add("None");
            for (int i = 0; i < Sounds.MusicFiles.Count; i++)
            {
                cmbMapMusic.Items.Add(Sounds.MusicFiles[i]);
            }
            cmbMapSound.Items.Clear();
            cmbMapSound.Items.Add("None");
            for (int i = 0; i < Sounds.SoundFiles.Count; i++)
            {
                cmbMapSound.Items.Add(Sounds.SoundFiles[i]);
            }
            cmbMapAttributeSound.Items.Clear();
            cmbMapAttributeSound.Items.Add("None");
            for (int i = 0; i < Sounds.SoundFiles.Count; i++)
            {
                cmbMapAttributeSound.Items.Add(Sounds.SoundFiles[i]);
            }
        }
        private void UpdateFogLists()
        {
            cmbFogs.Items.Clear();
            cmbFogs.Items.Add("None");
            for (int i = 0; i < Graphics.FogFileNames.Count; i++)
            {
                cmbFogs.Items.Add(Graphics.FogFileNames[i]);
            }
        }
        private void UpdateImageLists()
        {
            cmbPanorama.Items.Clear();
            cmbPanorama.Items.Add("None");
            for (int i = 0; i < Graphics.ImageFileNames.Count; i++)
            {
                cmbPanorama.Items.Add(Graphics.ImageFileNames[i]);
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

        private void HideMenus()
        {
            grpLayer.Hide();
            grpAttributes.Hide();
            grpLightEditor.Hide();
            grpMapList.Hide();
        }

        private void HandleLayerClick(object sender, EventArgs e)
        {
            var tmpTsi = (ToolStripItem)sender;
            Globals.CurrentLayer = (int)tmpTsi.Tag;
            tmpTsi.Select();
            HideMenus();
            if (Globals.CurrentLayer == Constants.LayerCount)
            {
                lblCurLayer.Text = @"Layer: Attributes";
                grpAttributes.Show();
            }
            else
            {
                lblCurLayer.Text = @"Layer #" + ((int)tmpTsi.Tag + 1);
                grpLayer.Show();
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
            Globals.ViewingMapProperties = true;
            grpMapProperties.BringToFront();
            txtMapName.Text = Globals.GameMaps[Globals.CurrentMap].MyName;
            chkIndoors.Checked = Globals.GameMaps[Globals.CurrentMap].IsIndoors;

            //Music
            if (Globals.GameMaps[Globals.CurrentMap].Music.Length > 0)
            {
                cmbMapMusic.SelectedIndex = cmbMapMusic.Items.IndexOf(Globals.GameMaps[Globals.CurrentMap].Music);
            }
            if (cmbMapMusic.SelectedIndex == -1) { cmbMapMusic.SelectedIndex = 0; }

            //Sound
            if (Globals.GameMaps[Globals.CurrentMap].Sound.Length > 0)
            {
                cmbMapSound.SelectedIndex = cmbMapSound.Items.IndexOf(Globals.GameMaps[Globals.CurrentMap].Sound);
            }
            if (cmbMapSound.SelectedIndex == -1) { cmbMapSound.SelectedIndex = 0; }

            //Panorama
            if (Globals.GameMaps[Globals.CurrentMap].Panorama.Length > 0)
            {
                cmbPanorama.SelectedIndex = cmbPanorama.Items.IndexOf(Globals.GameMaps[Globals.CurrentMap].Panorama);
            }
            if (cmbPanorama.SelectedIndex == -1) { cmbPanorama.SelectedIndex = 0; }


            //Fog Values
            if (Globals.GameMaps[Globals.CurrentMap].Fog.Length > 0)
            {
                cmbFogs.SelectedIndex = cmbPanorama.Items.IndexOf(Globals.GameMaps[Globals.CurrentMap].Fog);
            }
            if (cmbFogs.SelectedIndex == -1) { cmbFogs.SelectedIndex = 0; }

            scrlFogHorizontal.Value = Globals.GameMaps[Globals.CurrentMap].FogXSpeed;
            lblFogHorizontalSpeed.Text = "Fog Horitonzal Speed: " + scrlFogHorizontal.Value;
            scrlFogVertical.Value = Globals.GameMaps[Globals.CurrentMap].FogYSpeed;
            lblFogVerticalSpeed.Text = "Fog Vertical Speed: " + scrlFogVertical.Value;
            scrlFogIntensity.Value = Globals.GameMaps[Globals.CurrentMap].FogTransaprency;
            lblFogIntensity.Text = "Fog Intensity: " + scrlFogIntensity.Value;

            //Map Hue
            scrlMapRed.Value = Globals.GameMaps[Globals.CurrentMap].RHue;
            lblMapRed.Text = "Red: " + scrlMapRed.Value;
            scrlMapGreen.Value = Globals.GameMaps[Globals.CurrentMap].GHue;
            lblMapGreen.Text = "Green: " + scrlMapGreen.Value;
            scrlMapBlue.Value = Globals.GameMaps[Globals.CurrentMap].BHue;
            lblMapBlue.Text = "Blue: " + scrlMapBlue.Value;
            scrlMapAlpha.Value = Globals.GameMaps[Globals.CurrentMap].AHue;
            lblMapAlpha.Text = "Intensity: " + scrlMapAlpha.Value;

            //Brightness
            scrlBrightness.Value = Globals.GameMaps[Globals.CurrentMap].Brightness;
            lblBrightness.Text = "Brightness: " + scrlBrightness.Value;
            

            // Update the list incase npcs have been modified since form load.
            cmbNpc.Items.Clear();
            cmbNpc.Items.Add("None");
            for (int i = 0; i < Constants.MaxNpcs; i++)
            {
                cmbNpc.Items.Add(i + ") " + Globals.GameNpcs[i].Name);
            }

            // Add the map NPCs
            lstMapNpcs.Items.Clear();
            for (int i = 0; i < Globals.GameMaps[Globals.CurrentMap].Spawns.Count; i++)
            {
                lstMapNpcs.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameNpcs[Globals.GameMaps[Globals.CurrentMap].Spawns[i].NpcNum].Name);
            }

            // Don't select if there are no NPCs, to avoid crashes.
            cmbNpc.SelectedIndex = 0;
            cmbDir.SelectedIndex = 0;
            rbRandom.Checked = true;
            if (lstMapNpcs.Items.Count > 0)
            {
                lstMapNpcs.SelectedIndex = 0;
                cmbDir.SelectedIndex = Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Dir + 1;
                cmbNpc.SelectedIndex = Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].NpcNum + 1;
                if (Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].X >= 0)
                {
                    rbDeclared.Checked = true;
                }
            }

            grpMapProperties.Show();
        }
        private void hideDarknessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics.HideDarkness = !Graphics.HideDarkness;
            hideDarknessToolStripMenuItem.Checked = Graphics.HideDarkness;
        }
        private void hideFogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics.HideFog = !Graphics.HideFog;
            hideFogToolStripMenuItem.Checked = Graphics.HideFog;
        }
        private void hideOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics.HideOverlay = !Graphics.HideOverlay;
            hideOverlayToolStripMenuItem.Checked = Graphics.HideOverlay;
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
                                HideMenus();
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
                                HideMenus();
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

                            // Check for adding NPC Spawns in map properties
                            if (grpMapProperties.Visible == true && rbDeclared.Checked == true && lstMapNpcs.Items.Count > 0)
                            {
                                Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].X = Globals.CurTileX;
                                Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Y = Globals.CurTileY;
                            }
                            else // Add tiles normally.
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
                            if (grpMapProperties.Visible == false)
                            {
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = -1;
                                tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
                                tmpMap.Autotiles.InitAutotiles();
                            }
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

                        // Check for adding NPC Spawns in map properties
                        if (grpMapProperties.Visible == true && rbDeclared.Checked == true && lstMapNpcs.Items.Count > 0)
                        {
                            Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].X = Globals.CurTileX;
                            Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Y = Globals.CurTileY;
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
                        if (grpMapProperties.Visible == false)
                        {
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].TilesetIndex = -1;
                            tmpMap.Layers[Globals.CurrentLayer].Tiles[Globals.CurTileX, Globals.CurTileY].Autotile = 0;
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
        private void lblCloseMapProperties_Click(object sender, EventArgs e)
        {
            grpMapProperties.Visible = false;
            Globals.ViewingMapProperties = false;
        }
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
        private void cmbPanorama_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.CurrentMap].Panorama = cmbPanorama.Text;
        }
        private void cmbFogs_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.CurrentMap].Fog = cmbFogs.Text;
        }
        private void scrlFogHorizontal_Scroll(object sender, ScrollEventArgs e)
        {
            lblFogHorizontalSpeed.Text = "Fog Horizontal Speed: " + scrlFogHorizontal.Value;
            Globals.GameMaps[Globals.CurrentMap].FogXSpeed = scrlFogHorizontal.Value;
        }
        private void scrlFogVertical_Scroll(object sender, ScrollEventArgs e)
        {
            lblFogVerticalSpeed.Text = "Fog Vertical Speed: " + scrlFogVertical.Value;
            Globals.GameMaps[Globals.CurrentMap].FogYSpeed = scrlFogVertical.Value;
        }
        private void scrlFogIntensity_Scroll(object sender, ScrollEventArgs e)
        {
            lblFogIntensity.Text = "Fog Intensity: " + scrlFogIntensity.Value;
            Globals.GameMaps[Globals.CurrentMap].FogTransaprency = scrlFogIntensity.Value;
        }
        private void scrlMapRed_Scroll(object sender, ScrollEventArgs e)
        {
            lblMapRed.Text = "Red: " + scrlMapRed.Value;
            Globals.GameMaps[Globals.CurrentMap].RHue = scrlMapRed.Value;
        }
        private void scrlMapGreen_Scroll(object sender, ScrollEventArgs e)
        {
            lblMapGreen.Text = "Green: " + scrlMapGreen.Value;
            Globals.GameMaps[Globals.CurrentMap].GHue = scrlMapGreen.Value;
        }
        private void scrlMapBlue_Scroll(object sender, ScrollEventArgs e)
        {
            lblMapBlue.Text = "Blue: " + scrlMapBlue.Value;
            Globals.GameMaps[Globals.CurrentMap].BHue = scrlMapBlue.Value;
        }
        private void scrlMapAlpha_Scroll(object sender, ScrollEventArgs e)
        {
            lblMapAlpha.Text = "Intensity: " + scrlMapAlpha.Value;
            Globals.GameMaps[Globals.CurrentMap].AHue = scrlMapAlpha.Value;
        }
        private void scrlBrightness_Scroll(object sender, ScrollEventArgs e)
        {
            lblBrightness.Text = "Brightness: " + scrlBrightness.Value;
            Globals.GameMaps[Globals.CurrentMap].Brightness = scrlBrightness.Value;
        }
        private void cmbMapMusic_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.CurrentMap].Music = cmbMapMusic.Text;
        }
        private void cmbMapSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.CurrentMap].Sound = cmbMapSound.Text;
        }
        //Map NPC List
        private void btnAddMapNpc_Click(object sender, EventArgs e)
        {
            var n = new NpcSpawn();

            //Don't add nothing
            if (cmbNpc.SelectedIndex > 0)
            {
                n.NpcNum = cmbNpc.SelectedIndex - 1;
                n.X = -1;
                n.Y = -1;
                n.Dir = -1;

                Globals.GameMaps[Globals.CurrentMap].Spawns.Add(n);

                lstMapNpcs.Items.Add(Convert.ToString(lstMapNpcs.Items.Count + 1) + ") " + Globals.GameNpcs[cmbNpc.SelectedIndex - 1].Name);
                lstMapNpcs.SelectedIndex = lstMapNpcs.Items.Count - 1;
            }
        }
        private void btnRemoveMapNpc_Click(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.CurrentMap].Spawns.RemoveAt(lstMapNpcs.SelectedIndex);
            lstMapNpcs.Items.RemoveAt(lstMapNpcs.SelectedIndex);

            // Refresh List
            lstMapNpcs.Items.Clear();
            for (int i = 0; i < Globals.GameMaps[Globals.CurrentMap].Spawns.Count; i++)
            {
                lstMapNpcs.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameNpcs[Globals.GameMaps[Globals.CurrentMap].Spawns[i].NpcNum].Name);
            }

            if (lstMapNpcs.Items.Count > 0)
            {
                lstMapNpcs.SelectedIndex = 0;
            }
        }
        private void lstMapNpcs_Click(object sender, EventArgs e)
        {
            if (lstMapNpcs.Items.Count > 0)
            {
                cmbNpc.SelectedIndex = Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].NpcNum + 1;
                cmbDir.SelectedIndex = Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Dir + 1;
                if (Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].X >= 0)
                {
                    rbDeclared.Checked = true;
                }
                else
                {
                    rbRandom.Checked = true;
                }
            }
        }
        private void rbRandom_Click(object sender, EventArgs e)
        {
            Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].X = -1;
            Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Y = -1;
            Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Dir = -1;
        }
        private void cmbDir_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMapNpcs.SelectedIndex >= 0)
            {
                Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].Dir = cmbDir.SelectedIndex - 1;
            }
        }
        private void cmbNpc_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = 0;

            if (lstMapNpcs.SelectedIndex >= 0)
            {
                Globals.GameMaps[Globals.CurrentMap].Spawns[lstMapNpcs.SelectedIndex].NpcNum = cmbNpc.SelectedIndex - 1;

                // Refresh List
                n = lstMapNpcs.SelectedIndex;
                lstMapNpcs.Items.Clear();
                for (int i = 0; i < Globals.GameMaps[Globals.CurrentMap].Spawns.Count; i++)
                {
                    lstMapNpcs.Items.Add(Convert.ToString(i + 1) + ") " + Globals.GameNpcs[Globals.GameMaps[Globals.CurrentMap].Spawns[i].NpcNum].Name);
                }
                lstMapNpcs.SelectedIndex = n;
            }
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
            grpZDimension.Visible = false;
            grpWarp.Visible = false;
            grpSound.Visible = false;
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
        private void rbNPCAvoid_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
        }
        private void rbZDimension_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpZDimension.Visible = true;
        }
        private void rbWarp_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpWarp.Visible = true;
        }
        private void rbSound_CheckedChanged(object sender, EventArgs e)
        {
            hideAttributeMenus();
            grpSound.Visible = true;
        }

        // Used for returning an integer value depending on which radio button is selected on the forms. This is merely used to make PlaceAtrribute less messy.
        private int GetEditorDimensionGateway()
        {
            if (rbGateway1.Checked == true)
            {
                return 1;
            }
            else if (rbGateway2.Checked == true)
            {
                return 2;
            }
            return 0;
        }
        private int GetEditorDimensionBlock()
        {
            if (rbBlock1.Checked == true)
            {
                return 1;
            }
            else if (rbBlock2.Checked == true)
            {
                return 2;
            }
            return 0;
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
            else if (rbZDimension.Checked == true)
            {
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].value = (int)Enums.MapAttributes.ZDimension;
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data1 = GetEditorDimensionGateway();
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data2 = GetEditorDimensionBlock();
            }
            else if (rbNPCAvoid.Checked == true)
            {
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].value = (int)Enums.MapAttributes.NPCAvoid;
            }
            else if (rbWarp.Checked == true)
            {
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].value = (int)Enums.MapAttributes.Warp;
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data1 = scrlMap.Value;
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data2 = scrlX.Value;
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data3 = scrlY.Value;
            }
            else if (rbSound.Checked == true)
            {
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].value = (int)Enums.MapAttributes.Sound;
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data1 = scrlSoundDistance.Value;
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data2 = 0;
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data3 = 0;
                tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data4 = cmbMapAttributeSound.Text;
            }
        }
        private void RemoveAttribute()
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].value = 0;
            tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data1 = 0;
            tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data2 = 0;
            tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data3 = 0;
            tmpMap.Attributes[Globals.CurTileX, Globals.CurTileY].data4 = "";
        }
        //Item Attribute
        private void scrlMapItem_ValueChanged(object sender, EventArgs e)
        {
            lblMapItem.Text = "Item: " + scrlMapItem.Value + " " + Globals.GameItems[scrlMapItem.Value - 1].Name;
        }
        private void scrlMaxItemVal_ValueChanged(object sender, EventArgs e)
        {
            lblMaxItemAmount.Text = "Quantity: x" + scrlMaxItemVal.Value;
        }
        //Warp Attribute
        private void scrlMap_ValueChanged(object sender, EventArgs e)
        {
            lblMap.Text = "Map: " + scrlMap.Value;
        }
        private void scrlX_ValueChanged(object sender, EventArgs e)
        {
            lblX.Text = "X: " + scrlX.Value;
        }
        private void scrlY_ValueChanged(object sender, EventArgs e)
        {
            lblY.Text = "Y: " + scrlY.Value;
        }
        //Sound Attribute
        private void scrlSoundDistance_Scroll(object sender, ScrollEventArgs e)
        {
            lblSoundDistance.Text = "Distance: " + scrlSoundDistance.Value + " Tile(s)";
        }






        
    }
}
