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
            this.grpWarp = new DarkUI.Controls.DarkGroupBox();
            this.btnVisual = new DarkUI.Controls.DarkButton();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.scrlY = new DarkUI.Controls.DarkScrollBar();
            this.scrlX = new DarkUI.Controls.DarkScrollBar();
            this.cmbMap = new DarkUI.Controls.DarkComboBox();
            this.cmbDirection = new DarkUI.Controls.DarkComboBox();
            this.lblDir = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblMap = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.chkChangeInstance = new System.Windows.Forms.CheckBox();
            this.grpInstanceSettings = new DarkUI.Controls.DarkGroupBox();
            this.cmbInstanceType = new DarkUI.Controls.DarkComboBox();
            this.lblInstanceType = new System.Windows.Forms.Label();
            this.grpWarp.SuspendLayout();
            this.grpInstanceSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpWarp
            // 
            this.grpWarp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpWarp.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpWarp.Controls.Add(this.grpInstanceSettings);
            this.grpWarp.Controls.Add(this.chkChangeInstance);
            this.grpWarp.Controls.Add(this.btnVisual);
            this.grpWarp.Controls.Add(this.btnCancel);
            this.grpWarp.Controls.Add(this.btnSave);
            this.grpWarp.Controls.Add(this.scrlY);
            this.grpWarp.Controls.Add(this.scrlX);
            this.grpWarp.Controls.Add(this.cmbMap);
            this.grpWarp.Controls.Add(this.cmbDirection);
            this.grpWarp.Controls.Add(this.lblDir);
            this.grpWarp.Controls.Add(this.lblY);
            this.grpWarp.Controls.Add(this.lblMap);
            this.grpWarp.Controls.Add(this.lblX);
            this.grpWarp.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpWarp.Location = new System.Drawing.Point(3, 3);
            this.grpWarp.Name = "grpWarp";
            this.grpWarp.Size = new System.Drawing.Size(374, 201);
            this.grpWarp.TabIndex = 17;
            this.grpWarp.TabStop = false;
            this.grpWarp.Text = "Warp";
            // 
            // btnVisual
            // 
            this.btnVisual.Location = new System.Drawing.Point(12, 130);
            this.btnVisual.Name = "btnVisual";
            this.btnVisual.Padding = new System.Windows.Forms.Padding(5);
            this.btnVisual.Size = new System.Drawing.Size(155, 23);
            this.btnVisual.TabIndex = 21;
            this.btnVisual.Text = "Open Visual Interface";
            this.btnVisual.Click += new System.EventHandler(this.btnVisual_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(286, 168);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(205, 168);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // scrlY
            // 
            this.scrlY.Location = new System.Drawing.Point(46, 73);
            this.scrlY.Name = "scrlY";
            this.scrlY.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlY.Size = new System.Drawing.Size(121, 17);
            this.scrlY.TabIndex = 18;
            this.scrlY.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlY_Scroll);
            // 
            // scrlX
            // 
            this.scrlX.Location = new System.Drawing.Point(46, 46);
            this.scrlX.Name = "scrlX";
            this.scrlX.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlX.Size = new System.Drawing.Size(121, 17);
            this.scrlX.TabIndex = 17;
            this.scrlX.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlX_Scroll);
            // 
            // cmbMap
            // 
            this.cmbMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbMap.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMap.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMap.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbMap.DrawDropdownHoverOutline = false;
            this.cmbMap.DrawFocusRectangle = false;
            this.cmbMap.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMap.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbMap.FormattingEnabled = true;
            this.cmbMap.Location = new System.Drawing.Point(46, 16);
            this.cmbMap.Name = "cmbMap";
            this.cmbMap.Size = new System.Drawing.Size(121, 21);
            this.cmbMap.TabIndex = 16;
            this.cmbMap.Text = null;
            this.cmbMap.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbMap.SelectedIndexChanged += new System.EventHandler(this.cmbMap_SelectedIndexChanged);
            // 
            // cmbDirection
            // 
            this.cmbDirection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDirection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDirection.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDirection.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbDirection.DrawDropdownHoverOutline = false;
            this.cmbDirection.DrawFocusRectangle = false;
            this.cmbDirection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDirection.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbDirection.FormattingEnabled = true;
            this.cmbDirection.Location = new System.Drawing.Point(46, 97);
            this.cmbDirection.Name = "cmbDirection";
            this.cmbDirection.Size = new System.Drawing.Size(121, 21);
            this.cmbDirection.TabIndex = 15;
            this.cmbDirection.Text = null;
            this.cmbDirection.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbDirection.SelectedIndexChanged += new System.EventHandler(this.cmbDirection_SelectedIndexChanged);
            // 
            // lblDir
            // 
            this.lblDir.AutoSize = true;
            this.lblDir.Location = new System.Drawing.Point(9, 100);
            this.lblDir.Name = "lblDir";
            this.lblDir.Size = new System.Drawing.Size(23, 13);
            this.lblDir.TabIndex = 14;
            this.lblDir.Text = "Dir:";
            this.lblDir.Click += new System.EventHandler(this.label23_Click);
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(9, 73);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(26, 13);
            this.lblY.TabIndex = 13;
            this.lblY.Text = "Y: 0";
            this.lblY.Click += new System.EventHandler(this.lblY_Click);
            // 
            // lblMap
            // 
            this.lblMap.AutoSize = true;
            this.lblMap.Location = new System.Drawing.Point(9, 19);
            this.lblMap.Name = "lblMap";
            this.lblMap.Size = new System.Drawing.Size(31, 13);
            this.lblMap.TabIndex = 8;
            this.lblMap.Text = "Map:";
            this.lblMap.Click += new System.EventHandler(this.label21_Click);
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(9, 46);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(26, 13);
            this.lblX.TabIndex = 12;
            this.lblX.Text = "X: 0";
            this.lblX.Click += new System.EventHandler(this.lblX_Click);
            // 
            // chkChangeInstance
            // 
            this.chkChangeInstance.AutoSize = true;
            this.chkChangeInstance.Location = new System.Drawing.Point(178, 19);
            this.chkChangeInstance.Name = "chkChangeInstance";
            this.chkChangeInstance.Size = new System.Drawing.Size(112, 17);
            this.chkChangeInstance.TabIndex = 64;
            this.chkChangeInstance.Text = "Change instance?";
            this.chkChangeInstance.UseVisualStyleBackColor = true;
            this.chkChangeInstance.CheckedChanged += new System.EventHandler(this.chkChangeInstance_CheckedChanged);
            // 
            // grpInstanceSettings
            // 
            this.grpInstanceSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpInstanceSettings.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpInstanceSettings.Controls.Add(this.cmbInstanceType);
            this.grpInstanceSettings.Controls.Add(this.lblInstanceType);
            this.grpInstanceSettings.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpInstanceSettings.Location = new System.Drawing.Point(178, 51);
            this.grpInstanceSettings.Name = "grpInstanceSettings";
            this.grpInstanceSettings.Size = new System.Drawing.Size(183, 102);
            this.grpInstanceSettings.TabIndex = 65;
            this.grpInstanceSettings.TabStop = false;
            this.grpInstanceSettings.Text = "Instance Settings";
            this.grpInstanceSettings.Visible = false;
            // 
            // cmbInstanceType
            // 
            this.cmbInstanceType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbInstanceType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbInstanceType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbInstanceType.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbInstanceType.DrawDropdownHoverOutline = false;
            this.cmbInstanceType.DrawFocusRectangle = false;
            this.cmbInstanceType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbInstanceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstanceType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbInstanceType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbInstanceType.FormattingEnabled = true;
            this.cmbInstanceType.Location = new System.Drawing.Point(9, 47);
            this.cmbInstanceType.Name = "cmbInstanceType";
            this.cmbInstanceType.Size = new System.Drawing.Size(169, 21);
            this.cmbInstanceType.TabIndex = 64;
            this.cmbInstanceType.Text = null;
            this.cmbInstanceType.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblInstanceType
            // 
            this.lblInstanceType.AutoSize = true;
            this.lblInstanceType.Location = new System.Drawing.Point(6, 31);
            this.lblInstanceType.Name = "lblInstanceType";
            this.lblInstanceType.Size = new System.Drawing.Size(78, 13);
            this.lblInstanceType.TabIndex = 64;
            this.lblInstanceType.Text = "Instance Type:";
            // 
            // EventCommandWarp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpWarp);
            this.Name = "EventCommandWarp";
            this.Size = new System.Drawing.Size(383, 209);
            this.grpWarp.ResumeLayout(false);
            this.grpWarp.PerformLayout();
            this.grpInstanceSettings.ResumeLayout(false);
            this.grpInstanceSettings.PerformLayout();
            this.ResumeLayout(false);

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
        private DarkScrollBar scrlY;
        private DarkScrollBar scrlX;
        private DarkButton btnVisual;
        private DarkGroupBox grpInstanceSettings;
        private DarkComboBox cmbInstanceType;
        private System.Windows.Forms.Label lblInstanceType;
        private System.Windows.Forms.CheckBox chkChangeInstance;
    }
}
