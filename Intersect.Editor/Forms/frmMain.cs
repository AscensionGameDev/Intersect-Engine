using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;

using DarkUI.Controls;
using DarkUI.Forms;
using Intersect.Compression;
using Intersect.Config;
using Intersect.Editor.Classes.ContentManagement;
using Intersect.Editor.Content;
using Intersect.Editor.Forms.DockingElements;
using Intersect.Editor.Forms.Editors;
using Intersect.Editor.Forms.Editors.Quest;
using Intersect.Editor.General;
using Intersect.Editor.Localization;
using Intersect.Editor.Maps;
using Intersect.Editor.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Localization;
using Intersect.Network;
using Intersect.Updater;
using Intersect.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using WeifenLuo.WinFormsUI.Docking;

namespace Intersect.Editor.Forms
{

    public partial class FrmMain : Form
    {

        public delegate void HandleDisconnect();

        //Cross Thread Delegates
        public delegate void TryOpenEditor(GameObjectType type);

        public delegate void UpdateTimeList();

        public HandleDisconnect DisconnectDelegate;

        public TryOpenEditor EditorDelegate;

        //Editor References
        private FrmAnimation mAnimationEditor;

        private FrmClass mClassEditor;

        private FrmCommonEvent mCommonEventEditor;

        private FrmCraftingTables mCraftingTablesEditor;

        private FrmCrafts mCraftsEditor;

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

        public UpdateTimeList TimeDelegate;

        //Initialization & Setup Functions
        public FrmMain()
        {
            InitializeComponent();
            Icon = Program.Icon;

            dockLeft.Theme = new VS2015DarkTheme();
            Globals.MapListWindow = new FrmMapList();
            Globals.MapLayersWindow = new FrmMapLayers();
            Globals.MapGridWindowNew = new FrmMapGrid();
            Globals.MapEditorWindow = new FrmMapEditor();

            Globals.MapListWindow.Show(dockLeft, DockState.DockRight);
            Globals.MapLayersWindow.Show(dockLeft, DockState.DockLeft);
            Globals.MapGridWindowNew.Show(dockLeft, DockState.Document);
            Globals.MapEditorWindow.Show(dockLeft, DockState.Document);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(System.Drawing.Point pnt);

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

            WindowState = FormWindowState.Maximized;
        }

        private void InitLocalization()
        {
            InitLocalizationMenus();
            InitLocalizationToolstrip();
        }

        private void InitLocalizationMenus()
        {
            InitLocalizationMenuFile();
            InitLocalizationMenuEdit();
            InitLocalizationMenuView();
            InitLocalizationMenuGameEditors();
            InitLocalizationMenuTools();
            InitLocalizationMenuHelp();
        }

        private void InitLocalizationMenuFile()
        {
            fileToolStripMenuItem.Text = Strings.MainForm.file;
            saveMapToolStripMenuItem.Text = Strings.MainForm.SaveMap;
            newMapToolStripMenuItem.Text = Strings.MainForm.newmap;
            importMapToolStripMenuItem.Text = Strings.MainForm.importmap;
            exportMapToolStripMenuItem.Text = Strings.MainForm.exportmap;
            optionsToolStripMenuItem.Text = Strings.MainForm.options;
            exitToolStripMenuItem.Text = Strings.MainForm.exit;
        }

        private void InitLocalizationMenuEdit()
        {
            editToolStripMenuItem.Text = Strings.MainForm.edit;
            undoToolStripMenuItem.Text = Strings.MainForm.Undo;
            redoToolStripMenuItem.Text = Strings.MainForm.Redo;
            cutToolStripMenuItem.Text = Strings.MainForm.Cut;
            copyToolStripMenuItem.Text = Strings.MainForm.Copy;
            pasteToolStripMenuItem.Text = Strings.MainForm.Paste;
            fillToolStripMenuItem.Text = Strings.MainForm.Fill;
            eraseLayerToolStripMenuItem.Text = Strings.MainForm.Erase;
            selectToolStripMenuItem.Text = Strings.MainForm.selectlayers;
            allLayersToolStripMenuItem.Text = Strings.MainForm.alllayers;
            currentLayerOnlyToolStripMenuItem.Text = Strings.MainForm.currentonly;
        }

        private void InitLocalizationMenuView()
        {
            viewToolStripMenuItem.Text = Strings.MainForm.view;
            hideDarknessToolStripMenuItem.Text = Strings.MainForm.darkness;
            hideFogToolStripMenuItem.Text = Strings.MainForm.fog;
            hideOverlayToolStripMenuItem.Text = Strings.MainForm.overlay;
            hideResourcesToolStripMenuItem.Text = Strings.MainForm.resources;
            hideEventsToolStripMenuItem.Text = Strings.MainForm.Events;
            hideTilePreviewToolStripMenuItem.Text = Strings.MainForm.tilepreview;
            mapGridToolStripMenuItem.Text = Strings.MainForm.grid;
        }

        private void InitLocalizationMenuGameEditors()
        {
            contentEditorsToolStripMenuItem.Text = Strings.MainForm.editors;
            animationEditorToolStripMenuItem.Text = Strings.MainForm.animationeditor;
            classEditorToolStripMenuItem.Text = Strings.MainForm.classeditor;
            commonEventEditorToolStripMenuItem.Text = Strings.MainForm.commoneventeditor;
            craftingTableEditorToolStripMenuItem.Text = Strings.MainForm.craftingtableeditor;
            craftsEditorToolStripMenuItem.Text = Strings.MainForm.craftingeditor;
            itemEditorToolStripMenuItem.Text = Strings.MainForm.itemeditor;
            npcEditorToolStripMenuItem.Text = Strings.MainForm.npceditor;
            projectileEditorToolStripMenuItem.Text = Strings.MainForm.projectileeditor;
            questEditorToolStripMenuItem.Text = Strings.MainForm.questeditor;
            resourceEditorToolStripMenuItem.Text = Strings.MainForm.resourceeditor;
            shopEditorToolStripMenuItem.Text = Strings.MainForm.shopeditor;
            spellEditorToolStripMenuItem.Text = Strings.MainForm.spelleditor;
            variableEditorToolStripMenuItem.Text = Strings.MainForm.variableeditor;
            timeEditorToolStripMenuItem.Text = Strings.MainForm.timeeditor;
        }

