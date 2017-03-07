using DarkUI.Controls;

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
            this.grpShops = new DarkUI.Controls.DarkGroupBox();
            this.lstShops = new System.Windows.Forms.ListBox();
            this.grpGeneral = new DarkUI.Controls.DarkGroupBox();
            this.cmbDefaultCurrency = new DarkUI.Controls.DarkComboBox();
            this.lblDefaultCurrency = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new DarkUI.Controls.DarkTextBox();
            this.grpItemsSold = new DarkUI.Controls.DarkGroupBox();
            this.nudSellCost = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbSellFor = new DarkUI.Controls.DarkComboBox();
            this.lblSellFor = new System.Windows.Forms.Label();
            this.lblSellCost = new System.Windows.Forms.Label();
            this.btnDelSoldItem = new DarkUI.Controls.DarkButton();
            this.btnAddSoldItem = new DarkUI.Controls.DarkButton();
            this.cmbAddSoldItem = new DarkUI.Controls.DarkComboBox();
            this.lblAddSoldItem = new System.Windows.Forms.Label();
            this.lstSoldItems = new System.Windows.Forms.ListBox();
            this.grpItemsBought = new DarkUI.Controls.DarkGroupBox();
            this.nudBuyAmount = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbBuyFor = new DarkUI.Controls.DarkComboBox();
            this.lblBuyFor = new System.Windows.Forms.Label();
            this.lblBuyAmount = new System.Windows.Forms.Label();
            this.btnDelBoughtItem = new DarkUI.Controls.DarkButton();
            this.btnAddBoughtItem = new DarkUI.Controls.DarkButton();
            this.cmbAddBoughtItem = new DarkUI.Controls.DarkComboBox();
            this.lblItemBought = new System.Windows.Forms.Label();
            this.lstBoughtItems = new System.Windows.Forms.ListBox();
            this.rdoBuyBlacklist = new DarkUI.Controls.DarkRadioButton();
            this.rdoBuyWhitelist = new DarkUI.Controls.DarkRadioButton();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.toolStrip = new DarkUI.Controls.DarkToolStrip();
            this.toolStripItemNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripItemPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemUndo = new System.Windows.Forms.ToolStripButton();
            this.grpShops.SuspendLayout();
            this.grpGeneral.SuspendLayout();
            this.grpItemsSold.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSellCost)).BeginInit();
            this.grpItemsBought.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBuyAmount)).BeginInit();
            this.pnlContainer.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpShops
            // 
            this.grpShops.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpShops.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpShops.Controls.Add(this.lstShops);
            this.grpShops.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpShops.Location = new System.Drawing.Point(12, 34);
            this.grpShops.Name = "grpShops";
            this.grpShops.Size = new System.Drawing.Size(203, 467);
            this.grpShops.TabIndex = 15;
            this.grpShops.TabStop = false;
            this.grpShops.Text = "Shops";
            // 
            // lstShops
            // 
            this.lstShops.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstShops.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstShops.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstShops.FormattingEnabled = true;
            this.lstShops.Location = new System.Drawing.Point(6, 19);
            this.lstShops.Name = "lstShops";
            this.lstShops.Size = new System.Drawing.Size(191, 431);
            this.lstShops.TabIndex = 1;
            this.lstShops.Click += new System.EventHandler(this.lstShops_Click);
            this.lstShops.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemList_KeyDown);
            // 
            // grpGeneral
            // 
            this.grpGeneral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpGeneral.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpGeneral.Controls.Add(this.cmbDefaultCurrency);
            this.grpGeneral.Controls.Add(this.lblDefaultCurrency);
            this.grpGeneral.Controls.Add(this.lblName);
            this.grpGeneral.Controls.Add(this.txtName);
            this.grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpGeneral.Location = new System.Drawing.Point(-1, 2);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(523, 47);
            this.grpGeneral.TabIndex = 16;
            this.grpGeneral.TabStop = false;
            this.grpGeneral.Text = "General";
            // 
            // cmbDefaultCurrency
            // 
            this.cmbDefaultCurrency.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDefaultCurrency.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDefaultCurrency.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDefaultCurrency.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDefaultCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultCurrency.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDefaultCurrency.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbDefaultCurrency.FormattingEnabled = true;
            this.cmbDefaultCurrency.Location = new System.Drawing.Point(338, 17);
            this.cmbDefaultCurrency.Name = "cmbDefaultCurrency";
            this.cmbDefaultCurrency.Size = new System.Drawing.Size(179, 21);
            this.cmbDefaultCurrency.TabIndex = 8;
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
            this.lblName.Location = new System.Drawing.Point(6, 20);
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
            this.txtName.Location = new System.Drawing.Point(60, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(135, 20);
            this.txtName.TabIndex = 0;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // grpItemsSold
            // 
            this.grpItemsSold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpItemsSold.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpItemsSold.Controls.Add(this.nudSellCost);
            this.grpItemsSold.Controls.Add(this.cmbSellFor);
            this.grpItemsSold.Controls.Add(this.lblSellFor);
            this.grpItemsSold.Controls.Add(this.lblSellCost);
            this.grpItemsSold.Controls.Add(this.btnDelSoldItem);
            this.grpItemsSold.Controls.Add(this.btnAddSoldItem);
            this.grpItemsSold.Controls.Add(this.cmbAddSoldItem);
            this.grpItemsSold.Controls.Add(this.lblAddSoldItem);
            this.grpItemsSold.Controls.Add(this.lstSoldItems);
            this.grpItemsSold.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpItemsSold.Location = new System.Drawing.Point(0, 55);
            this.grpItemsSold.Name = "grpItemsSold";
            this.grpItemsSold.Size = new System.Drawing.Size(258, 414);
            this.grpItemsSold.TabIndex = 17;
            this.grpItemsSold.TabStop = false;
            this.grpItemsSold.Text = "Items Sold";
            // 
            // nudSellCost
            // 
            this.nudSellCost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudSellCost.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudSellCost.Location = new System.Drawing.Point(81, 351);
            this.nudSellCost.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudSellCost.Name = "nudSellCost";
            this.nudSellCost.Size = new System.Drawing.Size(171, 20);
            this.nudSellCost.TabIndex = 44;
            // 
            // cmbSellFor
            // 
            this.cmbSellFor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSellFor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSellFor.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSellFor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSellFor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSellFor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSellFor.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSellFor.FormattingEnabled = true;
            this.cmbSellFor.Location = new System.Drawing.Point(6, 322);
            this.cmbSellFor.Name = "cmbSellFor";
            this.cmbSellFor.Size = new System.Drawing.Size(246, 21);
            this.cmbSellFor.TabIndex = 11;
            // 
            // lblSellFor
            // 
            this.lblSellFor.AutoSize = true;
            this.lblSellFor.Location = new System.Drawing.Point(7, 306);
            this.lblSellFor.Name = "lblSellFor";
            this.lblSellFor.Size = new System.Drawing.Size(45, 13);
            this.lblSellFor.TabIndex = 10;
            this.lblSellFor.Text = "Sell For:";
            // 
            // lblSellCost
            // 
            this.lblSellCost.AutoSize = true;
            this.lblSellCost.Location = new System.Drawing.Point(7, 353);
            this.lblSellCost.Name = "lblSellCost";
            this.lblSellCost.Size = new System.Drawing.Size(51, 13);
            this.lblSellCost.TabIndex = 5;
            this.lblSellCost.Text = "Sell Cost:";
            // 
            // btnDelSoldItem
            // 
            this.btnDelSoldItem.Location = new System.Drawing.Point(135, 383);
            this.btnDelSoldItem.Name = "btnDelSoldItem";
            this.btnDelSoldItem.Padding = new System.Windows.Forms.Padding(5);
            this.btnDelSoldItem.Size = new System.Drawing.Size(117, 23);
            this.btnDelSoldItem.TabIndex = 4;
            this.btnDelSoldItem.Text = "Remove Selected";
            this.btnDelSoldItem.Click += new System.EventHandler(this.btnDelSoldItem_Click);
            // 
            // btnAddSoldItem
            // 
            this.btnAddSoldItem.Location = new System.Drawing.Point(6, 383);
            this.btnAddSoldItem.Name = "btnAddSoldItem";
            this.btnAddSoldItem.Padding = new System.Windows.Forms.Padding(5);
            this.btnAddSoldItem.Size = new System.Drawing.Size(117, 23);
            this.btnAddSoldItem.TabIndex = 3;
            this.btnAddSoldItem.Text = "Add Selected";
            this.btnAddSoldItem.Click += new System.EventHandler(this.btnAddSoldItem_Click);
            // 
            // cmbAddSoldItem
            // 
            this.cmbAddSoldItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAddSoldItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAddSoldItem.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAddSoldItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAddSoldItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddSoldItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAddSoldItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbAddSoldItem.FormattingEnabled = true;
            this.cmbAddSoldItem.Location = new System.Drawing.Point(6, 280);
            this.cmbAddSoldItem.Name = "cmbAddSoldItem";
            this.cmbAddSoldItem.Size = new System.Drawing.Size(246, 21);
            this.cmbAddSoldItem.TabIndex = 2;
            // 
            // lblAddSoldItem
            // 
            this.lblAddSoldItem.AutoSize = true;
            this.lblAddSoldItem.Location = new System.Drawing.Point(7, 264);
            this.lblAddSoldItem.Name = "lblAddSoldItem";
            this.lblAddSoldItem.Size = new System.Drawing.Size(108, 13);
            this.lblAddSoldItem.TabIndex = 1;
            this.lblAddSoldItem.Text = "Add Item To Be Sold:";
            // 
            // lstSoldItems
            // 
            this.lstSoldItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstSoldItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSoldItems.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstSoldItems.FormattingEnabled = true;
            this.lstSoldItems.Location = new System.Drawing.Point(7, 20);
            this.lstSoldItems.Name = "lstSoldItems";
            this.lstSoldItems.Size = new System.Drawing.Size(245, 236);
            this.lstSoldItems.TabIndex = 0;
            // 
            // grpItemsBought
            // 
            this.grpItemsBought.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpItemsBought.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpItemsBought.Controls.Add(this.nudBuyAmount);
            this.grpItemsBought.Controls.Add(this.cmbBuyFor);
            this.grpItemsBought.Controls.Add(this.lblBuyFor);
            this.grpItemsBought.Controls.Add(this.lblBuyAmount);
            this.grpItemsBought.Controls.Add(this.btnDelBoughtItem);
            this.grpItemsBought.Controls.Add(this.btnAddBoughtItem);
            this.grpItemsBought.Controls.Add(this.cmbAddBoughtItem);
            this.grpItemsBought.Controls.Add(this.lblItemBought);
            this.grpItemsBought.Controls.Add(this.lstBoughtItems);
            this.grpItemsBought.Controls.Add(this.rdoBuyBlacklist);
            this.grpItemsBought.Controls.Add(this.rdoBuyWhitelist);
            this.grpItemsBought.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpItemsBought.Location = new System.Drawing.Point(264, 55);
            this.grpItemsBought.Name = "grpItemsBought";
            this.grpItemsBought.Size = new System.Drawing.Size(258, 414);
            this.grpItemsBought.TabIndex = 18;
            this.grpItemsBought.TabStop = false;
            this.grpItemsBought.Text = "Items Bought (Whitelist - Buy Listed Items)";
            // 
            // nudBuyAmount
            // 
            this.nudBuyAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudBuyAmount.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudBuyAmount.Location = new System.Drawing.Point(82, 351);
            this.nudBuyAmount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudBuyAmount.Name = "nudBuyAmount";
            this.nudBuyAmount.Size = new System.Drawing.Size(171, 20);
            this.nudBuyAmount.TabIndex = 45;
            // 
            // cmbBuyFor
            // 
            this.cmbBuyFor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbBuyFor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbBuyFor.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbBuyFor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbBuyFor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBuyFor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBuyFor.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbBuyFor.FormattingEnabled = true;
            this.cmbBuyFor.Location = new System.Drawing.Point(8, 322);
            this.cmbBuyFor.Name = "cmbBuyFor";
            this.cmbBuyFor.Size = new System.Drawing.Size(246, 21);
            this.cmbBuyFor.TabIndex = 12;
            // 
            // lblBuyFor
            // 
            this.lblBuyFor.AutoSize = true;
            this.lblBuyFor.Location = new System.Drawing.Point(6, 306);
            this.lblBuyFor.Name = "lblBuyFor";
            this.lblBuyFor.Size = new System.Drawing.Size(46, 13);
            this.lblBuyFor.TabIndex = 11;
            this.lblBuyFor.Text = "Buy For:";
            // 
            // lblBuyAmount
            // 
            this.lblBuyAmount.AutoSize = true;
            this.lblBuyAmount.Location = new System.Drawing.Point(6, 353);
            this.lblBuyAmount.Name = "lblBuyAmount";
            this.lblBuyAmount.Size = new System.Drawing.Size(66, 13);
            this.lblBuyAmount.TabIndex = 6;
            this.lblBuyAmount.Text = "Sell Amount:";
            // 
            // btnDelBoughtItem
            // 
            this.btnDelBoughtItem.Location = new System.Drawing.Point(138, 383);
            this.btnDelBoughtItem.Name = "btnDelBoughtItem";
            this.btnDelBoughtItem.Padding = new System.Windows.Forms.Padding(5);
            this.btnDelBoughtItem.Size = new System.Drawing.Size(117, 23);
            this.btnDelBoughtItem.TabIndex = 6;
            this.btnDelBoughtItem.Text = "Remove Selected";
            this.btnDelBoughtItem.Click += new System.EventHandler(this.btnDelBoughtItem_Click);
            // 
            // btnAddBoughtItem
            // 
            this.btnAddBoughtItem.Location = new System.Drawing.Point(9, 383);
            this.btnAddBoughtItem.Name = "btnAddBoughtItem";
            this.btnAddBoughtItem.Padding = new System.Windows.Forms.Padding(5);
            this.btnAddBoughtItem.Size = new System.Drawing.Size(117, 23);
            this.btnAddBoughtItem.TabIndex = 5;
            this.btnAddBoughtItem.Text = "Add Selected";
            this.btnAddBoughtItem.Click += new System.EventHandler(this.btnAddBoughtItem_Click);
            // 
            // cmbAddBoughtItem
            // 
            this.cmbAddBoughtItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAddBoughtItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAddBoughtItem.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAddBoughtItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAddBoughtItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddBoughtItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAddBoughtItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbAddBoughtItem.FormattingEnabled = true;
            this.cmbAddBoughtItem.Location = new System.Drawing.Point(8, 280);
            this.cmbAddBoughtItem.Name = "cmbAddBoughtItem";
            this.cmbAddBoughtItem.Size = new System.Drawing.Size(246, 21);
            this.cmbAddBoughtItem.TabIndex = 3;
            // 
            // lblItemBought
            // 
            this.lblItemBought.AutoSize = true;
            this.lblItemBought.Location = new System.Drawing.Point(6, 266);
            this.lblItemBought.Name = "lblItemBought";
            this.lblItemBought.Size = new System.Drawing.Size(52, 13);
            this.lblItemBought.TabIndex = 2;
            this.lblItemBought.Text = "Add Item:";
            // 
            // lstBoughtItems
            // 
            this.lstBoughtItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstBoughtItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstBoughtItems.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstBoughtItems.FormattingEnabled = true;
            this.lstBoughtItems.Location = new System.Drawing.Point(8, 20);
            this.lstBoughtItems.Name = "lstBoughtItems";
            this.lstBoughtItems.Size = new System.Drawing.Size(245, 236);
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
            this.rdoBuyWhitelist.CheckedChanged += new System.EventHandler(this.rdoBuyWhitelist_CheckedChanged);
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.grpItemsBought);
            this.pnlContainer.Controls.Add(this.grpGeneral);
            this.pnlContainer.Controls.Add(this.grpItemsSold);
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
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(190, 27);
            this.btnCancel.TabIndex = 49;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(358, 509);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(190, 27);
            this.btnSave.TabIndex = 46;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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
            this.toolStrip.Size = new System.Drawing.Size(751, 25);
            this.toolStrip.TabIndex = 50;
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
            // frmShop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(751, 540);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpShops);
            this.Controls.Add(this.pnlContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmShop";
            this.Text = "Shop Editor";
            this.Load += new System.EventHandler(this.frmShop_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.grpShops.ResumeLayout(false);
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            this.grpItemsSold.ResumeLayout(false);
            this.grpItemsSold.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSellCost)).EndInit();
            this.grpItemsBought.ResumeLayout(false);
            this.grpItemsBought.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBuyAmount)).EndInit();
            this.pnlContainer.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpShops;
        private System.Windows.Forms.ListBox lstShops;
        private DarkGroupBox grpGeneral;
        private System.Windows.Forms.Label lblName;
        private DarkTextBox txtName;
        private DarkGroupBox grpItemsSold;
        private System.Windows.Forms.ListBox lstSoldItems;
        private DarkGroupBox grpItemsBought;
        private System.Windows.Forms.ListBox lstBoughtItems;
        private DarkRadioButton rdoBuyBlacklist;
        private DarkRadioButton rdoBuyWhitelist;
        private DarkButton btnDelSoldItem;
        private DarkButton btnAddSoldItem;
        private DarkComboBox cmbAddSoldItem;
        private System.Windows.Forms.Label lblAddSoldItem;
        private DarkButton btnDelBoughtItem;
        private DarkButton btnAddBoughtItem;
        private DarkComboBox cmbAddBoughtItem;
        private System.Windows.Forms.Label lblItemBought;
        private System.Windows.Forms.Label lblBuyAmount;
        private System.Windows.Forms.Label lblSellCost;
        private DarkComboBox cmbDefaultCurrency;
        private System.Windows.Forms.Label lblDefaultCurrency;
        private DarkComboBox cmbSellFor;
        private System.Windows.Forms.Label lblSellFor;
        private DarkComboBox cmbBuyFor;
        private System.Windows.Forms.Label lblBuyFor;
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
        private DarkNumericUpDown nudSellCost;
        private DarkNumericUpDown nudBuyAmount;
    }
}