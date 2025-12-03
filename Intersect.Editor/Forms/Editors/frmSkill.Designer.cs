using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
{
    partial class FrmSkill
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
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSkill));
            grpSkills = new DarkGroupBox();
            btnClearSearch = new DarkButton();
            txtSearch = new DarkTextBox();
            lstGameObjects = new Controls.GameObjectList();
            pnlContainer = new Panel();
            grpGeneral = new DarkGroupBox();
            txtIcon = new DarkTextBox();
            lblIcon = new Label();
            txtDescription = new DarkTextBox();
            lblDescription = new Label();
            nudExperienceIncrease = new DarkNumericUpDown();
            lblExperienceIncrease = new Label();
            nudBaseExperience = new DarkNumericUpDown();
            lblBaseExperience = new Label();
            nudMaxLevel = new DarkNumericUpDown();
            lblMaxLevel = new Label();
            btnAddFolder = new DarkButton();
            lblFolder = new Label();
            cmbFolder = new DarkComboBox();
            lblName = new Label();
            txtName = new DarkTextBox();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            toolStrip = new DarkToolStrip();
            toolStripItemNew = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripItemDelete = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            btnAlphabetical = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            toolStripItemCopy = new ToolStripButton();
            toolStripItemPaste = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripItemUndo = new ToolStripButton();
            grpSkills.SuspendLayout();
            pnlContainer.SuspendLayout();
            grpGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudExperienceIncrease).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudBaseExperience).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxLevel).BeginInit();
            toolStrip.SuspendLayout();
            SuspendLayout();
            // 
            // grpSkills
            // 
            grpSkills.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpSkills.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpSkills.Controls.Add(btnClearSearch);
            grpSkills.Controls.Add(txtSearch);
            grpSkills.Controls.Add(lstGameObjects);
            grpSkills.ForeColor = System.Drawing.Color.Gainsboro;
            grpSkills.Location = new System.Drawing.Point(14, 45);
            grpSkills.Margin = new Padding(4, 3, 4, 3);
            grpSkills.Name = "grpSkills";
            grpSkills.Padding = new Padding(4, 3, 4, 3);
            grpSkills.Size = new Size(237, 635);
            grpSkills.TabIndex = 14;
            grpSkills.TabStop = false;
            grpSkills.Text = "Skills";
            // 
            // btnClearSearch
            // 
            btnClearSearch.Location = new System.Drawing.Point(209, 22);
            btnClearSearch.Margin = new Padding(4, 3, 4, 3);
            btnClearSearch.Name = "btnClearSearch";
            btnClearSearch.Padding = new Padding(6);
            btnClearSearch.Size = new Size(21, 23);
            btnClearSearch.TabIndex = 34;
            btnClearSearch.Text = "X";
            btnClearSearch.Click += btnClearSearch_Click;
            // 
            // txtSearch
            // 
            txtSearch.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtSearch.Location = new System.Drawing.Point(7, 22);
            txtSearch.Margin = new Padding(4, 3, 4, 3);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(194, 23);
            txtSearch.TabIndex = 33;
            txtSearch.Text = "Search...";
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // lstGameObjects
            // 
            lstGameObjects.AllowDrop = true;
            lstGameObjects.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            lstGameObjects.BorderStyle = BorderStyle.None;
            lstGameObjects.ForeColor = System.Drawing.Color.Gainsboro;
            lstGameObjects.HideSelection = false;
            lstGameObjects.ImageIndex = 0;
            lstGameObjects.LineColor = System.Drawing.Color.FromArgb(150, 150, 150);
            lstGameObjects.Location = new System.Drawing.Point(7, 52);
            lstGameObjects.Margin = new Padding(4, 3, 4, 3);
            lstGameObjects.Name = "lstGameObjects";
            lstGameObjects.SelectedImageIndex = 0;
            lstGameObjects.Size = new Size(223, 572);
            lstGameObjects.TabIndex = 32;
            // 
            // pnlContainer
            // 
            pnlContainer.AutoScroll = true;
            pnlContainer.Controls.Add(grpGeneral);
            pnlContainer.Location = new System.Drawing.Point(258, 45);
            pnlContainer.Margin = new Padding(4, 3, 4, 3);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(853, 635);
            pnlContainer.TabIndex = 18;
            pnlContainer.Visible = false;
            // 
            // grpGeneral
            // 
            grpGeneral.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            grpGeneral.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpGeneral.Controls.Add(txtIcon);
            grpGeneral.Controls.Add(lblIcon);
            grpGeneral.Controls.Add(txtDescription);
            grpGeneral.Controls.Add(lblDescription);
            grpGeneral.Controls.Add(nudExperienceIncrease);
            grpGeneral.Controls.Add(lblExperienceIncrease);
            grpGeneral.Controls.Add(nudBaseExperience);
            grpGeneral.Controls.Add(lblBaseExperience);
            grpGeneral.Controls.Add(nudMaxLevel);
            grpGeneral.Controls.Add(lblMaxLevel);
            grpGeneral.Controls.Add(btnAddFolder);
            grpGeneral.Controls.Add(lblFolder);
            grpGeneral.Controls.Add(cmbFolder);
            grpGeneral.Controls.Add(lblName);
            grpGeneral.Controls.Add(txtName);
            grpGeneral.ForeColor = System.Drawing.Color.Gainsboro;
            grpGeneral.Location = new System.Drawing.Point(0, 0);
            grpGeneral.Margin = new Padding(4, 3, 4, 3);
            grpGeneral.Name = "grpGeneral";
            grpGeneral.Padding = new Padding(4, 3, 4, 3);
            grpGeneral.Size = new Size(500, 400);
            grpGeneral.TabIndex = 15;
            grpGeneral.TabStop = false;
            grpGeneral.Text = "General";
            // 
            // txtIcon
            // 
            txtIcon.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtIcon.BorderStyle = BorderStyle.FixedSingle;
            txtIcon.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtIcon.Location = new System.Drawing.Point(108, 200);
            txtIcon.Margin = new Padding(4, 3, 4, 3);
            txtIcon.Name = "txtIcon";
            txtIcon.Size = new Size(380, 23);
            txtIcon.TabIndex = 15;
            txtIcon.TextChanged += txtIcon_TextChanged;
            // 
            // lblIcon
            // 
            lblIcon.AutoSize = true;
            lblIcon.Location = new System.Drawing.Point(7, 203);
            lblIcon.Margin = new Padding(4, 0, 4, 0);
            lblIcon.Name = "lblIcon";
            lblIcon.Size = new Size(33, 15);
            lblIcon.TabIndex = 14;
            lblIcon.Text = "Icon:";
            // 
            // txtDescription
            // 
            txtDescription.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtDescription.BorderStyle = BorderStyle.FixedSingle;
            txtDescription.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtDescription.Location = new System.Drawing.Point(108, 170);
            txtDescription.Margin = new Padding(4, 3, 4, 3);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(380, 23);
            txtDescription.TabIndex = 13;
            txtDescription.TextChanged += txtDescription_TextChanged;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new System.Drawing.Point(7, 173);
            lblDescription.Margin = new Padding(4, 0, 4, 0);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(70, 15);
            lblDescription.TabIndex = 12;
            lblDescription.Text = "Description:";
            // 
            // nudExperienceIncrease
            // 
            nudExperienceIncrease.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudExperienceIncrease.ForeColor = System.Drawing.Color.Gainsboro;
            nudExperienceIncrease.Location = new System.Drawing.Point(150, 140);
            nudExperienceIncrease.Margin = new Padding(4, 3, 4, 3);
            nudExperienceIncrease.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudExperienceIncrease.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudExperienceIncrease.Name = "nudExperienceIncrease";
            nudExperienceIncrease.Size = new Size(338, 23);
            nudExperienceIncrease.TabIndex = 11;
            nudExperienceIncrease.Value = new decimal(new int[] { 50, 0, 0, 0 });
            nudExperienceIncrease.ValueChanged += nudExperienceIncrease_ValueChanged;
            // 
            // lblExperienceIncrease
            // 
            lblExperienceIncrease.AutoSize = true;
            lblExperienceIncrease.Location = new System.Drawing.Point(7, 143);
            lblExperienceIncrease.Margin = new Padding(4, 0, 4, 0);
            lblExperienceIncrease.Name = "lblExperienceIncrease";
            lblExperienceIncrease.Size = new Size(120, 15);
            lblExperienceIncrease.TabIndex = 10;
            lblExperienceIncrease.Text = "Experience Increase:";
            // 
            // nudBaseExperience
            // 
            nudBaseExperience.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudBaseExperience.ForeColor = System.Drawing.Color.Gainsboro;
            nudBaseExperience.Location = new System.Drawing.Point(150, 110);
            nudBaseExperience.Margin = new Padding(4, 3, 4, 3);
            nudBaseExperience.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            nudBaseExperience.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudBaseExperience.Name = "nudBaseExperience";
            nudBaseExperience.Size = new Size(338, 23);
            nudBaseExperience.TabIndex = 9;
            nudBaseExperience.Value = new decimal(new int[] { 100, 0, 0, 0 });
            nudBaseExperience.ValueChanged += nudBaseExperience_ValueChanged;
            // 
            // lblBaseExperience
            // 
            lblBaseExperience.AutoSize = true;
            lblBaseExperience.Location = new System.Drawing.Point(7, 113);
            lblBaseExperience.Margin = new Padding(4, 0, 4, 0);
            lblBaseExperience.Name = "lblBaseExperience";
            lblBaseExperience.Size = new Size(98, 15);
            lblBaseExperience.TabIndex = 8;
            lblBaseExperience.Text = "Base Experience:";
            // 
            // nudMaxLevel
            // 
            nudMaxLevel.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudMaxLevel.ForeColor = System.Drawing.Color.Gainsboro;
            nudMaxLevel.Location = new System.Drawing.Point(108, 80);
            nudMaxLevel.Margin = new Padding(4, 3, 4, 3);
            nudMaxLevel.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            nudMaxLevel.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudMaxLevel.Name = "nudMaxLevel";
            nudMaxLevel.Size = new Size(380, 23);
            nudMaxLevel.TabIndex = 7;
            nudMaxLevel.Value = new decimal(new int[] { 99, 0, 0, 0 });
            nudMaxLevel.ValueChanged += nudMaxLevel_ValueChanged;
            // 
            // lblMaxLevel
            // 
            lblMaxLevel.AutoSize = true;
            lblMaxLevel.Location = new System.Drawing.Point(7, 83);
            lblMaxLevel.Margin = new Padding(4, 0, 4, 0);
            lblMaxLevel.Name = "lblMaxLevel";
            lblMaxLevel.Size = new Size(65, 15);
            lblMaxLevel.TabIndex = 6;
            lblMaxLevel.Text = "Max Level:";
            // 
            // btnAddFolder
            // 
            btnAddFolder.Location = new System.Drawing.Point(224, 52);
            btnAddFolder.Margin = new Padding(4, 3, 4, 3);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Padding = new Padding(6);
            btnAddFolder.Size = new Size(21, 24);
            btnAddFolder.TabIndex = 5;
            btnAddFolder.Text = "+";
            btnAddFolder.Click += btnAddFolder_Click;
            // 
            // lblFolder
            // 
            lblFolder.AutoSize = true;
            lblFolder.Location = new System.Drawing.Point(7, 55);
            lblFolder.Margin = new Padding(4, 0, 4, 0);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new Size(43, 15);
            lblFolder.TabIndex = 4;
            lblFolder.Text = "Folder:";
            // 
            // cmbFolder
            // 
            cmbFolder.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbFolder.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbFolder.BorderStyle = ButtonBorderStyle.Solid;
            cmbFolder.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbFolder.DrawDropdownHoverOutline = false;
            cmbFolder.DrawFocusRectangle = false;
            cmbFolder.DrawMode = DrawMode.OwnerDrawFixed;
            cmbFolder.DropDownStyle = ComboBoxStyle.DropDown;
            cmbFolder.FlatStyle = FlatStyle.Flat;
            cmbFolder.ForeColor = System.Drawing.Color.Gainsboro;
            cmbFolder.FormattingEnabled = true;
            cmbFolder.Location = new System.Drawing.Point(88, 52);
            cmbFolder.Margin = new Padding(4, 3, 4, 3);
            cmbFolder.Name = "cmbFolder";
            cmbFolder.Size = new Size(131, 24);
            cmbFolder.TabIndex = 3;
            cmbFolder.Text = null;
            cmbFolder.TextPadding = new Padding(2);
            cmbFolder.TextChanged += cmbFolder_TextChanged;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new System.Drawing.Point(7, 23);
            lblName.Margin = new Padding(4, 0, 4, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(42, 15);
            lblName.TabIndex = 2;
            lblName.Text = "Name:";
            // 
            // txtName
            // 
            txtName.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtName.Location = new System.Drawing.Point(88, 23);
            txtName.Margin = new Padding(4, 3, 4, 3);
            txtName.Name = "txtName";
            txtName.Size = new Size(400, 23);
            txtName.TabIndex = 1;
            txtName.TextChanged += txtName_TextChanged;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(887, 688);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(222, 31);
            btnCancel.TabIndex = 44;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(658, 688);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(222, 31);
            btnSave.TabIndex = 41;
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;
            // 
            // toolStrip
            // 
            toolStrip.AutoSize = false;
            toolStrip.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            toolStrip.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStrip.Items.AddRange(new ToolStripItem[] { toolStripItemNew, toolStripSeparator1, toolStripItemDelete, toolStripSeparator2, btnAlphabetical, toolStripSeparator4, toolStripItemCopy, toolStripItemPaste, toolStripSeparator3, toolStripItemUndo });
            toolStrip.Location = new System.Drawing.Point(0, 0);
            toolStrip.Name = "toolStrip";
            toolStrip.Padding = new Padding(6, 0, 1, 0);
            toolStrip.Size = new Size(1119, 29);
            toolStrip.TabIndex = 47;
            toolStrip.Text = "toolStrip1";
            // 
            // toolStripItemNew
            // 
            toolStripItemNew.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripItemNew.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripItemNew.Image = (Image)resources.GetObject("toolStripItemNew.Image");
            toolStripItemNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripItemNew.Name = "toolStripItemNew";
            toolStripItemNew.Size = new Size(23, 26);
            toolStripItemNew.Text = "New";
            toolStripItemNew.Click += toolStripItemNew_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripSeparator1.Margin = new Padding(0, 0, 2, 0);
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 29);
            // 
            // toolStripItemDelete
            // 
            toolStripItemDelete.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripItemDelete.Enabled = false;
            toolStripItemDelete.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripItemDelete.Image = (Image)resources.GetObject("toolStripItemDelete.Image");
            toolStripItemDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripItemDelete.Name = "toolStripItemDelete";
            toolStripItemDelete.Size = new Size(23, 26);
            toolStripItemDelete.Text = "Delete";
            toolStripItemDelete.Click += toolStripItemDelete_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripSeparator2.Margin = new Padding(0, 0, 2, 0);
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 29);
            // 
            // btnAlphabetical
            // 
            btnAlphabetical.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnAlphabetical.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            btnAlphabetical.Image = (Image)resources.GetObject("btnAlphabetical.Image");
            btnAlphabetical.ImageTransparentColor = System.Drawing.Color.Magenta;
            btnAlphabetical.Name = "btnAlphabetical";
            btnAlphabetical.Size = new Size(23, 26);
            btnAlphabetical.Text = "Order Alphabetically";
            btnAlphabetical.Click += btnAlphabetical_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripSeparator4.Margin = new Padding(0, 0, 2, 0);
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 29);
            // 
            // toolStripItemCopy
            // 
            toolStripItemCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripItemCopy.Enabled = false;
            toolStripItemCopy.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripItemCopy.Image = (Image)resources.GetObject("toolStripItemCopy.Image");
            toolStripItemCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripItemCopy.Name = "toolStripItemCopy";
            toolStripItemCopy.Size = new Size(23, 26);
            toolStripItemCopy.Text = "Copy";
            toolStripItemCopy.Click += toolStripItemCopy_Click;
            // 
            // toolStripItemPaste
            // 
            toolStripItemPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripItemPaste.Enabled = false;
            toolStripItemPaste.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripItemPaste.Image = (Image)resources.GetObject("toolStripItemPaste.Image");
            toolStripItemPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripItemPaste.Name = "toolStripItemPaste";
            toolStripItemPaste.Size = new Size(23, 26);
            toolStripItemPaste.Text = "Paste";
            toolStripItemPaste.Click += toolStripItemPaste_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripSeparator3.Margin = new Padding(0, 0, 2, 0);
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 29);
            // 
            // toolStripItemUndo
            // 
            toolStripItemUndo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripItemUndo.Enabled = false;
            toolStripItemUndo.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            toolStripItemUndo.Image = (Image)resources.GetObject("toolStripItemUndo.Image");
            toolStripItemUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            toolStripItemUndo.Name = "toolStripItemUndo";
            toolStripItemUndo.Size = new Size(23, 26);
            toolStripItemUndo.Text = "Undo";
            toolStripItemUndo.Click += toolStripItemUndo_Click;
            // 
            // FrmSkill
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            ClientSize = new Size(1119, 728);
            ControlBox = true;
            Controls.Add(toolStrip);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(grpSkills);
            Controls.Add(pnlContainer);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            Margin = new Padding(4, 3, 4, 3);
            MinimizeBox = false;
            MaximizeBox = false;
            Name = "FrmSkill";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Skill Editor";
            FormClosed += frmSkill_FormClosed;
            Load += frmSkill_Load;
            KeyDown += form_KeyDown;
            grpSkills.ResumeLayout(false);
            grpSkills.PerformLayout();
            pnlContainer.ResumeLayout(false);
            grpGeneral.ResumeLayout(false);
            grpGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudExperienceIncrease).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudBaseExperience).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMaxLevel).EndInit();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpSkills;
        private DarkGroupBox grpGeneral;
        private System.Windows.Forms.Label lblName;
        private DarkTextBox txtName;
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
        private DarkButton btnClearSearch;
        private DarkTextBox txtSearch;
        private Controls.GameObjectList lstGameObjects;
        private DarkButton btnAddFolder;
        private System.Windows.Forms.Label lblFolder;
        private DarkComboBox cmbFolder;
        private DarkNumericUpDown nudMaxLevel;
        private System.Windows.Forms.Label lblMaxLevel;
        private DarkNumericUpDown nudBaseExperience;
        private System.Windows.Forms.Label lblBaseExperience;
        private DarkNumericUpDown nudExperienceIncrease;
        private System.Windows.Forms.Label lblExperienceIncrease;
        private DarkTextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private DarkTextBox txtIcon;
        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.ToolStripButton btnAlphabetical;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}

