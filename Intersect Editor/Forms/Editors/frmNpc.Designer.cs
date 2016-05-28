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
            this.lblSpd = new System.Windows.Forms.Label();
            this.lblMR = new System.Windows.Forms.Label();
            this.lblDef = new System.Windows.Forms.Label();
            this.lblMag = new System.Windows.Forms.Label();
            this.lblStr = new System.Windows.Forms.Label();
            this.scrlSpd = new System.Windows.Forms.HScrollBar();
            this.scrlMR = new System.Windows.Forms.HScrollBar();
            this.scrlDef = new System.Windows.Forms.HScrollBar();
            this.scrlMag = new System.Windows.Forms.HScrollBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnUndo = new System.Windows.Forms.Button();
            this.lstNpcs = new System.Windows.Forms.ListBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.scrlSightRange = new System.Windows.Forms.HScrollBar();
            this.lblSightRange = new System.Windows.Forms.Label();
            this.cmbSprite = new System.Windows.Forms.ComboBox();
            this.scrlSpawnDuration = new System.Windows.Forms.HScrollBar();
            this.lblSpawnDuration = new System.Windows.Forms.Label();
            this.lblPic = new System.Windows.Forms.Label();
            this.picNpc = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBehavior = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtExp = new System.Windows.Forms.TextBox();
            this.txtMana = new System.Windows.Forms.TextBox();
            this.txtHP = new System.Windows.Forms.TextBox();
            this.lblMana = new System.Windows.Forms.Label();
            this.lblHP = new System.Windows.Forms.Label();
            this.lblExp = new System.Windows.Forms.Label();
            this.scrlStr = new System.Windows.Forms.HScrollBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtDropAmount = new System.Windows.Forms.TextBox();
            this.lblDropAmount = new System.Windows.Forms.Label();
            this.scrlDropChance = new System.Windows.Forms.HScrollBar();
            this.lblDropChance = new System.Windows.Forms.Label();
            this.scrlDropItem = new System.Windows.Forms.HScrollBar();
            this.lblDropItem = new System.Windows.Forms.Label();
            this.scrlDropIndex = new System.Windows.Forms.HScrollBar();
            this.lblDropIndex = new System.Windows.Forms.Label();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNpc)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSpd
            // 
            this.lblSpd.AutoSize = true;
            this.lblSpd.Location = new System.Drawing.Point(15, 266);
            this.lblSpd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpd.Name = "lblSpd";
            this.lblSpd.Size = new System.Drawing.Size(115, 20);
            this.lblSpd.TabIndex = 9;
            this.lblSpd.Text = "Move Speed: 0";
            // 
            // lblMR
            // 
            this.lblMR.AutoSize = true;
            this.lblMR.Location = new System.Drawing.Point(186, 188);
            this.lblMR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMR.Name = "lblMR";
            this.lblMR.Size = new System.Drawing.Size(117, 20);
            this.lblMR.TabIndex = 8;
            this.lblMR.Text = "Magic Resist: 0";
            // 
            // lblDef
            // 
            this.lblDef.AutoSize = true;
            this.lblDef.Location = new System.Drawing.Point(15, 188);
            this.lblDef.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDef.Name = "lblDef";
            this.lblDef.Size = new System.Drawing.Size(69, 20);
            this.lblDef.TabIndex = 7;
            this.lblDef.Text = "Armor: 0";
            // 
            // lblMag
            // 
            this.lblMag.AutoSize = true;
            this.lblMag.Location = new System.Drawing.Point(186, 111);
            this.lblMag.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMag.Name = "lblMag";
            this.lblMag.Size = new System.Drawing.Size(68, 20);
            this.lblMag.TabIndex = 6;
            this.lblMag.Text = "Magic: 0";
            // 
            // lblStr
            // 
            this.lblStr.AutoSize = true;
            this.lblStr.Location = new System.Drawing.Point(15, 111);
            this.lblStr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStr.Name = "lblStr";
            this.lblStr.Size = new System.Drawing.Size(88, 20);
            this.lblStr.TabIndex = 5;
            this.lblStr.Text = "Strength: 0";
            // 
            // scrlSpd
            // 
            this.scrlSpd.LargeChange = 1;
            this.scrlSpd.Location = new System.Drawing.Point(20, 286);
            this.scrlSpd.Maximum = 255;
            this.scrlSpd.Name = "scrlSpd";
            this.scrlSpd.Size = new System.Drawing.Size(120, 17);
            this.scrlSpd.TabIndex = 3;
            this.scrlSpd.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpd_Scroll);
            // 
            // scrlMR
            // 
            this.scrlMR.LargeChange = 1;
            this.scrlMR.Location = new System.Drawing.Point(190, 211);
            this.scrlMR.Maximum = 255;
            this.scrlMR.Name = "scrlMR";
            this.scrlMR.Size = new System.Drawing.Size(120, 17);
            this.scrlMR.TabIndex = 2;
            this.scrlMR.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMR_Scroll);
            // 
            // scrlDef
            // 
            this.scrlDef.LargeChange = 1;
            this.scrlDef.Location = new System.Drawing.Point(20, 211);
            this.scrlDef.Maximum = 255;
            this.scrlDef.Name = "scrlDef";
            this.scrlDef.Size = new System.Drawing.Size(120, 17);
            this.scrlDef.TabIndex = 4;
            this.scrlDef.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDef_Scroll);
            // 
            // scrlMag
            // 
            this.scrlMag.LargeChange = 1;
            this.scrlMag.Location = new System.Drawing.Point(190, 140);
            this.scrlMag.Maximum = 255;
            this.scrlMag.Name = "scrlMag";
            this.scrlMag.Size = new System.Drawing.Size(120, 17);
            this.scrlMag.TabIndex = 1;
            this.scrlMag.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMag_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnUndo);
            this.groupBox1.Controls.Add(this.lstNpcs);
            this.groupBox1.Controls.Add(this.btnNew);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Location = new System.Drawing.Point(18, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(304, 663);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "NPCs";
            // 
            // btnUndo
            // 
            this.btnUndo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnUndo.Location = new System.Drawing.Point(9, 605);
            this.btnUndo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(285, 42);
            this.btnUndo.TabIndex = 22;
            this.btnUndo.Text = "Undo Changes";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // lstNpcs
            // 
            this.lstNpcs.FormattingEnabled = true;
            this.lstNpcs.ItemHeight = 20;
            this.lstNpcs.Location = new System.Drawing.Point(9, 29);
            this.lstNpcs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstNpcs.Name = "lstNpcs";
            this.lstNpcs.Size = new System.Drawing.Size(284, 464);
            this.lstNpcs.TabIndex = 1;
            this.lstNpcs.Click += new System.EventHandler(this.lstNpcs_Click);
            // 
            // btnNew
            // 
            this.btnNew.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNew.Location = new System.Drawing.Point(9, 503);
            this.btnNew.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(285, 42);
            this.btnNew.TabIndex = 20;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.Location = new System.Drawing.Point(10, 554);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(285, 42);
            this.btnDelete.TabIndex = 19;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
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
            this.groupBox2.Location = new System.Drawing.Point(3, 2);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(310, 338);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // scrlSightRange
            // 
            this.scrlSightRange.LargeChange = 1;
            this.scrlSightRange.Location = new System.Drawing.Point(9, 289);
            this.scrlSightRange.Maximum = 20;
            this.scrlSightRange.Name = "scrlSightRange";
            this.scrlSightRange.Size = new System.Drawing.Size(279, 18);
            this.scrlSightRange.TabIndex = 13;
            this.scrlSightRange.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSightRange_Scroll);
            // 
            // lblSightRange
            // 
            this.lblSightRange.AutoSize = true;
            this.lblSightRange.Location = new System.Drawing.Point(9, 266);
            this.lblSightRange.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSightRange.Name = "lblSightRange";
            this.lblSightRange.Size = new System.Drawing.Size(115, 20);
            this.lblSightRange.TabIndex = 12;
            this.lblSightRange.Text = "Sight Range: 0";
            // 
            // cmbSprite
            // 
            this.cmbSprite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSprite.FormattingEnabled = true;
            this.cmbSprite.Items.AddRange(new object[] {
            "None"});
            this.cmbSprite.Location = new System.Drawing.Point(118, 138);
            this.cmbSprite.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbSprite.Name = "cmbSprite";
            this.cmbSprite.Size = new System.Drawing.Size(172, 28);
            this.cmbSprite.TabIndex = 11;
            this.cmbSprite.SelectedIndexChanged += new System.EventHandler(this.cmbSprite_SelectedIndexChanged);
            // 
            // scrlSpawnDuration
            // 
            this.scrlSpawnDuration.Location = new System.Drawing.Point(9, 228);
            this.scrlSpawnDuration.Maximum = 3600;
            this.scrlSpawnDuration.Name = "scrlSpawnDuration";
            this.scrlSpawnDuration.Size = new System.Drawing.Size(279, 18);
            this.scrlSpawnDuration.TabIndex = 8;
            this.scrlSpawnDuration.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpawnDuration_Scroll);
            // 
            // lblSpawnDuration
            // 
            this.lblSpawnDuration.AutoSize = true;
            this.lblSpawnDuration.Location = new System.Drawing.Point(9, 205);
            this.lblSpawnDuration.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSpawnDuration.Name = "lblSpawnDuration";
            this.lblSpawnDuration.Size = new System.Drawing.Size(140, 20);
            this.lblSpawnDuration.TabIndex = 7;
            this.lblSpawnDuration.Text = "Spawn Duration: 0";
            // 
            // lblPic
            // 
            this.lblPic.AutoSize = true;
            this.lblPic.Location = new System.Drawing.Point(114, 114);
            this.lblPic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPic.Name = "lblPic";
            this.lblPic.Size = new System.Drawing.Size(55, 20);
            this.lblPic.TabIndex = 6;
            this.lblPic.Text = "Sprite:";
            // 
            // picNpc
            // 
            this.picNpc.BackColor = System.Drawing.Color.Black;
            this.picNpc.Location = new System.Drawing.Point(9, 102);
            this.picNpc.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.picNpc.Name = "picNpc";
            this.picNpc.Size = new System.Drawing.Size(96, 98);
            this.picNpc.TabIndex = 4;
            this.picNpc.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 74);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 20);
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
            this.cmbBehavior.Location = new System.Drawing.Point(90, 69);
            this.cmbBehavior.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbBehavior.Name = "cmbBehavior";
            this.cmbBehavior.Size = new System.Drawing.Size(200, 28);
            this.cmbBehavior.TabIndex = 2;
            this.cmbBehavior.SelectedIndexChanged += new System.EventHandler(this.cmbBehavior_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(90, 29);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 26);
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
            this.groupBox3.Location = new System.Drawing.Point(322, 2);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(339, 338);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Stats:";
            // 
            // txtExp
            // 
            this.txtExp.Location = new System.Drawing.Point(190, 286);
            this.txtExp.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtExp.Name = "txtExp";
            this.txtExp.Size = new System.Drawing.Size(114, 26);
            this.txtExp.TabIndex = 17;
            this.txtExp.TextChanged += new System.EventHandler(this.txtExp_TextChanged);
            // 
            // txtMana
            // 
            this.txtMana.Location = new System.Drawing.Point(190, 54);
            this.txtMana.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMana.Name = "txtMana";
            this.txtMana.Size = new System.Drawing.Size(114, 26);
            this.txtMana.TabIndex = 16;
            this.txtMana.TextChanged += new System.EventHandler(this.txtMana_TextChanged);
            // 
            // txtHP
            // 
            this.txtHP.Location = new System.Drawing.Point(20, 54);
            this.txtHP.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtHP.Name = "txtHP";
            this.txtHP.Size = new System.Drawing.Size(114, 26);
            this.txtHP.TabIndex = 14;
            this.txtHP.TextChanged += new System.EventHandler(this.txtHP_TextChanged);
            // 
            // lblMana
            // 
            this.lblMana.AutoSize = true;
            this.lblMana.Location = new System.Drawing.Point(186, 29);
            this.lblMana.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMana.Name = "lblMana";
            this.lblMana.Size = new System.Drawing.Size(53, 20);
            this.lblMana.TabIndex = 15;
            this.lblMana.Text = "Mana:";
            // 
            // lblHP
            // 
            this.lblHP.AutoSize = true;
            this.lblHP.Location = new System.Drawing.Point(15, 29);
            this.lblHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHP.Name = "lblHP";
            this.lblHP.Size = new System.Drawing.Size(35, 20);
            this.lblHP.TabIndex = 14;
            this.lblHP.Text = "HP:";
            // 
            // lblExp
            // 
            this.lblExp.AutoSize = true;
            this.lblExp.Location = new System.Drawing.Point(186, 266);
            this.lblExp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExp.Name = "lblExp";
            this.lblExp.Size = new System.Drawing.Size(40, 20);
            this.lblExp.TabIndex = 11;
            this.lblExp.Text = "Exp:";
            // 
            // scrlStr
            // 
            this.scrlStr.LargeChange = 1;
            this.scrlStr.Location = new System.Drawing.Point(20, 140);
            this.scrlStr.Maximum = 255;
            this.scrlStr.Name = "scrlStr";
            this.scrlStr.Size = new System.Drawing.Size(120, 17);
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
            this.groupBox4.Location = new System.Drawing.Point(3, 351);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox4.Size = new System.Drawing.Size(310, 255);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Drops";
            // 
            // txtDropAmount
            // 
            this.txtDropAmount.Location = new System.Drawing.Point(14, 152);
            this.txtDropAmount.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtDropAmount.Name = "txtDropAmount";
            this.txtDropAmount.Size = new System.Drawing.Size(272, 26);
            this.txtDropAmount.TabIndex = 16;
            this.txtDropAmount.TextChanged += new System.EventHandler(this.txtDropAmount_TextChanged);
            // 
            // lblDropAmount
            // 
            this.lblDropAmount.AutoSize = true;
            this.lblDropAmount.Location = new System.Drawing.Point(9, 126);
            this.lblDropAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDropAmount.Name = "lblDropAmount";
            this.lblDropAmount.Size = new System.Drawing.Size(69, 20);
            this.lblDropAmount.TabIndex = 15;
            this.lblDropAmount.Text = "Amount:";
            // 
            // scrlDropChance
            // 
            this.scrlDropChance.LargeChange = 1;
            this.scrlDropChance.Location = new System.Drawing.Point(9, 208);
            this.scrlDropChance.Name = "scrlDropChance";
            this.scrlDropChance.Size = new System.Drawing.Size(279, 18);
            this.scrlDropChance.TabIndex = 14;
            this.scrlDropChance.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDropChance_Scroll);
            // 
            // lblDropChance
            // 
            this.lblDropChance.AutoSize = true;
            this.lblDropChance.Location = new System.Drawing.Point(9, 185);
            this.lblDropChance.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDropChance.Name = "lblDropChance";
            this.lblDropChance.Size = new System.Drawing.Size(122, 20);
            this.lblDropChance.TabIndex = 13;
            this.lblDropChance.Text = "Chance (0/100):";
            // 
            // scrlDropItem
            // 
            this.scrlDropItem.LargeChange = 1;
            this.scrlDropItem.Location = new System.Drawing.Point(9, 98);
            this.scrlDropItem.Maximum = 3600;
            this.scrlDropItem.Minimum = -1;
            this.scrlDropItem.Name = "scrlDropItem";
            this.scrlDropItem.Size = new System.Drawing.Size(279, 18);
            this.scrlDropItem.TabIndex = 12;
            this.scrlDropItem.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDropItem_Scroll);
            // 
            // lblDropItem
            // 
            this.lblDropItem.AutoSize = true;
            this.lblDropItem.Location = new System.Drawing.Point(9, 75);
            this.lblDropItem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDropItem.Name = "lblDropItem";
            this.lblDropItem.Size = new System.Drawing.Size(58, 20);
            this.lblDropItem.TabIndex = 11;
            this.lblDropItem.Text = "Item: 1";
            // 
            // scrlDropIndex
            // 
            this.scrlDropIndex.LargeChange = 1;
            this.scrlDropIndex.Location = new System.Drawing.Point(9, 48);
            this.scrlDropIndex.Maximum = 9;
            this.scrlDropIndex.Name = "scrlDropIndex";
            this.scrlDropIndex.Size = new System.Drawing.Size(279, 18);
            this.scrlDropIndex.TabIndex = 10;
            this.scrlDropIndex.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDropIndex_Scroll);
            // 
            // lblDropIndex
            // 
            this.lblDropIndex.AutoSize = true;
            this.lblDropIndex.Location = new System.Drawing.Point(9, 25);
            this.lblDropIndex.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDropIndex.Name = "lblDropIndex";
            this.lblDropIndex.Size = new System.Drawing.Size(104, 20);
            this.lblDropIndex.TabIndex = 9;
            this.lblDropIndex.Text = "Drop Index: 1";
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.groupBox4);
            this.pnlContainer.Controls.Add(this.groupBox2);
            this.pnlContainer.Controls.Add(this.groupBox3);
            this.pnlContainer.Location = new System.Drawing.Point(338, 18);
            this.pnlContainer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(669, 618);
            this.pnlContainer.TabIndex = 17;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(714, 646);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(285, 42);
            this.btnCancel.TabIndex = 21;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(420, 646);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(285, 42);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // frmNpc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1008, 695);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "frmNpc";
            this.Text = "NPC Editor";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmNpc_FormClosed);
            this.Load += new System.EventHandler(this.frmNpc_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNpc)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.pnlContainer.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstNpcs;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbSprite;
        private System.Windows.Forms.HScrollBar scrlSpawnDuration;
        private System.Windows.Forms.Label lblSpawnDuration;
        private System.Windows.Forms.Label lblPic;
        private System.Windows.Forms.PictureBox picNpc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBehavior;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.HScrollBar scrlStr;
        private System.Windows.Forms.Label lblMana;
        private System.Windows.Forms.Label lblHP;
        private System.Windows.Forms.Label lblExp;
        private System.Windows.Forms.HScrollBar scrlSightRange;
        private System.Windows.Forms.Label lblSightRange;
        private System.Windows.Forms.TextBox txtExp;
        private System.Windows.Forms.TextBox txtMana;
        private System.Windows.Forms.TextBox txtHP;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblDropAmount;
        private System.Windows.Forms.HScrollBar scrlDropChance;
        private System.Windows.Forms.Label lblDropChance;
        private System.Windows.Forms.HScrollBar scrlDropItem;
        private System.Windows.Forms.Label lblDropItem;
        private System.Windows.Forms.HScrollBar scrlDropIndex;
        private System.Windows.Forms.Label lblDropIndex;
        private System.Windows.Forms.TextBox txtDropAmount;
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
    }
}