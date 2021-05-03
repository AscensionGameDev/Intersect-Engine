using System.Windows.Forms;
using DarkUI.Controls;
using Intersect.Editor.Forms.Controls;

namespace Intersect.Editor.Forms.DockingElements
{
    partial class FrmMapLayers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMapLayers));
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
            this.nudItemQuantity = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbItemAttribute = new DarkUI.Controls.DarkComboBox();
            this.lblMaxItemAmount = new System.Windows.Forms.Label();
            this.lblMapItem = new System.Windows.Forms.Label();
            this.grpWarp = new DarkUI.Controls.DarkGroupBox();
            this.nudWarpY = new DarkUI.Controls.DarkNumericUpDown();
            this.nudWarpX = new DarkUI.Controls.DarkNumericUpDown();
            this.btnVisualMapSelector = new DarkUI.Controls.DarkButton();
            this.cmbWarpMap = new DarkUI.Controls.DarkComboBox();
            this.cmbDirection = new DarkUI.Controls.DarkComboBox();
            this.lblWarpDir = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblMap = new System.Windows.Forms.Label();
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
            this.nudSoundLoopInterval = new DarkUI.Controls.DarkNumericUpDown();
            this.lblSoundInterval = new System.Windows.Forms.Label();
            this.nudSoundDistance = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbMapAttributeSound = new DarkUI.Controls.DarkComboBox();
            this.lblSoundDistance = new System.Windows.Forms.Label();
            this.lblMapSound = new System.Windows.Forms.Label();
            this.grpSlide = new DarkUI.Controls.DarkGroupBox();
            this.cmbSlideDir = new DarkUI.Controls.DarkComboBox();
            this.lblSlideDir = new System.Windows.Forms.Label();
            this.grpAnimation = new DarkUI.Controls.DarkGroupBox();
            this.cmbAnimationAttribute = new DarkUI.Controls.DarkComboBox();
            this.lblAnimation = new System.Windows.Forms.Label();
            this.chkAnimationBlock = new DarkUI.Controls.DarkCheckBox();
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
            this.pnlAttributes = new System.Windows.Forms.Panel();
            this.grpCritter = new DarkUI.Controls.DarkGroupBox();
            this.cmbCritterSprite = new DarkUI.Controls.DarkComboBox();
            this.lblCritterSprite = new System.Windows.Forms.Label();
            this.cmbCritterAnimation = new DarkUI.Controls.DarkComboBox();
            this.lblCritterAnimation = new System.Windows.Forms.Label();
            this.rbCritter = new DarkUI.Controls.DarkRadioButton();
            this.pnlNpcs = new System.Windows.Forms.Panel();
            this.pnlTiles = new System.Windows.Forms.Panel();
            this.cmbMapLayer = new DarkUI.Controls.DarkComboBox();
            this.picLayer5 = new System.Windows.Forms.PictureBox();
            this.picLayer4 = new System.Windows.Forms.PictureBox();
            this.picLayer3 = new System.Windows.Forms.PictureBox();
            this.picLayer2 = new System.Windows.Forms.PictureBox();
            this.picLayer1 = new System.Windows.Forms.PictureBox();
            this.pnlTilesetContainer = new Intersect.Editor.Forms.Controls.AutoDragPanel();
            this.picTileset = new System.Windows.Forms.PictureBox();
            this.pnlEvents = new System.Windows.Forms.Panel();
            this.pnlLights = new System.Windows.Forms.Panel();
            this.lightEditor = new Intersect.Editor.Forms.Controls.LightEditorCtrl();
            this.cmbCritterMovement = new DarkUI.Controls.DarkComboBox();
            this.lblCritterMovement = new System.Windows.Forms.Label();
            this.nudCritterMoveSpeed = new DarkUI.Controls.DarkNumericUpDown();
            this.nudCritterMoveFrequency = new DarkUI.Controls.DarkNumericUpDown();
            this.lblCritterMoveSpeed = new System.Windows.Forms.Label();
            this.lblCritterMoveFrequency = new System.Windows.Forms.Label();
            this.cmbCritterLayer = new DarkUI.Controls.DarkComboBox();
            this.lblCritterLayer = new System.Windows.Forms.Label();
            this.chkCritterIgnoreNpcAvoids = new DarkUI.Controls.DarkCheckBox();
            this.chkCritterBlockPlayers = new DarkUI.Controls.DarkCheckBox();
            this.cmbCritterDirection = new DarkUI.Controls.DarkComboBox();
            this.lblCritterDirection = new System.Windows.Forms.Label();
            this.grpResource.SuspendLayout();
            this.grpZResource.SuspendLayout();
            this.grpItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemQuantity)).BeginInit();
            this.grpWarp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarpY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarpX)).BeginInit();
            this.grpZDimension.SuspendLayout();
            this.grpGateway.SuspendLayout();
            this.grpDimBlock.SuspendLayout();
            this.grpSound.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoundLoopInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoundDistance)).BeginInit();
            this.grpSlide.SuspendLayout();
            this.grpAnimation.SuspendLayout();
            this.grpNpcList.SuspendLayout();
            this.grpSpawnLoc.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlAttributes.SuspendLayout();
            this.grpCritter.SuspendLayout();
            this.pnlNpcs.SuspendLayout();
            this.pnlTiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLayer5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLayer4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLayer3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLayer2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLayer1)).BeginInit();
            this.pnlTilesetContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTileset)).BeginInit();
            this.pnlEvents.SuspendLayout();
            this.pnlLights.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCritterMoveSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCritterMoveFrequency)).BeginInit();
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
            this.cmbAutotile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAutotile.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAutotile.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAutotile.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbAutotile.DrawDropdownHoverOutline = false;
            this.cmbAutotile.DrawFocusRectangle = false;
            this.cmbAutotile.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAutotile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutotile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAutotile.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbAutotile.FormattingEnabled = true;
            this.cmbAutotile.Items.AddRange(new object[] {
            "Normal",
            "Autotile    [VX Format]",
            "Fake          [VX Format]",
            "Animated [VX Format]",
            "Cliff           [VX Format]",
            "Waterfall   [VX Format]",
            "Autotile     [XP Format]",
            "Animated  [XP Format]"});
            this.cmbAutotile.Location = new System.Drawing.Point(84, 65);
            this.cmbAutotile.Name = "cmbAutotile";
            this.cmbAutotile.Size = new System.Drawing.Size(178, 21);
            this.cmbAutotile.TabIndex = 18;
            this.cmbAutotile.Text = "Normal";
            this.cmbAutotile.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbAutotile.SelectedIndexChanged += new System.EventHandler(this.cmbAutotile_SelectedIndexChanged);
            // 
            // cmbTilesets
            // 
            this.cmbTilesets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTilesets.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTilesets.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTilesets.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbTilesets.DrawDropdownHoverOutline = false;
            this.cmbTilesets.DrawFocusRectangle = false;
            this.cmbTilesets.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTilesets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTilesets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTilesets.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTilesets.FormattingEnabled = true;
            this.cmbTilesets.Location = new System.Drawing.Point(84, 36);
            this.cmbTilesets.Name = "cmbTilesets";
            this.cmbTilesets.Size = new System.Drawing.Size(178, 21);
            this.cmbTilesets.TabIndex = 17;
            this.cmbTilesets.Text = null;
            this.cmbTilesets.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbTilesets.SelectedIndexChanged += new System.EventHandler(this.cmbTilesets_SelectedIndexChanged);
            this.cmbTilesets.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cmbTilesets_MouseDown);
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
            this.rbAnimation.Location = new System.Drawing.Point(6, 146);
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
            this.rbResource.Location = new System.Drawing.Point(6, 123);
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
            this.rbSound.Location = new System.Drawing.Point(6, 100);
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
            this.rbWarp.Location = new System.Drawing.Point(6, 53);
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
            this.rbNPCAvoid.Location = new System.Drawing.Point(6, 30);
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
            this.rbZDimension.Location = new System.Drawing.Point(115, 53);
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
            this.rbItem.Location = new System.Drawing.Point(6, 76);
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
            this.grpResource.Location = new System.Drawing.Point(6, 179);
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
            this.cmbResourceAttribute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbResourceAttribute.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbResourceAttribute.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbResourceAttribute.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbResourceAttribute.DrawDropdownHoverOutline = false;
            this.cmbResourceAttribute.DrawFocusRectangle = false;
            this.cmbResourceAttribute.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbResourceAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResourceAttribute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbResourceAttribute.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbResourceAttribute.FormattingEnabled = true;
            this.cmbResourceAttribute.Location = new System.Drawing.Point(17, 36);
            this.cmbResourceAttribute.Name = "cmbResourceAttribute";
            this.cmbResourceAttribute.Size = new System.Drawing.Size(222, 21);
            this.cmbResourceAttribute.TabIndex = 11;
            this.cmbResourceAttribute.Text = null;
            this.cmbResourceAttribute.TextPadding = new System.Windows.Forms.Padding(2);
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
            this.grpItem.Controls.Add(this.nudItemQuantity);
            this.grpItem.Controls.Add(this.cmbItemAttribute);
            this.grpItem.Controls.Add(this.lblMaxItemAmount);
            this.grpItem.Controls.Add(this.lblMapItem);
            this.grpItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpItem.Location = new System.Drawing.Point(6, 179);
            this.grpItem.Name = "grpItem";
            this.grpItem.Size = new System.Drawing.Size(246, 98);
            this.grpItem.TabIndex = 22;
            this.grpItem.TabStop = false;
            this.grpItem.Text = "Map Item";
            this.grpItem.Visible = false;
            // 
            // nudItemQuantity
            // 
            this.nudItemQuantity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudItemQuantity.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudItemQuantity.Location = new System.Drawing.Point(16, 72);
            this.nudItemQuantity.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudItemQuantity.Name = "nudItemQuantity";
            this.nudItemQuantity.Size = new System.Drawing.Size(219, 20);
            this.nudItemQuantity.TabIndex = 10;
            this.nudItemQuantity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudItemQuantity.ValueChanged += new System.EventHandler(this.NudItemQuantity_ValueChanged);
            // 
            // cmbItemAttribute
            // 
            this.cmbItemAttribute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbItemAttribute.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbItemAttribute.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbItemAttribute.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbItemAttribute.DrawDropdownHoverOutline = false;
            this.cmbItemAttribute.DrawFocusRectangle = false;
            this.cmbItemAttribute.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbItemAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItemAttribute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbItemAttribute.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbItemAttribute.FormattingEnabled = true;
            this.cmbItemAttribute.Location = new System.Drawing.Point(16, 32);
            this.cmbItemAttribute.Name = "cmbItemAttribute";
            this.cmbItemAttribute.Size = new System.Drawing.Size(219, 21);
            this.cmbItemAttribute.TabIndex = 9;
            this.cmbItemAttribute.Text = null;
            this.cmbItemAttribute.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblMaxItemAmount
            // 
            this.lblMaxItemAmount.AutoSize = true;
            this.lblMaxItemAmount.Location = new System.Drawing.Point(13, 54);
            this.lblMaxItemAmount.Name = "lblMaxItemAmount";
            this.lblMaxItemAmount.Size = new System.Drawing.Size(49, 13);
            this.lblMaxItemAmount.TabIndex = 8;
            this.lblMaxItemAmount.Text = "Quantity:";
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
            // grpWarp
            // 
            this.grpWarp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpWarp.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpWarp.Controls.Add(this.nudWarpY);
            this.grpWarp.Controls.Add(this.nudWarpX);
            this.grpWarp.Controls.Add(this.btnVisualMapSelector);
            this.grpWarp.Controls.Add(this.cmbWarpMap);
            this.grpWarp.Controls.Add(this.cmbDirection);
            this.grpWarp.Controls.Add(this.lblWarpDir);
            this.grpWarp.Controls.Add(this.lblY);
            this.grpWarp.Controls.Add(this.lblX);
            this.grpWarp.Controls.Add(this.lblMap);
            this.grpWarp.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpWarp.Location = new System.Drawing.Point(6, 179);
            this.grpWarp.Name = "grpWarp";
            this.grpWarp.Size = new System.Drawing.Size(255, 180);
            this.grpWarp.TabIndex = 26;
            this.grpWarp.TabStop = false;
            this.grpWarp.Text = "Warp";
            this.grpWarp.Visible = false;
            // 
            // nudWarpY
            // 
            this.nudWarpY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudWarpY.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudWarpY.Location = new System.Drawing.Point(49, 87);
            this.nudWarpY.Name = "nudWarpY";
            this.nudWarpY.Size = new System.Drawing.Size(190, 20);
            this.nudWarpY.TabIndex = 26;
            this.nudWarpY.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // nudWarpX
            // 
            this.nudWarpX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudWarpX.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudWarpX.Location = new System.Drawing.Point(49, 59);
            this.nudWarpX.Name = "nudWarpX";
            this.nudWarpX.Size = new System.Drawing.Size(190, 20);
            this.nudWarpX.TabIndex = 25;
            this.nudWarpX.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // btnVisualMapSelector
            // 
            this.btnVisualMapSelector.Location = new System.Drawing.Point(16, 147);
            this.btnVisualMapSelector.Name = "btnVisualMapSelector";
            this.btnVisualMapSelector.Padding = new System.Windows.Forms.Padding(5);
            this.btnVisualMapSelector.Size = new System.Drawing.Size(222, 23);
            this.btnVisualMapSelector.TabIndex = 24;
            this.btnVisualMapSelector.Text = "Open Visual Interface";
            this.btnVisualMapSelector.Click += new System.EventHandler(this.btnVisualMapSelector_Click);
            // 
            // cmbWarpMap
            // 
            this.cmbWarpMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbWarpMap.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbWarpMap.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbWarpMap.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbWarpMap.DrawDropdownHoverOutline = false;
            this.cmbWarpMap.DrawFocusRectangle = false;
            this.cmbWarpMap.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbWarpMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWarpMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbWarpMap.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbWarpMap.FormattingEnabled = true;
            this.cmbWarpMap.Location = new System.Drawing.Point(17, 30);
            this.cmbWarpMap.Name = "cmbWarpMap";
            this.cmbWarpMap.Size = new System.Drawing.Size(221, 21);
            this.cmbWarpMap.TabIndex = 12;
            this.cmbWarpMap.Text = null;
            this.cmbWarpMap.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbDirection
            // 
            this.cmbDirection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDirection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDirection.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDirection.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbDirection.DrawDropdownHoverOutline = false;
            this.cmbDirection.DrawFocusRectangle = false;
            this.cmbDirection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDirection.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbDirection.FormattingEnabled = true;
            this.cmbDirection.Items.AddRange(new object[] {
            "Retain Direction",
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbDirection.Location = new System.Drawing.Point(49, 118);
            this.cmbDirection.Name = "cmbDirection";
            this.cmbDirection.Size = new System.Drawing.Size(189, 21);
            this.cmbDirection.TabIndex = 23;
            this.cmbDirection.Text = "Retain Direction";
            this.cmbDirection.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblWarpDir
            // 
            this.lblWarpDir.AutoSize = true;
            this.lblWarpDir.Location = new System.Drawing.Point(13, 121);
            this.lblWarpDir.Name = "lblWarpDir";
            this.lblWarpDir.Size = new System.Drawing.Size(23, 13);
            this.lblWarpDir.TabIndex = 22;
            this.lblWarpDir.Text = "Dir:";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(14, 89);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(17, 13);
            this.lblY.TabIndex = 11;
            this.lblY.Text = "Y:";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(14, 61);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(17, 13);
            this.lblX.TabIndex = 10;
            this.lblX.Text = "X:";
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
            // grpZDimension
            // 
            this.grpZDimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpZDimension.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpZDimension.Controls.Add(this.grpGateway);
            this.grpZDimension.Controls.Add(this.grpDimBlock);
            this.grpZDimension.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpZDimension.Location = new System.Drawing.Point(6, 179);
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
            this.rbGatewayNone.Size = new System.Drawing.Size(75, 17);
            this.rbGatewayNone.TabIndex = 10;
            this.rbGatewayNone.TabStop = true;
            this.rbGatewayNone.Text = "Not Found";
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
            this.grpSound.Controls.Add(this.nudSoundLoopInterval);
            this.grpSound.Controls.Add(this.lblSoundInterval);
            this.grpSound.Controls.Add(this.nudSoundDistance);
            this.grpSound.Controls.Add(this.cmbMapAttributeSound);
            this.grpSound.Controls.Add(this.lblSoundDistance);
            this.grpSound.Controls.Add(this.lblMapSound);
            this.grpSound.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSound.Location = new System.Drawing.Point(6, 179);
            this.grpSound.Name = "grpSound";
            this.grpSound.Size = new System.Drawing.Size(252, 150);
            this.grpSound.TabIndex = 29;
            this.grpSound.TabStop = false;
            this.grpSound.Text = "Map Sound";
            this.grpSound.Visible = false;
            // 
            // nudSoundLoopInterval
            // 
            this.nudSoundLoopInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudSoundLoopInterval.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudSoundLoopInterval.Location = new System.Drawing.Point(16, 118);
            this.nudSoundLoopInterval.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.nudSoundLoopInterval.Name = "nudSoundLoopInterval";
            this.nudSoundLoopInterval.Size = new System.Drawing.Size(219, 20);
            this.nudSoundLoopInterval.TabIndex = 12;
            this.nudSoundLoopInterval.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // lblSoundInterval
            // 
            this.lblSoundInterval.AutoSize = true;
            this.lblSoundInterval.Location = new System.Drawing.Point(13, 100);
            this.lblSoundInterval.Name = "lblSoundInterval";
            this.lblSoundInterval.Size = new System.Drawing.Size(107, 13);
            this.lblSoundInterval.TabIndex = 11;
            this.lblSoundInterval.Text = "Loop Interval (In Ms):";
            // 
            // nudSoundDistance
            // 
            this.nudSoundDistance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudSoundDistance.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudSoundDistance.Location = new System.Drawing.Point(16, 72);
            this.nudSoundDistance.Name = "nudSoundDistance";
            this.nudSoundDistance.Size = new System.Drawing.Size(219, 20);
            this.nudSoundDistance.TabIndex = 10;
            this.nudSoundDistance.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // cmbMapAttributeSound
            // 
            this.cmbMapAttributeSound.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbMapAttributeSound.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMapAttributeSound.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMapAttributeSound.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbMapAttributeSound.DrawDropdownHoverOutline = false;
            this.cmbMapAttributeSound.DrawFocusRectangle = false;
            this.cmbMapAttributeSound.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMapAttributeSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapAttributeSound.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMapAttributeSound.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbMapAttributeSound.FormattingEnabled = true;
            this.cmbMapAttributeSound.Location = new System.Drawing.Point(16, 30);
            this.cmbMapAttributeSound.Name = "cmbMapAttributeSound";
            this.cmbMapAttributeSound.Size = new System.Drawing.Size(219, 21);
            this.cmbMapAttributeSound.TabIndex = 9;
            this.cmbMapAttributeSound.Text = null;
            this.cmbMapAttributeSound.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblSoundDistance
            // 
            this.lblSoundDistance.AutoSize = true;
            this.lblSoundDistance.Location = new System.Drawing.Point(13, 54);
            this.lblSoundDistance.Name = "lblSoundDistance";
            this.lblSoundDistance.Size = new System.Drawing.Size(95, 13);
            this.lblSoundDistance.TabIndex = 8;
            this.lblSoundDistance.Text = "Distance (In Tiles):";
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
            // grpSlide
            // 
            this.grpSlide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpSlide.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSlide.Controls.Add(this.cmbSlideDir);
            this.grpSlide.Controls.Add(this.lblSlideDir);
            this.grpSlide.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSlide.Location = new System.Drawing.Point(5, 177);
            this.grpSlide.Name = "grpSlide";
            this.grpSlide.Size = new System.Drawing.Size(259, 75);
            this.grpSlide.TabIndex = 36;
            this.grpSlide.TabStop = false;
            this.grpSlide.Text = "Slide";
            this.grpSlide.Visible = false;
            // 
            // cmbSlideDir
            // 
            this.cmbSlideDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSlideDir.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSlideDir.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSlideDir.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbSlideDir.DrawDropdownHoverOutline = false;
            this.cmbSlideDir.DrawFocusRectangle = false;
            this.cmbSlideDir.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSlideDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSlideDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSlideDir.ForeColor = System.Drawing.Color.Gainsboro;
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
            this.cmbSlideDir.Text = "Retain Direction";
            this.cmbSlideDir.TextPadding = new System.Windows.Forms.Padding(2);
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
            this.grpAnimation.Controls.Add(this.chkAnimationBlock);
            this.grpAnimation.Controls.Add(this.cmbAnimationAttribute);
            this.grpAnimation.Controls.Add(this.lblAnimation);
            this.grpAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpAnimation.Location = new System.Drawing.Point(6, 179);
            this.grpAnimation.Name = "grpAnimation";
            this.grpAnimation.Size = new System.Drawing.Size(256, 96);
            this.grpAnimation.TabIndex = 33;
            this.grpAnimation.TabStop = false;
            this.grpAnimation.Text = "Animaton";
            this.grpAnimation.Visible = false;
            // 
            // cmbAnimationAttribute
            // 
            this.cmbAnimationAttribute.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAnimationAttribute.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAnimationAttribute.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAnimationAttribute.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbAnimationAttribute.DrawDropdownHoverOutline = false;
            this.cmbAnimationAttribute.DrawFocusRectangle = false;
            this.cmbAnimationAttribute.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAnimationAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnimationAttribute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAnimationAttribute.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbAnimationAttribute.FormattingEnabled = true;
            this.cmbAnimationAttribute.Location = new System.Drawing.Point(17, 36);
            this.cmbAnimationAttribute.Name = "cmbAnimationAttribute";
            this.cmbAnimationAttribute.Size = new System.Drawing.Size(222, 21);
            this.cmbAnimationAttribute.TabIndex = 11;
            this.cmbAnimationAttribute.Text = null;
            this.cmbAnimationAttribute.TextPadding = new System.Windows.Forms.Padding(2);
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
            // chkAnimationBlock
            // 
            this.chkAnimationBlock.AutoSize = true;
            this.chkAnimationBlock.Location = new System.Drawing.Point(16, 66);
            this.chkAnimationBlock.Name = "chkAnimationBlock";
            this.chkAnimationBlock.Size = new System.Drawing.Size(73, 17);
            this.chkAnimationBlock.TabIndex = 27;
            this.chkAnimationBlock.Text = "Block Tile";
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
            this.cmbNpc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbNpc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbNpc.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbNpc.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbNpc.DrawDropdownHoverOutline = false;
            this.cmbNpc.DrawFocusRectangle = false;
            this.cmbNpc.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbNpc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNpc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNpc.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbNpc.FormattingEnabled = true;
            this.cmbNpc.Location = new System.Drawing.Point(6, 18);
            this.cmbNpc.Name = "cmbNpc";
            this.cmbNpc.Size = new System.Drawing.Size(247, 21);
            this.cmbNpc.TabIndex = 4;
            this.cmbNpc.Text = null;
            this.cmbNpc.TextPadding = new System.Windows.Forms.Padding(2);
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
            this.cmbDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDir.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDir.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDir.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbDir.DrawDropdownHoverOutline = false;
            this.cmbDir.DrawFocusRectangle = false;
            this.cmbDir.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDir.ForeColor = System.Drawing.Color.Gainsboro;
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
            this.cmbDir.Text = "Random";
            this.cmbDir.TextPadding = new System.Windows.Forms.Padding(2);
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
            this.panel1.Controls.Add(this.pnlAttributes);
            this.panel1.Controls.Add(this.pnlNpcs);
            this.panel1.Controls.Add(this.pnlTiles);
            this.panel1.Controls.Add(this.pnlEvents);
            this.panel1.Controls.Add(this.pnlLights);
            this.panel1.Location = new System.Drawing.Point(8, 34);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(278, 444);
            this.panel1.TabIndex = 23;
            // 
            // pnlAttributes
            // 
            this.pnlAttributes.Controls.Add(this.grpCritter);
            this.pnlAttributes.Controls.Add(this.rbCritter);
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
            this.pnlAttributes.Controls.Add(this.grpAnimation);
            this.pnlAttributes.Controls.Add(this.grpSlide);
            this.pnlAttributes.Controls.Add(this.grpSound);
            this.pnlAttributes.Controls.Add(this.grpZDimension);
            this.pnlAttributes.Controls.Add(this.grpWarp);
            this.pnlAttributes.Controls.Add(this.grpItem);
            this.pnlAttributes.Controls.Add(this.grpResource);
            this.pnlAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAttributes.Location = new System.Drawing.Point(0, 0);
            this.pnlAttributes.Name = "pnlAttributes";
            this.pnlAttributes.Size = new System.Drawing.Size(276, 442);
            this.pnlAttributes.TabIndex = 1;
            // 
            // grpCritter
            // 
            this.grpCritter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpCritter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpCritter.Controls.Add(this.cmbCritterDirection);
            this.grpCritter.Controls.Add(this.lblCritterDirection);
            this.grpCritter.Controls.Add(this.chkCritterBlockPlayers);
            this.grpCritter.Controls.Add(this.chkCritterIgnoreNpcAvoids);
            this.grpCritter.Controls.Add(this.cmbCritterLayer);
            this.grpCritter.Controls.Add(this.lblCritterLayer);
            this.grpCritter.Controls.Add(this.lblCritterMoveFrequency);
            this.grpCritter.Controls.Add(this.lblCritterMoveSpeed);
            this.grpCritter.Controls.Add(this.nudCritterMoveFrequency);
            this.grpCritter.Controls.Add(this.nudCritterMoveSpeed);
            this.grpCritter.Controls.Add(this.cmbCritterMovement);
            this.grpCritter.Controls.Add(this.lblCritterMovement);
            this.grpCritter.Controls.Add(this.cmbCritterSprite);
            this.grpCritter.Controls.Add(this.lblCritterSprite);
            this.grpCritter.Controls.Add(this.cmbCritterAnimation);
            this.grpCritter.Controls.Add(this.lblCritterAnimation);
            this.grpCritter.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpCritter.Location = new System.Drawing.Point(6, 179);
            this.grpCritter.Name = "grpCritter";
            this.grpCritter.Size = new System.Drawing.Size(256, 260);
            this.grpCritter.TabIndex = 38;
            this.grpCritter.TabStop = false;
            this.grpCritter.Text = "Critter";
            this.grpCritter.Visible = false;
            // 
            // cmbCritterSprite
            // 
            this.cmbCritterSprite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbCritterSprite.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbCritterSprite.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbCritterSprite.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbCritterSprite.DrawDropdownHoverOutline = false;
            this.cmbCritterSprite.DrawFocusRectangle = false;
            this.cmbCritterSprite.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCritterSprite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCritterSprite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCritterSprite.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbCritterSprite.FormattingEnabled = true;
            this.cmbCritterSprite.Location = new System.Drawing.Point(82, 24);
            this.cmbCritterSprite.Name = "cmbCritterSprite";
            this.cmbCritterSprite.Size = new System.Drawing.Size(161, 21);
            this.cmbCritterSprite.TabIndex = 13;
            this.cmbCritterSprite.Text = null;
            this.cmbCritterSprite.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblCritterSprite
            // 
            this.lblCritterSprite.AutoSize = true;
            this.lblCritterSprite.Location = new System.Drawing.Point(13, 27);
            this.lblCritterSprite.Name = "lblCritterSprite";
            this.lblCritterSprite.Size = new System.Drawing.Size(37, 13);
            this.lblCritterSprite.TabIndex = 12;
            this.lblCritterSprite.Text = "Sprite:";
            // 
            // cmbCritterAnimation
            // 
            this.cmbCritterAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbCritterAnimation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbCritterAnimation.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbCritterAnimation.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbCritterAnimation.DrawDropdownHoverOutline = false;
            this.cmbCritterAnimation.DrawFocusRectangle = false;
            this.cmbCritterAnimation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCritterAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCritterAnimation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCritterAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbCritterAnimation.FormattingEnabled = true;
            this.cmbCritterAnimation.Location = new System.Drawing.Point(82, 51);
            this.cmbCritterAnimation.Name = "cmbCritterAnimation";
            this.cmbCritterAnimation.Size = new System.Drawing.Size(161, 21);
            this.cmbCritterAnimation.TabIndex = 11;
            this.cmbCritterAnimation.Text = null;
            this.cmbCritterAnimation.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblCritterAnimation
            // 
            this.lblCritterAnimation.AutoSize = true;
            this.lblCritterAnimation.Location = new System.Drawing.Point(13, 54);
            this.lblCritterAnimation.Name = "lblCritterAnimation";
            this.lblCritterAnimation.Size = new System.Drawing.Size(56, 13);
            this.lblCritterAnimation.TabIndex = 10;
            this.lblCritterAnimation.Text = "Animation:";
            // 
            // rbCritter
            // 
            this.rbCritter.AutoSize = true;
            this.rbCritter.ForeColor = System.Drawing.Color.Gainsboro;
            this.rbCritter.Location = new System.Drawing.Point(115, 73);
            this.rbCritter.Name = "rbCritter";
            this.rbCritter.Size = new System.Drawing.Size(52, 17);
            this.rbCritter.TabIndex = 37;
            this.rbCritter.Text = "Critter";
            this.rbCritter.CheckedChanged += new System.EventHandler(this.rbCritter_CheckedChanged);
            // 
            // pnlNpcs
            // 
            this.pnlNpcs.Controls.Add(this.grpNpcList);
            this.pnlNpcs.Controls.Add(this.lstMapNpcs);
            this.pnlNpcs.Controls.Add(this.grpSpawnLoc);
            this.pnlNpcs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNpcs.Location = new System.Drawing.Point(0, 0);
            this.pnlNpcs.Name = "pnlNpcs";
            this.pnlNpcs.Size = new System.Drawing.Size(276, 442);
            this.pnlNpcs.TabIndex = 1;
            // 
            // pnlTiles
            // 
            this.pnlTiles.Controls.Add(this.cmbMapLayer);
            this.pnlTiles.Controls.Add(this.picLayer5);
            this.pnlTiles.Controls.Add(this.picLayer4);
            this.pnlTiles.Controls.Add(this.picLayer3);
            this.pnlTiles.Controls.Add(this.picLayer2);
            this.pnlTiles.Controls.Add(this.picLayer1);
            this.pnlTiles.Controls.Add(this.lblLayer);
            this.pnlTiles.Controls.Add(this.cmbTilesets);
            this.pnlTiles.Controls.Add(this.lblTileType);
            this.pnlTiles.Controls.Add(this.cmbAutotile);
            this.pnlTiles.Controls.Add(this.lblTileset);
            this.pnlTiles.Controls.Add(this.pnlTilesetContainer);
            this.pnlTiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTiles.Location = new System.Drawing.Point(0, 0);
            this.pnlTiles.Name = "pnlTiles";
            this.pnlTiles.Size = new System.Drawing.Size(276, 442);
            this.pnlTiles.TabIndex = 0;
            // 
            // cmbMapLayer
            // 
            this.cmbMapLayer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbMapLayer.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMapLayer.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMapLayer.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbMapLayer.DrawDropdownHoverOutline = false;
            this.cmbMapLayer.DrawFocusRectangle = false;
            this.cmbMapLayer.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMapLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapLayer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMapLayer.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbMapLayer.FormattingEnabled = true;
            this.cmbMapLayer.Location = new System.Drawing.Point(84, 7);
            this.cmbMapLayer.Name = "cmbMapLayer";
            this.cmbMapLayer.Size = new System.Drawing.Size(178, 21);
            this.cmbMapLayer.TabIndex = 29;
            this.cmbMapLayer.Text = null;
            this.cmbMapLayer.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbMapLayer.SelectedIndexChanged += new System.EventHandler(this.cmbMapLayer_SelectedIndexChanged);
            // 
            // picLayer5
            // 
            this.picLayer5.Location = new System.Drawing.Point(232, 2);
            this.picLayer5.Name = "picLayer5";
            this.picLayer5.Size = new System.Drawing.Size(30, 32);
            this.picLayer5.TabIndex = 28;
            this.picLayer5.TabStop = false;
            this.picLayer5.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMapLayer_MouseClick);
            this.picLayer5.MouseHover += new System.EventHandler(this.picMapLayer_MouseHover);
            // 
            // picLayer4
            // 
            this.picLayer4.Location = new System.Drawing.Point(195, 2);
            this.picLayer4.Name = "picLayer4";
            this.picLayer4.Size = new System.Drawing.Size(30, 32);
            this.picLayer4.TabIndex = 27;
            this.picLayer4.TabStop = false;
            this.picLayer4.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMapLayer_MouseClick);
            this.picLayer4.MouseHover += new System.EventHandler(this.picMapLayer_MouseHover);
            // 
            // picLayer3
            // 
            this.picLayer3.Location = new System.Drawing.Point(158, 2);
            this.picLayer3.Name = "picLayer3";
            this.picLayer3.Size = new System.Drawing.Size(30, 32);
            this.picLayer3.TabIndex = 26;
            this.picLayer3.TabStop = false;
            this.picLayer3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMapLayer_MouseClick);
            this.picLayer3.MouseHover += new System.EventHandler(this.picMapLayer_MouseHover);
            // 
            // picLayer2
            // 
            this.picLayer2.Location = new System.Drawing.Point(121, 2);
            this.picLayer2.Name = "picLayer2";
            this.picLayer2.Size = new System.Drawing.Size(30, 32);
            this.picLayer2.TabIndex = 25;
            this.picLayer2.TabStop = false;
            this.picLayer2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMapLayer_MouseClick);
            this.picLayer2.MouseHover += new System.EventHandler(this.picMapLayer_MouseHover);
            // 
            // picLayer1
            // 
            this.picLayer1.Location = new System.Drawing.Point(84, 2);
            this.picLayer1.Name = "picLayer1";
            this.picLayer1.Size = new System.Drawing.Size(30, 32);
            this.picLayer1.TabIndex = 24;
            this.picLayer1.TabStop = false;
            this.picLayer1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMapLayer_MouseClick);
            this.picLayer1.MouseHover += new System.EventHandler(this.picMapLayer_MouseHover);
            // 
            // pnlTilesetContainer
            // 
            this.pnlTilesetContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlTilesetContainer.AutoScroll = true;
            this.pnlTilesetContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnlTilesetContainer.Controls.Add(this.picTileset);
            this.pnlTilesetContainer.Location = new System.Drawing.Point(9, 96);
            this.pnlTilesetContainer.Name = "pnlTilesetContainer";
            this.pnlTilesetContainer.Size = new System.Drawing.Size(264, 338);
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
            this.pnlEvents.Size = new System.Drawing.Size(276, 442);
            this.pnlEvents.TabIndex = 1;
            // 
            // pnlLights
            // 
            this.pnlLights.Controls.Add(this.lightEditor);
            this.pnlLights.Controls.Add(this.lblLightInstructions);
            this.pnlLights.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLights.Location = new System.Drawing.Point(0, 0);
            this.pnlLights.Name = "pnlLights";
            this.pnlLights.Size = new System.Drawing.Size(276, 442);
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
            // cmbCritterMovement
            // 
            this.cmbCritterMovement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbCritterMovement.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbCritterMovement.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbCritterMovement.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbCritterMovement.DrawDropdownHoverOutline = false;
            this.cmbCritterMovement.DrawFocusRectangle = false;
            this.cmbCritterMovement.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCritterMovement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCritterMovement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCritterMovement.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbCritterMovement.FormattingEnabled = true;
            this.cmbCritterMovement.Location = new System.Drawing.Point(82, 78);
            this.cmbCritterMovement.Name = "cmbCritterMovement";
            this.cmbCritterMovement.Size = new System.Drawing.Size(161, 21);
            this.cmbCritterMovement.TabIndex = 15;
            this.cmbCritterMovement.Text = null;
            this.cmbCritterMovement.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblCritterMovement
            // 
            this.lblCritterMovement.AutoSize = true;
            this.lblCritterMovement.Location = new System.Drawing.Point(13, 81);
            this.lblCritterMovement.Name = "lblCritterMovement";
            this.lblCritterMovement.Size = new System.Drawing.Size(60, 13);
            this.lblCritterMovement.TabIndex = 14;
            this.lblCritterMovement.Text = "Movement:";
            // 
            // nudCritterMoveSpeed
            // 
            this.nudCritterMoveSpeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudCritterMoveSpeed.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudCritterMoveSpeed.Location = new System.Drawing.Point(82, 159);
            this.nudCritterMoveSpeed.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudCritterMoveSpeed.Name = "nudCritterMoveSpeed";
            this.nudCritterMoveSpeed.Size = new System.Drawing.Size(161, 20);
            this.nudCritterMoveSpeed.TabIndex = 16;
            this.nudCritterMoveSpeed.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            // 
            // nudCritterMoveFrequency
            // 
            this.nudCritterMoveFrequency.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudCritterMoveFrequency.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudCritterMoveFrequency.Location = new System.Drawing.Point(82, 185);
            this.nudCritterMoveFrequency.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudCritterMoveFrequency.Name = "nudCritterMoveFrequency";
            this.nudCritterMoveFrequency.Size = new System.Drawing.Size(161, 20);
            this.nudCritterMoveFrequency.TabIndex = 17;
            this.nudCritterMoveFrequency.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblCritterMoveSpeed
            // 
            this.lblCritterMoveSpeed.AutoSize = true;
            this.lblCritterMoveSpeed.Location = new System.Drawing.Point(13, 161);
            this.lblCritterMoveSpeed.Name = "lblCritterMoveSpeed";
            this.lblCritterMoveSpeed.Size = new System.Drawing.Size(63, 13);
            this.lblCritterMoveSpeed.TabIndex = 18;
            this.lblCritterMoveSpeed.Text = "Speed (ms):";
            // 
            // lblCritterMoveFrequency
            // 
            this.lblCritterMoveFrequency.AutoSize = true;
            this.lblCritterMoveFrequency.Location = new System.Drawing.Point(13, 187);
            this.lblCritterMoveFrequency.Name = "lblCritterMoveFrequency";
            this.lblCritterMoveFrequency.Size = new System.Drawing.Size(53, 13);
            this.lblCritterMoveFrequency.TabIndex = 19;
            this.lblCritterMoveFrequency.Text = "Freq (ms):";
            // 
            // cmbCritterLayer
            // 
            this.cmbCritterLayer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbCritterLayer.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbCritterLayer.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbCritterLayer.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbCritterLayer.DrawDropdownHoverOutline = false;
            this.cmbCritterLayer.DrawFocusRectangle = false;
            this.cmbCritterLayer.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCritterLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCritterLayer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCritterLayer.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbCritterLayer.FormattingEnabled = true;
            this.cmbCritterLayer.Location = new System.Drawing.Point(82, 105);
            this.cmbCritterLayer.Name = "cmbCritterLayer";
            this.cmbCritterLayer.Size = new System.Drawing.Size(161, 21);
            this.cmbCritterLayer.TabIndex = 21;
            this.cmbCritterLayer.Text = null;
            this.cmbCritterLayer.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblCritterLayer
            // 
            this.lblCritterLayer.AutoSize = true;
            this.lblCritterLayer.Location = new System.Drawing.Point(13, 108);
            this.lblCritterLayer.Name = "lblCritterLayer";
            this.lblCritterLayer.Size = new System.Drawing.Size(36, 13);
            this.lblCritterLayer.TabIndex = 20;
            this.lblCritterLayer.Text = "Layer:";
            // 
            // chkCritterIgnoreNpcAvoids
            // 
            this.chkCritterIgnoreNpcAvoids.Location = new System.Drawing.Point(82, 212);
            this.chkCritterIgnoreNpcAvoids.Name = "chkCritterIgnoreNpcAvoids";
            this.chkCritterIgnoreNpcAvoids.Size = new System.Drawing.Size(162, 21);
            this.chkCritterIgnoreNpcAvoids.TabIndex = 22;
            this.chkCritterIgnoreNpcAvoids.Text = "Ignore Npc Avoids";
            // 
            // chkCritterBlockPlayers
            // 
            this.chkCritterBlockPlayers.Location = new System.Drawing.Point(82, 233);
            this.chkCritterBlockPlayers.Name = "chkCritterBlockPlayers";
            this.chkCritterBlockPlayers.Size = new System.Drawing.Size(162, 21);
            this.chkCritterBlockPlayers.TabIndex = 23;
            this.chkCritterBlockPlayers.Text = "Block Players";
            // 
            // cmbCritterDirection
            // 
            this.cmbCritterDirection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbCritterDirection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbCritterDirection.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbCritterDirection.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbCritterDirection.DrawDropdownHoverOutline = false;
            this.cmbCritterDirection.DrawFocusRectangle = false;
            this.cmbCritterDirection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCritterDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCritterDirection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCritterDirection.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbCritterDirection.FormattingEnabled = true;
            this.cmbCritterDirection.Location = new System.Drawing.Point(82, 132);
            this.cmbCritterDirection.Name = "cmbCritterDirection";
            this.cmbCritterDirection.Size = new System.Drawing.Size(161, 21);
            this.cmbCritterDirection.TabIndex = 25;
            this.cmbCritterDirection.Text = null;
            this.cmbCritterDirection.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblCritterDirection
            // 
            this.lblCritterDirection.AutoSize = true;
            this.lblCritterDirection.Location = new System.Drawing.Point(13, 135);
            this.lblCritterDirection.Name = "lblCritterDirection";
            this.lblCritterDirection.Size = new System.Drawing.Size(52, 13);
            this.lblCritterDirection.TabIndex = 24;
            this.lblCritterDirection.Text = "Direction:";
            // 
            // chkAnimationBlock
            // 
            this.chkAnimationBlock.AutoSize = true;
            this.chkAnimationBlock.Location = new System.Drawing.Point(16, 66);
            this.chkAnimationBlock.Name = "chkAnimationBlock";
            this.chkAnimationBlock.Size = new System.Drawing.Size(73, 17);
            this.chkAnimationBlock.TabIndex = 27;
            this.chkAnimationBlock.Text = "Block Tile";
            // 
            // FrmMapLayers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(312, 481);
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
            this.Name = "FrmMapLayers";
            this.Text = "Map Layers";
            this.DockStateChanged += new System.EventHandler(this.frmMapLayers_DockStateChanged);
            this.Load += new System.EventHandler(this.frmMapLayers_Load);
            this.grpResource.ResumeLayout(false);
            this.grpResource.PerformLayout();
            this.grpZResource.ResumeLayout(false);
            this.grpZResource.PerformLayout();
            this.grpItem.ResumeLayout(false);
            this.grpItem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudItemQuantity)).EndInit();
            this.grpWarp.ResumeLayout(false);
            this.grpWarp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarpY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarpX)).EndInit();
            this.grpZDimension.ResumeLayout(false);
            this.grpGateway.ResumeLayout(false);
            this.grpGateway.PerformLayout();
            this.grpDimBlock.ResumeLayout(false);
            this.grpDimBlock.PerformLayout();
            this.grpSound.ResumeLayout(false);
            this.grpSound.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoundLoopInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSoundDistance)).EndInit();
            this.grpSlide.ResumeLayout(false);
            this.grpSlide.PerformLayout();
            this.grpAnimation.ResumeLayout(false);
            this.grpAnimation.PerformLayout();
            this.grpNpcList.ResumeLayout(false);
            this.grpSpawnLoc.ResumeLayout(false);
            this.grpSpawnLoc.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.pnlAttributes.ResumeLayout(false);
            this.pnlAttributes.PerformLayout();
            this.grpCritter.ResumeLayout(false);
            this.grpCritter.PerformLayout();
            this.pnlNpcs.ResumeLayout(false);
            this.pnlTiles.ResumeLayout(false);
            this.pnlTiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLayer5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLayer4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLayer3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLayer2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLayer1)).EndInit();
            this.pnlTilesetContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTileset)).EndInit();
            this.pnlEvents.ResumeLayout(false);
            this.pnlEvents.PerformLayout();
            this.pnlLights.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudCritterMoveSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCritterMoveFrequency)).EndInit();
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
        private DarkGroupBox grpItem;
        private System.Windows.Forms.Label lblMaxItemAmount;
        private System.Windows.Forms.Label lblMapItem;
        private DarkGroupBox grpWarp;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblMap;
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
        private PictureBox picLayer5;
        private PictureBox picLayer4;
        private PictureBox picLayer3;
        private PictureBox picLayer2;
        private PictureBox picLayer1;
        private DarkNumericUpDown nudWarpY;
        private DarkNumericUpDown nudWarpX;
        private DarkNumericUpDown nudItemQuantity;
        private DarkNumericUpDown nudSoundDistance;
        private DarkCheckBox chkAnimationBlock;
        public DarkComboBox cmbMapLayer;
        private DarkNumericUpDown nudSoundLoopInterval;
        private Label lblSoundInterval;
        private DarkRadioButton rbCritter;
        private DarkGroupBox grpCritter;
        private DarkComboBox cmbCritterSprite;
        private Label lblCritterSprite;
        private DarkComboBox cmbCritterAnimation;
        private Label lblCritterAnimation;
        private DarkComboBox cmbCritterMovement;
        private Label lblCritterMovement;
        private Label lblCritterMoveFrequency;
        private Label lblCritterMoveSpeed;
        private DarkNumericUpDown nudCritterMoveFrequency;
        private DarkNumericUpDown nudCritterMoveSpeed;
        private DarkComboBox cmbCritterLayer;
        private Label lblCritterLayer;
        private DarkCheckBox chkCritterBlockPlayers;
        private DarkCheckBox chkCritterIgnoreNpcAvoids;
        private DarkComboBox cmbCritterDirection;
        private Label lblCritterDirection;
    }
}