using DarkUI.Controls;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSpell));
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
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.groupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDesc = new DarkUI.Controls.DarkTextBox();
            this.scrlHitAnimation = new DarkUI.Controls.DarkScrollBar();
            this.lblHitAnimation = new System.Windows.Forms.Label();
            this.scrlCastAnimation = new DarkUI.Controls.DarkScrollBar();
            this.lblCastAnimation = new System.Windows.Forms.Label();
            this.cmbSprite = new DarkUI.Controls.DarkComboBox();
            this.lblPic = new System.Windows.Forms.Label();
            this.picSpell = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new DarkUI.Controls.DarkComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new DarkUI.Controls.DarkTextBox();
            this.groupBox4 = new DarkUI.Controls.DarkGroupBox();
            this.scrlManaCost = new DarkUI.Controls.DarkScrollBar();
            this.scrlHPCost = new DarkUI.Controls.DarkScrollBar();
            this.lblMPCost = new System.Windows.Forms.Label();
            this.lblHPCost = new System.Windows.Forms.Label();
            this.scrlCooldownDuration = new DarkUI.Controls.DarkScrollBar();
            this.lblCastDuration = new System.Windows.Forms.Label();
            this.lblCooldownDuration = new System.Windows.Forms.Label();
            this.scrlCastDuration = new DarkUI.Controls.DarkScrollBar();
            this.grpTargetInfo = new DarkUI.Controls.DarkGroupBox();
            this.lblProjectile = new System.Windows.Forms.Label();
            this.scrlHitRadius = new DarkUI.Controls.DarkScrollBar();
            this.lblHitRadius = new System.Windows.Forms.Label();
            this.cmbTargetType = new DarkUI.Controls.DarkComboBox();
            this.lblCastRange = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.scrlProjectile = new DarkUI.Controls.DarkScrollBar();
            this.scrlCastRange = new DarkUI.Controls.DarkScrollBar();
            this.groupBox3 = new DarkUI.Controls.DarkGroupBox();
            this.lblLevelReq = new System.Windows.Forms.Label();
            this.scrlLevelReq = new DarkUI.Controls.DarkScrollBar();
            this.lblSpeedReq = new System.Windows.Forms.Label();
            this.lblMagicResistReq = new System.Windows.Forms.Label();
            this.lblDefenseReq = new System.Windows.Forms.Label();
            this.lblAbilityPwrReq = new System.Windows.Forms.Label();
            this.lblAttackReq = new System.Windows.Forms.Label();
            this.scrlDefenseReq = new DarkUI.Controls.DarkScrollBar();
            this.scrlSpeedReq = new DarkUI.Controls.DarkScrollBar();
            this.scrlMagicResistReq = new DarkUI.Controls.DarkScrollBar();
            this.scrlAbilityPwrReq = new DarkUI.Controls.DarkScrollBar();
            this.scrlAttackReq = new DarkUI.Controls.DarkScrollBar();
            this.grpBuffDebuff = new DarkUI.Controls.DarkGroupBox();
            this.darkGroupBox5 = new DarkUI.Controls.DarkGroupBox();
            this.chkHOTDOT = new DarkUI.Controls.DarkCheckBox();
            this.lblTick = new System.Windows.Forms.Label();
            this.scrlTick = new DarkUI.Controls.DarkScrollBar();
            this.darkGroupBox4 = new DarkUI.Controls.DarkGroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbExtraEffect = new DarkUI.Controls.DarkComboBox();
            this.picSprite = new System.Windows.Forms.PictureBox();
            this.cmbTransform = new DarkUI.Controls.DarkComboBox();
            this.lblSprite = new System.Windows.Forms.Label();
            this.darkGroupBox3 = new DarkUI.Controls.DarkGroupBox();
            this.lblBuffDuration = new System.Windows.Forms.Label();
            this.scrlBuffDuration = new DarkUI.Controls.DarkScrollBar();
            this.darkGroupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblMagicResist = new System.Windows.Forms.Label();
            this.lblDefense = new System.Windows.Forms.Label();
            this.lblAbilityPwr = new System.Windows.Forms.Label();
            this.lblAttack = new System.Windows.Forms.Label();
            this.scrlDefense = new DarkUI.Controls.DarkScrollBar();
            this.scrlSpeed = new DarkUI.Controls.DarkScrollBar();
            this.scrlMagicResist = new DarkUI.Controls.DarkScrollBar();
            this.scrlAbilityPwr = new DarkUI.Controls.DarkScrollBar();
            this.scrlAttack = new DarkUI.Controls.DarkScrollBar();
            this.darkGroupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.cmbScalingStat = new DarkUI.Controls.DarkComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkFriendly = new DarkUI.Controls.DarkCheckBox();
            this.lblCritChance = new System.Windows.Forms.Label();
            this.scrlCritChance = new DarkUI.Controls.DarkScrollBar();
            this.lblScaling = new System.Windows.Forms.Label();
            this.scrlScaling = new DarkUI.Controls.DarkScrollBar();
            this.cmbDamageType = new DarkUI.Controls.DarkComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.lblHPDamage = new System.Windows.Forms.Label();
            this.scrlManaDamage = new DarkUI.Controls.DarkScrollBar();
            this.lblManaDamage = new System.Windows.Forms.Label();
            this.scrlHPDamage = new DarkUI.Controls.DarkScrollBar();
            this.grpDash = new DarkUI.Controls.DarkGroupBox();
            this.groupBox5 = new DarkUI.Controls.DarkGroupBox();
            this.chkIgnoreInactiveResources = new DarkUI.Controls.DarkCheckBox();
            this.chkIgnoreZDimensionBlocks = new DarkUI.Controls.DarkCheckBox();
            this.chkIgnoreMapBlocks = new DarkUI.Controls.DarkCheckBox();
            this.chkIgnoreActiveResources = new DarkUI.Controls.DarkCheckBox();
            this.lblRange = new System.Windows.Forms.Label();
            this.scrlRange = new DarkUI.Controls.DarkScrollBar();
            this.grpEvent = new DarkUI.Controls.DarkGroupBox();
            this.scrlEvent = new DarkUI.Controls.DarkScrollBar();
            this.lblEvent = new System.Windows.Forms.Label();
            this.grpWarp = new DarkUI.Controls.DarkGroupBox();
            this.lblWarpDir = new System.Windows.Forms.Label();
            this.lblWarpX = new System.Windows.Forms.Label();
            this.lblWarpY = new System.Windows.Forms.Label();
            this.scrlWarpX = new DarkUI.Controls.DarkScrollBar();
            this.scrlWarpDir = new DarkUI.Controls.DarkScrollBar();
            this.scrlWarpY = new DarkUI.Controls.DarkScrollBar();
            this.txtWarpChunk = new DarkUI.Controls.DarkTextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox1 = new DarkUI.Controls.DarkGroupBox();
            this.lstSpells = new System.Windows.Forms.ListBox();
            this.pnlContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSpell)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.grpTargetInfo.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.grpBuffDebuff.SuspendLayout();
            this.darkGroupBox5.SuspendLayout();
            this.darkGroupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSprite)).BeginInit();
            this.darkGroupBox3.SuspendLayout();
            this.darkGroupBox2.SuspendLayout();
            this.darkGroupBox1.SuspendLayout();
            this.grpDash.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.grpEvent.SuspendLayout();
            this.grpWarp.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContainer
            // 
            this.pnlContainer.AutoScroll = true;
            this.pnlContainer.Controls.Add(this.groupBox2);
            this.pnlContainer.Controls.Add(this.groupBox4);
            this.pnlContainer.Controls.Add(this.grpTargetInfo);
            this.pnlContainer.Controls.Add(this.groupBox3);
            this.pnlContainer.Controls.Add(this.grpBuffDebuff);
            this.pnlContainer.Controls.Add(this.grpDash);
            this.pnlContainer.Controls.Add(this.grpEvent);
            this.pnlContainer.Controls.Add(this.grpWarp);
            this.pnlContainer.Location = new System.Drawing.Point(221, 40);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(460, 473);
            this.pnlContainer.TabIndex = 41;
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
            this.toolStrip.Size = new System.Drawing.Size(681, 25);
            this.toolStrip.TabIndex = 51;
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
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(476, 519);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 49;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(280, 519);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 46;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
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
            this.groupBox2.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox2.Location = new System.Drawing.Point(2, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(207, 294);
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
            this.txtDesc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDesc.Location = new System.Drawing.Point(60, 117);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(135, 80);
            this.txtDesc.TabIndex = 18;
            this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
            // 
            // scrlHitAnimation
            // 
            this.scrlHitAnimation.Location = new System.Drawing.Point(6, 265);
            this.scrlHitAnimation.Maximum = 6000;
            this.scrlHitAnimation.Minimum = -1;
            this.scrlHitAnimation.Name = "scrlHitAnimation";
            this.scrlHitAnimation.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlHitAnimation.Size = new System.Drawing.Size(186, 18);
            this.scrlHitAnimation.TabIndex = 17;
            this.scrlHitAnimation.Value = -1;
            this.scrlHitAnimation.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlHitAnimation_Scroll);
            // 
            // lblHitAnimation
            // 
            this.lblHitAnimation.AutoSize = true;
            this.lblHitAnimation.Location = new System.Drawing.Point(6, 250);
            this.lblHitAnimation.Name = "lblHitAnimation";
            this.lblHitAnimation.Size = new System.Drawing.Size(101, 13);
            this.lblHitAnimation.TabIndex = 16;
            this.lblHitAnimation.Text = "Hit Animation: None";
            // 
            // scrlCastAnimation
            // 
            this.scrlCastAnimation.Location = new System.Drawing.Point(6, 219);
            this.scrlCastAnimation.Maximum = 600;
            this.scrlCastAnimation.Minimum = -1;
            this.scrlCastAnimation.Name = "scrlCastAnimation";
            this.scrlCastAnimation.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlCastAnimation.Size = new System.Drawing.Size(186, 18);
            this.scrlCastAnimation.TabIndex = 15;
            this.scrlCastAnimation.Value = -1;
            this.scrlCastAnimation.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlCastAnimation_Scroll);
            // 
            // lblCastAnimation
            // 
            this.lblCastAnimation.AutoSize = true;
            this.lblCastAnimation.Location = new System.Drawing.Point(6, 206);
            this.lblCastAnimation.Name = "lblCastAnimation";
            this.lblCastAnimation.Size = new System.Drawing.Size(109, 13);
            this.lblCastAnimation.TabIndex = 14;
            this.lblCastAnimation.Text = "Cast Animation: None";
            // 
            // cmbSprite
            // 
            this.cmbSprite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSprite.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSprite.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSprite.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSprite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSprite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSprite.ForeColor = System.Drawing.Color.Gainsboro;
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
            this.picSpell.Location = new System.Drawing.Point(9, 79);
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
            this.cmbType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Combat Spell",
            "Warp to Map",
            "Warp to Target",
            "Dash",
            "Event"});
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
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtName.Location = new System.Drawing.Point(60, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(135, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox4.Controls.Add(this.scrlManaCost);
            this.groupBox4.Controls.Add(this.scrlHPCost);
            this.groupBox4.Controls.Add(this.lblMPCost);
            this.groupBox4.Controls.Add(this.lblHPCost);
            this.groupBox4.Controls.Add(this.scrlCooldownDuration);
            this.groupBox4.Controls.Add(this.lblCastDuration);
            this.groupBox4.Controls.Add(this.lblCooldownDuration);
            this.groupBox4.Controls.Add(this.scrlCastDuration);
            this.groupBox4.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox4.Location = new System.Drawing.Point(2, 295);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(438, 104);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Spell Cost:";
            // 
            // scrlManaCost
            // 
            this.scrlManaCost.Location = new System.Drawing.Point(9, 70);
            this.scrlManaCost.Maximum = 10000;
            this.scrlManaCost.Minimum = -10000;
            this.scrlManaCost.Name = "scrlManaCost";
            this.scrlManaCost.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlManaCost.Size = new System.Drawing.Size(186, 18);
            this.scrlManaCost.TabIndex = 24;
            this.scrlManaCost.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlManaCost_Scroll);
            // 
            // scrlHPCost
            // 
            this.scrlHPCost.Location = new System.Drawing.Point(9, 29);
            this.scrlHPCost.Maximum = 10000;
            this.scrlHPCost.Minimum = -10000;
            this.scrlHPCost.Name = "scrlHPCost";
            this.scrlHPCost.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlHPCost.Size = new System.Drawing.Size(186, 18);
            this.scrlHPCost.TabIndex = 20;
            this.scrlHPCost.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlHPCost_Scroll);
            // 
            // lblMPCost
            // 
            this.lblMPCost.AutoSize = true;
            this.lblMPCost.Location = new System.Drawing.Point(6, 55);
            this.lblMPCost.Name = "lblMPCost";
            this.lblMPCost.Size = new System.Drawing.Size(70, 13);
            this.lblMPCost.TabIndex = 23;
            this.lblMPCost.Text = "Mana Cost: 0";
            // 
            // lblHPCost
            // 
            this.lblHPCost.AutoSize = true;
            this.lblHPCost.Location = new System.Drawing.Point(6, 16);
            this.lblHPCost.Name = "lblHPCost";
            this.lblHPCost.Size = new System.Drawing.Size(58, 13);
            this.lblHPCost.TabIndex = 22;
            this.lblHPCost.Text = "HP Cost: 0";
            // 
            // scrlCooldownDuration
            // 
            this.scrlCooldownDuration.Location = new System.Drawing.Point(219, 70);
            this.scrlCooldownDuration.Maximum = 6000;
            this.scrlCooldownDuration.Name = "scrlCooldownDuration";
            this.scrlCooldownDuration.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlCooldownDuration.Size = new System.Drawing.Size(209, 18);
            this.scrlCooldownDuration.TabIndex = 13;
            this.scrlCooldownDuration.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlCooldownDuration_Scroll);
            // 
            // lblCastDuration
            // 
            this.lblCastDuration.AutoSize = true;
            this.lblCastDuration.Location = new System.Drawing.Point(219, 16);
            this.lblCastDuration.Name = "lblCastDuration";
            this.lblCastDuration.Size = new System.Drawing.Size(106, 13);
            this.lblCastDuration.TabIndex = 7;
            this.lblCastDuration.Text = "Cast Time (secs): 0.0";
            // 
            // lblCooldownDuration
            // 
            this.lblCooldownDuration.AutoSize = true;
            this.lblCooldownDuration.Location = new System.Drawing.Point(219, 55);
            this.lblCooldownDuration.Name = "lblCooldownDuration";
            this.lblCooldownDuration.Size = new System.Drawing.Size(109, 13);
            this.lblCooldownDuration.TabIndex = 12;
            this.lblCooldownDuration.Text = "Cooldown (secs): 0.0 ";
            // 
            // scrlCastDuration
            // 
            this.scrlCastDuration.Location = new System.Drawing.Point(219, 29);
            this.scrlCastDuration.Maximum = 600;
            this.scrlCastDuration.Name = "scrlCastDuration";
            this.scrlCastDuration.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlCastDuration.Size = new System.Drawing.Size(209, 18);
            this.scrlCastDuration.TabIndex = 8;
            this.scrlCastDuration.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlCastDuration_Scroll);
            // 
            // grpTargetInfo
            // 
            this.grpTargetInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpTargetInfo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpTargetInfo.Controls.Add(this.lblProjectile);
            this.grpTargetInfo.Controls.Add(this.scrlHitRadius);
            this.grpTargetInfo.Controls.Add(this.lblHitRadius);
            this.grpTargetInfo.Controls.Add(this.cmbTargetType);
            this.grpTargetInfo.Controls.Add(this.lblCastRange);
            this.grpTargetInfo.Controls.Add(this.label4);
            this.grpTargetInfo.Controls.Add(this.scrlProjectile);
            this.grpTargetInfo.Controls.Add(this.scrlCastRange);
            this.grpTargetInfo.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpTargetInfo.Location = new System.Drawing.Point(215, 149);
            this.grpTargetInfo.Name = "grpTargetInfo";
            this.grpTargetInfo.Size = new System.Drawing.Size(225, 145);
            this.grpTargetInfo.TabIndex = 19;
            this.grpTargetInfo.TabStop = false;
            this.grpTargetInfo.Text = "Targetting Info";
            this.grpTargetInfo.Visible = false;
            // 
            // lblProjectile
            // 
            this.lblProjectile.AutoSize = true;
            this.lblProjectile.Location = new System.Drawing.Point(6, 59);
            this.lblProjectile.Name = "lblProjectile";
            this.lblProjectile.Size = new System.Drawing.Size(91, 13);
            this.lblProjectile.TabIndex = 18;
            this.lblProjectile.Text = "Projectile: 0 None";
            // 
            // scrlHitRadius
            // 
            this.scrlHitRadius.Location = new System.Drawing.Point(6, 115);
            this.scrlHitRadius.Maximum = 20;
            this.scrlHitRadius.Name = "scrlHitRadius";
            this.scrlHitRadius.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlHitRadius.Size = new System.Drawing.Size(209, 18);
            this.scrlHitRadius.TabIndex = 17;
            this.scrlHitRadius.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlHitRadius_Scroll);
            // 
            // lblHitRadius
            // 
            this.lblHitRadius.AutoSize = true;
            this.lblHitRadius.Location = new System.Drawing.Point(6, 102);
            this.lblHitRadius.Name = "lblHitRadius";
            this.lblHitRadius.Size = new System.Drawing.Size(68, 13);
            this.lblHitRadius.TabIndex = 16;
            this.lblHitRadius.Text = "Hit Radius: 0";
            // 
            // cmbTargetType
            // 
            this.cmbTargetType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTargetType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTargetType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTargetType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTargetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTargetType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTargetType.FormattingEnabled = true;
            this.cmbTargetType.Items.AddRange(new object[] {
            "Self",
            "Single Target (includes self)",
            "AOE",
            "Linear (projectile)"});
            this.cmbTargetType.Location = new System.Drawing.Point(9, 32);
            this.cmbTargetType.Name = "cmbTargetType";
            this.cmbTargetType.Size = new System.Drawing.Size(206, 21);
            this.cmbTargetType.TabIndex = 15;
            this.cmbTargetType.SelectedIndexChanged += new System.EventHandler(this.cmbTargetType_SelectedIndexChanged);
            // 
            // lblCastRange
            // 
            this.lblCastRange.AutoSize = true;
            this.lblCastRange.Location = new System.Drawing.Point(6, 60);
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
            // scrlProjectile
            // 
            this.scrlProjectile.Location = new System.Drawing.Point(6, 72);
            this.scrlProjectile.Maximum = 20;
            this.scrlProjectile.Minimum = -1;
            this.scrlProjectile.Name = "scrlProjectile";
            this.scrlProjectile.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlProjectile.Size = new System.Drawing.Size(209, 18);
            this.scrlProjectile.TabIndex = 19;
            this.scrlProjectile.Value = -1;
            this.scrlProjectile.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlProjectile_Scroll);
            // 
            // scrlCastRange
            // 
            this.scrlCastRange.Location = new System.Drawing.Point(6, 73);
            this.scrlCastRange.Maximum = 20;
            this.scrlCastRange.Name = "scrlCastRange";
            this.scrlCastRange.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlCastRange.Size = new System.Drawing.Size(209, 18);
            this.scrlCastRange.TabIndex = 14;
            this.scrlCastRange.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlCastRange_Scroll);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
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
            this.groupBox3.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox3.Location = new System.Drawing.Point(215, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(226, 143);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Casting Requirements";
            // 
            // lblLevelReq
            // 
            this.lblLevelReq.AutoSize = true;
            this.lblLevelReq.Location = new System.Drawing.Point(120, 103);
            this.lblLevelReq.Name = "lblLevelReq";
            this.lblLevelReq.Size = new System.Drawing.Size(45, 13);
            this.lblLevelReq.TabIndex = 13;
            this.lblLevelReq.Text = "Level: 0";
            // 
            // scrlLevelReq
            // 
            this.scrlLevelReq.Location = new System.Drawing.Point(123, 116);
            this.scrlLevelReq.Maximum = 255;
            this.scrlLevelReq.Name = "scrlLevelReq";
            this.scrlLevelReq.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlLevelReq.Size = new System.Drawing.Size(80, 17);
            this.scrlLevelReq.TabIndex = 12;
            this.scrlLevelReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlLevelReq_Scroll);
            // 
            // lblSpeedReq
            // 
            this.lblSpeedReq.AutoSize = true;
            this.lblSpeedReq.Location = new System.Drawing.Point(6, 103);
            this.lblSpeedReq.Name = "lblSpeedReq";
            this.lblSpeedReq.Size = new System.Drawing.Size(50, 13);
            this.lblSpeedReq.TabIndex = 9;
            this.lblSpeedReq.Text = "Speed: 0";
            // 
            // lblMagicResistReq
            // 
            this.lblMagicResistReq.AutoSize = true;
            this.lblMagicResistReq.Location = new System.Drawing.Point(120, 62);
            this.lblMagicResistReq.Name = "lblMagicResistReq";
            this.lblMagicResistReq.Size = new System.Drawing.Size(80, 13);
            this.lblMagicResistReq.TabIndex = 8;
            this.lblMagicResistReq.Text = "Magic Resist: 0";
            // 
            // lblDefenseReq
            // 
            this.lblDefenseReq.AutoSize = true;
            this.lblDefenseReq.Location = new System.Drawing.Point(6, 62);
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
            this.scrlDefenseReq.Location = new System.Drawing.Point(9, 77);
            this.scrlDefenseReq.Maximum = 255;
            this.scrlDefenseReq.Name = "scrlDefenseReq";
            this.scrlDefenseReq.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlDefenseReq.Size = new System.Drawing.Size(80, 17);
            this.scrlDefenseReq.TabIndex = 4;
            this.scrlDefenseReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlDefenseReq_Scroll);
            // 
            // scrlSpeedReq
            // 
            this.scrlSpeedReq.Location = new System.Drawing.Point(9, 116);
            this.scrlSpeedReq.Maximum = 255;
            this.scrlSpeedReq.Name = "scrlSpeedReq";
            this.scrlSpeedReq.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlSpeedReq.Size = new System.Drawing.Size(80, 17);
            this.scrlSpeedReq.TabIndex = 3;
            this.scrlSpeedReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlSpeedReq_Scroll);
            // 
            // scrlMagicResistReq
            // 
            this.scrlMagicResistReq.Location = new System.Drawing.Point(123, 77);
            this.scrlMagicResistReq.Maximum = 255;
            this.scrlMagicResistReq.Name = "scrlMagicResistReq";
            this.scrlMagicResistReq.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlMagicResistReq.Size = new System.Drawing.Size(80, 17);
            this.scrlMagicResistReq.TabIndex = 2;
            this.scrlMagicResistReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlMagicResistReq_Scroll);
            // 
            // scrlAbilityPwrReq
            // 
            this.scrlAbilityPwrReq.Location = new System.Drawing.Point(123, 35);
            this.scrlAbilityPwrReq.Maximum = 255;
            this.scrlAbilityPwrReq.Name = "scrlAbilityPwrReq";
            this.scrlAbilityPwrReq.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlAbilityPwrReq.Size = new System.Drawing.Size(80, 17);
            this.scrlAbilityPwrReq.TabIndex = 1;
            this.scrlAbilityPwrReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAbilityPwrReq_Scroll);
            // 
            // scrlAttackReq
            // 
            this.scrlAttackReq.Location = new System.Drawing.Point(9, 35);
            this.scrlAttackReq.Maximum = 255;
            this.scrlAttackReq.Name = "scrlAttackReq";
            this.scrlAttackReq.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlAttackReq.Size = new System.Drawing.Size(80, 17);
            this.scrlAttackReq.TabIndex = 0;
            this.scrlAttackReq.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAttackReq_Scroll);
            // 
            // grpBuffDebuff
            // 
            this.grpBuffDebuff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpBuffDebuff.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpBuffDebuff.Controls.Add(this.darkGroupBox5);
            this.grpBuffDebuff.Controls.Add(this.darkGroupBox4);
            this.grpBuffDebuff.Controls.Add(this.darkGroupBox3);
            this.grpBuffDebuff.Controls.Add(this.darkGroupBox2);
            this.grpBuffDebuff.Controls.Add(this.darkGroupBox1);
            this.grpBuffDebuff.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpBuffDebuff.Location = new System.Drawing.Point(0, 409);
            this.grpBuffDebuff.Name = "grpBuffDebuff";
            this.grpBuffDebuff.Size = new System.Drawing.Size(440, 390);
            this.grpBuffDebuff.TabIndex = 39;
            this.grpBuffDebuff.TabStop = false;
            this.grpBuffDebuff.Text = "Combat Spell";
            this.grpBuffDebuff.Visible = false;
            // 
            // darkGroupBox5
            // 
            this.darkGroupBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.darkGroupBox5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.darkGroupBox5.Controls.Add(this.chkHOTDOT);
            this.darkGroupBox5.Controls.Add(this.lblTick);
            this.darkGroupBox5.Controls.Add(this.scrlTick);
            this.darkGroupBox5.ForeColor = System.Drawing.Color.Gainsboro;
            this.darkGroupBox5.Location = new System.Drawing.Point(6, 313);
            this.darkGroupBox5.Name = "darkGroupBox5";
            this.darkGroupBox5.Size = new System.Drawing.Size(188, 68);
            this.darkGroupBox5.TabIndex = 53;
            this.darkGroupBox5.TabStop = false;
            this.darkGroupBox5.Text = "Heal/Damage Over Time";
            // 
            // chkHOTDOT
            // 
            this.chkHOTDOT.Location = new System.Drawing.Point(5, 19);
            this.chkHOTDOT.Name = "chkHOTDOT";
            this.chkHOTDOT.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkHOTDOT.Size = new System.Drawing.Size(86, 24);
            this.chkHOTDOT.TabIndex = 22;
            this.chkHOTDOT.Text = "HOT/DOT?";
            this.chkHOTDOT.CheckedChanged += new System.EventHandler(this.chkHOTDOT_CheckedChanged);
            // 
            // lblTick
            // 
            this.lblTick.AutoSize = true;
            this.lblTick.Location = new System.Drawing.Point(100, 24);
            this.lblTick.Name = "lblTick";
            this.lblTick.Size = new System.Drawing.Size(54, 13);
            this.lblTick.TabIndex = 38;
            this.lblTick.Text = "Tick: 0.0s";
            // 
            // scrlTick
            // 
            this.scrlTick.Location = new System.Drawing.Point(103, 41);
            this.scrlTick.Maximum = 600;
            this.scrlTick.Name = "scrlTick";
            this.scrlTick.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlTick.Size = new System.Drawing.Size(73, 18);
            this.scrlTick.TabIndex = 37;
            this.scrlTick.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlTick_Scroll);
            // 
            // darkGroupBox4
            // 
            this.darkGroupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.darkGroupBox4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.darkGroupBox4.Controls.Add(this.label7);
            this.darkGroupBox4.Controls.Add(this.cmbExtraEffect);
            this.darkGroupBox4.Controls.Add(this.picSprite);
            this.darkGroupBox4.Controls.Add(this.cmbTransform);
            this.darkGroupBox4.Controls.Add(this.lblSprite);
            this.darkGroupBox4.ForeColor = System.Drawing.Color.Gainsboro;
            this.darkGroupBox4.Location = new System.Drawing.Point(201, 214);
            this.darkGroupBox4.Name = "darkGroupBox4";
            this.darkGroupBox4.Size = new System.Drawing.Size(233, 167);
            this.darkGroupBox4.TabIndex = 52;
            this.darkGroupBox4.TabStop = false;
            this.darkGroupBox4.Text = "Effect";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 35;
            this.label7.Text = "Extra Effect:";
            // 
            // cmbExtraEffect
            // 
            this.cmbExtraEffect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbExtraEffect.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbExtraEffect.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbExtraEffect.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbExtraEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExtraEffect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbExtraEffect.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbExtraEffect.FormattingEnabled = true;
            this.cmbExtraEffect.Items.AddRange(new object[] {
            "None",
            "Silence",
            "Stun",
            "Snare",
            "Blind",
            "Stealth",
            "Transform"});
            this.cmbExtraEffect.Location = new System.Drawing.Point(5, 31);
            this.cmbExtraEffect.Name = "cmbExtraEffect";
            this.cmbExtraEffect.Size = new System.Drawing.Size(80, 21);
            this.cmbExtraEffect.TabIndex = 36;
            this.cmbExtraEffect.SelectedIndexChanged += new System.EventHandler(this.cmbExtraEffect_SelectedIndexChanged);
            // 
            // picSprite
            // 
            this.picSprite.BackColor = System.Drawing.Color.Black;
            this.picSprite.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picSprite.Location = new System.Drawing.Point(5, 61);
            this.picSprite.Name = "picSprite";
            this.picSprite.Size = new System.Drawing.Size(222, 100);
            this.picSprite.TabIndex = 43;
            this.picSprite.TabStop = false;
            // 
            // cmbTransform
            // 
            this.cmbTransform.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTransform.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTransform.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTransform.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTransform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransform.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTransform.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTransform.FormattingEnabled = true;
            this.cmbTransform.Items.AddRange(new object[] {
            "None"});
            this.cmbTransform.Location = new System.Drawing.Point(137, 31);
            this.cmbTransform.Name = "cmbTransform";
            this.cmbTransform.Size = new System.Drawing.Size(80, 21);
            this.cmbTransform.TabIndex = 44;
            this.cmbTransform.SelectedIndexChanged += new System.EventHandler(this.cmbTransform_SelectedIndexChanged);
            // 
            // lblSprite
            // 
            this.lblSprite.AutoSize = true;
            this.lblSprite.Location = new System.Drawing.Point(134, 15);
            this.lblSprite.Name = "lblSprite";
            this.lblSprite.Size = new System.Drawing.Size(37, 13);
            this.lblSprite.TabIndex = 40;
            this.lblSprite.Text = "Sprite:";
            // 
            // darkGroupBox3
            // 
            this.darkGroupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.darkGroupBox3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.darkGroupBox3.Controls.Add(this.lblBuffDuration);
            this.darkGroupBox3.Controls.Add(this.scrlBuffDuration);
            this.darkGroupBox3.ForeColor = System.Drawing.Color.Gainsboro;
            this.darkGroupBox3.Location = new System.Drawing.Point(201, 170);
            this.darkGroupBox3.Name = "darkGroupBox3";
            this.darkGroupBox3.Size = new System.Drawing.Size(233, 41);
            this.darkGroupBox3.TabIndex = 51;
            this.darkGroupBox3.TabStop = false;
            this.darkGroupBox3.Text = "Stat Boost/Effect Duration";
            // 
            // lblBuffDuration
            // 
            this.lblBuffDuration.AutoSize = true;
            this.lblBuffDuration.Location = new System.Drawing.Point(6, 16);
            this.lblBuffDuration.Name = "lblBuffDuration";
            this.lblBuffDuration.Size = new System.Drawing.Size(73, 13);
            this.lblBuffDuration.TabIndex = 33;
            this.lblBuffDuration.Text = "Duration: 0.0s";
            // 
            // scrlBuffDuration
            // 
            this.scrlBuffDuration.Location = new System.Drawing.Point(123, 16);
            this.scrlBuffDuration.Maximum = 6000;
            this.scrlBuffDuration.Name = "scrlBuffDuration";
            this.scrlBuffDuration.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlBuffDuration.Size = new System.Drawing.Size(80, 18);
            this.scrlBuffDuration.TabIndex = 34;
            this.scrlBuffDuration.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlBuffDuration_Scroll);
            // 
            // darkGroupBox2
            // 
            this.darkGroupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.darkGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.darkGroupBox2.Controls.Add(this.lblSpeed);
            this.darkGroupBox2.Controls.Add(this.lblMagicResist);
            this.darkGroupBox2.Controls.Add(this.lblDefense);
            this.darkGroupBox2.Controls.Add(this.lblAbilityPwr);
            this.darkGroupBox2.Controls.Add(this.lblAttack);
            this.darkGroupBox2.Controls.Add(this.scrlDefense);
            this.darkGroupBox2.Controls.Add(this.scrlSpeed);
            this.darkGroupBox2.Controls.Add(this.scrlMagicResist);
            this.darkGroupBox2.Controls.Add(this.scrlAbilityPwr);
            this.darkGroupBox2.Controls.Add(this.scrlAttack);
            this.darkGroupBox2.ForeColor = System.Drawing.Color.Gainsboro;
            this.darkGroupBox2.Location = new System.Drawing.Point(201, 19);
            this.darkGroupBox2.Name = "darkGroupBox2";
            this.darkGroupBox2.Size = new System.Drawing.Size(233, 145);
            this.darkGroupBox2.TabIndex = 50;
            this.darkGroupBox2.TabStop = false;
            this.darkGroupBox2.Text = "Stat Modifiers";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(6, 103);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(50, 13);
            this.lblSpeed.TabIndex = 19;
            this.lblSpeed.Text = "Speed: 0";
            // 
            // lblMagicResist
            // 
            this.lblMagicResist.AutoSize = true;
            this.lblMagicResist.Location = new System.Drawing.Point(120, 62);
            this.lblMagicResist.Name = "lblMagicResist";
            this.lblMagicResist.Size = new System.Drawing.Size(80, 13);
            this.lblMagicResist.TabIndex = 18;
            this.lblMagicResist.Text = "Magic Resist: 0";
            // 
            // lblDefense
            // 
            this.lblDefense.AutoSize = true;
            this.lblDefense.Location = new System.Drawing.Point(6, 62);
            this.lblDefense.Name = "lblDefense";
            this.lblDefense.Size = new System.Drawing.Size(59, 13);
            this.lblDefense.TabIndex = 17;
            this.lblDefense.Text = "Defense: 0";
            // 
            // lblAbilityPwr
            // 
            this.lblAbilityPwr.AutoSize = true;
            this.lblAbilityPwr.Location = new System.Drawing.Point(120, 16);
            this.lblAbilityPwr.Name = "lblAbilityPwr";
            this.lblAbilityPwr.Size = new System.Drawing.Size(67, 13);
            this.lblAbilityPwr.TabIndex = 16;
            this.lblAbilityPwr.Text = "Ability Pwr: 0";
            // 
            // lblAttack
            // 
            this.lblAttack.AutoSize = true;
            this.lblAttack.Location = new System.Drawing.Point(6, 16);
            this.lblAttack.Name = "lblAttack";
            this.lblAttack.Size = new System.Drawing.Size(50, 13);
            this.lblAttack.TabIndex = 15;
            this.lblAttack.Text = "Attack: 0";
            // 
            // scrlDefense
            // 
            this.scrlDefense.Location = new System.Drawing.Point(9, 77);
            this.scrlDefense.Maximum = 255;
            this.scrlDefense.Minimum = -255;
            this.scrlDefense.Name = "scrlDefense";
            this.scrlDefense.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlDefense.Size = new System.Drawing.Size(80, 17);
            this.scrlDefense.TabIndex = 14;
            this.scrlDefense.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlDefense_Scroll);
            // 
            // scrlSpeed
            // 
            this.scrlSpeed.Location = new System.Drawing.Point(9, 116);
            this.scrlSpeed.Maximum = 255;
            this.scrlSpeed.Minimum = -255;
            this.scrlSpeed.Name = "scrlSpeed";
            this.scrlSpeed.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlSpeed.Size = new System.Drawing.Size(80, 17);
            this.scrlSpeed.TabIndex = 13;
            this.scrlSpeed.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlSpeed_Scroll);
            // 
            // scrlMagicResist
            // 
            this.scrlMagicResist.Location = new System.Drawing.Point(123, 77);
            this.scrlMagicResist.Maximum = 255;
            this.scrlMagicResist.Minimum = -255;
            this.scrlMagicResist.Name = "scrlMagicResist";
            this.scrlMagicResist.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlMagicResist.Size = new System.Drawing.Size(80, 17);
            this.scrlMagicResist.TabIndex = 12;
            this.scrlMagicResist.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlMagicResist_Scroll);
            // 
            // scrlAbilityPwr
            // 
            this.scrlAbilityPwr.Location = new System.Drawing.Point(123, 35);
            this.scrlAbilityPwr.Maximum = 255;
            this.scrlAbilityPwr.Minimum = -255;
            this.scrlAbilityPwr.Name = "scrlAbilityPwr";
            this.scrlAbilityPwr.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlAbilityPwr.Size = new System.Drawing.Size(80, 17);
            this.scrlAbilityPwr.TabIndex = 11;
            this.scrlAbilityPwr.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAbilityPwr_Scroll);
            // 
            // scrlAttack
            // 
            this.scrlAttack.Location = new System.Drawing.Point(9, 35);
            this.scrlAttack.Maximum = 255;
            this.scrlAttack.Minimum = -255;
            this.scrlAttack.Name = "scrlAttack";
            this.scrlAttack.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlAttack.Size = new System.Drawing.Size(80, 17);
            this.scrlAttack.TabIndex = 10;
            this.scrlAttack.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlAttack_Scroll);
            // 
            // darkGroupBox1
            // 
            this.darkGroupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.darkGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.darkGroupBox1.Controls.Add(this.cmbScalingStat);
            this.darkGroupBox1.Controls.Add(this.label3);
            this.darkGroupBox1.Controls.Add(this.chkFriendly);
            this.darkGroupBox1.Controls.Add(this.lblCritChance);
            this.darkGroupBox1.Controls.Add(this.scrlCritChance);
            this.darkGroupBox1.Controls.Add(this.lblScaling);
            this.darkGroupBox1.Controls.Add(this.scrlScaling);
            this.darkGroupBox1.Controls.Add(this.cmbDamageType);
            this.darkGroupBox1.Controls.Add(this.label11);
            this.darkGroupBox1.Controls.Add(this.lblHPDamage);
            this.darkGroupBox1.Controls.Add(this.scrlManaDamage);
            this.darkGroupBox1.Controls.Add(this.lblManaDamage);
            this.darkGroupBox1.Controls.Add(this.scrlHPDamage);
            this.darkGroupBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.darkGroupBox1.Location = new System.Drawing.Point(6, 19);
            this.darkGroupBox1.Name = "darkGroupBox1";
            this.darkGroupBox1.Size = new System.Drawing.Size(188, 288);
            this.darkGroupBox1.TabIndex = 49;
            this.darkGroupBox1.TabStop = false;
            this.darkGroupBox1.Text = "Damage";
            // 
            // cmbScalingStat
            // 
            this.cmbScalingStat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbScalingStat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbScalingStat.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbScalingStat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbScalingStat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScalingStat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbScalingStat.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbScalingStat.FormattingEnabled = true;
            this.cmbScalingStat.Location = new System.Drawing.Point(9, 159);
            this.cmbScalingStat.Name = "cmbScalingStat";
            this.cmbScalingStat.Size = new System.Drawing.Size(170, 21);
            this.cmbScalingStat.TabIndex = 57;
            this.cmbScalingStat.SelectedIndexChanged += new System.EventHandler(this.cmbScalingStat_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 56;
            this.label3.Text = "Scaling Stat:";
            // 
            // chkFriendly
            // 
            this.chkFriendly.AutoSize = true;
            this.chkFriendly.Location = new System.Drawing.Point(121, 10);
            this.chkFriendly.Name = "chkFriendly";
            this.chkFriendly.Size = new System.Drawing.Size(62, 17);
            this.chkFriendly.TabIndex = 55;
            this.chkFriendly.Text = "Friendly";
            this.chkFriendly.CheckedChanged += new System.EventHandler(this.chkFriendly_CheckedChanged);
            // 
            // lblCritChance
            // 
            this.lblCritChance.AutoSize = true;
            this.lblCritChance.Location = new System.Drawing.Point(7, 230);
            this.lblCritChance.Name = "lblCritChance";
            this.lblCritChance.Size = new System.Drawing.Size(82, 13);
            this.lblCritChance.TabIndex = 54;
            this.lblCritChance.Text = "Crit Chance: 0%";
            // 
            // scrlCritChance
            // 
            this.scrlCritChance.Location = new System.Drawing.Point(9, 246);
            this.scrlCritChance.Name = "scrlCritChance";
            this.scrlCritChance.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlCritChance.Size = new System.Drawing.Size(170, 17);
            this.scrlCritChance.TabIndex = 53;
            this.scrlCritChance.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlCritChance_Scroll);
            // 
            // lblScaling
            // 
            this.lblScaling.AutoSize = true;
            this.lblScaling.Location = new System.Drawing.Point(6, 189);
            this.lblScaling.Name = "lblScaling";
            this.lblScaling.Size = new System.Drawing.Size(107, 13);
            this.lblScaling.TabIndex = 52;
            this.lblScaling.Text = "Scaling Amount: x0.0";
            // 
            // scrlScaling
            // 
            this.scrlScaling.Location = new System.Drawing.Point(9, 206);
            this.scrlScaling.Maximum = 10000;
            this.scrlScaling.Name = "scrlScaling";
            this.scrlScaling.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlScaling.Size = new System.Drawing.Size(170, 17);
            this.scrlScaling.TabIndex = 51;
            this.scrlScaling.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlScaling_Scroll);
            // 
            // cmbDamageType
            // 
            this.cmbDamageType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDamageType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDamageType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDamageType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDamageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDamageType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDamageType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbDamageType.FormattingEnabled = true;
            this.cmbDamageType.Items.AddRange(new object[] {
            "Physical",
            "Magic",
            "True"});
            this.cmbDamageType.Location = new System.Drawing.Point(9, 117);
            this.cmbDamageType.Name = "cmbDamageType";
            this.cmbDamageType.Size = new System.Drawing.Size(170, 21);
            this.cmbDamageType.TabIndex = 50;
            this.cmbDamageType.SelectedIndexChanged += new System.EventHandler(this.cmbDamageType_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 100);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 13);
            this.label11.TabIndex = 49;
            this.label11.Text = "Damage Type:";
            // 
            // lblHPDamage
            // 
            this.lblHPDamage.AutoSize = true;
            this.lblHPDamage.Location = new System.Drawing.Point(6, 23);
            this.lblHPDamage.Name = "lblHPDamage";
            this.lblHPDamage.Size = new System.Drawing.Size(77, 13);
            this.lblHPDamage.TabIndex = 46;
            this.lblHPDamage.Text = "HP Damage: 0";
            // 
            // scrlManaDamage
            // 
            this.scrlManaDamage.Location = new System.Drawing.Point(9, 77);
            this.scrlManaDamage.Maximum = 10000;
            this.scrlManaDamage.Minimum = -10000;
            this.scrlManaDamage.Name = "scrlManaDamage";
            this.scrlManaDamage.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlManaDamage.Size = new System.Drawing.Size(170, 18);
            this.scrlManaDamage.TabIndex = 48;
            this.scrlManaDamage.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlManaDamage_Scroll);
            // 
            // lblManaDamage
            // 
            this.lblManaDamage.AutoSize = true;
            this.lblManaDamage.Location = new System.Drawing.Point(6, 62);
            this.lblManaDamage.Name = "lblManaDamage";
            this.lblManaDamage.Size = new System.Drawing.Size(89, 13);
            this.lblManaDamage.TabIndex = 47;
            this.lblManaDamage.Text = "Mana Damage: 0";
            // 
            // scrlHPDamage
            // 
            this.scrlHPDamage.Location = new System.Drawing.Point(9, 36);
            this.scrlHPDamage.Maximum = 10000;
            this.scrlHPDamage.Minimum = -10000;
            this.scrlHPDamage.Name = "scrlHPDamage";
            this.scrlHPDamage.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlHPDamage.Size = new System.Drawing.Size(170, 18);
            this.scrlHPDamage.TabIndex = 45;
            this.scrlHPDamage.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlHPDamage_Scroll);
            // 
            // grpDash
            // 
            this.grpDash.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpDash.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpDash.Controls.Add(this.groupBox5);
            this.grpDash.Controls.Add(this.lblRange);
            this.grpDash.Controls.Add(this.scrlRange);
            this.grpDash.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpDash.Location = new System.Drawing.Point(-4, 409);
            this.grpDash.Name = "grpDash";
            this.grpDash.Size = new System.Drawing.Size(200, 181);
            this.grpDash.TabIndex = 38;
            this.grpDash.TabStop = false;
            this.grpDash.Text = "Dash";
            this.grpDash.Visible = false;
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox5.Controls.Add(this.chkIgnoreInactiveResources);
            this.groupBox5.Controls.Add(this.chkIgnoreZDimensionBlocks);
            this.groupBox5.Controls.Add(this.chkIgnoreMapBlocks);
            this.groupBox5.Controls.Add(this.chkIgnoreActiveResources);
            this.groupBox5.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox5.Location = new System.Drawing.Point(12, 62);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(180, 106);
            this.groupBox5.TabIndex = 41;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Ignore Collision:";
            // 
            // chkIgnoreInactiveResources
            // 
            this.chkIgnoreInactiveResources.AutoSize = true;
            this.chkIgnoreInactiveResources.Location = new System.Drawing.Point(6, 62);
            this.chkIgnoreInactiveResources.Name = "chkIgnoreInactiveResources";
            this.chkIgnoreInactiveResources.Size = new System.Drawing.Size(118, 17);
            this.chkIgnoreInactiveResources.TabIndex = 38;
            this.chkIgnoreInactiveResources.Text = "Inactive Resources";
            this.chkIgnoreInactiveResources.CheckedChanged += new System.EventHandler(this.chkIgnoreInactiveResources_CheckedChanged);
            // 
            // chkIgnoreZDimensionBlocks
            // 
            this.chkIgnoreZDimensionBlocks.AutoSize = true;
            this.chkIgnoreZDimensionBlocks.Location = new System.Drawing.Point(6, 85);
            this.chkIgnoreZDimensionBlocks.Name = "chkIgnoreZDimensionBlocks";
            this.chkIgnoreZDimensionBlocks.Size = new System.Drawing.Size(120, 17);
            this.chkIgnoreZDimensionBlocks.TabIndex = 37;
            this.chkIgnoreZDimensionBlocks.Text = "Z-Dimension Blocks";
            this.chkIgnoreZDimensionBlocks.CheckedChanged += new System.EventHandler(this.chkIgnoreZDimensionBlocks_CheckedChanged);
            // 
            // chkIgnoreMapBlocks
            // 
            this.chkIgnoreMapBlocks.AutoSize = true;
            this.chkIgnoreMapBlocks.Location = new System.Drawing.Point(6, 16);
            this.chkIgnoreMapBlocks.Name = "chkIgnoreMapBlocks";
            this.chkIgnoreMapBlocks.Size = new System.Drawing.Size(82, 17);
            this.chkIgnoreMapBlocks.TabIndex = 33;
            this.chkIgnoreMapBlocks.Text = "Map Blocks";
            this.chkIgnoreMapBlocks.CheckedChanged += new System.EventHandler(this.chkIgnoreMapBlocks_CheckedChanged);
            // 
            // chkIgnoreActiveResources
            // 
            this.chkIgnoreActiveResources.AutoSize = true;
            this.chkIgnoreActiveResources.Location = new System.Drawing.Point(6, 39);
            this.chkIgnoreActiveResources.Name = "chkIgnoreActiveResources";
            this.chkIgnoreActiveResources.Size = new System.Drawing.Size(110, 17);
            this.chkIgnoreActiveResources.TabIndex = 36;
            this.chkIgnoreActiveResources.Text = "Active Resources";
            this.chkIgnoreActiveResources.CheckedChanged += new System.EventHandler(this.chkIgnoreActiveResources_CheckedChanged);
            // 
            // lblRange
            // 
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(11, 25);
            this.lblRange.Name = "lblRange";
            this.lblRange.Size = new System.Drawing.Size(51, 13);
            this.lblRange.TabIndex = 40;
            this.lblRange.Text = "Range: 0";
            // 
            // scrlRange
            // 
            this.scrlRange.Location = new System.Drawing.Point(14, 38);
            this.scrlRange.Maximum = 10;
            this.scrlRange.Name = "scrlRange";
            this.scrlRange.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlRange.Size = new System.Drawing.Size(168, 18);
            this.scrlRange.TabIndex = 39;
            this.scrlRange.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlRange_Scroll);
            // 
            // grpEvent
            // 
            this.grpEvent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpEvent.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEvent.Controls.Add(this.scrlEvent);
            this.grpEvent.Controls.Add(this.lblEvent);
            this.grpEvent.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEvent.Location = new System.Drawing.Point(6, 409);
            this.grpEvent.Name = "grpEvent";
            this.grpEvent.Size = new System.Drawing.Size(200, 67);
            this.grpEvent.TabIndex = 40;
            this.grpEvent.TabStop = false;
            this.grpEvent.Text = "Event";
            this.grpEvent.Visible = false;
            // 
            // scrlEvent
            // 
            this.scrlEvent.Location = new System.Drawing.Point(8, 36);
            this.scrlEvent.Minimum = -1;
            this.scrlEvent.Name = "scrlEvent";
            this.scrlEvent.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlEvent.Size = new System.Drawing.Size(186, 18);
            this.scrlEvent.TabIndex = 17;
            this.scrlEvent.Value = -1;
            this.scrlEvent.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlEvent_Scroll);
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
            // grpWarp
            // 
            this.grpWarp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpWarp.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpWarp.Controls.Add(this.lblWarpDir);
            this.grpWarp.Controls.Add(this.lblWarpX);
            this.grpWarp.Controls.Add(this.lblWarpY);
            this.grpWarp.Controls.Add(this.scrlWarpX);
            this.grpWarp.Controls.Add(this.scrlWarpDir);
            this.grpWarp.Controls.Add(this.scrlWarpY);
            this.grpWarp.Controls.Add(this.txtWarpChunk);
            this.grpWarp.Controls.Add(this.label16);
            this.grpWarp.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpWarp.Location = new System.Drawing.Point(2, 409);
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
            this.scrlWarpX.Location = new System.Drawing.Point(12, 70);
            this.scrlWarpX.Maximum = 29;
            this.scrlWarpX.Name = "scrlWarpX";
            this.scrlWarpX.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlWarpX.Size = new System.Drawing.Size(80, 17);
            this.scrlWarpX.TabIndex = 36;
            this.scrlWarpX.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlWarpX_Scroll);
            // 
            // scrlWarpDir
            // 
            this.scrlWarpDir.Location = new System.Drawing.Point(12, 119);
            this.scrlWarpDir.Maximum = 3;
            this.scrlWarpDir.Name = "scrlWarpDir";
            this.scrlWarpDir.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlWarpDir.Size = new System.Drawing.Size(80, 17);
            this.scrlWarpDir.TabIndex = 35;
            this.scrlWarpDir.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlWarpDir_Scroll);
            // 
            // scrlWarpY
            // 
            this.scrlWarpY.Location = new System.Drawing.Point(107, 70);
            this.scrlWarpY.Maximum = 29;
            this.scrlWarpY.Name = "scrlWarpY";
            this.scrlWarpY.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlWarpY.Size = new System.Drawing.Size(80, 17);
            this.scrlWarpY.TabIndex = 34;
            this.scrlWarpY.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlWarpY_Scroll);
            // 
            // txtWarpChunk
            // 
            this.txtWarpChunk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtWarpChunk.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWarpChunk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
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
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.groupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.groupBox1.Controls.Add(this.lstSpells);
            this.groupBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.groupBox1.Location = new System.Drawing.Point(12, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 473);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Spells";
            // 
            // lstSpells
            // 
            this.lstSpells.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstSpells.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSpells.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstSpells.FormattingEnabled = true;
            this.lstSpells.Location = new System.Drawing.Point(6, 19);
            this.lstSpells.Name = "lstSpells";
            this.lstSpells.Size = new System.Drawing.Size(191, 444);
            this.lstSpells.TabIndex = 1;
            this.lstSpells.Click += new System.EventHandler(this.lstSpells_Click);
            this.lstSpells.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemList_KeyDown);
            // 
            // frmSpell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(681, 549);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.pnlContainer);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "frmSpell";
            this.Text = "Spell Editor                       ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSpell_FormClosed);
            this.Load += new System.EventHandler(this.frmSpell_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.pnlContainer.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSpell)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.grpTargetInfo.ResumeLayout(false);
            this.grpTargetInfo.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grpBuffDebuff.ResumeLayout(false);
            this.darkGroupBox5.ResumeLayout(false);
            this.darkGroupBox5.PerformLayout();
            this.darkGroupBox4.ResumeLayout(false);
            this.darkGroupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSprite)).EndInit();
            this.darkGroupBox3.ResumeLayout(false);
            this.darkGroupBox3.PerformLayout();
            this.darkGroupBox2.ResumeLayout(false);
            this.darkGroupBox2.PerformLayout();
            this.darkGroupBox1.ResumeLayout(false);
            this.darkGroupBox1.PerformLayout();
            this.grpDash.ResumeLayout(false);
            this.grpDash.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.grpEvent.ResumeLayout(false);
            this.grpEvent.PerformLayout();
            this.grpWarp.ResumeLayout(false);
            this.grpWarp.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox groupBox1;
        private System.Windows.Forms.ListBox lstSpells;
        private DarkGroupBox groupBox2;
        private DarkScrollBar scrlCooldownDuration;
        private System.Windows.Forms.Label lblCooldownDuration;
        private DarkComboBox cmbSprite;
        private DarkScrollBar scrlCastDuration;
        private System.Windows.Forms.Label lblCastDuration;
        private System.Windows.Forms.Label lblPic;
        private System.Windows.Forms.PictureBox picSpell;
        private System.Windows.Forms.Label label1;
        private DarkTextBox txtName;
        private DarkGroupBox groupBox3;
        private System.Windows.Forms.Label lblSpeedReq;
        private System.Windows.Forms.Label lblMagicResistReq;
        private System.Windows.Forms.Label lblDefenseReq;
        private System.Windows.Forms.Label lblAbilityPwrReq;
        private System.Windows.Forms.Label lblAttackReq;
        private DarkScrollBar scrlDefenseReq;
        private DarkScrollBar scrlSpeedReq;
        private DarkScrollBar scrlMagicResistReq;
        private DarkScrollBar scrlAbilityPwrReq;
        private DarkScrollBar scrlAttackReq;
        private DarkScrollBar scrlHitAnimation;
        private System.Windows.Forms.Label lblHitAnimation;
        private DarkScrollBar scrlCastAnimation;
        private System.Windows.Forms.Label lblCastAnimation;
        private System.Windows.Forms.Label label2;
        private DarkComboBox cmbType;
        private System.Windows.Forms.Label lblLevelReq;
        private DarkScrollBar scrlLevelReq;
        private DarkGroupBox grpTargetInfo;
        private DarkScrollBar scrlHitRadius;
        private System.Windows.Forms.Label lblHitRadius;
        private DarkComboBox cmbTargetType;
        private DarkScrollBar scrlCastRange;
        private System.Windows.Forms.Label lblCastRange;
        private System.Windows.Forms.Label label4;
        private DarkGroupBox grpWarp;
        private System.Windows.Forms.Label lblWarpDir;
        private System.Windows.Forms.Label lblWarpX;
        private System.Windows.Forms.Label lblWarpY;
        private DarkScrollBar scrlWarpX;
        private DarkScrollBar scrlWarpDir;
        private DarkScrollBar scrlWarpY;
        private DarkTextBox txtWarpChunk;
        private System.Windows.Forms.Label label16;
        private DarkGroupBox groupBox4;
        private System.Windows.Forms.Label lblMPCost;
        private System.Windows.Forms.Label lblHPCost;
        private System.Windows.Forms.Label label6;
        private DarkTextBox txtDesc;
        private DarkGroupBox grpDash;
        private System.Windows.Forms.Label lblRange;
        private DarkScrollBar scrlRange;
        private DarkScrollBar scrlProjectile;
        private System.Windows.Forms.Label lblProjectile;
        private DarkGroupBox grpBuffDebuff;
        private DarkComboBox cmbExtraEffect;
        private System.Windows.Forms.Label label7;
        private DarkScrollBar scrlBuffDuration;
        private System.Windows.Forms.Label lblBuffDuration;
        private DarkCheckBox chkHOTDOT;
        private DarkScrollBar scrlTick;
        private System.Windows.Forms.Label lblTick;
        private DarkGroupBox groupBox5;
        private DarkCheckBox chkIgnoreInactiveResources;
        private DarkCheckBox chkIgnoreZDimensionBlocks;
        private DarkCheckBox chkIgnoreMapBlocks;
        private DarkCheckBox chkIgnoreActiveResources;
        private DarkComboBox cmbTransform;
        private System.Windows.Forms.PictureBox picSprite;
        private System.Windows.Forms.Label lblSprite;
        private DarkGroupBox grpEvent;
        private DarkScrollBar scrlEvent;
        private System.Windows.Forms.Label lblEvent;
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
        private DarkScrollBar scrlManaCost;
        private DarkScrollBar scrlHPCost;
        private DarkUI.Controls.DarkGroupBox darkGroupBox2;
        private DarkUI.Controls.DarkGroupBox darkGroupBox1;
        private System.Windows.Forms.Label lblHPDamage;
        private DarkScrollBar scrlManaDamage;
        private System.Windows.Forms.Label lblManaDamage;
        private DarkScrollBar scrlHPDamage;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblMagicResist;
        private System.Windows.Forms.Label lblDefense;
        private System.Windows.Forms.Label lblAbilityPwr;
        private System.Windows.Forms.Label lblAttack;
        private DarkScrollBar scrlDefense;
        private DarkScrollBar scrlSpeed;
        private DarkScrollBar scrlMagicResist;
        private DarkScrollBar scrlAbilityPwr;
        private DarkScrollBar scrlAttack;
        private System.Windows.Forms.Label lblScaling;
        private DarkScrollBar scrlScaling;
        private DarkComboBox cmbDamageType;
        private System.Windows.Forms.Label label11;
        private DarkUI.Controls.DarkGroupBox darkGroupBox5;
        private DarkUI.Controls.DarkGroupBox darkGroupBox4;
        private DarkUI.Controls.DarkGroupBox darkGroupBox3;
        private System.Windows.Forms.Label lblCritChance;
        private DarkScrollBar scrlCritChance;
        private DarkUI.Controls.DarkCheckBox chkFriendly;
        private DarkComboBox cmbScalingStat;
        private System.Windows.Forms.Label label3;
    }
}