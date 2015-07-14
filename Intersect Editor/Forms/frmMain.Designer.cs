using System.ComponentModel;
using System.Windows.Forms;

namespace Intersect_Editor.Forms
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eraseLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layerMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideDarknessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideFogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideOverlayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentEditorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.npcEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spellEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resourceEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpLayer = new System.Windows.Forms.GroupBox();
            this.cmbAutotile = new System.Windows.Forms.ComboBox();
            this.lblCurLayer = new System.Windows.Forms.Label();
            this.cmbTilesets = new System.Windows.Forms.ComboBox();
            this.grpMapList = new System.Windows.Forms.GroupBox();
            this.lstGameMaps = new System.Windows.Forms.ListBox();
            this.lblCloseMapList = new System.Windows.Forms.Label();
            this.grpLightEditor = new System.Windows.Forms.GroupBox();
            this.scrlLightRange = new System.Windows.Forms.HScrollBar();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLightRange = new System.Windows.Forms.TextBox();
            this.scrlLightIntensity = new System.Windows.Forms.HScrollBar();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLightIntensity = new System.Windows.Forms.TextBox();
            this.txtLightOffsetY = new System.Windows.Forms.TextBox();
            this.txtLightOffsetX = new System.Windows.Forms.TextBox();
            this.lblOffsetY = new System.Windows.Forms.Label();
            this.lblOffsetX = new System.Windows.Forms.Label();
            this.btnLightEditorRevert = new System.Windows.Forms.Button();
            this.btnLightEditorClose = new System.Windows.Forms.Button();
            this.grpAttributes = new System.Windows.Forms.GroupBox();
            this.grpResource = new System.Windows.Forms.GroupBox();
            this.lblResource = new System.Windows.Forms.Label();
            this.scrlResource = new System.Windows.Forms.HScrollBar();
            this.rbResource = new System.Windows.Forms.RadioButton();
            this.grpSound = new System.Windows.Forms.GroupBox();
            this.cmbMapAttributeSound = new System.Windows.Forms.ComboBox();
            this.lblSoundDistance = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.scrlSoundDistance = new System.Windows.Forms.HScrollBar();
            this.rbSound = new System.Windows.Forms.RadioButton();
            this.rbWarp = new System.Windows.Forms.RadioButton();
            this.rbNPCAvoid = new System.Windows.Forms.RadioButton();
            this.rbZDimension = new System.Windows.Forms.RadioButton();
            this.rbItem = new System.Windows.Forms.RadioButton();
            this.rbBlocked = new System.Windows.Forms.RadioButton();
            this.grpItem = new System.Windows.Forms.GroupBox();
            this.lblMaxItemAmount = new System.Windows.Forms.Label();
            this.lblMapItem = new System.Windows.Forms.Label();
            this.scrlMaxItemVal = new System.Windows.Forms.HScrollBar();
            this.scrlMapItem = new System.Windows.Forms.HScrollBar();
            this.grpWarp = new System.Windows.Forms.GroupBox();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblMap = new System.Windows.Forms.Label();
            this.scrlX = new System.Windows.Forms.HScrollBar();
            this.scrlY = new System.Windows.Forms.HScrollBar();
            this.scrlMap = new System.Windows.Forms.HScrollBar();
            this.grpZDimension = new System.Windows.Forms.GroupBox();
            this.grpGateway = new System.Windows.Forms.GroupBox();
            this.rbGateway2 = new System.Windows.Forms.RadioButton();
            this.rbGateway1 = new System.Windows.Forms.RadioButton();
            this.rbGatewayNone = new System.Windows.Forms.RadioButton();
            this.grpDimBlock = new System.Windows.Forms.GroupBox();
            this.rbBlock2 = new System.Windows.Forms.RadioButton();
            this.rbBlock1 = new System.Windows.Forms.RadioButton();
            this.rbBlockNone = new System.Windows.Forms.RadioButton();
            this.grpMapProperties = new System.Windows.Forms.GroupBox();
            this.scrlBrightness = new System.Windows.Forms.HScrollBar();
            this.lblBrightness = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.scrlMapAlpha = new System.Windows.Forms.HScrollBar();
            this.lblMapAlpha = new System.Windows.Forms.Label();
            this.scrlMapBlue = new System.Windows.Forms.HScrollBar();
            this.lblMapBlue = new System.Windows.Forms.Label();
            this.scrlMapGreen = new System.Windows.Forms.HScrollBar();
            this.lblMapGreen = new System.Windows.Forms.Label();
            this.scrlMapRed = new System.Windows.Forms.HScrollBar();
            this.lblMapRed = new System.Windows.Forms.Label();
            this.scrlFogIntensity = new System.Windows.Forms.HScrollBar();
            this.lblFogIntensity = new System.Windows.Forms.Label();
            this.scrlFogVertical = new System.Windows.Forms.HScrollBar();
            this.scrlFogHorizontal = new System.Windows.Forms.HScrollBar();
            this.lblFogVerticalSpeed = new System.Windows.Forms.Label();
            this.lblFogHorizontalSpeed = new System.Windows.Forms.Label();
            this.cmbFogs = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbPanorama = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbMapSound = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbMapMusic = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCloseMapProperties = new System.Windows.Forms.Label();
            this.grpMapNPCS = new System.Windows.Forms.GroupBox();
            this.grpManage = new System.Windows.Forms.GroupBox();
            this.btnRemoveMapNpc = new System.Windows.Forms.Button();
            this.btnAddMapNpc = new System.Windows.Forms.Button();
            this.cmbNpc = new System.Windows.Forms.ComboBox();
            this.grpSpawnLoc = new System.Windows.Forms.GroupBox();
            this.cmbDir = new System.Windows.Forms.ComboBox();
            this.lblDir = new System.Windows.Forms.Label();
            this.rbRandom = new System.Windows.Forms.RadioButton();
            this.rbDeclared = new System.Windows.Forms.RadioButton();
            this.lstMapNpcs = new System.Windows.Forms.ListBox();
            this.chkIndoors = new System.Windows.Forms.CheckBox();
            this.txtMapName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCloseProperties = new System.Windows.Forms.Button();
            this.spltContainer = new System.Windows.Forms.SplitContainer();
            this.picMap = new System.Windows.Forms.PictureBox();
            this.picTileset = new System.Windows.Forms.PictureBox();
            this.pnlTilesetContainer = new System.Windows.Forms.Panel();
            this.menuStrip.SuspendLayout();
            this.grpLayer.SuspendLayout();
            this.grpMapList.SuspendLayout();
            this.grpLightEditor.SuspendLayout();
            this.grpAttributes.SuspendLayout();
            this.grpResource.SuspendLayout();
            this.grpSound.SuspendLayout();
            this.grpItem.SuspendLayout();
            this.grpWarp.SuspendLayout();
            this.grpZDimension.SuspendLayout();
            this.grpGateway.SuspendLayout();
            this.grpDimBlock.SuspendLayout();
            this.grpMapProperties.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpMapNPCS.SuspendLayout();
            this.grpManage.SuspendLayout();
            this.grpSpawnLoc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltContainer)).BeginInit();
            this.spltContainer.Panel1.SuspendLayout();
            this.spltContainer.Panel2.SuspendLayout();
            this.spltContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTileset)).BeginInit();
            this.pnlTilesetContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.layerMenu,
            this.viewToolStripMenuItem,
            this.contentEditorsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1020, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveMapToolStripMenuItem,
            this.newMapToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // saveMapToolStripMenuItem
            // 
            this.saveMapToolStripMenuItem.Name = "saveMapToolStripMenuItem";
            this.saveMapToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.saveMapToolStripMenuItem.Text = "Save Map";
            this.saveMapToolStripMenuItem.Click += new System.EventHandler(this.saveMapToolStripMenuItem_Click);
            // 
            // newMapToolStripMenuItem
            // 
            this.newMapToolStripMenuItem.Name = "newMapToolStripMenuItem";
            this.newMapToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.newMapToolStripMenuItem.Text = "New Map";
            this.newMapToolStripMenuItem.ToolTipText = "Create a new, unconnected map.";
            this.newMapToolStripMenuItem.Click += new System.EventHandler(this.newMapToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fillToolStripMenuItem,
            this.eraseLayerToolStripMenuItem,
            this.mapPropertiesToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // fillToolStripMenuItem
            // 
            this.fillToolStripMenuItem.Name = "fillToolStripMenuItem";
            this.fillToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.fillToolStripMenuItem.Text = "Fill";
            this.fillToolStripMenuItem.Click += new System.EventHandler(this.fillToolStripMenuItem_Click);
            // 
            // eraseLayerToolStripMenuItem
            // 
            this.eraseLayerToolStripMenuItem.Name = "eraseLayerToolStripMenuItem";
            this.eraseLayerToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.eraseLayerToolStripMenuItem.Text = "Erase Layer";
            this.eraseLayerToolStripMenuItem.Click += new System.EventHandler(this.eraseLayerToolStripMenuItem_Click);
            // 
            // mapPropertiesToolStripMenuItem
            // 
            this.mapPropertiesToolStripMenuItem.Name = "mapPropertiesToolStripMenuItem";
            this.mapPropertiesToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.mapPropertiesToolStripMenuItem.Text = "Map Properties";
            this.mapPropertiesToolStripMenuItem.Click += new System.EventHandler(this.mapPropertiesToolStripMenuItem_Click);
            // 
            // layerMenu
            // 
            this.layerMenu.Name = "layerMenu";
            this.layerMenu.Size = new System.Drawing.Size(52, 20);
            this.layerMenu.Text = "Layers";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideDarknessToolStripMenuItem,
            this.hideFogToolStripMenuItem,
            this.hideOverlayToolStripMenuItem,
            this.mapListToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // hideDarknessToolStripMenuItem
            // 
            this.hideDarknessToolStripMenuItem.Name = "hideDarknessToolStripMenuItem";
            this.hideDarknessToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.hideDarknessToolStripMenuItem.Text = "Hide Darkness";
            this.hideDarknessToolStripMenuItem.Click += new System.EventHandler(this.hideDarknessToolStripMenuItem_Click);
            // 
            // hideFogToolStripMenuItem
            // 
            this.hideFogToolStripMenuItem.Name = "hideFogToolStripMenuItem";
            this.hideFogToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.hideFogToolStripMenuItem.Text = "Hide Fog";
            this.hideFogToolStripMenuItem.Click += new System.EventHandler(this.hideFogToolStripMenuItem_Click);
            // 
            // hideOverlayToolStripMenuItem
            // 
            this.hideOverlayToolStripMenuItem.Name = "hideOverlayToolStripMenuItem";
            this.hideOverlayToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.hideOverlayToolStripMenuItem.Text = "Hide Overlay";
            this.hideOverlayToolStripMenuItem.Click += new System.EventHandler(this.hideOverlayToolStripMenuItem_Click);
            // 
            // mapListToolStripMenuItem
            // 
            this.mapListToolStripMenuItem.Name = "mapListToolStripMenuItem";
            this.mapListToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.mapListToolStripMenuItem.Text = "Map List";
            this.mapListToolStripMenuItem.Click += new System.EventHandler(this.mapListToolStripMenuItem_Click);
            // 
            // contentEditorsToolStripMenuItem
            // 
            this.contentEditorsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemEditorToolStripMenuItem,
            this.npcEditorToolStripMenuItem,
            this.spellEditorToolStripMenuItem,
            this.animationEditorToolStripMenuItem,
            this.resourceEditorToolStripMenuItem});
            this.contentEditorsToolStripMenuItem.Name = "contentEditorsToolStripMenuItem";
            this.contentEditorsToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.contentEditorsToolStripMenuItem.Text = "Content Editors";
            // 
            // itemEditorToolStripMenuItem
            // 
            this.itemEditorToolStripMenuItem.Name = "itemEditorToolStripMenuItem";
            this.itemEditorToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.itemEditorToolStripMenuItem.Text = "Item Editor";
            this.itemEditorToolStripMenuItem.Click += new System.EventHandler(this.itemEditorToolStripMenuItem_Click);
            // 
            // npcEditorToolStripMenuItem
            // 
            this.npcEditorToolStripMenuItem.Name = "npcEditorToolStripMenuItem";
            this.npcEditorToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.npcEditorToolStripMenuItem.Text = "Npc Editor";
            this.npcEditorToolStripMenuItem.Click += new System.EventHandler(this.npcEditorToolStripMenuItem_Click);
            // 
            // spellEditorToolStripMenuItem
            // 
            this.spellEditorToolStripMenuItem.Name = "spellEditorToolStripMenuItem";
            this.spellEditorToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.spellEditorToolStripMenuItem.Text = "Spell Editor";
            this.spellEditorToolStripMenuItem.Click += new System.EventHandler(this.spellEditorToolStripMenuItem_Click);
            // 
            // animationEditorToolStripMenuItem
            // 
            this.animationEditorToolStripMenuItem.Name = "animationEditorToolStripMenuItem";
            this.animationEditorToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.animationEditorToolStripMenuItem.Text = "Animation Editor";
            this.animationEditorToolStripMenuItem.Click += new System.EventHandler(this.animationEditorToolStripMenuItem_Click);
            // 
            // resourceEditorToolStripMenuItem
            // 
            this.resourceEditorToolStripMenuItem.Name = "resourceEditorToolStripMenuItem";
            this.resourceEditorToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.resourceEditorToolStripMenuItem.Text = "Resource Editor";
            this.resourceEditorToolStripMenuItem.Click += new System.EventHandler(this.resourceEditorToolStripMenuItem_Click);
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
            // grpLayer
            // 
            this.grpLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLayer.Controls.Add(this.pnlTilesetContainer);
            this.grpLayer.Controls.Add(this.cmbAutotile);
            this.grpLayer.Controls.Add(this.lblCurLayer);
            this.grpLayer.Controls.Add(this.cmbTilesets);
            this.grpLayer.Location = new System.Drawing.Point(0, 0);
            this.grpLayer.Name = "grpLayer";
            this.grpLayer.Size = new System.Drawing.Size(290, 652);
            this.grpLayer.TabIndex = 5;
            this.grpLayer.TabStop = false;
            this.grpLayer.Text = "Tileset Container";
            // 
            // cmbAutotile
            // 
            this.cmbAutotile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutotile.FormattingEnabled = true;
            this.cmbAutotile.Items.AddRange(new object[] {
            "Normal",
            "Autotile",
            "Fake",
            "Animated",
            "Cliff",
            "Waterfall"});
            this.cmbAutotile.Location = new System.Drawing.Point(180, 47);
            this.cmbAutotile.Name = "cmbAutotile";
            this.cmbAutotile.Size = new System.Drawing.Size(104, 21);
            this.cmbAutotile.TabIndex = 9;
            this.cmbAutotile.SelectedIndexChanged += new System.EventHandler(this.cmbAutotile_SelectedIndexChanged);
            // 
            // lblCurLayer
            // 
            this.lblCurLayer.AutoSize = true;
            this.lblCurLayer.Location = new System.Drawing.Point(6, 16);
            this.lblCurLayer.Name = "lblCurLayer";
            this.lblCurLayer.Size = new System.Drawing.Size(49, 13);
            this.lblCurLayer.TabIndex = 8;
            this.lblCurLayer.Text = "Layer #1";
            // 
            // cmbTilesets
            // 
            this.cmbTilesets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTilesets.FormattingEnabled = true;
            this.cmbTilesets.Location = new System.Drawing.Point(7, 47);
            this.cmbTilesets.Name = "cmbTilesets";
            this.cmbTilesets.Size = new System.Drawing.Size(167, 21);
            this.cmbTilesets.TabIndex = 7;
            this.cmbTilesets.SelectedIndexChanged += new System.EventHandler(this.cmbTilesets_SelectedIndexChanged);
            // 
            // grpMapList
            // 
            this.grpMapList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpMapList.Controls.Add(this.lstGameMaps);
            this.grpMapList.Controls.Add(this.lblCloseMapList);
            this.grpMapList.Location = new System.Drawing.Point(0, 0);
            this.grpMapList.Name = "grpMapList";
            this.grpMapList.Size = new System.Drawing.Size(290, 630);
            this.grpMapList.TabIndex = 6;
            this.grpMapList.TabStop = false;
            this.grpMapList.Text = "Map List";
            this.grpMapList.Visible = false;
            // 
            // lstGameMaps
            // 
            this.lstGameMaps.FormattingEnabled = true;
            this.lstGameMaps.Location = new System.Drawing.Point(11, 20);
            this.lstGameMaps.Name = "lstGameMaps";
            this.lstGameMaps.Size = new System.Drawing.Size(273, 602);
            this.lstGameMaps.TabIndex = 1;
            this.lstGameMaps.DoubleClick += new System.EventHandler(this.lstGameMaps_DoubleClick);
            // 
            // lblCloseMapList
            // 
            this.lblCloseMapList.AutoSize = true;
            this.lblCloseMapList.Location = new System.Drawing.Point(273, 7);
            this.lblCloseMapList.Name = "lblCloseMapList";
            this.lblCloseMapList.Size = new System.Drawing.Size(14, 13);
            this.lblCloseMapList.TabIndex = 0;
            this.lblCloseMapList.Text = "X";
            this.lblCloseMapList.Click += new System.EventHandler(this.lblCloseMapList_Click);
            // 
            // grpLightEditor
            // 
            this.grpLightEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpLightEditor.Controls.Add(this.scrlLightRange);
            this.grpLightEditor.Controls.Add(this.label2);
            this.grpLightEditor.Controls.Add(this.txtLightRange);
            this.grpLightEditor.Controls.Add(this.scrlLightIntensity);
            this.grpLightEditor.Controls.Add(this.label3);
            this.grpLightEditor.Controls.Add(this.txtLightIntensity);
            this.grpLightEditor.Controls.Add(this.txtLightOffsetY);
            this.grpLightEditor.Controls.Add(this.txtLightOffsetX);
            this.grpLightEditor.Controls.Add(this.lblOffsetY);
            this.grpLightEditor.Controls.Add(this.lblOffsetX);
            this.grpLightEditor.Controls.Add(this.btnLightEditorRevert);
            this.grpLightEditor.Controls.Add(this.btnLightEditorClose);
            this.grpLightEditor.Location = new System.Drawing.Point(0, 0);
            this.grpLightEditor.Name = "grpLightEditor";
            this.grpLightEditor.Size = new System.Drawing.Size(290, 630);
            this.grpLightEditor.TabIndex = 7;
            this.grpLightEditor.TabStop = false;
            this.grpLightEditor.Text = "Light Editor";
            this.grpLightEditor.Visible = false;
            // 
            // scrlLightRange
            // 
            this.scrlLightRange.Location = new System.Drawing.Point(61, 132);
            this.scrlLightRange.Maximum = 179;
            this.scrlLightRange.Minimum = 2;
            this.scrlLightRange.Name = "scrlLightRange";
            this.scrlLightRange.Size = new System.Drawing.Size(225, 17);
            this.scrlLightRange.TabIndex = 23;
            this.scrlLightRange.Value = 2;
            this.scrlLightRange.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlLightRange_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Range:";
            // 
            // txtLightRange
            // 
            this.txtLightRange.Location = new System.Drawing.Point(61, 112);
            this.txtLightRange.Name = "txtLightRange";
            this.txtLightRange.Size = new System.Drawing.Size(225, 20);
            this.txtLightRange.TabIndex = 21;
            this.txtLightRange.Validated += new System.EventHandler(this.txtLightRange_TextChanged);
            // 
            // scrlLightIntensity
            // 
            this.scrlLightIntensity.LargeChange = 1000;
            this.scrlLightIntensity.Location = new System.Drawing.Point(61, 85);
            this.scrlLightIntensity.Maximum = 10000;
            this.scrlLightIntensity.Minimum = 1;
            this.scrlLightIntensity.Name = "scrlLightIntensity";
            this.scrlLightIntensity.Size = new System.Drawing.Size(225, 17);
            this.scrlLightIntensity.TabIndex = 20;
            this.scrlLightIntensity.Value = 1;
            this.scrlLightIntensity.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlLightIntensity_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Intensity:";
            // 
            // txtLightIntensity
            // 
            this.txtLightIntensity.Location = new System.Drawing.Point(61, 65);
            this.txtLightIntensity.Name = "txtLightIntensity";
            this.txtLightIntensity.Size = new System.Drawing.Size(225, 20);
            this.txtLightIntensity.TabIndex = 18;
            this.txtLightIntensity.Validated += new System.EventHandler(this.txtLightIntensity_TextChanged);
            // 
            // txtLightOffsetY
            // 
            this.txtLightOffsetY.Location = new System.Drawing.Point(61, 39);
            this.txtLightOffsetY.Name = "txtLightOffsetY";
            this.txtLightOffsetY.Size = new System.Drawing.Size(114, 20);
            this.txtLightOffsetY.TabIndex = 17;
            this.txtLightOffsetY.Validated += new System.EventHandler(this.txtLightOffsetY_TextChanged);
            // 
            // txtLightOffsetX
            // 
            this.txtLightOffsetX.Location = new System.Drawing.Point(61, 17);
            this.txtLightOffsetX.Name = "txtLightOffsetX";
            this.txtLightOffsetX.Size = new System.Drawing.Size(114, 20);
            this.txtLightOffsetX.TabIndex = 16;
            this.txtLightOffsetX.Validated += new System.EventHandler(this.txtLightOffsetX_TextChanged);
            // 
            // lblOffsetY
            // 
            this.lblOffsetY.AutoSize = true;
            this.lblOffsetY.Location = new System.Drawing.Point(6, 42);
            this.lblOffsetY.Name = "lblOffsetY";
            this.lblOffsetY.Size = new System.Drawing.Size(48, 13);
            this.lblOffsetY.TabIndex = 15;
            this.lblOffsetY.Text = "Offset Y:";
            // 
            // lblOffsetX
            // 
            this.lblOffsetX.AutoSize = true;
            this.lblOffsetX.Location = new System.Drawing.Point(7, 20);
            this.lblOffsetX.Name = "lblOffsetX";
            this.lblOffsetX.Size = new System.Drawing.Size(48, 13);
            this.lblOffsetX.TabIndex = 14;
            this.lblOffsetX.Text = "Offset X:";
            // 
            // btnLightEditorRevert
            // 
            this.btnLightEditorRevert.Location = new System.Drawing.Point(130, 218);
            this.btnLightEditorRevert.Name = "btnLightEditorRevert";
            this.btnLightEditorRevert.Size = new System.Drawing.Size(75, 23);
            this.btnLightEditorRevert.TabIndex = 13;
            this.btnLightEditorRevert.Text = "Revert";
            this.btnLightEditorRevert.UseVisualStyleBackColor = true;
            this.btnLightEditorRevert.Click += new System.EventHandler(this.btnLightEditorRevert_Click);
            // 
            // btnLightEditorClose
            // 
            this.btnLightEditorClose.Location = new System.Drawing.Point(211, 218);
            this.btnLightEditorClose.Name = "btnLightEditorClose";
            this.btnLightEditorClose.Size = new System.Drawing.Size(75, 23);
            this.btnLightEditorClose.TabIndex = 12;
            this.btnLightEditorClose.Text = "Close";
            this.btnLightEditorClose.UseVisualStyleBackColor = true;
            this.btnLightEditorClose.Click += new System.EventHandler(this.btnLightEditorClose_Click);
            // 
            // grpAttributes
            // 
            this.grpAttributes.Controls.Add(this.grpResource);
            this.grpAttributes.Controls.Add(this.rbResource);
            this.grpAttributes.Controls.Add(this.grpSound);
            this.grpAttributes.Controls.Add(this.rbSound);
            this.grpAttributes.Controls.Add(this.rbWarp);
            this.grpAttributes.Controls.Add(this.rbNPCAvoid);
            this.grpAttributes.Controls.Add(this.rbZDimension);
            this.grpAttributes.Controls.Add(this.rbItem);
            this.grpAttributes.Controls.Add(this.rbBlocked);
            this.grpAttributes.Controls.Add(this.grpItem);
            this.grpAttributes.Controls.Add(this.grpWarp);
            this.grpAttributes.Controls.Add(this.grpZDimension);
            this.grpAttributes.Location = new System.Drawing.Point(0, 0);
            this.grpAttributes.Name = "grpAttributes";
            this.grpAttributes.Size = new System.Drawing.Size(292, 632);
            this.grpAttributes.TabIndex = 8;
            this.grpAttributes.TabStop = false;
            this.grpAttributes.Text = "Attributes";
            this.grpAttributes.Visible = false;
            // 
            // grpResource
            // 
            this.grpResource.Controls.Add(this.lblResource);
            this.grpResource.Controls.Add(this.scrlResource);
            this.grpResource.Location = new System.Drawing.Point(1, 522);
            this.grpResource.Name = "grpResource";
            this.grpResource.Size = new System.Drawing.Size(280, 100);
            this.grpResource.TabIndex = 19;
            this.grpResource.TabStop = false;
            this.grpResource.Text = "Resource";
            this.grpResource.Visible = false;
            // 
            // lblResource
            // 
            this.lblResource.AutoSize = true;
            this.lblResource.Location = new System.Drawing.Point(14, 31);
            this.lblResource.Name = "lblResource";
            this.lblResource.Size = new System.Drawing.Size(94, 13);
            this.lblResource.TabIndex = 10;
            this.lblResource.Text = "Resource: 0 None";
            // 
            // scrlResource
            // 
            this.scrlResource.LargeChange = 1;
            this.scrlResource.Location = new System.Drawing.Point(17, 53);
            this.scrlResource.Maximum = 255;
            this.scrlResource.Minimum = -1;
            this.scrlResource.Name = "scrlResource";
            this.scrlResource.Size = new System.Drawing.Size(249, 17);
            this.scrlResource.TabIndex = 9;
            this.scrlResource.Value = -1;
            this.scrlResource.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlResource_Scroll);
            // 
            // rbResource
            // 
            this.rbResource.AutoSize = true;
            this.rbResource.Location = new System.Drawing.Point(11, 159);
            this.rbResource.Name = "rbResource";
            this.rbResource.Size = new System.Drawing.Size(71, 17);
            this.rbResource.TabIndex = 17;
            this.rbResource.Text = "Resource";
            this.rbResource.UseVisualStyleBackColor = true;
            this.rbResource.CheckedChanged += new System.EventHandler(this.rbResource_CheckedChanged);
            // 
            // grpSound
            // 
            this.grpSound.Controls.Add(this.cmbMapAttributeSound);
            this.grpSound.Controls.Add(this.lblSoundDistance);
            this.grpSound.Controls.Add(this.label7);
            this.grpSound.Controls.Add(this.scrlSoundDistance);
            this.grpSound.Location = new System.Drawing.Point(7, 484);
            this.grpSound.Name = "grpSound";
            this.grpSound.Size = new System.Drawing.Size(278, 132);
            this.grpSound.TabIndex = 15;
            this.grpSound.TabStop = false;
            this.grpSound.Text = "Map Sound";
            this.grpSound.Visible = false;
            // 
            // cmbMapAttributeSound
            // 
            this.cmbMapAttributeSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapAttributeSound.FormattingEnabled = true;
            this.cmbMapAttributeSound.Location = new System.Drawing.Point(16, 41);
            this.cmbMapAttributeSound.Name = "cmbMapAttributeSound";
            this.cmbMapAttributeSound.Size = new System.Drawing.Size(249, 21);
            this.cmbMapAttributeSound.TabIndex = 9;
            // 
            // lblSoundDistance
            // 
            this.lblSoundDistance.AutoSize = true;
            this.lblSoundDistance.Location = new System.Drawing.Point(13, 72);
            this.lblSoundDistance.Name = "lblSoundDistance";
            this.lblSoundDistance.Size = new System.Drawing.Size(92, 13);
            this.lblSoundDistance.TabIndex = 8;
            this.lblSoundDistance.Text = "Distance: 1 Tile(s)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Sound:";
            // 
            // scrlSoundDistance
            // 
            this.scrlSoundDistance.LargeChange = 1;
            this.scrlSoundDistance.Location = new System.Drawing.Point(16, 94);
            this.scrlSoundDistance.Maximum = 15;
            this.scrlSoundDistance.Minimum = 1;
            this.scrlSoundDistance.Name = "scrlSoundDistance";
            this.scrlSoundDistance.Size = new System.Drawing.Size(249, 17);
            this.scrlSoundDistance.TabIndex = 6;
            this.scrlSoundDistance.Value = 1;
            this.scrlSoundDistance.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSoundDistance_Scroll);
            // 
            // rbSound
            // 
            this.rbSound.AutoSize = true;
            this.rbSound.Location = new System.Drawing.Point(11, 136);
            this.rbSound.Name = "rbSound";
            this.rbSound.Size = new System.Drawing.Size(80, 17);
            this.rbSound.TabIndex = 14;
            this.rbSound.Text = "Map Sound";
            this.rbSound.UseVisualStyleBackColor = true;
            this.rbSound.CheckedChanged += new System.EventHandler(this.rbSound_CheckedChanged);
            // 
            // rbWarp
            // 
            this.rbWarp.AutoSize = true;
            this.rbWarp.Location = new System.Drawing.Point(11, 89);
            this.rbWarp.Name = "rbWarp";
            this.rbWarp.Size = new System.Drawing.Size(51, 17);
            this.rbWarp.TabIndex = 6;
            this.rbWarp.Text = "Warp";
            this.rbWarp.UseVisualStyleBackColor = true;
            this.rbWarp.CheckedChanged += new System.EventHandler(this.rbWarp_CheckedChanged);
            // 
            // rbNPCAvoid
            // 
            this.rbNPCAvoid.AutoSize = true;
            this.rbNPCAvoid.Location = new System.Drawing.Point(11, 66);
            this.rbNPCAvoid.Name = "rbNPCAvoid";
            this.rbNPCAvoid.Size = new System.Drawing.Size(77, 17);
            this.rbNPCAvoid.TabIndex = 5;
            this.rbNPCAvoid.Text = "NPC Avoid";
            this.rbNPCAvoid.UseVisualStyleBackColor = true;
            this.rbNPCAvoid.CheckedChanged += new System.EventHandler(this.rbNPCAvoid_CheckedChanged);
            // 
            // rbZDimension
            // 
            this.rbZDimension.AutoSize = true;
            this.rbZDimension.Location = new System.Drawing.Point(11, 42);
            this.rbZDimension.Name = "rbZDimension";
            this.rbZDimension.Size = new System.Drawing.Size(84, 17);
            this.rbZDimension.TabIndex = 4;
            this.rbZDimension.Text = "Z-Dimension";
            this.rbZDimension.UseVisualStyleBackColor = true;
            this.rbZDimension.CheckedChanged += new System.EventHandler(this.rbZDimension_CheckedChanged);
            // 
            // rbItem
            // 
            this.rbItem.AutoSize = true;
            this.rbItem.Location = new System.Drawing.Point(11, 112);
            this.rbItem.Name = "rbItem";
            this.rbItem.Size = new System.Drawing.Size(81, 17);
            this.rbItem.TabIndex = 1;
            this.rbItem.Text = "Item Spawn";
            this.rbItem.UseVisualStyleBackColor = true;
            this.rbItem.CheckedChanged += new System.EventHandler(this.rbItem_CheckedChanged);
            // 
            // rbBlocked
            // 
            this.rbBlocked.AutoSize = true;
            this.rbBlocked.Checked = true;
            this.rbBlocked.Location = new System.Drawing.Point(12, 20);
            this.rbBlocked.Name = "rbBlocked";
            this.rbBlocked.Size = new System.Drawing.Size(64, 17);
            this.rbBlocked.TabIndex = 0;
            this.rbBlocked.TabStop = true;
            this.rbBlocked.Text = "Blocked";
            this.rbBlocked.UseVisualStyleBackColor = true;
            this.rbBlocked.CheckedChanged += new System.EventHandler(this.rbBlocked_CheckedChanged);
            // 
            // grpItem
            // 
            this.grpItem.Controls.Add(this.lblMaxItemAmount);
            this.grpItem.Controls.Add(this.lblMapItem);
            this.grpItem.Controls.Add(this.scrlMaxItemVal);
            this.grpItem.Controls.Add(this.scrlMapItem);
            this.grpItem.Location = new System.Drawing.Point(7, 487);
            this.grpItem.Name = "grpItem";
            this.grpItem.Size = new System.Drawing.Size(278, 132);
            this.grpItem.TabIndex = 3;
            this.grpItem.TabStop = false;
            this.grpItem.Text = "Map Item";
            this.grpItem.Visible = false;
            // 
            // lblMaxItemAmount
            // 
            this.lblMaxItemAmount.AutoSize = true;
            this.lblMaxItemAmount.Location = new System.Drawing.Point(13, 72);
            this.lblMaxItemAmount.Name = "lblMaxItemAmount";
            this.lblMaxItemAmount.Size = new System.Drawing.Size(63, 13);
            this.lblMaxItemAmount.TabIndex = 8;
            this.lblMaxItemAmount.Text = "Quantity: x1";
            // 
            // lblMapItem
            // 
            this.lblMapItem.AutoSize = true;
            this.lblMapItem.Location = new System.Drawing.Point(13, 21);
            this.lblMapItem.Name = "lblMapItem";
            this.lblMapItem.Size = new System.Drawing.Size(39, 13);
            this.lblMapItem.TabIndex = 7;
            this.lblMapItem.Text = "Item: 1";
            // 
            // scrlMaxItemVal
            // 
            this.scrlMaxItemVal.LargeChange = 1;
            this.scrlMaxItemVal.Location = new System.Drawing.Point(16, 94);
            this.scrlMaxItemVal.Maximum = 1000;
            this.scrlMaxItemVal.Minimum = 1;
            this.scrlMaxItemVal.Name = "scrlMaxItemVal";
            this.scrlMaxItemVal.Size = new System.Drawing.Size(249, 17);
            this.scrlMaxItemVal.TabIndex = 6;
            this.scrlMaxItemVal.Value = 1;
            this.scrlMaxItemVal.ValueChanged += new System.EventHandler(this.scrlMaxItemVal_ValueChanged);
            // 
            // scrlMapItem
            // 
            this.scrlMapItem.LargeChange = 1;
            this.scrlMapItem.Location = new System.Drawing.Point(16, 44);
            this.scrlMapItem.Minimum = 1;
            this.scrlMapItem.Name = "scrlMapItem";
            this.scrlMapItem.Size = new System.Drawing.Size(249, 17);
            this.scrlMapItem.TabIndex = 5;
            this.scrlMapItem.Value = 1;
            this.scrlMapItem.ValueChanged += new System.EventHandler(this.scrlMapItem_ValueChanged);
            // 
            // grpWarp
            // 
            this.grpWarp.Controls.Add(this.lblY);
            this.grpWarp.Controls.Add(this.lblX);
            this.grpWarp.Controls.Add(this.lblMap);
            this.grpWarp.Controls.Add(this.scrlX);
            this.grpWarp.Controls.Add(this.scrlY);
            this.grpWarp.Controls.Add(this.scrlMap);
            this.grpWarp.Location = new System.Drawing.Point(4, 457);
            this.grpWarp.Name = "grpWarp";
            this.grpWarp.Size = new System.Drawing.Size(280, 162);
            this.grpWarp.TabIndex = 9;
            this.grpWarp.TabStop = false;
            this.grpWarp.Text = "Warp";
            this.grpWarp.Visible = false;
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(22, 106);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(26, 13);
            this.lblY.TabIndex = 11;
            this.lblY.Text = "Y: 0";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(22, 65);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(26, 13);
            this.lblX.TabIndex = 10;
            this.lblX.Text = "X: 0";
            // 
            // lblMap
            // 
            this.lblMap.AutoSize = true;
            this.lblMap.Location = new System.Drawing.Point(19, 21);
            this.lblMap.Name = "lblMap";
            this.lblMap.Size = new System.Drawing.Size(40, 13);
            this.lblMap.TabIndex = 9;
            this.lblMap.Text = "Map: 0";
            // 
            // scrlX
            // 
            this.scrlX.LargeChange = 1;
            this.scrlX.Location = new System.Drawing.Point(19, 78);
            this.scrlX.Name = "scrlX";
            this.scrlX.Size = new System.Drawing.Size(242, 21);
            this.scrlX.TabIndex = 8;
            this.scrlX.ValueChanged += new System.EventHandler(this.scrlX_ValueChanged);
            // 
            // scrlY
            // 
            this.scrlY.LargeChange = 1;
            this.scrlY.Location = new System.Drawing.Point(19, 121);
            this.scrlY.Name = "scrlY";
            this.scrlY.Size = new System.Drawing.Size(243, 21);
            this.scrlY.TabIndex = 7;
            this.scrlY.ValueChanged += new System.EventHandler(this.scrlY_ValueChanged);
            // 
            // scrlMap
            // 
            this.scrlMap.LargeChange = 1;
            this.scrlMap.Location = new System.Drawing.Point(20, 34);
            this.scrlMap.Name = "scrlMap";
            this.scrlMap.Size = new System.Drawing.Size(242, 21);
            this.scrlMap.TabIndex = 6;
            this.scrlMap.ValueChanged += new System.EventHandler(this.scrlMap_ValueChanged);
            // 
            // grpZDimension
            // 
            this.grpZDimension.Controls.Add(this.grpGateway);
            this.grpZDimension.Controls.Add(this.grpDimBlock);
            this.grpZDimension.Location = new System.Drawing.Point(2, 490);
            this.grpZDimension.Name = "grpZDimension";
            this.grpZDimension.Size = new System.Drawing.Size(281, 132);
            this.grpZDimension.TabIndex = 13;
            this.grpZDimension.TabStop = false;
            this.grpZDimension.Text = "Z-Dimension";
            this.grpZDimension.Visible = false;
            // 
            // grpGateway
            // 
            this.grpGateway.Controls.Add(this.rbGateway2);
            this.grpGateway.Controls.Add(this.rbGateway1);
            this.grpGateway.Controls.Add(this.rbGatewayNone);
            this.grpGateway.Location = new System.Drawing.Point(10, 25);
            this.grpGateway.Name = "grpGateway";
            this.grpGateway.Size = new System.Drawing.Size(119, 91);
            this.grpGateway.TabIndex = 9;
            this.grpGateway.TabStop = false;
            this.grpGateway.Text = "Gateway";
            // 
            // rbGateway2
            // 
            this.rbGateway2.AutoSize = true;
            this.rbGateway2.Location = new System.Drawing.Point(6, 64);
            this.rbGateway2.Name = "rbGateway2";
            this.rbGateway2.Size = new System.Drawing.Size(60, 17);
            this.rbGateway2.TabIndex = 12;
            this.rbGateway2.Text = "Level 2";
            this.rbGateway2.UseVisualStyleBackColor = true;
            // 
            // rbGateway1
            // 
            this.rbGateway1.AutoSize = true;
            this.rbGateway1.Location = new System.Drawing.Point(6, 41);
            this.rbGateway1.Name = "rbGateway1";
            this.rbGateway1.Size = new System.Drawing.Size(60, 17);
            this.rbGateway1.TabIndex = 11;
            this.rbGateway1.Text = "Level 1";
            this.rbGateway1.UseVisualStyleBackColor = true;
            // 
            // rbGatewayNone
            // 
            this.rbGatewayNone.AutoSize = true;
            this.rbGatewayNone.Checked = true;
            this.rbGatewayNone.Location = new System.Drawing.Point(6, 19);
            this.rbGatewayNone.Name = "rbGatewayNone";
            this.rbGatewayNone.Size = new System.Drawing.Size(51, 17);
            this.rbGatewayNone.TabIndex = 10;
            this.rbGatewayNone.TabStop = true;
            this.rbGatewayNone.Text = "None";
            this.rbGatewayNone.UseVisualStyleBackColor = true;
            // 
            // grpDimBlock
            // 
            this.grpDimBlock.Controls.Add(this.rbBlock2);
            this.grpDimBlock.Controls.Add(this.rbBlock1);
            this.grpDimBlock.Controls.Add(this.rbBlockNone);
            this.grpDimBlock.Location = new System.Drawing.Point(141, 25);
            this.grpDimBlock.Name = "grpDimBlock";
            this.grpDimBlock.Size = new System.Drawing.Size(122, 91);
            this.grpDimBlock.TabIndex = 8;
            this.grpDimBlock.TabStop = false;
            this.grpDimBlock.Text = "Block";
            // 
            // rbBlock2
            // 
            this.rbBlock2.AutoSize = true;
            this.rbBlock2.Location = new System.Drawing.Point(6, 64);
            this.rbBlock2.Name = "rbBlock2";
            this.rbBlock2.Size = new System.Drawing.Size(60, 17);
            this.rbBlock2.TabIndex = 15;
            this.rbBlock2.Text = "Level 2";
            this.rbBlock2.UseVisualStyleBackColor = true;
            // 
            // rbBlock1
            // 
            this.rbBlock1.AutoSize = true;
            this.rbBlock1.Location = new System.Drawing.Point(6, 41);
            this.rbBlock1.Name = "rbBlock1";
            this.rbBlock1.Size = new System.Drawing.Size(60, 17);
            this.rbBlock1.TabIndex = 14;
            this.rbBlock1.Text = "Level 1";
            this.rbBlock1.UseVisualStyleBackColor = true;
            // 
            // rbBlockNone
            // 
            this.rbBlockNone.AutoSize = true;
            this.rbBlockNone.Checked = true;
            this.rbBlockNone.Location = new System.Drawing.Point(6, 19);
            this.rbBlockNone.Name = "rbBlockNone";
            this.rbBlockNone.Size = new System.Drawing.Size(51, 17);
            this.rbBlockNone.TabIndex = 13;
            this.rbBlockNone.TabStop = true;
            this.rbBlockNone.Text = "None";
            this.rbBlockNone.UseVisualStyleBackColor = true;
            // 
            // grpMapProperties
            // 
            this.grpMapProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpMapProperties.Controls.Add(this.scrlBrightness);
            this.grpMapProperties.Controls.Add(this.lblBrightness);
            this.grpMapProperties.Controls.Add(this.groupBox1);
            this.grpMapProperties.Controls.Add(this.scrlFogIntensity);
            this.grpMapProperties.Controls.Add(this.lblFogIntensity);
            this.grpMapProperties.Controls.Add(this.scrlFogVertical);
            this.grpMapProperties.Controls.Add(this.scrlFogHorizontal);
            this.grpMapProperties.Controls.Add(this.lblFogVerticalSpeed);
            this.grpMapProperties.Controls.Add(this.lblFogHorizontalSpeed);
            this.grpMapProperties.Controls.Add(this.cmbFogs);
            this.grpMapProperties.Controls.Add(this.label8);
            this.grpMapProperties.Controls.Add(this.cmbPanorama);
            this.grpMapProperties.Controls.Add(this.label6);
            this.grpMapProperties.Controls.Add(this.cmbMapSound);
            this.grpMapProperties.Controls.Add(this.label5);
            this.grpMapProperties.Controls.Add(this.cmbMapMusic);
            this.grpMapProperties.Controls.Add(this.label4);
            this.grpMapProperties.Controls.Add(this.lblCloseMapProperties);
            this.grpMapProperties.Controls.Add(this.grpMapNPCS);
            this.grpMapProperties.Controls.Add(this.chkIndoors);
            this.grpMapProperties.Controls.Add(this.txtMapName);
            this.grpMapProperties.Controls.Add(this.label1);
            this.grpMapProperties.Controls.Add(this.btnCloseProperties);
            this.grpMapProperties.Location = new System.Drawing.Point(0, 0);
            this.grpMapProperties.Name = "grpMapProperties";
            this.grpMapProperties.Size = new System.Drawing.Size(292, 630);
            this.grpMapProperties.TabIndex = 9;
            this.grpMapProperties.TabStop = false;
            this.grpMapProperties.Text = "Map Properties";
            this.grpMapProperties.Visible = false;
            // 
            // scrlBrightness
            // 
            this.scrlBrightness.LargeChange = 1;
            this.scrlBrightness.Location = new System.Drawing.Point(168, 55);
            this.scrlBrightness.Minimum = 10;
            this.scrlBrightness.Name = "scrlBrightness";
            this.scrlBrightness.Size = new System.Drawing.Size(117, 17);
            this.scrlBrightness.TabIndex = 23;
            this.scrlBrightness.Value = 100;
            this.scrlBrightness.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlBrightness_Scroll);
            // 
            // lblBrightness
            // 
            this.lblBrightness.AutoSize = true;
            this.lblBrightness.Location = new System.Drawing.Point(165, 41);
            this.lblBrightness.Name = "lblBrightness";
            this.lblBrightness.Size = new System.Drawing.Size(80, 13);
            this.lblBrightness.TabIndex = 22;
            this.lblBrightness.Text = "Brightness: 100";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.scrlMapAlpha);
            this.groupBox1.Controls.Add(this.lblMapAlpha);
            this.groupBox1.Controls.Add(this.scrlMapBlue);
            this.groupBox1.Controls.Add(this.lblMapBlue);
            this.groupBox1.Controls.Add(this.scrlMapGreen);
            this.groupBox1.Controls.Add(this.lblMapGreen);
            this.groupBox1.Controls.Add(this.scrlMapRed);
            this.groupBox1.Controls.Add(this.lblMapRed);
            this.groupBox1.Location = new System.Drawing.Point(168, 85);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(117, 169);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map Overlay";
            // 
            // scrlMapAlpha
            // 
            this.scrlMapAlpha.LargeChange = 1;
            this.scrlMapAlpha.Location = new System.Drawing.Point(10, 137);
            this.scrlMapAlpha.Maximum = 255;
            this.scrlMapAlpha.Name = "scrlMapAlpha";
            this.scrlMapAlpha.Size = new System.Drawing.Size(96, 17);
            this.scrlMapAlpha.TabIndex = 7;
            this.scrlMapAlpha.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMapAlpha_Scroll);
            // 
            // lblMapAlpha
            // 
            this.lblMapAlpha.AutoSize = true;
            this.lblMapAlpha.Location = new System.Drawing.Point(7, 123);
            this.lblMapAlpha.Name = "lblMapAlpha";
            this.lblMapAlpha.Size = new System.Drawing.Size(58, 13);
            this.lblMapAlpha.TabIndex = 6;
            this.lblMapAlpha.Text = "Intensity: 0";
            // 
            // scrlMapBlue
            // 
            this.scrlMapBlue.LargeChange = 1;
            this.scrlMapBlue.Location = new System.Drawing.Point(10, 101);
            this.scrlMapBlue.Maximum = 255;
            this.scrlMapBlue.Name = "scrlMapBlue";
            this.scrlMapBlue.Size = new System.Drawing.Size(96, 17);
            this.scrlMapBlue.TabIndex = 5;
            this.scrlMapBlue.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMapBlue_Scroll);
            // 
            // lblMapBlue
            // 
            this.lblMapBlue.AutoSize = true;
            this.lblMapBlue.Location = new System.Drawing.Point(7, 87);
            this.lblMapBlue.Name = "lblMapBlue";
            this.lblMapBlue.Size = new System.Drawing.Size(40, 13);
            this.lblMapBlue.TabIndex = 4;
            this.lblMapBlue.Text = "Blue: 0";
            // 
            // scrlMapGreen
            // 
            this.scrlMapGreen.LargeChange = 1;
            this.scrlMapGreen.Location = new System.Drawing.Point(10, 64);
            this.scrlMapGreen.Maximum = 255;
            this.scrlMapGreen.Name = "scrlMapGreen";
            this.scrlMapGreen.Size = new System.Drawing.Size(96, 17);
            this.scrlMapGreen.TabIndex = 3;
            this.scrlMapGreen.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMapGreen_Scroll);
            // 
            // lblMapGreen
            // 
            this.lblMapGreen.AutoSize = true;
            this.lblMapGreen.Location = new System.Drawing.Point(7, 50);
            this.lblMapGreen.Name = "lblMapGreen";
            this.lblMapGreen.Size = new System.Drawing.Size(48, 13);
            this.lblMapGreen.TabIndex = 2;
            this.lblMapGreen.Text = "Green: 0";
            // 
            // scrlMapRed
            // 
            this.scrlMapRed.LargeChange = 1;
            this.scrlMapRed.Location = new System.Drawing.Point(10, 27);
            this.scrlMapRed.Maximum = 255;
            this.scrlMapRed.Name = "scrlMapRed";
            this.scrlMapRed.Size = new System.Drawing.Size(96, 17);
            this.scrlMapRed.TabIndex = 1;
            this.scrlMapRed.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMapRed_Scroll);
            // 
            // lblMapRed
            // 
            this.lblMapRed.AutoSize = true;
            this.lblMapRed.Location = new System.Drawing.Point(7, 13);
            this.lblMapRed.Name = "lblMapRed";
            this.lblMapRed.Size = new System.Drawing.Size(39, 13);
            this.lblMapRed.TabIndex = 0;
            this.lblMapRed.Text = "Red: 0";
            // 
            // scrlFogIntensity
            // 
            this.scrlFogIntensity.LargeChange = 1;
            this.scrlFogIntensity.Location = new System.Drawing.Point(11, 231);
            this.scrlFogIntensity.Maximum = 255;
            this.scrlFogIntensity.Name = "scrlFogIntensity";
            this.scrlFogIntensity.Size = new System.Drawing.Size(145, 16);
            this.scrlFogIntensity.TabIndex = 20;
            this.scrlFogIntensity.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlFogIntensity_Scroll);
            // 
            // lblFogIntensity
            // 
            this.lblFogIntensity.AutoSize = true;
            this.lblFogIntensity.Location = new System.Drawing.Point(6, 218);
            this.lblFogIntensity.Name = "lblFogIntensity";
            this.lblFogIntensity.Size = new System.Drawing.Size(79, 13);
            this.lblFogIntensity.TabIndex = 19;
            this.lblFogIntensity.Text = "Fog Intensity: 0";
            // 
            // scrlFogVertical
            // 
            this.scrlFogVertical.LargeChange = 1;
            this.scrlFogVertical.Location = new System.Drawing.Point(11, 198);
            this.scrlFogVertical.Maximum = 5;
            this.scrlFogVertical.Minimum = -5;
            this.scrlFogVertical.Name = "scrlFogVertical";
            this.scrlFogVertical.Size = new System.Drawing.Size(145, 16);
            this.scrlFogVertical.TabIndex = 18;
            this.scrlFogVertical.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlFogVertical_Scroll);
            // 
            // scrlFogHorizontal
            // 
            this.scrlFogHorizontal.LargeChange = 1;
            this.scrlFogHorizontal.Location = new System.Drawing.Point(11, 166);
            this.scrlFogHorizontal.Maximum = 5;
            this.scrlFogHorizontal.Minimum = -5;
            this.scrlFogHorizontal.Name = "scrlFogHorizontal";
            this.scrlFogHorizontal.Size = new System.Drawing.Size(145, 16);
            this.scrlFogHorizontal.TabIndex = 17;
            this.scrlFogHorizontal.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlFogHorizontal_Scroll);
            // 
            // lblFogVerticalSpeed
            // 
            this.lblFogVerticalSpeed.AutoSize = true;
            this.lblFogVerticalSpeed.Location = new System.Drawing.Point(6, 185);
            this.lblFogVerticalSpeed.Name = "lblFogVerticalSpeed";
            this.lblFogVerticalSpeed.Size = new System.Drawing.Size(109, 13);
            this.lblFogVerticalSpeed.TabIndex = 16;
            this.lblFogVerticalSpeed.Text = "Fog Vertical Speed: 0";
            // 
            // lblFogHorizontalSpeed
            // 
            this.lblFogHorizontalSpeed.AutoSize = true;
            this.lblFogHorizontalSpeed.Location = new System.Drawing.Point(6, 149);
            this.lblFogHorizontalSpeed.Name = "lblFogHorizontalSpeed";
            this.lblFogHorizontalSpeed.Size = new System.Drawing.Size(121, 13);
            this.lblFogHorizontalSpeed.TabIndex = 15;
            this.lblFogHorizontalSpeed.Text = "Fog Horizontal Speed: 0";
            // 
            // cmbFogs
            // 
            this.cmbFogs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFogs.FormattingEnabled = true;
            this.cmbFogs.Location = new System.Drawing.Point(74, 121);
            this.cmbFogs.Name = "cmbFogs";
            this.cmbFogs.Size = new System.Drawing.Size(82, 21);
            this.cmbFogs.TabIndex = 14;
            this.cmbFogs.SelectedIndexChanged += new System.EventHandler(this.cmbFogs_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 124);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(28, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Fog:";
            // 
            // cmbPanorama
            // 
            this.cmbPanorama.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPanorama.FormattingEnabled = true;
            this.cmbPanorama.Location = new System.Drawing.Point(74, 94);
            this.cmbPanorama.Name = "cmbPanorama";
            this.cmbPanorama.Size = new System.Drawing.Size(82, 21);
            this.cmbPanorama.TabIndex = 12;
            this.cmbPanorama.SelectedIndexChanged += new System.EventHandler(this.cmbPanorama_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 97);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Panorama:";
            // 
            // cmbMapSound
            // 
            this.cmbMapSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapSound.FormattingEnabled = true;
            this.cmbMapSound.Location = new System.Drawing.Point(63, 65);
            this.cmbMapSound.Name = "cmbMapSound";
            this.cmbMapSound.Size = new System.Drawing.Size(93, 21);
            this.cmbMapSound.TabIndex = 10;
            this.cmbMapSound.SelectedIndexChanged += new System.EventHandler(this.cmbMapSound_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "BG Sound:";
            // 
            // cmbMapMusic
            // 
            this.cmbMapMusic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapMusic.FormattingEnabled = true;
            this.cmbMapMusic.Location = new System.Drawing.Point(63, 41);
            this.cmbMapMusic.Name = "cmbMapMusic";
            this.cmbMapMusic.Size = new System.Drawing.Size(93, 21);
            this.cmbMapMusic.TabIndex = 8;
            this.cmbMapMusic.SelectedIndexChanged += new System.EventHandler(this.cmbMapMusic_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "BG Music:";
            // 
            // lblCloseMapProperties
            // 
            this.lblCloseMapProperties.AutoSize = true;
            this.lblCloseMapProperties.Location = new System.Drawing.Point(276, 7);
            this.lblCloseMapProperties.Name = "lblCloseMapProperties";
            this.lblCloseMapProperties.Size = new System.Drawing.Size(14, 13);
            this.lblCloseMapProperties.TabIndex = 6;
            this.lblCloseMapProperties.Text = "X";
            this.lblCloseMapProperties.Click += new System.EventHandler(this.lblCloseMapProperties_Click);
            // 
            // grpMapNPCS
            // 
            this.grpMapNPCS.Controls.Add(this.grpManage);
            this.grpMapNPCS.Controls.Add(this.grpSpawnLoc);
            this.grpMapNPCS.Controls.Add(this.lstMapNpcs);
            this.grpMapNPCS.Location = new System.Drawing.Point(9, 260);
            this.grpMapNPCS.Name = "grpMapNPCS";
            this.grpMapNPCS.Size = new System.Drawing.Size(200, 342);
            this.grpMapNPCS.TabIndex = 4;
            this.grpMapNPCS.TabStop = false;
            this.grpMapNPCS.Text = "Map NPCs";
            // 
            // grpManage
            // 
            this.grpManage.Controls.Add(this.btnRemoveMapNpc);
            this.grpManage.Controls.Add(this.btnAddMapNpc);
            this.grpManage.Controls.Add(this.cmbNpc);
            this.grpManage.Location = new System.Drawing.Point(8, 260);
            this.grpManage.Name = "grpManage";
            this.grpManage.Size = new System.Drawing.Size(188, 76);
            this.grpManage.TabIndex = 9;
            this.grpManage.TabStop = false;
            this.grpManage.Text = "Add/Remove Map NPCs";
            // 
            // btnRemoveMapNpc
            // 
            this.btnRemoveMapNpc.Location = new System.Drawing.Point(107, 47);
            this.btnRemoveMapNpc.Name = "btnRemoveMapNpc";
            this.btnRemoveMapNpc.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveMapNpc.TabIndex = 6;
            this.btnRemoveMapNpc.Text = "Remove";
            this.btnRemoveMapNpc.UseVisualStyleBackColor = true;
            this.btnRemoveMapNpc.Click += new System.EventHandler(this.btnRemoveMapNpc_Click);
            // 
            // btnAddMapNpc
            // 
            this.btnAddMapNpc.Location = new System.Drawing.Point(9, 47);
            this.btnAddMapNpc.Name = "btnAddMapNpc";
            this.btnAddMapNpc.Size = new System.Drawing.Size(75, 23);
            this.btnAddMapNpc.TabIndex = 5;
            this.btnAddMapNpc.Text = "Add";
            this.btnAddMapNpc.UseVisualStyleBackColor = true;
            this.btnAddMapNpc.Click += new System.EventHandler(this.btnAddMapNpc_Click);
            // 
            // cmbNpc
            // 
            this.cmbNpc.FormattingEnabled = true;
            this.cmbNpc.Location = new System.Drawing.Point(6, 13);
            this.cmbNpc.Name = "cmbNpc";
            this.cmbNpc.Size = new System.Drawing.Size(176, 21);
            this.cmbNpc.TabIndex = 4;
            this.cmbNpc.SelectedIndexChanged += new System.EventHandler(this.cmbNpc_SelectedIndexChanged);
            // 
            // grpSpawnLoc
            // 
            this.grpSpawnLoc.Controls.Add(this.cmbDir);
            this.grpSpawnLoc.Controls.Add(this.lblDir);
            this.grpSpawnLoc.Controls.Add(this.rbRandom);
            this.grpSpawnLoc.Controls.Add(this.rbDeclared);
            this.grpSpawnLoc.Location = new System.Drawing.Point(6, 173);
            this.grpSpawnLoc.Name = "grpSpawnLoc";
            this.grpSpawnLoc.Size = new System.Drawing.Size(188, 81);
            this.grpSpawnLoc.TabIndex = 7;
            this.grpSpawnLoc.TabStop = false;
            this.grpSpawnLoc.Text = "Spawn Location: Random";
            // 
            // cmbDir
            // 
            this.cmbDir.FormattingEnabled = true;
            this.cmbDir.Items.AddRange(new object[] {
            "Random",
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbDir.Location = new System.Drawing.Point(5, 54);
            this.cmbDir.Name = "cmbDir";
            this.cmbDir.Size = new System.Drawing.Size(177, 21);
            this.cmbDir.TabIndex = 3;
            this.cmbDir.SelectedIndexChanged += new System.EventHandler(this.cmbDir_SelectedIndexChanged);
            // 
            // lblDir
            // 
            this.lblDir.AutoSize = true;
            this.lblDir.Location = new System.Drawing.Point(5, 40);
            this.lblDir.Name = "lblDir";
            this.lblDir.Size = new System.Drawing.Size(52, 13);
            this.lblDir.TabIndex = 2;
            this.lblDir.Text = "Direction:";
            // 
            // rbRandom
            // 
            this.rbRandom.AutoSize = true;
            this.rbRandom.Checked = true;
            this.rbRandom.Location = new System.Drawing.Point(117, 20);
            this.rbRandom.Name = "rbRandom";
            this.rbRandom.Size = new System.Drawing.Size(65, 17);
            this.rbRandom.TabIndex = 1;
            this.rbRandom.TabStop = true;
            this.rbRandom.Text = "Random";
            this.rbRandom.UseVisualStyleBackColor = true;
            this.rbRandom.Click += new System.EventHandler(this.rbRandom_Click);
            // 
            // rbDeclared
            // 
            this.rbDeclared.AutoSize = true;
            this.rbDeclared.Location = new System.Drawing.Point(9, 20);
            this.rbDeclared.Name = "rbDeclared";
            this.rbDeclared.Size = new System.Drawing.Size(68, 17);
            this.rbDeclared.TabIndex = 0;
            this.rbDeclared.Text = "Declared";
            this.rbDeclared.UseVisualStyleBackColor = true;
            // 
            // lstMapNpcs
            // 
            this.lstMapNpcs.FormattingEnabled = true;
            this.lstMapNpcs.Location = new System.Drawing.Point(6, 19);
            this.lstMapNpcs.Name = "lstMapNpcs";
            this.lstMapNpcs.Size = new System.Drawing.Size(188, 147);
            this.lstMapNpcs.TabIndex = 0;
            this.lstMapNpcs.Click += new System.EventHandler(this.lstMapNpcs_Click);
            // 
            // chkIndoors
            // 
            this.chkIndoors.AutoSize = true;
            this.chkIndoors.Location = new System.Drawing.Point(168, 19);
            this.chkIndoors.Name = "chkIndoors";
            this.chkIndoors.Size = new System.Drawing.Size(106, 17);
            this.chkIndoors.TabIndex = 3;
            this.chkIndoors.Text = "Is an Indoor Map";
            this.chkIndoors.UseVisualStyleBackColor = true;
            // 
            // txtMapName
            // 
            this.txtMapName.Location = new System.Drawing.Point(74, 17);
            this.txtMapName.Name = "txtMapName";
            this.txtMapName.Size = new System.Drawing.Size(82, 20);
            this.txtMapName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Map Name:";
            // 
            // btnCloseProperties
            // 
            this.btnCloseProperties.Location = new System.Drawing.Point(943, 603);
            this.btnCloseProperties.Name = "btnCloseProperties";
            this.btnCloseProperties.Size = new System.Drawing.Size(75, 23);
            this.btnCloseProperties.TabIndex = 0;
            this.btnCloseProperties.Text = "Close";
            this.btnCloseProperties.UseVisualStyleBackColor = true;
            // 
            // spltContainer
            // 
            this.spltContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spltContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spltContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.spltContainer.Location = new System.Drawing.Point(0, 29);
            this.spltContainer.Name = "spltContainer";
            // 
            // spltContainer.Panel1
            // 
            this.spltContainer.Panel1.Controls.Add(this.grpLayer);
            this.spltContainer.Panel1.Controls.Add(this.grpLightEditor);
            this.spltContainer.Panel1.Controls.Add(this.grpAttributes);
            this.spltContainer.Panel1.Controls.Add(this.grpMapProperties);
            this.spltContainer.Panel1.Controls.Add(this.grpMapList);
            this.spltContainer.Panel1MinSize = 292;
            // 
            // spltContainer.Panel2
            // 
            this.spltContainer.Panel2.AutoScroll = true;
            this.spltContainer.Panel2.Controls.Add(this.picMap);
            this.spltContainer.Panel2MinSize = 600;
            this.spltContainer.Size = new System.Drawing.Size(1020, 653);
            this.spltContainer.SplitterDistance = 292;
            this.spltContainer.TabIndex = 2;
            // 
            // picMap
            // 
            this.picMap.Location = new System.Drawing.Point(3, 1);
            this.picMap.Name = "picMap";
            this.picMap.Size = new System.Drawing.Size(32, 32);
            this.picMap.TabIndex = 1;
            this.picMap.TabStop = false;
            this.picMap.DoubleClick += new System.EventHandler(this.picMap_DoubleClick);
            this.picMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseDown);
            this.picMap.MouseEnter += new System.EventHandler(this.picMap_MouseEnter);
            this.picMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseMove);
            this.picMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseUp);
            // 
            // picTileset
            // 
            this.picTileset.Location = new System.Drawing.Point(1, 1);
            this.picTileset.Name = "picTileset";
            this.picTileset.Size = new System.Drawing.Size(193, 152);
            this.picTileset.TabIndex = 2;
            this.picTileset.TabStop = false;
            this.picTileset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picTileset_MouseDown);
            this.picTileset.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picTileset_MouseMove);
            this.picTileset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picTileset_MouseUp);
            // 
            // pnlTilesetContainer
            // 
            this.pnlTilesetContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTilesetContainer.AutoScroll = true;
            this.pnlTilesetContainer.Controls.Add(this.picTileset);
            this.pnlTilesetContainer.Location = new System.Drawing.Point(7, 75);
            this.pnlTilesetContainer.Name = "pnlTilesetContainer";
            this.pnlTilesetContainer.Size = new System.Drawing.Size(277, 573);
            this.pnlTilesetContainer.TabIndex = 10;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 682);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.spltContainer);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Intersect Editor";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.grpLayer.ResumeLayout(false);
            this.grpLayer.PerformLayout();
            this.grpMapList.ResumeLayout(false);
            this.grpMapList.PerformLayout();
            this.grpLightEditor.ResumeLayout(false);
            this.grpLightEditor.PerformLayout();
            this.grpAttributes.ResumeLayout(false);
            this.grpAttributes.PerformLayout();
            this.grpResource.ResumeLayout(false);
            this.grpResource.PerformLayout();
            this.grpSound.ResumeLayout(false);
            this.grpSound.PerformLayout();
            this.grpItem.ResumeLayout(false);
            this.grpItem.PerformLayout();
            this.grpWarp.ResumeLayout(false);
            this.grpWarp.PerformLayout();
            this.grpZDimension.ResumeLayout(false);
            this.grpGateway.ResumeLayout(false);
            this.grpGateway.PerformLayout();
            this.grpDimBlock.ResumeLayout(false);
            this.grpDimBlock.PerformLayout();
            this.grpMapProperties.ResumeLayout(false);
            this.grpMapProperties.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpMapNPCS.ResumeLayout(false);
            this.grpManage.ResumeLayout(false);
            this.grpSpawnLoc.ResumeLayout(false);
            this.grpSpawnLoc.PerformLayout();
            this.spltContainer.Panel1.ResumeLayout(false);
            this.spltContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spltContainer)).EndInit();
            this.spltContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTileset)).EndInit();
            this.pnlTilesetContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layerMenu;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpLayer;
        public System.Windows.Forms.ComboBox cmbTilesets;
        private System.Windows.Forms.Label lblCurLayer;
        private System.Windows.Forms.ToolStripMenuItem saveMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fillToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eraseLayerToolStripMenuItem;
        private System.Windows.Forms.ComboBox cmbAutotile;
        private System.Windows.Forms.ToolStripMenuItem newMapToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpMapList;
        private System.Windows.Forms.Label lblCloseMapList;
        private System.Windows.Forms.ListBox lstGameMaps;
        private System.Windows.Forms.ToolStripMenuItem mapPropertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideDarknessToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpLightEditor;
        private System.Windows.Forms.HScrollBar scrlLightRange;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLightRange;
        private System.Windows.Forms.HScrollBar scrlLightIntensity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLightIntensity;
        private System.Windows.Forms.TextBox txtLightOffsetY;
        private System.Windows.Forms.TextBox txtLightOffsetX;
        private System.Windows.Forms.Label lblOffsetY;
        private System.Windows.Forms.Label lblOffsetX;
        private System.Windows.Forms.Button btnLightEditorRevert;
        private System.Windows.Forms.Button btnLightEditorClose;
        private System.Windows.Forms.ToolStripMenuItem hihiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentEditorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem itemEditorToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpAttributes;
        private System.Windows.Forms.GroupBox grpItem;
        private System.Windows.Forms.RadioButton rbItem;
        private System.Windows.Forms.RadioButton rbBlocked;
        private ToolStripMenuItem npcEditorToolStripMenuItem;
        private ToolStripMenuItem spellEditorToolStripMenuItem;
        private ToolStripMenuItem animationEditorToolStripMenuItem;
        private RadioButton rbWarp;
        private RadioButton rbNPCAvoid;
        private RadioButton rbZDimension;
        private GroupBox grpWarp;
        private Label lblMaxItemAmount;
        private Label lblMapItem;
        private HScrollBar scrlMaxItemVal;
        private HScrollBar scrlMapItem;
        private Label lblY;
        private Label lblX;
        private Label lblMap;
        private HScrollBar scrlX;
        private HScrollBar scrlY;
        private HScrollBar scrlMap;
        private GroupBox grpZDimension;
        private GroupBox grpGateway;
        private RadioButton rbGateway2;
        private RadioButton rbGateway1;
        private RadioButton rbGatewayNone;
        private GroupBox grpDimBlock;
        private RadioButton rbBlock2;
        private RadioButton rbBlock1;
        private RadioButton rbBlockNone;
        private GroupBox grpMapProperties;
        private ComboBox cmbMapSound;
        private Label label5;
        private ComboBox cmbMapMusic;
        private Label label4;
        private Label lblCloseMapProperties;
        private GroupBox grpMapNPCS;
        private GroupBox grpManage;
        private Button btnRemoveMapNpc;
        private Button btnAddMapNpc;
        private ComboBox cmbNpc;
        private GroupBox grpSpawnLoc;
        private RadioButton rbRandom;
        private RadioButton rbDeclared;
        private ListBox lstMapNpcs;
        private CheckBox chkIndoors;
        private TextBox txtMapName;
        private Label label1;
        private Button btnCloseProperties;
        private ComboBox cmbDir;
        private Label lblDir;
        private RadioButton rbSound;
        private GroupBox grpSound;
        private Label lblSoundDistance;
        private Label label7;
        private HScrollBar scrlSoundDistance;
        private ComboBox cmbMapAttributeSound;
        private GroupBox groupBox1;
        private HScrollBar scrlMapAlpha;
        private Label lblMapAlpha;
        private HScrollBar scrlMapBlue;
        private Label lblMapBlue;
        private HScrollBar scrlMapGreen;
        private Label lblMapGreen;
        private HScrollBar scrlMapRed;
        private Label lblMapRed;
        private HScrollBar scrlFogIntensity;
        private Label lblFogIntensity;
        private HScrollBar scrlFogVertical;
        private HScrollBar scrlFogHorizontal;
        private Label lblFogVerticalSpeed;
        private Label lblFogHorizontalSpeed;
        private ComboBox cmbFogs;
        private Label label8;
        private ComboBox cmbPanorama;
        private Label label6;
        private HScrollBar scrlBrightness;
        private Label lblBrightness;
        private ToolStripMenuItem hideFogToolStripMenuItem;
        private ToolStripMenuItem hideOverlayToolStripMenuItem;
        private ToolStripMenuItem resourceEditorToolStripMenuItem;
        private RadioButton rbResource;
        private GroupBox grpResource;
        private Label lblResource;
        private HScrollBar scrlResource;
        private SplitContainer spltContainer;
        public PictureBox picMap;
        private Panel pnlTilesetContainer;
        public PictureBox picTileset;
    }
}

