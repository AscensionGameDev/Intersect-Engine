using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DarkUI.Forms;
using Intersect;
using Intersect.GameObjects;
using Intersect.Localization;
using Intersect_Editor.Classes;
using Intersect_Editor.Classes.Core;
using Intersect_Editor.Classes.General;
using Intersect_Editor.Classes.Maps;
using Intersect_Editor.Forms.DockingElements;
using Intersect_Editor.Forms.Editors;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect_Editor.Forms
{
    public partial class frmMain : Form
    {
        public delegate void HandleDisconnect();

        //Cross Thread Delegates
        public delegate void TryOpenEditor(GameObject type);

        public delegate void UpdateTimeList();

        //Editor References
        private frmAnimation _animationEditor;
        private frmClass _classEditor;
        private frmCommonEvent _commonEventEditor;
        private frmCrafting _craftEditor;
        private FrmItem _itemEditor;
        private frmNpc _npcEditor;
        private frmProjectile _projectileEditor;
        private frmQuest _questEditor;
        private frmResource _resourceEditor;
        private frmShop _shopEditor;
        private frmSpell _spellEditor;
        private frmSwitchVariable _switchVariableEditor;
        private frmTime _timeEditor;
        //General Editting Variables
        bool _tMouseDown;
        public HandleDisconnect DisconnectDelegate;
        public TryOpenEditor EditorDelegate;
        public UpdateTimeList TimeDelegate;

        //Initialization & Setup Functions
        public frmMain()
        {
            InitializeComponent();
            dockLeft.Theme = new VS2015DarkTheme();
            Globals.MapListWindow = new frmMapList();
            Globals.MapListWindow.Show(dockLeft, DockState.DockRight);
            Globals.MapLayersWindow = new frmMapLayers();
            Globals.MapLayersWindow.Show(dockLeft, DockState.DockLeft);

            Globals.MapEditorWindow = new frmMapEditor();
            Globals.MapEditorWindow.Show(dockLeft, DockState.Document);

            Globals.MapGridWindowNew = new frmMapGrid();
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
            fileToolStripMenuItem.Text = Strings.Get("mainform", "file");
            saveMapToolStripMenuItem.Text = Strings.Get("mainform", "savemap");
            toolStripBtnSaveMap.Text = Strings.Get("mainform", "savemap");
            newMapToolStripMenuItem.Text = Strings.Get("mainform", "newmap");
            toolStripBtnNewMap.Text = Strings.Get("mainform", "newmap");
            importMapToolStripMenuItem.Text = Strings.Get("mainform", "importmap");
            exportMapToolStripMenuItem.Text = Strings.Get("mainform", "exportmap");
            optionsToolStripMenuItem.Text = Strings.Get("mainform", "options");
            exitToolStripMenuItem.Text = Strings.Get("mainform", "exit");

            editToolStripMenuItem.Text = Strings.Get("mainform", "edit");
            undoToolStripMenuItem.Text = Strings.Get("mainform", "undo");
            redoToolStripMenuItem.Text = Strings.Get("mainform", "redo");
            cutToolStripMenuItem.Text = Strings.Get("mainform", "cut");
            copyToolStripMenuItem.Text = Strings.Get("mainform", "copy");
            pasteToolStripMenuItem.Text = Strings.Get("mainform", "paste");
            toolStripBtnUndo.Text = Strings.Get("mainform", "undo");
            toolStripBtnRedo.Text = Strings.Get("mainform", "redo");
            toolStripBtnCut.Text = Strings.Get("mainform", "cut");
            toolStripBtnCopy.Text = Strings.Get("mainform", "copy");
            toolStripBtnPaste.Text = Strings.Get("mainform", "paste");

            fillToolStripMenuItem.Text = Strings.Get("mainform", "fill");
            toolStripBtnFill.Text = Strings.Get("mainform", "fill");
            eraseLayerToolStripMenuItem.Text = Strings.Get("mainform", "erase");
            toolStripBtnErase.Text = Strings.Get("mainform", "erase");

            selectToolStripMenuItem.Text = Strings.Get("mainform", "selectlayers");
            allLayersToolStripMenuItem.Text = Strings.Get("mainform", "alllayers");
            currentLayerOnlyToolStripMenuItem.Text = Strings.Get("mainform", "currentonly");

            viewToolStripMenuItem.Text = Strings.Get("mainform", "view");
            hideDarknessToolStripMenuItem.Text = Strings.Get("mainform", "darkness");
            hideFogToolStripMenuItem.Text = Strings.Get("mainform", "fog");
            hideOverlayToolStripMenuItem.Text = Strings.Get("mainform", "overlay");
            hideResourcesToolStripMenuItem.Text = Strings.Get("mainform", "resources");
            hideTilePreviewToolStripMenuItem.Text = Strings.Get("mainform", "tilepreview");
            mapGridToolStripMenuItem.Text = Strings.Get("mainform", "grid");

            contentEditorsToolStripMenuItem.Text = Strings.Get("mainform", "editors");
            animationEditorToolStripMenuItem.Text = Strings.Get("mainform", "animationeditor");
            classEditorToolStripMenuItem.Text = Strings.Get("mainform", "classeditor");
            commonEventEditorToolStripMenuItem.Text = Strings.Get("mainform", "commoneventeditor");
            craftingEditorToolStripMenuItem.Text = Strings.Get("mainform", "craftingbencheditor");
            itemEditorToolStripMenuItem.Text = Strings.Get("mainform", "itemeditor");
            npcEditorToolStripMenuItem.Text = Strings.Get("mainform", "npceditor");
            projectileEditorToolStripMenuItem.Text = Strings.Get("mainform", "projectileeditor");
            questEditorToolStripMenuItem.Text = Strings.Get("mainform", "questeditor");
            resourceEditorToolStripMenuItem.Text = Strings.Get("mainform", "resourceeditor");
            shopEditorToolStripMenuItem.Text = Strings.Get("mainform", "shopeditor");
            spellEditorToolStripMenuItem.Text = Strings.Get("mainform", "spelleditor");
            switchVariableEditorToolStripMenuItem.Text = Strings.Get("mainform", "switchvariableeditor");
            timeEditorToolStripMenuItem.Text = Strings.Get("mainform", "timeeditor");

            externalToolsToolStripMenuItem.Text = Strings.Get("mainform", "externaltools");

            helpToolStripMenuItem.Text = Strings.Get("mainform", "help");
            postQuestionToolStripMenuItem.Text = Strings.Get("mainform", "postquestion");
            toolStripButtonQuestion.Text = Strings.Get("mainform", "postquestion");
            reportBugToolStripMenuItem.Text = Strings.Get("mainform", "reportbug");
            toolStripButtonBug.Text = Strings.Get("mainform", "reportbug");
            aboutToolStripMenuItem.Text = Strings.Get("mainform", "about");
            toolStripButtonDonate.Text = Strings.Get("mainform", "donate");

            toolStripBtnPen.Text = Strings.Get("mainform", "pen");
            toolStripBtnSelect.Text = Strings.Get("mainform", "selection");
            toolStripBtnRect.Text = Strings.Get("mainform", "rectangle");
            toolStripBtnEyeDrop.Text = Strings.Get("mainform", "droppler");
            toolStripTimeButton.Text = Strings.Get("mainform", "lighting");
            toolStripBtnScreenshot.Text = Strings.Get("mainform", "screenshot");
            toolStripBtnRun.Text = Strings.Get("mainform", "run");
        }

        private void InitExternalTools()
        {
            var foundTools = false;
            if (Directory.Exists(Strings.Get("mainform", "toolsdir")))
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
            if (!String.IsNullOrEmpty((string) ((ToolStripItem) sender).Tag))
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
            UpdateRunState();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }

        public void EnterMap(int mapNum)
        {
            Globals.CurrentMap = MapInstance.Lookup.Get(mapNum);
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
                toolStripLabelCoords.Text = Strings.Get("mainform", "loc", Globals.CurTileX, Globals.CurTileY);
                toolStripLabelRevision.Text = Strings.Get("mainform", "revision", Globals.CurrentMap.Revision);
                if (Text != Strings.Get("mainform", "title", Globals.CurrentMap.Name))
                {
                    Text = Strings.Get("mainform", "title", Globals.CurrentMap.Name);
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
                Globals.CurrentTool = (int) EdittingTool.Selection;
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 2) //Events
            {
                Globals.CurrentTool = (int) EdittingTool.Selection;
            }
            else if (Globals.CurrentLayer == Options.LayerCount + 3) //NPCS
            {
                Globals.CurrentTool = (int) EdittingTool.Selection;
            }
            else
            {
                toolStripBtnPen.Enabled = true;
                toolStripBtnRect.Enabled = true;
                toolStripBtnEyeDrop.Enabled = true;
            }

            switch (Globals.CurrentTool)
            {
                case (int) EdittingTool.Pen:
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
                case (int) EdittingTool.Selection:
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
                case (int) EdittingTool.Rectangle:
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
                case (int) EdittingTool.Droppler:
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
                        DarkMessageBox.ShowError(Strings.Get("errors", "disconnectedsave"),
                            Strings.Get("errors", "disconnectedsavecaption"), DarkDialogButton.YesNo,
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
                    DarkMessageBox.ShowError(Strings.Get("errors", "disconnectedclosing"),
                        Strings.Get("errors", "disconnected"), DarkDialogButton.Ok, Properties.Resources.Icon);
                    Application.Exit();
                }
            }
        }

        //MenuBar Functions -- File
        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowInformation(Strings.Get("mapping", "savemapdialoguesure"),
                    Strings.Get("mapping", "savemap"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                DialogResult.Yes)
            {
                SaveMap();
            }
        }

        private void SaveMap()
        {
            if (Globals.CurrentTool == (int) EdittingTool.Selection)
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

        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (
                DarkMessageBox.ShowWarning(Strings.Get("mapping", "newmap"), Strings.Get("mapping", "newmapcaption"),
                    DarkDialogButton.YesNo, Properties.Resources.Icon) != DialogResult.Yes) return;
            if (Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowInformation(Strings.Get("mapping", "savemapdialogue"),
                    Strings.Get("mapping", "savemap"), DarkDialogButton.YesNo, Properties.Resources.Icon) ==
                DialogResult.Yes)
            {
                SaveMap();
            }
            PacketSender.SendCreateMap(-1, ((DatabaseObject) Globals.CurrentMap).Id, null);
        }

        private void exportMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                Filter = "Intersect Map|*.imap",
                Title = Strings.Get("mainform", "exportmap")
            };
            fileDialog.ShowDialog();
            var buff = new ByteBuffer();
            buff.WriteString(Application.ProductVersion);
            buff.WriteBytes(Globals.CurrentMap.GetMapData(false));
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
                Title = Strings.Get("mainform", "importmap")
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
                    Globals.CurrentMap.Load(buff.ReadBytes(buff.Length(), true));
                    Globals.MapEditorWindow.AddUndoState();
                }
                else
                {
                    DarkMessageBox.ShowError(Strings.Get("errors", "importfailed"),
                        Strings.Get("errors", "importfailedcaption"), DarkDialogButton.Ok, Properties.Resources.Icon);
                }
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var optionsForm = new frmOptions();
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
            PacketSender.SendOpenEditor(GameObject.Quest);
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
                tmpMap.Load(Globals.MapEditorWindow.MapRedoStates[Globals.MapEditorWindow.MapRedoStates.Count - 1]);
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
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                Filter = "Png Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif",
                Title = Strings.Get("mainform", "screenshot")
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
            Globals.CurrentTool = (int) EdittingTool.Pen;
        }

        private void toolStripBtnSelect_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int) EdittingTool.Selection;
        }

        private void toolStripBtnRect_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int) EdittingTool.Rectangle;
            Globals.CurMapSelX = 0;
            Globals.CurMapSelY = 0;
        }

        private void toolStripBtnEyeDrop_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = (int) EdittingTool.Droppler;
            Globals.CurMapSelX = 0;
            Globals.CurMapSelY = 0;
        }

        private void toolStripBtnCopy_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentTool != (int) EdittingTool.Selection)
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
            if (Globals.CurrentTool != (int) EdittingTool.Selection)
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
            ToolStripDropDownButton btn = new ToolStripDropDownButton(Strings.Get("general", "none"))
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
                EditorGraphics.LightColor = (Intersect.Color) ((ToolStripDropDownButton) sender).Tag;
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
        private void TryOpenEditorMethod(GameObject type)
        {
            if (Globals.CurrentEditor == -1)
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
                Globals.CurrentEditor = (int) type;
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Globals.ClosingEditor && Globals.CurrentMap != null && Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowWarning(Strings.Get("mapping", "savemapdialogue"), Strings.Get("mapping", "savemap"),
                    DarkDialogButton.YesNo, Properties.Resources.Icon) == DialogResult.Yes)
            {
                SaveMap();
            }
            Globals.ClosingEditor = true;
        }
    }
}