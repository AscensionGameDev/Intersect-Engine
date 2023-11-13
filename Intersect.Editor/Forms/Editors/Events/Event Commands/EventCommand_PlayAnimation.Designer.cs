using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandPlayAnimation
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
            grpPlayAnimation = new DarkGroupBox();
            cmbAnimation = new DarkComboBox();
            lblAnimation = new Label();
            cmbConditionType = new DarkComboBox();
            lblSpawnType = new Label();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            grpTileSpawn = new DarkGroupBox();
            nudWarpY = new DarkNumericUpDown();
            nudWarpX = new DarkNumericUpDown();
            btnVisual = new DarkButton();
            cmbMap = new DarkComboBox();
            cmbDirection = new DarkComboBox();
            lblDir = new Label();
            lblY = new Label();
            lblMap = new Label();
            lblX = new Label();
            grpEntitySpawn = new DarkGroupBox();
            chkRotateDirection = new DarkCheckBox();
            chkRelativeLocation = new DarkCheckBox();
            pnlSpawnLoc = new Panel();
            lblRelativeLocation = new Label();
            cmbEntities = new DarkComboBox();
            lblEntity = new Label();
            chkInstanceToPlayer = new DarkCheckBox();
            grpPlayAnimation.SuspendLayout();
            grpTileSpawn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudWarpY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWarpX).BeginInit();
            grpEntitySpawn.SuspendLayout();
            SuspendLayout();
            // 
            // grpPlayAnimation
            // 
            grpPlayAnimation.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpPlayAnimation.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpPlayAnimation.Controls.Add(chkInstanceToPlayer);
            grpPlayAnimation.Controls.Add(cmbAnimation);
            grpPlayAnimation.Controls.Add(lblAnimation);
            grpPlayAnimation.Controls.Add(cmbConditionType);
            grpPlayAnimation.Controls.Add(lblSpawnType);
            grpPlayAnimation.Controls.Add(btnCancel);
            grpPlayAnimation.Controls.Add(btnSave);
            grpPlayAnimation.Controls.Add(grpTileSpawn);
            grpPlayAnimation.Controls.Add(grpEntitySpawn);
            grpPlayAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            grpPlayAnimation.Location = new System.Drawing.Point(4, 3);
            grpPlayAnimation.Margin = new Padding(4, 3, 4, 3);
            grpPlayAnimation.Name = "grpPlayAnimation";
            grpPlayAnimation.Padding = new Padding(4, 3, 4, 3);
            grpPlayAnimation.Size = new Size(299, 483);
            grpPlayAnimation.TabIndex = 17;
            grpPlayAnimation.TabStop = false;
            grpPlayAnimation.Text = "Play Animation";
            // 
            // cmbAnimation
            // 
            cmbAnimation.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbAnimation.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbAnimation.BorderStyle = ButtonBorderStyle.Solid;
            cmbAnimation.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbAnimation.DrawDropdownHoverOutline = false;
            cmbAnimation.DrawFocusRectangle = false;
            cmbAnimation.DrawMode = DrawMode.OwnerDrawFixed;
            cmbAnimation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAnimation.FlatStyle = FlatStyle.Flat;
            cmbAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            cmbAnimation.FormattingEnabled = true;
            cmbAnimation.Location = new System.Drawing.Point(103, 17);
            cmbAnimation.Margin = new Padding(4, 3, 4, 3);
            cmbAnimation.Name = "cmbAnimation";
            cmbAnimation.Size = new Size(182, 24);
            cmbAnimation.TabIndex = 26;
            cmbAnimation.Text = null;
            cmbAnimation.TextPadding = new Padding(2);
            // 
            // lblAnimation
            // 
            lblAnimation.AutoSize = true;
            lblAnimation.Location = new System.Drawing.Point(7, 21);
            lblAnimation.Margin = new Padding(4, 0, 4, 0);
            lblAnimation.Name = "lblAnimation";
            lblAnimation.Size = new Size(66, 15);
            lblAnimation.TabIndex = 25;
            lblAnimation.Text = "Animation:";
            // 
            // cmbConditionType
            // 
            cmbConditionType.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbConditionType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbConditionType.BorderStyle = ButtonBorderStyle.Solid;
            cmbConditionType.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbConditionType.DrawDropdownHoverOutline = false;
            cmbConditionType.DrawFocusRectangle = false;
            cmbConditionType.DrawMode = DrawMode.OwnerDrawFixed;
            cmbConditionType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbConditionType.FlatStyle = FlatStyle.Flat;
            cmbConditionType.ForeColor = System.Drawing.Color.Gainsboro;
            cmbConditionType.FormattingEnabled = true;
            cmbConditionType.Location = new System.Drawing.Point(103, 51);
            cmbConditionType.Margin = new Padding(4, 3, 4, 3);
            cmbConditionType.Name = "cmbConditionType";
            cmbConditionType.Size = new Size(182, 24);
            cmbConditionType.TabIndex = 22;
            cmbConditionType.Text = null;
            cmbConditionType.TextPadding = new Padding(2);
            cmbConditionType.SelectedIndexChanged += cmbConditionType_SelectedIndexChanged;
            // 
            // lblSpawnType
            // 
            lblSpawnType.AutoSize = true;
            lblSpawnType.Location = new System.Drawing.Point(7, 54);
            lblSpawnType.Margin = new Padding(4, 0, 4, 0);
            lblSpawnType.Name = "lblSpawnType";
            lblSpawnType.Size = new Size(72, 15);
            lblSpawnType.TabIndex = 21;
            lblSpawnType.Text = "Spawn Type:";
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(197, 450);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6, 6, 6, 6);
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 20;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(103, 450);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6, 6, 6, 6);
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 19;
            btnSave.Text = "Ok";
            btnSave.Click += btnSave_Click;
            // 
            // grpTileSpawn
            // 
            grpTileSpawn.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpTileSpawn.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpTileSpawn.Controls.Add(nudWarpY);
            grpTileSpawn.Controls.Add(nudWarpX);
            grpTileSpawn.Controls.Add(btnVisual);
            grpTileSpawn.Controls.Add(cmbMap);
            grpTileSpawn.Controls.Add(cmbDirection);
            grpTileSpawn.Controls.Add(lblDir);
            grpTileSpawn.Controls.Add(lblY);
            grpTileSpawn.Controls.Add(lblMap);
            grpTileSpawn.Controls.Add(lblX);
            grpTileSpawn.ForeColor = System.Drawing.Color.Gainsboro;
            grpTileSpawn.Location = new System.Drawing.Point(10, 95);
            grpTileSpawn.Margin = new Padding(4, 3, 4, 3);
            grpTileSpawn.Name = "grpTileSpawn";
            grpTileSpawn.Padding = new Padding(4, 3, 4, 3);
            grpTileSpawn.Size = new Size(275, 194);
            grpTileSpawn.TabIndex = 23;
            grpTileSpawn.TabStop = false;
            grpTileSpawn.Text = "Specific Tile";
            // 
            // nudWarpY
            // 
            nudWarpY.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudWarpY.ForeColor = System.Drawing.Color.Gainsboro;
            nudWarpY.Location = new System.Drawing.Point(86, 85);
            nudWarpY.Margin = new Padding(4, 3, 4, 3);
            nudWarpY.Name = "nudWarpY";
            nudWarpY.Size = new Size(140, 23);
            nudWarpY.TabIndex = 32;
            nudWarpY.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // nudWarpX
            // 
            nudWarpX.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudWarpX.ForeColor = System.Drawing.Color.Gainsboro;
            nudWarpX.Location = new System.Drawing.Point(86, 54);
            nudWarpX.Margin = new Padding(4, 3, 4, 3);
            nudWarpX.Name = "nudWarpX";
            nudWarpX.Size = new Size(141, 23);
            nudWarpX.TabIndex = 31;
            nudWarpX.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // btnVisual
            // 
            btnVisual.Location = new System.Drawing.Point(47, 153);
            btnVisual.Margin = new Padding(4, 3, 4, 3);
            btnVisual.Name = "btnVisual";
            btnVisual.Padding = new Padding(6, 6, 6, 6);
            btnVisual.Size = new Size(181, 27);
            btnVisual.TabIndex = 30;
            btnVisual.Text = "Open Visual Interface";
            btnVisual.Click += btnVisual_Click;
            // 
            // cmbMap
            // 
            cmbMap.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbMap.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbMap.BorderStyle = ButtonBorderStyle.Solid;
            cmbMap.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbMap.DrawDropdownHoverOutline = false;
            cmbMap.DrawFocusRectangle = false;
            cmbMap.DrawMode = DrawMode.OwnerDrawFixed;
            cmbMap.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMap.FlatStyle = FlatStyle.Flat;
            cmbMap.ForeColor = System.Drawing.Color.Gainsboro;
            cmbMap.FormattingEnabled = true;
            cmbMap.Location = new System.Drawing.Point(86, 22);
            cmbMap.Margin = new Padding(4, 3, 4, 3);
            cmbMap.Name = "cmbMap";
            cmbMap.Size = new Size(140, 24);
            cmbMap.TabIndex = 27;
            cmbMap.Text = null;
            cmbMap.TextPadding = new Padding(2);
            // 
            // cmbDirection
            // 
            cmbDirection.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbDirection.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbDirection.BorderStyle = ButtonBorderStyle.Solid;
            cmbDirection.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbDirection.DrawDropdownHoverOutline = false;
            cmbDirection.DrawFocusRectangle = false;
            cmbDirection.DrawMode = DrawMode.OwnerDrawFixed;
            cmbDirection.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDirection.FlatStyle = FlatStyle.Flat;
            cmbDirection.ForeColor = System.Drawing.Color.Gainsboro;
            cmbDirection.FormattingEnabled = true;
            cmbDirection.Location = new System.Drawing.Point(86, 115);
            cmbDirection.Margin = new Padding(4, 3, 4, 3);
            cmbDirection.Name = "cmbDirection";
            cmbDirection.Size = new Size(140, 24);
            cmbDirection.TabIndex = 26;
            cmbDirection.Text = null;
            cmbDirection.TextPadding = new Padding(2);
            // 
            // lblDir
            // 
            lblDir.AutoSize = true;
            lblDir.Location = new System.Drawing.Point(43, 119);
            lblDir.Margin = new Padding(4, 0, 4, 0);
            lblDir.Name = "lblDir";
            lblDir.Size = new Size(25, 15);
            lblDir.TabIndex = 25;
            lblDir.Text = "Dir:";
            // 
            // lblY
            // 
            lblY.AutoSize = true;
            lblY.Location = new System.Drawing.Point(43, 88);
            lblY.Margin = new Padding(4, 0, 4, 0);
            lblY.Name = "lblY";
            lblY.Size = new Size(20, 15);
            lblY.TabIndex = 24;
            lblY.Text = "Y: ";
            // 
            // lblMap
            // 
            lblMap.AutoSize = true;
            lblMap.Location = new System.Drawing.Point(43, 25);
            lblMap.Margin = new Padding(4, 0, 4, 0);
            lblMap.Name = "lblMap";
            lblMap.Size = new Size(34, 15);
            lblMap.TabIndex = 22;
            lblMap.Text = "Map:";
            // 
            // lblX
            // 
            lblX.AutoSize = true;
            lblX.Location = new System.Drawing.Point(43, 57);
            lblX.Margin = new Padding(4, 0, 4, 0);
            lblX.Name = "lblX";
            lblX.Size = new Size(20, 15);
            lblX.TabIndex = 23;
            lblX.Text = "X: ";
            // 
            // grpEntitySpawn
            // 
            grpEntitySpawn.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpEntitySpawn.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpEntitySpawn.Controls.Add(chkRotateDirection);
            grpEntitySpawn.Controls.Add(chkRelativeLocation);
            grpEntitySpawn.Controls.Add(pnlSpawnLoc);
            grpEntitySpawn.Controls.Add(lblRelativeLocation);
            grpEntitySpawn.Controls.Add(cmbEntities);
            grpEntitySpawn.Controls.Add(lblEntity);
            grpEntitySpawn.ForeColor = System.Drawing.Color.Gainsboro;
            grpEntitySpawn.Location = new System.Drawing.Point(10, 93);
            grpEntitySpawn.Margin = new Padding(4, 3, 4, 3);
            grpEntitySpawn.Name = "grpEntitySpawn";
            grpEntitySpawn.Padding = new Padding(4, 3, 4, 3);
            grpEntitySpawn.Size = new Size(275, 324);
            grpEntitySpawn.TabIndex = 24;
            grpEntitySpawn.TabStop = false;
            grpEntitySpawn.Text = "On/Around Entity";
            // 
            // chkRotateDirection
            // 
            chkRotateDirection.AutoSize = true;
            chkRotateDirection.Location = new System.Drawing.Point(44, 298);
            chkRotateDirection.Margin = new Padding(4, 3, 4, 3);
            chkRotateDirection.Name = "chkRotateDirection";
            chkRotateDirection.Size = new Size(169, 19);
            chkRotateDirection.TabIndex = 31;
            chkRotateDirection.Text = "Rotate Relative to Direction";
            // 
            // chkRelativeLocation
            // 
            chkRelativeLocation.AutoSize = true;
            chkRelativeLocation.Location = new System.Drawing.Point(44, 272);
            chkRelativeLocation.Margin = new Padding(4, 3, 4, 3);
            chkRelativeLocation.Name = "chkRelativeLocation";
            chkRelativeLocation.Size = new Size(170, 19);
            chkRelativeLocation.TabIndex = 30;
            chkRelativeLocation.Text = "Spawn Relative to Direction";
            // 
            // pnlSpawnLoc
            // 
            pnlSpawnLoc.Location = new System.Drawing.Point(44, 80);
            pnlSpawnLoc.Margin = new Padding(4, 3, 4, 3);
            pnlSpawnLoc.Name = "pnlSpawnLoc";
            pnlSpawnLoc.Size = new Size(187, 185);
            pnlSpawnLoc.TabIndex = 29;
            pnlSpawnLoc.MouseDown += pnlSpawnLoc_MouseDown;
            // 
            // lblRelativeLocation
            // 
            lblRelativeLocation.AutoSize = true;
            lblRelativeLocation.Location = new System.Drawing.Point(43, 57);
            lblRelativeLocation.Margin = new Padding(4, 0, 4, 0);
            lblRelativeLocation.Name = "lblRelativeLocation";
            lblRelativeLocation.Size = new Size(100, 15);
            lblRelativeLocation.TabIndex = 28;
            lblRelativeLocation.Text = "Relative Location:";
            // 
            // cmbEntities
            // 
            cmbEntities.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbEntities.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbEntities.BorderStyle = ButtonBorderStyle.Solid;
            cmbEntities.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbEntities.DrawDropdownHoverOutline = false;
            cmbEntities.DrawFocusRectangle = false;
            cmbEntities.DrawMode = DrawMode.OwnerDrawFixed;
            cmbEntities.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEntities.FlatStyle = FlatStyle.Flat;
            cmbEntities.ForeColor = System.Drawing.Color.Gainsboro;
            cmbEntities.FormattingEnabled = true;
            cmbEntities.Location = new System.Drawing.Point(86, 22);
            cmbEntities.Margin = new Padding(4, 3, 4, 3);
            cmbEntities.Name = "cmbEntities";
            cmbEntities.Size = new Size(140, 24);
            cmbEntities.TabIndex = 27;
            cmbEntities.Text = null;
            cmbEntities.TextPadding = new Padding(2);
            // 
            // lblEntity
            // 
            lblEntity.AutoSize = true;
            lblEntity.Location = new System.Drawing.Point(43, 25);
            lblEntity.Margin = new Padding(4, 0, 4, 0);
            lblEntity.Name = "lblEntity";
            lblEntity.Size = new Size(40, 15);
            lblEntity.TabIndex = 22;
            lblEntity.Text = "Entity:";
            // 
            // chkInstanceToPlayer
            // 
            chkInstanceToPlayer.AutoSize = true;
            chkInstanceToPlayer.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            chkInstanceToPlayer.Location = new System.Drawing.Point(10, 425);
            chkInstanceToPlayer.Margin = new Padding(4, 3, 4, 3);
            chkInstanceToPlayer.Name = "chkInstanceToPlayer";
            chkInstanceToPlayer.Size = new Size(124, 19);
            chkInstanceToPlayer.TabIndex = 42;
            chkInstanceToPlayer.Text = "Instance to Player?";
            // 
            // EventCommandPlayAnimation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            Controls.Add(grpPlayAnimation);
            Margin = new Padding(4, 3, 4, 3);
            Name = "EventCommandPlayAnimation";
            Size = new Size(312, 492);
            grpPlayAnimation.ResumeLayout(false);
            grpPlayAnimation.PerformLayout();
            grpTileSpawn.ResumeLayout(false);
            grpTileSpawn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudWarpY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWarpX).EndInit();
            grpEntitySpawn.ResumeLayout(false);
            grpEntitySpawn.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpPlayAnimation;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkGroupBox grpTileSpawn;
        private DarkComboBox cmbConditionType;
        private Label lblSpawnType;
        private DarkButton btnVisual;
        private DarkComboBox cmbMap;
        private DarkComboBox cmbDirection;
        private Label lblDir;
        private Label lblY;
        private Label lblMap;
        private Label lblX;
        private DarkGroupBox grpEntitySpawn;
        private DarkCheckBox chkRelativeLocation;
        private Panel pnlSpawnLoc;
        private DarkComboBox cmbEntities;
        private Label lblEntity;
        private Label lblRelativeLocation;
        private DarkComboBox cmbAnimation;
        private Label lblAnimation;
        private DarkCheckBox chkRotateDirection;
        private DarkNumericUpDown nudWarpY;
        private DarkNumericUpDown nudWarpX;
        private DarkCheckBox chkInstanceToPlayer;
    }
}
