namespace IntersectEditor
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.nightTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpMap = new System.Windows.Forms.GroupBox();
            this.picMap = new System.Windows.Forms.PictureBox();
            this.hScrollMap = new System.Windows.Forms.HScrollBar();
            this.vScrollMap = new System.Windows.Forms.VScrollBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbAutotile = new System.Windows.Forms.ComboBox();
            this.lblCurLayer = new System.Windows.Forms.Label();
            this.cmbTilesets = new System.Windows.Forms.ComboBox();
            this.hScrollTileset = new System.Windows.Forms.HScrollBar();
            this.vScrollTileset = new System.Windows.Forms.VScrollBar();
            this.grpTileset = new System.Windows.Forms.GroupBox();
            this.picTileset = new System.Windows.Forms.PictureBox();
            this.grpMapList = new System.Windows.Forms.GroupBox();
            this.lstGameMaps = new System.Windows.Forms.ListBox();
            this.lblCloseMapList = new System.Windows.Forms.Label();
            this.grpMapProperties = new System.Windows.Forms.GroupBox();
            this.chkIndoors = new System.Windows.Forms.CheckBox();
            this.txtMapName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCloseProperties = new System.Windows.Forms.Button();
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
            this.btnRevert = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.hihiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.grpMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.grpTileset.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTileset)).BeginInit();
            this.grpMapList.SuspendLayout();
            this.grpMapProperties.SuspendLayout();
            this.grpLightEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.layerMenu,
            this.viewToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1350, 24);
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
            this.nightTimeToolStripMenuItem,
            this.mapListToolStripMenuItem,
            this.hihiToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // nightTimeToolStripMenuItem
            // 
            this.nightTimeToolStripMenuItem.Name = "nightTimeToolStripMenuItem";
            this.nightTimeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.nightTimeToolStripMenuItem.Text = "Night Time";
            this.nightTimeToolStripMenuItem.Click += new System.EventHandler(this.nightTimeToolStripMenuItem_Click);
            // 
            // mapListToolStripMenuItem
            // 
            this.mapListToolStripMenuItem.Name = "mapListToolStripMenuItem";
            this.mapListToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mapListToolStripMenuItem.Text = "Map List";
            this.mapListToolStripMenuItem.Click += new System.EventHandler(this.mapListToolStripMenuItem_Click);
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
            // grpMap
            // 
            this.grpMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMap.BackColor = System.Drawing.Color.Black;
            this.grpMap.Controls.Add(this.picMap);
            this.grpMap.Location = new System.Drawing.Point(305, 27);
            this.grpMap.MaximumSize = new System.Drawing.Size(1024, 1024);
            this.grpMap.Name = "grpMap";
            this.grpMap.Size = new System.Drawing.Size(1024, 632);
            this.grpMap.TabIndex = 2;
            this.grpMap.TabStop = false;
            // 
            // picMap
            // 
            this.picMap.Location = new System.Drawing.Point(0, 0);
            this.picMap.Name = "picMap";
            this.picMap.Size = new System.Drawing.Size(1024, 1024);
            this.picMap.TabIndex = 1;
            this.picMap.TabStop = false;
            this.picMap.Click += new System.EventHandler(this.picMap_Click);
            this.picMap.DoubleClick += new System.EventHandler(this.picMap_DoubleClick);
            this.picMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseDown);
            this.picMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseMove);
            this.picMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseUp);
            // 
            // hScrollMap
            // 
            this.hScrollMap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollMap.LargeChange = 1;
            this.hScrollMap.Location = new System.Drawing.Point(305, 662);
            this.hScrollMap.MaximumSize = new System.Drawing.Size(1024, 17);
            this.hScrollMap.Name = "hScrollMap";
            this.hScrollMap.Size = new System.Drawing.Size(1024, 17);
            this.hScrollMap.TabIndex = 3;
            this.hScrollMap.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollMap_Scroll);
            // 
            // vScrollMap
            // 
            this.vScrollMap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollMap.LargeChange = 1;
            this.vScrollMap.Location = new System.Drawing.Point(1330, 27);
            this.vScrollMap.MaximumSize = new System.Drawing.Size(17, 960);
            this.vScrollMap.Name = "vScrollMap";
            this.vScrollMap.Size = new System.Drawing.Size(17, 632);
            this.vScrollMap.TabIndex = 4;
            this.vScrollMap.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollMap_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.cmbAutotile);
            this.groupBox1.Controls.Add(this.lblCurLayer);
            this.groupBox1.Controls.Add(this.cmbTilesets);
            this.groupBox1.Controls.Add(this.hScrollTileset);
            this.groupBox1.Controls.Add(this.vScrollTileset);
            this.groupBox1.Controls.Add(this.grpTileset);
            this.groupBox1.Location = new System.Drawing.Point(13, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(290, 632);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tileset Container";
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
            // hScrollTileset
            // 
            this.hScrollTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollTileset.LargeChange = 1;
            this.hScrollTileset.Location = new System.Drawing.Point(7, 609);
            this.hScrollTileset.MaximumSize = new System.Drawing.Size(960, 17);
            this.hScrollTileset.Name = "hScrollTileset";
            this.hScrollTileset.Size = new System.Drawing.Size(198, 17);
            this.hScrollTileset.TabIndex = 6;
            this.hScrollTileset.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollTileset_Scroll);
            // 
            // vScrollTileset
            // 
            this.vScrollTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollTileset.LargeChange = 1;
            this.vScrollTileset.Location = new System.Drawing.Point(269, 74);
            this.vScrollTileset.MaximumSize = new System.Drawing.Size(17, 960);
            this.vScrollTileset.Name = "vScrollTileset";
            this.vScrollTileset.Size = new System.Drawing.Size(17, 532);
            this.vScrollTileset.TabIndex = 5;
            this.vScrollTileset.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollTileset_Scroll);
            // 
            // grpTileset
            // 
            this.grpTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpTileset.Controls.Add(this.picTileset);
            this.grpTileset.Location = new System.Drawing.Point(7, 74);
            this.grpTileset.Name = "grpTileset";
            this.grpTileset.Size = new System.Drawing.Size(258, 532);
            this.grpTileset.TabIndex = 0;
            this.grpTileset.TabStop = false;
            // 
            // picTileset
            // 
            this.picTileset.Location = new System.Drawing.Point(0, 0);
            this.picTileset.Name = "picTileset";
            this.picTileset.Size = new System.Drawing.Size(1024, 1024);
            this.picTileset.TabIndex = 2;
            this.picTileset.TabStop = false;
            this.picTileset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picTileset_MouseDown);
            this.picTileset.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picTileset_MouseMove);
            this.picTileset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picTileset_MouseUp);
            // 
            // grpMapList
            // 
            this.grpMapList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpMapList.Controls.Add(this.lstGameMaps);
            this.grpMapList.Controls.Add(this.lblCloseMapList);
            this.grpMapList.Location = new System.Drawing.Point(11, 27);
            this.grpMapList.Name = "grpMapList";
            this.grpMapList.Size = new System.Drawing.Size(290, 632);
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
            // grpMapProperties
            // 
            this.grpMapProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMapProperties.Controls.Add(this.chkIndoors);
            this.grpMapProperties.Controls.Add(this.txtMapName);
            this.grpMapProperties.Controls.Add(this.label1);
            this.grpMapProperties.Controls.Add(this.btnCloseProperties);
            this.grpMapProperties.Location = new System.Drawing.Point(305, 27);
            this.grpMapProperties.Name = "grpMapProperties";
            this.grpMapProperties.Size = new System.Drawing.Size(1024, 632);
            this.grpMapProperties.TabIndex = 7;
            this.grpMapProperties.TabStop = false;
            this.grpMapProperties.Text = "Map Properties";
            this.grpMapProperties.Visible = false;
            // 
            // chkIndoors
            // 
            this.chkIndoors.AutoSize = true;
            this.chkIndoors.Location = new System.Drawing.Point(9, 51);
            this.chkIndoors.Name = "chkIndoors";
            this.chkIndoors.Size = new System.Drawing.Size(80, 17);
            this.chkIndoors.TabIndex = 3;
            this.chkIndoors.Text = "Indoor Map";
            this.chkIndoors.UseVisualStyleBackColor = true;
            this.chkIndoors.CheckedChanged += new System.EventHandler(this.chkIndoors_CheckedChanged);
            // 
            // txtMapName
            // 
            this.txtMapName.Location = new System.Drawing.Point(74, 17);
            this.txtMapName.Name = "txtMapName";
            this.txtMapName.Size = new System.Drawing.Size(100, 20);
            this.txtMapName.TabIndex = 2;
            this.txtMapName.TextChanged += new System.EventHandler(this.txtMapName_TextChanged);
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
            this.btnCloseProperties.Click += new System.EventHandler(this.btnCloseProperties_Click);
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
            this.grpLightEditor.Controls.Add(this.btnRevert);
            this.grpLightEditor.Controls.Add(this.btnClose);
            this.grpLightEditor.Location = new System.Drawing.Point(13, 27);
            this.grpLightEditor.Name = "grpLightEditor";
            this.grpLightEditor.Size = new System.Drawing.Size(290, 632);
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
            // btnRevert
            // 
            this.btnRevert.Location = new System.Drawing.Point(130, 218);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(75, 23);
            this.btnRevert.TabIndex = 13;
            this.btnRevert.Text = "Revert";
            this.btnRevert.UseVisualStyleBackColor = true;
            this.btnRevert.Click += new System.EventHandler(this.btnRevert_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(211, 218);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 12;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // hihiToolStripMenuItem
            // 
            this.hihiToolStripMenuItem.Name = "hihiToolStripMenuItem";
            this.hihiToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hihiToolStripMenuItem.Text = "hihi";
            this.hihiToolStripMenuItem.Click += new System.EventHandler(this.hihiToolStripMenuItem_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 682);
            this.Controls.Add(this.grpMapProperties);
            this.Controls.Add(this.grpLightEditor);
            this.Controls.Add(this.grpMapList);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.vScrollMap);
            this.Controls.Add(this.hScrollMap);
            this.Controls.Add(this.grpMap);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Intersect Editor";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.grpMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpTileset.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTileset)).EndInit();
            this.grpMapList.ResumeLayout(false);
            this.grpMapList.PerformLayout();
            this.grpMapProperties.ResumeLayout(false);
            this.grpMapProperties.PerformLayout();
            this.grpLightEditor.ResumeLayout(false);
            this.grpLightEditor.PerformLayout();
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
        private System.Windows.Forms.GroupBox grpMap;
        public System.Windows.Forms.PictureBox picMap;
        private System.Windows.Forms.HScrollBar hScrollMap;
        private System.Windows.Forms.VScrollBar vScrollMap;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.HScrollBar hScrollTileset;
        private System.Windows.Forms.VScrollBar vScrollTileset;
        private System.Windows.Forms.GroupBox grpTileset;
        private System.Windows.Forms.PictureBox picTileset;
        private System.Windows.Forms.ComboBox cmbTilesets;
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
        private System.Windows.Forms.GroupBox grpMapProperties;
        private System.Windows.Forms.Button btnCloseProperties;
        private System.Windows.Forms.TextBox txtMapName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem nightTimeToolStripMenuItem;
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
        private System.Windows.Forms.Button btnRevert;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox chkIndoors;
        private System.Windows.Forms.ToolStripMenuItem hihiToolStripMenuItem;
    }
}

