using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Events.Event_Commands
{
    partial class EventCommandVariable
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventCommandVariable));
            this.grpSetVariable = new DarkUI.Controls.DarkGroupBox();
            this.rdoGlobalVariable = new DarkUI.Controls.DarkRadioButton();
            this.rdoPlayerVariable = new DarkUI.Controls.DarkRadioButton();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpVariableSelection = new DarkUI.Controls.DarkGroupBox();
            this.chkSyncParty = new DarkUI.Controls.DarkCheckBox();
            this.optSystemTime = new DarkUI.Controls.DarkRadioButton();
            this.cmbVariable = new DarkUI.Controls.DarkComboBox();
            this.lblVariable = new System.Windows.Forms.Label();
            this.optSet = new DarkUI.Controls.DarkRadioButton();
            this.optAdd = new DarkUI.Controls.DarkRadioButton();
            this.optRandom = new DarkUI.Controls.DarkRadioButton();
            this.optSubtract = new DarkUI.Controls.DarkRadioButton();
            this.grpRegularValues = new DarkUI.Controls.DarkGroupBox();
            this.nudValue = new DarkUI.Controls.DarkNumericUpDown();
            this.optStaticVal = new DarkUI.Controls.DarkRadioButton();
            this.cmbSetGlobalVar = new DarkUI.Controls.DarkComboBox();
            this.cmbSetPlayerVar = new DarkUI.Controls.DarkComboBox();
            this.optGlobalVar = new DarkUI.Controls.DarkRadioButton();
            this.optPlayerVar = new DarkUI.Controls.DarkRadioButton();
            this.grpRandom = new DarkUI.Controls.DarkGroupBox();
            this.nudHigh = new DarkUI.Controls.DarkNumericUpDown();
            this.nudLow = new DarkUI.Controls.DarkNumericUpDown();
            this.lblRandomHigh = new System.Windows.Forms.Label();
            this.lblRandomLow = new System.Windows.Forms.Label();
            this.grpSetVariable.SuspendLayout();
            this.grpVariableSelection.SuspendLayout();
            this.grpRegularValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValue)).BeginInit();
            this.grpRandom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHigh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLow)).BeginInit();
            this.SuspendLayout();
            // 
            // grpSetVariable
            // 
            this.grpSetVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSetVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSetVariable.Controls.Add(this.rdoGlobalVariable);
            this.grpSetVariable.Controls.Add(this.rdoPlayerVariable);
            this.grpSetVariable.Controls.Add(this.btnCancel);
            this.grpSetVariable.Controls.Add(this.btnSave);
            this.grpSetVariable.Controls.Add(this.grpVariableSelection);
            this.grpSetVariable.Controls.Add(this.grpRegularValues);
            this.grpSetVariable.Controls.Add(this.grpRandom);
            this.grpSetVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSetVariable.Location = new System.Drawing.Point(3, 3);
            this.grpSetVariable.Name = "grpSetVariable";
            this.grpSetVariable.Size = new System.Drawing.Size(266, 361);
            this.grpSetVariable.TabIndex = 17;
            this.grpSetVariable.TabStop = false;
            this.grpSetVariable.Text = "Set Variable";
            // 
            // rdoGlobalVariable
            // 
            this.rdoGlobalVariable.AutoSize = true;
            this.rdoGlobalVariable.Location = new System.Drawing.Point(144, 19);
            this.rdoGlobalVariable.Name = "rdoGlobalVariable";
            this.rdoGlobalVariable.Size = new System.Drawing.Size(96, 17);
            this.rdoGlobalVariable.TabIndex = 35;
            this.rdoGlobalVariable.Text = "Global Variable";
            this.rdoGlobalVariable.CheckedChanged += new System.EventHandler(this.rdoGlobalVariable_CheckedChanged);
            // 
            // rdoPlayerVariable
            // 
            this.rdoPlayerVariable.AutoSize = true;
            this.rdoPlayerVariable.Checked = true;
            this.rdoPlayerVariable.Location = new System.Drawing.Point(12, 19);
            this.rdoPlayerVariable.Name = "rdoPlayerVariable";
            this.rdoPlayerVariable.Size = new System.Drawing.Size(95, 17);
            this.rdoPlayerVariable.TabIndex = 34;
            this.rdoPlayerVariable.TabStop = true;
            this.rdoPlayerVariable.Text = "Player Variable";
            this.rdoPlayerVariable.CheckedChanged += new System.EventHandler(this.rdoPlayerVariable_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(185, 323);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(15, 323);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpVariableSelection
            // 
            this.grpVariableSelection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpVariableSelection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpVariableSelection.Controls.Add(this.chkSyncParty);
            this.grpVariableSelection.Controls.Add(this.optSystemTime);
            this.grpVariableSelection.Controls.Add(this.cmbVariable);
            this.grpVariableSelection.Controls.Add(this.lblVariable);
            this.grpVariableSelection.Controls.Add(this.optSet);
            this.grpVariableSelection.Controls.Add(this.optAdd);
            this.grpVariableSelection.Controls.Add(this.optRandom);
            this.grpVariableSelection.Controls.Add(this.optSubtract);
            this.grpVariableSelection.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpVariableSelection.Location = new System.Drawing.Point(3, 43);
            this.grpVariableSelection.Name = "grpVariableSelection";
            this.grpVariableSelection.Size = new System.Drawing.Size(257, 162);
            this.grpVariableSelection.TabIndex = 36;
            this.grpVariableSelection.TabStop = false;
            this.grpVariableSelection.Text = "Variable:";
            // 
            // chkSyncParty
            // 
            this.chkSyncParty.AutoSize = true;
            this.chkSyncParty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.chkSyncParty.Location = new System.Drawing.Point(119, 45);
            this.chkSyncParty.Name = "chkSyncParty";
            this.chkSyncParty.Size = new System.Drawing.Size(129, 17);
            this.chkSyncParty.TabIndex = 40;
            this.chkSyncParty.Text = "Sync Party Variables?";
            // 
            // optSystemTime
            // 
            this.optSystemTime.Location = new System.Drawing.Point(9, 136);
            this.optSystemTime.Name = "optSystemTime";
            this.optSystemTime.Size = new System.Drawing.Size(125, 17);
            this.optSystemTime.TabIndex = 39;
            this.optSystemTime.Text = "System Time (Ms)";
            this.optSystemTime.CheckedChanged += new System.EventHandler(this.optSystemTime_CheckedChanged);
            // 
            // cmbVariable
            // 
            this.cmbVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbVariable.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbVariable.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbVariable.ButtonIcon")));
            this.cmbVariable.DrawDropdownHoverOutline = false;
            this.cmbVariable.DrawFocusRectangle = false;
            this.cmbVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbVariable.FormattingEnabled = true;
            this.cmbVariable.Location = new System.Drawing.Point(63, 16);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(188, 21);
            this.cmbVariable.TabIndex = 22;
            this.cmbVariable.Text = null;
            this.cmbVariable.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // lblVariable
            // 
            this.lblVariable.AutoSize = true;
            this.lblVariable.Location = new System.Drawing.Point(9, 19);
            this.lblVariable.Name = "lblVariable";
            this.lblVariable.Size = new System.Drawing.Size(48, 13);
            this.lblVariable.TabIndex = 21;
            this.lblVariable.Text = "Variable:";
            // 
            // optSet
            // 
            this.optSet.AutoSize = true;
            this.optSet.Location = new System.Drawing.Point(9, 44);
            this.optSet.Name = "optSet";
            this.optSet.Size = new System.Drawing.Size(41, 17);
            this.optSet.TabIndex = 26;
            this.optSet.Text = "Set";
            this.optSet.CheckedChanged += new System.EventHandler(this.optSet_CheckedChanged);
            // 
            // optAdd
            // 
            this.optAdd.AutoSize = true;
            this.optAdd.Location = new System.Drawing.Point(9, 67);
            this.optAdd.Name = "optAdd";
            this.optAdd.Size = new System.Drawing.Size(44, 17);
            this.optAdd.TabIndex = 25;
            this.optAdd.Text = "Add";
            this.optAdd.CheckedChanged += new System.EventHandler(this.optAdd_CheckedChanged);
            // 
            // optRandom
            // 
            this.optRandom.AutoSize = true;
            this.optRandom.Location = new System.Drawing.Point(9, 113);
            this.optRandom.Name = "optRandom";
            this.optRandom.Size = new System.Drawing.Size(65, 17);
            this.optRandom.TabIndex = 23;
            this.optRandom.Text = "Random";
            this.optRandom.CheckedChanged += new System.EventHandler(this.optRandom_CheckedChanged);
            // 
            // optSubtract
            // 
            this.optSubtract.AutoSize = true;
            this.optSubtract.Location = new System.Drawing.Point(9, 90);
            this.optSubtract.Name = "optSubtract";
            this.optSubtract.Size = new System.Drawing.Size(65, 17);
            this.optSubtract.TabIndex = 24;
            this.optSubtract.Text = "Subtract";
            this.optSubtract.CheckedChanged += new System.EventHandler(this.optSubtract_CheckedChanged);
            // 
            // grpRegularValues
            // 
            this.grpRegularValues.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpRegularValues.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpRegularValues.Controls.Add(this.nudValue);
            this.grpRegularValues.Controls.Add(this.optStaticVal);
            this.grpRegularValues.Controls.Add(this.cmbSetGlobalVar);
            this.grpRegularValues.Controls.Add(this.cmbSetPlayerVar);
            this.grpRegularValues.Controls.Add(this.optGlobalVar);
            this.grpRegularValues.Controls.Add(this.optPlayerVar);
            this.grpRegularValues.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpRegularValues.Location = new System.Drawing.Point(3, 211);
            this.grpRegularValues.Name = "grpRegularValues";
            this.grpRegularValues.Size = new System.Drawing.Size(257, 97);
            this.grpRegularValues.TabIndex = 37;
            this.grpRegularValues.TabStop = false;
            // 
            // nudValue
            // 
            this.nudValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudValue.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudValue.Location = new System.Drawing.Point(143, 9);
            this.nudValue.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nudValue.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.nudValue.Name = "nudValue";
            this.nudValue.Size = new System.Drawing.Size(105, 20);
            this.nudValue.TabIndex = 47;
            this.nudValue.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // optStaticVal
            // 
            this.optStaticVal.AutoSize = true;
            this.optStaticVal.Checked = true;
            this.optStaticVal.Location = new System.Drawing.Point(6, 9);
            this.optStaticVal.Name = "optStaticVal";
            this.optStaticVal.Size = new System.Drawing.Size(128, 17);
            this.optStaticVal.TabIndex = 46;
            this.optStaticVal.TabStop = true;
            this.optStaticVal.Text = "Player Variable Value:";
            // 
            // cmbSetGlobalVar
            // 
            this.cmbSetGlobalVar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSetGlobalVar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSetGlobalVar.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSetGlobalVar.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbSetGlobalVar.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbSetGlobalVar.ButtonIcon")));
            this.cmbSetGlobalVar.DrawDropdownHoverOutline = false;
            this.cmbSetGlobalVar.DrawFocusRectangle = false;
            this.cmbSetGlobalVar.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSetGlobalVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSetGlobalVar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSetGlobalVar.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSetGlobalVar.FormattingEnabled = true;
            this.cmbSetGlobalVar.Location = new System.Drawing.Point(143, 70);
            this.cmbSetGlobalVar.Name = "cmbSetGlobalVar";
            this.cmbSetGlobalVar.Size = new System.Drawing.Size(105, 21);
            this.cmbSetGlobalVar.TabIndex = 45;
            this.cmbSetGlobalVar.Text = null;
            this.cmbSetGlobalVar.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbSetPlayerVar
            // 
            this.cmbSetPlayerVar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSetPlayerVar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSetPlayerVar.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSetPlayerVar.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbSetPlayerVar.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbSetPlayerVar.ButtonIcon")));
            this.cmbSetPlayerVar.DrawDropdownHoverOutline = false;
            this.cmbSetPlayerVar.DrawFocusRectangle = false;
            this.cmbSetPlayerVar.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSetPlayerVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSetPlayerVar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSetPlayerVar.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSetPlayerVar.FormattingEnabled = true;
            this.cmbSetPlayerVar.Location = new System.Drawing.Point(143, 38);
            this.cmbSetPlayerVar.Name = "cmbSetPlayerVar";
            this.cmbSetPlayerVar.Size = new System.Drawing.Size(105, 21);
            this.cmbSetPlayerVar.TabIndex = 44;
            this.cmbSetPlayerVar.Text = null;
            this.cmbSetPlayerVar.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // optGlobalVar
            // 
            this.optGlobalVar.AutoSize = true;
            this.optGlobalVar.Location = new System.Drawing.Point(6, 70);
            this.optGlobalVar.Name = "optGlobalVar";
            this.optGlobalVar.Size = new System.Drawing.Size(129, 17);
            this.optGlobalVar.TabIndex = 43;
            this.optGlobalVar.Text = "Global Variable Value:";
            // 
            // optPlayerVar
            // 
            this.optPlayerVar.AutoSize = true;
            this.optPlayerVar.Location = new System.Drawing.Point(6, 38);
            this.optPlayerVar.Name = "optPlayerVar";
            this.optPlayerVar.Size = new System.Drawing.Size(128, 17);
            this.optPlayerVar.TabIndex = 42;
            this.optPlayerVar.Text = "Player Variable Value:";
            // 
            // grpRandom
            // 
            this.grpRandom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpRandom.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpRandom.Controls.Add(this.nudHigh);
            this.grpRandom.Controls.Add(this.nudLow);
            this.grpRandom.Controls.Add(this.lblRandomHigh);
            this.grpRandom.Controls.Add(this.lblRandomLow);
            this.grpRandom.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpRandom.Location = new System.Drawing.Point(5, 210);
            this.grpRandom.Name = "grpRandom";
            this.grpRandom.Size = new System.Drawing.Size(257, 97);
            this.grpRandom.TabIndex = 39;
            this.grpRandom.TabStop = false;
            this.grpRandom.Text = "Random Number:";
            // 
            // nudHigh
            // 
            this.nudHigh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudHigh.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudHigh.Location = new System.Drawing.Point(44, 51);
            this.nudHigh.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nudHigh.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.nudHigh.Name = "nudHigh";
            this.nudHigh.Size = new System.Drawing.Size(204, 20);
            this.nudHigh.TabIndex = 42;
            this.nudHigh.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // nudLow
            // 
            this.nudLow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudLow.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudLow.Location = new System.Drawing.Point(44, 25);
            this.nudLow.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nudLow.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.nudLow.Name = "nudLow";
            this.nudLow.Size = new System.Drawing.Size(204, 20);
            this.nudLow.TabIndex = 41;
            this.nudLow.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // lblRandomHigh
            // 
            this.lblRandomHigh.AutoSize = true;
            this.lblRandomHigh.Location = new System.Drawing.Point(9, 54);
            this.lblRandomHigh.Name = "lblRandomHigh";
            this.lblRandomHigh.Size = new System.Drawing.Size(29, 13);
            this.lblRandomHigh.TabIndex = 39;
            this.lblRandomHigh.Text = "High";
            // 
            // lblRandomLow
            // 
            this.lblRandomLow.AutoSize = true;
            this.lblRandomLow.Location = new System.Drawing.Point(11, 27);
            this.lblRandomLow.Name = "lblRandomLow";
            this.lblRandomLow.Size = new System.Drawing.Size(27, 13);
            this.lblRandomLow.TabIndex = 40;
            this.lblRandomLow.Text = "Low";
            // 
            // EventCommandVariable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpSetVariable);
            this.Name = "EventCommandVariable";
            this.Size = new System.Drawing.Size(272, 367);
            this.grpSetVariable.ResumeLayout(false);
            this.grpSetVariable.PerformLayout();
            this.grpVariableSelection.ResumeLayout(false);
            this.grpVariableSelection.PerformLayout();
            this.grpRegularValues.ResumeLayout(false);
            this.grpRegularValues.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudValue)).EndInit();
            this.grpRandom.ResumeLayout(false);
            this.grpRandom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHigh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpSetVariable;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        internal DarkRadioButton optRandom;
        internal DarkRadioButton optSubtract;
        internal DarkRadioButton optAdd;
        internal DarkRadioButton optSet;
        internal DarkComboBox cmbVariable;
        internal System.Windows.Forms.Label lblVariable;
        private DarkRadioButton rdoGlobalVariable;
        private DarkRadioButton rdoPlayerVariable;
        private DarkGroupBox grpVariableSelection;
        internal DarkRadioButton optSystemTime;
        private DarkGroupBox grpRegularValues;
        private DarkNumericUpDown nudValue;
        internal DarkRadioButton optStaticVal;
        internal DarkComboBox cmbSetGlobalVar;
        internal DarkComboBox cmbSetPlayerVar;
        internal DarkRadioButton optGlobalVar;
        internal DarkRadioButton optPlayerVar;
        private DarkGroupBox grpRandom;
        private DarkNumericUpDown nudHigh;
        private DarkNumericUpDown nudLow;
        internal System.Windows.Forms.Label lblRandomHigh;
        internal System.Windows.Forms.Label lblRandomLow;
        private DarkCheckBox chkSyncParty;
    }
}
