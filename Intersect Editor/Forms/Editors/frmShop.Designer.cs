namespace Intersect_Editor.Forms.Editors
{
    partial class frmShop
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmShop));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstShops = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbDefaultCurrency = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbSellFor = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSellCost = new System.Windows.Forms.TextBox();
            this.lblChargeRate = new System.Windows.Forms.Label();
            this.btnDelSoldItem = new System.Windows.Forms.Button();
            this.btnAddSoldItem = new System.Windows.Forms.Button();
            this.cmbAddSoldItem = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lstSoldItems = new System.Windows.Forms.ListBox();
            this.grpItemsBought = new System.Windows.Forms.GroupBox();
            this.cmbBuyFor = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBuyAmount = new System.Windows.Forms.TextBox();
            this.lblBuyRate = new System.Windows.Forms.Label();
            this.btnDelBoughtItem = new System.Windows.Forms.Button();
            this.btnAddBoughtItem = new System.Windows.Forms.Button();
            this.cmbAddBoughtItem = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lstBoughtItems = new System.Windows.Forms.ListBox();
            this.rdoBuyBlacklist = new System.Windows.Forms.RadioButton();
            this.rdoBuyWhitelist = new System.Windows.Forms.RadioButton();
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
            this.groupBox3.SuspendLayout();
            this.grpItemsBought.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstShops);
            this.groupBox1.Location = new System.Drawing.Point(12, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 467);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Shops";
            // 
            // lstShops
            // 
            this.lstShops.FormattingEnabled = true;
            this.lstShops.Location = new System.Drawing.Point(6, 19);
            this.lstShops.Name = "lstShops";
            this.lstShops.Size = new System.Drawing.Size(191, 433);
            this.lstShops.TabIndex = 1;
            this.lstShops.Click += new System.EventHandler(this.lstShops_Click);
            this.lstShops.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemList_KeyDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbDefaultCurrency);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Location = new System.Drawing.Point(-1, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(523, 47);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // cmbDefaultCurrency
            // 
            this.cmbDefaultCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultCurrency.FormattingEnabled = true;
            this.cmbDefaultCurrency.Location = new System.Drawing.Point(338, 17);
            this.cmbDefaultCurrency.Name = "cmbDefaultCurrency";
            this.cmbDefaultCurrency.Size = new System.Drawing.Size(179, 21);
            this.cmbDefaultCurrency.TabIndex = 8;
            this.cmbDefaultCurrency.SelectedIndexChanged += new System.EventHandler(this.cmbDefaultCurrency_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(247, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Default Currency: ";
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
            this.groupBox3.Controls.Add(this.cmbSellFor);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtSellCost);
            this.groupBox3.Controls.Add(this.lblChargeRate);
            this.groupBox3.Controls.Add(this.btnDelSoldItem);
            this.groupBox3.Controls.Add(this.btnAddSoldItem);
            this.groupBox3.Controls.Add(this.cmbAddSoldItem);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.lstSoldItems);
            this.groupBox3.Location = new System.Drawing.Point(0, 55);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(258, 414);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Items Sold";
            // 
            // cmbSellFor
            // 
            this.cmbSellFor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSellFor.FormattingEnabled = true;
            this.cmbSellFor.Location = new System.Drawing.Point(6, 322);
            this.cmbSellFor.Name = "cmbSellFor";
            this.cmbSellFor.Size = new System.Drawing.Size(246, 21);
            this.cmbSellFor.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 306);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Sell For:";
            // 
            // txtSellCost
            // 
            this.txtSellCost.Location = new System.Drawing.Point(79, 350);
            this.txtSellCost.Name = "txtSellCost";
            this.txtSellCost.Size = new System.Drawing.Size(173, 20);
            this.txtSellCost.TabIndex = 9;
            // 
            // lblChargeRate
            // 
            this.lblChargeRate.AutoSize = true;
            this.lblChargeRate.Location = new System.Drawing.Point(7, 353);
            this.lblChargeRate.Name = "lblChargeRate";
            this.lblChargeRate.Size = new System.Drawing.Size(51, 13);
            this.lblChargeRate.TabIndex = 5;
            this.lblChargeRate.Text = "Sell Cost:";
            // 
            // btnDelSoldItem
            // 
            this.btnDelSoldItem.Location = new System.Drawing.Point(135, 383);
            this.btnDelSoldItem.Name = "btnDelSoldItem";
            this.btnDelSoldItem.Size = new System.Drawing.Size(117, 23);
            this.btnDelSoldItem.TabIndex = 4;
            this.btnDelSoldItem.Text = "Remove Selected";
            this.btnDelSoldItem.UseVisualStyleBackColor = true;
            this.btnDelSoldItem.Click += new System.EventHandler(this.btnDelSoldItem_Click);
            // 
            // btnAddSoldItem
            // 
            this.btnAddSoldItem.Location = new System.Drawing.Point(6, 383);
            this.btnAddSoldItem.Name = "btnAddSoldItem";
            this.btnAddSoldItem.Size = new System.Drawing.Size(117, 23);
            this.btnAddSoldItem.TabIndex = 3;
            this.btnAddSoldItem.Text = "Add Selected";
            this.btnAddSoldItem.UseVisualStyleBackColor = true;
            this.btnAddSoldItem.Click += new System.EventHandler(this.btnAddSoldItem_Click);
            // 
            // cmbAddSoldItem
            // 
            this.cmbAddSoldItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddSoldItem.FormattingEnabled = true;
            this.cmbAddSoldItem.Location = new System.Drawing.Point(6, 280);
            this.cmbAddSoldItem.Name = "cmbAddSoldItem";
            this.cmbAddSoldItem.Size = new System.Drawing.Size(246, 21);
            this.cmbAddSoldItem.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 264);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Add Item To Be Sold:";
            // 
            // lstSoldItems
            // 
            this.lstSoldItems.FormattingEnabled = true;
            this.lstSoldItems.Location = new System.Drawing.Point(7, 20);
            this.lstSoldItems.Name = "lstSoldItems";
            this.lstSoldItems.Size = new System.Drawing.Size(245, 238);
            this.lstSoldItems.TabIndex = 0;
            // 
            // grpItemsBought
            // 
            this.grpItemsBought.Controls.Add(this.cmbBuyFor);
            this.grpItemsBought.Controls.Add(this.label6);
            this.grpItemsBought.Controls.Add(this.txtBuyAmount);
            this.grpItemsBought.Controls.Add(this.lblBuyRate);
            this.grpItemsBought.Controls.Add(this.btnDelBoughtItem);
            this.grpItemsBought.Controls.Add(this.btnAddBoughtItem);
            this.grpItemsBought.Controls.Add(this.cmbAddBoughtItem);
            this.grpItemsBought.Controls.Add(this.label3);
            this.grpItemsBought.Controls.Add(this.lstBoughtItems);
            this.grpItemsBought.Controls.Add(this.rdoBuyBlacklist);
            this.grpItemsBought.Controls.Add(this.rdoBuyWhitelist);
            this.grpItemsBought.Location = new System.Drawing.Point(264, 55);
            this.grpItemsBought.Name = "grpItemsBought";
            this.grpItemsBought.Size = new System.Drawing.Size(258, 414);
            this.grpItemsBought.TabIndex = 18;
            this.grpItemsBought.TabStop = false;
            this.grpItemsBought.Text = "Items Bought (Whitelist - Buy Listed Items)";
            // 
            // cmbBuyFor
            // 
            this.cmbBuyFor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuyFor.FormattingEnabled = true;
            this.cmbBuyFor.Location = new System.Drawing.Point(8, 322);
            this.cmbBuyFor.Name = "cmbBuyFor";
            this.cmbBuyFor.Size = new System.Drawing.Size(246, 21);
            this.cmbBuyFor.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 306);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Buy For:";
            // 
            // txtBuyAmount
            // 
            this.txtBuyAmount.Location = new System.Drawing.Point(78, 350);
            this.txtBuyAmount.Name = "txtBuyAmount";
            this.txtBuyAmount.Size = new System.Drawing.Size(173, 20);
            this.txtBuyAmount.TabIndex = 10;
            // 
            // lblBuyRate
            // 
            this.lblBuyRate.AutoSize = true;
            this.lblBuyRate.Location = new System.Drawing.Point(6, 353);
            this.lblBuyRate.Name = "lblBuyRate";
            this.lblBuyRate.Size = new System.Drawing.Size(66, 13);
            this.lblBuyRate.TabIndex = 6;
            this.lblBuyRate.Text = "Sell Amount:";
            // 
            // btnDelBoughtItem
            // 
            this.btnDelBoughtItem.Location = new System.Drawing.Point(138, 383);
            this.btnDelBoughtItem.Name = "btnDelBoughtItem";
            this.btnDelBoughtItem.Size = new System.Drawing.Size(117, 23);
            this.btnDelBoughtItem.TabIndex = 6;
            this.btnDelBoughtItem.Text = "Remove Selected";
            this.btnDelBoughtItem.UseVisualStyleBackColor = true;
            this.btnDelBoughtItem.Click += new System.EventHandler(this.btnDelBoughtItem_Click);
            // 
            // btnAddBoughtItem
            // 
            this.btnAddBoughtItem.Location = new System.Drawing.Point(9, 383);
            this.btnAddBoughtItem.Name = "btnAddBoughtItem";
            this.btnAddBoughtItem.Size = new System.Drawing.Size(117, 23);
            this.btnAddBoughtItem.TabIndex = 5;
            this.btnAddBoughtItem.Text = "Add Selected";
            this.btnAddBoughtItem.UseVisualStyleBackColor = true;
            this.btnAddBoughtItem.Click += new System.EventHandler(this.btnAddBoughtItem_Click);
            // 
            // cmbAddBoughtItem
            // 
            this.cmbAddBoughtItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddBoughtItem.FormattingEnabled = true;
            this.cmbAddBoughtItem.Location = new System.Drawing.Point(8, 280);
            this.cmbAddBoughtItem.Name = "cmbAddBoughtItem";
            this.cmbAddBoughtItem.Size = new System.Drawing.Size(246, 21);
            this.cmbAddBoughtItem.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 266);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Add Item:";
            // 
            // lstBoughtItems
            // 
            this.lstBoughtItems.FormattingEnabled = true;
            this.lstBoughtItems.Location = new System.Drawing.Point(8, 20);
            this.lstBoughtItems.Name = "lstBoughtItems";
            this.lstBoughtItems.Size = new System.Drawing.Size(245, 238);
            this.lstBoughtItems.TabIndex = 2;
            // 
            // rdoBuyBlacklist
            // 
            this.rdoBuyBlacklist.AutoSize = true;
            this.rdoBuyBlacklist.Location = new System.Drawing.Point(189, 262);
            this.rdoBuyBlacklist.Name = "rdoBuyBlacklist";
            this.rdoBuyBlacklist.Size = new System.Drawing.Size(64, 17);
            this.rdoBuyBlacklist.TabIndex = 1;
            this.rdoBuyBlacklist.Text = "Blacklist";
            this.rdoBuyBlacklist.UseVisualStyleBackColor = true;
            this.rdoBuyBlacklist.CheckedChanged += new System.EventHandler(this.rdoBuyBlacklist_CheckedChanged);
            // 
            // rdoBuyWhitelist
            // 
            this.rdoBuyWhitelist.AutoSize = true;
            this.rdoBuyWhitelist.Checked = true;
            this.rdoBuyWhitelist.Location = new System.Drawing.Point(118, 262);
            this.rdoBuyWhitelist.Name = "rdoBuyWhitelist";
            this.rdoBuyWhitelist.Size = new System.Drawing.Size(65, 17);
            this.rdoBuyWhitelist.TabIndex = 0;
            this.rdoBuyWhitelist.TabStop = true;
            this.rdoBuyWhitelist.Text = "Whitelist";
            this.rdoBuyWhitelist.UseVisualStyleBackColor = true;
            this.rdoBuyWhitelist.CheckedChanged += new System.EventHandler(this.rdoBuyWhitelist_CheckedChanged);
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.grpItemsBought);
            this.pnlContainer.Controls.Add(this.groupBox2);
            this.pnlContainer.Controls.Add(this.groupBox3);
            this.pnlContainer.Location = new System.Drawing.Point(222, 34);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(522, 467);
            this.pnlContainer.TabIndex = 19;
            this.pnlContainer.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(554, 509);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 49;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(358, 509);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 46;
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
            this.toolStrip.Size = new System.Drawing.Size(751, 25);
            this.toolStrip.TabIndex = 50;
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
            // frmShop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(751, 540);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmShop";
            this.Text = "Shop Editor";
            this.Load += new System.EventHandler(this.frmShop_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.grpItemsBought.ResumeLayout(false);
            this.grpItemsBought.PerformLayout();
            this.pnlContainer.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstShops;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lstSoldItems;
        private System.Windows.Forms.GroupBox grpItemsBought;
        private System.Windows.Forms.ListBox lstBoughtItems;
        private System.Windows.Forms.RadioButton rdoBuyBlacklist;
        private System.Windows.Forms.RadioButton rdoBuyWhitelist;
        private System.Windows.Forms.Button btnDelSoldItem;
        private System.Windows.Forms.Button btnAddSoldItem;
        private System.Windows.Forms.ComboBox cmbAddSoldItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDelBoughtItem;
        private System.Windows.Forms.Button btnAddBoughtItem;
        private System.Windows.Forms.ComboBox cmbAddBoughtItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblBuyRate;
        private System.Windows.Forms.Label lblChargeRate;
        private System.Windows.Forms.ComboBox cmbDefaultCurrency;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbSellFor;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSellCost;
        private System.Windows.Forms.ComboBox cmbBuyFor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBuyAmount;
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
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