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
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect_Editor.Forms
{
    public partial class FrmMain : Form
    {
        //General Editting Variables
        bool _tMouseDown;

        //Cross Thread Delegates
        public delegate void TryOpenEditor(int editorIndex);
        public TryOpenEditor EditorDelegate;

        //Editor References
        private frmAnimation _animationEditor;
        private FrmItem _itemEditor;
        private frmNpc _npcEditor;
        private frmResource _resourceEditor;
        private frmSpell _spellEditor;
        private frmClass _classEditor;

        //Initialization & Setup Functions
        public FrmMain()
        {
            InitializeComponent();
            Globals.MapListWindow = new frmMapList();
            Globals.MapListWindow.Show(dockLeft, DockState.DockRight);
            Globals.MapLayersWindow = new frmMapLayers();
            Globals.MapLayersWindow.Init();
            Globals.MapLayersWindow.Show(dockLeft,DockState.DockLeft);
            
            Globals.MapEditorWindow = new frmMapEditor();
            Globals.MapEditorWindow.Show(dockLeft, DockState.Document);
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Set form object properties based on constants to prevent user inputting invalid options.
            InitFormObjects();

            //Init Delegates
            EditorDelegate = new TryOpenEditor(TryOpenEditorMethod);

            // Initilise the editor.
            InitEditor();

            //Init Map Properties
            InitMapProperties();
            Show();
        }
        private void InitFormObjects()
        {
            Globals.MapLayersWindow.scrlMap.Maximum = Globals.GameMaps.Length;
            Globals.MapLayersWindow.scrlX.Maximum = Constants.MapWidth;
            Globals.MapLayersWindow.scrlY.Maximum = Constants.MapHeight;
            Globals.MapLayersWindow.scrlMapItem.Maximum = Constants.MaxItems;
        }
        private void InitMapProperties()
        {
            DockPane unhiddenPane = dockLeft.Panes[0];
            Globals.MapPropertiesWindow = new frmMapProperties();
            Globals.MapPropertiesWindow.Show(unhiddenPane, DockAlignment.Bottom, .4);
            Globals.MapPropertiesWindow.Init(Globals.GameMaps[Globals.CurrentMap]);
        }
        private void InitEditor()
        {
            EnterMap(0);
            Graphics.InitSfml(this);
            Sounds.Init();
            Globals.InEditor = true;
            if (Globals.GameMaps[Globals.CurrentMap].Up > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.CurrentMap].Up); }
            if (Globals.GameMaps[Globals.CurrentMap].Down > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.CurrentMap].Down); }
            if (Globals.GameMaps[Globals.CurrentMap].Left > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.CurrentMap].Left); }
            if (Globals.GameMaps[Globals.CurrentMap].Right > -1) { PacketSender.SendNeedMap(Globals.GameMaps[Globals.CurrentMap].Right); }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }
        public void EnterMap(int mapNum)
        {
            Globals.CurrentMap = mapNum;
            if (Globals.MapPropertiesWindow != null) { Globals.MapPropertiesWindow.Init(Globals.GameMaps[Globals.CurrentMap]); }
            if (Globals.GameMaps[mapNum] != null)
            {
                Text = @"Intersect Editor - Map# " + mapNum + @" " + Globals.GameMaps[mapNum].MyName + @" Revision: " + Globals.GameMaps[mapNum].Revision;
            }
            Globals.MapEditorWindow.picMap.Visible = false;
            PacketSender.SendNeedMap(Globals.CurrentMap);
        }

        //MenuBar Functions
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                        Globals.MapEditorWindow.picMap_MouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, x * 32 + 32, y * 32 + 32, 0));
                        Globals.MapEditorWindow.picMap_MouseUp(null, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
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
                    Globals.MapEditorWindow.picMap_MouseDown(null, new MouseEventArgs(MouseButtons.Right, 1, x * 32 + 32, y * 32 + 32, 0));
                    Globals.MapEditorWindow.picMap_MouseUp(null, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
                }
            }
        }

        private void mapListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //grpMapList.Visible = !grpMapList.Visible;
            //mapListToolStripMenuItem.Checked = grpMapList.Visible;
            //if (grpMapList.Visible)
            //{
                //UpdateMapList();
            //}
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
        private void resourceEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendResourceEditor();
        }
        private void classEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendClassEditor();
        }

        /*Map List Functions
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
<<<<<<< HEAD
        }*/

        //Cross Threading Delegate Methods
        private void TryOpenEditorMethod(int editorIndex)
        {
            if (Globals.CurrentEditor == -1)
            {
                switch (editorIndex)
                {
                    case (int)Enums.EditorTypes.Animation:
                        if (_animationEditor == null || _animationEditor.Visible == false)
                        {
                            _animationEditor = new frmAnimation();
                            _animationEditor.InitEditor();
                            _animationEditor.Show();
                        }
                        break;
                    case (int)Enums.EditorTypes.Item:
                        if (_itemEditor == null || _itemEditor.Visible == false)
                        {
                            _itemEditor = new FrmItem();
                            _itemEditor.InitEditor();
                            _itemEditor.Show();
                        }
                        break;
                    case (int)Enums.EditorTypes.Npc:
                        if (_npcEditor == null || _npcEditor.Visible == false)
                        {
                            _npcEditor = new frmNpc();
                            _npcEditor.InitEditor();
                            _npcEditor.Show();
                        }
                        break;
                    case (int)Enums.EditorTypes.Resource:
                        if (_resourceEditor == null || _resourceEditor.Visible == false)
                        {
                            _resourceEditor = new frmResource();
                            _resourceEditor.InitEditor();
                            _resourceEditor.Show();
                        }
                        break;
                    case (int)Enums.EditorTypes.Spell:
                        if (_spellEditor == null || _spellEditor.Visible == false)
                        {
                            _spellEditor = new frmSpell();
                            _spellEditor.InitEditor();
                            _spellEditor.Show();
                        }
                        break;
                    case (int)Enums.EditorTypes.Class:
                        if (_classEditor == null || _classEditor.Visible == false)
                        {
                            _classEditor = new frmClass();
                            _classEditor.InitEditor();
                            _classEditor.Show();
                        }
                        break;
                    default:
                        return;
                }
                Globals.CurrentEditor = editorIndex;
            }

        }
    }
}
