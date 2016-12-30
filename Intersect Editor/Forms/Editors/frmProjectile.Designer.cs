namespace Intersect_Editor.Classes
{
    partial class frmProjectile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProjectile));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstProjectiles = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.picSpawns = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblKnockback = new System.Windows.Forms.Label();
            this.scrlKnockback = new System.Windows.Forms.HScrollBar();
            this.lblAmount = new System.Windows.Forms.Label();
            this.scrlAmount = new System.Windows.Forms.HScrollBar();
            this.chkGrapple = new System.Windows.Forms.CheckBox();
            this.lblSpell = new System.Windows.Forms.Label();
            this.scrlSpell = new System.Windows.Forms.HScrollBar();
            this.chkHoming = new System.Windows.Forms.CheckBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblRange = new System.Windows.Forms.Label();
            this.scrlRange = new System.Windows.Forms.HScrollBar();
            this.lblSpawn = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.scrlSpawn = new System.Windows.Forms.HScrollBar();
            this.scrlSpeed = new System.Windows.Forms.HScrollBar();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.chkRotation = new System.Windows.Forms.CheckBox();
            this.lblAnimation = new System.Windows.Forms.Label();
            this.scrlAnimation = new System.Windows.Forms.HScrollBar();
            this.lstAnimations = new System.Windows.Forms.ListBox();
            this.lblSpawnRange = new System.Windows.Forms.Label();
            this.scrlSpawnRange = new System.Windows.Forms.HScrollBar();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkIgnoreInactiveResources = new System.Windows.Forms.CheckBox();
            this.chkIgnoreZDimensionBlocks = new System.Windows.Forms.CheckBox();
            this.chkIgnoreMapBlocks = new System.Windows.Forms.CheckBox();
            this.chkIgnoreActiveResources = new System.Windows.Forms.CheckBox();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
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
            ((System.ComponentModel.ISupportInitialize)(this.picSpawns)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstProjectiles);
            this.groupBox1.Location = new System.Drawing.Point(12, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 421);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Projectiles";
            // 
            // lstProjectiles
            // 
            this.lstProjectiles.FormattingEnabled = true;
            this.lstProjectiles.Location = new System.Drawing.Point(6, 19);
            this.lstProjectiles.Name = "lstProjectiles";
            this.lstProjectiles.Size = new System.Drawing.Size(191, 394);
            this.lstProjectiles.TabIndex = 1;
            this.lstProjectiles.Click += new System.EventHandler(this.lstProjectiles_Click);
            this.lstProjectiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemList_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.picSpawns);
            this.groupBox2.Location = new System.Drawing.Point(1, 259);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(186, 192);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Projectile Spawns";
            // 
            // picSpawns
            // 
            this.picSpawns.Location = new System.Drawing.Point(15, 19);
            this.picSpawns.Name = "picSpawns";
            this.picSpawns.Size = new System.Drawing.Size(160, 160);
            this.picSpawns.TabIndex = 17;
            this.picSpawns.TabStop = false;
            this.picSpawns.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picSpawns_MouseDown);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblKnockback);
            this.groupBox3.Controls.Add(this.scrlKnockback);
            this.groupBox3.Controls.Add(this.lblAmount);
            this.groupBox3.Controls.Add(this.scrlAmount);
            this.groupBox3.Controls.Add(this.chkGrapple);
            this.groupBox3.Controls.Add(this.lblSpell);
            this.groupBox3.Controls.Add(this.scrlSpell);
            this.groupBox3.Controls.Add(this.chkHoming);
            this.groupBox3.Controls.Add(this.lblName);
            this.groupBox3.Controls.Add(this.txtName);
            this.groupBox3.Controls.Add(this.lblRange);
            this.groupBox3.Controls.Add(this.scrlRange);
            this.groupBox3.Controls.Add(this.lblSpawn);
            this.groupBox3.Controls.Add(this.lblSpeed);
            this.groupBox3.Controls.Add(this.scrlSpawn);
            this.groupBox3.Controls.Add(this.scrlSpeed);
            this.groupBox3.Location = new System.Drawing.Point(1, 1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(186, 252);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Properties";
            // 
            // lblKnockback
            // 
            this.lblKnockback.AutoSize = true;
            this.lblKnockback.Location = new System.Drawing.Point(15, 196);
            this.lblKnockback.Name = "lblKnockback";
            this.lblKnockback.Size = new System.Drawing.Size(74, 13);
            this.lblKnockback.TabIndex = 40;
            this.lblKnockback.Text = "Knockback: 0";
            // 
            // scrlKnockback
            // 
            this.scrlKnockback.LargeChange = 1;
            this.scrlKnockback.Location = new System.Drawing.Point(15, 209);
            this.scrlKnockback.Name = "scrlKnockback";
            this.scrlKnockback.Size = new System.Drawing.Size(160, 17);
            this.scrlKnockback.TabIndex = 39;
            this.scrlKnockback.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlKnockback_Scroll);
            // 
            // lblAmount
            // 
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(15, 108);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(58, 13);
            this.lblAmount.TabIndex = 38;
            this.lblAmount.Text = "Quantity: 1";
            // 
            // scrlAmount
            // 
            this.scrlAmount.Location = new System.Drawing.Point(15, 121);
            this.scrlAmount.Minimum = 1;
            this.scrlAmount.Name = "scrlAmount";
            this.scrlAmount.Size = new System.Drawing.Size(160, 17);
            this.scrlAmount.TabIndex = 37;
            this.scrlAmount.Value = 1;
            this.scrlAmount.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlAmount_Scroll);
            // 
            // chkGrapple
            // 
            this.chkGrapple.AutoSize = true;
            this.chkGrapple.Location = new System.Drawing.Point(15, 229);
            this.chkGrapple.Name = "chkGrapple";
            this.chkGrapple.Size = new System.Drawing.Size(90, 17);
            this.chkGrapple.TabIndex = 36;
            this.chkGrapple.Text = "Graple hook?";
            this.chkGrapple.UseVisualStyleBackColor = true;
            this.chkGrapple.CheckedChanged += new System.EventHandler(this.chkGrapple_CheckedChanged);
            // 
            // lblSpell
            // 
            this.lblSpell.AutoSize = true;
            this.lblSpell.Location = new System.Drawing.Point(13, 167);
            this.lblSpell.Name = "lblSpell";
            this.lblSpell.Size = new System.Drawing.Size(112, 13);
            this.lblSpell.TabIndex = 24;
            this.lblSpell.Text = "Collision Spell: 0 None";
            // 
            // scrlSpell
            // 
            this.scrlSpell.LargeChange = 1;
            this.scrlSpell.Location = new System.Drawing.Point(16, 180);
            this.scrlSpell.Minimum = -1;
            this.scrlSpell.Name = "scrlSpell";
            this.scrlSpell.Size = new System.Drawing.Size(160, 17);
            this.scrlSpell.TabIndex = 23;
            this.scrlSpell.Value = -1;
            this.scrlSpell.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpell_Scroll);
            // 
            // chkHoming
            // 
            this.chkHoming.AutoSize = true;
            this.chkHoming.Location = new System.Drawing.Point(107, 229);
            this.chkHoming.Name = "chkHoming";
            this.chkHoming.Size = new System.Drawing.Size(68, 17);
            this.chkHoming.TabIndex = 8;
            this.chkHoming.Text = "Homing?";
            this.chkHoming.UseVisualStyleBackColor = true;
            this.chkHoming.Visible = false;
            this.chkHoming.CheckedChanged += new System.EventHandler(this.chkHoming_CheckedChanged);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(13, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 19;
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(57, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(118, 20);
            this.txtName.TabIndex = 18;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblRange
            // 
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(15, 138);
            this.lblRange.Name = "lblRange";
            this.lblRange.Size = new System.Drawing.Size(51, 13);
            this.lblRange.TabIndex = 7;
            this.lblRange.Text = "Range: 1";
            // 
            // scrlRange
            // 
            this.scrlRange.LargeChange = 1;
            this.scrlRange.Location = new System.Drawing.Point(15, 150);
            this.scrlRange.Minimum = 1;
            this.scrlRange.Name = "scrlRange";
            this.scrlRange.Size = new System.Drawing.Size(160, 17);
            this.scrlRange.TabIndex = 6;
            this.scrlRange.Value = 1;
            this.scrlRange.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlRange_Scroll);
            // 
            // lblSpawn
            // 
            this.lblSpawn.AutoSize = true;
            this.lblSpawn.Location = new System.Drawing.Point(15, 78);
            this.lblSpawn.Name = "lblSpawn";
            this.lblSpawn.Size = new System.Drawing.Size(95, 13);
            this.lblSpawn.TabIndex = 4;
            this.lblSpawn.Text = "Spawn Delay: 1ms";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(15, 42);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(63, 13);
            this.lblSpeed.TabIndex = 3;
            this.lblSpeed.Text = "Speed: 1ms";
            // 
            // scrlSpawn
            // 
            this.scrlSpawn.Location = new System.Drawing.Point(15, 91);
            this.scrlSpawn.Maximum = 5000;
            this.scrlSpawn.Minimum = 1;
            this.scrlSpawn.Name = "scrlSpawn";
            this.scrlSpawn.Size = new System.Drawing.Size(160, 17);
            this.scrlSpawn.TabIndex = 1;
            this.scrlSpawn.Value = 1;
            this.scrlSpawn.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpawn_Scroll);
            // 
            // scrlSpeed
            // 
            this.scrlSpeed.Location = new System.Drawing.Point(15, 58);
            this.scrlSpeed.Maximum = 5000;
            this.scrlSpeed.Minimum = 1;
            this.scrlSpeed.Name = "scrlSpeed";
            this.scrlSpeed.Size = new System.Drawing.Size(160, 17);
            this.scrlSpeed.TabIndex = 0;
            this.scrlSpeed.Value = 1;
            this.scrlSpeed.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpeed_Scroll);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnRemove);
            this.groupBox5.Controls.Add(this.btnAdd);
            this.groupBox5.Controls.Add(this.chkRotation);
            this.groupBox5.Controls.Add(this.lblAnimation);
            this.groupBox5.Controls.Add(this.scrlAnimation);
            this.groupBox5.Controls.Add(this.lstAnimations);
            this.groupBox5.Controls.Add(this.lblSpawnRange);
            this.groupBox5.Controls.Add(this.scrlSpawnRange);
            this.groupBox5.Location = new System.Drawing.Point(193, 1);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(273, 309);
            this.groupBox5.TabIndex = 27;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Animations";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(188, 272);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 38;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 272);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 37;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // chkRotation
            // 
            this.chkRotation.AutoSize = true;
            this.chkRotation.Location = new System.Drawing.Point(12, 248);
            this.chkRotation.Name = "chkRotation";
            this.chkRotation.Size = new System.Drawing.Size(138, 17);
            this.chkRotation.TabIndex = 36;
            this.chkRotation.Text = "Auto Rotate Animation?";
            this.chkRotation.UseVisualStyleBackColor = true;
            this.chkRotation.CheckedChanged += new System.EventHandler(this.chkRotation_CheckedChanged);
            // 
            // lblAnimation
            // 
            this.lblAnimation.AutoSize = true;
            this.lblAnimation.Location = new System.Drawing.Point(10, 154);
            this.lblAnimation.Name = "lblAnimation";
            this.lblAnimation.Size = new System.Drawing.Size(85, 13);
            this.lblAnimation.TabIndex = 31;
            this.lblAnimation.Text = "Animation: None";
            // 
            // scrlAnimation
            // 
            this.scrlAnimation.LargeChange = 1;
            this.scrlAnimation.Location = new System.Drawing.Point(12, 173);
            this.scrlAnimation.Maximum = 5000;
            this.scrlAnimation.Minimum = -1;
            this.scrlAnimation.Name = "scrlAnimation";
            this.scrlAnimation.Size = new System.Drawing.Size(251, 17);
            this.scrlAnimation.TabIndex = 30;
            this.scrlAnimation.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlAnimation_Scroll);
            // 
            // lstAnimations
            // 
            this.lstAnimations.FormattingEnabled = true;
            this.lstAnimations.Items.AddRange(new object[] {
            "[Spawn Range: 1 - 1] Animation: None"});
            this.lstAnimations.Location = new System.Drawing.Point(12, 17);
            this.lstAnimations.Name = "lstAnimations";
            this.lstAnimations.Size = new System.Drawing.Size(251, 121);
            this.lstAnimations.TabIndex = 29;
            this.lstAnimations.Click += new System.EventHandler(this.lstAnimations_Click);
            // 
            // lblSpawnRange
            // 
            this.lblSpawnRange.AutoSize = true;
            this.lblSpawnRange.Location = new System.Drawing.Point(10, 206);
            this.lblSpawnRange.Name = "lblSpawnRange";
            this.lblSpawnRange.Size = new System.Drawing.Size(102, 13);
            this.lblSpawnRange.TabIndex = 28;
            this.lblSpawnRange.Text = "Spawn Range: 1 - 1";
            // 
            // scrlSpawnRange
            // 
            this.scrlSpawnRange.LargeChange = 1;
            this.scrlSpawnRange.Location = new System.Drawing.Point(12, 219);
            this.scrlSpawnRange.Minimum = 1;
            this.scrlSpawnRange.Name = "scrlSpawnRange";
            this.scrlSpawnRange.Size = new System.Drawing.Size(251, 17);
            this.scrlSpawnRange.TabIndex = 27;
            this.scrlSpawnRange.Value = 1;
            this.scrlSpawnRange.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpawnRange_Scroll);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkIgnoreInactiveResources);
            this.groupBox4.Controls.Add(this.chkIgnoreZDimensionBlocks);
            this.groupBox4.Controls.Add(this.chkIgnoreMapBlocks);
            this.groupBox4.Controls.Add(this.chkIgnoreActiveResources);
            this.groupBox4.Location = new System.Drawing.Point(193, 309);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(273, 142);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Ignore Collision:";
            // 
            // chkIgnoreInactiveResources
            // 
            this.chkIgnoreInactiveResources.AutoSize = true;
            this.chkIgnoreInactiveResources.Location = new System.Drawing.Point(6, 62);
            this.chkIgnoreInactiveResources.Name = "chkIgnoreInactiveResources";
            this.chkIgnoreInactiveResources.Size = new System.Drawing.Size(118, 17);
            this.chkIgnoreInactiveResources.TabIndex = 38;
            this.chkIgnoreInactiveResources.Text = "Inactive Resources";
            this.chkIgnoreInactiveResources.UseVisualStyleBackColor = true;
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
            this.chkIgnoreZDimensionBlocks.UseVisualStyleBackColor = true;
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
            this.chkIgnoreMapBlocks.UseVisualStyleBackColor = true;
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
            this.chkIgnoreActiveResources.UseVisualStyleBackColor = true;
            this.chkIgnoreActiveResources.CheckedChanged += new System.EventHandler(this.chkIgnoreActiveResources_CheckedChanged);
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.groupBox4);
            this.pnlContainer.Controls.Add(this.groupBox3);
            this.pnlContainer.Controls.Add(this.groupBox5);
            this.pnlContainer.Controls.Add(this.groupBox2);
            this.pnlContainer.Location = new System.Drawing.Point(221, 36);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(465, 454);
            this.pnlContainer.TabIndex = 30;
            this.pnlContainer.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(497, 496);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 34;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(301, 496);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 31;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
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
            this.toolStrip.Size = new System.Drawing.Size(694, 25);
            this.toolStrip.TabIndex = 46;
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
            // frmProjectile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(694, 527);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "frmProjectile";
            this.Text = "Projectile Editor";
            this.Load += new System.EventHandler(this.frmProjectile_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picSpawns)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.pnlContainer.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstProjectiles;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.PictureBox picSpawns;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblRange;
        private System.Windows.Forms.HScrollBar scrlRange;
        private System.Windows.Forms.Label lblSpawn;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.HScrollBar scrlSpawn;
        private System.Windows.Forms.HScrollBar scrlSpeed;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblSpell;
        private System.Windows.Forms.HScrollBar scrlSpell;
        private System.Windows.Forms.CheckBox chkHoming;
        private System.Windows.Forms.CheckBox chkGrapple;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox chkRotation;
        private System.Windows.Forms.Label lblAnimation;
        private System.Windows.Forms.HScrollBar scrlAnimation;
        private System.Windows.Forms.ListBox lstAnimations;
        private System.Windows.Forms.Label lblSpawnRange;
        private System.Windows.Forms.HScrollBar scrlSpawnRange;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkIgnoreInactiveResources;
        private System.Windows.Forms.CheckBox chkIgnoreZDimensionBlocks;
        private System.Windows.Forms.CheckBox chkIgnoreMapBlocks;
        private System.Windows.Forms.CheckBox chkIgnoreActiveResources;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.HScrollBar scrlAmount;
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblKnockback;
        private System.Windows.Forms.HScrollBar scrlKnockback;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripItemNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripItemDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton toolStripItemCopy;
        public System.Windows.Forms.ToolStripButton toolStripItemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.ToolStripButton toolStripItemUndo;
    }
}