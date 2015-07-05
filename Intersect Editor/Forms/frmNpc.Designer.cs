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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lstNpcs = new System.Windows.Forms.ListBox();
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
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNpc)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSpd
            // 
            this.lblSpd.AutoSize = true;
            this.lblSpd.Location = new System.Drawing.Point(10, 173);
            this.lblSpd.Name = "lblSpd";
            this.lblSpd.Size = new System.Drawing.Size(80, 13);
            this.lblSpd.TabIndex = 9;
            this.lblSpd.Text = "Move Speed: 0";
            // 
            // lblMR
            // 
            this.lblMR.AutoSize = true;
            this.lblMR.Location = new System.Drawing.Point(124, 122);
            this.lblMR.Name = "lblMR";
            this.lblMR.Size = new System.Drawing.Size(80, 13);
            this.lblMR.TabIndex = 8;
            this.lblMR.Text = "Magic Resist: 0";
            // 
            // lblDef
            // 
            this.lblDef.AutoSize = true;
            this.lblDef.Location = new System.Drawing.Point(10, 122);
            this.lblDef.Name = "lblDef";
            this.lblDef.Size = new System.Drawing.Size(46, 13);
            this.lblDef.TabIndex = 7;
            this.lblDef.Text = "Armor: 0";
            // 
            // lblMag
            // 
            this.lblMag.AutoSize = true;
            this.lblMag.Location = new System.Drawing.Point(124, 72);
            this.lblMag.Name = "lblMag";
            this.lblMag.Size = new System.Drawing.Size(48, 13);
            this.lblMag.TabIndex = 6;
            this.lblMag.Text = "Magic: 0";
            // 
            // lblStr
            // 
            this.lblStr.AutoSize = true;
            this.lblStr.Location = new System.Drawing.Point(10, 72);
            this.lblStr.Name = "lblStr";
            this.lblStr.Size = new System.Drawing.Size(59, 13);
            this.lblStr.TabIndex = 5;
            this.lblStr.Text = "Strength: 0";
            // 
            // scrlSpd
            // 
            this.scrlSpd.LargeChange = 1;
            this.scrlSpd.Location = new System.Drawing.Point(13, 186);
            this.scrlSpd.Maximum = 255;
            this.scrlSpd.Name = "scrlSpd";
            this.scrlSpd.Size = new System.Drawing.Size(80, 17);
            this.scrlSpd.TabIndex = 3;
            this.scrlSpd.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpd_Scroll);
            // 
            // scrlMR
            // 
            this.scrlMR.LargeChange = 1;
            this.scrlMR.Location = new System.Drawing.Point(127, 137);
            this.scrlMR.Maximum = 255;
            this.scrlMR.Name = "scrlMR";
            this.scrlMR.Size = new System.Drawing.Size(80, 17);
            this.scrlMR.TabIndex = 2;
            this.scrlMR.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMR_Scroll);
            // 
            // scrlDef
            // 
            this.scrlDef.LargeChange = 1;
            this.scrlDef.Location = new System.Drawing.Point(13, 137);
            this.scrlDef.Maximum = 255;
            this.scrlDef.Name = "scrlDef";
            this.scrlDef.Size = new System.Drawing.Size(80, 17);
            this.scrlDef.TabIndex = 4;
            this.scrlDef.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDef_Scroll);
            // 
            // scrlMag
            // 
            this.scrlMag.LargeChange = 1;
            this.scrlMag.Location = new System.Drawing.Point(127, 91);
            this.scrlMag.Maximum = 255;
            this.scrlMag.Name = "scrlMag";
            this.scrlMag.Size = new System.Drawing.Size(80, 17);
            this.scrlMag.TabIndex = 1;
            this.scrlMag.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMag_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.lstNpcs);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 431);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "NPCs";
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
            // lstNpcs
            // 
            this.lstNpcs.FormattingEnabled = true;
            this.lstNpcs.Location = new System.Drawing.Point(6, 19);
            this.lstNpcs.Name = "lstNpcs";
            this.lstNpcs.Size = new System.Drawing.Size(191, 303);
            this.lstNpcs.TabIndex = 1;
            this.lstNpcs.Click += new System.EventHandler(this.lstNpcs_Click);
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
            this.groupBox2.Location = new System.Drawing.Point(225, 12);
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
            "Friendly"});
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
            this.groupBox3.Location = new System.Drawing.Point(438, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(226, 220);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Stats:";
            // 
            // txtExp
            // 
            this.txtExp.Location = new System.Drawing.Point(127, 186);
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
            this.lblExp.Location = new System.Drawing.Point(124, 173);
            this.lblExp.Name = "lblExp";
            this.lblExp.Size = new System.Drawing.Size(28, 13);
            this.lblExp.TabIndex = 11;
            this.lblExp.Text = "Exp:";
            // 
            // scrlStr
            // 
            this.scrlStr.LargeChange = 1;
            this.scrlStr.Location = new System.Drawing.Point(13, 91);
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
            this.groupBox4.Location = new System.Drawing.Point(225, 239);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(207, 166);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Drops";
            // 
            // txtDropAmount
            // 
            this.txtDropAmount.Location = new System.Drawing.Point(9, 99);
            this.txtDropAmount.Name = "txtDropAmount";
            this.txtDropAmount.Size = new System.Drawing.Size(183, 20);
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
            this.scrlDropChance.Size = new System.Drawing.Size(186, 18);
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
            this.scrlDropItem.Name = "scrlDropItem";
            this.scrlDropItem.Size = new System.Drawing.Size(186, 18);
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
            this.scrlDropIndex.Size = new System.Drawing.Size(186, 18);
            this.scrlDropIndex.TabIndex = 10;
            this.scrlDropIndex.Value = 1;
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
            // frmNpc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 452);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmNpc";
            this.Text = "NPC Editor";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmNpc_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picNpc)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
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
    }
}