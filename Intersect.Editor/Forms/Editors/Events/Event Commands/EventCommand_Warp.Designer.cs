using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandWarp
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
            grpWarp = new DarkGroupBox();
            nudWarpY = new DarkNumericUpDown();
            nudWarpX = new DarkNumericUpDown();
            grpInstanceSettings = new DarkGroupBox();
            cmbInstanceType = new DarkComboBox();
            lblInstanceType = new Label();
            chkChangeInstance = new CheckBox();
            btnVisual = new DarkButton();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            cmbMap = new DarkComboBox();
            cmbDirection = new DarkComboBox();
            lblDir = new Label();
            lblY = new Label();
            lblMap = new Label();
            lblX = new Label();
            grpWarp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudWarpY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWarpX).BeginInit();
            grpInstanceSettings.SuspendLayout();
            SuspendLayout();
            // 
            // grpWarp
            // 
            grpWarp.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpWarp.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpWarp.Controls.Add(nudWarpY);
            grpWarp.Controls.Add(nudWarpX);
            grpWarp.Controls.Add(grpInstanceSettings);
            grpWarp.Controls.Add(chkChangeInstance);
            grpWarp.Controls.Add(btnVisual);
            grpWarp.Controls.Add(btnCancel);
            grpWarp.Controls.Add(btnSave);
            grpWarp.Controls.Add(cmbMap);
            grpWarp.Controls.Add(cmbDirection);
            grpWarp.Controls.Add(lblDir);
            grpWarp.Controls.Add(lblY);
            grpWarp.Controls.Add(lblMap);
            grpWarp.Controls.Add(lblX);
            grpWarp.ForeColor = System.Drawing.Color.Gainsboro;
            grpWarp.Location = new System.Drawing.Point(4, 3);
            grpWarp.Margin = new Padding(4, 3, 4, 3);
            grpWarp.Name = "grpWarp";
            grpWarp.Padding = new Padding(4, 3, 4, 3);
            grpWarp.Size = new Size(436, 232);
            grpWarp.TabIndex = 17;
            grpWarp.TabStop = false;
            grpWarp.Text = "Warp";
            // 
            // nudWarpY
            // 
            nudWarpY.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudWarpY.ForeColor = System.Drawing.Color.Gainsboro;
            nudWarpY.Location = new System.Drawing.Point(54, 82);
            nudWarpY.Name = "nudWarpY";
            nudWarpY.Size = new Size(140, 23);
            nudWarpY.TabIndex = 67;
            nudWarpY.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudWarpY.ValueChanged += nudWarpY_ValueChanged;
            // 
            // nudWarpX
            // 
            nudWarpX.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudWarpX.ForeColor = System.Drawing.Color.Gainsboro;
            nudWarpX.Location = new System.Drawing.Point(54, 51);
            nudWarpX.Name = "nudWarpX";
            nudWarpX.Size = new Size(140, 23);
            nudWarpX.TabIndex = 66;
            nudWarpX.Value = new decimal(new int[] { 0, 0, 0, 0 });
            nudWarpX.ValueChanged += nudWarpX_ValueChanged;
            // 
            // grpInstanceSettings
            // 
            grpInstanceSettings.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpInstanceSettings.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpInstanceSettings.Controls.Add(cmbInstanceType);
            grpInstanceSettings.Controls.Add(lblInstanceType);
            grpInstanceSettings.ForeColor = System.Drawing.Color.Gainsboro;
            grpInstanceSettings.Location = new System.Drawing.Point(208, 59);
            grpInstanceSettings.Margin = new Padding(4, 3, 4, 3);
            grpInstanceSettings.Name = "grpInstanceSettings";
            grpInstanceSettings.Padding = new Padding(4, 3, 4, 3);
            grpInstanceSettings.Size = new Size(214, 118);
            grpInstanceSettings.TabIndex = 65;
            grpInstanceSettings.TabStop = false;
            grpInstanceSettings.Text = "Instance Settings";
            grpInstanceSettings.Visible = false;
            // 
            // cmbInstanceType
            // 
            cmbInstanceType.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbInstanceType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbInstanceType.BorderStyle = ButtonBorderStyle.Solid;
            cmbInstanceType.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbInstanceType.DrawDropdownHoverOutline = false;
            cmbInstanceType.DrawFocusRectangle = false;
            cmbInstanceType.DrawMode = DrawMode.OwnerDrawFixed;
            cmbInstanceType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbInstanceType.FlatStyle = FlatStyle.Flat;
            cmbInstanceType.ForeColor = System.Drawing.Color.Gainsboro;
            cmbInstanceType.FormattingEnabled = true;
            cmbInstanceType.Location = new System.Drawing.Point(10, 54);
            cmbInstanceType.Margin = new Padding(4, 3, 4, 3);
            cmbInstanceType.Name = "cmbInstanceType";
            cmbInstanceType.Size = new Size(196, 24);
            cmbInstanceType.TabIndex = 64;
            cmbInstanceType.Text = null;
            cmbInstanceType.TextPadding = new Padding(2);
            // 
            // lblInstanceType
            // 
            lblInstanceType.AutoSize = true;
            lblInstanceType.Location = new System.Drawing.Point(7, 36);
            lblInstanceType.Margin = new Padding(4, 0, 4, 0);
            lblInstanceType.Name = "lblInstanceType";
            lblInstanceType.Size = new Size(81, 15);
            lblInstanceType.TabIndex = 64;
            lblInstanceType.Text = "Instance Type:";
            // 
            // chkChangeInstance
            // 
            chkChangeInstance.AutoSize = true;
            chkChangeInstance.Location = new System.Drawing.Point(208, 22);
            chkChangeInstance.Margin = new Padding(4, 3, 4, 3);
            chkChangeInstance.Name = "chkChangeInstance";
            chkChangeInstance.Size = new Size(119, 19);
            chkChangeInstance.TabIndex = 64;
            chkChangeInstance.Text = "Change instance?";
            chkChangeInstance.UseVisualStyleBackColor = true;
            chkChangeInstance.CheckedChanged += chkChangeInstance_CheckedChanged;
            // 
            // btnVisual
            // 
            btnVisual.Location = new System.Drawing.Point(14, 150);
            btnVisual.Margin = new Padding(4, 3, 4, 3);
            btnVisual.Name = "btnVisual";
            btnVisual.Padding = new Padding(6);
            btnVisual.Size = new Size(181, 27);
            btnVisual.TabIndex = 21;
            btnVisual.Text = "Open Visual Interface";
            btnVisual.Click += btnVisual_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(334, 194);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Padding = new Padding(6);
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 20;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(239, 194);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 19;
            btnSave.Text = "Ok";
            btnSave.Click += btnSave_Click;
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
            cmbMap.Location = new System.Drawing.Point(54, 18);
            cmbMap.Margin = new Padding(4, 3, 4, 3);
            cmbMap.Name = "cmbMap";
            cmbMap.Size = new Size(140, 24);
            cmbMap.TabIndex = 16;
            cmbMap.Text = null;
            cmbMap.TextPadding = new Padding(2);
            cmbMap.SelectedIndexChanged += cmbMap_SelectedIndexChanged;
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
            cmbDirection.Location = new System.Drawing.Point(54, 115);
            cmbDirection.Margin = new Padding(4, 3, 4, 3);
            cmbDirection.Name = "cmbDirection";
            cmbDirection.Size = new Size(140, 24);
            cmbDirection.TabIndex = 15;
            cmbDirection.Text = null;
            cmbDirection.TextPadding = new Padding(2);
            cmbDirection.SelectedIndexChanged += cmbDirection_SelectedIndexChanged;
            // 
            // lblDir
            // 
            lblDir.AutoSize = true;
            lblDir.Location = new System.Drawing.Point(10, 115);
            lblDir.Margin = new Padding(4, 0, 4, 0);
            lblDir.Name = "lblDir";
            lblDir.Size = new Size(25, 15);
            lblDir.TabIndex = 14;
            lblDir.Text = "Dir:";
            lblDir.Click += label23_Click;
            // 
            // lblY
            // 
            lblY.AutoSize = true;
            lblY.Location = new System.Drawing.Point(10, 84);
            lblY.Margin = new Padding(4, 0, 4, 0);
            lblY.Name = "lblY";
            lblY.Size = new Size(26, 15);
            lblY.TabIndex = 13;
            lblY.Text = "Y: 0";
            lblY.Click += lblY_Click;
            // 
            // lblMap
            // 
            lblMap.AutoSize = true;
            lblMap.Location = new System.Drawing.Point(10, 22);
            lblMap.Margin = new Padding(4, 0, 4, 0);
            lblMap.Name = "lblMap";
            lblMap.Size = new Size(34, 15);
            lblMap.TabIndex = 8;
            lblMap.Text = "Map:";
            lblMap.Click += label21_Click;
            // 
            // lblX
            // 
            lblX.AutoSize = true;
            lblX.Location = new System.Drawing.Point(10, 53);
            lblX.Margin = new Padding(4, 0, 4, 0);
            lblX.Name = "lblX";
            lblX.Size = new Size(26, 15);
            lblX.TabIndex = 12;
            lblX.Text = "X: 0";
            lblX.Click += lblX_Click;
            // 
            // EventCommandWarp
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            Controls.Add(grpWarp);
            Margin = new Padding(4, 3, 4, 3);
            Name = "EventCommandWarp";
            Size = new Size(447, 241);
            grpWarp.ResumeLayout(false);
            grpWarp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudWarpY).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWarpX).EndInit();
            grpInstanceSettings.ResumeLayout(false);
            grpInstanceSettings.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpWarp;
        private DarkComboBox cmbMap;
        private DarkComboBox cmbDirection;
        private System.Windows.Forms.Label lblDir;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblMap;
        private System.Windows.Forms.Label lblX;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkButton btnVisual;
        private DarkGroupBox grpInstanceSettings;
        private DarkComboBox cmbInstanceType;
        private System.Windows.Forms.Label lblInstanceType;
        private System.Windows.Forms.CheckBox chkChangeInstance;
        private DarkNumericUpDown nudWarpY;
        private DarkNumericUpDown nudWarpX;
    }
}
