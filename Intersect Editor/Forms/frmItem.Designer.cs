using System.ComponentModel;
using System.Windows.Forms;

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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lstItems = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbPic = new System.Windows.Forms.ComboBox();
            this.scrlAnim = new System.Windows.Forms.HScrollBar();
            this.lblAnim = new System.Windows.Forms.Label();
            this.scrlPrice = new System.Windows.Forms.HScrollBar();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblPic = new System.Windows.Forms.Label();
            this.picItem = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbClass = new System.Windows.Forms.ComboBox();
            this.lblLevel = new System.Windows.Forms.Label();
            this.scrlLevel = new System.Windows.Forms.HScrollBar();
            this.lblSpdReq = new System.Windows.Forms.Label();
            this.lblMRReq = new System.Windows.Forms.Label();
            this.lblDefReq = new System.Windows.Forms.Label();
            this.lblMagReq = new System.Windows.Forms.Label();
            this.lblStrReq = new System.Windows.Forms.Label();
            this.scrlDefReq = new System.Windows.Forms.HScrollBar();
            this.scrlSpdReq = new System.Windows.Forms.HScrollBar();
            this.scrlMRReq = new System.Windows.Forms.HScrollBar();
            this.scrlMagReq = new System.Windows.Forms.HScrollBar();
            this.scrlStrReq = new System.Windows.Forms.HScrollBar();
            this.gbEquipment = new System.Windows.Forms.GroupBox();
            this.cmbPaperdoll = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblRange = new System.Windows.Forms.Label();
            this.scrlRange = new System.Windows.Forms.HScrollBar();
            this.picPaperdoll = new System.Windows.Forms.PictureBox();
            this.lblTool = new System.Windows.Forms.Label();
            this.scrlTool = new System.Windows.Forms.HScrollBar();
            this.lblAtkSpd = new System.Windows.Forms.Label();
            this.ScrlAtkSpd = new System.Windows.Forms.HScrollBar();
            this.lblDmg = new System.Windows.Forms.Label();
            this.scrlDmg = new System.Windows.Forms.HScrollBar();
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
            this.gbConsumable = new System.Windows.Forms.GroupBox();
            this.gbSpell = new System.Windows.Forms.GroupBox();
            this.scrlSpell = new System.Windows.Forms.HScrollBar();
            this.lblSpell = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbConsume = new System.Windows.Forms.ComboBox();
            this.scrlInterval = new System.Windows.Forms.HScrollBar();
            this.lblInterval = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.gbEquipment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPaperdoll)).BeginInit();
            this.gbConsumable.SuspendLayout();
            this.gbSpell.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.lstItems);
            this.groupBox1.Location = new System.Drawing.Point(12, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 431);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Items";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(6, 398);
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
            this.btnDelete.Location = new System.Drawing.Point(7, 366);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(190, 27);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 333);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lstItems
            // 
            this.lstItems.FormattingEnabled = true;
            this.lstItems.Location = new System.Drawing.Point(6, 19);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(191, 303);
            this.lstItems.TabIndex = 1;
            this.lstItems.Click += new System.EventHandler(this.lstItems_Click);
            // 
            // groupBox2
            // 
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
            this.groupBox2.Location = new System.Drawing.Point(225, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(207, 220);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // cmbPic
            // 
            this.cmbPic.FormattingEnabled = true;
            this.cmbPic.Items.AddRange(new object[] {
            "None"});
            this.cmbPic.Location = new System.Drawing.Point(60, 90);
            this.cmbPic.Name = "cmbPic";
            this.cmbPic.Size = new System.Drawing.Size(135, 21);
            this.cmbPic.TabIndex = 11;
            this.cmbPic.Text = "None";
            this.cmbPic.SelectedIndexChanged += new System.EventHandler(this.cmbPic_SelectedIndexChanged);
            // 
            // scrlAnim
            // 
            this.scrlAnim.Location = new System.Drawing.Point(19, 190);
            this.scrlAnim.Maximum = 1000;
            this.scrlAnim.Name = "scrlAnim";
            this.scrlAnim.Size = new System.Drawing.Size(176, 18);
            this.scrlAnim.TabIndex = 10;
            this.scrlAnim.ValueChanged += new System.EventHandler(this.scrlAnim_Scroll);
            // 
            // lblAnim
            // 
            this.lblAnim.AutoSize = true;
            this.lblAnim.Location = new System.Drawing.Point(16, 164);
            this.lblAnim.Name = "lblAnim";
            this.lblAnim.Size = new System.Drawing.Size(94, 13);
            this.lblAnim.TabIndex = 9;
            this.lblAnim.Text = "Animation: 0 None";
            // 
            // scrlPrice
            // 
            this.scrlPrice.Location = new System.Drawing.Point(19, 136);
            this.scrlPrice.Maximum = 1000;
            this.scrlPrice.Name = "scrlPrice";
            this.scrlPrice.Size = new System.Drawing.Size(176, 18);
            this.scrlPrice.TabIndex = 8;
            this.scrlPrice.ValueChanged += new System.EventHandler(this.scrlPrice_Scroll);
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(16, 114);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(43, 13);
            this.lblPrice.TabIndex = 7;
            this.lblPrice.Text = "Price: 0";
            // 
            // lblPic
            // 
            this.lblPic.AutoSize = true;
            this.lblPic.Location = new System.Drawing.Point(57, 74);
            this.lblPic.Name = "lblPic";
            this.lblPic.Size = new System.Drawing.Size(25, 13);
            this.lblPic.TabIndex = 6;
            this.lblPic.Text = "Pic:";
            // 
            // picItem
            // 
            this.picItem.Location = new System.Drawing.Point(15, 72);
            this.picItem.Name = "picItem";
            this.picItem.Size = new System.Drawing.Size(32, 32);
            this.picItem.TabIndex = 4;
            this.picItem.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type:";
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "None",
            "Weapon",
            "Armor",
            "Helmet",
            "Shield",
            "Consumable",
            "Key",
            "Currency",
            "Spell"});
            this.cmbType.Location = new System.Drawing.Point(60, 45);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(135, 21);
            this.cmbType.TabIndex = 2;
            this.cmbType.Text = "None";
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
            this.txtName.Location = new System.Drawing.Point(60, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(135, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.cmbClass);
            this.groupBox3.Controls.Add(this.lblLevel);
            this.groupBox3.Controls.Add(this.scrlLevel);
            this.groupBox3.Controls.Add(this.lblSpdReq);
            this.groupBox3.Controls.Add(this.lblMRReq);
            this.groupBox3.Controls.Add(this.lblDefReq);
            this.groupBox3.Controls.Add(this.lblMagReq);
            this.groupBox3.Controls.Add(this.lblStrReq);
            this.groupBox3.Controls.Add(this.scrlDefReq);
            this.groupBox3.Controls.Add(this.scrlSpdReq);
            this.groupBox3.Controls.Add(this.scrlMRReq);
            this.groupBox3.Controls.Add(this.scrlMagReq);
            this.groupBox3.Controls.Add(this.scrlStrReq);
            this.groupBox3.Location = new System.Drawing.Point(438, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(226, 220);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Requirements";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Class:";
            // 
            // cmbClass
            // 
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Items.AddRange(new object[] {
            "None"});
            this.cmbClass.Location = new System.Drawing.Point(13, 190);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(194, 21);
            this.cmbClass.TabIndex = 12;
            this.cmbClass.Text = "None";
            this.cmbClass.SelectedIndexChanged += new System.EventHandler(this.cmbClass_SelectedIndexChanged);
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(124, 122);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(45, 13);
            this.lblLevel.TabIndex = 11;
            this.lblLevel.Text = "Level: 0";
            // 
            // scrlLevel
            // 
            this.scrlLevel.Location = new System.Drawing.Point(127, 135);
            this.scrlLevel.Name = "scrlLevel";
            this.scrlLevel.Size = new System.Drawing.Size(80, 17);
            this.scrlLevel.TabIndex = 10;
            this.scrlLevel.ValueChanged += new System.EventHandler(this.scrlLevel_Scroll);
            // 
            // lblSpdReq
            // 
            this.lblSpdReq.AutoSize = true;
            this.lblSpdReq.Location = new System.Drawing.Point(10, 121);
            this.lblSpdReq.Name = "lblSpdReq";
            this.lblSpdReq.Size = new System.Drawing.Size(80, 13);
            this.lblSpdReq.TabIndex = 9;
            this.lblSpdReq.Text = "Move Speed: 0";
            // 
            // lblMRReq
            // 
            this.lblMRReq.AutoSize = true;
            this.lblMRReq.Location = new System.Drawing.Point(124, 70);
            this.lblMRReq.Name = "lblMRReq";
            this.lblMRReq.Size = new System.Drawing.Size(80, 13);
            this.lblMRReq.TabIndex = 8;
            this.lblMRReq.Text = "Magic Resist: 0";
            // 
            // lblDefReq
            // 
            this.lblDefReq.AutoSize = true;
            this.lblDefReq.Location = new System.Drawing.Point(10, 70);
            this.lblDefReq.Name = "lblDefReq";
            this.lblDefReq.Size = new System.Drawing.Size(46, 13);
            this.lblDefReq.TabIndex = 7;
            this.lblDefReq.Text = "Armor: 0";
            // 
            // lblMagReq
            // 
            this.lblMagReq.AutoSize = true;
            this.lblMagReq.Location = new System.Drawing.Point(124, 20);
            this.lblMagReq.Name = "lblMagReq";
            this.lblMagReq.Size = new System.Drawing.Size(48, 13);
            this.lblMagReq.TabIndex = 6;
            this.lblMagReq.Text = "Magic: 0";
            // 
            // lblStrReq
            // 
            this.lblStrReq.AutoSize = true;
            this.lblStrReq.Location = new System.Drawing.Point(10, 20);
            this.lblStrReq.Name = "lblStrReq";
            this.lblStrReq.Size = new System.Drawing.Size(59, 13);
            this.lblStrReq.TabIndex = 5;
            this.lblStrReq.Text = "Strength: 0";
            // 
            // scrlDefReq
            // 
            this.scrlDefReq.Location = new System.Drawing.Point(13, 85);
            this.scrlDefReq.Maximum = 255;
            this.scrlDefReq.Name = "scrlDefReq";
            this.scrlDefReq.Size = new System.Drawing.Size(80, 17);
            this.scrlDefReq.TabIndex = 4;
            this.scrlDefReq.ValueChanged += new System.EventHandler(this.scrlDefReq_Scroll);
            // 
            // scrlSpdReq
            // 
            this.scrlSpdReq.Location = new System.Drawing.Point(13, 134);
            this.scrlSpdReq.Maximum = 255;
            this.scrlSpdReq.Name = "scrlSpdReq";
            this.scrlSpdReq.Size = new System.Drawing.Size(80, 17);
            this.scrlSpdReq.TabIndex = 3;
            this.scrlSpdReq.ValueChanged += new System.EventHandler(this.scrlSpdReq_Scroll);
            // 
            // scrlMRReq
            // 
            this.scrlMRReq.Location = new System.Drawing.Point(127, 85);
            this.scrlMRReq.Maximum = 255;
            this.scrlMRReq.Name = "scrlMRReq";
            this.scrlMRReq.Size = new System.Drawing.Size(80, 17);
            this.scrlMRReq.TabIndex = 2;
            this.scrlMRReq.ValueChanged += new System.EventHandler(this.scrlMRReq_Scroll);
            // 
            // scrlMagReq
            // 
            this.scrlMagReq.Location = new System.Drawing.Point(127, 39);
            this.scrlMagReq.Maximum = 255;
            this.scrlMagReq.Name = "scrlMagReq";
            this.scrlMagReq.Size = new System.Drawing.Size(80, 17);
            this.scrlMagReq.TabIndex = 1;
            this.scrlMagReq.ValueChanged += new System.EventHandler(this.scrlMagReq_Scroll);
            // 
            // scrlStrReq
            // 
            this.scrlStrReq.Location = new System.Drawing.Point(13, 39);
            this.scrlStrReq.Maximum = 255;
            this.scrlStrReq.Name = "scrlStrReq";
            this.scrlStrReq.Size = new System.Drawing.Size(80, 17);
            this.scrlStrReq.TabIndex = 0;
            this.scrlStrReq.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlStrReq_Scroll);
            this.scrlStrReq.ValueChanged += new System.EventHandler(this.scrlStrReq_Scroll);
            // 
            // gbEquipment
            // 
            this.gbEquipment.Controls.Add(this.cmbPaperdoll);
            this.gbEquipment.Controls.Add(this.label5);
            this.gbEquipment.Controls.Add(this.lblRange);
            this.gbEquipment.Controls.Add(this.scrlRange);
            this.gbEquipment.Controls.Add(this.picPaperdoll);
            this.gbEquipment.Controls.Add(this.lblTool);
            this.gbEquipment.Controls.Add(this.scrlTool);
            this.gbEquipment.Controls.Add(this.lblAtkSpd);
            this.gbEquipment.Controls.Add(this.ScrlAtkSpd);
            this.gbEquipment.Controls.Add(this.lblDmg);
            this.gbEquipment.Controls.Add(this.scrlDmg);
            this.gbEquipment.Controls.Add(this.lblSpd);
            this.gbEquipment.Controls.Add(this.lblMR);
            this.gbEquipment.Controls.Add(this.lblDef);
            this.gbEquipment.Controls.Add(this.lblMag);
            this.gbEquipment.Controls.Add(this.lblStr);
            this.gbEquipment.Controls.Add(this.scrlDef);
            this.gbEquipment.Controls.Add(this.scrlSpd);
            this.gbEquipment.Controls.Add(this.scrlMR);
            this.gbEquipment.Controls.Add(this.scrlMag);
            this.gbEquipment.Controls.Add(this.scrlStr);
            this.gbEquipment.Location = new System.Drawing.Point(225, 233);
            this.gbEquipment.Name = "gbEquipment";
            this.gbEquipment.Size = new System.Drawing.Size(439, 205);
            this.gbEquipment.TabIndex = 12;
            this.gbEquipment.TabStop = false;
            this.gbEquipment.Text = "Equipment";
            this.gbEquipment.Visible = false;
            // 
            // cmbPaperdoll
            // 
            this.cmbPaperdoll.FormattingEnabled = true;
            this.cmbPaperdoll.Items.AddRange(new object[] {
            "None"});
            this.cmbPaperdoll.Location = new System.Drawing.Point(226, 181);
            this.cmbPaperdoll.Name = "cmbPaperdoll";
            this.cmbPaperdoll.Size = new System.Drawing.Size(194, 21);
            this.cmbPaperdoll.TabIndex = 22;
            this.cmbPaperdoll.Text = "None";
            this.cmbPaperdoll.SelectedIndexChanged += new System.EventHandler(this.cmbPaperdoll_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(223, 165);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Paperdoll:";
            // 
            // lblRange
            // 
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(220, 59);
            this.lblRange.Name = "lblRange";
            this.lblRange.Size = new System.Drawing.Size(118, 13);
            this.lblRange.TabIndex = 20;
            this.lblRange.Text = "Stat Bonus Range: +- 0";
            // 
            // scrlRange
            // 
            this.scrlRange.Location = new System.Drawing.Point(223, 78);
            this.scrlRange.Name = "scrlRange";
            this.scrlRange.Size = new System.Drawing.Size(194, 18);
            this.scrlRange.TabIndex = 19;
            this.scrlRange.ValueChanged += new System.EventHandler(this.scrlRange_Scroll);
            // 
            // picPaperdoll
            // 
            this.picPaperdoll.Location = new System.Drawing.Point(292, 103);
            this.picPaperdoll.Name = "picPaperdoll";
            this.picPaperdoll.Size = new System.Drawing.Size(128, 64);
            this.picPaperdoll.TabIndex = 16;
            this.picPaperdoll.TabStop = false;
            // 
            // lblTool
            // 
            this.lblTool.AutoSize = true;
            this.lblTool.Location = new System.Drawing.Point(223, 20);
            this.lblTool.Name = "lblTool";
            this.lblTool.Size = new System.Drawing.Size(69, 13);
            this.lblTool.TabIndex = 15;
            this.lblTool.Text = "Tool Index: 0";
            // 
            // scrlTool
            // 
            this.scrlTool.Location = new System.Drawing.Point(226, 39);
            this.scrlTool.Name = "scrlTool";
            this.scrlTool.Size = new System.Drawing.Size(194, 18);
            this.scrlTool.TabIndex = 14;
            this.scrlTool.ValueChanged += new System.EventHandler(this.scrlTool_Scroll);
            // 
            // lblAtkSpd
            // 
            this.lblAtkSpd.AutoSize = true;
            this.lblAtkSpd.Location = new System.Drawing.Point(12, 164);
            this.lblAtkSpd.Name = "lblAtkSpd";
            this.lblAtkSpd.Size = new System.Drawing.Size(112, 13);
            this.lblAtkSpd.TabIndex = 13;
            this.lblAtkSpd.Text = "AttackSpeed: 0.0 Sec";
            // 
            // ScrlAtkSpd
            // 
            this.ScrlAtkSpd.Location = new System.Drawing.Point(13, 184);
            this.ScrlAtkSpd.Maximum = 255;
            this.ScrlAtkSpd.Name = "ScrlAtkSpd";
            this.ScrlAtkSpd.Size = new System.Drawing.Size(194, 18);
            this.ScrlAtkSpd.TabIndex = 12;
            this.ScrlAtkSpd.ValueChanged += new System.EventHandler(this.ScrlAtkSpd_Scroll);
            // 
            // lblDmg
            // 
            this.lblDmg.AutoSize = true;
            this.lblDmg.Location = new System.Drawing.Point(124, 121);
            this.lblDmg.Name = "lblDmg";
            this.lblDmg.Size = new System.Drawing.Size(59, 13);
            this.lblDmg.TabIndex = 11;
            this.lblDmg.Text = "Damage: 0";
            // 
            // scrlDmg
            // 
            this.scrlDmg.Location = new System.Drawing.Point(127, 135);
            this.scrlDmg.Name = "scrlDmg";
            this.scrlDmg.Size = new System.Drawing.Size(80, 17);
            this.scrlDmg.TabIndex = 10;
            this.scrlDmg.ValueChanged += new System.EventHandler(this.scrlDmg_Scroll);
            // 
            // lblSpd
            // 
            this.lblSpd.AutoSize = true;
            this.lblSpd.Location = new System.Drawing.Point(9, 121);
            this.lblSpd.Name = "lblSpd";
            this.lblSpd.Size = new System.Drawing.Size(80, 13);
            this.lblSpd.TabIndex = 9;
            this.lblSpd.Text = "Move Speed: 0";
            // 
            // lblMR
            // 
            this.lblMR.AutoSize = true;
            this.lblMR.Location = new System.Drawing.Point(124, 70);
            this.lblMR.Name = "lblMR";
            this.lblMR.Size = new System.Drawing.Size(80, 13);
            this.lblMR.TabIndex = 8;
            this.lblMR.Text = "Magic Resist: 0";
            // 
            // lblDef
            // 
            this.lblDef.AutoSize = true;
            this.lblDef.Location = new System.Drawing.Point(10, 70);
            this.lblDef.Name = "lblDef";
            this.lblDef.Size = new System.Drawing.Size(46, 13);
            this.lblDef.TabIndex = 7;
            this.lblDef.Text = "Armor: 0";
            // 
            // lblMag
            // 
            this.lblMag.AutoSize = true;
            this.lblMag.Location = new System.Drawing.Point(124, 20);
            this.lblMag.Name = "lblMag";
            this.lblMag.Size = new System.Drawing.Size(48, 13);
            this.lblMag.TabIndex = 6;
            this.lblMag.Text = "Magic: 0";
            // 
            // lblStr
            // 
            this.lblStr.AutoSize = true;
            this.lblStr.Location = new System.Drawing.Point(10, 20);
            this.lblStr.Name = "lblStr";
            this.lblStr.Size = new System.Drawing.Size(59, 13);
            this.lblStr.TabIndex = 5;
            this.lblStr.Text = "Strength: 0";
            // 
            // scrlDef
            // 
            this.scrlDef.Location = new System.Drawing.Point(13, 85);
            this.scrlDef.Maximum = 255;
            this.scrlDef.Name = "scrlDef";
            this.scrlDef.Size = new System.Drawing.Size(80, 17);
            this.scrlDef.TabIndex = 4;
            this.scrlDef.ValueChanged += new System.EventHandler(this.scrlDef_Scroll);
            // 
            // scrlSpd
            // 
            this.scrlSpd.Location = new System.Drawing.Point(13, 134);
            this.scrlSpd.Maximum = 255;
            this.scrlSpd.Name = "scrlSpd";
            this.scrlSpd.Size = new System.Drawing.Size(80, 17);
            this.scrlSpd.TabIndex = 3;
            this.scrlSpd.ValueChanged += new System.EventHandler(this.scrlSpd_Scroll);
            // 
            // scrlMR
            // 
            this.scrlMR.Location = new System.Drawing.Point(127, 85);
            this.scrlMR.Maximum = 255;
            this.scrlMR.Name = "scrlMR";
            this.scrlMR.Size = new System.Drawing.Size(80, 17);
            this.scrlMR.TabIndex = 2;
            this.scrlMR.ValueChanged += new System.EventHandler(this.scrlMR_Scroll);
            // 
            // scrlMag
            // 
            this.scrlMag.Location = new System.Drawing.Point(127, 39);
            this.scrlMag.Maximum = 255;
            this.scrlMag.Name = "scrlMag";
            this.scrlMag.Size = new System.Drawing.Size(80, 17);
            this.scrlMag.TabIndex = 1;
            this.scrlMag.ValueChanged += new System.EventHandler(this.scrlMag_Scroll);
            // 
            // scrlStr
            // 
            this.scrlStr.Location = new System.Drawing.Point(13, 39);
            this.scrlStr.Maximum = 255;
            this.scrlStr.Name = "scrlStr";
            this.scrlStr.Size = new System.Drawing.Size(80, 17);
            this.scrlStr.TabIndex = 0;
            this.scrlStr.ValueChanged += new System.EventHandler(this.scrlStr_Scroll);
            // 
            // gbConsumable
            // 
            this.gbConsumable.Controls.Add(this.gbSpell);
            this.gbConsumable.Controls.Add(this.label4);
            this.gbConsumable.Controls.Add(this.cmbConsume);
            this.gbConsumable.Controls.Add(this.scrlInterval);
            this.gbConsumable.Controls.Add(this.lblInterval);
            this.gbConsumable.Location = new System.Drawing.Point(225, 233);
            this.gbConsumable.Name = "gbConsumable";
            this.gbConsumable.Size = new System.Drawing.Size(217, 122);
            this.gbConsumable.TabIndex = 12;
            this.gbConsumable.TabStop = false;
            this.gbConsumable.Text = "Consumable";
            this.gbConsumable.Visible = false;
            // 
            // gbSpell
            // 
            this.gbSpell.Controls.Add(this.scrlSpell);
            this.gbSpell.Controls.Add(this.lblSpell);
            this.gbSpell.Location = new System.Drawing.Point(0, 0);
            this.gbSpell.Name = "gbSpell";
            this.gbSpell.Size = new System.Drawing.Size(217, 67);
            this.gbSpell.TabIndex = 13;
            this.gbSpell.TabStop = false;
            this.gbSpell.Text = "Spell";
            this.gbSpell.Visible = false;
            // 
            // scrlSpell
            // 
            this.scrlSpell.Location = new System.Drawing.Point(12, 39);
            this.scrlSpell.Maximum = 1000;
            this.scrlSpell.Name = "scrlSpell";
            this.scrlSpell.Size = new System.Drawing.Size(176, 18);
            this.scrlSpell.TabIndex = 12;
            this.scrlSpell.ValueChanged += new System.EventHandler(this.scrlSpell_Scroll);
            // 
            // lblSpell
            // 
            this.lblSpell.AutoSize = true;
            this.lblSpell.Location = new System.Drawing.Point(12, 20);
            this.lblSpell.Name = "lblSpell";
            this.lblSpell.Size = new System.Drawing.Size(71, 13);
            this.lblSpell.TabIndex = 11;
            this.lblSpell.Text = "Spell: 0 None";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Vital:";
            // 
            // cmbConsume
            // 
            this.cmbConsume.FormattingEnabled = true;
            this.cmbConsume.Items.AddRange(new object[] {
            "Health",
            "Mana",
            "Experience"});
            this.cmbConsume.Location = new System.Drawing.Point(19, 36);
            this.cmbConsume.Name = "cmbConsume";
            this.cmbConsume.Size = new System.Drawing.Size(176, 21);
            this.cmbConsume.TabIndex = 11;
            this.cmbConsume.Text = "Health";
            this.cmbConsume.SelectedIndexChanged += new System.EventHandler(this.cmbConsume_SelectedIndexChanged);
            // 
            // scrlInterval
            // 
            this.scrlInterval.Location = new System.Drawing.Point(19, 88);
            this.scrlInterval.Maximum = 1000;
            this.scrlInterval.Name = "scrlInterval";
            this.scrlInterval.Size = new System.Drawing.Size(176, 18);
            this.scrlInterval.TabIndex = 10;
            this.scrlInterval.ValueChanged += new System.EventHandler(this.scrlInterval_Scroll);
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(19, 69);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(54, 13);
            this.lblInterval.TabIndex = 9;
            this.lblInterval.Text = "Interval: 0";
            // 
            // FrmItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 450);
            this.ControlBox = false;
            this.Controls.Add(this.gbConsumable);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbEquipment);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmItem";
            this.Text = "Item Editor";
            this.Load += new System.EventHandler(this.frmItem_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbEquipment.ResumeLayout(false);
            this.gbEquipment.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPaperdoll)).EndInit();
            this.gbConsumable.ResumeLayout(false);
            this.gbConsumable.PerformLayout();
            this.gbSpell.ResumeLayout(false);
            this.gbSpell.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private ListBox lstItems;
        private Button btnDelete;
        private Button btnSave;
        private GroupBox groupBox2;
        private Label label1;
        private TextBox txtName;
        private Label label2;
        private ComboBox cmbType;
        private HScrollBar scrlAnim;
        private Label lblAnim;
        private HScrollBar scrlPrice;
        private Label lblPrice;
        private PictureBox picItem;
        private GroupBox groupBox3;
        private Label label3;
        private ComboBox cmbClass;
        private Label lblLevel;
        private HScrollBar scrlLevel;
        private Label lblSpdReq;
        private Label lblMRReq;
        private Label lblDefReq;
        private Label lblMagReq;
        private Label lblStrReq;
        private HScrollBar scrlDefReq;
        private HScrollBar scrlSpdReq;
        private HScrollBar scrlMRReq;
        private HScrollBar scrlMagReq;
        private HScrollBar scrlStrReq;
        private GroupBox gbEquipment;
        private Label lblTool;
        private HScrollBar scrlTool;
        private Label lblAtkSpd;
        private HScrollBar ScrlAtkSpd;
        private Label lblDmg;
        private HScrollBar scrlDmg;
        private Label lblSpd;
        private Label lblMR;
        private Label lblDef;
        private Label lblMag;
        private Label lblStr;
        private HScrollBar scrlDef;
        private HScrollBar scrlSpd;
        private HScrollBar scrlMR;
        private HScrollBar scrlMag;
        private HScrollBar scrlStr;
        private PictureBox picPaperdoll;
        private GroupBox gbConsumable;
        private ComboBox cmbConsume;
        private HScrollBar scrlInterval;
        private Label lblInterval;
        private Label label4;
        private GroupBox gbSpell;
        private HScrollBar scrlSpell;
        private Label lblSpell;
        private Button btnCancel;
        private Label lblRange;
        private HScrollBar scrlRange;
        private ComboBox cmbPic;
        private Label lblPic;
        private ComboBox cmbPaperdoll;
        private Label label5;

    }
}