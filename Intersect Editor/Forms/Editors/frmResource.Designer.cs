using DarkUI.Controls;

namespace Intersect_Editor.Classes
{
    partial class frmResource
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmResource));
            this.groupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.lstResources = new System.Windows.Forms.ListBox();
            this.groupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.scrlAnimation = new System.Windows.Forms.HScrollBar();
            this.lblAnimation = new System.Windows.Forms.Label();
            this.txtMaxHp = new DarkUI.Controls.DarkTextBox();
            this.lblMaxHp = new System.Windows.Forms.Label();
            this.scrlSpawnDuration = new System.Windows.Forms.HScrollBar();
            this.lblSpawnDuration = new System.Windows.Forms.Label();
            this.chkWalkableAfter = new DarkUI.Controls.DarkCheckBox();
            this.chkWalkableBefore = new DarkUI.Controls.DarkCheckBox();
            this.cmbToolType = new DarkUI.Controls.DarkComboBox();
            this.lblToolType = new System.Windows.Forms.Label();
            this.txtHP = new DarkUI.Controls.DarkTextBox();
            this.lblHP = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new DarkUI.Controls.DarkTextBox();
            this.groupBox3 = new DarkUI.Controls.DarkGroupBox();
            this.grpEndTileset = new DarkUI.Controls.DarkGroupBox();
            this.picEndResource = new System.Windows.Forms.PictureBox();
            this.grpInitialTileset = new DarkUI.Controls.DarkGroupBox();
            this.picInitialResource = new System.Windows.Forms.PictureBox();
            this.hScrollEndTileset = new System.Windows.Forms.HScrollBar();
            this.vScrollEndTileset = new System.Windows.Forms.VScrollBar();
            this.hScrollStartTileset = new System.Windows.Forms.HScrollBar();
            this.vScrollStartTileset = new System.Windows.Forms.VScrollBar();
            this.cmbEndSprite = new DarkUI.Controls.DarkComboBox();
            this.lblPic2 = new System.Windows.Forms.Label();
            this.cmbInitialSprite = new DarkUI.Controls.DarkComboBox();
            this.lblPic = new System.Windows.Forms.Label();
            this.groupBox4 = new DarkUI.Controls.DarkGroupBox();
            this.txtDropAmount = new DarkUI.Controls.DarkTextBox();
            this.lblDropAmount = new System.Windows.Forms.Label();
            this.scrlDropChance = new System.Windows.Forms.HScrollBar();
            this.lblDropChance = new System.Windows.Forms.Label();
            this.scrlDropItem = new System.Windows.Forms.HScrollBar();
            this.lblDropItem = new System.Windows.Forms.Label();
            this.scrlDropIndex = new System.Windows.Forms.HScrollBar();
            this.lblDropIndex = new System.Windows.Forms.Label();
            this.tmrRender = new System.Windows.Forms.Timer(this.components);
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.toolStrip = new DarkUI.Controls.DarkToolStrip();
            this.toolStripItemNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripItemPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemUndo = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.grpEndTileset.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picEndResource)).BeginInit();
            this.grpInitialTileset.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picInitialResource)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox1.Controls.Add(this.lstResources);
            this.groupBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox1.Location = new System.Drawing.Point(12, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 437);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resources";
            // 
            // lstResources
            // 
            this.lstResources.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstResources.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstResources.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstResources.FormattingEnabled = true;
            this.lstResources.Location = new System.Drawing.Point(6, 19);
            this.lstResources.Name = "lstResources";
            this.lstResources.Size = new System.Drawing.Size(191, 405);
            this.lstResources.TabIndex = 1;
            this.lstResources.Click += new System.EventHandler(this.lstResources_Click);
            this.lstResources.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemList_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox2.Controls.Add(this.scrlAnimation);
            this.groupBox2.Controls.Add(this.lblAnimation);
            this.groupBox2.Controls.Add(this.txtMaxHp);
            this.groupBox2.Controls.Add(this.lblMaxHp);
            this.groupBox2.Controls.Add(this.scrlSpawnDuration);
            this.groupBox2.Controls.Add(this.lblSpawnDuration);
            this.groupBox2.Controls.Add(this.chkWalkableAfter);
            this.groupBox2.Controls.Add(this.chkWalkableBefore);
            this.groupBox2.Controls.Add(this.cmbToolType);
            this.groupBox2.Controls.Add(this.lblToolType);
            this.groupBox2.Controls.Add(this.txtHP);
            this.groupBox2.Controls.Add(this.lblHP);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(223, 264);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // scrlAnimation
            // 
            this.scrlAnimation.LargeChange = 1;
            this.scrlAnimation.Location = new System.Drawing.Point(9, 186);
            this.scrlAnimation.Maximum = 3600;
            this.scrlAnimation.Minimum = -1;
            this.scrlAnimation.Name = "scrlAnimation";
            this.scrlAnimation.Size = new System.Drawing.Size(201, 18);
            this.scrlAnimation.TabIndex = 37;
            this.scrlAnimation.Value = -1;
            this.scrlAnimation.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlAnimation_Scroll);
            // 
            // lblAnimation
            // 
            this.lblAnimation.AutoSize = true;
            this.lblAnimation.Location = new System.Drawing.Point(6, 173);
            this.lblAnimation.Name = "lblAnimation";
            this.lblAnimation.Size = new System.Drawing.Size(94, 13);
            this.lblAnimation.TabIndex = 36;
            this.lblAnimation.Text = "Animation: 0 None";
            // 
            // txtMaxHp
            // 
            this.txtMaxHp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtMaxHp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMaxHp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtMaxHp.Location = new System.Drawing.Point(75, 103);
            this.txtMaxHp.Name = "txtMaxHp";
            this.txtMaxHp.Size = new System.Drawing.Size(135, 20);
            this.txtMaxHp.TabIndex = 34;
            this.txtMaxHp.TextChanged += new System.EventHandler(this.txtMaxHp_TextChanged);
            // 
            // lblMaxHp
            // 
            this.lblMaxHp.AutoSize = true;
            this.lblMaxHp.Location = new System.Drawing.Point(6, 106);
            this.lblMaxHp.Name = "lblMaxHp";
            this.lblMaxHp.Size = new System.Drawing.Size(48, 13);
            this.lblMaxHp.TabIndex = 35;
            this.lblMaxHp.Text = "Max HP:";
            // 
            // scrlSpawnDuration
            // 
            this.scrlSpawnDuration.Location = new System.Drawing.Point(9, 151);
            this.scrlSpawnDuration.Maximum = 3600;
            this.scrlSpawnDuration.Name = "scrlSpawnDuration";
            this.scrlSpawnDuration.Size = new System.Drawing.Size(201, 18);
            this.scrlSpawnDuration.TabIndex = 33;
            this.scrlSpawnDuration.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpawnDuration_Scroll);
            // 
            // lblSpawnDuration
            // 
            this.lblSpawnDuration.AutoSize = true;
            this.lblSpawnDuration.Location = new System.Drawing.Point(6, 138);
            this.lblSpawnDuration.Name = "lblSpawnDuration";
            this.lblSpawnDuration.Size = new System.Drawing.Size(95, 13);
            this.lblSpawnDuration.TabIndex = 32;
            this.lblSpawnDuration.Text = "Spawn Duration: 0";
            // 
            // chkWalkableAfter
            // 
            this.chkWalkableAfter.AutoSize = true;
            this.chkWalkableAfter.Location = new System.Drawing.Point(6, 240);
            this.chkWalkableAfter.Name = "chkWalkableAfter";
            this.chkWalkableAfter.Size = new System.Drawing.Size(185, 17);
            this.chkWalkableAfter.TabIndex = 31;
            this.chkWalkableAfter.Text = "Walkable after resource removal?";
            this.chkWalkableAfter.CheckedChanged += new System.EventHandler(this.chkWalkableAfter_CheckedChanged);
            // 
            // chkWalkableBefore
            // 
            this.chkWalkableBefore.AutoSize = true;
            this.chkWalkableBefore.Location = new System.Drawing.Point(6, 217);
            this.chkWalkableBefore.Name = "chkWalkableBefore";
            this.chkWalkableBefore.Size = new System.Drawing.Size(194, 17);
            this.chkWalkableBefore.TabIndex = 30;
            this.chkWalkableBefore.Text = "Walkable before resource removal?";
            this.chkWalkableBefore.CheckedChanged += new System.EventHandler(this.chkWalkableBefore_CheckedChanged);
            // 
            // cmbToolType
            // 
            this.cmbToolType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbToolType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbToolType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbToolType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbToolType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbToolType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbToolType.FormattingEnabled = true;
            this.cmbToolType.Location = new System.Drawing.Point(75, 46);
            this.cmbToolType.Name = "cmbToolType";
            this.cmbToolType.Size = new System.Drawing.Size(135, 21);
            this.cmbToolType.TabIndex = 29;
            this.cmbToolType.SelectedIndexChanged += new System.EventHandler(this.cmbToolType_SelectedIndexChanged);
            // 
            // lblToolType
            // 
            this.lblToolType.AutoSize = true;
            this.lblToolType.Location = new System.Drawing.Point(6, 49);
            this.lblToolType.Name = "lblToolType";
            this.lblToolType.Size = new System.Drawing.Size(58, 13);
            this.lblToolType.TabIndex = 28;
            this.lblToolType.Text = "Tool Type:";
            // 
            // txtHP
            // 
            this.txtHP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtHP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtHP.Location = new System.Drawing.Point(75, 77);
            this.txtHP.Name = "txtHP";
            this.txtHP.Size = new System.Drawing.Size(135, 20);
            this.txtHP.TabIndex = 15;
            this.txtHP.TextChanged += new System.EventHandler(this.txtHP_TextChanged);
            // 
            // lblHP
            // 
            this.lblHP.AutoSize = true;
            this.lblHP.Location = new System.Drawing.Point(6, 80);
            this.lblHP.Name = "lblHP";
            this.lblHP.Size = new System.Drawing.Size(45, 13);
            this.lblHP.TabIndex = 16;
            this.lblHP.Text = "Min HP:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtName.Location = new System.Drawing.Point(75, 20);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(135, 20);
            this.txtName.TabIndex = 2;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox3.Controls.Add(this.grpEndTileset);
            this.groupBox3.Controls.Add(this.grpInitialTileset);
            this.groupBox3.Controls.Add(this.hScrollEndTileset);
            this.groupBox3.Controls.Add(this.vScrollEndTileset);
            this.groupBox3.Controls.Add(this.hScrollStartTileset);
            this.groupBox3.Controls.Add(this.vScrollStartTileset);
            this.groupBox3.Controls.Add(this.cmbEndSprite);
            this.groupBox3.Controls.Add(this.lblPic2);
            this.groupBox3.Controls.Add(this.cmbInitialSprite);
            this.groupBox3.Controls.Add(this.lblPic);
            this.groupBox3.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox3.Location = new System.Drawing.Point(229, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(476, 437);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Graphics";
            // 
            // grpEndTileset
            // 
            this.grpEndTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpEndTileset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpEndTileset.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEndTileset.Controls.Add(this.picEndResource);
            this.grpEndTileset.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEndTileset.Location = new System.Drawing.Point(248, 62);
            this.grpEndTileset.Name = "grpEndTileset";
            this.grpEndTileset.Size = new System.Drawing.Size(195, 341);
            this.grpEndTileset.TabIndex = 23;
            this.grpEndTileset.TabStop = false;
            // 
            // picEndResource
            // 
            this.picEndResource.Location = new System.Drawing.Point(0, 0);
            this.picEndResource.Name = "picEndResource";
            this.picEndResource.Size = new System.Drawing.Size(195, 341);
            this.picEndResource.TabIndex = 2;
            this.picEndResource.TabStop = false;
            // 
            // grpInitialTileset
            // 
            this.grpInitialTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpInitialTileset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpInitialTileset.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpInitialTileset.Controls.Add(this.picInitialResource);
            this.grpInitialTileset.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpInitialTileset.Location = new System.Drawing.Point(13, 62);
            this.grpInitialTileset.Name = "grpInitialTileset";
            this.grpInitialTileset.Size = new System.Drawing.Size(195, 341);
            this.grpInitialTileset.TabIndex = 22;
            this.grpInitialTileset.TabStop = false;
            // 
            // picInitialResource
            // 
            this.picInitialResource.Location = new System.Drawing.Point(0, 0);
            this.picInitialResource.Name = "picInitialResource";
            this.picInitialResource.Size = new System.Drawing.Size(195, 341);
            this.picInitialResource.TabIndex = 2;
            this.picInitialResource.TabStop = false;
            // 
            // hScrollEndTileset
            // 
            this.hScrollEndTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollEndTileset.LargeChange = 1;
            this.hScrollEndTileset.Location = new System.Drawing.Point(248, 406);
            this.hScrollEndTileset.MaximumSize = new System.Drawing.Size(960, 17);
            this.hScrollEndTileset.Name = "hScrollEndTileset";
            this.hScrollEndTileset.Size = new System.Drawing.Size(199, 17);
            this.hScrollEndTileset.TabIndex = 21;
            this.hScrollEndTileset.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollEndTileset_Scroll);
            // 
            // vScrollEndTileset
            // 
            this.vScrollEndTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollEndTileset.LargeChange = 1;
            this.vScrollEndTileset.Location = new System.Drawing.Point(451, 59);
            this.vScrollEndTileset.MaximumSize = new System.Drawing.Size(17, 960);
            this.vScrollEndTileset.Name = "vScrollEndTileset";
            this.vScrollEndTileset.Size = new System.Drawing.Size(17, 344);
            this.vScrollEndTileset.TabIndex = 20;
            this.vScrollEndTileset.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollEndTileset_Scroll);
            // 
            // hScrollStartTileset
            // 
            this.hScrollStartTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollStartTileset.LargeChange = 1;
            this.hScrollStartTileset.Location = new System.Drawing.Point(13, 406);
            this.hScrollStartTileset.MaximumSize = new System.Drawing.Size(960, 17);
            this.hScrollStartTileset.Name = "hScrollStartTileset";
            this.hScrollStartTileset.Size = new System.Drawing.Size(199, 17);
            this.hScrollStartTileset.TabIndex = 19;
            this.hScrollStartTileset.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollStartTileset_Scroll);
            // 
            // vScrollStartTileset
            // 
            this.vScrollStartTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollStartTileset.LargeChange = 1;
            this.vScrollStartTileset.Location = new System.Drawing.Point(216, 59);
            this.vScrollStartTileset.MaximumSize = new System.Drawing.Size(17, 960);
            this.vScrollStartTileset.Name = "vScrollStartTileset";
            this.vScrollStartTileset.Size = new System.Drawing.Size(17, 344);
            this.vScrollStartTileset.TabIndex = 18;
            this.vScrollStartTileset.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollStartTileset_Scroll);
            // 
            // cmbEndSprite
            // 
            this.cmbEndSprite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEndSprite.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbEndSprite.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbEndSprite.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEndSprite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEndSprite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEndSprite.FormattingEnabled = true;
            this.cmbEndSprite.Items.AddRange(new object[] {
            "None"});
            this.cmbEndSprite.Location = new System.Drawing.Point(248, 32);
            this.cmbEndSprite.Name = "cmbEndSprite";
            this.cmbEndSprite.Size = new System.Drawing.Size(196, 21);
            this.cmbEndSprite.TabIndex = 16;
            this.cmbEndSprite.SelectedIndexChanged += new System.EventHandler(this.cmbEndSprite_SelectedIndexChanged);
            // 
            // lblPic2
            // 
            this.lblPic2.AutoSize = true;
            this.lblPic2.Location = new System.Drawing.Point(245, 16);
            this.lblPic2.Name = "lblPic2";
            this.lblPic2.Size = new System.Drawing.Size(96, 13);
            this.lblPic2.TabIndex = 15;
            this.lblPic2.Text = "Removed Graphic:";
            // 
            // cmbInitialSprite
            // 
            this.cmbInitialSprite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbInitialSprite.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbInitialSprite.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbInitialSprite.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbInitialSprite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInitialSprite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbInitialSprite.FormattingEnabled = true;
            this.cmbInitialSprite.Items.AddRange(new object[] {
            "None"});
            this.cmbInitialSprite.Location = new System.Drawing.Point(13, 32);
            this.cmbInitialSprite.Name = "cmbInitialSprite";
            this.cmbInitialSprite.Size = new System.Drawing.Size(195, 21);
            this.cmbInitialSprite.TabIndex = 14;
            this.cmbInitialSprite.SelectedIndexChanged += new System.EventHandler(this.cmbInitialSprite_SelectedIndexChanged);
            // 
            // lblPic
            // 
            this.lblPic.AutoSize = true;
            this.lblPic.Location = new System.Drawing.Point(10, 16);
            this.lblPic.Name = "lblPic";
            this.lblPic.Size = new System.Drawing.Size(74, 13);
            this.lblPic.TabIndex = 13;
            this.lblPic.Text = "Initial Graphic:";
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox4.Controls.Add(this.txtDropAmount);
            this.groupBox4.Controls.Add(this.lblDropAmount);
            this.groupBox4.Controls.Add(this.scrlDropChance);
            this.groupBox4.Controls.Add(this.lblDropChance);
            this.groupBox4.Controls.Add(this.scrlDropItem);
            this.groupBox4.Controls.Add(this.lblDropItem);
            this.groupBox4.Controls.Add(this.scrlDropIndex);
            this.groupBox4.Controls.Add(this.lblDropIndex);
            this.groupBox4.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox4.Location = new System.Drawing.Point(3, 270);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(220, 167);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Drops";
            // 
            // txtDropAmount
            // 
            this.txtDropAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDropAmount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDropAmount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDropAmount.Location = new System.Drawing.Point(9, 99);
            this.txtDropAmount.Name = "txtDropAmount";
            this.txtDropAmount.Size = new System.Drawing.Size(197, 20);
            this.txtDropAmount.TabIndex = 16;
            this.txtDropAmount.TextChanged += new System.EventHandler(this.txtDropAmount_TextChanged);
            // 
            // lblDropAmount
            // 
            this.lblDropAmount.AutoSize = true;
            this.lblDropAmount.Location = new System.Drawing.Point(6, 82);
            this.lblDropAmount.Name = "lblDropAmount";
            this.lblDropAmount.Size = new System.Drawing.Size(46, 13);
            this.lblDropAmount.TabIndex = 15;
            this.lblDropAmount.Text = "Amount:";
            // 
            // scrlDropChance
            // 
            this.scrlDropChance.LargeChange = 1;
            this.scrlDropChance.Location = new System.Drawing.Point(6, 135);
            this.scrlDropChance.Name = "scrlDropChance";
            this.scrlDropChance.Size = new System.Drawing.Size(200, 18);
            this.scrlDropChance.TabIndex = 14;
            this.scrlDropChance.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDropChance_Scroll);
            // 
            // lblDropChance
            // 
            this.lblDropChance.AutoSize = true;
            this.lblDropChance.Location = new System.Drawing.Point(6, 120);
            this.lblDropChance.Name = "lblDropChance";
            this.lblDropChance.Size = new System.Drawing.Size(85, 13);
            this.lblDropChance.TabIndex = 13;
            this.lblDropChance.Text = "Chance (0/100):";
            // 
            // scrlDropItem
            // 
            this.scrlDropItem.LargeChange = 1;
            this.scrlDropItem.Location = new System.Drawing.Point(6, 64);
            this.scrlDropItem.Maximum = 3600;
            this.scrlDropItem.Minimum = -1;
            this.scrlDropItem.Name = "scrlDropItem";
            this.scrlDropItem.Size = new System.Drawing.Size(201, 18);
            this.scrlDropItem.TabIndex = 12;
            this.scrlDropItem.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDropItem_Scroll);
            // 
            // lblDropItem
            // 
            this.lblDropItem.AutoSize = true;
            this.lblDropItem.Location = new System.Drawing.Point(6, 49);
            this.lblDropItem.Name = "lblDropItem";
            this.lblDropItem.Size = new System.Drawing.Size(59, 13);
            this.lblDropItem.TabIndex = 11;
            this.lblDropItem.Text = "Item: None";
            // 
            // scrlDropIndex
            // 
            this.scrlDropIndex.LargeChange = 1;
            this.scrlDropIndex.Location = new System.Drawing.Point(6, 31);
            this.scrlDropIndex.Maximum = 10;
            this.scrlDropIndex.Minimum = 1;
            this.scrlDropIndex.Name = "scrlDropIndex";
            this.scrlDropIndex.Size = new System.Drawing.Size(200, 18);
            this.scrlDropIndex.TabIndex = 10;
            this.scrlDropIndex.Value = 1;
            this.scrlDropIndex.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDropIndex_Scroll);
            // 
            // lblDropIndex
            // 
            this.lblDropIndex.AutoSize = true;
            this.lblDropIndex.Location = new System.Drawing.Point(6, 16);
            this.lblDropIndex.Name = "lblDropIndex";
            this.lblDropIndex.Size = new System.Drawing.Size(71, 13);
            this.lblDropIndex.TabIndex = 9;
            this.lblDropIndex.Text = "Drop Index: 1";
            // 
            // tmrRender
            // 
            this.tmrRender.Enabled = true;
            this.tmrRender.Interval = 10;
            this.tmrRender.Tick += new System.EventHandler(this.tmrRender_Tick);
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.groupBox4);
            this.pnlContainer.Controls.Add(this.groupBox2);
            this.pnlContainer.Controls.Add(this.groupBox3);
            this.pnlContainer.Location = new System.Drawing.Point(221, 39);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(705, 437);
            this.pnlContainer.TabIndex = 18;
            this.pnlContainer.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(736, 482);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 44;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(540, 482);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 41;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.AutoSize = false;
            this.toolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.toolStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripItemNew,
            this.toolStripSeparator1,
            this.toolStripItemDelete,
            this.toolStripSeparator2,
            this.toolStripItemCopy,
            this.toolStripItemPaste,
            this.toolStripSeparator3,
            this.toolStripItemUndo});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.toolStrip.Size = new System.Drawing.Size(936, 25);
            this.toolStrip.TabIndex = 47;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripItemNew
            // 
            this.toolStripItemNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemNew.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripItemNew.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemNew.Image")));
            this.toolStripItemNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemNew.Name = "toolStripItemNew";
            this.toolStripItemNew.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemNew.Text = "New";
            this.toolStripItemNew.Click += new System.EventHandler(this.toolStripItemNew_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripItemDelete
            // 
            this.toolStripItemDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemDelete.Enabled = false;
            this.toolStripItemDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripItemDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemDelete.Image")));
            this.toolStripItemDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemDelete.Name = "toolStripItemDelete";
            this.toolStripItemDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemDelete.Text = "Delete";
            this.toolStripItemDelete.Click += new System.EventHandler(this.toolStripItemDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripItemCopy
            // 
            this.toolStripItemCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemCopy.Enabled = false;
            this.toolStripItemCopy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripItemCopy.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemCopy.Image")));
            this.toolStripItemCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemCopy.Name = "toolStripItemCopy";
            this.toolStripItemCopy.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemCopy.Text = "Copy";
            this.toolStripItemCopy.Click += new System.EventHandler(this.toolStripItemCopy_Click);
            // 
            // toolStripItemPaste
            // 
            this.toolStripItemPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemPaste.Enabled = false;
            this.toolStripItemPaste.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripItemPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemPaste.Image")));
            this.toolStripItemPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemPaste.Name = "toolStripItemPaste";
            this.toolStripItemPaste.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemPaste.Text = "Paste";
            this.toolStripItemPaste.Click += new System.EventHandler(this.toolStripItemPaste_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripItemUndo
            // 
            this.toolStripItemUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemUndo.Enabled = false;
            this.toolStripItemUndo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripItemUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemUndo.Image")));
            this.toolStripItemUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemUndo.Name = "toolStripItemUndo";
            this.toolStripItemUndo.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemUndo.Text = "Undo";
            this.toolStripItemUndo.Click += new System.EventHandler(this.toolStripItemUndo_Click);
            // 
            // frmResource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(936, 514);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "frmResource";
            this.Text = "Resource Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmResource_FormClosed);
            this.Load += new System.EventHandler(this.frmResource_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grpEndTileset.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picEndResource)).EndInit();
            this.grpInitialTileset.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picInitialResource)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.pnlContainer.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox groupBox1;
        private System.Windows.Forms.ListBox lstResources;
        private DarkGroupBox groupBox2;
        private DarkGroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private DarkTextBox txtName;
        private DarkTextBox txtHP;
        private System.Windows.Forms.Label lblHP;
        private DarkCheckBox chkWalkableAfter;
        private DarkCheckBox chkWalkableBefore;
        private DarkComboBox cmbToolType;
        private System.Windows.Forms.Label lblToolType;
        private DarkGroupBox groupBox4;
        private DarkTextBox txtDropAmount;
        private System.Windows.Forms.Label lblDropAmount;
        private System.Windows.Forms.HScrollBar scrlDropChance;
        private System.Windows.Forms.Label lblDropChance;
        private System.Windows.Forms.HScrollBar scrlDropItem;
        private System.Windows.Forms.Label lblDropItem;
        private System.Windows.Forms.HScrollBar scrlDropIndex;
        private System.Windows.Forms.Label lblDropIndex;
        private DarkComboBox cmbEndSprite;
        private System.Windows.Forms.Label lblPic2;
        private DarkComboBox cmbInitialSprite;
        private System.Windows.Forms.Label lblPic;
        private System.Windows.Forms.HScrollBar scrlSpawnDuration;
        private System.Windows.Forms.Label lblSpawnDuration;
        private System.Windows.Forms.HScrollBar hScrollEndTileset;
        private System.Windows.Forms.VScrollBar vScrollEndTileset;
        private System.Windows.Forms.HScrollBar hScrollStartTileset;
        private System.Windows.Forms.VScrollBar vScrollStartTileset;
        private DarkGroupBox grpEndTileset;
        public System.Windows.Forms.PictureBox picEndResource;
        private DarkGroupBox grpInitialTileset;
        public System.Windows.Forms.PictureBox picInitialResource;
        private DarkTextBox txtMaxHp;
        private System.Windows.Forms.Label lblMaxHp;
        private System.Windows.Forms.HScrollBar scrlAnimation;
        private System.Windows.Forms.Label lblAnimation;
        private System.Windows.Forms.Timer tmrRender;
        private System.Windows.Forms.Panel pnlContainer;
        private DarkButton btnSave;
        private DarkButton btnCancel;
        private DarkToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripItemNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripItemDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton toolStripItemCopy;
        public System.Windows.Forms.ToolStripButton toolStripItemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.ToolStripButton toolStripItemUndo;
    }
}