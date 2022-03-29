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
            this.grpChangeItems = new DarkUI.Controls.DarkGroupBox();
            this.grpVariableAmount = new DarkUI.Controls.DarkGroupBox();
            this.lblVariable = new System.Windows.Forms.Label();
            this.rdoGlobalVariable = new DarkUI.Controls.DarkRadioButton();
            this.rdoGuildVariable = new DarkUI.Controls.DarkRadioButton();
            this.rdoPlayerVariable = new DarkUI.Controls.DarkRadioButton();
            this.grpManualAmount = new DarkUI.Controls.DarkGroupBox();
            this.nudGiveTakeAmount = new DarkUI.Controls.DarkNumericUpDown();
            this.lblAmount = new System.Windows.Forms.Label();
            this.grpAmountType = new DarkUI.Controls.DarkGroupBox();
            this.rdoVariable = new DarkUI.Controls.DarkRadioButton();
            this.rdoManual = new DarkUI.Controls.DarkRadioButton();
            this.lblMethod = new System.Windows.Forms.Label();
            this.lblItem = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.cmbVariable = new DarkUI.Controls.DarkComboBox();
            this.cmbMethod = new DarkUI.Controls.DarkComboBox();
            this.cmbItem = new DarkUI.Controls.DarkComboBox();
            this.cmbAction = new DarkUI.Controls.DarkComboBox();
            this.grpChangeItems.SuspendLayout();
            this.grpVariableAmount.SuspendLayout();
            this.grpManualAmount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGiveTakeAmount)).BeginInit();
            this.grpAmountType.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChangeItems
            // 
            this.grpChangeItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpChangeItems.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpChangeItems.Controls.Add(this.grpVariableAmount);
            this.grpChangeItems.Controls.Add(this.grpManualAmount);
            this.grpChangeItems.Controls.Add(this.grpAmountType);
            this.grpChangeItems.Controls.Add(this.cmbMethod);
            this.grpChangeItems.Controls.Add(this.lblMethod);
            this.grpChangeItems.Controls.Add(this.cmbItem);
            this.grpChangeItems.Controls.Add(this.lblItem);
            this.grpChangeItems.Controls.Add(this.cmbAction);
            this.grpChangeItems.Controls.Add(this.lblAction);
            this.grpChangeItems.Controls.Add(this.btnCancel);
            this.grpChangeItems.Controls.Add(this.btnSave);
            this.grpChangeItems.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpChangeItems.Location = new System.Drawing.Point(3, 3);
            this.grpChangeItems.Name = "grpChangeItems";
            this.grpChangeItems.Size = new System.Drawing.Size(336, 208);
            this.grpChangeItems.TabIndex = 17;
            this.grpChangeItems.TabStop = false;
            this.grpChangeItems.Text = "Change Player Items:";
            // 
            // grpVariableAmount
            // 
            this.grpVariableAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpVariableAmount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpVariableAmount.Controls.Add(this.cmbVariable);
            this.grpVariableAmount.Controls.Add(this.lblVariable);
            this.grpVariableAmount.Controls.Add(this.rdoGlobalVariable);
            this.grpVariableAmount.Controls.Add(this.rdoGuildVariable);
            this.grpVariableAmount.Controls.Add(this.rdoPlayerVariable);
            this.grpVariableAmount.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpVariableAmount.Location = new System.Drawing.Point(8, 100);
            this.grpVariableAmount.Name = "grpVariableAmount";
            this.grpVariableAmount.Size = new System.Drawing.Size(320, 71);
            this.grpVariableAmount.TabIndex = 38;
            this.grpVariableAmount.TabStop = false;
            this.grpVariableAmount.Text = "Variable Amount:";
            this.grpVariableAmount.Visible = false;
            // 
            // lblVariable
            // 
            this.lblVariable.AutoSize = true;
            this.lblVariable.Location = new System.Drawing.Point(8, 46);
            this.lblVariable.Name = "lblVariable";
            this.lblVariable.Size = new System.Drawing.Size(45, 13);
            this.lblVariable.TabIndex = 38;
            this.lblVariable.Text = "Variable";
            // 
            // rdoGlobalVariable
            // 
            this.rdoGlobalVariable.AutoSize = true;
            this.rdoGlobalVariable.Location = new System.Drawing.Point(112, 19);
            this.rdoGlobalVariable.Name = "rdoGlobalVariable";
            this.rdoGlobalVariable.Size = new System.Drawing.Size(96, 17);
            this.rdoGlobalVariable.TabIndex = 37;
            this.rdoGlobalVariable.Text = "Global Variable";
            this.rdoGlobalVariable.CheckedChanged += new System.EventHandler(this.rdoGlobalVariable_CheckedChanged);
            // 
            // rdoGuildVariable
            // 
            this.rdoGuildVariable.AutoSize = true;
            this.rdoGuildVariable.Location = new System.Drawing.Point(224, 19);
            this.rdoGuildVariable.Name = "rdoGuildVariable";
            this.rdoGuildVariable.Size = new System.Drawing.Size(90, 17);
            this.rdoGuildVariable.TabIndex = 37;
            this.rdoGuildVariable.Text = "Guild Variable";
            this.rdoGuildVariable.CheckedChanged += new System.EventHandler(this.rdoGuildVariable_CheckedChanged);
            // 
            // rdoPlayerVariable
            // 
            this.rdoPlayerVariable.AutoSize = true;
            this.rdoPlayerVariable.Checked = true;
            this.rdoPlayerVariable.Location = new System.Drawing.Point(6, 19);
            this.rdoPlayerVariable.Name = "rdoPlayerVariable";
            this.rdoPlayerVariable.Size = new System.Drawing.Size(95, 17);
            this.rdoPlayerVariable.TabIndex = 36;
            this.rdoPlayerVariable.TabStop = true;
            this.rdoPlayerVariable.Text = "Player Variable";
            this.rdoPlayerVariable.CheckedChanged += new System.EventHandler(this.rdoPlayerVariable_CheckedChanged);
            // 
            // grpManualAmount
            // 
            this.grpManualAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpManualAmount.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpManualAmount.Controls.Add(this.nudGiveTakeAmount);
            this.grpManualAmount.Controls.Add(this.lblAmount);
            this.grpManualAmount.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpManualAmount.Location = new System.Drawing.Point(8, 100);
            this.grpManualAmount.Name = "grpManualAmount";
            this.grpManualAmount.Size = new System.Drawing.Size(292, 71);
            this.grpManualAmount.TabIndex = 37;
            this.grpManualAmount.TabStop = false;
            this.grpManualAmount.Text = "Manual Amount:";
            // 
            // nudGiveTakeAmount
            // 
            this.nudGiveTakeAmount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudGiveTakeAmount.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudGiveTakeAmount.Location = new System.Drawing.Point(67, 32);
            this.nudGiveTakeAmount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudGiveTakeAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudGiveTakeAmount.Name = "nudGiveTakeAmount";
            this.nudGiveTakeAmount.Size = new System.Drawing.Size(115, 20);
            this.nudGiveTakeAmount.TabIndex = 28;
            this.nudGiveTakeAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblAmount
            // 
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(8, 32);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(46, 13);
            this.lblAmount.TabIndex = 27;
            this.lblAmount.Text = "Amount:";
            // 
            // grpAmountType
            // 
            this.grpAmountType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpAmountType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpAmountType.Controls.Add(this.rdoVariable);
            this.grpAmountType.Controls.Add(this.rdoManual);
            this.grpAmountType.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpAmountType.Location = new System.Drawing.Point(213, 19);
            this.grpAmountType.Name = "grpAmountType";
            this.grpAmountType.Size = new System.Drawing.Size(115, 75);
            this.grpAmountType.TabIndex = 36;
            this.grpAmountType.TabStop = false;
            this.grpAmountType.Text = "Amount Type:";
            // 
            // rdoVariable
            // 
            this.rdoVariable.AutoSize = true;
            this.rdoVariable.Location = new System.Drawing.Point(9, 42);
            this.rdoVariable.Name = "rdoVariable";
            this.rdoVariable.Size = new System.Drawing.Size(63, 17);
            this.rdoVariable.TabIndex = 36;
            this.rdoVariable.Text = "Variable";
            this.rdoVariable.CheckedChanged += new System.EventHandler(this.rdoVariable_CheckedChanged);
            // 
            // rdoManual
            // 
            this.rdoManual.AutoSize = true;
            this.rdoManual.Checked = true;
            this.rdoManual.Location = new System.Drawing.Point(9, 19);
            this.rdoManual.Name = "rdoManual";
            this.rdoManual.Size = new System.Drawing.Size(60, 17);
            this.rdoManual.TabIndex = 35;
            this.rdoManual.TabStop = true;
            this.rdoManual.Text = "Manual";
            this.rdoManual.CheckedChanged += new System.EventHandler(this.rdoManual_CheckedChanged);
            // 
            // lblMethod
            // 
            this.lblMethod.AutoSize = true;
            this.lblMethod.Location = new System.Drawing.Point(5, 76);
            this.lblMethod.Name = "lblMethod";
            this.lblMethod.Size = new System.Drawing.Size(46, 13);
            this.lblMethod.TabIndex = 27;
            this.lblMethod.Text = "Method:";
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(5, 48);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(30, 13);
            this.lblItem.TabIndex = 23;
            this.lblItem.Text = "Item:";
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.Location = new System.Drawing.Point(5, 21);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(40, 13);
            this.lblAction.TabIndex = 21;
            this.lblAction.Text = "Action:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(253, 177);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(8, 177);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbVariable
            // 
            this.cmbVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbVariable.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbVariable.DrawDropdownHoverOutline = false;
            this.cmbVariable.DrawFocusRectangle = false;
            this.cmbVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbVariable.FormattingEnabled = true;
            this.cmbVariable.Location = new System.Drawing.Point(67, 44);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(247, 21);
            this.cmbVariable.TabIndex = 39;
            this.cmbVariable.Text = null;
            this.cmbVariable.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbMethod
            // 
            this.cmbMethod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbMethod.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbMethod.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbMethod.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbMethod.DrawDropdownHoverOutline = false;
            this.cmbMethod.DrawFocusRectangle = false;
            this.cmbMethod.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMethod.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbMethod.FormattingEnabled = true;
            this.cmbMethod.Items.AddRange(new object[] {
            "Normal",
            "Allow Overflow",
            "Up to Amount"});
            this.cmbMethod.Location = new System.Drawing.Point(64, 73);
            this.cmbMethod.Name = "cmbMethod";
            this.cmbMethod.Size = new System.Drawing.Size(143, 21);
            this.cmbMethod.TabIndex = 28;
            this.cmbMethod.Text = "Normal";
            this.cmbMethod.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbItem
            // 
            this.cmbItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbItem.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbItem.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbItem.DrawDropdownHoverOutline = false;
            this.cmbItem.DrawFocusRectangle = false;
            this.cmbItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Location = new System.Drawing.Point(64, 46);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(143, 21);
            this.cmbItem.TabIndex = 24;
            this.cmbItem.Text = null;
            this.cmbItem.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbAction
            // 
            this.cmbAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbAction.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbAction.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbAction.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbAction.DrawDropdownHoverOutline = false;
            this.cmbAction.DrawFocusRectangle = false;
            this.cmbAction.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAction.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbAction.FormattingEnabled = true;
            this.cmbAction.Items.AddRange(new object[] {
            "Give",
            "Take"});
            this.cmbAction.Location = new System.Drawing.Point(64, 19);
            this.cmbAction.Name = "cmbAction";
            this.cmbAction.Size = new System.Drawing.Size(143, 21);
            this.cmbAction.TabIndex = 22;
            this.cmbAction.Text = "Give";
            this.cmbAction.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // EventCommandChangeItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpChangeItems);
            this.Name = "EventCommandChangeItems";
            this.Size = new System.Drawing.Size(349, 218);
            this.grpChangeItems.ResumeLayout(false);
            this.grpChangeItems.PerformLayout();
            this.grpVariableAmount.ResumeLayout(false);
            this.grpVariableAmount.PerformLayout();
            this.grpManualAmount.ResumeLayout(false);
            this.grpManualAmount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudGiveTakeAmount)).EndInit();
            this.grpAmountType.ResumeLayout(false);
            this.grpAmountType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpChangeItems;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkComboBox cmbAction;
        private System.Windows.Forms.Label lblAction;
        private DarkComboBox cmbItem;
        private System.Windows.Forms.Label lblItem;
        private DarkComboBox cmbMethod;
        private System.Windows.Forms.Label lblMethod;
        private DarkGroupBox grpAmountType;
        private DarkGroupBox grpManualAmount;
        private DarkNumericUpDown nudGiveTakeAmount;
        private System.Windows.Forms.Label lblAmount;
        private DarkGroupBox grpVariableAmount;
        private DarkRadioButton rdoVariable;
        private DarkRadioButton rdoManual;
        private DarkComboBox cmbVariable;
        private System.Windows.Forms.Label lblVariable;
        private DarkRadioButton rdoGlobalVariable;
        private DarkRadioButton rdoGuildVariable;
        private DarkRadioButton rdoPlayerVariable;
    }
}
