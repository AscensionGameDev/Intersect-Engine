using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
{
    partial class FrmResource
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmResource));
            grpResources = new DarkGroupBox();
            btnClearSearch = new DarkButton();
            txtSearch = new DarkTextBox();
            lstGameObjects = new Controls.GameObjectList();
            grpGeneral = new DarkGroupBox();
            nudHpRegen = new DarkNumericUpDown();
            chkUseExplicitMaxHealthForResourceStates = new DarkCheckBox();
            lblHpRegen = new Label();
            btnAddFolder = new DarkButton();
            lblFolder = new Label();
            cmbFolder = new DarkComboBox();
            nudMaxHp = new DarkNumericUpDown();
            nudMinHp = new DarkNumericUpDown();
            nudSpawnDuration = new DarkNumericUpDown();
            cmbDeathAnimation = new DarkComboBox();
            lblDeathAnimation = new Label();
            lblMaxHp = new Label();
            lblSpawnDuration = new Label();
            chkWalkableAfter = new DarkCheckBox();
            chkWalkableBefore = new DarkCheckBox();
            cmbToolType = new DarkComboBox();
            lblToolType = new Label();
            lblHP = new Label();
            lblName = new Label();
            txtName = new DarkTextBox();
            btnRequirements = new DarkButton();
            grpGraphics = new DarkGroupBox();
            btnRemoveState = new DarkButton();
            btnAddState = new DarkButton();
            lblStateName = new Label();
            lstStates = new ListBox();
            txtStateName = new DarkTextBox();
            grpGraphicData = new DarkGroupBox();
            nudStateRangeMax = new DarkNumericUpDown();
            nudStateRangeMin = new DarkNumericUpDown();
            lblStateRange = new Label();
            cmbTextureSource = new DarkComboBox();
            lblTerxtureSource = new Label();
            cmbAnimation = new DarkComboBox();
            lblAnimation = new Label();
            chkRenderBelowEntity = new DarkCheckBox();
            cmbTextureType = new DarkComboBox();
            lblTextureType = new Label();
            lblStates = new Label();
            graphicContainer = new Panel();
            picResource = new PictureBox();
            tmrRender = new System.Windows.Forms.Timer(components);
            pnlContainer = new Panel();
            grpRequirements = new DarkGroupBox();
            lblCannotHarvest = new Label();
            txtCannotHarvest = new DarkTextBox();
            grpCommonEvent = new DarkGroupBox();
            cmbEvent = new DarkComboBox();
            lblEvent = new Label();
            grpDrops = new DarkGroupBox();
            nudDropMinAmount = new DarkNumericUpDown();
            lblDropMinAmount = new Label();
            btnDropRemove = new DarkButton();
            btnDropAdd = new DarkButton();
            lstDrops = new ListBox();
            nudDropMaxAmount = new DarkNumericUpDown();
            nudDropChance = new DarkNumericUpDown();
            cmbDropItem = new DarkComboBox();
            lblDropMaxAmount = new Label();
            lblDropChance = new Label();
            lblDropItem = new Label();
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
            grpResources.SuspendLayout();
            grpGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudHpRegen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxHp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMinHp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudSpawnDuration).BeginInit();
            grpGraphics.SuspendLayout();
            grpGraphicData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudStateRangeMax).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudStateRangeMin).BeginInit();
            graphicContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picResource).BeginInit();
            pnlContainer.SuspendLayout();
            grpRequirements.SuspendLayout();
            grpCommonEvent.SuspendLayout();
            grpDrops.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudDropMinAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDropMaxAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDropChance).BeginInit();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // grpResources
            // 
            grpResources.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpResources.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpResources.Controls.Add(btnClearSearch);
            grpResources.Controls.Add(txtSearch);
            grpResources.Controls.Add(lstGameObjects);
            grpResources.ForeColor = System.Drawing.Color.Gainsboro;
            grpResources.Location = new System.Drawing.Point(14, 45);
            grpResources.Margin = new Padding(4, 3, 4, 3);
            grpResources.Name = "grpResources";
            grpResources.Padding = new Padding(4, 3, 4, 3);
            grpResources.Size = new Size(237, 635);
            grpResources.TabIndex = 14;
            grpResources.TabStop = false;
            grpResources.Text = "Resources";
            // 
            // btnClearSearch
            // 
            btnClearSearch.Location = new System.Drawing.Point(209, 22);
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
            txtSearch.Location = new System.Drawing.Point(7, 22);
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
            lstGameObjects.Location = new System.Drawing.Point(7, 52);
            lstGameObjects.Margin = new Padding(4, 3, 4, 3);
            lstGameObjects.Name = "lstGameObjects";
            lstGameObjects.SelectedImageIndex = 0;
            lstGameObjects.Size = new Size(223, 572);
            lstGameObjects.TabIndex = 32;
            // 
            // grpGeneral
            // 
            grpGeneral.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpGeneral.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpGeneral.Controls.Add(nudHpRegen);
            grpGeneral.Controls.Add(chkUseExplicitMaxHealthForResourceStates);
            grpGeneral.Controls.Add(lblHpRegen);
            grpGeneral.Controls.Add(btnAddFolder);
            grpGeneral.Controls.Add(lblFolder);
            grpGeneral.Controls.Add(cmbFolder);
            grpGeneral.Controls.Add(nudMaxHp);
            grpGeneral.Controls.Add(nudMinHp);
            grpGeneral.Controls.Add(nudSpawnDuration);
            grpGeneral.Controls.Add(cmbDeathAnimation);
            grpGeneral.Controls.Add(lblDeathAnimation);
            grpGeneral.Controls.Add(lblMaxHp);
            grpGeneral.Controls.Add(lblSpawnDuration);
            grpGeneral.Controls.Add(chkWalkableAfter);
            grpGeneral.Controls.Add(chkWalkableBefore);
            grpGeneral.Controls.Add(cmbToolType);
            grpGeneral.Controls.Add(lblToolType);
            grpGeneral.Controls.Add(lblHP);
            grpGeneral.Controls.Add(lblName);
            grpGeneral.Controls.Add(txtName);
            grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
            grpGeneral.Location = new System.Drawing.Point(0, 0);
            grpGeneral.Margin = new Padding(4, 3, 4, 3);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Padding = new Padding(4, 3, 4, 3);
            grpGeneral.Size = new Size(260, 380);
            grpGeneral.TabIndex = 15;
            grpGeneral.TabStop = false;
            grpGeneral.Text = "General";
            // 
            // nudHpRegen
            // 
            nudHpRegen.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudHpRegen.ForeColor = System.Drawing.Color.Gainsboro;
            nudHpRegen.Location = new System.Drawing.Point(108, 173);
            nudHpRegen.Margin = new Padding(4, 3, 4, 3);
            nudHpRegen.Name = "nudHpRegen";
            nudHpRegen.Size = new Size(138, 23);
            nudHpRegen.TabIndex = 30;
            nudHpRegen.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudHpRegen.ValueChanged += nudHpRegen_ValueChanged;
            // 
            // chkUseExplicitMaxHealthForResourceStates
            // 
            chkUseExplicitMaxHealthForResourceStates.Location = new System.Drawing.Point(7, 339);
            chkUseExplicitMaxHealthForResourceStates.Margin = new Padding(4, 3, 4, 3);
            chkUseExplicitMaxHealthForResourceStates.Name = "chkUseExplicitMaxHealthForResourceStates";
            chkUseExplicitMaxHealthForResourceStates.Size = new Size(246, 34);
            chkUseExplicitMaxHealthForResourceStates.TabIndex = 53;
            chkUseExplicitMaxHealthForResourceStates.Text = "Use explicit Max Health for Resources States?";
            chkUseExplicitMaxHealthForResourceStates.CheckedChanged += chkUseExplicitMaxHealthForResourceStates_CheckedChanged;
            // 
            // lblHpRegen
            // 
            lblHpRegen.AutoSize = true;
            lblHpRegen.Location = new System.Drawing.Point(7, 175);
            lblHpRegen.Margin = new Padding(2, 0, 2, 0);
            lblHpRegen.Name = "lblHpRegen";
            lblHpRegen.Size = new Size(83, 15);
            lblHpRegen.TabIndex = 26;
            lblHpRegen.Text = "HP Regen (%):";
            // 
            // btnAddFolder
            // 
            btnAddFolder.Location = new System.Drawing.Point(224, 52);
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
            lblFolder.Location = new System.Drawing.Point(7, 55);
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
            cmbFolder.Location = new System.Drawing.Point(88, 52);
            cmbFolder.Margin = new Padding(4, 3, 4, 3);
            cmbFolder.Name = "cmbFolder";
            cmbFolder.Size = new Size(131, 24);
            cmbFolder.TabIndex = 50;
            cmbFolder.Text = null;
            cmbFolder.TextPadding = new Padding(2);
            cmbFolder.SelectedIndexChanged += cmbFolder_SelectedIndexChanged;
            // 
            // nudMaxHp
            // 
            nudMaxHp.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMaxHp.ForeColor = System.Drawing.Color.Gainsboro;
            nudMaxHp.Location = new System.Drawing.Point(88, 144);
            nudMaxHp.Margin = new Padding(4, 3, 4, 3);
            nudMaxHp.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudMaxHp.Name = "nudMaxHp";
            nudMaxHp.Size = new Size(158, 23);
            nudMaxHp.TabIndex = 42;
            nudMaxHp.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMaxHp.ValueChanged += nudMaxHp_ValueChanged;
            // 
            // nudMinHp
            // 
            nudMinHp.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMinHp.ForeColor = System.Drawing.Color.Gainsboro;
            nudMinHp.Location = new System.Drawing.Point(88, 114);
            nudMinHp.Margin = new Padding(4, 3, 4, 3);
            nudMinHp.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudMinHp.Name = "nudMinHp";
            nudMinHp.Size = new Size(158, 23);
            nudMinHp.TabIndex = 41;
            nudMinHp.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMinHp.ValueChanged += nudMinHp_ValueChanged;
            // 
            // nudSpawnDuration
            // 
            nudSpawnDuration.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudSpawnDuration.ForeColor = System.Drawing.Color.Gainsboro;
            nudSpawnDuration.Location = new System.Drawing.Point(108, 202);
            nudSpawnDuration.Margin = new Padding(4, 3, 4, 3);
            nudSpawnDuration.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudSpawnDuration.Name = "nudSpawnDuration";
            nudSpawnDuration.Size = new Size(137, 23);
            nudSpawnDuration.TabIndex = 40;
            nudSpawnDuration.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudSpawnDuration.ValueChanged += nudSpawnDuration_ValueChanged;
            // 
            // cmbDeathAnimation
            // 
            cmbDeathAnimation.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbDeathAnimation.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbDeathAnimation.BorderStyle = ButtonBorderStyle.Solid;
            cmbDeathAnimation.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbDeathAnimation.DrawDropdownHoverOutline = false;
            cmbDeathAnimation.DrawFocusRectangle = false;
            cmbDeathAnimation.DrawMode = DrawMode.OwnerDrawFixed;
            cmbDeathAnimation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDeathAnimation.FlatStyle = FlatStyle.Flat;
            cmbDeathAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            cmbDeathAnimation.FormattingEnabled = true;
            cmbDeathAnimation.Location = new System.Drawing.Point(7, 255);
            cmbDeathAnimation.Margin = new Padding(4, 3, 4, 3);
            cmbDeathAnimation.Name = "cmbDeathAnimation";
            cmbDeathAnimation.Size = new Size(239, 24);
            cmbDeathAnimation.TabIndex = 39;
            cmbDeathAnimation.Text = null;
            cmbDeathAnimation.TextPadding = new Padding(2);
            cmbDeathAnimation.SelectedIndexChanged += cmbDeathAnimation_SelectedIndexChanged;
            // 
            // lblDeathAnimation
            // 
            lblDeathAnimation.AutoSize = true;
            lblDeathAnimation.Location = new System.Drawing.Point(8, 232);
            lblDeathAnimation.Margin = new Padding(4, 0, 4, 0);
            lblDeathAnimation.Name = "lblDeathAnimation";
            lblDeathAnimation.Size = new Size(100, 15);
            lblDeathAnimation.TabIndex = 36;
            lblDeathAnimation.Text = "Death Animation:";
            // 
            // lblMaxHp
            // 
            lblMaxHp.AutoSize = true;
            lblMaxHp.Location = new System.Drawing.Point(7, 147);
            lblMaxHp.Margin = new Padding(4, 0, 4, 0);
            lblMaxHp.Name = "lblMaxHp";
            lblMaxHp.Size = new Size(52, 15);
            lblMaxHp.TabIndex = 35;
            lblMaxHp.Text = "Max HP:";
            // 
            // lblSpawnDuration
            // 
            lblSpawnDuration.AutoSize = true;
            lblSpawnDuration.Location = new System.Drawing.Point(6, 204);
            lblSpawnDuration.Margin = new Padding(4, 0, 4, 0);
            lblSpawnDuration.Name = "lblSpawnDuration";
            lblSpawnDuration.Size = new Size(94, 15);
            lblSpawnDuration.TabIndex = 32;
            lblSpawnDuration.Text = "Spawn Duration:";
            // 
            // chkWalkableAfter
            // 
            chkWalkableAfter.Location = new System.Drawing.Point(7, 314);
            chkWalkableAfter.Margin = new Padding(4, 3, 4, 3);
            chkWalkableAfter.Name = "chkWalkableAfter";
            chkWalkableAfter.Size = new Size(246, 20);
            chkWalkableAfter.TabIndex = 31;
            chkWalkableAfter.Text = "Walkable after resource removal?";
            chkWalkableAfter.CheckedChanged += chkWalkableAfter_CheckedChanged;
            // 
            // chkWalkableBefore
            // 
            chkWalkableBefore.Location = new System.Drawing.Point(7, 287);
            chkWalkableBefore.Margin = new Padding(4, 3, 4, 3);
            chkWalkableBefore.Name = "chkWalkableBefore";
            chkWalkableBefore.Size = new Size(246, 20);
            chkWalkableBefore.TabIndex = 30;
            chkWalkableBefore.Text = "Walkable before resource removal?";
            chkWalkableBefore.CheckedChanged += chkWalkableBefore_CheckedChanged;
            // 
            // cmbToolType
            // 
            cmbToolType.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbToolType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbToolType.BorderStyle = ButtonBorderStyle.Solid;
            cmbToolType.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbToolType.DrawDropdownHoverOutline = false;
            cmbToolType.DrawFocusRectangle = false;
            cmbToolType.DrawMode = DrawMode.OwnerDrawFixed;
            cmbToolType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbToolType.FlatStyle = FlatStyle.Flat;
            cmbToolType.ForeColor = System.Drawing.Color.Gainsboro;
            cmbToolType.FormattingEnabled = true;
            cmbToolType.Location = new System.Drawing.Point(88, 83);
            cmbToolType.Margin = new Padding(4, 3, 4, 3);
            cmbToolType.Name = "cmbToolType";
            cmbToolType.Size = new Size(157, 24);
            cmbToolType.TabIndex = 29;
            cmbToolType.Text = null;
            cmbToolType.TextPadding = new Padding(2);
            cmbToolType.SelectedIndexChanged += cmbToolType_SelectedIndexChanged;
            // 
            // lblToolType
            // 
            lblToolType.AutoSize = true;
            lblToolType.Location = new System.Drawing.Point(7, 87);
            lblToolType.Margin = new Padding(4, 0, 4, 0);
            lblToolType.Name = "lblToolType";
            lblToolType.Size = new Size(59, 15);
            lblToolType.TabIndex = 28;
            lblToolType.Text = "Tool Type:";
            // 
            // lblHP
            // 
            lblHP.AutoSize = true;
            lblHP.Location = new System.Drawing.Point(7, 117);
            lblHP.Margin = new Padding(4, 0, 4, 0);
            lblHP.Name = "lblHP";
            lblHP.Size = new Size(50, 15);
            lblHP.TabIndex = 16;
            lblHP.Text = "Min HP:";
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(7, 23);
            lblName.Margin = new Padding(4, 0, 4, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(42, 15);
            lblName.TabIndex = 3;
            lblName.Text = "Name:";
            // 
            // txtName
            // 
            txtName.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtName.Location = new System.Drawing.Point(88, 23);
            txtName.Margin = new Padding(4, 3, 4, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(157, 23);
            txtName.TabIndex = 2;
            txtName.TextChanged += txtName_TextChanged;
            // 
            // btnRequirements
            // 
            btnRequirements.Location = new System.Drawing.Point(9, 21);
            btnRequirements.Margin = new Padding(4, 3, 4, 3);
            btnRequirements.Name = "btnRequirements";
            btnRequirements.Padding = new Padding(6);
            btnRequirements.Size = new Size(262, 27);
            btnRequirements.TabIndex = 38;
            btnRequirements.Text = "Harvesting Requirements";
            btnRequirements.Click += btnRequirements_Click;
            // 
            // grpGraphics
            // 
            grpGraphics.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpGraphics.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpGraphics.Controls.Add(btnRemoveState);
            grpGraphics.Controls.Add(btnAddState);
            grpGraphics.Controls.Add(lblStateName);
            grpGraphics.Controls.Add(lstStates);
            grpGraphics.Controls.Add(txtStateName);
            grpGraphics.Controls.Add(grpGraphicData);
            grpGraphics.Controls.Add(lblStates);
            grpGraphics.Controls.Add(graphicContainer);
            grpGraphics.ForeColor = System.Drawing.Color.Gainsboro;
            grpGraphics.Location = new System.Drawing.Point(1, 386);
            grpGraphics.Margin = new Padding(4, 3, 4, 3);
            grpGraphics.Name = "grpGraphics";
            grpGraphics.Padding = new Padding(4, 3, 4, 3);
            grpGraphics.Size = new Size(824, 524);
            grpGraphics.TabIndex = 16;
            grpGraphics.TabStop = false;
            grpGraphics.Text = "Appearance";
            // 
            // btnRemoveState
            // 
            btnRemoveState.Location = new System.Drawing.Point(172, 187);
            btnRemoveState.Margin = new Padding(4, 3, 4, 3);
            btnRemoveState.Name = "btnRemoveState";
            btnRemoveState.Padding = new Padding(6);
            btnRemoveState.Size = new Size(88, 27);
            btnRemoveState.TabIndex = 67;
            btnRemoveState.Text = "Remove";
            btnRemoveState.Click += btnRemoveState_Click;
            // 
            // btnAddState
            // 
            btnAddState.Location = new System.Drawing.Point(7, 187);
            btnAddState.Margin = new Padding(4, 3, 4, 3);
            btnAddState.Name = "btnAddState";
            btnAddState.Padding = new Padding(6);
            btnAddState.Size = new Size(88, 27);
            btnAddState.TabIndex = 67;
            btnAddState.Text = "Add";
            btnAddState.Click += btnAddState_Click;
            // 
            // lblStateName
            // 
            lblStateName.AutoSize = true;
            lblStateName.Location = new System.Drawing.Point(4, 139);
            lblStateName.Margin = new Padding(4, 0, 4, 0);
            lblStateName.Name = "lblStateName";
            lblStateName.Size = new Size(71, 15);
            lblStateName.TabIndex = 56;
            lblStateName.Text = "State Name:";
            // 
            // lstStates
            // 
            lstStates.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstStates.BorderStyle = BorderStyle.FixedSingle;
            lstStates.ForeColor = System.Drawing.Color.Gainsboro;
            lstStates.FormattingEnabled = true;
            lstStates.ItemHeight = 15;
            lstStates.Location = new System.Drawing.Point(7, 39);
            lstStates.Margin = new Padding(4, 3, 4, 3);
            lstStates.Name = "lstStates";
            lstStates.Size = new Size(253, 92);
            lstStates.TabIndex = 60;
            lstStates.SelectedIndexChanged += lstStates_SelectedIndexChanged;
            // 
            // txtStateName
            // 
            txtStateName.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtStateName.BorderStyle = BorderStyle.FixedSingle;
            txtStateName.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtStateName.Location = new System.Drawing.Point(7, 158);
            txtStateName.Margin = new Padding(4, 3, 4, 3);
            txtStateName.Name = "txtStateName";
            txtStateName.Size = new Size(253, 23);
            txtStateName.TabIndex = 55;
            // 
            // grpGraphicData
            // 
            grpGraphicData.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpGraphicData.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpGraphicData.Controls.Add(nudStateRangeMax);
            grpGraphicData.Controls.Add(nudStateRangeMin);
            grpGraphicData.Controls.Add(lblStateRange);
            grpGraphicData.Controls.Add(cmbTextureSource);
            grpGraphicData.Controls.Add(lblTerxtureSource);
            grpGraphicData.Controls.Add(cmbAnimation);
            grpGraphicData.Controls.Add(lblAnimation);
            grpGraphicData.Controls.Add(chkRenderBelowEntity);
            grpGraphicData.Controls.Add(cmbTextureType);
            grpGraphicData.Controls.Add(lblTextureType);
            grpGraphicData.ForeColor = System.Drawing.Color.Gainsboro;
            grpGraphicData.Location = new System.Drawing.Point(7, 221);
            grpGraphicData.Margin = new Padding(2);
            grpGraphicData.Name = "grpGraphicData";
            grpGraphicData.Padding = new Padding(2);
            grpGraphicData.Size = new Size(253, 292);
            grpGraphicData.TabIndex = 34;
            grpGraphicData.TabStop = false;
            grpGraphicData.Text = "Graphic Data";
            // 
            // nudStateRangeMax
            // 
            nudStateRangeMax.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudStateRangeMax.ForeColor = System.Drawing.Color.Gainsboro;
            nudStateRangeMax.Location = new System.Drawing.Point(137, 229);
            nudStateRangeMax.Margin = new Padding(4, 3, 4, 3);
            nudStateRangeMax.Name = "nudStateRangeMax";
            nudStateRangeMax.Size = new Size(102, 23);
            nudStateRangeMax.TabIndex = 68;
            nudStateRangeMax.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudStateRangeMax.ValueChanged += nudStateRangeMax_ValueChanged;
            // 
            // nudStateRangeMin
            // 
            nudStateRangeMin.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudStateRangeMin.ForeColor = System.Drawing.Color.Gainsboro;
            nudStateRangeMin.Location = new System.Drawing.Point(12, 229);
            nudStateRangeMin.Margin = new Padding(4, 3, 4, 3);
            nudStateRangeMin.Name = "nudStateRangeMin";
            nudStateRangeMin.Size = new Size(102, 23);
            nudStateRangeMin.TabIndex = 67;
            nudStateRangeMin.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudStateRangeMin.ValueChanged += nudStateRangeMin_ValueChanged;
            // 
            // lblStateRange
            // 
            lblStateRange.AutoSize = true;
            lblStateRange.Location = new System.Drawing.Point(12, 208);
            lblStateRange.Margin = new Padding(4, 0, 4, 0);
            lblStateRange.Name = "lblStateRange";
            lblStateRange.Size = new Size(151, 15);
            lblStateRange.TabIndex = 58;
            lblStateRange.Text = "State Range Min - Max (%):";
            // 
            // cmbTextureSource
            // 
            cmbTextureSource.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbTextureSource.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbTextureSource.BorderStyle = ButtonBorderStyle.Solid;
            cmbTextureSource.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbTextureSource.DrawDropdownHoverOutline = false;
            cmbTextureSource.DrawFocusRectangle = false;
            cmbTextureSource.DrawMode = DrawMode.OwnerDrawFixed;
            cmbTextureSource.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTextureSource.FlatStyle = FlatStyle.Flat;
            cmbTextureSource.ForeColor = System.Drawing.Color.Gainsboro;
            cmbTextureSource.FormattingEnabled = true;
            cmbTextureSource.Location = new System.Drawing.Point(12, 119);
            cmbTextureSource.Margin = new Padding(4, 3, 4, 3);
            cmbTextureSource.Name = "cmbTextureSource";
            cmbTextureSource.Size = new Size(227, 24);
            cmbTextureSource.TabIndex = 57;
            cmbTextureSource.Text = null;
            cmbTextureSource.TextPadding = new Padding(2);
            cmbTextureSource.SelectedIndexChanged += cmbGraphicFile_SelectedIndexChanged;
            // 
            // lblTerxtureSource
            // 
            lblTerxtureSource.AutoSize = true;
            lblTerxtureSource.Location = new System.Drawing.Point(9, 99);
            lblTerxtureSource.Margin = new Padding(4, 0, 4, 0);
            lblTerxtureSource.Name = "lblTerxtureSource";
            lblTerxtureSource.Size = new Size(87, 15);
            lblTerxtureSource.TabIndex = 56;
            lblTerxtureSource.Text = "Texture Source:";
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
            cmbAnimation.Location = new System.Drawing.Point(12, 175);
            cmbAnimation.Margin = new Padding(4, 3, 4, 3);
            cmbAnimation.Name = "cmbAnimation";
            cmbAnimation.Size = new Size(227, 24);
            cmbAnimation.TabIndex = 55;
            cmbAnimation.Text = null;
            cmbAnimation.TextPadding = new Padding(2);
            cmbAnimation.SelectedIndexChanged += cmbAnimation_SelectedIndexChanged;
            // 
            // lblAnimation
            // 
            lblAnimation.AutoSize = true;
            lblAnimation.Location = new System.Drawing.Point(9, 155);
            lblAnimation.Margin = new Padding(4, 0, 4, 0);
            lblAnimation.Name = "lblAnimation";
            lblAnimation.Size = new Size(66, 15);
            lblAnimation.TabIndex = 54;
            lblAnimation.Text = "Animation:";
            // 
            // chkRenderBelowEntity
            // 
            chkRenderBelowEntity.Location = new System.Drawing.Point(12, 71);
            chkRenderBelowEntity.Margin = new Padding(4, 3, 4, 3);
            chkRenderBelowEntity.Name = "chkRenderBelowEntity";
            chkRenderBelowEntity.Size = new Size(227, 20);
            chkRenderBelowEntity.TabIndex = 53;
            chkRenderBelowEntity.Text = "Render Below Entity";
            chkRenderBelowEntity.CheckedChanged += chkRenderBelowEntity_CheckedChanged;
            // 
            // cmbTextureType
            // 
            cmbTextureType.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbTextureType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbTextureType.BorderStyle = ButtonBorderStyle.Solid;
            cmbTextureType.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbTextureType.DrawDropdownHoverOutline = false;
            cmbTextureType.DrawFocusRectangle = false;
            cmbTextureType.DrawMode = DrawMode.OwnerDrawFixed;
            cmbTextureType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTextureType.FlatStyle = FlatStyle.Flat;
            cmbTextureType.ForeColor = System.Drawing.Color.Gainsboro;
            cmbTextureType.FormattingEnabled = true;
            cmbTextureType.Location = new System.Drawing.Point(12, 41);
            cmbTextureType.Margin = new Padding(4, 3, 4, 3);
            cmbTextureType.Name = "cmbTextureType";
            cmbTextureType.Size = new Size(227, 24);
            cmbTextureType.TabIndex = 21;
            cmbTextureType.Text = null;
            cmbTextureType.TextPadding = new Padding(2);
            cmbTextureType.SelectedIndexChanged += cmbGraphicType_SelectedIndexChanged;
            // 
            // lblTextureType
            // 
            lblTextureType.AutoSize = true;
            lblTextureType.Location = new System.Drawing.Point(9, 21);
            lblTextureType.Margin = new Padding(4, 0, 4, 0);
            lblTextureType.Name = "lblTextureType";
            lblTextureType.Size = new Size(73, 15);
            lblTextureType.TabIndex = 20;
            lblTextureType.Text = "Terture Type:";
            // 
            // lblStates
            // 
            lblStates.AutoSize = true;
            lblStates.Location = new System.Drawing.Point(10, 21);
            lblStates.Margin = new Padding(4, 0, 4, 0);
            lblStates.Name = "lblStates";
            lblStates.Size = new Size(41, 15);
            lblStates.TabIndex = 55;
            lblStates.Text = "States:";
            // 
            // graphicContainer
            // 
            graphicContainer.AutoScroll = true;
            graphicContainer.Controls.Add(picResource);
            graphicContainer.Location = new System.Drawing.Point(270, 22);
            graphicContainer.Margin = new Padding(4, 3, 4, 3);
            graphicContainer.Name = "graphicContainer";
            graphicContainer.Size = new Size(540, 491);
            graphicContainer.TabIndex = 24;
            // 
            // picResource
            // 
            picResource.Location = new System.Drawing.Point(0, 0);
            picResource.Margin = new Padding(4, 3, 4, 3);
            picResource.Name = "picResource";
            picResource.Size = new Size(540, 491);
            picResource.TabIndex = 2;
            picResource.TabStop = false;
            picResource.MouseDown += picResource_MouseDown;
            picResource.MouseMove += picResource_MouseMove;
            picResource.MouseUp += picResource_MouseUp;
            // 
            // tmrRender
            // 
            tmrRender.Enabled = true;
            tmrRender.Interval = 10;
            tmrRender.Tick += Render;
            // 
            // pnlContainer
            // 
            pnlContainer.AutoScroll = true;
            pnlContainer.Controls.Add(grpRequirements);
            pnlContainer.Controls.Add(grpCommonEvent);
            pnlContainer.Controls.Add(grpDrops);
            pnlContainer.Controls.Add(grpGeneral);
            pnlContainer.Controls.Add(grpGraphics);
            pnlContainer.Location = new System.Drawing.Point(258, 45);
            pnlContainer.Margin = new Padding(4, 3, 4, 3);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(853, 635);
            pnlContainer.TabIndex = 18;
            pnlContainer.Visible = false;
            // 
            // grpRequirements
            // 
            grpRequirements.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpRequirements.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpRequirements.Controls.Add(lblCannotHarvest);
            grpRequirements.Controls.Add(btnRequirements);
            grpRequirements.Controls.Add(txtCannotHarvest);
            grpRequirements.ForeColor = System.Drawing.Color.Gainsboro;
            grpRequirements.Location = new System.Drawing.Point(539, 0);
            grpRequirements.Margin = new Padding(2);
            grpRequirements.Name = "grpRequirements";
            grpRequirements.Padding = new Padding(2);
            grpRequirements.Size = new Size(285, 107);
            grpRequirements.TabIndex = 33;
            grpRequirements.TabStop = false;
            grpRequirements.Text = "Requirements";
            // 
            // lblCannotHarvest
            // 
            lblCannotHarvest.AutoSize = true;
            lblCannotHarvest.Location = new System.Drawing.Point(6, 54);
            lblCannotHarvest.Margin = new Padding(4, 0, 4, 0);
            lblCannotHarvest.Name = "lblCannotHarvest";
            lblCannotHarvest.Size = new Size(141, 15);
            lblCannotHarvest.TabIndex = 54;
            lblCannotHarvest.Text = "Cannot Harvest Message:";
            // 
            // txtCannotHarvest
            // 
            txtCannotHarvest.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtCannotHarvest.BorderStyle = BorderStyle.FixedSingle;
            txtCannotHarvest.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtCannotHarvest.Location = new System.Drawing.Point(9, 73);
            txtCannotHarvest.Margin = new Padding(4, 3, 4, 3);
            txtCannotHarvest.Name = "txtCannotHarvest";
            txtCannotHarvest.Size = new Size(262, 23);
            txtCannotHarvest.TabIndex = 53;
            txtCannotHarvest.TextChanged += txtCannotHarvest_TextChanged;
            // 
            // grpCommonEvent
            // 
            grpCommonEvent.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpCommonEvent.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpCommonEvent.Controls.Add(cmbEvent);
            grpCommonEvent.Controls.Add(lblEvent);
            grpCommonEvent.ForeColor = System.Drawing.Color.Gainsboro;
            grpCommonEvent.Location = new System.Drawing.Point(539, 114);
            grpCommonEvent.Margin = new Padding(2);
            grpCommonEvent.Name = "grpCommonEvent";
            grpCommonEvent.Padding = new Padding(2);
            grpCommonEvent.Size = new Size(285, 80);
            grpCommonEvent.TabIndex = 33;
            grpCommonEvent.TabStop = false;
            grpCommonEvent.Text = "Common Event";
            // 
            // cmbEvent
            // 
            cmbEvent.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbEvent.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbEvent.BorderStyle = ButtonBorderStyle.Solid;
            cmbEvent.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbEvent.DrawDropdownHoverOutline = false;
            cmbEvent.DrawFocusRectangle = false;
            cmbEvent.DrawMode = DrawMode.OwnerDrawFixed;
            cmbEvent.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEvent.FlatStyle = FlatStyle.Flat;
            cmbEvent.ForeColor = System.Drawing.Color.Gainsboro;
            cmbEvent.FormattingEnabled = true;
            cmbEvent.Location = new System.Drawing.Point(9, 39);
            cmbEvent.Margin = new Padding(4, 3, 4, 3);
            cmbEvent.Name = "cmbEvent";
            cmbEvent.Size = new Size(262, 24);
            cmbEvent.TabIndex = 19;
            cmbEvent.Text = null;
            cmbEvent.TextPadding = new Padding(2);
            cmbEvent.SelectedIndexChanged += cmbEvent_SelectedIndexChanged;
            // 
            // lblEvent
            // 
            lblEvent.AutoSize = true;
            lblEvent.Location = new System.Drawing.Point(6, 21);
            lblEvent.Margin = new Padding(4, 0, 4, 0);
            lblEvent.Name = "lblEvent";
            lblEvent.Size = new Size(39, 15);
            lblEvent.TabIndex = 18;
            lblEvent.Text = "Event:";
            // 
            // grpDrops
            // 
            grpDrops.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpDrops.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpDrops.Controls.Add(nudDropMinAmount);
            grpDrops.Controls.Add(lblDropMinAmount);
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
            grpDrops.Location = new System.Drawing.Point(270, 0);
            grpDrops.Margin = new Padding(4, 3, 4, 3);
            grpDrops.Name = "grpDrops";
            grpDrops.Padding = new Padding(4, 3, 4, 3);
            grpDrops.Size = new Size(264, 380);
            grpDrops.TabIndex = 31;
            grpDrops.TabStop = false;
            grpDrops.Text = "Drops";
            // 
            // nudDropMinAmount
            // 
            nudDropMinAmount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDropMinAmount.ForeColor = System.Drawing.Color.Gainsboro;
            nudDropMinAmount.Location = new System.Drawing.Point(10, 254);
            nudDropMinAmount.Margin = new Padding(4, 3, 4, 3);
            nudDropMinAmount.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            nudDropMinAmount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDropMinAmount.Name = "nudDropMinAmount";
            nudDropMinAmount.Size = new Size(87, 23);
            nudDropMinAmount.TabIndex = 66;
            nudDropMinAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudDropMinAmount.ValueChanged += nudDropMinAmount_ValueChanged;
            // 
            // lblDropMinAmount
            // 
            lblDropMinAmount.AutoSize = true;
            lblDropMinAmount.Location = new System.Drawing.Point(10, 236);
            lblDropMinAmount.Margin = new Padding(4, 0, 4, 0);
            lblDropMinAmount.Name = "lblDropMinAmount";
            lblDropMinAmount.Size = new Size(78, 15);
            lblDropMinAmount.TabIndex = 65;
            lblDropMinAmount.Text = "Min Amount:";
            // 
            // btnDropRemove
            // 
            btnDropRemove.Location = new System.Drawing.Point(168, 342);
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
            btnDropAdd.Location = new System.Drawing.Point(10, 342);
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
            lstDrops.Location = new System.Drawing.Point(10, 22);
            lstDrops.Margin = new Padding(4, 3, 4, 3);
            lstDrops.Name = "lstDrops";
            lstDrops.Size = new Size(246, 152);
            lstDrops.TabIndex = 62;
            lstDrops.SelectedIndexChanged += lstDrops_SelectedIndexChanged;
            // 
            // nudDropMaxAmount
            // 
            nudDropMaxAmount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDropMaxAmount.ForeColor = System.Drawing.Color.Gainsboro;
            nudDropMaxAmount.Location = new System.Drawing.Point(169, 254);
            nudDropMaxAmount.Margin = new Padding(4, 3, 4, 3);
            nudDropMaxAmount.Maximum = new decimal(new int[] { 10000000, 0, 0, 0 });
            nudDropMaxAmount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDropMaxAmount.Name = "nudDropMaxAmount";
            nudDropMaxAmount.Size = new Size(87, 23);
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
            nudDropChance.Location = new System.Drawing.Point(10, 307);
            nudDropChance.Margin = new Padding(4, 3, 4, 3);
            nudDropChance.Name = "nudDropChance";
            nudDropChance.Size = new Size(246, 23);
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
            cmbDropItem.Location = new System.Drawing.Point(10, 205);
            cmbDropItem.Margin = new Padding(4, 3, 4, 3);
            cmbDropItem.Name = "cmbDropItem";
            cmbDropItem.Size = new Size(246, 24);
            cmbDropItem.TabIndex = 17;
            cmbDropItem.Text = null;
            cmbDropItem.TextPadding = new Padding(2);
            cmbDropItem.SelectedIndexChanged += cmbDropItem_SelectedIndexChanged;
            // 
            // lblDropMaxAmount
            // 
            lblDropMaxAmount.AutoSize = true;
            lblDropMaxAmount.Location = new System.Drawing.Point(169, 236);
            lblDropMaxAmount.Margin = new Padding(4, 0, 4, 0);
            lblDropMaxAmount.Name = "lblDropMaxAmount";
            lblDropMaxAmount.Size = new Size(80, 15);
            lblDropMaxAmount.TabIndex = 15;
            lblDropMaxAmount.Text = "Max Amount:";
            // 
            // lblDropChance
            // 
            lblDropChance.AutoSize = true;
            lblDropChance.Location = new System.Drawing.Point(10, 284);
            lblDropChance.Margin = new Padding(4, 0, 4, 0);
            lblDropChance.Name = "lblDropChance";
            lblDropChance.Size = new Size(71, 15);
            lblDropChance.TabIndex = 13;
            lblDropChance.Text = "Chance (%):";
            // 
            // lblDropItem
            // 
            lblDropItem.AutoSize = true;
            lblDropItem.Location = new System.Drawing.Point(10, 186);
            lblDropItem.Margin = new Padding(4, 0, 4, 0);
            lblDropItem.Name = "lblDropItem";
            lblDropItem.Size = new Size(34, 15);
            lblDropItem.TabIndex = 11;
            lblDropItem.Text = "Item:";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(887, 688);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(222, 31);
            btnCancel.TabIndex = 44;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(658, 688);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(222, 31);
            btnSave.TabIndex = 41;
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
            toolStrip.Size = new Size(1119, 29);
            toolStrip.TabIndex = 47;
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
            // FrmResource
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            ClientSize = new Size(1119, 728);
            ControlBox = true;
            Controls.Add(toolStrip);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(grpResources);
            Controls.Add(pnlContainer);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            MinimizeBox = false;
            MaximizeBox = false;
            Name = "FrmResource";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Resource Editor";
            FormClosed += frmResource_FormClosed;
            Load += frmResource_Load;
            KeyDown += form_KeyDown;
            grpResources.ResumeLayout(false);
            grpResources.PerformLayout();
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudHpRegen).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxHp).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMinHp).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudSpawnDuration).EndInit();
            grpGraphics.ResumeLayout(false);
            grpGraphics.PerformLayout();
            grpGraphicData.ResumeLayout(false);
            grpGraphicData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudStateRangeMax).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudStateRangeMin).EndInit();
            graphicContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picResource).EndInit();
            pnlContainer.ResumeLayout(false);
            grpRequirements.ResumeLayout(false);
            grpRequirements.PerformLayout();
            grpCommonEvent.ResumeLayout(false);
            grpCommonEvent.PerformLayout();
            grpDrops.ResumeLayout(false);
            grpDrops.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudDropMinAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDropMaxAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDropChance).EndInit();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpResources;
        private DarkGroupBox grpGeneral;
        private DarkGroupBox grpGraphics;
        private System.Windows.Forms.Label lblName;
        private DarkTextBox txtName;
        private System.Windows.Forms.Label lblHP;
        private DarkCheckBox chkWalkableAfter;
        private DarkCheckBox chkWalkableBefore;
        private DarkComboBox cmbToolType;
        private System.Windows.Forms.Label lblToolType;
        private System.Windows.Forms.Label lblSpawnDuration;
        public System.Windows.Forms.PictureBox picResource;
        private System.Windows.Forms.Label lblMaxHp;
        private System.Windows.Forms.Label lblDeathAnimation;
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
        private System.Windows.Forms.Panel graphicContainer;
        private DarkButton btnRequirements;
        private DarkComboBox cmbDeathAnimation;
        private DarkNumericUpDown nudSpawnDuration;
        private DarkNumericUpDown nudMaxHp;
        private DarkNumericUpDown nudMinHp;
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
        private DarkNumericUpDown nudHpRegen;
        private DarkGroupBox grpCommonEvent;
        private DarkComboBox cmbEvent;
        private System.Windows.Forms.Label lblEvent;
        private DarkButton btnClearSearch;
        private DarkTextBox txtSearch;
        private DarkButton btnAddFolder;
        private System.Windows.Forms.Label lblFolder;
        private DarkComboBox cmbFolder;
        private System.Windows.Forms.ToolStripButton btnAlphabetical;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private Controls.GameObjectList lstGameObjects;
        private DarkGroupBox grpRequirements;
        private System.Windows.Forms.Label lblCannotHarvest;
        private DarkTextBox txtCannotHarvest;
        private DarkNumericUpDown nudDropMinAmount;
        private Label lblDropMinAmount;
        private Label lblStates;
        private DarkGroupBox grpGraphicData;
        private DarkComboBox cmbTextureType;
        private Label lblTextureType;
        private DarkCheckBox chkRenderBelowEntity;
        private DarkComboBox cmbAnimation;
        private Label lblAnimation;
        private DarkComboBox cmbTextureSource;
        private Label lblTerxtureSource;
        private ListBox lstStates;
        private DarkButton btnRemoveState;
        private DarkButton btnAddState;
        private Label lblStateName;
        private DarkTextBox txtStateName;
        private Label lblStateRange;
        private DarkNumericUpDown nudStateRangeMax;
        private DarkNumericUpDown nudStateRangeMin;
        private DarkCheckBox chkUseExplicitMaxHealthForResourceStates;
        private Label lblHpRegen;
    }
}