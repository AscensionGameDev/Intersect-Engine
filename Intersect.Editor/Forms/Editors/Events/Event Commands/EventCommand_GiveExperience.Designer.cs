using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandGiveExperience
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
            grpGiveExperience = new DarkGroupBox();
            chkEnableLevelDown = new DarkCheckBox();
            grpVariableAmount = new DarkGroupBox();
            cmbVariable = new DarkComboBox();
            lblVariable = new Label();
            rdoGlobalVariable = new DarkRadioButton();
            rdoGuildVariable = new DarkRadioButton();
            rdoPlayerVariable = new DarkRadioButton();
            grpAmountType = new DarkGroupBox();
            rdoVariable = new DarkRadioButton();
            rdoManual = new DarkRadioButton();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            grpManualAmount = new DarkGroupBox();
            nudExperience = new DarkNumericUpDown();
            lblExperience = new Label();
            grpGiveExperience.SuspendLayout();
            grpVariableAmount.SuspendLayout();
            grpAmountType.SuspendLayout();
            grpManualAmount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudExperience).BeginInit();
            SuspendLayout();
            // 
            // grpGiveExperience
            // 
            grpGiveExperience.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpGiveExperience.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpGiveExperience.Controls.Add(chkEnableLevelDown);
            grpGiveExperience.Controls.Add(grpVariableAmount);
            grpGiveExperience.Controls.Add(grpAmountType);
            grpGiveExperience.Controls.Add(btnCancel);
            grpGiveExperience.Controls.Add(btnSave);
            grpGiveExperience.Controls.Add(grpManualAmount);
            grpGiveExperience.ForeColor = System.Drawing.Color.Gainsboro;
            grpGiveExperience.Location = new System.Drawing.Point(4, 3);
            grpGiveExperience.Margin = new Padding(4, 3, 4, 3);
            grpGiveExperience.Name = "grpGiveExperience";
            grpGiveExperience.Padding = new Padding(4, 3, 4, 3);
            grpGiveExperience.Size = new Size(498, 181);
            grpGiveExperience.TabIndex = 17;
            grpGiveExperience.TabStop = false;
            grpGiveExperience.Text = "Give Experience:";
            // 
            // chkEnableLevelDown
            // 
            chkEnableLevelDown.AutoSize = true;
            chkEnableLevelDown.Location = new System.Drawing.Point(356, 110);
            chkEnableLevelDown.Margin = new Padding(4, 3, 4, 3);
            chkEnableLevelDown.Name = "chkEnableLevelDown";
            chkEnableLevelDown.Size = new Size(130, 19);
            chkEnableLevelDown.TabIndex = 41;
            chkEnableLevelDown.Text = "Enable Level Down?";
            // 
            // grpVariableAmount
            // 
            grpVariableAmount.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpVariableAmount.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpVariableAmount.Controls.Add(cmbVariable);
            grpVariableAmount.Controls.Add(lblVariable);
            grpVariableAmount.Controls.Add(rdoGlobalVariable);
            grpVariableAmount.Controls.Add(rdoGuildVariable);
            grpVariableAmount.Controls.Add(rdoPlayerVariable);
            grpVariableAmount.ForeColor = System.Drawing.Color.Gainsboro;
            grpVariableAmount.Location = new System.Drawing.Point(7, 22);
            grpVariableAmount.Margin = new Padding(4, 3, 4, 3);
            grpVariableAmount.Name = "grpVariableAmount";
            grpVariableAmount.Padding = new Padding(4, 3, 4, 3);
            grpVariableAmount.Size = new Size(341, 119);
            grpVariableAmount.TabIndex = 39;
            grpVariableAmount.TabStop = false;
            grpVariableAmount.Text = "Variable";
            grpVariableAmount.Visible = false;
            // 
            // cmbVariable
            // 
            cmbVariable.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbVariable.BorderStyle = ButtonBorderStyle.Solid;
            cmbVariable.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbVariable.DrawDropdownHoverOutline = false;
            cmbVariable.DrawFocusRectangle = false;
            cmbVariable.DrawMode = DrawMode.OwnerDrawFixed;
            cmbVariable.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVariable.FlatStyle = FlatStyle.Flat;
            cmbVariable.ForeColor = System.Drawing.Color.Gainsboro;
            cmbVariable.FormattingEnabled = true;
            cmbVariable.Location = new System.Drawing.Point(78, 83);
            cmbVariable.Margin = new Padding(4, 3, 4, 3);
            cmbVariable.Name = "cmbVariable";
            cmbVariable.Size = new Size(255, 24);
            cmbVariable.TabIndex = 39;
            cmbVariable.Text = null;
            cmbVariable.TextPadding = new Padding(2);
            // 
            // lblVariable
            // 
            lblVariable.AutoSize = true;
            lblVariable.Location = new System.Drawing.Point(9, 85);
            lblVariable.Margin = new Padding(4, 0, 4, 0);
            lblVariable.Name = "lblVariable";
            lblVariable.Size = new Size(48, 15);
            lblVariable.TabIndex = 38;
            lblVariable.Text = "Variable";
            // 
            // rdoGlobalVariable
            // 
            rdoGlobalVariable.AutoSize = true;
            rdoGlobalVariable.Location = new System.Drawing.Point(192, 22);
            rdoGlobalVariable.Margin = new Padding(4, 3, 4, 3);
            rdoGlobalVariable.Name = "rdoGlobalVariable";
            rdoGlobalVariable.Size = new Size(103, 19);
            rdoGlobalVariable.TabIndex = 37;
            rdoGlobalVariable.Text = "Global Variable";
            rdoGlobalVariable.CheckedChanged += SetupAmountInput;
            // 
            // rdoGuildVariable
            // 
            rdoGuildVariable.AutoSize = true;
            rdoGuildVariable.Location = new System.Drawing.Point(7, 48);
            rdoGuildVariable.Margin = new Padding(4, 3, 4, 3);
            rdoGuildVariable.Name = "rdoGuildVariable";
            rdoGuildVariable.Size = new Size(97, 19);
            rdoGuildVariable.TabIndex = 37;
            rdoGuildVariable.Text = "Guild Variable";
            rdoGuildVariable.CheckedChanged += SetupAmountInput;
            // 
            // rdoPlayerVariable
            // 
            rdoPlayerVariable.AutoSize = true;
            rdoPlayerVariable.Checked = true;
            rdoPlayerVariable.Location = new System.Drawing.Point(7, 22);
            rdoPlayerVariable.Margin = new Padding(4, 3, 4, 3);
            rdoPlayerVariable.Name = "rdoPlayerVariable";
            rdoPlayerVariable.Size = new Size(101, 19);
            rdoPlayerVariable.TabIndex = 36;
            rdoPlayerVariable.TabStop = true;
            rdoPlayerVariable.Text = "Player Variable";
            rdoPlayerVariable.CheckedChanged += SetupAmountInput;
            // 
            // grpAmountType
            // 
            grpAmountType.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpAmountType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpAmountType.Controls.Add(rdoVariable);
            grpAmountType.Controls.Add(rdoManual);
            grpAmountType.ForeColor = System.Drawing.Color.Gainsboro;
            grpAmountType.Location = new System.Drawing.Point(356, 22);
            grpAmountType.Margin = new Padding(4, 3, 4, 3);
            grpAmountType.Name = "grpAmountType";
            grpAmountType.Padding = new Padding(4, 3, 4, 3);
            grpAmountType.Size = new Size(134, 82);
            grpAmountType.TabIndex = 37;
            grpAmountType.TabStop = false;
            grpAmountType.Text = "Amount Type:";
            // 
            // rdoVariable
            // 
            rdoVariable.AutoSize = true;
            rdoVariable.Location = new System.Drawing.Point(10, 48);
            rdoVariable.Margin = new Padding(4, 3, 4, 3);
            rdoVariable.Name = "rdoVariable";
            rdoVariable.Size = new Size(66, 19);
            rdoVariable.TabIndex = 36;
            rdoVariable.Text = "Variable";
            rdoVariable.CheckedChanged += SetupAmountInput;
            // 
            // rdoManual
            // 
            rdoManual.AutoSize = true;
            rdoManual.Checked = true;
            rdoManual.Location = new System.Drawing.Point(10, 22);
            rdoManual.Margin = new Padding(4, 3, 4, 3);
            rdoManual.Name = "rdoManual";
            rdoManual.Size = new Size(65, 19);
            rdoManual.TabIndex = 35;
            rdoManual.TabStop = true;
            rdoManual.Text = "Manual";
            rdoManual.CheckedChanged += SetupAmountInput;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(260, 148);
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
            btnSave.Location = new System.Drawing.Point(7, 148);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 19;
            btnSave.Text = "Ok";
            btnSave.Click += btnSave_Click;
            // 
            // grpManualAmount
            // 
            grpManualAmount.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpManualAmount.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpManualAmount.Controls.Add(nudExperience);
            grpManualAmount.Controls.Add(lblExperience);
            grpManualAmount.ForeColor = System.Drawing.Color.Gainsboro;
            grpManualAmount.Location = new System.Drawing.Point(7, 22);
            grpManualAmount.Margin = new Padding(4, 3, 4, 3);
            grpManualAmount.Name = "grpManualAmount";
            grpManualAmount.Padding = new Padding(4, 3, 4, 3);
            grpManualAmount.Size = new Size(341, 82);
            grpManualAmount.TabIndex = 40;
            grpManualAmount.TabStop = false;
            grpManualAmount.Text = "Manual";
            grpManualAmount.Visible = false;
            // 
            // nudExperience
            // 
            nudExperience.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudExperience.ForeColor = System.Drawing.Color.Gainsboro;
            nudExperience.Location = new System.Drawing.Point(152, 29);
            nudExperience.Margin = new Padding(4, 3, 4, 3);
            nudExperience.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            nudExperience.Name = "nudExperience";
            nudExperience.Size = new Size(164, 23);
            nudExperience.TabIndex = 24;
            nudExperience.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblExperience
            // 
            lblExperience.AutoSize = true;
            lblExperience.Location = new System.Drawing.Point(26, 31);
            lblExperience.Margin = new Padding(4, 0, 4, 0);
            lblExperience.Name = "lblExperience";
            lblExperience.Size = new Size(96, 15);
            lblExperience.TabIndex = 23;
            lblExperience.Text = "Give Experience: ";
            // 
            // EventCommandGiveExperience
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            Controls.Add(grpGiveExperience);
            Margin = new Padding(4, 3, 4, 3);
            Name = "EventCommandGiveExperience";
            Size = new Size(509, 188);
            grpGiveExperience.ResumeLayout(false);
            grpGiveExperience.PerformLayout();
            grpVariableAmount.ResumeLayout(false);
            grpVariableAmount.PerformLayout();
            grpAmountType.ResumeLayout(false);
            grpAmountType.PerformLayout();
            grpManualAmount.ResumeLayout(false);
            grpManualAmount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudExperience).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpGiveExperience;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkGroupBox grpAmountType;
        private DarkRadioButton rdoVariable;
        private DarkRadioButton rdoManual;
        private DarkGroupBox grpManualAmount;
        private DarkNumericUpDown nudExperience;
        private System.Windows.Forms.Label lblExperience;
        private DarkGroupBox grpVariableAmount;
        private DarkComboBox cmbVariable;
        private System.Windows.Forms.Label lblVariable;
        private DarkRadioButton rdoGlobalVariable;
        private DarkRadioButton rdoGuildVariable;
        private DarkRadioButton rdoPlayerVariable;
        private DarkCheckBox chkEnableLevelDown;
    }
}
