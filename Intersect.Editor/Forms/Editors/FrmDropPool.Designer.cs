namespace Intersect.Editor.Forms.Editors
{
	partial class FrmDropPool
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDropPool));
			this.grpItems = new DarkUI.Controls.DarkGroupBox();
			this.nudDropAmount = new DarkUI.Controls.DarkNumericUpDown();
			this.nudDropChance = new DarkUI.Controls.DarkNumericUpDown();
			this.lblDropAmount = new System.Windows.Forms.Label();
			this.lblDropChance = new System.Windows.Forms.Label();
			this.btnDelItem = new DarkUI.Controls.DarkButton();
			this.btnAddItem = new DarkUI.Controls.DarkButton();
			this.cmbAddItem = new DarkUI.Controls.DarkComboBox();
			this.lblAddItem = new System.Windows.Forms.Label();
			this.lstItems = new System.Windows.Forms.ListBox();
			this.grpDropPool = new DarkUI.Controls.DarkGroupBox();
			this.lstDropLoot = new System.Windows.Forms.TreeView();
			this.toolStrip = new DarkUI.Controls.DarkToolStrip();
			this.toolStripItemNew = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripItemDelete = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.btnCancel = new DarkUI.Controls.DarkButton();
			this.btnSave = new DarkUI.Controls.DarkButton();
			this.grpGeneral = new DarkUI.Controls.DarkGroupBox();
			this.lblName = new System.Windows.Forms.Label();
			this.txtName = new DarkUI.Controls.DarkTextBox();
			this.grpItems.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudDropAmount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDropChance)).BeginInit();
			this.grpDropPool.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.grpGeneral.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpItems
			// 
			this.grpItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.grpItems.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpItems.Controls.Add(this.nudDropAmount);
			this.grpItems.Controls.Add(this.nudDropChance);
			this.grpItems.Controls.Add(this.lblDropAmount);
			this.grpItems.Controls.Add(this.lblDropChance);
			this.grpItems.Controls.Add(this.btnDelItem);
			this.grpItems.Controls.Add(this.btnAddItem);
			this.grpItems.Controls.Add(this.cmbAddItem);
			this.grpItems.Controls.Add(this.lblAddItem);
			this.grpItems.Controls.Add(this.lstItems);
			this.grpItems.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpItems.Location = new System.Drawing.Point(221, 101);
			this.grpItems.Name = "grpItems";
			this.grpItems.Size = new System.Drawing.Size(258, 394);
			this.grpItems.TabIndex = 53;
			this.grpItems.TabStop = false;
			this.grpItems.Text = "Items List";
			// 
			// nudDropAmount
			// 
			this.nudDropAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.nudDropAmount.ForeColor = System.Drawing.Color.Gainsboro;
			this.nudDropAmount.Location = new System.Drawing.Point(10, 294);
			this.nudDropAmount.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
			this.nudDropAmount.Name = "nudDropAmount";
			this.nudDropAmount.Size = new System.Drawing.Size(242, 20);
			this.nudDropAmount.TabIndex = 65;
			this.nudDropAmount.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// nudDropChance
			// 
			this.nudDropChance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.nudDropChance.DecimalPlaces = 2;
			this.nudDropChance.ForeColor = System.Drawing.Color.Gainsboro;
			this.nudDropChance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.nudDropChance.Location = new System.Drawing.Point(10, 340);
			this.nudDropChance.Name = "nudDropChance";
			this.nudDropChance.Size = new System.Drawing.Size(242, 20);
			this.nudDropChance.TabIndex = 64;
			this.nudDropChance.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// lblDropAmount
			// 
			this.lblDropAmount.AutoSize = true;
			this.lblDropAmount.Location = new System.Drawing.Point(7, 278);
			this.lblDropAmount.Name = "lblDropAmount";
			this.lblDropAmount.Size = new System.Drawing.Size(46, 13);
			this.lblDropAmount.TabIndex = 63;
			this.lblDropAmount.Text = "Amount:";
			// 
			// lblDropChance
			// 
			this.lblDropChance.AutoSize = true;
			this.lblDropChance.Location = new System.Drawing.Point(7, 323);
			this.lblDropChance.Name = "lblDropChance";
			this.lblDropChance.Size = new System.Drawing.Size(64, 13);
			this.lblDropChance.TabIndex = 62;
			this.lblDropChance.Text = "Chance (%):";
			// 
			// btnDelItem
			// 
			this.btnDelItem.Location = new System.Drawing.Point(135, 366);
			this.btnDelItem.Name = "btnDelItem";
			this.btnDelItem.Padding = new System.Windows.Forms.Padding(5);
			this.btnDelItem.Size = new System.Drawing.Size(117, 23);
			this.btnDelItem.TabIndex = 4;
			this.btnDelItem.Text = "Remove Selected";
			this.btnDelItem.Click += new System.EventHandler(this.btnDelItem_Click);
			// 
			// btnAddItem
			// 
			this.btnAddItem.Location = new System.Drawing.Point(6, 366);
			this.btnAddItem.Name = "btnAddItem";
			this.btnAddItem.Padding = new System.Windows.Forms.Padding(5);
			this.btnAddItem.Size = new System.Drawing.Size(117, 23);
			this.btnAddItem.TabIndex = 3;
			this.btnAddItem.Text = "Add Selected";
			this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
			// 
			// cmbAddItem
			// 
			this.cmbAddItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.cmbAddItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.cmbAddItem.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
			this.cmbAddItem.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.cmbAddItem.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbAddItem.ButtonIcon")));
			this.cmbAddItem.DrawDropdownHoverOutline = false;
			this.cmbAddItem.DrawFocusRectangle = false;
			this.cmbAddItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.cmbAddItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbAddItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmbAddItem.ForeColor = System.Drawing.Color.Gainsboro;
			this.cmbAddItem.FormattingEnabled = true;
			this.cmbAddItem.Location = new System.Drawing.Point(6, 254);
			this.cmbAddItem.Name = "cmbAddItem";
			this.cmbAddItem.Size = new System.Drawing.Size(246, 21);
			this.cmbAddItem.TabIndex = 2;
			this.cmbAddItem.Text = null;
			this.cmbAddItem.TextPadding = new System.Windows.Forms.Padding(2);
			// 
			// lblAddItem
			// 
			this.lblAddItem.AutoSize = true;
			this.lblAddItem.Location = new System.Drawing.Point(7, 238);
			this.lblAddItem.Name = "lblAddItem";
			this.lblAddItem.Size = new System.Drawing.Size(84, 13);
			this.lblAddItem.TabIndex = 1;
			this.lblAddItem.Text = "Add Item To List";
			// 
			// lstItems
			// 
			this.lstItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
			this.lstItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lstItems.ForeColor = System.Drawing.Color.Gainsboro;
			this.lstItems.FormattingEnabled = true;
			this.lstItems.Location = new System.Drawing.Point(7, 20);
			this.lstItems.Name = "lstItems";
			this.lstItems.Size = new System.Drawing.Size(245, 210);
			this.lstItems.TabIndex = 0;
			// 
			// grpDropPool
			// 
			this.grpDropPool.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.grpDropPool.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpDropPool.Controls.Add(this.lstDropLoot);
			this.grpDropPool.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpDropPool.Location = new System.Drawing.Point(12, 28);
			this.grpDropPool.Name = "grpDropPool";
			this.grpDropPool.Size = new System.Drawing.Size(203, 467);
			this.grpDropPool.TabIndex = 55;
			this.grpDropPool.TabStop = false;
			this.grpDropPool.Text = "Drop Loot";
			// 
			// lstDropLoot
			// 
			this.lstDropLoot.AllowDrop = true;
			this.lstDropLoot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
			this.lstDropLoot.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstDropLoot.ForeColor = System.Drawing.Color.Gainsboro;
			this.lstDropLoot.HideSelection = false;
			this.lstDropLoot.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
			this.lstDropLoot.Location = new System.Drawing.Point(6, 18);
			this.lstDropLoot.Name = "lstDropLoot";
			this.lstDropLoot.Size = new System.Drawing.Size(191, 443);
			this.lstDropLoot.TabIndex = 35;
			this.lstDropLoot.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.lstDropLoot_AfterSelect);
			this.lstDropLoot.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.lstDropLoot_NodeMouseClick);
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
            this.toolStripSeparator4,
            this.toolStripSeparator3});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
			this.toolStrip.Size = new System.Drawing.Size(487, 25);
			this.toolStrip.TabIndex = 54;
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
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.toolStripSeparator4.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(289, 501);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
			this.btnCancel.Size = new System.Drawing.Size(190, 27);
			this.btnCancel.TabIndex = 57;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(12, 501);
			this.btnSave.Name = "btnSave";
			this.btnSave.Padding = new System.Windows.Forms.Padding(5);
			this.btnSave.Size = new System.Drawing.Size(190, 27);
			this.btnSave.TabIndex = 56;
			this.btnSave.Text = "Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// grpGeneral
			// 
			this.grpGeneral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.grpGeneral.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpGeneral.Controls.Add(this.lblName);
			this.grpGeneral.Controls.Add(this.txtName);
			this.grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpGeneral.Location = new System.Drawing.Point(221, 28);
			this.grpGeneral.Name = "grpGeneral";
			this.grpGeneral.Size = new System.Drawing.Size(258, 67);
			this.grpGeneral.TabIndex = 58;
			this.grpGeneral.TabStop = false;
			this.grpGeneral.Text = "General";
			// 
			// lblName
			// 
			this.lblName.AutoSize = true;
			this.lblName.Location = new System.Drawing.Point(6, 17);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(38, 13);
			this.lblName.TabIndex = 1;
			this.lblName.Text = "Name:";
			// 
			// txtName
			// 
			this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.txtName.Location = new System.Drawing.Point(60, 16);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(192, 20);
			this.txtName.TabIndex = 0;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			// 
			// FrmDropPool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.ClientSize = new System.Drawing.Size(487, 540);
			this.ControlBox = false;
			this.Controls.Add(this.grpGeneral);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.grpItems);
			this.Controls.Add(this.grpDropPool);
			this.Controls.Add(this.toolStrip);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmDropPool";
			this.Text = "FrmDropPool";
			this.Load += new System.EventHandler(this.FrmDropPool_Load);
			this.grpItems.ResumeLayout(false);
			this.grpItems.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudDropAmount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudDropChance)).EndInit();
			this.grpDropPool.ResumeLayout(false);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.grpGeneral.ResumeLayout(false);
			this.grpGeneral.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private DarkUI.Controls.DarkGroupBox grpItems;
		private DarkUI.Controls.DarkButton btnDelItem;
		private DarkUI.Controls.DarkButton btnAddItem;
		private DarkUI.Controls.DarkComboBox cmbAddItem;
		private System.Windows.Forms.Label lblAddItem;
		private System.Windows.Forms.ListBox lstItems;
		private DarkUI.Controls.DarkGroupBox grpDropPool;
		public System.Windows.Forms.TreeView lstDropLoot;
		private DarkUI.Controls.DarkToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton toolStripItemNew;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripItemDelete;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private DarkUI.Controls.DarkButton btnCancel;
		private DarkUI.Controls.DarkButton btnSave;
		private DarkUI.Controls.DarkNumericUpDown nudDropAmount;
		private DarkUI.Controls.DarkNumericUpDown nudDropChance;
		private System.Windows.Forms.Label lblDropAmount;
		private System.Windows.Forms.Label lblDropChance;
		private DarkUI.Controls.DarkGroupBox grpGeneral;
		private System.Windows.Forms.Label lblName;
		private DarkUI.Controls.DarkTextBox txtName;
	}
}