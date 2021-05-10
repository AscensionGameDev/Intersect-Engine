using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
{
    partial class FrmAnimation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAnimation));
            this.grpAnimations = new DarkUI.Controls.DarkGroupBox();
            this.btnClearSearch = new DarkUI.Controls.DarkButton();
            this.txtSearch = new DarkUI.Controls.DarkTextBox();
            this.lstGameObjects = new Intersect.Editor.Forms.Controls.GameObjectList();
            this.grpGeneral = new DarkUI.Controls.DarkGroupBox();
            this.chkCompleteSoundPlayback = new DarkUI.Controls.DarkCheckBox();
            this.btnAddFolder = new DarkUI.Controls.DarkButton();
            this.lblFolder = new System.Windows.Forms.Label();
            this.cmbFolder = new DarkUI.Controls.DarkComboBox();
            this.btnSwap = new DarkUI.Controls.DarkButton();
            this.scrlDarkness = new DarkUI.Controls.DarkScrollBar();
            this.labelDarkness = new System.Windows.Forms.Label();
            this.lblSound = new System.Windows.Forms.Label();
            this.cmbSound = new DarkUI.Controls.DarkComboBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new DarkUI.Controls.DarkTextBox();
            this.grpLower = new DarkUI.Controls.DarkGroupBox();
            this.chkRenderAbovePlayer = new DarkUI.Controls.DarkCheckBox();
            this.chkDisableLowerRotations = new DarkUI.Controls.DarkCheckBox();
            this.nudLowerLoopCount = new DarkUI.Controls.DarkNumericUpDown();
            this.nudLowerFrameDuration = new DarkUI.Controls.DarkNumericUpDown();
            this.nudLowerFrameCount = new DarkUI.Controls.DarkNumericUpDown();
            this.nudLowerVerticalFrames = new DarkUI.Controls.DarkNumericUpDown();
            this.nudLowerHorizontalFrames = new DarkUI.Controls.DarkNumericUpDown();
            this.grpLowerFrameOpts = new DarkUI.Controls.DarkGroupBox();
            this.btnLowerClone = new DarkUI.Controls.DarkButton();
            this.lightEditorLower = new Intersect.Editor.Forms.Controls.LightEditorCtrl();
            this.grpLowerPlayback = new DarkUI.Controls.DarkGroupBox();
            this.btnPlayLower = new DarkUI.Controls.DarkButton();
            this.scrlLowerFrame = new DarkUI.Controls.DarkScrollBar();
            this.lblLowerFrame = new System.Windows.Forms.Label();
            this.lblLowerLoopCount = new System.Windows.Forms.Label();
            this.lblLowerFrameDuration = new System.Windows.Forms.Label();
            this.lblLowerFrameCount = new System.Windows.Forms.Label();
            this.lblLowerVerticalFrames = new System.Windows.Forms.Label();
            this.lblLowerHorizontalFrames = new System.Windows.Forms.Label();
            this.cmbLowerGraphic = new DarkUI.Controls.DarkComboBox();
            this.lblLowerGraphic = new System.Windows.Forms.Label();
            this.picLowerAnimation = new System.Windows.Forms.PictureBox();
            this.grpUpper = new DarkUI.Controls.DarkGroupBox();
            this.chkRenderBelowFringe = new DarkUI.Controls.DarkCheckBox();
            this.chkDisableUpperRotations = new DarkUI.Controls.DarkCheckBox();
            this.nudUpperLoopCount = new DarkUI.Controls.DarkNumericUpDown();
            this.nudUpperFrameDuration = new DarkUI.Controls.DarkNumericUpDown();
            this.nudUpperFrameCount = new DarkUI.Controls.DarkNumericUpDown();
            this.nudUpperVerticalFrames = new DarkUI.Controls.DarkNumericUpDown();
            this.nudUpperHorizontalFrames = new DarkUI.Controls.DarkNumericUpDown();
            this.grpUpperPlayback = new DarkUI.Controls.DarkGroupBox();
            this.btnPlayUpper = new DarkUI.Controls.DarkButton();
            this.scrlUpperFrame = new DarkUI.Controls.DarkScrollBar();
            this.lblUpperFrame = new System.Windows.Forms.Label();
            this.grpUpperFrameOpts = new DarkUI.Controls.DarkGroupBox();
            this.btnUpperClone = new DarkUI.Controls.DarkButton();
            this.lightEditorUpper = new Intersect.Editor.Forms.Controls.LightEditorCtrl();
            this.lblUpperLoopCount = new System.Windows.Forms.Label();
            this.lblUpperFrameDuration = new System.Windows.Forms.Label();
            this.lblUpperFrameCount = new System.Windows.Forms.Label();
            this.lblUpperVerticalFrames = new System.Windows.Forms.Label();
            this.lblUpperHorizontalFrames = new System.Windows.Forms.Label();
            this.cmbUpperGraphic = new DarkUI.Controls.DarkComboBox();
            this.lblUpperGraphic = new System.Windows.Forms.Label();
            this.picUpperAnimation = new System.Windows.Forms.PictureBox();
            this.tmrUpperAnimation = new System.Windows.Forms.Timer(this.components);
            this.tmrLowerAnimation = new System.Windows.Forms.Timer(this.components);
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.toolStrip = new DarkUI.Controls.DarkToolStrip();
            this.toolStripItemNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnChronological = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripItemPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemUndo = new System.Windows.Forms.ToolStripButton();
            this.tmrRender = new System.Windows.Forms.Timer(this.components);
            this.grpAnimations.SuspendLayout();
            this.grpGeneral.SuspendLayout();
            this.grpLower.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerLoopCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerFrameDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerFrameCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerVerticalFrames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerHorizontalFrames)).BeginInit();
            this.grpLowerFrameOpts.SuspendLayout();
            this.grpLowerPlayback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLowerAnimation)).BeginInit();
            this.grpUpper.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperLoopCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperFrameDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperFrameCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperVerticalFrames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperHorizontalFrames)).BeginInit();
            this.grpUpperPlayback.SuspendLayout();
            this.grpUpperFrameOpts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUpperAnimation)).BeginInit();
            this.pnlContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpAnimations
            // 
            this.grpAnimations.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpAnimations.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpAnimations.Controls.Add(this.btnClearSearch);
            this.grpAnimations.Controls.Add(this.txtSearch);
            this.grpAnimations.Controls.Add(this.lstGameObjects);
            this.grpAnimations.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpAnimations.Location = new System.Drawing.Point(3, 28);
            this.grpAnimations.Name = "grpAnimations";
            this.grpAnimations.Size = new System.Drawing.Size(203, 537);
            this.grpAnimations.TabIndex = 17;
            this.grpAnimations.TabStop = false;
            this.grpAnimations.Text = "Animations";
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.Location = new System.Drawing.Point(179, 19);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Padding = new System.Windows.Forms.Padding(5);
            this.btnClearSearch.Size = new System.Drawing.Size(18, 20);
            this.btnClearSearch.TabIndex = 19;
            this.btnClearSearch.Text = "X";
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSearch.Location = new System.Drawing.Point(6, 19);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(167, 20);
            this.txtSearch.TabIndex = 18;
            this.txtSearch.Text = "Search...";
            this.txtSearch.Click += new System.EventHandler(this.txtSearch_Click);
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.Enter += new System.EventHandler(this.txtSearch_Enter);
            this.txtSearch.Leave += new System.EventHandler(this.txtSearch_Leave);
            // 
            // lstGameObjects
            // 
            this.lstGameObjects.AllowDrop = true;
            this.lstGameObjects.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstGameObjects.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstGameObjects.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstGameObjects.HideSelection = false;
            this.lstGameObjects.ImageIndex = 0;
            this.lstGameObjects.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.lstGameObjects.Location = new System.Drawing.Point(6, 46);
            this.lstGameObjects.Name = "lstGameObjects";
            this.lstGameObjects.SelectedImageIndex = 0;
            this.lstGameObjects.Size = new System.Drawing.Size(191, 485);
            this.lstGameObjects.TabIndex = 2;
            // 
            // grpGeneral
            // 
            this.grpGeneral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpGeneral.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpGeneral.Controls.Add(this.chkCompleteSoundPlayback);
            this.grpGeneral.Controls.Add(this.btnAddFolder);
            this.grpGeneral.Controls.Add(this.lblFolder);
            this.grpGeneral.Controls.Add(this.cmbFolder);
            this.grpGeneral.Controls.Add(this.btnSwap);
            this.grpGeneral.Controls.Add(this.scrlDarkness);
            this.grpGeneral.Controls.Add(this.labelDarkness);
            this.grpGeneral.Controls.Add(this.lblSound);
            this.grpGeneral.Controls.Add(this.cmbSound);
            this.grpGeneral.Controls.Add(this.lblName);
            this.grpGeneral.Controls.Add(this.txtName);
            this.grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpGeneral.Location = new System.Drawing.Point(1, 1);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(988, 76);
            this.grpGeneral.TabIndex = 18;
            this.grpGeneral.TabStop = false;
            this.grpGeneral.Text = "General";
            // 
            // chkCompleteSoundPlayback
            // 
            this.chkCompleteSoundPlayback.Location = new System.Drawing.Point(221, 47);
            this.chkCompleteSoundPlayback.Name = "chkCompleteSoundPlayback";
            this.chkCompleteSoundPlayback.Size = new System.Drawing.Size(246, 17);
            this.chkCompleteSoundPlayback.TabIndex = 29;
            this.chkCompleteSoundPlayback.Text = "Complete Sound Playback After Anim Dies";
            this.chkCompleteSoundPlayback.CheckedChanged += new System.EventHandler(this.chkCompleteSoundPlayback_CheckedChanged);
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.Location = new System.Drawing.Point(185, 45);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Padding = new System.Windows.Forms.Padding(5);
            this.btnAddFolder.Size = new System.Drawing.Size(18, 21);
            this.btnAddFolder.TabIndex = 17;
            this.btnAddFolder.Text = "+";
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(5, 48);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(39, 13);
            this.lblFolder.TabIndex = 8;
            this.lblFolder.Text = "Folder:";
            // 
            // cmbFolder
            // 
            this.cmbFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbFolder.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbFolder.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbFolder.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbFolder.DrawDropdownHoverOutline = false;
            this.cmbFolder.DrawFocusRectangle = false;
            this.cmbFolder.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbFolder.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbFolder.FormattingEnabled = true;
            this.cmbFolder.Location = new System.Drawing.Point(60, 45);
            this.cmbFolder.Name = "cmbFolder";
            this.cmbFolder.Size = new System.Drawing.Size(119, 21);
            this.cmbFolder.TabIndex = 7;
            this.cmbFolder.Text = null;
            this.cmbFolder.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbFolder.SelectedIndexChanged += new System.EventHandler(this.cmbFolder_SelectedIndexChanged);
            // 
            // btnSwap
            // 
            this.btnSwap.Location = new System.Drawing.Point(473, 43);
            this.btnSwap.Name = "btnSwap";
            this.btnSwap.Padding = new System.Windows.Forms.Padding(5);
            this.btnSwap.Size = new System.Drawing.Size(158, 23);
            this.btnSwap.TabIndex = 6;
            this.btnSwap.Text = "Swap Upper/Lower";
            this.btnSwap.Click += new System.EventHandler(this.btnSwap_Click);
            // 
            // scrlDarkness
            // 
            this.scrlDarkness.Location = new System.Drawing.Point(584, 19);
            this.scrlDarkness.Name = "scrlDarkness";
            this.scrlDarkness.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlDarkness.Size = new System.Drawing.Size(218, 17);
            this.scrlDarkness.TabIndex = 5;
            this.scrlDarkness.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlDarkness_Scroll);
            // 
            // labelDarkness
            // 
            this.labelDarkness.AutoSize = true;
            this.labelDarkness.Location = new System.Drawing.Point(470, 20);
            this.labelDarkness.Name = "labelDarkness";
            this.labelDarkness.Size = new System.Drawing.Size(107, 13);
            this.labelDarkness.TabIndex = 4;
            this.labelDarkness.Text = "Simulate Darkness: 0";
            // 
            // lblSound
            // 
            this.lblSound.AutoSize = true;
            this.lblSound.Location = new System.Drawing.Point(218, 21);
            this.lblSound.Name = "lblSound";
            this.lblSound.Size = new System.Drawing.Size(41, 13);
            this.lblSound.TabIndex = 3;
            this.lblSound.Text = "Sound:";
            // 
            // cmbSound
            // 
            this.cmbSound.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSound.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSound.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSound.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbSound.DrawDropdownHoverOutline = false;
            this.cmbSound.DrawFocusRectangle = false;
            this.cmbSound.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSound.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSound.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSound.FormattingEnabled = true;
            this.cmbSound.Items.AddRange(new object[] {
            "None"});
            this.cmbSound.Location = new System.Drawing.Point(265, 17);
            this.cmbSound.Name = "cmbSound";
            this.cmbSound.Size = new System.Drawing.Size(187, 21);
            this.cmbSound.TabIndex = 2;
            this.cmbSound.Text = "None";
            this.cmbSound.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbSound.SelectedIndexChanged += new System.EventHandler(this.cmbSound_SelectedIndexChanged);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 21);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtName.Location = new System.Drawing.Point(60, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(143, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // grpLower
            // 
            this.grpLower.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpLower.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpLower.Controls.Add(this.chkRenderAbovePlayer);
            this.grpLower.Controls.Add(this.chkDisableLowerRotations);
            this.grpLower.Controls.Add(this.nudLowerLoopCount);
            this.grpLower.Controls.Add(this.nudLowerFrameDuration);
            this.grpLower.Controls.Add(this.nudLowerFrameCount);
            this.grpLower.Controls.Add(this.nudLowerVerticalFrames);
            this.grpLower.Controls.Add(this.nudLowerHorizontalFrames);
            this.grpLower.Controls.Add(this.grpLowerFrameOpts);
            this.grpLower.Controls.Add(this.grpLowerPlayback);
            this.grpLower.Controls.Add(this.lblLowerLoopCount);
            this.grpLower.Controls.Add(this.lblLowerFrameDuration);
            this.grpLower.Controls.Add(this.lblLowerFrameCount);
            this.grpLower.Controls.Add(this.lblLowerVerticalFrames);
            this.grpLower.Controls.Add(this.lblLowerHorizontalFrames);
            this.grpLower.Controls.Add(this.cmbLowerGraphic);
            this.grpLower.Controls.Add(this.lblLowerGraphic);
            this.grpLower.Controls.Add(this.picLowerAnimation);
            this.grpLower.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpLower.Location = new System.Drawing.Point(1, 84);
            this.grpLower.Name = "grpLower";
            this.grpLower.Size = new System.Drawing.Size(484, 450);
            this.grpLower.TabIndex = 19;
            this.grpLower.TabStop = false;
            this.grpLower.Text = "Lower Layer (Below Target)";
            // 
            // chkRenderAbovePlayer
            // 
            this.chkRenderAbovePlayer.AutoSize = true;
            this.chkRenderAbovePlayer.Location = new System.Drawing.Point(134, 424);
            this.chkRenderAbovePlayer.Name = "chkRenderAbovePlayer";
            this.chkRenderAbovePlayer.Size = new System.Drawing.Size(127, 17);
            this.chkRenderAbovePlayer.TabIndex = 27;
            this.chkRenderAbovePlayer.Text = "Render Above Player";
            this.chkRenderAbovePlayer.CheckedChanged += new System.EventHandler(this.chkRenderAbovePlayer_CheckedChanged);
            // 
            // chkDisableLowerRotations
            // 
            this.chkDisableLowerRotations.AutoSize = true;
            this.chkDisableLowerRotations.Location = new System.Drawing.Point(9, 424);
            this.chkDisableLowerRotations.Name = "chkDisableLowerRotations";
            this.chkDisableLowerRotations.Size = new System.Drawing.Size(109, 17);
            this.chkDisableLowerRotations.TabIndex = 26;
            this.chkDisableLowerRotations.Text = "Disable Rotations";
            this.chkDisableLowerRotations.CheckedChanged += new System.EventHandler(this.chkDisableLowerRotations_CheckedChanged);
            // 
            // nudLowerLoopCount
            // 
            this.nudLowerLoopCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudLowerLoopCount.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudLowerLoopCount.Location = new System.Drawing.Point(9, 398);
            this.nudLowerLoopCount.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudLowerLoopCount.Name = "nudLowerLoopCount";
            this.nudLowerLoopCount.Size = new System.Drawing.Size(194, 20);
            this.nudLowerLoopCount.TabIndex = 25;
            this.nudLowerLoopCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudLowerLoopCount.ValueChanged += new System.EventHandler(this.nudLowerLoopCount_ValueChanged);
            // 
            // nudLowerFrameDuration
            // 
            this.nudLowerFrameDuration.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudLowerFrameDuration.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudLowerFrameDuration.Location = new System.Drawing.Point(10, 363);
            this.nudLowerFrameDuration.Maximum = new decimal(new int[] {
            -10,
            4,
            0,
            0});
            this.nudLowerFrameDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLowerFrameDuration.Name = "nudLowerFrameDuration";
            this.nudLowerFrameDuration.Size = new System.Drawing.Size(194, 20);
            this.nudLowerFrameDuration.TabIndex = 24;
            this.nudLowerFrameDuration.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudLowerFrameDuration.ValueChanged += new System.EventHandler(this.nudLowerFrameDuration_ValueChanged);
            // 
            // nudLowerFrameCount
            // 
            this.nudLowerFrameCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudLowerFrameCount.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudLowerFrameCount.Location = new System.Drawing.Point(10, 330);
            this.nudLowerFrameCount.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.nudLowerFrameCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLowerFrameCount.Name = "nudLowerFrameCount";
            this.nudLowerFrameCount.Size = new System.Drawing.Size(194, 20);
            this.nudLowerFrameCount.TabIndex = 23;
            this.nudLowerFrameCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLowerFrameCount.ValueChanged += new System.EventHandler(this.nudLowerFrameCount_ValueChanged);
            // 
            // nudLowerVerticalFrames
            // 
            this.nudLowerVerticalFrames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudLowerVerticalFrames.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudLowerVerticalFrames.Location = new System.Drawing.Point(10, 296);
            this.nudLowerVerticalFrames.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudLowerVerticalFrames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLowerVerticalFrames.Name = "nudLowerVerticalFrames";
            this.nudLowerVerticalFrames.Size = new System.Drawing.Size(194, 20);
            this.nudLowerVerticalFrames.TabIndex = 22;
            this.nudLowerVerticalFrames.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLowerVerticalFrames.ValueChanged += new System.EventHandler(this.nudLowerVerticalFrames_ValueChanged);
            // 
            // nudLowerHorizontalFrames
            // 
            this.nudLowerHorizontalFrames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudLowerHorizontalFrames.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudLowerHorizontalFrames.Location = new System.Drawing.Point(10, 260);
            this.nudLowerHorizontalFrames.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudLowerHorizontalFrames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLowerHorizontalFrames.Name = "nudLowerHorizontalFrames";
            this.nudLowerHorizontalFrames.Size = new System.Drawing.Size(194, 20);
            this.nudLowerHorizontalFrames.TabIndex = 21;
            this.nudLowerHorizontalFrames.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLowerHorizontalFrames.ValueChanged += new System.EventHandler(this.nudLowerHorizontalFrames_ValueChanged);
            // 
            // grpLowerFrameOpts
            // 
            this.grpLowerFrameOpts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpLowerFrameOpts.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpLowerFrameOpts.Controls.Add(this.btnLowerClone);
            this.grpLowerFrameOpts.Controls.Add(this.lightEditorLower);
            this.grpLowerFrameOpts.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpLowerFrameOpts.Location = new System.Drawing.Point(213, 93);
            this.grpLowerFrameOpts.Name = "grpLowerFrameOpts";
            this.grpLowerFrameOpts.Size = new System.Drawing.Size(265, 319);
            this.grpLowerFrameOpts.TabIndex = 20;
            this.grpLowerFrameOpts.TabStop = false;
            this.grpLowerFrameOpts.Text = "Frame Options";
            // 
            // btnLowerClone
            // 
            this.btnLowerClone.Location = new System.Drawing.Point(91, 9);
            this.btnLowerClone.Name = "btnLowerClone";
            this.btnLowerClone.Padding = new System.Windows.Forms.Padding(5);
            this.btnLowerClone.Size = new System.Drawing.Size(163, 23);
            this.btnLowerClone.TabIndex = 16;
            this.btnLowerClone.Text = "Clone From Previous";
            this.btnLowerClone.Click += new System.EventHandler(this.btnLowerClone_Click);
            // 
            // lightEditorLower
            // 
            this.lightEditorLower.Location = new System.Drawing.Point(6, 28);
            this.lightEditorLower.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lightEditorLower.Name = "lightEditorLower";
            this.lightEditorLower.Size = new System.Drawing.Size(253, 274);
            this.lightEditorLower.TabIndex = 15;
            // 
            // grpLowerPlayback
            // 
            this.grpLowerPlayback.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpLowerPlayback.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpLowerPlayback.Controls.Add(this.btnPlayLower);
            this.grpLowerPlayback.Controls.Add(this.scrlLowerFrame);
            this.grpLowerPlayback.Controls.Add(this.lblLowerFrame);
            this.grpLowerPlayback.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpLowerPlayback.Location = new System.Drawing.Point(213, 19);
            this.grpLowerPlayback.Name = "grpLowerPlayback";
            this.grpLowerPlayback.Size = new System.Drawing.Size(265, 68);
            this.grpLowerPlayback.TabIndex = 16;
            this.grpLowerPlayback.TabStop = false;
            this.grpLowerPlayback.Text = "Playback";
            // 
            // btnPlayLower
            // 
            this.btnPlayLower.Location = new System.Drawing.Point(57, 39);
            this.btnPlayLower.Name = "btnPlayLower";
            this.btnPlayLower.Padding = new System.Windows.Forms.Padding(5);
            this.btnPlayLower.Size = new System.Drawing.Size(197, 23);
            this.btnPlayLower.TabIndex = 16;
            this.btnPlayLower.Text = "Play Lower Animation";
            this.btnPlayLower.Click += new System.EventHandler(this.btnPlayLower_Click);
            // 
            // scrlLowerFrame
            // 
            this.scrlLowerFrame.Location = new System.Drawing.Point(57, 16);
            this.scrlLowerFrame.Maximum = 1;
            this.scrlLowerFrame.Minimum = 1;
            this.scrlLowerFrame.Name = "scrlLowerFrame";
            this.scrlLowerFrame.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlLowerFrame.Size = new System.Drawing.Size(197, 17);
            this.scrlLowerFrame.TabIndex = 15;
            this.scrlLowerFrame.Value = 1;
            this.scrlLowerFrame.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlLowerFrame_Scroll);
            // 
            // lblLowerFrame
            // 
            this.lblLowerFrame.AutoSize = true;
            this.lblLowerFrame.Location = new System.Drawing.Point(6, 16);
            this.lblLowerFrame.Name = "lblLowerFrame";
            this.lblLowerFrame.Size = new System.Drawing.Size(48, 13);
            this.lblLowerFrame.TabIndex = 14;
            this.lblLowerFrame.Text = "Frame: 1";
            // 
            // lblLowerLoopCount
            // 
            this.lblLowerLoopCount.AutoSize = true;
            this.lblLowerLoopCount.Location = new System.Drawing.Point(7, 382);
            this.lblLowerLoopCount.Name = "lblLowerLoopCount";
            this.lblLowerLoopCount.Size = new System.Drawing.Size(65, 13);
            this.lblLowerLoopCount.TabIndex = 12;
            this.lblLowerLoopCount.Text = "Loop Count:";
            // 
            // lblLowerFrameDuration
            // 
            this.lblLowerFrameDuration.AutoSize = true;
            this.lblLowerFrameDuration.Location = new System.Drawing.Point(7, 350);
            this.lblLowerFrameDuration.Name = "lblLowerFrameDuration";
            this.lblLowerFrameDuration.Size = new System.Drawing.Size(104, 13);
            this.lblLowerFrameDuration.TabIndex = 10;
            this.lblLowerFrameDuration.Text = "Frame Duration (ms):";
            // 
            // lblLowerFrameCount
            // 
            this.lblLowerFrameCount.AutoSize = true;
            this.lblLowerFrameCount.Location = new System.Drawing.Point(7, 317);
            this.lblLowerFrameCount.Name = "lblLowerFrameCount";
            this.lblLowerFrameCount.Size = new System.Drawing.Size(110, 13);
            this.lblLowerFrameCount.TabIndex = 8;
            this.lblLowerFrameCount.Text = "Graphic Frame Count:";
            // 
            // lblLowerVerticalFrames
            // 
            this.lblLowerVerticalFrames.AutoSize = true;
            this.lblLowerVerticalFrames.Location = new System.Drawing.Point(7, 283);
            this.lblLowerVerticalFrames.Name = "lblLowerVerticalFrames";
            this.lblLowerVerticalFrames.Size = new System.Drawing.Size(122, 13);
            this.lblLowerVerticalFrames.TabIndex = 5;
            this.lblLowerVerticalFrames.Text = "Graphic Vertical Frames:";
            // 
            // lblLowerHorizontalFrames
            // 
            this.lblLowerHorizontalFrames.AutoSize = true;
            this.lblLowerHorizontalFrames.Location = new System.Drawing.Point(7, 247);
            this.lblLowerHorizontalFrames.Name = "lblLowerHorizontalFrames";
            this.lblLowerHorizontalFrames.Size = new System.Drawing.Size(137, 13);
            this.lblLowerHorizontalFrames.TabIndex = 4;
            this.lblLowerHorizontalFrames.Text = "Graphic Horizontal Frames: ";
            // 
            // cmbLowerGraphic
            // 
            this.cmbLowerGraphic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbLowerGraphic.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbLowerGraphic.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbLowerGraphic.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbLowerGraphic.DrawDropdownHoverOutline = false;
            this.cmbLowerGraphic.DrawFocusRectangle = false;
            this.cmbLowerGraphic.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbLowerGraphic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLowerGraphic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbLowerGraphic.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbLowerGraphic.FormattingEnabled = true;
            this.cmbLowerGraphic.Items.AddRange(new object[] {
            "//General/none"});
            this.cmbLowerGraphic.Location = new System.Drawing.Point(54, 223);
            this.cmbLowerGraphic.Name = "cmbLowerGraphic";
            this.cmbLowerGraphic.Size = new System.Drawing.Size(149, 21);
            this.cmbLowerGraphic.TabIndex = 3;
            this.cmbLowerGraphic.Text = "//General/none";
            this.cmbLowerGraphic.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbLowerGraphic.SelectedIndexChanged += new System.EventHandler(this.cmbLowerGraphic_SelectedIndexChanged);
            // 
            // lblLowerGraphic
            // 
            this.lblLowerGraphic.AutoSize = true;
            this.lblLowerGraphic.Location = new System.Drawing.Point(7, 226);
            this.lblLowerGraphic.Name = "lblLowerGraphic";
            this.lblLowerGraphic.Size = new System.Drawing.Size(50, 13);
            this.lblLowerGraphic.TabIndex = 1;
            this.lblLowerGraphic.Text = "Graphic: ";
            // 
            // picLowerAnimation
            // 
            this.picLowerAnimation.Location = new System.Drawing.Point(7, 19);
            this.picLowerAnimation.Name = "picLowerAnimation";
            this.picLowerAnimation.Size = new System.Drawing.Size(200, 200);
            this.picLowerAnimation.TabIndex = 0;
            this.picLowerAnimation.TabStop = false;
            // 
            // grpUpper
            // 
            this.grpUpper.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpUpper.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpUpper.Controls.Add(this.chkRenderBelowFringe);
            this.grpUpper.Controls.Add(this.chkDisableUpperRotations);
            this.grpUpper.Controls.Add(this.nudUpperLoopCount);
            this.grpUpper.Controls.Add(this.nudUpperFrameDuration);
            this.grpUpper.Controls.Add(this.nudUpperFrameCount);
            this.grpUpper.Controls.Add(this.nudUpperVerticalFrames);
            this.grpUpper.Controls.Add(this.nudUpperHorizontalFrames);
            this.grpUpper.Controls.Add(this.grpUpperPlayback);
            this.grpUpper.Controls.Add(this.grpUpperFrameOpts);
            this.grpUpper.Controls.Add(this.lblUpperLoopCount);
            this.grpUpper.Controls.Add(this.lblUpperFrameDuration);
            this.grpUpper.Controls.Add(this.lblUpperFrameCount);
            this.grpUpper.Controls.Add(this.lblUpperVerticalFrames);
            this.grpUpper.Controls.Add(this.lblUpperHorizontalFrames);
            this.grpUpper.Controls.Add(this.cmbUpperGraphic);
            this.grpUpper.Controls.Add(this.lblUpperGraphic);
            this.grpUpper.Controls.Add(this.picUpperAnimation);
            this.grpUpper.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpUpper.Location = new System.Drawing.Point(504, 84);
            this.grpUpper.Name = "grpUpper";
            this.grpUpper.Size = new System.Drawing.Size(485, 450);
            this.grpUpper.TabIndex = 20;
            this.grpUpper.TabStop = false;
            this.grpUpper.Text = "Upper Layer (Above Target)";
            // 
            // chkRenderBelowFringe
            // 
            this.chkRenderBelowFringe.AutoSize = true;
            this.chkRenderBelowFringe.Location = new System.Drawing.Point(139, 424);
            this.chkRenderBelowFringe.Name = "chkRenderBelowFringe";
            this.chkRenderBelowFringe.Size = new System.Drawing.Size(125, 17);
            this.chkRenderBelowFringe.TabIndex = 31;
            this.chkRenderBelowFringe.Text = "Render Below Fringe";
            this.chkRenderBelowFringe.CheckedChanged += new System.EventHandler(this.chkRenderBelowFringe_CheckedChanged);
            // 
            // chkDisableUpperRotations
            // 
            this.chkDisableUpperRotations.AutoSize = true;
            this.chkDisableUpperRotations.Location = new System.Drawing.Point(6, 424);
            this.chkDisableUpperRotations.Name = "chkDisableUpperRotations";
            this.chkDisableUpperRotations.Size = new System.Drawing.Size(109, 17);
            this.chkDisableUpperRotations.TabIndex = 27;
            this.chkDisableUpperRotations.Text = "Disable Rotations";
            this.chkDisableUpperRotations.CheckedChanged += new System.EventHandler(this.chkDisableUpperRotations_CheckedChanged);
            // 
            // nudUpperLoopCount
            // 
            this.nudUpperLoopCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudUpperLoopCount.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudUpperLoopCount.Location = new System.Drawing.Point(6, 398);
            this.nudUpperLoopCount.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudUpperLoopCount.Name = "nudUpperLoopCount";
            this.nudUpperLoopCount.Size = new System.Drawing.Size(194, 20);
            this.nudUpperLoopCount.TabIndex = 30;
            this.nudUpperLoopCount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudUpperLoopCount.ValueChanged += new System.EventHandler(this.nudUpperLoopCount_ValueChanged);
            // 
            // nudUpperFrameDuration
            // 
            this.nudUpperFrameDuration.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudUpperFrameDuration.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudUpperFrameDuration.Location = new System.Drawing.Point(6, 363);
            this.nudUpperFrameDuration.Maximum = new decimal(new int[] {
            -10,
            4,
            0,
            0});
            this.nudUpperFrameDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudUpperFrameDuration.Name = "nudUpperFrameDuration";
            this.nudUpperFrameDuration.Size = new System.Drawing.Size(194, 20);
            this.nudUpperFrameDuration.TabIndex = 29;
            this.nudUpperFrameDuration.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudUpperFrameDuration.ValueChanged += new System.EventHandler(this.nudUpperFrameDuration_ValueChanged);
            // 
            // nudUpperFrameCount
            // 
            this.nudUpperFrameCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudUpperFrameCount.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudUpperFrameCount.Location = new System.Drawing.Point(6, 330);
            this.nudUpperFrameCount.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.nudUpperFrameCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudUpperFrameCount.Name = "nudUpperFrameCount";
            this.nudUpperFrameCount.Size = new System.Drawing.Size(194, 20);
            this.nudUpperFrameCount.TabIndex = 28;
            this.nudUpperFrameCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudUpperFrameCount.ValueChanged += new System.EventHandler(this.nudUpperFrameCount_ValueChanged);
            // 
            // nudUpperVerticalFrames
            // 
            this.nudUpperVerticalFrames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudUpperVerticalFrames.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudUpperVerticalFrames.Location = new System.Drawing.Point(6, 296);
            this.nudUpperVerticalFrames.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudUpperVerticalFrames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudUpperVerticalFrames.Name = "nudUpperVerticalFrames";
            this.nudUpperVerticalFrames.Size = new System.Drawing.Size(194, 20);
            this.nudUpperVerticalFrames.TabIndex = 27;
            this.nudUpperVerticalFrames.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudUpperVerticalFrames.ValueChanged += new System.EventHandler(this.nudUpperVerticalFrames_ValueChanged);
            // 
            // nudUpperHorizontalFrames
            // 
            this.nudUpperHorizontalFrames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudUpperHorizontalFrames.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudUpperHorizontalFrames.Location = new System.Drawing.Point(6, 263);
            this.nudUpperHorizontalFrames.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
            this.nudUpperHorizontalFrames.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudUpperHorizontalFrames.Name = "nudUpperHorizontalFrames";
            this.nudUpperHorizontalFrames.Size = new System.Drawing.Size(194, 20);
            this.nudUpperHorizontalFrames.TabIndex = 26;
            this.nudUpperHorizontalFrames.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudUpperHorizontalFrames.ValueChanged += new System.EventHandler(this.nudUpperHorizontalFrames_ValueChanged);
            // 
            // grpUpperPlayback
            // 
            this.grpUpperPlayback.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpUpperPlayback.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpUpperPlayback.Controls.Add(this.btnPlayUpper);
            this.grpUpperPlayback.Controls.Add(this.scrlUpperFrame);
            this.grpUpperPlayback.Controls.Add(this.lblUpperFrame);
            this.grpUpperPlayback.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpUpperPlayback.Location = new System.Drawing.Point(212, 19);
            this.grpUpperPlayback.Name = "grpUpperPlayback";
            this.grpUpperPlayback.Size = new System.Drawing.Size(265, 68);
            this.grpUpperPlayback.TabIndex = 18;
            this.grpUpperPlayback.TabStop = false;
            this.grpUpperPlayback.Text = "Playback";
            // 
            // btnPlayUpper
            // 
            this.btnPlayUpper.Location = new System.Drawing.Point(57, 39);
            this.btnPlayUpper.Name = "btnPlayUpper";
            this.btnPlayUpper.Padding = new System.Windows.Forms.Padding(5);
            this.btnPlayUpper.Size = new System.Drawing.Size(197, 23);
            this.btnPlayUpper.TabIndex = 16;
            this.btnPlayUpper.Text = "Play Upper Animation";
            this.btnPlayUpper.Click += new System.EventHandler(this.btnPlayUpper_Click);
            // 
            // scrlUpperFrame
            // 
            this.scrlUpperFrame.Location = new System.Drawing.Point(57, 16);
            this.scrlUpperFrame.Maximum = 1;
            this.scrlUpperFrame.Minimum = 1;
            this.scrlUpperFrame.Name = "scrlUpperFrame";
            this.scrlUpperFrame.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlUpperFrame.Size = new System.Drawing.Size(197, 17);
            this.scrlUpperFrame.TabIndex = 15;
            this.scrlUpperFrame.Value = 1;
            this.scrlUpperFrame.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlUpperFrame_Scroll);
            // 
            // lblUpperFrame
            // 
            this.lblUpperFrame.AutoSize = true;
            this.lblUpperFrame.Location = new System.Drawing.Point(6, 16);
            this.lblUpperFrame.Name = "lblUpperFrame";
            this.lblUpperFrame.Size = new System.Drawing.Size(48, 13);
            this.lblUpperFrame.TabIndex = 14;
            this.lblUpperFrame.Text = "Frame: 1";
            // 
            // grpUpperFrameOpts
            // 
            this.grpUpperFrameOpts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpUpperFrameOpts.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpUpperFrameOpts.Controls.Add(this.btnUpperClone);
            this.grpUpperFrameOpts.Controls.Add(this.lightEditorUpper);
            this.grpUpperFrameOpts.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpUpperFrameOpts.Location = new System.Drawing.Point(212, 93);
            this.grpUpperFrameOpts.Name = "grpUpperFrameOpts";
            this.grpUpperFrameOpts.Size = new System.Drawing.Size(265, 319);
            this.grpUpperFrameOpts.TabIndex = 19;
            this.grpUpperFrameOpts.TabStop = false;
            this.grpUpperFrameOpts.Text = "Frame Options";
            // 
            // btnUpperClone
            // 
            this.btnUpperClone.Location = new System.Drawing.Point(96, 9);
            this.btnUpperClone.Name = "btnUpperClone";
            this.btnUpperClone.Padding = new System.Windows.Forms.Padding(5);
            this.btnUpperClone.Size = new System.Drawing.Size(163, 23);
            this.btnUpperClone.TabIndex = 17;
            this.btnUpperClone.Text = "Clone From Previous";
            this.btnUpperClone.Click += new System.EventHandler(this.btnUpperClone_Click);
            // 
            // lightEditorUpper
            // 
            this.lightEditorUpper.Location = new System.Drawing.Point(6, 28);
            this.lightEditorUpper.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lightEditorUpper.Name = "lightEditorUpper";
            this.lightEditorUpper.Size = new System.Drawing.Size(253, 274);
            this.lightEditorUpper.TabIndex = 15;
            // 
            // lblUpperLoopCount
            // 
            this.lblUpperLoopCount.AutoSize = true;
            this.lblUpperLoopCount.Location = new System.Drawing.Point(6, 382);
            this.lblUpperLoopCount.Name = "lblUpperLoopCount";
            this.lblUpperLoopCount.Size = new System.Drawing.Size(65, 13);
            this.lblUpperLoopCount.TabIndex = 24;
            this.lblUpperLoopCount.Text = "Loop Count:";
            // 
            // lblUpperFrameDuration
            // 
            this.lblUpperFrameDuration.AutoSize = true;
            this.lblUpperFrameDuration.Location = new System.Drawing.Point(6, 350);
            this.lblUpperFrameDuration.Name = "lblUpperFrameDuration";
            this.lblUpperFrameDuration.Size = new System.Drawing.Size(104, 13);
            this.lblUpperFrameDuration.TabIndex = 22;
            this.lblUpperFrameDuration.Text = "Frame Duration (ms):";
            // 
            // lblUpperFrameCount
            // 
            this.lblUpperFrameCount.AutoSize = true;
            this.lblUpperFrameCount.Location = new System.Drawing.Point(6, 317);
            this.lblUpperFrameCount.Name = "lblUpperFrameCount";
            this.lblUpperFrameCount.Size = new System.Drawing.Size(110, 13);
            this.lblUpperFrameCount.TabIndex = 20;
            this.lblUpperFrameCount.Text = "Graphic Frame Count:";
            // 
            // lblUpperVerticalFrames
            // 
            this.lblUpperVerticalFrames.AutoSize = true;
            this.lblUpperVerticalFrames.Location = new System.Drawing.Point(6, 283);
            this.lblUpperVerticalFrames.Name = "lblUpperVerticalFrames";
            this.lblUpperVerticalFrames.Size = new System.Drawing.Size(122, 13);
            this.lblUpperVerticalFrames.TabIndex = 17;
            this.lblUpperVerticalFrames.Text = "Graphic Vertical Frames:";
            // 
            // lblUpperHorizontalFrames
            // 
            this.lblUpperHorizontalFrames.AutoSize = true;
            this.lblUpperHorizontalFrames.Location = new System.Drawing.Point(6, 247);
            this.lblUpperHorizontalFrames.Name = "lblUpperHorizontalFrames";
            this.lblUpperHorizontalFrames.Size = new System.Drawing.Size(134, 13);
            this.lblUpperHorizontalFrames.TabIndex = 16;
            this.lblUpperHorizontalFrames.Text = "Graphic Horizontal Frames:";
            // 
            // cmbUpperGraphic
            // 
            this.cmbUpperGraphic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbUpperGraphic.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbUpperGraphic.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbUpperGraphic.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbUpperGraphic.DrawDropdownHoverOutline = false;
            this.cmbUpperGraphic.DrawFocusRectangle = false;
            this.cmbUpperGraphic.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbUpperGraphic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUpperGraphic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbUpperGraphic.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbUpperGraphic.FormattingEnabled = true;
            this.cmbUpperGraphic.Items.AddRange(new object[] {
            "//General/none"});
            this.cmbUpperGraphic.Location = new System.Drawing.Point(57, 223);
            this.cmbUpperGraphic.Name = "cmbUpperGraphic";
            this.cmbUpperGraphic.Size = new System.Drawing.Size(143, 21);
            this.cmbUpperGraphic.TabIndex = 15;
            this.cmbUpperGraphic.Text = "//General/none";
            this.cmbUpperGraphic.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbUpperGraphic.SelectedIndexChanged += new System.EventHandler(this.cmbUpperGraphic_SelectedIndexChanged);
            // 
            // lblUpperGraphic
            // 
            this.lblUpperGraphic.AutoSize = true;
            this.lblUpperGraphic.Location = new System.Drawing.Point(6, 226);
            this.lblUpperGraphic.Name = "lblUpperGraphic";
            this.lblUpperGraphic.Size = new System.Drawing.Size(50, 13);
            this.lblUpperGraphic.TabIndex = 14;
            this.lblUpperGraphic.Text = "Graphic: ";
            // 
            // picUpperAnimation
            // 
            this.picUpperAnimation.Location = new System.Drawing.Point(6, 19);
            this.picUpperAnimation.Name = "picUpperAnimation";
            this.picUpperAnimation.Size = new System.Drawing.Size(200, 200);
            this.picUpperAnimation.TabIndex = 1;
            this.picUpperAnimation.TabStop = false;
            // 
            // tmrUpperAnimation
            // 
            this.tmrUpperAnimation.Enabled = true;
            this.tmrUpperAnimation.Tick += new System.EventHandler(this.tmrUpperAnimation_Tick);
            // 
            // tmrLowerAnimation
            // 
            this.tmrLowerAnimation.Enabled = true;
            this.tmrLowerAnimation.Tick += new System.EventHandler(this.tmrLowerAnimation_Tick);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(1014, 574);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(818, 574);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.grpUpper);
            this.pnlContainer.Controls.Add(this.grpGeneral);
            this.pnlContainer.Controls.Add(this.grpLower);
            this.pnlContainer.Location = new System.Drawing.Point(216, 28);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(988, 537);
            this.pnlContainer.TabIndex = 21;
            this.pnlContainer.Visible = false;
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
            this.btnChronological,
            this.toolStripSeparator4,
            this.toolStripItemCopy,
            this.toolStripItemPaste,
            this.toolStripSeparator3,
            this.toolStripItemUndo});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.toolStrip.Size = new System.Drawing.Size(1210, 25);
            this.toolStrip.TabIndex = 41;
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
            // btnChronological
            // 
            this.btnChronological.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnChronological.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnChronological.Image = ((System.Drawing.Image)(resources.GetObject("btnChronological.Image")));
            this.btnChronological.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnChronological.Name = "btnChronological";
            this.btnChronological.Size = new System.Drawing.Size(23, 22);
            this.btnChronological.Text = "Order Chronologically";
            this.btnChronological.Click += new System.EventHandler(this.btnChronological_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator4.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
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
            // tmrRender
            // 
            this.tmrRender.Enabled = true;
            this.tmrRender.Interval = 16;
            this.tmrRender.Tick += new System.EventHandler(this.tmrRender_Tick);
            // 
            // FrmAnimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(1210, 607);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpAnimations);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "FrmAnimation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Animation Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmAnimation_FormClosed);
            this.Load += new System.EventHandler(this.frmAnimation_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.grpAnimations.ResumeLayout(false);
            this.grpAnimations.PerformLayout();
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            this.grpLower.ResumeLayout(false);
            this.grpLower.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerLoopCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerFrameDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerFrameCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerVerticalFrames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerHorizontalFrames)).EndInit();
            this.grpLowerFrameOpts.ResumeLayout(false);
            this.grpLowerPlayback.ResumeLayout(false);
            this.grpLowerPlayback.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLowerAnimation)).EndInit();
            this.grpUpper.ResumeLayout(false);
            this.grpUpper.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperLoopCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperFrameDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperFrameCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperVerticalFrames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperHorizontalFrames)).EndInit();
            this.grpUpperPlayback.ResumeLayout(false);
            this.grpUpperPlayback.PerformLayout();
            this.grpUpperFrameOpts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picUpperAnimation)).EndInit();
            this.pnlContainer.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpAnimations;
        private DarkGroupBox grpGeneral;
        private System.Windows.Forms.Label lblSound;
        private DarkComboBox cmbSound;
        private System.Windows.Forms.Label lblName;
        private DarkTextBox txtName;
        private DarkGroupBox grpLower;
        private System.Windows.Forms.Label lblLowerLoopCount;
        private System.Windows.Forms.Label lblLowerFrameDuration;
        private System.Windows.Forms.Label lblLowerFrameCount;
        private System.Windows.Forms.Label lblLowerVerticalFrames;
        private System.Windows.Forms.Label lblLowerHorizontalFrames;
        private DarkComboBox cmbLowerGraphic;
        private System.Windows.Forms.Label lblLowerGraphic;
        private System.Windows.Forms.PictureBox picLowerAnimation;
        private DarkGroupBox grpUpper;
        private System.Windows.Forms.Label lblUpperLoopCount;
        private System.Windows.Forms.Label lblUpperFrameDuration;
        private System.Windows.Forms.Label lblUpperFrameCount;
        private System.Windows.Forms.Label lblUpperVerticalFrames;
        private System.Windows.Forms.Label lblUpperHorizontalFrames;
        private DarkComboBox cmbUpperGraphic;
        private System.Windows.Forms.Label lblUpperGraphic;
        private System.Windows.Forms.PictureBox picUpperAnimation;
        private System.Windows.Forms.Timer tmrUpperAnimation;
        private System.Windows.Forms.Timer tmrLowerAnimation;
        private DarkGroupBox grpLowerPlayback;
        private DarkButton btnPlayLower;
        private DarkScrollBar scrlLowerFrame;
        private System.Windows.Forms.Label lblLowerFrame;
        private DarkGroupBox grpUpperPlayback;
        private DarkButton btnPlayUpper;
        private DarkScrollBar scrlUpperFrame;
        private System.Windows.Forms.Label lblUpperFrame;
        private DarkGroupBox grpUpperFrameOpts;
        private Controls.LightEditorCtrl lightEditorUpper;
        private DarkGroupBox grpLowerFrameOpts;
        private DarkButton btnLowerClone;
        private DarkButton btnUpperClone;
        public Controls.LightEditorCtrl lightEditorLower;
        private DarkScrollBar scrlDarkness;
        private System.Windows.Forms.Label labelDarkness;
        private DarkButton btnSwap;
        private DarkButton btnSave;
        private DarkButton btnCancel;
        private System.Windows.Forms.Panel pnlContainer;
        private DarkToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripItemNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripItemDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton toolStripItemCopy;
        public System.Windows.Forms.ToolStripButton toolStripItemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.ToolStripButton toolStripItemUndo;
        private DarkNumericUpDown nudLowerHorizontalFrames;
        private DarkNumericUpDown nudLowerLoopCount;
        private DarkNumericUpDown nudLowerFrameDuration;
        private DarkNumericUpDown nudLowerFrameCount;
        private DarkNumericUpDown nudLowerVerticalFrames;
        private DarkNumericUpDown nudUpperLoopCount;
        private DarkNumericUpDown nudUpperFrameDuration;
        private DarkNumericUpDown nudUpperFrameCount;
        private DarkNumericUpDown nudUpperVerticalFrames;
        private DarkNumericUpDown nudUpperHorizontalFrames;
        private System.Windows.Forms.Timer tmrRender;
        private DarkCheckBox chkDisableLowerRotations;
        private DarkCheckBox chkDisableUpperRotations;
        private DarkCheckBox chkRenderAbovePlayer;
        private DarkCheckBox chkRenderBelowFringe;
        private System.Windows.Forms.Label lblFolder;
        private DarkComboBox cmbFolder;
        private DarkButton btnAddFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton btnChronological;
        private DarkButton btnClearSearch;
        private DarkTextBox txtSearch;
        private DarkCheckBox chkCompleteSoundPlayback;
        private Controls.GameObjectList lstGameObjects;
    }
}