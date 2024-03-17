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
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAnimation));
            imglstPlayPause = new ImageList(components);
            grpAnimations = new DarkGroupBox();
            btnClearSearch = new DarkButton();
            txtSearch = new DarkTextBox();
            lstGameObjects = new Controls.GameObjectList();
            grpGeneral = new DarkGroupBox();
            chkCompleteSoundPlayback = new DarkCheckBox();
            chkLoopSoundDuringPreview = new DarkCheckBox();
            btnAddFolder = new DarkButton();
            lblFolder = new Label();
            cmbFolder = new DarkComboBox();
            btnSwap = new DarkButton();
            scrlDarkness = new DarkScrollBar();
            labelDarkness = new Label();
            lblSound = new Label();
            cmbSound = new DarkComboBox();
            btnPlaySound = new DarkButton();
            lblName = new Label();
            txtName = new DarkTextBox();
            grpLower = new DarkGroupBox();
            grpLowerExtraOptions = new DarkGroupBox();
            chkDisableLowerRotations = new DarkCheckBox();
            chkRenderAbovePlayer = new DarkCheckBox();
            grpLowerFrameOpts = new DarkGroupBox();
            btnLowerClone = new DarkButton();
            nudLowerLoopCount = new DarkNumericUpDown();
            lblLowerLoopCount = new Label();
            nudLowerFrameDuration = new DarkNumericUpDown();
            nudLowerFrameCount = new DarkNumericUpDown();
            nudLowerVerticalFrames = new DarkNumericUpDown();
            lblLowerFrameDuration = new Label();
            nudLowerHorizontalFrames = new DarkNumericUpDown();
            lblLowerFrameCount = new Label();
            lblLowerHorizontalFrames = new Label();
            lblLowerVerticalFrames = new Label();
            lightEditorLower = new Controls.LightEditorCtrl();
            grpLowerPlayback = new DarkGroupBox();
            btnPlayLower = new DarkButton();
            scrlLowerFrame = new DarkScrollBar();
            lblLowerFrame = new Label();
            cmbLowerGraphic = new DarkComboBox();
            lblLowerGraphic = new Label();
            picLowerAnimation = new PictureBox();
            grpUpper = new DarkGroupBox();
            grpUpperExtraOptions = new DarkGroupBox();
            chkDisableUpperRotations = new DarkCheckBox();
            chkRenderBelowFringe = new DarkCheckBox();
            lightEditorUpper = new Controls.LightEditorCtrl();
            grpUpperPlayback = new DarkGroupBox();
            btnPlayUpper = new DarkButton();
            scrlUpperFrame = new DarkScrollBar();
            lblUpperFrame = new Label();
            grpUpperFrameOpts = new DarkGroupBox();
            btnUpperClone = new DarkButton();
            nudUpperLoopCount = new DarkNumericUpDown();
            lblUpperFrameCount = new Label();
            nudUpperFrameDuration = new DarkNumericUpDown();
            nudUpperFrameCount = new DarkNumericUpDown();
            lblUpperLoopCount = new Label();
            nudUpperVerticalFrames = new DarkNumericUpDown();
            lblUpperHorizontalFrames = new Label();
            nudUpperHorizontalFrames = new DarkNumericUpDown();
            lblUpperFrameDuration = new Label();
            lblUpperVerticalFrames = new Label();
            cmbUpperGraphic = new DarkComboBox();
            lblUpperGraphic = new Label();
            picUpperAnimation = new PictureBox();
            tmrUpperAnimation = new System.Windows.Forms.Timer(components);
            tmrLowerAnimation = new System.Windows.Forms.Timer(components);
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            pnlContainer = new Panel();
            toolStrip = new DarkToolStrip();
            toolStripItemNew = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripItemDelete = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            btnAlphabetical = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            toolStripItemCopy = new ToolStripButton();
            toolStripItemPaste = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripItemUndo = new ToolStripButton();
            tmrRender = new System.Windows.Forms.Timer(components);
            grpAnimations.SuspendLayout();
            grpGeneral.SuspendLayout();
            grpLower.SuspendLayout();
            grpLowerExtraOptions.SuspendLayout();
            grpLowerFrameOpts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudLowerLoopCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLowerFrameDuration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLowerFrameCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLowerVerticalFrames).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLowerHorizontalFrames).BeginInit();
            grpLowerPlayback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLowerAnimation).BeginInit();
            grpUpper.SuspendLayout();
            grpUpperExtraOptions.SuspendLayout();
            grpUpperPlayback.SuspendLayout();
            grpUpperFrameOpts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudUpperLoopCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudUpperFrameDuration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudUpperFrameCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudUpperVerticalFrames).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudUpperHorizontalFrames).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picUpperAnimation).BeginInit();
            pnlContainer.SuspendLayout();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // imglstPlayPause
            // 
            imglstPlayPause.ColorDepth = ColorDepth.Depth32Bit;
            imglstPlayPause.ImageStream = (ImageListStreamer)resources.GetObject("imglstPlayPause.ImageStream");
            imglstPlayPause.TransparentColor = System.Drawing.Color.Transparent;
            imglstPlayPause.Images.SetKeyName(0, "sharp_pause_white_48dp.png");
            imglstPlayPause.Images.SetKeyName(1, "sharp_play_arrow_white_48dp.png");
            // 
            // grpAnimations
            // 
            grpAnimations.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpAnimations.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpAnimations.Controls.Add(btnClearSearch);
            grpAnimations.Controls.Add(txtSearch);
            grpAnimations.Controls.Add(lstGameObjects);
            grpAnimations.ForeColor = System.Drawing.Color.Gainsboro;
            grpAnimations.Location = new System.Drawing.Point(4, 32);
            grpAnimations.Margin = new Padding(4, 3, 4, 3);
            grpAnimations.Name = "grpAnimations";
            grpAnimations.Padding = new Padding(4, 3, 4, 3);
            grpAnimations.Size = new Size(233, 586);
            grpAnimations.TabIndex = 17;
            grpAnimations.TabStop = false;
            grpAnimations.Text = "Animations";
            // 
            // btnClearSearch
            // 
            btnClearSearch.Location = new System.Drawing.Point(204, 22);
            btnClearSearch.Margin = new Padding(4, 3, 4, 3);
            btnClearSearch.Name = "btnClearSearch";
            btnClearSearch.Padding = new Padding(6);
            btnClearSearch.Size = new Size(21, 23);
            btnClearSearch.TabIndex = 19;
            btnClearSearch.Text = "X";
            btnClearSearch.Click += btnClearSearch_Click;
            // 
            // txtSearch
            // 
            txtSearch.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtSearch.Location = new System.Drawing.Point(6, 22);
            txtSearch.Margin = new Padding(4, 3, 4, 3);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(192, 23);
            txtSearch.TabIndex = 18;
            txtSearch.Text = "Search...";
            txtSearch.Click += txtSearch_Click;
            txtSearch.TextChanged += txtSearch_TextChanged;
            txtSearch.Enter += txtSearch_Enter;
            txtSearch.Leave += txtSearch_Leave;
            // 
            // lstGameObjects
            // 
            lstGameObjects.AllowDrop = true;
            lstGameObjects.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstGameObjects.BorderStyle = BorderStyle.None;
            lstGameObjects.ForeColor = System.Drawing.Color.Gainsboro;
            lstGameObjects.HideSelection = false;
            lstGameObjects.ImageIndex = 0;
            lstGameObjects.LineColor = System.Drawing.Color.FromArgb(150, 150, 150);
            lstGameObjects.Location = new System.Drawing.Point(6, 53);
            lstGameObjects.Margin = new Padding(4, 3, 4, 3);
            lstGameObjects.Name = "lstGameObjects";
            lstGameObjects.SelectedImageIndex = 0;
            lstGameObjects.Size = new Size(222, 525);
            lstGameObjects.TabIndex = 2;
            // 
            // grpGeneral
            // 
            grpGeneral.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpGeneral.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpGeneral.Controls.Add(chkCompleteSoundPlayback);
            grpGeneral.Controls.Add(chkLoopSoundDuringPreview);
            grpGeneral.Controls.Add(btnAddFolder);
            grpGeneral.Controls.Add(lblFolder);
            grpGeneral.Controls.Add(cmbFolder);
            grpGeneral.Controls.Add(btnSwap);
            grpGeneral.Controls.Add(scrlDarkness);
            grpGeneral.Controls.Add(labelDarkness);
            grpGeneral.Controls.Add(lblSound);
            grpGeneral.Controls.Add(cmbSound);
            grpGeneral.Controls.Add(btnPlaySound);
            grpGeneral.Controls.Add(lblName);
            grpGeneral.Controls.Add(txtName);
            grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
            grpGeneral.Location = new System.Drawing.Point(8, 0);
            grpGeneral.Margin = new Padding(4, 3, 4, 3);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Padding = new Padding(4, 3, 4, 3);
            grpGeneral.Size = new Size(832, 100);
            grpGeneral.TabIndex = 18;
            grpGeneral.TabStop = false;
            grpGeneral.Text = "General";
            // 
            // chkCompleteSoundPlayback
            // 
            chkCompleteSoundPlayback.Location = new System.Drawing.Point(288, 52);
            chkCompleteSoundPlayback.Margin = new Padding(4, 3, 4, 3);
            chkCompleteSoundPlayback.Name = "chkCompleteSoundPlayback";
            chkCompleteSoundPlayback.Size = new Size(287, 20);
            chkCompleteSoundPlayback.TabIndex = 29;
            chkCompleteSoundPlayback.Text = "Complete Sound Playback After Anim Dies";
            chkCompleteSoundPlayback.CheckedChanged += chkCompleteSoundPlayback_CheckedChanged;
            //
            // chkLoopSoundDuringPreview
            //
            chkLoopSoundDuringPreview.Location = new System.Drawing.Point(288, 72);
            chkLoopSoundDuringPreview.Margin = new Padding(4, 3, 4, 3);
            chkLoopSoundDuringPreview.Name = "chkLoopSoundDuringPreview";
            chkLoopSoundDuringPreview.Size = new Size(287, 20);
            chkLoopSoundDuringPreview.TabIndex = 29;
            chkLoopSoundDuringPreview.Text = "Loop sound during preview";
            chkLoopSoundDuringPreview.CheckedChanged += chkLoopSoundDuringPreview_CheckedChanged;
            // 
            // btnAddFolder
            // 
            btnAddFolder.Location = new System.Drawing.Point(216, 59);
            btnAddFolder.Margin = new Padding(4, 3, 4, 3);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Padding = new Padding(6);
            btnAddFolder.Size = new Size(24, 24);
            btnAddFolder.TabIndex = 17;
            btnAddFolder.Text = "+";
            btnAddFolder.Click += btnAddFolder_Click;
            // 
            // lblFolder
            // 
            lblFolder.AutoSize = true;
            lblFolder.Location = new System.Drawing.Point(6, 59);
            lblFolder.Margin = new Padding(4, 0, 4, 0);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new Size(43, 15);
            lblFolder.TabIndex = 8;
            lblFolder.Text = "Folder:";
            // 
            // cmbFolder
            // 
            cmbFolder.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbFolder.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbFolder.BorderStyle = ButtonBorderStyle.Solid;
            cmbFolder.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbFolder.DrawDropdownHoverOutline = false;
            cmbFolder.DrawFocusRectangle = false;
            cmbFolder.DrawMode = DrawMode.OwnerDrawFixed;
            cmbFolder.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFolder.FlatStyle = FlatStyle.Flat;
            cmbFolder.ForeColor = System.Drawing.Color.Gainsboro;
            cmbFolder.FormattingEnabled = true;
            cmbFolder.Location = new System.Drawing.Point(70, 60);
            cmbFolder.Margin = new Padding(4, 3, 4, 3);
            cmbFolder.Name = "cmbFolder";
            cmbFolder.Size = new Size(138, 24);
            cmbFolder.TabIndex = 7;
            cmbFolder.Text = null;
            cmbFolder.TextPadding = new Padding(2);
            cmbFolder.SelectedIndexChanged += cmbFolder_SelectedIndexChanged;
            // 
            // btnSwap
            // 
            btnSwap.Location = new System.Drawing.Point(630, 22);
            btnSwap.Margin = new Padding(4, 3, 4, 3);
            btnSwap.Name = "btnSwap";
            btnSwap.Padding = new Padding(6);
            btnSwap.Size = new Size(184, 24);
            btnSwap.TabIndex = 6;
            btnSwap.Text = "Swap Upper/Lower";
            btnSwap.Click += btnSwap_Click;
            // 
            // scrlDarkness
            // 
            scrlDarkness.Location = new System.Drawing.Point(642, 70);
            scrlDarkness.Margin = new Padding(4, 3, 4, 3);
            scrlDarkness.Name = "scrlDarkness";
            scrlDarkness.ScrollOrientation = DarkScrollOrientation.Horizontal;
            scrlDarkness.Size = new Size(153, 20);
            scrlDarkness.TabIndex = 5;
            scrlDarkness.ValueChanged += scrlDarkness_Scroll;
            // 
            // labelDarkness
            // 
            labelDarkness.AutoSize = true;
            labelDarkness.Location = new System.Drawing.Point(660, 56);
            labelDarkness.Margin = new Padding(0, 0, 0, 0);
            labelDarkness.Name = "labelDarkness";
            labelDarkness.Size = new Size(115, 15);
            labelDarkness.TabIndex = 4;
            labelDarkness.Text = "Simulate Darkness: 0";
            // 
            // lblSound
            // 
            lblSound.AutoSize = true;
            lblSound.Location = new System.Drawing.Point(284, 20);
            lblSound.Margin = new Padding(4, 0, 4, 0);
            lblSound.Name = "lblSound";
            lblSound.Size = new Size(44, 15);
            lblSound.TabIndex = 3;
            lblSound.Text = "Sound:";
            // 
            // cmbSound
            // 
            cmbSound.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbSound.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbSound.BorderStyle = ButtonBorderStyle.Solid;
            cmbSound.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbSound.DrawDropdownHoverOutline = false;
            cmbSound.DrawFocusRectangle = false;
            cmbSound.DrawMode = DrawMode.OwnerDrawFixed;
            cmbSound.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSound.FlatStyle = FlatStyle.Flat;
            cmbSound.ForeColor = System.Drawing.Color.Gainsboro;
            cmbSound.FormattingEnabled = true;
            cmbSound.Items.AddRange(new object[] { "None" });
            cmbSound.Location = new System.Drawing.Point(339, 22);
            cmbSound.Margin = new Padding(4, 3, 4, 3);
            cmbSound.Name = "cmbSound";
            cmbSound.Size = new Size(210, 15);
            cmbSound.TabIndex = 2;
            cmbSound.Text = "None";
            cmbSound.TextPadding = new Padding(2);
            cmbSound.SelectedIndexChanged += cmbSound_SelectedIndexChanged;
            // 
            // btnPlaySound
            //
            btnPlaySound.BackgroundImageLayout = ImageLayout.Stretch;
            btnPlaySound.ImageKey = "sharp_play_arrow_white_48dp.png";
            btnPlaySound.ImageList = imglstPlayPause;
            btnPlaySound.ImagePadding = 0;
            btnPlaySound.Location = new System.Drawing.Point(558, 22);
            btnPlaySound.Margin = new Padding(4, 3, 4, 3);
            btnPlaySound.Name = "btnPlaySound";
            btnPlaySound.Padding = new Padding(6);
            btnPlaySound.Size = new Size(20, 20);
            btnPlaySound.TabIndex = 1;
            btnPlaySound.Click += btnPlaySound_Click;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(7, 22);
            lblName.Margin = new Padding(4, 0, 4, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(42, 15);
            lblName.TabIndex = 1;
            lblName.Text = "Name:";
            // 
            // txtName
            // 
            txtName.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtName.Location = new System.Drawing.Point(70, 20);
            txtName.Margin = new Padding(4, 3, 4, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(166, 23);
            txtName.TabIndex = 0;
            txtName.TextChanged += txtName_TextChanged;
            // 
            // grpLower
            // 
            grpLower.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpLower.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpLower.Controls.Add(grpLowerExtraOptions);
            grpLower.Controls.Add(grpLowerFrameOpts);
            grpLower.Controls.Add(lightEditorLower);
            grpLower.Controls.Add(grpLowerPlayback);
            grpLower.Controls.Add(cmbLowerGraphic);
            grpLower.Controls.Add(lblLowerGraphic);
            grpLower.Controls.Add(picLowerAnimation);
            grpLower.ForeColor = System.Drawing.Color.Gainsboro;
            grpLower.Location = new System.Drawing.Point(8, 102);
            grpLower.Margin = new Padding(4);
            grpLower.Name = "grpLower";
            grpLower.Padding = new Padding(4);
            grpLower.Size = new Size(488, 468);
            grpLower.TabIndex = 19;
            grpLower.TabStop = false;
            grpLower.Text = "Lower Layer (Below Target)";
            // 
            // grpLowerExtraOptions
            // 
            grpLowerExtraOptions.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpLowerExtraOptions.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpLowerExtraOptions.Controls.Add(chkDisableLowerRotations);
            grpLowerExtraOptions.Controls.Add(chkRenderAbovePlayer);
            grpLowerExtraOptions.ForeColor = System.Drawing.Color.Gainsboro;
            grpLowerExtraOptions.Location = new System.Drawing.Point(208, 208);
            grpLowerExtraOptions.Margin = new Padding(4, 3, 4, 3);
            grpLowerExtraOptions.Name = "grpLowerExtraOptions";
            grpLowerExtraOptions.Padding = new Padding(4, 3, 4, 3);
            grpLowerExtraOptions.Size = new Size(272, 70);
            grpLowerExtraOptions.TabIndex = 28;
            grpLowerExtraOptions.TabStop = false;
            grpLowerExtraOptions.Text = "Extra Options:";
            // 
            // chkDisableLowerRotations
            // 
            chkDisableLowerRotations.AutoSize = true;
            chkDisableLowerRotations.Location = new System.Drawing.Point(8, 22);
            chkDisableLowerRotations.Margin = new Padding(4, 3, 4, 3);
            chkDisableLowerRotations.Name = "chkDisableLowerRotations";
            chkDisableLowerRotations.Size = new Size(117, 19);
            chkDisableLowerRotations.TabIndex = 26;
            chkDisableLowerRotations.Text = "Disable Rotations";
            chkDisableLowerRotations.CheckedChanged += chkDisableLowerRotations_CheckedChanged;
            // 
            // chkRenderAbovePlayer
            // 
            chkRenderAbovePlayer.AutoSize = true;
            chkRenderAbovePlayer.Location = new System.Drawing.Point(8, 43);
            chkRenderAbovePlayer.Margin = new Padding(4, 3, 4, 3);
            chkRenderAbovePlayer.Name = "chkRenderAbovePlayer";
            chkRenderAbovePlayer.Size = new Size(135, 19);
            chkRenderAbovePlayer.TabIndex = 27;
            chkRenderAbovePlayer.Text = "Render Above Player";
            chkRenderAbovePlayer.CheckedChanged += chkRenderAbovePlayer_CheckedChanged;
            // 
            // grpLowerFrameOpts
            // 
            grpLowerFrameOpts.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpLowerFrameOpts.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpLowerFrameOpts.Controls.Add(btnLowerClone);
            grpLowerFrameOpts.Controls.Add(nudLowerLoopCount);
            grpLowerFrameOpts.Controls.Add(lblLowerLoopCount);
            grpLowerFrameOpts.Controls.Add(nudLowerFrameDuration);
            grpLowerFrameOpts.Controls.Add(nudLowerFrameCount);
            grpLowerFrameOpts.Controls.Add(nudLowerVerticalFrames);
            grpLowerFrameOpts.Controls.Add(lblLowerFrameDuration);
            grpLowerFrameOpts.Controls.Add(nudLowerHorizontalFrames);
            grpLowerFrameOpts.Controls.Add(lblLowerFrameCount);
            grpLowerFrameOpts.Controls.Add(lblLowerHorizontalFrames);
            grpLowerFrameOpts.Controls.Add(lblLowerVerticalFrames);
            grpLowerFrameOpts.ForeColor = System.Drawing.Color.Gainsboro;
            grpLowerFrameOpts.Location = new System.Drawing.Point(208, 67);
            grpLowerFrameOpts.Margin = new Padding(4, 3, 4, 3);
            grpLowerFrameOpts.Name = "grpLowerFrameOpts";
            grpLowerFrameOpts.Padding = new Padding(4, 3, 4, 3);
            grpLowerFrameOpts.Size = new Size(272, 135);
            grpLowerFrameOpts.TabIndex = 20;
            grpLowerFrameOpts.TabStop = false;
            grpLowerFrameOpts.Text = "Frame Options";
            // 
            // btnLowerClone
            // 
            btnLowerClone.Location = new System.Drawing.Point(104, 15);
            btnLowerClone.Margin = new Padding(4, 3, 4, 3);
            btnLowerClone.Name = "btnLowerClone";
            btnLowerClone.Padding = new Padding(6);
            btnLowerClone.Size = new Size(160, 22);
            btnLowerClone.TabIndex = 16;
            btnLowerClone.Text = "Clone From Previous";
            btnLowerClone.Click += btnLowerClone_Click;
            // 
            // nudLowerLoopCount
            // 
            nudLowerLoopCount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudLowerLoopCount.ForeColor = System.Drawing.Color.Gainsboro;
            nudLowerLoopCount.Location = new System.Drawing.Point(174, 104);
            nudLowerLoopCount.Margin = new Padding(4, 3, 4, 3);
            nudLowerLoopCount.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudLowerLoopCount.Name = "nudLowerLoopCount";
            nudLowerLoopCount.Size = new Size(90, 23);
            nudLowerLoopCount.TabIndex = 25;
            nudLowerLoopCount.Value = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudLowerLoopCount.ValueChanged += nudLowerLoopCount_ValueChanged;
            // 
            // lblLowerLoopCount
            // 
            lblLowerLoopCount.AutoSize = true;
            lblLowerLoopCount.Location = new System.Drawing.Point(174, 86);
            lblLowerLoopCount.Margin = new Padding(4, 0, 4, 0);
            lblLowerLoopCount.Name = "lblLowerLoopCount";
            lblLowerLoopCount.Size = new Size(70, 15);
            lblLowerLoopCount.TabIndex = 12;
            lblLowerLoopCount.Text = "Loop Count";
            // 
            // nudLowerFrameDuration
            // 
            nudLowerFrameDuration.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudLowerFrameDuration.ForeColor = System.Drawing.Color.Gainsboro;
            nudLowerFrameDuration.Location = new System.Drawing.Point(8, 104);
            nudLowerFrameDuration.Margin = new Padding(4, 3, 4, 3);
            nudLowerFrameDuration.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudLowerFrameDuration.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudLowerFrameDuration.Name = "nudLowerFrameDuration";
            nudLowerFrameDuration.Size = new Size(90, 23);
            nudLowerFrameDuration.TabIndex = 24;
            nudLowerFrameDuration.Value = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudLowerFrameDuration.ValueChanged += nudLowerFrameDuration_ValueChanged;
            // 
            // nudLowerFrameCount
            // 
            nudLowerFrameCount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudLowerFrameCount.ForeColor = System.Drawing.Color.Gainsboro;
            nudLowerFrameCount.Location = new System.Drawing.Point(8, 60);
            nudLowerFrameCount.Margin = new Padding(4, 3, 4, 3);
            nudLowerFrameCount.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            nudLowerFrameCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudLowerFrameCount.Name = "nudLowerFrameCount";
            nudLowerFrameCount.Size = new Size(80, 23);
            nudLowerFrameCount.TabIndex = 23;
            nudLowerFrameCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudLowerFrameCount.ValueChanged += nudLowerFrameCount_ValueChanged;
            // 
            // nudLowerVerticalFrames
            // 
            nudLowerVerticalFrames.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudLowerVerticalFrames.ForeColor = System.Drawing.Color.Gainsboro;
            nudLowerVerticalFrames.Location = new System.Drawing.Point(184, 60);
            nudLowerVerticalFrames.Margin = new Padding(4, 3, 4, 3);
            nudLowerVerticalFrames.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            nudLowerVerticalFrames.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudLowerVerticalFrames.Name = "nudLowerVerticalFrames";
            nudLowerVerticalFrames.Size = new Size(80, 23);
            nudLowerVerticalFrames.TabIndex = 22;
            nudLowerVerticalFrames.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudLowerVerticalFrames.ValueChanged += nudLowerVerticalFrames_ValueChanged;
            // 
            // lblLowerFrameDuration
            // 
            lblLowerFrameDuration.AutoSize = true;
            lblLowerFrameDuration.Location = new System.Drawing.Point(8, 86);
            lblLowerFrameDuration.Margin = new Padding(4, 0, 4, 0);
            lblLowerFrameDuration.Name = "lblLowerFrameDuration";
            lblLowerFrameDuration.Size = new Size(116, 15);
            lblLowerFrameDuration.TabIndex = 10;
            lblLowerFrameDuration.Text = "Frame Duration (ms)";
            // 
            // nudLowerHorizontalFrames
            // 
            nudLowerHorizontalFrames.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudLowerHorizontalFrames.ForeColor = System.Drawing.Color.Gainsboro;
            nudLowerHorizontalFrames.Location = new System.Drawing.Point(96, 60);
            nudLowerHorizontalFrames.Margin = new Padding(4, 3, 4, 3);
            nudLowerHorizontalFrames.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            nudLowerHorizontalFrames.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudLowerHorizontalFrames.Name = "nudLowerHorizontalFrames";
            nudLowerHorizontalFrames.Size = new Size(80, 23);
            nudLowerHorizontalFrames.TabIndex = 21;
            nudLowerHorizontalFrames.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudLowerHorizontalFrames.ValueChanged += nudLowerHorizontalFrames_ValueChanged;
            // 
            // lblLowerFrameCount
            // 
            lblLowerFrameCount.AutoSize = true;
            lblLowerFrameCount.Location = new System.Drawing.Point(8, 42);
            lblLowerFrameCount.Margin = new Padding(4, 0, 4, 0);
            lblLowerFrameCount.Name = "lblLowerFrameCount";
            lblLowerFrameCount.Size = new Size(45, 15);
            lblLowerFrameCount.TabIndex = 8;
            lblLowerFrameCount.Text = "Frames";
            // 
            // lblLowerHorizontalFrames
            // 
            lblLowerHorizontalFrames.AutoSize = true;
            lblLowerHorizontalFrames.Location = new System.Drawing.Point(96, 42);
            lblLowerHorizontalFrames.Margin = new Padding(4, 0, 4, 0);
            lblLowerHorizontalFrames.Name = "lblLowerHorizontalFrames";
            lblLowerHorizontalFrames.Size = new Size(62, 15);
            lblLowerHorizontalFrames.TabIndex = 4;
            lblLowerHorizontalFrames.Text = "Horizontal";
            // 
            // lblLowerVerticalFrames
            // 
            lblLowerVerticalFrames.AutoSize = true;
            lblLowerVerticalFrames.Location = new System.Drawing.Point(184, 42);
            lblLowerVerticalFrames.Margin = new Padding(4, 0, 4, 0);
            lblLowerVerticalFrames.Name = "lblLowerVerticalFrames";
            lblLowerVerticalFrames.Size = new Size(45, 15);
            lblLowerVerticalFrames.TabIndex = 5;
            lblLowerVerticalFrames.Text = "Vertical";
            // 
            // lightEditorLower
            // 
            lightEditorLower.Location = new System.Drawing.Point(8, 268);
            lightEditorLower.Margin = new Padding(5, 6, 5, 6);
            lightEditorLower.Name = "lightEditorLower";
            lightEditorLower.Size = new Size(184, 192);
            lightEditorLower.TabIndex = 15;
            // 
            // grpLowerPlayback
            // 
            grpLowerPlayback.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpLowerPlayback.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpLowerPlayback.Controls.Add(btnPlayLower);
            grpLowerPlayback.Controls.Add(scrlLowerFrame);
            grpLowerPlayback.Controls.Add(lblLowerFrame);
            grpLowerPlayback.ForeColor = System.Drawing.Color.Gainsboro;
            grpLowerPlayback.Location = new System.Drawing.Point(208, 15);
            grpLowerPlayback.Margin = new Padding(4, 3, 4, 3);
            grpLowerPlayback.Name = "grpLowerPlayback";
            grpLowerPlayback.Padding = new Padding(4, 3, 4, 3);
            grpLowerPlayback.Size = new Size(272, 46);
            grpLowerPlayback.TabIndex = 16;
            grpLowerPlayback.TabStop = false;
            grpLowerPlayback.Text = "Playback";
            // 
            // btnPlayLower
            // 
            btnPlayLower.BackgroundImageLayout = ImageLayout.Stretch;
            btnPlayLower.ImageKey = "sharp_play_arrow_white_48dp.png";
            btnPlayLower.ImageList = imglstPlayPause;
            btnPlayLower.ImagePadding = 0;
            btnPlayLower.Location = new System.Drawing.Point(8, 18);
            btnPlayLower.Margin = new Padding(4, 3, 4, 3);
            btnPlayLower.Name = "btnPlayLower";
            btnPlayLower.Padding = new Padding(6);
            btnPlayLower.Size = new Size(20, 20);
            btnPlayLower.TabIndex = 16;
            btnPlayLower.Click += btnPlayLower_Click;
            // 
            // scrlLowerFrame
            // 
            scrlLowerFrame.Location = new System.Drawing.Point(36, 20);
            scrlLowerFrame.Margin = new Padding(4, 3, 4, 3);
            scrlLowerFrame.Maximum = 1;
            scrlLowerFrame.Minimum = 1;
            scrlLowerFrame.Name = "scrlLowerFrame";
            scrlLowerFrame.ScrollOrientation = DarkScrollOrientation.Horizontal;
            scrlLowerFrame.Size = new Size(168, 15);
            scrlLowerFrame.TabIndex = 15;
            scrlLowerFrame.Value = 1;
            scrlLowerFrame.ValueChanged += scrlLowerFrame_Scroll;
            // 
            // lblLowerFrame
            // 
            lblLowerFrame.AutoSize = true;
            lblLowerFrame.Location = new System.Drawing.Point(212, 20);
            lblLowerFrame.Margin = new Padding(4, 0, 4, 0);
            lblLowerFrame.Name = "lblLowerFrame";
            lblLowerFrame.Size = new Size(52, 15);
            lblLowerFrame.TabIndex = 14;
            lblLowerFrame.Text = "Frame: 1";
            // 
            // cmbLowerGraphic
            // 
            cmbLowerGraphic.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbLowerGraphic.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbLowerGraphic.BorderStyle = ButtonBorderStyle.Solid;
            cmbLowerGraphic.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbLowerGraphic.DrawDropdownHoverOutline = false;
            cmbLowerGraphic.DrawFocusRectangle = false;
            cmbLowerGraphic.DrawMode = DrawMode.OwnerDrawFixed;
            cmbLowerGraphic.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLowerGraphic.FlatStyle = FlatStyle.Flat;
            cmbLowerGraphic.ForeColor = System.Drawing.Color.Gainsboro;
            cmbLowerGraphic.FormattingEnabled = true;
            cmbLowerGraphic.Items.AddRange(new object[] { "//General/none" });
            cmbLowerGraphic.Location = new System.Drawing.Point(8, 235);
            cmbLowerGraphic.Margin = new Padding(4, 3, 4, 3);
            cmbLowerGraphic.Name = "cmbLowerGraphic";
            cmbLowerGraphic.Size = new Size(192, 24);
            cmbLowerGraphic.TabIndex = 3;
            cmbLowerGraphic.Text = "//General/none";
            cmbLowerGraphic.TextPadding = new Padding(2);
            cmbLowerGraphic.SelectedIndexChanged += cmbLowerGraphic_SelectedIndexChanged;
            // 
            // lblLowerGraphic
            // 
            lblLowerGraphic.AutoSize = true;
            lblLowerGraphic.Location = new System.Drawing.Point(8, 217);
            lblLowerGraphic.Margin = new Padding(4, 0, 4, 0);
            lblLowerGraphic.Name = "lblLowerGraphic";
            lblLowerGraphic.Size = new Size(54, 15);
            lblLowerGraphic.TabIndex = 1;
            lblLowerGraphic.Text = "Graphic: ";
            // 
            // picLowerAnimation
            // 
            picLowerAnimation.Location = new System.Drawing.Point(8, 22);
            picLowerAnimation.Margin = new Padding(4, 3, 4, 3);
            picLowerAnimation.Name = "picLowerAnimation";
            picLowerAnimation.Size = new Size(192, 192);
            picLowerAnimation.TabIndex = 0;
            picLowerAnimation.TabStop = false;
            // 
            // grpUpper
            // 
            grpUpper.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpUpper.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpUpper.Controls.Add(grpUpperExtraOptions);
            grpUpper.Controls.Add(lightEditorUpper);
            grpUpper.Controls.Add(grpUpperPlayback);
            grpUpper.Controls.Add(grpUpperFrameOpts);
            grpUpper.Controls.Add(cmbUpperGraphic);
            grpUpper.Controls.Add(lblUpperGraphic);
            grpUpper.Controls.Add(picUpperAnimation);
            grpUpper.ForeColor = System.Drawing.Color.Gainsboro;
            grpUpper.Location = new System.Drawing.Point(504, 102);
            grpUpper.Margin = new Padding(4, 3, 4, 3);
            grpUpper.Name = "grpUpper";
            grpUpper.Padding = new Padding(4, 3, 4, 3);
            grpUpper.Size = new Size(488, 468);
            grpUpper.TabIndex = 20;
            grpUpper.TabStop = false;
            grpUpper.Text = "Upper Layer (Above Target)";
            // 
            // grpUpperExtraOptions
            // 
            grpUpperExtraOptions.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpUpperExtraOptions.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpUpperExtraOptions.Controls.Add(chkDisableUpperRotations);
            grpUpperExtraOptions.Controls.Add(chkRenderBelowFringe);
            grpUpperExtraOptions.ForeColor = System.Drawing.Color.Gainsboro;
            grpUpperExtraOptions.Location = new System.Drawing.Point(208, 208);
            grpUpperExtraOptions.Margin = new Padding(4, 3, 4, 3);
            grpUpperExtraOptions.Name = "grpUpperExtraOptions";
            grpUpperExtraOptions.Padding = new Padding(4, 3, 4, 3);
            grpUpperExtraOptions.Size = new Size(272, 70);
            grpUpperExtraOptions.TabIndex = 29;
            grpUpperExtraOptions.TabStop = false;
            grpUpperExtraOptions.Text = "Extra Options:";
            // 
            // chkDisableUpperRotations
            // 
            chkDisableUpperRotations.AutoSize = true;
            chkDisableUpperRotations.Location = new System.Drawing.Point(8, 22);
            chkDisableUpperRotations.Margin = new Padding(4, 3, 4, 3);
            chkDisableUpperRotations.Name = "chkDisableUpperRotations";
            chkDisableUpperRotations.Size = new Size(117, 19);
            chkDisableUpperRotations.TabIndex = 27;
            chkDisableUpperRotations.Text = "Disable Rotations";
            chkDisableUpperRotations.CheckedChanged += chkDisableUpperRotations_CheckedChanged;
            // 
            // chkRenderBelowFringe
            // 
            chkRenderBelowFringe.AutoSize = true;
            chkRenderBelowFringe.Location = new System.Drawing.Point(8, 43);
            chkRenderBelowFringe.Margin = new Padding(4, 3, 4, 3);
            chkRenderBelowFringe.Name = "chkRenderBelowFringe";
            chkRenderBelowFringe.Size = new Size(134, 19);
            chkRenderBelowFringe.TabIndex = 31;
            chkRenderBelowFringe.Text = "Render Below Fringe";
            chkRenderBelowFringe.CheckedChanged += chkRenderBelowFringe_CheckedChanged;
            // 
            // lightEditorUpper
            // 
            lightEditorUpper.Location = new System.Drawing.Point(8, 268);
            lightEditorUpper.Margin = new Padding(5, 6, 5, 6);
            lightEditorUpper.Name = "lightEditorUpper";
            lightEditorUpper.Size = new Size(184, 192);
            lightEditorUpper.TabIndex = 15;
            // 
            // grpUpperPlayback
            // 
            grpUpperPlayback.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpUpperPlayback.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpUpperPlayback.Controls.Add(btnPlayUpper);
            grpUpperPlayback.Controls.Add(scrlUpperFrame);
            grpUpperPlayback.Controls.Add(lblUpperFrame);
            grpUpperPlayback.ForeColor = System.Drawing.Color.Gainsboro;
            grpUpperPlayback.Location = new System.Drawing.Point(208, 15);
            grpUpperPlayback.Margin = new Padding(4, 3, 4, 3);
            grpUpperPlayback.Name = "grpUpperPlayback";
            grpUpperPlayback.Padding = new Padding(4, 3, 4, 3);
            grpUpperPlayback.Size = new Size(272, 46);
            grpUpperPlayback.TabIndex = 18;
            grpUpperPlayback.TabStop = false;
            grpUpperPlayback.Text = "Playback";
            // 
            // btnPlayUpper
            // 
            btnPlayUpper.ImageKey = "sharp_pause_white_48dp.png";
            btnPlayUpper.ImageList = imglstPlayPause;
            btnPlayUpper.Location = new System.Drawing.Point(8, 18);
            btnPlayUpper.Margin = new Padding(4, 3, 4, 3);
            btnPlayUpper.Name = "btnPlayUpper";
            btnPlayUpper.Padding = new Padding(6);
            btnPlayUpper.Size = new Size(20, 20);
            btnPlayUpper.TabIndex = 16;
            btnPlayUpper.Click += btnPlayUpper_Click;
            // 
            // scrlUpperFrame
            // 
            scrlUpperFrame.Location = new System.Drawing.Point(36, 20);
            scrlUpperFrame.Margin = new Padding(4, 3, 4, 3);
            scrlUpperFrame.Maximum = 1;
            scrlUpperFrame.Minimum = 1;
            scrlUpperFrame.Name = "scrlUpperFrame";
            scrlUpperFrame.ScrollOrientation = DarkScrollOrientation.Horizontal;
            scrlUpperFrame.Size = new Size(168, 15);
            scrlUpperFrame.TabIndex = 15;
            scrlUpperFrame.Value = 1;
            scrlUpperFrame.ValueChanged += scrlUpperFrame_Scroll;
            // 
            // lblUpperFrame
            // 
            lblUpperFrame.AutoSize = true;
            lblUpperFrame.Location = new System.Drawing.Point(212, 20);
            lblUpperFrame.Margin = new Padding(4, 0, 4, 0);
            lblUpperFrame.Name = "lblUpperFrame";
            lblUpperFrame.Size = new Size(52, 15);
            lblUpperFrame.TabIndex = 14;
            lblUpperFrame.Text = "Frame: 1";
            // 
            // grpUpperFrameOpts
            // 
            grpUpperFrameOpts.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpUpperFrameOpts.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpUpperFrameOpts.Controls.Add(btnUpperClone);
            grpUpperFrameOpts.Controls.Add(nudUpperLoopCount);
            grpUpperFrameOpts.Controls.Add(lblUpperFrameCount);
            grpUpperFrameOpts.Controls.Add(nudUpperFrameDuration);
            grpUpperFrameOpts.Controls.Add(nudUpperFrameCount);
            grpUpperFrameOpts.Controls.Add(lblUpperLoopCount);
            grpUpperFrameOpts.Controls.Add(nudUpperVerticalFrames);
            grpUpperFrameOpts.Controls.Add(lblUpperHorizontalFrames);
            grpUpperFrameOpts.Controls.Add(nudUpperHorizontalFrames);
            grpUpperFrameOpts.Controls.Add(lblUpperFrameDuration);
            grpUpperFrameOpts.Controls.Add(lblUpperVerticalFrames);
            grpUpperFrameOpts.ForeColor = System.Drawing.Color.Gainsboro;
            grpUpperFrameOpts.Location = new System.Drawing.Point(208, 67);
            grpUpperFrameOpts.Margin = new Padding(4, 3, 4, 3);
            grpUpperFrameOpts.Name = "grpUpperFrameOpts";
            grpUpperFrameOpts.Padding = new Padding(4, 3, 4, 3);
            grpUpperFrameOpts.Size = new Size(272, 135);
            grpUpperFrameOpts.TabIndex = 19;
            grpUpperFrameOpts.TabStop = false;
            grpUpperFrameOpts.Text = "Frame Options";
            // 
            // btnUpperClone
            // 
            btnUpperClone.Location = new System.Drawing.Point(104, 15);
            btnUpperClone.Margin = new Padding(4, 3, 4, 3);
            btnUpperClone.Name = "btnUpperClone";
            btnUpperClone.Padding = new Padding(6);
            btnUpperClone.Size = new Size(160, 22);
            btnUpperClone.TabIndex = 17;
            btnUpperClone.Text = "Clone From Previous";
            btnUpperClone.Click += btnUpperClone_Click;
            // 
            // nudUpperLoopCount
            // 
            nudUpperLoopCount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudUpperLoopCount.ForeColor = System.Drawing.Color.Gainsboro;
            nudUpperLoopCount.Location = new System.Drawing.Point(174, 104);
            nudUpperLoopCount.Margin = new Padding(4, 3, 4, 3);
            nudUpperLoopCount.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudUpperLoopCount.Name = "nudUpperLoopCount";
            nudUpperLoopCount.Size = new Size(90, 23);
            nudUpperLoopCount.TabIndex = 30;
            nudUpperLoopCount.Value = new decimal(new int[] { 10, 0, 0, 0 });
            nudUpperLoopCount.ValueChanged += nudUpperLoopCount_ValueChanged;
            // 
            // lblUpperFrameCount
            // 
            lblUpperFrameCount.AutoSize = true;
            lblUpperFrameCount.Location = new System.Drawing.Point(8, 42);
            lblUpperFrameCount.Margin = new Padding(4, 0, 4, 0);
            lblUpperFrameCount.Name = "lblUpperFrameCount";
            lblUpperFrameCount.Size = new Size(45, 15);
            lblUpperFrameCount.TabIndex = 20;
            lblUpperFrameCount.Text = "Frames";
            // 
            // nudUpperFrameDuration
            // 
            nudUpperFrameDuration.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudUpperFrameDuration.ForeColor = System.Drawing.Color.Gainsboro;
            nudUpperFrameDuration.Location = new System.Drawing.Point(8, 104);
            nudUpperFrameDuration.Margin = new Padding(4, 3, 4, 3);
            nudUpperFrameDuration.Maximum = new decimal(new int[] { -10, 4, 0, 0 });
            nudUpperFrameDuration.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudUpperFrameDuration.Name = "nudUpperFrameDuration";
            nudUpperFrameDuration.Size = new Size(90, 23);
            nudUpperFrameDuration.TabIndex = 29;
            nudUpperFrameDuration.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudUpperFrameDuration.ValueChanged += nudUpperFrameDuration_ValueChanged;
            // 
            // nudUpperFrameCount
            // 
            nudUpperFrameCount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudUpperFrameCount.ForeColor = System.Drawing.Color.Gainsboro;
            nudUpperFrameCount.Location = new System.Drawing.Point(8, 60);
            nudUpperFrameCount.Margin = new Padding(4, 3, 4, 3);
            nudUpperFrameCount.Maximum = new decimal(new int[] { 65536, 0, 0, 0 });
            nudUpperFrameCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudUpperFrameCount.Name = "nudUpperFrameCount";
            nudUpperFrameCount.Size = new Size(80, 23);
            nudUpperFrameCount.TabIndex = 28;
            nudUpperFrameCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudUpperFrameCount.ValueChanged += nudUpperFrameCount_ValueChanged;
            // 
            // lblUpperLoopCount
            // 
            lblUpperLoopCount.AutoSize = true;
            lblUpperLoopCount.Location = new System.Drawing.Point(174, 86);
            lblUpperLoopCount.Margin = new Padding(4, 0, 4, 0);
            lblUpperLoopCount.Name = "lblUpperLoopCount";
            lblUpperLoopCount.Size = new Size(70, 15);
            lblUpperLoopCount.TabIndex = 24;
            lblUpperLoopCount.Text = "Loop Count";
            // 
            // nudUpperVerticalFrames
            // 
            nudUpperVerticalFrames.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudUpperVerticalFrames.ForeColor = System.Drawing.Color.Gainsboro;
            nudUpperVerticalFrames.Location = new System.Drawing.Point(184, 60);
            nudUpperVerticalFrames.Margin = new Padding(4, 3, 4, 3);
            nudUpperVerticalFrames.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            nudUpperVerticalFrames.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudUpperVerticalFrames.Name = "nudUpperVerticalFrames";
            nudUpperVerticalFrames.Size = new Size(80, 23);
            nudUpperVerticalFrames.TabIndex = 27;
            nudUpperVerticalFrames.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudUpperVerticalFrames.ValueChanged += nudUpperVerticalFrames_ValueChanged;
            // 
            // lblUpperHorizontalFrames
            // 
            lblUpperHorizontalFrames.AutoSize = true;
            lblUpperHorizontalFrames.Location = new System.Drawing.Point(96, 42);
            lblUpperHorizontalFrames.Margin = new Padding(4, 0, 4, 0);
            lblUpperHorizontalFrames.Name = "lblUpperHorizontalFrames";
            lblUpperHorizontalFrames.Size = new Size(62, 15);
            lblUpperHorizontalFrames.TabIndex = 16;
            lblUpperHorizontalFrames.Text = "Horizontal";
            // 
            // nudUpperHorizontalFrames
            // 
            nudUpperHorizontalFrames.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudUpperHorizontalFrames.ForeColor = System.Drawing.Color.Gainsboro;
            nudUpperHorizontalFrames.Location = new System.Drawing.Point(96, 60);
            nudUpperHorizontalFrames.Margin = new Padding(4, 3, 4, 3);
            nudUpperHorizontalFrames.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            nudUpperHorizontalFrames.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudUpperHorizontalFrames.Name = "nudUpperHorizontalFrames";
            nudUpperHorizontalFrames.Size = new Size(80, 23);
            nudUpperHorizontalFrames.TabIndex = 26;
            nudUpperHorizontalFrames.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudUpperHorizontalFrames.ValueChanged += nudUpperHorizontalFrames_ValueChanged;
            // 
            // lblUpperFrameDuration
            // 
            lblUpperFrameDuration.AutoSize = true;
            lblUpperFrameDuration.Location = new System.Drawing.Point(8, 86);
            lblUpperFrameDuration.Margin = new Padding(4, 0, 4, 0);
            lblUpperFrameDuration.Name = "lblUpperFrameDuration";
            lblUpperFrameDuration.Size = new Size(116, 15);
            lblUpperFrameDuration.TabIndex = 22;
            lblUpperFrameDuration.Text = "Frame Duration (ms)";
            // 
            // lblUpperVerticalFrames
            // 
            lblUpperVerticalFrames.AutoSize = true;
            lblUpperVerticalFrames.Location = new System.Drawing.Point(184, 42);
            lblUpperVerticalFrames.Margin = new Padding(4, 0, 4, 0);
            lblUpperVerticalFrames.Name = "lblUpperVerticalFrames";
            lblUpperVerticalFrames.Size = new Size(45, 15);
            lblUpperVerticalFrames.TabIndex = 17;
            lblUpperVerticalFrames.Text = "Vertical";
            // 
            // cmbUpperGraphic
            // 
            cmbUpperGraphic.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbUpperGraphic.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbUpperGraphic.BorderStyle = ButtonBorderStyle.Solid;
            cmbUpperGraphic.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbUpperGraphic.DrawDropdownHoverOutline = false;
            cmbUpperGraphic.DrawFocusRectangle = false;
            cmbUpperGraphic.DrawMode = DrawMode.OwnerDrawFixed;
            cmbUpperGraphic.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUpperGraphic.FlatStyle = FlatStyle.Flat;
            cmbUpperGraphic.ForeColor = System.Drawing.Color.Gainsboro;
            cmbUpperGraphic.FormattingEnabled = true;
            cmbUpperGraphic.Items.AddRange(new object[] { "//General/none" });
            cmbUpperGraphic.Location = new System.Drawing.Point(8, 235);
            cmbUpperGraphic.Margin = new Padding(4, 3, 4, 3);
            cmbUpperGraphic.Name = "cmbUpperGraphic";
            cmbUpperGraphic.Size = new Size(192, 24);
            cmbUpperGraphic.TabIndex = 15;
            cmbUpperGraphic.Text = "//General/none";
            cmbUpperGraphic.TextPadding = new Padding(2);
            cmbUpperGraphic.SelectedIndexChanged += cmbUpperGraphic_SelectedIndexChanged;
            // 
            // lblUpperGraphic
            // 
            lblUpperGraphic.AutoSize = true;
            lblUpperGraphic.Location = new System.Drawing.Point(8, 217);
            lblUpperGraphic.Margin = new Padding(4, 0, 4, 0);
            lblUpperGraphic.Name = "lblUpperGraphic";
            lblUpperGraphic.Size = new Size(54, 15);
            lblUpperGraphic.TabIndex = 14;
            lblUpperGraphic.Text = "Graphic: ";
            // 
            // picUpperAnimation
            // 
            picUpperAnimation.Location = new System.Drawing.Point(7, 22);
            picUpperAnimation.Margin = new Padding(4, 3, 4, 3);
            picUpperAnimation.Name = "picUpperAnimation";
            picUpperAnimation.Size = new Size(192, 192);
            picUpperAnimation.TabIndex = 1;
            picUpperAnimation.TabStop = false;
            // 
            // tmrUpperAnimation
            // 
            tmrUpperAnimation.Enabled = true;
            tmrUpperAnimation.Tick += tmrUpperAnimation_Tick;
            // 
            // tmrLowerAnimation
            // 
            tmrLowerAnimation.Enabled = true;
            tmrLowerAnimation.Tick += tmrLowerAnimation_Tick;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(848, 56);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(144, 44);
            btnCancel.TabIndex = 9;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(848, 7);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(144, 44);
            btnSave.TabIndex = 6;
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;
            // 
            // pnlContainer
            // 
            pnlContainer.Controls.Add(grpUpper);
            pnlContainer.Controls.Add(btnSave);
            pnlContainer.Controls.Add(btnCancel);
            pnlContainer.Controls.Add(grpGeneral);
            pnlContainer.Controls.Add(grpLower);
            pnlContainer.Location = new System.Drawing.Point(245, 30);
            pnlContainer.Margin = new Padding(4, 3, 4, 3);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(1000, 578);
            pnlContainer.TabIndex = 21;
            pnlContainer.Visible = false;
            // 
            // toolStrip
            // 
            toolStrip.AutoSize = false;
            toolStrip.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            toolStrip.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStrip.Items.AddRange(new ToolStripItem[] { toolStripItemNew, toolStripSeparator1, toolStripItemDelete, toolStripSeparator2, btnAlphabetical, toolStripSeparator4, toolStripItemCopy, toolStripItemPaste, toolStripSeparator3, toolStripItemUndo });
            toolStrip.Location = new System.Drawing.Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Padding = new Padding(6, 0, 1, 0);
            toolStrip.Size = new Size(1249, 29);
            toolStrip.TabIndex = 41;
            toolStrip.Text = "toolStrip1";
            // 
            // toolStripItemNew
            // 
            toolStripItemNew.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripItemNew.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripItemNew.Image = (Image)resources.GetObject("toolStripItemNew.Image");
            toolStripItemNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripItemNew.Name = "toolStripItemNew";
            toolStripItemNew.Size = new Size(23, 26);
            toolStripItemNew.Text = "New";
            toolStripItemNew.Click += toolStripItemNew_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripSeparator1.Margin = new Padding(0, 0, 2, 0);
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 29);
            // 
            // toolStripItemDelete
            // 
            toolStripItemDelete.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripItemDelete.Enabled = false;
            toolStripItemDelete.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripItemDelete.Image = (Image)resources.GetObject("toolStripItemDelete.Image");
            toolStripItemDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripItemDelete.Name = "toolStripItemDelete";
            toolStripItemDelete.Size = new Size(23, 26);
            toolStripItemDelete.Text = "Delete";
            toolStripItemDelete.Click += toolStripItemDelete_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripSeparator2.Margin = new Padding(0, 0, 2, 0);
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 29);
            // 
            // btnAlphabetical
            // 
            btnAlphabetical.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnAlphabetical.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            btnAlphabetical.Image = (Image)resources.GetObject("btnAlphabetical.Image");
            btnAlphabetical.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnAlphabetical.Name = "btnAlphabetical";
            btnAlphabetical.Size = new Size(23, 26);
            btnAlphabetical.Text = "Order Chronologically";
            btnAlphabetical.Click += btnAlphabetical_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripSeparator4.Margin = new Padding(0, 0, 2, 0);
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 29);
            // 
            // toolStripItemCopy
            // 
            toolStripItemCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripItemCopy.Enabled = false;
            toolStripItemCopy.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripItemCopy.Image = (Image)resources.GetObject("toolStripItemCopy.Image");
            toolStripItemCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripItemCopy.Name = "toolStripItemCopy";
            toolStripItemCopy.Size = new Size(23, 26);
            toolStripItemCopy.Text = "Copy";
            toolStripItemCopy.Click += toolStripItemCopy_Click;
            // 
            // toolStripItemPaste
            // 
            toolStripItemPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripItemPaste.Enabled = false;
            toolStripItemPaste.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripItemPaste.Image = (Image)resources.GetObject("toolStripItemPaste.Image");
            toolStripItemPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripItemPaste.Name = "toolStripItemPaste";
            toolStripItemPaste.Size = new Size(23, 26);
            toolStripItemPaste.Text = "Paste";
            toolStripItemPaste.Click += toolStripItemPaste_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripSeparator3.Margin = new Padding(0, 0, 2, 0);
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 29);
            // 
            // toolStripItemUndo
            // 
            toolStripItemUndo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripItemUndo.Enabled = false;
            toolStripItemUndo.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripItemUndo.Image = (Image)resources.GetObject("toolStripItemUndo.Image");
            toolStripItemUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripItemUndo.Name = "toolStripItemUndo";
            toolStripItemUndo.Size = new Size(23, 26);
            toolStripItemUndo.Text = "Undo";
            toolStripItemUndo.Click += toolStripItemUndo_Click;
            // 
            // tmrRender
            // 
            tmrRender.Enabled = true;
            tmrRender.Interval = 16;
            tmrRender.Tick += tmrRender_Tick;
            // 
            // FrmAnimation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            ClientSize = new Size(1249, 622);
            ControlBox = false;
            Controls.Add(toolStrip);
            Controls.Add(grpAnimations);
            Controls.Add(pnlContainer);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "FrmAnimation";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Animation Editor";
            FormClosed += frmAnimation_FormClosed;
            Load += frmAnimation_Load;
            KeyDown += form_KeyDown;
            grpAnimations.ResumeLayout(false);
            grpAnimations.PerformLayout();
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            grpLower.ResumeLayout(false);
            grpLower.PerformLayout();
            grpLowerExtraOptions.ResumeLayout(false);
            grpLowerExtraOptions.PerformLayout();
            grpLowerFrameOpts.ResumeLayout(false);
            grpLowerFrameOpts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudLowerLoopCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLowerFrameDuration).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLowerFrameCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLowerVerticalFrames).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLowerHorizontalFrames).EndInit();
            grpLowerPlayback.ResumeLayout(false);
            grpLowerPlayback.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLowerAnimation).EndInit();
            grpUpper.ResumeLayout(false);
            grpUpper.PerformLayout();
            grpUpperExtraOptions.ResumeLayout(false);
            grpUpperExtraOptions.PerformLayout();
            grpUpperPlayback.ResumeLayout(false);
            grpUpperPlayback.PerformLayout();
            grpUpperFrameOpts.ResumeLayout(false);
            grpUpperFrameOpts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudUpperLoopCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudUpperFrameDuration).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudUpperFrameCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudUpperVerticalFrames).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudUpperHorizontalFrames).EndInit();
            ((System.ComponentModel.ISupportInitialize)picUpperAnimation).EndInit();
            pnlContainer.ResumeLayout(false);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ImageList imglstPlayPause;
        private DarkGroupBox grpAnimations;
        private DarkGroupBox grpGeneral;
        private Label lblSound;
        private DarkComboBox cmbSound;
        private DarkButton btnPlaySound;
        private Label lblName;
        private DarkTextBox txtName;
        private DarkGroupBox grpLower;
        private Label lblLowerLoopCount;
        private Label lblLowerFrameDuration;
        private Label lblLowerFrameCount;
        private Label lblLowerVerticalFrames;
        private Label lblLowerHorizontalFrames;
        private DarkComboBox cmbLowerGraphic;
        private Label lblLowerGraphic;
        private PictureBox picLowerAnimation;
        private DarkGroupBox grpUpper;
        private Label lblUpperLoopCount;
        private Label lblUpperFrameDuration;
        private Label lblUpperFrameCount;
        private Label lblUpperVerticalFrames;
        private Label lblUpperHorizontalFrames;
        private DarkComboBox cmbUpperGraphic;
        private Label lblUpperGraphic;
        private PictureBox picUpperAnimation;
        private System.Windows.Forms.Timer tmrUpperAnimation;
        private System.Windows.Forms.Timer tmrLowerAnimation;
        private DarkGroupBox grpLowerPlayback;
        private DarkButton btnPlayLower;
        private DarkScrollBar scrlLowerFrame;
        private Label lblLowerFrame;
        private DarkGroupBox grpUpperPlayback;
        private DarkButton btnPlayUpper;
        private DarkScrollBar scrlUpperFrame;
        private Label lblUpperFrame;
        private DarkGroupBox grpUpperFrameOpts;
        private Controls.LightEditorCtrl lightEditorUpper;
        private DarkGroupBox grpLowerFrameOpts;
        private DarkButton btnLowerClone;
        private DarkButton btnUpperClone;
        public Controls.LightEditorCtrl lightEditorLower;
        private DarkScrollBar scrlDarkness;
        private Label labelDarkness;
        private DarkButton btnSwap;
        private DarkButton btnSave;
        private DarkButton btnCancel;
        private Panel pnlContainer;
        private DarkToolStrip toolStrip;
        private ToolStripButton toolStripItemNew;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripItemDelete;
        private ToolStripSeparator toolStripSeparator2;
        public ToolStripButton toolStripItemCopy;
        public ToolStripButton toolStripItemPaste;
        private ToolStripSeparator toolStripSeparator3;
        public ToolStripButton toolStripItemUndo;
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
        private Label lblFolder;
        private DarkComboBox cmbFolder;
        private DarkButton btnAddFolder;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton btnAlphabetical;
        private DarkButton btnClearSearch;
        private DarkTextBox txtSearch;
        private DarkCheckBox chkCompleteSoundPlayback;
        private DarkCheckBox chkLoopSoundDuringPreview;
        private Controls.GameObjectList lstGameObjects;
        private DarkGroupBox grpLowerExtraOptions;
        private DarkGroupBox grpUpperExtraOptions;
    }
}