using DarkUI.Controls;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_Variable
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
            this.grpSetVariable = new DarkUI.Controls.DarkGroupBox();
            this.rdoGlobalVariable = new DarkUI.Controls.DarkRadioButton();
            this.rdoPlayerVariable = new DarkUI.Controls.DarkRadioButton();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpVariableSelection = new DarkUI.Controls.DarkGroupBox();
            this.cmbVariable = new DarkUI.Controls.DarkComboBox();
            this.lblVariable = new System.Windows.Forms.Label();
            this.lblRandomHigh = new System.Windows.Forms.Label();
            this.optSet = new DarkUI.Controls.DarkRadioButton();
            this.lblRandomLow = new System.Windows.Forms.Label();
            this.txtSet = new DarkUI.Controls.DarkTextBox();
            this.txtRandomHigh = new DarkUI.Controls.DarkTextBox();
            this.optAdd = new DarkUI.Controls.DarkRadioButton();
            this.txtRandomLow = new DarkUI.Controls.DarkTextBox();
            this.txtAdd = new DarkUI.Controls.DarkTextBox();
            this.optRandom = new DarkUI.Controls.DarkRadioButton();
            this.optSubtract = new DarkUI.Controls.DarkRadioButton();
            this.txtSubtract = new DarkUI.Controls.DarkTextBox();
            this.grpSetVariable.SuspendLayout();
            this.grpVariableSelection.SuspendLayout();
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
            this.grpSetVariable.Size = new System.Drawing.Size(210, 253);
            this.grpSetVariable.TabIndex = 17;
            this.grpSetVariable.TabStop = false;
            this.grpSetVariable.Text = "Set Variable";
            // 
            // rdoGlobalVariable
            // 
            this.rdoGlobalVariable.AutoSize = true;
            this.rdoGlobalVariable.Location = new System.Drawing.Point(108, 19);
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
            this.rdoPlayerVariable.Location = new System.Drawing.Point(3, 19);
            this.rdoPlayerVariable.Name = "rdoPlayerVariable";
            this.rdoPlayerVariable.Size = new System.Drawing.Size(95, 17);
            this.rdoPlayerVariable.TabIndex = 34;
            this.rdoPlayerVariable.TabStop = true;
            this.rdoPlayerVariable.Text = "Player Variable";
            this.rdoPlayerVariable.CheckedChanged += new System.EventHandler(this.rdoPlayerVariable_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(118, 224);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(14, 224);
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
            this.grpVariableSelection.Controls.Add(this.cmbVariable);
            this.grpVariableSelection.Controls.Add(this.lblVariable);
            this.grpVariableSelection.Controls.Add(this.lblRandomHigh);
            this.grpVariableSelection.Controls.Add(this.optSet);
            this.grpVariableSelection.Controls.Add(this.lblRandomLow);
            this.grpVariableSelection.Controls.Add(this.txtSet);
            this.grpVariableSelection.Controls.Add(this.txtRandomHigh);
            this.grpVariableSelection.Controls.Add(this.optAdd);
            this.grpVariableSelection.Controls.Add(this.txtRandomLow);
            this.grpVariableSelection.Controls.Add(this.txtAdd);
            this.grpVariableSelection.Controls.Add(this.optRandom);
            this.grpVariableSelection.Controls.Add(this.optSubtract);
            this.grpVariableSelection.Controls.Add(this.txtSubtract);
            this.grpVariableSelection.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpVariableSelection.Location = new System.Drawing.Point(3, 43);
            this.grpVariableSelection.Name = "grpVariableSelection";
            this.grpVariableSelection.Size = new System.Drawing.Size(201, 175);
            this.grpVariableSelection.TabIndex = 36;
            this.grpVariableSelection.TabStop = false;
            this.grpVariableSelection.Text = "Variable:";
            // 
            // cmbVariable
            // 
            this.cmbVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbVariable.FormattingEnabled = true;
            this.cmbVariable.Location = new System.Drawing.Point(63, 16);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(128, 21);
            this.cmbVariable.TabIndex = 22;
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
            this.lblRandomHigh.Location = new System.Drawing.Point(78, 155);
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
            this.lblRandomLow.Location = new System.Drawing.Point(80, 126);
            this.lblRandomLow.Name = "lblRandomLow";
            this.lblRandomLow.Size = new System.Drawing.Size(27, 13);
            this.lblRandomLow.TabIndex = 33;
            this.lblRandomLow.Text = "Low";
            // 
            // txtSet
            // 
            this.txtSet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSet.Location = new System.Drawing.Point(109, 43);
            this.txtSet.Name = "txtSet";
            this.txtSet.Size = new System.Drawing.Size(82, 20);
            this.txtSet.TabIndex = 31;
            this.txtSet.Text = "0";
            // 
            // txtRandomHigh
            // 
            this.txtRandomHigh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtRandomHigh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRandomHigh.Enabled = false;
            this.txtRandomHigh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtRandomHigh.Location = new System.Drawing.Point(109, 148);
            this.txtRandomHigh.Name = "txtRandomHigh";
            this.txtRandomHigh.Size = new System.Drawing.Size(82, 20);
            this.txtRandomHigh.TabIndex = 27;
            this.txtRandomHigh.Text = "0";
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
            // txtRandomLow
            // 
            this.txtRandomLow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtRandomLow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRandomLow.Enabled = false;
            this.txtRandomLow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtRandomLow.Location = new System.Drawing.Point(109, 121);
            this.txtRandomLow.Name = "txtRandomLow";
            this.txtRandomLow.Size = new System.Drawing.Size(82, 20);
            this.txtRandomLow.TabIndex = 28;
            this.txtRandomLow.Text = "0";
            // 
            // txtAdd
            // 
            this.txtAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtAdd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAdd.Enabled = false;
            this.txtAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtAdd.Location = new System.Drawing.Point(109, 69);
            this.txtAdd.Name = "txtAdd";
            this.txtAdd.Size = new System.Drawing.Size(82, 20);
            this.txtAdd.TabIndex = 30;
            this.txtAdd.Text = "0";
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
            // txtSubtract
            // 
            this.txtSubtract.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSubtract.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSubtract.Enabled = false;
            this.txtSubtract.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSubtract.Location = new System.Drawing.Point(109, 95);
            this.txtSubtract.Name = "txtSubtract";
            this.txtSubtract.Size = new System.Drawing.Size(82, 20);
            this.txtSubtract.TabIndex = 29;
            this.txtSubtract.Text = "0";
            // 
            // EventCommand_Variable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpSetVariable);
            this.Name = "EventCommand_Variable";
            this.Size = new System.Drawing.Size(216, 260);
            this.grpSetVariable.ResumeLayout(false);
            this.grpSetVariable.PerformLayout();
            this.grpVariableSelection.ResumeLayout(false);
            this.grpVariableSelection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpSetVariable;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        internal System.Windows.Forms.Label lblRandomHigh;
        internal System.Windows.Forms.Label lblRandomLow;
        internal DarkTextBox txtRandomHigh;
        internal DarkTextBox txtRandomLow;
        internal DarkRadioButton optRandom;
        internal DarkTextBox txtSubtract;
        internal DarkRadioButton optSubtract;
        internal DarkTextBox txtAdd;
        internal DarkRadioButton optAdd;
        internal DarkTextBox txtSet;
        internal DarkRadioButton optSet;
        internal DarkComboBox cmbVariable;
        internal System.Windows.Forms.Label lblVariable;
        private DarkRadioButton rdoGlobalVariable;
        private DarkRadioButton rdoPlayerVariable;
        private DarkGroupBox grpVariableSelection;
    }
}
