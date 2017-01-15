using System.Windows.Forms;
using DarkUI.Controls;
using Intersect_Editor.Forms.Controls;

namespace Intersect_Editor.Forms
{
    partial class frmMapLayers
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
            this.lblLayer = new System.Windows.Forms.Label();
            this.lblTileType = new System.Windows.Forms.Label();
            this.lblTileset = new System.Windows.Forms.Label();
            this.cmbAutotile = new DarkUI.Controls.DarkComboBox();
            this.cmbTilesets = new DarkUI.Controls.DarkComboBox();
            this.rbSlide = new DarkUI.Controls.DarkRadioButton();
            this.rbGrappleStone = new DarkUI.Controls.DarkRadioButton();
            this.rbAnimation = new DarkUI.Controls.DarkRadioButton();
            this.rbResource = new DarkUI.Controls.DarkRadioButton();
            this.rbSound = new DarkUI.Controls.DarkRadioButton();
            this.rbWarp = new DarkUI.Controls.DarkRadioButton();
            this.rbNPCAvoid = new DarkUI.Controls.DarkRadioButton();
            this.rbZDimension = new DarkUI.Controls.DarkRadioButton();
            this.rbItem = new DarkUI.Controls.DarkRadioButton();
            this.rbBlocked = new DarkUI.Controls.DarkRadioButton();
            this.grpResource = new DarkUI.Controls.DarkGroupBox();
            this.grpZResource = new DarkUI.Controls.DarkGroupBox();
            this.rbLevel2 = new DarkUI.Controls.DarkRadioButton();
            this.rbLevel1 = new DarkUI.Controls.DarkRadioButton();
            this.cmbResourceAttribute = new DarkUI.Controls.DarkComboBox();
            this.lblResource = new System.Windows.Forms.Label();
            this.grpItem = new DarkUI.Controls.DarkGroupBox();
            this.cmbItemAttribute = new DarkUI.Controls.DarkComboBox();
            this.lblMaxItemAmount = new System.Windows.Forms.Label();
            this.lblMapItem = new System.Windows.Forms.Label();
            this.scrlMaxItemVal = new DarkUI.Controls.DarkScrollBar();
            this.grpWarp = new DarkUI.Controls.DarkGroupBox();
            this.btnVisualMapSelector = new DarkUI.Controls.DarkButton();
            this.cmbWarpMap = new DarkUI.Controls.DarkComboBox();
            this.cmbDirection = new DarkUI.Controls.DarkComboBox();
            this.lblWarpDir = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblMap = new System.Windows.Forms.Label();
            this.scrlX = new DarkUI.Controls.DarkScrollBar();
            this.scrlY = new DarkUI.Controls.DarkScrollBar();
            this.grpZDimension = new DarkUI.Controls.DarkGroupBox();
            this.grpGateway = new DarkUI.Controls.DarkGroupBox();
            this.rbGateway2 = new DarkUI.Controls.DarkRadioButton();
            this.rbGateway1 = new DarkUI.Controls.DarkRadioButton();
            this.rbGatewayNone = new DarkUI.Controls.DarkRadioButton();
            this.grpDimBlock = new DarkUI.Controls.DarkGroupBox();
            this.rbBlock2 = new DarkUI.Controls.DarkRadioButton();
            this.rbBlock1 = new DarkUI.Controls.DarkRadioButton();
            this.rbBlockNone = new DarkUI.Controls.DarkRadioButton();
            this.grpSound = new DarkUI.Controls.DarkGroupBox();
            this.cmbMapAttributeSound = new DarkUI.Controls.DarkComboBox();
            this.lblSoundDistance = new System.Windows.Forms.Label();
            this.lblMapSound = new System.Windows.Forms.Label();
            this.scrlSoundDistance = new DarkUI.Controls.DarkScrollBar();
            this.grpSlide = new DarkUI.Controls.DarkGroupBox();
            this.cmbSlideDir = new DarkUI.Controls.DarkComboBox();
            this.lblSlideDir = new System.Windows.Forms.Label();
            this.grpAnimation = new DarkUI.Controls.DarkGroupBox();
            this.cmbAnimationAttribute = new DarkUI.Controls.DarkComboBox();
            this.lblAnimation = new System.Windows.Forms.Label();
            this.lblLightInstructions = new System.Windows.Forms.Label();
            this.lblEventInstructions = new System.Windows.Forms.Label();
            this.grpNpcList = new DarkUI.Controls.DarkGroupBox();
            this.btnRemoveMapNpc = new DarkUI.Controls.DarkButton();
            this.btnAddMapNpc = new DarkUI.Controls.DarkButton();
            this.cmbNpc = new DarkUI.Controls.DarkComboBox();
            this.grpSpawnLoc = new DarkUI.Controls.DarkGroupBox();
            this.cmbDir = new DarkUI.Controls.DarkComboBox();
            this.lblDir = new System.Windows.Forms.Label();
            this.rbRandom = new DarkUI.Controls.DarkRadioButton();
            this.rbDeclared = new DarkUI.Controls.DarkRadioButton();
            this.lstMapNpcs = new System.Windows.Forms.ListBox();
            this.btnTileHeader = new DarkUI.Controls.DarkButton();
            this.btnAttributeHeader = new DarkUI.Controls.DarkButton();
            this.btnLightsHeader = new DarkUI.Controls.DarkButton();
            this.btnEventsHeader = new DarkUI.Controls.DarkButton();
            this.btnNpcsHeader = new DarkUI.Controls.DarkButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlTiles = new System.Windows.Forms.Panel();
            this.picFringe2 = new System.Windows.Forms.PictureBox();
            this.picFringe = new System.Windows.Forms.PictureBox();
            this.picMask2 = new System.Windows.Forms.PictureBox();
            this.picMask = new System.Windows.Forms.PictureBox();
            this.picGround = new System.Windows.Forms.PictureBox();
            this.pnlTilesetContainer = new Intersect_Editor.Forms.Controls.AutoDragPanel();
            this.picTileset = new System.Windows.Forms.PictureBox();
            this.pnlEvents = new System.Windows.Forms.Panel();
            this.pnlLights = new System.Windows.Forms.Panel();
            this.lightEditor = new Intersect_Editor.Forms.Controls.LightEditorCtrl();
            this.pnlAttributes = new System.Windows.Forms.Panel();
            this.pnlNpcs = new System.Windows.Forms.Panel();
            this.grpResource.SuspendLayout();
            this.grpZResource.SuspendLayout();
            this.grpItem.SuspendLayout();
            this.grpWarp.SuspendLayout();
            this.grpZDimension.SuspendLayout();
            this.grpGateway.SuspendLayout();
            this.grpDimBlock.SuspendLayout();
            this.grpSound.SuspendLayout();
            this.grpSlide.SuspendLayout();
            this.grpAnimation.SuspendLayout();
            this.grpNpcList.SuspendLayout();
            this.grpSpawnLoc.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlTiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFringe2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFringe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMask2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMask)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGround)).BeginInit();
            this.pnlTilesetContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTileset)).BeginInit();
            this.pnlEvents.SuspendLayout();
            this.pnlLights.SuspendLayout();
            this.pnlAttributes.SuspendLayout();
            this.pnlNpcs.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLayer
            // 
            this.lblLayer.AutoSize = true;
            this.lblLayer.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblLayer.Location = new System.Drawing.Point(9, 12);
            this.lblLayer.Name = "lblLayer";
            this.lblLayer.Size = new System.Drawing.Size(36, 13);
            this.lblLayer.TabIndex = 23;
            this.lblLayer.Text = "Layer:";
            // 
            // lblTileType
            // 
            this.lblTileType.AutoSize = true;
            this.lblTileType.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblTileType.Location = new System.Drawing.Point(9, 68);
            this.lblTileType.Name = "lblTileType";
            this.lblTileType.Size = new System.Drawing.Size(54, 13);
            this.lblTileType.TabIndex = 21;
            this.lblTileType.Text = "Tile Type:";
            // 
            // lblTileset
            // 
            this.lblTileset.AutoSize = true;
            this.lblTileset.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblTileset.Location = new System.Drawing.Point(9, 39);
            this.lblTileset.Name = "lblTileset";
            this.lblTileset.Size = new System.Drawing.Size(41, 13);
            this.lblTileset.TabIndex = 20;
            this.lblTileset.Text = "Tileset:";
            // 
            // cmbAutotile
            // 
            this.cmbAutotile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbAutotile.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAutotile.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAutotile.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAutotile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutotile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAutotile.FormattingEnabled = true;
            this.cmbAutotile.Items.AddRange(new object[] {
            "Normal",
            "Autotile",
            "Fake",
            "Animated",
            "Cliff",
            "Waterfall"});
            this.cmbAutotile.Location = new System.Drawing.Point(84, 65);
            this.cmbAutotile.Name = "cmbAutotile";
            this.cmbAutotile.Size = new System.Drawing.Size(178, 21);
            this.cmbAutotile.TabIndex = 18;
            this.cmbAutotile.SelectedIndexChanged += new System.EventHandler(this.cmbAutotile_SelectedIndexChanged);
            // 
            // cmbTilesets
            // 
            this.cmbTilesets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbTilesets.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTilesets.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTilesets.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTilesets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTilesets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTilesets.FormattingEnabled = true;
            this.cmbTilesets.Location = new System.Drawing.Point(84, 36);
            this.cmbTilesets.Name = "cmbTilesets";
            this.cmbTilesets.Size = new System.Drawing.Size(178, 21);
            this.cmbTilesets.TabIndex = 17;
            this.cmbTilesets.SelectedIndexChanged += new System.EventHandler(this.cmbTilesets_SelectedIndexChanged);
            // 
            // rbSlide
            // 
            this.rbSlide.AutoSize = true;
            this.rbSlide.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbSlide.Location = new System.Drawing.Point(115, 30);
            this.rbSlide.Name = "rbSlide";
            this.rbSlide.Size = new System.Drawing.Size(48, 17);
            this.rbSlide.TabIndex = 35;
            this.rbSlide.Text = "Slide";
            this.rbSlide.CheckedChanged += new System.EventHandler(this.rbSlide_CheckedChanged);
            // 
            // rbGrappleStone
            // 
            this.rbGrappleStone.AutoSize = true;
            this.rbGrappleStone.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbGrappleStone.Location = new System.Drawing.Point(115, 8);
            this.rbGrappleStone.Name = "rbGrappleStone";
            this.rbGrappleStone.Size = new System.Drawing.Size(93, 17);
            this.rbGrappleStone.TabIndex = 34;
            this.rbGrappleStone.Text = "Grapple Stone";
            this.rbGrappleStone.CheckedChanged += new System.EventHandler(this.rbGrappleStone_CheckedChanged);
            // 
            // rbAnimation
            // 
            this.rbAnimation.AutoSize = true;
            this.rbAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbAnimation.Location = new System.Drawing.Point(5, 170);
            this.rbAnimation.Name = "rbAnimation";
            this.rbAnimation.Size = new System.Drawing.Size(71, 17);
            this.rbAnimation.TabIndex = 32;
            this.rbAnimation.Text = "Animation";
            this.rbAnimation.CheckedChanged += new System.EventHandler(this.rbAnimation_CheckedChanged);
            // 
            // rbResource
            // 
            this.rbResource.AutoSize = true;
            this.rbResource.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbResource.Location = new System.Drawing.Point(5, 147);
            this.rbResource.Name = "rbResource";
            this.rbResource.Size = new System.Drawing.Size(71, 17);
            this.rbResource.TabIndex = 30;
            this.rbResource.Text = "Resource";
            this.rbResource.CheckedChanged += new System.EventHandler(this.rbResource_CheckedChanged);
            // 
            // rbSound
            // 
            this.rbSound.AutoSize = true;
            this.rbSound.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbSound.Location = new System.Drawing.Point(5, 124);
            this.rbSound.Name = "rbSound";
            this.rbSound.Size = new System.Drawing.Size(80, 17);
            this.rbSound.TabIndex = 28;
            this.rbSound.Text = "Map Sound";
            this.rbSound.CheckedChanged += new System.EventHandler(this.rbSound_CheckedChanged);
            // 
            // rbWarp
            // 
            this.rbWarp.AutoSize = true;
            this.rbWarp.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbWarp.Location = new System.Drawing.Point(5, 77);
            this.rbWarp.Name = "rbWarp";
            this.rbWarp.Size = new System.Drawing.Size(51, 17);
            this.rbWarp.TabIndex = 25;
            this.rbWarp.Text = "Warp";
            this.rbWarp.CheckedChanged += new System.EventHandler(this.rbWarp_CheckedChanged);
            // 
            // rbNPCAvoid
            // 
            this.rbNPCAvoid.AutoSize = true;
            this.rbNPCAvoid.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbNPCAvoid.Location = new System.Drawing.Point(5, 54);
            this.rbNPCAvoid.Name = "rbNPCAvoid";
            this.rbNPCAvoid.Size = new System.Drawing.Size(77, 17);
            this.rbNPCAvoid.TabIndex = 24;
            this.rbNPCAvoid.Text = "NPC Avoid";
            this.rbNPCAvoid.CheckedChanged += new System.EventHandler(this.rbNPCAvoid_CheckedChanged);
            // 
            // rbZDimension
            // 
            this.rbZDimension.AutoSize = true;
            this.rbZDimension.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbZDimension.Location = new System.Drawing.Point(5, 30);
            this.rbZDimension.Name = "rbZDimension";
            this.rbZDimension.Size = new System.Drawing.Size(84, 17);
            this.rbZDimension.TabIndex = 23;
            this.rbZDimension.Text = "Z-Dimension";
            this.rbZDimension.CheckedChanged += new System.EventHandler(this.rbZDimension_CheckedChanged);
            // 
            // rbItem
            // 
            this.rbItem.AutoSize = true;
            this.rbItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbItem.Location = new System.Drawing.Point(5, 100);
            this.rbItem.Name = "rbItem";
            this.rbItem.Size = new System.Drawing.Size(81, 17);
            this.rbItem.TabIndex = 21;
            this.rbItem.Text = "Item Spawn";
            this.rbItem.CheckedChanged += new System.EventHandler(this.rbItem_CheckedChanged);
            // 
            // rbBlocked
            // 
            this.rbBlocked.AutoSize = true;
            this.rbBlocked.Checked = true;
            this.rbBlocked.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbBlocked.Location = new System.Drawing.Point(6, 8);
            this.rbBlocked.Name = "rbBlocked";
            this.rbBlocked.Size = new System.Drawing.Size(64, 17);
            this.rbBlocked.TabIndex = 20;
            this.rbBlocked.TabStop = true;
            this.rbBlocked.Text = "Blocked";
            this.rbBlocked.CheckedChanged += new System.EventHandler(this.rbBlocked_CheckedChanged);
            // 
            // grpResource
            // 
            this.grpResource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpResource.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpResource.Controls.Add(this.grpZResource);
            this.grpResource.Controls.Add(this.cmbResourceAttribute);
            this.grpResource.Controls.Add(this.lblResource);
            this.grpResource.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpResource.Location = new System.Drawing.Point(6, 202);
            this.grpResource.Name = "grpResource";
            this.grpResource.Size = new System.Drawing.Size(256, 116);
            this.grpResource.TabIndex = 31;
            this.grpResource.TabStop = false;
            this.grpResource.Text = "Resource";
            this.grpResource.Visible = false;
            // 
            // grpZResource
            // 
            this.grpZResource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpZResource.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpZResource.Controls.Add(this.rbLevel2);
            this.grpZResource.Controls.Add(this.rbLevel1);
            this.grpZResource.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpZResource.Location = new System.Drawing.Point(16, 65);
            this.grpZResource.Name = "grpZResource";
            this.grpZResource.Size = new System.Drawing.Size(224, 45);
            this.grpZResource.TabIndex = 12;
            this.grpZResource.TabStop = false;
            this.grpZResource.Text = "Z-Dimension";
            // 
            // rbLevel2
            // 
            this.rbLevel2.AutoSize = true;
            this.rbLevel2.Location = new System.Drawing.Point(158, 20);
            this.rbLevel2.Name = "rbLevel2";
            this.rbLevel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rbLevel2.Size = new System.Drawing.Size(60, 17);
            this.rbLevel2.TabIndex = 15;
            this.rbLevel2.Text = "Level 2";
            // 
            // rbLevel1
            // 
            this.rbLevel1.AutoSize = true;
            this.rbLevel1.Checked = true;
            this.rbLevel1.Location = new System.Drawing.Point(6, 20);
            this.rbLevel1.Name = "rbLevel1";
            this.rbLevel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.rbLevel1.Size = new System.Drawing.Size(60, 17);
            this.rbLevel1.TabIndex = 14;
            this.rbLevel1.TabStop = true;
            this.rbLevel1.Text = "Level 1";
            // 
            // cmbResourceAttribute
            // 
            this.cmbResourceAttribute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbResourceAttribute.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbResourceAttribute.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbResourceAttribute.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbResourceAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResourceAttribute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbResourceAttribute.FormattingEnabled = true;
            this.cmbResourceAttribute.Location = new System.Drawing.Point(17, 36);
            this.cmbResourceAttribute.Name = "cmbResourceAttribute";
            this.cmbResourceAttribute.Size = new System.Drawing.Size(222, 21);
            this.cmbResourceAttribute.TabIndex = 11;
            // 
            // lblResource
            // 
            this.lblResource.AutoSize = true;
            this.lblResource.Location = new System.Drawing.Point(14, 16);
            this.lblResource.Name = "lblResource";
            this.lblResource.Size = new System.Drawing.Size(56, 13);
            this.lblResource.TabIndex = 10;
            this.lblResource.Text = "Resource:";
            // 
            // grpItem
            // 
            this.grpItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpItem.Controls.Add(this.cmbItemAttribute);
            this.grpItem.Controls.Add(this.lblMaxItemAmount);
            this.grpItem.Controls.Add(this.lblMapItem);
            this.grpItem.Controls.Add(this.scrlMaxItemVal);
            this.grpItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpItem.Location = new System.Drawing.Point(6, 202);
            this.grpItem.Name = "grpItem";
            this.grpItem.Size = new System.Drawing.Size(246, 98);
            this.grpItem.TabIndex = 22;
            this.grpItem.TabStop = false;
            this.grpItem.Text = "Map Item";
            this.grpItem.Visible = false;
            // 
            // cmbItemAttribute
            // 
            this.cmbItemAttribute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbItemAttribute.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbItemAttribute.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbItemAttribute.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbItemAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItemAttribute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbItemAttribute.FormattingEnabled = true;
            this.cmbItemAttribute.Location = new System.Drawing.Point(16, 32);
            this.cmbItemAttribute.Name = "cmbItemAttribute";
            this.cmbItemAttribute.Size = new System.Drawing.Size(219, 21);
            this.cmbItemAttribute.TabIndex = 9;
            // 
            // lblMaxItemAmount
            // 
            this.lblMaxItemAmount.AutoSize = true;
            this.lblMaxItemAmount.Location = new System.Drawing.Point(13, 54);
            this.lblMaxItemAmount.Name = "lblMaxItemAmount";
            this.lblMaxItemAmount.Size = new System.Drawing.Size(63, 13);
            this.lblMaxItemAmount.TabIndex = 8;
            this.lblMaxItemAmount.Text = "Quantity: x1";
            // 
            // lblMapItem
            // 
            this.lblMapItem.AutoSize = true;
            this.lblMapItem.Location = new System.Drawing.Point(13, 16);
            this.lblMapItem.Name = "lblMapItem";
            this.lblMapItem.Size = new System.Drawing.Size(30, 13);
            this.lblMapItem.TabIndex = 7;
            this.lblMapItem.Text = "Item:";
            // 
            // scrlMaxItemVal
            // 
            this.scrlMaxItemVal.Location = new System.Drawing.Point(16, 70);
            this.scrlMaxItemVal.Maximum = 1000;
            this.scrlMaxItemVal.Minimum = 1;
            this.scrlMaxItemVal.Name = "scrlMaxItemVal";
            this.scrlMaxItemVal.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlMaxItemVal.Size = new System.Drawing.Size(219, 18);
            this.scrlMaxItemVal.TabIndex = 6;
            this.scrlMaxItemVal.Value = 1;
            this.scrlMaxItemVal.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlMaxItemVal_ValueChanged);
            // 
            // grpWarp
            // 
            this.grpWarp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpWarp.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpWarp.Controls.Add(this.btnVisualMapSelector);
            this.grpWarp.Controls.Add(this.cmbWarpMap);
            this.grpWarp.Controls.Add(this.cmbDirection);
            this.grpWarp.Controls.Add(this.lblWarpDir);
            this.grpWarp.Controls.Add(this.lblY);
            this.grpWarp.Controls.Add(this.lblX);
            this.grpWarp.Controls.Add(this.lblMap);
            this.grpWarp.Controls.Add(this.scrlX);
            this.grpWarp.Controls.Add(this.scrlY);
            this.grpWarp.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpWarp.Location = new System.Drawing.Point(6, 202);
            this.grpWarp.Name = "grpWarp";
            this.grpWarp.Size = new System.Drawing.Size(255, 162);
            this.grpWarp.TabIndex = 26;
            this.grpWarp.TabStop = false;
            this.grpWarp.Text = "Warp";
            this.grpWarp.Visible = false;
            // 
            // btnVisualMapSelector
            // 
            this.btnVisualMapSelector.Location = new System.Drawing.Point(16, 131);
            this.btnVisualMapSelector.Name = "btnVisualMapSelector";
            this.btnVisualMapSelector.Padding = new System.Windows.Forms.Padding(5);
            this.btnVisualMapSelector.Size = new System.Drawing.Size(222, 23);
            this.btnVisualMapSelector.TabIndex = 24;
            this.btnVisualMapSelector.Text = "Open Visual Interface";
            this.btnVisualMapSelector.Click += new System.EventHandler(this.btnVisualMapSelector_Click);
            // 
            // cmbWarpMap
            // 
            this.cmbWarpMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbWarpMap.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbWarpMap.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbWarpMap.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbWarpMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWarpMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbWarpMap.FormattingEnabled = true;
            this.cmbWarpMap.Location = new System.Drawing.Point(17, 30);
            this.cmbWarpMap.Name = "cmbWarpMap";
            this.cmbWarpMap.Size = new System.Drawing.Size(221, 21);
            this.cmbWarpMap.TabIndex = 12;
            // 
            // cmbDirection
            // 
            this.cmbDirection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbDirection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDirection.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDirection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDirection.FormattingEnabled = true;
            this.cmbDirection.Items.AddRange(new object[] {
            "Retain Direction",
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbDirection.Location = new System.Drawing.Point(46, 102);
            this.cmbDirection.Name = "cmbDirection";
            this.cmbDirection.Size = new System.Drawing.Size(192, 21);
            this.cmbDirection.TabIndex = 23;
            // 
            // lblWarpDir
            // 
            this.lblWarpDir.AutoSize = true;
            this.lblWarpDir.Location = new System.Drawing.Point(13, 105);
            this.lblWarpDir.Name = "lblWarpDir";
            this.lblWarpDir.Size = new System.Drawing.Size(23, 13);
            this.lblWarpDir.TabIndex = 22;
            this.lblWarpDir.Text = "Dir:";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(14, 82);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(26, 13);
            this.lblY.TabIndex = 11;
            this.lblY.Text = "Y: 0";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(14, 61);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(26, 13);
            this.lblX.TabIndex = 10;
            this.lblX.Text = "X: 0";
            // 
            // lblMap
            // 
            this.lblMap.AutoSize = true;
            this.lblMap.Location = new System.Drawing.Point(13, 14);
            this.lblMap.Name = "lblMap";
            this.lblMap.Size = new System.Drawing.Size(31, 13);
            this.lblMap.TabIndex = 9;
            this.lblMap.Text = "Map:";
            // 
            // scrlX
            // 
            this.scrlX.Location = new System.Drawing.Point(67, 59);
            this.scrlX.Name = "scrlX";
            this.scrlX.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlX.Size = new System.Drawing.Size(171, 16);
            this.scrlX.TabIndex = 8;
            this.scrlX.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlX_ValueChanged);
            // 
            // scrlY
            // 
            this.scrlY.Location = new System.Drawing.Point(67, 82);
            this.scrlY.Name = "scrlY";
            this.scrlY.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlY.Size = new System.Drawing.Size(171, 17);
            this.scrlY.TabIndex = 7;
            this.scrlY.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlY_ValueChanged);
            // 
            // grpZDimension
            // 
            this.grpZDimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpZDimension.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpZDimension.Controls.Add(this.grpGateway);
            this.grpZDimension.Controls.Add(this.grpDimBlock);
            this.grpZDimension.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpZDimension.Location = new System.Drawing.Point(6, 202);
            this.grpZDimension.Name = "grpZDimension";
            this.grpZDimension.Size = new System.Drawing.Size(257, 132);
            this.grpZDimension.TabIndex = 27;
            this.grpZDimension.TabStop = false;
            this.grpZDimension.Text = "Z-Dimension";
            this.grpZDimension.Visible = false;
            // 
            // grpGateway
            // 
            this.grpGateway.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpGateway.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpGateway.Controls.Add(this.rbGateway2);
            this.grpGateway.Controls.Add(this.rbGateway1);
            this.grpGateway.Controls.Add(this.rbGatewayNone);
            this.grpGateway.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpGateway.Location = new System.Drawing.Point(10, 25);
            this.grpGateway.Name = "grpGateway";
            this.grpGateway.Size = new System.Drawing.Size(107, 91);
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
            // 
            // rbGateway1
            // 
            this.rbGateway1.AutoSize = true;
            this.rbGateway1.Location = new System.Drawing.Point(6, 41);
            this.rbGateway1.Name = "rbGateway1";
            this.rbGateway1.Size = new System.Drawing.Size(60, 17);
            this.rbGateway1.TabIndex = 11;
            this.rbGateway1.Text = "Level 1";
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
            // 
            // grpDimBlock
            // 
            this.grpDimBlock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpDimBlock.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpDimBlock.Controls.Add(this.rbBlock2);
            this.grpDimBlock.Controls.Add(this.rbBlock1);
            this.grpDimBlock.Controls.Add(this.rbBlockNone);
            this.grpDimBlock.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpDimBlock.Location = new System.Drawing.Point(123, 25);
            this.grpDimBlock.Name = "grpDimBlock";
            this.grpDimBlock.Size = new System.Drawing.Size(111, 91);
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
            // 
            // rbBlock1
            // 
            this.rbBlock1.AutoSize = true;
            this.rbBlock1.Location = new System.Drawing.Point(6, 41);
            this.rbBlock1.Name = "rbBlock1";
            this.rbBlock1.Size = new System.Drawing.Size(60, 17);
            this.rbBlock1.TabIndex = 14;
            this.rbBlock1.Text = "Level 1";
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
            // 
            // grpSound
            // 
            this.grpSound.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpSound.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSound.Controls.Add(this.cmbMapAttributeSound);
            this.grpSound.Controls.Add(this.lblSoundDistance);
            this.grpSound.Controls.Add(this.lblMapSound);
            this.grpSound.Controls.Add(this.scrlSoundDistance);
            this.grpSound.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSound.Location = new System.Drawing.Point(6, 202);
            this.grpSound.Name = "grpSound";
            this.grpSound.Size = new System.Drawing.Size(252, 102);
            this.grpSound.TabIndex = 29;
            this.grpSound.TabStop = false;
            this.grpSound.Text = "Map Sound";
            this.grpSound.Visible = false;
            // 
            // cmbMapAttributeSound
            // 
            this.cmbMapAttributeSound.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbMapAttributeSound.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMapAttributeSound.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMapAttributeSound.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMapAttributeSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapAttributeSound.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMapAttributeSound.FormattingEnabled = true;
            this.cmbMapAttributeSound.Location = new System.Drawing.Point(16, 30);
            this.cmbMapAttributeSound.Name = "cmbMapAttributeSound";
            this.cmbMapAttributeSound.Size = new System.Drawing.Size(219, 21);
            this.cmbMapAttributeSound.TabIndex = 9;
            // 
            // lblSoundDistance
            // 
            this.lblSoundDistance.AutoSize = true;
            this.lblSoundDistance.Location = new System.Drawing.Point(13, 54);
            this.lblSoundDistance.Name = "lblSoundDistance";
            this.lblSoundDistance.Size = new System.Drawing.Size(92, 13);
            this.lblSoundDistance.TabIndex = 8;
            this.lblSoundDistance.Text = "Distance: 1 Tile(s)";
            // 
            // lblMapSound
            // 
            this.lblMapSound.AutoSize = true;
            this.lblMapSound.Location = new System.Drawing.Point(13, 16);
            this.lblMapSound.Name = "lblMapSound";
            this.lblMapSound.Size = new System.Drawing.Size(41, 13);
            this.lblMapSound.TabIndex = 7;
            this.lblMapSound.Text = "Sound:";
            // 
            // scrlSoundDistance
            // 
            this.scrlSoundDistance.Location = new System.Drawing.Point(16, 76);
            this.scrlSoundDistance.Maximum = 15;
            this.scrlSoundDistance.Minimum = 1;
            this.scrlSoundDistance.Name = "scrlSoundDistance";
            this.scrlSoundDistance.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlSoundDistance.Size = new System.Drawing.Size(219, 17);
            this.scrlSoundDistance.TabIndex = 6;
            this.scrlSoundDistance.Value = 1;
            this.scrlSoundDistance.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlSoundDistance_Scroll);
            // 
            // grpSlide
            // 
            this.grpSlide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpSlide.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSlide.Controls.Add(this.cmbSlideDir);
            this.grpSlide.Controls.Add(this.lblSlideDir);
            this.grpSlide.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSlide.Location = new System.Drawing.Point(5, 200);
            this.grpSlide.Name = "grpSlide";
            this.grpSlide.Size = new System.Drawing.Size(259, 75);
            this.grpSlide.TabIndex = 36;
            this.grpSlide.TabStop = false;
            this.grpSlide.Text = "Slide";
            this.grpSlide.Visible = false;
            // 
            // cmbSlideDir
            // 
            this.cmbSlideDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbSlideDir.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSlideDir.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSlideDir.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSlideDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSlideDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSlideDir.FormattingEnabled = true;
            this.cmbSlideDir.Items.AddRange(new object[] {
            "Retain Direction",
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbSlideDir.Location = new System.Drawing.Point(50, 27);
            this.cmbSlideDir.Name = "cmbSlideDir";
            this.cmbSlideDir.Size = new System.Drawing.Size(192, 21);
            this.cmbSlideDir.TabIndex = 25;
            // 
            // lblSlideDir
            // 
            this.lblSlideDir.AutoSize = true;
            this.lblSlideDir.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblSlideDir.Location = new System.Drawing.Point(17, 30);
            this.lblSlideDir.Name = "lblSlideDir";
            this.lblSlideDir.Size = new System.Drawing.Size(23, 13);
            this.lblSlideDir.TabIndex = 24;
            this.lblSlideDir.Text = "Dir:";
            // 
            // grpAnimation
            // 
            this.grpAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpAnimation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpAnimation.Controls.Add(this.cmbAnimationAttribute);
            this.grpAnimation.Controls.Add(this.lblAnimation);
            this.grpAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpAnimation.Location = new System.Drawing.Point(6, 202);
            this.grpAnimation.Name = "grpAnimation";
            this.grpAnimation.Size = new System.Drawing.Size(256, 69);
            this.grpAnimation.TabIndex = 33;
            this.grpAnimation.TabStop = false;
            this.grpAnimation.Text = "Animaton";
            this.grpAnimation.Visible = false;
            // 
            // cmbAnimationAttribute
            // 
            this.cmbAnimationAttribute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbAnimationAttribute.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAnimationAttribute.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAnimationAttribute.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAnimationAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnimationAttribute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAnimationAttribute.FormattingEnabled = true;
            this.cmbAnimationAttribute.Location = new System.Drawing.Point(17, 36);
            this.cmbAnimationAttribute.Name = "cmbAnimationAttribute";
            this.cmbAnimationAttribute.Size = new System.Drawing.Size(222, 21);
            this.cmbAnimationAttribute.TabIndex = 11;
            // 
            // lblAnimation
            // 
            this.lblAnimation.AutoSize = true;
            this.lblAnimation.Location = new System.Drawing.Point(14, 16);
            this.lblAnimation.Name = "lblAnimation";
            this.lblAnimation.Size = new System.Drawing.Size(56, 13);
            this.lblAnimation.TabIndex = 10;
            this.lblAnimation.Text = "Animation:";
            // 
            // lblLightInstructions
            // 
            this.lblLightInstructions.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblLightInstructions.Location = new System.Drawing.Point(6, 9);
            this.lblLightInstructions.Name = "lblLightInstructions";
            this.lblLightInstructions.Size = new System.Drawing.Size(259, 38);
            this.lblLightInstructions.TabIndex = 1;
            this.lblLightInstructions.Text = "Lower the maps brightness and double click on a tile to create a light!";
            // 
            // lblEventInstructions
            // 
            this.lblEventInstructions.AutoSize = true;
            this.lblEventInstructions.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblEventInstructions.Location = new System.Drawing.Point(9, 9);
            this.lblEventInstructions.Name = "lblEventInstructions";
            this.lblEventInstructions.Size = new System.Drawing.Size(240, 13);
            this.lblEventInstructions.TabIndex = 0;
            this.lblEventInstructions.Text = "Double click a tile on the map to create an event!";
            // 
            // grpNpcList
            // 
            this.grpNpcList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpNpcList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpNpcList.Controls.Add(this.btnRemoveMapNpc);
            this.grpNpcList.Controls.Add(this.btnAddMapNpc);
            this.grpNpcList.Controls.Add(this.cmbNpc);
            this.grpNpcList.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpNpcList.Location = new System.Drawing.Point(7, 277);
            this.grpNpcList.Name = "grpNpcList";
            this.grpNpcList.Size = new System.Drawing.Size(259, 85);
            this.grpNpcList.TabIndex = 12;
            this.grpNpcList.TabStop = false;
            this.grpNpcList.Text = "Add/Remove Map NPCs";
            // 
            // btnRemoveMapNpc
            // 
            this.btnRemoveMapNpc.Location = new System.Drawing.Point(147, 47);
            this.btnRemoveMapNpc.Name = "btnRemoveMapNpc";
            this.btnRemoveMapNpc.Padding = new System.Windows.Forms.Padding(5);
            this.btnRemoveMapNpc.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveMapNpc.TabIndex = 6;
            this.btnRemoveMapNpc.Text = "Remove";
            this.btnRemoveMapNpc.Click += new System.EventHandler(this.btnRemoveMapNpc_Click);
            // 
            // btnAddMapNpc
            // 
            this.btnAddMapNpc.Location = new System.Drawing.Point(26, 47);
            this.btnAddMapNpc.Name = "btnAddMapNpc";
            this.btnAddMapNpc.Padding = new System.Windows.Forms.Padding(5);
            this.btnAddMapNpc.Size = new System.Drawing.Size(75, 23);
            this.btnAddMapNpc.TabIndex = 5;
            this.btnAddMapNpc.Text = "Add";
            this.btnAddMapNpc.Click += new System.EventHandler(this.btnAddMapNpc_Click);
            // 
            // cmbNpc
            // 
            this.cmbNpc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbNpc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbNpc.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbNpc.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbNpc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNpc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNpc.FormattingEnabled = true;
            this.cmbNpc.Location = new System.Drawing.Point(6, 18);
            this.cmbNpc.Name = "cmbNpc";
            this.cmbNpc.Size = new System.Drawing.Size(247, 21);
            this.cmbNpc.TabIndex = 4;
            this.cmbNpc.SelectedIndexChanged += new System.EventHandler(this.cmbNpc_SelectedIndexChanged);
            // 
            // grpSpawnLoc
            // 
            this.grpSpawnLoc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpSpawnLoc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSpawnLoc.Controls.Add(this.cmbDir);
            this.grpSpawnLoc.Controls.Add(this.lblDir);
            this.grpSpawnLoc.Controls.Add(this.rbRandom);
            this.grpSpawnLoc.Controls.Add(this.rbDeclared);
            this.grpSpawnLoc.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSpawnLoc.Location = new System.Drawing.Point(5, 183);
            this.grpSpawnLoc.Name = "grpSpawnLoc";
            this.grpSpawnLoc.Size = new System.Drawing.Size(259, 81);
            this.grpSpawnLoc.TabIndex = 11;
            this.grpSpawnLoc.TabStop = false;
            this.grpSpawnLoc.Text = "Spawn Location: Random";
            // 
            // cmbDir
            // 
            this.cmbDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.cmbDir.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDir.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDir.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDir.FormattingEnabled = true;
            this.cmbDir.Items.AddRange(new object[] {
            "Random",
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbDir.Location = new System.Drawing.Point(5, 54);
            this.cmbDir.Name = "cmbDir";
            this.cmbDir.Size = new System.Drawing.Size(248, 21);
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
            // 
            // lstMapNpcs
            // 
            this.lstMapNpcs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstMapNpcs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstMapNpcs.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstMapNpcs.FormattingEnabled = true;
            this.lstMapNpcs.Location = new System.Drawing.Point(5, 6);
            this.lstMapNpcs.Name = "lstMapNpcs";
            this.lstMapNpcs.Size = new System.Drawing.Size(259, 171);
            this.lstMapNpcs.TabIndex = 10;
            this.lstMapNpcs.Click += new System.EventHandler(this.lstMapNpcs_Click);
            this.lstMapNpcs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstMapNpcs_MouseDown);
            // 
            // btnTileHeader
            // 
            this.btnTileHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.btnTileHeader.Location = new System.Drawing.Point(8, 12);
            this.btnTileHeader.Name = "btnTileHeader";
            this.btnTileHeader.Padding = new System.Windows.Forms.Padding(5);
            this.btnTileHeader.Size = new System.Drawing.Size(44, 23);
            this.btnTileHeader.TabIndex = 18;
            this.btnTileHeader.Text = "Tiles";
            this.btnTileHeader.Click += new System.EventHandler(this.btnTileHeader_Click);
            // 
            // btnAttributeHeader
            // 
            this.btnAttributeHeader.Location = new System.Drawing.Point(52, 12);
            this.btnAttributeHeader.Name = "btnAttributeHeader";
            this.btnAttributeHeader.Padding = new System.Windows.Forms.Padding(5);
            this.btnAttributeHeader.Size = new System.Drawing.Size(64, 23);
            this.btnAttributeHeader.TabIndex = 19;
            this.btnAttributeHeader.Text = "Attributes";
            this.btnAttributeHeader.Click += new System.EventHandler(this.btnAttributeHeader_Click);
            // 
            // btnLightsHeader
            // 
            this.btnLightsHeader.Location = new System.Drawing.Point(115, 12);
            this.btnLightsHeader.Name = "btnLightsHeader";
            this.btnLightsHeader.Padding = new System.Windows.Forms.Padding(5);
            this.btnLightsHeader.Size = new System.Drawing.Size(49, 23);
            this.btnLightsHeader.TabIndex = 20;
            this.btnLightsHeader.Text = "Lights";
            this.btnLightsHeader.Click += new System.EventHandler(this.btnLightsHeader_Click);
            // 
            // btnEventsHeader
            // 
            this.btnEventsHeader.Location = new System.Drawing.Point(163, 12);
            this.btnEventsHeader.Name = "btnEventsHeader";
            this.btnEventsHeader.Padding = new System.Windows.Forms.Padding(5);
            this.btnEventsHeader.Size = new System.Drawing.Size(51, 23);
            this.btnEventsHeader.TabIndex = 21;
            this.btnEventsHeader.Text = "Events";
            this.btnEventsHeader.Click += new System.EventHandler(this.btnEventsHeader_Click);
            // 
            // btnNpcsHeader
            // 
            this.btnNpcsHeader.Location = new System.Drawing.Point(213, 12);
            this.btnNpcsHeader.Name = "btnNpcsHeader";
            this.btnNpcsHeader.Padding = new System.Windows.Forms.Padding(5);
            this.btnNpcsHeader.Size = new System.Drawing.Size(46, 23);
            this.btnNpcsHeader.TabIndex = 22;
            this.btnNpcsHeader.Text = "Npcs";
            this.btnNpcsHeader.Click += new System.EventHandler(this.btnNpcsHeader_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pnlTiles);
            this.panel1.Controls.Add(this.pnlEvents);
            this.panel1.Controls.Add(this.pnlLights);
            this.panel1.Controls.Add(this.pnlAttributes);
            this.panel1.Controls.Add(this.pnlNpcs);
            this.panel1.Location = new System.Drawing.Point(8, 34);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(278, 424);
            this.panel1.TabIndex = 23;
            // 
            // pnlTiles
            // 
            this.pnlTiles.Controls.Add(this.picFringe2);
            this.pnlTiles.Controls.Add(this.picFringe);
            this.pnlTiles.Controls.Add(this.picMask2);
            this.pnlTiles.Controls.Add(this.picMask);
            this.pnlTiles.Controls.Add(this.picGround);
            this.pnlTiles.Controls.Add(this.lblLayer);
            this.pnlTiles.Controls.Add(this.cmbTilesets);
            this.pnlTiles.Controls.Add(this.lblTileType);
            this.pnlTiles.Controls.Add(this.cmbAutotile);
            this.pnlTiles.Controls.Add(this.lblTileset);
            this.pnlTiles.Controls.Add(this.pnlTilesetContainer);
            this.pnlTiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTiles.Location = new System.Drawing.Point(0, 0);
            this.pnlTiles.Name = "pnlTiles";
            this.pnlTiles.Size = new System.Drawing.Size(276, 422);
            this.pnlTiles.TabIndex = 0;
            // 
            // picFringe2
            // 
            this.picFringe2.Location = new System.Drawing.Point(232, 2);
            this.picFringe2.Name = "picFringe2";
            this.picFringe2.Size = new System.Drawing.Size(30, 32);
            this.picFringe2.TabIndex = 28;
            this.picFringe2.TabStop = false;
            this.picFringe2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMapLayer_MouseClick);
            this.picFringe2.MouseHover += new System.EventHandler(this.picMapLayer_MouseHover);
            // 
            // picFringe
            // 
            this.picFringe.Location = new System.Drawing.Point(195, 2);
            this.picFringe.Name = "picFringe";
            this.picFringe.Size = new System.Drawing.Size(30, 32);
            this.picFringe.TabIndex = 27;
            this.picFringe.TabStop = false;
            this.picFringe.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMapLayer_MouseClick);
            this.picFringe.MouseHover += new System.EventHandler(this.picMapLayer_MouseHover);
            // 
            // picMask2
            // 
            this.picMask2.Location = new System.Drawing.Point(158, 2);
            this.picMask2.Name = "picMask2";
            this.picMask2.Size = new System.Drawing.Size(30, 32);
            this.picMask2.TabIndex = 26;
            this.picMask2.TabStop = false;
            this.picMask2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMapLayer_MouseClick);
            this.picMask2.MouseHover += new System.EventHandler(this.picMapLayer_MouseHover);
            // 
            // picMask
            // 
            this.picMask.Location = new System.Drawing.Point(121, 2);
            this.picMask.Name = "picMask";
            this.picMask.Size = new System.Drawing.Size(30, 32);
            this.picMask.TabIndex = 25;
            this.picMask.TabStop = false;
            this.picMask.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMapLayer_MouseClick);
            this.picMask.MouseHover += new System.EventHandler(this.picMapLayer_MouseHover);
            // 
            // picGround
            // 
            this.picGround.Location = new System.Drawing.Point(84, 2);
            this.picGround.Name = "picGround";
            this.picGround.Size = new System.Drawing.Size(30, 32);
            this.picGround.TabIndex = 24;
            this.picGround.TabStop = false;
            this.picGround.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMapLayer_MouseClick);
            this.picGround.MouseHover += new System.EventHandler(this.picMapLayer_MouseHover);
            // 
            // pnlTilesetContainer
            // 
            this.pnlTilesetContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTilesetContainer.AutoScroll = true;
            this.pnlTilesetContainer.BackColor = System.Drawing.SystemColors.Control;
            this.pnlTilesetContainer.Controls.Add(this.picTileset);
            this.pnlTilesetContainer.Location = new System.Drawing.Point(9, 96);
            this.pnlTilesetContainer.Name = "pnlTilesetContainer";
            this.pnlTilesetContainer.Size = new System.Drawing.Size(264, 318);
            this.pnlTilesetContainer.TabIndex = 19;
            // 
            // picTileset
            // 
            this.picTileset.Location = new System.Drawing.Point(0, 0);
            this.picTileset.Name = "picTileset";
            this.picTileset.Size = new System.Drawing.Size(167, 148);
            this.picTileset.TabIndex = 2;
            this.picTileset.TabStop = false;
            this.picTileset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picTileset_MouseDown);
            this.picTileset.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picTileset_MouseMove);
            this.picTileset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picTileset_MouseUp);
            // 
            // pnlEvents
            // 
            this.pnlEvents.Controls.Add(this.lblEventInstructions);
            this.pnlEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEvents.Location = new System.Drawing.Point(0, 0);
            this.pnlEvents.Name = "pnlEvents";
            this.pnlEvents.Size = new System.Drawing.Size(276, 422);
            this.pnlEvents.TabIndex = 1;
            // 
            // pnlLights
            // 
            this.pnlLights.Controls.Add(this.lightEditor);
            this.pnlLights.Controls.Add(this.lblLightInstructions);
            this.pnlLights.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLights.Location = new System.Drawing.Point(0, 0);
            this.pnlLights.Name = "pnlLights";
            this.pnlLights.Size = new System.Drawing.Size(276, 422);
            this.pnlLights.TabIndex = 1;
            // 
            // lightEditor
            // 
            this.lightEditor.ForeColor = System.Drawing.Color.Gainsboro;
            this.lightEditor.Location = new System.Drawing.Point(6, 6);
            this.lightEditor.Name = "lightEditor";
            this.lightEditor.Size = new System.Drawing.Size(256, 358);
            this.lightEditor.TabIndex = 2;
            this.lightEditor.Visible = false;
            this.lightEditor.Load += new System.EventHandler(this.lightEditor_Load);
            // 
            // pnlAttributes
            // 
            this.pnlAttributes.Controls.Add(this.rbSlide);
            this.pnlAttributes.Controls.Add(this.rbBlocked);
            this.pnlAttributes.Controls.Add(this.rbGrappleStone);
            this.pnlAttributes.Controls.Add(this.rbAnimation);
            this.pnlAttributes.Controls.Add(this.rbResource);
            this.pnlAttributes.Controls.Add(this.rbSound);
            this.pnlAttributes.Controls.Add(this.rbWarp);
            this.pnlAttributes.Controls.Add(this.rbNPCAvoid);
            this.pnlAttributes.Controls.Add(this.rbZDimension);
            this.pnlAttributes.Controls.Add(this.rbItem);
            this.pnlAttributes.Controls.Add(this.grpWarp);
            this.pnlAttributes.Controls.Add(this.grpItem);
            this.pnlAttributes.Controls.Add(this.grpResource);
            this.pnlAttributes.Controls.Add(this.grpAnimation);
            this.pnlAttributes.Controls.Add(this.grpSlide);
            this.pnlAttributes.Controls.Add(this.grpSound);
            this.pnlAttributes.Controls.Add(this.grpZDimension);
            this.pnlAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAttributes.Location = new System.Drawing.Point(0, 0);
            this.pnlAttributes.Name = "pnlAttributes";
            this.pnlAttributes.Size = new System.Drawing.Size(276, 422);
            this.pnlAttributes.TabIndex = 1;
            // 
            // pnlNpcs
            // 
            this.pnlNpcs.Controls.Add(this.grpNpcList);
            this.pnlNpcs.Controls.Add(this.lstMapNpcs);
            this.pnlNpcs.Controls.Add(this.grpSpawnLoc);
            this.pnlNpcs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNpcs.Location = new System.Drawing.Point(0, 0);
            this.pnlNpcs.Name = "pnlNpcs";
            this.pnlNpcs.Size = new System.Drawing.Size(276, 422);
            this.pnlNpcs.TabIndex = 1;
            // 
            // frmMapLayers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(312, 461);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.ControlBox = false;
            this.Controls.Add(this.btnAttributeHeader);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnNpcsHeader);
            this.Controls.Add(this.btnEventsHeader);
            this.Controls.Add(this.btnLightsHeader);
            this.Controls.Add(this.btnTileHeader);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HideOnClose = true;
            this.MinimumSize = new System.Drawing.Size(314, 250);
            this.Name = "frmMapLayers";
            this.Text = "Map Layers";
            this.DockStateChanged += new System.EventHandler(this.frmMapLayers_DockStateChanged);
            this.Load += new System.EventHandler(this.frmMapLayers_Load);
            this.grpResource.ResumeLayout(false);
            this.grpResource.PerformLayout();
            this.grpZResource.ResumeLayout(false);
            this.grpZResource.PerformLayout();
            this.grpItem.ResumeLayout(false);
            this.grpItem.PerformLayout();
            this.grpWarp.ResumeLayout(false);
            this.grpWarp.PerformLayout();
            this.grpZDimension.ResumeLayout(false);
            this.grpGateway.ResumeLayout(false);
            this.grpGateway.PerformLayout();
            this.grpDimBlock.ResumeLayout(false);
            this.grpDimBlock.PerformLayout();
            this.grpSound.ResumeLayout(false);
            this.grpSound.PerformLayout();
            this.grpSlide.ResumeLayout(false);
            this.grpSlide.PerformLayout();
            this.grpAnimation.ResumeLayout(false);
            this.grpAnimation.PerformLayout();
            this.grpNpcList.ResumeLayout(false);
            this.grpSpawnLoc.ResumeLayout(false);
            this.grpSpawnLoc.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.pnlTiles.ResumeLayout(false);
            this.pnlTiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFringe2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFringe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMask2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMask)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picGround)).EndInit();
            this.pnlTilesetContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTileset)).EndInit();
            this.pnlEvents.ResumeLayout(false);
            this.pnlEvents.PerformLayout();
            this.pnlLights.ResumeLayout(false);
            this.pnlAttributes.ResumeLayout(false);
            this.pnlAttributes.PerformLayout();
            this.pnlNpcs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblTileType;
        private System.Windows.Forms.Label lblTileset;
        private AutoDragPanel pnlTilesetContainer;
        public System.Windows.Forms.PictureBox picTileset;
        private DarkComboBox cmbAutotile;
        public DarkComboBox cmbTilesets;
        private DarkRadioButton rbResource;
        private DarkRadioButton rbSound;
        private DarkRadioButton rbWarp;
        private DarkRadioButton rbNPCAvoid;
        private DarkRadioButton rbZDimension;
        private DarkRadioButton rbItem;
        private DarkRadioButton rbBlocked;
        private DarkGroupBox grpResource;
        private DarkGroupBox grpSound;
        public DarkComboBox cmbMapAttributeSound;
        private System.Windows.Forms.Label lblSoundDistance;
        private System.Windows.Forms.Label lblMapSound;
        private DarkScrollBar scrlSoundDistance;
        private DarkGroupBox grpItem;
        private System.Windows.Forms.Label lblMaxItemAmount;
        private System.Windows.Forms.Label lblMapItem;
        private DarkScrollBar scrlMaxItemVal;
        private DarkGroupBox grpWarp;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblMap;
        public DarkScrollBar scrlX;
        public DarkScrollBar scrlY;
        private DarkGroupBox grpZDimension;
        private DarkGroupBox grpGateway;
        private DarkRadioButton rbGateway2;
        private DarkRadioButton rbGateway1;
        private DarkRadioButton rbGatewayNone;
        private DarkGroupBox grpDimBlock;
        private DarkRadioButton rbBlock2;
        private DarkRadioButton rbBlock1;
        private DarkRadioButton rbBlockNone;
        private System.Windows.Forms.Label lblLayer;
        private DarkGroupBox grpNpcList;
        private DarkButton btnRemoveMapNpc;
        private DarkButton btnAddMapNpc;
        private DarkComboBox cmbNpc;
        private DarkGroupBox grpSpawnLoc;
        private DarkComboBox cmbDir;
        private System.Windows.Forms.Label lblDir;
        public DarkRadioButton rbRandom;
        public DarkRadioButton rbDeclared;
        public System.Windows.Forms.ListBox lstMapNpcs;
        private System.Windows.Forms.Label lblLightInstructions;
        private System.Windows.Forms.Label lblEventInstructions;
        public Controls.LightEditorCtrl lightEditor;
        private DarkComboBox cmbResourceAttribute;
        private System.Windows.Forms.Label lblResource;
        private DarkComboBox cmbItemAttribute;
        private DarkComboBox cmbWarpMap;
        private DarkButton btnVisualMapSelector;
        private DarkComboBox cmbDirection;
        private System.Windows.Forms.Label lblWarpDir;
        private DarkGroupBox grpAnimation;
        private DarkComboBox cmbAnimationAttribute;
        private System.Windows.Forms.Label lblAnimation;
        private DarkRadioButton rbAnimation;
        private DarkRadioButton rbSlide;
        private DarkRadioButton rbGrappleStone;
        private DarkGroupBox grpSlide;
        private DarkComboBox cmbSlideDir;
        private System.Windows.Forms.Label lblSlideDir;
        private DarkGroupBox grpZResource;
        private DarkRadioButton rbLevel2;
        private DarkRadioButton rbLevel1;
        private DarkUI.Controls.DarkButton btnTileHeader;
        private DarkUI.Controls.DarkButton btnAttributeHeader;
        private DarkUI.Controls.DarkButton btnLightsHeader;
        private DarkUI.Controls.DarkButton btnEventsHeader;
        private DarkUI.Controls.DarkButton btnNpcsHeader;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlEvents;
        private System.Windows.Forms.Panel pnlLights;
        private System.Windows.Forms.Panel pnlNpcs;
        private System.Windows.Forms.Panel pnlAttributes;
        private System.Windows.Forms.Panel pnlTiles;
        private PictureBox picFringe2;
        private PictureBox picFringe;
        private PictureBox picMask2;
        private PictureBox picMask;
        private PictureBox picGround;
    }
}