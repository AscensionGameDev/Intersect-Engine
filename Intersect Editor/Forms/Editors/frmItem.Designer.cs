using System.ComponentModel;
using System.Windows.Forms;
using DarkUI.Controls;

namespace Intersect_Editor.Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmItem));
            this.groupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.lstItems = new System.Windows.Forms.ListBox();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.groupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDesc = new DarkUI.Controls.DarkTextBox();
            this.cmbPic = new DarkUI.Controls.DarkComboBox();
            this.scrlAnim = new DarkScrollBar();
            this.lblAnim = new System.Windows.Forms.Label();
            this.scrlPrice = new DarkScrollBar();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblPic = new System.Windows.Forms.Label();
            this.picItem = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new DarkUI.Controls.DarkComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new DarkUI.Controls.DarkTextBox();
            this.groupBox3 = new DarkUI.Controls.DarkGroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbGender = new DarkUI.Controls.DarkComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbClass = new DarkUI.Controls.DarkComboBox();
            this.lblLevelReq = new System.Windows.Forms.Label();
            this.scrlLevelReq = new DarkScrollBar();
            this.lblSpeedReq = new System.Windows.Forms.Label();
            this.lblMagicResistReq = new System.Windows.Forms.Label();
            this.lblDefenseReq = new System.Windows.Forms.Label();
            this.lblAbilityPowerReq = new System.Windows.Forms.Label();
            this.lblAttackReq = new System.Windows.Forms.Label();
            this.scrlDefenseReq = new DarkScrollBar();
            this.scrlSpeedReq = new DarkScrollBar();
            this.scrlMagicResistReq = new DarkScrollBar();
            this.scrlAbilityPowerReq = new DarkScrollBar();
            this.scrlAttackReq = new DarkScrollBar();
            this.gbEquipment = new DarkUI.Controls.DarkGroupBox();
            this.darkGroupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.lblAttack = new System.Windows.Forms.Label();
            this.scrlAttack = new DarkScrollBar();
            this.scrlAbilityPower = new DarkScrollBar();
            this.scrlMagicResist = new DarkScrollBar();
            this.scrlSpeed = new DarkScrollBar();
            this.scrlDefense = new DarkScrollBar();
            this.lblAbilityPower = new System.Windows.Forms.Label();
            this.lblDefense = new System.Windows.Forms.Label();
            this.lblMagicResist = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.scrlRange = new DarkScrollBar();
            this.lblRange = new System.Windows.Forms.Label();
            this.cmbFemalePaperdoll = new DarkUI.Controls.DarkComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.picFemalePaperdoll = new System.Windows.Forms.PictureBox();
            this.lblEffectPercent = new System.Windows.Forms.Label();
            this.scrlEffectAmount = new DarkScrollBar();
            this.cmbEquipmentBonus = new DarkUI.Controls.DarkComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbEquipmentSlot = new DarkUI.Controls.DarkComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbMalePaperdoll = new DarkUI.Controls.DarkComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.picMalePaperdoll = new System.Windows.Forms.PictureBox();
            this.grpWeaponProperties = new DarkUI.Controls.DarkGroupBox();
            this.cmbProjectile = new DarkUI.Controls.DarkComboBox();
            this.cmbScalingStat = new DarkUI.Controls.DarkComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lblScaling = new System.Windows.Forms.Label();
            this.scrlScaling = new DarkScrollBar();
            this.cmbDamageType = new DarkUI.Controls.DarkComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.lblCritChance = new System.Windows.Forms.Label();
            this.scrlCritChance = new DarkScrollBar();
            this.cmbAttackAnimation = new DarkUI.Controls.DarkComboBox();
            this.lblAttackAnimation = new System.Windows.Forms.Label();
            this.chk2Hand = new DarkUI.Controls.DarkCheckBox();
            this.lblToolType = new System.Windows.Forms.Label();
            this.cmbToolType = new DarkUI.Controls.DarkComboBox();
            this.lblProjectile = new System.Windows.Forms.Label();
            this.lblDamage = new System.Windows.Forms.Label();
            this.scrlDamage = new DarkScrollBar();
            this.gbConsumable = new DarkUI.Controls.DarkGroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbConsume = new DarkUI.Controls.DarkComboBox();
            this.scrlInterval = new DarkScrollBar();
            this.lblInterval = new System.Windows.Forms.Label();
            this.gbSpell = new DarkUI.Controls.DarkGroupBox();
            this.scrlSpell = new DarkScrollBar();
            this.lblSpell = new System.Windows.Forms.Label();
            this.grpEvent = new DarkUI.Controls.DarkGroupBox();
            this.scrlEvent = new DarkScrollBar();
            this.lblEvent = new System.Windows.Forms.Label();
            this.pnlContainer = new System.Windows.Forms.Panel();
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
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.gbEquipment.SuspendLayout();
            this.darkGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFemalePaperdoll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMalePaperdoll)).BeginInit();
            this.grpWeaponProperties.SuspendLayout();
            this.gbConsumable.SuspendLayout();
            this.gbSpell.SuspendLayout();
            this.grpEvent.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox1.Controls.Add(this.lstItems);
            this.groupBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox1.Location = new System.Drawing.Point(12, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 476);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Items";
            // 
            // lstItems
            // 
            this.lstItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstItems.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstItems.FormattingEnabled = true;
            this.lstItems.Location = new System.Drawing.Point(6, 19);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(191, 444);
            this.lstItems.TabIndex = 1;
            this.lstItems.Click += new System.EventHandler(this.lstItems_Click);
            this.lstItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemList_KeyDown);
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
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtDesc);
            this.groupBox2.Controls.Add(this.cmbPic);
            this.groupBox2.Controls.Add(this.scrlAnim);
            this.groupBox2.Controls.Add(this.lblAnim);
            this.groupBox2.Controls.Add(this.scrlPrice);
            this.groupBox2.Controls.Add(this.lblPrice);
            this.groupBox2.Controls.Add(this.lblPic);
            this.groupBox2.Controls.Add(this.picItem);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmbType);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox2.Location = new System.Drawing.Point(2, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(207, 225);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Desc:";
            // 
            // txtDesc
            // 
            this.txtDesc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDesc.Location = new System.Drawing.Point(60, 74);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(135, 20);
            this.txtDesc.TabIndex = 12;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // cmbPic
            // 
            this.cmbPic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbPic.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbPic.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbPic.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbPic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPic.FormattingEnabled = true;
            this.cmbPic.Items.AddRange(new object[] {
            "None"});
            this.cmbPic.Location = new System.Drawing.Point(60, 122);
            this.cmbPic.Name = "cmbPic";
            this.cmbPic.Size = new System.Drawing.Size(135, 21);
            this.cmbPic.TabIndex = 11;
            this.cmbPic.SelectedIndexChanged += new System.EventHandler(this.cmbPic_SelectedIndexChanged);
            // 
            // scrlAnim
            // 
            
            this.scrlAnim.Location = new System.Drawing.Point(19, 195);
            this.scrlAnim.Maximum = 1000;
            this.scrlAnim.Minimum = -1;
            this.scrlAnim.Name = "scrlAnim";
            this.scrlAnim.Size = new System.Drawing.Size(176, 18);
            this.scrlAnim.TabIndex = 10;
            this.scrlAnim.Value = -1;
            this.scrlAnim.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAnim_Scroll);
            this.scrlAnim.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // lblAnim
            // 
            this.lblAnim.AutoSize = true;
            this.lblAnim.Location = new System.Drawing.Point(16, 181);
            this.lblAnim.Name = "lblAnim";
            this.lblAnim.Size = new System.Drawing.Size(94, 13);
            this.lblAnim.TabIndex = 9;
            this.lblAnim.Text = "Animation: 0 None";
            // 
            // scrlPrice
            // 
            this.scrlPrice.Location = new System.Drawing.Point(19, 163);
            this.scrlPrice.Maximum = 1000;
            this.scrlPrice.Name = "scrlPrice";
            this.scrlPrice.Size = new System.Drawing.Size(176, 18);
            this.scrlPrice.TabIndex = 8;
            this.scrlPrice.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlPrice_Scroll);
            this.scrlPrice.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(16, 143);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(43, 13);
            this.lblPrice.TabIndex = 7;
            this.lblPrice.Text = "Price: 0";
            // 
            // lblPic
            // 
            this.lblPic.AutoSize = true;
            this.lblPic.Location = new System.Drawing.Point(57, 105);
            this.lblPic.Name = "lblPic";
            this.lblPic.Size = new System.Drawing.Size(25, 13);
            this.lblPic.TabIndex = 6;
            this.lblPic.Text = "Pic:";
            // 
            // picItem
            // 
            this.picItem.Location = new System.Drawing.Point(15, 103);
            this.picItem.Name = "picItem";
            this.picItem.Size = new System.Drawing.Size(32, 33);
            this.picItem.TabIndex = 4;
            this.picItem.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type:";
            // 
            // cmbType
            // 
            this.cmbType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "None",
            "Equipment",
            "Consumable",
            "Currency",
            "Spell",
            "Event"});
            this.cmbType.Location = new System.Drawing.Point(60, 46);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(135, 21);
            this.cmbType.TabIndex = 2;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtName.Location = new System.Drawing.Point(60, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(135, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.cmbGender);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.cmbClass);
            this.groupBox3.Controls.Add(this.lblLevelReq);
            this.groupBox3.Controls.Add(this.scrlLevelReq);
            this.groupBox3.Controls.Add(this.lblSpeedReq);
            this.groupBox3.Controls.Add(this.lblMagicResistReq);
            this.groupBox3.Controls.Add(this.lblDefenseReq);
            this.groupBox3.Controls.Add(this.lblAbilityPowerReq);
            this.groupBox3.Controls.Add(this.lblAttackReq);
            this.groupBox3.Controls.Add(this.scrlDefenseReq);
            this.groupBox3.Controls.Add(this.scrlSpeedReq);
            this.groupBox3.Controls.Add(this.scrlMagicResistReq);
            this.groupBox3.Controls.Add(this.scrlAbilityPowerReq);
            this.groupBox3.Controls.Add(this.scrlAttackReq);
            this.groupBox3.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox3.Location = new System.Drawing.Point(215, 1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(226, 225);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Requirements";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(124, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Gender:";
            // 
            // cmbGender
            // 
            this.cmbGender.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbGender.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbGender.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbGender.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbGender.FormattingEnabled = true;
            this.cmbGender.Items.AddRange(new object[] {
            "None",
            "Male",
            "Female"});
            this.cmbGender.Location = new System.Drawing.Point(127, 195);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(80, 21);
            this.cmbGender.TabIndex = 14;
            this.cmbGender.SelectedIndexChanged += new System.EventHandler(this.cmbGender_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Class:";
            // 
            // cmbClass
            // 
            this.cmbClass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbClass.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbClass.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbClass.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Items.AddRange(new object[] {
            "None"});
            this.cmbClass.Location = new System.Drawing.Point(13, 195);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(80, 21);
            this.cmbClass.TabIndex = 12;
            this.cmbClass.SelectedIndexChanged += new System.EventHandler(this.cmbClass_SelectedIndexChanged);
            // 
            // lblLevelReq
            // 
            this.lblLevelReq.AutoSize = true;
            this.lblLevelReq.Location = new System.Drawing.Point(124, 125);
            this.lblLevelReq.Name = "lblLevelReq";
            this.lblLevelReq.Size = new System.Drawing.Size(45, 13);
            this.lblLevelReq.TabIndex = 11;
            this.lblLevelReq.Text = "Level: 0";
            // 
            // scrlLevelReq
            // 
            
            this.scrlLevelReq.Location = new System.Drawing.Point(127, 139);
            this.scrlLevelReq.Name = "scrlLevelReq";
            this.scrlLevelReq.Size = new System.Drawing.Size(80, 17);
            this.scrlLevelReq.TabIndex = 10;
            this.scrlLevelReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlLevel_Scroll);
            this.scrlLevelReq.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // lblSpeedReq
            // 
            this.lblSpeedReq.AutoSize = true;
            this.lblSpeedReq.Location = new System.Drawing.Point(10, 124);
            this.lblSpeedReq.Name = "lblSpeedReq";
            this.lblSpeedReq.Size = new System.Drawing.Size(50, 13);
            this.lblSpeedReq.TabIndex = 9;
            this.lblSpeedReq.Text = "Speed: 0";
            // 
            // lblMagicResistReq
            // 
            this.lblMagicResistReq.AutoSize = true;
            this.lblMagicResistReq.Location = new System.Drawing.Point(124, 72);
            this.lblMagicResistReq.Name = "lblMagicResistReq";
            this.lblMagicResistReq.Size = new System.Drawing.Size(80, 13);
            this.lblMagicResistReq.TabIndex = 8;
            this.lblMagicResistReq.Text = "Magic Resist: 0";
            // 
            // lblDefenseReq
            // 
            this.lblDefenseReq.AutoSize = true;
            this.lblDefenseReq.Location = new System.Drawing.Point(10, 72);
            this.lblDefenseReq.Name = "lblDefenseReq";
            this.lblDefenseReq.Size = new System.Drawing.Size(59, 13);
            this.lblDefenseReq.TabIndex = 7;
            this.lblDefenseReq.Text = "Defense: 0";
            // 
            // lblAbilityPowerReq
            // 
            this.lblAbilityPowerReq.AutoSize = true;
            this.lblAbilityPowerReq.Location = new System.Drawing.Point(124, 21);
            this.lblAbilityPowerReq.Name = "lblAbilityPowerReq";
            this.lblAbilityPowerReq.Size = new System.Drawing.Size(67, 13);
            this.lblAbilityPowerReq.TabIndex = 6;
            this.lblAbilityPowerReq.Text = "Ability Pwr: 0";
            // 
            // lblAttackReq
            // 
            this.lblAttackReq.AutoSize = true;
            this.lblAttackReq.Location = new System.Drawing.Point(10, 21);
            this.lblAttackReq.Name = "lblAttackReq";
            this.lblAttackReq.Size = new System.Drawing.Size(50, 13);
            this.lblAttackReq.TabIndex = 5;
            this.lblAttackReq.Text = "Attack: 0";
            // 
            // scrlDefenseReq
            // 
            
            this.scrlDefenseReq.Location = new System.Drawing.Point(13, 87);
            this.scrlDefenseReq.Maximum = 255;
            this.scrlDefenseReq.Name = "scrlDefenseReq";
            this.scrlDefenseReq.Size = new System.Drawing.Size(80, 17);
            this.scrlDefenseReq.TabIndex = 4;
            this.scrlDefenseReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlDefenseReq_Scroll);
            this.scrlDefenseReq.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // scrlSpeedReq
            // 
            
            this.scrlSpeedReq.Location = new System.Drawing.Point(13, 137);
            this.scrlSpeedReq.Maximum = 255;
            this.scrlSpeedReq.Name = "scrlSpeedReq";
            this.scrlSpeedReq.Size = new System.Drawing.Size(80, 17);
            this.scrlSpeedReq.TabIndex = 3;
            this.scrlSpeedReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlSpeedReq_Scroll);
            this.scrlSpeedReq.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // scrlMagicResistReq
            // 
            
            this.scrlMagicResistReq.Location = new System.Drawing.Point(127, 87);
            this.scrlMagicResistReq.Maximum = 255;
            this.scrlMagicResistReq.Name = "scrlMagicResistReq";
            this.scrlMagicResistReq.Size = new System.Drawing.Size(80, 17);
            this.scrlMagicResistReq.TabIndex = 2;
            this.scrlMagicResistReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlMagicResistReq_Scroll);
            this.scrlMagicResistReq.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // scrlAbilityPowerReq
            // 
            
            this.scrlAbilityPowerReq.Location = new System.Drawing.Point(127, 40);
            this.scrlAbilityPowerReq.Maximum = 255;
            this.scrlAbilityPowerReq.Name = "scrlAbilityPowerReq";
            this.scrlAbilityPowerReq.Size = new System.Drawing.Size(80, 17);
            this.scrlAbilityPowerReq.TabIndex = 1;
            this.scrlAbilityPowerReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAbilityPowerReq_Scroll);
            this.scrlAbilityPowerReq.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // scrlAttackReq
            // 
            
            this.scrlAttackReq.Location = new System.Drawing.Point(13, 40);
            this.scrlAttackReq.Maximum = 255;
            this.scrlAttackReq.Name = "scrlAttackReq";
            this.scrlAttackReq.Size = new System.Drawing.Size(80, 17);
            this.scrlAttackReq.TabIndex = 0;
            this.scrlAttackReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAttackReq_Scroll);
            this.scrlAttackReq.ScrollOrientation = DarkScrollOrientation.Horizontal;
            this.scrlAttackReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAttackReq_Scroll);
            this.scrlAttackReq.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // gbEquipment
            // 
            this.gbEquipment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.gbEquipment.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.gbEquipment.Controls.Add(this.darkGroupBox1);
            this.gbEquipment.Controls.Add(this.cmbFemalePaperdoll);
            this.gbEquipment.Controls.Add(this.label10);
            this.gbEquipment.Controls.Add(this.picFemalePaperdoll);
            this.gbEquipment.Controls.Add(this.lblEffectPercent);
            this.gbEquipment.Controls.Add(this.scrlEffectAmount);
            this.gbEquipment.Controls.Add(this.cmbEquipmentBonus);
            this.gbEquipment.Controls.Add(this.label9);
            this.gbEquipment.Controls.Add(this.cmbEquipmentSlot);
            this.gbEquipment.Controls.Add(this.label7);
            this.gbEquipment.Controls.Add(this.cmbMalePaperdoll);
            this.gbEquipment.Controls.Add(this.label5);
            this.gbEquipment.Controls.Add(this.picMalePaperdoll);
            this.gbEquipment.Controls.Add(this.grpWeaponProperties);
            this.gbEquipment.ForeColor = System.Drawing.Color.Gainsboro;
            this.gbEquipment.Location = new System.Drawing.Point(2, 233);
            this.gbEquipment.Name = "gbEquipment";
            this.gbEquipment.Size = new System.Drawing.Size(439, 581);
            this.gbEquipment.TabIndex = 12;
            this.gbEquipment.TabStop = false;
            this.gbEquipment.Text = "Equipment";
            this.gbEquipment.Visible = false;
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.darkGroupBox1.Controls.Add(this.lblAttack);
            this.darkGroupBox1.Controls.Add(this.scrlAttack);
            this.darkGroupBox1.Controls.Add(this.scrlAbilityPower);
            this.darkGroupBox1.Controls.Add(this.scrlMagicResist);
            this.darkGroupBox1.Controls.Add(this.scrlSpeed);
            this.darkGroupBox1.Controls.Add(this.scrlDefense);
            this.darkGroupBox1.Controls.Add(this.lblAbilityPower);
            this.darkGroupBox1.Controls.Add(this.lblDefense);
            this.darkGroupBox1.Controls.Add(this.lblMagicResist);
            this.darkGroupBox1.Controls.Add(this.lblSpeed);
            this.darkGroupBox1.Controls.Add(this.scrlRange);
            this.darkGroupBox1.Controls.Add(this.lblRange);
            this.darkGroupBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.darkGroupBox1.Location = new System.Drawing.Point(13, 67);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(200, 204);
            this.darkGroupBox1.TabIndex = 40;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "Stat Bonuses";
            // 
            // lblAttack
            // 
            this.lblAttack.AutoSize = true;
            this.lblAttack.Location = new System.Drawing.Point(9, 16);
            this.lblAttack.Name = "lblAttack";
            this.lblAttack.Size = new System.Drawing.Size(50, 13);
            this.lblAttack.TabIndex = 5;
            this.lblAttack.Text = "Attack: 0";
            // 
            // scrlAttack
            // 
            
            this.scrlAttack.Location = new System.Drawing.Point(12, 35);
            this.scrlAttack.Maximum = 255;
            this.scrlAttack.Name = "scrlAttack";
            this.scrlAttack.Size = new System.Drawing.Size(80, 17);
            this.scrlAttack.TabIndex = 0;
            this.scrlAttack.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAttack_Scroll);
            this.scrlAttack.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // scrlAbilityPower
            // 
            this.scrlAbilityPower.Location = new System.Drawing.Point(103, 35);
            this.scrlAbilityPower.Maximum = 255;
            this.scrlAbilityPower.Name = "scrlAbilityPower";
            this.scrlAbilityPower.Size = new System.Drawing.Size(80, 17);
            this.scrlAbilityPower.TabIndex = 1;
            this.scrlAbilityPower.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAbilityPower_Scroll);
            this.scrlAbilityPower.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // scrlMagicResist
            // 
            
            this.scrlMagicResist.Location = new System.Drawing.Point(103, 82);
            this.scrlMagicResist.Maximum = 255;
            this.scrlMagicResist.Name = "scrlMagicResist";
            this.scrlMagicResist.Size = new System.Drawing.Size(80, 17);
            this.scrlMagicResist.TabIndex = 2;
            this.scrlMagicResist.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlMagicResist_Scroll);
            this.scrlMagicResist.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // scrlSpeed
            // 
            
            this.scrlSpeed.Location = new System.Drawing.Point(12, 125);
            this.scrlSpeed.Maximum = 255;
            this.scrlSpeed.Name = "scrlSpeed";
            this.scrlSpeed.Size = new System.Drawing.Size(80, 17);
            this.scrlSpeed.TabIndex = 3;
            this.scrlSpeed.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlSpeed_Scroll);
            this.scrlSpeed.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // scrlDefense
            // 
            
            this.scrlDefense.Location = new System.Drawing.Point(12, 82);
            this.scrlDefense.Maximum = 255;
            this.scrlDefense.Name = "scrlDefense";
            this.scrlDefense.Size = new System.Drawing.Size(80, 17);
            this.scrlDefense.TabIndex = 4;
            this.scrlDefense.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlDefense_Scroll);
            this.scrlDefense.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // lblAbilityPower
            // 
            this.lblAbilityPower.AutoSize = true;
            this.lblAbilityPower.Location = new System.Drawing.Point(100, 16);
            this.lblAbilityPower.Name = "lblAbilityPower";
            this.lblAbilityPower.Size = new System.Drawing.Size(67, 13);
            this.lblAbilityPower.TabIndex = 6;
            this.lblAbilityPower.Text = "Ability Pwr: 0";
            // 
            // lblDefense
            // 
            this.lblDefense.AutoSize = true;
            this.lblDefense.Location = new System.Drawing.Point(9, 67);
            this.lblDefense.Name = "lblDefense";
            this.lblDefense.Size = new System.Drawing.Size(59, 13);
            this.lblDefense.TabIndex = 7;
            this.lblDefense.Text = "Defense: 0";
            // 
            // lblMagicResist
            // 
            this.lblMagicResist.AutoSize = true;
            this.lblMagicResist.Location = new System.Drawing.Point(100, 67);
            this.lblMagicResist.Name = "lblMagicResist";
            this.lblMagicResist.Size = new System.Drawing.Size(80, 13);
            this.lblMagicResist.TabIndex = 8;
            this.lblMagicResist.Text = "Magic Resist: 0";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(8, 112);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(50, 13);
            this.lblSpeed.TabIndex = 9;
            this.lblSpeed.Text = "Speed: 0";
            // 
            // scrlRange
            // 
            
            this.scrlRange.Location = new System.Drawing.Point(12, 172);
            this.scrlRange.Name = "scrlRange";
            this.scrlRange.Size = new System.Drawing.Size(80, 18);
            this.scrlRange.TabIndex = 19;
            this.scrlRange.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlRange_Scroll);
            this.scrlRange.ScrollOrientation = DarkScrollOrientation.Horizontal;
            this.scrlRange.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlRange_Scroll);
            this.scrlRange.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // lblRange
            // 
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(8, 153);
            this.lblRange.Name = "lblRange";
            this.lblRange.Size = new System.Drawing.Size(118, 13);
            this.lblRange.TabIndex = 20;
            this.lblRange.Text = "Stat Bonus Range: +- 0";
            // 
            // cmbFemalePaperdoll
            // 
            this.cmbFemalePaperdoll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbFemalePaperdoll.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbFemalePaperdoll.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbFemalePaperdoll.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbFemalePaperdoll.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFemalePaperdoll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbFemalePaperdoll.FormattingEnabled = true;
            this.cmbFemalePaperdoll.Items.AddRange(new object[] {
            "None"});
            this.cmbFemalePaperdoll.Location = new System.Drawing.Point(222, 379);
            this.cmbFemalePaperdoll.Name = "cmbFemalePaperdoll";
            this.cmbFemalePaperdoll.Size = new System.Drawing.Size(168, 21);
            this.cmbFemalePaperdoll.TabIndex = 36;
            this.cmbFemalePaperdoll.SelectedIndexChanged += new System.EventHandler(this.cmbFemalePaperdoll_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(219, 362);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "Female Paperdoll:";
            // 
            // picFemalePaperdoll
            // 
            this.picFemalePaperdoll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picFemalePaperdoll.Location = new System.Drawing.Point(222, 406);
            this.picFemalePaperdoll.Name = "picFemalePaperdoll";
            this.picFemalePaperdoll.Size = new System.Drawing.Size(200, 156);
            this.picFemalePaperdoll.TabIndex = 34;
            this.picFemalePaperdoll.TabStop = false;
            // 
            // lblEffectPercent
            // 
            this.lblEffectPercent.AutoSize = true;
            this.lblEffectPercent.Location = new System.Drawing.Point(12, 316);
            this.lblEffectPercent.Name = "lblEffectPercent";
            this.lblEffectPercent.Size = new System.Drawing.Size(94, 13);
            this.lblEffectPercent.TabIndex = 31;
            this.lblEffectPercent.Text = "Effect Amount: 0%";
            this.lblEffectPercent.Click += new System.EventHandler(this.lblEffectPercent_Click);
            // 
            // scrlEffectAmount
            // 
            
            this.scrlEffectAmount.Location = new System.Drawing.Point(15, 333);
            this.scrlEffectAmount.Name = "scrlEffectAmount";
            this.scrlEffectAmount.Size = new System.Drawing.Size(198, 17);
            this.scrlEffectAmount.TabIndex = 30;
            this.scrlEffectAmount.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlEffectAmount_ValueChanged);
            this.scrlEffectAmount.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // cmbEquipmentBonus
            // 
            this.cmbEquipmentBonus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEquipmentBonus.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbEquipmentBonus.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbEquipmentBonus.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEquipmentBonus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquipmentBonus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEquipmentBonus.FormattingEnabled = true;
            this.cmbEquipmentBonus.Location = new System.Drawing.Point(15, 292);
            this.cmbEquipmentBonus.Name = "cmbEquipmentBonus";
            this.cmbEquipmentBonus.Size = new System.Drawing.Size(198, 21);
            this.cmbEquipmentBonus.TabIndex = 29;
            this.cmbEquipmentBonus.SelectedIndexChanged += new System.EventHandler(this.cmbEquipmentBonus_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 275);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Bonus Effect:";
            // 
            // cmbEquipmentSlot
            // 
            this.cmbEquipmentSlot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEquipmentSlot.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbEquipmentSlot.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbEquipmentSlot.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEquipmentSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEquipmentSlot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEquipmentSlot.FormattingEnabled = true;
            this.cmbEquipmentSlot.Location = new System.Drawing.Point(15, 40);
            this.cmbEquipmentSlot.Name = "cmbEquipmentSlot";
            this.cmbEquipmentSlot.Size = new System.Drawing.Size(198, 21);
            this.cmbEquipmentSlot.TabIndex = 24;
            this.cmbEquipmentSlot.SelectedIndexChanged += new System.EventHandler(this.cmbEquipmentSlot_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Equipment Slot:";
            // 
            // cmbMalePaperdoll
            // 
            this.cmbMalePaperdoll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbMalePaperdoll.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMalePaperdoll.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMalePaperdoll.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMalePaperdoll.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMalePaperdoll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMalePaperdoll.FormattingEnabled = true;
            this.cmbMalePaperdoll.Items.AddRange(new object[] {
            "None"});
            this.cmbMalePaperdoll.Location = new System.Drawing.Point(12, 379);
            this.cmbMalePaperdoll.Name = "cmbMalePaperdoll";
            this.cmbMalePaperdoll.Size = new System.Drawing.Size(168, 21);
            this.cmbMalePaperdoll.TabIndex = 22;
            this.cmbMalePaperdoll.SelectedIndexChanged += new System.EventHandler(this.cmbPaperdoll_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 362);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Male Paperdoll:";
            // 
            // picMalePaperdoll
            // 
            this.picMalePaperdoll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picMalePaperdoll.Location = new System.Drawing.Point(12, 406);
            this.picMalePaperdoll.Name = "picMalePaperdoll";
            this.picMalePaperdoll.Size = new System.Drawing.Size(200, 156);
            this.picMalePaperdoll.TabIndex = 16;
            this.picMalePaperdoll.TabStop = false;
            // 
            // grpWeaponProperties
            // 
            this.grpWeaponProperties.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpWeaponProperties.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpWeaponProperties.Controls.Add(this.cmbProjectile);
            this.grpWeaponProperties.Controls.Add(this.cmbScalingStat);
            this.grpWeaponProperties.Controls.Add(this.label12);
            this.grpWeaponProperties.Controls.Add(this.lblScaling);
            this.grpWeaponProperties.Controls.Add(this.scrlScaling);
            this.grpWeaponProperties.Controls.Add(this.cmbDamageType);
            this.grpWeaponProperties.Controls.Add(this.label11);
            this.grpWeaponProperties.Controls.Add(this.lblCritChance);
            this.grpWeaponProperties.Controls.Add(this.scrlCritChance);
            this.grpWeaponProperties.Controls.Add(this.cmbAttackAnimation);
            this.grpWeaponProperties.Controls.Add(this.lblAttackAnimation);
            this.grpWeaponProperties.Controls.Add(this.chk2Hand);
            this.grpWeaponProperties.Controls.Add(this.lblToolType);
            this.grpWeaponProperties.Controls.Add(this.cmbToolType);
            this.grpWeaponProperties.Controls.Add(this.lblProjectile);
            this.grpWeaponProperties.Controls.Add(this.lblDamage);
            this.grpWeaponProperties.Controls.Add(this.scrlDamage);
            this.grpWeaponProperties.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpWeaponProperties.Location = new System.Drawing.Point(222, 19);
            this.grpWeaponProperties.Name = "grpWeaponProperties";
            this.grpWeaponProperties.Size = new System.Drawing.Size(207, 340);
            this.grpWeaponProperties.TabIndex = 39;
            this.grpWeaponProperties.TabStop = false;
            this.grpWeaponProperties.Text = "Weapon Properties";
            this.grpWeaponProperties.Visible = false;
            // 
            // cmbProjectile
            // 
            this.cmbProjectile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbProjectile.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbProjectile.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbProjectile.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbProjectile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProjectile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbProjectile.FormattingEnabled = true;
            this.cmbProjectile.Location = new System.Drawing.Point(15, 230);
            this.cmbProjectile.Name = "cmbProjectile";
            this.cmbProjectile.Size = new System.Drawing.Size(180, 21);
            this.cmbProjectile.TabIndex = 47;
            this.cmbProjectile.SelectedIndexChanged += new System.EventHandler(this.cmbProjectile_SelectedIndexChanged);
            // 
            // cmbScalingStat
            // 
            this.cmbScalingStat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbScalingStat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbScalingStat.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbScalingStat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbScalingStat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScalingStat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbScalingStat.FormattingEnabled = true;
            this.cmbScalingStat.Location = new System.Drawing.Point(16, 153);
            this.cmbScalingStat.Name = "cmbScalingStat";
            this.cmbScalingStat.Size = new System.Drawing.Size(180, 21);
            this.cmbScalingStat.TabIndex = 46;
            this.cmbScalingStat.SelectedIndexChanged += new System.EventHandler(this.cmbScalingStat_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 136);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 13);
            this.label12.TabIndex = 45;
            this.label12.Text = "Scaling Stat:";
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // lblScaling
            // 
            this.lblScaling.AutoSize = true;
            this.lblScaling.Location = new System.Drawing.Point(12, 179);
            this.lblScaling.Name = "lblScaling";
            this.lblScaling.Size = new System.Drawing.Size(107, 13);
            this.lblScaling.TabIndex = 44;
            this.lblScaling.Text = "Scaling Amount: x0.0";
            this.lblScaling.Click += new System.EventHandler(this.lblScaling_Click);
            // 
            // scrlScaling
            // 
            
            this.scrlScaling.Location = new System.Drawing.Point(15, 193);
            this.scrlScaling.Maximum = 10000;
            this.scrlScaling.Name = "scrlScaling";
            this.scrlScaling.Size = new System.Drawing.Size(180, 17);
            this.scrlScaling.TabIndex = 43;
            this.scrlScaling.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlScaling_Scroll);
            this.scrlScaling.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // cmbDamageType
            // 
            this.cmbDamageType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDamageType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDamageType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDamageType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDamageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDamageType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDamageType.FormattingEnabled = true;
            this.cmbDamageType.Items.AddRange(new object[] {
            "Physical",
            "Magic",
            "True"});
            this.cmbDamageType.Location = new System.Drawing.Point(16, 112);
            this.cmbDamageType.Name = "cmbDamageType";
            this.cmbDamageType.Size = new System.Drawing.Size(180, 21);
            this.cmbDamageType.TabIndex = 42;
            this.cmbDamageType.SelectedIndexChanged += new System.EventHandler(this.cmbDamageType_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 95);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 13);
            this.label11.TabIndex = 41;
            this.label11.Text = "Damage Type:";
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // lblCritChance
            // 
            this.lblCritChance.AutoSize = true;
            this.lblCritChance.Location = new System.Drawing.Point(12, 59);
            this.lblCritChance.Name = "lblCritChance";
            this.lblCritChance.Size = new System.Drawing.Size(82, 13);
            this.lblCritChance.TabIndex = 40;
            this.lblCritChance.Text = "Crit Chance: 0%";
            this.lblCritChance.Click += new System.EventHandler(this.lblCritChance_Click);
            // 
            // scrlCritChance
            // 
            
            this.scrlCritChance.Location = new System.Drawing.Point(15, 73);
            this.scrlCritChance.Name = "scrlCritChance";
            this.scrlCritChance.Size = new System.Drawing.Size(180, 17);
            this.scrlCritChance.TabIndex = 39;
            this.scrlCritChance.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlCritChance_Scroll);
            this.scrlCritChance.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // cmbAttackAnimation
            // 
            this.cmbAttackAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAttackAnimation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAttackAnimation.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAttackAnimation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAttackAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAttackAnimation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAttackAnimation.FormattingEnabled = true;
            this.cmbAttackAnimation.Location = new System.Drawing.Point(15, 268);
            this.cmbAttackAnimation.Name = "cmbAttackAnimation";
            this.cmbAttackAnimation.Size = new System.Drawing.Size(180, 21);
            this.cmbAttackAnimation.TabIndex = 38;
            this.cmbAttackAnimation.SelectedIndexChanged += new System.EventHandler(this.cmbAttackAnimation_SelectedIndexChanged);
            // 
            // lblAttackAnimation
            // 
            this.lblAttackAnimation.AutoSize = true;
            this.lblAttackAnimation.Location = new System.Drawing.Point(12, 253);
            this.lblAttackAnimation.Name = "lblAttackAnimation";
            this.lblAttackAnimation.Size = new System.Drawing.Size(90, 13);
            this.lblAttackAnimation.TabIndex = 37;
            this.lblAttackAnimation.Text = "Attack Animation:";
            this.lblAttackAnimation.Click += new System.EventHandler(this.lblAttackAnimation_Click);
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
            this.lblToolType.Location = new System.Drawing.Point(13, 295);
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
            this.cmbToolType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbToolType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbToolType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbToolType.FormattingEnabled = true;
            this.cmbToolType.Location = new System.Drawing.Point(16, 309);
            this.cmbToolType.Name = "cmbToolType";
            this.cmbToolType.Size = new System.Drawing.Size(180, 21);
            this.cmbToolType.TabIndex = 27;
            this.cmbToolType.SelectedIndexChanged += new System.EventHandler(this.cmbToolType_SelectedIndexChanged);
            // 
            // lblProjectile
            // 
            this.lblProjectile.AutoSize = true;
            this.lblProjectile.Location = new System.Drawing.Point(12, 214);
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
            this.lblDamage.Size = new System.Drawing.Size(86, 13);
            this.lblDamage.TabIndex = 11;
            this.lblDamage.Text = "Base Damage: 0";
            this.lblDamage.Click += new System.EventHandler(this.lblDamage_Click);
            // 
            // scrlDamage
            // 
            
            this.scrlDamage.Location = new System.Drawing.Point(15, 33);
            this.scrlDamage.Maximum = 10000;
            this.scrlDamage.Name = "scrlDamage";
            this.scrlDamage.Size = new System.Drawing.Size(180, 17);
            this.scrlDamage.TabIndex = 10;
            this.scrlDamage.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlDamage_Scroll);
            this.scrlDamage.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // gbConsumable
            // 
            this.gbConsumable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.gbConsumable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.gbConsumable.Controls.Add(this.label4);
            this.gbConsumable.Controls.Add(this.cmbConsume);
            this.gbConsumable.Controls.Add(this.scrlInterval);
            this.gbConsumable.Controls.Add(this.lblInterval);
            this.gbConsumable.ForeColor = System.Drawing.Color.Gainsboro;
            this.gbConsumable.Location = new System.Drawing.Point(1, 235);
            this.gbConsumable.Name = "gbConsumable";
            this.gbConsumable.Size = new System.Drawing.Size(217, 125);
            this.gbConsumable.TabIndex = 12;
            this.gbConsumable.TabStop = false;
            this.gbConsumable.Text = "Consumable";
            this.gbConsumable.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Vital:";
            // 
            // cmbConsume
            // 
            this.cmbConsume.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbConsume.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbConsume.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbConsume.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbConsume.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConsume.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbConsume.FormattingEnabled = true;
            this.cmbConsume.Items.AddRange(new object[] {
            "Health",
            "Mana",
            "Experience"});
            this.cmbConsume.Location = new System.Drawing.Point(19, 37);
            this.cmbConsume.Name = "cmbConsume";
            this.cmbConsume.Size = new System.Drawing.Size(176, 21);
            this.cmbConsume.TabIndex = 11;
            this.cmbConsume.SelectedIndexChanged += new System.EventHandler(this.cmbConsume_SelectedIndexChanged);
            // 
            // scrlInterval
            // 
            this.scrlInterval.Location = new System.Drawing.Point(19, 90);
            this.scrlInterval.Maximum = 1000;
            this.scrlInterval.Minimum = -1000;
            this.scrlInterval.Name = "scrlInterval";
            this.scrlInterval.Size = new System.Drawing.Size(176, 18);
            this.scrlInterval.TabIndex = 10;
            this.scrlInterval.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlInterval_Scroll);
            this.scrlInterval.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(19, 71);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(54, 13);
            this.lblInterval.TabIndex = 9;
            this.lblInterval.Text = "Interval: 0";
            // 
            // gbSpell
            // 
            this.gbSpell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.gbSpell.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.gbSpell.Controls.Add(this.scrlSpell);
            this.gbSpell.Controls.Add(this.lblSpell);
            this.gbSpell.ForeColor = System.Drawing.Color.Gainsboro;
            this.gbSpell.Location = new System.Drawing.Point(2, 233);
            this.gbSpell.Name = "gbSpell";
            this.gbSpell.Size = new System.Drawing.Size(217, 69);
            this.gbSpell.TabIndex = 13;
            this.gbSpell.TabStop = false;
            this.gbSpell.Text = "Spell";
            this.gbSpell.Visible = false;
            // 
            // scrlSpell
            // 
            
            this.scrlSpell.Location = new System.Drawing.Point(12, 40);
            this.scrlSpell.Maximum = 1000;
            this.scrlSpell.Minimum = -1;
            this.scrlSpell.Name = "scrlSpell";
            this.scrlSpell.Size = new System.Drawing.Size(176, 18);
            this.scrlSpell.TabIndex = 12;
            this.scrlSpell.Value = -1;
            this.scrlSpell.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlSpell_Scroll);
            this.scrlSpell.ScrollOrientation = DarkScrollOrientation.Horizontal;
            this.scrlSpell.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlSpell_Scroll);
            this.scrlSpell.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // lblSpell
            // 
            this.lblSpell.AutoSize = true;
            this.lblSpell.Location = new System.Drawing.Point(12, 21);
            this.lblSpell.Name = "lblSpell";
            this.lblSpell.Size = new System.Drawing.Size(71, 13);
            this.lblSpell.TabIndex = 11;
            this.lblSpell.Text = "Spell: 0 None";
            // 
            // grpEvent
            // 
            this.grpEvent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpEvent.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEvent.Controls.Add(this.scrlEvent);
            this.grpEvent.Controls.Add(this.lblEvent);
            this.grpEvent.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEvent.Location = new System.Drawing.Point(4, 231);
            this.grpEvent.Name = "grpEvent";
            this.grpEvent.Size = new System.Drawing.Size(217, 69);
            this.grpEvent.TabIndex = 42;
            this.grpEvent.TabStop = false;
            this.grpEvent.Text = "Event";
            this.grpEvent.Visible = false;
            // 
            // scrlEvent
            // 
            
            this.scrlEvent.Location = new System.Drawing.Point(8, 37);
            this.scrlEvent.Minimum = -1;
            this.scrlEvent.Name = "scrlEvent";
            this.scrlEvent.Size = new System.Drawing.Size(187, 18);
            this.scrlEvent.TabIndex = 17;
            this.scrlEvent.Value = -1;
            this.scrlEvent.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlEvent_Scroll);
            this.scrlEvent.ScrollOrientation = DarkScrollOrientation.Horizontal;
            // 
            // lblEvent
            // 
            this.lblEvent.AutoSize = true;
            this.lblEvent.Location = new System.Drawing.Point(8, 23);
            this.lblEvent.Name = "lblEvent";
            this.lblEvent.Size = new System.Drawing.Size(67, 13);
            this.lblEvent.TabIndex = 16;
            this.lblEvent.Text = "Event: None";
            // 
            // pnlContainer
            // 
            this.pnlContainer.AutoScroll = true;
            this.pnlContainer.Controls.Add(this.groupBox2);
            this.pnlContainer.Controls.Add(this.groupBox3);
            this.pnlContainer.Controls.Add(this.gbEquipment);
            this.pnlContainer.Controls.Add(this.gbSpell);
            this.pnlContainer.Controls.Add(this.grpEvent);
            this.pnlContainer.Controls.Add(this.gbConsumable);
            this.pnlContainer.Location = new System.Drawing.Point(221, 34);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(463, 442);
            this.pnlContainer.TabIndex = 43;
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
            // FrmItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(686, 517);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "FrmItem";
            this.Text = "Item Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmItem_FormClosed);
            this.Load += new System.EventHandler(this.frmItem_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbEquipment.ResumeLayout(false);
            this.gbEquipment.PerformLayout();
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picFemalePaperdoll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMalePaperdoll)).EndInit();
            this.grpWeaponProperties.ResumeLayout(false);
            this.grpWeaponProperties.PerformLayout();
            this.gbConsumable.ResumeLayout(false);
            this.gbConsumable.PerformLayout();
            this.gbSpell.ResumeLayout(false);
            this.gbSpell.PerformLayout();
            this.grpEvent.ResumeLayout(false);
            this.grpEvent.PerformLayout();
            this.pnlContainer.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private ListBox lstItems;
        private Label label1;
        private Label label2;
        private DarkScrollBar scrlAnim;
        private Label lblAnim;
        private DarkScrollBar scrlPrice;
        private Label lblPrice;
        private PictureBox picItem;
        private Label label3;
        private Label lblLevelReq;
        private DarkScrollBar scrlLevelReq;
        private Label lblSpeedReq;
        private Label lblMagicResistReq;
        private Label lblDefenseReq;
        private Label lblAbilityPowerReq;
        private Label lblAttackReq;
        private DarkScrollBar scrlDefenseReq;
        private DarkScrollBar scrlSpeedReq;
        private DarkScrollBar scrlMagicResistReq;
        private DarkScrollBar scrlAbilityPowerReq;
        private DarkScrollBar scrlAttackReq;
        private Label lblDamage;
        private DarkScrollBar scrlDamage;
        private Label lblSpeed;
        private Label lblMagicResist;
        private Label lblDefense;
        private Label lblAbilityPower;
        private Label lblAttack;
        private DarkScrollBar scrlDefense;
        private DarkScrollBar scrlSpeed;
        private DarkScrollBar scrlMagicResist;
        private DarkScrollBar scrlAbilityPower;
        private DarkScrollBar scrlAttack;
        private PictureBox picMalePaperdoll;
        private DarkScrollBar scrlInterval;
        private Label lblInterval;
        private Label label4;
        private DarkScrollBar scrlSpell;
        private Label lblSpell;
        private Label lblRange;
        private DarkScrollBar scrlRange;
        private Label lblPic;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label lblEffectPercent;
        private DarkScrollBar scrlEffectAmount;
        private Label label9;
        private Label lblToolType;
        private Label lblProjectile;
        private DarkScrollBar scrlEvent;
        private Label lblEvent;
        private Panel pnlContainer;
        private Label label8;
        private Label label10;
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
        private DarkUI.Controls.DarkGroupBox darkGroupBox1;
        private DarkUI.Controls.DarkGroupBox grpWeaponProperties;
        private Label lblScaling;
        private DarkScrollBar scrlScaling;
        private Label label11;
        private Label lblCritChance;
        private DarkScrollBar scrlCritChance;
        private Label label12;
        private DarkGroupBox groupBox1;
        private DarkButton btnSave;
        private DarkGroupBox groupBox2;
        private DarkTextBox txtName;
        private DarkComboBox cmbType;
        private DarkGroupBox groupBox3;
        private DarkComboBox cmbClass;
        private DarkGroupBox gbEquipment;
        private DarkGroupBox gbConsumable;
        private DarkComboBox cmbConsume;
        private DarkGroupBox gbSpell;
        private DarkButton btnCancel;
        private DarkComboBox cmbPic;
        private DarkComboBox cmbMalePaperdoll;
        private DarkTextBox txtDesc;
        private DarkCheckBox chk2Hand;
        private DarkComboBox cmbEquipmentSlot;
        private DarkComboBox cmbEquipmentBonus;
        private DarkComboBox cmbToolType;
        private DarkGroupBox grpEvent;
        private DarkComboBox cmbGender;
        private DarkComboBox cmbFemalePaperdoll;
        private DarkComboBox cmbAttackAnimation;
        private DarkComboBox cmbDamageType;
        private DarkComboBox cmbScalingStat;
        private DarkComboBox cmbProjectile;
        private DarkToolStrip toolStrip;
    }
}