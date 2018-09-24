using DarkUI.Controls;

namespace Intersect.Editor.Forms.Editors.Event_Commands
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
            this.cmbSetGlobalVar = new DarkUI.Controls.DarkComboBox();
            this.cmbSetPlayerVar = new DarkUI.Controls.DarkComboBox();
            this.optGlobalVar = new DarkUI.Controls.DarkRadioButton();
            this.optPlayerVar = new DarkUI.Controls.DarkRadioButton();
            this.optSystemTime = new DarkUI.Controls.DarkRadioButton();
            this.nudHigh = new DarkUI.Controls.DarkNumericUpDown();
            this.nudLow = new DarkUI.Controls.DarkNumericUpDown();
            this.nudSubtract = new DarkUI.Controls.DarkNumericUpDown();
            this.nudAdd = new DarkUI.Controls.DarkNumericUpDown();
            this.nudSet = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbVariable = new DarkUI.Controls.DarkComboBox();
            this.lblVariable = new System.Windows.Forms.Label();
            this.lblRandomHigh = new System.Windows.Forms.Label();
            this.optSet = new DarkUI.Controls.DarkRadioButton();
            this.lblRandomLow = new System.Windows.Forms.Label();
            this.optAdd = new DarkUI.Controls.DarkRadioButton();
            this.optRandom = new DarkUI.Controls.DarkRadioButton();
            this.optSubtract = new DarkUI.Controls.DarkRadioButton();
            this.grpSetVariable.SuspendLayout();
            this.grpVariableSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHigh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSubtract)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAdd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSet)).BeginInit();
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
            this.grpVariableSelection.Controls.Add(this.cmbSetGlobalVar);
            this.grpVariableSelection.Controls.Add(this.cmbSetPlayerVar);
            this.grpVariableSelection.Controls.Add(this.optGlobalVar);
            this.grpVariableSelection.Controls.Add(this.optPlayerVar);
            this.grpVariableSelection.Controls.Add(this.optSystemTime);
            this.grpVariableSelection.Controls.Add(this.nudHigh);
            this.grpVariableSelection.Controls.Add(this.nudLow);
            this.grpVariableSelection.Controls.Add(this.nudSubtract);
            this.grpVariableSelection.Controls.Add(this.nudAdd);
            this.grpVariableSelection.Controls.Add(this.nudSet);
            this.grpVariableSelection.Controls.Add(this.cmbVariable);
            this.grpVariableSelection.Controls.Add(this.lblVariable);
            this.grpVariableSelection.Controls.Add(this.lblRandomHigh);
            this.grpVariableSelection.Controls.Add(this.optSet);
            this.grpVariableSelection.Controls.Add(this.lblRandomLow);
            this.grpVariableSelection.Controls.Add(this.optAdd);
            this.grpVariableSelection.Controls.Add(this.optRandom);
            this.grpVariableSelection.Controls.Add(this.optSubtract);
            this.grpVariableSelection.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpVariableSelection.Location = new System.Drawing.Point(3, 43);
            this.grpVariableSelection.Name = "grpVariableSelection";
            this.grpVariableSelection.Size = new System.Drawing.Size(257, 264);
            this.grpVariableSelection.TabIndex = 36;
            this.grpVariableSelection.TabStop = false;
            this.grpVariableSelection.Text = "Variable:";
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
            this.cmbSetGlobalVar.Location = new System.Drawing.Point(141, 232);
            this.cmbSetGlobalVar.Name = "cmbSetGlobalVar";
            this.cmbSetGlobalVar.Size = new System.Drawing.Size(110, 21);
            this.cmbSetGlobalVar.TabIndex = 43;
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
            this.cmbSetPlayerVar.Location = new System.Drawing.Point(141, 205);
            this.cmbSetPlayerVar.Name = "cmbSetPlayerVar";
            this.cmbSetPlayerVar.Size = new System.Drawing.Size(110, 21);
            this.cmbSetPlayerVar.TabIndex = 42;
            this.cmbSetPlayerVar.Text = null;
            this.cmbSetPlayerVar.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // optGlobalVar
            // 
            this.optGlobalVar.AutoSize = true;
            this.optGlobalVar.Location = new System.Drawing.Point(9, 233);
            this.optGlobalVar.Name = "optGlobalVar";
            this.optGlobalVar.Size = new System.Drawing.Size(129, 17);
            this.optGlobalVar.TabIndex = 41;
            this.optGlobalVar.Text = "Global Variable Value:";
            this.optGlobalVar.CheckedChanged += new System.EventHandler(this.optGlobalVar_CheckedChanged);
            // 
            // optPlayerVar
            // 
            this.optPlayerVar.AutoSize = true;
            this.optPlayerVar.Location = new System.Drawing.Point(9, 206);
            this.optPlayerVar.Name = "optPlayerVar";
            this.optPlayerVar.Size = new System.Drawing.Size(128, 17);
            this.optPlayerVar.TabIndex = 40;
            this.optPlayerVar.Text = "Player Variable Value:";
            this.optPlayerVar.CheckedChanged += new System.EventHandler(this.optPlayerVar_CheckedChanged);
            // 
            // optSystemTime
            // 
            this.optSystemTime.Location = new System.Drawing.Point(9, 178);
            this.optSystemTime.Name = "optSystemTime";
            this.optSystemTime.Size = new System.Drawing.Size(214, 17);
            this.optSystemTime.TabIndex = 39;
            this.optSystemTime.Text = "System Time (Ms)";
            this.optSystemTime.CheckedChanged += new System.EventHandler(this.optSystemTime_CheckedChanged);
            // 
            // nudHigh
            // 
            this.nudHigh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudHigh.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudHigh.Location = new System.Drawing.Point(169, 151);
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
            this.nudHigh.Size = new System.Drawing.Size(82, 20);
            this.nudHigh.TabIndex = 38;
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
            this.nudLow.Location = new System.Drawing.Point(169, 124);
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
            this.nudLow.Size = new System.Drawing.Size(82, 20);
            this.nudLow.TabIndex = 37;
            this.nudLow.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // nudSubtract
            // 
            this.nudSubtract.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudSubtract.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudSubtract.Location = new System.Drawing.Point(169, 98);
            this.nudSubtract.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nudSubtract.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.nudSubtract.Name = "nudSubtract";
            this.nudSubtract.Size = new System.Drawing.Size(82, 20);
            this.nudSubtract.TabIndex = 36;
            this.nudSubtract.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // nudAdd
            // 
            this.nudAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudAdd.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudAdd.Location = new System.Drawing.Point(169, 70);
            this.nudAdd.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nudAdd.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.nudAdd.Name = "nudAdd";
            this.nudAdd.Size = new System.Drawing.Size(82, 20);
            this.nudAdd.TabIndex = 35;
            this.nudAdd.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // nudSet
            // 
            this.nudSet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudSet.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudSet.Location = new System.Drawing.Point(169, 44);
            this.nudSet.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.nudSet.Minimum = new decimal(new int[] {
            1000000000,
            0,
            0,
            -2147483648});
            this.nudSet.Name = "nudSet";
            this.nudSet.Size = new System.Drawing.Size(82, 20);
            this.nudSet.TabIndex = 34;
            this.nudSet.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
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
            // lblRandomHigh
            // 
            this.lblRandomHigh.AutoSize = true;
            this.lblRandomHigh.Location = new System.Drawing.Point(138, 155);
            this.lblRandomHigh.Name = "lblRandomHigh";
            this.lblRandomHigh.Size = new System.Drawing.Size(29, 13);
            this.lblRandomHigh.TabIndex = 32;
            this.lblRandomHigh.Text = "High";
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
            // lblRandomLow
            // 
            this.lblRandomLow.AutoSize = true;
            this.lblRandomLow.Location = new System.Drawing.Point(140, 126);
            this.lblRandomLow.Name = "lblRandomLow";
            this.lblRandomLow.Size = new System.Drawing.Size(27, 13);
            this.lblRandomLow.TabIndex = 33;
            this.lblRandomLow.Text = "Low";
            // 
            // optAdd
            // 
            this.optAdd.AutoSize = true;
            this.optAdd.Location = new System.Drawing.Point(9, 70);
            this.optAdd.Name = "optAdd";
            this.optAdd.Size = new System.Drawing.Size(44, 17);
            this.optAdd.TabIndex = 25;
            this.optAdd.Text = "Add";
            this.optAdd.CheckedChanged += new System.EventHandler(this.optAdd_CheckedChanged);
            // 
            // optRandom
            // 
            this.optRandom.AutoSize = true;
            this.optRandom.Location = new System.Drawing.Point(9, 126);
            this.optRandom.Name = "optRandom";
            this.optRandom.Size = new System.Drawing.Size(65, 17);
            this.optRandom.TabIndex = 23;
            this.optRandom.Text = "Random";
            this.optRandom.CheckedChanged += new System.EventHandler(this.optRandom_CheckedChanged);
            // 
            // optSubtract
            // 
            this.optSubtract.AutoSize = true;
            this.optSubtract.Location = new System.Drawing.Point(9, 98);
            this.optSubtract.Name = "optSubtract";
            this.optSubtract.Size = new System.Drawing.Size(65, 17);
            this.optSubtract.TabIndex = 24;
            this.optSubtract.Text = "Subtract";
            this.optSubtract.CheckedChanged += new System.EventHandler(this.optSubtract_CheckedChanged);
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
            ((System.ComponentModel.ISupportInitialize)(this.nudHigh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSubtract)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAdd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpSetVariable;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        internal System.Windows.Forms.Label lblRandomHigh;
        internal System.Windows.Forms.Label lblRandomLow;
        internal DarkRadioButton optRandom;
        internal DarkRadioButton optSubtract;
        internal DarkRadioButton optAdd;
        internal DarkRadioButton optSet;
        internal DarkComboBox cmbVariable;
        internal System.Windows.Forms.Label lblVariable;
        private DarkRadioButton rdoGlobalVariable;
        private DarkRadioButton rdoPlayerVariable;
        private DarkGroupBox grpVariableSelection;
        private DarkNumericUpDown nudHigh;
        private DarkNumericUpDown nudLow;
        private DarkNumericUpDown nudSubtract;
        private DarkNumericUpDown nudAdd;
        private DarkNumericUpDown nudSet;
        internal DarkComboBox cmbSetGlobalVar;
        internal DarkComboBox cmbSetPlayerVar;
        internal DarkRadioButton optGlobalVar;
        internal DarkRadioButton optPlayerVar;
        internal DarkRadioButton optSystemTime;
    }
}
