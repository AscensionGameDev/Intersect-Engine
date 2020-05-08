namespace Intersect.Editor.Forms.Editors
{
	partial class frmHDV
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHDV));
			this.toolStrip = new DarkUI.Controls.DarkToolStrip();
			this.toolStripItemNew = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripItemDelete = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.btnChronological = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripItemCopy = new System.Windows.Forms.ToolStripButton();
			this.toolStripItemPaste = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripItemUndo = new System.Windows.Forms.ToolStripButton();
			this.grpHDV = new DarkUI.Controls.DarkGroupBox();
			this.lstHdv = new System.Windows.Forms.TreeView();
			this.pnlContainer = new System.Windows.Forms.Panel();
			this.grpItems = new DarkUI.Controls.DarkGroupBox();
			this.btnDelItem = new DarkUI.Controls.DarkButton();
			this.btnAddItem = new DarkUI.Controls.DarkButton();
			this.cmbAddItem = new DarkUI.Controls.DarkComboBox();
			this.lblAddItem = new System.Windows.Forms.Label();
			this.lstItems = new System.Windows.Forms.ListBox();
			this.grpGeneral = new DarkUI.Controls.DarkGroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.rdoBlacklist = new DarkUI.Controls.DarkRadioButton();
			this.rdoWhitelist = new DarkUI.Controls.DarkRadioButton();
			this.btnAddFolder = new DarkUI.Controls.DarkButton();
			this.lblFolder = new System.Windows.Forms.Label();
			this.cmbFolder = new DarkUI.Controls.DarkComboBox();
			this.cmbDefaultCurrency = new DarkUI.Controls.DarkComboBox();
			this.lblDefaultCurrency = new System.Windows.Forms.Label();
			this.lblName = new System.Windows.Forms.Label();
			this.txtName = new DarkUI.Controls.DarkTextBox();
			this.btnCancel = new DarkUI.Controls.DarkButton();
			this.btnSave = new DarkUI.Controls.DarkButton();
			this.toolStrip.SuspendLayout();
			this.grpHDV.SuspendLayout();
			this.pnlContainer.SuspendLayout();
			this.grpItems.SuspendLayout();
			this.grpGeneral.SuspendLayout();
			this.SuspendLayout();
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
            this.btnChronological,
            this.toolStripSeparator4,
            this.toolStripItemCopy,
            this.toolStripItemPaste,
            this.toolStripSeparator3,
            this.toolStripItemUndo});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
			this.toolStrip.Size = new System.Drawing.Size(751, 25);
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
			// btnChronological
			// 
			this.btnChronological.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnChronological.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.btnChronological.Image = ((System.Drawing.Image)(resources.GetObject("btnChronological.Image")));
			this.btnChronological.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnChronological.Name = "btnChronological";
			this.btnChronological.Size = new System.Drawing.Size(23, 22);
			this.btnChronological.Text = "Order Chronologically";
			this.btnChronological.Click += new System.EventHandler(this.btnChronological_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
			this.toolStripSeparator4.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
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
			// grpHDV
			// 
			this.grpHDV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.grpHDV.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpHDV.Controls.Add(this.lstHdv);
			this.grpHDV.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpHDV.Location = new System.Drawing.Point(12, 28);
			this.grpHDV.Name = "grpHDV";
			this.grpHDV.Size = new System.Drawing.Size(203, 467);
			this.grpHDV.TabIndex = 52;
			this.grpHDV.TabStop = false;
			this.grpHDV.Text = "HDV";
			// 
			// lstHdv
			// 
			this.lstHdv.AllowDrop = true;
			this.lstHdv.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
			this.lstHdv.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstHdv.ForeColor = System.Drawing.Color.Gainsboro;
			this.lstHdv.HideSelection = false;
			this.lstHdv.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
			this.lstHdv.Location = new System.Drawing.Point(6, 18);
			this.lstHdv.Name = "lstHdv";
			this.lstHdv.Size = new System.Drawing.Size(191, 443);
			this.lstHdv.TabIndex = 35;
			this.lstHdv.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.lstHdv_AfterSelect);
			this.lstHdv.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.lstHdv_NodeMouseClick);
			// 
			// pnlContainer
			// 
			this.pnlContainer.Controls.Add(this.grpItems);
			this.pnlContainer.Controls.Add(this.grpGeneral);
			this.pnlContainer.Location = new System.Drawing.Point(221, 28);
			this.pnlContainer.Name = "pnlContainer";
			this.pnlContainer.Size = new System.Drawing.Size(522, 467);
			this.pnlContainer.TabIndex = 53;
			this.pnlContainer.Visible = false;
			// 
			// grpItems
			// 
			this.grpItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.grpItems.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpItems.Controls.Add(this.btnDelItem);
			this.grpItems.Controls.Add(this.btnAddItem);
			this.grpItems.Controls.Add(this.cmbAddItem);
			this.grpItems.Controls.Add(this.lblAddItem);
			this.grpItems.Controls.Add(this.lstItems);
			this.grpItems.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpItems.Location = new System.Drawing.Point(8, 77);
			this.grpItems.Name = "grpItems";
			this.grpItems.Size = new System.Drawing.Size(258, 390);
			this.grpItems.TabIndex = 18;
			this.grpItems.TabStop = false;
			this.grpItems.Text = "Items List";
			// 
			// btnDelItem
			// 
			this.btnDelItem.Location = new System.Drawing.Point(135, 281);
			this.btnDelItem.Name = "btnDelItem";
			this.btnDelItem.Padding = new System.Windows.Forms.Padding(5);
			this.btnDelItem.Size = new System.Drawing.Size(117, 23);
			this.btnDelItem.TabIndex = 4;
			this.btnDelItem.Text = "Remove Selected";
			this.btnDelItem.Click += new System.EventHandler(this.btnDelItem_Click);
			// 
			// btnAddItem
			// 
			this.btnAddItem.Location = new System.Drawing.Point(6, 281);
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
			// grpGeneral
			// 
			this.grpGeneral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.grpGeneral.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.grpGeneral.Controls.Add(this.label1);
			this.grpGeneral.Controls.Add(this.rdoBlacklist);
			this.grpGeneral.Controls.Add(this.rdoWhitelist);
			this.grpGeneral.Controls.Add(this.btnAddFolder);
			this.grpGeneral.Controls.Add(this.lblFolder);
			this.grpGeneral.Controls.Add(this.cmbFolder);
			this.grpGeneral.Controls.Add(this.cmbDefaultCurrency);
			this.grpGeneral.Controls.Add(this.lblDefaultCurrency);
			this.grpGeneral.Controls.Add(this.lblName);
			this.grpGeneral.Controls.Add(this.txtName);
			this.grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
			this.grpGeneral.Location = new System.Drawing.Point(-1, 2);
			this.grpGeneral.Name = "grpGeneral";
			this.grpGeneral.Size = new System.Drawing.Size(523, 67);
			this.grpGeneral.TabIndex = 16;
			this.grpGeneral.TabStop = false;
			this.grpGeneral.Text = "General";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(247, 47);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 13);
			this.label1.TabIndex = 56;
			this.label1.Text = "Item List:";
			// 
			// rdoBlacklist
			// 
			this.rdoBlacklist.AutoSize = true;
			this.rdoBlacklist.Location = new System.Drawing.Point(412, 45);
			this.rdoBlacklist.Name = "rdoBlacklist";
			this.rdoBlacklist.Size = new System.Drawing.Size(64, 17);
			this.rdoBlacklist.TabIndex = 6;
			this.rdoBlacklist.Text = "Blacklist";
			this.rdoBlacklist.CheckedChanged += new System.EventHandler(this.rdoBuyBlacklist_CheckedChanged);
			// 
			// rdoWhitelist
			// 
			this.rdoWhitelist.AutoSize = true;
			this.rdoWhitelist.Checked = true;
			this.rdoWhitelist.Location = new System.Drawing.Point(341, 45);
			this.rdoWhitelist.Name = "rdoWhitelist";
			this.rdoWhitelist.Size = new System.Drawing.Size(65, 17);
			this.rdoWhitelist.TabIndex = 5;
			this.rdoWhitelist.TabStop = true;
			this.rdoWhitelist.Text = "Whitelist";
			this.rdoWhitelist.CheckedChanged += new System.EventHandler(this.rdoBuyWhitelist_CheckedChanged);
			// 
			// btnAddFolder
			// 
			this.btnAddFolder.Location = new System.Drawing.Point(197, 41);
			this.btnAddFolder.Name = "btnAddFolder";
			this.btnAddFolder.Padding = new System.Windows.Forms.Padding(5);
			this.btnAddFolder.Size = new System.Drawing.Size(18, 21);
			this.btnAddFolder.TabIndex = 55;
			this.btnAddFolder.Text = "+";
			this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
			// 
			// lblFolder
			// 
			this.lblFolder.AutoSize = true;
			this.lblFolder.Location = new System.Drawing.Point(6, 44);
			this.lblFolder.Name = "lblFolder";
			this.lblFolder.Size = new System.Drawing.Size(39, 13);
			this.lblFolder.TabIndex = 54;
			this.lblFolder.Text = "Folder:";
			// 
			// cmbFolder
			// 
			this.cmbFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.cmbFolder.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.cmbFolder.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
			this.cmbFolder.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.cmbFolder.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbFolder.ButtonIcon")));
			this.cmbFolder.DrawDropdownHoverOutline = false;
			this.cmbFolder.DrawFocusRectangle = false;
			this.cmbFolder.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.cmbFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmbFolder.ForeColor = System.Drawing.Color.Gainsboro;
			this.cmbFolder.FormattingEnabled = true;
			this.cmbFolder.Location = new System.Drawing.Point(60, 41);
			this.cmbFolder.Name = "cmbFolder";
			this.cmbFolder.Size = new System.Drawing.Size(131, 21);
			this.cmbFolder.TabIndex = 53;
			this.cmbFolder.Text = null;
			this.cmbFolder.TextPadding = new System.Windows.Forms.Padding(2);
			this.cmbFolder.SelectedIndexChanged += new System.EventHandler(this.cmbFolder_SelectedIndexChanged);
			// 
			// cmbDefaultCurrency
			// 
			this.cmbDefaultCurrency.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
			this.cmbDefaultCurrency.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
			this.cmbDefaultCurrency.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
			this.cmbDefaultCurrency.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
			this.cmbDefaultCurrency.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbDefaultCurrency.ButtonIcon")));
			this.cmbDefaultCurrency.DrawDropdownHoverOutline = false;
			this.cmbDefaultCurrency.DrawFocusRectangle = false;
			this.cmbDefaultCurrency.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.cmbDefaultCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbDefaultCurrency.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmbDefaultCurrency.ForeColor = System.Drawing.Color.Gainsboro;
			this.cmbDefaultCurrency.FormattingEnabled = true;
			this.cmbDefaultCurrency.Location = new System.Drawing.Point(338, 17);
			this.cmbDefaultCurrency.Name = "cmbDefaultCurrency";
			this.cmbDefaultCurrency.Size = new System.Drawing.Size(179, 21);
			this.cmbDefaultCurrency.TabIndex = 8;
			this.cmbDefaultCurrency.Text = null;
			this.cmbDefaultCurrency.TextPadding = new System.Windows.Forms.Padding(2);
			this.cmbDefaultCurrency.SelectedIndexChanged += new System.EventHandler(this.cmbDefaultCurrency_SelectedIndexChanged);
			// 
			// lblDefaultCurrency
			// 
			this.lblDefaultCurrency.AutoSize = true;
			this.lblDefaultCurrency.Location = new System.Drawing.Point(247, 20);
			this.lblDefaultCurrency.Name = "lblDefaultCurrency";
			this.lblDefaultCurrency.Size = new System.Drawing.Size(92, 13);
			this.lblDefaultCurrency.TabIndex = 2;
			this.lblDefaultCurrency.Text = "Default Currency: ";
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
			this.txtName.Size = new System.Drawing.Size(155, 20);
			this.txtName.TabIndex = 0;
			this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(553, 501);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
			this.btnCancel.Size = new System.Drawing.Size(190, 27);
			this.btnCancel.TabIndex = 55;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(357, 501);
			this.btnSave.Name = "btnSave";
			this.btnSave.Padding = new System.Windows.Forms.Padding(5);
			this.btnSave.Size = new System.Drawing.Size(190, 27);
			this.btnSave.TabIndex = 54;
			this.btnSave.Text = "Save";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// frmHDV
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
			this.ClientSize = new System.Drawing.Size(751, 540);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.pnlContainer);
			this.Controls.Add(this.grpHDV);
			this.Controls.Add(this.toolStrip);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmHDV";
			this.Text = "frmHDV";
			this.Load += new System.EventHandler(this.frmHDV_Load);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.grpHDV.ResumeLayout(false);
			this.pnlContainer.ResumeLayout(false);
			this.grpItems.ResumeLayout(false);
			this.grpItems.PerformLayout();
			this.grpGeneral.ResumeLayout(false);
			this.grpGeneral.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private DarkUI.Controls.DarkToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton toolStripItemNew;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton toolStripItemDelete;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton btnChronological;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		public System.Windows.Forms.ToolStripButton toolStripItemCopy;
		public System.Windows.Forms.ToolStripButton toolStripItemPaste;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		public System.Windows.Forms.ToolStripButton toolStripItemUndo;
		private DarkUI.Controls.DarkGroupBox grpHDV;
		public System.Windows.Forms.TreeView lstHdv;
		private System.Windows.Forms.Panel pnlContainer;
		private DarkUI.Controls.DarkGroupBox grpGeneral;
		private DarkUI.Controls.DarkButton btnAddFolder;
		private System.Windows.Forms.Label lblFolder;
		private DarkUI.Controls.DarkComboBox cmbFolder;
		private DarkUI.Controls.DarkComboBox cmbDefaultCurrency;
		private System.Windows.Forms.Label lblDefaultCurrency;
		private System.Windows.Forms.Label lblName;
		private DarkUI.Controls.DarkTextBox txtName;
		private DarkUI.Controls.DarkButton btnCancel;
		private DarkUI.Controls.DarkButton btnSave;
		private DarkUI.Controls.DarkGroupBox grpItems;
		private DarkUI.Controls.DarkButton btnDelItem;
		private DarkUI.Controls.DarkButton btnAddItem;
		private DarkUI.Controls.DarkComboBox cmbAddItem;
		private System.Windows.Forms.Label lblAddItem;
		private System.Windows.Forms.ListBox lstItems;
		private System.Windows.Forms.Label label1;
		private DarkUI.Controls.DarkRadioButton rdoBlacklist;
		private DarkUI.Controls.DarkRadioButton rdoWhitelist;
	}
}