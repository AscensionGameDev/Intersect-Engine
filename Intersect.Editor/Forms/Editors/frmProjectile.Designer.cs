using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
{
    partial class FrmProjectile
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmProjectile));
            grpProjectiles = new DarkGroupBox();
            btnClearSearch = new DarkButton();
            txtSearch = new DarkTextBox();
            lstGameObjects = new Controls.GameObjectList();
            grpSpawns = new DarkGroupBox();
            picSpawns = new PictureBox();
            grpProperties = new DarkGroupBox();
            btnAddFolder = new DarkButton();
            lblFolder = new Label();
            cmbFolder = new DarkComboBox();
            cmbSpell = new DarkComboBox();
            nudKnockback = new DarkNumericUpDown();
            nudRange = new DarkNumericUpDown();
            nudAmount = new DarkNumericUpDown();
            nudSpawn = new DarkNumericUpDown();
            nudSpeed = new DarkNumericUpDown();
            lblKnockback = new Label();
            lblAmount = new Label();
            lblSpell = new Label();
            lblName = new Label();
            txtName = new DarkTextBox();
            lblRange = new Label();
            lblSpawn = new Label();
            lblSpeed = new Label();
            grpAnimations = new DarkGroupBox();
            cmbAnimation = new DarkComboBox();
            btnRemove = new DarkButton();
            btnAdd = new DarkButton();
            chkRotation = new DarkCheckBox();
            lblAnimation = new Label();
            lstAnimations = new ListBox();
            lblSpawnRange = new Label();
            scrlSpawnRange = new DarkScrollBar();
            grpCollisions = new DarkGroupBox();
            chkPierce = new DarkCheckBox();
            chkIgnoreInactiveResources = new DarkCheckBox();
            chkIgnoreZDimensionBlocks = new DarkCheckBox();
            chkIgnoreMapBlocks = new DarkCheckBox();
            chkIgnoreActiveResources = new DarkCheckBox();
            pnlContainer = new Panel();
            grpTargettingOptions = new DarkGroupBox();
            rdoBehaviorDefault = new DarkRadioButton();
            rdoBehaviorDirectShot = new DarkRadioButton();
            rdoBehaviorHoming = new DarkRadioButton();
            grpGrappleOptions = new DarkGroupBox();
            chkGrappleOnNpc = new DarkCheckBox();
            chkGrappleOnResource = new DarkCheckBox();
            chkGrappleOnMap = new DarkCheckBox();
            chkGrappleOnPlayer = new DarkCheckBox();
            grpAmmo = new DarkGroupBox();
            nudConsume = new DarkNumericUpDown();
            cmbItem = new DarkComboBox();
            lblAmmoItem = new Label();
            lblAmmoAmount = new Label();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
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
            grpProjectiles.SuspendLayout();
            grpSpawns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picSpawns).BeginInit();
            grpProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudKnockback).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudSpawn).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudSpeed).BeginInit();
            grpAnimations.SuspendLayout();
            grpCollisions.SuspendLayout();
            pnlContainer.SuspendLayout();
            grpTargettingOptions.SuspendLayout();
            grpGrappleOptions.SuspendLayout();
            grpAmmo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudConsume).BeginInit();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // grpProjectiles
            // 
            grpProjectiles.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpProjectiles.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpProjectiles.Controls.Add(btnClearSearch);
            grpProjectiles.Controls.Add(txtSearch);
            grpProjectiles.Controls.Add(lstGameObjects);
            grpProjectiles.ForeColor = System.Drawing.Color.Gainsboro;
            grpProjectiles.Location = new System.Drawing.Point(14, 42);
            grpProjectiles.Margin = new Padding(4, 3, 4, 3);
            grpProjectiles.Name = "grpProjectiles";
            grpProjectiles.Padding = new Padding(4, 3, 4, 3);
            grpProjectiles.Size = new Size(237, 642);
            grpProjectiles.TabIndex = 15;
            grpProjectiles.TabStop = false;
            grpProjectiles.Text = "Projectiles";
            // 
            // btnClearSearch
            // 
            btnClearSearch.Location = new System.Drawing.Point(209, 21);
            btnClearSearch.Margin = new Padding(4, 3, 4, 3);
            btnClearSearch.Name = "btnClearSearch";
            btnClearSearch.Padding = new Padding(6);
            btnClearSearch.Size = new Size(21, 23);
            btnClearSearch.TabIndex = 34;
            btnClearSearch.Text = "X";
            btnClearSearch.Click += btnClearSearch_Click;
            // 
            // txtSearch
            // 
            txtSearch.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtSearch.Location = new System.Drawing.Point(7, 21);
            txtSearch.Margin = new Padding(4, 3, 4, 3);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(194, 23);
            txtSearch.TabIndex = 33;
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
            lstGameObjects.Location = new System.Drawing.Point(7, 51);
            lstGameObjects.Margin = new Padding(4, 3, 4, 3);
            lstGameObjects.Name = "lstGameObjects";
            lstGameObjects.SelectedImageIndex = 0;
            lstGameObjects.Size = new Size(223, 585);
            lstGameObjects.TabIndex = 32;
            // 
            // grpSpawns
            // 
            grpSpawns.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpSpawns.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpSpawns.Controls.Add(picSpawns);
            grpSpawns.ForeColor = System.Drawing.Color.Gainsboro;
            grpSpawns.Location = new System.Drawing.Point(2, 423);
            grpSpawns.Margin = new Padding(4, 3, 4, 3);
            grpSpawns.Name = "grpSpawns";
            grpSpawns.Padding = new Padding(4, 3, 4, 3);
            grpSpawns.Size = new Size(217, 212);
            grpSpawns.TabIndex = 17;
            grpSpawns.TabStop = false;
            grpSpawns.Text = "Projectile Spawns";
            // 
            // picSpawns
            // 
            picSpawns.BackgroundImageLayout = ImageLayout.Stretch;
            picSpawns.Location = new System.Drawing.Point(18, 22);
            picSpawns.Margin = new Padding(4, 3, 4, 3);
            picSpawns.Name = "picSpawns";
            picSpawns.Size = new Size(187, 185);
            picSpawns.TabIndex = 17;
            picSpawns.TabStop = false;
            picSpawns.MouseDown += picSpawns_MouseDown;
            // 
            // grpProperties
            // 
            grpProperties.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpProperties.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpProperties.Controls.Add(btnAddFolder);
            grpProperties.Controls.Add(lblFolder);
            grpProperties.Controls.Add(cmbFolder);
            grpProperties.Controls.Add(cmbSpell);
            grpProperties.Controls.Add(nudKnockback);
            grpProperties.Controls.Add(nudRange);
            grpProperties.Controls.Add(nudAmount);
            grpProperties.Controls.Add(nudSpawn);
            grpProperties.Controls.Add(nudSpeed);
            grpProperties.Controls.Add(lblKnockback);
            grpProperties.Controls.Add(lblAmount);
            grpProperties.Controls.Add(lblSpell);
            grpProperties.Controls.Add(lblName);
            grpProperties.Controls.Add(txtName);
            grpProperties.Controls.Add(lblRange);
            grpProperties.Controls.Add(lblSpawn);
            grpProperties.Controls.Add(lblSpeed);
            grpProperties.ForeColor = System.Drawing.Color.Gainsboro;
            grpProperties.Location = new System.Drawing.Point(2, 1);
            grpProperties.Margin = new Padding(4, 3, 4, 3);
            grpProperties.Name = "grpProperties";
            grpProperties.Padding = new Padding(4, 3, 4, 3);
            grpProperties.Size = new Size(216, 314);
            grpProperties.TabIndex = 18;
            grpProperties.TabStop = false;
            grpProperties.Text = "Properties";
            // 
            // btnAddFolder
            // 
            btnAddFolder.Location = new System.Drawing.Point(183, 53);
            btnAddFolder.Margin = new Padding(4, 3, 4, 3);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Padding = new Padding(6);
            btnAddFolder.Size = new Size(21, 24);
            btnAddFolder.TabIndex = 52;
            btnAddFolder.Text = "+";
            btnAddFolder.Click += btnAddFolder_Click;
            // 
            // lblFolder
            // 
            lblFolder.AutoSize = true;
            lblFolder.Location = new System.Drawing.Point(7, 57);
            lblFolder.Margin = new Padding(4, 0, 4, 0);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new Size(43, 15);
            lblFolder.TabIndex = 51;
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
            cmbFolder.Location = new System.Drawing.Point(66, 53);
            cmbFolder.Margin = new Padding(4, 3, 4, 3);
            cmbFolder.Name = "cmbFolder";
            cmbFolder.Size = new Size(109, 24);
            cmbFolder.TabIndex = 50;
            cmbFolder.Text = null;
            cmbFolder.TextPadding = new Padding(2);
            cmbFolder.SelectedIndexChanged += cmbFolder_SelectedIndexChanged;
            // 
            // cmbSpell
            // 
            cmbSpell.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbSpell.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbSpell.BorderStyle = ButtonBorderStyle.Solid;
            cmbSpell.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbSpell.DrawDropdownHoverOutline = false;
            cmbSpell.DrawFocusRectangle = false;
            cmbSpell.DrawMode = DrawMode.OwnerDrawFixed;
            cmbSpell.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSpell.FlatStyle = FlatStyle.Flat;
            cmbSpell.ForeColor = System.Drawing.Color.Gainsboro;
            cmbSpell.FormattingEnabled = true;
            cmbSpell.Location = new System.Drawing.Point(10, 275);
            cmbSpell.Margin = new Padding(4, 3, 4, 3);
            cmbSpell.Name = "cmbSpell";
            cmbSpell.Size = new Size(194, 24);
            cmbSpell.TabIndex = 46;
            cmbSpell.Text = null;
            cmbSpell.TextPadding = new Padding(2);
            cmbSpell.SelectedIndexChanged += cmbSpell_SelectedIndexChanged;
            // 
            // nudKnockback
            // 
            nudKnockback.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudKnockback.ForeColor = System.Drawing.Color.Gainsboro;
            nudKnockback.Location = new System.Drawing.Point(125, 216);
            nudKnockback.Margin = new Padding(4, 3, 4, 3);
            nudKnockback.Name = "nudKnockback";
            nudKnockback.Size = new Size(80, 23);
            nudKnockback.TabIndex = 45;
            nudKnockback.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudKnockback.ValueChanged += nudKnockback_ValueChanged;
            // 
            // nudRange
            // 
            nudRange.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudRange.ForeColor = System.Drawing.Color.Gainsboro;
            nudRange.Location = new System.Drawing.Point(125, 186);
            nudRange.Margin = new Padding(4, 3, 4, 3);
            nudRange.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudRange.Name = "nudRange";
            nudRange.Size = new Size(80, 23);
            nudRange.TabIndex = 44;
            nudRange.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudRange.ValueChanged += nudRange_ValueChanged;
            // 
            // nudAmount
            // 
            nudAmount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudAmount.ForeColor = System.Drawing.Color.Gainsboro;
            nudAmount.Location = new System.Drawing.Point(125, 156);
            nudAmount.Margin = new Padding(4, 3, 4, 3);
            nudAmount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudAmount.Name = "nudAmount";
            nudAmount.Size = new Size(80, 23);
            nudAmount.TabIndex = 43;
            nudAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudAmount.ValueChanged += nudAmount_ValueChanged;
            // 
            // nudSpawn
            // 
            nudSpawn.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudSpawn.ForeColor = System.Drawing.Color.Gainsboro;
            nudSpawn.Location = new System.Drawing.Point(125, 125);
            nudSpawn.Margin = new Padding(4, 3, 4, 3);
            nudSpawn.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudSpawn.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudSpawn.Name = "nudSpawn";
            nudSpawn.Size = new Size(80, 23);
            nudSpawn.TabIndex = 42;
            nudSpawn.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudSpawn.ValueChanged += nudSpawnDelay_ValueChanged;
            // 
            // nudSpeed
            // 
            nudSpeed.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudSpeed.ForeColor = System.Drawing.Color.Gainsboro;
            nudSpeed.Location = new System.Drawing.Point(125, 95);
            nudSpeed.Margin = new Padding(4, 3, 4, 3);
            nudSpeed.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudSpeed.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudSpeed.Name = "nudSpeed";
            nudSpeed.Size = new Size(80, 23);
            nudSpeed.TabIndex = 41;
            nudSpeed.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudSpeed.ValueChanged += nudSpeed_ValueChanged;
            // 
            // lblKnockback
            // 
            lblKnockback.AutoSize = true;
            lblKnockback.Location = new System.Drawing.Point(7, 216);
            lblKnockback.Margin = new Padding(4, 0, 4, 0);
            lblKnockback.Name = "lblKnockback";
            lblKnockback.Size = new Size(68, 15);
            lblKnockback.TabIndex = 40;
            lblKnockback.Text = "Knockback:";
            // 
            // lblAmount
            // 
            lblAmount.AutoSize = true;
            lblAmount.Location = new System.Drawing.Point(7, 156);
            lblAmount.Margin = new Padding(4, 0, 4, 0);
            lblAmount.Name = "lblAmount";
            lblAmount.Size = new Size(56, 15);
            lblAmount.TabIndex = 38;
            lblAmount.Text = "Quantity:";
            // 
            // lblSpell
            // 
            lblSpell.AutoSize = true;
            lblSpell.Location = new System.Drawing.Point(7, 249);
            lblSpell.Margin = new Padding(4, 0, 4, 0);
            lblSpell.Name = "lblSpell";
            lblSpell.Size = new Size(84, 15);
            lblSpell.TabIndex = 24;
            lblSpell.Text = "Collision Spell:";
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(7, 24);
            lblName.Margin = new Padding(4, 0, 4, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(42, 15);
            lblName.TabIndex = 19;
            lblName.Text = "Name:";
            // 
            // txtName
            // 
            txtName.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtName.Location = new System.Drawing.Point(66, 22);
            txtName.Margin = new Padding(4, 3, 4, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(137, 23);
            txtName.TabIndex = 18;
            txtName.TextChanged += txtName_TextChanged;
            // 
            // lblRange
            // 
            lblRange.AutoSize = true;
            lblRange.Location = new System.Drawing.Point(7, 188);
            lblRange.Margin = new Padding(4, 0, 4, 0);
            lblRange.Name = "lblRange";
            lblRange.Size = new Size(43, 15);
            lblRange.TabIndex = 7;
            lblRange.Text = "Range:";
            // 
            // lblSpawn
            // 
            lblSpawn.AutoSize = true;
            lblSpawn.Location = new System.Drawing.Point(7, 127);
            lblSpawn.Margin = new Padding(4, 0, 4, 0);
            lblSpawn.Name = "lblSpawn";
            lblSpawn.Size = new Size(104, 15);
            lblSpawn.TabIndex = 4;
            lblSpawn.Text = "Spawn Delay (ms):";
            // 
            // lblSpeed
            // 
            lblSpeed.AutoSize = true;
            lblSpeed.Location = new System.Drawing.Point(7, 97);
            lblSpeed.Margin = new Padding(4, 0, 4, 0);
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Size = new Size(69, 15);
            lblSpeed.TabIndex = 3;
            lblSpeed.Text = "Speed (ms):";
            // 
            // grpAnimations
            // 
            grpAnimations.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpAnimations.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpAnimations.Controls.Add(cmbAnimation);
            grpAnimations.Controls.Add(btnRemove);
            grpAnimations.Controls.Add(btnAdd);
            grpAnimations.Controls.Add(chkRotation);
            grpAnimations.Controls.Add(lblAnimation);
            grpAnimations.Controls.Add(lstAnimations);
            grpAnimations.Controls.Add(lblSpawnRange);
            grpAnimations.Controls.Add(scrlSpawnRange);
            grpAnimations.ForeColor = System.Drawing.Color.Gainsboro;
            grpAnimations.Location = new System.Drawing.Point(225, 1);
            grpAnimations.Margin = new Padding(4, 3, 4, 3);
            grpAnimations.Name = "grpAnimations";
            grpAnimations.Padding = new Padding(4, 3, 4, 3);
            grpAnimations.Size = new Size(318, 291);
            grpAnimations.TabIndex = 27;
            grpAnimations.TabStop = false;
            grpAnimations.Text = "Animations";
            // 
            // cmbAnimation
            // 
            cmbAnimation.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbAnimation.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbAnimation.BorderStyle = ButtonBorderStyle.Solid;
            cmbAnimation.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbAnimation.DrawDropdownHoverOutline = false;
            cmbAnimation.DrawFocusRectangle = false;
            cmbAnimation.DrawMode = DrawMode.OwnerDrawFixed;
            cmbAnimation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAnimation.FlatStyle = FlatStyle.Flat;
            cmbAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            cmbAnimation.FormattingEnabled = true;
            cmbAnimation.Location = new System.Drawing.Point(83, 160);
            cmbAnimation.Margin = new Padding(4, 3, 4, 3);
            cmbAnimation.Name = "cmbAnimation";
            cmbAnimation.Size = new Size(223, 24);
            cmbAnimation.TabIndex = 39;
            cmbAnimation.Text = null;
            cmbAnimation.TextPadding = new Padding(2);
            cmbAnimation.SelectedIndexChanged += cmbAnimation_SelectedIndexChanged;
            // 
            // btnRemove
            // 
            btnRemove.Location = new System.Drawing.Point(219, 253);
            btnRemove.Margin = new Padding(4, 3, 4, 3);
            btnRemove.Name = "btnRemove";
            btnRemove.Padding = new Padding(6);
            btnRemove.Size = new Size(88, 27);
            btnRemove.TabIndex = 38;
            btnRemove.Text = "Remove";
            btnRemove.Click += btnRemove_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new System.Drawing.Point(14, 253);
            btnAdd.Margin = new Padding(4, 3, 4, 3);
            btnAdd.Name = "btnAdd";
            btnAdd.Padding = new Padding(6);
            btnAdd.Size = new Size(88, 27);
            btnAdd.TabIndex = 37;
            btnAdd.Text = "Add";
            btnAdd.Click += btnAdd_Click;
            // 
            // chkRotation
            // 
            chkRotation.AutoSize = true;
            chkRotation.Location = new System.Drawing.Point(14, 226);
            chkRotation.Margin = new Padding(4, 3, 4, 3);
            chkRotation.Name = "chkRotation";
            chkRotation.Size = new Size(153, 19);
            chkRotation.TabIndex = 36;
            chkRotation.Text = "Auto Rotate Animation?";
            chkRotation.CheckedChanged += chkRotation_CheckedChanged;
            // 
            // lblAnimation
            // 
            lblAnimation.AutoSize = true;
            lblAnimation.Location = new System.Drawing.Point(10, 160);
            lblAnimation.Margin = new Padding(4, 0, 4, 0);
            lblAnimation.Name = "lblAnimation";
            lblAnimation.Size = new Size(66, 15);
            lblAnimation.TabIndex = 31;
            lblAnimation.Text = "Animation:";
            // 
            // lstAnimations
            // 
            lstAnimations.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstAnimations.BorderStyle = BorderStyle.FixedSingle;
            lstAnimations.ForeColor = System.Drawing.Color.Gainsboro;
            lstAnimations.FormattingEnabled = true;
            lstAnimations.ItemHeight = 15;
            lstAnimations.Items.AddRange(new object[] { "[Spawn Range: 1 - 1] Animation: None" });
            lstAnimations.Location = new System.Drawing.Point(14, 20);
            lstAnimations.Margin = new Padding(4, 3, 4, 3);
            lstAnimations.Name = "lstAnimations";
            lstAnimations.Size = new Size(292, 137);
            lstAnimations.TabIndex = 29;
            lstAnimations.Click += lstAnimations_Click;
            // 
            // lblSpawnRange
            // 
            lblSpawnRange.AutoSize = true;
            lblSpawnRange.Location = new System.Drawing.Point(10, 193);
            lblSpawnRange.Margin = new Padding(4, 0, 4, 0);
            lblSpawnRange.Name = "lblSpawnRange";
            lblSpawnRange.Size = new Size(107, 15);
            lblSpawnRange.TabIndex = 28;
            lblSpawnRange.Text = "Spawn Range: 1 - 1";
            // 
            // scrlSpawnRange
            // 
            scrlSpawnRange.Location = new System.Drawing.Point(14, 208);
            scrlSpawnRange.Margin = new Padding(4, 3, 4, 3);
            scrlSpawnRange.Minimum = 1;
            scrlSpawnRange.Name = "scrlSpawnRange";
            scrlSpawnRange.ScrollOrientation = DarkScrollOrientation.Horizontal;
            scrlSpawnRange.Size = new Size(293, 20);
            scrlSpawnRange.TabIndex = 27;
            scrlSpawnRange.Value = 1;
            scrlSpawnRange.ValueChanged += scrlSpawnRange_Scroll;
            // 
            // grpCollisions
            // 
            grpCollisions.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpCollisions.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpCollisions.Controls.Add(chkPierce);
            grpCollisions.Controls.Add(chkIgnoreInactiveResources);
            grpCollisions.Controls.Add(chkIgnoreZDimensionBlocks);
            grpCollisions.Controls.Add(chkIgnoreMapBlocks);
            grpCollisions.Controls.Add(chkIgnoreActiveResources);
            grpCollisions.ForeColor = System.Drawing.Color.Gainsboro;
            grpCollisions.Location = new System.Drawing.Point(386, 299);
            grpCollisions.Margin = new Padding(4, 3, 4, 3);
            grpCollisions.Name = "grpCollisions";
            grpCollisions.Padding = new Padding(4, 3, 4, 3);
            grpCollisions.Size = new Size(158, 152);
            grpCollisions.TabIndex = 29;
            grpCollisions.TabStop = false;
            grpCollisions.Text = "Ignore Collision:";
            // 
            // chkPierce
            // 
            chkPierce.AutoSize = true;
            chkPierce.Location = new System.Drawing.Point(7, 125);
            chkPierce.Margin = new Padding(4, 3, 4, 3);
            chkPierce.Name = "chkPierce";
            chkPierce.RightToLeft = RightToLeft.No;
            chkPierce.Size = new Size(98, 19);
            chkPierce.TabIndex = 39;
            chkPierce.Text = "Pierce Target?";
            chkPierce.CheckedChanged += chkPierce_CheckedChanged;
            // 
            // chkIgnoreInactiveResources
            // 
            chkIgnoreInactiveResources.AutoSize = true;
            chkIgnoreInactiveResources.Location = new System.Drawing.Point(7, 72);
            chkIgnoreInactiveResources.Margin = new Padding(4, 3, 4, 3);
            chkIgnoreInactiveResources.Name = "chkIgnoreInactiveResources";
            chkIgnoreInactiveResources.Size = new Size(123, 19);
            chkIgnoreInactiveResources.TabIndex = 38;
            chkIgnoreInactiveResources.Text = "Inactive Resources";
            chkIgnoreInactiveResources.CheckedChanged += chkIgnoreInactiveResources_CheckedChanged;
            // 
            // chkIgnoreZDimensionBlocks
            // 
            chkIgnoreZDimensionBlocks.AutoSize = true;
            chkIgnoreZDimensionBlocks.Location = new System.Drawing.Point(7, 98);
            chkIgnoreZDimensionBlocks.Margin = new Padding(4, 3, 4, 3);
            chkIgnoreZDimensionBlocks.Name = "chkIgnoreZDimensionBlocks";
            chkIgnoreZDimensionBlocks.Size = new Size(132, 19);
            chkIgnoreZDimensionBlocks.TabIndex = 37;
            chkIgnoreZDimensionBlocks.Text = "Z-Dimension Blocks";
            chkIgnoreZDimensionBlocks.CheckedChanged += chkIgnoreZDimensionBlocks_CheckedChanged;
            // 
            // chkIgnoreMapBlocks
            // 
            chkIgnoreMapBlocks.AutoSize = true;
            chkIgnoreMapBlocks.Location = new System.Drawing.Point(7, 18);
            chkIgnoreMapBlocks.Margin = new Padding(4, 3, 4, 3);
            chkIgnoreMapBlocks.Name = "chkIgnoreMapBlocks";
            chkIgnoreMapBlocks.Size = new Size(87, 19);
            chkIgnoreMapBlocks.TabIndex = 33;
            chkIgnoreMapBlocks.Text = "Map Blocks";
            chkIgnoreMapBlocks.CheckedChanged += chkIgnoreMapBlocks_CheckedChanged;
            // 
            // chkIgnoreActiveResources
            // 
            chkIgnoreActiveResources.AutoSize = true;
            chkIgnoreActiveResources.Location = new System.Drawing.Point(7, 45);
            chkIgnoreActiveResources.Margin = new Padding(4, 3, 4, 3);
            chkIgnoreActiveResources.Name = "chkIgnoreActiveResources";
            chkIgnoreActiveResources.Size = new Size(115, 19);
            chkIgnoreActiveResources.TabIndex = 36;
            chkIgnoreActiveResources.Text = "Active Resources";
            chkIgnoreActiveResources.CheckedChanged += chkIgnoreActiveResources_CheckedChanged;
            // 
            // pnlContainer
            // 
            pnlContainer.AutoScroll = true;
            pnlContainer.Controls.Add(grpTargettingOptions);
            pnlContainer.Controls.Add(grpGrappleOptions);
            pnlContainer.Controls.Add(grpAmmo);
            pnlContainer.Controls.Add(grpCollisions);
            pnlContainer.Controls.Add(grpProperties);
            pnlContainer.Controls.Add(grpAnimations);
            pnlContainer.Controls.Add(grpSpawns);
            pnlContainer.Location = new System.Drawing.Point(258, 42);
            pnlContainer.Margin = new Padding(4, 3, 4, 3);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(553, 642);
            pnlContainer.TabIndex = 30;
            pnlContainer.Visible = false;
            // 
            // grpTargettingOptions
            // 
            grpTargettingOptions.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpTargettingOptions.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpTargettingOptions.Controls.Add(rdoBehaviorDefault);
            grpTargettingOptions.Controls.Add(rdoBehaviorDirectShot);
            grpTargettingOptions.Controls.Add(rdoBehaviorHoming);
            grpTargettingOptions.ForeColor = System.Drawing.Color.Gainsboro;
            grpTargettingOptions.Location = new System.Drawing.Point(2, 317);
            grpTargettingOptions.Margin = new Padding(4, 3, 4, 3);
            grpTargettingOptions.Name = "grpTargettingOptions";
            grpTargettingOptions.Padding = new Padding(4, 3, 4, 3);
            grpTargettingOptions.Size = new Size(216, 101);
            grpTargettingOptions.TabIndex = 41;
            grpTargettingOptions.TabStop = false;
            grpTargettingOptions.Text = "Targeting Options:";
            // 
            // rdoBehaviorDefault
            // 
            rdoBehaviorDefault.AutoSize = true;
            rdoBehaviorDefault.Checked = true;
            rdoBehaviorDefault.Location = new System.Drawing.Point(8, 73);
            rdoBehaviorDefault.Margin = new Padding(4, 3, 4, 3);
            rdoBehaviorDefault.Name = "rdoBehaviorDefault";
            rdoBehaviorDefault.Size = new Size(112, 19);
            rdoBehaviorDefault.TabIndex = 38;
            rdoBehaviorDefault.TabStop = true;
            rdoBehaviorDefault.Text = "Default Behavior";
            rdoBehaviorDefault.CheckedChanged += rdoBehaviorDefault_CheckedChanged;
            // 
            // rdoBehaviorDirectShot
            // 
            rdoBehaviorDirectShot.AutoSize = true;
            rdoBehaviorDirectShot.Location = new System.Drawing.Point(8, 47);
            rdoBehaviorDirectShot.Margin = new Padding(4, 3, 4, 3);
            rdoBehaviorDirectShot.Name = "rdoBehaviorDirectShot";
            rdoBehaviorDirectShot.Size = new Size(132, 19);
            rdoBehaviorDirectShot.TabIndex = 37;
            rdoBehaviorDirectShot.Text = "Direct Shot Behavior";
            rdoBehaviorDirectShot.CheckedChanged += rdoBehaviorDirectShot_CheckedChanged;
            // 
            // rdoBehaviorHoming
            // 
            rdoBehaviorHoming.AutoSize = true;
            rdoBehaviorHoming.Location = new System.Drawing.Point(8, 21);
            rdoBehaviorHoming.Margin = new Padding(4, 3, 4, 3);
            rdoBehaviorHoming.Name = "rdoBehaviorHoming";
            rdoBehaviorHoming.Size = new Size(118, 19);
            rdoBehaviorHoming.TabIndex = 36;
            rdoBehaviorHoming.Text = "Homing Behavior";
            rdoBehaviorHoming.CheckedChanged += rdoBehaviorHoming_CheckedChanged;
            // 
            // grpGrappleOptions
            // 
            grpGrappleOptions.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpGrappleOptions.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpGrappleOptions.Controls.Add(chkGrappleOnNpc);
            grpGrappleOptions.Controls.Add(chkGrappleOnResource);
            grpGrappleOptions.Controls.Add(chkGrappleOnMap);
            grpGrappleOptions.Controls.Add(chkGrappleOnPlayer);
            grpGrappleOptions.ForeColor = System.Drawing.Color.Gainsboro;
            grpGrappleOptions.Location = new System.Drawing.Point(225, 299);
            grpGrappleOptions.Margin = new Padding(4, 3, 4, 3);
            grpGrappleOptions.Name = "grpGrappleOptions";
            grpGrappleOptions.Padding = new Padding(4, 3, 4, 3);
            grpGrappleOptions.Size = new Size(152, 152);
            grpGrappleOptions.TabIndex = 40;
            grpGrappleOptions.TabStop = false;
            grpGrappleOptions.Text = "Grapple Options:";
            // 
            // chkGrappleOnNpc
            // 
            chkGrappleOnNpc.AutoSize = true;
            chkGrappleOnNpc.Location = new System.Drawing.Point(10, 76);
            chkGrappleOnNpc.Margin = new Padding(4, 3, 4, 3);
            chkGrappleOnNpc.Name = "chkGrappleOnNpc";
            chkGrappleOnNpc.Size = new Size(69, 19);
            chkGrappleOnNpc.TabIndex = 38;
            chkGrappleOnNpc.Text = "On NPC";
            chkGrappleOnNpc.CheckedChanged += chkGrappleOnNpc_CheckedChanged;
            // 
            // chkGrappleOnResource
            // 
            chkGrappleOnResource.AutoSize = true;
            chkGrappleOnResource.Location = new System.Drawing.Point(10, 103);
            chkGrappleOnResource.Margin = new Padding(4, 3, 4, 3);
            chkGrappleOnResource.Name = "chkGrappleOnResource";
            chkGrappleOnResource.Size = new Size(93, 19);
            chkGrappleOnResource.TabIndex = 37;
            chkGrappleOnResource.Text = "On Resource";
            chkGrappleOnResource.CheckedChanged += chkGrappleOnResource_CheckedChanged;
            // 
            // chkGrappleOnMap
            // 
            chkGrappleOnMap.AutoSize = true;
            chkGrappleOnMap.Location = new System.Drawing.Point(10, 23);
            chkGrappleOnMap.Margin = new Padding(4, 3, 4, 3);
            chkGrappleOnMap.Name = "chkGrappleOnMap";
            chkGrappleOnMap.Size = new Size(119, 19);
            chkGrappleOnMap.TabIndex = 33;
            chkGrappleOnMap.Text = "On Map Attribute";
            chkGrappleOnMap.CheckedChanged += chkGrappleOnMap_CheckedChanged;
            // 
            // chkGrappleOnPlayer
            // 
            chkGrappleOnPlayer.AutoSize = true;
            chkGrappleOnPlayer.Location = new System.Drawing.Point(10, 50);
            chkGrappleOnPlayer.Margin = new Padding(4, 3, 4, 3);
            chkGrappleOnPlayer.Name = "chkGrappleOnPlayer";
            chkGrappleOnPlayer.Size = new Size(77, 19);
            chkGrappleOnPlayer.TabIndex = 36;
            chkGrappleOnPlayer.Text = "On Player";
            chkGrappleOnPlayer.CheckedChanged += chkGrappleOnPlayer_CheckedChanged;
            // 
            // grpAmmo
            // 
            grpAmmo.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpAmmo.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpAmmo.Controls.Add(nudConsume);
            grpAmmo.Controls.Add(cmbItem);
            grpAmmo.Controls.Add(lblAmmoItem);
            grpAmmo.Controls.Add(lblAmmoAmount);
            grpAmmo.ForeColor = System.Drawing.Color.Gainsboro;
            grpAmmo.Location = new System.Drawing.Point(225, 458);
            grpAmmo.Margin = new Padding(4, 3, 4, 3);
            grpAmmo.Name = "grpAmmo";
            grpAmmo.Padding = new Padding(4, 3, 4, 3);
            grpAmmo.Size = new Size(317, 91);
            grpAmmo.TabIndex = 30;
            grpAmmo.TabStop = false;
            grpAmmo.Text = "Ammunition Requirements: ";
            // 
            // nudConsume
            // 
            nudConsume.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudConsume.ForeColor = System.Drawing.Color.Gainsboro;
            nudConsume.Location = new System.Drawing.Point(97, 61);
            nudConsume.Margin = new Padding(4, 3, 4, 3);
            nudConsume.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudConsume.Name = "nudConsume";
            nudConsume.Size = new Size(210, 23);
            nudConsume.TabIndex = 46;
            nudConsume.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudConsume.ValueChanged += nudConsume_ValueChanged;
            // 
            // cmbItem
            // 
            cmbItem.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbItem.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbItem.BorderStyle = ButtonBorderStyle.Solid;
            cmbItem.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbItem.DrawDropdownHoverOutline = false;
            cmbItem.DrawFocusRectangle = false;
            cmbItem.DrawMode = DrawMode.OwnerDrawFixed;
            cmbItem.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbItem.FlatStyle = FlatStyle.Flat;
            cmbItem.ForeColor = System.Drawing.Color.Gainsboro;
            cmbItem.FormattingEnabled = true;
            cmbItem.Location = new System.Drawing.Point(97, 22);
            cmbItem.Margin = new Padding(4, 3, 4, 3);
            cmbItem.Name = "cmbItem";
            cmbItem.Size = new Size(209, 24);
            cmbItem.TabIndex = 26;
            cmbItem.Text = null;
            cmbItem.TextPadding = new Padding(2);
            cmbItem.SelectedIndexChanged += cmbItem_SelectedIndexChanged;
            // 
            // lblAmmoItem
            // 
            lblAmmoItem.AutoSize = true;
            lblAmmoItem.Location = new System.Drawing.Point(10, 22);
            lblAmmoItem.Margin = new Padding(4, 0, 4, 0);
            lblAmmoItem.Name = "lblAmmoItem";
            lblAmmoItem.Size = new Size(34, 15);
            lblAmmoItem.TabIndex = 25;
            lblAmmoItem.Text = "Item:";
            // 
            // lblAmmoAmount
            // 
            lblAmmoAmount.AutoSize = true;
            lblAmmoAmount.Location = new System.Drawing.Point(10, 61);
            lblAmmoAmount.Margin = new Padding(4, 0, 4, 0);
            lblAmmoAmount.Name = "lblAmmoAmount";
            lblAmmoAmount.Size = new Size(54, 15);
            lblAmmoAmount.TabIndex = 9;
            lblAmmoAmount.Text = "Amount:";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(583, 686);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(222, 31);
            btnCancel.TabIndex = 34;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(354, 686);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(222, 31);
            btnSave.TabIndex = 31;
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;
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
            toolStrip.Size = new Size(818, 29);
            toolStrip.TabIndex = 46;
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
            // FrmProjectile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            ClientSize = new Size(818, 729);
            ControlBox = true;
            Controls.Add(toolStrip);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(grpProjectiles);
            Controls.Add(pnlContainer);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FrmProjectile";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Projectile Editor";
            FormClosed += FrmProjectile_FormClosed;
            Load += frmProjectile_Load;
            KeyDown += form_KeyDown;
            grpProjectiles.ResumeLayout(false);
            grpProjectiles.PerformLayout();
            grpSpawns.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picSpawns).EndInit();
            grpProperties.ResumeLayout(false);
            grpProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudKnockback).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRange).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudSpawn).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudSpeed).EndInit();
            grpAnimations.ResumeLayout(false);
            grpAnimations.PerformLayout();
            grpCollisions.ResumeLayout(false);
            grpCollisions.PerformLayout();
            pnlContainer.ResumeLayout(false);
            grpTargettingOptions.ResumeLayout(false);
            grpTargettingOptions.PerformLayout();
            grpGrappleOptions.ResumeLayout(false);
            grpGrappleOptions.PerformLayout();
            grpAmmo.ResumeLayout(false);
            grpAmmo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudConsume).EndInit();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpProjectiles;
        private DarkGroupBox grpSpawns;
        public System.Windows.Forms.PictureBox picSpawns;
        private DarkGroupBox grpProperties;
        private System.Windows.Forms.Label lblRange;
        private System.Windows.Forms.Label lblSpawn;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblName;
        private DarkTextBox txtName;
        private System.Windows.Forms.Label lblSpell;
        private DarkGroupBox grpAnimations;
        private DarkCheckBox chkRotation;
        private System.Windows.Forms.Label lblAnimation;
        private System.Windows.Forms.ListBox lstAnimations;
        private System.Windows.Forms.Label lblSpawnRange;
        private DarkScrollBar scrlSpawnRange;
        private DarkGroupBox grpCollisions;
        private DarkCheckBox chkIgnoreInactiveResources;
        private DarkCheckBox chkIgnoreZDimensionBlocks;
        private DarkCheckBox chkIgnoreMapBlocks;
        private DarkCheckBox chkIgnoreActiveResources;
        private DarkButton btnRemove;
        private DarkButton btnAdd;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.Panel pnlContainer;
        private DarkButton btnSave;
        private DarkButton btnCancel;
        private System.Windows.Forms.Label lblKnockback;
        private DarkToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripItemNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripItemDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton toolStripItemCopy;
        public System.Windows.Forms.ToolStripButton toolStripItemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.ToolStripButton toolStripItemUndo;
        private DarkGroupBox grpAmmo;
        private System.Windows.Forms.Label lblAmmoAmount;
        private DarkComboBox cmbItem;
        private System.Windows.Forms.Label lblAmmoItem;
        private DarkComboBox cmbAnimation;
        private DarkComboBox cmbSpell;
        private DarkNumericUpDown nudKnockback;
        private DarkNumericUpDown nudRange;
        private DarkNumericUpDown nudAmount;
        private DarkNumericUpDown nudSpawn;
        private DarkNumericUpDown nudSpeed;
        private DarkNumericUpDown nudConsume;
        private DarkButton btnClearSearch;
        private DarkTextBox txtSearch;
        private DarkButton btnAddFolder;
        private System.Windows.Forms.Label lblFolder;
        private DarkComboBox cmbFolder;
        private System.Windows.Forms.ToolStripButton btnAlphabetical;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private DarkCheckBox chkPierce;
        private Controls.GameObjectList lstGameObjects;
        private DarkGroupBox grpGrappleOptions;
        private DarkCheckBox chkGrappleOnNpc;
        private DarkCheckBox chkGrappleOnResource;
        private DarkCheckBox chkGrappleOnMap;
        private DarkCheckBox chkGrappleOnPlayer;
        private DarkGroupBox grpTargettingOptions;
        private DarkRadioButton rdoBehaviorHoming;
        private DarkRadioButton rdoBehaviorDirectShot;
        private DarkRadioButton rdoBehaviorDefault;
    }
}