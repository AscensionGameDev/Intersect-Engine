using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandChangeItems
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
            grpChangeItems = new DarkGroupBox();
            grpVariableAmount = new DarkGroupBox();
            cmbVariable = new DarkComboBox();
            lblVariable = new Label();
            rdoGlobalVariable = new DarkRadioButton();
            rdoGuildVariable = new DarkRadioButton();
            rdoPlayerVariable = new DarkRadioButton();
            grpManualAmount = new DarkGroupBox();
            nudGiveTakeAmount = new DarkNumericUpDown();
            lblAmount = new Label();
            grpAmountType = new DarkGroupBox();
            rdoVariable = new DarkRadioButton();
            rdoManual = new DarkRadioButton();
            cmbMethod = new DarkComboBox();
            lblMethod = new Label();
            cmbItem = new DarkComboBox();
            lblItem = new Label();
            cmbAction = new DarkComboBox();
            lblAction = new Label();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            grpChangeItems.SuspendLayout();
            grpVariableAmount.SuspendLayout();
            grpManualAmount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudGiveTakeAmount).BeginInit();
            grpAmountType.SuspendLayout();
            SuspendLayout();
            // 
            // grpChangeItems
            // 
            grpChangeItems.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpChangeItems.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpChangeItems.Controls.Add(grpVariableAmount);
            grpChangeItems.Controls.Add(grpManualAmount);
            grpChangeItems.Controls.Add(grpAmountType);
            grpChangeItems.Controls.Add(cmbMethod);
            grpChangeItems.Controls.Add(lblMethod);
            grpChangeItems.Controls.Add(cmbItem);
            grpChangeItems.Controls.Add(lblItem);
            grpChangeItems.Controls.Add(cmbAction);
            grpChangeItems.Controls.Add(lblAction);
            grpChangeItems.Controls.Add(btnCancel);
            grpChangeItems.Controls.Add(btnSave);
            grpChangeItems.ForeColor = System.Drawing.Color.Gainsboro;
            grpChangeItems.Location = new System.Drawing.Point(4, 3);
            grpChangeItems.Margin = new Padding(4, 3, 4, 3);
            grpChangeItems.Name = "grpChangeItems";
            grpChangeItems.Padding = new Padding(4, 3, 4, 3);
            grpChangeItems.Size = new Size(392, 240);
            grpChangeItems.TabIndex = 17;
            grpChangeItems.TabStop = false;
            grpChangeItems.Text = "Change Player Items:";
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
            grpVariableAmount.Location = new System.Drawing.Point(9, 115);
            grpVariableAmount.Margin = new Padding(4, 3, 4, 3);
            grpVariableAmount.Name = "grpVariableAmount";
            grpVariableAmount.Padding = new Padding(4, 3, 4, 3);
            grpVariableAmount.Size = new Size(373, 82);
            grpVariableAmount.TabIndex = 38;
            grpVariableAmount.TabStop = false;
            grpVariableAmount.Text = "Variable Amount:";
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
            cmbVariable.Location = new System.Drawing.Point(78, 51);
            cmbVariable.Margin = new Padding(4, 3, 4, 3);
            cmbVariable.Name = "cmbVariable";
            cmbVariable.Size = new Size(288, 24);
            cmbVariable.TabIndex = 39;
            cmbVariable.Text = null;
            cmbVariable.TextPadding = new Padding(2);
            // 
            // lblVariable
            // 
            lblVariable.AutoSize = true;
            lblVariable.Location = new System.Drawing.Point(9, 53);
            lblVariable.Margin = new Padding(4, 0, 4, 0);
            lblVariable.Name = "lblVariable";
            lblVariable.Size = new Size(48, 15);
            lblVariable.TabIndex = 38;
            lblVariable.Text = "Variable";
            // 
            // rdoGlobalVariable
            // 
            rdoGlobalVariable.AutoSize = true;
            rdoGlobalVariable.Location = new System.Drawing.Point(131, 22);
            rdoGlobalVariable.Margin = new Padding(4, 3, 4, 3);
            rdoGlobalVariable.Name = "rdoGlobalVariable";
            rdoGlobalVariable.Size = new Size(103, 19);
            rdoGlobalVariable.TabIndex = 37;
            rdoGlobalVariable.Text = "Global Variable";
            rdoGlobalVariable.CheckedChanged += rdoGlobalVariable_CheckedChanged;
            // 
            // rdoGuildVariable
            // 
            rdoGuildVariable.AutoSize = true;
            rdoGuildVariable.Location = new System.Drawing.Point(261, 22);
            rdoGuildVariable.Margin = new Padding(4, 3, 4, 3);
            rdoGuildVariable.Name = "rdoGuildVariable";
            rdoGuildVariable.Size = new Size(97, 19);
            rdoGuildVariable.TabIndex = 37;
            rdoGuildVariable.Text = "Guild Variable";
            rdoGuildVariable.CheckedChanged += rdoGuildVariable_CheckedChanged;
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
            rdoPlayerVariable.CheckedChanged += rdoPlayerVariable_CheckedChanged;
            // 
            // grpManualAmount
            // 
            grpManualAmount.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpManualAmount.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpManualAmount.Controls.Add(nudGiveTakeAmount);
            grpManualAmount.Controls.Add(lblAmount);
            grpManualAmount.ForeColor = System.Drawing.Color.Gainsboro;
            grpManualAmount.Location = new System.Drawing.Point(9, 115);
            grpManualAmount.Margin = new Padding(4, 3, 4, 3);
            grpManualAmount.Name = "grpManualAmount";
            grpManualAmount.Padding = new Padding(4, 3, 4, 3);
            grpManualAmount.Size = new Size(341, 82);
            grpManualAmount.TabIndex = 37;
            grpManualAmount.TabStop = false;
            grpManualAmount.Text = "Manual Amount:";
            // 
            // nudGiveTakeAmount
            // 
            nudGiveTakeAmount.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudGiveTakeAmount.ForeColor = System.Drawing.Color.Gainsboro;
            nudGiveTakeAmount.Location = new System.Drawing.Point(78, 37);
            nudGiveTakeAmount.Margin = new Padding(4, 3, 4, 3);
            nudGiveTakeAmount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            nudGiveTakeAmount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudGiveTakeAmount.Name = "nudGiveTakeAmount";
            nudGiveTakeAmount.Size = new Size(134, 23);
            nudGiveTakeAmount.TabIndex = 28;
            nudGiveTakeAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblAmount
            // 
            lblAmount.AutoSize = true;
            lblAmount.Location = new System.Drawing.Point(9, 37);
            lblAmount.Margin = new Padding(4, 0, 4, 0);
            lblAmount.Name = "lblAmount";
            lblAmount.Size = new Size(54, 15);
            lblAmount.TabIndex = 27;
            lblAmount.Text = "Amount:";
            // 
            // grpAmountType
            // 
            grpAmountType.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpAmountType.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpAmountType.Controls.Add(rdoVariable);
            grpAmountType.Controls.Add(rdoManual);
            grpAmountType.ForeColor = System.Drawing.Color.Gainsboro;
            grpAmountType.Location = new System.Drawing.Point(248, 22);
            grpAmountType.Margin = new Padding(4, 3, 4, 3);
            grpAmountType.Name = "grpAmountType";
            grpAmountType.Padding = new Padding(4, 3, 4, 3);
            grpAmountType.Size = new Size(134, 87);
            grpAmountType.TabIndex = 36;
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
            rdoVariable.CheckedChanged += rdoVariable_CheckedChanged;
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
            rdoManual.CheckedChanged += rdoManual_CheckedChanged;
            // 
            // cmbMethod
            // 
            cmbMethod.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbMethod.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbMethod.BorderStyle = ButtonBorderStyle.Solid;
            cmbMethod.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbMethod.DrawDropdownHoverOutline = false;
            cmbMethod.DrawFocusRectangle = false;
            cmbMethod.DrawMode = DrawMode.OwnerDrawFixed;
            cmbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMethod.FlatStyle = FlatStyle.Flat;
            cmbMethod.ForeColor = System.Drawing.Color.Gainsboro;
            cmbMethod.FormattingEnabled = true;
            cmbMethod.Items.AddRange(new object[] { "Normal", "Allow Overflow", "Up to Amount" });
            cmbMethod.Location = new System.Drawing.Point(75, 84);
            cmbMethod.Margin = new Padding(4, 3, 4, 3);
            cmbMethod.Name = "cmbMethod";
            cmbMethod.Size = new Size(166, 24);
            cmbMethod.TabIndex = 28;
            cmbMethod.Text = "Normal";
            cmbMethod.TextPadding = new Padding(2);
            // 
            // lblMethod
            // 
            lblMethod.AutoSize = true;
            lblMethod.Location = new System.Drawing.Point(6, 88);
            lblMethod.Margin = new Padding(4, 0, 4, 0);
            lblMethod.Name = "lblMethod";
            lblMethod.Size = new Size(52, 15);
            lblMethod.TabIndex = 27;
            lblMethod.Text = "Method:";
            // 
            // cmbItem
            // 
            cmbItem.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbItem.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbItem.BorderStyle = ButtonBorderStyle.Solid;
            cmbItem.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbItem.DrawDropdownHoverOutline = false;
            cmbItem.DrawFocusRectangle = false;
            cmbItem.DrawMode = DrawMode.OwnerDrawFixed;
            cmbItem.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbItem.FlatStyle = FlatStyle.Flat;
            cmbItem.ForeColor = System.Drawing.Color.Gainsboro;
            cmbItem.FormattingEnabled = true;
            cmbItem.Location = new System.Drawing.Point(75, 53);
            cmbItem.Margin = new Padding(4, 3, 4, 3);
            cmbItem.Name = "cmbItem";
            cmbItem.Size = new Size(166, 24);
            cmbItem.TabIndex = 24;
            cmbItem.Text = null;
            cmbItem.TextPadding = new Padding(2);
            // 
            // lblItem
            // 
            lblItem.AutoSize = true;
            lblItem.Location = new System.Drawing.Point(6, 55);
            lblItem.Margin = new Padding(4, 0, 4, 0);
            lblItem.Name = "lblItem";
            lblItem.Size = new Size(34, 15);
            lblItem.TabIndex = 23;
            lblItem.Text = "Item:";
            // 
            // cmbAction
            // 
            cmbAction.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            cmbAction.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            cmbAction.BorderStyle = ButtonBorderStyle.Solid;
            cmbAction.ButtonColor = System.Drawing.Color.FromArgb(43, 43, 43);
            cmbAction.DrawDropdownHoverOutline = false;
            cmbAction.DrawFocusRectangle = false;
            cmbAction.DrawMode = DrawMode.OwnerDrawFixed;
            cmbAction.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAction.FlatStyle = FlatStyle.Flat;
            cmbAction.ForeColor = System.Drawing.Color.Gainsboro;
            cmbAction.FormattingEnabled = true;
            cmbAction.Items.AddRange(new object[] { "Give", "Take" });
            cmbAction.Location = new System.Drawing.Point(75, 22);
            cmbAction.Margin = new Padding(4, 3, 4, 3);
            cmbAction.Name = "cmbAction";
            cmbAction.Size = new Size(166, 24);
            cmbAction.TabIndex = 22;
            cmbAction.Text = "Give";
            cmbAction.TextPadding = new Padding(2);
            // 
            // lblAction
            // 
            lblAction.AutoSize = true;
            lblAction.Location = new System.Drawing.Point(6, 24);
            lblAction.Margin = new Padding(4, 0, 4, 0);
            lblAction.Name = "lblAction";
            lblAction.Size = new Size(45, 15);
            lblAction.TabIndex = 21;
            lblAction.Text = "Action:";
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(295, 204);
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
            btnSave.Location = new System.Drawing.Point(9, 204);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6, 6, 6, 6);
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 19;
            btnSave.Text = "Ok";
            btnSave.Click += btnSave_Click;
            // 
            // EventCommandChangeItems
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            Controls.Add(grpChangeItems);
            Margin = new Padding(4, 3, 4, 3);
            Name = "EventCommandChangeItems";
            Size = new Size(407, 252);
            grpChangeItems.ResumeLayout(false);
            grpChangeItems.PerformLayout();
            grpVariableAmount.ResumeLayout(false);
            grpVariableAmount.PerformLayout();
            grpManualAmount.ResumeLayout(false);
            grpManualAmount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudGiveTakeAmount).EndInit();
            grpAmountType.ResumeLayout(false);
            grpAmountType.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DarkGroupBox grpChangeItems;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkComboBox cmbAction;
        private Label lblAction;
        private DarkComboBox cmbItem;
        private Label lblItem;
        private DarkComboBox cmbMethod;
        private Label lblMethod;
        private DarkGroupBox grpAmountType;
        private DarkGroupBox grpManualAmount;
        private DarkNumericUpDown nudGiveTakeAmount;
        private Label lblAmount;
        private DarkGroupBox grpVariableAmount;
        private DarkRadioButton rdoVariable;
        private DarkRadioButton rdoManual;
        private DarkComboBox cmbVariable;
        private Label lblVariable;
        private DarkRadioButton rdoGlobalVariable;
        private DarkRadioButton rdoGuildVariable;
        private DarkRadioButton rdoPlayerVariable;
    }
}
