namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class Event_MoveRouteDesigner
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewGroup listViewGroup31 = new System.Windows.Forms.ListViewGroup("Move", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup32 = new System.Windows.Forms.ListViewGroup("Turn", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup33 = new System.Windows.Forms.ListViewGroup("Set Speed", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup34 = new System.Windows.Forms.ListViewGroup("Set Movement Frequency", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup35 = new System.Windows.Forms.ListViewGroup("Set Attribute", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup36 = new System.Windows.Forms.ListViewGroup("Etc", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem221 = new System.Windows.Forms.ListViewItem("Move Up");
            System.Windows.Forms.ListViewItem listViewItem222 = new System.Windows.Forms.ListViewItem("Move Down");
            System.Windows.Forms.ListViewItem listViewItem223 = new System.Windows.Forms.ListViewItem("Move Left");
            System.Windows.Forms.ListViewItem listViewItem224 = new System.Windows.Forms.ListViewItem("Move Right");
            System.Windows.Forms.ListViewItem listViewItem225 = new System.Windows.Forms.ListViewItem("Move Randomly");
            System.Windows.Forms.ListViewItem listViewItem226 = new System.Windows.Forms.ListViewItem("Move Toward Player");
            System.Windows.Forms.ListViewItem listViewItem227 = new System.Windows.Forms.ListViewItem("Move Away From Player");
            System.Windows.Forms.ListViewItem listViewItem228 = new System.Windows.Forms.ListViewItem("Step Forward");
            System.Windows.Forms.ListViewItem listViewItem229 = new System.Windows.Forms.ListViewItem("Step Back");
            System.Windows.Forms.ListViewItem listViewItem230 = new System.Windows.Forms.ListViewItem("Face Up");
            System.Windows.Forms.ListViewItem listViewItem231 = new System.Windows.Forms.ListViewItem("Face Down");
            System.Windows.Forms.ListViewItem listViewItem232 = new System.Windows.Forms.ListViewItem("Face Left");
            System.Windows.Forms.ListViewItem listViewItem233 = new System.Windows.Forms.ListViewItem("Face Right");
            System.Windows.Forms.ListViewItem listViewItem234 = new System.Windows.Forms.ListViewItem("Turn 90* Clockwise");
            System.Windows.Forms.ListViewItem listViewItem235 = new System.Windows.Forms.ListViewItem("Turn 90* Counter Clockwise");
            System.Windows.Forms.ListViewItem listViewItem236 = new System.Windows.Forms.ListViewItem("Turn 180*");
            System.Windows.Forms.ListViewItem listViewItem237 = new System.Windows.Forms.ListViewItem("Turn Randomly");
            System.Windows.Forms.ListViewItem listViewItem238 = new System.Windows.Forms.ListViewItem("Turn Toward Player");
            System.Windows.Forms.ListViewItem listViewItem239 = new System.Windows.Forms.ListViewItem("Turn Away From Player");
            System.Windows.Forms.ListViewItem listViewItem240 = new System.Windows.Forms.ListViewItem("Slowest");
            System.Windows.Forms.ListViewItem listViewItem241 = new System.Windows.Forms.ListViewItem("Slower");
            System.Windows.Forms.ListViewItem listViewItem242 = new System.Windows.Forms.ListViewItem("Normal");
            System.Windows.Forms.ListViewItem listViewItem243 = new System.Windows.Forms.ListViewItem("Faster");
            System.Windows.Forms.ListViewItem listViewItem244 = new System.Windows.Forms.ListViewItem("Fastest");
            System.Windows.Forms.ListViewItem listViewItem245 = new System.Windows.Forms.ListViewItem("Lowest");
            System.Windows.Forms.ListViewItem listViewItem246 = new System.Windows.Forms.ListViewItem("Lower");
            System.Windows.Forms.ListViewItem listViewItem247 = new System.Windows.Forms.ListViewItem("Normal");
            System.Windows.Forms.ListViewItem listViewItem248 = new System.Windows.Forms.ListViewItem("Higher");
            System.Windows.Forms.ListViewItem listViewItem249 = new System.Windows.Forms.ListViewItem("Highest");
            System.Windows.Forms.ListViewItem listViewItem250 = new System.Windows.Forms.ListViewItem("Walking Animation: On");
            System.Windows.Forms.ListViewItem listViewItem251 = new System.Windows.Forms.ListViewItem("Walking Animation: Off");
            System.Windows.Forms.ListViewItem listViewItem252 = new System.Windows.Forms.ListViewItem("Direction Fix: On");
            System.Windows.Forms.ListViewItem listViewItem253 = new System.Windows.Forms.ListViewItem("Direction Fix: Off");
            System.Windows.Forms.ListViewItem listViewItem254 = new System.Windows.Forms.ListViewItem("Walkthrough: On");
            System.Windows.Forms.ListViewItem listViewItem255 = new System.Windows.Forms.ListViewItem("Walkthrough: Off");
            System.Windows.Forms.ListViewItem listViewItem256 = new System.Windows.Forms.ListViewItem("Show Name");
            System.Windows.Forms.ListViewItem listViewItem257 = new System.Windows.Forms.ListViewItem("Hide Name");
            System.Windows.Forms.ListViewItem listViewItem258 = new System.Windows.Forms.ListViewItem("Set Layer: Below Player");
            System.Windows.Forms.ListViewItem listViewItem259 = new System.Windows.Forms.ListViewItem("Set Layer: With Player");
            System.Windows.Forms.ListViewItem listViewItem260 = new System.Windows.Forms.ListViewItem("Set Layer: Above Player");
            System.Windows.Forms.ListViewItem listViewItem261 = new System.Windows.Forms.ListViewItem("Wait 100ms");
            System.Windows.Forms.ListViewItem listViewItem262 = new System.Windows.Forms.ListViewItem("Wait 500ms");
            System.Windows.Forms.ListViewItem listViewItem263 = new System.Windows.Forms.ListViewItem("Wait 1000ms");
            System.Windows.Forms.ListViewItem listViewItem264 = new System.Windows.Forms.ListViewItem("Set Graphic...");
            this.grpMoveRoute = new System.Windows.Forms.GroupBox();
            this.btnOkay = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpCommands = new System.Windows.Forms.GroupBox();
            this.lstCommands = new System.Windows.Forms.ListView();
            this.chkRepeatRoute = new System.Windows.Forms.CheckBox();
            this.chkIgnoreIfBlocked = new System.Windows.Forms.CheckBox();
            this.lstActions = new System.Windows.Forms.ListBox();
            this.cmbTarget = new System.Windows.Forms.ComboBox();
            this.grpMoveRoute.SuspendLayout();
            this.grpCommands.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMoveRoute
            // 
            this.grpMoveRoute.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMoveRoute.Controls.Add(this.btnOkay);
            this.grpMoveRoute.Controls.Add(this.btnCancel);
            this.grpMoveRoute.Controls.Add(this.grpCommands);
            this.grpMoveRoute.Controls.Add(this.chkRepeatRoute);
            this.grpMoveRoute.Controls.Add(this.chkIgnoreIfBlocked);
            this.grpMoveRoute.Controls.Add(this.lstActions);
            this.grpMoveRoute.Controls.Add(this.cmbTarget);
            this.grpMoveRoute.Location = new System.Drawing.Point(3, 3);
            this.grpMoveRoute.Name = "grpMoveRoute";
            this.grpMoveRoute.Size = new System.Drawing.Size(531, 486);
            this.grpMoveRoute.TabIndex = 0;
            this.grpMoveRoute.TabStop = false;
            this.grpMoveRoute.Text = "Move Route";
            // 
            // btnOkay
            // 
            this.btnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOkay.Location = new System.Drawing.Point(369, 457);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(75, 23);
            this.btnOkay.TabIndex = 6;
            this.btnOkay.Text = "Ok";
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(450, 457);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpCommands
            // 
            this.grpCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCommands.Controls.Add(this.lstCommands);
            this.grpCommands.Location = new System.Drawing.Point(216, 20);
            this.grpCommands.Name = "grpCommands";
            this.grpCommands.Size = new System.Drawing.Size(309, 408);
            this.grpCommands.TabIndex = 4;
            this.grpCommands.TabStop = false;
            this.grpCommands.Text = "Commands";
            // 
            // lstCommands
            // 
            this.lstCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            listViewGroup31.Header = "Move";
            listViewGroup31.Name = "Move";
            listViewGroup32.Header = "Turn";
            listViewGroup32.Name = "Turn";
            listViewGroup33.Header = "Set Speed";
            listViewGroup33.Name = "Set Speed";
            listViewGroup34.Header = "Set Movement Frequency";
            listViewGroup34.Name = "Set Movement Frequency";
            listViewGroup35.Header = "Set Attribute";
            listViewGroup35.Name = "Set Attribute";
            listViewGroup36.Header = "Etc";
            listViewGroup36.Name = "Etc";
            this.lstCommands.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup31,
            listViewGroup32,
            listViewGroup33,
            listViewGroup34,
            listViewGroup35,
            listViewGroup36});
            this.lstCommands.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            listViewItem221.Group = listViewGroup31;
            listViewItem222.Group = listViewGroup31;
            listViewItem223.Group = listViewGroup31;
            listViewItem224.Group = listViewGroup31;
            listViewItem225.Group = listViewGroup31;
            listViewItem226.Group = listViewGroup31;
            listViewItem227.Group = listViewGroup31;
            listViewItem228.Group = listViewGroup31;
            listViewItem229.Group = listViewGroup31;
            listViewItem230.Group = listViewGroup32;
            listViewItem231.Group = listViewGroup32;
            listViewItem232.Group = listViewGroup32;
            listViewItem233.Group = listViewGroup32;
            listViewItem234.Group = listViewGroup32;
            listViewItem235.Group = listViewGroup32;
            listViewItem236.Group = listViewGroup32;
            listViewItem237.Group = listViewGroup32;
            listViewItem238.Group = listViewGroup32;
            listViewItem239.Group = listViewGroup32;
            listViewItem240.Group = listViewGroup33;
            listViewItem241.Group = listViewGroup33;
            listViewItem242.Group = listViewGroup33;
            listViewItem243.Group = listViewGroup33;
            listViewItem244.Group = listViewGroup33;
            listViewItem245.Group = listViewGroup34;
            listViewItem246.Group = listViewGroup34;
            listViewItem247.Group = listViewGroup34;
            listViewItem248.Group = listViewGroup34;
            listViewItem249.Group = listViewGroup34;
            listViewItem250.Group = listViewGroup35;
            listViewItem251.Group = listViewGroup35;
            listViewItem252.Group = listViewGroup35;
            listViewItem253.Group = listViewGroup35;
            listViewItem254.Group = listViewGroup35;
            listViewItem255.Group = listViewGroup35;
            listViewItem256.Group = listViewGroup35;
            listViewItem257.Group = listViewGroup35;
            listViewItem258.Group = listViewGroup35;
            listViewItem259.Group = listViewGroup35;
            listViewItem260.Group = listViewGroup35;
            listViewItem261.Group = listViewGroup36;
            listViewItem262.Group = listViewGroup36;
            listViewItem263.Group = listViewGroup36;
            listViewItem264.Group = listViewGroup36;
            this.lstCommands.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem221,
            listViewItem222,
            listViewItem223,
            listViewItem224,
            listViewItem225,
            listViewItem226,
            listViewItem227,
            listViewItem228,
            listViewItem229,
            listViewItem230,
            listViewItem231,
            listViewItem232,
            listViewItem233,
            listViewItem234,
            listViewItem235,
            listViewItem236,
            listViewItem237,
            listViewItem238,
            listViewItem239,
            listViewItem240,
            listViewItem241,
            listViewItem242,
            listViewItem243,
            listViewItem244,
            listViewItem245,
            listViewItem246,
            listViewItem247,
            listViewItem248,
            listViewItem249,
            listViewItem250,
            listViewItem251,
            listViewItem252,
            listViewItem253,
            listViewItem254,
            listViewItem255,
            listViewItem256,
            listViewItem257,
            listViewItem258,
            listViewItem259,
            listViewItem260,
            listViewItem261,
            listViewItem262,
            listViewItem263,
            listViewItem264});
            this.lstCommands.Location = new System.Drawing.Point(7, 20);
            this.lstCommands.MultiSelect = false;
            this.lstCommands.Name = "lstCommands";
            this.lstCommands.Size = new System.Drawing.Size(296, 382);
            this.lstCommands.TabIndex = 0;
            this.lstCommands.TileSize = new System.Drawing.Size(120, 30);
            this.lstCommands.UseCompatibleStateImageBehavior = false;
            this.lstCommands.View = System.Windows.Forms.View.Tile;
            this.lstCommands.ItemActivate += new System.EventHandler(this.lstCommands_ItemActivate);
            // 
            // chkRepeatRoute
            // 
            this.chkRepeatRoute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkRepeatRoute.AutoSize = true;
            this.chkRepeatRoute.Location = new System.Drawing.Point(7, 457);
            this.chkRepeatRoute.Name = "chkRepeatRoute";
            this.chkRepeatRoute.Size = new System.Drawing.Size(93, 17);
            this.chkRepeatRoute.TabIndex = 3;
            this.chkRepeatRoute.Text = "Repeat Route";
            this.chkRepeatRoute.UseVisualStyleBackColor = true;
            this.chkRepeatRoute.CheckedChanged += new System.EventHandler(this.chkRepeatRoute_CheckedChanged);
            // 
            // chkIgnoreIfBlocked
            // 
            this.chkIgnoreIfBlocked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkIgnoreIfBlocked.AutoSize = true;
            this.chkIgnoreIfBlocked.Location = new System.Drawing.Point(7, 434);
            this.chkIgnoreIfBlocked.Name = "chkIgnoreIfBlocked";
            this.chkIgnoreIfBlocked.Size = new System.Drawing.Size(106, 17);
            this.chkIgnoreIfBlocked.TabIndex = 2;
            this.chkIgnoreIfBlocked.Text = "Ignore if Blocked";
            this.chkIgnoreIfBlocked.UseVisualStyleBackColor = true;
            this.chkIgnoreIfBlocked.CheckedChanged += new System.EventHandler(this.chkIgnoreIfBlocked_CheckedChanged);
            // 
            // lstActions
            // 
            this.lstActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstActions.FormattingEnabled = true;
            this.lstActions.Location = new System.Drawing.Point(7, 47);
            this.lstActions.Name = "lstActions";
            this.lstActions.Size = new System.Drawing.Size(190, 381);
            this.lstActions.TabIndex = 1;
            this.lstActions.DoubleClick += new System.EventHandler(this.lstActions_DoubleClick);
            this.lstActions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstActions_KeyDown);
            this.lstActions.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstActions_MouseDown);
            // 
            // cmbTarget
            // 
            this.cmbTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTarget.FormattingEnabled = true;
            this.cmbTarget.Items.AddRange(new object[] {
            "Self"});
            this.cmbTarget.Location = new System.Drawing.Point(7, 20);
            this.cmbTarget.Name = "cmbTarget";
            this.cmbTarget.Size = new System.Drawing.Size(191, 21);
            this.cmbTarget.TabIndex = 0;
            // 
            // Event_MoveRouteDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpMoveRoute);
            this.Name = "Event_MoveRouteDesigner";
            this.Size = new System.Drawing.Size(537, 492);
            this.grpMoveRoute.ResumeLayout(false);
            this.grpMoveRoute.PerformLayout();
            this.grpCommands.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMoveRoute;
        private System.Windows.Forms.GroupBox grpCommands;
        private System.Windows.Forms.ListView lstCommands;
        private System.Windows.Forms.CheckBox chkRepeatRoute;
        private System.Windows.Forms.CheckBox chkIgnoreIfBlocked;
        private System.Windows.Forms.ListBox lstActions;
        private System.Windows.Forms.ComboBox cmbTarget;
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Button btnCancel;
    }
}
