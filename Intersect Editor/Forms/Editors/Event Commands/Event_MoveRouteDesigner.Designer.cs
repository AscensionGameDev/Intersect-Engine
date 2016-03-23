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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Move", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Turn", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Set Speed", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Set Movement Frequency", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Set Attribute", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Etc", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Move Up");
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Move Down");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Move Left");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Move Right");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Move Randomly");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Move Toward Player");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Move Away From Player");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Step Forward");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Step Back");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Face Up");
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("Face Down");
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("Face Left");
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem("Face Right");
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem("Turn 90* Clockwise");
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem("Turn 90* Counter Clockwise");
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem("Turn 180*");
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem("Turn Randomly");
            System.Windows.Forms.ListViewItem listViewItem18 = new System.Windows.Forms.ListViewItem("Turn Toward Player");
            System.Windows.Forms.ListViewItem listViewItem19 = new System.Windows.Forms.ListViewItem("Turn Away From Player");
            System.Windows.Forms.ListViewItem listViewItem20 = new System.Windows.Forms.ListViewItem("Slowest");
            System.Windows.Forms.ListViewItem listViewItem21 = new System.Windows.Forms.ListViewItem("Slower");
            System.Windows.Forms.ListViewItem listViewItem22 = new System.Windows.Forms.ListViewItem("Normal");
            System.Windows.Forms.ListViewItem listViewItem23 = new System.Windows.Forms.ListViewItem("Faster");
            System.Windows.Forms.ListViewItem listViewItem24 = new System.Windows.Forms.ListViewItem("Fastest");
            System.Windows.Forms.ListViewItem listViewItem25 = new System.Windows.Forms.ListViewItem("Lowest");
            System.Windows.Forms.ListViewItem listViewItem26 = new System.Windows.Forms.ListViewItem("Lower");
            System.Windows.Forms.ListViewItem listViewItem27 = new System.Windows.Forms.ListViewItem("Normal");
            System.Windows.Forms.ListViewItem listViewItem28 = new System.Windows.Forms.ListViewItem("Higher");
            System.Windows.Forms.ListViewItem listViewItem29 = new System.Windows.Forms.ListViewItem("Highest");
            System.Windows.Forms.ListViewItem listViewItem30 = new System.Windows.Forms.ListViewItem("Walking Animation: On");
            System.Windows.Forms.ListViewItem listViewItem31 = new System.Windows.Forms.ListViewItem("Walking Animation: Off");
            System.Windows.Forms.ListViewItem listViewItem32 = new System.Windows.Forms.ListViewItem("Direction Fix: On");
            System.Windows.Forms.ListViewItem listViewItem33 = new System.Windows.Forms.ListViewItem("Direction Fix: Off");
            System.Windows.Forms.ListViewItem listViewItem34 = new System.Windows.Forms.ListViewItem("Walkthrough: On");
            System.Windows.Forms.ListViewItem listViewItem35 = new System.Windows.Forms.ListViewItem("Walkthrough: Off");
            System.Windows.Forms.ListViewItem listViewItem36 = new System.Windows.Forms.ListViewItem("Show Name");
            System.Windows.Forms.ListViewItem listViewItem37 = new System.Windows.Forms.ListViewItem("Hide Name");
            System.Windows.Forms.ListViewItem listViewItem38 = new System.Windows.Forms.ListViewItem("Set Layer: Below Player");
            System.Windows.Forms.ListViewItem listViewItem39 = new System.Windows.Forms.ListViewItem("Set Layer: With Player");
            System.Windows.Forms.ListViewItem listViewItem40 = new System.Windows.Forms.ListViewItem("Set Layer: Above Player");
            System.Windows.Forms.ListViewItem listViewItem41 = new System.Windows.Forms.ListViewItem("Wait 100ms");
            System.Windows.Forms.ListViewItem listViewItem42 = new System.Windows.Forms.ListViewItem("Wait 500ms");
            System.Windows.Forms.ListViewItem listViewItem43 = new System.Windows.Forms.ListViewItem("Wait 1000ms");
            System.Windows.Forms.ListViewItem listViewItem44 = new System.Windows.Forms.ListViewItem("Set Graphic...");
            System.Windows.Forms.ListViewItem listViewItem45 = new System.Windows.Forms.ListViewItem("Set Animation...");
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
            listViewGroup1.Header = "Move";
            listViewGroup1.Name = "Move";
            listViewGroup2.Header = "Turn";
            listViewGroup2.Name = "Turn";
            listViewGroup3.Header = "Set Speed";
            listViewGroup3.Name = "Set Speed";
            listViewGroup4.Header = "Set Movement Frequency";
            listViewGroup4.Name = "Set Movement Frequency";
            listViewGroup5.Header = "Set Attribute";
            listViewGroup5.Name = "Set Attribute";
            listViewGroup6.Header = "Etc";
            listViewGroup6.Name = "Etc";
            this.lstCommands.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6});
            this.lstCommands.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            listViewItem1.Group = listViewGroup1;
            listViewItem2.Group = listViewGroup1;
            listViewItem3.Group = listViewGroup1;
            listViewItem4.Group = listViewGroup1;
            listViewItem5.Group = listViewGroup1;
            listViewItem6.Group = listViewGroup1;
            listViewItem7.Group = listViewGroup1;
            listViewItem8.Group = listViewGroup1;
            listViewItem9.Group = listViewGroup1;
            listViewItem10.Group = listViewGroup2;
            listViewItem11.Group = listViewGroup2;
            listViewItem12.Group = listViewGroup2;
            listViewItem13.Group = listViewGroup2;
            listViewItem14.Group = listViewGroup2;
            listViewItem15.Group = listViewGroup2;
            listViewItem16.Group = listViewGroup2;
            listViewItem17.Group = listViewGroup2;
            listViewItem18.Group = listViewGroup2;
            listViewItem19.Group = listViewGroup2;
            listViewItem20.Group = listViewGroup3;
            listViewItem21.Group = listViewGroup3;
            listViewItem22.Group = listViewGroup3;
            listViewItem23.Group = listViewGroup3;
            listViewItem24.Group = listViewGroup3;
            listViewItem25.Group = listViewGroup4;
            listViewItem26.Group = listViewGroup4;
            listViewItem27.Group = listViewGroup4;
            listViewItem28.Group = listViewGroup4;
            listViewItem29.Group = listViewGroup4;
            listViewItem30.Group = listViewGroup5;
            listViewItem31.Group = listViewGroup5;
            listViewItem32.Group = listViewGroup5;
            listViewItem33.Group = listViewGroup5;
            listViewItem34.Group = listViewGroup5;
            listViewItem35.Group = listViewGroup5;
            listViewItem36.Group = listViewGroup5;
            listViewItem37.Group = listViewGroup5;
            listViewItem38.Group = listViewGroup5;
            listViewItem39.Group = listViewGroup5;
            listViewItem40.Group = listViewGroup5;
            listViewItem41.Group = listViewGroup6;
            listViewItem42.Group = listViewGroup6;
            listViewItem43.Group = listViewGroup6;
            listViewItem44.Group = listViewGroup6;
            listViewItem45.Group = listViewGroup6;
            this.lstCommands.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17,
            listViewItem18,
            listViewItem19,
            listViewItem20,
            listViewItem21,
            listViewItem22,
            listViewItem23,
            listViewItem24,
            listViewItem25,
            listViewItem26,
            listViewItem27,
            listViewItem28,
            listViewItem29,
            listViewItem30,
            listViewItem31,
            listViewItem32,
            listViewItem33,
            listViewItem34,
            listViewItem35,
            listViewItem36,
            listViewItem37,
            listViewItem38,
            listViewItem39,
            listViewItem40,
            listViewItem41,
            listViewItem42,
            listViewItem43,
            listViewItem44,
            listViewItem45});
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
