namespace Intersect_Editor.Forms
{
    partial class frmClass
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lstClasses = new System.Windows.Forms.ListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtDropAmount = new System.Windows.Forms.TextBox();
            this.lblDropAmount = new System.Windows.Forms.Label();
            this.scrlDropItem = new System.Windows.Forms.HScrollBar();
            this.lblDropItem = new System.Windows.Forms.Label();
            this.scrlDropIndex = new System.Windows.Forms.HScrollBar();
            this.lblDropIndex = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblPoints = new System.Windows.Forms.Label();
            this.scrlPoints = new System.Windows.Forms.HScrollBar();
            this.txtMana = new System.Windows.Forms.TextBox();
            this.txtHP = new System.Windows.Forms.TextBox();
            this.lblMana = new System.Windows.Forms.Label();
            this.lblHP = new System.Windows.Forms.Label();
            this.lblSpd = new System.Windows.Forms.Label();
            this.lblMR = new System.Windows.Forms.Label();
            this.lblDef = new System.Windows.Forms.Label();
            this.lblMag = new System.Windows.Forms.Label();
            this.lblStr = new System.Windows.Forms.Label();
            this.scrlDef = new System.Windows.Forms.HScrollBar();
            this.scrlSpd = new System.Windows.Forms.HScrollBar();
            this.scrlMR = new System.Windows.Forms.HScrollBar();
            this.scrlMag = new System.Windows.Forms.HScrollBar();
            this.scrlStr = new System.Windows.Forms.HScrollBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.rbFemale = new System.Windows.Forms.RadioButton();
            this.rbMale = new System.Windows.Forms.RadioButton();
            this.lstSprites = new System.Windows.Forms.ListBox();
            this.cmbSprite = new System.Windows.Forms.ComboBox();
            this.lblPic = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblLevel = new System.Windows.Forms.Label();
            this.scrlLevel = new System.Windows.Forms.HScrollBar();
            this.lblSpellNum = new System.Windows.Forms.Label();
            this.scrlSpell = new System.Windows.Forms.HScrollBar();
            this.btnRemoveSpell = new System.Windows.Forms.Button();
            this.btnAddSpell = new System.Windows.Forms.Button();
            this.lstSpells = new System.Windows.Forms.ListBox();
            this.picSprite = new System.Windows.Forms.PictureBox();
            this.grpWarp = new System.Windows.Forms.GroupBox();
            this.btnVisualMapSelector = new System.Windows.Forms.Button();
            this.cmbWarpMap = new System.Windows.Forms.ComboBox();
            this.cmbDirection = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblMap = new System.Windows.Forms.Label();
            this.scrlX = new System.Windows.Forms.HScrollBar();
            this.scrlY = new System.Windows.Forms.HScrollBar();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSprite)).BeginInit();
            this.grpWarp.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.lstClasses);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 403);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Classes";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(6, 360);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.Location = new System.Drawing.Point(6, 327);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(190, 27);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 293);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lstClasses
            // 
            this.lstClasses.FormattingEnabled = true;
            this.lstClasses.Location = new System.Drawing.Point(6, 19);
            this.lstClasses.Name = "lstClasses";
            this.lstClasses.Size = new System.Drawing.Size(191, 264);
            this.lstClasses.TabIndex = 1;
            this.lstClasses.Click += new System.EventHandler(this.lstClasses_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox4.Controls.Add(this.txtDropAmount);
            this.groupBox4.Controls.Add(this.lblDropAmount);
            this.groupBox4.Controls.Add(this.scrlDropItem);
            this.groupBox4.Controls.Add(this.lblDropItem);
            this.groupBox4.Controls.Add(this.scrlDropIndex);
            this.groupBox4.Controls.Add(this.lblDropIndex);
            this.groupBox4.Location = new System.Drawing.Point(640, 238);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(156, 177);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Starting Items";
            // 
            // txtDropAmount
            // 
            this.txtDropAmount.Location = new System.Drawing.Point(9, 99);
            this.txtDropAmount.Name = "txtDropAmount";
            this.txtDropAmount.Size = new System.Drawing.Size(138, 20);
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
            // scrlDropItem
            // 
            this.scrlDropItem.LargeChange = 1;
            this.scrlDropItem.Location = new System.Drawing.Point(6, 64);
            this.scrlDropItem.Maximum = 3600;
            this.scrlDropItem.Name = "scrlDropItem";
            this.scrlDropItem.Size = new System.Drawing.Size(141, 18);
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
            this.scrlDropIndex.Maximum = 10;
            this.scrlDropIndex.Minimum = 1;
            this.scrlDropIndex.Name = "scrlDropIndex";
            this.scrlDropIndex.Size = new System.Drawing.Size(141, 18);
            this.scrlDropIndex.TabIndex = 10;
            this.scrlDropIndex.Value = 1;
            this.scrlDropIndex.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDropIndex_Scroll);
            // 
            // lblDropIndex
            // 
            this.lblDropIndex.AutoSize = true;
            this.lblDropIndex.Location = new System.Drawing.Point(6, 16);
            this.lblDropIndex.Name = "lblDropIndex";
            this.lblDropIndex.Size = new System.Drawing.Size(68, 13);
            this.lblDropIndex.TabIndex = 9;
            this.lblDropIndex.Text = "Item Index: 1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblPoints);
            this.groupBox3.Controls.Add(this.scrlPoints);
            this.groupBox3.Controls.Add(this.txtMana);
            this.groupBox3.Controls.Add(this.txtHP);
            this.groupBox3.Controls.Add(this.lblMana);
            this.groupBox3.Controls.Add(this.lblHP);
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
            this.groupBox3.Location = new System.Drawing.Point(396, 238);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(238, 176);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Base Stats:";
            // 
            // lblPoints
            // 
            this.lblPoints.AutoSize = true;
            this.lblPoints.Location = new System.Drawing.Point(128, 130);
            this.lblPoints.Name = "lblPoints";
            this.lblPoints.Size = new System.Drawing.Size(48, 13);
            this.lblPoints.TabIndex = 18;
            this.lblPoints.Text = "Points: 0";
            // 
            // scrlPoints
            // 
            this.scrlPoints.LargeChange = 1;
            this.scrlPoints.Location = new System.Drawing.Point(131, 143);
            this.scrlPoints.Maximum = 255;
            this.scrlPoints.Name = "scrlPoints";
            this.scrlPoints.Size = new System.Drawing.Size(90, 17);
            this.scrlPoints.TabIndex = 17;
            this.scrlPoints.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlPoints_Scroll);
            // 
            // txtMana
            // 
            this.txtMana.Location = new System.Drawing.Point(131, 35);
            this.txtMana.Name = "txtMana";
            this.txtMana.Size = new System.Drawing.Size(87, 20);
            this.txtMana.TabIndex = 16;
            this.txtMana.TextChanged += new System.EventHandler(this.txtMana_TextChanged);
            // 
            // txtHP
            // 
            this.txtHP.Location = new System.Drawing.Point(13, 35);
            this.txtHP.Name = "txtHP";
            this.txtHP.Size = new System.Drawing.Size(90, 20);
            this.txtHP.TabIndex = 14;
            this.txtHP.TextChanged += new System.EventHandler(this.txtHP_TextChanged);
            // 
            // lblMana
            // 
            this.lblMana.AutoSize = true;
            this.lblMana.Location = new System.Drawing.Point(128, 19);
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
            // lblSpd
            // 
            this.lblSpd.AutoSize = true;
            this.lblSpd.Location = new System.Drawing.Point(13, 130);
            this.lblSpd.Name = "lblSpd";
            this.lblSpd.Size = new System.Drawing.Size(80, 13);
            this.lblSpd.TabIndex = 9;
            this.lblSpd.Text = "Move Speed: 0";
            // 
            // lblMR
            // 
            this.lblMR.AutoSize = true;
            this.lblMR.Location = new System.Drawing.Point(128, 95);
            this.lblMR.Name = "lblMR";
            this.lblMR.Size = new System.Drawing.Size(80, 13);
            this.lblMR.TabIndex = 8;
            this.lblMR.Text = "Magic Resist: 0";
            // 
            // lblDef
            // 
            this.lblDef.AutoSize = true;
            this.lblDef.Location = new System.Drawing.Point(10, 95);
            this.lblDef.Name = "lblDef";
            this.lblDef.Size = new System.Drawing.Size(46, 13);
            this.lblDef.TabIndex = 7;
            this.lblDef.Text = "Armor: 0";
            // 
            // lblMag
            // 
            this.lblMag.AutoSize = true;
            this.lblMag.Location = new System.Drawing.Point(128, 59);
            this.lblMag.Name = "lblMag";
            this.lblMag.Size = new System.Drawing.Size(48, 13);
            this.lblMag.TabIndex = 6;
            this.lblMag.Text = "Magic: 0";
            // 
            // lblStr
            // 
            this.lblStr.AutoSize = true;
            this.lblStr.Location = new System.Drawing.Point(10, 58);
            this.lblStr.Name = "lblStr";
            this.lblStr.Size = new System.Drawing.Size(59, 13);
            this.lblStr.TabIndex = 5;
            this.lblStr.Text = "Strength: 0";
            // 
            // scrlDef
            // 
            this.scrlDef.LargeChange = 1;
            this.scrlDef.Location = new System.Drawing.Point(13, 108);
            this.scrlDef.Maximum = 255;
            this.scrlDef.Name = "scrlDef";
            this.scrlDef.Size = new System.Drawing.Size(90, 17);
            this.scrlDef.TabIndex = 4;
            this.scrlDef.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDef_Scroll);
            // 
            // scrlSpd
            // 
            this.scrlSpd.LargeChange = 1;
            this.scrlSpd.Location = new System.Drawing.Point(13, 143);
            this.scrlSpd.Maximum = 255;
            this.scrlSpd.Name = "scrlSpd";
            this.scrlSpd.Size = new System.Drawing.Size(90, 17);
            this.scrlSpd.TabIndex = 3;
            this.scrlSpd.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpd_Scroll);
            // 
            // scrlMR
            // 
            this.scrlMR.LargeChange = 1;
            this.scrlMR.Location = new System.Drawing.Point(131, 108);
            this.scrlMR.Maximum = 255;
            this.scrlMR.Name = "scrlMR";
            this.scrlMR.Size = new System.Drawing.Size(90, 17);
            this.scrlMR.TabIndex = 2;
            this.scrlMR.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMR_Scroll);
            // 
            // scrlMag
            // 
            this.scrlMag.LargeChange = 1;
            this.scrlMag.Location = new System.Drawing.Point(131, 72);
            this.scrlMag.Maximum = 255;
            this.scrlMag.Name = "scrlMag";
            this.scrlMag.Size = new System.Drawing.Size(90, 17);
            this.scrlMag.TabIndex = 1;
            this.scrlMag.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMag_Scroll);
            // 
            // scrlStr
            // 
            this.scrlStr.LargeChange = 1;
            this.scrlStr.Location = new System.Drawing.Point(13, 71);
            this.scrlStr.Maximum = 255;
            this.scrlStr.Name = "scrlStr";
            this.scrlStr.Size = new System.Drawing.Size(90, 18);
            this.scrlStr.TabIndex = 0;
            this.scrlStr.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlStr_Scroll);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnRemove);
            this.groupBox2.Controls.Add(this.btnAdd);
            this.groupBox2.Controls.Add(this.rbFemale);
            this.groupBox2.Controls.Add(this.rbMale);
            this.groupBox2.Controls.Add(this.lstSprites);
            this.groupBox2.Controls.Add(this.cmbSprite);
            this.groupBox2.Controls.Add(this.lblPic);
            this.groupBox2.Controls.Add(this.picSprite);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Location = new System.Drawing.Point(221, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(238, 221);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(164, 186);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(66, 23);
            this.btnRemove.TabIndex = 21;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(92, 186);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(66, 23);
            this.btnAdd.TabIndex = 20;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // rbFemale
            // 
            this.rbFemale.AutoSize = true;
            this.rbFemale.Location = new System.Drawing.Point(11, 159);
            this.rbFemale.Name = "rbFemale";
            this.rbFemale.Size = new System.Drawing.Size(59, 17);
            this.rbFemale.TabIndex = 19;
            this.rbFemale.Text = "Female";
            this.rbFemale.UseVisualStyleBackColor = true;
            this.rbFemale.Click += new System.EventHandler(this.rbFemale_Click);
            // 
            // rbMale
            // 
            this.rbMale.AutoSize = true;
            this.rbMale.Checked = true;
            this.rbMale.Location = new System.Drawing.Point(11, 136);
            this.rbMale.Name = "rbMale";
            this.rbMale.Size = new System.Drawing.Size(48, 17);
            this.rbMale.TabIndex = 18;
            this.rbMale.TabStop = true;
            this.rbMale.Text = "Male";
            this.rbMale.UseVisualStyleBackColor = true;
            this.rbMale.Click += new System.EventHandler(this.rbMale_Click);
            // 
            // lstSprites
            // 
            this.lstSprites.FormattingEnabled = true;
            this.lstSprites.Location = new System.Drawing.Point(92, 46);
            this.lstSprites.Name = "lstSprites";
            this.lstSprites.Size = new System.Drawing.Size(138, 134);
            this.lstSprites.TabIndex = 17;
            this.lstSprites.Click += new System.EventHandler(this.lstSprites_Click);
            // 
            // cmbSprite
            // 
            this.cmbSprite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSprite.FormattingEnabled = true;
            this.cmbSprite.Items.AddRange(new object[] {
            "None"});
            this.cmbSprite.Location = new System.Drawing.Point(6, 188);
            this.cmbSprite.Name = "cmbSprite";
            this.cmbSprite.Size = new System.Drawing.Size(80, 21);
            this.cmbSprite.TabIndex = 16;
            this.cmbSprite.SelectedIndexChanged += new System.EventHandler(this.cmbSprite_SelectedIndexChanged);
            // 
            // lblPic
            // 
            this.lblPic.AutoSize = true;
            this.lblPic.Location = new System.Drawing.Point(8, 50);
            this.lblPic.Name = "lblPic";
            this.lblPic.Size = new System.Drawing.Size(42, 13);
            this.lblPic.TabIndex = 15;
            this.lblPic.Text = "Sprites:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(92, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(138, 20);
            this.txtName.TabIndex = 12;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblLevel);
            this.groupBox6.Controls.Add(this.scrlLevel);
            this.groupBox6.Controls.Add(this.lblSpellNum);
            this.groupBox6.Controls.Add(this.scrlSpell);
            this.groupBox6.Controls.Add(this.btnRemoveSpell);
            this.groupBox6.Controls.Add(this.btnAddSpell);
            this.groupBox6.Controls.Add(this.lstSpells);
            this.groupBox6.Location = new System.Drawing.Point(465, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(331, 220);
            this.groupBox6.TabIndex = 21;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Spells";
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(153, 54);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(45, 13);
            this.lblLevel.TabIndex = 25;
            this.lblLevel.Text = "Level: 0";
            // 
            // scrlLevel
            // 
            this.scrlLevel.LargeChange = 1;
            this.scrlLevel.Location = new System.Drawing.Point(156, 67);
            this.scrlLevel.Maximum = 255;
            this.scrlLevel.Name = "scrlLevel";
            this.scrlLevel.Size = new System.Drawing.Size(166, 17);
            this.scrlLevel.TabIndex = 24;
            this.scrlLevel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlLevel_Scroll);
            // 
            // lblSpellNum
            // 
            this.lblSpellNum.AutoSize = true;
            this.lblSpellNum.Location = new System.Drawing.Point(153, 19);
            this.lblSpellNum.Name = "lblSpellNum";
            this.lblSpellNum.Size = new System.Drawing.Size(42, 13);
            this.lblSpellNum.TabIndex = 23;
            this.lblSpellNum.Text = "Spell: 0";
            // 
            // scrlSpell
            // 
            this.scrlSpell.LargeChange = 1;
            this.scrlSpell.Location = new System.Drawing.Point(156, 32);
            this.scrlSpell.Maximum = 255;
            this.scrlSpell.Name = "scrlSpell";
            this.scrlSpell.Size = new System.Drawing.Size(166, 17);
            this.scrlSpell.TabIndex = 22;
            this.scrlSpell.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpell_Scroll);
            // 
            // btnRemoveSpell
            // 
            this.btnRemoveSpell.Location = new System.Drawing.Point(256, 94);
            this.btnRemoveSpell.Name = "btnRemoveSpell";
            this.btnRemoveSpell.Size = new System.Drawing.Size(66, 23);
            this.btnRemoveSpell.TabIndex = 21;
            this.btnRemoveSpell.Text = "Remove";
            this.btnRemoveSpell.UseVisualStyleBackColor = true;
            this.btnRemoveSpell.Click += new System.EventHandler(this.btnRemoveSpell_Click);
            // 
            // btnAddSpell
            // 
            this.btnAddSpell.Location = new System.Drawing.Point(153, 94);
            this.btnAddSpell.Name = "btnAddSpell";
            this.btnAddSpell.Size = new System.Drawing.Size(66, 23);
            this.btnAddSpell.TabIndex = 20;
            this.btnAddSpell.Text = "Add";
            this.btnAddSpell.UseVisualStyleBackColor = true;
            this.btnAddSpell.Click += new System.EventHandler(this.btnAddSpell_Click);
            // 
            // lstSpells
            // 
            this.lstSpells.FormattingEnabled = true;
            this.lstSpells.Location = new System.Drawing.Point(9, 19);
            this.lstSpells.Name = "lstSpells";
            this.lstSpells.Size = new System.Drawing.Size(138, 173);
            this.lstSpells.TabIndex = 17;
            this.lstSpells.Click += new System.EventHandler(this.lstSpells_Click);
            // 
            // picSprite
            // 
            this.picSprite.BackColor = System.Drawing.Color.Black;
            this.picSprite.Location = new System.Drawing.Point(11, 66);
            this.picSprite.Name = "picSprite";
            this.picSprite.Size = new System.Drawing.Size(64, 64);
            this.picSprite.TabIndex = 14;
            this.picSprite.TabStop = false;
            // 
            // grpWarp
            // 
            this.grpWarp.Controls.Add(this.btnVisualMapSelector);
            this.grpWarp.Controls.Add(this.cmbWarpMap);
            this.grpWarp.Controls.Add(this.cmbDirection);
            this.grpWarp.Controls.Add(this.label23);
            this.grpWarp.Controls.Add(this.lblY);
            this.grpWarp.Controls.Add(this.lblX);
            this.grpWarp.Controls.Add(this.lblMap);
            this.grpWarp.Controls.Add(this.scrlX);
            this.grpWarp.Controls.Add(this.scrlY);
            this.grpWarp.Location = new System.Drawing.Point(221, 238);
            this.grpWarp.Name = "grpWarp";
            this.grpWarp.Size = new System.Drawing.Size(169, 176);
            this.grpWarp.TabIndex = 27;
            this.grpWarp.TabStop = false;
            this.grpWarp.Text = "Spawn Point";
            // 
            // btnVisualMapSelector
            // 
            this.btnVisualMapSelector.Location = new System.Drawing.Point(16, 131);
            this.btnVisualMapSelector.Name = "btnVisualMapSelector";
            this.btnVisualMapSelector.Size = new System.Drawing.Size(142, 23);
            this.btnVisualMapSelector.TabIndex = 24;
            this.btnVisualMapSelector.Text = "Open Visual Interface";
            this.btnVisualMapSelector.UseVisualStyleBackColor = true;
            this.btnVisualMapSelector.Click += new System.EventHandler(this.btnVisualMapSelector_Click);
            // 
            // cmbWarpMap
            // 
            this.cmbWarpMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWarpMap.FormattingEnabled = true;
            this.cmbWarpMap.Location = new System.Drawing.Point(17, 30);
            this.cmbWarpMap.Name = "cmbWarpMap";
            this.cmbWarpMap.Size = new System.Drawing.Size(141, 21);
            this.cmbWarpMap.TabIndex = 12;
            this.cmbWarpMap.SelectedIndexChanged += new System.EventHandler(this.cmbWarpMap_SelectedIndexChanged);
            // 
            // cmbDirection
            // 
            this.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirection.FormattingEnabled = true;
            this.cmbDirection.Items.AddRange(new object[] {
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbDirection.Location = new System.Drawing.Point(46, 102);
            this.cmbDirection.Name = "cmbDirection";
            this.cmbDirection.Size = new System.Drawing.Size(112, 21);
            this.cmbDirection.TabIndex = 23;
            this.cmbDirection.SelectedIndexChanged += new System.EventHandler(this.cmbDirection_SelectedIndexChanged);
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(13, 105);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(23, 13);
            this.label23.TabIndex = 22;
            this.label23.Text = "Dir:";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(14, 82);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(26, 13);
            this.lblY.TabIndex = 11;
            this.lblY.Text = "Y: 0";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(14, 61);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(26, 13);
            this.lblX.TabIndex = 10;
            this.lblX.Text = "X: 0";
            // 
            // lblMap
            // 
            this.lblMap.AutoSize = true;
            this.lblMap.Location = new System.Drawing.Point(13, 14);
            this.lblMap.Name = "lblMap";
            this.lblMap.Size = new System.Drawing.Size(31, 13);
            this.lblMap.TabIndex = 9;
            this.lblMap.Text = "Map:";
            // 
            // scrlX
            // 
            this.scrlX.LargeChange = 1;
            this.scrlX.Location = new System.Drawing.Point(46, 54);
            this.scrlX.Name = "scrlX";
            this.scrlX.Size = new System.Drawing.Size(112, 21);
            this.scrlX.TabIndex = 8;
            this.scrlX.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlX_Scroll);
            // 
            // scrlY
            // 
            this.scrlY.LargeChange = 1;
            this.scrlY.Location = new System.Drawing.Point(46, 78);
            this.scrlY.Name = "scrlY";
            this.scrlY.Size = new System.Drawing.Size(112, 21);
            this.scrlY.TabIndex = 7;
            this.scrlY.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlY_Scroll);
            // 
            // frmClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 426);
            this.ControlBox = false;
            this.Controls.Add(this.grpWarp);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmClass";
            this.Text = "Class Editor";
            this.Load += new System.EventHandler(this.frmClass_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSprite)).EndInit();
            this.grpWarp.ResumeLayout(false);
            this.grpWarp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lstClasses;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtDropAmount;
        private System.Windows.Forms.Label lblDropAmount;
        private System.Windows.Forms.HScrollBar scrlDropItem;
        private System.Windows.Forms.Label lblDropItem;
        private System.Windows.Forms.HScrollBar scrlDropIndex;
        private System.Windows.Forms.Label lblDropIndex;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtMana;
        private System.Windows.Forms.TextBox txtHP;
        private System.Windows.Forms.Label lblMana;
        private System.Windows.Forms.Label lblHP;
        private System.Windows.Forms.Label lblSpd;
        private System.Windows.Forms.Label lblMR;
        private System.Windows.Forms.Label lblDef;
        private System.Windows.Forms.Label lblMag;
        private System.Windows.Forms.Label lblStr;
        private System.Windows.Forms.HScrollBar scrlDef;
        private System.Windows.Forms.HScrollBar scrlSpd;
        private System.Windows.Forms.HScrollBar scrlMR;
        private System.Windows.Forms.HScrollBar scrlMag;
        private System.Windows.Forms.HScrollBar scrlStr;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstSprites;
        private System.Windows.Forms.ComboBox cmbSprite;
        private System.Windows.Forms.Label lblPic;
        private System.Windows.Forms.PictureBox picSprite;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.RadioButton rbFemale;
        private System.Windows.Forms.RadioButton rbMale;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.HScrollBar scrlLevel;
        private System.Windows.Forms.Label lblSpellNum;
        private System.Windows.Forms.HScrollBar scrlSpell;
        private System.Windows.Forms.Button btnRemoveSpell;
        private System.Windows.Forms.Button btnAddSpell;
        private System.Windows.Forms.ListBox lstSpells;
        private System.Windows.Forms.Label lblPoints;
        private System.Windows.Forms.HScrollBar scrlPoints;
        private System.Windows.Forms.GroupBox grpWarp;
        private System.Windows.Forms.Button btnVisualMapSelector;
        private System.Windows.Forms.ComboBox cmbWarpMap;
        private System.Windows.Forms.ComboBox cmbDirection;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblMap;
        public System.Windows.Forms.HScrollBar scrlX;
        public System.Windows.Forms.HScrollBar scrlY;
    }
}