        private void InitLocalizationMenuTools()
        {
            toolsToolStripMenuItem.Text = Strings.MainForm.tools;
            packageUpdateToolStripMenuItem.Text = Strings.MainForm.MenuToolsPackageUpdate;
        }

        private void InitLocalizationMenuHelp()
        {
            helpToolStripMenuItem.Text = Strings.MainForm.help;
            postQuestionToolStripMenuItem.Text = Strings.MainForm.postquestion;
            toolStripButtonQuestion.Text = Strings.MainForm.postquestion;
            reportBugToolStripMenuItem.Text = Strings.MainForm.reportbug;
            toolStripButtonBug.Text = Strings.MainForm.reportbug;
            aboutToolStripMenuItem.Text = Strings.MainForm.about;
        }

        private void InitLocalizationToolstrip()
        {
            toolStripBtnNewMap.Text = Strings.MainForm.newmap;
            toolStripBtnSaveMap.Text = Strings.MainForm.SaveMap;

            toolStripBtnCut.Text = Strings.MainForm.Cut;
            toolStripBtnCopy.Text = Strings.MainForm.Copy;
            toolStripBtnPaste.Text = Strings.MainForm.Paste;

            toolStripBtnUndo.Text = Strings.MainForm.Undo;
            toolStripBtnRedo.Text = Strings.MainForm.Redo;

            toolStripBtnBrush.Text = Strings.MainForm.Brush;
            toolStripBtnSelect.Text = Strings.MainForm.Selection;
            toolStripBtnRect.Text = Strings.MainForm.Rectangle;

            toolStripBtnFlipVertical.Text = Strings.MainForm.FlipVertical;
            toolStripBtnFlipHorizontal.Text = Strings.MainForm.FlipHorizontal;

            toolStripBtnFill.Text = Strings.MainForm.Fill;
            toolStripBtnErase.Text = Strings.MainForm.Erase;
            toolStripBtnDropper.Text = Strings.MainForm.Dropper;

            toolStripTimeButton.Text = Strings.MainForm.lighting;

            toolStripBtnScreenshot.Text = Strings.MainForm.screenshot;

            toolStripBtnRun.Text = Strings.MainForm.run;
        }

        private void InitExternalTools()
        {
            var foundTools = false;
            if (Directory.Exists(Strings.MainForm.toolsdir))
            {
                var childDirs = Directory.GetDirectories("tools");
                for (var i = 0; i < childDirs.Length; i++)
                {
                    var executables = Directory.GetFiles(childDirs[i], "*.exe");
                    for (var x = 0; x < executables.Length; x++)
                    {
                        var item = toolsToolStripMenuItem.DropDownItems.Add(
                            executables[x]
                                .Replace(childDirs[i], "")
                                .Replace(".exe", "")
                                .Replace(Path.DirectorySeparatorChar.ToString(), "")
                        );

                        item.Tag = executables[x];
                        item.Click += externalToolItem_Click;
                    }
                }
            }
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
            switch (e.KeyData)
            {
                case Keys.Control | Keys.Z:
                    toolStripBtnUndo_Click(null, null);
                    return;

                case Keys.Control | Keys.Y:
                    toolStripBtnRedo_Click(null, null);
                    return;

                case Keys.Control | Keys.X:
                    toolStripBtnCut_Click(null, null);
                    return;

                case Keys.Control | Keys.C:
                    toolStripBtnCopy_Click(null, null);
                    return;

                case Keys.Control | Keys.V:
                    toolStripBtnPaste_Click(null, null);
                    return;

                case Keys.Control | Keys.S:
                    toolStripBtnSaveMap_Click(null, null);
                    return;
            }

            var xDiff = 0;
            var yDiff = 0;
            if (dockLeft.ActiveContent == Globals.MapEditorWindow ||
                dockLeft.ActiveContent == null &&
                Globals.MapEditorWindow.DockPanel.ActiveDocument == Globals.MapEditorWindow)
            {
                switch (e.KeyCode)
                {
                    // Shortcuts: Map grid scrolling.
                    case Keys.W:
                    case Keys.Up:
                        yDiff -= 20;
                        break;

                    case Keys.S:
                    case Keys.Down:
                        yDiff += 20;
                        break;

                    case Keys.A:
                    case Keys.Left:
                        xDiff -= 20;
                        break;

                    case Keys.D:
                    case Keys.Right:
                        xDiff += 20;
                        break;

                    // Shortcuts: Map grid Tools.
                    case Keys.B: // Brush.
                        toolStripBtnBrush_Click(null, null);
                        break;

                    case Keys.M: // Marquee Selection.
                        toolStripBtnSelect_Click(null, null);
                        break;

                    case Keys.R: // Rectangle.
                        toolStripBtnRect_Click(null, null);
                        break;

                    case Keys.PageUp: // Vertical Flip Selection.
                        toolStripBtnFlipVertical_Click(null, null);
                        break;

                    case Keys.PageDown: // Horizontal Flip Selection.
                        toolStripBtnFlipHorizontal_Click(null, null);
                        break;

                    case Keys.F: // Fill Tool.
                        toolStripBtnFill_Click(null, null);
                        break;

                    case Keys.E: // Erase.
                        toolStripBtnErase_Click(null, null);
                        break;

                    case Keys.I: // Dropper Tool.
                        toolStripBtnDropper_Click(null, null);
                        break;

                    case Keys.Delete: // Delete Selection.
                        ToolKeyDelete();
                        break;
                }

                if (xDiff != 0 || yDiff != 0)
                {
                    var hWnd = WindowFromPoint(MousePosition);
                    if (hWnd != IntPtr.Zero)
                    {
                        var ctl = FromHandle(hWnd);
                        if (ctl != null)
                        {
                            if (ctl is ComboBox || ctl is DarkComboBox)
                            {
                                return;
                            }
                        }
                    }

                    Core.Graphics.CurrentView.X -= xDiff;
                    Core.Graphics.CurrentView.Y -= yDiff;
                    if (Core.Graphics.CurrentView.X > Options.MapWidth * Options.TileWidth)
                    {
                        Core.Graphics.CurrentView.X = Options.MapWidth * Options.TileWidth;
                    }

                    if (Core.Graphics.CurrentView.Y > Options.MapHeight * Options.TileHeight)
                    {
                        Core.Graphics.CurrentView.Y = Options.MapHeight * Options.TileHeight;
                    }

                    if (Core.Graphics.CurrentView.X - Globals.MapEditorWindow.picMap.Width <
                        -Options.TileWidth * Options.MapWidth * 2)
                    {
                        Core.Graphics.CurrentView.X = -Options.TileWidth * Options.MapWidth * 2 +
                                                      Globals.MapEditorWindow.picMap.Width;
                    }

                    if (Core.Graphics.CurrentView.Y - Globals.MapEditorWindow.picMap.Height <
                        -Options.TileHeight * Options.MapHeight * 2)
                    {
                        Core.Graphics.CurrentView.Y = -Options.TileHeight * Options.MapHeight * 2 +
                                                      Globals.MapEditorWindow.picMap.Height;
                    }
                }
            }
        }

