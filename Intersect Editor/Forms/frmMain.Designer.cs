using System.ComponentModel;
using System.Windows.Forms;

namespace Intersect_Editor.Forms
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin2 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
            WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin2 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient4 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient8 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin2 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient9 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient5 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient10 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient11 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient12 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient6 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient13 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient14 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eraseLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allLayersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentLayerOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideDarknessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideFogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideTilePreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideResourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentEditorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.npcEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spellEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resourceEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.classEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.questEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectileEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripBtnNewMap = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnSaveMap = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnCut = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnUndo = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnPen = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnSelect = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnRect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnFill = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnErase = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnEyeDrop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnScreenshot = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnRun = new System.Windows.Forms.ToolStripButton();
            this.vS2012LightTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2012LightTheme();
            this.dockLeft = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripLabelCoords = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelRevision = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelDebug = new System.Windows.Forms.ToolStripStatusLabel();
            this.commonEventEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.contentEditorsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1186, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveMapToolStripMenuItem,
            this.newMapToolStripMenuItem,
            this.importMapToolStripMenuItem,
            this.exportMapToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveMapToolStripMenuItem
            // 
            this.saveMapToolStripMenuItem.Name = "saveMapToolStripMenuItem";
            this.saveMapToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.saveMapToolStripMenuItem.Text = "Save Map";
            this.saveMapToolStripMenuItem.Click += new System.EventHandler(this.saveMapToolStripMenuItem_Click);
            // 
            // newMapToolStripMenuItem
            // 
            this.newMapToolStripMenuItem.Name = "newMapToolStripMenuItem";
            this.newMapToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.newMapToolStripMenuItem.Text = "New Map";
            this.newMapToolStripMenuItem.ToolTipText = "Create a new, unconnected map.";
            this.newMapToolStripMenuItem.Click += new System.EventHandler(this.newMapToolStripMenuItem_Click);
            // 
            // importMapToolStripMenuItem
            // 
            this.importMapToolStripMenuItem.Name = "importMapToolStripMenuItem";
            this.importMapToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.importMapToolStripMenuItem.Text = "Import Map";
            this.importMapToolStripMenuItem.Click += new System.EventHandler(this.importMapToolStripMenuItem_Click);
            // 
            // exportMapToolStripMenuItem
            // 
            this.exportMapToolStripMenuItem.Name = "exportMapToolStripMenuItem";
            this.exportMapToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.exportMapToolStripMenuItem.Text = "Export Map";
            this.exportMapToolStripMenuItem.Click += new System.EventHandler(this.exportMapToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.fillToolStripMenuItem,
            this.eraseLayerToolStripMenuItem,
            this.selectToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Enabled = false;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Enabled = false;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Enabled = false;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Enabled = false;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // fillToolStripMenuItem
            // 
            this.fillToolStripMenuItem.Name = "fillToolStripMenuItem";
            this.fillToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.fillToolStripMenuItem.Text = "Fill";
            this.fillToolStripMenuItem.Click += new System.EventHandler(this.fillToolStripMenuItem_Click);
            // 
            // eraseLayerToolStripMenuItem
            // 
            this.eraseLayerToolStripMenuItem.Name = "eraseLayerToolStripMenuItem";
            this.eraseLayerToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.eraseLayerToolStripMenuItem.Text = "Erase Layer";
            this.eraseLayerToolStripMenuItem.Click += new System.EventHandler(this.eraseLayerToolStripMenuItem_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allLayersToolStripMenuItem,
            this.currentLayerOnlyToolStripMenuItem});
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.selectToolStripMenuItem.Text = "Select....";
            // 
            // allLayersToolStripMenuItem
            // 
            this.allLayersToolStripMenuItem.Checked = true;
            this.allLayersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.allLayersToolStripMenuItem.Name = "allLayersToolStripMenuItem";
            this.allLayersToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.allLayersToolStripMenuItem.Text = "All Layers";
            this.allLayersToolStripMenuItem.Click += new System.EventHandler(this.allLayersToolStripMenuItem_Click);
            // 
            // currentLayerOnlyToolStripMenuItem
            // 
            this.currentLayerOnlyToolStripMenuItem.Name = "currentLayerOnlyToolStripMenuItem";
            this.currentLayerOnlyToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.currentLayerOnlyToolStripMenuItem.Text = "Current Layer Only";
            this.currentLayerOnlyToolStripMenuItem.Click += new System.EventHandler(this.currentLayerOnlyToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideDarknessToolStripMenuItem,
            this.hideFogToolStripMenuItem,
            this.hideOverlayToolStripMenuItem,
            this.hideTilePreviewToolStripMenuItem,
            this.hideResourcesToolStripMenuItem,
            this.mapGridToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // hideDarknessToolStripMenuItem
            // 
            this.hideDarknessToolStripMenuItem.Checked = true;
            this.hideDarknessToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideDarknessToolStripMenuItem.Name = "hideDarknessToolStripMenuItem";
            this.hideDarknessToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.hideDarknessToolStripMenuItem.Text = "Darkness";
            this.hideDarknessToolStripMenuItem.Click += new System.EventHandler(this.hideDarknessToolStripMenuItem_Click);
            // 
            // hideFogToolStripMenuItem
            // 
            this.hideFogToolStripMenuItem.Checked = true;
            this.hideFogToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideFogToolStripMenuItem.Name = "hideFogToolStripMenuItem";
            this.hideFogToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.hideFogToolStripMenuItem.Text = "Fog";
            this.hideFogToolStripMenuItem.Click += new System.EventHandler(this.hideFogToolStripMenuItem_Click);
            // 
            // hideOverlayToolStripMenuItem
            // 
            this.hideOverlayToolStripMenuItem.Checked = true;
            this.hideOverlayToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideOverlayToolStripMenuItem.Name = "hideOverlayToolStripMenuItem";
            this.hideOverlayToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.hideOverlayToolStripMenuItem.Text = "Overlay";
            this.hideOverlayToolStripMenuItem.Click += new System.EventHandler(this.hideOverlayToolStripMenuItem_Click);
            // 
            // hideTilePreviewToolStripMenuItem
            // 
            this.hideTilePreviewToolStripMenuItem.Checked = true;
            this.hideTilePreviewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideTilePreviewToolStripMenuItem.Name = "hideTilePreviewToolStripMenuItem";
            this.hideTilePreviewToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.hideTilePreviewToolStripMenuItem.Text = "Tile Preview";
            this.hideTilePreviewToolStripMenuItem.Click += new System.EventHandler(this.hideTilePreviewToolStripMenuItem_Click);
            // 
            // hideResourcesToolStripMenuItem
            // 
            this.hideResourcesToolStripMenuItem.Checked = true;
            this.hideResourcesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideResourcesToolStripMenuItem.Name = "hideResourcesToolStripMenuItem";
            this.hideResourcesToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.hideResourcesToolStripMenuItem.Text = "Resources";
            this.hideResourcesToolStripMenuItem.Click += new System.EventHandler(this.hideResourcesToolStripMenuItem_Click);
            // 
            // mapGridToolStripMenuItem
            // 
            this.mapGridToolStripMenuItem.Name = "mapGridToolStripMenuItem";
            this.mapGridToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.mapGridToolStripMenuItem.Text = "Map Grid";
            this.mapGridToolStripMenuItem.Click += new System.EventHandler(this.mapGridToolStripMenuItem_Click);
            // 
            // contentEditorsToolStripMenuItem
            // 
            this.contentEditorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemEditorToolStripMenuItem,
            this.npcEditorToolStripMenuItem,
            this.spellEditorToolStripMenuItem,
            this.animationEditorToolStripMenuItem,
            this.resourceEditorToolStripMenuItem,
            this.classEditorToolStripMenuItem,
            this.questEditorToolStripMenuItem,
            this.projectileEditorToolStripMenuItem,
            this.commonEventEditorToolStripMenuItem});
            this.contentEditorsToolStripMenuItem.Name = "contentEditorsToolStripMenuItem";
            this.contentEditorsToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.contentEditorsToolStripMenuItem.Text = "Content Editors";
            // 
            // itemEditorToolStripMenuItem
            // 
            this.itemEditorToolStripMenuItem.Name = "itemEditorToolStripMenuItem";
            this.itemEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.itemEditorToolStripMenuItem.Text = "Item Editor";
            this.itemEditorToolStripMenuItem.Click += new System.EventHandler(this.itemEditorToolStripMenuItem_Click);
            // 
            // npcEditorToolStripMenuItem
            // 
            this.npcEditorToolStripMenuItem.Name = "npcEditorToolStripMenuItem";
            this.npcEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.npcEditorToolStripMenuItem.Text = "Npc Editor";
            this.npcEditorToolStripMenuItem.Click += new System.EventHandler(this.npcEditorToolStripMenuItem_Click);
            // 
            // spellEditorToolStripMenuItem
            // 
            this.spellEditorToolStripMenuItem.Name = "spellEditorToolStripMenuItem";
            this.spellEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.spellEditorToolStripMenuItem.Text = "Spell Editor";
            this.spellEditorToolStripMenuItem.Click += new System.EventHandler(this.spellEditorToolStripMenuItem_Click);
            // 
            // animationEditorToolStripMenuItem
            // 
            this.animationEditorToolStripMenuItem.Name = "animationEditorToolStripMenuItem";
            this.animationEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.animationEditorToolStripMenuItem.Text = "Animation Editor";
            this.animationEditorToolStripMenuItem.Click += new System.EventHandler(this.animationEditorToolStripMenuItem_Click);
            // 
            // resourceEditorToolStripMenuItem
            // 
            this.resourceEditorToolStripMenuItem.Name = "resourceEditorToolStripMenuItem";
            this.resourceEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.resourceEditorToolStripMenuItem.Text = "Resource Editor";
            this.resourceEditorToolStripMenuItem.Click += new System.EventHandler(this.resourceEditorToolStripMenuItem_Click);
            // 
            // classEditorToolStripMenuItem
            // 
            this.classEditorToolStripMenuItem.Name = "classEditorToolStripMenuItem";
            this.classEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.classEditorToolStripMenuItem.Text = "Class Editor";
            this.classEditorToolStripMenuItem.Click += new System.EventHandler(this.classEditorToolStripMenuItem_Click);
            // 
            // questEditorToolStripMenuItem
            // 
            this.questEditorToolStripMenuItem.Enabled = false;
            this.questEditorToolStripMenuItem.Name = "questEditorToolStripMenuItem";
            this.questEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.questEditorToolStripMenuItem.Text = "Quest Editor";
            this.questEditorToolStripMenuItem.Click += new System.EventHandler(this.questEditorToolStripMenuItem_Click);
            // 
            // projectileEditorToolStripMenuItem
            // 
            this.projectileEditorToolStripMenuItem.Name = "projectileEditorToolStripMenuItem";
            this.projectileEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.projectileEditorToolStripMenuItem.Text = "Projectile Editor";
            this.projectileEditorToolStripMenuItem.Click += new System.EventHandler(this.projectileEditorToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBtnNewMap,
            this.toolStripBtnSaveMap,
            this.toolStripSeparator1,
            this.toolStripBtnCut,
            this.toolStripBtnCopy,
            this.toolStripBtnPaste,
            this.toolStripSeparator2,
            this.toolStripBtnUndo,
            this.toolStripBtnRedo,
            this.toolStripSeparator3,
            this.toolStripBtnPen,
            this.toolStripBtnSelect,
            this.toolStripBtnRect,
            this.toolStripSeparator4,
            this.toolStripBtnFill,
            this.toolStripBtnErase,
            this.toolStripBtnEyeDrop,
            this.toolStripSeparator5,
            this.toolStripBtnScreenshot,
            this.toolStripSeparator6,
            this.toolStripBtnRun});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(1186, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripBtnNewMap
            // 
            this.toolStripBtnNewMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnNewMap.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnNewMap.Image")));
            this.toolStripBtnNewMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnNewMap.Margin = new System.Windows.Forms.Padding(12, 1, 0, 2);
            this.toolStripBtnNewMap.Name = "toolStripBtnNewMap";
            this.toolStripBtnNewMap.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnNewMap.Text = "New Unconnected Map";
            this.toolStripBtnNewMap.Click += new System.EventHandler(this.toolStripBtnNewMap_Click);
            // 
            // toolStripBtnSaveMap
            // 
            this.toolStripBtnSaveMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnSaveMap.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnSaveMap.Image")));
            this.toolStripBtnSaveMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnSaveMap.Name = "toolStripBtnSaveMap";
            this.toolStripBtnSaveMap.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnSaveMap.Text = "Save Map";
            this.toolStripBtnSaveMap.Click += new System.EventHandler(this.toolStripBtnSaveMap_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnCut
            // 
            this.toolStripBtnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnCut.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnCut.Image")));
            this.toolStripBtnCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnCut.Name = "toolStripBtnCut";
            this.toolStripBtnCut.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnCut.Text = "Cut Selection";
            this.toolStripBtnCut.Click += new System.EventHandler(this.toolStripBtnCut_Click);
            // 
            // toolStripBtnCopy
            // 
            this.toolStripBtnCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnCopy.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnCopy.Image")));
            this.toolStripBtnCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnCopy.Name = "toolStripBtnCopy";
            this.toolStripBtnCopy.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnCopy.Text = "Copy Selection";
            this.toolStripBtnCopy.Click += new System.EventHandler(this.toolStripBtnCopy_Click);
            // 
            // toolStripBtnPaste
            // 
            this.toolStripBtnPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnPaste.Image")));
            this.toolStripBtnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnPaste.Name = "toolStripBtnPaste";
            this.toolStripBtnPaste.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnPaste.Text = "Paste";
            this.toolStripBtnPaste.Click += new System.EventHandler(this.toolStripBtnPaste_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnUndo
            // 
            this.toolStripBtnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnUndo.Enabled = false;
            this.toolStripBtnUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnUndo.Image")));
            this.toolStripBtnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnUndo.Name = "toolStripBtnUndo";
            this.toolStripBtnUndo.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnUndo.Text = "Undo";
            this.toolStripBtnUndo.Click += new System.EventHandler(this.toolStripBtnUndo_Click);
            // 
            // toolStripBtnRedo
            // 
            this.toolStripBtnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnRedo.Enabled = false;
            this.toolStripBtnRedo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnRedo.Image")));
            this.toolStripBtnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnRedo.Name = "toolStripBtnRedo";
            this.toolStripBtnRedo.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnRedo.Text = "Redo";
            this.toolStripBtnRedo.Click += new System.EventHandler(this.toolStripBtnRedo_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnPen
            // 
            this.toolStripBtnPen.Checked = true;
            this.toolStripBtnPen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripBtnPen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnPen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnPen.Image")));
            this.toolStripBtnPen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnPen.Name = "toolStripBtnPen";
            this.toolStripBtnPen.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnPen.Text = "Single Tile";
            this.toolStripBtnPen.Click += new System.EventHandler(this.toolStripBtnPen_Click);
            // 
            // toolStripBtnSelect
            // 
            this.toolStripBtnSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnSelect.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnSelect.Image")));
            this.toolStripBtnSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnSelect.Name = "toolStripBtnSelect";
            this.toolStripBtnSelect.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnSelect.Text = "Selection";
            this.toolStripBtnSelect.Click += new System.EventHandler(this.toolStripBtnSelect_Click);
            // 
            // toolStripBtnRect
            // 
            this.toolStripBtnRect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnRect.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnRect.Image")));
            this.toolStripBtnRect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnRect.Name = "toolStripBtnRect";
            this.toolStripBtnRect.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnRect.Text = "Rectangle Fill";
            this.toolStripBtnRect.Click += new System.EventHandler(this.toolStripBtnRect_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnFill
            // 
            this.toolStripBtnFill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnFill.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnFill.Image")));
            this.toolStripBtnFill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnFill.Name = "toolStripBtnFill";
            this.toolStripBtnFill.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnFill.Text = "Fill Layer";
            this.toolStripBtnFill.Click += new System.EventHandler(this.toolStripBtnFill_Click);
            // 
            // toolStripBtnErase
            // 
            this.toolStripBtnErase.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnErase.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnErase.Image")));
            this.toolStripBtnErase.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnErase.Name = "toolStripBtnErase";
            this.toolStripBtnErase.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnErase.Text = "Clear Layer";
            this.toolStripBtnErase.Click += new System.EventHandler(this.toolStripBtnErase_Click);
            // 
            // toolStripBtnEyeDrop
            // 
            this.toolStripBtnEyeDrop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnEyeDrop.Enabled = false;
            this.toolStripBtnEyeDrop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnEyeDrop.Image")));
            this.toolStripBtnEyeDrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnEyeDrop.Name = "toolStripBtnEyeDrop";
            this.toolStripBtnEyeDrop.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnEyeDrop.Text = "Eye Drop Tool";
            this.toolStripBtnEyeDrop.Click += new System.EventHandler(this.toolStripBtnEyeDrop_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnScreenshot
            // 
            this.toolStripBtnScreenshot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnScreenshot.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnScreenshot.Image")));
            this.toolStripBtnScreenshot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnScreenshot.Name = "toolStripBtnScreenshot";
            this.toolStripBtnScreenshot.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnScreenshot.Text = "Screenshot Map";
            this.toolStripBtnScreenshot.Click += new System.EventHandler(this.toolStripBtnScreenshot_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnRun
            // 
            this.toolStripBtnRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnRun.Enabled = false;
            this.toolStripBtnRun.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnRun.Image")));
            this.toolStripBtnRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnRun.Name = "toolStripBtnRun";
            this.toolStripBtnRun.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnRun.Text = "Run Client";
            // 
            // dockLeft
            // 
            this.dockLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dockLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockLeft.DockLeftPortion = 316D;
            this.dockLeft.DockRightPortion = 0.15D;
            this.dockLeft.Location = new System.Drawing.Point(0, 49);
            this.dockLeft.Name = "dockLeft";
            this.dockLeft.Size = new System.Drawing.Size(1186, 610);
            dockPanelGradient4.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient4.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            autoHideStripSkin2.DockStripGradient = dockPanelGradient4;
            tabGradient8.EndColor = System.Drawing.SystemColors.Control;
            tabGradient8.StartColor = System.Drawing.SystemColors.Control;
            tabGradient8.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            autoHideStripSkin2.TabGradient = tabGradient8;
            autoHideStripSkin2.TextFont = new System.Drawing.Font("Segoe UI", 9F);
            dockPanelSkin2.AutoHideStripSkin = autoHideStripSkin2;
            tabGradient9.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(206)))), ((int)(((byte)(219)))));
            tabGradient9.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            tabGradient9.TextColor = System.Drawing.Color.White;
            dockPaneStripGradient2.ActiveTabGradient = tabGradient9;
            dockPanelGradient5.EndColor = System.Drawing.SystemColors.Control;
            dockPanelGradient5.StartColor = System.Drawing.SystemColors.Control;
            dockPaneStripGradient2.DockStripGradient = dockPanelGradient5;
            tabGradient10.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(151)))), ((int)(((byte)(234)))));
            tabGradient10.StartColor = System.Drawing.SystemColors.Control;
            tabGradient10.TextColor = System.Drawing.Color.Black;
            dockPaneStripGradient2.InactiveTabGradient = tabGradient10;
            dockPaneStripSkin2.DocumentGradient = dockPaneStripGradient2;
            dockPaneStripSkin2.TextFont = new System.Drawing.Font("Segoe UI", 9F);
            tabGradient11.EndColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(170)))), ((int)(((byte)(220)))));
            tabGradient11.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient11.StartColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            tabGradient11.TextColor = System.Drawing.Color.White;
            dockPaneStripToolWindowGradient2.ActiveCaptionGradient = tabGradient11;
            tabGradient12.EndColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient12.StartColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient12.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dockPaneStripToolWindowGradient2.ActiveTabGradient = tabGradient12;
            dockPanelGradient6.EndColor = System.Drawing.SystemColors.Control;
            dockPanelGradient6.StartColor = System.Drawing.SystemColors.Control;
            dockPaneStripToolWindowGradient2.DockStripGradient = dockPanelGradient6;
            tabGradient13.EndColor = System.Drawing.SystemColors.ControlDark;
            tabGradient13.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient13.StartColor = System.Drawing.SystemColors.Control;
            tabGradient13.TextColor = System.Drawing.SystemColors.GrayText;
            dockPaneStripToolWindowGradient2.InactiveCaptionGradient = tabGradient13;
            tabGradient14.EndColor = System.Drawing.SystemColors.Control;
            tabGradient14.StartColor = System.Drawing.SystemColors.Control;
            tabGradient14.TextColor = System.Drawing.SystemColors.GrayText;
            dockPaneStripToolWindowGradient2.InactiveTabGradient = tabGradient14;
            dockPaneStripSkin2.ToolWindowGradient = dockPaneStripToolWindowGradient2;
            dockPanelSkin2.DockPaneStripSkin = dockPaneStripSkin2;
            this.dockLeft.Skin = dockPanelSkin2;
            this.dockLeft.SupportDeeplyNestedContent = true;
            this.dockLeft.TabIndex = 7;
            this.dockLeft.Theme = this.vS2012LightTheme1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelCoords,
            this.toolStripSeparator7,
            this.toolStripLabelRevision,
            this.toolStripSeparator8,
            this.toolStripLabelFPS,
            this.toolStripSeparator9,
            this.toolStripLabelDebug});
            this.statusStrip1.Location = new System.Drawing.Point(0, 659);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1186, 23);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripLabelCoords
            // 
            this.toolStripLabelCoords.Name = "toolStripLabelCoords";
            this.toolStripLabelCoords.Size = new System.Drawing.Size(118, 18);
            this.toolStripLabelCoords.Text = "toolStripStatusLabel1";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripLabelRevision
            // 
            this.toolStripLabelRevision.Name = "toolStripLabelRevision";
            this.toolStripLabelRevision.Size = new System.Drawing.Size(63, 18);
            this.toolStripLabelRevision.Text = "Revision: 0";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripLabelFPS
            // 
            this.toolStripLabelFPS.Name = "toolStripLabelFPS";
            this.toolStripLabelFPS.Size = new System.Drawing.Size(44, 18);
            this.toolStripLabelFPS.Text = "FPS: 64";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripLabelDebug
            // 
            this.toolStripLabelDebug.Name = "toolStripLabelDebug";
            this.toolStripLabelDebug.Size = new System.Drawing.Size(0, 18);
            // 
            // commonEventEditorToolStripMenuItem
            // 
            this.commonEventEditorToolStripMenuItem.Name = "commonEventEditorToolStripMenuItem";
            this.commonEventEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.commonEventEditorToolStripMenuItem.Text = "Common Event Editor";
            this.commonEventEditorToolStripMenuItem.Click += new System.EventHandler(this.commonEventEditorToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1186, 682);
            this.Controls.Add(this.dockLeft);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Intersect Editor";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMain_KeyDown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem fillToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem eraseLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideDarknessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hihiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentEditorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem itemEditorToolStripMenuItem;
        private ToolStripMenuItem npcEditorToolStripMenuItem;
        private ToolStripMenuItem spellEditorToolStripMenuItem;
        private ToolStripMenuItem animationEditorToolStripMenuItem;
        private ToolStripMenuItem hideFogToolStripMenuItem;
        private ToolStripMenuItem hideOverlayToolStripMenuItem;
        private ToolStripMenuItem resourceEditorToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripBtnNewMap;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripBtnSaveMap;
        public ToolStripButton toolStripBtnCut;
        public ToolStripButton toolStripBtnCopy;
        private WeifenLuo.WinFormsUI.Docking.VS2012LightTheme vS2012LightTheme1;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockLeft;
        private ToolStripMenuItem classEditorToolStripMenuItem;
        private StatusStrip statusStrip1;
        public ToolStripButton toolStripBtnPaste;
        private ToolStripSeparator toolStripSeparator2;
        public ToolStripButton toolStripBtnUndo;
        public ToolStripButton toolStripBtnRedo;
        private ToolStripSeparator toolStripSeparator3;
        public ToolStripButton toolStripBtnPen;
        public ToolStripButton toolStripBtnRect;
        private ToolStripSeparator toolStripSeparator4;
        public ToolStripButton toolStripBtnFill;
        public ToolStripButton toolStripBtnErase;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton toolStripBtnRun;
        private ToolStripMenuItem hideTilePreviewToolStripMenuItem;
        private ToolStripMenuItem questEditorToolStripMenuItem;
        private ToolStripButton toolStripBtnScreenshot;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem importMapToolStripMenuItem;
        private ToolStripMenuItem exportMapToolStripMenuItem;
        public ToolStripStatusLabel toolStripLabelCoords;
        private ToolStripSeparator toolStripSeparator7;
        public ToolStripStatusLabel toolStripLabelFPS;
        private ToolStripSeparator toolStripSeparator8;
        public ToolStripStatusLabel toolStripLabelDebug;
        public ToolStripStatusLabel toolStripLabelRevision;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem hideResourcesToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem selectToolStripMenuItem;
        private ToolStripMenuItem allLayersToolStripMenuItem;
        private ToolStripMenuItem currentLayerOnlyToolStripMenuItem;
        public ToolStripButton toolStripBtnSelect;
        private ToolStripButton toolStripBtnEyeDrop;
        private ToolStripMenuItem projectileEditorToolStripMenuItem;
        private ToolStripMenuItem mapGridToolStripMenuItem;
        private ToolStripMenuItem commonEventEditorToolStripMenuItem;
    }
}

