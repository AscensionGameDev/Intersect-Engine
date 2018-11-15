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
            this.grpPlayAnimation = new DarkUI.Controls.DarkGroupBox();
            this.cmbAnimation = new DarkUI.Controls.DarkComboBox();
            this.lblAnimation = new System.Windows.Forms.Label();
            this.cmbConditionType = new DarkUI.Controls.DarkComboBox();
            this.lblSpawnType = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpTileSpawn = new DarkUI.Controls.DarkGroupBox();
            this.btnVisual = new DarkUI.Controls.DarkButton();
            this.cmbMap = new DarkUI.Controls.DarkComboBox();
            this.cmbDirection = new DarkUI.Controls.DarkComboBox();
            this.lblDir = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblMap = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.grpEntitySpawn = new DarkUI.Controls.DarkGroupBox();
            this.chkRotateDirection = new DarkUI.Controls.DarkCheckBox();
            this.chkRelativeLocation = new DarkUI.Controls.DarkCheckBox();
            this.pnlSpawnLoc = new System.Windows.Forms.Panel();
            this.lblRelativeLocation = new System.Windows.Forms.Label();
            this.cmbEntities = new DarkUI.Controls.DarkComboBox();
            this.lblEntity = new System.Windows.Forms.Label();
            this.nudWarpX = new DarkUI.Controls.DarkNumericUpDown();
            this.nudWarpY = new DarkUI.Controls.DarkNumericUpDown();
            this.grpPlayAnimation.SuspendLayout();
            this.grpTileSpawn.SuspendLayout();
            this.grpEntitySpawn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarpX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarpY)).BeginInit();
            this.SuspendLayout();
            // 
            // grpPlayAnimation
            // 
            this.grpPlayAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpPlayAnimation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpPlayAnimation.Controls.Add(this.cmbAnimation);
            this.grpPlayAnimation.Controls.Add(this.lblAnimation);
            this.grpPlayAnimation.Controls.Add(this.cmbConditionType);
            this.grpPlayAnimation.Controls.Add(this.lblSpawnType);
            this.grpPlayAnimation.Controls.Add(this.btnCancel);
            this.grpPlayAnimation.Controls.Add(this.btnSave);
            this.grpPlayAnimation.Controls.Add(this.grpTileSpawn);
            this.grpPlayAnimation.Controls.Add(this.grpEntitySpawn);
            this.grpPlayAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpPlayAnimation.Location = new System.Drawing.Point(3, 3);
            this.grpPlayAnimation.Name = "grpPlayAnimation";
            this.grpPlayAnimation.Size = new System.Drawing.Size(256, 407);
            this.grpPlayAnimation.TabIndex = 17;
            this.grpPlayAnimation.TabStop = false;
            this.grpPlayAnimation.Text = "Play Animation";
            // 
            // cmbAnimation
            // 
            this.cmbAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAnimation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAnimation.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAnimation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAnimation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAnimation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAnimation.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbAnimation.FormattingEnabled = true;
            this.cmbAnimation.Location = new System.Drawing.Point(88, 15);
            this.cmbAnimation.Name = "cmbAnimation";
            this.cmbAnimation.Size = new System.Drawing.Size(157, 21);
            this.cmbAnimation.TabIndex = 26;
            // 
            // lblAnimation
            // 
            this.lblAnimation.AutoSize = true;
            this.lblAnimation.Location = new System.Drawing.Point(6, 18);
            this.lblAnimation.Name = "lblAnimation";
            this.lblAnimation.Size = new System.Drawing.Size(56, 13);
            this.lblAnimation.TabIndex = 25;
            this.lblAnimation.Text = "Animation:";
            // 
            // cmbConditionType
            // 
            this.cmbConditionType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbConditionType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbConditionType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbConditionType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbConditionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConditionType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbConditionType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbConditionType.FormattingEnabled = true;
            this.cmbConditionType.Location = new System.Drawing.Point(88, 44);
            this.cmbConditionType.Name = "cmbConditionType";
            this.cmbConditionType.Size = new System.Drawing.Size(157, 21);
            this.cmbConditionType.TabIndex = 22;
            this.cmbConditionType.SelectedIndexChanged += new System.EventHandler(this.cmbConditionType_SelectedIndexChanged);
            // 
            // lblSpawnType
            // 
            this.lblSpawnType.AutoSize = true;
            this.lblSpawnType.Location = new System.Drawing.Point(6, 47);
            this.lblSpawnType.Name = "lblSpawnType";
            this.lblSpawnType.Size = new System.Drawing.Size(70, 13);
            this.lblSpawnType.TabIndex = 21;
            this.lblSpawnType.Text = "Spawn Type:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(90, 378);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(9, 378);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpTileSpawn
            // 
            this.grpTileSpawn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpTileSpawn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpTileSpawn.Controls.Add(this.nudWarpY);
            this.grpTileSpawn.Controls.Add(this.nudWarpX);
            this.grpTileSpawn.Controls.Add(this.btnVisual);
            this.grpTileSpawn.Controls.Add(this.cmbMap);
            this.grpTileSpawn.Controls.Add(this.cmbDirection);
            this.grpTileSpawn.Controls.Add(this.lblDir);
            this.grpTileSpawn.Controls.Add(this.lblY);
            this.grpTileSpawn.Controls.Add(this.lblMap);
            this.grpTileSpawn.Controls.Add(this.lblX);
            this.grpTileSpawn.ForeColor = System.Drawing.Color.Gainsboro;
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
            this.btnVisual.Padding = new System.Windows.Forms.Padding(5);
            this.btnVisual.Size = new System.Drawing.Size(155, 23);
            this.btnVisual.TabIndex = 30;
            this.btnVisual.Text = "Open Visual Interface";
            this.btnVisual.Click += new System.EventHandler(this.btnVisual_Click);
            // 
            // cmbMap
            // 
            this.cmbMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbMap.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMap.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMap.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMap.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbMap.FormattingEnabled = true;
            this.cmbMap.Location = new System.Drawing.Point(74, 19);
            this.cmbMap.Name = "cmbMap";
            this.cmbMap.Size = new System.Drawing.Size(121, 21);
            this.cmbMap.TabIndex = 27;
            // 
            // cmbDirection
            // 
            this.cmbDirection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDirection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDirection.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDirection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDirection.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbDirection.FormattingEnabled = true;
            this.cmbDirection.Location = new System.Drawing.Point(74, 100);
            this.cmbDirection.Name = "cmbDirection";
            this.cmbDirection.Size = new System.Drawing.Size(121, 21);
            this.cmbDirection.TabIndex = 26;
            // 
            // lblDir
            // 
            this.lblDir.AutoSize = true;
            this.lblDir.Location = new System.Drawing.Point(37, 103);
            this.lblDir.Name = "lblDir";
            this.lblDir.Size = new System.Drawing.Size(23, 13);
            this.lblDir.TabIndex = 25;
            this.lblDir.Text = "Dir:";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(37, 76);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(20, 13);
            this.lblY.TabIndex = 24;
            this.lblY.Text = "Y: ";
            // 
            // lblMap
            // 
            this.lblMap.AutoSize = true;
            this.lblMap.Location = new System.Drawing.Point(37, 22);
            this.lblMap.Name = "lblMap";
            this.lblMap.Size = new System.Drawing.Size(31, 13);
            this.lblMap.TabIndex = 22;
            this.lblMap.Text = "Map:";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(37, 49);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(20, 13);
            this.lblX.TabIndex = 23;
            this.lblX.Text = "X: ";
            // 
            // grpEntitySpawn
            // 
            this.grpEntitySpawn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpEntitySpawn.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEntitySpawn.Controls.Add(this.chkRotateDirection);
            this.grpEntitySpawn.Controls.Add(this.chkRelativeLocation);
            this.grpEntitySpawn.Controls.Add(this.pnlSpawnLoc);
            this.grpEntitySpawn.Controls.Add(this.lblRelativeLocation);
            this.grpEntitySpawn.Controls.Add(this.cmbEntities);
            this.grpEntitySpawn.Controls.Add(this.lblEntity);
            this.grpEntitySpawn.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEntitySpawn.Location = new System.Drawing.Point(9, 81);
            this.grpEntitySpawn.Name = "grpEntitySpawn";
            this.grpEntitySpawn.Size = new System.Drawing.Size(236, 281);
            this.grpEntitySpawn.TabIndex = 24;
            this.grpEntitySpawn.TabStop = false;
            this.grpEntitySpawn.Text = "On/Around Entity";
            // 
            // chkRotateDirection
            // 
            this.chkRotateDirection.AutoSize = true;
            this.chkRotateDirection.Location = new System.Drawing.Point(38, 258);
            this.chkRotateDirection.Name = "chkRotateDirection";
            this.chkRotateDirection.Size = new System.Drawing.Size(157, 17);
            this.chkRotateDirection.TabIndex = 31;
            this.chkRotateDirection.Text = "Rotate Relative to Direction";
            // 
            // chkRelativeLocation
            // 
            this.chkRelativeLocation.AutoSize = true;
            this.chkRelativeLocation.Location = new System.Drawing.Point(38, 236);
            this.chkRelativeLocation.Name = "chkRelativeLocation";
            this.chkRelativeLocation.Size = new System.Drawing.Size(158, 17);
            this.chkRelativeLocation.TabIndex = 30;
            this.chkRelativeLocation.Text = "Spawn Relative to Direction";
            // 
            // pnlSpawnLoc
            // 
            this.pnlSpawnLoc.Location = new System.Drawing.Point(38, 69);
            this.pnlSpawnLoc.Name = "pnlSpawnLoc";
            this.pnlSpawnLoc.Size = new System.Drawing.Size(160, 160);
            this.pnlSpawnLoc.TabIndex = 29;
            this.pnlSpawnLoc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlSpawnLoc_MouseDown);
            // 
            // lblRelativeLocation
            // 
            this.lblRelativeLocation.AutoSize = true;
            this.lblRelativeLocation.Location = new System.Drawing.Point(37, 49);
            this.lblRelativeLocation.Name = "lblRelativeLocation";
            this.lblRelativeLocation.Size = new System.Drawing.Size(93, 13);
            this.lblRelativeLocation.TabIndex = 28;
            this.lblRelativeLocation.Text = "Relative Location:";
            // 
            // cmbEntities
            // 
            this.cmbEntities.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEntities.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbEntities.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbEntities.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEntities.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntities.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEntities.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbEntities.FormattingEnabled = true;
            this.cmbEntities.Location = new System.Drawing.Point(74, 19);
            this.cmbEntities.Name = "cmbEntities";
            this.cmbEntities.Size = new System.Drawing.Size(121, 21);
            this.cmbEntities.TabIndex = 27;
            // 
            // lblEntity
            // 
            this.lblEntity.AutoSize = true;
            this.lblEntity.Location = new System.Drawing.Point(37, 22);
            this.lblEntity.Name = "lblEntity";
            this.lblEntity.Size = new System.Drawing.Size(36, 13);
            this.lblEntity.TabIndex = 22;
            this.lblEntity.Text = "Entity:";
            // 
            // nudWarpX
            // 
            this.nudWarpX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudWarpX.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudWarpX.Location = new System.Drawing.Point(74, 47);
            this.nudWarpX.Name = "nudWarpX";
            this.nudWarpX.Size = new System.Drawing.Size(121, 20);
            this.nudWarpX.TabIndex = 31;
            // 
            // nudWarpY
            // 
            this.nudWarpY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudWarpY.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudWarpY.Location = new System.Drawing.Point(74, 74);
            this.nudWarpY.Name = "nudWarpY";
            this.nudWarpY.Size = new System.Drawing.Size(120, 20);
            this.nudWarpY.TabIndex = 32;
            // 
            // EventCommand_PlayAnimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpPlayAnimation);
            this.Name = "EventCommandPlayAnimation";
            this.Size = new System.Drawing.Size(267, 413);
            this.grpPlayAnimation.ResumeLayout(false);
            this.grpPlayAnimation.PerformLayout();
            this.grpTileSpawn.ResumeLayout(false);
            this.grpTileSpawn.PerformLayout();
            this.grpEntitySpawn.ResumeLayout(false);
            this.grpEntitySpawn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarpX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarpY)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpPlayAnimation;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkGroupBox grpTileSpawn;
        private DarkComboBox cmbConditionType;
        private System.Windows.Forms.Label lblSpawnType;
        private DarkButton btnVisual;
        private DarkComboBox cmbMap;
        private DarkComboBox cmbDirection;
        private System.Windows.Forms.Label lblDir;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblMap;
        private System.Windows.Forms.Label lblX;
        private DarkGroupBox grpEntitySpawn;
        private DarkCheckBox chkRelativeLocation;
        private System.Windows.Forms.Panel pnlSpawnLoc;
        private DarkComboBox cmbEntities;
        private System.Windows.Forms.Label lblEntity;
        private System.Windows.Forms.Label lblRelativeLocation;
        private DarkComboBox cmbAnimation;
        private System.Windows.Forms.Label lblAnimation;
        private DarkCheckBox chkRotateDirection;
        private DarkNumericUpDown nudWarpY;
        private DarkNumericUpDown nudWarpX;
    }
}
