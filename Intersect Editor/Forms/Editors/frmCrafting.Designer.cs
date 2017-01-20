using DarkUI.Controls;

namespace Intersect_Editor.Forms.Editors
{
    partial class frmCrafting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCrafting));
            this.btnCancel = new DarkButton();
            this.btnSave = new DarkButton();
            this.groupBox1 = new DarkGroupBox();
            this.lstCrafts = new System.Windows.Forms.ListBox();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.groupBox2 = new DarkGroupBox();
            this.lstCompositions = new System.Windows.Forms.ListBox();
            this.btnNewComposition = new DarkButton();
            this.btnDeleteCraft = new DarkButton();
            this.grpCraft = new DarkGroupBox();
            this.lblItem = new System.Windows.Forms.Label();
            this.scrlItem = new System.Windows.Forms.HScrollBar();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new DarkTextBox();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.scrlSpeed = new System.Windows.Forms.HScrollBar();
            this.groupBox5 = new DarkGroupBox();
            this.btnRemove = new DarkButton();
            this.btnAdd = new DarkButton();
            this.lblIngredient = new System.Windows.Forms.Label();
            this.scrlIngredient = new System.Windows.Forms.HScrollBar();
            this.lstIngredients = new System.Windows.Forms.ListBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.scrlQuantity = new System.Windows.Forms.HScrollBar();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripItemNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripItemPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripItemUndo = new System.Windows.Forms.ToolStripButton();
            this.btnDupIngredient = new DarkButton();
            this.btnDupCraft = new DarkButton();
            this.groupBox1.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.grpCraft.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(544, 442);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(172, 27);
            this.btnCancel.TabIndex = 24;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(333, 442);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(169, 27);
            this.btnSave.TabIndex = 23;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstCrafts);
            this.groupBox1.Location = new System.Drawing.Point(12, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 398);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Benches";
            // 
            // lstCrafts
            // 
            this.lstCrafts.FormattingEnabled = true;
            this.lstCrafts.Location = new System.Drawing.Point(6, 19);
            this.lstCrafts.Name = "lstCrafts";
            this.lstCrafts.Size = new System.Drawing.Size(191, 368);
            this.lstCrafts.TabIndex = 1;
            this.lstCrafts.Click += new System.EventHandler(this.lstCrafts_Click);
            this.lstCrafts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemList_KeyDown);
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.groupBox2);
            this.pnlContainer.Controls.Add(this.grpCraft);
            this.pnlContainer.Controls.Add(this.groupBox5);
            this.pnlContainer.Location = new System.Drawing.Point(221, 36);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(495, 398);
            this.pnlContainer.TabIndex = 31;
            this.pnlContainer.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnDupCraft);
            this.groupBox2.Controls.Add(this.lstCompositions);
            this.groupBox2.Controls.Add(this.btnNewComposition);
            this.groupBox2.Controls.Add(this.btnDeleteCraft);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(203, 392);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Crafts";
            // 
            // lstCompositions
            // 
            this.lstCompositions.FormattingEnabled = true;
            this.lstCompositions.Location = new System.Drawing.Point(6, 19);
            this.lstCompositions.Name = "lstCompositions";
            this.lstCompositions.Size = new System.Drawing.Size(191, 264);
            this.lstCompositions.TabIndex = 1;
            this.lstCompositions.Click += new System.EventHandler(this.lstCompositions_Click);
            this.lstCompositions.SelectedIndexChanged += new System.EventHandler(this.lstCompositions_SelectedIndexChanged);
            // 
            // btnNewComposition
            // 
            this.btnNewComposition.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNewComposition.Location = new System.Drawing.Point(7, 290);
            this.btnNewComposition.Name = "btnNewComposition";
            this.btnNewComposition.Size = new System.Drawing.Size(190, 27);
            this.btnNewComposition.TabIndex = 20;
            this.btnNewComposition.Text = "New";
            this.btnNewComposition.Click += new System.EventHandler(this.btnNewComposition_Click);
            // 
            // btnDeleteCraft
            // 
            this.btnDeleteCraft.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDeleteCraft.Location = new System.Drawing.Point(7, 323);
            this.btnDeleteCraft.Name = "btnDeleteCraft";
            this.btnDeleteCraft.Size = new System.Drawing.Size(190, 27);
            this.btnDeleteCraft.TabIndex = 19;
            this.btnDeleteCraft.Text = "Delete";
            this.btnDeleteCraft.Click += new System.EventHandler(this.btnDeleteCraft_Click);
            // 
            // grpCraft
            // 
            this.grpCraft.Controls.Add(this.lblItem);
            this.grpCraft.Controls.Add(this.scrlItem);
            this.grpCraft.Controls.Add(this.lblName);
            this.grpCraft.Controls.Add(this.txtName);
            this.grpCraft.Controls.Add(this.lblSpeed);
            this.grpCraft.Controls.Add(this.scrlSpeed);
            this.grpCraft.Location = new System.Drawing.Point(212, 3);
            this.grpCraft.Name = "grpCraft";
            this.grpCraft.Size = new System.Drawing.Size(273, 113);
            this.grpCraft.TabIndex = 31;
            this.grpCraft.TabStop = false;
            this.grpCraft.Text = "General";
            this.grpCraft.Visible = false;
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(10, 42);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(59, 13);
            this.lblItem.TabIndex = 33;
            this.lblItem.Text = "Item: None";
            // 
            // scrlItem
            // 
            this.scrlItem.LargeChange = 1;
            this.scrlItem.Location = new System.Drawing.Point(16, 55);
            this.scrlItem.Maximum = 5000;
            this.scrlItem.Minimum = -1;
            this.scrlItem.Name = "scrlItem";
            this.scrlItem.Size = new System.Drawing.Size(247, 17);
            this.scrlItem.TabIndex = 32;
            this.scrlItem.Value = -1;
            this.scrlItem.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlItem_Scroll);
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
            this.txtName.Size = new System.Drawing.Size(206, 20);
            this.txtName.TabIndex = 18;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(10, 72);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(55, 13);
            this.lblSpeed.TabIndex = 3;
            this.lblSpeed.Text = "Time: 1ms";
            // 
            // scrlSpeed
            // 
            this.scrlSpeed.Location = new System.Drawing.Point(15, 85);
            this.scrlSpeed.Maximum = 5000;
            this.scrlSpeed.Minimum = 1;
            this.scrlSpeed.Name = "scrlSpeed";
            this.scrlSpeed.Size = new System.Drawing.Size(248, 17);
            this.scrlSpeed.TabIndex = 0;
            this.scrlSpeed.Value = 1;
            this.scrlSpeed.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlSpeed_Scroll);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnDupIngredient);
            this.groupBox5.Controls.Add(this.btnRemove);
            this.groupBox5.Controls.Add(this.btnAdd);
            this.groupBox5.Controls.Add(this.lblIngredient);
            this.groupBox5.Controls.Add(this.scrlIngredient);
            this.groupBox5.Controls.Add(this.lstIngredients);
            this.groupBox5.Controls.Add(this.lblQuantity);
            this.groupBox5.Controls.Add(this.scrlQuantity);
            this.groupBox5.Location = new System.Drawing.Point(212, 122);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(273, 242);
            this.groupBox5.TabIndex = 30;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Ingredients";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(97, 208);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 38;
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(6, 208);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 37;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lblIngredient
            // 
            this.lblIngredient.AutoSize = true;
            this.lblIngredient.Location = new System.Drawing.Point(9, 141);
            this.lblIngredient.Name = "lblIngredient";
            this.lblIngredient.Size = new System.Drawing.Size(59, 13);
            this.lblIngredient.TabIndex = 31;
            this.lblIngredient.Text = "Item: None";
            // 
            // scrlIngredient
            // 
            this.scrlIngredient.LargeChange = 1;
            this.scrlIngredient.Location = new System.Drawing.Point(12, 154);
            this.scrlIngredient.Maximum = 5000;
            this.scrlIngredient.Minimum = -1;
            this.scrlIngredient.Name = "scrlIngredient";
            this.scrlIngredient.Size = new System.Drawing.Size(251, 17);
            this.scrlIngredient.TabIndex = 30;
            this.scrlIngredient.Value = -1;
            this.scrlIngredient.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlIngredient_Scroll);
            // 
            // lstIngredients
            // 
            this.lstIngredients.FormattingEnabled = true;
            this.lstIngredients.Items.AddRange(new object[] {
            "Ingredient: None x1"});
            this.lstIngredients.Location = new System.Drawing.Point(12, 17);
            this.lstIngredients.Name = "lstIngredients";
            this.lstIngredients.Size = new System.Drawing.Size(251, 121);
            this.lstIngredients.TabIndex = 29;
            this.lstIngredients.Click += new System.EventHandler(this.lstIngredients_Click);
            this.lstIngredients.SelectedIndexChanged += new System.EventHandler(this.lstIngredients_SelectedIndexChanged);
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(10, 171);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(58, 13);
            this.lblQuantity.TabIndex = 28;
            this.lblQuantity.Text = "Quantity: 1";
            // 
            // scrlQuantity
            // 
            this.scrlQuantity.LargeChange = 1;
            this.scrlQuantity.Location = new System.Drawing.Point(12, 184);
            this.scrlQuantity.Minimum = 1;
            this.scrlQuantity.Name = "scrlQuantity";
            this.scrlQuantity.Size = new System.Drawing.Size(251, 17);
            this.scrlQuantity.TabIndex = 27;
            this.scrlQuantity.Value = 1;
            this.scrlQuantity.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlQuantity_Scroll);
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
            this.toolStrip.Size = new System.Drawing.Size(725, 25);
            this.toolStrip.TabIndex = 43;
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
            // btnDupIngredient
            // 
            this.btnDupIngredient.Location = new System.Drawing.Point(188, 208);
            this.btnDupIngredient.Name = "btnDupIngredient";
            this.btnDupIngredient.Size = new System.Drawing.Size(75, 23);
            this.btnDupIngredient.TabIndex = 39;
            this.btnDupIngredient.Text = "Duplicate";
            this.btnDupIngredient.Click += new System.EventHandler(this.btnDupIngredient_Click);
            // 
            // btnDupCraft
            // 
            this.btnDupCraft.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDupCraft.Location = new System.Drawing.Point(6, 356);
            this.btnDupCraft.Name = "btnDupCraft";
            this.btnDupCraft.Size = new System.Drawing.Size(190, 27);
            this.btnDupCraft.TabIndex = 21;
            this.btnDupCraft.Text = "Duplicate";
            this.btnDupCraft.Click += new System.EventHandler(this.btnDupCraft_Click);
            // 
            // frmCrafting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 474);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.pnlContainer);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCrafting";
            this.Text = "Crafting Bench Editor";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.pnlContainer.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.grpCraft.ResumeLayout(false);
            this.grpCraft.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkGroupBox groupBox1;
        private System.Windows.Forms.ListBox lstCrafts;
        private System.Windows.Forms.Panel pnlContainer;
        private DarkGroupBox grpCraft;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.HScrollBar scrlItem;
        private System.Windows.Forms.Label lblName;
        private DarkTextBox txtName;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.HScrollBar scrlSpeed;
        private DarkGroupBox groupBox5;
        private DarkButton btnRemove;
        private DarkButton btnAdd;
        private System.Windows.Forms.Label lblIngredient;
        private System.Windows.Forms.HScrollBar scrlIngredient;
        private System.Windows.Forms.ListBox lstIngredients;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.HScrollBar scrlQuantity;
        private DarkGroupBox groupBox2;
        private System.Windows.Forms.ListBox lstCompositions;
        private DarkButton btnNewComposition;
        private DarkButton btnDeleteCraft;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripItemNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripItemDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton toolStripItemCopy;
        public System.Windows.Forms.ToolStripButton toolStripItemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.ToolStripButton toolStripItemUndo;
        private DarkButton btnDupCraft;
        private DarkButton btnDupIngredient;
    }
}