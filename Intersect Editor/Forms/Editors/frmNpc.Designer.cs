using DarkUI.Controls;

namespace Intersect_Editor.Forms
{
    partial class frmNpc
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNpc));
            this.lblSpd = new System.Windows.Forms.Label();
            this.lblMR = new System.Windows.Forms.Label();
            this.lblDef = new System.Windows.Forms.Label();
            this.lblMag = new System.Windows.Forms.Label();
            this.lblStr = new System.Windows.Forms.Label();
            this.scrlSpd = new System.Windows.Forms.HScrollBar();
            this.scrlMR = new System.Windows.Forms.HScrollBar();
            this.scrlDef = new System.Windows.Forms.HScrollBar();
            this.scrlMag = new System.Windows.Forms.HScrollBar();
            this.groupBox1 = new DarkGroupBox();
            this.lstNpcs = new System.Windows.Forms.ListBox();
            this.groupBox2 = new DarkGroupBox();
            this.scrlSightRange = new System.Windows.Forms.HScrollBar();
            this.lblSightRange = new System.Windows.Forms.Label();
            this.cmbSprite = new DarkComboBox();
            this.scrlSpawnDuration = new System.Windows.Forms.HScrollBar();
            this.lblSpawnDuration = new System.Windows.Forms.Label();
            this.lblPic = new System.Windows.Forms.Label();
            this.picNpc = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBehavior = new DarkComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new DarkTextBox();
            this.groupBox3 = new DarkGroupBox();
            this.txtExp = new DarkTextBox();
            this.txtMana = new DarkTextBox();
            this.txtHP = new DarkTextBox();
            this.lblMana = new System.Windows.Forms.Label();
            this.lblHP = new System.Windows.Forms.Label();
            this.lblExp = new System.Windows.Forms.Label();
            this.scrlStr = new System.Windows.Forms.HScrollBar();
            this.groupBox4 = new DarkGroupBox();
            this.txtDropAmount = new DarkTextBox();
            this.lblDropAmount = new System.Windows.Forms.Label();
            this.scrlDropChance = new System.Windows.Forms.HScrollBar();
            this.lblDropChance = new System.Windows.Forms.Label();
            this.scrlDropItem = new System.Windows.Forms.HScrollBar();
            this.lblDropItem = new System.Windows.Forms.Label();
            this.scrlDropIndex = new System.Windows.Forms.HScrollBar();
            this.lblDropIndex = new System.Windows.Forms.Label();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.groupBox6 = new DarkGroupBox();
            this.scrlNPC = new System.Windows.Forms.HScrollBar();
            this.lblNPC = new System.Windows.Forms.Label();
            this.btnRemoveAggro = new DarkButton();
            this.btnAddAggro = new DarkButton();
            this.lstAggro = new System.Windows.Forms.ListBox();
            this.chkAttackAllies = new DarkCheckBox();
            this.chkEnabled = new DarkCheckBox();
            this.groupBox5 = new DarkGroupBox();
            this.cmbFreq = new DarkComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.scrlSpell = new System.Windows.Forms.HScrollBar();
            this.lblSpell = new System.Windows.Forms.Label();
            this.btnRemove = new DarkButton();
            this.btnAdd = new DarkButton();
            this.lstSpells = new System.Windows.Forms.ListBox();
            this.btnCancel = new DarkButton();
            this.btnSave = new DarkButton();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripItemNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripItemPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemUndo = new System.Windows.Forms.ToolStripButton();
            this.groupBox7 = new DarkGroupBox();
            this.cmbScalingStat = new DarkComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lblScaling = new System.Windows.Forms.Label();
            this.scrlScaling = new System.Windows.Forms.HScrollBar();
            this.cmbDamageType = new DarkComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.lblCritChance = new System.Windows.Forms.Label();
            this.scrlCritChance = new System.Windows.Forms.HScrollBar();
            this.cmbAttackAnimation = new DarkComboBox();
            this.lblAttackAnimation = new System.Windows.Forms.Label();
            this.lblDamage = new System.Windows.Forms.Label();
            this.scrlDamage = new System.Windows.Forms.HScrollBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNpc)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSpd
            // 
            this.lblSpd.AutoSize = true;
            this.lblSpd.Location = new System.Drawing.Point(10, 149);
            this.lblSpd.Name = "lblSpd";
            this.lblSpd.Size = new System.Drawing.Size(80, 13);
            this.lblSpd.TabIndex = 9;
            this.lblSpd.Text = "Move Speed: 0";
            // 
            // lblMR
            // 
            this.lblMR.AutoSize = true;
            this.lblMR.Location = new System.Drawing.Point(124, 107);
            this.lblMR.Name = "lblMR";
            this.lblMR.Size = new System.Drawing.Size(80, 13);
            this.lblMR.TabIndex = 8;
            this.lblMR.Text = "Magic Resist: 0";
            // 
            // lblDef
            // 
            this.lblDef.AutoSize = true;
            this.lblDef.Location = new System.Drawing.Point(10, 107);
            this.lblDef.Name = "lblDef";
            this.lblDef.Size = new System.Drawing.Size(46, 13);
            this.lblDef.TabIndex = 7;
            this.lblDef.Text = "Armor: 0";
            // 
            // lblMag
            // 
            this.lblMag.AutoSize = true;
            this.lblMag.Location = new System.Drawing.Point(124, 64);
            this.lblMag.Name = "lblMag";
            this.lblMag.Size = new System.Drawing.Size(48, 13);
            this.lblMag.TabIndex = 6;
            this.lblMag.Text = "Magic: 0";
            // 
            // lblStr
            // 
            this.lblStr.AutoSize = true;
            this.lblStr.Location = new System.Drawing.Point(10, 64);
            this.lblStr.Name = "lblStr";
            this.lblStr.Size = new System.Drawing.Size(59, 13);
            this.lblStr.TabIndex = 5;
            this.lblStr.Text = "Strength: 0";
            // 
            // scrlSpd
            // 
            this.scrlSpd.LargeChange = 1;
            this.scrlSpd.Location = new System.Drawing.Point(13, 162);
            this.scrlSpd.Maximum = 255;
            this.scrlSpd.Name = "scrlSpd";
            this.scrlSpd.Size = new System.Drawing.Size(80, 17);
            this.scrlSpd.TabIndex = 3;
            this.scrlSpd.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpd_Scroll);
            // 
            // scrlMR
            // 
            this.scrlMR.LargeChange = 1;
            this.scrlMR.Location = new System.Drawing.Point(127, 122);
            this.scrlMR.Maximum = 255;
            this.scrlMR.Name = "scrlMR";
            this.scrlMR.Size = new System.Drawing.Size(80, 17);
            this.scrlMR.TabIndex = 2;
            this.scrlMR.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMR_Scroll);
            // 
            // scrlDef
            // 
            this.scrlDef.LargeChange = 1;
            this.scrlDef.Location = new System.Drawing.Point(13, 122);
            this.scrlDef.Maximum = 255;
            this.scrlDef.Name = "scrlDef";
            this.scrlDef.Size = new System.Drawing.Size(80, 17);
            this.scrlDef.TabIndex = 4;
            this.scrlDef.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDef_Scroll);
            // 
            // scrlMag
            // 
            this.scrlMag.LargeChange = 1;
            this.scrlMag.Location = new System.Drawing.Point(127, 83);
            this.scrlMag.Maximum = 255;
            this.scrlMag.Name = "scrlMag";
            this.scrlMag.Size = new System.Drawing.Size(80, 17);
            this.scrlMag.TabIndex = 1;
            this.scrlMag.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMag_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstNpcs);
            this.groupBox1.Location = new System.Drawing.Point(12, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 529);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "NPCs";
            // 
            // lstNpcs
            // 
            this.lstNpcs.FormattingEnabled = true;
            this.lstNpcs.Location = new System.Drawing.Point(6, 19);
            this.lstNpcs.Name = "lstNpcs";
            this.lstNpcs.Size = new System.Drawing.Size(191, 498);
            this.lstNpcs.TabIndex = 1;
            this.lstNpcs.Click += new System.EventHandler(this.lstNpcs_Click);
            this.lstNpcs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemList_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.scrlSightRange);
            this.groupBox2.Controls.Add(this.lblSightRange);
            this.groupBox2.Controls.Add(this.cmbSprite);
            this.groupBox2.Controls.Add(this.scrlSpawnDuration);
            this.groupBox2.Controls.Add(this.lblSpawnDuration);
            this.groupBox2.Controls.Add(this.lblPic);
            this.groupBox2.Controls.Add(this.picNpc);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmbBehavior);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Location = new System.Drawing.Point(2, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(207, 220);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // scrlSightRange
            // 
            this.scrlSightRange.LargeChange = 1;
            this.scrlSightRange.Location = new System.Drawing.Point(6, 188);
            this.scrlSightRange.Maximum = 20;
            this.scrlSightRange.Name = "scrlSightRange";
            this.scrlSightRange.Size = new System.Drawing.Size(186, 18);
            this.scrlSightRange.TabIndex = 13;
            this.scrlSightRange.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSightRange_Scroll);
            // 
            // lblSightRange
            // 
            this.lblSightRange.AutoSize = true;
            this.lblSightRange.Location = new System.Drawing.Point(6, 173);
            this.lblSightRange.Name = "lblSightRange";
            this.lblSightRange.Size = new System.Drawing.Size(78, 13);
            this.lblSightRange.TabIndex = 12;
            this.lblSightRange.Text = "Sight Range: 0";
            // 
            // cmbSprite
            // 
            this.cmbSprite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSprite.FormattingEnabled = true;
            this.cmbSprite.Items.AddRange(new object[] {
            "None"});
            this.cmbSprite.Location = new System.Drawing.Point(79, 90);
            this.cmbSprite.Name = "cmbSprite";
            this.cmbSprite.Size = new System.Drawing.Size(116, 21);
            this.cmbSprite.TabIndex = 11;
            this.cmbSprite.SelectedIndexChanged += new System.EventHandler(this.cmbSprite_SelectedIndexChanged);
            // 
            // scrlSpawnDuration
            // 
            this.scrlSpawnDuration.Location = new System.Drawing.Point(6, 148);
            this.scrlSpawnDuration.Maximum = 3600;
            this.scrlSpawnDuration.Name = "scrlSpawnDuration";
            this.scrlSpawnDuration.Size = new System.Drawing.Size(186, 18);
            this.scrlSpawnDuration.TabIndex = 8;
            this.scrlSpawnDuration.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpawnDuration_Scroll);
            // 
            // lblSpawnDuration
            // 
            this.lblSpawnDuration.AutoSize = true;
            this.lblSpawnDuration.Location = new System.Drawing.Point(6, 133);
            this.lblSpawnDuration.Name = "lblSpawnDuration";
            this.lblSpawnDuration.Size = new System.Drawing.Size(95, 13);
            this.lblSpawnDuration.TabIndex = 7;
            this.lblSpawnDuration.Text = "Spawn Duration: 0";
            // 
            // lblPic
            // 
            this.lblPic.AutoSize = true;
            this.lblPic.Location = new System.Drawing.Point(76, 74);
            this.lblPic.Name = "lblPic";
            this.lblPic.Size = new System.Drawing.Size(37, 13);
            this.lblPic.TabIndex = 6;
            this.lblPic.Text = "Sprite:";
            // 
            // picNpc
            // 
            this.picNpc.BackColor = System.Drawing.Color.Black;
            this.picNpc.Location = new System.Drawing.Point(6, 66);
            this.picNpc.Name = "picNpc";
            this.picNpc.Size = new System.Drawing.Size(64, 64);
            this.picNpc.TabIndex = 4;
            this.picNpc.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Behavior:";
            // 
            // cmbBehavior
            // 
            this.cmbBehavior.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBehavior.FormattingEnabled = true;
            this.cmbBehavior.Items.AddRange(new object[] {
            "Attack When Attacked",
            "Attack On Sight",
            "Friendly",
            "Guard"});
            this.cmbBehavior.Location = new System.Drawing.Point(60, 45);
            this.cmbBehavior.Name = "cmbBehavior";
            this.cmbBehavior.Size = new System.Drawing.Size(135, 21);
            this.cmbBehavior.TabIndex = 2;
            this.cmbBehavior.SelectedIndexChanged += new System.EventHandler(this.cmbBehavior_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(60, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(135, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtExp);
            this.groupBox3.Controls.Add(this.txtMana);
            this.groupBox3.Controls.Add(this.txtHP);
            this.groupBox3.Controls.Add(this.lblMana);
            this.groupBox3.Controls.Add(this.lblHP);
            this.groupBox3.Controls.Add(this.lblExp);
            this.groupBox3.Controls.Add(this.lblSpd);
            this.groupBox3.Controls.Add(this.lblMR);
            this.groupBox3.Controls.Add(this.lblDef);
            this.groupBox3.Controls.Add(this.lblMag);
            this.groupBox3.Controls.Add(this.lblStr);
            this.groupBox3.Controls.Add(this.scrlDef);
            this.groupBox3.Controls.Add(this.scrlSpd);
            this.groupBox3.Controls.Add(this.scrlMR);
            this.groupBox3.Controls.Add(this.scrlMag);
            this.groupBox3.Controls.Add(this.scrlStr);
            this.groupBox3.Location = new System.Drawing.Point(215, 1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(226, 194);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Stats:";
            // 
            // txtExp
            // 
            this.txtExp.Location = new System.Drawing.Point(127, 162);
            this.txtExp.Name = "txtExp";
            this.txtExp.Size = new System.Drawing.Size(77, 20);
            this.txtExp.TabIndex = 17;
            this.txtExp.TextChanged += new System.EventHandler(this.txtExp_TextChanged);
            // 
            // txtMana
            // 
            this.txtMana.Location = new System.Drawing.Point(127, 35);
            this.txtMana.Name = "txtMana";
            this.txtMana.Size = new System.Drawing.Size(77, 20);
            this.txtMana.TabIndex = 16;
            this.txtMana.TextChanged += new System.EventHandler(this.txtMana_TextChanged);
            // 
            // txtHP
            // 
            this.txtHP.Location = new System.Drawing.Point(13, 35);
            this.txtHP.Name = "txtHP";
            this.txtHP.Size = new System.Drawing.Size(77, 20);
            this.txtHP.TabIndex = 14;
            this.txtHP.TextChanged += new System.EventHandler(this.txtHP_TextChanged);
            // 
            // lblMana
            // 
            this.lblMana.AutoSize = true;
            this.lblMana.Location = new System.Drawing.Point(124, 19);
            this.lblMana.Name = "lblMana";
            this.lblMana.Size = new System.Drawing.Size(37, 13);
            this.lblMana.TabIndex = 15;
            this.lblMana.Text = "Mana:";
            // 
            // lblHP
            // 
            this.lblHP.AutoSize = true;
            this.lblHP.Location = new System.Drawing.Point(10, 19);
            this.lblHP.Name = "lblHP";
            this.lblHP.Size = new System.Drawing.Size(25, 13);
            this.lblHP.TabIndex = 14;
            this.lblHP.Text = "HP:";
            // 
            // lblExp
            // 
            this.lblExp.AutoSize = true;
            this.lblExp.Location = new System.Drawing.Point(124, 149);
            this.lblExp.Name = "lblExp";
            this.lblExp.Size = new System.Drawing.Size(28, 13);
            this.lblExp.TabIndex = 11;
            this.lblExp.Text = "Exp:";
            // 
            // scrlStr
            // 
            this.scrlStr.LargeChange = 1;
            this.scrlStr.Location = new System.Drawing.Point(13, 83);
            this.scrlStr.Maximum = 255;
            this.scrlStr.Name = "scrlStr";
            this.scrlStr.Size = new System.Drawing.Size(80, 17);
            this.scrlStr.TabIndex = 0;
            this.scrlStr.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlStr_Scroll);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox4.Controls.Add(this.txtDropAmount);
            this.groupBox4.Controls.Add(this.lblDropAmount);
            this.groupBox4.Controls.Add(this.scrlDropChance);
            this.groupBox4.Controls.Add(this.lblDropChance);
            this.groupBox4.Controls.Add(this.scrlDropItem);
            this.groupBox4.Controls.Add(this.lblDropItem);
            this.groupBox4.Controls.Add(this.scrlDropIndex);
            this.groupBox4.Controls.Add(this.lblDropIndex);
            this.groupBox4.Location = new System.Drawing.Point(215, 471);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(226, 166);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Drops";
            // 
            // txtDropAmount
            // 
            this.txtDropAmount.Location = new System.Drawing.Point(9, 99);
            this.txtDropAmount.Name = "txtDropAmount";
            this.txtDropAmount.Size = new System.Drawing.Size(195, 20);
            this.txtDropAmount.TabIndex = 16;
            this.txtDropAmount.TextChanged += new System.EventHandler(this.txtDropAmount_TextChanged);
            // 
            // lblDropAmount
            // 
            this.lblDropAmount.AutoSize = true;
            this.lblDropAmount.Location = new System.Drawing.Point(6, 82);
            this.lblDropAmount.Name = "lblDropAmount";
            this.lblDropAmount.Size = new System.Drawing.Size(46, 13);
            this.lblDropAmount.TabIndex = 15;
            this.lblDropAmount.Text = "Amount:";
            // 
            // scrlDropChance
            // 
            this.scrlDropChance.LargeChange = 1;
            this.scrlDropChance.Location = new System.Drawing.Point(6, 135);
            this.scrlDropChance.Name = "scrlDropChance";
            this.scrlDropChance.Size = new System.Drawing.Size(198, 18);
            this.scrlDropChance.TabIndex = 14;
            this.scrlDropChance.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDropChance_Scroll);
            // 
            // lblDropChance
            // 
            this.lblDropChance.AutoSize = true;
            this.lblDropChance.Location = new System.Drawing.Point(6, 120);
            this.lblDropChance.Name = "lblDropChance";
            this.lblDropChance.Size = new System.Drawing.Size(85, 13);
            this.lblDropChance.TabIndex = 13;
            this.lblDropChance.Text = "Chance (0/100):";
            // 
            // scrlDropItem
            // 
            this.scrlDropItem.LargeChange = 1;
            this.scrlDropItem.Location = new System.Drawing.Point(6, 64);
            this.scrlDropItem.Maximum = 3600;
            this.scrlDropItem.Minimum = -1;
            this.scrlDropItem.Name = "scrlDropItem";
            this.scrlDropItem.Size = new System.Drawing.Size(198, 18);
            this.scrlDropItem.TabIndex = 12;
            this.scrlDropItem.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDropItem_Scroll);
            // 
            // lblDropItem
            // 
            this.lblDropItem.AutoSize = true;
            this.lblDropItem.Location = new System.Drawing.Point(6, 49);
            this.lblDropItem.Name = "lblDropItem";
            this.lblDropItem.Size = new System.Drawing.Size(39, 13);
            this.lblDropItem.TabIndex = 11;
            this.lblDropItem.Text = "Item: 1";
            // 
            // scrlDropIndex
            // 
            this.scrlDropIndex.LargeChange = 1;
            this.scrlDropIndex.Location = new System.Drawing.Point(6, 31);
            this.scrlDropIndex.Maximum = 9;
            this.scrlDropIndex.Name = "scrlDropIndex";
            this.scrlDropIndex.Size = new System.Drawing.Size(198, 18);
            this.scrlDropIndex.TabIndex = 10;
            this.scrlDropIndex.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDropIndex_Scroll);
            // 
            // lblDropIndex
            // 
            this.lblDropIndex.AutoSize = true;
            this.lblDropIndex.Location = new System.Drawing.Point(6, 16);
            this.lblDropIndex.Name = "lblDropIndex";
            this.lblDropIndex.Size = new System.Drawing.Size(71, 13);
            this.lblDropIndex.TabIndex = 9;
            this.lblDropIndex.Text = "Drop Index: 1";
            // 
            // pnlContainer
            // 
            this.pnlContainer.AutoScroll = true;
            this.pnlContainer.Controls.Add(this.groupBox4);
            this.pnlContainer.Controls.Add(this.groupBox7);
            this.pnlContainer.Controls.Add(this.groupBox6);
            this.pnlContainer.Controls.Add(this.groupBox5);
            this.pnlContainer.Controls.Add(this.groupBox2);
            this.pnlContainer.Controls.Add(this.groupBox3);
            this.pnlContainer.Location = new System.Drawing.Point(225, 39);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(464, 529);
            this.pnlContainer.TabIndex = 17;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.scrlNPC);
            this.groupBox6.Controls.Add(this.lblNPC);
            this.groupBox6.Controls.Add(this.btnRemoveAggro);
            this.groupBox6.Controls.Add(this.btnAddAggro);
            this.groupBox6.Controls.Add(this.lstAggro);
            this.groupBox6.Controls.Add(this.chkAttackAllies);
            this.groupBox6.Controls.Add(this.chkEnabled);
            this.groupBox6.Location = new System.Drawing.Point(3, 428);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(206, 234);
            this.groupBox6.TabIndex = 29;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "NPC vs NPC Combat/Hostility List";
            // 
            // scrlNPC
            // 
            this.scrlNPC.LargeChange = 1;
            this.scrlNPC.Location = new System.Drawing.Point(8, 90);
            this.scrlNPC.Maximum = 1000;
            this.scrlNPC.Minimum = -1;
            this.scrlNPC.Name = "scrlNPC";
            this.scrlNPC.Size = new System.Drawing.Size(192, 18);
            this.scrlNPC.TabIndex = 45;
            this.scrlNPC.Value = -1;
            this.scrlNPC.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlNPC_Scroll);
            // 
            // lblNPC
            // 
            this.lblNPC.AutoSize = true;
            this.lblNPC.Location = new System.Drawing.Point(6, 67);
            this.lblNPC.Name = "lblNPC";
            this.lblNPC.Size = new System.Drawing.Size(70, 13);
            this.lblNPC.TabIndex = 44;
            this.lblNPC.Text = "NPC: 0 None";
            // 
            // btnRemoveAggro
            // 
            this.btnRemoveAggro.Location = new System.Drawing.Point(125, 203);
            this.btnRemoveAggro.Name = "btnRemoveAggro";
            this.btnRemoveAggro.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveAggro.TabIndex = 43;
            this.btnRemoveAggro.Text = "Remove";
            this.btnRemoveAggro.Click += new System.EventHandler(this.btnRemoveAggro_Click);
            // 
            // btnAddAggro
            // 
            this.btnAddAggro.Location = new System.Drawing.Point(9, 203);
            this.btnAddAggro.Name = "btnAddAggro";
            this.btnAddAggro.Size = new System.Drawing.Size(75, 23);
            this.btnAddAggro.TabIndex = 42;
            this.btnAddAggro.Text = "Add";
            this.btnAddAggro.Click += new System.EventHandler(this.btnAddAggro_Click);
            // 
            // lstAggro
            // 
            this.lstAggro.FormattingEnabled = true;
            this.lstAggro.Items.AddRange(new object[] {
            "NPC: 0 None"});
            this.lstAggro.Location = new System.Drawing.Point(9, 122);
            this.lstAggro.Name = "lstAggro";
            this.lstAggro.Size = new System.Drawing.Size(191, 69);
            this.lstAggro.TabIndex = 41;
            this.lstAggro.SelectedIndexChanged += new System.EventHandler(this.lstAggro_SelectedIndexChanged);
            // 
            // chkAttackAllies
            // 
            this.chkAttackAllies.AutoSize = true;
            this.chkAttackAllies.Location = new System.Drawing.Point(8, 42);
            this.chkAttackAllies.Name = "chkAttackAllies";
            this.chkAttackAllies.Size = new System.Drawing.Size(90, 17);
            this.chkAttackAllies.TabIndex = 1;
            this.chkAttackAllies.Text = "Attack Allies?";
            this.chkAttackAllies.CheckedChanged += new System.EventHandler(this.chkAttackAllies_CheckedChanged);
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(8, 19);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(71, 17);
            this.chkEnabled.TabIndex = 0;
            this.chkEnabled.Text = "Enabled?";
            this.chkEnabled.CheckedChanged += new System.EventHandler(this.chkEnabled_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cmbFreq);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.scrlSpell);
            this.groupBox5.Controls.Add(this.lblSpell);
            this.groupBox5.Controls.Add(this.btnRemove);
            this.groupBox5.Controls.Add(this.btnAdd);
            this.groupBox5.Controls.Add(this.lstSpells);
            this.groupBox5.Location = new System.Drawing.Point(2, 226);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(207, 193);
            this.groupBox5.TabIndex = 28;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Spells";
            // 
            // cmbFreq
            // 
            this.cmbFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFreq.FormattingEnabled = true;
            this.cmbFreq.Items.AddRange(new object[] {
            "Not Very Often",
            "Not Often",
            "Normal",
            "Often",
            "Very Often"});
            this.cmbFreq.Location = new System.Drawing.Point(47, 166);
            this.cmbFreq.Name = "cmbFreq";
            this.cmbFreq.Size = new System.Drawing.Size(145, 21);
            this.cmbFreq.TabIndex = 42;
            this.cmbFreq.SelectedIndexChanged += new System.EventHandler(this.cmbFreq_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 13);
            this.label4.TabIndex = 41;
            this.label4.Text = "Freq:";
            // 
            // scrlSpell
            // 
            this.scrlSpell.LargeChange = 1;
            this.scrlSpell.Location = new System.Drawing.Point(13, 108);
            this.scrlSpell.Maximum = 1000;
            this.scrlSpell.Minimum = -1;
            this.scrlSpell.Name = "scrlSpell";
            this.scrlSpell.Size = new System.Drawing.Size(179, 18);
            this.scrlSpell.TabIndex = 40;
            this.scrlSpell.Value = -1;
            this.scrlSpell.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpell_Scroll);
            // 
            // lblSpell
            // 
            this.lblSpell.AutoSize = true;
            this.lblSpell.Location = new System.Drawing.Point(10, 87);
            this.lblSpell.Name = "lblSpell";
            this.lblSpell.Size = new System.Drawing.Size(71, 13);
            this.lblSpell.TabIndex = 39;
            this.lblSpell.Text = "Spell: 0 None";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(117, 134);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 38;
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 134);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 37;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstSpells
            // 
            this.lstSpells.FormattingEnabled = true;
            this.lstSpells.Items.AddRange(new object[] {
            "Spell: 0 None"});
            this.lstSpells.Location = new System.Drawing.Point(12, 15);
            this.lstSpells.Name = "lstSpells";
            this.lstSpells.Size = new System.Drawing.Size(180, 69);
            this.lstSpells.TabIndex = 29;
            this.lstSpells.Click += new System.EventHandler(this.lstSpells_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(476, 582);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(271, 582);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStrip
            // 
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
            this.toolStrip.Size = new System.Drawing.Size(689, 25);
            this.toolStrip.TabIndex = 45;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripItemNew
            // 
            this.toolStripItemNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemNew.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemNew.Image")));
            this.toolStripItemNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemNew.Name = "toolStripItemNew";
            this.toolStripItemNew.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemNew.Text = "New";
            this.toolStripItemNew.Click += new System.EventHandler(this.toolStripItemNew_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripItemDelete
            // 
            this.toolStripItemDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemDelete.Enabled = false;
            this.toolStripItemDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemDelete.Image")));
            this.toolStripItemDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemDelete.Name = "toolStripItemDelete";
            this.toolStripItemDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemDelete.Text = "Delete";
            this.toolStripItemDelete.Click += new System.EventHandler(this.toolStripItemDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripItemCopy
            // 
            this.toolStripItemCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemCopy.Enabled = false;
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
            this.toolStripItemPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemPaste.Image")));
            this.toolStripItemPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemPaste.Name = "toolStripItemPaste";
            this.toolStripItemPaste.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemPaste.Text = "Paste";
            this.toolStripItemPaste.Click += new System.EventHandler(this.toolStripItemPaste_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripItemUndo
            // 
            this.toolStripItemUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripItemUndo.Enabled = false;
            this.toolStripItemUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripItemUndo.Image")));
            this.toolStripItemUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripItemUndo.Name = "toolStripItemUndo";
            this.toolStripItemUndo.Size = new System.Drawing.Size(23, 22);
            this.toolStripItemUndo.Text = "Undo";
            this.toolStripItemUndo.Click += new System.EventHandler(this.toolStripItemUndo_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox7.Controls.Add(this.cmbScalingStat);
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Controls.Add(this.lblScaling);
            this.groupBox7.Controls.Add(this.scrlScaling);
            this.groupBox7.Controls.Add(this.cmbDamageType);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.lblCritChance);
            this.groupBox7.Controls.Add(this.scrlCritChance);
            this.groupBox7.Controls.Add(this.cmbAttackAnimation);
            this.groupBox7.Controls.Add(this.lblAttackAnimation);
            this.groupBox7.Controls.Add(this.lblDamage);
            this.groupBox7.Controls.Add(this.scrlDamage);
            this.groupBox7.Location = new System.Drawing.Point(215, 201);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(226, 264);
            this.groupBox7.TabIndex = 17;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Combat";
            // 
            // cmbScalingStat
            // 
            this.cmbScalingStat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScalingStat.FormattingEnabled = true;
            this.cmbScalingStat.Location = new System.Drawing.Point(13, 152);
            this.cmbScalingStat.Name = "cmbScalingStat";
            this.cmbScalingStat.Size = new System.Drawing.Size(191, 21);
            this.cmbScalingStat.TabIndex = 58;
            this.cmbScalingStat.SelectedIndexChanged += new System.EventHandler(this.cmbScalingStat_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 135);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(67, 13);
            this.label12.TabIndex = 57;
            this.label12.Text = "Scaling Stat:";
            // 
            // lblScaling
            // 
            this.lblScaling.AutoSize = true;
            this.lblScaling.Location = new System.Drawing.Point(9, 178);
            this.lblScaling.Name = "lblScaling";
            this.lblScaling.Size = new System.Drawing.Size(107, 13);
            this.lblScaling.TabIndex = 56;
            this.lblScaling.Text = "Scaling Amount: x0.0";
            // 
            // scrlScaling
            // 
            this.scrlScaling.LargeChange = 1;
            this.scrlScaling.Location = new System.Drawing.Point(12, 192);
            this.scrlScaling.Maximum = 10000;
            this.scrlScaling.Name = "scrlScaling";
            this.scrlScaling.Size = new System.Drawing.Size(192, 17);
            this.scrlScaling.TabIndex = 55;
            this.scrlScaling.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlScaling_Scroll);
            // 
            // cmbDamageType
            // 
            this.cmbDamageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDamageType.FormattingEnabled = true;
            this.cmbDamageType.Items.AddRange(new object[] {
            "Physical",
            "Magic",
            "True"});
            this.cmbDamageType.Location = new System.Drawing.Point(13, 111);
            this.cmbDamageType.Name = "cmbDamageType";
            this.cmbDamageType.Size = new System.Drawing.Size(191, 21);
            this.cmbDamageType.TabIndex = 54;
            this.cmbDamageType.SelectedIndexChanged += new System.EventHandler(this.cmbDamageType_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 94);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 13);
            this.label11.TabIndex = 53;
            this.label11.Text = "Damage Type:";
            // 
            // lblCritChance
            // 
            this.lblCritChance.AutoSize = true;
            this.lblCritChance.Location = new System.Drawing.Point(9, 58);
            this.lblCritChance.Name = "lblCritChance";
            this.lblCritChance.Size = new System.Drawing.Size(82, 13);
            this.lblCritChance.TabIndex = 52;
            this.lblCritChance.Text = "Crit Chance: 0%";
            // 
            // scrlCritChance
            // 
            this.scrlCritChance.LargeChange = 1;
            this.scrlCritChance.Location = new System.Drawing.Point(12, 72);
            this.scrlCritChance.Name = "scrlCritChance";
            this.scrlCritChance.Size = new System.Drawing.Size(192, 17);
            this.scrlCritChance.TabIndex = 51;
            this.scrlCritChance.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlCritChance_Scroll);
            // 
            // cmbAttackAnimation
            // 
            this.cmbAttackAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAttackAnimation.FormattingEnabled = true;
            this.cmbAttackAnimation.Location = new System.Drawing.Point(12, 235);
            this.cmbAttackAnimation.Name = "cmbAttackAnimation";
            this.cmbAttackAnimation.Size = new System.Drawing.Size(192, 21);
            this.cmbAttackAnimation.TabIndex = 50;
            this.cmbAttackAnimation.SelectedIndexChanged += new System.EventHandler(this.cmbAttackAnimation_SelectedIndexChanged);
            // 
            // lblAttackAnimation
            // 
            this.lblAttackAnimation.AutoSize = true;
            this.lblAttackAnimation.Location = new System.Drawing.Point(9, 220);
            this.lblAttackAnimation.Name = "lblAttackAnimation";
            this.lblAttackAnimation.Size = new System.Drawing.Size(90, 13);
            this.lblAttackAnimation.TabIndex = 49;
            this.lblAttackAnimation.Text = "Attack Animation:";
            // 
            // lblDamage
            // 
            this.lblDamage.AutoSize = true;
            this.lblDamage.Location = new System.Drawing.Point(9, 18);
            this.lblDamage.Name = "lblDamage";
            this.lblDamage.Size = new System.Drawing.Size(86, 13);
            this.lblDamage.TabIndex = 48;
            this.lblDamage.Text = "Base Damage: 0";
            // 
            // scrlDamage
            // 
            this.scrlDamage.LargeChange = 1;
            this.scrlDamage.Location = new System.Drawing.Point(12, 32);
            this.scrlDamage.Maximum = 10000;
            this.scrlDamage.Name = "scrlDamage";
            this.scrlDamage.Size = new System.Drawing.Size(192, 17);
            this.scrlDamage.TabIndex = 47;
            this.scrlDamage.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDamage_Scroll);
            // 
            // frmNpc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(689, 615);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "frmNpc";
            this.Text = "NPC Editor";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmNpc_FormClosed);
            this.Load += new System.EventHandler(this.frmNpc_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNpc)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.pnlContainer.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSpd;
        private System.Windows.Forms.Label lblMR;
        private System.Windows.Forms.Label lblDef;
        private System.Windows.Forms.Label lblMag;
        private System.Windows.Forms.Label lblStr;
        private System.Windows.Forms.HScrollBar scrlSpd;
        private System.Windows.Forms.HScrollBar scrlMR;
        private System.Windows.Forms.HScrollBar scrlDef;
        private System.Windows.Forms.HScrollBar scrlMag;
        private DarkGroupBox groupBox1;
        private System.Windows.Forms.ListBox lstNpcs;
        private DarkGroupBox groupBox2;
        private DarkComboBox cmbSprite;
        private System.Windows.Forms.HScrollBar scrlSpawnDuration;
        private System.Windows.Forms.Label lblSpawnDuration;
        private System.Windows.Forms.Label lblPic;
        private System.Windows.Forms.PictureBox picNpc;
        private System.Windows.Forms.Label label2;
        private DarkComboBox cmbBehavior;
        private System.Windows.Forms.Label label1;
        private DarkTextBox txtName;
        private DarkGroupBox groupBox3;
        private System.Windows.Forms.HScrollBar scrlStr;
        private System.Windows.Forms.Label lblMana;
        private System.Windows.Forms.Label lblHP;
        private System.Windows.Forms.Label lblExp;
        private System.Windows.Forms.HScrollBar scrlSightRange;
        private System.Windows.Forms.Label lblSightRange;
        private DarkTextBox txtExp;
        private DarkTextBox txtMana;
        private DarkTextBox txtHP;
        private DarkGroupBox groupBox4;
        private System.Windows.Forms.Label lblDropAmount;
        private System.Windows.Forms.HScrollBar scrlDropChance;
        private System.Windows.Forms.Label lblDropChance;
        private System.Windows.Forms.HScrollBar scrlDropItem;
        private System.Windows.Forms.Label lblDropItem;
        private System.Windows.Forms.HScrollBar scrlDropIndex;
        private System.Windows.Forms.Label lblDropIndex;
        private DarkTextBox txtDropAmount;
        private System.Windows.Forms.Panel pnlContainer;
        private DarkButton btnSave;
        private DarkButton btnCancel;
        private DarkGroupBox groupBox5;
        private DarkButton btnRemove;
        private DarkButton btnAdd;
        private System.Windows.Forms.ListBox lstSpells;
        private System.Windows.Forms.HScrollBar scrlSpell;
        private System.Windows.Forms.Label lblSpell;
        private DarkComboBox cmbFreq;
        private System.Windows.Forms.Label label4;
        private DarkGroupBox groupBox6;
        private System.Windows.Forms.HScrollBar scrlNPC;
        private System.Windows.Forms.Label lblNPC;
        private DarkButton btnRemoveAggro;
        private DarkButton btnAddAggro;
        private System.Windows.Forms.ListBox lstAggro;
        private DarkCheckBox chkAttackAllies;
        private DarkCheckBox chkEnabled;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripItemNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripItemDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton toolStripItemCopy;
        public System.Windows.Forms.ToolStripButton toolStripItemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.ToolStripButton toolStripItemUndo;
        private DarkGroupBox groupBox7;
        private DarkComboBox cmbScalingStat;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblScaling;
        private System.Windows.Forms.HScrollBar scrlScaling;
        private DarkComboBox cmbDamageType;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblCritChance;
        private System.Windows.Forms.HScrollBar scrlCritChance;
        private DarkComboBox cmbAttackAnimation;
        private System.Windows.Forms.Label lblAttackAnimation;
        private System.Windows.Forms.Label lblDamage;
        private System.Windows.Forms.HScrollBar scrlDamage;
    }
}