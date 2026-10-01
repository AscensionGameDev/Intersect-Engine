using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
{
    partial class FrmDungeon
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

        private DarkCheckBox MakeCheckBox(string text, int x, int y)
        {
            return new DarkCheckBox
            {
                AutoSize = true,
                ForeColor = System.Drawing.Color.Gainsboro,
                Location = new System.Drawing.Point(x, y),
                Text = text,
            };
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDungeon));
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
            this.grpDungeons = MakeGroup("Dungeons", 12, 36, 203, 573);
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
                Size = new System.Drawing.Size(191, 525),
            };
            this.grpDungeons.Controls.Add(this.btnClearSearch);
            this.grpDungeons.Controls.Add(this.txtSearch);
            this.grpDungeons.Controls.Add(this.lstGameObjects);

            // ---- Right: container
            this.pnlContainer = new System.Windows.Forms.Panel
            {
                Location = new System.Drawing.Point(221, 36),
                Size = new System.Drawing.Size(527, 573),
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

            // Entry
            this.grpEntry = MakeGroup("Entrance", 7, 139, 513, 78);
            this.lblEntryMap = MakeLabel("Map:", 6, 22);
            this.cmbEntryMap = MakeCombo(70, 19, 200);
            this.cmbEntryMap.SelectedIndexChanged += new System.EventHandler(this.cmbEntryMap_SelectedIndexChanged);
            this.lblEntryX = MakeLabel("X:", 280, 22);
            this.nudEntryX = MakeNumeric(300, 20, 50, 0, 255, 0);
            this.nudEntryX.ValueChanged += new System.EventHandler(this.nudEntry_ValueChanged);
            this.lblEntryY = MakeLabel("Y:", 358, 22);
            this.nudEntryY = MakeNumeric(378, 20, 50, 0, 255, 0);
            this.nudEntryY.ValueChanged += new System.EventHandler(this.nudEntry_ValueChanged);
            this.lblEntryDir = MakeLabel("Direction:", 6, 50);
            this.cmbEntryDir = MakeCombo(70, 47, 100);
            this.cmbEntryDir.SelectedIndexChanged += new System.EventHandler(this.cmbEntryDir_SelectedIndexChanged);
            this.btnVisual = MakeButton("Open Visual Interface", 300, 46, 207);
            this.btnVisual.Click += new System.EventHandler(this.btnVisual_Click);
            this.grpEntry.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblEntryMap, this.cmbEntryMap, this.lblEntryX, this.nudEntryX, this.lblEntryY, this.nudEntryY, this.lblEntryDir, this.cmbEntryDir, this.btnVisual });

            // Rules
            this.grpRules = MakeGroup("Rules", 7, 223, 513, 130);
            this.lblAccess = MakeLabel("Access:", 6, 22);
            this.cmbAccess = MakeCombo(110, 19, 130);
            this.cmbAccess.SelectedIndexChanged += new System.EventHandler(this.cmbAccess_SelectedIndexChanged);
            this.lblMinPlayers = MakeLabel("Min players:", 262, 22);
            this.nudMinPlayers = MakeNumeric(370, 20, 50, 1, 100, 1);
            this.nudMinPlayers.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.lblMaxPlayers = MakeLabel("Max (0=any):", 428, 22);
            this.nudMaxPlayers = MakeNumeric(500, 20, 0, 0, 0, 0); // placed below via SetBounds
            this.nudMaxPlayers.SetBounds(370, 46, 50, 20);
            this.nudMaxPlayers.Maximum = 100;
            this.nudMaxPlayers.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.lblMaxPlayers.Location = new System.Drawing.Point(262, 48);
            this.lblLevel = MakeLabel("Required level:", 6, 48);
            this.nudLevel = MakeNumeric(110, 46, 60, 1, 1000, 1);
            this.nudLevel.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.lblTimeLimit = MakeLabel("Time limit (s):", 6, 74);
            this.nudTimeLimit = MakeNumeric(110, 72, 60, 0, 86400, 0);
            this.nudTimeLimit.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.lblLives = MakeLabel("Lives (0=inf):", 262, 74);
            this.nudLives = MakeNumeric(370, 72, 50, 0, 999, 3);
            this.nudLives.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.lblCooldown = MakeLabel("Cooldown (s):", 6, 100);
            this.nudCooldown = MakeNumeric(110, 98, 60, 0, 604800, 0);
            this.nudCooldown.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.lblExitDelay = MakeLabel("Exit delay (s):", 262, 100);
            this.nudExitDelay = MakeNumeric(370, 98, 50, 0, 300, 8);
            this.nudExitDelay.ValueChanged += new System.EventHandler(this.nudRules_ValueChanged);
            this.grpRules.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblAccess, this.cmbAccess, this.lblMinPlayers, this.nudMinPlayers, this.lblMaxPlayers, this.nudMaxPlayers,
                this.lblLevel, this.nudLevel, this.lblTimeLimit, this.nudTimeLimit, this.lblLives, this.nudLives,
                this.lblCooldown, this.nudCooldown, this.lblExitDelay, this.nudExitDelay });

            // Rewards
            this.grpRewards = MakeGroup("Rewards", 7, 361, 513, 196);
            this.lstRewards = new System.Windows.Forms.ListBox
            {
                BackColor = System.Drawing.Color.FromArgb(60, 63, 65),
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                ForeColor = System.Drawing.Color.Gainsboro,
                FormattingEnabled = true,
                Location = new System.Drawing.Point(6, 19),
                Size = new System.Drawing.Size(240, 108),
            };
            this.lblRewardItem = MakeLabel("Item:", 254, 22);
            this.cmbRewardItem = MakeCombo(300, 19, 207);
            this.lblRewardQuantity = MakeLabel("Quantity:", 254, 48);
            this.nudRewardQuantity = MakeNumeric(300, 46, 80, 1, 1000000, 1);
            this.btnAddReward = MakeButton("Add", 300, 72, 100);
            this.btnAddReward.Click += new System.EventHandler(this.btnAddReward_Click);
            this.btnRemoveReward = MakeButton("Remove", 407, 72, 100);
            this.btnRemoveReward.Click += new System.EventHandler(this.btnRemoveReward_Click);
            this.lblRewardExp = MakeLabel("Experience:", 6, 140);
            this.nudRewardExp = MakeNumeric(80, 138, 120, 0, int.MaxValue, 0);
            this.nudRewardExp.ValueChanged += new System.EventHandler(this.nudRewardExp_ValueChanged);
            this.lblRewardTitle = MakeLabel("Title:", 254, 140);
            this.cmbRewardTitle = MakeCombo(300, 137, 207);
            this.cmbRewardTitle.SelectedIndexChanged += new System.EventHandler(this.cmbRewardTitle_SelectedIndexChanged);
            this.lblRewardAchievement = MakeLabel("Achievement:", 6, 166);
            this.cmbRewardAchievement = MakeCombo(100, 163, 190);
            this.cmbRewardAchievement.SelectedIndexChanged += new System.EventHandler(this.cmbRewardAchievement_SelectedIndexChanged);
            this.grpRewards.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lstRewards, this.lblRewardItem, this.cmbRewardItem, this.lblRewardQuantity, this.nudRewardQuantity,
                this.btnAddReward, this.btnRemoveReward, this.lblRewardExp, this.nudRewardExp, this.lblRewardTitle, this.cmbRewardTitle,
                this.lblRewardAchievement, this.cmbRewardAchievement });

            this.pnlContainer.Controls.AddRange(new System.Windows.Forms.Control[] { this.grpGeneral, this.grpEntry, this.grpRules, this.grpRewards });

            // ---- Bottom buttons
            this.btnSave = MakeButton("Save", 380, 617, 169, 27);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnCancel = MakeButton("Cancel", 555, 617, 172, 27);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // ---- Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = PanelColor;
            this.ClientSize = new System.Drawing.Size(760, 657);
            this.ControlBox = true;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.pnlContainer);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpDungeons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmDungeon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dungeon Editor";
            FormClosed += FrmDungeon_FormClosed;
            this.Load += new System.EventHandler(this.frmDungeon_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.form_KeyDown);
            this.ResumeLayout(false);
        }

        #endregion

        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkGroupBox grpDungeons;
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
        private System.Windows.Forms.Label lblEntryDir;
        private DarkComboBox cmbEntryDir;
        private DarkButton btnVisual;

        private DarkGroupBox grpRules;
        private System.Windows.Forms.Label lblAccess;
        private DarkComboBox cmbAccess;
        private System.Windows.Forms.Label lblMinPlayers;
        private DarkNumericUpDown nudMinPlayers;
        private System.Windows.Forms.Label lblMaxPlayers;
        private DarkNumericUpDown nudMaxPlayers;
        private System.Windows.Forms.Label lblLevel;
        private DarkNumericUpDown nudLevel;
        private System.Windows.Forms.Label lblTimeLimit;
        private DarkNumericUpDown nudTimeLimit;
        private System.Windows.Forms.Label lblLives;
        private DarkNumericUpDown nudLives;
        private System.Windows.Forms.Label lblCooldown;
        private DarkNumericUpDown nudCooldown;
        private System.Windows.Forms.Label lblExitDelay;
        private DarkNumericUpDown nudExitDelay;

        private DarkGroupBox grpRewards;
        private System.Windows.Forms.ListBox lstRewards;
        private System.Windows.Forms.Label lblRewardItem;
        private DarkComboBox cmbRewardItem;
        private System.Windows.Forms.Label lblRewardQuantity;
        private DarkNumericUpDown nudRewardQuantity;
        private DarkButton btnAddReward;
        private DarkButton btnRemoveReward;
        private System.Windows.Forms.Label lblRewardExp;
        private DarkNumericUpDown nudRewardExp;
        private System.Windows.Forms.Label lblRewardTitle;
        private DarkComboBox cmbRewardTitle;
        private System.Windows.Forms.Label lblRewardAchievement;
        private DarkComboBox cmbRewardAchievement;
    }
}
