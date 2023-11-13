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
            grpSetVariable = new DarkGroupBox();
            grpSelectVariable = new DarkGroupBox();
            lblVarSelection = new Label();
            lblCurrentVar = new Label();
            btnVarSelector = new DarkButton();
            btnCancel = new DarkButton();
            btnSave = new DarkButton();
            chkSyncParty = new DarkCheckBox();
            grpSettingVariable = new DarkGroupBox();
            lblSettingVarSelection = new Label();
            lblSettingVarCurrentValue = new Label();
            btnSettingVariableSelector = new DarkButton();
            grpBooleanVariable = new DarkGroupBox();
            rdoBoolVariable = new DarkRadioButton();
            optBooleanTrue = new DarkRadioButton();
            optBooleanFalse = new DarkRadioButton();
            grpNumericVariable = new DarkGroupBox();
            optNumericRightShift = new DarkRadioButton();
            optNumericLeftShift = new DarkRadioButton();
            optNumericDivide = new DarkRadioButton();
            optNumericMultiply = new DarkRadioButton();
            optNumericSet = new DarkRadioButton();
            optNumericAdd = new DarkRadioButton();
            optNumericRandom = new DarkRadioButton();
            optNumericSubtract = new DarkRadioButton();
            optNumericSystemTime = new DarkRadioButton();
            grpNumericValues = new DarkGroupBox();
            rdoVariableValue = new DarkRadioButton();
            nudNumericValue = new DarkNumericUpDown();
            optNumericStaticVal = new DarkRadioButton();
            grpNumericRandom = new DarkGroupBox();
            nudHigh = new DarkNumericUpDown();
            nudLow = new DarkNumericUpDown();
            lblNumericRandomHigh = new Label();
            lblNumericRandomLow = new Label();
            grpStringVariable = new DarkGroupBox();
            lblStringTextVariables = new Label();
            grpStringReplace = new DarkGroupBox();
            txtStringReplace = new DarkTextBox();
            txtStringFind = new DarkTextBox();
            lblStringReplace = new Label();
            lblStringFind = new Label();
            optReplaceString = new DarkRadioButton();
            optStaticString = new DarkRadioButton();
            grpStringSet = new DarkGroupBox();
            lblStringValue = new Label();
            txtStringValue = new DarkTextBox();
            grpSetVariable.SuspendLayout();
            grpSelectVariable.SuspendLayout();
            grpSettingVariable.SuspendLayout();
            grpBooleanVariable.SuspendLayout();
            grpNumericVariable.SuspendLayout();
            grpNumericValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudNumericValue).BeginInit();
            grpNumericRandom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudHigh).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudLow).BeginInit();
            grpStringVariable.SuspendLayout();
            grpStringReplace.SuspendLayout();
            grpStringSet.SuspendLayout();
            SuspendLayout();
            // 
            // grpSetVariable
            // 
            grpSetVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpSetVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpSetVariable.Controls.Add(grpSelectVariable);
            grpSetVariable.Controls.Add(btnCancel);
            grpSetVariable.Controls.Add(btnSave);
            grpSetVariable.Controls.Add(chkSyncParty);
            grpSetVariable.Controls.Add(grpSettingVariable);
            grpSetVariable.Controls.Add(grpBooleanVariable);
            grpSetVariable.Controls.Add(grpNumericVariable);
            grpSetVariable.Controls.Add(grpStringVariable);
            grpSetVariable.ForeColor = System.Drawing.Color.Gainsboro;
            grpSetVariable.Location = new System.Drawing.Point(4, 3);
            grpSetVariable.Margin = new Padding(4, 3, 4, 3);
            grpSetVariable.Name = "grpSetVariable";
            grpSetVariable.Padding = new Padding(4, 3, 4, 3);
            grpSetVariable.Size = new Size(361, 430);
            grpSetVariable.TabIndex = 17;
            grpSetVariable.TabStop = false;
            grpSetVariable.Text = "Set Variable";
            // 
            // grpSelectVariable
            // 
            grpSelectVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpSelectVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpSelectVariable.Controls.Add(lblVarSelection);
            grpSelectVariable.Controls.Add(lblCurrentVar);
            grpSelectVariable.Controls.Add(btnVarSelector);
            grpSelectVariable.ForeColor = System.Drawing.Color.Gainsboro;
            grpSelectVariable.Location = new System.Drawing.Point(7, 23);
            grpSelectVariable.Margin = new Padding(4, 3, 4, 3);
            grpSelectVariable.Name = "grpSelectVariable";
            grpSelectVariable.Padding = new Padding(4, 3, 4, 3);
            grpSelectVariable.Size = new Size(345, 88);
            grpSelectVariable.TabIndex = 0;
            grpSelectVariable.TabStop = false;
            grpSelectVariable.Text = "Select Variable";
            // 
            // lblVarSelection
            // 
            lblVarSelection.AutoSize = true;
            lblVarSelection.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblVarSelection.Location = new System.Drawing.Point(20, 67);
            lblVarSelection.Margin = new Padding(4, 0, 4, 0);
            lblVarSelection.Name = "lblVarSelection";
            lblVarSelection.Size = new Size(93, 15);
            lblVarSelection.TabIndex = 42;
            lblVarSelection.Text = "None Selected!";
            // 
            // lblCurrentVar
            // 
            lblCurrentVar.AutoSize = true;
            lblCurrentVar.Location = new System.Drawing.Point(10, 52);
            lblCurrentVar.Margin = new Padding(4, 0, 4, 0);
            lblCurrentVar.Name = "lblCurrentVar";
            lblCurrentVar.Size = new Size(101, 15);
            lblCurrentVar.TabIndex = 41;
            lblCurrentVar.Text = "Current Selection:";
            // 
            // btnVarSelector
            // 
            btnVarSelector.Location = new System.Drawing.Point(8, 22);
            btnVarSelector.Margin = new Padding(4, 3, 4, 3);
            btnVarSelector.Name = "btnVarSelector";
            btnVarSelector.Padding = new Padding(6);
            btnVarSelector.Size = new Size(327, 27);
            btnVarSelector.TabIndex = 22;
            btnVarSelector.Text = "Select Variable";
            btnVarSelector.Click += btnVisual_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(264, 396);
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
            btnSave.Location = new System.Drawing.Point(168, 396);
            btnSave.Margin = new Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Padding = new Padding(6);
            btnSave.Size = new Size(88, 27);
            btnSave.TabIndex = 19;
            btnSave.Text = "Ok";
            btnSave.Click += btnSave_Click;
            // 
            // chkSyncParty
            // 
            chkSyncParty.AutoSize = true;
            chkSyncParty.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            chkSyncParty.Location = new System.Drawing.Point(17, 396);
            chkSyncParty.Margin = new Padding(4, 3, 4, 3);
            chkSyncParty.Name = "chkSyncParty";
            chkSyncParty.Size = new Size(86, 19);
            chkSyncParty.TabIndex = 40;
            chkSyncParty.Text = "Party Sync?";
            // 
            // grpSettingVariable
            // 
            grpSettingVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpSettingVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpSettingVariable.Controls.Add(lblSettingVarSelection);
            grpSettingVariable.Controls.Add(lblSettingVarCurrentValue);
            grpSettingVariable.Controls.Add(btnSettingVariableSelector);
            grpSettingVariable.ForeColor = System.Drawing.Color.Gainsboro;
            grpSettingVariable.Location = new System.Drawing.Point(8, 302);
            grpSettingVariable.Margin = new Padding(4, 3, 4, 3);
            grpSettingVariable.Name = "grpSettingVariable";
            grpSettingVariable.Padding = new Padding(4, 3, 4, 3);
            grpSettingVariable.Size = new Size(345, 88);
            grpSettingVariable.TabIndex = 43;
            grpSettingVariable.TabStop = false;
            grpSettingVariable.Text = "Set to Variable";
            // 
            // lblSettingVarSelection
            // 
            lblSettingVarSelection.AutoSize = true;
            lblSettingVarSelection.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            lblSettingVarSelection.Location = new System.Drawing.Point(21, 67);
            lblSettingVarSelection.Margin = new Padding(4, 0, 4, 0);
            lblSettingVarSelection.Name = "lblSettingVarSelection";
            lblSettingVarSelection.Size = new Size(93, 15);
            lblSettingVarSelection.TabIndex = 42;
            lblSettingVarSelection.Text = "None Selected!";
            // 
            // lblSettingVarCurrentValue
            // 
            lblSettingVarCurrentValue.AutoSize = true;
            lblSettingVarCurrentValue.Location = new System.Drawing.Point(11, 52);
            lblSettingVarCurrentValue.Margin = new Padding(4, 0, 4, 0);
            lblSettingVarCurrentValue.Name = "lblSettingVarCurrentValue";
            lblSettingVarCurrentValue.Size = new Size(101, 15);
            lblSettingVarCurrentValue.TabIndex = 41;
            lblSettingVarCurrentValue.Text = "Current Selection:";
            // 
            // btnSettingVariableSelector
            // 
            btnSettingVariableSelector.Location = new System.Drawing.Point(10, 22);
            btnSettingVariableSelector.Margin = new Padding(4, 3, 4, 3);
            btnSettingVariableSelector.Name = "btnSettingVariableSelector";
            btnSettingVariableSelector.Padding = new Padding(6);
            btnSettingVariableSelector.Size = new Size(325, 27);
            btnSettingVariableSelector.TabIndex = 22;
            btnSettingVariableSelector.Text = "Select Variable";
            btnSettingVariableSelector.Click += btnSettingVariableSelector_Click;
            // 
            // grpBooleanVariable
            // 
            grpBooleanVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpBooleanVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpBooleanVariable.Controls.Add(rdoBoolVariable);
            grpBooleanVariable.Controls.Add(optBooleanTrue);
            grpBooleanVariable.Controls.Add(optBooleanFalse);
            grpBooleanVariable.ForeColor = System.Drawing.Color.Gainsboro;
            grpBooleanVariable.Location = new System.Drawing.Point(7, 123);
            grpBooleanVariable.Margin = new Padding(4, 3, 4, 3);
            grpBooleanVariable.Name = "grpBooleanVariable";
            grpBooleanVariable.Padding = new Padding(4, 3, 4, 3);
            grpBooleanVariable.Size = new Size(345, 57);
            grpBooleanVariable.TabIndex = 1;
            grpBooleanVariable.TabStop = false;
            grpBooleanVariable.Text = "Boolean Variable:";
            // 
            // rdoBoolVariable
            // 
            rdoBoolVariable.AutoSize = true;
            rdoBoolVariable.Location = new System.Drawing.Point(113, 22);
            rdoBoolVariable.Margin = new Padding(4, 3, 4, 3);
            rdoBoolVariable.Name = "rdoBoolVariable";
            rdoBoolVariable.Size = new Size(97, 19);
            rdoBoolVariable.TabIndex = 52;
            rdoBoolVariable.Text = "Variable Value";
            rdoBoolVariable.CheckedChanged += rdoBoolVariable_CheckedChanged;
            // 
            // optBooleanTrue
            // 
            optBooleanTrue.AutoSize = true;
            optBooleanTrue.Location = new System.Drawing.Point(10, 22);
            optBooleanTrue.Margin = new Padding(4, 3, 4, 3);
            optBooleanTrue.Name = "optBooleanTrue";
            optBooleanTrue.Size = new Size(47, 19);
            optBooleanTrue.TabIndex = 26;
            optBooleanTrue.Text = "True";
            optBooleanTrue.CheckedChanged += optBooleanTrue_CheckedChanged;
            // 
            // optBooleanFalse
            // 
            optBooleanFalse.AutoSize = true;
            optBooleanFalse.Location = new System.Drawing.Point(60, 22);
            optBooleanFalse.Margin = new Padding(4, 3, 4, 3);
            optBooleanFalse.Name = "optBooleanFalse";
            optBooleanFalse.Size = new Size(51, 19);
            optBooleanFalse.TabIndex = 25;
            optBooleanFalse.Text = "False";
            optBooleanFalse.CheckedChanged += optBooleanFalse_CheckedChanged;
            // 
            // grpNumericVariable
            // 
            grpNumericVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpNumericVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpNumericVariable.Controls.Add(optNumericRightShift);
            grpNumericVariable.Controls.Add(optNumericLeftShift);
            grpNumericVariable.Controls.Add(optNumericDivide);
            grpNumericVariable.Controls.Add(optNumericMultiply);
            grpNumericVariable.Controls.Add(optNumericSet);
            grpNumericVariable.Controls.Add(optNumericAdd);
            grpNumericVariable.Controls.Add(optNumericRandom);
            grpNumericVariable.Controls.Add(optNumericSubtract);
            grpNumericVariable.Controls.Add(optNumericSystemTime);
            grpNumericVariable.Controls.Add(grpNumericValues);
            grpNumericVariable.Controls.Add(grpNumericRandom);
            grpNumericVariable.ForeColor = System.Drawing.Color.Gainsboro;
            grpNumericVariable.Location = new System.Drawing.Point(7, 117);
            grpNumericVariable.Margin = new Padding(4, 3, 4, 3);
            grpNumericVariable.Name = "grpNumericVariable";
            grpNumericVariable.Padding = new Padding(4, 3, 4, 3);
            grpNumericVariable.Size = new Size(345, 182);
            grpNumericVariable.TabIndex = 3;
            grpNumericVariable.TabStop = false;
            grpNumericVariable.Text = "Numeric Variable:";
            // 
            // optNumericRightShift
            // 
            optNumericRightShift.AutoSize = true;
            optNumericRightShift.Location = new System.Drawing.Point(72, 48);
            optNumericRightShift.Margin = new Padding(4, 3, 4, 3);
            optNumericRightShift.Name = "optNumericRightShift";
            optNumericRightShift.Size = new Size(56, 19);
            optNumericRightShift.TabIndex = 43;
            optNumericRightShift.Text = "RShift";
            optNumericRightShift.CheckedChanged += optNumericRightShift_CheckedChanged;
            // 
            // optNumericLeftShift
            // 
            optNumericLeftShift.AutoSize = true;
            optNumericLeftShift.Location = new System.Drawing.Point(10, 48);
            optNumericLeftShift.Margin = new Padding(4, 3, 4, 3);
            optNumericLeftShift.Name = "optNumericLeftShift";
            optNumericLeftShift.Size = new Size(55, 19);
            optNumericLeftShift.TabIndex = 42;
            optNumericLeftShift.Text = "LShift";
            optNumericLeftShift.CheckedChanged += optNumericLeftShift_CheckedChanged;
            // 
            // optNumericDivide
            // 
            optNumericDivide.AutoSize = true;
            optNumericDivide.Location = new System.Drawing.Point(274, 22);
            optNumericDivide.Margin = new Padding(4, 3, 4, 3);
            optNumericDivide.Name = "optNumericDivide";
            optNumericDivide.Size = new Size(58, 19);
            optNumericDivide.TabIndex = 41;
            optNumericDivide.Text = "Divide";
            optNumericDivide.CheckedChanged += optNumericDivide_CheckedChanged;
            // 
            // optNumericMultiply
            // 
            optNumericMultiply.AutoSize = true;
            optNumericMultiply.Location = new System.Drawing.Point(197, 22);
            optNumericMultiply.Margin = new Padding(4, 3, 4, 3);
            optNumericMultiply.Name = "optNumericMultiply";
            optNumericMultiply.Size = new Size(69, 19);
            optNumericMultiply.TabIndex = 40;
            optNumericMultiply.Text = "Multiply";
            optNumericMultiply.CheckedChanged += optNumericMultiply_CheckedChanged;
            // 
            // optNumericSet
            // 
            optNumericSet.AutoSize = true;
            optNumericSet.Location = new System.Drawing.Point(10, 22);
            optNumericSet.Margin = new Padding(4, 3, 4, 3);
            optNumericSet.Name = "optNumericSet";
            optNumericSet.Size = new Size(41, 19);
            optNumericSet.TabIndex = 26;
            optNumericSet.Text = "Set";
            optNumericSet.CheckedChanged += optNumericSet_CheckedChanged;
            // 
            // optNumericAdd
            // 
            optNumericAdd.AutoSize = true;
            optNumericAdd.Location = new System.Drawing.Point(58, 22);
            optNumericAdd.Margin = new Padding(4, 3, 4, 3);
            optNumericAdd.Name = "optNumericAdd";
            optNumericAdd.Size = new Size(47, 19);
            optNumericAdd.TabIndex = 25;
            optNumericAdd.Text = "Add";
            optNumericAdd.CheckedChanged += optNumericAdd_CheckedChanged;
            // 
            // optNumericRandom
            // 
            optNumericRandom.AutoSize = true;
            optNumericRandom.Location = new System.Drawing.Point(135, 48);
            optNumericRandom.Margin = new Padding(4, 3, 4, 3);
            optNumericRandom.Name = "optNumericRandom";
            optNumericRandom.Size = new Size(70, 19);
            optNumericRandom.TabIndex = 23;
            optNumericRandom.Text = "Random";
            optNumericRandom.CheckedChanged += optNumericRandom_CheckedChanged;
            // 
            // optNumericSubtract
            // 
            optNumericSubtract.AutoSize = true;
            optNumericSubtract.Location = new System.Drawing.Point(114, 22);
            optNumericSubtract.Margin = new Padding(4, 3, 4, 3);
            optNumericSubtract.Name = "optNumericSubtract";
            optNumericSubtract.Size = new Size(69, 19);
            optNumericSubtract.TabIndex = 24;
            optNumericSubtract.Text = "Subtract";
            optNumericSubtract.CheckedChanged += optNumericSubtract_CheckedChanged;
            // 
            // optNumericSystemTime
            // 
            optNumericSystemTime.Location = new System.Drawing.Point(211, 48);
            optNumericSystemTime.Margin = new Padding(4, 3, 4, 3);
            optNumericSystemTime.Name = "optNumericSystemTime";
            optNumericSystemTime.Size = new Size(127, 20);
            optNumericSystemTime.TabIndex = 39;
            optNumericSystemTime.Text = "System Time (Ms)";
            optNumericSystemTime.CheckedChanged += optNumericSystemTime_CheckedChanged;
            // 
            // grpNumericValues
            // 
            grpNumericValues.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpNumericValues.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpNumericValues.Controls.Add(rdoVariableValue);
            grpNumericValues.Controls.Add(nudNumericValue);
            grpNumericValues.Controls.Add(optNumericStaticVal);
            grpNumericValues.ForeColor = System.Drawing.Color.Gainsboro;
            grpNumericValues.Location = new System.Drawing.Point(7, 75);
            grpNumericValues.Margin = new Padding(4, 3, 4, 3);
            grpNumericValues.Name = "grpNumericValues";
            grpNumericValues.Padding = new Padding(4, 3, 4, 3);
            grpNumericValues.Size = new Size(331, 64);
            grpNumericValues.TabIndex = 37;
            grpNumericValues.TabStop = false;
            // 
            // rdoVariableValue
            // 
            rdoVariableValue.AutoSize = true;
            rdoVariableValue.Location = new System.Drawing.Point(7, 39);
            rdoVariableValue.Margin = new Padding(4, 3, 4, 3);
            rdoVariableValue.Name = "rdoVariableValue";
            rdoVariableValue.Size = new Size(97, 19);
            rdoVariableValue.TabIndex = 48;
            rdoVariableValue.Text = "Variable Value";
            rdoVariableValue.CheckedChanged += rdoVariableValue_CheckedChanged;
            // 
            // nudNumericValue
            // 
            nudNumericValue.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudNumericValue.ForeColor = System.Drawing.Color.Gainsboro;
            nudNumericValue.Location = new System.Drawing.Point(167, 10);
            nudNumericValue.Margin = new Padding(4, 3, 4, 3);
            nudNumericValue.Maximum = new decimal(new int[] { -1, -1, -1, 0 });
            nudNumericValue.Minimum = new decimal(new int[] { -1, -1, -1, int.MinValue });
            nudNumericValue.Name = "nudNumericValue";
            nudNumericValue.Size = new Size(146, 23);
            nudNumericValue.TabIndex = 47;
            nudNumericValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // optNumericStaticVal
            // 
            optNumericStaticVal.AutoSize = true;
            optNumericStaticVal.Checked = true;
            optNumericStaticVal.Location = new System.Drawing.Point(7, 10);
            optNumericStaticVal.Margin = new Padding(4, 3, 4, 3);
            optNumericStaticVal.Name = "optNumericStaticVal";
            optNumericStaticVal.Size = new Size(88, 19);
            optNumericStaticVal.TabIndex = 46;
            optNumericStaticVal.TabStop = true;
            optNumericStaticVal.Text = "Static Value:";
            optNumericStaticVal.CheckedChanged += optNumericStaticVal_CheckedChanged;
            // 
            // grpNumericRandom
            // 
            grpNumericRandom.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpNumericRandom.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpNumericRandom.Controls.Add(nudHigh);
            grpNumericRandom.Controls.Add(nudLow);
            grpNumericRandom.Controls.Add(lblNumericRandomHigh);
            grpNumericRandom.Controls.Add(lblNumericRandomLow);
            grpNumericRandom.ForeColor = System.Drawing.Color.Gainsboro;
            grpNumericRandom.Location = new System.Drawing.Point(7, 85);
            grpNumericRandom.Margin = new Padding(4, 3, 4, 3);
            grpNumericRandom.Name = "grpNumericRandom";
            grpNumericRandom.Padding = new Padding(4, 3, 4, 3);
            grpNumericRandom.Size = new Size(331, 95);
            grpNumericRandom.TabIndex = 39;
            grpNumericRandom.TabStop = false;
            grpNumericRandom.Text = "Random Number:";
            // 
            // nudHigh
            // 
            nudHigh.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudHigh.ForeColor = System.Drawing.Color.Gainsboro;
            nudHigh.Location = new System.Drawing.Point(51, 59);
            nudHigh.Margin = new Padding(4, 3, 4, 3);
            nudHigh.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            nudHigh.Minimum = new decimal(new int[] { 1000000000, 0, 0, int.MinValue });
            nudHigh.Name = "nudHigh";
            nudHigh.Size = new Size(261, 23);
            nudHigh.TabIndex = 42;
            nudHigh.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // nudLow
            // 
            nudLow.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            nudLow.ForeColor = System.Drawing.Color.Gainsboro;
            nudLow.Location = new System.Drawing.Point(51, 29);
            nudLow.Margin = new Padding(4, 3, 4, 3);
            nudLow.Maximum = new decimal(new int[] { 1000000000, 0, 0, 0 });
            nudLow.Minimum = new decimal(new int[] { 1000000000, 0, 0, int.MinValue });
            nudLow.Name = "nudLow";
            nudLow.Size = new Size(261, 23);
            nudLow.TabIndex = 41;
            nudLow.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblNumericRandomHigh
            // 
            lblNumericRandomHigh.AutoSize = true;
            lblNumericRandomHigh.Location = new System.Drawing.Point(10, 62);
            lblNumericRandomHigh.Margin = new Padding(4, 0, 4, 0);
            lblNumericRandomHigh.Name = "lblNumericRandomHigh";
            lblNumericRandomHigh.Size = new Size(33, 15);
            lblNumericRandomHigh.TabIndex = 39;
            lblNumericRandomHigh.Text = "High";
            // 
            // lblNumericRandomLow
            // 
            lblNumericRandomLow.AutoSize = true;
            lblNumericRandomLow.Location = new System.Drawing.Point(13, 31);
            lblNumericRandomLow.Margin = new Padding(4, 0, 4, 0);
            lblNumericRandomLow.Name = "lblNumericRandomLow";
            lblNumericRandomLow.Size = new Size(29, 15);
            lblNumericRandomLow.TabIndex = 40;
            lblNumericRandomLow.Text = "Low";
            // 
            // grpStringVariable
            // 
            grpStringVariable.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpStringVariable.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpStringVariable.Controls.Add(lblStringTextVariables);
            grpStringVariable.Controls.Add(grpStringReplace);
            grpStringVariable.Controls.Add(optReplaceString);
            grpStringVariable.Controls.Add(optStaticString);
            grpStringVariable.Controls.Add(grpStringSet);
            grpStringVariable.ForeColor = System.Drawing.Color.Gainsboro;
            grpStringVariable.Location = new System.Drawing.Point(7, 119);
            grpStringVariable.Margin = new Padding(4, 3, 4, 3);
            grpStringVariable.Name = "grpStringVariable";
            grpStringVariable.Padding = new Padding(4, 3, 4, 3);
            grpStringVariable.Size = new Size(345, 211);
            grpStringVariable.TabIndex = 2;
            grpStringVariable.TabStop = false;
            grpStringVariable.Text = "String Variable:";
            grpStringVariable.Visible = false;
            // 
            // lblStringTextVariables
            // 
            lblStringTextVariables.AutoSize = true;
            lblStringTextVariables.BackColor = System.Drawing.Color.Transparent;
            lblStringTextVariables.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Underline, GraphicsUnit.Point);
            lblStringTextVariables.ForeColor = SystemColors.MenuHighlight;
            lblStringTextVariables.Location = new System.Drawing.Point(7, 183);
            lblStringTextVariables.Margin = new Padding(4, 0, 4, 0);
            lblStringTextVariables.Name = "lblStringTextVariables";
            lblStringTextVariables.Size = new Size(249, 13);
            lblStringTextVariables.TabIndex = 68;
            lblStringTextVariables.Text = "Text variables work with strings. Click here for a list!";
            lblStringTextVariables.Click += lblStringTextVariables_Click;
            // 
            // grpStringReplace
            // 
            grpStringReplace.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpStringReplace.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpStringReplace.Controls.Add(txtStringReplace);
            grpStringReplace.Controls.Add(txtStringFind);
            grpStringReplace.Controls.Add(lblStringReplace);
            grpStringReplace.Controls.Add(lblStringFind);
            grpStringReplace.ForeColor = System.Drawing.Color.Gainsboro;
            grpStringReplace.Location = new System.Drawing.Point(7, 44);
            grpStringReplace.Margin = new Padding(4, 3, 4, 3);
            grpStringReplace.Name = "grpStringReplace";
            grpStringReplace.Padding = new Padding(4, 3, 4, 3);
            grpStringReplace.Size = new Size(331, 104);
            grpStringReplace.TabIndex = 65;
            grpStringReplace.TabStop = false;
            grpStringReplace.Text = "Replace";
            // 
            // txtStringReplace
            // 
            txtStringReplace.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtStringReplace.BorderStyle = BorderStyle.FixedSingle;
            txtStringReplace.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtStringReplace.Location = new System.Drawing.Point(90, 60);
            txtStringReplace.Margin = new Padding(4, 3, 4, 3);
            txtStringReplace.Name = "txtStringReplace";
            txtStringReplace.Size = new Size(234, 23);
            txtStringReplace.TabIndex = 64;
            // 
            // txtStringFind
            // 
            txtStringFind.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtStringFind.BorderStyle = BorderStyle.FixedSingle;
            txtStringFind.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtStringFind.Location = new System.Drawing.Point(90, 23);
            txtStringFind.Margin = new Padding(4, 3, 4, 3);
            txtStringFind.Name = "txtStringFind";
            txtStringFind.Size = new Size(234, 23);
            txtStringFind.TabIndex = 63;
            // 
            // lblStringReplace
            // 
            lblStringReplace.AutoSize = true;
            lblStringReplace.Location = new System.Drawing.Point(10, 62);
            lblStringReplace.Margin = new Padding(4, 0, 4, 0);
            lblStringReplace.Name = "lblStringReplace";
            lblStringReplace.Size = new Size(48, 15);
            lblStringReplace.TabIndex = 39;
            lblStringReplace.Text = "Replace";
            // 
            // lblStringFind
            // 
            lblStringFind.AutoSize = true;
            lblStringFind.Location = new System.Drawing.Point(13, 31);
            lblStringFind.Margin = new Padding(4, 0, 4, 0);
            lblStringFind.Name = "lblStringFind";
            lblStringFind.Size = new Size(30, 15);
            lblStringFind.TabIndex = 40;
            lblStringFind.Text = "Find";
            // 
            // optReplaceString
            // 
            optReplaceString.AutoSize = true;
            optReplaceString.Location = new System.Drawing.Point(72, 22);
            optReplaceString.Margin = new Padding(4, 3, 4, 3);
            optReplaceString.Name = "optReplaceString";
            optReplaceString.Size = new Size(66, 19);
            optReplaceString.TabIndex = 63;
            optReplaceString.Text = "Replace";
            optReplaceString.CheckedChanged += optReplaceString_CheckedChanged;
            // 
            // optStaticString
            // 
            optStaticString.AutoSize = true;
            optStaticString.Location = new System.Drawing.Point(10, 22);
            optStaticString.Margin = new Padding(4, 3, 4, 3);
            optStaticString.Name = "optStaticString";
            optStaticString.Size = new Size(41, 19);
            optStaticString.TabIndex = 51;
            optStaticString.Text = "Set";
            optStaticString.CheckedChanged += optStaticString_CheckedChanged;
            // 
            // grpStringSet
            // 
            grpStringSet.BackColor = System.Drawing.Color.FromArgb(60, 63, 65);
            grpStringSet.BorderColor = System.Drawing.Color.FromArgb(90, 90, 90);
            grpStringSet.Controls.Add(lblStringValue);
            grpStringSet.Controls.Add(txtStringValue);
            grpStringSet.ForeColor = System.Drawing.Color.Gainsboro;
            grpStringSet.Location = new System.Drawing.Point(7, 63);
            grpStringSet.Margin = new Padding(4, 3, 4, 3);
            grpStringSet.Name = "grpStringSet";
            grpStringSet.Padding = new Padding(4, 3, 4, 3);
            grpStringSet.Size = new Size(331, 104);
            grpStringSet.TabIndex = 67;
            grpStringSet.TabStop = false;
            grpStringSet.Text = "Set";
            // 
            // lblStringValue
            // 
            lblStringValue.AutoSize = true;
            lblStringValue.Location = new System.Drawing.Point(7, 32);
            lblStringValue.Margin = new Padding(4, 0, 4, 0);
            lblStringValue.Name = "lblStringValue";
            lblStringValue.Size = new Size(38, 15);
            lblStringValue.TabIndex = 66;
            lblStringValue.Text = "Value:";
            // 
            // txtStringValue
            // 
            txtStringValue.BackColor = System.Drawing.Color.FromArgb(69, 73, 74);
            txtStringValue.BorderStyle = BorderStyle.FixedSingle;
            txtStringValue.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            txtStringValue.Location = new System.Drawing.Point(83, 29);
            txtStringValue.Margin = new Padding(4, 3, 4, 3);
            txtStringValue.Name = "txtStringValue";
            txtStringValue.Size = new Size(241, 23);
            txtStringValue.TabIndex = 62;
            // 
            // EventCommandVariable
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            Controls.Add(grpSetVariable);
            Margin = new Padding(4, 3, 4, 3);
            Name = "EventCommandVariable";
            Size = new Size(369, 438);
            grpSetVariable.ResumeLayout(false);
            grpSetVariable.PerformLayout();
            grpSelectVariable.ResumeLayout(false);
            grpSelectVariable.PerformLayout();
            grpSettingVariable.ResumeLayout(false);
            grpSettingVariable.PerformLayout();
            grpBooleanVariable.ResumeLayout(false);
            grpBooleanVariable.PerformLayout();
            grpNumericVariable.ResumeLayout(false);
            grpNumericVariable.PerformLayout();
            grpNumericValues.ResumeLayout(false);
            grpNumericValues.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudNumericValue).EndInit();
            grpNumericRandom.ResumeLayout(false);
            grpNumericRandom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudHigh).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudLow).EndInit();
            grpStringVariable.ResumeLayout(false);
            grpStringVariable.PerformLayout();
            grpStringReplace.ResumeLayout(false);
            grpStringReplace.PerformLayout();
            grpStringSet.ResumeLayout(false);
            grpStringSet.PerformLayout();
            ResumeLayout(false);
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
        private DarkRadioButton rdoGuildVariable;
        private DarkRadioButton rdoPlayerVariable;
        private DarkGroupBox grpNumericVariable;
        internal DarkRadioButton optNumericSystemTime;
        private DarkGroupBox grpNumericValues;
        private DarkNumericUpDown nudNumericValue;
        internal DarkRadioButton optNumericStaticVal;
        private DarkGroupBox grpNumericRandom;
        private DarkNumericUpDown nudHigh;
        private DarkNumericUpDown nudLow;
        internal Label lblNumericRandomHigh;
        internal Label lblNumericRandomLow;
        private DarkCheckBox chkSyncParty;
        private DarkGroupBox grpSelectVariable;
        private DarkGroupBox grpBooleanVariable;
        internal DarkRadioButton optBooleanTrue;
        internal DarkRadioButton optBooleanFalse;
        private DarkGroupBox grpStringVariable;
        private DarkTextBox txtStringValue;
        internal Label lblStringValue;
        private DarkGroupBox grpStringReplace;
        private DarkTextBox txtStringReplace;
        private DarkTextBox txtStringFind;
        internal Label lblStringReplace;
        internal Label lblStringFind;
        private DarkRadioButton optReplaceString;
        private DarkRadioButton optStaticString;
        private DarkGroupBox grpStringSet;
        private Label lblStringTextVariables;
        internal DarkRadioButton optNumericRightShift;
        internal DarkRadioButton optNumericLeftShift;
        internal DarkRadioButton optNumericDivide;
        internal DarkRadioButton optNumericMultiply;
        private DarkRadioButton rdoUserVariable;
        private DarkButton btnVarSelector;
        private Label lblCurrentVar;
        private Label lblVarSelection;
        internal DarkRadioButton rdoBoolVariable;
        private DarkGroupBox grpSettingVariable;
        private Label lblSettingVarSelection;
        private Label lblSettingVarCurrentValue;
        private DarkButton btnSettingVariableSelector;
        internal DarkRadioButton rdoVariableValue;
    }
}
