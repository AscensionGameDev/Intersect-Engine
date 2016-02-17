namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_SpawnNpc
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grpTileSpawn = new System.Windows.Forms.GroupBox();
            this.btnVisual = new System.Windows.Forms.Button();
            this.scrlY = new System.Windows.Forms.HScrollBar();
            this.scrlX = new System.Windows.Forms.HScrollBar();
            this.cmbMap = new System.Windows.Forms.ComboBox();
            this.cmbDirection = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.cmbConditionType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpEntitySpawn = new System.Windows.Forms.GroupBox();
            this.cmbEntities = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlSpawnLoc = new System.Windows.Forms.Panel();
            this.chkDirRelative = new System.Windows.Forms.CheckBox();
            this.cmbNpc = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.grpTileSpawn.SuspendLayout();
            this.grpEntitySpawn.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbNpc);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.grpTileSpawn);
            this.groupBox1.Controls.Add(this.cmbConditionType);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.grpEntitySpawn);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(256, 388);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Spawn Npc";
            // 
            // grpTileSpawn
            // 
            this.grpTileSpawn.Controls.Add(this.btnVisual);
            this.grpTileSpawn.Controls.Add(this.scrlY);
            this.grpTileSpawn.Controls.Add(this.scrlX);
            this.grpTileSpawn.Controls.Add(this.cmbMap);
            this.grpTileSpawn.Controls.Add(this.cmbDirection);
            this.grpTileSpawn.Controls.Add(this.label23);
            this.grpTileSpawn.Controls.Add(this.lblY);
            this.grpTileSpawn.Controls.Add(this.label21);
            this.grpTileSpawn.Controls.Add(this.lblX);
            this.grpTileSpawn.Location = new System.Drawing.Point(9, 82);
            this.grpTileSpawn.Name = "grpTileSpawn";
            this.grpTileSpawn.Size = new System.Drawing.Size(236, 168);
            this.grpTileSpawn.TabIndex = 23;
            this.grpTileSpawn.TabStop = false;
            this.grpTileSpawn.Text = "Specific Tile";
            // 
            // btnVisual
            // 
            this.btnVisual.Location = new System.Drawing.Point(40, 133);
            this.btnVisual.Name = "btnVisual";
            this.btnVisual.Size = new System.Drawing.Size(155, 23);
            this.btnVisual.TabIndex = 30;
            this.btnVisual.Text = "Open Visual Interface";
            this.btnVisual.UseVisualStyleBackColor = true;
            this.btnVisual.Click += new System.EventHandler(this.btnVisual_Click);
            // 
            // scrlY
            // 
            this.scrlY.Location = new System.Drawing.Point(74, 76);
            this.scrlY.Name = "scrlY";
            this.scrlY.Size = new System.Drawing.Size(121, 17);
            this.scrlY.TabIndex = 29;
            this.scrlY.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlY_Scroll);
            // 
            // scrlX
            // 
            this.scrlX.Location = new System.Drawing.Point(74, 49);
            this.scrlX.Name = "scrlX";
            this.scrlX.Size = new System.Drawing.Size(121, 17);
            this.scrlX.TabIndex = 28;
            this.scrlX.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrlX_Scroll);
            // 
            // cmbMap
            // 
            this.cmbMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMap.FormattingEnabled = true;
            this.cmbMap.Items.AddRange(new object[] {
            "Retain Direction",
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbMap.Location = new System.Drawing.Point(74, 19);
            this.cmbMap.Name = "cmbMap";
            this.cmbMap.Size = new System.Drawing.Size(121, 21);
            this.cmbMap.TabIndex = 27;
            // 
            // cmbDirection
            // 
            this.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirection.FormattingEnabled = true;
            this.cmbDirection.Items.AddRange(new object[] {
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbDirection.Location = new System.Drawing.Point(74, 100);
            this.cmbDirection.Name = "cmbDirection";
            this.cmbDirection.Size = new System.Drawing.Size(121, 21);
            this.cmbDirection.TabIndex = 26;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(37, 103);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(23, 13);
            this.label23.TabIndex = 25;
            this.label23.Text = "Dir:";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(37, 76);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(26, 13);
            this.lblY.TabIndex = 24;
            this.lblY.Text = "Y: 0";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(37, 22);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(31, 13);
            this.label21.TabIndex = 22;
            this.label21.Text = "Map:";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(37, 49);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(26, 13);
            this.lblX.TabIndex = 23;
            this.lblX.Text = "X: 0";
            // 
            // cmbConditionType
            // 
            this.cmbConditionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConditionType.FormattingEnabled = true;
            this.cmbConditionType.Items.AddRange(new object[] {
            "Specific Tile",
            "On/Around Entity"});
            this.cmbConditionType.Location = new System.Drawing.Point(88, 44);
            this.cmbConditionType.Name = "cmbConditionType";
            this.cmbConditionType.Size = new System.Drawing.Size(157, 21);
            this.cmbConditionType.TabIndex = 22;
            this.cmbConditionType.SelectedIndexChanged += new System.EventHandler(this.cmbConditionType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Spawn Type:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(90, 359);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(9, 359);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpEntitySpawn
            // 
            this.grpEntitySpawn.Controls.Add(this.chkDirRelative);
            this.grpEntitySpawn.Controls.Add(this.pnlSpawnLoc);
            this.grpEntitySpawn.Controls.Add(this.label2);
            this.grpEntitySpawn.Controls.Add(this.cmbEntities);
            this.grpEntitySpawn.Controls.Add(this.label4);
            this.grpEntitySpawn.Location = new System.Drawing.Point(9, 81);
            this.grpEntitySpawn.Name = "grpEntitySpawn";
            this.grpEntitySpawn.Size = new System.Drawing.Size(236, 264);
            this.grpEntitySpawn.TabIndex = 24;
            this.grpEntitySpawn.TabStop = false;
            this.grpEntitySpawn.Text = "On/Around Entity";
            // 
            // cmbEntities
            // 
            this.cmbEntities.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntities.FormattingEnabled = true;
            this.cmbEntities.Items.AddRange(new object[] {
            "Retain Direction",
            "Up",
            "Down",
            "Left",
            "Right"});
            this.cmbEntities.Location = new System.Drawing.Point(74, 19);
            this.cmbEntities.Name = "cmbEntities";
            this.cmbEntities.Size = new System.Drawing.Size(121, 21);
            this.cmbEntities.TabIndex = 27;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Entity:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Relative Location:";
            // 
            // pnlSpawnLoc
            // 
            this.pnlSpawnLoc.Location = new System.Drawing.Point(38, 69);
            this.pnlSpawnLoc.Name = "pnlSpawnLoc";
            this.pnlSpawnLoc.Size = new System.Drawing.Size(160, 160);
            this.pnlSpawnLoc.TabIndex = 29;
            this.pnlSpawnLoc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlSpawnLoc_MouseDown);
            // 
            // chkDirRelative
            // 
            this.chkDirRelative.AutoSize = true;
            this.chkDirRelative.Location = new System.Drawing.Point(38, 236);
            this.chkDirRelative.Name = "chkDirRelative";
            this.chkDirRelative.Size = new System.Drawing.Size(151, 17);
            this.chkDirRelative.TabIndex = 30;
            this.chkDirRelative.Text = "Relative to Entity Direction";
            this.chkDirRelative.UseVisualStyleBackColor = true;
            // 
            // cmbNpc
            // 
            this.cmbNpc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNpc.FormattingEnabled = true;
            this.cmbNpc.Items.AddRange(new object[] {
            "Specific Tile",
            "On/Around Entity"});
            this.cmbNpc.Location = new System.Drawing.Point(88, 15);
            this.cmbNpc.Name = "cmbNpc";
            this.cmbNpc.Size = new System.Drawing.Size(157, 21);
            this.cmbNpc.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Npc:";
            // 
            // EventCommand_SpawnNpc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Name = "EventCommand_SpawnNpc";
            this.Size = new System.Drawing.Size(267, 394);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpTileSpawn.ResumeLayout(false);
            this.grpTileSpawn.PerformLayout();
            this.grpEntitySpawn.ResumeLayout(false);
            this.grpEntitySpawn.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox grpTileSpawn;
        private System.Windows.Forms.ComboBox cmbConditionType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnVisual;
        private System.Windows.Forms.HScrollBar scrlY;
        private System.Windows.Forms.HScrollBar scrlX;
        private System.Windows.Forms.ComboBox cmbMap;
        private System.Windows.Forms.ComboBox cmbDirection;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.GroupBox grpEntitySpawn;
        private System.Windows.Forms.CheckBox chkDirRelative;
        private System.Windows.Forms.Panel pnlSpawnLoc;
        private System.Windows.Forms.ComboBox cmbEntities;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbNpc;
        private System.Windows.Forms.Label label3;
    }
}
