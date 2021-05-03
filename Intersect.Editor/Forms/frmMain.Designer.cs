using System.ComponentModel;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Renderers;
using WeifenLuo.WinFormsUI.Docking;

namespace Intersect.Editor.Forms
{
	partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.statusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            this.toolStripLabelCoords = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelRevision = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelDebug = new System.Windows.Forms.ToolStripStatusLabel();
            this.dockLeft = new WeifenLuo.WinFormsUI.Docking.DockPanel();
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
            this.toolStripTimeButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnScreenshot = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnRun = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonBug = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnFlipVertical = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnFlipHorizontal = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonQuestion = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new DarkUI.Controls.DarkToolStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.hideEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentEditorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.classEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commonEventEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.craftsEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.craftingTableEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.npcEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectileEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.questEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resourceEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shopEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spellEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.variableEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.postQuestionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportBugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip = new DarkUI.Controls.DarkMenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.packageUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.statusStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelCoords,
            this.toolStripSeparator7,
            this.toolStripLabelRevision,
            this.toolStripSeparator8,
            this.toolStripLabelFPS,
            this.toolStripSeparator9,
            this.toolStripLabelDebug});
            this.statusStrip1.Location = new System.Drawing.Point(0, 645);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.statusStrip1.Size = new System.Drawing.Size(1186, 30);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripLabelCoords
            // 
            this.toolStripLabelCoords.ForeColor = System.Drawing.SystemColors.Control;
            this.toolStripLabelCoords.Name = "toolStripLabelCoords";
            this.toolStripLabelCoords.Size = new System.Drawing.Size(118, 17);
            this.toolStripLabelCoords.Text = "toolStripStatusLabel1";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 22);
            // 
            // toolStripLabelRevision
            // 
            this.toolStripLabelRevision.ForeColor = System.Drawing.SystemColors.Control;
            this.toolStripLabelRevision.Name = "toolStripLabelRevision";
            this.toolStripLabelRevision.Size = new System.Drawing.Size(63, 17);
            this.toolStripLabelRevision.Text = "Revision: 0";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 22);
            // 
            // toolStripLabelFPS
            // 
            this.toolStripLabelFPS.ForeColor = System.Drawing.SystemColors.Control;
            this.toolStripLabelFPS.Name = "toolStripLabelFPS";
            this.toolStripLabelFPS.Size = new System.Drawing.Size(44, 17);
            this.toolStripLabelFPS.Text = "FPS: 64";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 22);
            // 
            // toolStripLabelDebug
            // 
            this.toolStripLabelDebug.Name = "toolStripLabelDebug";
            this.toolStripLabelDebug.Size = new System.Drawing.Size(0, 17);
            // 
            // dockLeft
            // 
            this.dockLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dockLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockLeft.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.dockLeft.DockLeftPortion = 316D;
            this.dockLeft.DockRightPortion = 0.15D;
            this.dockLeft.Location = new System.Drawing.Point(0, 49);
            this.dockLeft.Name = "dockLeft";
            this.dockLeft.ShowAutoHideContentOnHover = false;
            this.dockLeft.Size = new System.Drawing.Size(1186, 596);
            this.dockLeft.SupportDeeplyNestedContent = true;
            this.dockLeft.TabIndex = 7;
            // 
            // toolStripBtnNewMap
            // 
            this.toolStripBtnNewMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnNewMap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.toolStripBtnSaveMap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripBtnSaveMap.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnSaveMap.Image")));
            this.toolStripBtnSaveMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnSaveMap.Name = "toolStripBtnSaveMap";
            this.toolStripBtnSaveMap.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnSaveMap.Text = "Save Map";
            this.toolStripBtnSaveMap.Click += new System.EventHandler(this.toolStripBtnSaveMap_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnCut
            // 
            this.toolStripBtnCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnCut.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.toolStripBtnCopy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.toolStripBtnPaste.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripBtnPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnPaste.Image")));
            this.toolStripBtnPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnPaste.Name = "toolStripBtnPaste";
            this.toolStripBtnPaste.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnPaste.Text = "Paste";
            this.toolStripBtnPaste.Click += new System.EventHandler(this.toolStripBtnPaste_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnUndo
            // 
            this.toolStripBtnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnUndo.Enabled = false;
            this.toolStripBtnUndo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.toolStripBtnRedo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripBtnRedo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnRedo.Image")));
            this.toolStripBtnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnRedo.Name = "toolStripBtnRedo";
            this.toolStripBtnRedo.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnRedo.Text = "Redo";
            this.toolStripBtnRedo.Click += new System.EventHandler(this.toolStripBtnRedo_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnPen
            // 
            this.toolStripBtnPen.Checked = true;
            this.toolStripBtnPen.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripBtnPen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnPen.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.toolStripBtnSelect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.toolStripBtnRect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripBtnRect.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnRect.Image")));
            this.toolStripBtnRect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnRect.Name = "toolStripBtnRect";
            this.toolStripBtnRect.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnRect.Text = "Rectangle Fill";
            this.toolStripBtnRect.Click += new System.EventHandler(this.toolStripBtnRect_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator4.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnFill
            // 
            this.toolStripBtnFill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnFill.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.toolStripBtnErase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.toolStripBtnEyeDrop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripBtnEyeDrop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnEyeDrop.Image")));
            this.toolStripBtnEyeDrop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnEyeDrop.Name = "toolStripBtnEyeDrop";
            this.toolStripBtnEyeDrop.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnEyeDrop.Text = "Eye Drop Tool";
            this.toolStripBtnEyeDrop.Click += new System.EventHandler(this.toolStripBtnEyeDrop_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator5.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripTimeButton
            // 
            this.toolStripTimeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripTimeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripTimeButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripTimeButton.Image")));
            this.toolStripTimeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripTimeButton.Name = "toolStripTimeButton";
            this.toolStripTimeButton.Size = new System.Drawing.Size(29, 22);
            this.toolStripTimeButton.Text = "Toggle On/Off TOD Simulation";
            this.toolStripTimeButton.Click += new System.EventHandler(this.toolStripTimeButton_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator6.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnScreenshot
            // 
            this.toolStripBtnScreenshot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnScreenshot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripBtnScreenshot.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnScreenshot.Image")));
            this.toolStripBtnScreenshot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnScreenshot.Name = "toolStripBtnScreenshot";
            this.toolStripBtnScreenshot.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnScreenshot.Text = "Screenshot Map";
            this.toolStripBtnScreenshot.Click += new System.EventHandler(this.toolStripBtnScreenshot_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator10.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnRun
            // 
            this.toolStripBtnRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnRun.Enabled = false;
            this.toolStripBtnRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripBtnRun.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnRun.Image")));
            this.toolStripBtnRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnRun.Name = "toolStripBtnRun";
            this.toolStripBtnRun.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnRun.Text = "Run Client";
            this.toolStripBtnRun.Click += new System.EventHandler(this.toolStripBtnRun_Click);
            // 
            // toolStripButtonBug
            // 
            this.toolStripButtonBug.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonBug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBug.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripButtonBug.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBug.Image")));
            this.toolStripButtonBug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBug.Name = "toolStripButtonBug";
            this.toolStripButtonBug.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBug.Text = "Report a Bug";
            this.toolStripButtonBug.Click += new System.EventHandler(this.toolStripButtonBug_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator11.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBtnFlipVertical
            // 
            this.toolStripBtnFlipVertical.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnFlipVertical.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripBtnFlipVertical.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnFlipVertical.Image")));
            this.toolStripBtnFlipVertical.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnFlipVertical.Name = "toolStripBtnFlipVertical";
            this.toolStripBtnFlipVertical.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnFlipVertical.Text = "toolStripBtnFlipVertical";
            this.toolStripBtnFlipVertical.ToolTipText = "Flip Vertical";
            this.toolStripBtnFlipVertical.Click += new System.EventHandler(this.toolStripBtnFlipVertical_Click);
            // 
            // toolStripBtnFlipHorizontal
            // 
            this.toolStripBtnFlipHorizontal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtnFlipHorizontal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripBtnFlipHorizontal.Image = ((System.Drawing.Image)(resources.GetObject("toolStripBtnFlipHorizontal.Image")));
            this.toolStripBtnFlipHorizontal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnFlipHorizontal.Name = "toolStripBtnFlipHorizontal";
            this.toolStripBtnFlipHorizontal.Size = new System.Drawing.Size(23, 22);
            this.toolStripBtnFlipHorizontal.Text = "toolStripBtnFlipHorizontal";
            this.toolStripBtnFlipHorizontal.ToolTipText = "Flip Horizontal";
            this.toolStripBtnFlipHorizontal.Click += new System.EventHandler(this.toolStripBtnFlipHorizontal_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator13.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonQuestion
            // 
            this.toolStripButtonQuestion.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonQuestion.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonQuestion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripButtonQuestion.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonQuestion.Image")));
            this.toolStripButtonQuestion.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonQuestion.Name = "toolStripButtonQuestion";
            this.toolStripButtonQuestion.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonQuestion.Text = "Ask a Question";
            this.toolStripButtonQuestion.Click += new System.EventHandler(this.toolStripButtonQuestion_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.toolStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.toolStripBtnFlipVertical,
            this.toolStripBtnFlipHorizontal,
            this.toolStripSeparator5,
            this.toolStripBtnFill,
            this.toolStripBtnErase,
            this.toolStripBtnEyeDrop,
            this.toolStripSeparator6,
            this.toolStripTimeButton,
            this.toolStripSeparator13,
            this.toolStripBtnScreenshot,
            this.toolStripSeparator10,
            this.toolStripBtnRun,
            this.toolStripButtonBug,
            this.toolStripSeparator11,
            this.toolStripButtonQuestion});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.Size = new System.Drawing.Size(1186, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveMapToolStripMenuItem,
            this.newMapToolStripMenuItem,
            this.importMapToolStripMenuItem,
            this.exportMapToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveMapToolStripMenuItem
            // 
            this.saveMapToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.saveMapToolStripMenuItem.Name = "saveMapToolStripMenuItem";
            this.saveMapToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.saveMapToolStripMenuItem.Text = "Save Map";
            this.saveMapToolStripMenuItem.Click += new System.EventHandler(this.saveMapToolStripMenuItem_Click);
            // 
            // newMapToolStripMenuItem
            // 
            this.newMapToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.newMapToolStripMenuItem.Name = "newMapToolStripMenuItem";
            this.newMapToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.newMapToolStripMenuItem.Text = "New Map";
            this.newMapToolStripMenuItem.ToolTipText = "Create a new, unconnected map.";
            this.newMapToolStripMenuItem.Click += new System.EventHandler(this.NewMapToolStripMenuItem_Click);
            // 
            // importMapToolStripMenuItem
            // 
            this.importMapToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.importMapToolStripMenuItem.Name = "importMapToolStripMenuItem";
            this.importMapToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.importMapToolStripMenuItem.Text = "Import Map";
            this.importMapToolStripMenuItem.Visible = false;
            this.importMapToolStripMenuItem.Click += new System.EventHandler(this.importMapToolStripMenuItem_Click);
            // 
            // exportMapToolStripMenuItem
            // 
            this.exportMapToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.exportMapToolStripMenuItem.Name = "exportMapToolStripMenuItem";
            this.exportMapToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.exportMapToolStripMenuItem.Text = "Export Map";
            this.exportMapToolStripMenuItem.Visible = false;
            this.exportMapToolStripMenuItem.Click += new System.EventHandler(this.exportMapToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.editToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Enabled = false;
            this.undoToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Enabled = false;
            this.cutToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Enabled = false;
            this.copyToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Enabled = false;
            this.pasteToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // fillToolStripMenuItem
            // 
            this.fillToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.fillToolStripMenuItem.Name = "fillToolStripMenuItem";
            this.fillToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.fillToolStripMenuItem.Text = "Fill";
            this.fillToolStripMenuItem.Click += new System.EventHandler(this.fillToolStripMenuItem_Click);
            // 
            // eraseLayerToolStripMenuItem
            // 
            this.eraseLayerToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.selectToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            this.mapGridToolStripMenuItem,
            this.hideEventsToolStripMenuItem,
            this.layersToolStripMenuItem});
            this.viewToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // hideDarknessToolStripMenuItem
            // 
            this.hideDarknessToolStripMenuItem.Checked = true;
            this.hideDarknessToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideDarknessToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.hideDarknessToolStripMenuItem.Name = "hideDarknessToolStripMenuItem";
            this.hideDarknessToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.hideDarknessToolStripMenuItem.Text = "Darkness";
            this.hideDarknessToolStripMenuItem.Click += new System.EventHandler(this.hideDarknessToolStripMenuItem_Click);
            // 
            // hideFogToolStripMenuItem
            // 
            this.hideFogToolStripMenuItem.Checked = true;
            this.hideFogToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideFogToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.hideFogToolStripMenuItem.Name = "hideFogToolStripMenuItem";
            this.hideFogToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.hideFogToolStripMenuItem.Text = "Fog";
            this.hideFogToolStripMenuItem.Click += new System.EventHandler(this.hideFogToolStripMenuItem_Click);
            // 
            // hideOverlayToolStripMenuItem
            // 
            this.hideOverlayToolStripMenuItem.Checked = true;
            this.hideOverlayToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideOverlayToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.hideOverlayToolStripMenuItem.Name = "hideOverlayToolStripMenuItem";
            this.hideOverlayToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.hideOverlayToolStripMenuItem.Text = "Overlay";
            this.hideOverlayToolStripMenuItem.Click += new System.EventHandler(this.hideOverlayToolStripMenuItem_Click);
            // 
            // hideTilePreviewToolStripMenuItem
            // 
            this.hideTilePreviewToolStripMenuItem.Checked = true;
            this.hideTilePreviewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideTilePreviewToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.hideTilePreviewToolStripMenuItem.Name = "hideTilePreviewToolStripMenuItem";
            this.hideTilePreviewToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.hideTilePreviewToolStripMenuItem.Text = "Tile Preview";
            this.hideTilePreviewToolStripMenuItem.Click += new System.EventHandler(this.hideTilePreviewToolStripMenuItem_Click);
            // 
            // hideResourcesToolStripMenuItem
            // 
            this.hideResourcesToolStripMenuItem.Checked = true;
            this.hideResourcesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideResourcesToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.hideResourcesToolStripMenuItem.Name = "hideResourcesToolStripMenuItem";
            this.hideResourcesToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.hideResourcesToolStripMenuItem.Text = "Resources";
            this.hideResourcesToolStripMenuItem.Click += new System.EventHandler(this.hideResourcesToolStripMenuItem_Click);
            // 
            // mapGridToolStripMenuItem
            // 
            this.mapGridToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.mapGridToolStripMenuItem.Name = "mapGridToolStripMenuItem";
            this.mapGridToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.mapGridToolStripMenuItem.Text = "Map Grid";
            this.mapGridToolStripMenuItem.Click += new System.EventHandler(this.mapGridToolStripMenuItem_Click);
            // 
            // hideEventsToolStripMenuItem
            // 
            this.hideEventsToolStripMenuItem.Checked = true;
            this.hideEventsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideEventsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.hideEventsToolStripMenuItem.Name = "hideEventsToolStripMenuItem";
            this.hideEventsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.hideEventsToolStripMenuItem.Text = "Events";
            this.hideEventsToolStripMenuItem.Click += new System.EventHandler(this.hideEventsToolStripMenuItem_Click);
            // 
            // layersToolStripMenuItem
            // 
            this.layersToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.layersToolStripMenuItem.Name = "layersToolStripMenuItem";
            this.layersToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.layersToolStripMenuItem.Text = "Layers";
            this.layersToolStripMenuItem.DropDownOpened += new System.EventHandler(this.layersToolStripMenuItem_DropDownOpened);
            // 
            // contentEditorsToolStripMenuItem
            // 
            this.contentEditorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.animationEditorToolStripMenuItem,
            this.classEditorToolStripMenuItem,
            this.commonEventEditorToolStripMenuItem,
            this.craftsEditorToolStripMenuItem,
            this.craftingTableEditorToolStripMenuItem,
            this.itemEditorToolStripMenuItem,
            this.npcEditorToolStripMenuItem,
            this.projectileEditorToolStripMenuItem,
            this.questEditorToolStripMenuItem,
            this.resourceEditorToolStripMenuItem,
            this.shopEditorToolStripMenuItem,
            this.spellEditorToolStripMenuItem,
            this.variableEditorToolStripMenuItem,
            this.timeEditorToolStripMenuItem});
            this.contentEditorsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.contentEditorsToolStripMenuItem.Name = "contentEditorsToolStripMenuItem";
            this.contentEditorsToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.contentEditorsToolStripMenuItem.Text = "Content Editors";
            // 
            // animationEditorToolStripMenuItem
            // 
            this.animationEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.animationEditorToolStripMenuItem.Name = "animationEditorToolStripMenuItem";
            this.animationEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.animationEditorToolStripMenuItem.Text = "Animation Editor";
            this.animationEditorToolStripMenuItem.Click += new System.EventHandler(this.animationEditorToolStripMenuItem_Click);
            // 
            // classEditorToolStripMenuItem
            // 
            this.classEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.classEditorToolStripMenuItem.Name = "classEditorToolStripMenuItem";
            this.classEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.classEditorToolStripMenuItem.Text = "Class Editor";
            this.classEditorToolStripMenuItem.Click += new System.EventHandler(this.classEditorToolStripMenuItem_Click);
            // 
            // commonEventEditorToolStripMenuItem
            // 
            this.commonEventEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.commonEventEditorToolStripMenuItem.Name = "commonEventEditorToolStripMenuItem";
            this.commonEventEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.commonEventEditorToolStripMenuItem.Text = "Common Event Editor";
            this.commonEventEditorToolStripMenuItem.Click += new System.EventHandler(this.commonEventEditorToolStripMenuItem_Click);
            // 
            // craftsEditorToolStripMenuItem
            // 
            this.craftsEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.craftsEditorToolStripMenuItem.Name = "craftsEditorToolStripMenuItem";
            this.craftsEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.craftsEditorToolStripMenuItem.Text = "Crafts Editor";
            this.craftsEditorToolStripMenuItem.Click += new System.EventHandler(this.craftsEditorToolStripMenuItem_Click);
            // 
            // craftingTableEditorToolStripMenuItem
            // 
            this.craftingTableEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.craftingTableEditorToolStripMenuItem.Name = "craftingTableEditorToolStripMenuItem";
            this.craftingTableEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.craftingTableEditorToolStripMenuItem.Text = "Crafting Table Editor";
            this.craftingTableEditorToolStripMenuItem.Click += new System.EventHandler(this.craftingTablesEditorToolStripMenuItem_Click);
            // 
            // itemEditorToolStripMenuItem
            // 
            this.itemEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.itemEditorToolStripMenuItem.Name = "itemEditorToolStripMenuItem";
            this.itemEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.itemEditorToolStripMenuItem.Text = "Item Editor";
            this.itemEditorToolStripMenuItem.Click += new System.EventHandler(this.itemEditorToolStripMenuItem_Click);
            // 
            // npcEditorToolStripMenuItem
            // 
            this.npcEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.npcEditorToolStripMenuItem.Name = "npcEditorToolStripMenuItem";
            this.npcEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.npcEditorToolStripMenuItem.Text = "Npc Editor";
            this.npcEditorToolStripMenuItem.Click += new System.EventHandler(this.npcEditorToolStripMenuItem_Click);
            // 
            // projectileEditorToolStripMenuItem
            // 
            this.projectileEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.projectileEditorToolStripMenuItem.Name = "projectileEditorToolStripMenuItem";
            this.projectileEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.projectileEditorToolStripMenuItem.Text = "Projectile Editor";
            this.projectileEditorToolStripMenuItem.Click += new System.EventHandler(this.projectileEditorToolStripMenuItem_Click);
            // 
            // questEditorToolStripMenuItem
            // 
            this.questEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.questEditorToolStripMenuItem.Name = "questEditorToolStripMenuItem";
            this.questEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.questEditorToolStripMenuItem.Text = "Quest Editor";
            this.questEditorToolStripMenuItem.Click += new System.EventHandler(this.questEditorToolStripMenuItem_Click);
            // 
            // resourceEditorToolStripMenuItem
            // 
            this.resourceEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.resourceEditorToolStripMenuItem.Name = "resourceEditorToolStripMenuItem";
            this.resourceEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.resourceEditorToolStripMenuItem.Text = "Resource Editor";
            this.resourceEditorToolStripMenuItem.Click += new System.EventHandler(this.resourceEditorToolStripMenuItem_Click);
            // 
            // shopEditorToolStripMenuItem
            // 
            this.shopEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.shopEditorToolStripMenuItem.Name = "shopEditorToolStripMenuItem";
            this.shopEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.shopEditorToolStripMenuItem.Text = "Shop Editor";
            this.shopEditorToolStripMenuItem.Click += new System.EventHandler(this.shopEditorToolStripMenuItem_Click);
            // 
            // spellEditorToolStripMenuItem
            // 
            this.spellEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.spellEditorToolStripMenuItem.Name = "spellEditorToolStripMenuItem";
            this.spellEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.spellEditorToolStripMenuItem.Text = "Spell Editor";
            this.spellEditorToolStripMenuItem.Click += new System.EventHandler(this.spellEditorToolStripMenuItem_Click);
            // 
            // variableEditorToolStripMenuItem
            // 
            this.variableEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.variableEditorToolStripMenuItem.Name = "variableEditorToolStripMenuItem";
            this.variableEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.variableEditorToolStripMenuItem.Text = "Variable Editor";
            this.variableEditorToolStripMenuItem.Click += new System.EventHandler(this.variableEditorToolStripMenuItem_Click);
            // 
            // timeEditorToolStripMenuItem
            // 
            this.timeEditorToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.timeEditorToolStripMenuItem.Name = "timeEditorToolStripMenuItem";
            this.timeEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.timeEditorToolStripMenuItem.Text = "Time Editor";
            this.timeEditorToolStripMenuItem.Click += new System.EventHandler(this.timeEditorToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.postQuestionToolStripMenuItem,
            this.reportBugToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // postQuestionToolStripMenuItem
            // 
            this.postQuestionToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.postQuestionToolStripMenuItem.Name = "postQuestionToolStripMenuItem";
            this.postQuestionToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.postQuestionToolStripMenuItem.Text = "Post Question";
            this.postQuestionToolStripMenuItem.Click += new System.EventHandler(this.postQuestionToolStripMenuItem_Click);
            // 
            // reportBugToolStripMenuItem
            // 
            this.reportBugToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.reportBugToolStripMenuItem.Name = "reportBugToolStripMenuItem";
            this.reportBugToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.reportBugToolStripMenuItem.Text = "Report Bug";
            this.reportBugToolStripMenuItem.Click += new System.EventHandler(this.reportBugToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.menuStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.contentEditorsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(3, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1186, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.packageUpdateToolStripMenuItem});
            this.toolsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // packageUpdateToolStripMenuItem
            // 
            this.packageUpdateToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.packageUpdateToolStripMenuItem.Name = "packageUpdateToolStripMenuItem";
            this.packageUpdateToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.packageUpdateToolStripMenuItem.Text = "Package Update";
            this.packageUpdateToolStripMenuItem.Click += new System.EventHandler(this.packageUpdateToolStripMenuItem_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1186, 675);
            this.Controls.Add(this.dockLeft);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Intersect Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMain_KeyDown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ToolStripMenuItem hihiToolStripMenuItem;
		private DarkStatusStrip statusStrip1;
		public ToolStripStatusLabel toolStripLabelCoords;
		private ToolStripSeparator toolStripSeparator7;
		public ToolStripStatusLabel toolStripLabelFPS;
		private ToolStripSeparator toolStripSeparator8;
		public ToolStripStatusLabel toolStripLabelDebug;
		public ToolStripStatusLabel toolStripLabelRevision;
		private ToolStripSeparator toolStripSeparator9;
		private WeifenLuo.WinFormsUI.Docking.DockPanel dockLeft;
		private ToolStripButton toolStripBtnNewMap;
		private ToolStripButton toolStripBtnSaveMap;
		private ToolStripSeparator toolStripSeparator1;
		public ToolStripButton toolStripBtnCut;
		public ToolStripButton toolStripBtnCopy;
		public ToolStripButton toolStripBtnPaste;
		private ToolStripSeparator toolStripSeparator2;
		public ToolStripButton toolStripBtnUndo;
		public ToolStripButton toolStripBtnRedo;
		private ToolStripSeparator toolStripSeparator3;
		public ToolStripButton toolStripBtnPen;
		public ToolStripButton toolStripBtnSelect;
		public ToolStripButton toolStripBtnRect;
		private ToolStripSeparator toolStripSeparator4;
		public ToolStripButton toolStripBtnFill;
		public ToolStripButton toolStripBtnErase;
		private ToolStripButton toolStripBtnEyeDrop;
		private ToolStripSeparator toolStripSeparator5;
		private ToolStripDropDownButton toolStripTimeButton;
		private ToolStripSeparator toolStripSeparator6;
		private ToolStripButton toolStripBtnScreenshot;
		private ToolStripSeparator toolStripSeparator10;
		private ToolStripButton toolStripBtnRun;
		private ToolStripButton toolStripButtonBug;
		private ToolStripSeparator toolStripSeparator11;
		private ToolStripButton toolStripButtonQuestion;
		private DarkToolStrip toolStrip1;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem saveMapToolStripMenuItem;
		private ToolStripMenuItem newMapToolStripMenuItem;
		private ToolStripMenuItem importMapToolStripMenuItem;
		private ToolStripMenuItem exportMapToolStripMenuItem;
		private ToolStripMenuItem optionsToolStripMenuItem;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripMenuItem editToolStripMenuItem;
		private ToolStripMenuItem undoToolStripMenuItem;
		private ToolStripMenuItem redoToolStripMenuItem;
		private ToolStripMenuItem cutToolStripMenuItem;
		private ToolStripMenuItem copyToolStripMenuItem;
		private ToolStripMenuItem pasteToolStripMenuItem;
		public ToolStripMenuItem fillToolStripMenuItem;
		public ToolStripMenuItem eraseLayerToolStripMenuItem;
		private ToolStripMenuItem selectToolStripMenuItem;
		private ToolStripMenuItem allLayersToolStripMenuItem;
		private ToolStripMenuItem currentLayerOnlyToolStripMenuItem;
		private ToolStripMenuItem viewToolStripMenuItem;
		private ToolStripMenuItem hideDarknessToolStripMenuItem;
		private ToolStripMenuItem hideFogToolStripMenuItem;
		private ToolStripMenuItem hideOverlayToolStripMenuItem;
		private ToolStripMenuItem hideTilePreviewToolStripMenuItem;
		private ToolStripMenuItem hideResourcesToolStripMenuItem;
		private ToolStripMenuItem mapGridToolStripMenuItem;
		private ToolStripMenuItem contentEditorsToolStripMenuItem;
		private ToolStripMenuItem animationEditorToolStripMenuItem;
		private ToolStripMenuItem classEditorToolStripMenuItem;
		private ToolStripMenuItem commonEventEditorToolStripMenuItem;
		private ToolStripMenuItem craftingTableEditorToolStripMenuItem;
		private ToolStripMenuItem itemEditorToolStripMenuItem;
		private ToolStripMenuItem npcEditorToolStripMenuItem;
		private ToolStripMenuItem projectileEditorToolStripMenuItem;
		private ToolStripMenuItem questEditorToolStripMenuItem;
		private ToolStripMenuItem resourceEditorToolStripMenuItem;
		private ToolStripMenuItem shopEditorToolStripMenuItem;
		private ToolStripMenuItem spellEditorToolStripMenuItem;
		private ToolStripMenuItem variableEditorToolStripMenuItem;
		private ToolStripMenuItem timeEditorToolStripMenuItem;
		private ToolStripMenuItem helpToolStripMenuItem;
		private ToolStripMenuItem postQuestionToolStripMenuItem;
		private ToolStripMenuItem reportBugToolStripMenuItem;
		private ToolStripMenuItem aboutToolStripMenuItem;
		private DarkMenuStrip menuStrip;
		private ToolStripMenuItem toolsToolStripMenuItem;
		private ToolStripButton toolStripBtnFlipHorizontal;
		private ToolStripButton toolStripBtnFlipVertical;
		private ToolStripSeparator toolStripSeparator13;
		private ToolStripMenuItem craftsEditorToolStripMenuItem;
        private ToolStripMenuItem packageUpdateToolStripMenuItem;
        private ToolStripMenuItem layersToolStripMenuItem;
        private ToolStripMenuItem hideEventsToolStripMenuItem;
    }
}