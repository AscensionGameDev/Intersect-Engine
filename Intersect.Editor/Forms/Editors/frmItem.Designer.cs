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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmItem));
            this.grpItems = new DarkUI.Controls.DarkGroupBox();
            this.btnClearSearch = new DarkUI.Controls.DarkButton();
            this.txtSearch = new DarkUI.Controls.DarkTextBox();
            this.lstGameObjects = new Intersect.Editor.Forms.Controls.GameObjectList();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpGeneral = new DarkUI.Controls.DarkGroupBox();
            this.chkCanGuildBank = new DarkUI.Controls.DarkCheckBox();
            this.nudBankStackLimit = new DarkUI.Controls.DarkNumericUpDown();
            this.nudInvStackLimit = new DarkUI.Controls.DarkNumericUpDown();
            this.lblBankStackLimit = new System.Windows.Forms.Label();
            this.lblInvStackLimit = new System.Windows.Forms.Label();
            this.nudDeathDropChance = new DarkUI.Controls.DarkNumericUpDown();
            this.lblDeathDropChance = new System.Windows.Forms.Label();
            this.chkCanSell = new DarkUI.Controls.DarkCheckBox();
            this.chkCanTrade = new DarkUI.Controls.DarkCheckBox();
            this.chkCanBag = new DarkUI.Controls.DarkCheckBox();
            this.chkCanBank = new DarkUI.Controls.DarkCheckBox();
            this.chkIgnoreCdr = new DarkUI.Controls.DarkCheckBox();
            this.chkIgnoreGlobalCooldown = new DarkUI.Controls.DarkCheckBox();
            this.btnAddCooldownGroup = new DarkUI.Controls.DarkButton();
            this.cmbCooldownGroup = new DarkUI.Controls.DarkComboBox();
            this.lblCooldownGroup = new System.Windows.Forms.Label();
            this.lblAlpha = new System.Windows.Forms.Label();
            this.lblBlue = new System.Windows.Forms.Label();
            this.lblGreen = new System.Windows.Forms.Label();
            this.lblRed = new System.Windows.Forms.Label();
            this.nudRgbaA = new DarkUI.Controls.DarkNumericUpDown();
            this.nudRgbaB = new DarkUI.Controls.DarkNumericUpDown();
            this.nudRgbaG = new DarkUI.Controls.DarkNumericUpDown();
            this.nudRgbaR = new DarkUI.Controls.DarkNumericUpDown();
            this.btnAddFolder = new DarkUI.Controls.DarkButton();
            this.lblFolder = new System.Windows.Forms.Label();
            this.cmbFolder = new DarkUI.Controls.DarkComboBox();
            this.cmbRarity = new DarkUI.Controls.DarkComboBox();
            this.lblRarity = new System.Windows.Forms.Label();
            this.nudCooldown = new DarkUI.Controls.DarkNumericUpDown();
            this.lblCooldown = new System.Windows.Forms.Label();
            this.btnEditRequirements = new DarkUI.Controls.DarkButton();
            this.chkStackable = new DarkUI.Controls.DarkCheckBox();
            this.nudPrice = new DarkUI.Controls.DarkNumericUpDown();
            this.chkCanDrop = new DarkUI.Controls.DarkCheckBox();
            this.cmbAnimation = new DarkUI.Controls.DarkComboBox();
            this.lblDesc = new System.Windows.Forms.Label();
            this.txtDesc = new DarkUI.Controls.DarkTextBox();
            this.cmbPic = new DarkUI.Controls.DarkComboBox();
            this.lblAnim = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblPic = new System.Windows.Forms.Label();
            this.picItem = new System.Windows.Forms.PictureBox();
            this.lblType = new System.Windows.Forms.Label();
            this.cmbType = new DarkUI.Controls.DarkComboBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new DarkUI.Controls.DarkTextBox();
            this.grpEquipment = new DarkUI.Controls.DarkGroupBox();
            this.grpRegen = new DarkUI.Controls.DarkGroupBox();
            this.nudMpRegen = new DarkUI.Controls.DarkNumericUpDown();
            this.nudHPRegen = new DarkUI.Controls.DarkNumericUpDown();
            this.lblHpRegen = new System.Windows.Forms.Label();
            this.lblManaRegen = new System.Windows.Forms.Label();
            this.lblRegenHint = new System.Windows.Forms.Label();
            this.grpVitalBonuses = new DarkUI.Controls.DarkGroupBox();
            this.lblPercentage2 = new System.Windows.Forms.Label();
            this.lblPercentage1 = new System.Windows.Forms.Label();
            this.nudMPPercentage = new DarkUI.Controls.DarkNumericUpDown();
            this.nudHPPercentage = new DarkUI.Controls.DarkNumericUpDown();
            this.lblPlus2 = new System.Windows.Forms.Label();
            this.lblPlus1 = new System.Windows.Forms.Label();
            this.nudManaBonus = new DarkUI.Controls.DarkNumericUpDown();
            this.nudHealthBonus = new DarkUI.Controls.DarkNumericUpDown();
            this.lblManaBonus = new System.Windows.Forms.Label();
            this.lblHealthBonus = new System.Windows.Forms.Label();
            this.cmbEquipmentAnimation = new DarkUI.Controls.DarkComboBox();
            this.lblEquipmentAnimation = new System.Windows.Forms.Label();
            this.nudEffectPercent = new DarkUI.Controls.DarkNumericUpDown();
            this.grpStatBonuses = new DarkUI.Controls.DarkGroupBox();
            this.lblPercentage5 = new System.Windows.Forms.Label();
            this.lblPercentage4 = new System.Windows.Forms.Label();
            this.lblPercentage8 = new System.Windows.Forms.Label();
            this.lblPercentage7 = new System.Windows.Forms.Label();
            this.lblPercentage6 = new System.Windows.Forms.Label();
            this.nudSpdPercentage = new DarkUI.Controls.DarkNumericUpDown();
            this.nudMRPercentage = new DarkUI.Controls.DarkNumericUpDown();
            this.nudDefPercentage = new DarkUI.Controls.DarkNumericUpDown();
            this.nudMagPercentage = new DarkUI.Controls.DarkNumericUpDown();
            this.nudStrPercentage = new DarkUI.Controls.DarkNumericUpDown();
            this.lblPlus5 = new System.Windows.Forms.Label();
            this.lblPlus4 = new System.Windows.Forms.Label();
            this.lblPlus8 = new System.Windows.Forms.Label();
            this.lblPlus7 = new System.Windows.Forms.Label();
            this.lblPlus6 = new System.Windows.Forms.Label();
            this.nudRange = new DarkUI.Controls.DarkNumericUpDown();
            this.nudSpd = new DarkUI.Controls.DarkNumericUpDown();
            this.nudMR = new DarkUI.Controls.DarkNumericUpDown();
            this.nudDef = new DarkUI.Controls.DarkNumericUpDown();
            this.nudMag = new DarkUI.Controls.DarkNumericUpDown();
            this.nudStr = new DarkUI.Controls.DarkNumericUpDown();
            this.lblSpd = new System.Windows.Forms.Label();
            this.lblMR = new System.Windows.Forms.Label();
            this.lblDef = new System.Windows.Forms.Label();
            this.lblMag = new System.Windows.Forms.Label();
            this.lblStr = new System.Windows.Forms.Label();
            this.lblRange = new System.Windows.Forms.Label();
            this.cmbFemalePaperdoll = new DarkUI.Controls.DarkComboBox();
            this.lblFemalePaperdoll = new System.Windows.Forms.Label();
            this.picFemalePaperdoll = new System.Windows.Forms.PictureBox();
            this.lblEffectPercent = new System.Windows.Forms.Label();
            this.cmbEquipmentBonus = new DarkUI.Controls.DarkComboBox();
            this.lblBonusEffect = new System.Windows.Forms.Label();
            this.cmbEquipmentSlot = new DarkUI.Controls.DarkComboBox();
            this.lblEquipmentSlot = new System.Windows.Forms.Label();
            this.cmbMalePaperdoll = new DarkUI.Controls.DarkComboBox();
            this.lblMalePaperdoll = new System.Windows.Forms.Label();
            this.picMalePaperdoll = new System.Windows.Forms.PictureBox();
            this.grpWeaponProperties = new DarkUI.Controls.DarkGroupBox();
            this.nudCritMultiplier = new DarkUI.Controls.DarkNumericUpDown();
            this.lblCritMultiplier = new System.Windows.Forms.Label();
            this.grpAttackSpeed = new DarkUI.Controls.DarkGroupBox();
            this.nudAttackSpeedValue = new DarkUI.Controls.DarkNumericUpDown();
            this.lblAttackSpeedValue = new System.Windows.Forms.Label();
            this.cmbAttackSpeedModifier = new DarkUI.Controls.DarkComboBox();
            this.lblAttackSpeedModifier = new System.Windows.Forms.Label();
            this.nudScaling = new DarkUI.Controls.DarkNumericUpDown();
            this.nudCritChance = new DarkUI.Controls.DarkNumericUpDown();
            this.nudDamage = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbProjectile = new DarkUI.Controls.DarkComboBox();
            this.cmbScalingStat = new DarkUI.Controls.DarkComboBox();
            this.lblScalingStat = new System.Windows.Forms.Label();
            this.lblScalingAmount = new System.Windows.Forms.Label();
            this.cmbDamageType = new DarkUI.Controls.DarkComboBox();
            this.lblDamageType = new System.Windows.Forms.Label();
            this.lblCritChance = new System.Windows.Forms.Label();
            this.cmbAttackAnimation = new DarkUI.Controls.DarkComboBox();
            this.lblAttackAnimation = new System.Windows.Forms.Label();
            this.chk2Hand = new DarkUI.Controls.DarkCheckBox();
            this.lblToolType = new System.Windows.Forms.Label();
            this.cmbToolType = new DarkUI.Controls.DarkComboBox();
            this.lblProjectile = new System.Windows.Forms.Label();
            this.lblDamage = new System.Windows.Forms.Label();
            this.grpEvent = new DarkUI.Controls.DarkGroupBox();
            this.chkSingleUseEvent = new DarkUI.Controls.DarkCheckBox();
            this.cmbEvent = new DarkUI.Controls.DarkComboBox();
            this.grpConsumable = new DarkUI.Controls.DarkGroupBox();
            this.lblPercentage3 = new System.Windows.Forms.Label();
            this.nudIntervalPercentage = new DarkUI.Controls.DarkNumericUpDown();
            this.lblPlus3 = new System.Windows.Forms.Label();
            this.nudInterval = new DarkUI.Controls.DarkNumericUpDown();
            this.lblVital = new System.Windows.Forms.Label();
            this.cmbConsume = new DarkUI.Controls.DarkComboBox();
            this.lblInterval = new System.Windows.Forms.Label();
            this.grpSpell = new DarkUI.Controls.DarkGroupBox();
            this.chkSingleUseSpell = new DarkUI.Controls.DarkCheckBox();
            this.chkQuickCast = new DarkUI.Controls.DarkCheckBox();
            this.cmbTeachSpell = new DarkUI.Controls.DarkComboBox();
            this.lblSpell = new System.Windows.Forms.Label();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.grpBags = new DarkUI.Controls.DarkGroupBox();
            this.nudBag = new DarkUI.Controls.DarkNumericUpDown();
            this.lblBag = new System.Windows.Forms.Label();
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
            this.grpRequirements = new DarkUI.Controls.DarkGroupBox();
            this.lblCannotUse = new System.Windows.Forms.Label();
            this.txtCannotUse = new DarkUI.Controls.DarkTextBox();
            this.grpItems.SuspendLayout();
            this.grpGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBankStackLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInvStackLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDeathDropChance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRgbaA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRgbaB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRgbaG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRgbaR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCooldown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).BeginInit();
            this.grpEquipment.SuspendLayout();
            this.grpRegen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMpRegen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHPRegen)).BeginInit();
            this.grpVitalBonuses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMPPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHPPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudManaBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHealthBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEffectPercent)).BeginInit();
            this.grpStatBonuses.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpdPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMRPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDefPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMagPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStrPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDef)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFemalePaperdoll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMalePaperdoll)).BeginInit();
            this.grpWeaponProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCritMultiplier)).BeginInit();
            this.grpAttackSpeed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAttackSpeedValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScaling)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCritChance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDamage)).BeginInit();
            this.grpEvent.SuspendLayout();
            this.grpConsumable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval)).BeginInit();
            this.grpSpell.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.grpBags.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBag)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.grpRequirements.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpItems
            // 
            this.grpItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpItems.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpItems.Controls.Add(this.btnClearSearch);
            this.grpItems.Controls.Add(this.txtSearch);
            this.grpItems.Controls.Add(this.lstGameObjects);
            this.grpItems.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpItems.Location = new System.Drawing.Point(12, 34);
            this.grpItems.Name = "grpItems";
            this.grpItems.Size = new System.Drawing.Size(203, 476);
            this.grpItems.TabIndex = 1;
            this.grpItems.TabStop = false;
            this.grpItems.Text = "Items";
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.Location = new System.Drawing.Point(179, 17);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Padding = new System.Windows.Forms.Padding(5);
            this.btnClearSearch.Size = new System.Drawing.Size(18, 20);
            this.btnClearSearch.TabIndex = 31;
            this.btnClearSearch.Text = "X";
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSearch.Location = new System.Drawing.Point(6, 17);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(167, 20);
            this.txtSearch.TabIndex = 30;
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
            this.lstGameObjects.Location = new System.Drawing.Point(6, 43);
            this.lstGameObjects.Name = "lstGameObjects";
            this.lstGameObjects.SelectedImageIndex = 0;
            this.lstGameObjects.Size = new System.Drawing.Size(191, 427);
            this.lstGameObjects.TabIndex = 29;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(494, 482);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(190, 28);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(298, 482);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(190, 28);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpGeneral
            // 
            this.grpGeneral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpGeneral.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpGeneral.Controls.Add(this.grpRequirements);
            this.grpGeneral.Controls.Add(this.chkCanGuildBank);
            this.grpGeneral.Controls.Add(this.nudBankStackLimit);
            this.grpGeneral.Controls.Add(this.nudInvStackLimit);
            this.grpGeneral.Controls.Add(this.lblBankStackLimit);
            this.grpGeneral.Controls.Add(this.lblInvStackLimit);
            this.grpGeneral.Controls.Add(this.nudDeathDropChance);
            this.grpGeneral.Controls.Add(this.lblDeathDropChance);
            this.grpGeneral.Controls.Add(this.chkCanSell);
            this.grpGeneral.Controls.Add(this.chkCanTrade);
            this.grpGeneral.Controls.Add(this.chkCanBag);
            this.grpGeneral.Controls.Add(this.chkCanBank);
            this.grpGeneral.Controls.Add(this.chkIgnoreCdr);
            this.grpGeneral.Controls.Add(this.chkIgnoreGlobalCooldown);
            this.grpGeneral.Controls.Add(this.btnAddCooldownGroup);
            this.grpGeneral.Controls.Add(this.cmbCooldownGroup);
            this.grpGeneral.Controls.Add(this.lblCooldownGroup);
            this.grpGeneral.Controls.Add(this.lblAlpha);
            this.grpGeneral.Controls.Add(this.lblBlue);
            this.grpGeneral.Controls.Add(this.lblGreen);
            this.grpGeneral.Controls.Add(this.lblRed);
            this.grpGeneral.Controls.Add(this.nudRgbaA);
            this.grpGeneral.Controls.Add(this.nudRgbaB);
            this.grpGeneral.Controls.Add(this.nudRgbaG);
            this.grpGeneral.Controls.Add(this.nudRgbaR);
            this.grpGeneral.Controls.Add(this.btnAddFolder);
            this.grpGeneral.Controls.Add(this.lblFolder);
            this.grpGeneral.Controls.Add(this.cmbFolder);
            this.grpGeneral.Controls.Add(this.cmbRarity);
            this.grpGeneral.Controls.Add(this.lblRarity);
            this.grpGeneral.Controls.Add(this.nudCooldown);
            this.grpGeneral.Controls.Add(this.lblCooldown);
            this.grpGeneral.Controls.Add(this.chkStackable);
            this.grpGeneral.Controls.Add(this.nudPrice);
            this.grpGeneral.Controls.Add(this.chkCanDrop);
            this.grpGeneral.Controls.Add(this.cmbAnimation);
            this.grpGeneral.Controls.Add(this.lblDesc);
            this.grpGeneral.Controls.Add(this.txtDesc);
            this.grpGeneral.Controls.Add(this.cmbPic);
            this.grpGeneral.Controls.Add(this.lblAnim);
            this.grpGeneral.Controls.Add(this.lblPrice);
            this.grpGeneral.Controls.Add(this.lblPic);
            this.grpGeneral.Controls.Add(this.picItem);
            this.grpGeneral.Controls.Add(this.lblType);
            this.grpGeneral.Controls.Add(this.cmbType);
            this.grpGeneral.Controls.Add(this.lblName);
            this.grpGeneral.Controls.Add(this.txtName);
            this.grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpGeneral.Location = new System.Drawing.Point(2, 1);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(439, 427);
            this.grpGeneral.TabIndex = 2;
            this.grpGeneral.TabStop = false;
            this.grpGeneral.Text = "General";
            // 
            // chkCanGuildBank
            // 
            this.chkCanGuildBank.AutoSize = true;
            this.chkCanGuildBank.Location = new System.Drawing.Point(121, 241);
            this.chkCanGuildBank.Name = "chkCanGuildBank";
            this.chkCanGuildBank.Size = new System.Drawing.Size(106, 17);
            this.chkCanGuildBank.TabIndex = 99;
            this.chkCanGuildBank.Text = "Can Guild Bank?";
            this.chkCanGuildBank.CheckedChanged += new System.EventHandler(this.chkCanGuildBank_CheckedChanged);
            // 
            // nudBankStackLimit
            // 
            this.nudBankStackLimit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudBankStackLimit.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudBankStackLimit.Location = new System.Drawing.Point(262, 401);
            this.nudBankStackLimit.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudBankStackLimit.Name = "nudBankStackLimit";
            this.nudBankStackLimit.Size = new System.Drawing.Size(171, 20);
            this.nudBankStackLimit.TabIndex = 98;
            this.nudBankStackLimit.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudBankStackLimit.ValueChanged += new System.EventHandler(this.nudBankStackLimit_ValueChanged);
            // 
            // nudInvStackLimit
            // 
            this.nudInvStackLimit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudInvStackLimit.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudInvStackLimit.Location = new System.Drawing.Point(262, 362);
            this.nudInvStackLimit.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nudInvStackLimit.Name = "nudInvStackLimit";
            this.nudInvStackLimit.Size = new System.Drawing.Size(171, 20);
            this.nudInvStackLimit.TabIndex = 97;
            this.nudInvStackLimit.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudInvStackLimit.ValueChanged += new System.EventHandler(this.nudInvStackLimit_ValueChanged);
            // 
            // lblBankStackLimit
            // 
            this.lblBankStackLimit.AutoSize = true;
            this.lblBankStackLimit.Location = new System.Drawing.Point(259, 384);
            this.lblBankStackLimit.Name = "lblBankStackLimit";
            this.lblBankStackLimit.Size = new System.Drawing.Size(90, 13);
            this.lblBankStackLimit.TabIndex = 96;
            this.lblBankStackLimit.Text = "Bank Stack Limit:";
            // 
            // lblInvStackLimit
            // 
            this.lblInvStackLimit.AutoSize = true;
            this.lblInvStackLimit.Location = new System.Drawing.Point(259, 346);
            this.lblInvStackLimit.Name = "lblInvStackLimit";
            this.lblInvStackLimit.Size = new System.Drawing.Size(109, 13);
            this.lblInvStackLimit.TabIndex = 95;
            this.lblInvStackLimit.Text = "Inventory Stack Limit:";
            // 
            // nudDeathDropChance
            // 
            this.nudDeathDropChance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudDeathDropChance.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudDeathDropChance.Location = new System.Drawing.Point(154, 300);
            this.nudDeathDropChance.Name = "nudDeathDropChance";
            this.nudDeathDropChance.Size = new System.Drawing.Size(48, 20);
            this.nudDeathDropChance.TabIndex = 94;
            this.nudDeathDropChance.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudDeathDropChance.ValueChanged += new System.EventHandler(this.nudDeathDropChance_ValueChanged);
            // 
            // lblDeathDropChance
            // 
            this.lblDeathDropChance.AutoSize = true;
            this.lblDeathDropChance.Location = new System.Drawing.Point(12, 305);
            this.lblDeathDropChance.Name = "lblDeathDropChance";
            this.lblDeathDropChance.Size = new System.Drawing.Size(136, 13);
            this.lblDeathDropChance.TabIndex = 93;
            this.lblDeathDropChance.Text = "Drop chance on Death (%):";
            // 
            // chkCanSell
            // 
            this.chkCanSell.AutoSize = true;
            this.chkCanSell.Location = new System.Drawing.Point(121, 281);
            this.chkCanSell.Name = "chkCanSell";
            this.chkCanSell.Size = new System.Drawing.Size(71, 17);
            this.chkCanSell.TabIndex = 92;
            this.chkCanSell.Text = "Can Sell?";
            this.chkCanSell.CheckedChanged += new System.EventHandler(this.chkCanSell_CheckedChanged);
            // 
            // chkCanTrade
            // 
            this.chkCanTrade.AutoSize = true;
            this.chkCanTrade.Location = new System.Drawing.Point(13, 281);
            this.chkCanTrade.Name = "chkCanTrade";
            this.chkCanTrade.Size = new System.Drawing.Size(82, 17);
            this.chkCanTrade.TabIndex = 91;
            this.chkCanTrade.Text = "Can Trade?";
            this.chkCanTrade.CheckedChanged += new System.EventHandler(this.chkCanTrade_CheckedChanged);
            // 
            // chkCanBag
            // 
            this.chkCanBag.AutoSize = true;
            this.chkCanBag.Location = new System.Drawing.Point(121, 261);
            this.chkCanBag.Name = "chkCanBag";
            this.chkCanBag.Size = new System.Drawing.Size(73, 17);
            this.chkCanBag.TabIndex = 90;
            this.chkCanBag.Text = "Can Bag?";
            this.chkCanBag.CheckedChanged += new System.EventHandler(this.chkCanBag_CheckedChanged);
            // 
            // chkCanBank
            // 
            this.chkCanBank.AutoSize = true;
            this.chkCanBank.Location = new System.Drawing.Point(13, 261);
            this.chkCanBank.Name = "chkCanBank";
            this.chkCanBank.Size = new System.Drawing.Size(79, 17);
            this.chkCanBank.TabIndex = 89;
            this.chkCanBank.Text = "Can Bank?";
            this.chkCanBank.CheckedChanged += new System.EventHandler(this.chkCanBank_CheckedChanged);
            // 
            // chkIgnoreCdr
            // 
            this.chkIgnoreCdr.AutoSize = true;
            this.chkIgnoreCdr.Location = new System.Drawing.Point(262, 296);
            this.chkIgnoreCdr.Name = "chkIgnoreCdr";
            this.chkIgnoreCdr.Size = new System.Drawing.Size(164, 17);
            this.chkIgnoreCdr.TabIndex = 87;
            this.chkIgnoreCdr.Text = "Ignore Cooldown Reduction?";
            this.chkIgnoreCdr.CheckedChanged += new System.EventHandler(this.chkIgnoreCdr_CheckedChanged);
            // 
            // chkIgnoreGlobalCooldown
            // 
            this.chkIgnoreGlobalCooldown.AutoSize = true;
            this.chkIgnoreGlobalCooldown.Location = new System.Drawing.Point(262, 277);
            this.chkIgnoreGlobalCooldown.Name = "chkIgnoreGlobalCooldown";
            this.chkIgnoreGlobalCooldown.Size = new System.Drawing.Size(145, 17);
            this.chkIgnoreGlobalCooldown.TabIndex = 53;
            this.chkIgnoreGlobalCooldown.Text = "Ignore Global Cooldown?";
            this.chkIgnoreGlobalCooldown.CheckedChanged += new System.EventHandler(this.chkIgnoreGlobalCooldown_CheckedChanged);
            // 
            // btnAddCooldownGroup
            // 
            this.btnAddCooldownGroup.Location = new System.Drawing.Point(415, 254);
            this.btnAddCooldownGroup.Name = "btnAddCooldownGroup";
            this.btnAddCooldownGroup.Padding = new System.Windows.Forms.Padding(5);
            this.btnAddCooldownGroup.Size = new System.Drawing.Size(18, 21);
            this.btnAddCooldownGroup.TabIndex = 52;
            this.btnAddCooldownGroup.Text = "+";
            this.btnAddCooldownGroup.Click += new System.EventHandler(this.btnAddCooldownGroup_Click);
            // 
            // cmbCooldownGroup
            // 
            this.cmbCooldownGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbCooldownGroup.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbCooldownGroup.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbCooldownGroup.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbCooldownGroup.DrawDropdownHoverOutline = false;
            this.cmbCooldownGroup.DrawFocusRectangle = false;
            this.cmbCooldownGroup.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCooldownGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCooldownGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCooldownGroup.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbCooldownGroup.FormattingEnabled = true;
            this.cmbCooldownGroup.Location = new System.Drawing.Point(262, 254);
            this.cmbCooldownGroup.Name = "cmbCooldownGroup";
            this.cmbCooldownGroup.Size = new System.Drawing.Size(147, 21);
            this.cmbCooldownGroup.TabIndex = 51;
            this.cmbCooldownGroup.Text = null;
            this.cmbCooldownGroup.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbCooldownGroup.SelectedIndexChanged += new System.EventHandler(this.cmbCooldownGroup_SelectedIndexChanged);
            // 
            // lblCooldownGroup
            // 
            this.lblCooldownGroup.AutoSize = true;
            this.lblCooldownGroup.Location = new System.Drawing.Point(259, 237);
            this.lblCooldownGroup.Name = "lblCooldownGroup";
            this.lblCooldownGroup.Size = new System.Drawing.Size(89, 13);
            this.lblCooldownGroup.TabIndex = 50;
            this.lblCooldownGroup.Text = "Cooldown Group:";
            // 
            // lblAlpha
            // 
            this.lblAlpha.AutoSize = true;
            this.lblAlpha.Location = new System.Drawing.Point(344, 88);
            this.lblAlpha.Name = "lblAlpha";
            this.lblAlpha.Size = new System.Drawing.Size(37, 13);
            this.lblAlpha.TabIndex = 86;
            this.lblAlpha.Text = "Alpha:";
            // 
            // lblBlue
            // 
            this.lblBlue.AutoSize = true;
            this.lblBlue.Location = new System.Drawing.Point(259, 86);
            this.lblBlue.Name = "lblBlue";
            this.lblBlue.Size = new System.Drawing.Size(31, 13);
            this.lblBlue.TabIndex = 85;
            this.lblBlue.Text = "Blue:";
            // 
            // lblGreen
            // 
            this.lblGreen.AutoSize = true;
            this.lblGreen.Location = new System.Drawing.Point(344, 62);
            this.lblGreen.Name = "lblGreen";
            this.lblGreen.Size = new System.Drawing.Size(39, 13);
            this.lblGreen.TabIndex = 84;
            this.lblGreen.Text = "Green:";
            // 
            // lblRed
            // 
            this.lblRed.AutoSize = true;
            this.lblRed.Location = new System.Drawing.Point(259, 60);
            this.lblRed.Name = "lblRed";
            this.lblRed.Size = new System.Drawing.Size(30, 13);
            this.lblRed.TabIndex = 83;
            this.lblRed.Text = "Red:";
            // 
            // nudRgbaA
            // 
            this.nudRgbaA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudRgbaA.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudRgbaA.Location = new System.Drawing.Point(391, 86);
            this.nudRgbaA.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRgbaA.Name = "nudRgbaA";
            this.nudRgbaA.Size = new System.Drawing.Size(42, 20);
            this.nudRgbaA.TabIndex = 82;
            this.nudRgbaA.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRgbaA.ValueChanged += new System.EventHandler(this.nudRgbaA_ValueChanged);
            // 
            // nudRgbaB
            // 
            this.nudRgbaB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudRgbaB.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudRgbaB.Location = new System.Drawing.Point(296, 84);
            this.nudRgbaB.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRgbaB.Name = "nudRgbaB";
            this.nudRgbaB.Size = new System.Drawing.Size(42, 20);
            this.nudRgbaB.TabIndex = 81;
            this.nudRgbaB.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRgbaB.ValueChanged += new System.EventHandler(this.nudRgbaB_ValueChanged);
            // 
            // nudRgbaG
            // 
            this.nudRgbaG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudRgbaG.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudRgbaG.Location = new System.Drawing.Point(391, 60);
            this.nudRgbaG.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRgbaG.Name = "nudRgbaG";
            this.nudRgbaG.Size = new System.Drawing.Size(42, 20);
            this.nudRgbaG.TabIndex = 80;
            this.nudRgbaG.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRgbaG.ValueChanged += new System.EventHandler(this.nudRgbaG_ValueChanged);
            // 
            // nudRgbaR
            // 
            this.nudRgbaR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudRgbaR.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudRgbaR.Location = new System.Drawing.Point(296, 58);
            this.nudRgbaR.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRgbaR.Name = "nudRgbaR";
            this.nudRgbaR.Size = new System.Drawing.Size(42, 20);
            this.nudRgbaR.TabIndex = 79;
            this.nudRgbaR.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudRgbaR.ValueChanged += new System.EventHandler(this.nudRgbaR_ValueChanged);
            // 
            // btnAddFolder
            // 
            this.btnAddFolder.Location = new System.Drawing.Point(180, 58);
            this.btnAddFolder.Name = "btnAddFolder";
            this.btnAddFolder.Padding = new System.Windows.Forms.Padding(5);
            this.btnAddFolder.Size = new System.Drawing.Size(18, 21);
            this.btnAddFolder.TabIndex = 49;
            this.btnAddFolder.Text = "+";
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            // 
            // lblFolder
            // 
            this.lblFolder.AutoSize = true;
            this.lblFolder.Location = new System.Drawing.Point(8, 62);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(39, 13);
            this.lblFolder.TabIndex = 48;
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
            this.cmbFolder.Location = new System.Drawing.Point(53, 58);
            this.cmbFolder.Name = "cmbFolder";
            this.cmbFolder.Size = new System.Drawing.Size(123, 21);
            this.cmbFolder.TabIndex = 47;
            this.cmbFolder.Text = null;
            this.cmbFolder.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbFolder.SelectedIndexChanged += new System.EventHandler(this.cmbFolder_SelectedIndexChanged);
            // 
            // cmbRarity
            // 
            this.cmbRarity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbRarity.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbRarity.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbRarity.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbRarity.DrawDropdownHoverOutline = false;
            this.cmbRarity.DrawFocusRectangle = false;
            this.cmbRarity.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbRarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRarity.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbRarity.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbRarity.FormattingEnabled = true;
            this.cmbRarity.Items.AddRange(new object[] {
            "None",
            "Common",
            "Uncommon",
            "Rare",
            "Epic",
            "Legendary"});
            this.cmbRarity.Location = new System.Drawing.Point(53, 127);
            this.cmbRarity.Name = "cmbRarity";
            this.cmbRarity.Size = new System.Drawing.Size(149, 21);
            this.cmbRarity.TabIndex = 41;
            this.cmbRarity.Text = "None";
            this.cmbRarity.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbRarity.SelectedIndexChanged += new System.EventHandler(this.cmbRarity_SelectedIndexChanged);
            // 
            // lblRarity
            // 
            this.lblRarity.AutoSize = true;
            this.lblRarity.Location = new System.Drawing.Point(8, 130);
            this.lblRarity.Name = "lblRarity";
            this.lblRarity.Size = new System.Drawing.Size(37, 13);
            this.lblRarity.TabIndex = 40;
            this.lblRarity.Text = "Rarity:";
            // 
            // nudCooldown
            // 
            this.nudCooldown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudCooldown.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudCooldown.Location = new System.Drawing.Point(262, 210);
            this.nudCooldown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudCooldown.Name = "nudCooldown";
            this.nudCooldown.Size = new System.Drawing.Size(171, 20);
            this.nudCooldown.TabIndex = 39;
            this.nudCooldown.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudCooldown.ValueChanged += new System.EventHandler(this.nudCooldown_ValueChanged);
            // 
            // lblCooldown
            // 
            this.lblCooldown.AutoSize = true;
            this.lblCooldown.Location = new System.Drawing.Point(259, 193);
            this.lblCooldown.Name = "lblCooldown";
            this.lblCooldown.Size = new System.Drawing.Size(79, 13);
            this.lblCooldown.TabIndex = 38;
            this.lblCooldown.Text = "Cooldown (ms):";
            // 
            // btnEditRequirements
            // 
            this.btnEditRequirements.Location = new System.Drawing.Point(8, 18);
            this.btnEditRequirements.Name = "btnEditRequirements";
            this.btnEditRequirements.Padding = new System.Windows.Forms.Padding(5);
            this.btnEditRequirements.Size = new System.Drawing.Size(207, 23);
            this.btnEditRequirements.TabIndex = 0;
            this.btnEditRequirements.Text = "Edit Usage Requirements";
            this.btnEditRequirements.Click += new System.EventHandler(this.btnEditRequirements_Click);
            // 
            // chkStackable
            // 
            this.chkStackable.AutoSize = true;
            this.chkStackable.Location = new System.Drawing.Point(262, 326);
            this.chkStackable.Name = "chkStackable";
            this.chkStackable.Size = new System.Drawing.Size(80, 17);
            this.chkStackable.TabIndex = 27;
            this.chkStackable.Text = "Stackable?";
            this.chkStackable.CheckedChanged += new System.EventHandler(this.chkStackable_CheckedChanged);
            // 
            // nudPrice
            // 
            this.nudPrice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudPrice.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudPrice.Location = new System.Drawing.Point(262, 128);
            this.nudPrice.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudPrice.Name = "nudPrice";
            this.nudPrice.Size = new System.Drawing.Size(171, 20);
            this.nudPrice.TabIndex = 37;
            this.nudPrice.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudPrice.ValueChanged += new System.EventHandler(this.nudPrice_ValueChanged);
            // 
            // chkCanDrop
            // 
            this.chkCanDrop.AutoSize = true;
            this.chkCanDrop.Location = new System.Drawing.Point(13, 241);
            this.chkCanDrop.Name = "chkCanDrop";
            this.chkCanDrop.Size = new System.Drawing.Size(77, 17);
            this.chkCanDrop.TabIndex = 26;
            this.chkCanDrop.Text = "Can Drop?";
            this.chkCanDrop.CheckedChanged += new System.EventHandler(this.chkBound_CheckedChanged);
            // 
            // cmbAnimation
            // 
            this.cmbAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAnimation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAnimation.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAnimation.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbAnimation.DrawDropdownHoverOutline = false;
            this.cmbAnimation.DrawFocusRectangle = false;
            this.cmbAnimation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnimation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbAnimation.FormattingEnabled = true;
            this.cmbAnimation.Location = new System.Drawing.Point(262, 168);
            this.cmbAnimation.Name = "cmbAnimation";
            this.cmbAnimation.Size = new System.Drawing.Size(171, 21);
            this.cmbAnimation.TabIndex = 14;
            this.cmbAnimation.Text = null;
            this.cmbAnimation.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbAnimation.SelectedIndexChanged += new System.EventHandler(this.cmbAnimation_SelectedIndexChanged);
            // 
            // lblDesc
            // 
            this.lblDesc.AutoSize = true;
            this.lblDesc.Location = new System.Drawing.Point(11, 152);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(35, 13);
            this.lblDesc.TabIndex = 13;
            this.lblDesc.Text = "Desc:";
            // 
            // txtDesc
            // 
            this.txtDesc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDesc.Location = new System.Drawing.Point(12, 168);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(186, 67);
            this.txtDesc.TabIndex = 12;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // cmbPic
            // 
            this.cmbPic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbPic.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbPic.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbPic.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbPic.DrawDropdownHoverOutline = false;
            this.cmbPic.DrawFocusRectangle = false;
            this.cmbPic.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbPic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPic.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbPic.FormattingEnabled = true;
            this.cmbPic.Items.AddRange(new object[] {
            "None"});
            this.cmbPic.Location = new System.Drawing.Point(262, 31);
            this.cmbPic.Name = "cmbPic";
            this.cmbPic.Size = new System.Drawing.Size(171, 21);
            this.cmbPic.TabIndex = 11;
            this.cmbPic.Text = "None";
            this.cmbPic.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbPic.SelectedIndexChanged += new System.EventHandler(this.cmbPic_SelectedIndexChanged);
            // 
            // lblAnim
            // 
            this.lblAnim.AutoSize = true;
            this.lblAnim.Location = new System.Drawing.Point(259, 152);
            this.lblAnim.Name = "lblAnim";
            this.lblAnim.Size = new System.Drawing.Size(53, 13);
            this.lblAnim.TabIndex = 9;
            this.lblAnim.Text = "Animation";
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(259, 111);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(34, 13);
            this.lblPrice.TabIndex = 7;
            this.lblPrice.Text = "Price:";
            // 
            // lblPic
            // 
            this.lblPic.AutoSize = true;
            this.lblPic.Location = new System.Drawing.Point(259, 14);
            this.lblPic.Name = "lblPic";
            this.lblPic.Size = new System.Drawing.Size(25, 13);
            this.lblPic.TabIndex = 6;
            this.lblPic.Text = "Pic:";
            // 
            // picItem
            // 
            this.picItem.BackColor = System.Drawing.Color.Black;
            this.picItem.Location = new System.Drawing.Point(221, 25);
            this.picItem.Name = "picItem";
            this.picItem.Size = new System.Drawing.Size(32, 32);
            this.picItem.TabIndex = 4;
            this.picItem.TabStop = false;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(9, 96);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(34, 13);
            this.lblType.TabIndex = 3;
            this.lblType.Text = "Type:";
            // 
            // cmbType
            // 
            this.cmbType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbType.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbType.DrawDropdownHoverOutline = false;
            this.cmbType.DrawFocusRectangle = false;
            this.cmbType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "None",
            "Equipment",
            "Consumable",
            "Currency",
            "Spell",
            "Event",
            "Bag"});
            this.cmbType.Location = new System.Drawing.Point(53, 93);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(149, 21);
            this.cmbType.TabIndex = 2;
            this.cmbType.Text = "None";
            this.cmbType.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(8, 27);
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
            this.txtName.Location = new System.Drawing.Point(53, 25);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(145, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // grpEquipment
            // 
            this.grpEquipment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpEquipment.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEquipment.Controls.Add(this.grpRegen);
            this.grpEquipment.Controls.Add(this.grpVitalBonuses);
            this.grpEquipment.Controls.Add(this.cmbEquipmentAnimation);
            this.grpEquipment.Controls.Add(this.lblEquipmentAnimation);
            this.grpEquipment.Controls.Add(this.nudEffectPercent);
            this.grpEquipment.Controls.Add(this.grpStatBonuses);
            this.grpEquipment.Controls.Add(this.cmbFemalePaperdoll);
            this.grpEquipment.Controls.Add(this.lblFemalePaperdoll);
            this.grpEquipment.Controls.Add(this.picFemalePaperdoll);
            this.grpEquipment.Controls.Add(this.lblEffectPercent);
            this.grpEquipment.Controls.Add(this.cmbEquipmentBonus);
            this.grpEquipment.Controls.Add(this.lblBonusEffect);
            this.grpEquipment.Controls.Add(this.cmbEquipmentSlot);
            this.grpEquipment.Controls.Add(this.lblEquipmentSlot);
            this.grpEquipment.Controls.Add(this.cmbMalePaperdoll);
            this.grpEquipment.Controls.Add(this.lblMalePaperdoll);
            this.grpEquipment.Controls.Add(this.picMalePaperdoll);
            this.grpEquipment.Controls.Add(this.grpWeaponProperties);
            this.grpEquipment.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEquipment.Location = new System.Drawing.Point(2, 435);
            this.grpEquipment.Name = "grpEquipment";
            this.grpEquipment.Size = new System.Drawing.Size(439, 742);
            this.grpEquipment.TabIndex = 12;
            this.grpEquipment.TabStop = false;
            this.grpEquipment.Text = "Equipment";
            this.grpEquipment.Visible = false;
            // 
            // grpRegen
            // 
            this.grpRegen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpRegen.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpRegen.Controls.Add(this.nudMpRegen);
            this.grpRegen.Controls.Add(this.nudHPRegen);
            this.grpRegen.Controls.Add(this.lblHpRegen);
            this.grpRegen.Controls.Add(this.lblManaRegen);
            this.grpRegen.Controls.Add(this.lblRegenHint);
            this.grpRegen.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpRegen.Location = new System.Drawing.Point(13, 388);
            this.grpRegen.Margin = new System.Windows.Forms.Padding(2);
            this.grpRegen.Name = "grpRegen";
            this.grpRegen.Padding = new System.Windows.Forms.Padding(2);
            this.grpRegen.Size = new System.Drawing.Size(202, 90);
            this.grpRegen.TabIndex = 59;
            this.grpRegen.TabStop = false;
            this.grpRegen.Text = "Regen";
            // 
            // nudMpRegen
            // 
            this.nudMpRegen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudMpRegen.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudMpRegen.Location = new System.Drawing.Point(106, 31);
            this.nudMpRegen.Name = "nudMpRegen";
            this.nudMpRegen.Size = new System.Drawing.Size(87, 20);
            this.nudMpRegen.TabIndex = 31;
            this.nudMpRegen.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMpRegen.ValueChanged += new System.EventHandler(this.nudMpRegen_ValueChanged);
            // 
            // nudHPRegen
            // 
            this.nudHPRegen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudHPRegen.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudHPRegen.Location = new System.Drawing.Point(8, 31);
            this.nudHPRegen.Name = "nudHPRegen";
            this.nudHPRegen.Size = new System.Drawing.Size(87, 20);
            this.nudHPRegen.TabIndex = 30;
            this.nudHPRegen.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudHPRegen.ValueChanged += new System.EventHandler(this.nudHPRegen_ValueChanged);
            // 
            // lblHpRegen
            // 
            this.lblHpRegen.AutoSize = true;
            this.lblHpRegen.Location = new System.Drawing.Point(5, 17);
            this.lblHpRegen.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHpRegen.Name = "lblHpRegen";
            this.lblHpRegen.Size = new System.Drawing.Size(42, 13);
            this.lblHpRegen.TabIndex = 26;
            this.lblHpRegen.Text = "HP: (%)";
            // 
            // lblManaRegen
            // 
            this.lblManaRegen.AutoSize = true;
            this.lblManaRegen.Location = new System.Drawing.Point(103, 17);
            this.lblManaRegen.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblManaRegen.Name = "lblManaRegen";
            this.lblManaRegen.Size = new System.Drawing.Size(54, 13);
            this.lblManaRegen.TabIndex = 27;
            this.lblManaRegen.Text = "Mana: (%)";
            // 
            // lblRegenHint
            // 
            this.lblRegenHint.Location = new System.Drawing.Point(7, 54);
            this.lblRegenHint.Name = "lblRegenHint";
            this.lblRegenHint.Size = new System.Drawing.Size(191, 31);
            this.lblRegenHint.TabIndex = 0;
            this.lblRegenHint.Text = "% of HP/Mana to restore per tick.\r\nTick timer saved in server config.json.";
            // 
            // grpVitalBonuses
            // 
            this.grpVitalBonuses.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpVitalBonuses.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpVitalBonuses.Controls.Add(this.lblPercentage2);
            this.grpVitalBonuses.Controls.Add(this.lblPercentage1);
            this.grpVitalBonuses.Controls.Add(this.nudMPPercentage);
            this.grpVitalBonuses.Controls.Add(this.nudHPPercentage);
            this.grpVitalBonuses.Controls.Add(this.lblPlus2);
            this.grpVitalBonuses.Controls.Add(this.lblPlus1);
            this.grpVitalBonuses.Controls.Add(this.nudManaBonus);
            this.grpVitalBonuses.Controls.Add(this.nudHealthBonus);
            this.grpVitalBonuses.Controls.Add(this.lblManaBonus);
            this.grpVitalBonuses.Controls.Add(this.lblHealthBonus);
            this.grpVitalBonuses.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpVitalBonuses.Location = new System.Drawing.Point(15, 42);
            this.grpVitalBonuses.Name = "grpVitalBonuses";
            this.grpVitalBonuses.Size = new System.Drawing.Size(200, 102);
            this.grpVitalBonuses.TabIndex = 58;
            this.grpVitalBonuses.TabStop = false;
            this.grpVitalBonuses.Text = "Vital Bonuses";
            // 
            // lblPercentage2
            // 
            this.lblPercentage2.AutoSize = true;
            this.lblPercentage2.Location = new System.Drawing.Point(179, 75);
            this.lblPercentage2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPercentage2.Name = "lblPercentage2";
            this.lblPercentage2.Size = new System.Drawing.Size(15, 13);
            this.lblPercentage2.TabIndex = 70;
            this.lblPercentage2.Text = "%";
            // 
            // lblPercentage1
            // 
            this.lblPercentage1.AutoSize = true;
            this.lblPercentage1.Location = new System.Drawing.Point(179, 39);
            this.lblPercentage1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPercentage1.Name = "lblPercentage1";
            this.lblPercentage1.Size = new System.Drawing.Size(15, 13);
            this.lblPercentage1.TabIndex = 69;
            this.lblPercentage1.Text = "%";
            // 
            // nudMPPercentage
            // 
            this.nudMPPercentage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudMPPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudMPPercentage.Location = new System.Drawing.Point(132, 74);
            this.nudMPPercentage.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMPPercentage.Name = "nudMPPercentage";
            this.nudMPPercentage.Size = new System.Drawing.Size(43, 20);
            this.nudMPPercentage.TabIndex = 68;
            this.nudMPPercentage.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMPPercentage.ValueChanged += new System.EventHandler(this.nudMPPercentage_ValueChanged);
            // 
            // nudHPPercentage
            // 
            this.nudHPPercentage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudHPPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudHPPercentage.Location = new System.Drawing.Point(132, 37);
            this.nudHPPercentage.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudHPPercentage.Name = "nudHPPercentage";
            this.nudHPPercentage.Size = new System.Drawing.Size(43, 20);
            this.nudHPPercentage.TabIndex = 67;
            this.nudHPPercentage.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudHPPercentage.ValueChanged += new System.EventHandler(this.nudHPPercentage_ValueChanged);
            // 
            // lblPlus2
            // 
            this.lblPlus2.AutoSize = true;
            this.lblPlus2.Location = new System.Drawing.Point(114, 75);
            this.lblPlus2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlus2.Name = "lblPlus2";
            this.lblPlus2.Size = new System.Drawing.Size(13, 13);
            this.lblPlus2.TabIndex = 66;
            this.lblPlus2.Text = "+";
            // 
            // lblPlus1
            // 
            this.lblPlus1.AutoSize = true;
            this.lblPlus1.Location = new System.Drawing.Point(114, 39);
            this.lblPlus1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlus1.Name = "lblPlus1";
            this.lblPlus1.Size = new System.Drawing.Size(13, 13);
            this.lblPlus1.TabIndex = 65;
            this.lblPlus1.Text = "+";
            // 
            // nudManaBonus
            // 
            this.nudManaBonus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudManaBonus.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudManaBonus.Location = new System.Drawing.Point(12, 76);
            this.nudManaBonus.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nudManaBonus.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.nudManaBonus.Name = "nudManaBonus";
            this.nudManaBonus.Size = new System.Drawing.Size(95, 20);
            this.nudManaBonus.TabIndex = 49;
            this.nudManaBonus.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudManaBonus.ValueChanged += new System.EventHandler(this.nudManaBonus_ValueChanged);
            // 
            // nudHealthBonus
            // 
            this.nudHealthBonus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudHealthBonus.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudHealthBonus.Location = new System.Drawing.Point(12, 37);
            this.nudHealthBonus.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nudHealthBonus.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.nudHealthBonus.Name = "nudHealthBonus";
            this.nudHealthBonus.Size = new System.Drawing.Size(95, 20);
            this.nudHealthBonus.TabIndex = 48;
            this.nudHealthBonus.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudHealthBonus.ValueChanged += new System.EventHandler(this.nudHealthBonus_ValueChanged);
            // 
            // lblManaBonus
            // 
            this.lblManaBonus.AutoSize = true;
            this.lblManaBonus.Location = new System.Drawing.Point(9, 63);
            this.lblManaBonus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblManaBonus.Name = "lblManaBonus";
            this.lblManaBonus.Size = new System.Drawing.Size(37, 13);
            this.lblManaBonus.TabIndex = 44;
            this.lblManaBonus.Text = "Mana:";
            // 
            // lblHealthBonus
            // 
            this.lblHealthBonus.AutoSize = true;
            this.lblHealthBonus.Location = new System.Drawing.Point(8, 21);
            this.lblHealthBonus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHealthBonus.Name = "lblHealthBonus";
            this.lblHealthBonus.Size = new System.Drawing.Size(41, 13);
            this.lblHealthBonus.TabIndex = 43;
            this.lblHealthBonus.Text = "Health:";
            // 
            // cmbEquipmentAnimation
            // 
            this.cmbEquipmentAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEquipmentAnimation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbEquipmentAnimation.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbEquipmentAnimation.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbEquipmentAnimation.DrawDropdownHoverOutline = false;
            this.cmbEquipmentAnimation.DrawFocusRectangle = false;
            this.cmbEquipmentAnimation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEquipmentAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquipmentAnimation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEquipmentAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbEquipmentAnimation.FormattingEnabled = true;
            this.cmbEquipmentAnimation.Items.AddRange(new object[] {
            "None"});
            this.cmbEquipmentAnimation.Location = new System.Drawing.Point(221, 509);
            this.cmbEquipmentAnimation.Name = "cmbEquipmentAnimation";
            this.cmbEquipmentAnimation.Size = new System.Drawing.Size(207, 21);
            this.cmbEquipmentAnimation.TabIndex = 57;
            this.cmbEquipmentAnimation.Text = "None";
            this.cmbEquipmentAnimation.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbEquipmentAnimation.SelectedIndexChanged += new System.EventHandler(this.cmbEquipmentAnimation_SelectedIndexChanged);
            // 
            // lblEquipmentAnimation
            // 
            this.lblEquipmentAnimation.AutoSize = true;
            this.lblEquipmentAnimation.Location = new System.Drawing.Point(218, 493);
            this.lblEquipmentAnimation.Name = "lblEquipmentAnimation";
            this.lblEquipmentAnimation.Size = new System.Drawing.Size(109, 13);
            this.lblEquipmentAnimation.TabIndex = 56;
            this.lblEquipmentAnimation.Text = "Equipment Animation:";
            // 
            // nudEffectPercent
            // 
            this.nudEffectPercent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudEffectPercent.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudEffectPercent.Location = new System.Drawing.Point(103, 510);
            this.nudEffectPercent.Name = "nudEffectPercent";
            this.nudEffectPercent.Size = new System.Drawing.Size(114, 20);
            this.nudEffectPercent.TabIndex = 55;
            this.nudEffectPercent.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudEffectPercent.ValueChanged += new System.EventHandler(this.nudEffectPercent_ValueChanged);
            // 
            // grpStatBonuses
            // 
            this.grpStatBonuses.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpStatBonuses.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpStatBonuses.Controls.Add(this.lblPercentage5);
            this.grpStatBonuses.Controls.Add(this.lblPercentage4);
            this.grpStatBonuses.Controls.Add(this.lblPercentage8);
            this.grpStatBonuses.Controls.Add(this.lblPercentage7);
            this.grpStatBonuses.Controls.Add(this.lblPercentage6);
            this.grpStatBonuses.Controls.Add(this.nudSpdPercentage);
            this.grpStatBonuses.Controls.Add(this.nudMRPercentage);
            this.grpStatBonuses.Controls.Add(this.nudDefPercentage);
            this.grpStatBonuses.Controls.Add(this.nudMagPercentage);
            this.grpStatBonuses.Controls.Add(this.nudStrPercentage);
            this.grpStatBonuses.Controls.Add(this.lblPlus5);
            this.grpStatBonuses.Controls.Add(this.lblPlus4);
            this.grpStatBonuses.Controls.Add(this.lblPlus8);
            this.grpStatBonuses.Controls.Add(this.lblPlus7);
            this.grpStatBonuses.Controls.Add(this.lblPlus6);
            this.grpStatBonuses.Controls.Add(this.nudRange);
            this.grpStatBonuses.Controls.Add(this.nudSpd);
            this.grpStatBonuses.Controls.Add(this.nudMR);
            this.grpStatBonuses.Controls.Add(this.nudDef);
            this.grpStatBonuses.Controls.Add(this.nudMag);
            this.grpStatBonuses.Controls.Add(this.nudStr);
            this.grpStatBonuses.Controls.Add(this.lblSpd);
            this.grpStatBonuses.Controls.Add(this.lblMR);
            this.grpStatBonuses.Controls.Add(this.lblDef);
            this.grpStatBonuses.Controls.Add(this.lblMag);
            this.grpStatBonuses.Controls.Add(this.lblStr);
            this.grpStatBonuses.Controls.Add(this.lblRange);
            this.grpStatBonuses.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpStatBonuses.Location = new System.Drawing.Point(13, 150);
            this.grpStatBonuses.Name = "grpStatBonuses";
            this.grpStatBonuses.Size = new System.Drawing.Size(202, 234);
            this.grpStatBonuses.TabIndex = 40;
            this.grpStatBonuses.TabStop = false;
            this.grpStatBonuses.Text = "Stat Bonuses";
            // 
            // lblPercentage5
            // 
            this.lblPercentage5.AutoSize = true;
            this.lblPercentage5.Location = new System.Drawing.Point(179, 180);
            this.lblPercentage5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPercentage5.Name = "lblPercentage5";
            this.lblPercentage5.Size = new System.Drawing.Size(15, 13);
            this.lblPercentage5.TabIndex = 82;
            this.lblPercentage5.Text = "%";
            // 
            // lblPercentage4
            // 
            this.lblPercentage4.AutoSize = true;
            this.lblPercentage4.Location = new System.Drawing.Point(179, 142);
            this.lblPercentage4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPercentage4.Name = "lblPercentage4";
            this.lblPercentage4.Size = new System.Drawing.Size(15, 13);
            this.lblPercentage4.TabIndex = 81;
            this.lblPercentage4.Text = "%";
            // 
            // lblPercentage8
            // 
            this.lblPercentage8.AutoSize = true;
            this.lblPercentage8.Location = new System.Drawing.Point(181, 102);
            this.lblPercentage8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPercentage8.Name = "lblPercentage8";
            this.lblPercentage8.Size = new System.Drawing.Size(15, 13);
            this.lblPercentage8.TabIndex = 80;
            this.lblPercentage8.Text = "%";
            // 
            // lblPercentage7
            // 
            this.lblPercentage7.AutoSize = true;
            this.lblPercentage7.Location = new System.Drawing.Point(179, 64);
            this.lblPercentage7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPercentage7.Name = "lblPercentage7";
            this.lblPercentage7.Size = new System.Drawing.Size(15, 13);
            this.lblPercentage7.TabIndex = 79;
            this.lblPercentage7.Text = "%";
            // 
            // lblPercentage6
            // 
            this.lblPercentage6.AutoSize = true;
            this.lblPercentage6.Location = new System.Drawing.Point(179, 30);
            this.lblPercentage6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPercentage6.Name = "lblPercentage6";
            this.lblPercentage6.Size = new System.Drawing.Size(15, 13);
            this.lblPercentage6.TabIndex = 78;
            this.lblPercentage6.Text = "%";
            // 
            // nudSpdPercentage
            // 
            this.nudSpdPercentage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudSpdPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudSpdPercentage.Location = new System.Drawing.Point(132, 178);
            this.nudSpdPercentage.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudSpdPercentage.Name = "nudSpdPercentage";
            this.nudSpdPercentage.Size = new System.Drawing.Size(43, 20);
            this.nudSpdPercentage.TabIndex = 77;
            this.nudSpdPercentage.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudSpdPercentage.ValueChanged += new System.EventHandler(this.nudSpdPercentage_ValueChanged);
            // 
            // nudMRPercentage
            // 
            this.nudMRPercentage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudMRPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudMRPercentage.Location = new System.Drawing.Point(132, 140);
            this.nudMRPercentage.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMRPercentage.Name = "nudMRPercentage";
            this.nudMRPercentage.Size = new System.Drawing.Size(43, 20);
            this.nudMRPercentage.TabIndex = 76;
            this.nudMRPercentage.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMRPercentage.ValueChanged += new System.EventHandler(this.nudMRPercentage_ValueChanged);
            // 
            // nudDefPercentage
            // 
            this.nudDefPercentage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudDefPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudDefPercentage.Location = new System.Drawing.Point(134, 101);
            this.nudDefPercentage.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudDefPercentage.Name = "nudDefPercentage";
            this.nudDefPercentage.Size = new System.Drawing.Size(43, 20);
            this.nudDefPercentage.TabIndex = 75;
            this.nudDefPercentage.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudDefPercentage.ValueChanged += new System.EventHandler(this.nudDefPercentage_ValueChanged);
            // 
            // nudMagPercentage
            // 
            this.nudMagPercentage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudMagPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudMagPercentage.Location = new System.Drawing.Point(132, 63);
            this.nudMagPercentage.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMagPercentage.Name = "nudMagPercentage";
            this.nudMagPercentage.Size = new System.Drawing.Size(43, 20);
            this.nudMagPercentage.TabIndex = 74;
            this.nudMagPercentage.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMagPercentage.ValueChanged += new System.EventHandler(this.nudMagPercentage_ValueChanged);
            // 
            // nudStrPercentage
            // 
            this.nudStrPercentage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudStrPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudStrPercentage.Location = new System.Drawing.Point(132, 28);
            this.nudStrPercentage.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudStrPercentage.Name = "nudStrPercentage";
            this.nudStrPercentage.Size = new System.Drawing.Size(43, 20);
            this.nudStrPercentage.TabIndex = 73;
            this.nudStrPercentage.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudStrPercentage.ValueChanged += new System.EventHandler(this.nudStrPercentage_ValueChanged);
            // 
            // lblPlus5
            // 
            this.lblPlus5.AutoSize = true;
            this.lblPlus5.Location = new System.Drawing.Point(114, 180);
            this.lblPlus5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlus5.Name = "lblPlus5";
            this.lblPlus5.Size = new System.Drawing.Size(13, 13);
            this.lblPlus5.TabIndex = 72;
            this.lblPlus5.Text = "+";
            // 
            // lblPlus4
            // 
            this.lblPlus4.AutoSize = true;
            this.lblPlus4.Location = new System.Drawing.Point(114, 142);
            this.lblPlus4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlus4.Name = "lblPlus4";
            this.lblPlus4.Size = new System.Drawing.Size(13, 13);
            this.lblPlus4.TabIndex = 71;
            this.lblPlus4.Text = "+";
            // 
            // lblPlus8
            // 
            this.lblPlus8.AutoSize = true;
            this.lblPlus8.Location = new System.Drawing.Point(116, 102);
            this.lblPlus8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlus8.Name = "lblPlus8";
            this.lblPlus8.Size = new System.Drawing.Size(13, 13);
            this.lblPlus8.TabIndex = 70;
            this.lblPlus8.Text = "+";
            // 
            // lblPlus7
            // 
            this.lblPlus7.AutoSize = true;
            this.lblPlus7.Location = new System.Drawing.Point(114, 64);
            this.lblPlus7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlus7.Name = "lblPlus7";
            this.lblPlus7.Size = new System.Drawing.Size(13, 13);
            this.lblPlus7.TabIndex = 69;
            this.lblPlus7.Text = "+";
            // 
            // lblPlus6
            // 
            this.lblPlus6.AutoSize = true;
            this.lblPlus6.Location = new System.Drawing.Point(114, 30);
            this.lblPlus6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlus6.Name = "lblPlus6";
            this.lblPlus6.Size = new System.Drawing.Size(13, 13);
            this.lblPlus6.TabIndex = 68;
            this.lblPlus6.Text = "+";
            // 
            // nudRange
            // 
            this.nudRange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudRange.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudRange.Location = new System.Drawing.Point(132, 208);
            this.nudRange.Name = "nudRange";
            this.nudRange.Size = new System.Drawing.Size(62, 20);
            this.nudRange.TabIndex = 53;
            this.nudRange.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudRange.ValueChanged += new System.EventHandler(this.nudRange_ValueChanged);
            // 
            // nudSpd
            // 
            this.nudSpd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudSpd.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudSpd.Location = new System.Drawing.Point(14, 178);
            this.nudSpd.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudSpd.Name = "nudSpd";
            this.nudSpd.Size = new System.Drawing.Size(97, 20);
            this.nudSpd.TabIndex = 52;
            this.nudSpd.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudSpd.ValueChanged += new System.EventHandler(this.nudSpd_ValueChanged);
            // 
            // nudMR
            // 
            this.nudMR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudMR.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudMR.Location = new System.Drawing.Point(13, 140);
            this.nudMR.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudMR.Name = "nudMR";
            this.nudMR.Size = new System.Drawing.Size(98, 20);
            this.nudMR.TabIndex = 51;
            this.nudMR.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMR.ValueChanged += new System.EventHandler(this.nudMR_ValueChanged);
            // 
            // nudDef
            // 
            this.nudDef.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudDef.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudDef.Location = new System.Drawing.Point(13, 101);
            this.nudDef.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDef.Name = "nudDef";
            this.nudDef.Size = new System.Drawing.Size(98, 20);
            this.nudDef.TabIndex = 50;
            this.nudDef.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudDef.ValueChanged += new System.EventHandler(this.nudDef_ValueChanged);
            // 
            // nudMag
            // 
            this.nudMag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudMag.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudMag.Location = new System.Drawing.Point(13, 63);
            this.nudMag.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudMag.Name = "nudMag";
            this.nudMag.Size = new System.Drawing.Size(98, 20);
            this.nudMag.TabIndex = 49;
            this.nudMag.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudMag.ValueChanged += new System.EventHandler(this.nudMag_ValueChanged);
            // 
            // nudStr
            // 
            this.nudStr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudStr.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudStr.Location = new System.Drawing.Point(13, 28);
            this.nudStr.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudStr.Name = "nudStr";
            this.nudStr.Size = new System.Drawing.Size(98, 20);
            this.nudStr.TabIndex = 48;
            this.nudStr.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudStr.ValueChanged += new System.EventHandler(this.nudStr_ValueChanged);
            // 
            // lblSpd
            // 
            this.lblSpd.AutoSize = true;
            this.lblSpd.Location = new System.Drawing.Point(11, 162);
            this.lblSpd.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSpd.Name = "lblSpd";
            this.lblSpd.Size = new System.Drawing.Size(41, 13);
            this.lblSpd.TabIndex = 47;
            this.lblSpd.Text = "Speed:";
            // 
            // lblMR
            // 
            this.lblMR.AutoSize = true;
            this.lblMR.Location = new System.Drawing.Point(10, 124);
            this.lblMR.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMR.Name = "lblMR";
            this.lblMR.Size = new System.Drawing.Size(71, 13);
            this.lblMR.TabIndex = 46;
            this.lblMR.Text = "Magic Resist:";
            // 
            // lblDef
            // 
            this.lblDef.AutoSize = true;
            this.lblDef.Location = new System.Drawing.Point(10, 85);
            this.lblDef.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDef.Name = "lblDef";
            this.lblDef.Size = new System.Drawing.Size(37, 13);
            this.lblDef.TabIndex = 45;
            this.lblDef.Text = "Armor:";
            // 
            // lblMag
            // 
            this.lblMag.AutoSize = true;
            this.lblMag.Location = new System.Drawing.Point(10, 50);
            this.lblMag.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMag.Name = "lblMag";
            this.lblMag.Size = new System.Drawing.Size(39, 13);
            this.lblMag.TabIndex = 44;
            this.lblMag.Text = "Magic:";
            // 
            // lblStr
            // 
            this.lblStr.AutoSize = true;
            this.lblStr.Location = new System.Drawing.Point(11, 12);
            this.lblStr.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStr.Name = "lblStr";
            this.lblStr.Size = new System.Drawing.Size(50, 13);
            this.lblStr.TabIndex = 43;
            this.lblStr.Text = "Strength:";
            // 
            // lblRange
            // 
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(11, 210);
            this.lblRange.Name = "lblRange";
            this.lblRange.Size = new System.Drawing.Size(115, 13);
            this.lblRange.TabIndex = 20;
            this.lblRange.Text = "Stat Bonus Range (+-):";
            // 
            // cmbFemalePaperdoll
            // 
            this.cmbFemalePaperdoll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbFemalePaperdoll.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbFemalePaperdoll.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbFemalePaperdoll.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbFemalePaperdoll.DrawDropdownHoverOutline = false;
            this.cmbFemalePaperdoll.DrawFocusRectangle = false;
            this.cmbFemalePaperdoll.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbFemalePaperdoll.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFemalePaperdoll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbFemalePaperdoll.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbFemalePaperdoll.FormattingEnabled = true;
            this.cmbFemalePaperdoll.Items.AddRange(new object[] {
            "None"});
            this.cmbFemalePaperdoll.Location = new System.Drawing.Point(221, 550);
            this.cmbFemalePaperdoll.Name = "cmbFemalePaperdoll";
            this.cmbFemalePaperdoll.Size = new System.Drawing.Size(168, 21);
            this.cmbFemalePaperdoll.TabIndex = 36;
            this.cmbFemalePaperdoll.Text = "None";
            this.cmbFemalePaperdoll.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbFemalePaperdoll.SelectedIndexChanged += new System.EventHandler(this.cmbFemalePaperdoll_SelectedIndexChanged);
            // 
            // lblFemalePaperdoll
            // 
            this.lblFemalePaperdoll.AutoSize = true;
            this.lblFemalePaperdoll.Location = new System.Drawing.Point(218, 533);
            this.lblFemalePaperdoll.Name = "lblFemalePaperdoll";
            this.lblFemalePaperdoll.Size = new System.Drawing.Size(91, 13);
            this.lblFemalePaperdoll.TabIndex = 35;
            this.lblFemalePaperdoll.Text = "Female Paperdoll:";
            // 
            // picFemalePaperdoll
            // 
            this.picFemalePaperdoll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picFemalePaperdoll.Location = new System.Drawing.Point(221, 577);
            this.picFemalePaperdoll.Name = "picFemalePaperdoll";
            this.picFemalePaperdoll.Size = new System.Drawing.Size(200, 156);
            this.picFemalePaperdoll.TabIndex = 34;
            this.picFemalePaperdoll.TabStop = false;
            // 
            // lblEffectPercent
            // 
            this.lblEffectPercent.AutoSize = true;
            this.lblEffectPercent.Location = new System.Drawing.Point(10, 512);
            this.lblEffectPercent.Name = "lblEffectPercent";
            this.lblEffectPercent.Size = new System.Drawing.Size(94, 13);
            this.lblEffectPercent.TabIndex = 31;
            this.lblEffectPercent.Text = "Effect Amount (%):";
            // 
            // cmbEquipmentBonus
            // 
            this.cmbEquipmentBonus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEquipmentBonus.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbEquipmentBonus.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbEquipmentBonus.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbEquipmentBonus.DrawDropdownHoverOutline = false;
            this.cmbEquipmentBonus.DrawFocusRectangle = false;
            this.cmbEquipmentBonus.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEquipmentBonus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquipmentBonus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEquipmentBonus.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbEquipmentBonus.FormattingEnabled = true;
            this.cmbEquipmentBonus.Location = new System.Drawing.Point(103, 483);
            this.cmbEquipmentBonus.Name = "cmbEquipmentBonus";
            this.cmbEquipmentBonus.Size = new System.Drawing.Size(114, 21);
            this.cmbEquipmentBonus.TabIndex = 29;
            this.cmbEquipmentBonus.Text = null;
            this.cmbEquipmentBonus.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbEquipmentBonus.SelectedIndexChanged += new System.EventHandler(this.cmbEquipmentBonus_SelectedIndexChanged);
            // 
            // lblBonusEffect
            // 
            this.lblBonusEffect.AutoSize = true;
            this.lblBonusEffect.Location = new System.Drawing.Point(10, 486);
            this.lblBonusEffect.Name = "lblBonusEffect";
            this.lblBonusEffect.Size = new System.Drawing.Size(71, 13);
            this.lblBonusEffect.TabIndex = 28;
            this.lblBonusEffect.Text = "Bonus Effect:";
            // 
            // cmbEquipmentSlot
            // 
            this.cmbEquipmentSlot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEquipmentSlot.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbEquipmentSlot.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbEquipmentSlot.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbEquipmentSlot.DrawDropdownHoverOutline = false;
            this.cmbEquipmentSlot.DrawFocusRectangle = false;
            this.cmbEquipmentSlot.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEquipmentSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquipmentSlot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEquipmentSlot.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbEquipmentSlot.FormattingEnabled = true;
            this.cmbEquipmentSlot.Location = new System.Drawing.Point(103, 19);
            this.cmbEquipmentSlot.Name = "cmbEquipmentSlot";
            this.cmbEquipmentSlot.Size = new System.Drawing.Size(111, 21);
            this.cmbEquipmentSlot.TabIndex = 24;
            this.cmbEquipmentSlot.Text = null;
            this.cmbEquipmentSlot.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbEquipmentSlot.SelectedIndexChanged += new System.EventHandler(this.cmbEquipmentSlot_SelectedIndexChanged);
            // 
            // lblEquipmentSlot
            // 
            this.lblEquipmentSlot.AutoSize = true;
            this.lblEquipmentSlot.Location = new System.Drawing.Point(12, 23);
            this.lblEquipmentSlot.Name = "lblEquipmentSlot";
            this.lblEquipmentSlot.Size = new System.Drawing.Size(81, 13);
            this.lblEquipmentSlot.TabIndex = 23;
            this.lblEquipmentSlot.Text = "Equipment Slot:";
            // 
            // cmbMalePaperdoll
            // 
            this.cmbMalePaperdoll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbMalePaperdoll.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMalePaperdoll.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMalePaperdoll.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbMalePaperdoll.DrawDropdownHoverOutline = false;
            this.cmbMalePaperdoll.DrawFocusRectangle = false;
            this.cmbMalePaperdoll.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMalePaperdoll.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMalePaperdoll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMalePaperdoll.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbMalePaperdoll.FormattingEnabled = true;
            this.cmbMalePaperdoll.Items.AddRange(new object[] {
            "None"});
            this.cmbMalePaperdoll.Location = new System.Drawing.Point(11, 550);
            this.cmbMalePaperdoll.Name = "cmbMalePaperdoll";
            this.cmbMalePaperdoll.Size = new System.Drawing.Size(168, 21);
            this.cmbMalePaperdoll.TabIndex = 22;
            this.cmbMalePaperdoll.Text = "None";
            this.cmbMalePaperdoll.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbMalePaperdoll.SelectedIndexChanged += new System.EventHandler(this.cmbPaperdoll_SelectedIndexChanged);
            // 
            // lblMalePaperdoll
            // 
            this.lblMalePaperdoll.AutoSize = true;
            this.lblMalePaperdoll.Location = new System.Drawing.Point(8, 533);
            this.lblMalePaperdoll.Name = "lblMalePaperdoll";
            this.lblMalePaperdoll.Size = new System.Drawing.Size(80, 13);
            this.lblMalePaperdoll.TabIndex = 21;
            this.lblMalePaperdoll.Text = "Male Paperdoll:";
            // 
            // picMalePaperdoll
            // 
            this.picMalePaperdoll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picMalePaperdoll.Location = new System.Drawing.Point(11, 577);
            this.picMalePaperdoll.Name = "picMalePaperdoll";
            this.picMalePaperdoll.Size = new System.Drawing.Size(200, 156);
            this.picMalePaperdoll.TabIndex = 16;
            this.picMalePaperdoll.TabStop = false;
            // 
            // grpWeaponProperties
            // 
            this.grpWeaponProperties.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpWeaponProperties.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpWeaponProperties.Controls.Add(this.nudCritMultiplier);
            this.grpWeaponProperties.Controls.Add(this.lblCritMultiplier);
            this.grpWeaponProperties.Controls.Add(this.grpAttackSpeed);
            this.grpWeaponProperties.Controls.Add(this.nudScaling);
            this.grpWeaponProperties.Controls.Add(this.nudCritChance);
            this.grpWeaponProperties.Controls.Add(this.nudDamage);
            this.grpWeaponProperties.Controls.Add(this.cmbProjectile);
            this.grpWeaponProperties.Controls.Add(this.cmbScalingStat);
            this.grpWeaponProperties.Controls.Add(this.lblScalingStat);
            this.grpWeaponProperties.Controls.Add(this.lblScalingAmount);
            this.grpWeaponProperties.Controls.Add(this.cmbDamageType);
            this.grpWeaponProperties.Controls.Add(this.lblDamageType);
            this.grpWeaponProperties.Controls.Add(this.lblCritChance);
            this.grpWeaponProperties.Controls.Add(this.cmbAttackAnimation);
            this.grpWeaponProperties.Controls.Add(this.lblAttackAnimation);
            this.grpWeaponProperties.Controls.Add(this.chk2Hand);
            this.grpWeaponProperties.Controls.Add(this.lblToolType);
            this.grpWeaponProperties.Controls.Add(this.cmbToolType);
            this.grpWeaponProperties.Controls.Add(this.lblProjectile);
            this.grpWeaponProperties.Controls.Add(this.lblDamage);
            this.grpWeaponProperties.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpWeaponProperties.Location = new System.Drawing.Point(221, 14);
            this.grpWeaponProperties.Name = "grpWeaponProperties";
            this.grpWeaponProperties.Size = new System.Drawing.Size(207, 463);
            this.grpWeaponProperties.TabIndex = 39;
            this.grpWeaponProperties.TabStop = false;
            this.grpWeaponProperties.Text = "Weapon Properties";
            this.grpWeaponProperties.Visible = false;
            // 
            // nudCritMultiplier
            // 
            this.nudCritMultiplier.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudCritMultiplier.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudCritMultiplier.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudCritMultiplier.Location = new System.Drawing.Point(15, 111);
            this.nudCritMultiplier.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudCritMultiplier.Name = "nudCritMultiplier";
            this.nudCritMultiplier.Size = new System.Drawing.Size(180, 20);
            this.nudCritMultiplier.TabIndex = 58;
            this.nudCritMultiplier.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudCritMultiplier.ValueChanged += new System.EventHandler(this.nudCritMultiplier_ValueChanged);
            // 
            // lblCritMultiplier
            // 
            this.lblCritMultiplier.AutoSize = true;
            this.lblCritMultiplier.Location = new System.Drawing.Point(12, 97);
            this.lblCritMultiplier.Name = "lblCritMultiplier";
            this.lblCritMultiplier.Size = new System.Drawing.Size(135, 13);
            this.lblCritMultiplier.TabIndex = 57;
            this.lblCritMultiplier.Text = "Crit Multiplier (Default 1.5x):";
            // 
            // grpAttackSpeed
            // 
            this.grpAttackSpeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpAttackSpeed.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpAttackSpeed.Controls.Add(this.nudAttackSpeedValue);
            this.grpAttackSpeed.Controls.Add(this.lblAttackSpeedValue);
            this.grpAttackSpeed.Controls.Add(this.cmbAttackSpeedModifier);
            this.grpAttackSpeed.Controls.Add(this.lblAttackSpeedModifier);
            this.grpAttackSpeed.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpAttackSpeed.Location = new System.Drawing.Point(15, 371);
            this.grpAttackSpeed.Name = "grpAttackSpeed";
            this.grpAttackSpeed.Size = new System.Drawing.Size(180, 86);
            this.grpAttackSpeed.TabIndex = 56;
            this.grpAttackSpeed.TabStop = false;
            this.grpAttackSpeed.Text = "Attack Speed";
            // 
            // nudAttackSpeedValue
            // 
            this.nudAttackSpeedValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudAttackSpeedValue.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudAttackSpeedValue.Location = new System.Drawing.Point(60, 58);
            this.nudAttackSpeedValue.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudAttackSpeedValue.Name = "nudAttackSpeedValue";
            this.nudAttackSpeedValue.Size = new System.Drawing.Size(114, 20);
            this.nudAttackSpeedValue.TabIndex = 56;
            this.nudAttackSpeedValue.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudAttackSpeedValue.ValueChanged += new System.EventHandler(this.nudAttackSpeedValue_ValueChanged);
            // 
            // lblAttackSpeedValue
            // 
            this.lblAttackSpeedValue.AutoSize = true;
            this.lblAttackSpeedValue.Location = new System.Drawing.Point(9, 60);
            this.lblAttackSpeedValue.Name = "lblAttackSpeedValue";
            this.lblAttackSpeedValue.Size = new System.Drawing.Size(37, 13);
            this.lblAttackSpeedValue.TabIndex = 29;
            this.lblAttackSpeedValue.Text = "Value:";
            // 
            // cmbAttackSpeedModifier
            // 
            this.cmbAttackSpeedModifier.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAttackSpeedModifier.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAttackSpeedModifier.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAttackSpeedModifier.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbAttackSpeedModifier.DrawDropdownHoverOutline = false;
            this.cmbAttackSpeedModifier.DrawFocusRectangle = false;
            this.cmbAttackSpeedModifier.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAttackSpeedModifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAttackSpeedModifier.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAttackSpeedModifier.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbAttackSpeedModifier.FormattingEnabled = true;
            this.cmbAttackSpeedModifier.Location = new System.Drawing.Point(60, 24);
            this.cmbAttackSpeedModifier.Name = "cmbAttackSpeedModifier";
            this.cmbAttackSpeedModifier.Size = new System.Drawing.Size(114, 21);
            this.cmbAttackSpeedModifier.TabIndex = 28;
            this.cmbAttackSpeedModifier.Text = null;
            this.cmbAttackSpeedModifier.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbAttackSpeedModifier.SelectedIndexChanged += new System.EventHandler(this.cmbAttackSpeedModifier_SelectedIndexChanged);
            // 
            // lblAttackSpeedModifier
            // 
            this.lblAttackSpeedModifier.AutoSize = true;
            this.lblAttackSpeedModifier.Location = new System.Drawing.Point(9, 27);
            this.lblAttackSpeedModifier.Name = "lblAttackSpeedModifier";
            this.lblAttackSpeedModifier.Size = new System.Drawing.Size(47, 13);
            this.lblAttackSpeedModifier.TabIndex = 0;
            this.lblAttackSpeedModifier.Text = "Modifier:";
            // 
            // nudScaling
            // 
            this.nudScaling.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudScaling.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudScaling.Location = new System.Drawing.Point(15, 234);
            this.nudScaling.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudScaling.Name = "nudScaling";
            this.nudScaling.Size = new System.Drawing.Size(180, 20);
            this.nudScaling.TabIndex = 55;
            this.nudScaling.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudScaling.ValueChanged += new System.EventHandler(this.nudScaling_ValueChanged);
            // 
            // nudCritChance
            // 
            this.nudCritChance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudCritChance.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudCritChance.Location = new System.Drawing.Point(15, 73);
            this.nudCritChance.Name = "nudCritChance";
            this.nudCritChance.Size = new System.Drawing.Size(180, 20);
            this.nudCritChance.TabIndex = 54;
            this.nudCritChance.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudCritChance.ValueChanged += new System.EventHandler(this.nudCritChance_ValueChanged);
            // 
            // nudDamage
            // 
            this.nudDamage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudDamage.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudDamage.Location = new System.Drawing.Point(15, 36);
            this.nudDamage.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudDamage.Name = "nudDamage";
            this.nudDamage.Size = new System.Drawing.Size(180, 20);
            this.nudDamage.TabIndex = 49;
            this.nudDamage.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudDamage.ValueChanged += new System.EventHandler(this.nudDamage_ValueChanged);
            // 
            // cmbProjectile
            // 
            this.cmbProjectile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbProjectile.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbProjectile.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbProjectile.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbProjectile.DrawDropdownHoverOutline = false;
            this.cmbProjectile.DrawFocusRectangle = false;
            this.cmbProjectile.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbProjectile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProjectile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbProjectile.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbProjectile.FormattingEnabled = true;
            this.cmbProjectile.Location = new System.Drawing.Point(15, 270);
            this.cmbProjectile.Name = "cmbProjectile";
            this.cmbProjectile.Size = new System.Drawing.Size(180, 21);
            this.cmbProjectile.TabIndex = 47;
            this.cmbProjectile.Text = null;
            this.cmbProjectile.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbProjectile.SelectedIndexChanged += new System.EventHandler(this.cmbProjectile_SelectedIndexChanged);
            // 
            // cmbScalingStat
            // 
            this.cmbScalingStat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbScalingStat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbScalingStat.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbScalingStat.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbScalingStat.DrawDropdownHoverOutline = false;
            this.cmbScalingStat.DrawFocusRectangle = false;
            this.cmbScalingStat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbScalingStat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScalingStat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbScalingStat.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbScalingStat.FormattingEnabled = true;
            this.cmbScalingStat.Location = new System.Drawing.Point(16, 193);
            this.cmbScalingStat.Name = "cmbScalingStat";
            this.cmbScalingStat.Size = new System.Drawing.Size(180, 21);
            this.cmbScalingStat.TabIndex = 46;
            this.cmbScalingStat.Text = null;
            this.cmbScalingStat.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbScalingStat.SelectedIndexChanged += new System.EventHandler(this.cmbScalingStat_SelectedIndexChanged);
            // 
            // lblScalingStat
            // 
            this.lblScalingStat.AutoSize = true;
            this.lblScalingStat.Location = new System.Drawing.Point(13, 176);
            this.lblScalingStat.Name = "lblScalingStat";
            this.lblScalingStat.Size = new System.Drawing.Size(67, 13);
            this.lblScalingStat.TabIndex = 45;
            this.lblScalingStat.Text = "Scaling Stat:";
            // 
            // lblScalingAmount
            // 
            this.lblScalingAmount.AutoSize = true;
            this.lblScalingAmount.Location = new System.Drawing.Point(12, 219);
            this.lblScalingAmount.Name = "lblScalingAmount";
            this.lblScalingAmount.Size = new System.Drawing.Size(84, 13);
            this.lblScalingAmount.TabIndex = 44;
            this.lblScalingAmount.Text = "Scaling Amount:";
            // 
            // cmbDamageType
            // 
            this.cmbDamageType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDamageType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDamageType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDamageType.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbDamageType.DrawDropdownHoverOutline = false;
            this.cmbDamageType.DrawFocusRectangle = false;
            this.cmbDamageType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDamageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDamageType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDamageType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbDamageType.FormattingEnabled = true;
            this.cmbDamageType.Items.AddRange(new object[] {
            "Physical",
            "Magic",
            "True"});
            this.cmbDamageType.Location = new System.Drawing.Point(16, 152);
            this.cmbDamageType.Name = "cmbDamageType";
            this.cmbDamageType.Size = new System.Drawing.Size(180, 21);
            this.cmbDamageType.TabIndex = 42;
            this.cmbDamageType.Text = "Physical";
            this.cmbDamageType.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbDamageType.SelectedIndexChanged += new System.EventHandler(this.cmbDamageType_SelectedIndexChanged);
            // 
            // lblDamageType
            // 
            this.lblDamageType.AutoSize = true;
            this.lblDamageType.Location = new System.Drawing.Point(13, 135);
            this.lblDamageType.Name = "lblDamageType";
            this.lblDamageType.Size = new System.Drawing.Size(77, 13);
            this.lblDamageType.TabIndex = 41;
            this.lblDamageType.Text = "Damage Type:";
            // 
            // lblCritChance
            // 
            this.lblCritChance.AutoSize = true;
            this.lblCritChance.Location = new System.Drawing.Point(12, 59);
            this.lblCritChance.Name = "lblCritChance";
            this.lblCritChance.Size = new System.Drawing.Size(82, 13);
            this.lblCritChance.TabIndex = 40;
            this.lblCritChance.Text = "Crit Chance (%):";
            // 
            // cmbAttackAnimation
            // 
            this.cmbAttackAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAttackAnimation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAttackAnimation.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAttackAnimation.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbAttackAnimation.DrawDropdownHoverOutline = false;
            this.cmbAttackAnimation.DrawFocusRectangle = false;
            this.cmbAttackAnimation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAttackAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAttackAnimation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAttackAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbAttackAnimation.FormattingEnabled = true;
            this.cmbAttackAnimation.Location = new System.Drawing.Point(15, 308);
            this.cmbAttackAnimation.Name = "cmbAttackAnimation";
            this.cmbAttackAnimation.Size = new System.Drawing.Size(180, 21);
            this.cmbAttackAnimation.TabIndex = 38;
            this.cmbAttackAnimation.Text = null;
            this.cmbAttackAnimation.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbAttackAnimation.SelectedIndexChanged += new System.EventHandler(this.cmbAttackAnimation_SelectedIndexChanged);
            // 
            // lblAttackAnimation
            // 
            this.lblAttackAnimation.AutoSize = true;
            this.lblAttackAnimation.Location = new System.Drawing.Point(12, 293);
            this.lblAttackAnimation.Name = "lblAttackAnimation";
            this.lblAttackAnimation.Size = new System.Drawing.Size(90, 13);
            this.lblAttackAnimation.TabIndex = 37;
            this.lblAttackAnimation.Text = "Attack Animation:";
            // 
            // chk2Hand
            // 
            this.chk2Hand.AutoSize = true;
            this.chk2Hand.Location = new System.Drawing.Point(134, 14);
            this.chk2Hand.Name = "chk2Hand";
            this.chk2Hand.Size = new System.Drawing.Size(61, 17);
            this.chk2Hand.TabIndex = 25;
            this.chk2Hand.Text = "2 Hand";
            this.chk2Hand.CheckedChanged += new System.EventHandler(this.chk2Hand_CheckedChanged);
            // 
            // lblToolType
            // 
            this.lblToolType.AutoSize = true;
            this.lblToolType.Location = new System.Drawing.Point(13, 335);
            this.lblToolType.Name = "lblToolType";
            this.lblToolType.Size = new System.Drawing.Size(58, 13);
            this.lblToolType.TabIndex = 26;
            this.lblToolType.Text = "Tool Type:";
            // 
            // cmbToolType
            // 
            this.cmbToolType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbToolType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbToolType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbToolType.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbToolType.DrawDropdownHoverOutline = false;
            this.cmbToolType.DrawFocusRectangle = false;
            this.cmbToolType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbToolType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbToolType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbToolType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbToolType.FormattingEnabled = true;
            this.cmbToolType.Location = new System.Drawing.Point(16, 349);
            this.cmbToolType.Name = "cmbToolType";
            this.cmbToolType.Size = new System.Drawing.Size(180, 21);
            this.cmbToolType.TabIndex = 27;
            this.cmbToolType.Text = null;
            this.cmbToolType.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbToolType.SelectedIndexChanged += new System.EventHandler(this.cmbToolType_SelectedIndexChanged);
            // 
            // lblProjectile
            // 
            this.lblProjectile.AutoSize = true;
            this.lblProjectile.Location = new System.Drawing.Point(12, 254);
            this.lblProjectile.Name = "lblProjectile";
            this.lblProjectile.Size = new System.Drawing.Size(53, 13);
            this.lblProjectile.TabIndex = 33;
            this.lblProjectile.Text = "Projectile:";
            // 
            // lblDamage
            // 
            this.lblDamage.AutoSize = true;
            this.lblDamage.Location = new System.Drawing.Point(12, 19);
            this.lblDamage.Name = "lblDamage";
            this.lblDamage.Size = new System.Drawing.Size(77, 13);
            this.lblDamage.TabIndex = 11;
            this.lblDamage.Text = "Base Damage:";
            // 
            // grpEvent
            // 
            this.grpEvent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpEvent.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEvent.Controls.Add(this.chkSingleUseEvent);
            this.grpEvent.Controls.Add(this.cmbEvent);
            this.grpEvent.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEvent.Location = new System.Drawing.Point(2, 435);
            this.grpEvent.Name = "grpEvent";
            this.grpEvent.Size = new System.Drawing.Size(200, 65);
            this.grpEvent.TabIndex = 42;
            this.grpEvent.TabStop = false;
            this.grpEvent.Text = "Event";
            this.grpEvent.Visible = false;
            // 
            // chkSingleUseEvent
            // 
            this.chkSingleUseEvent.AutoSize = true;
            this.chkSingleUseEvent.Checked = true;
            this.chkSingleUseEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSingleUseEvent.Location = new System.Drawing.Point(9, 42);
            this.chkSingleUseEvent.Name = "chkSingleUseEvent";
            this.chkSingleUseEvent.Size = new System.Drawing.Size(107, 17);
            this.chkSingleUseEvent.TabIndex = 29;
            this.chkSingleUseEvent.Text = "Destroy On Use?";
            this.chkSingleUseEvent.CheckedChanged += new System.EventHandler(this.chkSingleUse_CheckedChanged);
            // 
            // cmbEvent
            // 
            this.cmbEvent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEvent.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbEvent.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbEvent.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbEvent.DrawDropdownHoverOutline = false;
            this.cmbEvent.DrawFocusRectangle = false;
            this.cmbEvent.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEvent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEvent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEvent.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbEvent.FormattingEnabled = true;
            this.cmbEvent.Location = new System.Drawing.Point(9, 15);
            this.cmbEvent.Name = "cmbEvent";
            this.cmbEvent.Size = new System.Drawing.Size(185, 21);
            this.cmbEvent.TabIndex = 17;
            this.cmbEvent.Text = null;
            this.cmbEvent.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbEvent.SelectedIndexChanged += new System.EventHandler(this.cmbEvent_SelectedIndexChanged);
            // 
            // grpConsumable
            // 
            this.grpConsumable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpConsumable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpConsumable.Controls.Add(this.lblPercentage3);
            this.grpConsumable.Controls.Add(this.nudIntervalPercentage);
            this.grpConsumable.Controls.Add(this.lblPlus3);
            this.grpConsumable.Controls.Add(this.nudInterval);
            this.grpConsumable.Controls.Add(this.lblVital);
            this.grpConsumable.Controls.Add(this.cmbConsume);
            this.grpConsumable.Controls.Add(this.lblInterval);
            this.grpConsumable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpConsumable.Location = new System.Drawing.Point(2, 436);
            this.grpConsumable.Name = "grpConsumable";
            this.grpConsumable.Size = new System.Drawing.Size(217, 125);
            this.grpConsumable.TabIndex = 12;
            this.grpConsumable.TabStop = false;
            this.grpConsumable.Text = "Consumable";
            this.grpConsumable.Visible = false;
            // 
            // lblPercentage3
            // 
            this.lblPercentage3.AutoSize = true;
            this.lblPercentage3.Location = new System.Drawing.Point(195, 91);
            this.lblPercentage3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPercentage3.Name = "lblPercentage3";
            this.lblPercentage3.Size = new System.Drawing.Size(15, 13);
            this.lblPercentage3.TabIndex = 73;
            this.lblPercentage3.Text = "%";
            // 
            // nudIntervalPercentage
            // 
            this.nudIntervalPercentage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudIntervalPercentage.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudIntervalPercentage.Location = new System.Drawing.Point(148, 90);
            this.nudIntervalPercentage.Name = "nudIntervalPercentage";
            this.nudIntervalPercentage.Size = new System.Drawing.Size(43, 20);
            this.nudIntervalPercentage.TabIndex = 72;
            this.nudIntervalPercentage.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudIntervalPercentage.ValueChanged += new System.EventHandler(this.nudIntervalPercentage_ValueChanged);
            // 
            // lblPlus3
            // 
            this.lblPlus3.AutoSize = true;
            this.lblPlus3.Location = new System.Drawing.Point(130, 91);
            this.lblPlus3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPlus3.Name = "lblPlus3";
            this.lblPlus3.Size = new System.Drawing.Size(13, 13);
            this.lblPlus3.TabIndex = 71;
            this.lblPlus3.Text = "+";
            // 
            // nudInterval
            // 
            this.nudInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudInterval.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudInterval.Location = new System.Drawing.Point(19, 90);
            this.nudInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudInterval.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudInterval.Name = "nudInterval";
            this.nudInterval.Size = new System.Drawing.Size(106, 20);
            this.nudInterval.TabIndex = 37;
            this.nudInterval.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudInterval.ValueChanged += new System.EventHandler(this.nudInterval_ValueChanged);
            // 
            // lblVital
            // 
            this.lblVital.AutoSize = true;
            this.lblVital.Location = new System.Drawing.Point(16, 17);
            this.lblVital.Name = "lblVital";
            this.lblVital.Size = new System.Drawing.Size(30, 13);
            this.lblVital.TabIndex = 12;
            this.lblVital.Text = "Vital:";
            // 
            // cmbConsume
            // 
            this.cmbConsume.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbConsume.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbConsume.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbConsume.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbConsume.DrawDropdownHoverOutline = false;
            this.cmbConsume.DrawFocusRectangle = false;
            this.cmbConsume.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbConsume.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConsume.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbConsume.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbConsume.FormattingEnabled = true;
            this.cmbConsume.Items.AddRange(new object[] {
            "Health",
            "Mana",
            "Experience"});
            this.cmbConsume.Location = new System.Drawing.Point(19, 37);
            this.cmbConsume.Name = "cmbConsume";
            this.cmbConsume.Size = new System.Drawing.Size(176, 21);
            this.cmbConsume.TabIndex = 11;
            this.cmbConsume.Text = "Health";
            this.cmbConsume.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbConsume.SelectedIndexChanged += new System.EventHandler(this.cmbConsume_SelectedIndexChanged);
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(19, 71);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(45, 13);
            this.lblInterval.TabIndex = 9;
            this.lblInterval.Text = "Interval:";
            // 
            // grpSpell
            // 
            this.grpSpell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpSpell.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSpell.Controls.Add(this.chkSingleUseSpell);
            this.grpSpell.Controls.Add(this.chkQuickCast);
            this.grpSpell.Controls.Add(this.cmbTeachSpell);
            this.grpSpell.Controls.Add(this.lblSpell);
            this.grpSpell.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSpell.Location = new System.Drawing.Point(2, 436);
            this.grpSpell.Name = "grpSpell";
            this.grpSpell.Size = new System.Drawing.Size(217, 127);
            this.grpSpell.TabIndex = 13;
            this.grpSpell.TabStop = false;
            this.grpSpell.Text = "Spell";
            this.grpSpell.Visible = false;
            // 
            // chkSingleUseSpell
            // 
            this.chkSingleUseSpell.AutoSize = true;
            this.chkSingleUseSpell.Checked = true;
            this.chkSingleUseSpell.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSingleUseSpell.Location = new System.Drawing.Point(15, 95);
            this.chkSingleUseSpell.Name = "chkSingleUseSpell";
            this.chkSingleUseSpell.Size = new System.Drawing.Size(107, 17);
            this.chkSingleUseSpell.TabIndex = 29;
            this.chkSingleUseSpell.Text = "Destroy On Use?";
            this.chkSingleUseSpell.CheckedChanged += new System.EventHandler(this.chkSingleUse_CheckedChanged);
            // 
            // chkQuickCast
            // 
            this.chkQuickCast.AutoSize = true;
            this.chkQuickCast.Location = new System.Drawing.Point(15, 72);
            this.chkQuickCast.Name = "chkQuickCast";
            this.chkQuickCast.Size = new System.Drawing.Size(110, 17);
            this.chkQuickCast.TabIndex = 28;
            this.chkQuickCast.Text = "Quick Cast Spell?";
            this.chkQuickCast.CheckedChanged += new System.EventHandler(this.chkQuickCast_CheckedChanged);
            // 
            // cmbTeachSpell
            // 
            this.cmbTeachSpell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTeachSpell.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTeachSpell.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTeachSpell.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbTeachSpell.DrawDropdownHoverOutline = false;
            this.cmbTeachSpell.DrawFocusRectangle = false;
            this.cmbTeachSpell.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTeachSpell.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTeachSpell.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTeachSpell.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTeachSpell.FormattingEnabled = true;
            this.cmbTeachSpell.Location = new System.Drawing.Point(15, 40);
            this.cmbTeachSpell.Name = "cmbTeachSpell";
            this.cmbTeachSpell.Size = new System.Drawing.Size(180, 21);
            this.cmbTeachSpell.TabIndex = 12;
            this.cmbTeachSpell.Text = null;
            this.cmbTeachSpell.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbTeachSpell.SelectedIndexChanged += new System.EventHandler(this.cmbTeachSpell_SelectedIndexChanged);
            // 
            // lblSpell
            // 
            this.lblSpell.AutoSize = true;
            this.lblSpell.Location = new System.Drawing.Point(12, 21);
            this.lblSpell.Name = "lblSpell";
            this.lblSpell.Size = new System.Drawing.Size(33, 13);
            this.lblSpell.TabIndex = 11;
            this.lblSpell.Text = "Spell:";
            // 
            // pnlContainer
            // 
            this.pnlContainer.AutoScroll = true;
            this.pnlContainer.Controls.Add(this.grpEvent);
            this.pnlContainer.Controls.Add(this.grpGeneral);
            this.pnlContainer.Controls.Add(this.grpEquipment);
            this.pnlContainer.Controls.Add(this.grpSpell);
            this.pnlContainer.Controls.Add(this.grpBags);
            this.pnlContainer.Controls.Add(this.grpConsumable);
            this.pnlContainer.Location = new System.Drawing.Point(221, 34);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(463, 442);
            this.pnlContainer.TabIndex = 43;
            this.pnlContainer.Visible = false;
            // 
            // grpBags
            // 
            this.grpBags.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpBags.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpBags.Controls.Add(this.nudBag);
            this.grpBags.Controls.Add(this.lblBag);
            this.grpBags.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpBags.Location = new System.Drawing.Point(2, 435);
            this.grpBags.Name = "grpBags";
            this.grpBags.Size = new System.Drawing.Size(222, 57);
            this.grpBags.TabIndex = 44;
            this.grpBags.TabStop = false;
            this.grpBags.Text = "Bag:";
            this.grpBags.Visible = false;
            // 
            // nudBag
            // 
            this.nudBag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudBag.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudBag.Location = new System.Drawing.Point(69, 23);
            this.nudBag.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.nudBag.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBag.Name = "nudBag";
            this.nudBag.Size = new System.Drawing.Size(144, 20);
            this.nudBag.TabIndex = 38;
            this.nudBag.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBag.ValueChanged += new System.EventHandler(this.nudBag_ValueChanged);
            // 
            // lblBag
            // 
            this.lblBag.AutoSize = true;
            this.lblBag.Location = new System.Drawing.Point(8, 25);
            this.lblBag.Name = "lblBag";
            this.lblBag.Size = new System.Drawing.Size(55, 13);
            this.lblBag.TabIndex = 11;
            this.lblBag.Text = "Bag Slots:";
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
            this.toolStrip.Size = new System.Drawing.Size(686, 25);
            this.toolStrip.TabIndex = 44;
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
            // grpRequirements
            // 
            this.grpRequirements.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpRequirements.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpRequirements.Controls.Add(this.lblCannotUse);
            this.grpRequirements.Controls.Add(this.txtCannotUse);
            this.grpRequirements.Controls.Add(this.btnEditRequirements);
            this.grpRequirements.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpRequirements.Location = new System.Drawing.Point(12, 326);
            this.grpRequirements.Margin = new System.Windows.Forms.Padding(2);
            this.grpRequirements.Name = "grpRequirements";
            this.grpRequirements.Padding = new System.Windows.Forms.Padding(2);
            this.grpRequirements.Size = new System.Drawing.Size(229, 92);
            this.grpRequirements.TabIndex = 100;
            this.grpRequirements.TabStop = false;
            this.grpRequirements.Text = "Requirements";
            // 
            // lblCannotUse
            // 
            this.lblCannotUse.AutoSize = true;
            this.lblCannotUse.Location = new System.Drawing.Point(5, 47);
            this.lblCannotUse.Name = "lblCannotUse";
            this.lblCannotUse.Size = new System.Drawing.Size(112, 13);
            this.lblCannotUse.TabIndex = 54;
            this.lblCannotUse.Text = "Cannot Use Message:";
            // 
            // txtCannotUse
            // 
            this.txtCannotUse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtCannotUse.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCannotUse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtCannotUse.Location = new System.Drawing.Point(8, 63);
            this.txtCannotUse.Name = "txtCannotUse";
            this.txtCannotUse.Size = new System.Drawing.Size(207, 20);
            this.txtCannotUse.TabIndex = 53;
            this.txtCannotUse.TextChanged += new System.EventHandler(this.txtCannotUse_TextChanged);
            // 
            // FrmItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(686, 517);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpItems);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "FrmItem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Item Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmItem_FormClosed);
            this.Load += new System.EventHandler(this.frmItem_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.grpItems.ResumeLayout(false);
            this.grpItems.PerformLayout();
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBankStackLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInvStackLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDeathDropChance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRgbaA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRgbaB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRgbaG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRgbaR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCooldown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).EndInit();
            this.grpEquipment.ResumeLayout(false);
            this.grpEquipment.PerformLayout();
            this.grpRegen.ResumeLayout(false);
            this.grpRegen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMpRegen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHPRegen)).EndInit();
            this.grpVitalBonuses.ResumeLayout(false);
            this.grpVitalBonuses.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMPPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHPPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudManaBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHealthBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEffectPercent)).EndInit();
            this.grpStatBonuses.ResumeLayout(false);
            this.grpStatBonuses.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpdPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMRPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDefPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMagPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStrPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDef)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFemalePaperdoll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMalePaperdoll)).EndInit();
            this.grpWeaponProperties.ResumeLayout(false);
            this.grpWeaponProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCritMultiplier)).EndInit();
            this.grpAttackSpeed.ResumeLayout(false);
            this.grpAttackSpeed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAttackSpeedValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScaling)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCritChance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDamage)).EndInit();
            this.grpEvent.ResumeLayout(false);
            this.grpEvent.PerformLayout();
            this.grpConsumable.ResumeLayout(false);
            this.grpConsumable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudIntervalPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInterval)).EndInit();
            this.grpSpell.ResumeLayout(false);
            this.grpSpell.PerformLayout();
            this.pnlContainer.ResumeLayout(false);
            this.grpBags.ResumeLayout(false);
            this.grpBags.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBag)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.grpRequirements.ResumeLayout(false);
            this.grpRequirements.PerformLayout();
            this.ResumeLayout(false);

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
        private Label lblRange;
        private Label lblPic;
        private Label lblMalePaperdoll;
        private Label lblDesc;
        private Label lblEquipmentSlot;
        private Label lblEffectPercent;
        private Label lblBonusEffect;
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
        private DarkUI.Controls.DarkGroupBox grpStatBonuses;
        private DarkUI.Controls.DarkGroupBox grpWeaponProperties;
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
        private DarkComboBox cmbEquipmentBonus;
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
        private DarkNumericUpDown nudRange;
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
        private ToolStripButton btnChronological;
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
    }
}
