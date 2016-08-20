/*
    Intersect Game Engine (Editor)
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect_Editor.Classes;
using WeifenLuo.WinFormsUI.Docking;
using Intersect_Editor.Classes.General;
using Intersect_Editor.Classes.Maps;
using Intersect_Editor.Forms.DockingElements;
using Intersect_Editor.Forms.Editors;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Microsoft.Xna.Framework.Graphics;

namespace Intersect_Editor.Forms
{
    public partial class frmMain : Form
    {
        //General Editting Variables
        bool _tMouseDown;

        //Cross Thread Delegates
        public delegate void TryOpenEditor(GameObject type);
        public TryOpenEditor EditorDelegate;

        public delegate void UpdateTimeList();
        public UpdateTimeList TimeDelegate;

        public delegate void HandleDisconnect();
        public HandleDisconnect DisconnectDelegate;

        //Editor References
        private frmAnimation _animationEditor;
        private FrmItem _itemEditor;
        private frmNpc _npcEditor;
        private frmResource _resourceEditor;
        private frmSpell _spellEditor;
        private frmCrafting _craftEditor;
        private frmClass _classEditor;
        private frmQuest _questEditor;
        private frmProjectile _projectileEditor;
        private frmCommonEvent _commonEventEditor;
        private frmSwitchVariable _switchVariableEditor;
        private frmShop _shopEditor;
        private frmTime _timeEditor;

        //Initialization & Setup Functions
        public frmMain()
        {
            InitializeComponent();
            Globals.MapListWindow = new frmMapList();
            Globals.MapListWindow.Show(dockLeft, DockState.DockRight);
            Globals.MapLayersWindow = new frmMapLayers();
            Globals.MapLayersWindow.Show(dockLeft, DockState.DockLeft);

            Globals.MapEditorWindow = new frmMapEditor();
            Globals.MapEditorWindow.Show(dockLeft, DockState.Document);

            Globals.MapGridWindowNew = new frmMapGrid();
            Globals.MapGridWindowNew.Show(dockLeft,DockState.Document);
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Set form object properties based on constants to prevent user inputting invalid options.
            InitFormObjects();

            //Init Delegates
            EditorDelegate = TryOpenEditorMethod;
            DisconnectDelegate = HandleServerDisconnect;
            TimeDelegate = UpdateTimeSimulationList;

            // Initilise the editor.
            InitEditor();

            //Init Map Properties
            InitMapProperties();
            Show();

            //Init Forms with RenderTargets
            Globals.MapEditorWindow.InitMapEditor();
            Globals.MapLayersWindow.InitMapLayers();
            Globals.MapGridWindowNew.InitGridWindow();
            UpdateTimeSimulationList();
        }
        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.Z))
            {
                toolStripBtnUndo_Click(null, null);
                return;
            }
            else if (e.KeyData == (Keys.Control | Keys.Y))
            {
                toolStripBtnRedo_Click(null, null);
                return;
            }
            else if (e.KeyData == (Keys.Control | Keys.X))
            {
                toolStripBtnCut_Click(null, null);
                return;
            }
            else if (e.KeyData == (Keys.Control | Keys.C))
            {
                toolStripBtnCopy_Click(null, null);
                return;
            }
            else if (e.KeyData == (Keys.Control | Keys.V))
            {
                toolStripBtnPaste_Click(null, null);
                return;
            }
            else if (e.KeyData == (Keys.Control | Keys.S))
            {
                toolStripBtnSaveMap_Click(null, null);
                return;
            }
            var xDiff = 0;
            var yDiff = 0;
            if (dockLeft.ActiveContent == Globals.MapEditorWindow || (dockLeft.ActiveContent == null && Globals.MapEditorWindow.DockPanel.ActiveDocument == Globals.MapEditorWindow))
            {
                if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
                {
                    yDiff -= 20;
                }
                if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
                {
                    yDiff += 20;
                }
                if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                {
                    xDiff -= 20;
                }
                if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                {
                    xDiff += 20;
                }
                if (xDiff != 0 || yDiff != 0)
                {
                    EditorGraphics.CurrentView.X -= (xDiff);
                    EditorGraphics.CurrentView.Y -= (yDiff);
                    if (EditorGraphics.CurrentView.X > Options.MapWidth*Options.TileWidth)
                        EditorGraphics.CurrentView.X = Options.MapWidth*Options.TileWidth;
                    if (EditorGraphics.CurrentView.Y > Options.MapHeight*Options.TileHeight)
                        EditorGraphics.CurrentView.Y = Options.MapHeight*Options.TileHeight;
                    if (EditorGraphics.CurrentView.X - Globals.MapEditorWindow.picMap.Width <
                        -Options.TileWidth*Options.MapWidth*2)
                    {
                        EditorGraphics.CurrentView.X = -Options.TileWidth*Options.MapWidth*2 +
                                                       Globals.MapEditorWindow.picMap.Width;
                    }
                    if (EditorGraphics.CurrentView.Y - Globals.MapEditorWindow.picMap.Height <
                        -Options.TileHeight*Options.MapHeight*2)
                    {
                        EditorGraphics.CurrentView.Y = -Options.TileHeight*Options.MapHeight*2 +
                                                       Globals.MapEditorWindow.picMap.Height;
                    }
                }
            }
        }
        private void InitFormObjects()
        {
            Globals.MapLayersWindow.scrlX.Maximum = Options.MapWidth;
            Globals.MapLayersWindow.scrlY.Maximum = Options.MapHeight;
        }
        private void InitMapProperties()
        {
            DockPane unhiddenPane = dockLeft.Panes[0];
            Globals.MapPropertiesWindow = new frmMapProperties();
            Globals.MapPropertiesWindow.Show(unhiddenPane, DockAlignment.Bottom, .4);
            Globals.MapPropertiesWindow.Init(Globals.CurrentMap);
            Globals.MapEditorWindow.DockPanel.Focus();
        }
        private void InitEditor()
        {
            EditorGraphics.InitMonogame();
            Globals.MapLayersWindow.InitTilesets();
            Globals.MapLayersWindow.Init();
            Globals.InEditor = true;
            GrabMouseDownEvents();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }
        public void EnterMap(int mapNum)
        {
            Globals.CurrentMap = MapInstance.GetMap(mapNum);
            Globals.LoadingMap = mapNum;
            if (Globals.CurrentMap == null)
            {
                Text = @"Intersect Editor";
            }
            else
            {
                if (Globals.MapPropertiesWindow != null) { Globals.MapPropertiesWindow.Init(Globals.CurrentMap); }
            }
            Globals.MapEditorWindow.picMap.Visible = false;
            Globals.MapEditorWindow.ResetUndoRedoStates();
            PacketSender.SendNeedMap(mapNum);
            PacketSender.SendNeedGrid(mapNum);
            EditorGraphics.TilePreviewUpdated = true;
        }
        private void GrabMouseDownEvents()
        {
            GrabMouseDownEvents(this);
        }
        private void GrabMouseDownEvents(Control e)
        {
            foreach (Control t in e.Controls)
            {
                if (t.GetType() == typeof(MenuStrip))
                {
                    foreach (ToolStripMenuItem t1 in ((MenuStrip)t).Items)
                    {
                        t1.MouseDown += MouseDownHandler;
                    }
                    t.MouseDown += MouseDownHandler;
                }
                else if (t.GetType() == typeof(PropertyGrid))
                {
                }
                else
                {
                    GrabMouseDownEvents(t);
                }
            }
            e.MouseDown += MouseDownHandler;
        }
        public void MouseDownHandler(object sender, MouseEventArgs e)
        {
            if (sender != Globals.MapEditorWindow && sender != Globals.MapEditorWindow.pnlMapContainer &&
                sender != Globals.MapEditorWindow.picMap)
            {
                Globals.MapEditorWindow.PlaceSelection();
            }
        }

        //Update
        public void Update()
        {
            if (Globals.CurrentMap != null)
            {
                toolStripLabelCoords.Text = @" CurX: " + Globals.CurTileX + @" CurY: " + Globals.CurTileY;
                toolStripLabelRevision.Text = @"Revision: " + Globals.CurrentMap.Revision;
                if (Text != @"Intersect Editor - " +  Globals.CurrentMap.MyName)
                {
                    Text = @"Intersect Editor - " + Globals.CurrentMap.MyName;
                }
            }

            //Process the Undo/Redo Buttons
            if (Globals.MapEditorWindow.MapUndoStates.Count > 0)
            {
                toolStripBtnUndo.Enabled = true;
                undoToolStripMenuItem.Enabled = true;
            }
            else
            {
                toolStripBtnUndo.Enabled = false;
                undoToolStripMenuItem.Enabled = false;
            }
            if (Globals.MapEditorWindow.MapRedoStates.Count > 0)
            {
                toolStripBtnRedo.Enabled = true;
                redoToolStripMenuItem.Enabled = true;
            }
            else
            {
                toolStripBtnRedo.Enabled = false;
                redoToolStripMenuItem.Enabled = false;
            }

            //Process the Fill/Erase Buttons
            if (Globals.CurrentLayer <= Options.LayerCount)
            {
                toolStripBtnFill.Enabled = true;
                fillToolStripMenuItem.Enabled = true;
                toolStripBtnErase.Enabled = true;
                eraseLayerToolStripMenuItem.Enabled = true;
            }
            else
            {
                toolStripBtnFill.Enabled = false;
                fillToolStripMenuItem.Enabled = false;
                toolStripBtnErase.Enabled = false;
                eraseLayerToolStripMenuItem.Enabled = false;
            }

            //Process the Tool Buttons
            toolStripBtnPen.Enabled = false;
            toolStripBtnSelect.Enabled = true;
            toolStripBtnRect.Enabled = false;
            toolStripBtnEyeDrop.Enabled = false;
            if (Globals.CurrentLayer == Options.LayerCount) //Attributes
            {
                toolStripBtnPen.Enabled = true;
                toolStripBtnRect.Enabled = true;
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 1) //Lights
            {
                Globals.CurrentTool = (int)EdittingTool.Selection;
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 2) //Events
            {
                Globals.CurrentTool = (int)EdittingTool.Selection;
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 3) //NPCS
            {
                Globals.CurrentTool = (int)EdittingTool.Selection;
            }
            else
            {
                toolStripBtnPen.Enabled = true;
                toolStripBtnRect.Enabled = true;
                toolStripBtnEyeDrop.Enabled = true;
            }

            switch (Globals.CurrentTool)
            {
                case (int)EdittingTool.Pen:
                    if (!toolStripBtnPen.Checked) { toolStripBtnPen.Checked = true; }
                    if (toolStripBtnSelect.Checked) { toolStripBtnSelect.Checked = false; }
                    if (toolStripBtnRect.Checked) { toolStripBtnRect.Checked = false; }
                    if (toolStripBtnEyeDrop.Checked) { toolStripBtnEyeDrop.Checked = false; }

                    if (toolStripBtnCut.Enabled) { toolStripBtnCut.Enabled = false; }
                    if (toolStripBtnCopy.Enabled) { toolStripBtnCopy.Enabled = false; }
                    if (cutToolStripMenuItem.Enabled) { cutToolStripMenuItem.Enabled = false; }
                    if (copyToolStripMenuItem.Enabled) { copyToolStripMenuItem.Enabled = false; }
                    break;
                case (int)EdittingTool.Selection:
                    if (toolStripBtnPen.Checked) { toolStripBtnPen.Checked = false; }
                    if (!toolStripBtnSelect.Checked) { toolStripBtnSelect.Checked = true; }
                    if (toolStripBtnRect.Checked) { toolStripBtnRect.Checked = false; }
                    if (toolStripBtnEyeDrop.Checked) { toolStripBtnEyeDrop.Checked = false; }

                    if (!toolStripBtnCut.Enabled) { toolStripBtnCut.Enabled = true; }
                    if (!toolStripBtnCopy.Enabled) { toolStripBtnCopy.Enabled = true; }
                    if (!cutToolStripMenuItem.Enabled) { cutToolStripMenuItem.Enabled = true; }
                    if (!copyToolStripMenuItem.Enabled) { copyToolStripMenuItem.Enabled = true; }
                    break;
                case (int)EdittingTool.Rectangle:
                    if (toolStripBtnPen.Checked) { toolStripBtnPen.Checked = false; }
                    if (toolStripBtnSelect.Checked) { toolStripBtnSelect.Checked = false; }
                    if (!toolStripBtnRect.Checked) { toolStripBtnRect.Checked = true; }
                    if (toolStripBtnEyeDrop.Checked) { toolStripBtnEyeDrop.Checked = false; }

                    if (toolStripBtnCut.Enabled) { toolStripBtnCut.Enabled = false; }
                    if (toolStripBtnCopy.Enabled) { toolStripBtnCopy.Enabled = false; }
                    if (cutToolStripMenuItem.Enabled) { cutToolStripMenuItem.Enabled = false; }
                    if (copyToolStripMenuItem.Enabled) { copyToolStripMenuItem.Enabled = false; }
                    break;
                case (int)EdittingTool.Droppler:
                    if (toolStripBtnPen.Checked) { toolStripBtnPen.Checked = false; }
                    if (toolStripBtnSelect.Checked) { toolStripBtnSelect.Checked = false; }
                    if (toolStripBtnRect.Checked) { toolStripBtnRect.Checked = false; }
                    if (!toolStripBtnEyeDrop.Checked) { toolStripBtnEyeDrop.Checked = true; }

                    if (toolStripBtnCut.Enabled) { toolStripBtnCut.Enabled = false; }
                    if (toolStripBtnCopy.Enabled) { toolStripBtnCopy.Enabled = false; }
                    if (cutToolStripMenuItem.Enabled) { cutToolStripMenuItem.Enabled = false; }
                    if (copyToolStripMenuItem.Enabled) { copyToolStripMenuItem.Enabled = false; }
                    break;
            }

            if (Globals.HasCopy)
            {
                toolStripBtnPaste.Enabled = true;
                pasteToolStripMenuItem.Enabled = true;
            }
            else
            {
                toolStripBtnPaste.Enabled = false;
                pasteToolStripMenuItem.Enabled = false;
            }

            if (Globals.Dragging)
            {
                if (Globals.MainForm.ActiveControl.GetType() == typeof(WeifenLuo.WinFormsUI.Docking.DockPane))
                {
                    Control ctrl = ((WeifenLuo.WinFormsUI.Docking.DockPane)Globals.MainForm.ActiveControl).ActiveControl;
                    if (ctrl != Globals.MapEditorWindow)
                    {
                        Globals.MapEditorWindow.PlaceSelection();
                    }
                }
            }
        }

        //Disconnection
        private void HandleServerDisconnect()
        {
            if (!Globals.ClosingEditor)
            {
                Globals.ClosingEditor = true;
                //Offer to export map
                if (Globals.CurrentMap != null)
                {
                    if (MessageBox.Show(
                            "You have been disconnected from the server! Would you like to export this map before closing this editor?",
                            "Disconnected -- Export Map?", MessageBoxButtons.YesNo) ==
                        System.Windows.Forms.DialogResult.Yes)
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
            if (Globals.CurrentMap.Changed() && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PacketSender.SendMap(Globals.CurrentMap);
            }
            PacketSender.SendCreateMap(-1, Globals.CurrentMap.GetId(), null);
        }
        private void exportMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Intersect Map|*.imap";
            fileDialog.Title = "Export Map";
            fileDialog.ShowDialog();
            var buff = new ByteBuffer();
            buff.WriteString(Application.ProductVersion);
            buff.WriteBytes(Globals.CurrentMap.GetMapData(false));
            if (fileDialog.FileName != "")
            {
                File.WriteAllBytes(fileDialog.FileName,buff.ToArray());
            }
            buff.Dispose();
        }
        private void importMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Intersect Map|*.imap";
            fileDialog.Title = "Import Map";
            fileDialog.ShowDialog();

            if (fileDialog.FileName != "")
            {
                var data = File.ReadAllBytes(fileDialog.FileName);
                var buff = new ByteBuffer();
                buff.WriteBytes(data);
                if (buff.ReadString() == Application.ProductVersion)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    Globals.CurrentMap.Load(buff.ReadBytes(buff.Length(), true));
                    Globals.MapEditorWindow.AddUndoState();
                }
                else
                {
                    MessageBox.Show("Cannot import map. Currently selected map is not an Intersect map file or was exported with a different version of the Intersect editor!","Failed to Import Map");
                }
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //Edit
        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer <= Options.LayerCount)
            {
                Globals.MapEditorWindow.FillLayer();
            }
        }
        private void eraseLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer <= Options.LayerCount)
            {
                Globals.MapEditorWindow.EraseLayer();
            }
        }
        private void allLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.SelectionType = (int)SelectionTypes.AllLayers;
            allLayersToolStripMenuItem.Checked = true;
            currentLayerOnlyToolStripMenuItem.Checked = false;
        }
        private void currentLayerOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.SelectionType = (int)SelectionTypes.CurrentLayer;
            allLayersToolStripMenuItem.Checked = false;
            currentLayerOnlyToolStripMenuItem.Checked = true;
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripBtnUndo_Click(null, null);
        }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripBtnRedo_Click(null, null);
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripBtnCut_Click(null, null);
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripBtnCopy_Click(null, null);
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripBtnPaste_Click(null, null);
        }
        //View
        private void hideDarknessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorGraphics.HideDarkness = !EditorGraphics.HideDarkness;
            hideDarknessToolStripMenuItem.Checked = !EditorGraphics.HideDarkness;
        }
        private void hideFogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorGraphics.HideFog = !EditorGraphics.HideFog;
            hideFogToolStripMenuItem.Checked = !EditorGraphics.HideFog;
        }
        private void hideOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorGraphics.HideOverlay = !EditorGraphics.HideOverlay;
            hideOverlayToolStripMenuItem.Checked = !EditorGraphics.HideOverlay;
        }
        private void hideTilePreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorGraphics.HideTilePreview = !EditorGraphics.HideTilePreview;
            hideTilePreviewToolStripMenuItem.Checked = !EditorGraphics.HideTilePreview;
        }
        private void hideResourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorGraphics.HideResources = !EditorGraphics.HideResources;
            hideResourcesToolStripMenuItem.Checked = !EditorGraphics.HideResources;
        }
        private void mapGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditorGraphics.HideGrid = !EditorGraphics.HideGrid;
            mapGridToolStripMenuItem.Checked = !EditorGraphics.HideGrid;
        }
        //Content Editors
        private void itemEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.Item);
        }
        private void npcEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.Npc);
        }
        private void spellEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.Spell);
        }
        private void craftingEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.Bench);
        }
        private void animationEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.Animation);
        }
        private void resourceEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.Resource);
        }
        private void classEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.Class);
        }
        private void questEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //PacketSender.SendQuestEditor();
        }
        private void projectileEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.Projectile);
        }
        private void commonEventEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.CommonEvent);
        }
        private void switchVariableEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.PlayerSwitch);
        }
        private void shopEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.Shop);
        }
        private void timeEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObject.Time);
        }
        //Help
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout aboutfrm = new frmAbout();
            aboutfrm.ShowDialog();
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
            var tmpMap = Globals.CurrentMap;
            if (Globals.MapEditorWindow.MapUndoStates.Count > 0)
            {
                tmpMap.Load(Globals.MapEditorWindow.MapUndoStates[Globals.MapEditorWindow.MapUndoStates.Count - 1]);
                Globals.MapEditorWindow.MapRedoStates.Add(Globals.MapEditorWindow.CurrentMapState);
                Globals.MapEditorWindow.CurrentMapState = Globals.MapEditorWindow.MapUndoStates[Globals.MapEditorWindow.MapUndoStates.Count - 1];
                Globals.MapEditorWindow.MapUndoStates.RemoveAt(Globals.MapEditorWindow.MapUndoStates.Count - 1);
                Globals.MapPropertiesWindow.Update();
                EditorGraphics.TilePreviewUpdated = true;
            }
        }
        private void toolStripBtnRedo_Click(object sender, EventArgs e)
        {
            var tmpMap = Globals.CurrentMap;
            if (Globals.MapEditorWindow.MapRedoStates.Count > 0)
            {
                tmpMap.Load(Globals.MapEditorWindow.MapRedoStates[Globals.MapEditorWindow.MapRedoStates.Count - 1]);
                Globals.MapEditorWindow.MapUndoStates.Add(Globals.MapEditorWindow.CurrentMapState);
                Globals.MapEditorWindow.CurrentMapState = Globals.MapEditorWindow.MapRedoStates[Globals.MapEditorWindow.MapRedoStates.Count - 1];
                Globals.MapEditorWindow.MapRedoStates.RemoveAt(Globals.MapEditorWindow.MapRedoStates.Count - 1);
                Globals.MapPropertiesWindow.Update();
                EditorGraphics.TilePreviewUpdated = true;
            }
        }
        private void toolStripBtnFill_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer <= Options.LayerCount)
            {
                Globals.MapEditorWindow.FillLayer();
            }
        }
        private void toolStripBtnErase_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentLayer <= Options.LayerCount)
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
                using (var fs = new FileStream(fileDialog.FileName, FileMode.OpenOrCreate))
                {
                    var screenshotTexture = EditorGraphics.ScreenShotMap();
                    screenshotTexture.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                    fs.Close();
                }
            }
        }
        private void toolStripBtnPen_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int)EdittingTool.Pen;
        }
        private void toolStripBtnSelect_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int)EdittingTool.Selection;
        }
        private void toolStripBtnRect_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int)EdittingTool.Rectangle;
            Globals.CurMapSelX = 0;
            Globals.CurMapSelY = 0;
        }
        private void toolStripBtnEyeDrop_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int)EdittingTool.Droppler;
            Globals.CurMapSelX = 0;
            Globals.CurMapSelY = 0;
        }
        private void toolStripBtnCopy_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentTool != (int)EdittingTool.Selection) { return; }
            Globals.MapEditorWindow.Copy();
        }
        private void toolStripBtnPaste_Click(object sender, EventArgs e)
        {
            if (!Globals.HasCopy) { return; }
            Globals.MapEditorWindow.Paste();
        }
        private void toolStripBtnCut_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentTool != (int)EdittingTool.Selection) { return; }
            Globals.MapEditorWindow.Cut();
        }

        //Cross Threading Delegate Methods
        private void TryOpenEditorMethod(GameObject type)
        {
            if (Globals.CurrentEditor != -2)
            {
                switch (type)
                {
                    case GameObject.Animation:
                        if (_animationEditor == null || _animationEditor.Visible == false)
                        {
                            _animationEditor = new frmAnimation();
                            _animationEditor.InitEditor();
                            _animationEditor.Show();
                        }
                        break;
                    case GameObject.Item:
                        if (_itemEditor == null || _itemEditor.Visible == false)
                        {
                            _itemEditor = new FrmItem();
                            _itemEditor.InitEditor();
                            _itemEditor.Show();
                        }
                        break;
                    case GameObject.Npc:
                        if (_npcEditor == null || _npcEditor.Visible == false)
                        {
                            _npcEditor = new frmNpc();
                            _npcEditor.InitEditor();
                            _npcEditor.Show();
                        }
                        break;
                    case GameObject.Resource:
                        if (_resourceEditor == null || _resourceEditor.Visible == false)
                        {
                            _resourceEditor = new frmResource();
                            _resourceEditor.InitEditor();
                            _resourceEditor.Show();
                        }
                        break;
                    case GameObject.Spell:
                        if (_spellEditor == null || _spellEditor.Visible == false)
                        {
                            _spellEditor = new frmSpell();
                            _spellEditor.InitEditor();
                            _spellEditor.Show();
                        }
                        break;
                    case GameObject.Bench:
                        if (_craftEditor == null || _craftEditor.Visible == false)
                        {
                            _craftEditor = new frmCrafting();
                            _craftEditor.InitEditor();
                            _craftEditor.Show();
                        }
                        break;
                    case GameObject.Class:
                        if (_classEditor == null || _classEditor.Visible == false)
                        {
                            _classEditor = new frmClass();
                            _classEditor.InitEditor();
                            _classEditor.Show();
                        }
                        break;
                    case GameObject.Quest:
                        if (_questEditor == null || _questEditor.Visible == false)
                        {
                            _questEditor = new frmQuest();
                            _questEditor.InitEditor();
                            _questEditor.Show();
                        }
                        break;
                    case GameObject.Projectile:
                        if (_projectileEditor == null || _projectileEditor.Visible == false)
                        {
                            _projectileEditor = new frmProjectile();
                            _projectileEditor.InitEditor();
                            _projectileEditor.Show();
                        }
                        break;
                    case GameObject.CommonEvent:
                        if (_commonEventEditor == null || _commonEventEditor.Visible == false)
                        {
                            _commonEventEditor = new frmCommonEvent();
                            _commonEventEditor.Show();
                        }
                        break;
                    case GameObject.PlayerSwitch:
                        if (_switchVariableEditor == null || _switchVariableEditor.Visible == false)
                        {
                            _switchVariableEditor = new frmSwitchVariable();
                            _switchVariableEditor.InitEditor();
                            _switchVariableEditor.Show();
                        }
                        break;
                    case GameObject.Shop:
                        if (_shopEditor == null || _shopEditor.Visible == false)
                        {
                            _shopEditor = new frmShop();
                            _shopEditor.InitEditor();
                            _shopEditor.Show();
                        }
                        break;
                    case GameObject.Time:
                        if (_timeEditor == null || _timeEditor.Visible == false)
                        {
                            _timeEditor = new frmTime();
                            _timeEditor.InitEditor(TimeBase.GetTimeBase());
                            _timeEditor.Show();
                        }
                        break;
                    default:
                        return;
                }
                Globals.CurrentEditor = (int)type;
            }

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Globals.ClosingEditor && Globals.CurrentMap != null && Globals.CurrentMap.Changed() && MessageBox.Show(@"Do you want to save your current map?", @"Save current map?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PacketSender.SendMap(Globals.CurrentMap);
            }
            Globals.ClosingEditor = true;
        }

        private void toolStripTimeButton_Click(object sender, EventArgs e)
        {

        }

        private void UpdateTimeSimulationList()
        {
            Bitmap transtile = null;
            if (File.Exists("resources/misc/transtile.png"))
            {
                transtile = new Bitmap("resources/misc/transtile.png");
            }
            toolStripTimeButton.DropDownItems.Clear();
            var time = new DateTime(2000, 1, 1, 0, 0, 0);
            var x = 0;
            ToolStripDropDownButton btn = new ToolStripDropDownButton("None");
            btn.Tag = null;
            btn.Click += TimeDropdownButton_Click;
            toolStripTimeButton.DropDownItems.Add(btn);
            for (int i = 0; i < 1440; i += TimeBase.GetTimeBase().RangeInterval)
            {
                var addRange = time.ToString("h:mm:ss tt") + " to ";
                time = time.AddMinutes(TimeBase.GetTimeBase().RangeInterval);
                addRange += time.ToString("h:mm:ss tt");

                //Create image of overlay color
                var img = new Bitmap(32, 32);
                var g = System.Drawing.Graphics.FromImage(img);
                g.Clear(System.Drawing.Color.Transparent);
                //Draw the trans tile if we have it
                if (transtile != null)
                {
                    g.DrawImage(transtile, new System.Drawing.Point(0, 0));
                }
                var clr = TimeBase.GetTimeBase().RangeColors[x];
                Brush brush =
                new SolidBrush(System.Drawing.Color.FromArgb( clr.A, clr.R,clr.G,clr.B));
                g.FillRectangle(brush, new System.Drawing.Rectangle(0, 0,32,32));

                //Draw the overlay color
                g.Dispose();

                btn = new ToolStripDropDownButton(addRange,img);
                btn.Tag = clr;
                btn.Click += TimeDropdownButton_Click;
                toolStripTimeButton.DropDownItems.Add(btn);
                x++;
            }
            if (transtile != null) transtile.Dispose();
        }

        private void TimeDropdownButton_Click(object sender, EventArgs e)
        {
            if (((ToolStripDropDownButton) sender).Tag == null)
            {
                EditorGraphics.LightColor = null;
            }
            else
            {
                EditorGraphics.LightColor = (Intersect_Library.Color)((ToolStripDropDownButton)sender).Tag;
            }
        }

        private void btnGridView_Click(object sender, EventArgs e)
        {
            //This should toggle us in/out of "grid view"
            Globals.GridView = !Globals.GridView;
            Globals.MapEditorWindow.InitMapEditor();
        }

    }
}
