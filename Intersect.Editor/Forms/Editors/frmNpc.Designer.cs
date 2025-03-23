using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
{
    partial class FrmNpc
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmNpc));
            grpNpcs = new DarkGroupBox();
            btnClearSearch = new DarkButton();
            txtSearch = new DarkTextBox();
            lstGameObjects = new Controls.GameObjectList();
            grpGeneral = new DarkGroupBox();
            lblAlpha = new Label();
            lblBlue = new Label();
            lblGreen = new Label();
            lblRed = new Label();
            nudRgbaA = new DarkNumericUpDown();
            nudRgbaB = new DarkNumericUpDown();
            nudRgbaG = new DarkNumericUpDown();
            nudRgbaR = new DarkNumericUpDown();
            btnAddFolder = new DarkButton();
            lblFolder = new Label();
            cmbFolder = new DarkComboBox();
            lblLevel = new Label();
            nudLevel = new DarkNumericUpDown();
            cmbSprite = new DarkComboBox();
            lblPic = new Label();
            picNpc = new PictureBox();
            lblName = new Label();
            txtName = new DarkTextBox();
            nudSpawnDuration = new DarkNumericUpDown();
            lblSpawnDuration = new Label();
            nudSightRange = new DarkNumericUpDown();
            lblSightRange = new Label();
            grpStats = new DarkGroupBox();
            nudExp = new DarkNumericUpDown();
            nudMana = new DarkNumericUpDown();
            nudHp = new DarkNumericUpDown();
            nudSpd = new DarkNumericUpDown();
            nudMR = new DarkNumericUpDown();
            nudDef = new DarkNumericUpDown();
            nudMag = new DarkNumericUpDown();
            nudStr = new DarkNumericUpDown();
            lblSpd = new Label();
            lblMR = new Label();
            lblDef = new Label();
            lblMag = new Label();
            lblStr = new Label();
            lblMana = new Label();
            lblHP = new Label();
            lblExp = new Label();
            pnlContainer = new Panel();
            grpImmunities = new DarkGroupBox();
            nudTenacity = new DarkNumericUpDown();
            lblTenacity = new Label();
            chkTaunt = new DarkCheckBox();
            chkSleep = new DarkCheckBox();
            chkTransform = new DarkCheckBox();
            chkBlind = new DarkCheckBox();
            chkSnare = new DarkCheckBox();
            chkStun = new DarkCheckBox();
            chkSilence = new DarkCheckBox();
            chkKnockback = new DarkCheckBox();
            grpCombat = new DarkGroupBox();
            grpAttackSpeed = new DarkGroupBox();
            nudAttackSpeedValue = new DarkNumericUpDown();
            lblAttackSpeedValue = new Label();
            cmbAttackSpeedModifier = new DarkComboBox();
            lblAttackSpeedModifier = new Label();
            nudCritMultiplier = new DarkNumericUpDown();
            lblCritMultiplier = new Label();
            nudScaling = new DarkNumericUpDown();
            nudDamage = new DarkNumericUpDown();
            nudCritChance = new DarkNumericUpDown();
            cmbScalingStat = new DarkComboBox();
            lblScalingStat = new Label();
            lblScaling = new Label();
            cmbDamageType = new DarkComboBox();
            lblDamageType = new Label();
            lblCritChance = new Label();
            cmbAttackAnimation = new DarkComboBox();
            lblAttackAnimation = new Label();
            lblDamage = new Label();
            grpCommonEvents = new DarkGroupBox();
            cmbOnDeathEventParty = new DarkComboBox();
            lblOnDeathEventParty = new Label();
            cmbOnDeathEventKiller = new DarkComboBox();
            lblOnDeathEventKiller = new Label();
            grpBehavior = new DarkGroupBox();
            nudResetRadius = new DarkNumericUpDown();
            lblResetRadius = new Label();
            chkFocusDamageDealer = new DarkCheckBox();
            nudFlee = new DarkNumericUpDown();
            lblFlee = new Label();
            chkSwarm = new DarkCheckBox();
            grpConditions = new DarkGroupBox();
            btnAttackOnSightCond = new DarkButton();
            btnPlayerCanAttackCond = new DarkButton();
            btnPlayerFriendProtectorCond = new DarkButton();
            lblMovement = new Label();
            cmbMovement = new DarkComboBox();
            chkAggressive = new DarkCheckBox();
            grpRegen = new DarkGroupBox();
            nudMpRegen = new DarkNumericUpDown();
            nudHpRegen = new DarkNumericUpDown();
            lblHpRegen = new Label();
            lblManaRegen = new Label();
            lblRegenHint = new Label();
            grpDrops = new DarkGroupBox();
            nudDropMinAmount = new DarkNumericUpDown();
            lblDropMinAmount = new Label();
            chkIndividualLoot = new DarkCheckBox();
            btnDropRemove = new DarkButton();
            btnDropAdd = new DarkButton();
            lstDrops = new ListBox();
            nudDropMaxAmount = new DarkNumericUpDown();
            nudDropChance = new DarkNumericUpDown();
            cmbDropItem = new DarkComboBox();
            lblDropMaxAmount = new Label();
            lblDropChance = new Label();
            lblDropItem = new Label();
            grpNpcVsNpc = new DarkGroupBox();
            cmbHostileNPC = new DarkComboBox();
            lblNPC = new Label();
            btnRemoveAggro = new DarkButton();
            btnAddAggro = new DarkButton();
            lstAggro = new ListBox();
            chkAttackAllies = new DarkCheckBox();
            chkEnabled = new DarkCheckBox();
            grpSpells = new DarkGroupBox();
            cmbSpell = new DarkComboBox();
            cmbFreq = new DarkComboBox();
            lblFreq = new Label();
            lblSpell = new Label();
            btnRemove = new DarkButton();
            btnAdd = new DarkButton();
            lstSpells = new ListBox();
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
            grpNpcs.SuspendLayout();
            grpGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudRgbaA).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRgbaB).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRgbaG).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudRgbaR).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picNpc).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudSpawnDuration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudSightRange).BeginInit();
            grpStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudExp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMana).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudHp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudSpd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMR).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMag).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudStr).BeginInit();
            pnlContainer.SuspendLayout();
            grpImmunities.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudTenacity).BeginInit();
            grpCombat.SuspendLayout();
            grpAttackSpeed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudAttackSpeedValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCritMultiplier).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudScaling).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDamage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCritChance).BeginInit();
            grpCommonEvents.SuspendLayout();
            grpBehavior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudResetRadius).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFlee).BeginInit();
            grpConditions.SuspendLayout();
            grpRegen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudMpRegen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudHpRegen).BeginInit();
            grpDrops.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudDropMinAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDropMaxAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDropChance).BeginInit();
            grpNpcVsNpc.SuspendLayout();
            grpSpells.SuspendLayout();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // grpNpcs
            // 
            grpNpcs.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpNpcs.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpNpcs.Controls.Add(btnClearSearch);
            grpNpcs.Controls.Add(txtSearch);
            grpNpcs.Controls.Add(lstGameObjects);
            grpNpcs.ForeColor = System.Drawing.Color.Gainsboro;
            grpNpcs.Location = new System.Drawing.Point(4, 32);
            grpNpcs.Margin = new Padding(4, 3, 4, 3);
            grpNpcs.Name = "grpNpcs";
            grpNpcs.Padding = new Padding(4, 3, 4, 3);
            grpNpcs.Size = new Size(233, 643);
            grpNpcs.TabIndex = 13;
            grpNpcs.TabStop = false;
            grpNpcs.Text = "NPCs";
            // 
            // btnClearSearch
            // 
            btnClearSearch.Location = new System.Drawing.Point(204, 22);
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
            txtSearch.Location = new System.Drawing.Point(6, 22);
            txtSearch.Margin = new Padding(4, 3, 4, 3);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(192, 23);
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
            lstGameObjects.Location = new System.Drawing.Point(6, 53);
            lstGameObjects.Margin = new Padding(4, 3, 4, 3);
            lstGameObjects.Name = "lstGameObjects";
            lstGameObjects.SelectedImageIndex = 0;
            lstGameObjects.Size = new Size(222, 583);
            lstGameObjects.TabIndex = 32;
            // 
            // grpGeneral
            // 
            grpGeneral.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpGeneral.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpGeneral.Controls.Add(lblAlpha);
            grpGeneral.Controls.Add(lblBlue);
            grpGeneral.Controls.Add(lblGreen);
            grpGeneral.Controls.Add(lblRed);
            grpGeneral.Controls.Add(nudRgbaA);
            grpGeneral.Controls.Add(nudRgbaB);
            grpGeneral.Controls.Add(nudRgbaG);
            grpGeneral.Controls.Add(nudRgbaR);
            grpGeneral.Controls.Add(btnAddFolder);
            grpGeneral.Controls.Add(lblFolder);
            grpGeneral.Controls.Add(cmbFolder);
            grpGeneral.Controls.Add(lblLevel);
            grpGeneral.Controls.Add(nudLevel);
            grpGeneral.Controls.Add(cmbSprite);
            grpGeneral.Controls.Add(lblPic);
            grpGeneral.Controls.Add(picNpc);
            grpGeneral.Controls.Add(lblName);
            grpGeneral.Controls.Add(txtName);
            grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
            grpGeneral.Location = new System.Drawing.Point(10, 6);
            grpGeneral.Margin = new Padding(4, 3, 4, 3);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Padding = new Padding(4, 3, 4, 3);
            grpGeneral.Size = new Size(241, 330);
            grpGeneral.TabIndex = 14;
            grpGeneral.TabStop = false;
            grpGeneral.Text = "General";
            // 
            // lblAlpha
            // 
            lblAlpha.AutoSize = true;
            lblAlpha.Location = new System.Drawing.Point(126, 302);
            lblAlpha.Margin = new Padding(4, 0, 4, 0);
            lblAlpha.Name = "lblAlpha";
            lblAlpha.Size = new Size(41, 15);
            lblAlpha.TabIndex = 78;
            lblAlpha.Text = "Alpha:";
            // 
            // lblBlue
            // 
            lblBlue.AutoSize = true;
            lblBlue.Location = new System.Drawing.Point(126, 272);
            lblBlue.Margin = new Padding(4, 0, 4, 0);
            lblBlue.Name = "lblBlue";
            lblBlue.Size = new Size(33, 15);
            lblBlue.TabIndex = 77;
            lblBlue.Text = "Blue:";
            // 
            // lblGreen
            // 
            lblGreen.AutoSize = true;
            lblGreen.Location = new System.Drawing.Point(10, 302);
            lblGreen.Margin = new Padding(4, 0, 4, 0);
            lblGreen.Name = "lblGreen";
            lblGreen.Size = new Size(41, 15);
            lblGreen.TabIndex = 76;
            lblGreen.Text = "Green:";
            // 
            // lblRed
            // 
            lblRed.AutoSize = true;
            lblRed.Location = new System.Drawing.Point(10, 272);
            lblRed.Margin = new Padding(4, 0, 4, 0);
            lblRed.Name = "lblRed";
            lblRed.Size = new Size(30, 15);
            lblRed.TabIndex = 75;
            lblRed.Text = "Red:";
            // 
            // nudRgbaA
            // 
            nudRgbaA.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudRgbaA.ForeColor = System.Drawing.Color.Gainsboro;
            nudRgbaA.Location = new System.Drawing.Point(178, 300);
            nudRgbaA.Margin = new Padding(4, 3, 4, 3);
            nudRgbaA.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaA.Name = "nudRgbaA";
            nudRgbaA.Size = new Size(49, 23);
            nudRgbaA.TabIndex = 74;
            nudRgbaA.Value = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaA.ValueChanged += nudRgbaA_ValueChanged;
            // 
            // nudRgbaB
            // 
            nudRgbaB.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudRgbaB.ForeColor = System.Drawing.Color.Gainsboro;
            nudRgbaB.Location = new System.Drawing.Point(178, 270);
            nudRgbaB.Margin = new Padding(4, 3, 4, 3);
            nudRgbaB.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaB.Name = "nudRgbaB";
            nudRgbaB.Size = new Size(49, 23);
            nudRgbaB.TabIndex = 73;
            nudRgbaB.Value = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaB.ValueChanged += nudRgbaB_ValueChanged;
            // 
            // nudRgbaG
            // 
            nudRgbaG.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudRgbaG.ForeColor = System.Drawing.Color.Gainsboro;
            nudRgbaG.Location = new System.Drawing.Point(64, 300);
            nudRgbaG.Margin = new Padding(4, 3, 4, 3);
            nudRgbaG.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaG.Name = "nudRgbaG";
            nudRgbaG.Size = new Size(49, 23);
            nudRgbaG.TabIndex = 72;
            nudRgbaG.Value = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaG.ValueChanged += nudRgbaG_ValueChanged;
            // 
            // nudRgbaR
            // 
            nudRgbaR.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudRgbaR.ForeColor = System.Drawing.Color.Gainsboro;
            nudRgbaR.Location = new System.Drawing.Point(64, 270);
            nudRgbaR.Margin = new Padding(4, 3, 4, 3);
            nudRgbaR.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaR.Name = "nudRgbaR";
            nudRgbaR.Size = new Size(49, 23);
            nudRgbaR.TabIndex = 71;
            nudRgbaR.Value = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaR.ValueChanged += nudRgbaR_ValueChanged;
            // 
            // btnAddFolder
            // 
            btnAddFolder.Location = new System.Drawing.Point(206, 54);
            btnAddFolder.Margin = new Padding(4, 3, 4, 3);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Padding = new Padding(6);
            btnAddFolder.Size = new Size(21, 24);
            btnAddFolder.TabIndex = 67;
            btnAddFolder.Text = "+";
            btnAddFolder.Click += btnAddFolder_Click;
            // 
            // lblFolder
            // 
            lblFolder.AutoSize = true;
            lblFolder.Location = new System.Drawing.Point(10, 59);
            lblFolder.Margin = new Padding(4, 0, 4, 0);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new Size(43, 15);
            lblFolder.TabIndex = 66;
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
            cmbFolder.Location = new System.Drawing.Point(70, 54);
            cmbFolder.Margin = new Padding(4, 3, 4, 3);
            cmbFolder.Name = "cmbFolder";
            cmbFolder.Size = new Size(131, 24);
            cmbFolder.TabIndex = 65;
            cmbFolder.Text = null;
            cmbFolder.TextPadding = new Padding(2);
            cmbFolder.SelectedIndexChanged += cmbFolder_SelectedIndexChanged;
            // 
            // lblLevel
            // 
            lblLevel.AutoSize = true;
            lblLevel.Location = new System.Drawing.Point(10, 91);
            lblLevel.Margin = new Padding(4, 0, 4, 0);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(37, 15);
            lblLevel.TabIndex = 64;
            lblLevel.Text = "Level:";
            // 
            // nudLevel
            // 
            nudLevel.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudLevel.ForeColor = System.Drawing.Color.Gainsboro;
            nudLevel.Location = new System.Drawing.Point(70, 89);
            nudLevel.Margin = new Padding(4, 3, 4, 3);
            nudLevel.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudLevel.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudLevel.Name = "nudLevel";
            nudLevel.Size = new Size(156, 23);
            nudLevel.TabIndex = 63;
            nudLevel.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudLevel.ValueChanged += nudLevel_ValueChanged;
            // 
            // cmbSprite
            // 
            cmbSprite.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbSprite.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbSprite.BorderStyle = ButtonBorderStyle.Solid;
            cmbSprite.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbSprite.DrawDropdownHoverOutline = false;
            cmbSprite.DrawFocusRectangle = false;
            cmbSprite.DrawMode = DrawMode.OwnerDrawFixed;
            cmbSprite.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSprite.FlatStyle = FlatStyle.Flat;
            cmbSprite.ForeColor = System.Drawing.Color.Gainsboro;
            cmbSprite.FormattingEnabled = true;
            cmbSprite.Items.AddRange(new object[] { "None" });
            cmbSprite.Location = new System.Drawing.Point(70, 121);
            cmbSprite.Margin = new Padding(4, 3, 4, 3);
            cmbSprite.Name = "cmbSprite";
            cmbSprite.Size = new Size(156, 24);
            cmbSprite.TabIndex = 11;
            cmbSprite.Text = "None";
            cmbSprite.TextPadding = new Padding(2);
            cmbSprite.SelectedIndexChanged += cmbSprite_SelectedIndexChanged;
            // 
            // lblPic
            // 
            lblPic.AutoSize = true;
            lblPic.Location = new System.Drawing.Point(10, 125);
            lblPic.Margin = new Padding(4, 0, 4, 0);
            lblPic.Name = "lblPic";
            lblPic.Size = new Size(40, 15);
            lblPic.TabIndex = 6;
            lblPic.Text = "Sprite:";
            // 
            // picNpc
            // 
            picNpc.BackColor = System.Drawing.Color.Black;
            picNpc.Location = new System.Drawing.Point(64, 153);
            picNpc.Margin = new Padding(4, 3, 4, 3);
            picNpc.Name = "picNpc";
            picNpc.Size = new Size(112, 111);
            picNpc.TabIndex = 4;
            picNpc.TabStop = false;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(10, 24);
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
            txtName.Location = new System.Drawing.Point(70, 22);
            txtName.Margin = new Padding(4, 3, 4, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(157, 23);
            txtName.TabIndex = 0;
            txtName.TextChanged += txtName_TextChanged;
            // 
            // nudSpawnDuration
            // 
            nudSpawnDuration.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudSpawnDuration.ForeColor = System.Drawing.Color.Gainsboro;
            nudSpawnDuration.Location = new System.Drawing.Point(119, 181);
            nudSpawnDuration.Margin = new Padding(4, 3, 4, 3);
            nudSpawnDuration.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudSpawnDuration.Name = "nudSpawnDuration";
            nudSpawnDuration.Size = new Size(135, 23);
            nudSpawnDuration.TabIndex = 61;
            nudSpawnDuration.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudSpawnDuration.ValueChanged += nudSpawnDuration_ValueChanged;
            // 
            // lblSpawnDuration
            // 
            lblSpawnDuration.AutoSize = true;
            lblSpawnDuration.Location = new System.Drawing.Point(12, 183);
            lblSpawnDuration.Margin = new Padding(4, 0, 4, 0);
            lblSpawnDuration.Name = "lblSpawnDuration";
            lblSpawnDuration.Size = new Size(94, 15);
            lblSpawnDuration.TabIndex = 7;
            lblSpawnDuration.Text = "Spawn Duration:";
            // 
            // nudSightRange
            // 
            nudSightRange.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudSightRange.ForeColor = System.Drawing.Color.Gainsboro;
            nudSightRange.Location = new System.Drawing.Point(119, 80);
            nudSightRange.Margin = new Padding(4, 3, 4, 3);
            nudSightRange.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            nudSightRange.Name = "nudSightRange";
            nudSightRange.Size = new Size(135, 23);
            nudSightRange.TabIndex = 62;
            nudSightRange.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudSightRange.ValueChanged += nudSightRange_ValueChanged;
            // 
            // lblSightRange
            // 
            lblSightRange.AutoSize = true;
            lblSightRange.Location = new System.Drawing.Point(12, 82);
            lblSightRange.Margin = new Padding(4, 0, 4, 0);
            lblSightRange.Name = "lblSightRange";
            lblSightRange.Size = new Size(73, 15);
            lblSightRange.TabIndex = 12;
            lblSightRange.Text = "Sight Range:";
            // 
            // grpStats
            // 
            grpStats.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpStats.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpStats.Controls.Add(nudExp);
            grpStats.Controls.Add(nudMana);
            grpStats.Controls.Add(nudHp);
            grpStats.Controls.Add(nudSpd);
            grpStats.Controls.Add(nudMR);
            grpStats.Controls.Add(nudDef);
            grpStats.Controls.Add(nudMag);
            grpStats.Controls.Add(nudStr);
            grpStats.Controls.Add(lblSpd);
            grpStats.Controls.Add(lblMR);
            grpStats.Controls.Add(lblDef);
            grpStats.Controls.Add(lblMag);
            grpStats.Controls.Add(lblStr);
            grpStats.Controls.Add(lblMana);
            grpStats.Controls.Add(lblHP);
            grpStats.Controls.Add(lblExp);
            grpStats.ForeColor = System.Drawing.Color.Gainsboro;
            grpStats.Location = new System.Drawing.Point(10, 492);
            grpStats.Margin = new Padding(4, 3, 4, 3);
            grpStats.Name = "grpStats";
            grpStats.Padding = new Padding(4, 3, 4, 3);
            grpStats.Size = new Size(241, 246);
            grpStats.TabIndex = 15;
            grpStats.TabStop = false;
            grpStats.Text = "Stats:";
            // 
            // nudExp
            // 
            nudExp.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudExp.ForeColor = System.Drawing.Color.Gainsboro;
            nudExp.Location = new System.Drawing.Point(122, 190);
            nudExp.Margin = new Padding(4, 3, 4, 3);
            nudExp.Maximum = new decimal(new int[] { 1410065407, 2, 0, 0 });
            nudExp.Name = "nudExp";
            nudExp.Size = new Size(90, 23);
            nudExp.TabIndex = 45;
            nudExp.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudExp.ValueChanged += nudExp_ValueChanged;
            // 
            // nudMana
            // 
            nudMana.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMana.ForeColor = System.Drawing.Color.Gainsboro;
            nudMana.Location = new System.Drawing.Point(122, 40);
            nudMana.Margin = new Padding(4, 3, 4, 3);
            nudMana.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudMana.Name = "nudMana";
            nudMana.Size = new Size(90, 23);
            nudMana.TabIndex = 44;
            nudMana.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMana.ValueChanged += nudMana_ValueChanged;
            // 
            // nudHp
            // 
            nudHp.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudHp.ForeColor = System.Drawing.Color.Gainsboro;
            nudHp.Location = new System.Drawing.Point(14, 40);
            nudHp.Margin = new Padding(4, 3, 4, 3);
            nudHp.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudHp.Name = "nudHp";
            nudHp.Size = new Size(90, 23);
            nudHp.TabIndex = 43;
            nudHp.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudHp.ValueChanged += nudHp_ValueChanged;
            // 
            // nudSpd
            // 
            nudSpd.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudSpd.ForeColor = System.Drawing.Color.Gainsboro;
            nudSpd.Location = new System.Drawing.Point(15, 190);
            nudSpd.Margin = new Padding(4, 3, 4, 3);
            nudSpd.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudSpd.Name = "nudSpd";
            nudSpd.Size = new Size(90, 23);
            nudSpd.TabIndex = 42;
            nudSpd.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudSpd.ValueChanged += nudSpd_ValueChanged;
            // 
            // nudMR
            // 
            nudMR.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMR.ForeColor = System.Drawing.Color.Gainsboro;
            nudMR.Location = new System.Drawing.Point(122, 142);
            nudMR.Margin = new Padding(4, 3, 4, 3);
            nudMR.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudMR.Name = "nudMR";
            nudMR.Size = new Size(92, 23);
            nudMR.TabIndex = 41;
            nudMR.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMR.ValueChanged += nudMR_ValueChanged;
            // 
            // nudDef
            // 
            nudDef.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDef.ForeColor = System.Drawing.Color.Gainsboro;
            nudDef.Location = new System.Drawing.Point(14, 142);
            nudDef.Margin = new Padding(4, 3, 4, 3);
            nudDef.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudDef.Name = "nudDef";
            nudDef.Size = new Size(92, 23);
            nudDef.TabIndex = 40;
            nudDef.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudDef.ValueChanged += nudDef_ValueChanged;
            // 
            // nudMag
            // 
            nudMag.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMag.ForeColor = System.Drawing.Color.Gainsboro;
            nudMag.Location = new System.Drawing.Point(122, 92);
            nudMag.Margin = new Padding(4, 3, 4, 3);
            nudMag.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudMag.Name = "nudMag";
            nudMag.Size = new Size(90, 23);
            nudMag.TabIndex = 39;
            nudMag.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMag.ValueChanged += nudMag_ValueChanged;
            // 
            // nudStr
            // 
            nudStr.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudStr.ForeColor = System.Drawing.Color.Gainsboro;
            nudStr.Location = new System.Drawing.Point(15, 92);
            nudStr.Margin = new Padding(4, 3, 4, 3);
            nudStr.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudStr.Name = "nudStr";
            nudStr.Size = new Size(90, 23);
            nudStr.TabIndex = 38;
            nudStr.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudStr.ValueChanged += nudStr_ValueChanged;
            // 
            // lblSpd
            // 
            lblSpd.AutoSize = true;
            lblSpd.Location = new System.Drawing.Point(12, 175);
            lblSpd.Margin = new Padding(2, 0, 2, 0);
            lblSpd.Name = "lblSpd";
            lblSpd.Size = new Size(75, 15);
            lblSpd.TabIndex = 37;
            lblSpd.Text = "Move Speed:";
            // 
            // lblMR
            // 
            lblMR.AutoSize = true;
            lblMR.Location = new System.Drawing.Point(121, 123);
            lblMR.Margin = new Padding(2, 0, 2, 0);
            lblMR.Name = "lblMR";
            lblMR.Size = new Size(76, 15);
            lblMR.TabIndex = 36;
            lblMR.Text = "Magic Resist:";
            // 
            // lblDef
            // 
            lblDef.AutoSize = true;
            lblDef.Location = new System.Drawing.Point(10, 123);
            lblDef.Margin = new Padding(2, 0, 2, 0);
            lblDef.Name = "lblDef";
            lblDef.Size = new Size(44, 15);
            lblDef.TabIndex = 35;
            lblDef.Text = "Armor:";
            // 
            // lblMag
            // 
            lblMag.AutoSize = true;
            lblMag.Location = new System.Drawing.Point(124, 73);
            lblMag.Margin = new Padding(2, 0, 2, 0);
            lblMag.Name = "lblMag";
            lblMag.Size = new Size(43, 15);
            lblMag.TabIndex = 34;
            lblMag.Text = "Magic:";
            // 
            // lblStr
            // 
            lblStr.AutoSize = true;
            lblStr.Location = new System.Drawing.Point(10, 74);
            lblStr.Margin = new Padding(2, 0, 2, 0);
            lblStr.Name = "lblStr";
            lblStr.Size = new Size(55, 15);
            lblStr.TabIndex = 33;
            lblStr.Text = "Strength:";
            // 
            // lblMana
            // 
            lblMana.AutoSize = true;
            lblMana.Location = new System.Drawing.Point(126, 21);
            lblMana.Margin = new Padding(4, 0, 4, 0);
            lblMana.Name = "lblMana";
            lblMana.Size = new Size(40, 15);
            lblMana.TabIndex = 15;
            lblMana.Text = "Mana:";
            // 
            // lblHP
            // 
            lblHP.AutoSize = true;
            lblHP.Location = new System.Drawing.Point(12, 22);
            lblHP.Margin = new Padding(4, 0, 4, 0);
            lblHP.Name = "lblHP";
            lblHP.Size = new Size(26, 15);
            lblHP.TabIndex = 14;
            lblHP.Text = "HP:";
            // 
            // lblExp
            // 
            lblExp.AutoSize = true;
            lblExp.Location = new System.Drawing.Point(124, 175);
            lblExp.Margin = new Padding(4, 0, 4, 0);
            lblExp.Name = "lblExp";
            lblExp.Size = new Size(29, 15);
            lblExp.TabIndex = 11;
            lblExp.Text = "Exp:";
            // 
            // pnlContainer
            // 
            pnlContainer.AutoScroll = true;
            pnlContainer.Controls.Add(grpImmunities);
            pnlContainer.Controls.Add(grpCombat);
            pnlContainer.Controls.Add(grpCommonEvents);
            pnlContainer.Controls.Add(grpBehavior);
            pnlContainer.Controls.Add(grpRegen);
            pnlContainer.Controls.Add(grpDrops);
            pnlContainer.Controls.Add(grpNpcVsNpc);
            pnlContainer.Controls.Add(grpSpells);
            pnlContainer.Controls.Add(grpGeneral);
            pnlContainer.Controls.Add(grpStats);
            pnlContainer.Location = new System.Drawing.Point(252, 39);
            pnlContainer.Margin = new Padding(4, 3, 4, 3);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(1138, 635);
            pnlContainer.TabIndex = 17;
            // 
            // grpImmunities
            // 
            grpImmunities.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpImmunities.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpImmunities.Controls.Add(nudTenacity);
            grpImmunities.Controls.Add(lblTenacity);
            grpImmunities.Controls.Add(chkTaunt);
            grpImmunities.Controls.Add(chkSleep);
            grpImmunities.Controls.Add(chkTransform);
            grpImmunities.Controls.Add(chkBlind);
            grpImmunities.Controls.Add(chkSnare);
            grpImmunities.Controls.Add(chkStun);
            grpImmunities.Controls.Add(chkSilence);
            grpImmunities.Controls.Add(chkKnockback);
            grpImmunities.ForeColor = System.Drawing.Color.Gainsboro;
            grpImmunities.Location = new System.Drawing.Point(536, 507);
            grpImmunities.Margin = new Padding(2);
            grpImmunities.Name = "grpImmunities";
            grpImmunities.Padding = new Padding(2);
            grpImmunities.Size = new Size(298, 209);
            grpImmunities.TabIndex = 33;
            grpImmunities.TabStop = false;
            grpImmunities.Text = "Immunities";
            // 
            // nudTenacity
            // 
            nudTenacity.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudTenacity.DecimalPlaces = 2;
            nudTenacity.ForeColor = System.Drawing.Color.Gainsboro;
            nudTenacity.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudTenacity.Location = new System.Drawing.Point(15, 160);
            nudTenacity.Margin = new Padding(4, 3, 4, 3);
            nudTenacity.Name = "nudTenacity";
            nudTenacity.Size = new Size(275, 23);
            nudTenacity.TabIndex = 79;
            nudTenacity.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudTenacity.ValueChanged += nudTenacity_ValueChanged;
            // 
            // lblTenacity
            // 
            lblTenacity.AutoSize = true;
            lblTenacity.Location = new System.Drawing.Point(6, 141);
            lblTenacity.Margin = new Padding(4, 0, 4, 0);
            lblTenacity.Name = "lblTenacity";
            lblTenacity.Size = new Size(74, 15);
            lblTenacity.TabIndex = 79;
            lblTenacity.Text = "Tenacity (%):";
            // 
            // chkTaunt
            // 
            chkTaunt.AutoSize = true;
            chkTaunt.Location = new System.Drawing.Point(205, 110);
            chkTaunt.Margin = new Padding(4, 3, 4, 3);
            chkTaunt.Name = "chkTaunt";
            chkTaunt.Size = new Size(55, 19);
            chkTaunt.TabIndex = 86;
            chkTaunt.Text = "Taunt";
            chkTaunt.CheckedChanged += chkTaunt_CheckedChanged;
            // 
            // chkSleep
            // 
            chkSleep.AutoSize = true;
            chkSleep.Location = new System.Drawing.Point(15, 110);
            chkSleep.Margin = new Padding(4, 3, 4, 3);
            chkSleep.Name = "chkSleep";
            chkSleep.Size = new Size(54, 19);
            chkSleep.TabIndex = 85;
            chkSleep.Text = "Sleep";
            chkSleep.CheckedChanged += chkSleep_CheckedChanged;
            // 
            // chkTransform
            // 
            chkTransform.AutoSize = true;
            chkTransform.Location = new System.Drawing.Point(205, 83);
            chkTransform.Margin = new Padding(4, 3, 4, 3);
            chkTransform.Name = "chkTransform";
            chkTransform.Size = new Size(79, 19);
            chkTransform.TabIndex = 84;
            chkTransform.Text = "Transform";
            chkTransform.CheckedChanged += chkTransform_CheckedChanged;
            // 
            // chkBlind
            // 
            chkBlind.AutoSize = true;
            chkBlind.Location = new System.Drawing.Point(15, 83);
            chkBlind.Margin = new Padding(4, 3, 4, 3);
            chkBlind.Name = "chkBlind";
            chkBlind.Size = new Size(53, 19);
            chkBlind.TabIndex = 83;
            chkBlind.Text = "Blind";
            chkBlind.CheckedChanged += chkBlind_CheckedChanged;
            // 
            // chkSnare
            // 
            chkSnare.AutoSize = true;
            chkSnare.Location = new System.Drawing.Point(205, 57);
            chkSnare.Margin = new Padding(4, 3, 4, 3);
            chkSnare.Name = "chkSnare";
            chkSnare.Size = new Size(55, 19);
            chkSnare.TabIndex = 82;
            chkSnare.Text = "Snare";
            chkSnare.CheckedChanged += chkSnare_CheckedChanged;
            // 
            // chkStun
            // 
            chkStun.AutoSize = true;
            chkStun.Location = new System.Drawing.Point(15, 57);
            chkStun.Margin = new Padding(4, 3, 4, 3);
            chkStun.Name = "chkStun";
            chkStun.Size = new Size(50, 19);
            chkStun.TabIndex = 81;
            chkStun.Text = "Stun";
            chkStun.CheckedChanged += chkStun_CheckedChanged;
            // 
            // chkSilence
            // 
            chkSilence.AutoSize = true;
            chkSilence.Location = new System.Drawing.Point(205, 30);
            chkSilence.Margin = new Padding(4, 3, 4, 3);
            chkSilence.Name = "chkSilence";
            chkSilence.Size = new Size(63, 19);
            chkSilence.TabIndex = 80;
            chkSilence.Text = "Silence";
            chkSilence.CheckedChanged += chkSilence_CheckedChanged;
            // 
            // chkKnockback
            // 
            chkKnockback.AutoSize = true;
            chkKnockback.Location = new System.Drawing.Point(15, 30);
            chkKnockback.Margin = new Padding(4, 3, 4, 3);
            chkKnockback.Name = "chkKnockback";
            chkKnockback.Size = new Size(84, 19);
            chkKnockback.TabIndex = 79;
            chkKnockback.Text = "Knockback";
            chkKnockback.CheckedChanged += chkKnockback_CheckedChanged;
            // 
            // grpCombat
            // 
            grpCombat.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpCombat.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpCombat.Controls.Add(grpAttackSpeed);
            grpCombat.Controls.Add(nudCritMultiplier);
            grpCombat.Controls.Add(lblCritMultiplier);
            grpCombat.Controls.Add(nudScaling);
            grpCombat.Controls.Add(nudDamage);
            grpCombat.Controls.Add(nudCritChance);
            grpCombat.Controls.Add(cmbScalingStat);
            grpCombat.Controls.Add(lblScalingStat);
            grpCombat.Controls.Add(lblScaling);
            grpCombat.Controls.Add(cmbDamageType);
            grpCombat.Controls.Add(lblDamageType);
            grpCombat.Controls.Add(lblCritChance);
            grpCombat.Controls.Add(cmbAttackAnimation);
            grpCombat.Controls.Add(lblAttackAnimation);
            grpCombat.Controls.Add(lblDamage);
            grpCombat.ForeColor = System.Drawing.Color.Gainsboro;
            grpCombat.Location = new System.Drawing.Point(536, 6);
            grpCombat.Margin = new Padding(4, 3, 4, 3);
            grpCombat.Name = "grpCombat";
            grpCombat.Padding = new Padding(4, 3, 4, 3);
            grpCombat.Size = new Size(298, 495);
            grpCombat.TabIndex = 17;
            grpCombat.TabStop = false;
            grpCombat.Text = "Combat";
            // 
            // grpAttackSpeed
            // 
            grpAttackSpeed.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpAttackSpeed.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpAttackSpeed.Controls.Add(nudAttackSpeedValue);
            grpAttackSpeed.Controls.Add(lblAttackSpeedValue);
            grpAttackSpeed.Controls.Add(cmbAttackSpeedModifier);
            grpAttackSpeed.Controls.Add(lblAttackSpeedModifier);
            grpAttackSpeed.ForeColor = System.Drawing.Color.Gainsboro;
            grpAttackSpeed.Location = new System.Drawing.Point(15, 383);
            grpAttackSpeed.Margin = new Padding(4, 3, 4, 3);
            grpAttackSpeed.Name = "grpAttackSpeed";
            grpAttackSpeed.Padding = new Padding(4, 3, 4, 3);
            grpAttackSpeed.Size = new Size(275, 104);
            grpAttackSpeed.TabIndex = 64;
            grpAttackSpeed.TabStop = false;
            grpAttackSpeed.Text = "Attack Speed";
            // 
            // nudAttackSpeedValue
            // 
            nudAttackSpeedValue.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudAttackSpeedValue.ForeColor = System.Drawing.Color.Gainsboro;
            nudAttackSpeedValue.Location = new System.Drawing.Point(70, 67);
            nudAttackSpeedValue.Margin = new Padding(4, 3, 4, 3);
            nudAttackSpeedValue.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudAttackSpeedValue.Name = "nudAttackSpeedValue";
            nudAttackSpeedValue.Size = new Size(184, 23);
            nudAttackSpeedValue.TabIndex = 56;
            nudAttackSpeedValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudAttackSpeedValue.ValueChanged += nudAttackSpeedValue_ValueChanged;
            // 
            // lblAttackSpeedValue
            // 
            lblAttackSpeedValue.AutoSize = true;
            lblAttackSpeedValue.Location = new System.Drawing.Point(10, 69);
            lblAttackSpeedValue.Margin = new Padding(4, 0, 4, 0);
            lblAttackSpeedValue.Name = "lblAttackSpeedValue";
            lblAttackSpeedValue.Size = new Size(38, 15);
            lblAttackSpeedValue.TabIndex = 29;
            lblAttackSpeedValue.Text = "Value:";
            // 
            // cmbAttackSpeedModifier
            // 
            cmbAttackSpeedModifier.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbAttackSpeedModifier.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbAttackSpeedModifier.BorderStyle = ButtonBorderStyle.Solid;
            cmbAttackSpeedModifier.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbAttackSpeedModifier.DrawDropdownHoverOutline = false;
            cmbAttackSpeedModifier.DrawFocusRectangle = false;
            cmbAttackSpeedModifier.DrawMode = DrawMode.OwnerDrawFixed;
            cmbAttackSpeedModifier.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAttackSpeedModifier.FlatStyle = FlatStyle.Flat;
            cmbAttackSpeedModifier.ForeColor = System.Drawing.Color.Gainsboro;
            cmbAttackSpeedModifier.FormattingEnabled = true;
            cmbAttackSpeedModifier.Location = new System.Drawing.Point(70, 28);
            cmbAttackSpeedModifier.Margin = new Padding(4, 3, 4, 3);
            cmbAttackSpeedModifier.Name = "cmbAttackSpeedModifier";
            cmbAttackSpeedModifier.Size = new Size(184, 24);
            cmbAttackSpeedModifier.TabIndex = 28;
            cmbAttackSpeedModifier.Text = null;
            cmbAttackSpeedModifier.TextPadding = new Padding(2);
            cmbAttackSpeedModifier.SelectedIndexChanged += cmbAttackSpeedModifier_SelectedIndexChanged;
            // 
            // lblAttackSpeedModifier
            // 
            lblAttackSpeedModifier.AutoSize = true;
            lblAttackSpeedModifier.Location = new System.Drawing.Point(10, 31);
            lblAttackSpeedModifier.Margin = new Padding(4, 0, 4, 0);
            lblAttackSpeedModifier.Name = "lblAttackSpeedModifier";
            lblAttackSpeedModifier.Size = new Size(55, 15);
            lblAttackSpeedModifier.TabIndex = 0;
            lblAttackSpeedModifier.Text = "Modifier:";
            // 
            // nudCritMultiplier
            // 
            nudCritMultiplier.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudCritMultiplier.DecimalPlaces = 2;
            nudCritMultiplier.ForeColor = System.Drawing.Color.Gainsboro;
            nudCritMultiplier.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudCritMultiplier.Location = new System.Drawing.Point(15, 135);
            nudCritMultiplier.Margin = new Padding(4, 3, 4, 3);
            nudCritMultiplier.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudCritMultiplier.Name = "nudCritMultiplier";
            nudCritMultiplier.Size = new Size(275, 23);
            nudCritMultiplier.TabIndex = 63;
            nudCritMultiplier.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudCritMultiplier.ValueChanged += nudCritMultiplier_ValueChanged;
            // 
            // lblCritMultiplier
            // 
            lblCritMultiplier.AutoSize = true;
            lblCritMultiplier.Location = new System.Drawing.Point(12, 119);
            lblCritMultiplier.Margin = new Padding(4, 0, 4, 0);
            lblCritMultiplier.Name = "lblCritMultiplier";
            lblCritMultiplier.Size = new Size(156, 15);
            lblCritMultiplier.TabIndex = 62;
            lblCritMultiplier.Text = "Crit Multiplier (Default 1.5x):";
            // 
            // nudScaling
            // 
            nudScaling.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudScaling.ForeColor = System.Drawing.Color.Gainsboro;
            nudScaling.Location = new System.Drawing.Point(14, 297);
            nudScaling.Margin = new Padding(4, 3, 4, 3);
            nudScaling.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudScaling.Name = "nudScaling";
            nudScaling.Size = new Size(276, 23);
            nudScaling.TabIndex = 61;
            nudScaling.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudScaling.ValueChanged += nudScaling_ValueChanged;
            // 
            // nudDamage
            // 
            nudDamage.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDamage.ForeColor = System.Drawing.Color.Gainsboro;
            nudDamage.Location = new System.Drawing.Point(14, 39);
            nudDamage.Margin = new Padding(4, 3, 4, 3);
            nudDamage.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudDamage.Name = "nudDamage";
            nudDamage.Size = new Size(276, 23);
            nudDamage.TabIndex = 60;
            nudDamage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudDamage.ValueChanged += nudDamage_ValueChanged;
            // 
            // nudCritChance
            // 
            nudCritChance.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudCritChance.ForeColor = System.Drawing.Color.Gainsboro;
            nudCritChance.Location = new System.Drawing.Point(15, 87);
            nudCritChance.Margin = new Padding(4, 3, 4, 3);
            nudCritChance.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudCritChance.Name = "nudCritChance";
            nudCritChance.Size = new Size(275, 23);
            nudCritChance.TabIndex = 59;
            nudCritChance.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudCritChance.ValueChanged += nudCritChance_ValueChanged;
            // 
            // cmbScalingStat
            // 
            cmbScalingStat.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbScalingStat.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbScalingStat.BorderStyle = ButtonBorderStyle.Solid;
            cmbScalingStat.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbScalingStat.DrawDropdownHoverOutline = false;
            cmbScalingStat.DrawFocusRectangle = false;
            cmbScalingStat.DrawMode = DrawMode.OwnerDrawFixed;
            cmbScalingStat.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbScalingStat.FlatStyle = FlatStyle.Flat;
            cmbScalingStat.ForeColor = System.Drawing.Color.Gainsboro;
            cmbScalingStat.FormattingEnabled = true;
            cmbScalingStat.Location = new System.Drawing.Point(15, 240);
            cmbScalingStat.Margin = new Padding(4, 3, 4, 3);
            cmbScalingStat.Name = "cmbScalingStat";
            cmbScalingStat.Size = new Size(275, 24);
            cmbScalingStat.TabIndex = 58;
            cmbScalingStat.Text = null;
            cmbScalingStat.TextPadding = new Padding(2);
            cmbScalingStat.SelectedIndexChanged += cmbScalingStat_SelectedIndexChanged;
            // 
            // lblScalingStat
            // 
            lblScalingStat.AutoSize = true;
            lblScalingStat.Location = new System.Drawing.Point(12, 220);
            lblScalingStat.Margin = new Padding(4, 0, 4, 0);
            lblScalingStat.Name = "lblScalingStat";
            lblScalingStat.Size = new Size(71, 15);
            lblScalingStat.TabIndex = 57;
            lblScalingStat.Text = "Scaling Stat:";
            // 
            // lblScaling
            // 
            lblScaling.AutoSize = true;
            lblScaling.Location = new System.Drawing.Point(10, 273);
            lblScaling.Margin = new Padding(4, 0, 4, 0);
            lblScaling.Name = "lblScaling";
            lblScaling.Size = new Size(95, 15);
            lblScaling.TabIndex = 56;
            lblScaling.Text = "Scaling Amount:";
            // 
            // cmbDamageType
            // 
            cmbDamageType.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbDamageType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbDamageType.BorderStyle = ButtonBorderStyle.Solid;
            cmbDamageType.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbDamageType.DrawDropdownHoverOutline = false;
            cmbDamageType.DrawFocusRectangle = false;
            cmbDamageType.DrawMode = DrawMode.OwnerDrawFixed;
            cmbDamageType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDamageType.FlatStyle = FlatStyle.Flat;
            cmbDamageType.ForeColor = System.Drawing.Color.Gainsboro;
            cmbDamageType.FormattingEnabled = true;
            cmbDamageType.Items.AddRange(new object[] { "Physical", "Magic", "True" });
            cmbDamageType.Location = new System.Drawing.Point(14, 187);
            cmbDamageType.Margin = new Padding(4, 3, 4, 3);
            cmbDamageType.Name = "cmbDamageType";
            cmbDamageType.Size = new Size(275, 24);
            cmbDamageType.TabIndex = 54;
            cmbDamageType.Text = "Physical";
            cmbDamageType.TextPadding = new Padding(2);
            cmbDamageType.SelectedIndexChanged += cmbDamageType_SelectedIndexChanged;
            // 
            // lblDamageType
            // 
            lblDamageType.AutoSize = true;
            lblDamageType.Location = new System.Drawing.Point(10, 167);
            lblDamageType.Margin = new Padding(4, 0, 4, 0);
            lblDamageType.Name = "lblDamageType";
            lblDamageType.Size = new Size(81, 15);
            lblDamageType.TabIndex = 53;
            lblDamageType.Text = "Damage Type:";
            // 
            // lblCritChance
            // 
            lblCritChance.AutoSize = true;
            lblCritChance.Location = new System.Drawing.Point(10, 72);
            lblCritChance.Margin = new Padding(4, 0, 4, 0);
            lblCritChance.Name = "lblCritChance";
            lblCritChance.Size = new Size(93, 15);
            lblCritChance.TabIndex = 52;
            lblCritChance.Text = "Crit Chance (%):";
            // 
            // cmbAttackAnimation
            // 
            cmbAttackAnimation.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbAttackAnimation.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbAttackAnimation.BorderStyle = ButtonBorderStyle.Solid;
            cmbAttackAnimation.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbAttackAnimation.DrawDropdownHoverOutline = false;
            cmbAttackAnimation.DrawFocusRectangle = false;
            cmbAttackAnimation.DrawMode = DrawMode.OwnerDrawFixed;
            cmbAttackAnimation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAttackAnimation.FlatStyle = FlatStyle.Flat;
            cmbAttackAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            cmbAttackAnimation.FormattingEnabled = true;
            cmbAttackAnimation.Location = new System.Drawing.Point(14, 346);
            cmbAttackAnimation.Margin = new Padding(4, 3, 4, 3);
            cmbAttackAnimation.Name = "cmbAttackAnimation";
            cmbAttackAnimation.Size = new Size(276, 24);
            cmbAttackAnimation.TabIndex = 50;
            cmbAttackAnimation.Text = null;
            cmbAttackAnimation.TextPadding = new Padding(2);
            cmbAttackAnimation.SelectedIndexChanged += cmbAttackAnimation_SelectedIndexChanged;
            // 
            // lblAttackAnimation
            // 
            lblAttackAnimation.AutoSize = true;
            lblAttackAnimation.Location = new System.Drawing.Point(10, 329);
            lblAttackAnimation.Margin = new Padding(4, 0, 4, 0);
            lblAttackAnimation.Name = "lblAttackAnimation";
            lblAttackAnimation.Size = new Size(103, 15);
            lblAttackAnimation.TabIndex = 49;
            lblAttackAnimation.Text = "Attack Animation:";
            // 
            // lblDamage
            // 
            lblDamage.AutoSize = true;
            lblDamage.Location = new System.Drawing.Point(10, 21);
            lblDamage.Margin = new Padding(4, 0, 4, 0);
            lblDamage.Name = "lblDamage";
            lblDamage.Size = new Size(81, 15);
            lblDamage.TabIndex = 48;
            lblDamage.Text = "Base Damage:";
            // 
            // grpCommonEvents
            // 
            grpCommonEvents.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpCommonEvents.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpCommonEvents.Controls.Add(cmbOnDeathEventParty);
            grpCommonEvents.Controls.Add(lblOnDeathEventParty);
            grpCommonEvents.Controls.Add(cmbOnDeathEventKiller);
            grpCommonEvents.Controls.Add(lblOnDeathEventKiller);
            grpCommonEvents.ForeColor = System.Drawing.Color.Gainsboro;
            grpCommonEvents.Location = new System.Drawing.Point(10, 345);
            grpCommonEvents.Margin = new Padding(2);
            grpCommonEvents.Name = "grpCommonEvents";
            grpCommonEvents.Padding = new Padding(2);
            grpCommonEvents.Size = new Size(241, 134);
            grpCommonEvents.TabIndex = 32;
            grpCommonEvents.TabStop = false;
            grpCommonEvents.Text = "Common Events";
            // 
            // cmbOnDeathEventParty
            // 
            cmbOnDeathEventParty.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbOnDeathEventParty.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbOnDeathEventParty.BorderStyle = ButtonBorderStyle.Solid;
            cmbOnDeathEventParty.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbOnDeathEventParty.DrawDropdownHoverOutline = false;
            cmbOnDeathEventParty.DrawFocusRectangle = false;
            cmbOnDeathEventParty.DrawMode = DrawMode.OwnerDrawFixed;
            cmbOnDeathEventParty.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOnDeathEventParty.FlatStyle = FlatStyle.Flat;
            cmbOnDeathEventParty.ForeColor = System.Drawing.Color.Gainsboro;
            cmbOnDeathEventParty.FormattingEnabled = true;
            cmbOnDeathEventParty.Location = new System.Drawing.Point(14, 92);
            cmbOnDeathEventParty.Margin = new Padding(4, 3, 4, 3);
            cmbOnDeathEventParty.Name = "cmbOnDeathEventParty";
            cmbOnDeathEventParty.Size = new Size(212, 24);
            cmbOnDeathEventParty.TabIndex = 21;
            cmbOnDeathEventParty.Text = null;
            cmbOnDeathEventParty.TextPadding = new Padding(2);
            cmbOnDeathEventParty.SelectedIndexChanged += cmbOnDeathEventParty_SelectedIndexChanged;
            // 
            // lblOnDeathEventParty
            // 
            lblOnDeathEventParty.AutoSize = true;
            lblOnDeathEventParty.Location = new System.Drawing.Point(10, 74);
            lblOnDeathEventParty.Margin = new Padding(4, 0, 4, 0);
            lblOnDeathEventParty.Name = "lblOnDeathEventParty";
            lblOnDeathEventParty.Size = new Size(116, 15);
            lblOnDeathEventParty.TabIndex = 20;
            lblOnDeathEventParty.Text = "On Death (for party):";
            // 
            // cmbOnDeathEventKiller
            // 
            cmbOnDeathEventKiller.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbOnDeathEventKiller.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbOnDeathEventKiller.BorderStyle = ButtonBorderStyle.Solid;
            cmbOnDeathEventKiller.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbOnDeathEventKiller.DrawDropdownHoverOutline = false;
            cmbOnDeathEventKiller.DrawFocusRectangle = false;
            cmbOnDeathEventKiller.DrawMode = DrawMode.OwnerDrawFixed;
            cmbOnDeathEventKiller.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOnDeathEventKiller.FlatStyle = FlatStyle.Flat;
            cmbOnDeathEventKiller.ForeColor = System.Drawing.Color.Gainsboro;
            cmbOnDeathEventKiller.FormattingEnabled = true;
            cmbOnDeathEventKiller.Location = new System.Drawing.Point(14, 42);
            cmbOnDeathEventKiller.Margin = new Padding(4, 3, 4, 3);
            cmbOnDeathEventKiller.Name = "cmbOnDeathEventKiller";
            cmbOnDeathEventKiller.Size = new Size(212, 24);
            cmbOnDeathEventKiller.TabIndex = 19;
            cmbOnDeathEventKiller.Text = null;
            cmbOnDeathEventKiller.TextPadding = new Padding(2);
            cmbOnDeathEventKiller.SelectedIndexChanged += cmbOnDeathEventKiller_SelectedIndexChanged;
            // 
            // lblOnDeathEventKiller
            // 
            lblOnDeathEventKiller.AutoSize = true;
            lblOnDeathEventKiller.Location = new System.Drawing.Point(10, 23);
            lblOnDeathEventKiller.Margin = new Padding(4, 0, 4, 0);
            lblOnDeathEventKiller.Name = "lblOnDeathEventKiller";
            lblOnDeathEventKiller.Size = new Size(114, 15);
            lblOnDeathEventKiller.TabIndex = 18;
            lblOnDeathEventKiller.Text = "On Death (for killer):";
            // 
            // grpBehavior
            // 
            grpBehavior.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpBehavior.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpBehavior.Controls.Add(nudResetRadius);
            grpBehavior.Controls.Add(lblResetRadius);
            grpBehavior.Controls.Add(chkFocusDamageDealer);
            grpBehavior.Controls.Add(nudSpawnDuration);
            grpBehavior.Controls.Add(lblSpawnDuration);
            grpBehavior.Controls.Add(nudFlee);
            grpBehavior.Controls.Add(lblFlee);
            grpBehavior.Controls.Add(chkSwarm);
            grpBehavior.Controls.Add(grpConditions);
            grpBehavior.Controls.Add(lblMovement);
            grpBehavior.Controls.Add(cmbMovement);
            grpBehavior.Controls.Add(chkAggressive);
            grpBehavior.Controls.Add(nudSightRange);
            grpBehavior.Controls.Add(lblSightRange);
            grpBehavior.ForeColor = System.Drawing.Color.Gainsboro;
            grpBehavior.Location = new System.Drawing.Point(844, 6);
            grpBehavior.Margin = new Padding(4, 3, 4, 3);
            grpBehavior.Name = "grpBehavior";
            grpBehavior.Padding = new Padding(4, 3, 4, 3);
            grpBehavior.Size = new Size(264, 391);
            grpBehavior.TabIndex = 32;
            grpBehavior.TabStop = false;
            grpBehavior.Text = "Behavior:";
            // 
            // nudResetRadius
            // 
            nudResetRadius.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudResetRadius.ForeColor = System.Drawing.Color.Gainsboro;
            nudResetRadius.Location = new System.Drawing.Point(119, 113);
            nudResetRadius.Margin = new Padding(4, 3, 4, 3);
            nudResetRadius.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            nudResetRadius.Name = "nudResetRadius";
            nudResetRadius.Size = new Size(135, 23);
            nudResetRadius.TabIndex = 76;
            nudResetRadius.Value = new decimal(new int[] { 9999, 0, 0, 0 });
            nudResetRadius.ValueChanged += nudResetRadius_ValueChanged;
            // 
            // lblResetRadius
            // 
            lblResetRadius.AutoSize = true;
            lblResetRadius.Location = new System.Drawing.Point(12, 115);
            lblResetRadius.Margin = new Padding(4, 0, 4, 0);
            lblResetRadius.Name = "lblResetRadius";
            lblResetRadius.Size = new Size(76, 15);
            lblResetRadius.TabIndex = 75;
            lblResetRadius.Text = "Reset Radius:";
            // 
            // chkFocusDamageDealer
            // 
            chkFocusDamageDealer.AutoSize = true;
            chkFocusDamageDealer.Location = new System.Drawing.Point(15, 48);
            chkFocusDamageDealer.Margin = new Padding(4, 3, 4, 3);
            chkFocusDamageDealer.Name = "chkFocusDamageDealer";
            chkFocusDamageDealer.Size = new Size(187, 19);
            chkFocusDamageDealer.TabIndex = 71;
            chkFocusDamageDealer.Text = "Focus Highest Damage Dealer:";
            chkFocusDamageDealer.CheckedChanged += chkFocusDamageDealer_CheckedChanged;
            // 
            // nudFlee
            // 
            nudFlee.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudFlee.ForeColor = System.Drawing.Color.Gainsboro;
            nudFlee.Location = new System.Drawing.Point(119, 215);
            nudFlee.Margin = new Padding(4, 3, 4, 3);
            nudFlee.Name = "nudFlee";
            nudFlee.Size = new Size(135, 23);
            nudFlee.TabIndex = 70;
            nudFlee.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudFlee.ValueChanged += nudFlee_ValueChanged;
            // 
            // lblFlee
            // 
            lblFlee.AutoSize = true;
            lblFlee.Location = new System.Drawing.Point(12, 217);
            lblFlee.Margin = new Padding(4, 0, 4, 0);
            lblFlee.Name = "lblFlee";
            lblFlee.Size = new Size(82, 15);
            lblFlee.TabIndex = 69;
            lblFlee.Text = "Flee Health %:";
            // 
            // chkSwarm
            // 
            chkSwarm.AutoSize = true;
            chkSwarm.Location = new System.Drawing.Point(189, 20);
            chkSwarm.Margin = new Padding(4, 3, 4, 3);
            chkSwarm.Name = "chkSwarm";
            chkSwarm.Size = new Size(62, 19);
            chkSwarm.TabIndex = 67;
            chkSwarm.Text = "Swarm";
            chkSwarm.CheckedChanged += chkSwarm_CheckedChanged;
            // 
            // grpConditions
            // 
            grpConditions.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpConditions.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpConditions.Controls.Add(btnAttackOnSightCond);
            grpConditions.Controls.Add(btnPlayerCanAttackCond);
            grpConditions.Controls.Add(btnPlayerFriendProtectorCond);
            grpConditions.ForeColor = System.Drawing.Color.Gainsboro;
            grpConditions.Location = new System.Drawing.Point(12, 248);
            grpConditions.Margin = new Padding(4, 3, 4, 3);
            grpConditions.Name = "grpConditions";
            grpConditions.Padding = new Padding(4, 3, 4, 3);
            grpConditions.Size = new Size(243, 125);
            grpConditions.TabIndex = 66;
            grpConditions.TabStop = false;
            grpConditions.Text = "Conditions:";
            // 
            // btnAttackOnSightCond
            // 
            btnAttackOnSightCond.Location = new System.Drawing.Point(7, 55);
            btnAttackOnSightCond.Margin = new Padding(4, 3, 4, 3);
            btnAttackOnSightCond.Name = "btnAttackOnSightCond";
            btnAttackOnSightCond.Padding = new Padding(6);
            btnAttackOnSightCond.Size = new Size(227, 27);
            btnAttackOnSightCond.TabIndex = 47;
            btnAttackOnSightCond.Text = "Should Not Attack Player On Sight";
            btnAttackOnSightCond.Click += btnAttackOnSightCond_Click;
            // 
            // btnPlayerCanAttackCond
            // 
            btnPlayerCanAttackCond.Location = new System.Drawing.Point(7, 89);
            btnPlayerCanAttackCond.Margin = new Padding(4, 3, 4, 3);
            btnPlayerCanAttackCond.Name = "btnPlayerCanAttackCond";
            btnPlayerCanAttackCond.Padding = new Padding(6);
            btnPlayerCanAttackCond.Size = new Size(227, 27);
            btnPlayerCanAttackCond.TabIndex = 46;
            btnPlayerCanAttackCond.Text = "Player Can Attack (Default: True)";
            btnPlayerCanAttackCond.Click += btnPlayerCanAttackCond_Click;
            // 
            // btnPlayerFriendProtectorCond
            // 
            btnPlayerFriendProtectorCond.Location = new System.Drawing.Point(7, 22);
            btnPlayerFriendProtectorCond.Margin = new Padding(4, 3, 4, 3);
            btnPlayerFriendProtectorCond.Name = "btnPlayerFriendProtectorCond";
            btnPlayerFriendProtectorCond.Padding = new Padding(6);
            btnPlayerFriendProtectorCond.Size = new Size(227, 27);
            btnPlayerFriendProtectorCond.TabIndex = 44;
            btnPlayerFriendProtectorCond.Text = "Player Friend/Protector";
            btnPlayerFriendProtectorCond.Click += btnPlayerFriendProtectorCond_Click;
            // 
            // lblMovement
            // 
            lblMovement.AutoSize = true;
            lblMovement.Location = new System.Drawing.Point(12, 150);
            lblMovement.Margin = new Padding(4, 0, 4, 0);
            lblMovement.Name = "lblMovement";
            lblMovement.Size = new Size(68, 15);
            lblMovement.TabIndex = 65;
            lblMovement.Text = "Movement:";
            // 
            // cmbMovement
            // 
            cmbMovement.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbMovement.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbMovement.BorderStyle = ButtonBorderStyle.Solid;
            cmbMovement.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbMovement.DrawDropdownHoverOutline = false;
            cmbMovement.DrawFocusRectangle = false;
            cmbMovement.DrawMode = DrawMode.OwnerDrawFixed;
            cmbMovement.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMovement.FlatStyle = FlatStyle.Flat;
            cmbMovement.ForeColor = System.Drawing.Color.Gainsboro;
            cmbMovement.FormattingEnabled = true;
            cmbMovement.Items.AddRange(new object[] { "Move Randomly", "Turn Randomly", "No Movement" });
            cmbMovement.Location = new System.Drawing.Point(119, 147);
            cmbMovement.Margin = new Padding(4, 3, 4, 3);
            cmbMovement.Name = "cmbMovement";
            cmbMovement.Size = new Size(135, 24);
            cmbMovement.TabIndex = 64;
            cmbMovement.Text = "Move Randomly";
            cmbMovement.TextPadding = new Padding(2);
            cmbMovement.SelectedIndexChanged += cmbMovement_SelectedIndexChanged;
            // 
            // chkAggressive
            // 
            chkAggressive.AutoSize = true;
            chkAggressive.Location = new System.Drawing.Point(15, 20);
            chkAggressive.Margin = new Padding(4, 3, 4, 3);
            chkAggressive.Name = "chkAggressive";
            chkAggressive.Size = new Size(83, 19);
            chkAggressive.TabIndex = 1;
            chkAggressive.Text = "Aggressive";
            chkAggressive.CheckedChanged += chkAggressive_CheckedChanged;
            // 
            // grpRegen
            // 
            grpRegen.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpRegen.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpRegen.Controls.Add(nudMpRegen);
            grpRegen.Controls.Add(nudHpRegen);
            grpRegen.Controls.Add(lblHpRegen);
            grpRegen.Controls.Add(lblManaRegen);
            grpRegen.Controls.Add(lblRegenHint);
            grpRegen.ForeColor = System.Drawing.Color.Gainsboro;
            grpRegen.Location = new System.Drawing.Point(259, 6);
            grpRegen.Margin = new Padding(2);
            grpRegen.Name = "grpRegen";
            grpRegen.Padding = new Padding(2);
            grpRegen.Size = new Size(268, 136);
            grpRegen.TabIndex = 31;
            grpRegen.TabStop = false;
            grpRegen.Text = "Regen";
            // 
            // nudMpRegen
            // 
            nudMpRegen.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMpRegen.ForeColor = System.Drawing.Color.Gainsboro;
            nudMpRegen.Location = new System.Drawing.Point(140, 47);
            nudMpRegen.Margin = new Padding(4, 3, 4, 3);
            nudMpRegen.Name = "nudMpRegen";
            nudMpRegen.Size = new Size(117, 23);
            nudMpRegen.TabIndex = 31;
            nudMpRegen.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMpRegen.ValueChanged += nudMpRegen_ValueChanged;
            // 
            // nudHpRegen
            // 
            nudHpRegen.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudHpRegen.ForeColor = System.Drawing.Color.Gainsboro;
            nudHpRegen.Location = new System.Drawing.Point(13, 47);
            nudHpRegen.Margin = new Padding(4, 3, 4, 3);
            nudHpRegen.Name = "nudHpRegen";
            nudHpRegen.Size = new Size(117, 23);
            nudHpRegen.TabIndex = 30;
            nudHpRegen.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudHpRegen.ValueChanged += nudHpRegen_ValueChanged;
            // 
            // lblHpRegen
            // 
            lblHpRegen.AutoSize = true;
            lblHpRegen.Location = new System.Drawing.Point(9, 25);
            lblHpRegen.Margin = new Padding(2, 0, 2, 0);
            lblHpRegen.Name = "lblHpRegen";
            lblHpRegen.Size = new Size(47, 15);
            lblHpRegen.TabIndex = 26;
            lblHpRegen.Text = "HP: (%)";
            // 
            // lblManaRegen
            // 
            lblManaRegen.AutoSize = true;
            lblManaRegen.Location = new System.Drawing.Point(136, 25);
            lblManaRegen.Margin = new Padding(2, 0, 2, 0);
            lblManaRegen.Name = "lblManaRegen";
            lblManaRegen.Size = new Size(61, 15);
            lblManaRegen.TabIndex = 27;
            lblManaRegen.Text = "Mana: (%)";
            // 
            // lblRegenHint
            // 
            lblRegenHint.Location = new System.Drawing.Point(7, 80);
            lblRegenHint.Margin = new Padding(4, 0, 4, 0);
            lblRegenHint.Name = "lblRegenHint";
            lblRegenHint.Size = new Size(250, 51);
            lblRegenHint.TabIndex = 0;
            lblRegenHint.Text = "% of HP/Mana to restore per tick.\r\n\r\nTick timer saved in server config.json.";
            // 
            // grpDrops
            // 
            grpDrops.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpDrops.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpDrops.Controls.Add(nudDropMinAmount);
            grpDrops.Controls.Add(lblDropMinAmount);
            grpDrops.Controls.Add(chkIndividualLoot);
            grpDrops.Controls.Add(btnDropRemove);
            grpDrops.Controls.Add(btnDropAdd);
            grpDrops.Controls.Add(lstDrops);
            grpDrops.Controls.Add(nudDropMaxAmount);
            grpDrops.Controls.Add(nudDropChance);
            grpDrops.Controls.Add(cmbDropItem);
            grpDrops.Controls.Add(lblDropMaxAmount);
            grpDrops.Controls.Add(lblDropChance);
            grpDrops.Controls.Add(lblDropItem);
            grpDrops.ForeColor = System.Drawing.Color.Gainsboro;
            grpDrops.Location = new System.Drawing.Point(261, 408);
            grpDrops.Margin = new Padding(4, 3, 4, 3);
            grpDrops.Name = "grpDrops";
            grpDrops.Padding = new Padding(4, 3, 4, 3);
            grpDrops.Size = new Size(266, 329);
            grpDrops.TabIndex = 30;
            grpDrops.TabStop = false;
            grpDrops.Text = "Drops";
            // 
            // nudDropMinAmount
            // 
            nudDropMinAmount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDropMinAmount.ForeColor = System.Drawing.Color.Gainsboro;
            nudDropMinAmount.Location = new System.Drawing.Point(20, 176);
            nudDropMinAmount.Margin = new Padding(4, 3, 4, 3);
            nudDropMinAmount.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            nudDropMinAmount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDropMinAmount.Name = "nudDropMinAmount";
            nudDropMinAmount.Size = new Size(86, 23);
            nudDropMinAmount.TabIndex = 80;
            nudDropMinAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudDropMinAmount.ValueChanged += nudDropMinAmount_ValueChanged;
            // 
            // lblDropMinAmount
            // 
            lblDropMinAmount.AutoSize = true;
            lblDropMinAmount.Location = new System.Drawing.Point(16, 156);
            lblDropMinAmount.Margin = new Padding(4, 0, 4, 0);
            lblDropMinAmount.Name = "lblDropMinAmount";
            lblDropMinAmount.Size = new Size(78, 15);
            lblDropMinAmount.TabIndex = 79;
            lblDropMinAmount.Text = "Min Amount:";
            // 
            // chkIndividualLoot
            // 
            chkIndividualLoot.AutoSize = true;
            chkIndividualLoot.Location = new System.Drawing.Point(20, 265);
            chkIndividualLoot.Margin = new Padding(4, 3, 4, 3);
            chkIndividualLoot.Name = "chkIndividualLoot";
            chkIndividualLoot.Size = new Size(178, 19);
            chkIndividualLoot.TabIndex = 78;
            chkIndividualLoot.Text = "Spawn Loot for all Attackers?";
            chkIndividualLoot.CheckedChanged += chkIndividualLoot_CheckedChanged;
            // 
            // btnDropRemove
            // 
            btnDropRemove.Location = new System.Drawing.Point(167, 295);
            btnDropRemove.Margin = new Padding(4, 3, 4, 3);
            btnDropRemove.Name = "btnDropRemove";
            btnDropRemove.Padding = new Padding(6);
            btnDropRemove.Size = new Size(88, 27);
            btnDropRemove.TabIndex = 64;
            btnDropRemove.Text = "Remove";
            btnDropRemove.Click += btnDropRemove_Click;
            // 
            // btnDropAdd
            // 
            btnDropAdd.Location = new System.Drawing.Point(18, 295);
            btnDropAdd.Margin = new Padding(4, 3, 4, 3);
            btnDropAdd.Name = "btnDropAdd";
            btnDropAdd.Padding = new Padding(6);
            btnDropAdd.Size = new Size(88, 27);
            btnDropAdd.TabIndex = 63;
            btnDropAdd.Text = "Add";
            btnDropAdd.Click += btnDropAdd_Click;
            // 
            // lstDrops
            // 
            lstDrops.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstDrops.BorderStyle = BorderStyle.FixedSingle;
            lstDrops.ForeColor = System.Drawing.Color.Gainsboro;
            lstDrops.FormattingEnabled = true;
            lstDrops.ItemHeight = 15;
            lstDrops.Location = new System.Drawing.Point(18, 22);
            lstDrops.Margin = new Padding(4, 3, 4, 3);
            lstDrops.Name = "lstDrops";
            lstDrops.Size = new Size(236, 77);
            lstDrops.TabIndex = 62;
            lstDrops.SelectedIndexChanged += lstDrops_SelectedIndexChanged;
            // 
            // nudDropMaxAmount
            // 
            nudDropMaxAmount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDropMaxAmount.ForeColor = System.Drawing.Color.Gainsboro;
            nudDropMaxAmount.Location = new System.Drawing.Point(167, 176);
            nudDropMaxAmount.Margin = new Padding(4, 3, 4, 3);
            nudDropMaxAmount.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            nudDropMaxAmount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDropMaxAmount.Name = "nudDropMaxAmount";
            nudDropMaxAmount.Size = new Size(86, 23);
            nudDropMaxAmount.TabIndex = 61;
            nudDropMaxAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudDropMaxAmount.ValueChanged += nudDropMaxAmount_ValueChanged;
            // 
            // nudDropChance
            // 
            nudDropChance.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDropChance.DecimalPlaces = 2;
            nudDropChance.ForeColor = System.Drawing.Color.Gainsboro;
            nudDropChance.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudDropChance.Location = new System.Drawing.Point(20, 231);
            nudDropChance.Margin = new Padding(4, 3, 4, 3);
            nudDropChance.Name = "nudDropChance";
            nudDropChance.Size = new Size(234, 23);
            nudDropChance.TabIndex = 60;
            nudDropChance.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudDropChance.ValueChanged += nudDropChance_ValueChanged;
            // 
            // cmbDropItem
            // 
            cmbDropItem.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbDropItem.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbDropItem.BorderStyle = ButtonBorderStyle.Solid;
            cmbDropItem.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbDropItem.DrawDropdownHoverOutline = false;
            cmbDropItem.DrawFocusRectangle = false;
            cmbDropItem.DrawMode = DrawMode.OwnerDrawFixed;
            cmbDropItem.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDropItem.FlatStyle = FlatStyle.Flat;
            cmbDropItem.ForeColor = System.Drawing.Color.Gainsboro;
            cmbDropItem.FormattingEnabled = true;
            cmbDropItem.Location = new System.Drawing.Point(20, 127);
            cmbDropItem.Margin = new Padding(4, 3, 4, 3);
            cmbDropItem.Name = "cmbDropItem";
            cmbDropItem.Size = new Size(234, 24);
            cmbDropItem.TabIndex = 17;
            cmbDropItem.Text = null;
            cmbDropItem.TextPadding = new Padding(2);
            cmbDropItem.SelectedIndexChanged += cmbDropItem_SelectedIndexChanged;
            // 
            // lblDropMaxAmount
            // 
            lblDropMaxAmount.AutoSize = true;
            lblDropMaxAmount.Location = new System.Drawing.Point(163, 156);
            lblDropMaxAmount.Margin = new Padding(4, 0, 4, 0);
            lblDropMaxAmount.Name = "lblDropMaxAmount";
            lblDropMaxAmount.Size = new Size(80, 15);
            lblDropMaxAmount.TabIndex = 15;
            lblDropMaxAmount.Text = "Max Amount:";
            // 
            // lblDropChance
            // 
            lblDropChance.AutoSize = true;
            lblDropChance.Location = new System.Drawing.Point(18, 207);
            lblDropChance.Margin = new Padding(4, 0, 4, 0);
            lblDropChance.Name = "lblDropChance";
            lblDropChance.Size = new Size(71, 15);
            lblDropChance.TabIndex = 13;
            lblDropChance.Text = "Chance (%):";
            // 
            // lblDropItem
            // 
            lblDropItem.AutoSize = true;
            lblDropItem.Location = new System.Drawing.Point(14, 106);
            lblDropItem.Margin = new Padding(4, 0, 4, 0);
            lblDropItem.Name = "lblDropItem";
            lblDropItem.Size = new Size(34, 15);
            lblDropItem.TabIndex = 11;
            lblDropItem.Text = "Item:";
            // 
            // grpNpcVsNpc
            // 
            grpNpcVsNpc.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpNpcVsNpc.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpNpcVsNpc.Controls.Add(cmbHostileNPC);
            grpNpcVsNpc.Controls.Add(lblNPC);
            grpNpcVsNpc.Controls.Add(btnRemoveAggro);
            grpNpcVsNpc.Controls.Add(btnAddAggro);
            grpNpcVsNpc.Controls.Add(lstAggro);
            grpNpcVsNpc.Controls.Add(chkAttackAllies);
            grpNpcVsNpc.Controls.Add(chkEnabled);
            grpNpcVsNpc.ForeColor = System.Drawing.Color.Gainsboro;
            grpNpcVsNpc.Location = new System.Drawing.Point(844, 404);
            grpNpcVsNpc.Margin = new Padding(4, 3, 4, 3);
            grpNpcVsNpc.Name = "grpNpcVsNpc";
            grpNpcVsNpc.Padding = new Padding(4, 3, 4, 3);
            grpNpcVsNpc.Size = new Size(264, 333);
            grpNpcVsNpc.TabIndex = 29;
            grpNpcVsNpc.TabStop = false;
            grpNpcVsNpc.Text = "NPC vs NPC Combat/Hostility ";
            // 
            // cmbHostileNPC
            // 
            cmbHostileNPC.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbHostileNPC.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbHostileNPC.BorderStyle = ButtonBorderStyle.Solid;
            cmbHostileNPC.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbHostileNPC.DrawDropdownHoverOutline = false;
            cmbHostileNPC.DrawFocusRectangle = false;
            cmbHostileNPC.DrawMode = DrawMode.OwnerDrawFixed;
            cmbHostileNPC.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHostileNPC.FlatStyle = FlatStyle.Flat;
            cmbHostileNPC.ForeColor = System.Drawing.Color.Gainsboro;
            cmbHostileNPC.FormattingEnabled = true;
            cmbHostileNPC.Location = new System.Drawing.Point(12, 97);
            cmbHostileNPC.Margin = new Padding(4, 3, 4, 3);
            cmbHostileNPC.Name = "cmbHostileNPC";
            cmbHostileNPC.Size = new Size(242, 24);
            cmbHostileNPC.TabIndex = 45;
            cmbHostileNPC.Text = null;
            cmbHostileNPC.TextPadding = new Padding(2);
            // 
            // lblNPC
            // 
            lblNPC.AutoSize = true;
            lblNPC.Location = new System.Drawing.Point(15, 75);
            lblNPC.Margin = new Padding(4, 0, 4, 0);
            lblNPC.Name = "lblNPC";
            lblNPC.Size = new Size(34, 15);
            lblNPC.TabIndex = 44;
            lblNPC.Text = "NPC:";
            // 
            // btnRemoveAggro
            // 
            btnRemoveAggro.Location = new System.Drawing.Point(167, 300);
            btnRemoveAggro.Margin = new Padding(4, 3, 4, 3);
            btnRemoveAggro.Name = "btnRemoveAggro";
            btnRemoveAggro.Padding = new Padding(6);
            btnRemoveAggro.Size = new Size(88, 27);
            btnRemoveAggro.TabIndex = 43;
            btnRemoveAggro.Text = "Remove";
            btnRemoveAggro.Click += btnRemoveAggro_Click;
            // 
            // btnAddAggro
            // 
            btnAddAggro.Location = new System.Drawing.Point(12, 300);
            btnAddAggro.Margin = new Padding(4, 3, 4, 3);
            btnAddAggro.Name = "btnAddAggro";
            btnAddAggro.Padding = new Padding(6);
            btnAddAggro.Size = new Size(88, 27);
            btnAddAggro.TabIndex = 42;
            btnAddAggro.Text = "Add";
            btnAddAggro.Click += btnAddAggro_Click;
            // 
            // lstAggro
            // 
            lstAggro.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstAggro.BorderStyle = BorderStyle.FixedSingle;
            lstAggro.ForeColor = System.Drawing.Color.Gainsboro;
            lstAggro.FormattingEnabled = true;
            lstAggro.ItemHeight = 15;
            lstAggro.Items.AddRange(new object[] { "NPC:" });
            lstAggro.Location = new System.Drawing.Point(12, 137);
            lstAggro.Margin = new Padding(4, 3, 4, 3);
            lstAggro.Name = "lstAggro";
            lstAggro.Size = new Size(242, 152);
            lstAggro.TabIndex = 41;
            // 
            // chkAttackAllies
            // 
            chkAttackAllies.AutoSize = true;
            chkAttackAllies.Location = new System.Drawing.Point(12, 46);
            chkAttackAllies.Margin = new Padding(4, 3, 4, 3);
            chkAttackAllies.Name = "chkAttackAllies";
            chkAttackAllies.Size = new Size(96, 19);
            chkAttackAllies.TabIndex = 1;
            chkAttackAllies.Text = "Attack Allies?";
            chkAttackAllies.CheckedChanged += chkAttackAllies_CheckedChanged;
            // 
            // chkEnabled
            // 
            chkEnabled.AutoSize = true;
            chkEnabled.Location = new System.Drawing.Point(12, 20);
            chkEnabled.Margin = new Padding(4, 3, 4, 3);
            chkEnabled.Name = "chkEnabled";
            chkEnabled.Size = new Size(73, 19);
            chkEnabled.TabIndex = 0;
            chkEnabled.Text = "Enabled?";
            chkEnabled.CheckedChanged += chkEnabled_CheckedChanged;
            // 
            // grpSpells
            // 
            grpSpells.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpSpells.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpSpells.Controls.Add(cmbSpell);
            grpSpells.Controls.Add(cmbFreq);
            grpSpells.Controls.Add(lblFreq);
            grpSpells.Controls.Add(lblSpell);
            grpSpells.Controls.Add(btnRemove);
            grpSpells.Controls.Add(btnAdd);
            grpSpells.Controls.Add(lstSpells);
            grpSpells.ForeColor = System.Drawing.Color.Gainsboro;
            grpSpells.Location = new System.Drawing.Point(261, 148);
            grpSpells.Margin = new Padding(4, 3, 4, 3);
            grpSpells.Name = "grpSpells";
            grpSpells.Padding = new Padding(4, 3, 4, 3);
            grpSpells.Size = new Size(266, 249);
            grpSpells.TabIndex = 28;
            grpSpells.TabStop = false;
            grpSpells.Text = "Spells";
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
            cmbSpell.Location = new System.Drawing.Point(18, 144);
            cmbSpell.Margin = new Padding(4, 3, 4, 3);
            cmbSpell.Name = "cmbSpell";
            cmbSpell.Size = new Size(236, 24);
            cmbSpell.TabIndex = 43;
            cmbSpell.Text = null;
            cmbSpell.TextPadding = new Padding(2);
            cmbSpell.SelectedIndexChanged += cmbSpell_SelectedIndexChanged;
            // 
            // cmbFreq
            // 
            cmbFreq.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbFreq.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbFreq.BorderStyle = ButtonBorderStyle.Solid;
            cmbFreq.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbFreq.DrawDropdownHoverOutline = false;
            cmbFreq.DrawFocusRectangle = false;
            cmbFreq.DrawMode = DrawMode.OwnerDrawFixed;
            cmbFreq.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFreq.FlatStyle = FlatStyle.Flat;
            cmbFreq.ForeColor = System.Drawing.Color.Gainsboro;
            cmbFreq.FormattingEnabled = true;
            cmbFreq.Items.AddRange(new object[] { "Not Very Often", "Not Often", "Normal", "Often", "Very Often" });
            cmbFreq.Location = new System.Drawing.Point(92, 216);
            cmbFreq.Margin = new Padding(4, 3, 4, 3);
            cmbFreq.Name = "cmbFreq";
            cmbFreq.Size = new Size(162, 24);
            cmbFreq.TabIndex = 42;
            cmbFreq.Text = "Not Very Often";
            cmbFreq.TextPadding = new Padding(2);
            cmbFreq.SelectedIndexChanged += cmbFreq_SelectedIndexChanged;
            // 
            // lblFreq
            // 
            lblFreq.AutoSize = true;
            lblFreq.Location = new System.Drawing.Point(14, 219);
            lblFreq.Margin = new Padding(4, 0, 4, 0);
            lblFreq.Name = "lblFreq";
            lblFreq.Size = new Size(65, 15);
            lblFreq.TabIndex = 41;
            lblFreq.Text = "Frequence:";
            // 
            // lblSpell
            // 
            lblSpell.AutoSize = true;
            lblSpell.Location = new System.Drawing.Point(16, 123);
            lblSpell.Margin = new Padding(4, 0, 4, 0);
            lblSpell.Name = "lblSpell";
            lblSpell.Size = new Size(35, 15);
            lblSpell.TabIndex = 39;
            lblSpell.Text = "Spell:";
            // 
            // btnRemove
            // 
            btnRemove.Location = new System.Drawing.Point(167, 178);
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
            btnAdd.Location = new System.Drawing.Point(18, 178);
            btnAdd.Margin = new Padding(4, 3, 4, 3);
            btnAdd.Name = "btnAdd";
            btnAdd.Padding = new Padding(6);
            btnAdd.Size = new Size(88, 27);
            btnAdd.TabIndex = 37;
            btnAdd.Text = "Add";
            btnAdd.Click += btnAdd_Click;
            // 
            // lstSpells
            // 
            lstSpells.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstSpells.BorderStyle = BorderStyle.FixedSingle;
            lstSpells.ForeColor = System.Drawing.Color.Gainsboro;
            lstSpells.FormattingEnabled = true;
            lstSpells.ItemHeight = 15;
            lstSpells.Location = new System.Drawing.Point(18, 22);
            lstSpells.Margin = new Padding(4, 3, 4, 3);
            lstSpells.Name = "lstSpells";
            lstSpells.Size = new Size(236, 92);
            lstSpells.TabIndex = 29;
            lstSpells.SelectedIndexChanged += lstSpells_SelectedIndexChanged;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(1168, 687);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(222, 31);
            btnCancel.TabIndex = 21;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(929, 687);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(222, 31);
            btnSave.TabIndex = 18;
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
            toolStrip.Size = new Size(1393, 29);
            toolStrip.TabIndex = 45;
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
            // FrmNpc
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            ClientSize = new Size(1393, 728);
            ControlBox = true;
            Controls.Add(toolStrip);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(grpNpcs);
            Controls.Add(pnlContainer);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmNpc";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "NPC Editor";
            FormClosed += frmNpc_FormClosed;
            Load += frmNpc_Load;
            KeyDown += form_KeyDown;
            grpNpcs.ResumeLayout(false);
            grpNpcs.PerformLayout();
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudRgbaA).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRgbaB).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRgbaG).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudRgbaR).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)picNpc).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudSpawnDuration).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudSightRange).EndInit();
            grpStats.ResumeLayout(false);
            grpStats.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudExp).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMana).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudHp).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudSpd).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMR).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDef).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMag).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudStr).EndInit();
            pnlContainer.ResumeLayout(false);
            grpImmunities.ResumeLayout(false);
            grpImmunities.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudTenacity).EndInit();
            grpCombat.ResumeLayout(false);
            grpCombat.PerformLayout();
            grpAttackSpeed.ResumeLayout(false);
            grpAttackSpeed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudAttackSpeedValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCritMultiplier).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudScaling).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDamage).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudCritChance).EndInit();
            grpCommonEvents.ResumeLayout(false);
            grpCommonEvents.PerformLayout();
            grpBehavior.ResumeLayout(false);
            grpBehavior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudResetRadius).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudFlee).EndInit();
            grpConditions.ResumeLayout(false);
            grpRegen.ResumeLayout(false);
            grpRegen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudMpRegen).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudHpRegen).EndInit();
            grpDrops.ResumeLayout(false);
            grpDrops.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudDropMinAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDropMaxAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDropChance).EndInit();
            grpNpcVsNpc.ResumeLayout(false);
            grpNpcVsNpc.PerformLayout();
            grpSpells.ResumeLayout(false);
            grpSpells.PerformLayout();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private DarkGroupBox grpNpcs;
        private DarkGroupBox grpGeneral;
        private DarkComboBox cmbSprite;
        private System.Windows.Forms.Label lblSpawnDuration;
        private System.Windows.Forms.Label lblPic;
        private System.Windows.Forms.PictureBox picNpc;
        private System.Windows.Forms.Label lblName;
        private DarkTextBox txtName;
        private DarkGroupBox grpStats;
        private System.Windows.Forms.Label lblMana;
        private System.Windows.Forms.Label lblHP;
        private System.Windows.Forms.Label lblExp;
        private System.Windows.Forms.Label lblSightRange;
        private System.Windows.Forms.Panel pnlContainer;
        private DarkButton btnSave;
        private DarkButton btnCancel;
        private DarkGroupBox grpSpells;
        private DarkButton btnRemove;
        private DarkButton btnAdd;
        private System.Windows.Forms.ListBox lstSpells;
        private System.Windows.Forms.Label lblSpell;
        private DarkComboBox cmbFreq;
        private System.Windows.Forms.Label lblFreq;
        private DarkGroupBox grpNpcVsNpc;
        private System.Windows.Forms.Label lblNPC;
        private DarkButton btnRemoveAggro;
        private DarkButton btnAddAggro;
        private System.Windows.Forms.ListBox lstAggro;
        private DarkCheckBox chkAttackAllies;
        private DarkCheckBox chkEnabled;
        private DarkToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripItemNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripItemDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton toolStripItemCopy;
        public System.Windows.Forms.ToolStripButton toolStripItemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.ToolStripButton toolStripItemUndo;
        private DarkGroupBox grpCombat;
        private DarkComboBox cmbScalingStat;
        private System.Windows.Forms.Label lblScalingStat;
        private System.Windows.Forms.Label lblScaling;
        private DarkComboBox cmbDamageType;
        private System.Windows.Forms.Label lblDamageType;
        private System.Windows.Forms.Label lblCritChance;
        private DarkComboBox cmbAttackAnimation;
        private System.Windows.Forms.Label lblAttackAnimation;
        private System.Windows.Forms.Label lblDamage;
        private DarkComboBox cmbHostileNPC;
        private DarkComboBox cmbSpell;
        private DarkNumericUpDown nudSpd;
        private DarkNumericUpDown nudMR;
        private DarkNumericUpDown nudDef;
        private DarkNumericUpDown nudMag;
        private DarkNumericUpDown nudStr;
        private System.Windows.Forms.Label lblSpd;
        private System.Windows.Forms.Label lblMR;
        private System.Windows.Forms.Label lblDef;
        private System.Windows.Forms.Label lblMag;
        private System.Windows.Forms.Label lblStr;
        private DarkNumericUpDown nudScaling;
        private DarkNumericUpDown nudDamage;
        private DarkNumericUpDown nudCritChance;
        private DarkNumericUpDown nudSightRange;
        private DarkNumericUpDown nudSpawnDuration;
        private DarkNumericUpDown nudMana;
        private DarkNumericUpDown nudHp;
        private DarkNumericUpDown nudExp;
        private System.Windows.Forms.Label lblLevel;
        private DarkNumericUpDown nudLevel;
        private DarkGroupBox grpDrops;
        private DarkButton btnDropRemove;
        private DarkButton btnDropAdd;
        private System.Windows.Forms.ListBox lstDrops;
        private DarkNumericUpDown nudDropMaxAmount;
        private DarkNumericUpDown nudDropChance;
        private DarkComboBox cmbDropItem;
        private System.Windows.Forms.Label lblDropMaxAmount;
        private System.Windows.Forms.Label lblDropChance;
        private System.Windows.Forms.Label lblDropItem;
        private DarkGroupBox grpRegen;
        private DarkNumericUpDown nudMpRegen;
        private DarkNumericUpDown nudHpRegen;
        private System.Windows.Forms.Label lblHpRegen;
        private System.Windows.Forms.Label lblManaRegen;
        private System.Windows.Forms.Label lblRegenHint;
        private DarkGroupBox grpCommonEvents;
        private DarkGroupBox grpBehavior;
        private DarkCheckBox chkSwarm;
        private DarkGroupBox grpConditions;
        private DarkButton btnAttackOnSightCond;
        private DarkButton btnPlayerCanAttackCond;
        private DarkButton btnPlayerFriendProtectorCond;
        private System.Windows.Forms.Label lblMovement;
        private DarkComboBox cmbMovement;
        private DarkCheckBox chkAggressive;
        private DarkComboBox cmbOnDeathEventParty;
        private System.Windows.Forms.Label lblOnDeathEventParty;
        private DarkComboBox cmbOnDeathEventKiller;
        private System.Windows.Forms.Label lblOnDeathEventKiller;
        private DarkNumericUpDown nudFlee;
        private System.Windows.Forms.Label lblFlee;
        private DarkCheckBox chkFocusDamageDealer;
        private DarkNumericUpDown nudCritMultiplier;
        private System.Windows.Forms.Label lblCritMultiplier;
        private DarkButton btnClearSearch;
        private DarkTextBox txtSearch;
        private DarkButton btnAddFolder;
        private System.Windows.Forms.Label lblFolder;
        private DarkComboBox cmbFolder;
        private System.Windows.Forms.ToolStripButton btnAlphabetical;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private DarkGroupBox grpAttackSpeed;
        private DarkNumericUpDown nudAttackSpeedValue;
        private System.Windows.Forms.Label lblAttackSpeedValue;
        private DarkComboBox cmbAttackSpeedModifier;
        private System.Windows.Forms.Label lblAttackSpeedModifier;
        private DarkNumericUpDown nudRgbaA;
        private DarkNumericUpDown nudRgbaB;
        private DarkNumericUpDown nudRgbaG;
        private DarkNumericUpDown nudRgbaR;
        private System.Windows.Forms.Label lblAlpha;
        private System.Windows.Forms.Label lblBlue;
        private System.Windows.Forms.Label lblGreen;
        private System.Windows.Forms.Label lblRed;
        private DarkNumericUpDown nudResetRadius;
        private System.Windows.Forms.Label lblResetRadius;
        private DarkCheckBox chkIndividualLoot;
        private Controls.GameObjectList lstGameObjects;
        private DarkGroupBox grpImmunities;
        private DarkNumericUpDown nudTenacity;
        private System.Windows.Forms.Label lblTenacity;
        private DarkCheckBox chkTaunt;
        private DarkCheckBox chkSleep;
        private DarkCheckBox chkTransform;
        private DarkCheckBox chkBlind;
        private DarkCheckBox chkSnare;
        private DarkCheckBox chkStun;
        private DarkCheckBox chkSilence;
        private DarkCheckBox chkKnockback;
        private DarkNumericUpDown nudDropMinAmount;
        private Label lblDropMinAmount;
    }
}