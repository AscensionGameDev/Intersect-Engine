using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Forms;
using Intersect.Editor.Classes;
using Intersect.Editor.Classes.Core;
using Intersect.Editor.Classes.General;
using Intersect.Editor.Classes.Maps;
using Intersect.Editor.Forms.DockingElements;
using Intersect.Editor.Forms.Editors;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Editor.Classes.Localization;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect.Editor.Forms
{
    public partial class FrmMain : Form
    {
        public delegate void HandleDisconnect();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(System.Drawing.Point pnt);

        //Cross Thread Delegates
        public delegate void TryOpenEditor(GameObjectType type);

        public delegate void UpdateTimeList();

        //Editor References
        private FrmAnimation mAnimationEditor;

        private FrmClass mClassEditor;
        private FrmCommonEvent mCommonEventEditor;
        private FrmCrafting mCraftEditor;
        private FrmItem mItemEditor;
        private FrmNpc mNpcEditor;
        private FrmProjectile mProjectileEditor;
        private FrmQuest mQuestEditor;
        private FrmResource mResourceEditor;
        private FrmShop mShopEditor;
        private FrmSpell mSpellEditor;
        private FrmSwitchVariable mSwitchVariableEditor;

        private FrmTime mTimeEditor;

        //General Editting Variables
        bool mTMouseDown;

        public HandleDisconnect DisconnectDelegate;
        public TryOpenEditor EditorDelegate;
        public UpdateTimeList TimeDelegate;

        //Initialization & Setup Functions
        public FrmMain()
        {
            InitializeComponent();
            dockLeft.Theme = new VS2015DarkTheme();
            Globals.MapListWindow = new FrmMapList();
            Globals.MapListWindow.Show(dockLeft, DockState.DockRight);
            Globals.MapLayersWindow = new FrmMapLayers();
            Globals.MapLayersWindow.Show(dockLeft, DockState.DockLeft);

            Globals.MapEditorWindow = new FrmMapEditor();
            Globals.MapEditorWindow.Show(dockLeft, DockState.Document);

            Globals.MapGridWindowNew = new FrmMapGrid();
            Globals.MapGridWindowNew.Show(dockLeft, DockState.Document);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //Init Delegates
            EditorDelegate = TryOpenEditorMethod;
            DisconnectDelegate = HandleServerDisconnect;
            TimeDelegate = UpdateTimeSimulationList;

            // Initilise the editor.
            InitEditor();

            //Init Map Properties
            InitMapProperties();
            InitLocalization();
            InitExternalTools();
            Show();

            //Init Forms with RenderTargets
            Globals.MapEditorWindow.InitMapEditor();
            Globals.MapLayersWindow.InitMapLayers();
            Globals.MapGridWindowNew.InitGridWindow();
            UpdateTimeSimulationList();

            toolStripButtonDonate.Size = new Size(54, 25);
            WindowState = FormWindowState.Maximized;
        }

        private void InitLocalization()
        {
            fileToolStripMenuItem.Text = Strings.mainform.file;
            saveMapToolStripMenuItem.Text = Strings.mainform.savemap;
            toolStripBtnSaveMap.Text = Strings.mainform.savemap;
            newMapToolStripMenuItem.Text = Strings.mainform.newmap;
            toolStripBtnNewMap.Text = Strings.mainform.newmap;
            importMapToolStripMenuItem.Text = Strings.mainform.importmap;
            exportMapToolStripMenuItem.Text = Strings.mainform.exportmap;
            optionsToolStripMenuItem.Text = Strings.mainform.options;
            exitToolStripMenuItem.Text = Strings.mainform.exit;

            editToolStripMenuItem.Text = Strings.mainform.edit;
            undoToolStripMenuItem.Text = Strings.mainform.undo;
            redoToolStripMenuItem.Text = Strings.mainform.redo;
            cutToolStripMenuItem.Text = Strings.mainform.cut;
            copyToolStripMenuItem.Text = Strings.mainform.copy;
            pasteToolStripMenuItem.Text = Strings.mainform.paste;
            toolStripBtnUndo.Text = Strings.mainform.undo;
            toolStripBtnRedo.Text = Strings.mainform.redo;
            toolStripBtnCut.Text = Strings.mainform.cut;
            toolStripBtnCopy.Text = Strings.mainform.copy;
            toolStripBtnPaste.Text = Strings.mainform.paste;

            fillToolStripMenuItem.Text = Strings.mainform.fill;
            toolStripBtnFill.Text = Strings.mainform.fill;
            eraseLayerToolStripMenuItem.Text = Strings.mainform.erase;
            toolStripBtnErase.Text = Strings.mainform.erase;

            selectToolStripMenuItem.Text = Strings.mainform.selectlayers;
            allLayersToolStripMenuItem.Text = Strings.mainform.alllayers;
            currentLayerOnlyToolStripMenuItem.Text = Strings.mainform.currentonly;

            viewToolStripMenuItem.Text = Strings.mainform.view;
            hideDarknessToolStripMenuItem.Text = Strings.mainform.darkness;
            hideFogToolStripMenuItem.Text = Strings.mainform.fog;
            hideOverlayToolStripMenuItem.Text = Strings.mainform.overlay;
            hideResourcesToolStripMenuItem.Text = Strings.mainform.resources;
            hideTilePreviewToolStripMenuItem.Text = Strings.mainform.tilepreview;
            mapGridToolStripMenuItem.Text = Strings.mainform.grid;

            contentEditorsToolStripMenuItem.Text = Strings.mainform.editors;
            animationEditorToolStripMenuItem.Text = Strings.mainform.animationeditor;
            classEditorToolStripMenuItem.Text = Strings.mainform.classeditor;
            commonEventEditorToolStripMenuItem.Text = Strings.mainform.commoneventeditor;
            craftingEditorToolStripMenuItem.Text = Strings.mainform.craftingbencheditor;
            itemEditorToolStripMenuItem.Text = Strings.mainform.itemeditor;
            npcEditorToolStripMenuItem.Text = Strings.mainform.npceditor;
            projectileEditorToolStripMenuItem.Text = Strings.mainform.projectileeditor;
            questEditorToolStripMenuItem.Text = Strings.mainform.questeditor;
            resourceEditorToolStripMenuItem.Text = Strings.mainform.resourceeditor;
            shopEditorToolStripMenuItem.Text = Strings.mainform.shopeditor;
            spellEditorToolStripMenuItem.Text = Strings.mainform.spelleditor;
            switchVariableEditorToolStripMenuItem.Text = Strings.mainform.switchvariableeditor;
            timeEditorToolStripMenuItem.Text = Strings.mainform.timeeditor;

            externalToolsToolStripMenuItem.Text = Strings.mainform.externaltools;

            helpToolStripMenuItem.Text = Strings.mainform.help;
            postQuestionToolStripMenuItem.Text = Strings.mainform.postquestion;
            toolStripButtonQuestion.Text = Strings.mainform.postquestion;
            reportBugToolStripMenuItem.Text = Strings.mainform.reportbug;
            toolStripButtonBug.Text = Strings.mainform.reportbug;
            aboutToolStripMenuItem.Text = Strings.mainform.about;
            toolStripButtonDonate.Text = Strings.mainform.donate;

            toolStripBtnPen.Text = Strings.mainform.pen;
            toolStripBtnSelect.Text = Strings.mainform.selection;
            toolStripBtnRect.Text = Strings.mainform.rectangle;
            toolStripBtnEyeDrop.Text = Strings.mainform.droppler;
            toolStripTimeButton.Text = Strings.mainform.lighting;
            toolStripBtnScreenshot.Text = Strings.mainform.screenshot;
            toolStripBtnRun.Text = Strings.mainform.run;
        }

        private void InitExternalTools()
        {
            var foundTools = false;
            if (Directory.Exists(Strings.mainform.toolsdir))
            {
                var childDirs = Directory.GetDirectories("tools");
                for (int i = 0; i < childDirs.Length; i++)
                {
                    var executables = Directory.GetFiles(childDirs[i], "*.exe");
                    for (int x = 0; x < executables.Length; x++)
                    {
                        var item =
                            externalToolsToolStripMenuItem.DropDownItems.Add(
                                executables[x].Replace(childDirs[i], "")
                                    .Replace(".exe", "")
                                    .Replace(Path.DirectorySeparatorChar.ToString(), ""));
                        item.Tag = executables[x];
                        item.Click += externalToolItem_Click;
                        foundTools = true;
                    }
                }
            }
            externalToolsToolStripMenuItem.Visible = foundTools;
        }

        private void externalToolItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty((string) ((ToolStripItem) sender).Tag))
            {
                var psi = new ProcessStartInfo(Path.GetFileName((string) ((ToolStripItem) sender).Tag))
                {
                    WorkingDirectory = Path.GetDirectoryName((string) ((ToolStripItem) sender).Tag)
                };
                Process.Start(psi);
            }
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
            if (dockLeft.ActiveContent == Globals.MapEditorWindow ||
                (dockLeft.ActiveContent == null &&
                 Globals.MapEditorWindow.DockPanel.ActiveDocument == Globals.MapEditorWindow))
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
                    IntPtr hWnd = WindowFromPoint((System.Drawing.Point)MousePosition);
                    if (hWnd != IntPtr.Zero)
                    {
                        Control ctl = Control.FromHandle(hWnd);
                        if (ctl != null)
                        {
                            if (ctl.GetType() == typeof(ComboBox) || ctl.GetType() == typeof(DarkComboBox)) return;
                        }
                    }
                    EditorGraphics.CurrentView.X -= (xDiff);
                    EditorGraphics.CurrentView.Y -= (yDiff);
                    if (EditorGraphics.CurrentView.X > Options.MapWidth * Options.TileWidth)
                        EditorGraphics.CurrentView.X = Options.MapWidth * Options.TileWidth;
                    if (EditorGraphics.CurrentView.Y > Options.MapHeight * Options.TileHeight)
                        EditorGraphics.CurrentView.Y = Options.MapHeight * Options.TileHeight;
                    if (EditorGraphics.CurrentView.X - Globals.MapEditorWindow.picMap.Width <
                        -Options.TileWidth * Options.MapWidth * 2)
                    {
                        EditorGraphics.CurrentView.X = -Options.TileWidth * Options.MapWidth * 2 +
                                                       Globals.MapEditorWindow.picMap.Width;
                    }
                    if (EditorGraphics.CurrentView.Y - Globals.MapEditorWindow.picMap.Height <
                        -Options.TileHeight * Options.MapHeight * 2)
                    {
                        EditorGraphics.CurrentView.Y = -Options.TileHeight * Options.MapHeight * 2 +
                                                       Globals.MapEditorWindow.picMap.Height;
                    }
                }
            }
        }

        private void InitMapProperties()
        {
            DockPane unhiddenPane = dockLeft.Panes[0];
            Globals.MapPropertiesWindow = new FrmMapProperties();
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
            UpdateRunState();
        }

        protected override void OnClosed(EventArgs e)
        {
            EditorNetwork.EditorLidgrenNetwork?.Disconnect("quitting");
            base.OnClosed(e);
            Application.Exit();
        }

        public void EnterMap(int mapNum)
        {
            Globals.CurrentMap = MapInstance.Lookup.Get<MapInstance>(mapNum);
            Globals.LoadingMap = mapNum;
            if (Globals.CurrentMap == null)
            {
                Text = @"Intersect Editor";
            }
            else
            {
                if (Globals.MapPropertiesWindow != null)
                {
                    Globals.MapPropertiesWindow.Init(Globals.CurrentMap);
                }
            }
            Globals.MapEditorWindow.picMap.Visible = false;
            Globals.MapEditorWindow.ResetUndoRedoStates();
            PacketSender.SendEnterMap(mapNum);
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
                    foreach (ToolStripMenuItem t1 in ((MenuStrip) t).Items)
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
            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.None) return;
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
                toolStripLabelCoords.Text = Strings.mainform.loc.ToString( Globals.CurTileX, Globals.CurTileY);
                toolStripLabelRevision.Text = Strings.mainform.revision.ToString( Globals.CurrentMap.Revision);
                if (Text != Strings.mainform.title.ToString( Globals.CurrentMap.Name))
                {
                    Text = Strings.mainform.title.ToString( Globals.CurrentMap.Name);
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
                Globals.CurrentTool = (int) EditingTool.Selection;
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 2) //Events
            {
                Globals.CurrentTool = (int) EditingTool.Selection;
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 3) //NPCS
            {
                Globals.CurrentTool = (int) EditingTool.Selection;
            }
            else
            {
                toolStripBtnPen.Enabled = true;
                toolStripBtnRect.Enabled = true;
                toolStripBtnEyeDrop.Enabled = true;
            }

            switch (Globals.CurrentTool)
            {
                case (int) EditingTool.Pen:
                    if (!toolStripBtnPen.Checked)
                    {
                        toolStripBtnPen.Checked = true;
                    }
                    if (toolStripBtnSelect.Checked)
                    {
                        toolStripBtnSelect.Checked = false;
                    }
                    if (toolStripBtnRect.Checked)
                    {
                        toolStripBtnRect.Checked = false;
                    }
                    if (toolStripBtnFill.Checked)
                    {
                        toolStripBtnFill.Checked = false;
                    }
                    if (toolStripBtnErase.Checked)
                    {
                        toolStripBtnErase.Checked = false;
                    }
                    if (toolStripBtnEyeDrop.Checked)
                    {
                        toolStripBtnEyeDrop.Checked = false;
                    }

                    if (toolStripBtnCut.Enabled)
                    {
                        toolStripBtnCut.Enabled = false;
                    }
                    if (toolStripBtnCopy.Enabled)
                    {
                        toolStripBtnCopy.Enabled = false;
                    }
                    if (cutToolStripMenuItem.Enabled)
                    {
                        cutToolStripMenuItem.Enabled = false;
                    }
                    if (copyToolStripMenuItem.Enabled)
                    {
                        copyToolStripMenuItem.Enabled = false;
                    }
                    break;
                case (int) EditingTool.Selection:
                    if (toolStripBtnPen.Checked)
                    {
                        toolStripBtnPen.Checked = false;
                    }
                    if (!toolStripBtnSelect.Checked)
                    {
                        toolStripBtnSelect.Checked = true;
                    }
                    if (toolStripBtnRect.Checked)
                    {
                        toolStripBtnRect.Checked = false;
                    }
                    if (toolStripBtnFill.Checked)
                    {
                        toolStripBtnFill.Checked = false;
                    }
                    if (toolStripBtnErase.Checked)
                    {
                        toolStripBtnErase.Checked = false;
                    }
                    if (toolStripBtnEyeDrop.Checked)
                    {
                        toolStripBtnEyeDrop.Checked = false;
                    }

                    if (!toolStripBtnCut.Enabled)
                    {
                        toolStripBtnCut.Enabled = true;
                    }
                    if (!toolStripBtnCopy.Enabled)
                    {
                        toolStripBtnCopy.Enabled = true;
                    }
                    if (!cutToolStripMenuItem.Enabled)
                    {
                        cutToolStripMenuItem.Enabled = true;
                    }
                    if (!copyToolStripMenuItem.Enabled)
                    {
                        copyToolStripMenuItem.Enabled = true;
                    }
                    break;
                case (int) EditingTool.Rectangle:
                    if (toolStripBtnPen.Checked)
                    {
                        toolStripBtnPen.Checked = false;
                    }
                    if (toolStripBtnSelect.Checked)
                    {
                        toolStripBtnSelect.Checked = false;
                    }
                    if (!toolStripBtnRect.Checked)
                    {
                        toolStripBtnRect.Checked = true;
                    }
                    if (toolStripBtnFill.Checked)
                    {
                        toolStripBtnFill.Checked = false;
                    }
                    if (toolStripBtnErase.Checked)
                    {
                        toolStripBtnErase.Checked = false;
                    }
                    if (toolStripBtnEyeDrop.Checked)
                    {
                        toolStripBtnEyeDrop.Checked = false;
                    }

                    if (toolStripBtnCut.Enabled)
                    {
                        toolStripBtnCut.Enabled = false;
                    }
                    if (toolStripBtnCopy.Enabled)
                    {
                        toolStripBtnCopy.Enabled = false;
                    }
                    if (cutToolStripMenuItem.Enabled)
                    {
                        cutToolStripMenuItem.Enabled = false;
                    }
                    if (copyToolStripMenuItem.Enabled)
                    {
                        copyToolStripMenuItem.Enabled = false;
                    }
                    break;
                case (int) EditingTool.Fill:
                    if (toolStripBtnPen.Checked)
                    {
                        toolStripBtnPen.Checked = false;
                    }
                    if (toolStripBtnSelect.Checked)
                    {
                        toolStripBtnSelect.Checked = false;
                    }
                    if (toolStripBtnRect.Checked)
                    {
                        toolStripBtnRect.Checked = false;
                    }
                    if (!toolStripBtnFill.Checked)
                    {
                        toolStripBtnFill.Checked = true;
                    }
                    if (toolStripBtnErase.Checked)
                    {
                        toolStripBtnErase.Checked = false;
                    }
                    if (toolStripBtnEyeDrop.Checked)
                    {
                        toolStripBtnEyeDrop.Checked = false;
                    }

                    if (toolStripBtnCut.Enabled)
                    {
                        toolStripBtnCut.Enabled = false;
                    }
                    if (toolStripBtnCopy.Enabled)
                    {
                        toolStripBtnCopy.Enabled = false;
                    }
                    if (cutToolStripMenuItem.Enabled)
                    {
                        cutToolStripMenuItem.Enabled = false;
                    }
                    if (copyToolStripMenuItem.Enabled)
                    {
                        copyToolStripMenuItem.Enabled = false;
                    }
                    break;
                case (int) EditingTool.Erase:
                    if (toolStripBtnPen.Checked)
                    {
                        toolStripBtnPen.Checked = false;
                    }
                    if (toolStripBtnSelect.Checked)
                    {
                        toolStripBtnSelect.Checked = false;
                    }
                    if (toolStripBtnRect.Checked)
                    {
                        toolStripBtnRect.Checked = false;
                    }
                    if (toolStripBtnFill.Checked)
                    {
                        toolStripBtnFill.Checked = false;
                    }
                    if (!toolStripBtnErase.Checked)
                    {
                        toolStripBtnErase.Checked = true;
                    }
                    if (toolStripBtnEyeDrop.Checked)
                    {
                        toolStripBtnEyeDrop.Checked = false;
                    }

                    if (toolStripBtnCut.Enabled)
                    {
                        toolStripBtnCut.Enabled = false;
                    }
                    if (toolStripBtnCopy.Enabled)
                    {
                        toolStripBtnCopy.Enabled = false;
                    }
                    if (cutToolStripMenuItem.Enabled)
                    {
                        cutToolStripMenuItem.Enabled = false;
                    }
                    if (copyToolStripMenuItem.Enabled)
                    {
                        copyToolStripMenuItem.Enabled = false;
                    }
                    break;
                case (int) EditingTool.Droppler:
                    if (toolStripBtnPen.Checked)
                    {
                        toolStripBtnPen.Checked = false;
                    }
                    if (toolStripBtnSelect.Checked)
                    {
                        toolStripBtnSelect.Checked = false;
                    }
                    if (toolStripBtnRect.Checked)
                    {
                        toolStripBtnRect.Checked = false;
                    }
                    if (toolStripBtnFill.Checked)
                    {
                        toolStripBtnFill.Checked = false;
                    }
                    if (toolStripBtnErase.Checked)
                    {
                        toolStripBtnErase.Checked = false;
                    }
                    if (!toolStripBtnEyeDrop.Checked)
                    {
                        toolStripBtnEyeDrop.Checked = true;
                    }

                    if (toolStripBtnCut.Enabled)
                    {
                        toolStripBtnCut.Enabled = false;
                    }
                    if (toolStripBtnCopy.Enabled)
                    {
                        toolStripBtnCopy.Enabled = false;
                    }
                    if (cutToolStripMenuItem.Enabled)
                    {
                        cutToolStripMenuItem.Enabled = false;
                    }
                    if (copyToolStripMenuItem.Enabled)
                    {
                        copyToolStripMenuItem.Enabled = false;
                    }
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
                if (Globals.MainForm.ActiveControl.GetType() == typeof(DockPane))
                {
                    Control ctrl = ((DockPane) Globals.MainForm.ActiveControl).ActiveControl;
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
                    if (
                        DarkMessageBox.ShowError(Strings.errors.disconnectedsave,
                            Strings.errors.disconnectedsavecaption, DarkDialogButton.YesNo,
                            Properties.Resources.Icon) == DialogResult.Yes)
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
                    DarkMessageBox.ShowError(Strings.errors.disconnectedclosing,
                        Strings.errors.disconnected, DarkDialogButton.Ok, Properties.Resources.Icon);
                    Application.Exit();
                }
            }
        }

        //MenuBar Functions -- File
        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowInformation(Strings.mapping.savemapdialoguesure,
                    Strings.mapping.savemap, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                DialogResult.Yes)
            {
                SaveMap();
            }
        }

        private static void SaveMap()
        {
            if (Globals.CurrentTool == (int) EditingTool.Selection)
            {
                if (Globals.Dragging)
                {
                    //Place the change, we done!
                    Globals.MapEditorWindow?.ProcessSelectionMovement(Globals.CurrentMap, true);
                    Globals.MapEditorWindow?.PlaceSelection();
                }
            }

            PacketSender.SendMap(Globals.CurrentMap);
        }

        private void NewMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (
                DarkMessageBox.ShowWarning(Strings.mapping.newmap, Strings.mapping.newmapcaption,
                    DarkDialogButton.YesNo, Properties.Resources.Icon) != DialogResult.Yes) return;
            if (Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowInformation(Strings.mapping.savemapdialogue,
                    Strings.mapping.savemap, DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                DialogResult.Yes)
            {
                SaveMap();
            }
            PacketSender.SendCreateMap(-1, Globals.CurrentMap.Index, null);
        }

        private void exportMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                Filter = "Intersect Map|*.imap",
                Title = Strings.mainform.exportmap
            };
            fileDialog.ShowDialog();
            var buff = new ByteBuffer();
            buff.WriteString(Application.ProductVersion);
            buff.WriteBytes(Globals.CurrentMap.SaveInternal());
            if (fileDialog.FileName != "")
            {
                File.WriteAllBytes(fileDialog.FileName, buff.ToArray());
            }
            buff.Dispose();
        }

        private void importMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Intersect Map|*.imap",
                Title = Strings.mainform.importmap
            };
            fileDialog.ShowDialog();

            if (fileDialog.FileName != "")
            {
                var data = File.ReadAllBytes(fileDialog.FileName);
                var buff = new ByteBuffer();
                buff.WriteBytes(data);
                if (buff.ReadString() == Application.ProductVersion)
                {
                    Globals.MapEditorWindow.PrepUndoState();
                    Globals.CurrentMap.LoadInternal(buff.ReadBytes(buff.Length(), true));
                    Globals.MapEditorWindow.AddUndoState();
                }
                else
                {
                    DarkMessageBox.ShowError(Strings.errors.importfailed,
                        Strings.errors.importfailedcaption, DarkDialogButton.Ok, Properties.Resources.Icon);
                }
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var optionsForm = new FrmOptions();
            optionsForm.ShowDialog();
            UpdateRunState();
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
            Globals.SelectionType = (int) SelectionTypes.AllLayers;
            allLayersToolStripMenuItem.Checked = true;
            currentLayerOnlyToolStripMenuItem.Checked = false;
        }

        private void currentLayerOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.SelectionType = (int) SelectionTypes.CurrentLayer;
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
            PacketSender.SendOpenEditor(GameObjectType.Item);
        }

        private void npcEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Npc);
        }

        private void spellEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Spell);
        }

        private void craftingEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Bench);
        }

        private void animationEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Animation);
        }

        private void resourceEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Resource);
        }

        private void classEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Class);
        }

        private void questEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Quest);
        }

        private void projectileEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Projectile);
        }

        private void commonEventEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.CommonEvent);
        }

        private void switchVariableEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.PlayerSwitch);
        }

        private void shopEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Shop);
        }

        private void timeEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Time);
        }

        //Help
        private void postQuestionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtonQuestion_Click(null, null);
        }

        private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButtonBug_Click(null, null);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout aboutfrm = new FrmAbout();
            aboutfrm.ShowDialog();
        }

        //ToolStrip Functions
        private void toolStripBtnNewMap_Click(object sender, EventArgs e)
        {
            NewMapToolStripMenuItem_Click(null, null);
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
                tmpMap.LoadInternal(
                    Globals.MapEditorWindow.MapUndoStates[Globals.MapEditorWindow.MapUndoStates.Count - 1]);
                Globals.MapEditorWindow.MapRedoStates.Add(Globals.MapEditorWindow.CurrentMapState);
                Globals.MapEditorWindow.CurrentMapState =
                    Globals.MapEditorWindow.MapUndoStates[Globals.MapEditorWindow.MapUndoStates.Count - 1];
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
                tmpMap.LoadInternal(
                    Globals.MapEditorWindow.MapRedoStates[Globals.MapEditorWindow.MapRedoStates.Count - 1]);
                Globals.MapEditorWindow.MapUndoStates.Add(Globals.MapEditorWindow.CurrentMapState);
                Globals.MapEditorWindow.CurrentMapState =
                    Globals.MapEditorWindow.MapRedoStates[Globals.MapEditorWindow.MapRedoStates.Count - 1];
                Globals.MapEditorWindow.MapRedoStates.RemoveAt(Globals.MapEditorWindow.MapRedoStates.Count - 1);
                Globals.MapPropertiesWindow.Update();
                EditorGraphics.TilePreviewUpdated = true;
            }
        }

        private void toolStripBtnFill_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int) EditingTool.Fill;
        }

        private void toolStripBtnErase_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int) EditingTool.Erase;
        }

        private void toolStripBtnScreenshot_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                Filter = "Png Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif",
                Title = Strings.mainform.screenshot
            };
            fileDialog.ShowDialog();

            if (fileDialog.FileName != "")
            {
                using (var fs = new FileStream(fileDialog.FileName, FileMode.OpenOrCreate))
                {
                    var screenshotTexture = EditorGraphics.ScreenShotMap();
                    screenshotTexture.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        private void toolStripBtnPen_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int) EditingTool.Pen;
        }

        private void toolStripBtnSelect_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int) EditingTool.Selection;
        }

        private void toolStripBtnRect_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int) EditingTool.Rectangle;
            Globals.CurMapSelX = 0;
            Globals.CurMapSelY = 0;
        }

        private void toolStripBtnEyeDrop_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int) EditingTool.Droppler;
            Globals.CurMapSelX = 0;
            Globals.CurMapSelY = 0;
        }

        private void toolStripBtnCopy_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentTool != (int) EditingTool.Selection)
            {
                return;
            }
            Globals.MapEditorWindow.Copy();
        }

        private void toolStripBtnPaste_Click(object sender, EventArgs e)
        {
            if (!Globals.HasCopy)
            {
                return;
            }
            Globals.MapEditorWindow.Paste();
        }

        private void toolStripBtnCut_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentTool != (int) EditingTool.Selection)
            {
                return;
            }
            Globals.MapEditorWindow.Cut();
        }

        private void toolStripTimeButton_Click(object sender, EventArgs e)
        {
        }

        private void toolStripBtnRun_Click(object sender, EventArgs e)
        {
            var path = Preferences.LoadPreference("ClientPath");
            if (path != "" && File.Exists(path))
            {
                var processStartInfo = new ProcessStartInfo(path)
                {
                    WorkingDirectory = Directory.GetParent(path).FullName
                };
                var process = Process.Start(processStartInfo);
            }
        }

        private void toolStripButtonDonate_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.freemmorpgmaker.com/donate.php");
        }

        private void toolStripButtonQuestion_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.ascensiongamedev.com/community/forum/53-questions-and-answers/");
        }

        private void toolStripButtonBug_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.ascensiongamedev.com/community/bug_tracker/intersect/");
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
            ToolStripDropDownButton btn = new ToolStripDropDownButton(Strings.general.none)
            {
                Tag = null
            };
            btn.Click += TimeDropdownButton_Click;
            toolStripTimeButton.DropDownItems.Add(btn);
            for (int i = 0; i < 1440; i += TimeBase.GetTimeBase().RangeInterval)
            {
                var addRange = time.ToString("h:mm:ss tt") + " to ";
                time = time.AddMinutes(TimeBase.GetTimeBase().RangeInterval);
                addRange += time.ToString("h:mm:ss tt");

                //Create image of overlay color
                var img = new Bitmap(16, 16);
                var g = Graphics.FromImage(img);
                g.Clear(System.Drawing.Color.Transparent);
                //Draw the trans tile if we have it
                if (transtile != null)
                {
                    g.DrawImage(transtile, new System.Drawing.Point(0, 0));
                }
                var clr = TimeBase.GetTimeBase().RangeColors[x];
                Brush brush =
                    new SolidBrush(System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));
                g.FillRectangle(brush, new Rectangle(0, 0, 32, 32));

                //Draw the overlay color
                g.Dispose();

                btn = new ToolStripDropDownButton(addRange, img)
                {
                    Tag = clr
                };
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
                EditorGraphics.LightColor = (Color) ((ToolStripDropDownButton) sender).Tag;
            }
        }

        private void UpdateRunState()
        {
            toolStripBtnRun.Enabled = false;
            var path = Preferences.LoadPreference("ClientPath");
            if (path != "" && File.Exists(path))
            {
                toolStripBtnRun.Enabled = true;
            }
        }

        //Cross Threading Delegate Methods
        private void TryOpenEditorMethod(GameObjectType type)
        {
            if (Globals.CurrentEditor == -1)
            {
                switch (type)
                {
                    case GameObjectType.Animation:
                        if (mAnimationEditor == null || mAnimationEditor.Visible == false)
                        {
                            mAnimationEditor = new FrmAnimation();
                            mAnimationEditor.InitEditor();
                            mAnimationEditor.Show();
                        }
                        break;
                    case GameObjectType.Item:
                        if (mItemEditor == null || mItemEditor.Visible == false)
                        {
                            mItemEditor = new FrmItem();
                            mItemEditor.InitEditor();
                            mItemEditor.Show();
                        }
                        break;
                    case GameObjectType.Npc:
                        if (mNpcEditor == null || mNpcEditor.Visible == false)
                        {
                            mNpcEditor = new FrmNpc();
                            mNpcEditor.InitEditor();
                            mNpcEditor.Show();
                        }
                        break;
                    case GameObjectType.Resource:
                        if (mResourceEditor == null || mResourceEditor.Visible == false)
                        {
                            mResourceEditor = new FrmResource();
                            mResourceEditor.InitEditor();
                            mResourceEditor.Show();
                        }
                        break;
                    case GameObjectType.Spell:
                        if (mSpellEditor == null || mSpellEditor.Visible == false)
                        {
                            mSpellEditor = new FrmSpell();
                            mSpellEditor.InitEditor();
                            mSpellEditor.Show();
                        }
                        break;
                    case GameObjectType.Bench:
                        if (mCraftEditor == null || mCraftEditor.Visible == false)
                        {
                            mCraftEditor = new FrmCrafting();
                            mCraftEditor.InitEditor();
                            mCraftEditor.Show();
                        }
                        break;
                    case GameObjectType.Class:
                        if (mClassEditor == null || mClassEditor.Visible == false)
                        {
                            mClassEditor = new FrmClass();
                            mClassEditor.InitEditor();
                            mClassEditor.Show();
                        }
                        break;
                    case GameObjectType.Quest:
                        if (mQuestEditor == null || mQuestEditor.Visible == false)
                        {
                            mQuestEditor = new FrmQuest();
                            mQuestEditor.InitEditor();
                            mQuestEditor.Show();
                        }
                        break;
                    case GameObjectType.Projectile:
                        if (mProjectileEditor == null || mProjectileEditor.Visible == false)
                        {
                            mProjectileEditor = new FrmProjectile();
                            mProjectileEditor.InitEditor();
                            mProjectileEditor.Show();
                        }
                        break;
                    case GameObjectType.CommonEvent:
                        if (mCommonEventEditor == null || mCommonEventEditor.Visible == false)
                        {
                            mCommonEventEditor = new FrmCommonEvent();
                            mCommonEventEditor.Show();
                        }
                        break;
                    case GameObjectType.PlayerSwitch:
                        if (mSwitchVariableEditor == null || mSwitchVariableEditor.Visible == false)
                        {
                            mSwitchVariableEditor = new FrmSwitchVariable();
                            mSwitchVariableEditor.InitEditor();
                            mSwitchVariableEditor.Show();
                        }
                        break;
                    case GameObjectType.Shop:
                        if (mShopEditor == null || mShopEditor.Visible == false)
                        {
                            mShopEditor = new FrmShop();
                            mShopEditor.InitEditor();
                            mShopEditor.Show();
                        }
                        break;
                    case GameObjectType.Time:
                        if (mTimeEditor == null || mTimeEditor.Visible == false)
                        {
                            mTimeEditor = new FrmTime();
                            mTimeEditor.InitEditor(TimeBase.GetTimeBase());
                            mTimeEditor.Show();
                        }
                        break;
                    default:
                        return;
                }
                Globals.CurrentEditor = (int) type;
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Globals.ClosingEditor && Globals.CurrentMap != null && Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowWarning(Strings.mapping.savemapdialogue, Strings.mapping.savemap,
                    DarkDialogButton.YesNo, Properties.Resources.Icon) == DialogResult.Yes)
            {
                SaveMap();
            }
            Globals.ClosingEditor = true;
        }
    }
}