        private void InitMapProperties()
        {
            var unhiddenPane = dockLeft.Panes[0];
            Globals.MapPropertiesWindow = new FrmMapProperties();
            Globals.MapPropertiesWindow.Show(unhiddenPane, DockAlignment.Bottom, .4);
            Globals.MapPropertiesWindow.Init(Globals.CurrentMap);
            Globals.MapEditorWindow.DockPanel.Focus();
        }

        private void InitEditor()
        {
            Core.Graphics.InitMonogame();
            Globals.MapLayersWindow.InitTilesets();
            Globals.MapLayersWindow.Init();
            Globals.InEditor = true;
            GrabMouseDownEvents();
            UpdateRunState();

            //Init layer visibility buttons
            foreach (var layer in Options.Instance.MapOpts.Layers.All)
            {
                Strings.Tiles.maplayers.TryGetValue(layer.ToLower(), out LocalizedString layerName);
                if (layerName == null) layerName = layer;
                var btn = new ToolStripMenuItem(layerName);
                btn.Checked = true;
                btn.Click += HideLayerBtn_Click;
                btn.Tag = layer;
                layersToolStripMenuItem.DropDownItems.Add(btn);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Networking.Network.EditorLidgrenNetwork?.Disconnect(NetworkStatus.Quitting.ToString());
            base.OnClosed(e);
            Application.Exit();
        }

        public void ShowDialogForm(Form form)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker) delegate { ShowDialogForm(form); });

