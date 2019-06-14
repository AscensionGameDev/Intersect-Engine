using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors
{
    partial class FrmSwitchVariable
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSwitchVariable));
            this.grpTypes = new DarkUI.Controls.DarkGroupBox();
            this.rdoGlobalVariables = new DarkUI.Controls.DarkRadioButton();
            this.rdoPlayerVariables = new DarkUI.Controls.DarkRadioButton();
            this.grpList = new DarkUI.Controls.DarkGroupBox();
            this.btnUndo = new DarkUI.Controls.DarkButton();
            this.lstObjects = new System.Windows.Forms.ListBox();
            this.btnNew = new DarkUI.Controls.DarkButton();
            this.btnDelete = new DarkUI.Controls.DarkButton();
            this.grpEditor = new DarkUI.Controls.DarkGroupBox();
            this.grpValue = new DarkUI.Controls.DarkGroupBox();
            this.cmbBooleanValue = new DarkUI.Controls.DarkComboBox();
            this.nudVariableValue = new DarkUI.Controls.DarkNumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbVariableType = new DarkUI.Controls.DarkComboBox();
            this.txtObjectName = new DarkUI.Controls.DarkTextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblObject = new System.Windows.Forms.Label();
            this.txtId = new DarkUI.Controls.DarkTextBox();
            this.lblId = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpTypes.SuspendLayout();
            this.grpList.SuspendLayout();
            this.grpEditor.SuspendLayout();
            this.grpValue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudVariableValue)).BeginInit();
            this.SuspendLayout();
            // 
            // grpTypes
            // 
            this.grpTypes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpTypes.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpTypes.Controls.Add(this.rdoGlobalVariables);
            this.grpTypes.Controls.Add(this.rdoPlayerVariables);
            this.grpTypes.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpTypes.Location = new System.Drawing.Point(13, 13);
            this.grpTypes.Name = "grpTypes";
            this.grpTypes.Size = new System.Drawing.Size(490, 53);
            this.grpTypes.TabIndex = 0;
            this.grpTypes.TabStop = false;
            this.grpTypes.Text = "Variable Type";
            // 
            // rdoGlobalVariables
            // 
            this.rdoGlobalVariables.AutoSize = true;
            this.rdoGlobalVariables.Location = new System.Drawing.Point(124, 20);
            this.rdoGlobalVariables.Name = "rdoGlobalVariables";
            this.rdoGlobalVariables.Size = new System.Drawing.Size(101, 17);
            this.rdoGlobalVariables.TabIndex = 3;
            this.rdoGlobalVariables.Text = "Global Variables";
            this.rdoGlobalVariables.CheckedChanged += new System.EventHandler(this.rdoGlobalVariables_CheckedChanged);
            // 
            // rdoPlayerVariables
            // 
            this.rdoPlayerVariables.AutoSize = true;
            this.rdoPlayerVariables.Checked = true;
            this.rdoPlayerVariables.Location = new System.Drawing.Point(7, 20);
            this.rdoPlayerVariables.Name = "rdoPlayerVariables";
            this.rdoPlayerVariables.Size = new System.Drawing.Size(100, 17);
            this.rdoPlayerVariables.TabIndex = 1;
            this.rdoPlayerVariables.TabStop = true;
            this.rdoPlayerVariables.Text = "Player Variables";
            this.rdoPlayerVariables.CheckedChanged += new System.EventHandler(this.rdoPlayerVariables_CheckedChanged);
            // 
            // grpList
            // 
            this.grpList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpList.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpList.Controls.Add(this.btnUndo);
            this.grpList.Controls.Add(this.lstObjects);
            this.grpList.Controls.Add(this.btnNew);
            this.grpList.Controls.Add(this.btnDelete);
            this.grpList.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpList.Location = new System.Drawing.Point(13, 73);
            this.grpList.Name = "grpList";
            this.grpList.Size = new System.Drawing.Size(200, 469);
            this.grpList.TabIndex = 1;
            this.grpList.TabStop = false;
            this.grpList.Text = "Variable List";
            // 
            // btnUndo
            // 
            this.btnUndo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnUndo.Location = new System.Drawing.Point(7, 431);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Padding = new System.Windows.Forms.Padding(5);
            this.btnUndo.Size = new System.Drawing.Size(187, 27);
            this.btnUndo.TabIndex = 55;
            this.btnUndo.Text = "Undo Changes";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // lstObjects
            // 
            this.lstObjects.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstObjects.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstObjects.FormattingEnabled = true;
            this.lstObjects.Location = new System.Drawing.Point(7, 20);
            this.lstObjects.Name = "lstObjects";
            this.lstObjects.Size = new System.Drawing.Size(187, 327);
            this.lstObjects.TabIndex = 0;
            this.lstObjects.SelectedIndexChanged += new System.EventHandler(this.lstObjects_Click);
            // 
            // btnNew
            // 
            this.btnNew.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNew.Location = new System.Drawing.Point(7, 365);
            this.btnNew.Name = "btnNew";
            this.btnNew.Padding = new System.Windows.Forms.Padding(5);
            this.btnNew.Size = new System.Drawing.Size(187, 27);
            this.btnNew.TabIndex = 53;
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDelete.Location = new System.Drawing.Point(7, 398);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Padding = new System.Windows.Forms.Padding(5);
            this.btnDelete.Size = new System.Drawing.Size(187, 27);
            this.btnDelete.TabIndex = 52;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // grpEditor
            // 
            this.grpEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpEditor.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpEditor.Controls.Add(this.grpValue);
            this.grpEditor.Controls.Add(this.label1);
            this.grpEditor.Controls.Add(this.cmbVariableType);
            this.grpEditor.Controls.Add(this.txtObjectName);
            this.grpEditor.Controls.Add(this.lblName);
            this.grpEditor.Controls.Add(this.lblObject);
            this.grpEditor.Controls.Add(this.txtId);
            this.grpEditor.Controls.Add(this.lblId);
            this.grpEditor.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpEditor.Location = new System.Drawing.Point(219, 73);
            this.grpEditor.Name = "grpEditor";
            this.grpEditor.Size = new System.Drawing.Size(284, 436);
            this.grpEditor.TabIndex = 2;
            this.grpEditor.TabStop = false;
            this.grpEditor.Text = "Variable Editor";
            this.grpEditor.Visible = false;
            // 
            // grpValue
            // 
            this.grpValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.grpValue.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpValue.Controls.Add(this.cmbBooleanValue);
            this.grpValue.Controls.Add(this.nudVariableValue);
            this.grpValue.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpValue.Location = new System.Drawing.Point(13, 119);
            this.grpValue.Name = "grpValue";
            this.grpValue.Size = new System.Drawing.Size(265, 200);
            this.grpValue.TabIndex = 63;
            this.grpValue.TabStop = false;
            this.grpValue.Text = "Value";
            // 
            // cmbBooleanValue
            // 
            this.cmbBooleanValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbBooleanValue.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbBooleanValue.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbBooleanValue.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbBooleanValue.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbBooleanValue.ButtonIcon")));
            this.cmbBooleanValue.DrawDropdownHoverOutline = false;
            this.cmbBooleanValue.DrawFocusRectangle = false;
            this.cmbBooleanValue.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbBooleanValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBooleanValue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBooleanValue.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbBooleanValue.FormattingEnabled = true;
            this.cmbBooleanValue.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbBooleanValue.Location = new System.Drawing.Point(6, 19);
            this.cmbBooleanValue.Name = "cmbBooleanValue";
            this.cmbBooleanValue.Size = new System.Drawing.Size(252, 21);
            this.cmbBooleanValue.TabIndex = 5;
            this.cmbBooleanValue.Text = "False";
            this.cmbBooleanValue.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbBooleanValue.SelectedIndexChanged += new System.EventHandler(this.cmbBooleanValue_SelectedIndexChanged);
            // 
            // nudVariableValue
            // 
            this.nudVariableValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudVariableValue.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudVariableValue.Location = new System.Drawing.Point(6, 20);
            this.nudVariableValue.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudVariableValue.Name = "nudVariableValue";
            this.nudVariableValue.Size = new System.Drawing.Size(252, 20);
            this.nudVariableValue.TabIndex = 60;
            this.nudVariableValue.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudVariableValue.ValueChanged += new System.EventHandler(this.nudVariableValue_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 61;
            this.label1.Text = "Type:";
            // 
            // cmbVariableType
            // 
            this.cmbVariableType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbVariableType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbVariableType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbVariableType.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbVariableType.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbVariableType.ButtonIcon")));
            this.cmbVariableType.DrawDropdownHoverOutline = false;
            this.cmbVariableType.DrawFocusRectangle = false;
            this.cmbVariableType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbVariableType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariableType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbVariableType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbVariableType.FormattingEnabled = true;
            this.cmbVariableType.Items.AddRange(new object[] {
            "Integer"});
            this.cmbVariableType.Location = new System.Drawing.Point(85, 66);
            this.cmbVariableType.Name = "cmbVariableType";
            this.cmbVariableType.Size = new System.Drawing.Size(193, 21);
            this.cmbVariableType.TabIndex = 62;
            this.cmbVariableType.Text = "Integer";
            this.cmbVariableType.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbVariableType.SelectedIndexChanged += new System.EventHandler(this.cmbVariableType_SelectedIndexChanged);
            // 
            // txtObjectName
            // 
            this.txtObjectName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtObjectName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtObjectName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtObjectName.Location = new System.Drawing.Point(85, 40);
            this.txtObjectName.Name = "txtObjectName";
            this.txtObjectName.Size = new System.Drawing.Size(193, 20);
            this.txtObjectName.TabIndex = 2;
            this.txtObjectName.TextChanged += new System.EventHandler(this.txtObjectName_TextChanged);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(10, 43);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name:";
            // 
            // lblObject
            // 
            this.lblObject.AutoSize = true;
            this.lblObject.Location = new System.Drawing.Point(10, 20);
            this.lblObject.Name = "lblObject";
            this.lblObject.Size = new System.Drawing.Size(93, 13);
            this.lblObject.TabIndex = 0;
            this.lblObject.Text = "Player Variable #1";
            // 
            // txtId
            // 
            this.txtId.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtId.Location = new System.Drawing.Point(85, 93);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(193, 20);
            this.txtId.TabIndex = 8;
            this.txtId.TextChanged += new System.EventHandler(this.txtId_TextChanged);
            this.txtId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtId_KeyPress);
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(10, 94);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(69, 13);
            this.lblId.TabIndex = 7;
            this.lblId.Text = "Text Id:  \\pv ";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(371, 515);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(132, 27);
            this.btnCancel.TabIndex = 54;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(233, 515);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(132, 27);
            this.btnSave.TabIndex = 51;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmSwitchVariable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(515, 554);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.grpList);
            this.Controls.Add(this.grpTypes);
            this.Controls.Add(this.grpEditor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmSwitchVariable";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Variable Editor";
            this.grpTypes.ResumeLayout(false);
            this.grpTypes.PerformLayout();
            this.grpList.ResumeLayout(false);
            this.grpEditor.ResumeLayout(false);
            this.grpEditor.PerformLayout();
            this.grpValue.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudVariableValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpTypes;
        private DarkRadioButton rdoGlobalVariables;
        private DarkRadioButton rdoPlayerVariables;
        private DarkGroupBox grpList;
        private System.Windows.Forms.ListBox lstObjects;
        private DarkGroupBox grpEditor;
        private DarkComboBox cmbBooleanValue;
        private DarkTextBox txtObjectName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblObject;
        private DarkButton btnUndo;
        private DarkButton btnNew;
        private DarkButton btnDelete;
        private DarkButton btnSave;
        private DarkButton btnCancel;
        private DarkTextBox txtId;
        private System.Windows.Forms.Label lblId;
        private DarkNumericUpDown nudVariableValue;
        private System.Windows.Forms.Label label1;
        private DarkComboBox cmbVariableType;
        private DarkGroupBox grpValue;
    }
}
