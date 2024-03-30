using System.ComponentModel;
using System.Windows.Forms;
using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
{
    partial class FrmItem
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
            components = new Container();
            var resources = new ComponentResourceManager(typeof(FrmItem));
            grpItems = new DarkGroupBox();
            btnClearSearch = new DarkButton();
            txtSearch = new DarkTextBox();
            lstGameObjects = new Controls.GameObjectList();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            grpGeneral = new DarkGroupBox();
            grpEvents = new DarkGroupBox();
            cmbEventTriggers = new DarkComboBox();
            lblEventForTrigger = new Label();
            lstEventTriggers = new ListBox();
            grpStack = new DarkGroupBox();
            chkStackable = new DarkCheckBox();
            lblInvStackLimit = new Label();
            nudInvStackLimit = new DarkNumericUpDown();
            lblBankStackLimit = new Label();
            nudBankStackLimit = new DarkNumericUpDown();
            grpCooldown = new DarkGroupBox();
            lblCooldown = new Label();
            nudCooldown = new DarkNumericUpDown();
            lblCooldownGroup = new Label();
            cmbCooldownGroup = new DarkComboBox();
            btnAddCooldownGroup = new DarkButton();
            chkIgnoreGlobalCooldown = new DarkCheckBox();
            chkIgnoreCdr = new DarkCheckBox();
            nudItemDespawnTime = new DarkNumericUpDown();
            lblDespawnTime = new Label();
            cmbEquipmentAnimation = new DarkComboBox();
            grpRequirements = new DarkGroupBox();
            lblCannotUse = new Label();
            txtCannotUse = new DarkTextBox();
            btnEditRequirements = new DarkButton();
            chkCanGuildBank = new DarkCheckBox();
            lblEquipmentAnimation = new Label();
            nudDeathDropChance = new DarkNumericUpDown();
            lblDeathDropChance = new Label();
            chkCanSell = new DarkCheckBox();
            chkCanTrade = new DarkCheckBox();
            chkCanBag = new DarkCheckBox();
            chkCanBank = new DarkCheckBox();
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
            cmbRarity = new DarkComboBox();
            lblRarity = new Label();
            nudPrice = new DarkNumericUpDown();
            chkCanDrop = new DarkCheckBox();
            cmbAnimation = new DarkComboBox();
            lblDesc = new Label();
            txtDesc = new DarkTextBox();
            cmbPic = new DarkComboBox();
            lblAnim = new Label();
            lblPrice = new Label();
            lblPic = new Label();
            picItem = new PictureBox();
            lblType = new Label();
            cmbType = new DarkComboBox();
            lblName = new Label();
            txtName = new DarkTextBox();
            grpConsumable = new DarkGroupBox();
            lblPercentage3 = new Label();
            nudIntervalPercentage = new DarkNumericUpDown();
            lblPlus3 = new Label();
            nudInterval = new DarkNumericUpDown();
            lblVital = new Label();
            cmbConsume = new DarkComboBox();
            lblInterval = new Label();
            grpEvent = new DarkGroupBox();
            chkSingleUseEvent = new DarkCheckBox();
            cmbEvent = new DarkComboBox();
            grpBags = new DarkGroupBox();
            nudBag = new DarkNumericUpDown();
            lblBag = new Label();
            grpSpell = new DarkGroupBox();
            chkSingleUseSpell = new DarkCheckBox();
            chkQuickCast = new DarkCheckBox();
            cmbTeachSpell = new DarkComboBox();
            lblSpell = new Label();
            grpEquipment = new DarkGroupBox();
            grpStatRanges = new DarkGroupBox();
            lblStatRangeFrom = new Label();
            lblStatRangeTo = new Label();
            nudStatRangeLow = new DarkNumericUpDown();
            lstStatRanges = new ListBox();
            nudStatRangeHigh = new DarkNumericUpDown();
            grpPaperdoll = new DarkGroupBox();
            picMalePaperdoll = new PictureBox();
            lblMalePaperdoll = new Label();
            cmbMalePaperdoll = new DarkComboBox();
            picFemalePaperdoll = new PictureBox();
            lblFemalePaperdoll = new Label();
            cmbFemalePaperdoll = new DarkComboBox();
            grpEffects = new DarkGroupBox();
            lstBonusEffects = new ListBox();
            lblEffectPercent = new Label();
            nudEffectPercent = new DarkNumericUpDown();
            grpRegen = new DarkGroupBox();
            nudMpRegen = new DarkNumericUpDown();
            nudHPRegen = new DarkNumericUpDown();
            lblHpRegen = new Label();
            lblManaRegen = new Label();
            lblRegenHint = new Label();
            grpVitalBonuses = new DarkGroupBox();
            lblPercentage2 = new Label();
            lblPercentage1 = new Label();
            nudMPPercentage = new DarkNumericUpDown();
            nudHPPercentage = new DarkNumericUpDown();
            lblPlus2 = new Label();
            lblPlus1 = new Label();
            nudManaBonus = new DarkNumericUpDown();
            nudHealthBonus = new DarkNumericUpDown();
            lblManaBonus = new Label();
            lblHealthBonus = new Label();
            grpStatBonuses = new DarkGroupBox();
            lblPercentage5 = new Label();
            lblPercentage4 = new Label();
            lblPercentage8 = new Label();
            lblPercentage7 = new Label();
            lblPercentage6 = new Label();
            nudSpdPercentage = new DarkNumericUpDown();
            nudMRPercentage = new DarkNumericUpDown();
            nudDefPercentage = new DarkNumericUpDown();
            nudMagPercentage = new DarkNumericUpDown();
            nudStrPercentage = new DarkNumericUpDown();
            lblPlus5 = new Label();
            lblPlus4 = new Label();
            lblPlus8 = new Label();
            lblPlus7 = new Label();
            lblPlus6 = new Label();
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
            cmbEquipmentSlot = new DarkComboBox();
            lblEquipmentSlot = new Label();
            grpWeaponProperties = new DarkGroupBox();
            cmbWeaponSprite = new DarkComboBox();
            lblSpriteAttack = new Label();
            nudCritMultiplier = new DarkNumericUpDown();
            lblCritMultiplier = new Label();
            grpAttackSpeed = new DarkGroupBox();
            nudAttackSpeedValue = new DarkNumericUpDown();
            lblAttackSpeedValue = new Label();
            cmbAttackSpeedModifier = new DarkComboBox();
            lblAttackSpeedModifier = new Label();
            nudScaling = new DarkNumericUpDown();
            nudCritChance = new DarkNumericUpDown();
            nudDamage = new DarkNumericUpDown();
            cmbProjectile = new DarkComboBox();
            cmbScalingStat = new DarkComboBox();
            lblScalingStat = new Label();
            lblScalingAmount = new Label();
            cmbDamageType = new DarkComboBox();
            lblDamageType = new Label();
            lblCritChance = new Label();
            cmbAttackAnimation = new DarkComboBox();
            lblAttackAnimation = new Label();
            chk2Hand = new DarkCheckBox();
            lblToolType = new Label();
            cmbToolType = new DarkComboBox();
            lblProjectile = new Label();
            lblDamage = new Label();
            grpShieldProperties = new DarkGroupBox();
            nudBlockDmgAbs = new DarkNumericUpDown();
            lblBlockDmgAbs = new Label();
            nudBlockAmount = new DarkNumericUpDown();
            lblBlockAmount = new Label();
            nudBlockChance = new DarkNumericUpDown();
            lblBlockChance = new Label();
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
            tooltips = new ToolTip(components);
            grpItems.SuspendLayout();
            grpGeneral.SuspendLayout();
            grpEvents.SuspendLayout();
            grpStack.SuspendLayout();
            ((ISupportInitialize)nudInvStackLimit).BeginInit();
            ((ISupportInitialize)nudBankStackLimit).BeginInit();
            grpCooldown.SuspendLayout();
            ((ISupportInitialize)nudCooldown).BeginInit();
            ((ISupportInitialize)nudItemDespawnTime).BeginInit();
            grpRequirements.SuspendLayout();
            ((ISupportInitialize)nudDeathDropChance).BeginInit();
            ((ISupportInitialize)nudRgbaA).BeginInit();
            ((ISupportInitialize)nudRgbaB).BeginInit();
            ((ISupportInitialize)nudRgbaG).BeginInit();
            ((ISupportInitialize)nudRgbaR).BeginInit();
            ((ISupportInitialize)nudPrice).BeginInit();
            ((ISupportInitialize)picItem).BeginInit();
            grpConsumable.SuspendLayout();
            ((ISupportInitialize)nudIntervalPercentage).BeginInit();
            ((ISupportInitialize)nudInterval).BeginInit();
            grpEvent.SuspendLayout();
            grpBags.SuspendLayout();
            ((ISupportInitialize)nudBag).BeginInit();
            grpSpell.SuspendLayout();
            grpEquipment.SuspendLayout();
            grpStatRanges.SuspendLayout();
            ((ISupportInitialize)nudStatRangeLow).BeginInit();
            ((ISupportInitialize)nudStatRangeHigh).BeginInit();
            grpPaperdoll.SuspendLayout();
            ((ISupportInitialize)picMalePaperdoll).BeginInit();
            ((ISupportInitialize)picFemalePaperdoll).BeginInit();
            grpEffects.SuspendLayout();
            ((ISupportInitialize)nudEffectPercent).BeginInit();
            grpRegen.SuspendLayout();
            ((ISupportInitialize)nudMpRegen).BeginInit();
            ((ISupportInitialize)nudHPRegen).BeginInit();
            grpVitalBonuses.SuspendLayout();
            ((ISupportInitialize)nudMPPercentage).BeginInit();
            ((ISupportInitialize)nudHPPercentage).BeginInit();
            ((ISupportInitialize)nudManaBonus).BeginInit();
            ((ISupportInitialize)nudHealthBonus).BeginInit();
            grpStatBonuses.SuspendLayout();
            ((ISupportInitialize)nudSpdPercentage).BeginInit();
            ((ISupportInitialize)nudMRPercentage).BeginInit();
            ((ISupportInitialize)nudDefPercentage).BeginInit();
            ((ISupportInitialize)nudMagPercentage).BeginInit();
            ((ISupportInitialize)nudStrPercentage).BeginInit();
            ((ISupportInitialize)nudSpd).BeginInit();
            ((ISupportInitialize)nudMR).BeginInit();
            ((ISupportInitialize)nudDef).BeginInit();
            ((ISupportInitialize)nudMag).BeginInit();
            ((ISupportInitialize)nudStr).BeginInit();
            grpWeaponProperties.SuspendLayout();
            ((ISupportInitialize)nudCritMultiplier).BeginInit();
            grpAttackSpeed.SuspendLayout();
            ((ISupportInitialize)nudAttackSpeedValue).BeginInit();
            ((ISupportInitialize)nudScaling).BeginInit();
            ((ISupportInitialize)nudCritChance).BeginInit();
            ((ISupportInitialize)nudDamage).BeginInit();
            grpShieldProperties.SuspendLayout();
            ((ISupportInitialize)nudBlockDmgAbs).BeginInit();
            ((ISupportInitialize)nudBlockAmount).BeginInit();
            ((ISupportInitialize)nudBlockChance).BeginInit();
            pnlContainer.SuspendLayout();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // grpItems
            // 
            grpItems.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpItems.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpItems.Controls.Add(btnClearSearch);
            grpItems.Controls.Add(txtSearch);
            grpItems.Controls.Add(lstGameObjects);
            grpItems.ForeColor = System.Drawing.Color.Gainsboro;
            grpItems.Location = new System.Drawing.Point(4, 32);
            grpItems.Margin = new Padding(4, 3, 4, 3);
            grpItems.Name = "grpItems";
            grpItems.Padding = new Padding(4, 3, 4, 3);
            grpItems.Size = new Size(233, 642);
            grpItems.TabIndex = 1;
            grpItems.TabStop = false;
            grpItems.Text = "Items";
            // 
            // btnClearSearch
            // 
            btnClearSearch.Location = new System.Drawing.Point(204, 22);
            btnClearSearch.Margin = new Padding(4, 3, 4, 3);
            btnClearSearch.Name = "btnClearSearch";
            btnClearSearch.Padding = new Padding(6);
            btnClearSearch.Size = new Size(21, 23);
            btnClearSearch.TabIndex = 31;
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
            txtSearch.TabIndex = 30;
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
            lstGameObjects.Size = new Size(222, 577);
            lstGameObjects.TabIndex = 29;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(1008, 683);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(222, 32);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(779, 683);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(222, 32);
            btnSave.TabIndex = 2;
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;
            // 
            // grpGeneral
            // 
            grpGeneral.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpGeneral.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpGeneral.Controls.Add(grpEvents);
            grpGeneral.Controls.Add(grpStack);
            grpGeneral.Controls.Add(grpCooldown);
            grpGeneral.Controls.Add(nudItemDespawnTime);
            grpGeneral.Controls.Add(lblDespawnTime);
            grpGeneral.Controls.Add(cmbEquipmentAnimation);
            grpGeneral.Controls.Add(grpRequirements);
            grpGeneral.Controls.Add(chkCanGuildBank);
            grpGeneral.Controls.Add(lblEquipmentAnimation);
            grpGeneral.Controls.Add(nudDeathDropChance);
            grpGeneral.Controls.Add(lblDeathDropChance);
            grpGeneral.Controls.Add(chkCanSell);
            grpGeneral.Controls.Add(chkCanTrade);
            grpGeneral.Controls.Add(chkCanBag);
            grpGeneral.Controls.Add(chkCanBank);
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
            grpGeneral.Controls.Add(cmbRarity);
            grpGeneral.Controls.Add(lblRarity);
            grpGeneral.Controls.Add(nudPrice);
            grpGeneral.Controls.Add(chkCanDrop);
            grpGeneral.Controls.Add(cmbAnimation);
            grpGeneral.Controls.Add(lblDesc);
            grpGeneral.Controls.Add(txtDesc);
            grpGeneral.Controls.Add(cmbPic);
            grpGeneral.Controls.Add(lblAnim);
            grpGeneral.Controls.Add(lblPrice);
            grpGeneral.Controls.Add(lblPic);
            grpGeneral.Controls.Add(picItem);
            grpGeneral.Controls.Add(lblType);
            grpGeneral.Controls.Add(cmbType);
            grpGeneral.Controls.Add(lblName);
            grpGeneral.Controls.Add(txtName);
            grpGeneral.Controls.Add(grpConsumable);
            grpGeneral.Controls.Add(grpEvent);
            grpGeneral.Controls.Add(grpBags);
            grpGeneral.Controls.Add(grpSpell);
            grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
            grpGeneral.Location = new System.Drawing.Point(11, 3);
            grpGeneral.Margin = new Padding(4, 3, 4, 3);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Padding = new Padding(4, 3, 4, 3);
            grpGeneral.Size = new Size(943, 634);
            grpGeneral.TabIndex = 2;
            grpGeneral.TabStop = false;
            grpGeneral.Text = "General";
            // 
            // grpEvents
            // 
            grpEvents.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpEvents.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpEvents.Controls.Add(cmbEventTriggers);
            grpEvents.Controls.Add(lblEventForTrigger);
            grpEvents.Controls.Add(lstEventTriggers);
            grpEvents.ForeColor = System.Drawing.Color.Gainsboro;
            grpEvents.Location = new System.Drawing.Point(596, 442);
            grpEvents.Margin = new Padding(4, 3, 4, 3);
            grpEvents.Name = "grpEvents";
            grpEvents.Padding = new Padding(4, 3, 4, 3);
            grpEvents.Size = new Size(329, 175);
            grpEvents.TabIndex = 57;
            grpEvents.TabStop = false;
            grpEvents.Text = "Event Triggers";
            // 
            // cmbEventTriggers
            // 
            cmbEventTriggers.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbEventTriggers.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbEventTriggers.BorderStyle = ButtonBorderStyle.Solid;
            cmbEventTriggers.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbEventTriggers.DrawDropdownHoverOutline = false;
            cmbEventTriggers.DrawFocusRectangle = false;
            cmbEventTriggers.DrawMode = DrawMode.OwnerDrawFixed;
            cmbEventTriggers.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEventTriggers.FlatStyle = FlatStyle.Flat;
            cmbEventTriggers.ForeColor = System.Drawing.Color.Gainsboro;
            cmbEventTriggers.FormattingEnabled = true;
            cmbEventTriggers.Location = new System.Drawing.Point(57, 138);
            cmbEventTriggers.Margin = new Padding(4, 3, 4, 3);
            cmbEventTriggers.Name = "cmbEventTriggers";
            cmbEventTriggers.Size = new Size(262, 24);
            cmbEventTriggers.TabIndex = 97;
            cmbEventTriggers.Text = null;
            cmbEventTriggers.TextPadding = new Padding(2);
            cmbEventTriggers.SelectedIndexChanged += cmbEventTriggers_SelectedIndexChanged;
            // 
            // lblEventForTrigger
            // 
            lblEventForTrigger.AutoSize = true;
            lblEventForTrigger.Location = new System.Drawing.Point(13, 141);
            lblEventForTrigger.Margin = new Padding(4, 0, 4, 0);
            lblEventForTrigger.Name = "lblEventForTrigger";
            lblEventForTrigger.Size = new Size(36, 15);
            lblEventForTrigger.TabIndex = 96;
            lblEventForTrigger.Text = "Event";
            // 
            // lstEventTriggers
            // 
            lstEventTriggers.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstEventTriggers.BorderStyle = BorderStyle.FixedSingle;
            lstEventTriggers.ForeColor = System.Drawing.Color.Gainsboro;
            lstEventTriggers.FormattingEnabled = true;
            lstEventTriggers.ItemHeight = 15;
            lstEventTriggers.Location = new System.Drawing.Point(13, 22);
            lstEventTriggers.Margin = new Padding(4, 3, 4, 3);
            lstEventTriggers.Name = "lstEventTriggers";
            lstEventTriggers.Size = new Size(306, 107);
            lstEventTriggers.TabIndex = 59;
            lstEventTriggers.SelectedIndexChanged += lstEventTriggers_SelectedIndexChanged;
            // 
            // grpStack
            // 
            grpStack.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpStack.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpStack.Controls.Add(chkStackable);
            grpStack.Controls.Add(lblInvStackLimit);
            grpStack.Controls.Add(nudInvStackLimit);
            grpStack.Controls.Add(lblBankStackLimit);
            grpStack.Controls.Add(nudBankStackLimit);
            grpStack.ForeColor = System.Drawing.Color.Gainsboro;
            grpStack.Location = new System.Drawing.Point(596, 307);
            grpStack.Margin = new Padding(4, 3, 4, 3);
            grpStack.Name = "grpStack";
            grpStack.Padding = new Padding(4, 3, 4, 3);
            grpStack.Size = new Size(329, 122);
            grpStack.TabIndex = 88;
            grpStack.TabStop = false;
            grpStack.Text = "Stack Options:";
            // 
            // chkStackable
            // 
            chkStackable.AutoSize = true;
            chkStackable.Location = new System.Drawing.Point(13, 28);
            chkStackable.Margin = new Padding(4, 3, 4, 3);
            chkStackable.Name = "chkStackable";
            chkStackable.Size = new Size(81, 19);
            chkStackable.TabIndex = 27;
            chkStackable.Text = "Stackable?";
            chkStackable.CheckedChanged += chkStackable_CheckedChanged;
            // 
            // lblInvStackLimit
            // 
            lblInvStackLimit.AutoSize = true;
            lblInvStackLimit.Location = new System.Drawing.Point(9, 60);
            lblInvStackLimit.Margin = new Padding(4, 0, 4, 0);
            lblInvStackLimit.Name = "lblInvStackLimit";
            lblInvStackLimit.Size = new Size(121, 15);
            lblInvStackLimit.TabIndex = 95;
            lblInvStackLimit.Text = "Inventory Stack Limit:";
            // 
            // nudInvStackLimit
            // 
            nudInvStackLimit.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudInvStackLimit.ForeColor = System.Drawing.Color.Gainsboro;
            nudInvStackLimit.Location = new System.Drawing.Point(144, 52);
            nudInvStackLimit.Margin = new Padding(4, 3, 4, 3);
            nudInvStackLimit.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudInvStackLimit.Name = "nudInvStackLimit";
            nudInvStackLimit.Size = new Size(176, 23);
            nudInvStackLimit.TabIndex = 97;
            nudInvStackLimit.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudInvStackLimit.ValueChanged += nudInvStackLimit_ValueChanged;
            // 
            // lblBankStackLimit
            // 
            lblBankStackLimit.AutoSize = true;
            lblBankStackLimit.Location = new System.Drawing.Point(9, 88);
            lblBankStackLimit.Margin = new Padding(4, 0, 4, 0);
            lblBankStackLimit.Name = "lblBankStackLimit";
            lblBankStackLimit.Size = new Size(97, 15);
            lblBankStackLimit.TabIndex = 96;
            lblBankStackLimit.Text = "Bank Stack Limit:";
            // 
            // nudBankStackLimit
            // 
            nudBankStackLimit.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudBankStackLimit.ForeColor = System.Drawing.Color.Gainsboro;
            nudBankStackLimit.Location = new System.Drawing.Point(144, 85);
            nudBankStackLimit.Margin = new Padding(4, 3, 4, 3);
            nudBankStackLimit.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudBankStackLimit.Name = "nudBankStackLimit";
            nudBankStackLimit.Size = new Size(176, 23);
            nudBankStackLimit.TabIndex = 98;
            nudBankStackLimit.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudBankStackLimit.ValueChanged += nudBankStackLimit_ValueChanged;
            // 
            // grpCooldown
            // 
            grpCooldown.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpCooldown.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpCooldown.Controls.Add(lblCooldown);
            grpCooldown.Controls.Add(nudCooldown);
            grpCooldown.Controls.Add(lblCooldownGroup);
            grpCooldown.Controls.Add(cmbCooldownGroup);
            grpCooldown.Controls.Add(btnAddCooldownGroup);
            grpCooldown.Controls.Add(chkIgnoreGlobalCooldown);
            grpCooldown.Controls.Add(chkIgnoreCdr);
            grpCooldown.ForeColor = System.Drawing.Color.Gainsboro;
            grpCooldown.Location = new System.Drawing.Point(596, 142);
            grpCooldown.Margin = new Padding(4, 3, 4, 3);
            grpCooldown.Name = "grpCooldown";
            grpCooldown.Padding = new Padding(4, 3, 4, 3);
            grpCooldown.Size = new Size(329, 150);
            grpCooldown.TabIndex = 74;
            grpCooldown.TabStop = false;
            grpCooldown.Text = "Cooldown Options:";
            // 
            // lblCooldown
            // 
            lblCooldown.AutoSize = true;
            lblCooldown.Location = new System.Drawing.Point(9, 23);
            lblCooldown.Margin = new Padding(4, 0, 4, 0);
            lblCooldown.Name = "lblCooldown";
            lblCooldown.Size = new Size(92, 15);
            lblCooldown.TabIndex = 38;
            lblCooldown.Text = "Cooldown (ms):";
            // 
            // nudCooldown
            // 
            nudCooldown.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudCooldown.ForeColor = System.Drawing.Color.Gainsboro;
            nudCooldown.Location = new System.Drawing.Point(120, 21);
            nudCooldown.Margin = new Padding(4, 3, 4, 3);
            nudCooldown.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudCooldown.Name = "nudCooldown";
            nudCooldown.Size = new Size(200, 23);
            nudCooldown.TabIndex = 39;
            nudCooldown.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudCooldown.ValueChanged += nudCooldown_ValueChanged;
            // 
            // lblCooldownGroup
            // 
            lblCooldownGroup.AutoSize = true;
            lblCooldownGroup.Location = new System.Drawing.Point(9, 52);
            lblCooldownGroup.Margin = new Padding(4, 0, 4, 0);
            lblCooldownGroup.Name = "lblCooldownGroup";
            lblCooldownGroup.Size = new Size(101, 15);
            lblCooldownGroup.TabIndex = 50;
            lblCooldownGroup.Text = "Cooldown Group:";
            // 
            // cmbCooldownGroup
            // 
            cmbCooldownGroup.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbCooldownGroup.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbCooldownGroup.BorderStyle = ButtonBorderStyle.Solid;
            cmbCooldownGroup.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbCooldownGroup.DrawDropdownHoverOutline = false;
            cmbCooldownGroup.DrawFocusRectangle = false;
            cmbCooldownGroup.DrawMode = DrawMode.OwnerDrawFixed;
            cmbCooldownGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCooldownGroup.FlatStyle = FlatStyle.Flat;
            cmbCooldownGroup.ForeColor = System.Drawing.Color.Gainsboro;
            cmbCooldownGroup.FormattingEnabled = true;
            cmbCooldownGroup.Location = new System.Drawing.Point(120, 51);
            cmbCooldownGroup.Margin = new Padding(4, 3, 4, 3);
            cmbCooldownGroup.Name = "cmbCooldownGroup";
            cmbCooldownGroup.Size = new Size(167, 24);
            cmbCooldownGroup.TabIndex = 51;
            cmbCooldownGroup.Text = null;
            cmbCooldownGroup.TextPadding = new Padding(2);
            cmbCooldownGroup.SelectedIndexChanged += cmbCooldownGroup_SelectedIndexChanged;
            // 
            // btnAddCooldownGroup
            // 
            btnAddCooldownGroup.Location = new System.Drawing.Point(295, 51);
            btnAddCooldownGroup.Margin = new Padding(4, 3, 4, 3);
            btnAddCooldownGroup.Name = "btnAddCooldownGroup";
            btnAddCooldownGroup.Padding = new Padding(6);
            btnAddCooldownGroup.Size = new Size(24, 24);
            btnAddCooldownGroup.TabIndex = 52;
            btnAddCooldownGroup.Text = "+";
            btnAddCooldownGroup.Click += btnAddCooldownGroup_Click;
            // 
            // chkIgnoreGlobalCooldown
            // 
            chkIgnoreGlobalCooldown.AutoSize = true;
            chkIgnoreGlobalCooldown.Location = new System.Drawing.Point(13, 83);
            chkIgnoreGlobalCooldown.Margin = new Padding(4, 3, 4, 3);
            chkIgnoreGlobalCooldown.Name = "chkIgnoreGlobalCooldown";
            chkIgnoreGlobalCooldown.Size = new Size(160, 19);
            chkIgnoreGlobalCooldown.TabIndex = 53;
            chkIgnoreGlobalCooldown.Text = "Ignore Global Cooldown?";
            chkIgnoreGlobalCooldown.CheckedChanged += chkIgnoreGlobalCooldown_CheckedChanged;
            // 
            // chkIgnoreCdr
            // 
            chkIgnoreCdr.AutoSize = true;
            chkIgnoreCdr.Location = new System.Drawing.Point(13, 112);
            chkIgnoreCdr.Margin = new Padding(4, 3, 4, 3);
            chkIgnoreCdr.Name = "chkIgnoreCdr";
            chkIgnoreCdr.Size = new Size(180, 19);
            chkIgnoreCdr.TabIndex = 87;
            chkIgnoreCdr.Text = "Ignore Cooldown Reduction?";
            chkIgnoreCdr.CheckedChanged += chkIgnoreCdr_CheckedChanged;
            // 
            // nudItemDespawnTime
            // 
            nudItemDespawnTime.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudItemDespawnTime.ForeColor = System.Drawing.Color.Gainsboro;
            nudItemDespawnTime.Location = new System.Drawing.Point(13, 501);
            nudItemDespawnTime.Margin = new Padding(4, 3, 4, 3);
            nudItemDespawnTime.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
            nudItemDespawnTime.Name = "nudItemDespawnTime";
            nudItemDespawnTime.Size = new Size(271, 23);
            nudItemDespawnTime.TabIndex = 102;
            nudItemDespawnTime.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudItemDespawnTime.ValueChanged += nudItemDespawnTime_ValueChanged;
            // 
            // lblDespawnTime
            // 
            lblDespawnTime.AutoSize = true;
            lblDespawnTime.Location = new System.Drawing.Point(9, 479);
            lblDespawnTime.Margin = new Padding(4, 0, 4, 0);
            lblDespawnTime.Name = "lblDespawnTime";
            lblDespawnTime.Size = new Size(262, 15);
            lblDespawnTime.TabIndex = 101;
            lblDespawnTime.Text = "Item Despawn Time (ms):     [0 for server default]";
            // 
            // cmbEquipmentAnimation
            // 
            cmbEquipmentAnimation.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbEquipmentAnimation.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbEquipmentAnimation.BorderStyle = ButtonBorderStyle.Solid;
            cmbEquipmentAnimation.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbEquipmentAnimation.DrawDropdownHoverOutline = false;
            cmbEquipmentAnimation.DrawFocusRectangle = false;
            cmbEquipmentAnimation.DrawMode = DrawMode.OwnerDrawFixed;
            cmbEquipmentAnimation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEquipmentAnimation.FlatStyle = FlatStyle.Flat;
            cmbEquipmentAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            cmbEquipmentAnimation.FormattingEnabled = true;
            cmbEquipmentAnimation.Items.AddRange(new object[] { "None" });
            cmbEquipmentAnimation.Location = new System.Drawing.Point(321, 268);
            cmbEquipmentAnimation.Margin = new Padding(4, 3, 4, 3);
            cmbEquipmentAnimation.Name = "cmbEquipmentAnimation";
            cmbEquipmentAnimation.Size = new Size(252, 24);
            cmbEquipmentAnimation.TabIndex = 57;
            cmbEquipmentAnimation.Text = "None";
            cmbEquipmentAnimation.TextPadding = new Padding(2);
            cmbEquipmentAnimation.SelectedIndexChanged += cmbEquipmentAnimation_SelectedIndexChanged;
            // 
            // grpRequirements
            // 
            grpRequirements.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpRequirements.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpRequirements.Controls.Add(lblCannotUse);
            grpRequirements.Controls.Add(txtCannotUse);
            grpRequirements.Controls.Add(btnEditRequirements);
            grpRequirements.ForeColor = System.Drawing.Color.Gainsboro;
            grpRequirements.Location = new System.Drawing.Point(596, 21);
            grpRequirements.Margin = new Padding(2);
            grpRequirements.Name = "grpRequirements";
            grpRequirements.Padding = new Padding(2);
            grpRequirements.Size = new Size(329, 106);
            grpRequirements.TabIndex = 100;
            grpRequirements.TabStop = false;
            grpRequirements.Text = "Requirements";
            // 
            // lblCannotUse
            // 
            lblCannotUse.AutoSize = true;
            lblCannotUse.Location = new System.Drawing.Point(6, 54);
            lblCannotUse.Margin = new Padding(4, 0, 4, 0);
            lblCannotUse.Name = "lblCannotUse";
            lblCannotUse.Size = new Size(120, 15);
            lblCannotUse.TabIndex = 54;
            lblCannotUse.Text = "Cannot Use Message:";
            // 
            // txtCannotUse
            // 
            txtCannotUse.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtCannotUse.BorderStyle = BorderStyle.FixedSingle;
            txtCannotUse.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtCannotUse.Location = new System.Drawing.Point(9, 73);
            txtCannotUse.Margin = new Padding(4, 3, 4, 3);
            txtCannotUse.Name = "txtCannotUse";
            txtCannotUse.Size = new Size(311, 23);
            txtCannotUse.TabIndex = 53;
            txtCannotUse.TextChanged += txtCannotUse_TextChanged;
            // 
            // btnEditRequirements
            // 
            btnEditRequirements.Location = new System.Drawing.Point(9, 21);
            btnEditRequirements.Margin = new Padding(4, 3, 4, 3);
            btnEditRequirements.Name = "btnEditRequirements";
            btnEditRequirements.Padding = new Padding(6);
            btnEditRequirements.Size = new Size(312, 27);
            btnEditRequirements.TabIndex = 0;
            btnEditRequirements.Text = "Edit Usage Requirements";
            btnEditRequirements.Click += btnEditRequirements_Click;
            // 
            // chkCanGuildBank
            // 
            chkCanGuildBank.AutoSize = true;
            chkCanGuildBank.Location = new System.Drawing.Point(160, 384);
            chkCanGuildBank.Margin = new Padding(4, 3, 4, 3);
            chkCanGuildBank.Name = "chkCanGuildBank";
            chkCanGuildBank.Size = new Size(112, 19);
            chkCanGuildBank.TabIndex = 99;
            chkCanGuildBank.Text = "Can Guild Bank?";
            chkCanGuildBank.CheckedChanged += chkCanGuildBank_CheckedChanged;
            // 
            // lblEquipmentAnimation
            // 
            lblEquipmentAnimation.AutoSize = true;
            lblEquipmentAnimation.Location = new System.Drawing.Point(317, 249);
            lblEquipmentAnimation.Margin = new Padding(4, 0, 4, 0);
            lblEquipmentAnimation.Name = "lblEquipmentAnimation";
            lblEquipmentAnimation.Size = new Size(127, 15);
            lblEquipmentAnimation.TabIndex = 56;
            lblEquipmentAnimation.Text = "Equipment Animation:";
            // 
            // nudDeathDropChance
            // 
            nudDeathDropChance.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDeathDropChance.ForeColor = System.Drawing.Color.Gainsboro;
            nudDeathDropChance.Location = new System.Drawing.Point(175, 442);
            nudDeathDropChance.Margin = new Padding(4, 3, 4, 3);
            nudDeathDropChance.Name = "nudDeathDropChance";
            nudDeathDropChance.Size = new Size(108, 23);
            nudDeathDropChance.TabIndex = 94;
            nudDeathDropChance.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudDeathDropChance.ValueChanged += nudDeathDropChance_ValueChanged;
            // 
            // lblDeathDropChance
            // 
            lblDeathDropChance.AutoSize = true;
            lblDeathDropChance.Location = new System.Drawing.Point(9, 447);
            lblDeathDropChance.Margin = new Padding(4, 0, 4, 0);
            lblDeathDropChance.Name = "lblDeathDropChance";
            lblDeathDropChance.Size = new Size(149, 15);
            lblDeathDropChance.TabIndex = 93;
            lblDeathDropChance.Text = "Drop chance on Death (%):";
            // 
            // chkCanSell
            // 
            chkCanSell.AutoSize = true;
            chkCanSell.Location = new System.Drawing.Point(27, 410);
            chkCanSell.Margin = new Padding(4, 3, 4, 3);
            chkCanSell.Name = "chkCanSell";
            chkCanSell.Size = new Size(73, 19);
            chkCanSell.TabIndex = 92;
            chkCanSell.Text = "Can Sell?";
            chkCanSell.CheckedChanged += chkCanSell_CheckedChanged;
            // 
            // chkCanTrade
            // 
            chkCanTrade.AutoSize = true;
            chkCanTrade.Location = new System.Drawing.Point(27, 384);
            chkCanTrade.Margin = new Padding(4, 3, 4, 3);
            chkCanTrade.Name = "chkCanTrade";
            chkCanTrade.Size = new Size(83, 19);
            chkCanTrade.TabIndex = 91;
            chkCanTrade.Text = "Can Trade?";
            chkCanTrade.CheckedChanged += chkCanTrade_CheckedChanged;
            // 
            // chkCanBag
            // 
            chkCanBag.AutoSize = true;
            chkCanBag.Location = new System.Drawing.Point(160, 410);
            chkCanBag.Margin = new Padding(4, 3, 4, 3);
            chkCanBag.Name = "chkCanBag";
            chkCanBag.Size = new Size(75, 19);
            chkCanBag.TabIndex = 90;
            chkCanBag.Text = "Can Bag?";
            chkCanBag.CheckedChanged += chkCanBag_CheckedChanged;
            // 
            // chkCanBank
            // 
            chkCanBank.AutoSize = true;
            chkCanBank.Location = new System.Drawing.Point(160, 359);
            chkCanBank.Margin = new Padding(4, 3, 4, 3);
            chkCanBank.Name = "chkCanBank";
            chkCanBank.Size = new Size(81, 19);
            chkCanBank.TabIndex = 89;
            chkCanBank.Text = "Can Bank?";
            chkCanBank.CheckedChanged += chkCanBank_CheckedChanged;
            // 
            // lblAlpha
            // 
            lblAlpha.AutoSize = true;
            lblAlpha.Location = new System.Drawing.Point(451, 115);
            lblAlpha.Margin = new Padding(4, 0, 4, 0);
            lblAlpha.Name = "lblAlpha";
            lblAlpha.Size = new Size(41, 15);
            lblAlpha.TabIndex = 86;
            lblAlpha.Text = "Alpha:";
            // 
            // lblBlue
            // 
            lblBlue.AutoSize = true;
            lblBlue.Location = new System.Drawing.Point(317, 115);
            lblBlue.Margin = new Padding(4, 0, 4, 0);
            lblBlue.Name = "lblBlue";
            lblBlue.Size = new Size(33, 15);
            lblBlue.TabIndex = 85;
            lblBlue.Text = "Blue:";
            // 
            // lblGreen
            // 
            lblGreen.AutoSize = true;
            lblGreen.Location = new System.Drawing.Point(451, 85);
            lblGreen.Margin = new Padding(4, 0, 4, 0);
            lblGreen.Name = "lblGreen";
            lblGreen.Size = new Size(41, 15);
            lblGreen.TabIndex = 84;
            lblGreen.Text = "Green:";
            // 
            // lblRed
            // 
            lblRed.AutoSize = true;
            lblRed.Location = new System.Drawing.Point(317, 85);
            lblRed.Margin = new Padding(4, 0, 4, 0);
            lblRed.Name = "lblRed";
            lblRed.Size = new Size(30, 15);
            lblRed.TabIndex = 83;
            lblRed.Text = "Red:";
            // 
            // nudRgbaA
            // 
            nudRgbaA.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudRgbaA.ForeColor = System.Drawing.Color.Gainsboro;
            nudRgbaA.Location = new System.Drawing.Point(504, 113);
            nudRgbaA.Margin = new Padding(4, 3, 4, 3);
            nudRgbaA.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaA.Name = "nudRgbaA";
            nudRgbaA.Size = new Size(70, 23);
            nudRgbaA.TabIndex = 82;
            nudRgbaA.Value = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaA.ValueChanged += nudRgbaA_ValueChanged;
            // 
            // nudRgbaB
            // 
            nudRgbaB.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudRgbaB.ForeColor = System.Drawing.Color.Gainsboro;
            nudRgbaB.Location = new System.Drawing.Point(360, 113);
            nudRgbaB.Margin = new Padding(4, 3, 4, 3);
            nudRgbaB.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaB.Name = "nudRgbaB";
            nudRgbaB.Size = new Size(70, 23);
            nudRgbaB.TabIndex = 81;
            nudRgbaB.Value = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaB.ValueChanged += nudRgbaB_ValueChanged;
            // 
            // nudRgbaG
            // 
            nudRgbaG.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudRgbaG.ForeColor = System.Drawing.Color.Gainsboro;
            nudRgbaG.Location = new System.Drawing.Point(504, 83);
            nudRgbaG.Margin = new Padding(4, 3, 4, 3);
            nudRgbaG.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaG.Name = "nudRgbaG";
            nudRgbaG.Size = new Size(70, 23);
            nudRgbaG.TabIndex = 80;
            nudRgbaG.Value = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaG.ValueChanged += nudRgbaG_ValueChanged;
            // 
            // nudRgbaR
            // 
            nudRgbaR.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudRgbaR.ForeColor = System.Drawing.Color.Gainsboro;
            nudRgbaR.Location = new System.Drawing.Point(360, 83);
            nudRgbaR.Margin = new Padding(4, 3, 4, 3);
            nudRgbaR.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaR.Name = "nudRgbaR";
            nudRgbaR.Size = new Size(70, 23);
            nudRgbaR.TabIndex = 79;
            nudRgbaR.Value = new decimal(new int[] { 255, 0, 0, 0 });
            nudRgbaR.ValueChanged += nudRgbaR_ValueChanged;
            // 
            // btnAddFolder
            // 
            btnAddFolder.Location = new System.Drawing.Point(262, 67);
            btnAddFolder.Margin = new Padding(4, 3, 4, 3);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Padding = new Padding(6);
            btnAddFolder.Size = new Size(21, 24);
            btnAddFolder.TabIndex = 49;
            btnAddFolder.Text = "+";
            btnAddFolder.Click += btnAddFolder_Click;
            // 
            // lblFolder
            // 
            lblFolder.AutoSize = true;
            lblFolder.Location = new System.Drawing.Point(9, 72);
            lblFolder.Margin = new Padding(4, 0, 4, 0);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new Size(43, 15);
            lblFolder.TabIndex = 48;
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
            cmbFolder.Location = new System.Drawing.Point(62, 67);
            cmbFolder.Margin = new Padding(4, 3, 4, 3);
            cmbFolder.Name = "cmbFolder";
            cmbFolder.Size = new Size(193, 24);
            cmbFolder.TabIndex = 47;
            cmbFolder.Text = null;
            cmbFolder.TextPadding = new Padding(2);
            cmbFolder.SelectedIndexChanged += cmbFolder_SelectedIndexChanged;
            // 
            // cmbRarity
            // 
            cmbRarity.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbRarity.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbRarity.BorderStyle = ButtonBorderStyle.Solid;
            cmbRarity.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbRarity.DrawDropdownHoverOutline = false;
            cmbRarity.DrawFocusRectangle = false;
            cmbRarity.DrawMode = DrawMode.OwnerDrawFixed;
            cmbRarity.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRarity.FlatStyle = FlatStyle.Flat;
            cmbRarity.ForeColor = System.Drawing.Color.Gainsboro;
            cmbRarity.FormattingEnabled = true;
            cmbRarity.Items.AddRange(new object[] { "None", "Common", "Uncommon", "Rare", "Epic", "Legendary" });
            cmbRarity.Location = new System.Drawing.Point(62, 147);
            cmbRarity.Margin = new Padding(4, 3, 4, 3);
            cmbRarity.Name = "cmbRarity";
            cmbRarity.Size = new Size(221, 24);
            cmbRarity.TabIndex = 41;
            cmbRarity.Text = "None";
            cmbRarity.TextPadding = new Padding(2);
            cmbRarity.SelectedIndexChanged += cmbRarity_SelectedIndexChanged;
            // 
            // lblRarity
            // 
            lblRarity.AutoSize = true;
            lblRarity.Location = new System.Drawing.Point(9, 150);
            lblRarity.Margin = new Padding(4, 0, 4, 0);
            lblRarity.Name = "lblRarity";
            lblRarity.Size = new Size(40, 15);
            lblRarity.TabIndex = 40;
            lblRarity.Text = "Rarity:";
            // 
            // nudPrice
            // 
            nudPrice.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudPrice.ForeColor = System.Drawing.Color.Gainsboro;
            nudPrice.Location = new System.Drawing.Point(321, 167);
            nudPrice.Margin = new Padding(4, 3, 4, 3);
            nudPrice.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudPrice.Name = "nudPrice";
            nudPrice.Size = new Size(253, 23);
            nudPrice.TabIndex = 37;
            nudPrice.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudPrice.ValueChanged += nudPrice_ValueChanged;
            // 
            // chkCanDrop
            // 
            chkCanDrop.AutoSize = true;
            chkCanDrop.Location = new System.Drawing.Point(27, 359);
            chkCanDrop.Margin = new Padding(4, 3, 4, 3);
            chkCanDrop.Name = "chkCanDrop";
            chkCanDrop.Size = new Size(81, 19);
            chkCanDrop.TabIndex = 26;
            chkCanDrop.Text = "Can Drop?";
            chkCanDrop.CheckedChanged += chkBound_CheckedChanged;
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
            cmbAnimation.Location = new System.Drawing.Point(321, 216);
            cmbAnimation.Margin = new Padding(4, 3, 4, 3);
            cmbAnimation.Name = "cmbAnimation";
            cmbAnimation.Size = new Size(252, 24);
            cmbAnimation.TabIndex = 14;
            cmbAnimation.Text = null;
            cmbAnimation.TextPadding = new Padding(2);
            cmbAnimation.SelectedIndexChanged += cmbAnimation_SelectedIndexChanged;
            // 
            // lblDesc
            // 
            lblDesc.AutoSize = true;
            lblDesc.Location = new System.Drawing.Point(13, 175);
            lblDesc.Margin = new Padding(4, 0, 4, 0);
            lblDesc.Name = "lblDesc";
            lblDesc.Size = new Size(35, 15);
            lblDesc.TabIndex = 13;
            lblDesc.Text = "Desc:";
            // 
            // txtDesc
            // 
            txtDesc.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtDesc.BorderStyle = BorderStyle.FixedSingle;
            txtDesc.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtDesc.Location = new System.Drawing.Point(14, 194);
            txtDesc.Margin = new Padding(4, 3, 4, 3);
            txtDesc.Multiline = true;
            txtDesc.Name = "txtDesc";
            txtDesc.Size = new Size(269, 153);
            txtDesc.TabIndex = 12;
            txtDesc.TextChanged += txtDesc_TextChanged;
            // 
            // cmbPic
            // 
            cmbPic.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbPic.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbPic.BorderStyle = ButtonBorderStyle.Solid;
            cmbPic.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbPic.DrawDropdownHoverOutline = false;
            cmbPic.DrawFocusRectangle = false;
            cmbPic.DrawMode = DrawMode.OwnerDrawFixed;
            cmbPic.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPic.FlatStyle = FlatStyle.Flat;
            cmbPic.ForeColor = System.Drawing.Color.Gainsboro;
            cmbPic.FormattingEnabled = true;
            cmbPic.Items.AddRange(new object[] { "None" });
            cmbPic.Location = new System.Drawing.Point(327, 45);
            cmbPic.Margin = new Padding(4, 3, 4, 3);
            cmbPic.Name = "cmbPic";
            cmbPic.Size = new Size(184, 24);
            cmbPic.TabIndex = 11;
            cmbPic.Text = "None";
            cmbPic.TextPadding = new Padding(2);
            cmbPic.SelectedIndexChanged += cmbPic_SelectedIndexChanged;
            // 
            // lblAnim
            // 
            lblAnim.AutoSize = true;
            lblAnim.Location = new System.Drawing.Point(317, 197);
            lblAnim.Margin = new Padding(4, 0, 4, 0);
            lblAnim.Name = "lblAnim";
            lblAnim.Size = new Size(102, 15);
            lblAnim.TabIndex = 9;
            lblAnim.Text = "Animation on Use";
            // 
            // lblPrice
            // 
            lblPrice.AutoSize = true;
            lblPrice.Location = new System.Drawing.Point(317, 148);
            lblPrice.Margin = new Padding(4, 0, 4, 0);
            lblPrice.Name = "lblPrice";
            lblPrice.Size = new Size(36, 15);
            lblPrice.TabIndex = 7;
            lblPrice.Text = "Price:";
            // 
            // lblPic
            // 
            lblPic.AutoSize = true;
            lblPic.Location = new System.Drawing.Point(323, 25);
            lblPic.Margin = new Padding(4, 0, 4, 0);
            lblPic.Name = "lblPic";
            lblPic.Size = new Size(26, 15);
            lblPic.TabIndex = 6;
            lblPic.Text = "Pic:";
            // 
            // picItem
            // 
            picItem.BackColor = System.Drawing.Color.Black;
            picItem.Location = new System.Drawing.Point(518, 21);
            picItem.Margin = new Padding(4, 3, 4, 3);
            picItem.Name = "picItem";
            picItem.Size = new Size(56, 55);
            picItem.TabIndex = 4;
            picItem.TabStop = false;
            // 
            // lblType
            // 
            lblType.AutoSize = true;
            lblType.Location = new System.Drawing.Point(9, 111);
            lblType.Margin = new Padding(4, 0, 4, 0);
            lblType.Name = "lblType";
            lblType.Size = new Size(34, 15);
            lblType.TabIndex = 3;
            lblType.Text = "Type:";
            // 
            // cmbType
            // 
            cmbType.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbType.BorderStyle = ButtonBorderStyle.Solid;
            cmbType.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbType.DrawDropdownHoverOutline = false;
            cmbType.DrawFocusRectangle = false;
            cmbType.DrawMode = DrawMode.OwnerDrawFixed;
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.FlatStyle = FlatStyle.Flat;
            cmbType.ForeColor = System.Drawing.Color.Gainsboro;
            cmbType.FormattingEnabled = true;
            cmbType.Items.AddRange(new object[] { "None", "Equipment", "Consumable", "Currency", "Spell", "Event", "Bag" });
            cmbType.Location = new System.Drawing.Point(62, 107);
            cmbType.Margin = new Padding(4, 3, 4, 3);
            cmbType.Name = "cmbType";
            cmbType.Size = new Size(221, 24);
            cmbType.TabIndex = 2;
            cmbType.Text = "None";
            cmbType.TextPadding = new Padding(2);
            cmbType.SelectedIndexChanged += cmbType_SelectedIndexChanged;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(9, 31);
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
            txtName.Location = new System.Drawing.Point(62, 29);
            txtName.Margin = new Padding(4, 3, 4, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(221, 23);
            txtName.TabIndex = 0;
            txtName.TextChanged += txtName_TextChanged;
            // 
            // grpConsumable
            // 
            grpConsumable.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpConsumable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpConsumable.Controls.Add(lblPercentage3);
            grpConsumable.Controls.Add(nudIntervalPercentage);
            grpConsumable.Controls.Add(lblPlus3);
            grpConsumable.Controls.Add(nudInterval);
            grpConsumable.Controls.Add(lblVital);
            grpConsumable.Controls.Add(cmbConsume);
            grpConsumable.Controls.Add(lblInterval);
            grpConsumable.ForeColor = System.Drawing.Color.Gainsboro;
            grpConsumable.Location = new System.Drawing.Point(321, 307);
            grpConsumable.Margin = new Padding(4, 3, 4, 3);
            grpConsumable.Name = "grpConsumable";
            grpConsumable.Padding = new Padding(4, 3, 4, 3);
            grpConsumable.Size = new Size(253, 150);
            grpConsumable.TabIndex = 12;
            grpConsumable.TabStop = false;
            grpConsumable.Text = "Consumable";
            grpConsumable.Visible = false;
            // 
            // lblPercentage3
            // 
            lblPercentage3.AutoSize = true;
            lblPercentage3.Location = new System.Drawing.Point(227, 105);
            lblPercentage3.Margin = new Padding(2, 0, 2, 0);
            lblPercentage3.Name = "lblPercentage3";
            lblPercentage3.Size = new Size(17, 15);
            lblPercentage3.TabIndex = 73;
            lblPercentage3.Text = "%";
            // 
            // nudIntervalPercentage
            // 
            nudIntervalPercentage.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudIntervalPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            nudIntervalPercentage.Location = new System.Drawing.Point(173, 104);
            nudIntervalPercentage.Margin = new Padding(4, 3, 4, 3);
            nudIntervalPercentage.Name = "nudIntervalPercentage";
            nudIntervalPercentage.Size = new Size(50, 23);
            nudIntervalPercentage.TabIndex = 72;
            nudIntervalPercentage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudIntervalPercentage.ValueChanged += nudIntervalPercentage_ValueChanged;
            // 
            // lblPlus3
            // 
            lblPlus3.AutoSize = true;
            lblPlus3.Location = new System.Drawing.Point(152, 105);
            lblPlus3.Margin = new Padding(2, 0, 2, 0);
            lblPlus3.Name = "lblPlus3";
            lblPlus3.Size = new Size(15, 15);
            lblPlus3.TabIndex = 71;
            lblPlus3.Text = "+";
            // 
            // nudInterval
            // 
            nudInterval.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudInterval.ForeColor = System.Drawing.Color.Gainsboro;
            nudInterval.Location = new System.Drawing.Point(22, 104);
            nudInterval.Margin = new Padding(4, 3, 4, 3);
            nudInterval.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudInterval.Minimum = new decimal(new int[] { 10000, 0, 0, int.MinValue });
            nudInterval.Name = "nudInterval";
            nudInterval.Size = new Size(124, 23);
            nudInterval.TabIndex = 37;
            nudInterval.Value = new decimal(new int[] { 10000, 0, 0, 0 });
            nudInterval.ValueChanged += nudInterval_ValueChanged;
            // 
            // lblVital
            // 
            lblVital.AutoSize = true;
            lblVital.Location = new System.Drawing.Point(19, 20);
            lblVital.Margin = new Padding(4, 0, 4, 0);
            lblVital.Name = "lblVital";
            lblVital.Size = new Size(33, 15);
            lblVital.TabIndex = 12;
            lblVital.Text = "Vital:";
            // 
            // cmbConsume
            // 
            cmbConsume.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbConsume.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbConsume.BorderStyle = ButtonBorderStyle.Solid;
            cmbConsume.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbConsume.DrawDropdownHoverOutline = false;
            cmbConsume.DrawFocusRectangle = false;
            cmbConsume.DrawMode = DrawMode.OwnerDrawFixed;
            cmbConsume.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbConsume.FlatStyle = FlatStyle.Flat;
            cmbConsume.ForeColor = System.Drawing.Color.Gainsboro;
            cmbConsume.FormattingEnabled = true;
            cmbConsume.Items.AddRange(new object[] { "Health", "Mana", "Experience" });
            cmbConsume.Location = new System.Drawing.Point(22, 43);
            cmbConsume.Margin = new Padding(4, 3, 4, 3);
            cmbConsume.Name = "cmbConsume";
            cmbConsume.Size = new Size(205, 24);
            cmbConsume.TabIndex = 11;
            cmbConsume.Text = "Health";
            cmbConsume.TextPadding = new Padding(2);
            cmbConsume.SelectedIndexChanged += cmbConsume_SelectedIndexChanged;
            // 
            // lblInterval
            // 
            lblInterval.AutoSize = true;
            lblInterval.Location = new System.Drawing.Point(22, 82);
            lblInterval.Margin = new Padding(4, 0, 4, 0);
            lblInterval.Name = "lblInterval";
            lblInterval.Size = new Size(49, 15);
            lblInterval.TabIndex = 9;
            lblInterval.Text = "Interval:";
            // 
            // grpEvent
            // 
            grpEvent.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpEvent.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpEvent.Controls.Add(chkSingleUseEvent);
            grpEvent.Controls.Add(cmbEvent);
            grpEvent.ForeColor = System.Drawing.Color.Gainsboro;
            grpEvent.Location = new System.Drawing.Point(321, 307);
            grpEvent.Margin = new Padding(4, 3, 4, 3);
            grpEvent.Name = "grpEvent";
            grpEvent.Padding = new Padding(4, 3, 4, 3);
            grpEvent.Size = new Size(233, 75);
            grpEvent.TabIndex = 42;
            grpEvent.TabStop = false;
            grpEvent.Text = "Event";
            grpEvent.Visible = false;
            // 
            // chkSingleUseEvent
            // 
            chkSingleUseEvent.AutoSize = true;
            chkSingleUseEvent.Checked = true;
            chkSingleUseEvent.CheckState = CheckState.Checked;
            chkSingleUseEvent.Location = new System.Drawing.Point(10, 48);
            chkSingleUseEvent.Margin = new Padding(4, 3, 4, 3);
            chkSingleUseEvent.Name = "chkSingleUseEvent";
            chkSingleUseEvent.Size = new Size(112, 19);
            chkSingleUseEvent.TabIndex = 29;
            chkSingleUseEvent.Text = "Destroy On Use?";
            chkSingleUseEvent.CheckedChanged += chkSingleUse_CheckedChanged;
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
            cmbEvent.Location = new System.Drawing.Point(10, 17);
            cmbEvent.Margin = new Padding(4, 3, 4, 3);
            cmbEvent.Name = "cmbEvent";
            cmbEvent.Size = new Size(215, 24);
            cmbEvent.TabIndex = 17;
            cmbEvent.Text = null;
            cmbEvent.TextPadding = new Padding(2);
            cmbEvent.SelectedIndexChanged += cmbEvent_SelectedIndexChanged;
            // 
            // grpBags
            // 
            grpBags.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpBags.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpBags.Controls.Add(nudBag);
            grpBags.Controls.Add(lblBag);
            grpBags.ForeColor = System.Drawing.Color.Gainsboro;
            grpBags.Location = new System.Drawing.Point(321, 307);
            grpBags.Margin = new Padding(4, 3, 4, 3);
            grpBags.Name = "grpBags";
            grpBags.Padding = new Padding(4, 3, 4, 3);
            grpBags.Size = new Size(253, 66);
            grpBags.TabIndex = 44;
            grpBags.TabStop = false;
            grpBags.Text = "Bag:";
            grpBags.Visible = false;
            // 
            // nudBag
            // 
            nudBag.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudBag.ForeColor = System.Drawing.Color.Gainsboro;
            nudBag.Location = new System.Drawing.Point(80, 27);
            nudBag.Margin = new Padding(4, 3, 4, 3);
            nudBag.Maximum = new decimal(new int[] { 35, 0, 0, 0 });
            nudBag.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudBag.Name = "nudBag";
            nudBag.Size = new Size(168, 23);
            nudBag.TabIndex = 38;
            nudBag.Value = new decimal(new int[] { 1, 0, 0, 0 });
            nudBag.ValueChanged += nudBag_ValueChanged;
            // 
            // lblBag
            // 
            lblBag.AutoSize = true;
            lblBag.Location = new System.Drawing.Point(9, 29);
            lblBag.Margin = new Padding(4, 0, 4, 0);
            lblBag.Name = "lblBag";
            lblBag.Size = new Size(58, 15);
            lblBag.TabIndex = 11;
            lblBag.Text = "Bag Slots:";
            // 
            // grpSpell
            // 
            grpSpell.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpSpell.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpSpell.Controls.Add(chkSingleUseSpell);
            grpSpell.Controls.Add(chkQuickCast);
            grpSpell.Controls.Add(cmbTeachSpell);
            grpSpell.Controls.Add(lblSpell);
            grpSpell.ForeColor = System.Drawing.Color.Gainsboro;
            grpSpell.Location = new System.Drawing.Point(321, 307);
            grpSpell.Margin = new Padding(4, 3, 4, 3);
            grpSpell.Name = "grpSpell";
            grpSpell.Padding = new Padding(4, 3, 4, 3);
            grpSpell.Size = new Size(253, 150);
            grpSpell.TabIndex = 13;
            grpSpell.TabStop = false;
            grpSpell.Text = "Spell";
            grpSpell.Visible = false;
            // 
            // chkSingleUseSpell
            // 
            chkSingleUseSpell.AutoSize = true;
            chkSingleUseSpell.Checked = true;
            chkSingleUseSpell.CheckState = CheckState.Checked;
            chkSingleUseSpell.Location = new System.Drawing.Point(18, 110);
            chkSingleUseSpell.Margin = new Padding(4, 3, 4, 3);
            chkSingleUseSpell.Name = "chkSingleUseSpell";
            chkSingleUseSpell.Size = new Size(112, 19);
            chkSingleUseSpell.TabIndex = 29;
            chkSingleUseSpell.Text = "Destroy On Use?";
            chkSingleUseSpell.CheckedChanged += chkSingleUse_CheckedChanged;
            // 
            // chkQuickCast
            // 
            chkQuickCast.AutoSize = true;
            chkQuickCast.Location = new System.Drawing.Point(18, 83);
            chkQuickCast.Margin = new Padding(4, 3, 4, 3);
            chkQuickCast.Name = "chkQuickCast";
            chkQuickCast.Size = new Size(116, 19);
            chkQuickCast.TabIndex = 28;
            chkQuickCast.Text = "Quick Cast Spell?";
            chkQuickCast.CheckedChanged += chkQuickCast_CheckedChanged;
            // 
            // cmbTeachSpell
            // 
            cmbTeachSpell.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbTeachSpell.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbTeachSpell.BorderStyle = ButtonBorderStyle.Solid;
            cmbTeachSpell.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbTeachSpell.DrawDropdownHoverOutline = false;
            cmbTeachSpell.DrawFocusRectangle = false;
            cmbTeachSpell.DrawMode = DrawMode.OwnerDrawFixed;
            cmbTeachSpell.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTeachSpell.FlatStyle = FlatStyle.Flat;
            cmbTeachSpell.ForeColor = System.Drawing.Color.Gainsboro;
            cmbTeachSpell.FormattingEnabled = true;
            cmbTeachSpell.Location = new System.Drawing.Point(18, 46);
            cmbTeachSpell.Margin = new Padding(4, 3, 4, 3);
            cmbTeachSpell.Name = "cmbTeachSpell";
            cmbTeachSpell.Size = new Size(209, 24);
            cmbTeachSpell.TabIndex = 12;
            cmbTeachSpell.Text = null;
            cmbTeachSpell.TextPadding = new Padding(2);
            cmbTeachSpell.SelectedIndexChanged += cmbTeachSpell_SelectedIndexChanged;
            // 
            // lblSpell
            // 
            lblSpell.AutoSize = true;
            lblSpell.Location = new System.Drawing.Point(14, 24);
            lblSpell.Margin = new Padding(4, 0, 4, 0);
            lblSpell.Name = "lblSpell";
            lblSpell.Size = new Size(35, 15);
            lblSpell.TabIndex = 11;
            lblSpell.Text = "Spell:";
            // 
            // grpEquipment
            // 
            grpEquipment.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpEquipment.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpEquipment.Controls.Add(grpStatRanges);
            grpEquipment.Controls.Add(grpPaperdoll);
            grpEquipment.Controls.Add(grpEffects);
            grpEquipment.Controls.Add(grpRegen);
            grpEquipment.Controls.Add(grpVitalBonuses);
            grpEquipment.Controls.Add(grpStatBonuses);
            grpEquipment.Controls.Add(cmbEquipmentSlot);
            grpEquipment.Controls.Add(lblEquipmentSlot);
            grpEquipment.Controls.Add(grpWeaponProperties);
            grpEquipment.Controls.Add(grpShieldProperties);
            grpEquipment.ForeColor = System.Drawing.Color.Gainsboro;
            grpEquipment.Location = new System.Drawing.Point(11, 643);
            grpEquipment.Margin = new Padding(4, 3, 4, 3);
            grpEquipment.Name = "grpEquipment";
            grpEquipment.Padding = new Padding(4, 3, 4, 3);
            grpEquipment.Size = new Size(943, 834);
            grpEquipment.TabIndex = 12;
            grpEquipment.TabStop = false;
            grpEquipment.Text = "Equipment";
            grpEquipment.Visible = false;
            // 
            // grpStatRanges
            // 
            grpStatRanges.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpStatRanges.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpStatRanges.Controls.Add(lblStatRangeFrom);
            grpStatRanges.Controls.Add(lblStatRangeTo);
            grpStatRanges.Controls.Add(nudStatRangeLow);
            grpStatRanges.Controls.Add(lstStatRanges);
            grpStatRanges.Controls.Add(nudStatRangeHigh);
            grpStatRanges.ForeColor = System.Drawing.Color.Gainsboro;
            grpStatRanges.Location = new System.Drawing.Point(327, 578);
            grpStatRanges.Margin = new Padding(4, 3, 4, 3);
            grpStatRanges.Name = "grpStatRanges";
            grpStatRanges.Padding = new Padding(4, 3, 4, 3);
            grpStatRanges.Size = new Size(217, 213);
            grpStatRanges.TabIndex = 83;
            grpStatRanges.TabStop = false;
            grpStatRanges.Text = "Stat Ranges";
            // 
            // lblStatRangeFrom
            // 
            lblStatRangeFrom.AutoSize = true;
            lblStatRangeFrom.Location = new System.Drawing.Point(8, 177);
            lblStatRangeFrom.Margin = new Padding(4, 0, 4, 0);
            lblStatRangeFrom.Name = "lblStatRangeFrom";
            lblStatRangeFrom.Size = new Size(35, 15);
            lblStatRangeFrom.TabIndex = 62;
            lblStatRangeFrom.Text = "From";
            lblStatRangeFrom.TextAlign = ContentAlignment.TopRight;
            // 
            // lblStatRangeTo
            // 
            lblStatRangeTo.AutoSize = true;
            lblStatRangeTo.Location = new System.Drawing.Point(122, 177);
            lblStatRangeTo.Margin = new Padding(4, 0, 4, 0);
            lblStatRangeTo.Name = "lblStatRangeTo";
            lblStatRangeTo.Size = new Size(18, 15);
            lblStatRangeTo.TabIndex = 61;
            lblStatRangeTo.Text = "to";
            lblStatRangeTo.TextAlign = ContentAlignment.TopCenter;
            // 
            // nudStatRangeLow
            // 
            nudStatRangeLow.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudStatRangeLow.ForeColor = System.Drawing.Color.Gainsboro;
            nudStatRangeLow.Location = new System.Drawing.Point(54, 175);
            nudStatRangeLow.Margin = new Padding(4, 3, 4, 3);
            nudStatRangeLow.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudStatRangeLow.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            nudStatRangeLow.Name = "nudStatRangeLow";
            nudStatRangeLow.Size = new Size(54, 23);
            nudStatRangeLow.TabIndex = 60;
            nudStatRangeLow.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudStatRangeLow.ValueChanged += nudStatRangeLow_ValueChanged;
            // 
            // lstStatRanges
            // 
            lstStatRanges.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstStatRanges.BorderStyle = BorderStyle.FixedSingle;
            lstStatRanges.ForeColor = System.Drawing.Color.Gainsboro;
            lstStatRanges.FormattingEnabled = true;
            lstStatRanges.ItemHeight = 15;
            lstStatRanges.Location = new System.Drawing.Point(8, 28);
            lstStatRanges.Margin = new Padding(4, 3, 4, 3);
            lstStatRanges.Name = "lstStatRanges";
            lstStatRanges.Size = new Size(198, 137);
            lstStatRanges.TabIndex = 59;
            lstStatRanges.SelectedIndexChanged += lstStatRanges_SelectedIndexChanged;
            // 
            // nudStatRangeHigh
            // 
            nudStatRangeHigh.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudStatRangeHigh.ForeColor = System.Drawing.Color.Gainsboro;
            nudStatRangeHigh.Location = new System.Drawing.Point(152, 175);
            nudStatRangeHigh.Margin = new Padding(4, 3, 4, 3);
            nudStatRangeHigh.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudStatRangeHigh.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            nudStatRangeHigh.Name = "nudStatRangeHigh";
            nudStatRangeHigh.Size = new Size(54, 23);
            nudStatRangeHigh.TabIndex = 53;
            nudStatRangeHigh.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudStatRangeHigh.ValueChanged += nudStatRangeHigh_ValueChanged;
            // 
            // grpPaperdoll
            // 
            grpPaperdoll.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpPaperdoll.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpPaperdoll.Controls.Add(picMalePaperdoll);
            grpPaperdoll.Controls.Add(lblMalePaperdoll);
            grpPaperdoll.Controls.Add(cmbMalePaperdoll);
            grpPaperdoll.Controls.Add(picFemalePaperdoll);
            grpPaperdoll.Controls.Add(lblFemalePaperdoll);
            grpPaperdoll.Controls.Add(cmbFemalePaperdoll);
            grpPaperdoll.ForeColor = System.Drawing.Color.Gainsboro;
            grpPaperdoll.Location = new System.Drawing.Point(338, 21);
            grpPaperdoll.Margin = new Padding(2);
            grpPaperdoll.Name = "grpPaperdoll";
            grpPaperdoll.Padding = new Padding(2);
            grpPaperdoll.Size = new Size(316, 473);
            grpPaperdoll.TabIndex = 60;
            grpPaperdoll.TabStop = false;
            grpPaperdoll.Text = "Paperdoll Options";
            // 
            // picMalePaperdoll
            // 
            picMalePaperdoll.BackgroundImageLayout = ImageLayout.None;
            picMalePaperdoll.Location = new System.Drawing.Point(14, 68);
            picMalePaperdoll.Margin = new Padding(4, 3, 4, 3);
            picMalePaperdoll.Name = "picMalePaperdoll";
            picMalePaperdoll.Size = new Size(287, 177);
            picMalePaperdoll.TabIndex = 16;
            picMalePaperdoll.TabStop = false;
            // 
            // lblMalePaperdoll
            // 
            lblMalePaperdoll.AutoSize = true;
            lblMalePaperdoll.Location = new System.Drawing.Point(13, 38);
            lblMalePaperdoll.Margin = new Padding(4, 0, 4, 0);
            lblMalePaperdoll.Name = "lblMalePaperdoll";
            lblMalePaperdoll.Size = new Size(89, 15);
            lblMalePaperdoll.TabIndex = 21;
            lblMalePaperdoll.Text = "Male Paperdoll:";
            // 
            // cmbMalePaperdoll
            // 
            cmbMalePaperdoll.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbMalePaperdoll.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbMalePaperdoll.BorderStyle = ButtonBorderStyle.Solid;
            cmbMalePaperdoll.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbMalePaperdoll.DrawDropdownHoverOutline = false;
            cmbMalePaperdoll.DrawFocusRectangle = false;
            cmbMalePaperdoll.DrawMode = DrawMode.OwnerDrawFixed;
            cmbMalePaperdoll.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMalePaperdoll.FlatStyle = FlatStyle.Flat;
            cmbMalePaperdoll.ForeColor = System.Drawing.Color.Gainsboro;
            cmbMalePaperdoll.FormattingEnabled = true;
            cmbMalePaperdoll.Items.AddRange(new object[] { "None" });
            cmbMalePaperdoll.Location = new System.Drawing.Point(124, 31);
            cmbMalePaperdoll.Margin = new Padding(4, 3, 4, 3);
            cmbMalePaperdoll.Name = "cmbMalePaperdoll";
            cmbMalePaperdoll.Size = new Size(177, 24);
            cmbMalePaperdoll.TabIndex = 22;
            cmbMalePaperdoll.Text = "None";
            cmbMalePaperdoll.TextPadding = new Padding(2);
            cmbMalePaperdoll.SelectedIndexChanged += cmbPaperdoll_SelectedIndexChanged;
            // 
            // picFemalePaperdoll
            // 
            picFemalePaperdoll.BackgroundImageLayout = ImageLayout.None;
            picFemalePaperdoll.Location = new System.Drawing.Point(14, 283);
            picFemalePaperdoll.Margin = new Padding(4, 3, 4, 3);
            picFemalePaperdoll.Name = "picFemalePaperdoll";
            picFemalePaperdoll.Size = new Size(287, 177);
            picFemalePaperdoll.TabIndex = 34;
            picFemalePaperdoll.TabStop = false;
            // 
            // lblFemalePaperdoll
            // 
            lblFemalePaperdoll.AutoSize = true;
            lblFemalePaperdoll.Location = new System.Drawing.Point(10, 255);
            lblFemalePaperdoll.Margin = new Padding(4, 0, 4, 0);
            lblFemalePaperdoll.Name = "lblFemalePaperdoll";
            lblFemalePaperdoll.Size = new Size(101, 15);
            lblFemalePaperdoll.TabIndex = 35;
            lblFemalePaperdoll.Text = "Female Paperdoll:";
            // 
            // cmbFemalePaperdoll
            // 
            cmbFemalePaperdoll.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbFemalePaperdoll.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbFemalePaperdoll.BorderStyle = ButtonBorderStyle.Solid;
            cmbFemalePaperdoll.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbFemalePaperdoll.DrawDropdownHoverOutline = false;
            cmbFemalePaperdoll.DrawFocusRectangle = false;
            cmbFemalePaperdoll.DrawMode = DrawMode.OwnerDrawFixed;
            cmbFemalePaperdoll.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFemalePaperdoll.FlatStyle = FlatStyle.Flat;
            cmbFemalePaperdoll.ForeColor = System.Drawing.Color.Gainsboro;
            cmbFemalePaperdoll.FormattingEnabled = true;
            cmbFemalePaperdoll.Items.AddRange(new object[] { "None" });
            cmbFemalePaperdoll.Location = new System.Drawing.Point(124, 252);
            cmbFemalePaperdoll.Margin = new Padding(4, 3, 4, 3);
            cmbFemalePaperdoll.Name = "cmbFemalePaperdoll";
            cmbFemalePaperdoll.Size = new Size(177, 24);
            cmbFemalePaperdoll.TabIndex = 36;
            cmbFemalePaperdoll.Text = "None";
            cmbFemalePaperdoll.TextPadding = new Padding(2);
            cmbFemalePaperdoll.SelectedIndexChanged += cmbFemalePaperdoll_SelectedIndexChanged;
            // 
            // grpEffects
            // 
            grpEffects.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpEffects.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpEffects.Controls.Add(lstBonusEffects);
            grpEffects.Controls.Add(lblEffectPercent);
            grpEffects.Controls.Add(nudEffectPercent);
            grpEffects.ForeColor = System.Drawing.Color.Gainsboro;
            grpEffects.Location = new System.Drawing.Point(16, 335);
            grpEffects.Margin = new Padding(4, 3, 4, 3);
            grpEffects.Name = "grpEffects";
            grpEffects.Padding = new Padding(4, 3, 4, 3);
            grpEffects.Size = new Size(307, 226);
            grpEffects.TabIndex = 57;
            grpEffects.TabStop = false;
            grpEffects.Text = "Bonus Effects";
            // 
            // lstBonusEffects
            // 
            lstBonusEffects.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstBonusEffects.BorderStyle = BorderStyle.FixedSingle;
            lstBonusEffects.ForeColor = System.Drawing.Color.Gainsboro;
            lstBonusEffects.FormattingEnabled = true;
            lstBonusEffects.ItemHeight = 15;
            lstBonusEffects.Location = new System.Drawing.Point(15, 22);
            lstBonusEffects.Margin = new Padding(4, 3, 4, 3);
            lstBonusEffects.Name = "lstBonusEffects";
            lstBonusEffects.Size = new Size(284, 137);
            lstBonusEffects.TabIndex = 58;
            lstBonusEffects.SelectedIndexChanged += lstBonusEffects_SelectedIndexChanged;
            // 
            // lblEffectPercent
            // 
            lblEffectPercent.AutoSize = true;
            lblEffectPercent.Location = new System.Drawing.Point(12, 168);
            lblEffectPercent.Margin = new Padding(4, 0, 4, 0);
            lblEffectPercent.Name = "lblEffectPercent";
            lblEffectPercent.Size = new Size(108, 15);
            lblEffectPercent.TabIndex = 31;
            lblEffectPercent.Text = "Effect Amount (%):";
            // 
            // nudEffectPercent
            // 
            nudEffectPercent.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudEffectPercent.ForeColor = System.Drawing.Color.Gainsboro;
            nudEffectPercent.Location = new System.Drawing.Point(15, 190);
            nudEffectPercent.Margin = new Padding(4, 3, 4, 3);
            nudEffectPercent.Name = "nudEffectPercent";
            nudEffectPercent.Size = new Size(282, 23);
            nudEffectPercent.TabIndex = 55;
            nudEffectPercent.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudEffectPercent.ValueChanged += nudEffectPercent_ValueChanged;
            // 
            // grpRegen
            // 
            grpRegen.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpRegen.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpRegen.Controls.Add(nudMpRegen);
            grpRegen.Controls.Add(nudHPRegen);
            grpRegen.Controls.Add(lblHpRegen);
            grpRegen.Controls.Add(lblManaRegen);
            grpRegen.Controls.Add(lblRegenHint);
            grpRegen.ForeColor = System.Drawing.Color.Gainsboro;
            grpRegen.Location = new System.Drawing.Point(16, 196);
            grpRegen.Margin = new Padding(2);
            grpRegen.Name = "grpRegen";
            grpRegen.Padding = new Padding(2);
            grpRegen.Size = new Size(307, 133);
            grpRegen.TabIndex = 59;
            grpRegen.TabStop = false;
            grpRegen.Text = "Regen";
            // 
            // nudMpRegen
            // 
            nudMpRegen.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMpRegen.ForeColor = System.Drawing.Color.Gainsboro;
            nudMpRegen.Location = new System.Drawing.Point(155, 40);
            nudMpRegen.Margin = new Padding(4, 3, 4, 3);
            nudMpRegen.Name = "nudMpRegen";
            nudMpRegen.Size = new Size(112, 23);
            nudMpRegen.TabIndex = 31;
            nudMpRegen.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMpRegen.ValueChanged += nudMpRegen_ValueChanged;
            // 
            // nudHPRegen
            // 
            nudHPRegen.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudHPRegen.ForeColor = System.Drawing.Color.Gainsboro;
            nudHPRegen.Location = new System.Drawing.Point(13, 42);
            nudHPRegen.Margin = new Padding(4, 3, 4, 3);
            nudHPRegen.Name = "nudHPRegen";
            nudHPRegen.Size = new Size(112, 23);
            nudHPRegen.TabIndex = 30;
            nudHPRegen.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudHPRegen.ValueChanged += nudHPRegen_ValueChanged;
            // 
            // lblHpRegen
            // 
            lblHpRegen.AutoSize = true;
            lblHpRegen.Location = new System.Drawing.Point(6, 22);
            lblHpRegen.Margin = new Padding(2, 0, 2, 0);
            lblHpRegen.Name = "lblHpRegen";
            lblHpRegen.Size = new Size(47, 15);
            lblHpRegen.TabIndex = 26;
            lblHpRegen.Text = "HP: (%)";
            // 
            // lblManaRegen
            // 
            lblManaRegen.AutoSize = true;
            lblManaRegen.Location = new System.Drawing.Point(155, 22);
            lblManaRegen.Margin = new Padding(2, 0, 2, 0);
            lblManaRegen.Name = "lblManaRegen";
            lblManaRegen.Size = new Size(61, 15);
            lblManaRegen.TabIndex = 27;
            lblManaRegen.Text = "Mana: (%)";
            // 
            // lblRegenHint
            // 
            lblRegenHint.Location = new System.Drawing.Point(8, 81);
            lblRegenHint.Margin = new Padding(4, 0, 4, 0);
            lblRegenHint.Name = "lblRegenHint";
            lblRegenHint.Size = new Size(292, 36);
            lblRegenHint.TabIndex = 0;
            lblRegenHint.Text = "% of HP/Mana to restore per tick.\r\nTick timer saved in server config.json.";
            // 
            // grpVitalBonuses
            // 
            grpVitalBonuses.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpVitalBonuses.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpVitalBonuses.Controls.Add(lblPercentage2);
            grpVitalBonuses.Controls.Add(lblPercentage1);
            grpVitalBonuses.Controls.Add(nudMPPercentage);
            grpVitalBonuses.Controls.Add(nudHPPercentage);
            grpVitalBonuses.Controls.Add(lblPlus2);
            grpVitalBonuses.Controls.Add(lblPlus1);
            grpVitalBonuses.Controls.Add(nudManaBonus);
            grpVitalBonuses.Controls.Add(nudHealthBonus);
            grpVitalBonuses.Controls.Add(lblManaBonus);
            grpVitalBonuses.Controls.Add(lblHealthBonus);
            grpVitalBonuses.ForeColor = System.Drawing.Color.Gainsboro;
            grpVitalBonuses.Location = new System.Drawing.Point(16, 62);
            grpVitalBonuses.Margin = new Padding(4, 3, 4, 3);
            grpVitalBonuses.Name = "grpVitalBonuses";
            grpVitalBonuses.Padding = new Padding(4, 3, 4, 3);
            grpVitalBonuses.Size = new Size(307, 127);
            grpVitalBonuses.TabIndex = 58;
            grpVitalBonuses.TabStop = false;
            grpVitalBonuses.Text = "Vital Bonuses";
            // 
            // lblPercentage2
            // 
            lblPercentage2.AutoSize = true;
            lblPercentage2.Location = new System.Drawing.Point(251, 95);
            lblPercentage2.Margin = new Padding(2, 0, 2, 0);
            lblPercentage2.Name = "lblPercentage2";
            lblPercentage2.Size = new Size(17, 15);
            lblPercentage2.TabIndex = 70;
            lblPercentage2.Text = "%";
            // 
            // lblPercentage1
            // 
            lblPercentage1.AutoSize = true;
            lblPercentage1.Location = new System.Drawing.Point(251, 45);
            lblPercentage1.Margin = new Padding(2, 0, 2, 0);
            lblPercentage1.Name = "lblPercentage1";
            lblPercentage1.Size = new Size(17, 15);
            lblPercentage1.TabIndex = 69;
            lblPercentage1.Text = "%";
            // 
            // nudMPPercentage
            // 
            nudMPPercentage.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMPPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            nudMPPercentage.Location = new System.Drawing.Point(155, 92);
            nudMPPercentage.Margin = new Padding(4, 3, 4, 3);
            nudMPPercentage.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudMPPercentage.Minimum = new decimal(new int[] { -100, 0, 0, int.MinValue });
            nudMPPercentage.Name = "nudMPPercentage";
            nudMPPercentage.Size = new Size(90, 23);
            nudMPPercentage.TabIndex = 68;
            nudMPPercentage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMPPercentage.ValueChanged += nudMPPercentage_ValueChanged;
            // 
            // nudHPPercentage
            // 
            nudHPPercentage.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudHPPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            nudHPPercentage.Location = new System.Drawing.Point(155, 43);
            nudHPPercentage.Margin = new Padding(4, 3, 4, 3);
            nudHPPercentage.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudHPPercentage.Minimum = new decimal(new int[] { -100, 0, 0, int.MinValue });
            nudHPPercentage.Name = "nudHPPercentage";
            nudHPPercentage.Size = new Size(90, 23);
            nudHPPercentage.TabIndex = 67;
            nudHPPercentage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudHPPercentage.ValueChanged += nudHPPercentage_ValueChanged;
            // 
            // lblPlus2
            // 
            lblPlus2.AutoSize = true;
            lblPlus2.Location = new System.Drawing.Point(133, 95);
            lblPlus2.Margin = new Padding(2, 0, 2, 0);
            lblPlus2.Name = "lblPlus2";
            lblPlus2.Size = new Size(15, 15);
            lblPlus2.TabIndex = 66;
            lblPlus2.Text = "+";
            // 
            // lblPlus1
            // 
            lblPlus1.AutoSize = true;
            lblPlus1.Location = new System.Drawing.Point(133, 45);
            lblPlus1.Margin = new Padding(2, 0, 2, 0);
            lblPlus1.Name = "lblPlus1";
            lblPlus1.Size = new Size(15, 15);
            lblPlus1.TabIndex = 65;
            lblPlus1.Text = "+";
            // 
            // nudManaBonus
            // 
            nudManaBonus.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudManaBonus.ForeColor = System.Drawing.Color.Gainsboro;
            nudManaBonus.Location = new System.Drawing.Point(14, 92);
            nudManaBonus.Margin = new Padding(4, 3, 4, 3);
            nudManaBonus.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            nudManaBonus.Minimum = new decimal(new int[] { 999999999, 0, 0, int.MinValue });
            nudManaBonus.Name = "nudManaBonus";
            nudManaBonus.Size = new Size(111, 23);
            nudManaBonus.TabIndex = 49;
            nudManaBonus.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudManaBonus.ValueChanged += nudManaBonus_ValueChanged;
            // 
            // nudHealthBonus
            // 
            nudHealthBonus.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudHealthBonus.ForeColor = System.Drawing.Color.Gainsboro;
            nudHealthBonus.Location = new System.Drawing.Point(14, 43);
            nudHealthBonus.Margin = new Padding(4, 3, 4, 3);
            nudHealthBonus.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            nudHealthBonus.Minimum = new decimal(new int[] { 999999999, 0, 0, int.MinValue });
            nudHealthBonus.Name = "nudHealthBonus";
            nudHealthBonus.Size = new Size(111, 23);
            nudHealthBonus.TabIndex = 48;
            nudHealthBonus.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudHealthBonus.ValueChanged += nudHealthBonus_ValueChanged;
            // 
            // lblManaBonus
            // 
            lblManaBonus.AutoSize = true;
            lblManaBonus.Location = new System.Drawing.Point(10, 74);
            lblManaBonus.Margin = new Padding(2, 0, 2, 0);
            lblManaBonus.Name = "lblManaBonus";
            lblManaBonus.Size = new Size(40, 15);
            lblManaBonus.TabIndex = 44;
            lblManaBonus.Text = "Mana:";
            // 
            // lblHealthBonus
            // 
            lblHealthBonus.AutoSize = true;
            lblHealthBonus.Location = new System.Drawing.Point(9, 24);
            lblHealthBonus.Margin = new Padding(2, 0, 2, 0);
            lblHealthBonus.Name = "lblHealthBonus";
            lblHealthBonus.Size = new Size(45, 15);
            lblHealthBonus.TabIndex = 43;
            lblHealthBonus.Text = "Health:";
            // 
            // grpStatBonuses
            // 
            grpStatBonuses.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpStatBonuses.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpStatBonuses.Controls.Add(lblPercentage5);
            grpStatBonuses.Controls.Add(lblPercentage4);
            grpStatBonuses.Controls.Add(lblPercentage8);
            grpStatBonuses.Controls.Add(lblPercentage7);
            grpStatBonuses.Controls.Add(lblPercentage6);
            grpStatBonuses.Controls.Add(nudSpdPercentage);
            grpStatBonuses.Controls.Add(nudMRPercentage);
            grpStatBonuses.Controls.Add(nudDefPercentage);
            grpStatBonuses.Controls.Add(nudMagPercentage);
            grpStatBonuses.Controls.Add(nudStrPercentage);
            grpStatBonuses.Controls.Add(lblPlus5);
            grpStatBonuses.Controls.Add(lblPlus4);
            grpStatBonuses.Controls.Add(lblPlus8);
            grpStatBonuses.Controls.Add(lblPlus7);
            grpStatBonuses.Controls.Add(lblPlus6);
            grpStatBonuses.Controls.Add(nudSpd);
            grpStatBonuses.Controls.Add(nudMR);
            grpStatBonuses.Controls.Add(nudDef);
            grpStatBonuses.Controls.Add(nudMag);
            grpStatBonuses.Controls.Add(nudStr);
            grpStatBonuses.Controls.Add(lblSpd);
            grpStatBonuses.Controls.Add(lblMR);
            grpStatBonuses.Controls.Add(lblDef);
            grpStatBonuses.Controls.Add(lblMag);
            grpStatBonuses.Controls.Add(lblStr);
            grpStatBonuses.ForeColor = System.Drawing.Color.Gainsboro;
            grpStatBonuses.Location = new System.Drawing.Point(16, 576);
            grpStatBonuses.Margin = new Padding(4, 3, 4, 3);
            grpStatBonuses.Name = "grpStatBonuses";
            grpStatBonuses.Padding = new Padding(4, 3, 4, 3);
            grpStatBonuses.Size = new Size(307, 250);
            grpStatBonuses.TabIndex = 40;
            grpStatBonuses.TabStop = false;
            grpStatBonuses.Text = "Stat Bonuses";
            // 
            // lblPercentage5
            // 
            lblPercentage5.AutoSize = true;
            lblPercentage5.Location = new System.Drawing.Point(280, 220);
            lblPercentage5.Margin = new Padding(2, 0, 2, 0);
            lblPercentage5.Name = "lblPercentage5";
            lblPercentage5.Size = new Size(17, 15);
            lblPercentage5.TabIndex = 82;
            lblPercentage5.Text = "%";
            // 
            // lblPercentage4
            // 
            lblPercentage4.AutoSize = true;
            lblPercentage4.Location = new System.Drawing.Point(280, 173);
            lblPercentage4.Margin = new Padding(2, 0, 2, 0);
            lblPercentage4.Name = "lblPercentage4";
            lblPercentage4.Size = new Size(17, 15);
            lblPercentage4.TabIndex = 81;
            lblPercentage4.Text = "%";
            // 
            // lblPercentage8
            // 
            lblPercentage8.AutoSize = true;
            lblPercentage8.Location = new System.Drawing.Point(282, 123);
            lblPercentage8.Margin = new Padding(2, 0, 2, 0);
            lblPercentage8.Name = "lblPercentage8";
            lblPercentage8.Size = new Size(17, 15);
            lblPercentage8.TabIndex = 80;
            lblPercentage8.Text = "%";
            // 
            // lblPercentage7
            // 
            lblPercentage7.AutoSize = true;
            lblPercentage7.Location = new System.Drawing.Point(280, 78);
            lblPercentage7.Margin = new Padding(2, 0, 2, 0);
            lblPercentage7.Name = "lblPercentage7";
            lblPercentage7.Size = new Size(17, 15);
            lblPercentage7.TabIndex = 79;
            lblPercentage7.Text = "%";
            // 
            // lblPercentage6
            // 
            lblPercentage6.AutoSize = true;
            lblPercentage6.Location = new System.Drawing.Point(280, 35);
            lblPercentage6.Margin = new Padding(2, 0, 2, 0);
            lblPercentage6.Name = "lblPercentage6";
            lblPercentage6.Size = new Size(17, 15);
            lblPercentage6.TabIndex = 78;
            lblPercentage6.Text = "%";
            // 
            // nudSpdPercentage
            // 
            nudSpdPercentage.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudSpdPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            nudSpdPercentage.Location = new System.Drawing.Point(192, 218);
            nudSpdPercentage.Margin = new Padding(4, 3, 4, 3);
            nudSpdPercentage.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudSpdPercentage.Minimum = new decimal(new int[] { -100, 0, 0, int.MinValue });
            nudSpdPercentage.Name = "nudSpdPercentage";
            nudSpdPercentage.Size = new Size(76, 23);
            nudSpdPercentage.TabIndex = 77;
            nudSpdPercentage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudSpdPercentage.ValueChanged += nudSpdPercentage_ValueChanged;
            // 
            // nudMRPercentage
            // 
            nudMRPercentage.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMRPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            nudMRPercentage.Location = new System.Drawing.Point(192, 171);
            nudMRPercentage.Margin = new Padding(4, 3, 4, 3);
            nudMRPercentage.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudMRPercentage.Minimum = new decimal(new int[] { -100, 0, 0, int.MinValue });
            nudMRPercentage.Name = "nudMRPercentage";
            nudMRPercentage.Size = new Size(76, 23);
            nudMRPercentage.TabIndex = 76;
            nudMRPercentage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMRPercentage.ValueChanged += nudMRPercentage_ValueChanged;
            // 
            // nudDefPercentage
            // 
            nudDefPercentage.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDefPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            nudDefPercentage.Location = new System.Drawing.Point(195, 122);
            nudDefPercentage.Margin = new Padding(4, 3, 4, 3);
            nudDefPercentage.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudDefPercentage.Minimum = new decimal(new int[] { -100, 0, 0, int.MinValue });
            nudDefPercentage.Name = "nudDefPercentage";
            nudDefPercentage.Size = new Size(76, 23);
            nudDefPercentage.TabIndex = 75;
            nudDefPercentage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudDefPercentage.ValueChanged += nudDefPercentage_ValueChanged;
            // 
            // nudMagPercentage
            // 
            nudMagPercentage.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMagPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            nudMagPercentage.Location = new System.Drawing.Point(192, 77);
            nudMagPercentage.Margin = new Padding(4, 3, 4, 3);
            nudMagPercentage.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudMagPercentage.Minimum = new decimal(new int[] { -100, 0, 0, int.MinValue });
            nudMagPercentage.Name = "nudMagPercentage";
            nudMagPercentage.Size = new Size(76, 23);
            nudMagPercentage.TabIndex = 74;
            nudMagPercentage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMagPercentage.ValueChanged += nudMagPercentage_ValueChanged;
            // 
            // nudStrPercentage
            // 
            nudStrPercentage.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudStrPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            nudStrPercentage.Location = new System.Drawing.Point(192, 32);
            nudStrPercentage.Margin = new Padding(4, 3, 4, 3);
            nudStrPercentage.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudStrPercentage.Minimum = new decimal(new int[] { -100, 0, 0, int.MinValue });
            nudStrPercentage.Name = "nudStrPercentage";
            nudStrPercentage.Size = new Size(76, 23);
            nudStrPercentage.TabIndex = 73;
            nudStrPercentage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudStrPercentage.ValueChanged += nudStrPercentage_ValueChanged;
            // 
            // lblPlus5
            // 
            lblPlus5.AutoSize = true;
            lblPlus5.Location = new System.Drawing.Point(164, 220);
            lblPlus5.Margin = new Padding(2, 0, 2, 0);
            lblPlus5.Name = "lblPlus5";
            lblPlus5.Size = new Size(15, 15);
            lblPlus5.TabIndex = 72;
            lblPlus5.Text = "+";
            // 
            // lblPlus4
            // 
            lblPlus4.AutoSize = true;
            lblPlus4.Location = new System.Drawing.Point(164, 173);
            lblPlus4.Margin = new Padding(2, 0, 2, 0);
            lblPlus4.Name = "lblPlus4";
            lblPlus4.Size = new Size(15, 15);
            lblPlus4.TabIndex = 71;
            lblPlus4.Text = "+";
            // 
            // lblPlus8
            // 
            lblPlus8.AutoSize = true;
            lblPlus8.Location = new System.Drawing.Point(167, 123);
            lblPlus8.Margin = new Padding(2, 0, 2, 0);
            lblPlus8.Name = "lblPlus8";
            lblPlus8.Size = new Size(15, 15);
            lblPlus8.TabIndex = 70;
            lblPlus8.Text = "+";
            // 
            // lblPlus7
            // 
            lblPlus7.AutoSize = true;
            lblPlus7.Location = new System.Drawing.Point(164, 78);
            lblPlus7.Margin = new Padding(2, 0, 2, 0);
            lblPlus7.Name = "lblPlus7";
            lblPlus7.Size = new Size(15, 15);
            lblPlus7.TabIndex = 69;
            lblPlus7.Text = "+";
            // 
            // lblPlus6
            // 
            lblPlus6.AutoSize = true;
            lblPlus6.Location = new System.Drawing.Point(164, 35);
            lblPlus6.Margin = new Padding(2, 0, 2, 0);
            lblPlus6.Name = "lblPlus6";
            lblPlus6.Size = new Size(15, 15);
            lblPlus6.TabIndex = 68;
            lblPlus6.Text = "+";
            // 
            // nudSpd
            // 
            nudSpd.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudSpd.ForeColor = System.Drawing.Color.Gainsboro;
            nudSpd.Location = new System.Drawing.Point(18, 218);
            nudSpd.Margin = new Padding(4, 3, 4, 3);
            nudSpd.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudSpd.Name = "nudSpd";
            nudSpd.Size = new Size(140, 23);
            nudSpd.TabIndex = 52;
            nudSpd.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudSpd.ValueChanged += nudSpd_ValueChanged;
            // 
            // nudMR
            // 
            nudMR.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMR.ForeColor = System.Drawing.Color.Gainsboro;
            nudMR.Location = new System.Drawing.Point(15, 171);
            nudMR.Margin = new Padding(4, 3, 4, 3);
            nudMR.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudMR.Name = "nudMR";
            nudMR.Size = new Size(140, 23);
            nudMR.TabIndex = 51;
            nudMR.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMR.ValueChanged += nudMR_ValueChanged;
            // 
            // nudDef
            // 
            nudDef.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDef.ForeColor = System.Drawing.Color.Gainsboro;
            nudDef.Location = new System.Drawing.Point(15, 122);
            nudDef.Margin = new Padding(4, 3, 4, 3);
            nudDef.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudDef.Name = "nudDef";
            nudDef.Size = new Size(140, 23);
            nudDef.TabIndex = 50;
            nudDef.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudDef.ValueChanged += nudDef_ValueChanged;
            // 
            // nudMag
            // 
            nudMag.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMag.ForeColor = System.Drawing.Color.Gainsboro;
            nudMag.Location = new System.Drawing.Point(15, 77);
            nudMag.Margin = new Padding(4, 3, 4, 3);
            nudMag.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudMag.Name = "nudMag";
            nudMag.Size = new Size(140, 23);
            nudMag.TabIndex = 49;
            nudMag.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudMag.ValueChanged += nudMag_ValueChanged;
            // 
            // nudStr
            // 
            nudStr.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudStr.ForeColor = System.Drawing.Color.Gainsboro;
            nudStr.Location = new System.Drawing.Point(15, 32);
            nudStr.Margin = new Padding(4, 3, 4, 3);
            nudStr.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            nudStr.Name = "nudStr";
            nudStr.Size = new Size(140, 23);
            nudStr.TabIndex = 48;
            nudStr.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudStr.ValueChanged += nudStr_ValueChanged;
            // 
            // lblSpd
            // 
            lblSpd.AutoSize = true;
            lblSpd.Location = new System.Drawing.Point(13, 200);
            lblSpd.Margin = new Padding(2, 0, 2, 0);
            lblSpd.Name = "lblSpd";
            lblSpd.Size = new Size(42, 15);
            lblSpd.TabIndex = 47;
            lblSpd.Text = "Speed:";
            // 
            // lblMR
            // 
            lblMR.AutoSize = true;
            lblMR.Location = new System.Drawing.Point(12, 152);
            lblMR.Margin = new Padding(2, 0, 2, 0);
            lblMR.Name = "lblMR";
            lblMR.Size = new Size(76, 15);
            lblMR.TabIndex = 46;
            lblMR.Text = "Magic Resist:";
            // 
            // lblDef
            // 
            lblDef.AutoSize = true;
            lblDef.Location = new System.Drawing.Point(12, 104);
            lblDef.Margin = new Padding(2, 0, 2, 0);
            lblDef.Name = "lblDef";
            lblDef.Size = new Size(44, 15);
            lblDef.TabIndex = 45;
            lblDef.Text = "Armor:";
            // 
            // lblMag
            // 
            lblMag.AutoSize = true;
            lblMag.Location = new System.Drawing.Point(12, 62);
            lblMag.Margin = new Padding(2, 0, 2, 0);
            lblMag.Name = "lblMag";
            lblMag.Size = new Size(43, 15);
            lblMag.TabIndex = 44;
            lblMag.Text = "Magic:";
            // 
            // lblStr
            // 
            lblStr.AutoSize = true;
            lblStr.Location = new System.Drawing.Point(13, 14);
            lblStr.Margin = new Padding(2, 0, 2, 0);
            lblStr.Name = "lblStr";
            lblStr.Size = new Size(55, 15);
            lblStr.TabIndex = 43;
            lblStr.Text = "Strength:";
            // 
            // cmbEquipmentSlot
            // 
            cmbEquipmentSlot.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbEquipmentSlot.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbEquipmentSlot.BorderStyle = ButtonBorderStyle.Solid;
            cmbEquipmentSlot.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbEquipmentSlot.DrawDropdownHoverOutline = false;
            cmbEquipmentSlot.DrawFocusRectangle = false;
            cmbEquipmentSlot.DrawMode = DrawMode.OwnerDrawFixed;
            cmbEquipmentSlot.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEquipmentSlot.FlatStyle = FlatStyle.Flat;
            cmbEquipmentSlot.ForeColor = System.Drawing.Color.Gainsboro;
            cmbEquipmentSlot.FormattingEnabled = true;
            cmbEquipmentSlot.Location = new System.Drawing.Point(120, 28);
            cmbEquipmentSlot.Margin = new Padding(4, 3, 4, 3);
            cmbEquipmentSlot.Name = "cmbEquipmentSlot";
            cmbEquipmentSlot.Size = new Size(202, 24);
            cmbEquipmentSlot.TabIndex = 24;
            cmbEquipmentSlot.Text = null;
            cmbEquipmentSlot.TextPadding = new Padding(2);
            cmbEquipmentSlot.SelectedIndexChanged += cmbEquipmentSlot_SelectedIndexChanged;
            // 
            // lblEquipmentSlot
            // 
            lblEquipmentSlot.AutoSize = true;
            lblEquipmentSlot.Location = new System.Drawing.Point(15, 31);
            lblEquipmentSlot.Margin = new Padding(4, 0, 4, 0);
            lblEquipmentSlot.Name = "lblEquipmentSlot";
            lblEquipmentSlot.Size = new Size(91, 15);
            lblEquipmentSlot.TabIndex = 23;
            lblEquipmentSlot.Text = "Equipment Slot:";
            // 
            // grpWeaponProperties
            // 
            grpWeaponProperties.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpWeaponProperties.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpWeaponProperties.Controls.Add(cmbWeaponSprite);
            grpWeaponProperties.Controls.Add(lblSpriteAttack);
            grpWeaponProperties.Controls.Add(nudCritMultiplier);
            grpWeaponProperties.Controls.Add(lblCritMultiplier);
            grpWeaponProperties.Controls.Add(grpAttackSpeed);
            grpWeaponProperties.Controls.Add(nudScaling);
            grpWeaponProperties.Controls.Add(nudCritChance);
            grpWeaponProperties.Controls.Add(nudDamage);
            grpWeaponProperties.Controls.Add(cmbProjectile);
            grpWeaponProperties.Controls.Add(cmbScalingStat);
            grpWeaponProperties.Controls.Add(lblScalingStat);
            grpWeaponProperties.Controls.Add(lblScalingAmount);
            grpWeaponProperties.Controls.Add(cmbDamageType);
            grpWeaponProperties.Controls.Add(lblDamageType);
            grpWeaponProperties.Controls.Add(lblCritChance);
            grpWeaponProperties.Controls.Add(cmbAttackAnimation);
            grpWeaponProperties.Controls.Add(lblAttackAnimation);
            grpWeaponProperties.Controls.Add(chk2Hand);
            grpWeaponProperties.Controls.Add(lblToolType);
            grpWeaponProperties.Controls.Add(cmbToolType);
            grpWeaponProperties.Controls.Add(lblProjectile);
            grpWeaponProperties.Controls.Add(lblDamage);
            grpWeaponProperties.ForeColor = System.Drawing.Color.Gainsboro;
            grpWeaponProperties.Location = new System.Drawing.Point(660, 21);
            grpWeaponProperties.Margin = new Padding(4, 3, 4, 3);
            grpWeaponProperties.Name = "grpWeaponProperties";
            grpWeaponProperties.Padding = new Padding(4, 3, 4, 3);
            grpWeaponProperties.Size = new Size(275, 599);
            grpWeaponProperties.TabIndex = 39;
            grpWeaponProperties.TabStop = false;
            grpWeaponProperties.Text = "Weapon Properties";
            grpWeaponProperties.Visible = false;
            // 
            // cmbWeaponSprite
            // 
            cmbWeaponSprite.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbWeaponSprite.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbWeaponSprite.BorderStyle = ButtonBorderStyle.Solid;
            cmbWeaponSprite.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbWeaponSprite.DrawDropdownHoverOutline = false;
            cmbWeaponSprite.DrawFocusRectangle = false;
            cmbWeaponSprite.DrawMode = DrawMode.OwnerDrawFixed;
            cmbWeaponSprite.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbWeaponSprite.FlatStyle = FlatStyle.Flat;
            cmbWeaponSprite.ForeColor = System.Drawing.Color.Gainsboro;
            cmbWeaponSprite.FormattingEnabled = true;
            cmbWeaponSprite.Location = new System.Drawing.Point(18, 360);
            cmbWeaponSprite.Margin = new Padding(4, 3, 4, 3);
            cmbWeaponSprite.Name = "cmbWeaponSprite";
            cmbWeaponSprite.Size = new Size(247, 24);
            cmbWeaponSprite.TabIndex = 60;
            cmbWeaponSprite.Text = null;
            cmbWeaponSprite.TextPadding = new Padding(2);
            cmbWeaponSprite.SelectedIndexChanged += cmbWeaponSprite_SelectedIndexChanged;
            // 
            // lblSpriteAttack
            // 
            lblSpriteAttack.AutoSize = true;
            lblSpriteAttack.Location = new System.Drawing.Point(14, 342);
            lblSpriteAttack.Margin = new Padding(4, 0, 4, 0);
            lblSpriteAttack.Name = "lblSpriteAttack";
            lblSpriteAttack.Size = new Size(136, 15);
            lblSpriteAttack.TabIndex = 59;
            lblSpriteAttack.Text = "Sprite Attack Animation:";
            // 
            // nudCritMultiplier
            // 
            nudCritMultiplier.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudCritMultiplier.DecimalPlaces = 2;
            nudCritMultiplier.ForeColor = System.Drawing.Color.Gainsboro;
            nudCritMultiplier.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudCritMultiplier.Location = new System.Drawing.Point(18, 128);
            nudCritMultiplier.Margin = new Padding(4, 3, 4, 3);
            nudCritMultiplier.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudCritMultiplier.Name = "nudCritMultiplier";
            nudCritMultiplier.Size = new Size(247, 23);
            nudCritMultiplier.TabIndex = 58;
            nudCritMultiplier.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudCritMultiplier.ValueChanged += nudCritMultiplier_ValueChanged;
            // 
            // lblCritMultiplier
            // 
            lblCritMultiplier.AutoSize = true;
            lblCritMultiplier.Location = new System.Drawing.Point(14, 112);
            lblCritMultiplier.Margin = new Padding(4, 0, 4, 0);
            lblCritMultiplier.Name = "lblCritMultiplier";
            lblCritMultiplier.Size = new Size(156, 15);
            lblCritMultiplier.TabIndex = 57;
            lblCritMultiplier.Text = "Crit Multiplier (Default 1.5x):";
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
            grpAttackSpeed.Location = new System.Drawing.Point(18, 488);
            grpAttackSpeed.Margin = new Padding(4, 3, 4, 3);
            grpAttackSpeed.Name = "grpAttackSpeed";
            grpAttackSpeed.Padding = new Padding(4, 3, 4, 3);
            grpAttackSpeed.Size = new Size(247, 99);
            grpAttackSpeed.TabIndex = 56;
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
            nudAttackSpeedValue.Size = new Size(168, 23);
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
            cmbAttackSpeedModifier.Size = new Size(167, 24);
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
            // nudScaling
            // 
            nudScaling.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudScaling.ForeColor = System.Drawing.Color.Gainsboro;
            nudScaling.Location = new System.Drawing.Point(18, 270);
            nudScaling.Margin = new Padding(4, 3, 4, 3);
            nudScaling.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudScaling.Name = "nudScaling";
            nudScaling.Size = new Size(247, 23);
            nudScaling.TabIndex = 55;
            nudScaling.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudScaling.ValueChanged += nudScaling_ValueChanged;
            // 
            // nudCritChance
            // 
            nudCritChance.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudCritChance.ForeColor = System.Drawing.Color.Gainsboro;
            nudCritChance.Location = new System.Drawing.Point(18, 84);
            nudCritChance.Margin = new Padding(4, 3, 4, 3);
            nudCritChance.Name = "nudCritChance";
            nudCritChance.Size = new Size(247, 23);
            nudCritChance.TabIndex = 54;
            nudCritChance.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudCritChance.ValueChanged += nudCritChance_ValueChanged;
            // 
            // nudDamage
            // 
            nudDamage.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudDamage.ForeColor = System.Drawing.Color.Gainsboro;
            nudDamage.Location = new System.Drawing.Point(18, 42);
            nudDamage.Margin = new Padding(4, 3, 4, 3);
            nudDamage.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            nudDamage.Name = "nudDamage";
            nudDamage.Size = new Size(247, 23);
            nudDamage.TabIndex = 49;
            nudDamage.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudDamage.ValueChanged += nudDamage_ValueChanged;
            // 
            // cmbProjectile
            // 
            cmbProjectile.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbProjectile.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbProjectile.BorderStyle = ButtonBorderStyle.Solid;
            cmbProjectile.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbProjectile.DrawDropdownHoverOutline = false;
            cmbProjectile.DrawFocusRectangle = false;
            cmbProjectile.DrawMode = DrawMode.OwnerDrawFixed;
            cmbProjectile.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProjectile.FlatStyle = FlatStyle.Flat;
            cmbProjectile.ForeColor = System.Drawing.Color.Gainsboro;
            cmbProjectile.FormattingEnabled = true;
            cmbProjectile.Location = new System.Drawing.Point(18, 312);
            cmbProjectile.Margin = new Padding(4, 3, 4, 3);
            cmbProjectile.Name = "cmbProjectile";
            cmbProjectile.Size = new Size(247, 24);
            cmbProjectile.TabIndex = 47;
            cmbProjectile.Text = null;
            cmbProjectile.TextPadding = new Padding(2);
            cmbProjectile.SelectedIndexChanged += cmbProjectile_SelectedIndexChanged;
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
            cmbScalingStat.Location = new System.Drawing.Point(18, 223);
            cmbScalingStat.Margin = new Padding(4, 3, 4, 3);
            cmbScalingStat.Name = "cmbScalingStat";
            cmbScalingStat.Size = new Size(248, 24);
            cmbScalingStat.TabIndex = 46;
            cmbScalingStat.Text = null;
            cmbScalingStat.TextPadding = new Padding(2);
            cmbScalingStat.SelectedIndexChanged += cmbScalingStat_SelectedIndexChanged;
            // 
            // lblScalingStat
            // 
            lblScalingStat.AutoSize = true;
            lblScalingStat.Location = new System.Drawing.Point(15, 203);
            lblScalingStat.Margin = new Padding(4, 0, 4, 0);
            lblScalingStat.Name = "lblScalingStat";
            lblScalingStat.Size = new Size(71, 15);
            lblScalingStat.TabIndex = 45;
            lblScalingStat.Text = "Scaling Stat:";
            // 
            // lblScalingAmount
            // 
            lblScalingAmount.AutoSize = true;
            lblScalingAmount.Location = new System.Drawing.Point(14, 253);
            lblScalingAmount.Margin = new Padding(4, 0, 4, 0);
            lblScalingAmount.Name = "lblScalingAmount";
            lblScalingAmount.Size = new Size(95, 15);
            lblScalingAmount.TabIndex = 44;
            lblScalingAmount.Text = "Scaling Amount:";
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
            cmbDamageType.Location = new System.Drawing.Point(18, 175);
            cmbDamageType.Margin = new Padding(4, 3, 4, 3);
            cmbDamageType.Name = "cmbDamageType";
            cmbDamageType.Size = new Size(248, 24);
            cmbDamageType.TabIndex = 42;
            cmbDamageType.Text = "Physical";
            cmbDamageType.TextPadding = new Padding(2);
            cmbDamageType.SelectedIndexChanged += cmbDamageType_SelectedIndexChanged;
            // 
            // lblDamageType
            // 
            lblDamageType.AutoSize = true;
            lblDamageType.Location = new System.Drawing.Point(15, 156);
            lblDamageType.Margin = new Padding(4, 0, 4, 0);
            lblDamageType.Name = "lblDamageType";
            lblDamageType.Size = new Size(81, 15);
            lblDamageType.TabIndex = 41;
            lblDamageType.Text = "Damage Type:";
            // 
            // lblCritChance
            // 
            lblCritChance.AutoSize = true;
            lblCritChance.Location = new System.Drawing.Point(14, 68);
            lblCritChance.Margin = new Padding(4, 0, 4, 0);
            lblCritChance.Name = "lblCritChance";
            lblCritChance.Size = new Size(93, 15);
            lblCritChance.TabIndex = 40;
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
            cmbAttackAnimation.Location = new System.Drawing.Point(18, 404);
            cmbAttackAnimation.Margin = new Padding(4, 3, 4, 3);
            cmbAttackAnimation.Name = "cmbAttackAnimation";
            cmbAttackAnimation.Size = new Size(247, 24);
            cmbAttackAnimation.TabIndex = 38;
            cmbAttackAnimation.Text = null;
            cmbAttackAnimation.TextPadding = new Padding(2);
            cmbAttackAnimation.SelectedIndexChanged += cmbAttackAnimation_SelectedIndexChanged;
            // 
            // lblAttackAnimation
            // 
            lblAttackAnimation.AutoSize = true;
            lblAttackAnimation.Location = new System.Drawing.Point(14, 387);
            lblAttackAnimation.Margin = new Padding(4, 0, 4, 0);
            lblAttackAnimation.Name = "lblAttackAnimation";
            lblAttackAnimation.Size = new Size(132, 15);
            lblAttackAnimation.TabIndex = 37;
            lblAttackAnimation.Text = "Extra Attack Animation:";
            // 
            // chk2Hand
            // 
            chk2Hand.AutoSize = true;
            chk2Hand.Location = new System.Drawing.Point(156, 16);
            chk2Hand.Margin = new Padding(4, 3, 4, 3);
            chk2Hand.Name = "chk2Hand";
            chk2Hand.Size = new Size(64, 19);
            chk2Hand.TabIndex = 25;
            chk2Hand.Text = "2 Hand";
            chk2Hand.CheckedChanged += chk2Hand_CheckedChanged;
            // 
            // lblToolType
            // 
            lblToolType.AutoSize = true;
            lblToolType.Location = new System.Drawing.Point(15, 435);
            lblToolType.Margin = new Padding(4, 0, 4, 0);
            lblToolType.Name = "lblToolType";
            lblToolType.Size = new Size(59, 15);
            lblToolType.TabIndex = 26;
            lblToolType.Text = "Tool Type:";
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
            cmbToolType.Location = new System.Drawing.Point(18, 451);
            cmbToolType.Margin = new Padding(4, 3, 4, 3);
            cmbToolType.Name = "cmbToolType";
            cmbToolType.Size = new Size(248, 24);
            cmbToolType.TabIndex = 27;
            cmbToolType.Text = null;
            cmbToolType.TextPadding = new Padding(2);
            cmbToolType.SelectedIndexChanged += cmbToolType_SelectedIndexChanged;
            // 
            // lblProjectile
            // 
            lblProjectile.AutoSize = true;
            lblProjectile.Location = new System.Drawing.Point(14, 293);
            lblProjectile.Margin = new Padding(4, 0, 4, 0);
            lblProjectile.Name = "lblProjectile";
            lblProjectile.Size = new Size(59, 15);
            lblProjectile.TabIndex = 33;
            lblProjectile.Text = "Projectile:";
            // 
            // lblDamage
            // 
            lblDamage.AutoSize = true;
            lblDamage.Location = new System.Drawing.Point(14, 22);
            lblDamage.Margin = new Padding(4, 0, 4, 0);
            lblDamage.Name = "lblDamage";
            lblDamage.Size = new Size(81, 15);
            lblDamage.TabIndex = 11;
            lblDamage.Text = "Base Damage:";
            // 
            // grpShieldProperties
            // 
            grpShieldProperties.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpShieldProperties.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpShieldProperties.Controls.Add(nudBlockDmgAbs);
            grpShieldProperties.Controls.Add(lblBlockDmgAbs);
            grpShieldProperties.Controls.Add(nudBlockAmount);
            grpShieldProperties.Controls.Add(lblBlockAmount);
            grpShieldProperties.Controls.Add(nudBlockChance);
            grpShieldProperties.Controls.Add(lblBlockChance);
            grpShieldProperties.ForeColor = System.Drawing.Color.Gainsboro;
            grpShieldProperties.Location = new System.Drawing.Point(660, 21);
            grpShieldProperties.Margin = new Padding(4, 3, 4, 3);
            grpShieldProperties.Name = "grpShieldProperties";
            grpShieldProperties.Padding = new Padding(4, 3, 4, 3);
            grpShieldProperties.Size = new Size(275, 168);
            grpShieldProperties.TabIndex = 45;
            grpShieldProperties.TabStop = false;
            grpShieldProperties.Text = "Shield Properties";
            // 
            // nudBlockDmgAbs
            // 
            nudBlockDmgAbs.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudBlockDmgAbs.ForeColor = System.Drawing.Color.Gainsboro;
            nudBlockDmgAbs.Location = new System.Drawing.Point(18, 132);
            nudBlockDmgAbs.Margin = new Padding(4, 3, 4, 3);
            nudBlockDmgAbs.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            nudBlockDmgAbs.Name = "nudBlockDmgAbs";
            nudBlockDmgAbs.Size = new Size(247, 23);
            nudBlockDmgAbs.TabIndex = 64;
            nudBlockDmgAbs.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudBlockDmgAbs.ValueChanged += nudBlockDmgAbs_ValueChanged;
            // 
            // lblBlockDmgAbs
            // 
            lblBlockDmgAbs.AutoSize = true;
            lblBlockDmgAbs.Location = new System.Drawing.Point(14, 115);
            lblBlockDmgAbs.Margin = new Padding(4, 0, 4, 0);
            lblBlockDmgAbs.Name = "lblBlockDmgAbs";
            lblBlockDmgAbs.Size = new Size(169, 15);
            lblBlockDmgAbs.TabIndex = 63;
            lblBlockDmgAbs.Text = "Block Damage Absorption (%):";
            // 
            // nudBlockAmount
            // 
            nudBlockAmount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudBlockAmount.ForeColor = System.Drawing.Color.Gainsboro;
            nudBlockAmount.Location = new System.Drawing.Point(18, 87);
            nudBlockAmount.Margin = new Padding(4, 3, 4, 3);
            nudBlockAmount.Name = "nudBlockAmount";
            nudBlockAmount.Size = new Size(247, 23);
            nudBlockAmount.TabIndex = 62;
            nudBlockAmount.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudBlockAmount.ValueChanged += nudBlockAmount_ValueChanged;
            // 
            // lblBlockAmount
            // 
            lblBlockAmount.AutoSize = true;
            lblBlockAmount.Location = new System.Drawing.Point(14, 70);
            lblBlockAmount.Margin = new Padding(4, 0, 4, 0);
            lblBlockAmount.Name = "lblBlockAmount";
            lblBlockAmount.Size = new Size(107, 15);
            lblBlockAmount.TabIndex = 61;
            lblBlockAmount.Text = "Block Amount (%):";
            // 
            // nudBlockChance
            // 
            nudBlockChance.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudBlockChance.ForeColor = System.Drawing.Color.Gainsboro;
            nudBlockChance.Location = new System.Drawing.Point(18, 42);
            nudBlockChance.Margin = new Padding(4, 3, 4, 3);
            nudBlockChance.Name = "nudBlockChance";
            nudBlockChance.Size = new Size(247, 23);
            nudBlockChance.TabIndex = 60;
            nudBlockChance.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudBlockChance.ValueChanged += nudBlockChance_ValueChanged;
            // 
            // lblBlockChance
            // 
            lblBlockChance.AutoSize = true;
            lblBlockChance.Location = new System.Drawing.Point(14, 22);
            lblBlockChance.Margin = new Padding(4, 0, 4, 0);
            lblBlockChance.Name = "lblBlockChance";
            lblBlockChance.Size = new Size(103, 15);
            lblBlockChance.TabIndex = 59;
            lblBlockChance.Text = "Block Chance (%):";
            // 
            // pnlContainer
            // 
            pnlContainer.AutoScroll = true;
            pnlContainer.Controls.Add(grpGeneral);
            pnlContainer.Controls.Add(grpEquipment);
            pnlContainer.Location = new System.Drawing.Point(250, 39);
            pnlContainer.Margin = new Padding(4, 3, 4, 3);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(980, 635);
            pnlContainer.TabIndex = 43;
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
            toolStrip.Size = new Size(1241, 29);
            toolStrip.TabIndex = 44;
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
            // FrmItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            ClientSize = new Size(1241, 728);
            ControlBox = false;
            Controls.Add(toolStrip);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(grpItems);
            Controls.Add(pnlContainer);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FrmItem";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Item Editor";
            FormClosed += FrmItem_FormClosed;
            Load += frmItem_Load;
            KeyDown += form_KeyDown;
            grpItems.ResumeLayout(false);
            grpItems.PerformLayout();
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            grpEvents.ResumeLayout(false);
            grpEvents.PerformLayout();
            grpStack.ResumeLayout(false);
            grpStack.PerformLayout();
            ((ISupportInitialize)nudInvStackLimit).EndInit();
            ((ISupportInitialize)nudBankStackLimit).EndInit();
            grpCooldown.ResumeLayout(false);
            grpCooldown.PerformLayout();
            ((ISupportInitialize)nudCooldown).EndInit();
            ((ISupportInitialize)nudItemDespawnTime).EndInit();
            grpRequirements.ResumeLayout(false);
            grpRequirements.PerformLayout();
            ((ISupportInitialize)nudDeathDropChance).EndInit();
            ((ISupportInitialize)nudRgbaA).EndInit();
            ((ISupportInitialize)nudRgbaB).EndInit();
            ((ISupportInitialize)nudRgbaG).EndInit();
            ((ISupportInitialize)nudRgbaR).EndInit();
            ((ISupportInitialize)nudPrice).EndInit();
            ((ISupportInitialize)picItem).EndInit();
            grpConsumable.ResumeLayout(false);
            grpConsumable.PerformLayout();
            ((ISupportInitialize)nudIntervalPercentage).EndInit();
            ((ISupportInitialize)nudInterval).EndInit();
            grpEvent.ResumeLayout(false);
            grpEvent.PerformLayout();
            grpBags.ResumeLayout(false);
            grpBags.PerformLayout();
            ((ISupportInitialize)nudBag).EndInit();
            grpSpell.ResumeLayout(false);
            grpSpell.PerformLayout();
            grpEquipment.ResumeLayout(false);
            grpEquipment.PerformLayout();
            grpStatRanges.ResumeLayout(false);
            grpStatRanges.PerformLayout();
            ((ISupportInitialize)nudStatRangeLow).EndInit();
            ((ISupportInitialize)nudStatRangeHigh).EndInit();
            grpPaperdoll.ResumeLayout(false);
            grpPaperdoll.PerformLayout();
            ((ISupportInitialize)picMalePaperdoll).EndInit();
            ((ISupportInitialize)picFemalePaperdoll).EndInit();
            grpEffects.ResumeLayout(false);
            grpEffects.PerformLayout();
            ((ISupportInitialize)nudEffectPercent).EndInit();
            grpRegen.ResumeLayout(false);
            grpRegen.PerformLayout();
            ((ISupportInitialize)nudMpRegen).EndInit();
            ((ISupportInitialize)nudHPRegen).EndInit();
            grpVitalBonuses.ResumeLayout(false);
            grpVitalBonuses.PerformLayout();
            ((ISupportInitialize)nudMPPercentage).EndInit();
            ((ISupportInitialize)nudHPPercentage).EndInit();
            ((ISupportInitialize)nudManaBonus).EndInit();
            ((ISupportInitialize)nudHealthBonus).EndInit();
            grpStatBonuses.ResumeLayout(false);
            grpStatBonuses.PerformLayout();
            ((ISupportInitialize)nudSpdPercentage).EndInit();
            ((ISupportInitialize)nudMRPercentage).EndInit();
            ((ISupportInitialize)nudDefPercentage).EndInit();
            ((ISupportInitialize)nudMagPercentage).EndInit();
            ((ISupportInitialize)nudStrPercentage).EndInit();
            ((ISupportInitialize)nudSpd).EndInit();
            ((ISupportInitialize)nudMR).EndInit();
            ((ISupportInitialize)nudDef).EndInit();
            ((ISupportInitialize)nudMag).EndInit();
            ((ISupportInitialize)nudStr).EndInit();
            grpWeaponProperties.ResumeLayout(false);
            grpWeaponProperties.PerformLayout();
            ((ISupportInitialize)nudCritMultiplier).EndInit();
            grpAttackSpeed.ResumeLayout(false);
            grpAttackSpeed.PerformLayout();
            ((ISupportInitialize)nudAttackSpeedValue).EndInit();
            ((ISupportInitialize)nudScaling).EndInit();
            ((ISupportInitialize)nudCritChance).EndInit();
            ((ISupportInitialize)nudDamage).EndInit();
            grpShieldProperties.ResumeLayout(false);
            grpShieldProperties.PerformLayout();
            ((ISupportInitialize)nudBlockDmgAbs).EndInit();
            ((ISupportInitialize)nudBlockAmount).EndInit();
            ((ISupportInitialize)nudBlockChance).EndInit();
            pnlContainer.ResumeLayout(false);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Label lblName;
        private Label lblType;
        private Label lblAnim;
        private Label lblPrice;
        private PictureBox picItem;
        private Label lblDamage;
        private PictureBox picMalePaperdoll;
        private Label lblInterval;
        private Label lblVital;
        private Label lblSpell;
        private Label lblPic;
        private Label lblMalePaperdoll;
        private Label lblDesc;
        private Label lblEquipmentSlot;
        private Label lblEffectPercent;
        private Label lblToolType;
        private Label lblProjectile;
        private Panel pnlContainer;
        private Label lblFemalePaperdoll;
        private PictureBox picFemalePaperdoll;
        private ToolStripButton toolStripItemNew;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripItemDelete;
        private ToolStripSeparator toolStripSeparator2;
        public ToolStripButton toolStripItemCopy;
        public ToolStripButton toolStripItemPaste;
        private ToolStripSeparator toolStripSeparator3;
        public ToolStripButton toolStripItemUndo;
        private Label lblAttackAnimation;
        private DarkGroupBox grpStatBonuses;
        private DarkGroupBox grpWeaponProperties;
        private Label lblScalingAmount;
        private Label lblDamageType;
        private Label lblCritChance;
        private Label lblScalingStat;
        private DarkGroupBox grpItems;
        private DarkButton btnSave;
        private DarkGroupBox grpGeneral;
        private DarkTextBox txtName;
        private DarkComboBox cmbType;
        private DarkGroupBox grpEquipment;
        private DarkGroupBox grpConsumable;
        private DarkComboBox cmbConsume;
        private DarkGroupBox grpSpell;
        private DarkButton btnCancel;
        private DarkComboBox cmbPic;
        private DarkComboBox cmbMalePaperdoll;
        private DarkTextBox txtDesc;
        private DarkCheckBox chk2Hand;
        private DarkComboBox cmbEquipmentSlot;
        private DarkComboBox cmbToolType;
        private DarkGroupBox grpEvent;
        private DarkComboBox cmbFemalePaperdoll;
        private DarkComboBox cmbAttackAnimation;
        private DarkComboBox cmbDamageType;
        private DarkComboBox cmbScalingStat;
        private DarkComboBox cmbProjectile;
        private DarkToolStrip toolStrip;
        private DarkButton btnEditRequirements;
        private DarkComboBox cmbAnimation;
        private DarkComboBox cmbTeachSpell;
        private DarkComboBox cmbEvent;
        private DarkGroupBox grpBags;
        private Label lblBag;
        private DarkNumericUpDown nudPrice;
        private DarkNumericUpDown nudBag;
        private DarkNumericUpDown nudInterval;
        private DarkNumericUpDown nudEffectPercent;
        private DarkNumericUpDown nudStatRangeHigh;
        private DarkNumericUpDown nudSpd;
        private DarkNumericUpDown nudMR;
        private DarkNumericUpDown nudDef;
        private DarkNumericUpDown nudMag;
        private DarkNumericUpDown nudStr;
        private Label lblSpd;
        private Label lblMR;
        private Label lblDef;
        private Label lblMag;
        private Label lblStr;
        private DarkNumericUpDown nudScaling;
        private DarkNumericUpDown nudCritChance;
        private DarkNumericUpDown nudDamage;
        private DarkCheckBox chkStackable;
        private DarkCheckBox chkCanDrop;
        private DarkGroupBox grpVitalBonuses;
        private DarkNumericUpDown nudManaBonus;
        private DarkNumericUpDown nudHealthBonus;
        private Label lblManaBonus;
        private Label lblHealthBonus;
        private DarkComboBox cmbEquipmentAnimation;
        private Label lblEquipmentAnimation;
        private DarkGroupBox grpAttackSpeed;
        private DarkNumericUpDown nudAttackSpeedValue;
        private Label lblAttackSpeedValue;
        private DarkComboBox cmbAttackSpeedModifier;
        private Label lblAttackSpeedModifier;
        private DarkNumericUpDown nudCritMultiplier;
        private Label lblCritMultiplier;
        private DarkNumericUpDown nudCooldown;
        private Label lblCooldown;
        private DarkCheckBox chkSingleUseSpell;
        private DarkCheckBox chkSingleUseEvent;
        private DarkCheckBox chkQuickCast;
        private DarkComboBox cmbRarity;
        private Label lblRarity;
        private DarkButton btnClearSearch;
        private DarkTextBox txtSearch;
        private ToolStripButton btnAlphabetical;
        private ToolStripSeparator toolStripSeparator4;
        private DarkButton btnAddFolder;
        private Label lblFolder;
        private DarkComboBox cmbFolder;
        private Label lblPercentage2;
        private Label lblPercentage1;
        private DarkNumericUpDown nudMPPercentage;
        private DarkNumericUpDown nudHPPercentage;
        private Label lblPlus2;
        private Label lblPlus1;
        private Label lblPercentage3;
        private DarkNumericUpDown nudIntervalPercentage;
        private Label lblPlus3;
        private Label lblPercentage5;
        private Label lblPercentage4;
        private Label lblPercentage8;
        private Label lblPercentage7;
        private Label lblPercentage6;
        private DarkNumericUpDown nudSpdPercentage;
        private DarkNumericUpDown nudMRPercentage;
        private DarkNumericUpDown nudDefPercentage;
        private DarkNumericUpDown nudMagPercentage;
        private DarkNumericUpDown nudStrPercentage;
        private Label lblPlus5;
        private Label lblPlus4;
        private Label lblPlus8;
        private Label lblPlus7;
        private Label lblPlus6;
        private DarkGroupBox grpRegen;
        private DarkNumericUpDown nudMpRegen;
        private DarkNumericUpDown nudHPRegen;
        private Label lblHpRegen;
        private Label lblManaRegen;
        private Label lblRegenHint;
        private DarkComboBox cmbCooldownGroup;
        private Label lblCooldownGroup;
        private DarkButton btnAddCooldownGroup;
        private DarkCheckBox chkIgnoreGlobalCooldown;
        private Label lblAlpha;
        private Label lblBlue;
        private Label lblGreen;
        private Label lblRed;
        private DarkNumericUpDown nudRgbaA;
        private DarkNumericUpDown nudRgbaB;
        private DarkNumericUpDown nudRgbaG;
        private DarkNumericUpDown nudRgbaR;
        private DarkCheckBox chkIgnoreCdr;
        private Controls.GameObjectList lstGameObjects;
        private DarkCheckBox chkCanSell;
        private DarkCheckBox chkCanTrade;
        private DarkCheckBox chkCanBag;
        private DarkCheckBox chkCanBank;
        private DarkNumericUpDown nudDeathDropChance;
        private Label lblDeathDropChance;
        private DarkNumericUpDown nudBankStackLimit;
        private DarkNumericUpDown nudInvStackLimit;
        private Label lblBankStackLimit;
        private Label lblInvStackLimit;
        private DarkCheckBox chkCanGuildBank;
        private DarkGroupBox grpRequirements;
        private Label lblCannotUse;
        private DarkTextBox txtCannotUse;
        private DarkGroupBox grpShieldProperties;
        private DarkNumericUpDown nudBlockDmgAbs;
        private Label lblBlockDmgAbs;
        private DarkNumericUpDown nudBlockAmount;
        private Label lblBlockAmount;
        private DarkNumericUpDown nudBlockChance;
        private Label lblBlockChance;
        private Label lblSpriteAttack;
        private DarkComboBox cmbWeaponSprite;
        private DarkNumericUpDown nudItemDespawnTime;
        private Label lblDespawnTime;
        private ToolTip tooltips;
        private DarkGroupBox grpEffects;
        private ListBox lstBonusEffects;
        private DarkGroupBox grpCooldown;
        private DarkGroupBox grpStack;
        private DarkGroupBox grpPaperdoll;
        private DarkGroupBox grpStatRanges;
        private Label lblStatRangeTo;
        private DarkNumericUpDown nudStatRangeLow;
        private ListBox lstStatRanges;
        private Label lblStatRangeFrom;
        private DarkGroupBox grpEvents;
        private DarkComboBox cmbEventTriggers;
        private Label lblEventForTrigger;
        private ListBox lstEventTriggers;
    }
}