                return;
            }

            form.ShowDialog(this);
        }

        public void EnterMap(Guid mapId, bool userEntered = false)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker) delegate { EnterMap(mapId, userEntered); });

                return;
            }

            Globals.CurrentMap = MapInstance.Get(mapId);
            Globals.LoadingMap = mapId;
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

            Globals.MapEditorWindow.UnloadMap();
            PacketSender.SendEnterMap(mapId);
            PacketSender.SendNeedMap(mapId);
            PacketSender.SendNeedGrid(mapId);
            Core.Graphics.TilePreviewUpdated = true;

            // Save that we've opened this map last if this was a user triggered action. This way we can load it again should we restart the editor.
            if (userEntered)
            {
                Preferences.SavePreference("LastMapOpened", mapId.ToString());
            }
        }

        private void GrabMouseDownEvents()
        {
            GrabMouseDownEvents(this);
        }

        private void GrabMouseDownEvents(Control e)
        {
            foreach (Control t in e.Controls)
            {
                if (t is MenuStrip menuStrip)
                {
                    foreach (ToolStripMenuItem t1 in menuStrip.Items)
                    {
                        t1.MouseDown += MouseDownHandler;
                    }

                    t.MouseDown += MouseDownHandler;
                }
                else if (t is PropertyGrid)
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
            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.None)
            {
                return;
            }

            if (sender != Globals.MapEditorWindow &&
                sender != Globals.MapEditorWindow.pnlMapContainer &&
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
                toolStripLabelCoords.Text = Strings.MainForm.loc.ToString(Globals.CurTileX, Globals.CurTileY);
                toolStripLabelRevision.Text = Strings.MainForm.revision.ToString(Globals.CurrentMap.Revision);
                if (Text != Strings.MainForm.title.ToString(Globals.CurrentMap.Name))
                {
                    Text = Strings.MainForm.title.ToString(Globals.CurrentMap.Name);
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

            //Process the Fill/Erase Buttons, these should display for all valid map layers as well as Attributes.
            if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer) || Globals.CurrentLayer == LayerOptions.Attributes)
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
            toolStripBtnBrush.Enabled = false;
            toolStripBtnSelect.Enabled = true;
            toolStripBtnRect.Enabled = false;
            toolStripBtnDropper.Enabled = false;
            if (Globals.CurrentLayer == LayerOptions.Attributes)
            {
                toolStripBtnBrush.Enabled = true;
                toolStripBtnRect.Enabled = true;
            }
            else if (Globals.CurrentLayer == LayerOptions.Lights)
            {
                Globals.CurrentTool = EditingTool.Selection;
            }
            else if (Globals.CurrentLayer == LayerOptions.Events)
            {
                Globals.CurrentTool = EditingTool.Selection;
            }
            else if (Globals.CurrentLayer == LayerOptions.Npcs)
            {
                Globals.CurrentTool = EditingTool.Selection;
            }
            else
            {
                toolStripBtnBrush.Enabled = true;
                toolStripBtnRect.Enabled = true;
                toolStripBtnDropper.Enabled = true;
            }

            switch (Globals.CurrentTool)
            {
                case EditingTool.Brush:
                    if (!toolStripBtnBrush.Checked)
                    {
                        toolStripBtnBrush.Checked = true;
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

                    if (toolStripBtnDropper.Checked)
                    {
                        toolStripBtnDropper.Checked = false;
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
                case EditingTool.Selection:
                    if (toolStripBtnBrush.Checked)
                    {
                        toolStripBtnBrush.Checked = false;
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

                    if (toolStripBtnDropper.Checked)
                    {
                        toolStripBtnDropper.Checked = false;
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
                case EditingTool.Rectangle:
                    if (toolStripBtnBrush.Checked)
                    {
                        toolStripBtnBrush.Checked = false;
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

                    if (toolStripBtnDropper.Checked)
                    {
                        toolStripBtnDropper.Checked = false;
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
                case EditingTool.Fill:
                    if (toolStripBtnBrush.Checked)
                    {
                        toolStripBtnBrush.Checked = false;
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

                    if (toolStripBtnDropper.Checked)
                    {
                        toolStripBtnDropper.Checked = false;
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
                case EditingTool.Erase:
                    if (toolStripBtnBrush.Checked)
                    {
                        toolStripBtnBrush.Checked = false;
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

                    if (toolStripBtnDropper.Checked)
                    {
                        toolStripBtnDropper.Checked = false;
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
                case EditingTool.Dropper:
                    if (toolStripBtnBrush.Checked)
                    {
                        toolStripBtnBrush.Checked = false;
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

                    if (!toolStripBtnDropper.Checked)
                    {
                        toolStripBtnDropper.Checked = true;
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
                if (Globals.MainForm.ActiveControl is DockPane dockPane)
                {
                    var ctrl = dockPane.ActiveControl;
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
                    if (DarkMessageBox.ShowError(
                            Strings.Errors.disconnectedsave, Strings.Errors.disconnectedsavecaption,
                            DarkDialogButton.YesNo, Icon
                        ) ==
                        DialogResult.Yes)
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
                    DarkMessageBox.ShowError(
                        Strings.Errors.disconnectedclosing, Strings.Errors.disconnected, DarkDialogButton.Ok,
                        Icon
                    );

                    Application.Exit();
                }
            }
        }

        //MenuBar Functions -- File
        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowInformation(
                    Strings.Mapping.savemapdialoguesure, Strings.Mapping.savemap, DarkDialogButton.YesNo,
                    Icon
                ) ==
                DialogResult.Yes)
            {
                SaveMap();
            }
        }

        private static void SaveMap()
        {
            if (Globals.CurrentTool == EditingTool.Selection)
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
            if (DarkMessageBox.ShowWarning(
                    Strings.Mapping.newmap, Strings.Mapping.newmapcaption, DarkDialogButton.YesNo,
                    Icon
                ) !=
                DialogResult.Yes)
            {
                return;
            }

            if (Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowInformation(
                    Strings.Mapping.savemapdialogue, Strings.Mapping.savemap, DarkDialogButton.YesNo,
                    Icon
                ) ==
                DialogResult.Yes)
            {
                SaveMap();
            }

            PacketSender.SendCreateMap(-1, Globals.CurrentMap.Id, null);
        }

        private void exportMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDialog = new SaveFileDialog()
            {
                Filter = "Intersect Map|*.imap",
                Title = Strings.MainForm.exportmap
            };

            //TODO Reimplement
            //fileDialog.ShowDialog();
            //var buff = new ByteBuffer();
            //buff.WriteString(Application.ProductVersion);
            //buff.WriteBytes(Globals.CurrentMap.SaveInternal());
            //if (fileDialog.FileName != "")
            //{
            //    File.WriteAllBytes(fileDialog.FileName, buff.ToArray());
            //}
            //buff.Dispose();
        }

        private void importMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "Intersect Map|*.imap",
                Title = Strings.MainForm.importmap
            };

            //TODO Reimplement
            //fileDialog.ShowDialog();

            //if (fileDialog.FileName != "")
            //{
            //    var data = File.ReadAllBytes(fileDialog.FileName);
            //    var buff = new ByteBuffer();
            //    buff.WriteBytes(data);
            //    if (buff.ReadString() == Application.ProductVersion)
            //    {
            //        Globals.MapEditorWindow.PrepUndoState();
            //        Globals.CurrentMap.LoadInternal(buff.ReadBytes(buff.Length(), true));
            //        Globals.MapEditorWindow.AddUndoState();
            //    }
            //    else
            //    {
            //        DarkMessageBox.ShowError(Strings.Errors.importfailed,
            //            Strings.Errors.importfailedcaption, DarkDialogButton.Ok, Icon);
            //    }
            //}
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
            if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer))
            {
                Globals.MapEditorWindow.FillLayer();
            }
        }

        private void eraseLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Options.Instance.MapOpts.Layers.All.Contains(Globals.CurrentLayer))
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
            Core.Graphics.HideDarkness = !Core.Graphics.HideDarkness;
            hideDarknessToolStripMenuItem.Checked = !Core.Graphics.HideDarkness;
        }

        private void hideFogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.Graphics.HideFog = !Core.Graphics.HideFog;
            hideFogToolStripMenuItem.Checked = !Core.Graphics.HideFog;
        }

        private void hideOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.Graphics.HideOverlay = !Core.Graphics.HideOverlay;
            hideOverlayToolStripMenuItem.Checked = !Core.Graphics.HideOverlay;
        }

        private void hideTilePreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.Graphics.HideTilePreview = !Core.Graphics.HideTilePreview;
            hideTilePreviewToolStripMenuItem.Checked = !Core.Graphics.HideTilePreview;
        }

        private void hideResourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.Graphics.HideResources = !Core.Graphics.HideResources;
            hideResourcesToolStripMenuItem.Checked = !Core.Graphics.HideResources;
        }

        private void hideEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.Graphics.HideEvents = !Core.Graphics.HideEvents;
            hideEventsToolStripMenuItem.Checked = !Core.Graphics.HideEvents;
        }

        private void mapGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.Graphics.HideGrid = !Core.Graphics.HideGrid;
            mapGridToolStripMenuItem.Checked = !Core.Graphics.HideGrid;
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

        private void craftingTablesEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.CraftTables);
        }

        private void craftsEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Crafts);
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
            PacketSender.SendOpenEditor(GameObjectType.Event);
        }

        private void variableEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.PlayerVariable);
        }

        private void shopEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Shop);
        }

        private void timeEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PacketSender.SendOpenEditor(GameObjectType.Time);
        }

        private void layersToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
        {
            foreach (var itm in ((ToolStripMenuItem)sender).DropDownItems)
            {
                var btn = (ToolStripMenuItem)itm;
                btn.Checked = Globals.MapLayersWindow.LayerVisibility[(string)btn.Tag];
            }
        }

        private void HideLayerBtn_Click(object sender, EventArgs e)
        {
            var btn = ((ToolStripMenuItem)sender);
            var tag = (string)btn.Tag;
            btn.Checked = !btn.Checked;
            Globals.MapLayersWindow.LayerVisibility[tag] = btn.Checked;
            Globals.MapLayersWindow.SetLayer(Globals.CurrentLayer);
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
            var aboutfrm = new FrmAbout();
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
                    Globals.MapEditorWindow.MapUndoStates[Globals.MapEditorWindow.MapUndoStates.Count - 1]
                );

                Globals.MapEditorWindow.MapRedoStates.Add(Globals.MapEditorWindow.CurrentMapState);
                Globals.MapEditorWindow.CurrentMapState =
                    Globals.MapEditorWindow.MapUndoStates[Globals.MapEditorWindow.MapUndoStates.Count - 1];

                Globals.MapEditorWindow.MapUndoStates.RemoveAt(Globals.MapEditorWindow.MapUndoStates.Count - 1);
                Globals.MapPropertiesWindow.Update();
                Core.Graphics.TilePreviewUpdated = true;
            }
        }

        private void toolStripBtnRedo_Click(object sender, EventArgs e)
        {
            var tmpMap = Globals.CurrentMap;
            if (Globals.MapEditorWindow.MapRedoStates.Count > 0)
            {
                tmpMap.LoadInternal(
                    Globals.MapEditorWindow.MapRedoStates[Globals.MapEditorWindow.MapRedoStates.Count - 1]
                );

                Globals.MapEditorWindow.MapUndoStates.Add(Globals.MapEditorWindow.CurrentMapState);
                Globals.MapEditorWindow.CurrentMapState =
                    Globals.MapEditorWindow.MapRedoStates[Globals.MapEditorWindow.MapRedoStates.Count - 1];

                Globals.MapEditorWindow.MapRedoStates.RemoveAt(Globals.MapEditorWindow.MapRedoStates.Count - 1);
                Globals.MapPropertiesWindow.Update();
                Core.Graphics.TilePreviewUpdated = true;
            }
        }

        private void toolStripBtnFill_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = EditingTool.Fill;
        }

        private void toolStripBtnErase_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = EditingTool.Erase;
        }

        private void toolStripBtnScreenshot_Click(object sender, EventArgs e)
        {
            var fileDialog = new SaveFileDialog()
            {
                Filter = "Png Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif",
                Title = Strings.MainForm.screenshot
            };

            fileDialog.ShowDialog();

            if (fileDialog.FileName != "")
            {
                using (var fs = new FileStream(fileDialog.FileName, FileMode.OpenOrCreate))
                {
                    var screenshotTexture = Core.Graphics.ScreenShotMap();
                    screenshotTexture.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        private void toolStripBtnBrush_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = EditingTool.Brush;
        }

        private void toolStripBtnSelect_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = EditingTool.Selection;
            Globals.CurMapSelX = 0;
            Globals.CurMapSelY = 0;
            Globals.CurMapSelW = 0;
            Globals.CurMapSelH = 0;
        }

        private void toolStripBtnRect_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = EditingTool.Rectangle;
            Globals.CurMapSelX = 0;
            Globals.CurMapSelY = 0;
            Globals.CurMapSelW = 0;
            Globals.CurMapSelH = 0;
        }

        private void toolStripBtnDropper_Click(object sender, EventArgs e)
        {
            Globals.CurrentTool = EditingTool.Dropper;
            Globals.CurMapSelX = 0;
            Globals.CurMapSelY = 0;
        }

        private void toolStripBtnCopy_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentTool != EditingTool.Selection)
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
            if (Globals.CurrentTool != EditingTool.Selection)
            {
                return;
            }

            Globals.MapEditorWindow.Cut();
        }

        private static void ToolKeyDelete()
        {
            if (Globals.CurrentTool != EditingTool.Selection)
            {
                return;
            }

            Globals.MapEditorWindow.Delete();
        }

        private void toolStripTimeButton_Click(object sender, EventArgs e)
        {
        }

        private void toolStripBtnRun_Click(object sender, EventArgs e)
        {
            var path = Preferences.LoadPreference("ClientPath");
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                var processStartInfo = new ProcessStartInfo(path)
                {
                    WorkingDirectory = Directory.GetParent(path).FullName
                };

                _ = Process.Start(processStartInfo);
            }
        }

        private void toolStripButtonQuestion_Click(object sender, EventArgs e)
        {
            BrowserUtils.Open("https://www.ascensiongamedev.com/community/forum/53-questions-and-answers/");
        }

        private void toolStripButtonBug_Click(object sender, EventArgs e)
        {
            BrowserUtils.Open("https://github.com/AscensionGameDev/Intersect-Engine/issues/new/choose");
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
            var btn = new ToolStripDropDownButton(Strings.General.None)
            {
                Tag = null
            };

            btn.Click += TimeDropdownButton_Click;
            toolStripTimeButton.DropDownItems.Add(btn);
            for (var i = 0; i < 1440; i += TimeBase.GetTimeBase().RangeInterval)
            {
                var addRange = time.ToString("h:mm:ss tt") + " to ";
                time = time.AddMinutes(TimeBase.GetTimeBase().RangeInterval);
                addRange += time.ToString("h:mm:ss tt");

                //Create image of overlay color
                var img = new Bitmap(16, 16);
                var g = System.Drawing.Graphics.FromImage(img);
                g.Clear(System.Drawing.Color.Transparent);

                //Draw the trans tile if we have it
                if (transtile != null)
                {
                    g.DrawImage(transtile, new System.Drawing.Point(0, 0));
                }

                var clr = TimeBase.GetTimeBase().DaylightHues[x];
                Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B));
                g.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, 32, 32));

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

            if (transtile != null)
            {
                transtile.Dispose();
            }
        }

        private void TimeDropdownButton_Click(object sender, EventArgs e)
        {
            if (((ToolStripDropDownButton) sender).Tag == null)
            {
                Core.Graphics.LightColor = null;
            }
            else
            {
                Core.Graphics.LightColor = (Color) ((ToolStripDropDownButton) sender).Tag;
            }
        }

        private void UpdateRunState()
        {
            toolStripBtnRun.Enabled = false;
            var path = Preferences.LoadPreference("ClientPath");
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
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
                    case GameObjectType.CraftTables:
                        if (mCraftingTablesEditor == null || mCraftingTablesEditor.Visible == false)
                        {
                            mCraftingTablesEditor = new FrmCraftingTables();
                            mCraftingTablesEditor.InitEditor();
                            mCraftingTablesEditor.Show();
                        }

                        break;
                    case GameObjectType.Crafts:
                        if (mCraftsEditor == null || mCraftsEditor.Visible == false)
                        {
                            mCraftsEditor = new FrmCrafts();
                            mCraftsEditor.InitEditor();
                            mCraftsEditor.Show();
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
                    case GameObjectType.Event:
                        if (mCommonEventEditor == null || mCommonEventEditor.Visible == false)
                        {
                            mCommonEventEditor = new FrmCommonEvent();
                            mCommonEventEditor.Show();
                        }

                        break;
                    case GameObjectType.PlayerVariable:
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
            if (!Globals.ClosingEditor &&
                Globals.CurrentMap != null &&
                Globals.CurrentMap.Changed() &&
                DarkMessageBox.ShowWarning(
                    Strings.Mapping.maphaschangesdialog, Strings.Mapping.mapnotsaved, DarkDialogButton.YesNo,
                    Icon
                ) ==
                DialogResult.No)
            {
                e.Cancel = true;

                return;
            }

            Globals.ClosingEditor = true;
        }

        private void toolStripBtnFlipVertical_Click(object sender, EventArgs e)
        {
            Globals.MapEditorWindow.FlipVertical();
        }

        private void toolStripBtnFlipHorizontal_Click(object sender, EventArgs e)
        {
            Globals.MapEditorWindow.FlipHorizontal();
        }

        private void packClientTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void packAssets(string rootDirectory)
        {
            //TODO: Make packing heuristic that the texture packer class should use configurable.
            var preferenceMusicPackSize = Preferences.LoadPreference("MusicPackSize");
            var preferenceSoundPackSize = Preferences.LoadPreference("SoundPackSize");
            var preferenceTexturePackSize = Preferences.LoadPreference("TexturePackSize");

            if (!int.TryParse(preferenceMusicPackSize, out var musicPackSize))
            {
                _ = MessageBox.Show(
                    this,
                    Strings.Errors.UnableToParseInvalidIntegerFormat.ToString(preferenceMusicPackSize),
                    Strings.Errors.InvalidInputXCaption.ToString(Strings.Options.MusicPackSize),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (!int.TryParse(preferenceSoundPackSize, out var soundPackSize))
            {
                _ = MessageBox.Show(
                    this,
                    Strings.Errors.UnableToParseInvalidIntegerFormat.ToString(preferenceSoundPackSize),
                    Strings.Errors.InvalidInputXCaption.ToString(Strings.Options.SoundPackSize),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (!int.TryParse(preferenceTexturePackSize, out var texturePackSize))
            {
                _ = MessageBox.Show(
                    this,
                    Strings.Errors.UnableToParseInvalidIntegerFormat.ToString(preferenceTexturePackSize),
                    Strings.Errors.InvalidInputXCaption.ToString(Strings.Options.TextureSize),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            var resourcesDirectory = Path.Combine(rootDirectory, "resources");
            var packsDirectory = Path.Combine(resourcesDirectory, "packs");

            //Delete Old Packs
            Globals.PackingProgressForm.SetProgress(Strings.AssetPacking.deleting, 10, false);
            Application.DoEvents();
            if (Directory.Exists(packsDirectory))
            {
                var di = new DirectoryInfo(packsDirectory);

                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }

                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            else
            {
                Directory.CreateDirectory(packsDirectory);
            }

            //Create two 'sets' of graphics we want to pack. Tilesets + Fogs in one set, everything else in the other.
            Globals.PackingProgressForm.SetProgress(Strings.AssetPacking.collecting, 20, false);
            Application.DoEvents();
            var toPack = new HashSet<Texture>();
            foreach (var tex in GameContentManager.TilesetTextures)
            {
                toPack.Add(tex);
            }

            foreach (var tex in GameContentManager.FogTextures)
            {
                toPack.Add(tex);
            }

            foreach (var tex in GameContentManager.AllTextures)
            {
                if (!toPack.Contains(tex))
                {
                    toPack.Add(tex);
                }
            }

            Globals.PackingProgressForm.SetProgress(Strings.AssetPacking.calculating, 30, false);
            Application.DoEvents();
            var packs = new List<TexturePacker>();
            while (toPack.Count > 0)
            {
                var tex = toPack.First();
                var inserted = false;
                toPack.Remove(tex);

                foreach (var pack in packs)
                {
                    if (pack.InsertTex(tex))
                    {
                        inserted = true;

                        break;
                    }
                }

                if (!inserted)
                {
                    if (tex.GetWidth() > texturePackSize || tex.GetHeight() > texturePackSize)
                    {
                        //Own texture
                        var pack = new TexturePacker(resourcesDirectory, tex.GetWidth(), tex.GetHeight(), false);
                        packs.Add(pack);
                        pack.InsertTex(tex);
                    }
                    else
                    {
                        var pack = new TexturePacker(resourcesDirectory, texturePackSize, texturePackSize, true);
                        packs.Add(pack);
                        if (!pack.InsertTex(tex))
                        {
                            throw new Exception("This shouldn't happen!");
                        }
                    }
                }
            }

            Globals.PackingProgressForm.SetProgress(Strings.AssetPacking.exporting, 40, false);
            Application.DoEvents();
            var packIndex = 0;
            foreach (var pack in packs)
            {
                pack.Export(packIndex);
                packIndex++;
            }

            // Package up sounds!
            Globals.PackingProgressForm.SetProgress(Strings.AssetPacking.sounds, 80, false);
            Application.DoEvents();
            AssetPacker.PackageAssets(Path.Combine(resourcesDirectory, "sounds"), "*.wav", packsDirectory, "sound.index", "sound", ".asset", soundPackSize);

            // Package up music!
            Globals.PackingProgressForm.SetProgress(Strings.AssetPacking.music, 90, false);
            Application.DoEvents();
            AssetPacker.PackageAssets(Path.Combine(resourcesDirectory, "music"), "*.ogg", packsDirectory, "music.index", "music", ".asset", musicPackSize);

            Globals.PackingProgressForm.SetProgress(Strings.AssetPacking.done, 100, false);
            Application.DoEvents();
            Thread.Sleep(1000);

            Globals.PackingProgressForm.NotifyClose();
        }

        private string SelectDirectoryWithRetry(string description, string initialPath, bool showNewFolderButton)
        {
            var attempts = 0;
            while (attempts < 2)
            {
                using (var folderBrowserDialog = new FolderBrowserDialog()
                {
                    Description = description,
                    SelectedPath = initialPath,
                    ShowNewFolderButton = showNewFolderButton,
                })
                {
                    var selectionDialogResult = folderBrowserDialog.ShowDialog();
                    switch (selectionDialogResult)
                    {
                        case DialogResult.OK:
                            var selectedPath = folderBrowserDialog.SelectedPath;
                            if (string.IsNullOrWhiteSpace(selectedPath) || !Directory.Exists(selectedPath))
                            {
                                var retryDialogueResult = DarkMessageBox.ShowError(
                                    Strings.Errors.InvalidDirectory,
                                    Strings.Errors.InvalidDirectoryCaption,
                                    DarkDialogButton.RetryCancel,
                                    Icon
                                );

                                switch (retryDialogueResult)
                                {
                                    case DialogResult.Retry:
                                        break;

                                    case DialogResult.Cancel:
                                        return default;
                                }
                                break;
                            }

                            return selectedPath;

                        case DialogResult.Cancel:
                            return default;

                        case DialogResult.None:
                        case DialogResult.Abort:
                        case DialogResult.Retry:
                        case DialogResult.Ignore:
                        case DialogResult.Yes:
                        case DialogResult.No:
                            throw new NotImplementedException($"No handler defined for {selectionDialogResult}, should be {DialogResult.OK} or {DialogResult.Cancel}.");

                        default:
                            throw new IndexOutOfRangeException($"{selectionDialogResult} is not a valid {nameof(System.Windows.Forms.DialogResult)}.");
                    }
                }
            }

            return default;
        }

        private void packageUpdateToolStripMenuItem_Click(object sender, EventArgs e) => PackageUpdate();

        private void PackageUpdate()
        {
            var lastSourceDirectory = Preferences.LoadPreference("update_sourceDirectory");
            var lastTargetDirectory = Preferences.LoadPreference("update_targetDirectory");

            var sourceDirectory = SelectDirectoryWithRetry(Strings.UpdatePacking.SourceDirectoryPromptDescription, string.IsNullOrWhiteSpace(lastSourceDirectory) ? Environment.CurrentDirectory : lastSourceDirectory, false);
            if (sourceDirectory == default)
            {
                return;
            }

            var targetDirectory = SelectDirectoryWithRetry(Strings.UpdatePacking.TargetDirectoryPromptDescription, string.IsNullOrWhiteSpace(lastTargetDirectory) ? Environment.CurrentDirectory : lastTargetDirectory, true);
            if (targetDirectory == default)
            {
                return;
            }

            Preferences.SavePreference("update_sourceDirectory", sourceDirectory);
            Preferences.SavePreference("update_targetDirectory", targetDirectory);

            var baseDir = new Uri($@"{sourceDirectory}\");
            var selectedDir = new Uri($@"{targetDirectory}\");

            if (baseDir.IsBaseOf(selectedDir))
            {
                // Error, cannot be put within editor folder else it would try to include itself?
                _ = DarkMessageBox.ShowError(
                    Strings.UpdatePacking.InvalidBase,
                    Strings.UpdatePacking.Error,
                    DarkDialogButton.Ok,
                    Icon
                );
                return;
            }

            Update existingUpdate = null;
            var targetUpdateFile = Path.Combine(targetDirectory, "update.json");
            if (File.Exists(targetUpdateFile))
            {
                //Existing update! Offer to create a differential folder where the only files within will be those that have changed
                if (DarkMessageBox.ShowError(
                        Strings.UpdatePacking.Differential,
                        Strings.UpdatePacking.DifferentialTitle,
                        DarkDialogButton.YesNo,
                        Icon
                    ) ==
                    DialogResult.Yes)
                {
                    existingUpdate = JsonConvert.DeserializeObject<Update>(File.ReadAllText(targetUpdateFile));
                }
            }
            else if (Directory.EnumerateFileSystemEntries(targetDirectory).Any())
            {
                //Folder must be empty!
                _ = DarkMessageBox.ShowError(
                    Strings.UpdatePacking.Empty,
                    Strings.UpdatePacking.Error,
                    DarkDialogButton.Ok,
                    Icon
                );
                return;
            }

            // Are we configured to package up our assets for an update?
            var packageUpdateAssets = Preferences.LoadPreference("PackageUpdateAssets");
            if (!string.IsNullOrWhiteSpace(packageUpdateAssets) && Convert.ToBoolean(packageUpdateAssets, CultureInfo.InvariantCulture))
            {
                Globals.PackingProgressForm = new FrmProgress();
                Globals.PackingProgressForm.SetTitle(Strings.AssetPacking.title);
                var assetThread = new Thread(() => packAssets(sourceDirectory));
                assetThread.Start();
                _ = Globals.PackingProgressForm.ShowDialog();
            }

            Globals.UpdateCreationProgressForm = new FrmProgress();
            Globals.UpdateCreationProgressForm.SetTitle(Strings.UpdatePacking.Title);
            Globals.UpdateCreationProgressForm.SetProgress(Strings.UpdatePacking.Deleting, 10, false);
            var packingthread = new Thread(() => createUpdate(sourceDirectory, targetDirectory, existingUpdate));
            packingthread.Start();
            _ = Globals.UpdateCreationProgressForm.ShowDialog();
        }

        private void createUpdate(string sourceDirectory, string targetDirectory, Update existingUpdate)
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory((targetDirectory));
            }

            if (Directory.Exists(targetDirectory))
            {
                DirectoryInfo di = new DirectoryInfo(targetDirectory);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }

                // Intersect excluded files
                var editorBaseName = Process.GetCurrentProcess()?.ProcessName.ToLowerInvariant() ?? "intersect editor";
                var editorFileNameExe = $"{editorBaseName}.exe";
                var editorFileNamePdb = $"{editorBaseName}.pdb";
                var excludeFiles = new string[] { "resources/mapcache.db", "update.json", "version.json" };
                var clientExcludeFiles = new List<string>(){ editorFileNameExe, editorFileNamePdb, "resources/editor_strings.json" };
                var editorExcludeFiles = new List<string>() { "resources/client_strings.json" };
                var excludeExtensions = new string[] { ".dll", ".xml", ".config", ".php" };
                var excludeDirectories = new string[] { "logs", "screenshots" };

                var resourcesDirectory = Path.Combine(sourceDirectory, "resources");
                var packsDirectory = Path.Combine(resourcesDirectory, "packs");
                if (Directory.Exists(packsDirectory))
                {
                    var packs = Directory.GetFiles(packsDirectory, "*.meta");
                    editorExcludeFiles.AddRange(packs);
                    foreach (var pack in packs)
                    {
                        var obj = JObject.Parse(GzipCompression.ReadDecompressedString(pack))["frames"];
                        foreach (var frame in obj.Children())
                        {
                            var filename = frame["filename"].ToString();
                            clientExcludeFiles.Add(filename);
                        }
                    }

                    var soundIndex = Path.Combine(packsDirectory, "sound.index");
                    if (File.Exists(soundIndex))
                    {
                        editorExcludeFiles.Add(soundIndex);
                        using (var soundPacker = new AssetPacker(soundIndex, packsDirectory))
                        {
                            editorExcludeFiles.AddRange(soundPacker.CachedPackages.Select(cachedPackage => Path.Combine(soundPacker.PackageLocation, cachedPackage)));
                            foreach (var sound in soundPacker.FileList)
                            {
                                // Add as lowercase as our update generator checks for lowercases!
                                clientExcludeFiles.Add(Path.Combine(resourcesDirectory, "sounds", sound.ToLower(CultureInfo.CurrentCulture)).Replace('\\', '/'));
                            }
                        }
                    }

                    var musicIndex = Path.Combine(packsDirectory, "music.index");
                    if (File.Exists(musicIndex))
                    {
                        editorExcludeFiles.Add(musicIndex);
                        using (var musicPacker = new AssetPacker(musicIndex, packsDirectory))
                        {
                            editorExcludeFiles.AddRange(musicPacker.CachedPackages.Select(cachedPackage => Path.Combine(musicPacker.PackageLocation, cachedPackage)));
                            foreach (var music in musicPacker.FileList)
                            {
                                // Add as lowercase as our update generator checks for lowercases!
                                clientExcludeFiles.Add(Path.Combine(resourcesDirectory, "music", music.ToLower(CultureInfo.CurrentCulture)).Replace('\\', '/'));
                            }
                        }
                    }

                }

                var fileCount = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories).Length;

                var update = new Update();
                queryFilesForUpdate(update, excludeFiles, clientExcludeFiles, editorExcludeFiles, excludeExtensions, excludeDirectories, sourceDirectory, sourceDirectory, sourceDirectory, targetDirectory, 0, fileCount, existingUpdate);
            }
        }

        private int queryFilesForUpdate(Update update, string[] excludeFiles, IEnumerable<string> clientExcludeFiles, IEnumerable<string> editorExcludeFiles, string[] excludeExtensions, string[] excludeDirectories, string workingDirectory, string sourcePath, string currentSourcePath, string targetPath, int filesProcessed, int fileCount, Update existingUpdate)
        {
            var di = new DirectoryInfo(currentSourcePath);
            var workingDir = new Uri(workingDirectory + "/");

            foreach (var file in di.GetFiles())
            {
                var relativePath = Uri.UnescapeDataString(workingDir.MakeRelativeUri(new Uri(Path.Combine(currentSourcePath, file.Name))).ToString().Replace('\\', '/'));
                if (!excludeFiles.Contains(relativePath) && !excludeExtensions.Contains(file.Extension))
                {
                    var md5Hash = string.Empty;
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = new BufferedStream(File.OpenRead(file.FullName), 1200000))
                        {
                            md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower(CultureInfo.InvariantCulture);
                        }
                    }

                    var updateFile = new UpdateFile(relativePath, md5Hash, file.Length)
                    {
                        ClientIgnore = clientExcludeFiles.Contains(relativePath.ToLower(CultureInfo.CurrentCulture)),
                        EditorIgnore = editorExcludeFiles.Contains(relativePath.ToLower(CultureInfo.CurrentCulture)),
                    };

                    update.Files.Add(updateFile);

                    //Copy File (If not in existing update)
                    UpdateFile existingFile = null;
                    if (existingUpdate != null)
                    {
                        existingFile = existingUpdate.Files.FirstOrDefault(f => f.Path == updateFile.Path);
                    }

                    if (existingFile == null || existingFile.Size != updateFile.Size || existingFile.Hash != updateFile.Hash) {
                        var relativeFolder = Uri.UnescapeDataString(workingDir.MakeRelativeUri(new Uri(currentSourcePath + "/")).ToString().Replace('\\', '/'));
                        if (!string.IsNullOrEmpty(relativeFolder))
                        {
                            _ = Directory.CreateDirectory(Path.Combine(targetPath, relativeFolder));
                            File.Copy(file.FullName, Path.Combine(targetPath, relativeFolder, file.Name));
                        }
                        else
                        {
                            File.Copy(file.FullName, Path.Combine(targetPath, file.Name));
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("File not added to update! " + relativePath);
                }

                filesProcessed++;

                var percentage = (float) (filesProcessed / (float) (fileCount + 1));
                var outofeighty = (int)(percentage * 80f);

                Globals.UpdateCreationProgressForm.SetProgress(
                    Strings.UpdatePacking.Calculating, outofeighty + 10, false
                );

                Application.DoEvents();
            }

            foreach (var dir in di.GetDirectories())
            {
                var relativePath = Uri.UnescapeDataString(workingDir.MakeRelativeUri(new Uri(Path.Combine(currentSourcePath, dir.Name))).ToString().Replace('\\', '/'));
                if (!excludeDirectories.Contains(relativePath))
                {
                    filesProcessed = queryFilesForUpdate(update, excludeFiles, clientExcludeFiles, editorExcludeFiles, excludeExtensions, excludeFiles, workingDirectory, sourcePath, Path.Combine(currentSourcePath, dir.Name), targetPath, filesProcessed, fileCount, existingUpdate);
                }
                else
                {
                    //MessageBox.Show("Directory not added to update! " + relativePath);
                }
            }

            if (string.Equals(sourcePath, currentSourcePath, StringComparison.Ordinal))
            {
                Globals.UpdateCreationProgressForm.SetProgress(Strings.UpdatePacking.Done, 100, false);
                Application.DoEvents();
                Thread.Sleep(1000);

                //TODO: Open folder with update files
                var targetJsonPath = Path.Combine(targetPath, "update.json");
                File.WriteAllText(targetJsonPath, JsonConvert.SerializeObject(update, Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore }));

                Globals.UpdateCreationProgressForm.NotifyClose();
            }

            return filesProcessed;
        }

    }

}
