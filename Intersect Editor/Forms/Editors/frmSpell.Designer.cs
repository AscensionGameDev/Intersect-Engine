namespace Intersect_Editor.Forms
{
    partial class frmSpell
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
            this.lstSpells = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.scrlHitAnimation = new System.Windows.Forms.HScrollBar();
            this.lblHitAnimation = new System.Windows.Forms.Label();
            this.scrlCastAnimation = new System.Windows.Forms.HScrollBar();
            this.lblCastAnimation = new System.Windows.Forms.Label();
            this.cmbSprite = new System.Windows.Forms.ComboBox();
            this.lblPic = new System.Windows.Forms.Label();
            this.picSpell = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.scrlCooldownDuration = new System.Windows.Forms.HScrollBar();
            this.lblCooldownDuration = new System.Windows.Forms.Label();
            this.scrlCastDuration = new System.Windows.Forms.HScrollBar();
            this.lblCastDuration = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblLevelReq = new System.Windows.Forms.Label();
            this.scrlLevelReq = new System.Windows.Forms.HScrollBar();
            this.lblSpeedReq = new System.Windows.Forms.Label();
            this.lblMagicResistReq = new System.Windows.Forms.Label();
            this.lblDefenseReq = new System.Windows.Forms.Label();
            this.lblAbilityPwrReq = new System.Windows.Forms.Label();
            this.lblAttackReq = new System.Windows.Forms.Label();
            this.scrlDefenseReq = new System.Windows.Forms.HScrollBar();
            this.scrlSpeedReq = new System.Windows.Forms.HScrollBar();
            this.scrlMagicResistReq = new System.Windows.Forms.HScrollBar();
            this.scrlAbilityPwrReq = new System.Windows.Forms.HScrollBar();
            this.scrlAttackReq = new System.Windows.Forms.HScrollBar();
            this.grpTargetInfo = new System.Windows.Forms.GroupBox();
            this.scrlHitRadius = new System.Windows.Forms.HScrollBar();
            this.lblHitRadius = new System.Windows.Forms.Label();
            this.cmbTargetType = new System.Windows.Forms.ComboBox();
            this.scrlCastRange = new System.Windows.Forms.HScrollBar();
            this.lblCastRange = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.grpBuffDebuff = new System.Windows.Forms.GroupBox();
            this.cmbExtraEffect = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.scrlBuffDuration = new System.Windows.Forms.HScrollBar();
            this.lblBuffDuration = new System.Windows.Forms.Label();
            this.txtMagicResistBuff = new System.Windows.Forms.TextBox();
            this.txtAbilityPwrBuff = new System.Windows.Forms.TextBox();
            this.txtSpeedBuff = new System.Windows.Forms.TextBox();
            this.txtDefenseBuff = new System.Windows.Forms.TextBox();
            this.txtAttackBuff = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.chkHOTDOT = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtManaDiff = new System.Windows.Forms.TextBox();
            this.txtHPDiff = new System.Windows.Forms.TextBox();
            this.lblMana = new System.Windows.Forms.Label();
            this.lblHP = new System.Windows.Forms.Label();
            this.grpWarp = new System.Windows.Forms.GroupBox();
            this.lblWarpDir = new System.Windows.Forms.Label();
            this.lblWarpX = new System.Windows.Forms.Label();
            this.lblWarpY = new System.Windows.Forms.Label();
            this.scrlWarpX = new System.Windows.Forms.HScrollBar();
            this.scrlWarpDir = new System.Windows.Forms.HScrollBar();
            this.scrlWarpY = new System.Windows.Forms.HScrollBar();
            this.txtWarpChunk = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtManaCost = new System.Windows.Forms.TextBox();
            this.txtHPCost = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSpell)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.grpTargetInfo.SuspendLayout();
            this.grpBuffDebuff.SuspendLayout();
            this.grpWarp.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.lstSpells);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 431);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Spells";
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
            // lstSpells
            // 
            this.lstSpells.FormattingEnabled = true;
            this.lstSpells.Location = new System.Drawing.Point(6, 19);
            this.lstSpells.Name = "lstSpells";
            this.lstSpells.Size = new System.Drawing.Size(191, 303);
            this.lstSpells.TabIndex = 1;
            this.lstSpells.Click += new System.EventHandler(this.lstSpells_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtDesc);
            this.groupBox2.Controls.Add(this.scrlHitAnimation);
            this.groupBox2.Controls.Add(this.lblHitAnimation);
            this.groupBox2.Controls.Add(this.scrlCastAnimation);
            this.groupBox2.Controls.Add(this.lblCastAnimation);
            this.groupBox2.Controls.Add(this.cmbSprite);
            this.groupBox2.Controls.Add(this.lblPic);
            this.groupBox2.Controls.Add(this.picSpell);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmbType);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Location = new System.Drawing.Point(225, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(207, 254);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Desc:";
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(60, 117);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(135, 64);
            this.txtDesc.TabIndex = 18;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // scrlHitAnimation
            // 
            this.scrlHitAnimation.LargeChange = 1;
            this.scrlHitAnimation.Location = new System.Drawing.Point(6, 230);
            this.scrlHitAnimation.Maximum = 6000;
            this.scrlHitAnimation.Minimum = -1;
            this.scrlHitAnimation.Name = "scrlHitAnimation";
            this.scrlHitAnimation.Size = new System.Drawing.Size(186, 18);
            this.scrlHitAnimation.TabIndex = 17;
            this.scrlHitAnimation.Value = -1;
            this.scrlHitAnimation.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlHitAnimation_Scroll);
            // 
            // lblHitAnimation
            // 
            this.lblHitAnimation.AutoSize = true;
            this.lblHitAnimation.Location = new System.Drawing.Point(6, 215);
            this.lblHitAnimation.Name = "lblHitAnimation";
            this.lblHitAnimation.Size = new System.Drawing.Size(81, 13);
            this.lblHitAnimation.TabIndex = 16;
            this.lblHitAnimation.Text = "Hit Animation: 0";
            // 
            // scrlCastAnimation
            // 
            this.scrlCastAnimation.LargeChange = 1;
            this.scrlCastAnimation.Location = new System.Drawing.Point(6, 197);
            this.scrlCastAnimation.Maximum = 600;
            this.scrlCastAnimation.Minimum = -1;
            this.scrlCastAnimation.Name = "scrlCastAnimation";
            this.scrlCastAnimation.Size = new System.Drawing.Size(186, 18);
            this.scrlCastAnimation.TabIndex = 15;
            this.scrlCastAnimation.Value = -1;
            this.scrlCastAnimation.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlCastAnimation_Scroll);
            // 
            // lblCastAnimation
            // 
            this.lblCastAnimation.AutoSize = true;
            this.lblCastAnimation.Location = new System.Drawing.Point(6, 184);
            this.lblCastAnimation.Name = "lblCastAnimation";
            this.lblCastAnimation.Size = new System.Drawing.Size(89, 13);
            this.lblCastAnimation.TabIndex = 14;
            this.lblCastAnimation.Text = "Cast Animation: 0";
            // 
            // cmbSprite
            // 
            this.cmbSprite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSprite.FormattingEnabled = true;
            this.cmbSprite.Items.AddRange(new object[] {
            "None"});
            this.cmbSprite.Location = new System.Drawing.Point(60, 90);
            this.cmbSprite.Name = "cmbSprite";
            this.cmbSprite.Size = new System.Drawing.Size(135, 21);
            this.cmbSprite.TabIndex = 11;
            this.cmbSprite.SelectedIndexChanged += new System.EventHandler(this.cmbSprite_SelectedIndexChanged);
            // 
            // lblPic
            // 
            this.lblPic.AutoSize = true;
            this.lblPic.Location = new System.Drawing.Point(57, 74);
            this.lblPic.Name = "lblPic";
            this.lblPic.Size = new System.Drawing.Size(31, 13);
            this.lblPic.TabIndex = 6;
            this.lblPic.Text = "Icon:";
            // 
            // picSpell
            // 
            this.picSpell.BackColor = System.Drawing.Color.Black;
            this.picSpell.Location = new System.Drawing.Point(12, 79);
            this.picSpell.Name = "picSpell";
            this.picSpell.Size = new System.Drawing.Size(32, 32);
            this.picSpell.TabIndex = 4;
            this.picSpell.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type:";
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Combat Spell",
            "Warp"});
            this.cmbType.Location = new System.Drawing.Point(60, 45);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(135, 21);
            this.cmbType.TabIndex = 2;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
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
            // scrlCooldownDuration
            // 
            this.scrlCooldownDuration.LargeChange = 1;
            this.scrlCooldownDuration.Location = new System.Drawing.Point(7, 140);
            this.scrlCooldownDuration.Maximum = 6000;
            this.scrlCooldownDuration.Name = "scrlCooldownDuration";
            this.scrlCooldownDuration.Size = new System.Drawing.Size(141, 18);
            this.scrlCooldownDuration.TabIndex = 13;
            this.scrlCooldownDuration.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlCooldownDuration_Scroll);
            // 
            // lblCooldownDuration
            // 
            this.lblCooldownDuration.AutoSize = true;
            this.lblCooldownDuration.Location = new System.Drawing.Point(7, 125);
            this.lblCooldownDuration.Name = "lblCooldownDuration";
            this.lblCooldownDuration.Size = new System.Drawing.Size(109, 13);
            this.lblCooldownDuration.TabIndex = 12;
            this.lblCooldownDuration.Text = "Cooldown (secs): 0.0 ";
            // 
            // scrlCastDuration
            // 
            this.scrlCastDuration.LargeChange = 1;
            this.scrlCastDuration.Location = new System.Drawing.Point(7, 107);
            this.scrlCastDuration.Maximum = 600;
            this.scrlCastDuration.Name = "scrlCastDuration";
            this.scrlCastDuration.Size = new System.Drawing.Size(141, 18);
            this.scrlCastDuration.TabIndex = 8;
            this.scrlCastDuration.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlCastDuration_Scroll);
            // 
            // lblCastDuration
            // 
            this.lblCastDuration.AutoSize = true;
            this.lblCastDuration.Location = new System.Drawing.Point(7, 94);
            this.lblCastDuration.Name = "lblCastDuration";
            this.lblCastDuration.Size = new System.Drawing.Size(106, 13);
            this.lblCastDuration.TabIndex = 7;
            this.lblCastDuration.Text = "Cast Time (secs): 0.0";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblLevelReq);
            this.groupBox3.Controls.Add(this.scrlLevelReq);
            this.groupBox3.Controls.Add(this.lblSpeedReq);
            this.groupBox3.Controls.Add(this.lblMagicResistReq);
            this.groupBox3.Controls.Add(this.lblDefenseReq);
            this.groupBox3.Controls.Add(this.lblAbilityPwrReq);
            this.groupBox3.Controls.Add(this.lblAttackReq);
            this.groupBox3.Controls.Add(this.scrlDefenseReq);
            this.groupBox3.Controls.Add(this.scrlSpeedReq);
            this.groupBox3.Controls.Add(this.scrlMagicResistReq);
            this.groupBox3.Controls.Add(this.scrlAbilityPwrReq);
            this.groupBox3.Controls.Add(this.scrlAttackReq);
            this.groupBox3.Location = new System.Drawing.Point(438, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(226, 157);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Casting Requirements";
            // 
            // lblLevelReq
            // 
            this.lblLevelReq.AutoSize = true;
            this.lblLevelReq.Location = new System.Drawing.Point(120, 117);
            this.lblLevelReq.Name = "lblLevelReq";
            this.lblLevelReq.Size = new System.Drawing.Size(45, 13);
            this.lblLevelReq.TabIndex = 13;
            this.lblLevelReq.Text = "Level: 0";
            // 
            // scrlLevelReq
            // 
            this.scrlLevelReq.LargeChange = 1;
            this.scrlLevelReq.Location = new System.Drawing.Point(123, 130);
            this.scrlLevelReq.Maximum = 255;
            this.scrlLevelReq.Name = "scrlLevelReq";
            this.scrlLevelReq.Size = new System.Drawing.Size(80, 17);
            this.scrlLevelReq.TabIndex = 12;
            this.scrlLevelReq.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlLevelReq_Scroll);
            // 
            // lblSpeedReq
            // 
            this.lblSpeedReq.AutoSize = true;
            this.lblSpeedReq.Location = new System.Drawing.Point(6, 117);
            this.lblSpeedReq.Name = "lblSpeedReq";
            this.lblSpeedReq.Size = new System.Drawing.Size(50, 13);
            this.lblSpeedReq.TabIndex = 9;
            this.lblSpeedReq.Text = "Speed: 0";
            // 
            // lblMagicResistReq
            // 
            this.lblMagicResistReq.AutoSize = true;
            this.lblMagicResistReq.Location = new System.Drawing.Point(120, 66);
            this.lblMagicResistReq.Name = "lblMagicResistReq";
            this.lblMagicResistReq.Size = new System.Drawing.Size(80, 13);
            this.lblMagicResistReq.TabIndex = 8;
            this.lblMagicResistReq.Text = "Magic Resist: 0";
            // 
            // lblDefenseReq
            // 
            this.lblDefenseReq.AutoSize = true;
            this.lblDefenseReq.Location = new System.Drawing.Point(6, 66);
            this.lblDefenseReq.Name = "lblDefenseReq";
            this.lblDefenseReq.Size = new System.Drawing.Size(59, 13);
            this.lblDefenseReq.TabIndex = 7;
            this.lblDefenseReq.Text = "Defense: 0";
            // 
            // lblAbilityPwrReq
            // 
            this.lblAbilityPwrReq.AutoSize = true;
            this.lblAbilityPwrReq.Location = new System.Drawing.Point(120, 16);
            this.lblAbilityPwrReq.Name = "lblAbilityPwrReq";
            this.lblAbilityPwrReq.Size = new System.Drawing.Size(67, 13);
            this.lblAbilityPwrReq.TabIndex = 6;
            this.lblAbilityPwrReq.Text = "Ability Pwr: 0";
            // 
            // lblAttackReq
            // 
            this.lblAttackReq.AutoSize = true;
            this.lblAttackReq.Location = new System.Drawing.Point(6, 16);
            this.lblAttackReq.Name = "lblAttackReq";
            this.lblAttackReq.Size = new System.Drawing.Size(50, 13);
            this.lblAttackReq.TabIndex = 5;
            this.lblAttackReq.Text = "Attack: 0";
            // 
            // scrlDefenseReq
            // 
            this.scrlDefenseReq.LargeChange = 1;
            this.scrlDefenseReq.Location = new System.Drawing.Point(9, 81);
            this.scrlDefenseReq.Maximum = 255;
            this.scrlDefenseReq.Name = "scrlDefenseReq";
            this.scrlDefenseReq.Size = new System.Drawing.Size(80, 17);
            this.scrlDefenseReq.TabIndex = 4;
            this.scrlDefenseReq.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlDefenseReq_Scroll);
            // 
            // scrlSpeedReq
            // 
            this.scrlSpeedReq.LargeChange = 1;
            this.scrlSpeedReq.Location = new System.Drawing.Point(9, 130);
            this.scrlSpeedReq.Maximum = 255;
            this.scrlSpeedReq.Name = "scrlSpeedReq";
            this.scrlSpeedReq.Size = new System.Drawing.Size(80, 17);
            this.scrlSpeedReq.TabIndex = 3;
            this.scrlSpeedReq.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpeedReq_Scroll);
            // 
            // scrlMagicResistReq
            // 
            this.scrlMagicResistReq.LargeChange = 1;
            this.scrlMagicResistReq.Location = new System.Drawing.Point(123, 81);
            this.scrlMagicResistReq.Maximum = 255;
            this.scrlMagicResistReq.Name = "scrlMagicResistReq";
            this.scrlMagicResistReq.Size = new System.Drawing.Size(80, 17);
            this.scrlMagicResistReq.TabIndex = 2;
            this.scrlMagicResistReq.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlMagicResistReq_Scroll);
            // 
            // scrlAbilityPwrReq
            // 
            this.scrlAbilityPwrReq.LargeChange = 1;
            this.scrlAbilityPwrReq.Location = new System.Drawing.Point(123, 35);
            this.scrlAbilityPwrReq.Maximum = 255;
            this.scrlAbilityPwrReq.Name = "scrlAbilityPwrReq";
            this.scrlAbilityPwrReq.Size = new System.Drawing.Size(80, 17);
            this.scrlAbilityPwrReq.TabIndex = 1;
            this.scrlAbilityPwrReq.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlAbilityPwrReq_Scroll);
            // 
            // scrlAttackReq
            // 
            this.scrlAttackReq.LargeChange = 1;
            this.scrlAttackReq.Location = new System.Drawing.Point(9, 35);
            this.scrlAttackReq.Maximum = 255;
            this.scrlAttackReq.Name = "scrlAttackReq";
            this.scrlAttackReq.Size = new System.Drawing.Size(80, 17);
            this.scrlAttackReq.TabIndex = 0;
            this.scrlAttackReq.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlAttackReq_Scroll);
            // 
            // grpTargetInfo
            // 
            this.grpTargetInfo.Controls.Add(this.scrlHitRadius);
            this.grpTargetInfo.Controls.Add(this.lblHitRadius);
            this.grpTargetInfo.Controls.Add(this.cmbTargetType);
            this.grpTargetInfo.Controls.Add(this.scrlCastRange);
            this.grpTargetInfo.Controls.Add(this.lblCastRange);
            this.grpTargetInfo.Controls.Add(this.label4);
            this.grpTargetInfo.Location = new System.Drawing.Point(438, 172);
            this.grpTargetInfo.Name = "grpTargetInfo";
            this.grpTargetInfo.Size = new System.Drawing.Size(225, 94);
            this.grpTargetInfo.TabIndex = 19;
            this.grpTargetInfo.TabStop = false;
            this.grpTargetInfo.Text = "Targetting Info";
            this.grpTargetInfo.Visible = false;
            // 
            // scrlHitRadius
            // 
            this.scrlHitRadius.LargeChange = 1;
            this.scrlHitRadius.Location = new System.Drawing.Point(112, 69);
            this.scrlHitRadius.Maximum = 20;
            this.scrlHitRadius.Name = "scrlHitRadius";
            this.scrlHitRadius.Size = new System.Drawing.Size(103, 18);
            this.scrlHitRadius.TabIndex = 17;
            this.scrlHitRadius.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlHitRadius_Scroll);
            // 
            // lblHitRadius
            // 
            this.lblHitRadius.AutoSize = true;
            this.lblHitRadius.Location = new System.Drawing.Point(112, 56);
            this.lblHitRadius.Name = "lblHitRadius";
            this.lblHitRadius.Size = new System.Drawing.Size(68, 13);
            this.lblHitRadius.TabIndex = 16;
            this.lblHitRadius.Text = "Hit Radius: 0";
            // 
            // cmbTargetType
            // 
            this.cmbTargetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetType.FormattingEnabled = true;
            this.cmbTargetType.Items.AddRange(new object[] {
            "Self",
            "Single Target (includes self)",
            "AOE",
            "Linear (projectile)"});
            this.cmbTargetType.Location = new System.Drawing.Point(9, 32);
            this.cmbTargetType.Name = "cmbTargetType";
            this.cmbTargetType.Size = new System.Drawing.Size(135, 21);
            this.cmbTargetType.TabIndex = 15;
            this.cmbTargetType.SelectedIndexChanged += new System.EventHandler(this.cmbTargetType_SelectedIndexChanged);
            // 
            // scrlCastRange
            // 
            this.scrlCastRange.LargeChange = 1;
            this.scrlCastRange.Location = new System.Drawing.Point(6, 68);
            this.scrlCastRange.Maximum = 20;
            this.scrlCastRange.Name = "scrlCastRange";
            this.scrlCastRange.Size = new System.Drawing.Size(103, 18);
            this.scrlCastRange.TabIndex = 14;
            this.scrlCastRange.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlCastRange_Scroll);
            // 
            // lblCastRange
            // 
            this.lblCastRange.AutoSize = true;
            this.lblCastRange.Location = new System.Drawing.Point(6, 55);
            this.lblCastRange.Name = "lblCastRange";
            this.lblCastRange.Size = new System.Drawing.Size(75, 13);
            this.lblCastRange.TabIndex = 13;
            this.lblCastRange.Text = "Cast Range: 0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Target Type:";
            // 
            // grpBuffDebuff
            // 
            this.grpBuffDebuff.Controls.Add(this.cmbExtraEffect);
            this.grpBuffDebuff.Controls.Add(this.label7);
            this.grpBuffDebuff.Controls.Add(this.scrlBuffDuration);
            this.grpBuffDebuff.Controls.Add(this.lblBuffDuration);
            this.grpBuffDebuff.Controls.Add(this.txtMagicResistBuff);
            this.grpBuffDebuff.Controls.Add(this.txtAbilityPwrBuff);
            this.grpBuffDebuff.Controls.Add(this.txtSpeedBuff);
            this.grpBuffDebuff.Controls.Add(this.txtDefenseBuff);
            this.grpBuffDebuff.Controls.Add(this.txtAttackBuff);
            this.grpBuffDebuff.Controls.Add(this.label10);
            this.grpBuffDebuff.Controls.Add(this.label11);
            this.grpBuffDebuff.Controls.Add(this.label12);
            this.grpBuffDebuff.Controls.Add(this.label13);
            this.grpBuffDebuff.Controls.Add(this.label14);
            this.grpBuffDebuff.Controls.Add(this.chkHOTDOT);
            this.grpBuffDebuff.Controls.Add(this.label9);
            this.grpBuffDebuff.Controls.Add(this.txtManaDiff);
            this.grpBuffDebuff.Controls.Add(this.txtHPDiff);
            this.grpBuffDebuff.Controls.Add(this.lblMana);
            this.grpBuffDebuff.Controls.Add(this.lblHP);
            this.grpBuffDebuff.Location = new System.Drawing.Point(225, 273);
            this.grpBuffDebuff.Name = "grpBuffDebuff";
            this.grpBuffDebuff.Size = new System.Drawing.Size(278, 170);
            this.grpBuffDebuff.TabIndex = 20;
            this.grpBuffDebuff.TabStop = false;
            this.grpBuffDebuff.Text = "Combat Spell";
            this.grpBuffDebuff.Visible = false;
            // 
            // cmbExtraEffect
            // 
            this.cmbExtraEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExtraEffect.FormattingEnabled = true;
            this.cmbExtraEffect.Items.AddRange(new object[] {
            "None",
            "Silence",
            "Stun"});
            this.cmbExtraEffect.Location = new System.Drawing.Point(12, 132);
            this.cmbExtraEffect.Name = "cmbExtraEffect";
            this.cmbExtraEffect.Size = new System.Drawing.Size(86, 21);
            this.cmbExtraEffect.TabIndex = 36;
            this.cmbExtraEffect.SelectedIndexChanged += new System.EventHandler(this.cmbExtraEffect_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 116);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Extra Effect:";
            // 
            // scrlBuffDuration
            // 
            this.scrlBuffDuration.LargeChange = 1;
            this.scrlBuffDuration.Location = new System.Drawing.Point(196, 129);
            this.scrlBuffDuration.Maximum = 600;
            this.scrlBuffDuration.Name = "scrlBuffDuration";
            this.scrlBuffDuration.Size = new System.Drawing.Size(73, 18);
            this.scrlBuffDuration.TabIndex = 34;
            this.scrlBuffDuration.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlBuffDuration_Scroll);
            // 
            // lblBuffDuration
            // 
            this.lblBuffDuration.Location = new System.Drawing.Point(193, 94);
            this.lblBuffDuration.Name = "lblBuffDuration";
            this.lblBuffDuration.Size = new System.Drawing.Size(79, 35);
            this.lblBuffDuration.TabIndex = 33;
            this.lblBuffDuration.Text = "Duration (seconds): 0.0";
            // 
            // txtMagicResistBuff
            // 
            this.txtMagicResistBuff.Location = new System.Drawing.Point(196, 67);
            this.txtMagicResistBuff.Name = "txtMagicResistBuff";
            this.txtMagicResistBuff.Size = new System.Drawing.Size(55, 20);
            this.txtMagicResistBuff.TabIndex = 32;
            this.txtMagicResistBuff.TextChanged += new System.EventHandler(this.txtMagicResistBuff_TextChanged);
            // 
            // txtAbilityPwrBuff
            // 
            this.txtAbilityPwrBuff.Location = new System.Drawing.Point(196, 32);
            this.txtAbilityPwrBuff.Name = "txtAbilityPwrBuff";
            this.txtAbilityPwrBuff.Size = new System.Drawing.Size(55, 20);
            this.txtAbilityPwrBuff.TabIndex = 31;
            this.txtAbilityPwrBuff.TextChanged += new System.EventHandler(this.txtAbilityPwrBuff_TextChanged);
            // 
            // txtSpeedBuff
            // 
            this.txtSpeedBuff.Location = new System.Drawing.Point(120, 103);
            this.txtSpeedBuff.Name = "txtSpeedBuff";
            this.txtSpeedBuff.Size = new System.Drawing.Size(56, 20);
            this.txtSpeedBuff.TabIndex = 30;
            this.txtSpeedBuff.TextChanged += new System.EventHandler(this.txtSpeedBuff_TextChanged);
            // 
            // txtDefenseBuff
            // 
            this.txtDefenseBuff.Location = new System.Drawing.Point(120, 67);
            this.txtDefenseBuff.Name = "txtDefenseBuff";
            this.txtDefenseBuff.Size = new System.Drawing.Size(56, 20);
            this.txtDefenseBuff.TabIndex = 29;
            this.txtDefenseBuff.TextChanged += new System.EventHandler(this.txtDefenseBuff_TextChanged);
            // 
            // txtAttackBuff
            // 
            this.txtAttackBuff.Location = new System.Drawing.Point(120, 32);
            this.txtAttackBuff.Name = "txtAttackBuff";
            this.txtAttackBuff.Size = new System.Drawing.Size(56, 20);
            this.txtAttackBuff.TabIndex = 28;
            this.txtAttackBuff.TextChanged += new System.EventHandler(this.txtAttackBuff_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(117, 88);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 13);
            this.label10.TabIndex = 27;
            this.label10.Text = "Speed:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(193, 51);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Magic Resist:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(117, 51);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(50, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Defense:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(193, 16);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(58, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "Ability Pwr:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(117, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 13);
            this.label14.TabIndex = 23;
            this.label14.Text = "Attack:";
            // 
            // chkHOTDOT
            // 
            this.chkHOTDOT.Location = new System.Drawing.Point(12, 92);
            this.chkHOTDOT.Name = "chkHOTDOT";
            this.chkHOTDOT.Size = new System.Drawing.Size(86, 24);
            this.chkHOTDOT.TabIndex = 22;
            this.chkHOTDOT.Text = "HOT/DOT?";
            this.chkHOTDOT.UseVisualStyleBackColor = true;
            this.chkHOTDOT.CheckedChanged += new System.EventHandler(this.chkHOTDOT_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(6, 156);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(229, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Hint: Negative values will reduce stats or vitals.";
            // 
            // txtManaDiff
            // 
            this.txtManaDiff.Location = new System.Drawing.Point(12, 71);
            this.txtManaDiff.Name = "txtManaDiff";
            this.txtManaDiff.Size = new System.Drawing.Size(86, 20);
            this.txtManaDiff.TabIndex = 20;
            this.txtManaDiff.TextChanged += new System.EventHandler(this.txtManaDiff_TextChanged);
            // 
            // txtHPDiff
            // 
            this.txtHPDiff.Location = new System.Drawing.Point(12, 32);
            this.txtHPDiff.Name = "txtHPDiff";
            this.txtHPDiff.Size = new System.Drawing.Size(86, 20);
            this.txtHPDiff.TabIndex = 17;
            this.txtHPDiff.TextChanged += new System.EventHandler(this.txtHPDiff_TextChanged);
            // 
            // lblMana
            // 
            this.lblMana.AutoSize = true;
            this.lblMana.Location = new System.Drawing.Point(9, 55);
            this.lblMana.Name = "lblMana";
            this.lblMana.Size = new System.Drawing.Size(89, 13);
            this.lblMana.TabIndex = 19;
            this.lblMana.Text = "Mana Difference:";
            // 
            // lblHP
            // 
            this.lblHP.AutoSize = true;
            this.lblHP.Location = new System.Drawing.Point(9, 16);
            this.lblHP.Name = "lblHP";
            this.lblHP.Size = new System.Drawing.Size(77, 13);
            this.lblHP.TabIndex = 18;
            this.lblHP.Text = "HP Difference:";
            // 
            // grpWarp
            // 
            this.grpWarp.Controls.Add(this.lblWarpDir);
            this.grpWarp.Controls.Add(this.lblWarpX);
            this.grpWarp.Controls.Add(this.lblWarpY);
            this.grpWarp.Controls.Add(this.scrlWarpX);
            this.grpWarp.Controls.Add(this.scrlWarpDir);
            this.grpWarp.Controls.Add(this.scrlWarpY);
            this.grpWarp.Controls.Add(this.txtWarpChunk);
            this.grpWarp.Controls.Add(this.label16);
            this.grpWarp.Location = new System.Drawing.Point(225, 273);
            this.grpWarp.Name = "grpWarp";
            this.grpWarp.Size = new System.Drawing.Size(200, 170);
            this.grpWarp.TabIndex = 35;
            this.grpWarp.TabStop = false;
            this.grpWarp.Text = "Warp Caster:";
            this.grpWarp.Visible = false;
            // 
            // lblWarpDir
            // 
            this.lblWarpDir.AutoSize = true;
            this.lblWarpDir.Location = new System.Drawing.Point(9, 106);
            this.lblWarpDir.Name = "lblWarpDir";
            this.lblWarpDir.Size = new System.Drawing.Size(40, 13);
            this.lblWarpDir.TabIndex = 39;
            this.lblWarpDir.Text = "Dir: Up";
            // 
            // lblWarpX
            // 
            this.lblWarpX.AutoSize = true;
            this.lblWarpX.Location = new System.Drawing.Point(9, 55);
            this.lblWarpX.Name = "lblWarpX";
            this.lblWarpX.Size = new System.Drawing.Size(26, 13);
            this.lblWarpX.TabIndex = 38;
            this.lblWarpX.Text = "X: 0";
            // 
            // lblWarpY
            // 
            this.lblWarpY.AutoSize = true;
            this.lblWarpY.Location = new System.Drawing.Point(104, 55);
            this.lblWarpY.Name = "lblWarpY";
            this.lblWarpY.Size = new System.Drawing.Size(26, 13);
            this.lblWarpY.TabIndex = 37;
            this.lblWarpY.Text = "Y: 0";
            // 
            // scrlWarpX
            // 
            this.scrlWarpX.LargeChange = 1;
            this.scrlWarpX.Location = new System.Drawing.Point(12, 70);
            this.scrlWarpX.Maximum = 29;
            this.scrlWarpX.Name = "scrlWarpX";
            this.scrlWarpX.Size = new System.Drawing.Size(80, 17);
            this.scrlWarpX.TabIndex = 36;
            this.scrlWarpX.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlWarpX_Scroll);
            // 
            // scrlWarpDir
            // 
            this.scrlWarpDir.LargeChange = 1;
            this.scrlWarpDir.Location = new System.Drawing.Point(12, 119);
            this.scrlWarpDir.Maximum = 3;
            this.scrlWarpDir.Name = "scrlWarpDir";
            this.scrlWarpDir.Size = new System.Drawing.Size(80, 17);
            this.scrlWarpDir.TabIndex = 35;
            this.scrlWarpDir.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlWarpDir_Scroll);
            // 
            // scrlWarpY
            // 
            this.scrlWarpY.LargeChange = 1;
            this.scrlWarpY.Location = new System.Drawing.Point(107, 70);
            this.scrlWarpY.Maximum = 29;
            this.scrlWarpY.Name = "scrlWarpY";
            this.scrlWarpY.Size = new System.Drawing.Size(80, 17);
            this.scrlWarpY.TabIndex = 34;
            this.scrlWarpY.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlWarpY_Scroll);
            // 
            // txtWarpChunk
            // 
            this.txtWarpChunk.Location = new System.Drawing.Point(12, 32);
            this.txtWarpChunk.Name = "txtWarpChunk";
            this.txtWarpChunk.Size = new System.Drawing.Size(86, 20);
            this.txtWarpChunk.TabIndex = 33;
            this.txtWarpChunk.TextChanged += new System.EventHandler(this.txtWarpChunk_TextChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 16);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(41, 13);
            this.label16.TabIndex = 32;
            this.label16.Text = "Chunk:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtManaCost);
            this.groupBox4.Controls.Add(this.txtHPCost);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.scrlCooldownDuration);
            this.groupBox4.Controls.Add(this.lblCastDuration);
            this.groupBox4.Controls.Add(this.lblCooldownDuration);
            this.groupBox4.Controls.Add(this.scrlCastDuration);
            this.groupBox4.Location = new System.Drawing.Point(509, 273);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(154, 170);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Spell Cost:";
            // 
            // txtManaCost
            // 
            this.txtManaCost.Location = new System.Drawing.Point(9, 71);
            this.txtManaCost.Name = "txtManaCost";
            this.txtManaCost.Size = new System.Drawing.Size(86, 20);
            this.txtManaCost.TabIndex = 24;
            this.txtManaCost.TextChanged += new System.EventHandler(this.txtManaCost_TextChanged);
            // 
            // txtHPCost
            // 
            this.txtHPCost.Location = new System.Drawing.Point(9, 32);
            this.txtHPCost.Name = "txtHPCost";
            this.txtHPCost.Size = new System.Drawing.Size(86, 20);
            this.txtHPCost.TabIndex = 21;
            this.txtHPCost.TextChanged += new System.EventHandler(this.txtHPCost_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Mana Cost:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "HP Cost:";
            // 
            // frmSpell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 461);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.grpBuffDebuff);
            this.Controls.Add(this.grpTargetInfo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.grpWarp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmSpell";
            this.Text = "Spell Editor                       ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSpell_FormClosed);
            this.Load += new System.EventHandler(this.frmSpell_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSpell)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grpTargetInfo.ResumeLayout(false);
            this.grpTargetInfo.PerformLayout();
            this.grpBuffDebuff.ResumeLayout(false);
            this.grpBuffDebuff.PerformLayout();
            this.grpWarp.ResumeLayout(false);
            this.grpWarp.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lstSpells;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.HScrollBar scrlCooldownDuration;
        private System.Windows.Forms.Label lblCooldownDuration;
        private System.Windows.Forms.ComboBox cmbSprite;
        private System.Windows.Forms.HScrollBar scrlCastDuration;
        private System.Windows.Forms.Label lblCastDuration;
        private System.Windows.Forms.Label lblPic;
        private System.Windows.Forms.PictureBox picSpell;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblSpeedReq;
        private System.Windows.Forms.Label lblMagicResistReq;
        private System.Windows.Forms.Label lblDefenseReq;
        private System.Windows.Forms.Label lblAbilityPwrReq;
        private System.Windows.Forms.Label lblAttackReq;
        private System.Windows.Forms.HScrollBar scrlDefenseReq;
        private System.Windows.Forms.HScrollBar scrlSpeedReq;
        private System.Windows.Forms.HScrollBar scrlMagicResistReq;
        private System.Windows.Forms.HScrollBar scrlAbilityPwrReq;
        private System.Windows.Forms.HScrollBar scrlAttackReq;
        private System.Windows.Forms.HScrollBar scrlHitAnimation;
        private System.Windows.Forms.Label lblHitAnimation;
        private System.Windows.Forms.HScrollBar scrlCastAnimation;
        private System.Windows.Forms.Label lblCastAnimation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label lblLevelReq;
        private System.Windows.Forms.HScrollBar scrlLevelReq;
        private System.Windows.Forms.GroupBox grpTargetInfo;
        private System.Windows.Forms.HScrollBar scrlHitRadius;
        private System.Windows.Forms.Label lblHitRadius;
        private System.Windows.Forms.ComboBox cmbTargetType;
        private System.Windows.Forms.HScrollBar scrlCastRange;
        private System.Windows.Forms.Label lblCastRange;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grpBuffDebuff;
        private System.Windows.Forms.HScrollBar scrlBuffDuration;
        private System.Windows.Forms.Label lblBuffDuration;
        private System.Windows.Forms.TextBox txtMagicResistBuff;
        private System.Windows.Forms.TextBox txtAbilityPwrBuff;
        private System.Windows.Forms.TextBox txtSpeedBuff;
        private System.Windows.Forms.TextBox txtDefenseBuff;
        private System.Windows.Forms.TextBox txtAttackBuff;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox chkHOTDOT;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtManaDiff;
        private System.Windows.Forms.TextBox txtHPDiff;
        private System.Windows.Forms.Label lblMana;
        private System.Windows.Forms.Label lblHP;
        private System.Windows.Forms.GroupBox grpWarp;
        private System.Windows.Forms.Label lblWarpDir;
        private System.Windows.Forms.Label lblWarpX;
        private System.Windows.Forms.Label lblWarpY;
        private System.Windows.Forms.HScrollBar scrlWarpX;
        private System.Windows.Forms.HScrollBar scrlWarpDir;
        private System.Windows.Forms.HScrollBar scrlWarpY;
        private System.Windows.Forms.TextBox txtWarpChunk;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtManaCost;
        private System.Windows.Forms.TextBox txtHPCost;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.ComboBox cmbExtraEffect;
        private System.Windows.Forms.Label label7;
    }
}