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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lstProjectiles = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.picSpawns = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkIgnoreInactiveResources = new System.Windows.Forms.CheckBox();
            this.chkIgnoreZDimensionBlocks = new System.Windows.Forms.CheckBox();
            this.chkIgnoreMapBlocks = new System.Windows.Forms.CheckBox();
            this.chkIgnoreActiveResources = new System.Windows.Forms.CheckBox();
            this.lblSpell = new System.Windows.Forms.Label();
            this.scrlSpell = new System.Windows.Forms.HScrollBar();
            this.chkHoming = new System.Windows.Forms.CheckBox();
            this.chkRotation = new System.Windows.Forms.CheckBox();
            this.lblAnimation = new System.Windows.Forms.Label();
            this.scrlAnimation = new System.Windows.Forms.HScrollBar();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblRange = new System.Windows.Forms.Label();
            this.scrlRange = new System.Windows.Forms.HScrollBar();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.lblSpawn = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.scrlQuantity = new System.Windows.Forms.HScrollBar();
            this.scrlSpawn = new System.Windows.Forms.HScrollBar();
            this.scrlSpeed = new System.Windows.Forms.HScrollBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSpawns)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.lstProjectiles);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 579);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Projectiles";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(6, 546);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(191, 27);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.Location = new System.Drawing.Point(6, 513);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(191, 27);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(6, 480);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(191, 27);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lstProjectiles
            // 
            this.lstProjectiles.FormattingEnabled = true;
            this.lstProjectiles.Location = new System.Drawing.Point(6, 19);
            this.lstProjectiles.Name = "lstProjectiles";
            this.lstProjectiles.Size = new System.Drawing.Size(191, 459);
            this.lstProjectiles.TabIndex = 1;
            this.lstProjectiles.Click += new System.EventHandler(this.lstProjectiles_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.picSpawns);
            this.groupBox2.Location = new System.Drawing.Point(221, 399);
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
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.lblSpell);
            this.groupBox3.Controls.Add(this.scrlSpell);
            this.groupBox3.Controls.Add(this.chkHoming);
            this.groupBox3.Controls.Add(this.chkRotation);
            this.groupBox3.Controls.Add(this.lblAnimation);
            this.groupBox3.Controls.Add(this.scrlAnimation);
            this.groupBox3.Controls.Add(this.lblName);
            this.groupBox3.Controls.Add(this.txtName);
            this.groupBox3.Controls.Add(this.lblRange);
            this.groupBox3.Controls.Add(this.scrlRange);
            this.groupBox3.Controls.Add(this.lblQuantity);
            this.groupBox3.Controls.Add(this.lblSpawn);
            this.groupBox3.Controls.Add(this.lblSpeed);
            this.groupBox3.Controls.Add(this.scrlQuantity);
            this.groupBox3.Controls.Add(this.scrlSpawn);
            this.groupBox3.Controls.Add(this.scrlSpeed);
            this.groupBox3.Location = new System.Drawing.Point(221, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(186, 381);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Properties";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkIgnoreInactiveResources);
            this.groupBox4.Controls.Add(this.chkIgnoreZDimensionBlocks);
            this.groupBox4.Controls.Add(this.chkIgnoreMapBlocks);
            this.groupBox4.Controls.Add(this.chkIgnoreActiveResources);
            this.groupBox4.Location = new System.Drawing.Point(18, 243);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(157, 112);
            this.groupBox4.TabIndex = 28;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Ignore:";
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
            // lblSpell
            // 
            this.lblSpell.AutoSize = true;
            this.lblSpell.Location = new System.Drawing.Point(15, 210);
            this.lblSpell.Name = "lblSpell";
            this.lblSpell.Size = new System.Drawing.Size(112, 13);
            this.lblSpell.TabIndex = 24;
            this.lblSpell.Text = "Collision Spell: 0 None";
            // 
            // scrlSpell
            // 
            this.scrlSpell.Location = new System.Drawing.Point(16, 223);
            this.scrlSpell.Name = "scrlSpell";
            this.scrlSpell.Size = new System.Drawing.Size(160, 17);
            this.scrlSpell.TabIndex = 23;
            this.scrlSpell.Value = 1;
            this.scrlSpell.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpell_Scroll);
            // 
            // chkHoming
            // 
            this.chkHoming.AutoSize = true;
            this.chkHoming.Location = new System.Drawing.Point(6, 358);
            this.chkHoming.Name = "chkHoming";
            this.chkHoming.Size = new System.Drawing.Size(68, 17);
            this.chkHoming.TabIndex = 34;
            this.chkHoming.Text = "Homing?";
            this.chkHoming.UseVisualStyleBackColor = true;
            this.chkHoming.CheckedChanged += new System.EventHandler(this.chkHoming_CheckedChanged);
            // 
            // chkRotation
            // 
            this.chkRotation.AutoSize = true;
            this.chkRotation.Location = new System.Drawing.Point(91, 358);
            this.chkRotation.Name = "chkRotation";
            this.chkRotation.Size = new System.Drawing.Size(89, 17);
            this.chkRotation.TabIndex = 35;
            this.chkRotation.Text = "Auto Rotate?";
            this.chkRotation.UseVisualStyleBackColor = true;
            this.chkRotation.CheckedChanged += new System.EventHandler(this.chkRotation_CheckedChanged);
            // 
            // lblAnimation
            // 
            this.lblAnimation.AutoSize = true;
            this.lblAnimation.Location = new System.Drawing.Point(15, 46);
            this.lblAnimation.Name = "lblAnimation";
            this.lblAnimation.Size = new System.Drawing.Size(85, 13);
            this.lblAnimation.TabIndex = 21;
            this.lblAnimation.Text = "Animation: None";
            // 
            // scrlAnimation
            // 
            this.scrlAnimation.Location = new System.Drawing.Point(15, 62);
            this.scrlAnimation.Maximum = 5000;
            this.scrlAnimation.Minimum = -1;
            this.scrlAnimation.Name = "scrlAnimation";
            this.scrlAnimation.Size = new System.Drawing.Size(160, 17);
            this.scrlAnimation.TabIndex = 20;
            this.scrlAnimation.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlAnimation_Scroll);
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
            this.txtName.Location = new System.Drawing.Point(75, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(100, 20);
            this.txtName.TabIndex = 18;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblRange
            // 
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(15, 180);
            this.lblRange.Name = "lblRange";
            this.lblRange.Size = new System.Drawing.Size(51, 13);
            this.lblRange.TabIndex = 7;
            this.lblRange.Text = "Range: 1";
            // 
            // scrlRange
            // 
            this.scrlRange.LargeChange = 1;
            this.scrlRange.Location = new System.Drawing.Point(15, 193);
            this.scrlRange.Minimum = 1;
            this.scrlRange.Name = "scrlRange";
            this.scrlRange.Size = new System.Drawing.Size(160, 17);
            this.scrlRange.TabIndex = 6;
            this.scrlRange.Value = 1;
            this.scrlRange.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlRange_Scroll);
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(15, 147);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(58, 13);
            this.lblQuantity.TabIndex = 5;
            this.lblQuantity.Text = "Quantity: 1";
            // 
            // lblSpawn
            // 
            this.lblSpawn.AutoSize = true;
            this.lblSpawn.Location = new System.Drawing.Point(15, 115);
            this.lblSpawn.Name = "lblSpawn";
            this.lblSpawn.Size = new System.Drawing.Size(95, 13);
            this.lblSpawn.TabIndex = 4;
            this.lblSpawn.Text = "Spawn Delay: 1ms";
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(15, 79);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(63, 13);
            this.lblSpeed.TabIndex = 3;
            this.lblSpeed.Text = "Speed: 1ms";
            // 
            // scrlQuantity
            // 
            this.scrlQuantity.LargeChange = 1;
            this.scrlQuantity.Location = new System.Drawing.Point(15, 160);
            this.scrlQuantity.Minimum = 1;
            this.scrlQuantity.Name = "scrlQuantity";
            this.scrlQuantity.Size = new System.Drawing.Size(160, 17);
            this.scrlQuantity.TabIndex = 2;
            this.scrlQuantity.Value = 1;
            this.scrlQuantity.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlQuantity_Scroll);
            // 
            // scrlSpawn
            // 
            this.scrlSpawn.Location = new System.Drawing.Point(15, 128);
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
            this.scrlSpeed.Location = new System.Drawing.Point(15, 95);
            this.scrlSpeed.Maximum = 5000;
            this.scrlSpeed.Minimum = 1;
            this.scrlSpeed.Name = "scrlSpeed";
            this.scrlSpeed.Size = new System.Drawing.Size(160, 17);
            this.scrlSpeed.TabIndex = 0;
            this.scrlSpeed.Value = 1;
            this.scrlSpeed.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpeed_Scroll);
            // 
            // frmProjectile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 603);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmProjectile";
            this.Text = "Projectile Editor";
            this.Load += new System.EventHandler(this.frmProjectile_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picSpawns)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lstProjectiles;
        private System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.PictureBox picSpawns;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblRange;
        private System.Windows.Forms.HScrollBar scrlRange;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.Label lblSpawn;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.HScrollBar scrlQuantity;
        private System.Windows.Forms.HScrollBar scrlSpawn;
        private System.Windows.Forms.HScrollBar scrlSpeed;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblAnimation;
        private System.Windows.Forms.HScrollBar scrlAnimation;
        private System.Windows.Forms.Label lblSpell;
        private System.Windows.Forms.HScrollBar scrlSpell;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkIgnoreInactiveResources;
        private System.Windows.Forms.CheckBox chkIgnoreZDimensionBlocks;
        private System.Windows.Forms.CheckBox chkIgnoreMapBlocks;
        private System.Windows.Forms.CheckBox chkIgnoreActiveResources;
        private System.Windows.Forms.CheckBox chkRotation;
        private System.Windows.Forms.CheckBox chkHoming;
    }
}