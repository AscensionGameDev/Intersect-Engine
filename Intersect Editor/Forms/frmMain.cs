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
using System.Collections.Generic;

namespace Intersect_Editor.Forms
{
    public partial class frmMain : Form
    {
        //General Editting Variables
        bool _tMouseDown;

        //Cross Thread Delegates
        public delegate void TryOpenEditor(int editorIndex);
        public TryOpenEditor EditorDelegate;
        public delegate void HandleDisconnect();
        public HandleDisconnect DisconnectDelegate;

        //Editor References
        private frmAnimation _animationEditor;
        private FrmItem _itemEditor;
        private frmNpc _npcEditor;
        private frmResource _resourceEditor;
        private frmSpell _spellEditor;
        private frmClass _classEditor;

        //Initialization & Setup Functions
        public frmMain()
        {
            InitializeComponent();
            Globals.MapListWindow = new frmMapList();
            Globals.MapListWindow.Show(dockLeft, DockState.DockRight);
            Globals.MapLayersWindow = new frmMapLayers();
            Globals.MapLayersWindow.Init();
            Globals.MapLayersWindow.Show(dockLeft, DockState.DockLeft);

            Globals.MapEditorWindow = new frmMapEditor();
            Globals.MapEditorWindow.Show(dockLeft, DockState.Document);
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Set form object properties based on constants to prevent user inputting invalid options.
            InitFormObjects();

            //Init Delegates
            EditorDelegate = new TryOpenEditor(TryOpenEditorMethod);
            DisconnectDelegate = new HandleDisconnect(HandleServerDisconnect);

            // Initilise the editor.
            InitEditor();

            //Init Map Properties
            InitMapProperties();
            Show();
        }
        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.Z))
            {
                toolStripBtnUndo_Click(null, null);
            }
            else if (e.KeyData == (Keys.Control | Keys.Y))
            {
                toolStripBtnRedo_Click(null, null);
            }
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
            Globals.MapPropertiesWindow.Init(Globals.CurrentMap);
        }
        private void InitEditor()
        {
            Graphics.InitSfml(this);
            Sounds.Init();
            Globals.InEditor = true;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }
        public void EnterMap(int mapNum)
        {
            Globals.CurrentMap = mapNum;
            if (Globals.MapPropertiesWindow != null) { Globals.MapPropertiesWindow.Init(Globals.CurrentMap); }
            if (mapNum > -1)
            {
                if (Globals.GameMaps[mapNum] != null)
                {
                    Text = @"Intersect Editor - Map# " + mapNum + @" " + Globals.GameMaps[mapNum].MyName + @" Revision: " + Globals.GameMaps[mapNum].Revision;
                }
                Globals.MapEditorWindow.picMap.Visible = false;
                PacketSender.SendNeedMap(Globals.CurrentMap);
            }
            else
            {
                Text = @"Intersect Editor";
                Globals.MapEditorWindow.picMap.Visible = false;
            }
        }

        //Disconnection
        private void HandleServerDisconnect()
        {
            //Offer to export map
            if (Globals.CurrentMap > -1 && Globals.GameMaps[Globals.CurrentMap] != null)
            {
                if (MessageBox.Show("You have been disconnected from the server! Would you like to export this map before closing this editor?", "Disconnected -- Export Map?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    exportMapToolStripMenuItem_Click(null, null);
                    Application.Exit();
                }
                else
                {
                    Application.Exit();
                }
            }
            else
            {
                MessageBox.Show(@"Disconnected!");
                Application.Exit();
            }
        }

        //MenuBar Functions -- File
        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Are you sure you want to save this map?", @"Save Map", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PacketSender.SendMap(Globals.CurrentMap);
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
            PacketSender.SendCreateMap(-1, Globals.CurrentMap, null);
        }
        private void exportMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Intersect Map|*.imap";
            fileDialog.Title = "Export Map";
            fileDialog.ShowDialog();

            if (fileDialog.FileName != "")
            {
                File.WriteAllBytes(fileDialog.FileName, Globals.GameMaps[Globals.CurrentMap].Save());
            }
        }
        private void importMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Intersect Map|*.imap";
            fileDialog.Title = "Import Map";
            fileDialog.ShowDialog();

            if (fileDialog.FileName != "")
            {
                Globals.MapEditorWindow.PrepUndoState();
                Globals.GameMaps[Globals.CurrentMap].Load(File.ReadAllBytes(fileDialog.FileName));
                Globals.MapEditorWindow.AddUndoState();
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //Edit
        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer <= Constants.LayerCount)
            {
                Globals.MapEditorWindow.FillLayer();
            }
        }
        private void eraseLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer <= Constants.LayerCount)
            {
                Globals.MapEditorWindow.EraseLayer();
            }
            return;
            if (
                MessageBox.Show(@"Are you sure you want to erase this layer?", @"Erase Layer", MessageBoxButtons.YesNo) !=
                DialogResult.Yes) return;
            for (var x = 0; x < Constants.MapWidth; x++)
            {
                for (var y = 0; y < Constants.MapHeight; y++)
                {
                    Globals.CurTileX = x;
                    Globals.CurTileY = y;
                    Globals.MapEditorWindow.picMap_MouseDown(null, new MouseEventArgs(MouseButtons.Right, 1, x * Globals.TileWidth + Globals.TileWidth, y * Globals.TileHeight + Globals.TileHeight, 0));
                    Globals.MapEditorWindow.picMap_MouseUp(null, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0));
                }
            }
        }
        //View
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
        private void hideTilePreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics.HideTilePreview = !Graphics.HideTilePreview;
            hideTilePreviewToolStripMenuItem.Checked = Graphics.HideTilePreview;
        }
        //Content Editors
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
        //Help
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        //ToolStrip Functions
        private void toolStripBtnNewMap_Click(object sender, EventArgs e)
        {
            newMapToolStripMenuItem_Click(null, null);
        }
        private void toolStripBtnSaveMap_Click(object sender, EventArgs e)
        {
            saveMapToolStripMenuItem_Click(null, null);
        }
        private void toolStripBtnUndo_Click(object sender, EventArgs e)
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (Globals.MapEditorWindow.MapUndoStates.Count > 0)
            {
                tmpMap.Load(Globals.MapEditorWindow.MapUndoStates[Globals.MapEditorWindow.MapUndoStates.Count - 1]);
                Globals.MapEditorWindow.MapRedoStates.Add(Globals.MapEditorWindow.CurrentMapState);
                Globals.MapEditorWindow.CurrentMapState = Globals.MapEditorWindow.MapUndoStates[Globals.MapEditorWindow.MapUndoStates.Count - 1];
                Globals.MapEditorWindow.MapUndoStates.RemoveAt(Globals.MapEditorWindow.MapUndoStates.Count - 1);
                Globals.MapPropertiesWindow.Update();
            }
        }
        private void toolStripBtnRedo_Click(object sender, EventArgs e)
        {
            var tmpMap = Globals.GameMaps[Globals.CurrentMap];
            if (Globals.MapEditorWindow.MapRedoStates.Count > 0)
            {
                tmpMap.Load(Globals.MapEditorWindow.MapRedoStates[Globals.MapEditorWindow.MapRedoStates.Count - 1]);
                Globals.MapEditorWindow.MapUndoStates.Add(Globals.MapEditorWindow.CurrentMapState);
                Globals.MapEditorWindow.CurrentMapState = Globals.MapEditorWindow.MapRedoStates[Globals.MapEditorWindow.MapRedoStates.Count - 1];
                Globals.MapEditorWindow.MapRedoStates.RemoveAt(Globals.MapEditorWindow.MapRedoStates.Count - 1);
                Globals.MapPropertiesWindow.Update();
            }
        }
        private void toolStripBtnFill_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer <= Constants.LayerCount)
            {
                Globals.MapEditorWindow.FillLayer();
            }
        }
        private void toolStripBtnErase_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer <= Constants.LayerCount)
            {
                Globals.MapEditorWindow.EraseLayer();
            }
        }
        private void toolStripBtnScreenshot_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Png Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            fileDialog.Title = "Save a screenshot of the map";
            fileDialog.ShowDialog();

            if (fileDialog.FileName != "")
            {
                Graphics.ScreenShotMap().SaveToFile(fileDialog.FileName);
            }
        }

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
