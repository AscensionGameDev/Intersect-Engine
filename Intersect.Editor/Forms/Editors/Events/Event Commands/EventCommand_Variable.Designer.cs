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
            this.grpSelectVariable = new DarkUI.Controls.DarkGroupBox();
            this.chkSyncParty = new DarkUI.Controls.DarkCheckBox();
            this.rdoPlayerVariable = new DarkUI.Controls.DarkRadioButton();
            this.cmbVariable = new DarkUI.Controls.DarkComboBox();
            this.rdoGlobalVariable = new DarkUI.Controls.DarkRadioButton();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.grpStringVariable = new DarkUI.Controls.DarkGroupBox();
            this.lblStringTextVariables = new System.Windows.Forms.Label();
            this.grpStringReplace = new DarkUI.Controls.DarkGroupBox();
            this.txtStringReplace = new DarkUI.Controls.DarkTextBox();
            this.txtStringFind = new DarkUI.Controls.DarkTextBox();
            this.lblStringReplace = new System.Windows.Forms.Label();
            this.lblStringFind = new System.Windows.Forms.Label();
            this.optReplaceString = new DarkUI.Controls.DarkRadioButton();
            this.optStaticString = new DarkUI.Controls.DarkRadioButton();
            this.grpStringSet = new DarkUI.Controls.DarkGroupBox();
            this.lblStringValue = new System.Windows.Forms.Label();
            this.txtStringValue = new DarkUI.Controls.DarkTextBox();
            this.grpBooleanVariable = new DarkUI.Controls.DarkGroupBox();
            this.cmbBooleanCloneGlobalVar = new DarkUI.Controls.DarkComboBox();
            this.cmbBooleanClonePlayerVar = new DarkUI.Controls.DarkComboBox();
            this.optBooleanCloneGlobalVar = new DarkUI.Controls.DarkRadioButton();
            this.optBooleanClonePlayerVar = new DarkUI.Controls.DarkRadioButton();
            this.optBooleanTrue = new DarkUI.Controls.DarkRadioButton();
            this.optBooleanFalse = new DarkUI.Controls.DarkRadioButton();
            this.grpNumericVariable = new DarkUI.Controls.DarkGroupBox();
            this.optNumericSet = new DarkUI.Controls.DarkRadioButton();
            this.optNumericAdd = new DarkUI.Controls.DarkRadioButton();
            this.optNumericRandom = new DarkUI.Controls.DarkRadioButton();
            this.optNumericSubtract = new DarkUI.Controls.DarkRadioButton();
            this.optNumericSystemTime = new DarkUI.Controls.DarkRadioButton();
            this.grpNumericValues = new DarkUI.Controls.DarkGroupBox();
            this.nudNumericValue = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbNumericCloneGlobalVar = new DarkUI.Controls.DarkComboBox();
            this.cmbNumericClonePlayerVar = new DarkUI.Controls.DarkComboBox();
            this.optNumericCloneGlobalVar = new DarkUI.Controls.DarkRadioButton();
            this.optNumericClonePlayerVar = new DarkUI.Controls.DarkRadioButton();
            this.optNumericStaticVal = new DarkUI.Controls.DarkRadioButton();
            this.grpNumericRandom = new DarkUI.Controls.DarkGroupBox();
            this.nudHigh = new DarkUI.Controls.DarkNumericUpDown();
            this.nudLow = new DarkUI.Controls.DarkNumericUpDown();
            this.lblNumericRandomHigh = new System.Windows.Forms.Label();
            this.lblNumericRandomLow = new System.Windows.Forms.Label();
            this.optNumericMultiply = new DarkUI.Controls.DarkRadioButton();
            this.optNumericDivide = new DarkUI.Controls.DarkRadioButton();
            this.optNumericLeftShift = new DarkUI.Controls.DarkRadioButton();
            this.optNumericRightShift = new DarkUI.Controls.DarkRadioButton();
            this.grpSetVariable.SuspendLayout();
            this.grpSelectVariable.SuspendLayout();
            this.grpStringVariable.SuspendLayout();
            this.grpStringReplace.SuspendLayout();
            this.grpStringSet.SuspendLayout();
            this.grpBooleanVariable.SuspendLayout();
            this.grpNumericVariable.SuspendLayout();
            this.grpNumericValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumericValue)).BeginInit();
            this.grpNumericRandom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHigh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLow)).BeginInit();
            this.SuspendLayout();
            // 
            // grpSetVariable
            // 
            this.grpSetVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSetVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSetVariable.Controls.Add(this.grpSelectVariable);
            this.grpSetVariable.Controls.Add(this.btnCancel);
            this.grpSetVariable.Controls.Add(this.btnSave);
            this.grpSetVariable.Controls.Add(this.grpNumericVariable);
            this.grpSetVariable.Controls.Add(this.grpStringVariable);
            this.grpSetVariable.Controls.Add(this.grpBooleanVariable);
            this.grpSetVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSetVariable.Location = new System.Drawing.Point(3, 3);
            this.grpSetVariable.Name = "grpSetVariable";
            this.grpSetVariable.Size = new System.Drawing.Size(308, 339);
            this.grpSetVariable.TabIndex = 17;
            this.grpSetVariable.TabStop = false;
            this.grpSetVariable.Text = "Set Variable";
            // 
            // grpSelectVariable
            // 
            this.grpSelectVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSelectVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSelectVariable.Controls.Add(this.chkSyncParty);
            this.grpSelectVariable.Controls.Add(this.rdoPlayerVariable);
            this.grpSelectVariable.Controls.Add(this.cmbVariable);
            this.grpSelectVariable.Controls.Add(this.rdoGlobalVariable);
            this.grpSelectVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSelectVariable.Location = new System.Drawing.Point(6, 19);
            this.grpSelectVariable.Name = "grpSelectVariable";
            this.grpSelectVariable.Size = new System.Drawing.Size(296, 75);
            this.grpSelectVariable.TabIndex = 40;
            this.grpSelectVariable.TabStop = false;
            this.grpSelectVariable.Text = "Select Variable";
            // 
            // chkSyncParty
            // 
            this.chkSyncParty.AutoSize = true;
            this.chkSyncParty.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.chkSyncParty.Location = new System.Drawing.Point(207, 19);
            this.chkSyncParty.Name = "chkSyncParty";
            this.chkSyncParty.Size = new System.Drawing.Size(83, 17);
            this.chkSyncParty.TabIndex = 40;
            this.chkSyncParty.Text = "Party Sync?";
            // 
            // rdoPlayerVariable
            // 
            this.rdoPlayerVariable.AutoSize = true;
            this.rdoPlayerVariable.Checked = true;
            this.rdoPlayerVariable.Location = new System.Drawing.Point(6, 19);
            this.rdoPlayerVariable.Name = "rdoPlayerVariable";
            this.rdoPlayerVariable.Size = new System.Drawing.Size(95, 17);
            this.rdoPlayerVariable.TabIndex = 34;
            this.rdoPlayerVariable.TabStop = true;
            this.rdoPlayerVariable.Text = "Player Variable";
            this.rdoPlayerVariable.CheckedChanged += new System.EventHandler(this.rdoPlayerVariable_CheckedChanged);
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
            this.cmbVariable.Location = new System.Drawing.Point(6, 42);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(284, 21);
            this.cmbVariable.TabIndex = 22;
            this.cmbVariable.Text = null;
            this.cmbVariable.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbVariable.SelectedIndexChanged += new System.EventHandler(this.cmbVariable_SelectedIndexChanged);
            // 
            // rdoGlobalVariable
            // 
            this.rdoGlobalVariable.AutoSize = true;
            this.rdoGlobalVariable.Location = new System.Drawing.Point(105, 19);
            this.rdoGlobalVariable.Name = "rdoGlobalVariable";
            this.rdoGlobalVariable.Size = new System.Drawing.Size(96, 17);
            this.rdoGlobalVariable.TabIndex = 35;
            this.rdoGlobalVariable.Text = "Global Variable";
            this.rdoGlobalVariable.CheckedChanged += new System.EventHandler(this.rdoGlobalVariable_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(221, 301);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 301);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpStringVariable
            // 
            this.grpStringVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpStringVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpStringVariable.Controls.Add(this.lblStringTextVariables);
            this.grpStringVariable.Controls.Add(this.grpStringReplace);
            this.grpStringVariable.Controls.Add(this.optReplaceString);
            this.grpStringVariable.Controls.Add(this.optStaticString);
            this.grpStringVariable.Controls.Add(this.grpStringSet);
            this.grpStringVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpStringVariable.Location = new System.Drawing.Point(6, 100);
            this.grpStringVariable.Name = "grpStringVariable";
            this.grpStringVariable.Size = new System.Drawing.Size(296, 183);
            this.grpStringVariable.TabIndex = 51;
            this.grpStringVariable.TabStop = false;
            this.grpStringVariable.Text = "String Variable:";
            this.grpStringVariable.Visible = false;
            // 
            // lblStringTextVariables
            // 
            this.lblStringTextVariables.AutoSize = true;
            this.lblStringTextVariables.BackColor = System.Drawing.Color.Transparent;
            this.lblStringTextVariables.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStringTextVariables.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.lblStringTextVariables.Location = new System.Drawing.Point(6, 159);
            this.lblStringTextVariables.Name = "lblStringTextVariables";
            this.lblStringTextVariables.Size = new System.Drawing.Size(249, 13);
            this.lblStringTextVariables.TabIndex = 68;
            this.lblStringTextVariables.Text = "Text variables work with strings. Click here for a list!";
            this.lblStringTextVariables.Click += new System.EventHandler(this.lblStringTextVariables_Click);
            // 
            // grpStringReplace
            // 
            this.grpStringReplace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpStringReplace.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpStringReplace.Controls.Add(this.txtStringReplace);
            this.grpStringReplace.Controls.Add(this.txtStringFind);
            this.grpStringReplace.Controls.Add(this.lblStringReplace);
            this.grpStringReplace.Controls.Add(this.lblStringFind);
            this.grpStringReplace.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpStringReplace.Location = new System.Drawing.Point(6, 55);
            this.grpStringReplace.Name = "grpStringReplace";
            this.grpStringReplace.Size = new System.Drawing.Size(284, 90);
            this.grpStringReplace.TabIndex = 65;
            this.grpStringReplace.TabStop = false;
            this.grpStringReplace.Text = "Replace";
            // 
            // txtStringReplace
            // 
            this.txtStringReplace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtStringReplace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStringReplace.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtStringReplace.Location = new System.Drawing.Point(77, 52);
            this.txtStringReplace.Name = "txtStringReplace";
            this.txtStringReplace.Size = new System.Drawing.Size(201, 20);
            this.txtStringReplace.TabIndex = 64;
            // 
            // txtStringFind
            // 
            this.txtStringFind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtStringFind.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStringFind.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtStringFind.Location = new System.Drawing.Point(77, 20);
            this.txtStringFind.Name = "txtStringFind";
            this.txtStringFind.Size = new System.Drawing.Size(201, 20);
            this.txtStringFind.TabIndex = 63;
            // 
            // lblStringReplace
            // 
            this.lblStringReplace.AutoSize = true;
            this.lblStringReplace.Location = new System.Drawing.Point(9, 54);
            this.lblStringReplace.Name = "lblStringReplace";
            this.lblStringReplace.Size = new System.Drawing.Size(47, 13);
            this.lblStringReplace.TabIndex = 39;
            this.lblStringReplace.Text = "Replace";
            // 
            // lblStringFind
            // 
            this.lblStringFind.AutoSize = true;
            this.lblStringFind.Location = new System.Drawing.Point(11, 27);
            this.lblStringFind.Name = "lblStringFind";
            this.lblStringFind.Size = new System.Drawing.Size(27, 13);
            this.lblStringFind.TabIndex = 40;
            this.lblStringFind.Text = "Find";
            // 
            // optReplaceString
            // 
            this.optReplaceString.AutoSize = true;
            this.optReplaceString.Location = new System.Drawing.Point(62, 19);
            this.optReplaceString.Name = "optReplaceString";
            this.optReplaceString.Size = new System.Drawing.Size(65, 17);
            this.optReplaceString.TabIndex = 63;
            this.optReplaceString.Text = "Replace";
            this.optReplaceString.CheckedChanged += new System.EventHandler(this.optReplaceString_CheckedChanged);
            // 
            // optStaticString
            // 
            this.optStaticString.AutoSize = true;
            this.optStaticString.Location = new System.Drawing.Point(9, 19);
            this.optStaticString.Name = "optStaticString";
            this.optStaticString.Size = new System.Drawing.Size(41, 17);
            this.optStaticString.TabIndex = 51;
            this.optStaticString.Text = "Set";
            this.optStaticString.CheckedChanged += new System.EventHandler(this.optStaticString_CheckedChanged);
            // 
            // grpStringSet
            // 
            this.grpStringSet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpStringSet.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpStringSet.Controls.Add(this.lblStringValue);
            this.grpStringSet.Controls.Add(this.txtStringValue);
            this.grpStringSet.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpStringSet.Location = new System.Drawing.Point(6, 55);
            this.grpStringSet.Name = "grpStringSet";
            this.grpStringSet.Size = new System.Drawing.Size(284, 90);
            this.grpStringSet.TabIndex = 67;
            this.grpStringSet.TabStop = false;
            this.grpStringSet.Text = "Set";
            // 
            // lblStringValue
            // 
            this.lblStringValue.AutoSize = true;
            this.lblStringValue.Location = new System.Drawing.Point(6, 28);
            this.lblStringValue.Name = "lblStringValue";
            this.lblStringValue.Size = new System.Drawing.Size(37, 13);
            this.lblStringValue.TabIndex = 66;
            this.lblStringValue.Text = "Value:";
            // 
            // txtStringValue
            // 
            this.txtStringValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtStringValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStringValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtStringValue.Location = new System.Drawing.Point(71, 25);
            this.txtStringValue.Name = "txtStringValue";
            this.txtStringValue.Size = new System.Drawing.Size(207, 20);
            this.txtStringValue.TabIndex = 62;
            // 
            // grpBooleanVariable
            // 
            this.grpBooleanVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpBooleanVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpBooleanVariable.Controls.Add(this.cmbBooleanCloneGlobalVar);
            this.grpBooleanVariable.Controls.Add(this.cmbBooleanClonePlayerVar);
            this.grpBooleanVariable.Controls.Add(this.optBooleanCloneGlobalVar);
            this.grpBooleanVariable.Controls.Add(this.optBooleanClonePlayerVar);
            this.grpBooleanVariable.Controls.Add(this.optBooleanTrue);
            this.grpBooleanVariable.Controls.Add(this.optBooleanFalse);
            this.grpBooleanVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpBooleanVariable.Location = new System.Drawing.Point(6, 100);
            this.grpBooleanVariable.Name = "grpBooleanVariable";
            this.grpBooleanVariable.Size = new System.Drawing.Size(296, 183);
            this.grpBooleanVariable.TabIndex = 40;
            this.grpBooleanVariable.TabStop = false;
            this.grpBooleanVariable.Text = "Boolean Variable:";
            // 
            // cmbBooleanCloneGlobalVar
            // 
            this.cmbBooleanCloneGlobalVar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbBooleanCloneGlobalVar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbBooleanCloneGlobalVar.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbBooleanCloneGlobalVar.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbBooleanCloneGlobalVar.DrawDropdownHoverOutline = false;
            this.cmbBooleanCloneGlobalVar.DrawFocusRectangle = false;
            this.cmbBooleanCloneGlobalVar.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbBooleanCloneGlobalVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBooleanCloneGlobalVar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBooleanCloneGlobalVar.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbBooleanCloneGlobalVar.FormattingEnabled = true;
            this.cmbBooleanCloneGlobalVar.Location = new System.Drawing.Point(146, 103);
            this.cmbBooleanCloneGlobalVar.Name = "cmbBooleanCloneGlobalVar";
            this.cmbBooleanCloneGlobalVar.Size = new System.Drawing.Size(138, 21);
            this.cmbBooleanCloneGlobalVar.TabIndex = 49;
            this.cmbBooleanCloneGlobalVar.Text = null;
            this.cmbBooleanCloneGlobalVar.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbBooleanClonePlayerVar
            // 
            this.cmbBooleanClonePlayerVar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbBooleanClonePlayerVar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbBooleanClonePlayerVar.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbBooleanClonePlayerVar.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbBooleanClonePlayerVar.DrawDropdownHoverOutline = false;
            this.cmbBooleanClonePlayerVar.DrawFocusRectangle = false;
            this.cmbBooleanClonePlayerVar.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbBooleanClonePlayerVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBooleanClonePlayerVar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBooleanClonePlayerVar.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbBooleanClonePlayerVar.FormattingEnabled = true;
            this.cmbBooleanClonePlayerVar.Location = new System.Drawing.Point(146, 71);
            this.cmbBooleanClonePlayerVar.Name = "cmbBooleanClonePlayerVar";
            this.cmbBooleanClonePlayerVar.Size = new System.Drawing.Size(138, 21);
            this.cmbBooleanClonePlayerVar.TabIndex = 48;
            this.cmbBooleanClonePlayerVar.Text = null;
            this.cmbBooleanClonePlayerVar.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // optBooleanCloneGlobalVar
            // 
            this.optBooleanCloneGlobalVar.AutoSize = true;
            this.optBooleanCloneGlobalVar.Location = new System.Drawing.Point(9, 103);
            this.optBooleanCloneGlobalVar.Name = "optBooleanCloneGlobalVar";
            this.optBooleanCloneGlobalVar.Size = new System.Drawing.Size(129, 17);
            this.optBooleanCloneGlobalVar.TabIndex = 47;
            this.optBooleanCloneGlobalVar.Text = "Global Variable Value:";
            // 
            // optBooleanClonePlayerVar
            // 
            this.optBooleanClonePlayerVar.AutoSize = true;
            this.optBooleanClonePlayerVar.Location = new System.Drawing.Point(9, 71);
            this.optBooleanClonePlayerVar.Name = "optBooleanClonePlayerVar";
            this.optBooleanClonePlayerVar.Size = new System.Drawing.Size(128, 17);
            this.optBooleanClonePlayerVar.TabIndex = 46;
            this.optBooleanClonePlayerVar.Text = "Player Variable Value:";
            // 
            // optBooleanTrue
            // 
            this.optBooleanTrue.AutoSize = true;
            this.optBooleanTrue.Location = new System.Drawing.Point(9, 19);
            this.optBooleanTrue.Name = "optBooleanTrue";
            this.optBooleanTrue.Size = new System.Drawing.Size(47, 17);
            this.optBooleanTrue.TabIndex = 26;
            this.optBooleanTrue.Text = "True";
            // 
            // optBooleanFalse
            // 
            this.optBooleanFalse.AutoSize = true;
            this.optBooleanFalse.Location = new System.Drawing.Point(9, 44);
            this.optBooleanFalse.Name = "optBooleanFalse";
            this.optBooleanFalse.Size = new System.Drawing.Size(50, 17);
            this.optBooleanFalse.TabIndex = 25;
            this.optBooleanFalse.Text = "False";
            // 
            // grpNumericVariable
            // 
            this.grpNumericVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpNumericVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpNumericVariable.Controls.Add(this.optNumericRightShift);
            this.grpNumericVariable.Controls.Add(this.optNumericLeftShift);
            this.grpNumericVariable.Controls.Add(this.optNumericDivide);
            this.grpNumericVariable.Controls.Add(this.optNumericMultiply);
            this.grpNumericVariable.Controls.Add(this.optNumericSet);
            this.grpNumericVariable.Controls.Add(this.optNumericAdd);
            this.grpNumericVariable.Controls.Add(this.optNumericRandom);
            this.grpNumericVariable.Controls.Add(this.optNumericSubtract);
            this.grpNumericVariable.Controls.Add(this.optNumericSystemTime);
            this.grpNumericVariable.Controls.Add(this.grpNumericValues);
            this.grpNumericVariable.Controls.Add(this.grpNumericRandom);
            this.grpNumericVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpNumericVariable.Location = new System.Drawing.Point(6, 100);
            this.grpNumericVariable.Name = "grpNumericVariable";
            this.grpNumericVariable.Size = new System.Drawing.Size(296, 183);
            this.grpNumericVariable.TabIndex = 36;
            this.grpNumericVariable.TabStop = false;
            this.grpNumericVariable.Text = "Numeric Variable:";
            // 
            // optNumericSet
            // 
            this.optNumericSet.AutoSize = true;
            this.optNumericSet.Location = new System.Drawing.Point(9, 19);
            this.optNumericSet.Name = "optNumericSet";
            this.optNumericSet.Size = new System.Drawing.Size(41, 17);
            this.optNumericSet.TabIndex = 26;
            this.optNumericSet.Text = "Set";
            this.optNumericSet.CheckedChanged += new System.EventHandler(this.optNumericSet_CheckedChanged);
            // 
            // optNumericAdd
            // 
            this.optNumericAdd.AutoSize = true;
            this.optNumericAdd.Location = new System.Drawing.Point(50, 19);
            this.optNumericAdd.Name = "optNumericAdd";
            this.optNumericAdd.Size = new System.Drawing.Size(44, 17);
            this.optNumericAdd.TabIndex = 25;
            this.optNumericAdd.Text = "Add";
            this.optNumericAdd.CheckedChanged += new System.EventHandler(this.optNumericAdd_CheckedChanged);
            // 
            // optNumericRandom
            // 
            this.optNumericRandom.AutoSize = true;
            this.optNumericRandom.Location = new System.Drawing.Point(116, 42);
            this.optNumericRandom.Name = "optNumericRandom";
            this.optNumericRandom.Size = new System.Drawing.Size(65, 17);
            this.optNumericRandom.TabIndex = 23;
            this.optNumericRandom.Text = "Random";
            this.optNumericRandom.CheckedChanged += new System.EventHandler(this.optNumericRandom_CheckedChanged);
            // 
            // optNumericSubtract
            // 
            this.optNumericSubtract.AutoSize = true;
            this.optNumericSubtract.Location = new System.Drawing.Point(98, 19);
            this.optNumericSubtract.Name = "optNumericSubtract";
            this.optNumericSubtract.Size = new System.Drawing.Size(65, 17);
            this.optNumericSubtract.TabIndex = 24;
            this.optNumericSubtract.Text = "Subtract";
            this.optNumericSubtract.CheckedChanged += new System.EventHandler(this.optNumericSubtract_CheckedChanged);
            // 
            // optNumericSystemTime
            // 
            this.optNumericSystemTime.Location = new System.Drawing.Point(181, 42);
            this.optNumericSystemTime.Name = "optNumericSystemTime";
            this.optNumericSystemTime.Size = new System.Drawing.Size(109, 17);
            this.optNumericSystemTime.TabIndex = 39;
            this.optNumericSystemTime.Text = "System Time (Ms)";
            this.optNumericSystemTime.CheckedChanged += new System.EventHandler(this.optNumericSystemTime_CheckedChanged);
            // 
            // grpNumericValues
            // 
            this.grpNumericValues.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpNumericValues.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpNumericValues.Controls.Add(this.nudNumericValue);
            this.grpNumericValues.Controls.Add(this.cmbNumericCloneGlobalVar);
            this.grpNumericValues.Controls.Add(this.cmbNumericClonePlayerVar);
            this.grpNumericValues.Controls.Add(this.optNumericCloneGlobalVar);
            this.grpNumericValues.Controls.Add(this.optNumericClonePlayerVar);
            this.grpNumericValues.Controls.Add(this.optNumericStaticVal);
            this.grpNumericValues.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpNumericValues.Location = new System.Drawing.Point(6, 71);
            this.grpNumericValues.Name = "grpNumericValues";
            this.grpNumericValues.Size = new System.Drawing.Size(284, 100);
            this.grpNumericValues.TabIndex = 37;
            this.grpNumericValues.TabStop = false;
            // 
            // nudNumericValue
            // 
            this.nudNumericValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.nudNumericValue.ForeColor = System.Drawing.Color.Gainsboro;
            this.nudNumericValue.Location = new System.Drawing.Point(143, 9);
            this.nudNumericValue.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.nudNumericValue.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.nudNumericValue.Name = "nudNumericValue";
            this.nudNumericValue.Size = new System.Drawing.Size(125, 20);
            this.nudNumericValue.TabIndex = 47;
            this.nudNumericValue.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // cmbNumericCloneGlobalVar
            // 
            this.cmbNumericCloneGlobalVar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbNumericCloneGlobalVar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbNumericCloneGlobalVar.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbNumericCloneGlobalVar.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbNumericCloneGlobalVar.DrawDropdownHoverOutline = false;
            this.cmbNumericCloneGlobalVar.DrawFocusRectangle = false;
            this.cmbNumericCloneGlobalVar.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbNumericCloneGlobalVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNumericCloneGlobalVar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNumericCloneGlobalVar.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbNumericCloneGlobalVar.FormattingEnabled = true;
            this.cmbNumericCloneGlobalVar.Location = new System.Drawing.Point(143, 70);
            this.cmbNumericCloneGlobalVar.Name = "cmbNumericCloneGlobalVar";
            this.cmbNumericCloneGlobalVar.Size = new System.Drawing.Size(125, 21);
            this.cmbNumericCloneGlobalVar.TabIndex = 45;
            this.cmbNumericCloneGlobalVar.Text = null;
            this.cmbNumericCloneGlobalVar.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // cmbNumericClonePlayerVar
            // 
            this.cmbNumericClonePlayerVar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbNumericClonePlayerVar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbNumericClonePlayerVar.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbNumericClonePlayerVar.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbNumericClonePlayerVar.DrawDropdownHoverOutline = false;
            this.cmbNumericClonePlayerVar.DrawFocusRectangle = false;
            this.cmbNumericClonePlayerVar.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbNumericClonePlayerVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNumericClonePlayerVar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNumericClonePlayerVar.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbNumericClonePlayerVar.FormattingEnabled = true;
            this.cmbNumericClonePlayerVar.Location = new System.Drawing.Point(143, 38);
            this.cmbNumericClonePlayerVar.Name = "cmbNumericClonePlayerVar";
            this.cmbNumericClonePlayerVar.Size = new System.Drawing.Size(125, 21);
            this.cmbNumericClonePlayerVar.TabIndex = 44;
            this.cmbNumericClonePlayerVar.Text = null;
            this.cmbNumericClonePlayerVar.TextPadding = new System.Windows.Forms.Padding(2);
            // 
            // optNumericCloneGlobalVar
            // 
            this.optNumericCloneGlobalVar.AutoSize = true;
            this.optNumericCloneGlobalVar.Location = new System.Drawing.Point(6, 70);
            this.optNumericCloneGlobalVar.Name = "optNumericCloneGlobalVar";
            this.optNumericCloneGlobalVar.Size = new System.Drawing.Size(129, 17);
            this.optNumericCloneGlobalVar.TabIndex = 43;
            this.optNumericCloneGlobalVar.Text = "Global Variable Value:";
            this.optNumericCloneGlobalVar.CheckedChanged += new System.EventHandler(this.optNumericCloneGlobalVar_CheckedChanged);
            // 
            // optNumericClonePlayerVar
            // 
            this.optNumericClonePlayerVar.AutoSize = true;
            this.optNumericClonePlayerVar.Location = new System.Drawing.Point(6, 38);
            this.optNumericClonePlayerVar.Name = "optNumericClonePlayerVar";
            this.optNumericClonePlayerVar.Size = new System.Drawing.Size(128, 17);
            this.optNumericClonePlayerVar.TabIndex = 42;
            this.optNumericClonePlayerVar.Text = "Player Variable Value:";
            this.optNumericClonePlayerVar.CheckedChanged += new System.EventHandler(this.optNumericClonePlayerVar_CheckedChanged);
            // 
            // optNumericStaticVal
            // 
            this.optNumericStaticVal.AutoSize = true;
            this.optNumericStaticVal.Checked = true;
            this.optNumericStaticVal.Location = new System.Drawing.Point(6, 9);
            this.optNumericStaticVal.Name = "optNumericStaticVal";
            this.optNumericStaticVal.Size = new System.Drawing.Size(85, 17);
            this.optNumericStaticVal.TabIndex = 46;
            this.optNumericStaticVal.TabStop = true;
            this.optNumericStaticVal.Text = "Static Value:";
            // 
            // grpNumericRandom
            // 
            this.grpNumericRandom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpNumericRandom.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpNumericRandom.Controls.Add(this.nudHigh);
            this.grpNumericRandom.Controls.Add(this.nudLow);
            this.grpNumericRandom.Controls.Add(this.lblNumericRandomHigh);
            this.grpNumericRandom.Controls.Add(this.lblNumericRandomLow);
            this.grpNumericRandom.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpNumericRandom.Location = new System.Drawing.Point(6, 74);
            this.grpNumericRandom.Name = "grpNumericRandom";
            this.grpNumericRandom.Size = new System.Drawing.Size(284, 97);
            this.grpNumericRandom.TabIndex = 39;
            this.grpNumericRandom.TabStop = false;
            this.grpNumericRandom.Text = "Random Number:";
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
            this.nudHigh.Size = new System.Drawing.Size(224, 20);
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
            this.nudLow.Size = new System.Drawing.Size(224, 20);
            this.nudLow.TabIndex = 41;
            this.nudLow.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // lblNumericRandomHigh
            // 
            this.lblNumericRandomHigh.AutoSize = true;
            this.lblNumericRandomHigh.Location = new System.Drawing.Point(9, 54);
            this.lblNumericRandomHigh.Name = "lblNumericRandomHigh";
            this.lblNumericRandomHigh.Size = new System.Drawing.Size(29, 13);
            this.lblNumericRandomHigh.TabIndex = 39;
            this.lblNumericRandomHigh.Text = "High";
            // 
            // lblNumericRandomLow
            // 
            this.lblNumericRandomLow.AutoSize = true;
            this.lblNumericRandomLow.Location = new System.Drawing.Point(11, 27);
            this.lblNumericRandomLow.Name = "lblNumericRandomLow";
            this.lblNumericRandomLow.Size = new System.Drawing.Size(27, 13);
            this.lblNumericRandomLow.TabIndex = 40;
            this.lblNumericRandomLow.Text = "Low";
            // 
            // optNumericMultiply
            // 
            this.optNumericMultiply.AutoSize = true;
            this.optNumericMultiply.Location = new System.Drawing.Point(169, 19);
            this.optNumericMultiply.Name = "optNumericMultiply";
            this.optNumericMultiply.Size = new System.Drawing.Size(60, 17);
            this.optNumericMultiply.TabIndex = 40;
            this.optNumericMultiply.Text = "Multiply";
            this.optNumericMultiply.CheckedChanged += new System.EventHandler(this.optNumericMultiply_CheckedChanged);
            // 
            // optNumericDivide
            // 
            this.optNumericDivide.AutoSize = true;
            this.optNumericDivide.Location = new System.Drawing.Point(235, 19);
            this.optNumericDivide.Name = "optNumericDivide";
            this.optNumericDivide.Size = new System.Drawing.Size(55, 17);
            this.optNumericDivide.TabIndex = 41;
            this.optNumericDivide.Text = "Divide";
            this.optNumericDivide.CheckedChanged += new System.EventHandler(this.optNumericDivide_CheckedChanged);
            // 
            // optNumericLeftShift
            // 
            this.optNumericLeftShift.AutoSize = true;
            this.optNumericLeftShift.Location = new System.Drawing.Point(9, 42);
            this.optNumericLeftShift.Name = "optNumericLeftShift";
            this.optNumericLeftShift.Size = new System.Drawing.Size(52, 17);
            this.optNumericLeftShift.TabIndex = 42;
            this.optNumericLeftShift.Text = "LShift";
            this.optNumericLeftShift.CheckedChanged += new System.EventHandler(this.optNumericLeftShift_CheckedChanged);
            // 
            // optNumericRightShift
            // 
            this.optNumericRightShift.AutoSize = true;
            this.optNumericRightShift.Location = new System.Drawing.Point(62, 42);
            this.optNumericRightShift.Name = "optNumericRightShift";
            this.optNumericRightShift.Size = new System.Drawing.Size(54, 17);
            this.optNumericRightShift.TabIndex = 43;
            this.optNumericRightShift.Text = "RShift";
            this.optNumericRightShift.CheckedChanged += new System.EventHandler(this.optNumericRightShift_CheckedChanged);
            // 
            // EventCommandVariable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpSetVariable);
            this.Name = "EventCommandVariable";
            this.Size = new System.Drawing.Size(315, 350);
            this.grpSetVariable.ResumeLayout(false);
            this.grpSelectVariable.ResumeLayout(false);
            this.grpSelectVariable.PerformLayout();
            this.grpStringVariable.ResumeLayout(false);
            this.grpStringVariable.PerformLayout();
            this.grpStringReplace.ResumeLayout(false);
            this.grpStringReplace.PerformLayout();
            this.grpStringSet.ResumeLayout(false);
            this.grpStringSet.PerformLayout();
            this.grpBooleanVariable.ResumeLayout(false);
            this.grpBooleanVariable.PerformLayout();
            this.grpNumericVariable.ResumeLayout(false);
            this.grpNumericVariable.PerformLayout();
            this.grpNumericValues.ResumeLayout(false);
            this.grpNumericValues.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumericValue)).EndInit();
            this.grpNumericRandom.ResumeLayout(false);
            this.grpNumericRandom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHigh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpSetVariable;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        internal DarkRadioButton optNumericRandom;
        internal DarkRadioButton optNumericSubtract;
        internal DarkRadioButton optNumericAdd;
        internal DarkRadioButton optNumericSet;
        internal DarkComboBox cmbVariable;
        private DarkRadioButton rdoGlobalVariable;
        private DarkRadioButton rdoPlayerVariable;
        private DarkGroupBox grpNumericVariable;
        internal DarkRadioButton optNumericSystemTime;
        private DarkGroupBox grpNumericValues;
        private DarkNumericUpDown nudNumericValue;
        internal DarkRadioButton optNumericStaticVal;
        internal DarkComboBox cmbNumericCloneGlobalVar;
        internal DarkComboBox cmbNumericClonePlayerVar;
        internal DarkRadioButton optNumericCloneGlobalVar;
        internal DarkRadioButton optNumericClonePlayerVar;
        private DarkGroupBox grpNumericRandom;
        private DarkNumericUpDown nudHigh;
        private DarkNumericUpDown nudLow;
        internal System.Windows.Forms.Label lblNumericRandomHigh;
        internal System.Windows.Forms.Label lblNumericRandomLow;
        private DarkCheckBox chkSyncParty;
        private DarkGroupBox grpSelectVariable;
        private DarkGroupBox grpBooleanVariable;
        internal DarkComboBox cmbBooleanCloneGlobalVar;
        internal DarkComboBox cmbBooleanClonePlayerVar;
        internal DarkRadioButton optBooleanCloneGlobalVar;
        internal DarkRadioButton optBooleanClonePlayerVar;
        internal DarkRadioButton optBooleanTrue;
        internal DarkRadioButton optBooleanFalse;
        private DarkGroupBox grpStringVariable;
        private DarkTextBox txtStringValue;
        internal System.Windows.Forms.Label lblStringValue;
        private DarkGroupBox grpStringReplace;
        private DarkTextBox txtStringReplace;
        private DarkTextBox txtStringFind;
        internal System.Windows.Forms.Label lblStringReplace;
        internal System.Windows.Forms.Label lblStringFind;
        private DarkRadioButton optReplaceString;
        private DarkRadioButton optStaticString;
        private DarkGroupBox grpStringSet;
        private System.Windows.Forms.Label lblStringTextVariables;
        internal DarkRadioButton optNumericRightShift;
        internal DarkRadioButton optNumericLeftShift;
        internal DarkRadioButton optNumericDivide;
        internal DarkRadioButton optNumericMultiply;
    }
}
