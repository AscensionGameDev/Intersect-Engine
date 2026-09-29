using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
{
    partial class FrmTerritory
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

        private static System.Drawing.Color PanelColor => System.Drawing.Color.FromArgb(45, 45, 48);
        private static System.Drawing.Color BorderColor => System.Drawing.Color.FromArgb(90, 90, 90);
        private static System.Drawing.Color InputColor => System.Drawing.Color.FromArgb(69, 73, 74);
        private static System.Drawing.Color TextColor => System.Drawing.Color.FromArgb(220, 220, 220);

        private DarkGroupBox MakeGroup(string text, int x, int y, int w, int h)
        {
            var group = new DarkGroupBox
            {
                BackColor = PanelColor,
                BorderColor = BorderColor,
                ForeColor = System.Drawing.Color.Gainsboro,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(w, h),
                TabStop = false,
                Text = text,
            };
            return group;
        }

        private System.Windows.Forms.Label MakeLabel(string text, int x, int y)
        {
            return new System.Windows.Forms.Label { AutoSize = true, Location = new System.Drawing.Point(x, y), Text = text };
        }

        private DarkComboBox MakeCombo(int x, int y, int w)
        {
            return new DarkComboBox
            {
                BackColor = InputColor,
                BorderColor = BorderColor,
                BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid,
                ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43),
                DrawDropdownHoverOutline = false,
                DrawFocusRectangle = false,
                DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed,
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                ForeColor = System.Drawing.Color.Gainsboro,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(w, 21),
                TextPadding = new System.Windows.Forms.Padding(2),
            };
        }

        private DarkNumericUpDown MakeNumeric(int x, int y, int w, int min, int max, int value)
        {
            var numeric = new DarkNumericUpDown
            {
                BackColor = InputColor,
                ForeColor = System.Drawing.Color.Gainsboro,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(w, 20),
                Minimum = min,
                Maximum = max,
            };
            numeric.Value = value;
            return numeric;
        }

        private DarkTextBox MakeTextBox(int x, int y, int w, int h = 20, bool multiline = false)
        {
            return new DarkTextBox
            {
                BackColor = InputColor,
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                ForeColor = TextColor,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(w, h),
                Multiline = multiline,
                ScrollBars = multiline ? System.Windows.Forms.ScrollBars.Vertical : System.Windows.Forms.ScrollBars.None,
            };
        }

        private DarkButton MakeButton(string text, int x, int y, int w, int h = 23)
        {
            return new DarkButton
            {
                Location = new System.Drawing.Point(x, y),
                Padding = new System.Windows.Forms.Padding(5),
                Size = new System.Drawing.Size(w, h),
                Text = text,
            };
        }

        private System.Windows.Forms.ToolStripButton MakeToolButton(string name, string text, System.ComponentModel.ComponentResourceManager resources, bool enabled = true)
        {
            return new System.Windows.Forms.ToolStripButton
            {
                DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image,
                Enabled = enabled,
                ForeColor = TextColor,
                Image = ((System.Drawing.Image)(resources.GetObject(name + ".Image"))),
                ImageTransparentColor = System.Drawing.Color.Magenta,
                Name = name,
                Size = new System.Drawing.Size(23, 22),
                Text = text,
            };
        }

        private static System.Windows.Forms.ToolStripSeparator MakeSeparator()
        {
            return new System.Windows.Forms.ToolStripSeparator
            {
                ForeColor = System.Drawing.Color.FromArgb(220, 220, 220),
                Margin = new System.Windows.Forms.Padding(0, 0, 2, 0),
                Size = new System.Drawing.Size(6, 25),
            };
        }

        /// <summary>
        /// Builds the form. Written by hand (no WinForms designer) using the same DarkUI controls as the other editors.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTerritory));
            this.SuspendLayout();

            // ---- Tool strip
            this.toolStrip = new DarkToolStrip
            {
                AutoSize = false,
                BackColor = PanelColor,
                ForeColor = TextColor,
                Location = new System.Drawing.Point(0, 0),
                Name = "toolStrip",
                Padding = new System.Windows.Forms.Padding(5, 0, 1, 0),
                Size = new System.Drawing.Size(760, 25),
            };
            this.toolStripItemNew = MakeToolButton("toolStripItemNew", "New", resources);
            this.toolStripItemNew.Click += new System.EventHandler(this.toolStripItemNew_Click);
            this.toolStripItemDelete = MakeToolButton("toolStripItemDelete", "Delete", resources, false);
            this.toolStripItemDelete.Click += new System.EventHandler(this.toolStripItemDelete_Click);
            this.btnAlphabetical = MakeToolButton("btnAlphabetical", "Order Chronologically", resources);
            this.btnAlphabetical.Click += new System.EventHandler(this.btnAlphabetical_Click);
            this.toolStripItemCopy = MakeToolButton("toolStripItemCopy", "Copy", resources, false);
            this.toolStripItemCopy.Click += new System.EventHandler(this.toolStripItemCopy_Click);
            this.toolStripItemPaste = MakeToolButton("toolStripItemPaste", "Paste", resources, false);
            this.toolStripItemPaste.Click += new System.EventHandler(this.toolStripItemPaste_Click);
            this.toolStripItemUndo = MakeToolButton("toolStripItemUndo", "Undo", resources, false);
            this.toolStripItemUndo.Click += new System.EventHandler(this.toolStripItemUndo_Click);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.toolStripItemNew, MakeSeparator(), this.toolStripItemDelete, MakeSeparator(), this.btnAlphabetical,
                MakeSeparator(), this.toolStripItemCopy, this.toolStripItemPaste, MakeSeparator(), this.toolStripItemUndo });

            // ---- Left: list
            this.grpTerritories = MakeGroup("Dungeons", 12, 36, 203, 560);
            this.txtSearch = MakeTextBox(6, 16, 167);
            this.txtSearch.Text = "Search...";
            this.txtSearch.Click += new System.EventHandler(this.txtSearch_Click);
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.Enter += new System.EventHandler(this.txtSearch_Enter);
            this.txtSearch.Leave += new System.EventHandler(this.txtSearch_Leave);
            this.btnClearSearch = MakeButton("X", 179, 16, 18, 20);
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            this.lstGameObjects = new Intersect.Editor.Forms.Controls.GameObjectList
            {
                AllowDrop = true,
                BackColor = System.Drawing.Color.FromArgb(60, 63, 65),
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                ForeColor = System.Drawing.Color.Gainsboro,
                HideSelection = false,
                ImageIndex = 0,
                LineColor = System.Drawing.Color.FromArgb(150, 150, 150),
                Location = new System.Drawing.Point(6, 42),
                Name = "lstGameObjects",
                SelectedImageIndex = 0,
                Size = new System.Drawing.Size(191, 512),
            };
            this.grpTerritories.Controls.Add(this.btnClearSearch);
            this.grpTerritories.Controls.Add(this.txtSearch);
            this.grpTerritories.Controls.Add(this.lstGameObjects);

            // ---- Right: container
            this.pnlContainer = new System.Windows.Forms.Panel
            {
                Location = new System.Drawing.Point(221, 36),
                Size = new System.Drawing.Size(527, 560),
                Visible = false,
            };

            // General
            this.grpGeneral = MakeGroup("General", 7, 3, 513, 130);
            this.lblName = MakeLabel("Name:", 6, 22);
            this.txtName = MakeTextBox(70, 19, 180);
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            this.lblFolder = MakeLabel("Folder:", 262, 22);
            this.cmbFolder = MakeCombo(310, 19, 170);
            this.cmbFolder.SelectedIndexChanged += new System.EventHandler(this.cmbFolder_SelectedIndexChanged);
            this.btnAddFolder = MakeButton("+", 486, 19, 21, 21);
            this.btnAddFolder.Click += new System.EventHandler(this.btnAddFolder_Click);
            this.lblDescription = MakeLabel("Description:", 6, 48);
            this.txtDescription = MakeTextBox(70, 45, 437, 76, true);
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            this.grpGeneral.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblName, this.txtName, this.lblFolder, this.cmbFolder, this.btnAddFolder, this.lblDescription, this.txtDescription });

            // Capture Point
            this.grpEntry = MakeGroup("Capture Point", 7, 139, 513, 78);
            this.lblEntryMap = MakeLabel("Map:", 6, 22);
            this.cmbEntryMap = MakeCombo(70, 19, 200);
            this.cmbEntryMap.SelectedIndexChanged += new System.EventHandler(this.cmbEntryMap_SelectedIndexChanged);
            this.lblEntryX = MakeLabel("X:", 280, 22);
            this.nudEntryX = MakeNumeric(300, 20, 50, 0, 255, 0);
            this.nudEntryX.ValueChanged += new System.EventHandler(this.nudEntry_ValueChanged);
            this.lblEntryY = MakeLabel("Y:", 358, 22);
            this.nudEntryY = MakeNumeric(378, 20, 50, 0, 255, 0);
            this.nudEntryY.ValueChanged += new System.EventHandler(this.nudEntry_ValueChanged);
            this.btnVisual = MakeButton("Open Visual Interface", 70, 47, 300);
            this.btnVisual.Click += new System.EventHandler(this.btnVisual_Click);
            this.grpEntry.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblEntryMap, this.cmbEntryMap, this.lblEntryX, this.nudEntryX, this.lblEntryY, this.nudEntryY, this.btnVisual });

            // Capture Settings
            this.grpRules = MakeGroup("Capture Settings", 7, 223, 513, 104);
            this.lblRadius = MakeLabel("Radius (tiles):", 6, 22);
            this.nudRadius = MakeNumeric(110, 20, 60, 1, 100, 10);
            this.nudRadius.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.lblCaptureTime = MakeLabel("Capture time (s):", 262, 22);
            this.nudCaptureTime = MakeNumeric(390, 20, 60, 5, 86400, 120);
            this.nudCaptureTime.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.lblRewardExp = MakeLabel("Tick experience:", 6, 48);
            this.nudRewardExp = MakeNumeric(110, 46, 60, 0, int.MaxValue, 0);
            this.nudRewardExp.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.lblRewardGold = MakeLabel("Tick gold:", 262, 48);
            this.nudRewardGold = MakeNumeric(390, 46, 60, 0, int.MaxValue, 0);
            this.nudRewardGold.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.lblRewardTick = MakeLabel("Tick every (s):", 6, 74);
            this.nudRewardTick = MakeNumeric(110, 72, 60, 5, 86400, 60);
            this.nudRewardTick.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.grpRules.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblRadius, this.nudRadius, this.lblCaptureTime, this.nudCaptureTime,
                this.lblRewardExp, this.nudRewardExp, this.lblRewardGold, this.nudRewardGold, this.lblRewardTick, this.nudRewardTick });

            this.pnlContainer.Controls.AddRange(new System.Windows.Forms.Control[] { this.grpGeneral, this.grpEntry, this.grpRules });

            // ---- Bottom buttons
            this.btnSave = MakeButton("Save", 380, 602, 169, 27);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnCancel = MakeButton("Cancel", 555, 602, 172, 27);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // ---- Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PanelColor;
            this.ClientSize = new System.Drawing.Size(760, 641);
            this.ControlBox = true;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.pnlContainer);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpTerritories);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmTerritory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dungeon Editor";
            FormClosed += FrmTerritory_FormClosed;
            this.Load += new System.EventHandler(this.frmDungeon_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.ResumeLayout(false);
        }

        #endregion

        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkGroupBox grpTerritories;
        private System.Windows.Forms.Panel pnlContainer;
        private DarkToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolStripItemNew;
        private System.Windows.Forms.ToolStripButton toolStripItemDelete;
        public System.Windows.Forms.ToolStripButton toolStripItemCopy;
        public System.Windows.Forms.ToolStripButton toolStripItemPaste;
        public System.Windows.Forms.ToolStripButton toolStripItemUndo;
        private System.Windows.Forms.ToolStripButton btnAlphabetical;
        private DarkTextBox txtSearch;
        private DarkButton btnClearSearch;
        private Controls.GameObjectList lstGameObjects;

        private DarkGroupBox grpGeneral;
        private System.Windows.Forms.Label lblName;
        private DarkTextBox txtName;
        private System.Windows.Forms.Label lblFolder;
        private DarkComboBox cmbFolder;
        private DarkButton btnAddFolder;
        private System.Windows.Forms.Label lblDescription;
        private DarkTextBox txtDescription;

        private DarkGroupBox grpEntry;
        private System.Windows.Forms.Label lblEntryMap;
        private DarkComboBox cmbEntryMap;
        private System.Windows.Forms.Label lblEntryX;
        private DarkNumericUpDown nudEntryX;
        private System.Windows.Forms.Label lblEntryY;
        private DarkNumericUpDown nudEntryY;
        private DarkButton btnVisual;

        private DarkGroupBox grpRules;
        private System.Windows.Forms.Label lblRadius;
        private DarkNumericUpDown nudRadius;
        private System.Windows.Forms.Label lblCaptureTime;
        private DarkNumericUpDown nudCaptureTime;
        private System.Windows.Forms.Label lblRewardExp;
        private DarkNumericUpDown nudRewardExp;
        private System.Windows.Forms.Label lblRewardGold;
        private DarkNumericUpDown nudRewardGold;
        private System.Windows.Forms.Label lblRewardTick;
        private DarkNumericUpDown nudRewardTick;
    }
}
