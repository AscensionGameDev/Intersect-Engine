using DarkUI.Controls;

namespace Intersect_Editor.Forms.Editors.Event_Commands
{
    partial class EventCommand_ConditionalBranch
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
            this.grpConditional = new DarkUI.Controls.DarkGroupBox();
            this.grpGender = new DarkUI.Controls.DarkGroupBox();
            this.cmbGender = new DarkUI.Controls.DarkComboBox();
            this.lblGender = new System.Windows.Forms.Label();
            this.grpPowerIs = new DarkUI.Controls.DarkGroupBox();
            this.cmbPower = new DarkUI.Controls.DarkComboBox();
            this.lblPower = new System.Windows.Forms.Label();
            this.grpSelfSwitch = new DarkUI.Controls.DarkGroupBox();
            this.cmbSelfSwitchVal = new DarkUI.Controls.DarkComboBox();
            this.lblSelfSwitchIs = new System.Windows.Forms.Label();
            this.cmbSelfSwitch = new DarkUI.Controls.DarkComboBox();
            this.lblSelfSwitch = new System.Windows.Forms.Label();
            this.grpSpell = new DarkUI.Controls.DarkGroupBox();
            this.cmbSpell = new DarkUI.Controls.DarkComboBox();
            this.lblSpell = new System.Windows.Forms.Label();
            this.grpClass = new DarkUI.Controls.DarkGroupBox();
            this.cmbClass = new DarkUI.Controls.DarkComboBox();
            this.lblClass = new System.Windows.Forms.Label();
            this.btnSave = new DarkUI.Controls.DarkButton();
            this.cmbConditionType = new DarkUI.Controls.DarkComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.grpHasItem = new DarkUI.Controls.DarkGroupBox();
            this.scrlItemQuantity = new DarkUI.Controls.DarkScrollBar();
            this.cmbItem = new DarkUI.Controls.DarkComboBox();
            this.lblItem = new System.Windows.Forms.Label();
            this.lblItemQuantity = new System.Windows.Forms.Label();
            this.grpSwitch = new DarkUI.Controls.DarkGroupBox();
            this.cmbSwitchVal = new DarkUI.Controls.DarkComboBox();
            this.lblSwitchIs = new System.Windows.Forms.Label();
            this.cmbSwitch = new DarkUI.Controls.DarkComboBox();
            this.lblSwitch = new System.Windows.Forms.Label();
            this.grpPlayerVariable = new DarkUI.Controls.DarkGroupBox();
            this.txtVariableVal = new DarkUI.Controls.DarkTextBox();
            this.lblVariableValue = new System.Windows.Forms.Label();
            this.cmbVariableMod = new DarkUI.Controls.DarkComboBox();
            this.lblComparator = new System.Windows.Forms.Label();
            this.cmbVariable = new DarkUI.Controls.DarkComboBox();
            this.lblVariable = new System.Windows.Forms.Label();
            this.grpQuestCompleted = new DarkUI.Controls.DarkGroupBox();
            this.lblQuestCompleted = new System.Windows.Forms.Label();
            this.cmbCompletedQuest = new DarkUI.Controls.DarkComboBox();
            this.grpQuestInProgress = new DarkUI.Controls.DarkGroupBox();
            this.lblQuestTask = new System.Windows.Forms.Label();
            this.cmbQuestTask = new DarkUI.Controls.DarkComboBox();
            this.cmbTaskModifier = new DarkUI.Controls.DarkComboBox();
            this.lblQuestIs = new System.Windows.Forms.Label();
            this.lblQuestProgress = new System.Windows.Forms.Label();
            this.cmbQuestInProgress = new DarkUI.Controls.DarkComboBox();
            this.grpStartQuest = new DarkUI.Controls.DarkGroupBox();
            this.lblStartQuest = new System.Windows.Forms.Label();
            this.cmbStartQuest = new DarkUI.Controls.DarkComboBox();
            this.grpLevelStat = new DarkUI.Controls.DarkGroupBox();
            this.cmbLevelStat = new DarkUI.Controls.DarkComboBox();
            this.lblLevelOrStat = new System.Windows.Forms.Label();
            this.scrlLevel = new DarkUI.Controls.DarkScrollBar();
            this.lblLvlStatValue = new System.Windows.Forms.Label();
            this.cmbLevelComparator = new DarkUI.Controls.DarkComboBox();
            this.lblLevelComparator = new System.Windows.Forms.Label();
            this.grpTime = new DarkUI.Controls.DarkGroupBox();
            this.lblEndRange = new System.Windows.Forms.Label();
            this.lblStartRange = new System.Windows.Forms.Label();
            this.cmbTime2 = new DarkUI.Controls.DarkComboBox();
            this.cmbTime1 = new DarkUI.Controls.DarkComboBox();
            this.lblAnd = new System.Windows.Forms.Label();
            this.grpConditional.SuspendLayout();
            this.grpGender.SuspendLayout();
            this.grpPowerIs.SuspendLayout();
            this.grpSelfSwitch.SuspendLayout();
            this.grpSpell.SuspendLayout();
            this.grpClass.SuspendLayout();
            this.grpHasItem.SuspendLayout();
            this.grpSwitch.SuspendLayout();
            this.grpPlayerVariable.SuspendLayout();
            this.grpQuestCompleted.SuspendLayout();
            this.grpQuestInProgress.SuspendLayout();
            this.grpStartQuest.SuspendLayout();
            this.grpLevelStat.SuspendLayout();
            this.grpTime.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConditional
            // 
            this.grpConditional.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpConditional.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpConditional.Controls.Add(this.grpGender);
            this.grpConditional.Controls.Add(this.grpPowerIs);
            this.grpConditional.Controls.Add(this.grpSelfSwitch);
            this.grpConditional.Controls.Add(this.grpSpell);
            this.grpConditional.Controls.Add(this.grpClass);
            this.grpConditional.Controls.Add(this.btnSave);
            this.grpConditional.Controls.Add(this.cmbConditionType);
            this.grpConditional.Controls.Add(this.lblType);
            this.grpConditional.Controls.Add(this.btnCancel);
            this.grpConditional.Controls.Add(this.grpHasItem);
            this.grpConditional.Controls.Add(this.grpSwitch);
            this.grpConditional.Controls.Add(this.grpPlayerVariable);
            this.grpConditional.Controls.Add(this.grpQuestCompleted);
            this.grpConditional.Controls.Add(this.grpQuestInProgress);
            this.grpConditional.Controls.Add(this.grpStartQuest);
            this.grpConditional.Controls.Add(this.grpLevelStat);
            this.grpConditional.Controls.Add(this.grpTime);
            this.grpConditional.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpConditional.Location = new System.Drawing.Point(3, 3);
            this.grpConditional.Name = "grpConditional";
            this.grpConditional.Size = new System.Drawing.Size(256, 202);
            this.grpConditional.TabIndex = 17;
            this.grpConditional.TabStop = false;
            this.grpConditional.Text = "Conditional";
            // 
            // grpGender
            // 
            this.grpGender.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpGender.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpGender.Controls.Add(this.cmbGender);
            this.grpGender.Controls.Add(this.lblGender);
            this.grpGender.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpGender.Location = new System.Drawing.Point(10, 42);
            this.grpGender.Name = "grpGender";
            this.grpGender.Size = new System.Drawing.Size(236, 51);
            this.grpGender.TabIndex = 33;
            this.grpGender.TabStop = false;
            this.grpGender.Text = "Gender Is...";
            // 
            // cmbGender
            // 
            this.cmbGender.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbGender.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbGender.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbGender.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbGender.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbGender.FormattingEnabled = true;
            this.cmbGender.Location = new System.Drawing.Point(79, 17);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(141, 21);
            this.cmbGender.TabIndex = 1;
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Location = new System.Drawing.Point(7, 20);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(45, 13);
            this.lblGender.TabIndex = 0;
            this.lblGender.Text = "Gender:";
            // 
            // grpPowerIs
            // 
            this.grpPowerIs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpPowerIs.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpPowerIs.Controls.Add(this.cmbPower);
            this.grpPowerIs.Controls.Add(this.lblPower);
            this.grpPowerIs.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpPowerIs.Location = new System.Drawing.Point(9, 41);
            this.grpPowerIs.Name = "grpPowerIs";
            this.grpPowerIs.Size = new System.Drawing.Size(236, 51);
            this.grpPowerIs.TabIndex = 25;
            this.grpPowerIs.TabStop = false;
            this.grpPowerIs.Text = "Power Is...";
            // 
            // cmbPower
            // 
            this.cmbPower.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbPower.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbPower.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbPower.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbPower.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPower.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPower.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbPower.FormattingEnabled = true;
            this.cmbPower.Location = new System.Drawing.Point(79, 17);
            this.cmbPower.Name = "cmbPower";
            this.cmbPower.Size = new System.Drawing.Size(141, 21);
            this.cmbPower.TabIndex = 1;
            // 
            // lblPower
            // 
            this.lblPower.AutoSize = true;
            this.lblPower.Location = new System.Drawing.Point(7, 20);
            this.lblPower.Name = "lblPower";
            this.lblPower.Size = new System.Drawing.Size(40, 13);
            this.lblPower.TabIndex = 0;
            this.lblPower.Text = "Power:";
            // 
            // grpSelfSwitch
            // 
            this.grpSelfSwitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSelfSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSelfSwitch.Controls.Add(this.cmbSelfSwitchVal);
            this.grpSelfSwitch.Controls.Add(this.lblSelfSwitchIs);
            this.grpSelfSwitch.Controls.Add(this.cmbSelfSwitch);
            this.grpSelfSwitch.Controls.Add(this.lblSelfSwitch);
            this.grpSelfSwitch.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSelfSwitch.Location = new System.Drawing.Point(9, 40);
            this.grpSelfSwitch.Name = "grpSelfSwitch";
            this.grpSelfSwitch.Size = new System.Drawing.Size(236, 89);
            this.grpSelfSwitch.TabIndex = 29;
            this.grpSelfSwitch.TabStop = false;
            this.grpSelfSwitch.Text = "Self Switch";
            // 
            // cmbSelfSwitchVal
            // 
            this.cmbSelfSwitchVal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSelfSwitchVal.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSelfSwitchVal.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSelfSwitchVal.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSelfSwitchVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelfSwitchVal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSelfSwitchVal.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSelfSwitchVal.FormattingEnabled = true;
            this.cmbSelfSwitchVal.Location = new System.Drawing.Point(79, 52);
            this.cmbSelfSwitchVal.Name = "cmbSelfSwitchVal";
            this.cmbSelfSwitchVal.Size = new System.Drawing.Size(141, 21);
            this.cmbSelfSwitchVal.TabIndex = 3;
            // 
            // lblSelfSwitchIs
            // 
            this.lblSelfSwitchIs.AutoSize = true;
            this.lblSelfSwitchIs.Location = new System.Drawing.Point(10, 55);
            this.lblSelfSwitchIs.Name = "lblSelfSwitchIs";
            this.lblSelfSwitchIs.Size = new System.Drawing.Size(21, 13);
            this.lblSelfSwitchIs.TabIndex = 2;
            this.lblSelfSwitchIs.Text = "Is: ";
            // 
            // cmbSelfSwitch
            // 
            this.cmbSelfSwitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSelfSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSelfSwitch.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSelfSwitch.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSelfSwitch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelfSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSelfSwitch.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSelfSwitch.FormattingEnabled = true;
            this.cmbSelfSwitch.Location = new System.Drawing.Point(79, 17);
            this.cmbSelfSwitch.Name = "cmbSelfSwitch";
            this.cmbSelfSwitch.Size = new System.Drawing.Size(141, 21);
            this.cmbSelfSwitch.TabIndex = 1;
            // 
            // lblSelfSwitch
            // 
            this.lblSelfSwitch.AutoSize = true;
            this.lblSelfSwitch.Location = new System.Drawing.Point(7, 20);
            this.lblSelfSwitch.Name = "lblSelfSwitch";
            this.lblSelfSwitch.Size = new System.Drawing.Size(66, 13);
            this.lblSelfSwitch.TabIndex = 0;
            this.lblSelfSwitch.Text = "Self Switch: ";
            // 
            // grpSpell
            // 
            this.grpSpell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSpell.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSpell.Controls.Add(this.cmbSpell);
            this.grpSpell.Controls.Add(this.lblSpell);
            this.grpSpell.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSpell.Location = new System.Drawing.Point(9, 40);
            this.grpSpell.Name = "grpSpell";
            this.grpSpell.Size = new System.Drawing.Size(236, 52);
            this.grpSpell.TabIndex = 26;
            this.grpSpell.TabStop = false;
            this.grpSpell.Text = "Knows Spell";
            // 
            // cmbSpell
            // 
            this.cmbSpell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSpell.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSpell.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSpell.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSpell.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSpell.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSpell.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSpell.FormattingEnabled = true;
            this.cmbSpell.Location = new System.Drawing.Point(79, 18);
            this.cmbSpell.Name = "cmbSpell";
            this.cmbSpell.Size = new System.Drawing.Size(151, 21);
            this.cmbSpell.TabIndex = 3;
            // 
            // lblSpell
            // 
            this.lblSpell.AutoSize = true;
            this.lblSpell.Location = new System.Drawing.Point(7, 20);
            this.lblSpell.Name = "lblSpell";
            this.lblSpell.Size = new System.Drawing.Size(33, 13);
            this.lblSpell.TabIndex = 2;
            this.lblSpell.Text = "Spell:";
            // 
            // grpClass
            // 
            this.grpClass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpClass.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpClass.Controls.Add(this.cmbClass);
            this.grpClass.Controls.Add(this.lblClass);
            this.grpClass.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpClass.Location = new System.Drawing.Point(9, 40);
            this.grpClass.Name = "grpClass";
            this.grpClass.Size = new System.Drawing.Size(236, 52);
            this.grpClass.TabIndex = 27;
            this.grpClass.TabStop = false;
            this.grpClass.Text = "Class is";
            // 
            // cmbClass
            // 
            this.cmbClass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbClass.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbClass.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbClass.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClass.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbClass.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbClass.FormattingEnabled = true;
            this.cmbClass.Location = new System.Drawing.Point(79, 18);
            this.cmbClass.Name = "cmbClass";
            this.cmbClass.Size = new System.Drawing.Size(151, 21);
            this.cmbClass.TabIndex = 3;
            // 
            // lblClass
            // 
            this.lblClass.AutoSize = true;
            this.lblClass.Location = new System.Drawing.Point(7, 20);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(35, 13);
            this.lblClass.TabIndex = 2;
            this.lblClass.Text = "Class:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(9, 168);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5);
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Ok";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbConditionType
            // 
            this.cmbConditionType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbConditionType.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbConditionType.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbConditionType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbConditionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConditionType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbConditionType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbConditionType.FormattingEnabled = true;
            this.cmbConditionType.Items.AddRange(new object[] {
            "Player Switch is...",
            "Player Variable is...",
            "Global Switch is...",
            "Global Variable is...",
            "Has item...",
            "Class is...",
            "Knows spell...",
            "Level is....",
            "Self Switch is....",
            "Power level is....",
            "Time is between....",
            "Can Start Quest....",
            "Quest In Progress....",
            "Quest Completed....",
            "Player death...",
            "No NPCs on the map..."});
            this.cmbConditionType.Location = new System.Drawing.Point(88, 13);
            this.cmbConditionType.Name = "cmbConditionType";
            this.cmbConditionType.Size = new System.Drawing.Size(157, 21);
            this.cmbConditionType.TabIndex = 22;
            this.cmbConditionType.SelectedIndexChanged += new System.EventHandler(this.cmbConditionType_SelectedIndexChanged);
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(6, 16);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(81, 13);
            this.lblType.TabIndex = 21;
            this.lblType.Text = "Condition Type:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(90, 168);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpHasItem
            // 
            this.grpHasItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpHasItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpHasItem.Controls.Add(this.scrlItemQuantity);
            this.grpHasItem.Controls.Add(this.cmbItem);
            this.grpHasItem.Controls.Add(this.lblItem);
            this.grpHasItem.Controls.Add(this.lblItemQuantity);
            this.grpHasItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpHasItem.Location = new System.Drawing.Point(9, 40);
            this.grpHasItem.Name = "grpHasItem";
            this.grpHasItem.Size = new System.Drawing.Size(236, 83);
            this.grpHasItem.TabIndex = 25;
            this.grpHasItem.TabStop = false;
            this.grpHasItem.Text = "Has Item";
            // 
            // scrlItemQuantity
            // 
            this.scrlItemQuantity.Location = new System.Drawing.Point(104, 18);
            this.scrlItemQuantity.Maximum = 32000;
            this.scrlItemQuantity.Minimum = 1;
            this.scrlItemQuantity.Name = "scrlItemQuantity";
            this.scrlItemQuantity.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlItemQuantity.Size = new System.Drawing.Size(116, 17);
            this.scrlItemQuantity.TabIndex = 4;
            this.scrlItemQuantity.Value = 1;
            this.scrlItemQuantity.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlItemQuantity_Scroll);
            // 
            // cmbItem
            // 
            this.cmbItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbItem.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Location = new System.Drawing.Point(104, 52);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(116, 21);
            this.cmbItem.TabIndex = 3;
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(7, 55);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(30, 13);
            this.lblItem.TabIndex = 2;
            this.lblItem.Text = "Item:";
            // 
            // lblItemQuantity
            // 
            this.lblItemQuantity.AutoSize = true;
            this.lblItemQuantity.Location = new System.Drawing.Point(7, 20);
            this.lblItemQuantity.Name = "lblItemQuantity";
            this.lblItemQuantity.Size = new System.Drawing.Size(75, 13);
            this.lblItemQuantity.TabIndex = 0;
            this.lblItemQuantity.Text = "Has at least: 1";
            // 
            // grpSwitch
            // 
            this.grpSwitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpSwitch.Controls.Add(this.cmbSwitchVal);
            this.grpSwitch.Controls.Add(this.lblSwitchIs);
            this.grpSwitch.Controls.Add(this.cmbSwitch);
            this.grpSwitch.Controls.Add(this.lblSwitch);
            this.grpSwitch.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpSwitch.Location = new System.Drawing.Point(9, 40);
            this.grpSwitch.Name = "grpSwitch";
            this.grpSwitch.Size = new System.Drawing.Size(236, 89);
            this.grpSwitch.TabIndex = 23;
            this.grpSwitch.TabStop = false;
            this.grpSwitch.Text = "Player Switch";
            // 
            // cmbSwitchVal
            // 
            this.cmbSwitchVal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSwitchVal.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSwitchVal.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSwitchVal.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSwitchVal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSwitchVal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSwitchVal.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSwitchVal.FormattingEnabled = true;
            this.cmbSwitchVal.Items.AddRange(new object[] {
            "False",
            "True"});
            this.cmbSwitchVal.Location = new System.Drawing.Point(79, 52);
            this.cmbSwitchVal.Name = "cmbSwitchVal";
            this.cmbSwitchVal.Size = new System.Drawing.Size(141, 21);
            this.cmbSwitchVal.TabIndex = 3;
            // 
            // lblSwitchIs
            // 
            this.lblSwitchIs.AutoSize = true;
            this.lblSwitchIs.Location = new System.Drawing.Point(10, 55);
            this.lblSwitchIs.Name = "lblSwitchIs";
            this.lblSwitchIs.Size = new System.Drawing.Size(21, 13);
            this.lblSwitchIs.TabIndex = 2;
            this.lblSwitchIs.Text = "Is: ";
            // 
            // cmbSwitch
            // 
            this.cmbSwitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbSwitch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbSwitch.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbSwitch.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSwitch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSwitch.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbSwitch.FormattingEnabled = true;
            this.cmbSwitch.Location = new System.Drawing.Point(79, 17);
            this.cmbSwitch.Name = "cmbSwitch";
            this.cmbSwitch.Size = new System.Drawing.Size(141, 21);
            this.cmbSwitch.TabIndex = 1;
            // 
            // lblSwitch
            // 
            this.lblSwitch.AutoSize = true;
            this.lblSwitch.Location = new System.Drawing.Point(7, 20);
            this.lblSwitch.Name = "lblSwitch";
            this.lblSwitch.Size = new System.Drawing.Size(45, 13);
            this.lblSwitch.TabIndex = 0;
            this.lblSwitch.Text = "Switch: ";
            // 
            // grpPlayerVariable
            // 
            this.grpPlayerVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpPlayerVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpPlayerVariable.Controls.Add(this.txtVariableVal);
            this.grpPlayerVariable.Controls.Add(this.lblVariableValue);
            this.grpPlayerVariable.Controls.Add(this.cmbVariableMod);
            this.grpPlayerVariable.Controls.Add(this.lblComparator);
            this.grpPlayerVariable.Controls.Add(this.cmbVariable);
            this.grpPlayerVariable.Controls.Add(this.lblVariable);
            this.grpPlayerVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpPlayerVariable.Location = new System.Drawing.Point(9, 40);
            this.grpPlayerVariable.Name = "grpPlayerVariable";
            this.grpPlayerVariable.Size = new System.Drawing.Size(236, 122);
            this.grpPlayerVariable.TabIndex = 24;
            this.grpPlayerVariable.TabStop = false;
            this.grpPlayerVariable.Text = "Player Variable";
            // 
            // txtVariableVal
            // 
            this.txtVariableVal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtVariableVal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVariableVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtVariableVal.Location = new System.Drawing.Point(79, 89);
            this.txtVariableVal.Name = "txtVariableVal";
            this.txtVariableVal.Size = new System.Drawing.Size(141, 20);
            this.txtVariableVal.TabIndex = 5;
            // 
            // lblVariableValue
            // 
            this.lblVariableValue.AutoSize = true;
            this.lblVariableValue.Location = new System.Drawing.Point(7, 92);
            this.lblVariableValue.Name = "lblVariableValue";
            this.lblVariableValue.Size = new System.Drawing.Size(37, 13);
            this.lblVariableValue.TabIndex = 4;
            this.lblVariableValue.Text = "Value:";
            // 
            // cmbVariableMod
            // 
            this.cmbVariableMod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbVariableMod.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbVariableMod.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbVariableMod.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbVariableMod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariableMod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbVariableMod.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbVariableMod.FormattingEnabled = true;
            this.cmbVariableMod.Items.AddRange(new object[] {
            "Equal To",
            "Greater Than or Equal To",
            "Less Than or Equal To",
            "Greater Than",
            "Less Than",
            "Does Not Equal"});
            this.cmbVariableMod.Location = new System.Drawing.Point(79, 52);
            this.cmbVariableMod.Name = "cmbVariableMod";
            this.cmbVariableMod.Size = new System.Drawing.Size(141, 21);
            this.cmbVariableMod.TabIndex = 3;
            // 
            // lblComparator
            // 
            this.lblComparator.AutoSize = true;
            this.lblComparator.Location = new System.Drawing.Point(7, 55);
            this.lblComparator.Name = "lblComparator";
            this.lblComparator.Size = new System.Drawing.Size(61, 13);
            this.lblComparator.TabIndex = 2;
            this.lblComparator.Text = "Comparator";
            // 
            // cmbVariable
            // 
            this.cmbVariable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbVariable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbVariable.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbVariable.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbVariable.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbVariable.FormattingEnabled = true;
            this.cmbVariable.Location = new System.Drawing.Point(79, 17);
            this.cmbVariable.Name = "cmbVariable";
            this.cmbVariable.Size = new System.Drawing.Size(141, 21);
            this.cmbVariable.TabIndex = 1;
            // 
            // lblVariable
            // 
            this.lblVariable.AutoSize = true;
            this.lblVariable.Location = new System.Drawing.Point(7, 20);
            this.lblVariable.Name = "lblVariable";
            this.lblVariable.Size = new System.Drawing.Size(45, 13);
            this.lblVariable.TabIndex = 0;
            this.lblVariable.Text = "Variable";
            // 
            // grpQuestCompleted
            // 
            this.grpQuestCompleted.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpQuestCompleted.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpQuestCompleted.Controls.Add(this.lblQuestCompleted);
            this.grpQuestCompleted.Controls.Add(this.cmbCompletedQuest);
            this.grpQuestCompleted.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpQuestCompleted.Location = new System.Drawing.Point(9, 40);
            this.grpQuestCompleted.Name = "grpQuestCompleted";
            this.grpQuestCompleted.Size = new System.Drawing.Size(236, 71);
            this.grpQuestCompleted.TabIndex = 32;
            this.grpQuestCompleted.TabStop = false;
            this.grpQuestCompleted.Text = "Quest Completed:";
            this.grpQuestCompleted.Visible = false;
            // 
            // lblQuestCompleted
            // 
            this.lblQuestCompleted.AutoSize = true;
            this.lblQuestCompleted.Location = new System.Drawing.Point(6, 21);
            this.lblQuestCompleted.Name = "lblQuestCompleted";
            this.lblQuestCompleted.Size = new System.Drawing.Size(38, 13);
            this.lblQuestCompleted.TabIndex = 5;
            this.lblQuestCompleted.Text = "Quest:";
            // 
            // cmbCompletedQuest
            // 
            this.cmbCompletedQuest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbCompletedQuest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbCompletedQuest.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbCompletedQuest.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbCompletedQuest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCompletedQuest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbCompletedQuest.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbCompletedQuest.FormattingEnabled = true;
            this.cmbCompletedQuest.Location = new System.Drawing.Point(92, 18);
            this.cmbCompletedQuest.Name = "cmbCompletedQuest";
            this.cmbCompletedQuest.Size = new System.Drawing.Size(138, 21);
            this.cmbCompletedQuest.TabIndex = 3;
            // 
            // grpQuestInProgress
            // 
            this.grpQuestInProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpQuestInProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpQuestInProgress.Controls.Add(this.lblQuestTask);
            this.grpQuestInProgress.Controls.Add(this.cmbQuestTask);
            this.grpQuestInProgress.Controls.Add(this.cmbTaskModifier);
            this.grpQuestInProgress.Controls.Add(this.lblQuestIs);
            this.grpQuestInProgress.Controls.Add(this.lblQuestProgress);
            this.grpQuestInProgress.Controls.Add(this.cmbQuestInProgress);
            this.grpQuestInProgress.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpQuestInProgress.Location = new System.Drawing.Point(8, 40);
            this.grpQuestInProgress.Name = "grpQuestInProgress";
            this.grpQuestInProgress.Size = new System.Drawing.Size(236, 122);
            this.grpQuestInProgress.TabIndex = 32;
            this.grpQuestInProgress.TabStop = false;
            this.grpQuestInProgress.Text = "Quest In Progress:";
            this.grpQuestInProgress.Visible = false;
            // 
            // lblQuestTask
            // 
            this.lblQuestTask.AutoSize = true;
            this.lblQuestTask.Location = new System.Drawing.Point(6, 86);
            this.lblQuestTask.Name = "lblQuestTask";
            this.lblQuestTask.Size = new System.Drawing.Size(34, 13);
            this.lblQuestTask.TabIndex = 9;
            this.lblQuestTask.Text = "Task:";
            // 
            // cmbQuestTask
            // 
            this.cmbQuestTask.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbQuestTask.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbQuestTask.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbQuestTask.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbQuestTask.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuestTask.Enabled = false;
            this.cmbQuestTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbQuestTask.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbQuestTask.FormattingEnabled = true;
            this.cmbQuestTask.Location = new System.Drawing.Point(92, 83);
            this.cmbQuestTask.Name = "cmbQuestTask";
            this.cmbQuestTask.Size = new System.Drawing.Size(138, 21);
            this.cmbQuestTask.TabIndex = 8;
            // 
            // cmbTaskModifier
            // 
            this.cmbTaskModifier.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTaskModifier.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTaskModifier.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTaskModifier.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTaskModifier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTaskModifier.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTaskModifier.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTaskModifier.FormattingEnabled = true;
            this.cmbTaskModifier.Location = new System.Drawing.Point(92, 50);
            this.cmbTaskModifier.Name = "cmbTaskModifier";
            this.cmbTaskModifier.Size = new System.Drawing.Size(138, 21);
            this.cmbTaskModifier.TabIndex = 7;
            this.cmbTaskModifier.SelectedIndexChanged += new System.EventHandler(this.cmbTaskModifier_SelectedIndexChanged);
            // 
            // lblQuestIs
            // 
            this.lblQuestIs.AutoSize = true;
            this.lblQuestIs.Location = new System.Drawing.Point(6, 52);
            this.lblQuestIs.Name = "lblQuestIs";
            this.lblQuestIs.Size = new System.Drawing.Size(18, 13);
            this.lblQuestIs.TabIndex = 6;
            this.lblQuestIs.Text = "Is:";
            // 
            // lblQuestProgress
            // 
            this.lblQuestProgress.AutoSize = true;
            this.lblQuestProgress.Location = new System.Drawing.Point(6, 21);
            this.lblQuestProgress.Name = "lblQuestProgress";
            this.lblQuestProgress.Size = new System.Drawing.Size(38, 13);
            this.lblQuestProgress.TabIndex = 5;
            this.lblQuestProgress.Text = "Quest:";
            // 
            // cmbQuestInProgress
            // 
            this.cmbQuestInProgress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbQuestInProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbQuestInProgress.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbQuestInProgress.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbQuestInProgress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuestInProgress.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbQuestInProgress.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbQuestInProgress.FormattingEnabled = true;
            this.cmbQuestInProgress.Location = new System.Drawing.Point(92, 18);
            this.cmbQuestInProgress.Name = "cmbQuestInProgress";
            this.cmbQuestInProgress.Size = new System.Drawing.Size(138, 21);
            this.cmbQuestInProgress.TabIndex = 3;
            this.cmbQuestInProgress.SelectedIndexChanged += new System.EventHandler(this.cmbQuestInProgress_SelectedIndexChanged);
            // 
            // grpStartQuest
            // 
            this.grpStartQuest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpStartQuest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpStartQuest.Controls.Add(this.lblStartQuest);
            this.grpStartQuest.Controls.Add(this.cmbStartQuest);
            this.grpStartQuest.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpStartQuest.Location = new System.Drawing.Point(9, 40);
            this.grpStartQuest.Name = "grpStartQuest";
            this.grpStartQuest.Size = new System.Drawing.Size(236, 71);
            this.grpStartQuest.TabIndex = 31;
            this.grpStartQuest.TabStop = false;
            this.grpStartQuest.Text = "Can Start Quest:";
            this.grpStartQuest.Visible = false;
            // 
            // lblStartQuest
            // 
            this.lblStartQuest.AutoSize = true;
            this.lblStartQuest.Location = new System.Drawing.Point(6, 21);
            this.lblStartQuest.Name = "lblStartQuest";
            this.lblStartQuest.Size = new System.Drawing.Size(38, 13);
            this.lblStartQuest.TabIndex = 5;
            this.lblStartQuest.Text = "Quest:";
            // 
            // cmbStartQuest
            // 
            this.cmbStartQuest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbStartQuest.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbStartQuest.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbStartQuest.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbStartQuest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStartQuest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbStartQuest.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbStartQuest.FormattingEnabled = true;
            this.cmbStartQuest.Location = new System.Drawing.Point(92, 18);
            this.cmbStartQuest.Name = "cmbStartQuest";
            this.cmbStartQuest.Size = new System.Drawing.Size(138, 21);
            this.cmbStartQuest.TabIndex = 3;
            // 
            // grpLevelStat
            // 
            this.grpLevelStat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpLevelStat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpLevelStat.Controls.Add(this.cmbLevelStat);
            this.grpLevelStat.Controls.Add(this.lblLevelOrStat);
            this.grpLevelStat.Controls.Add(this.scrlLevel);
            this.grpLevelStat.Controls.Add(this.lblLvlStatValue);
            this.grpLevelStat.Controls.Add(this.cmbLevelComparator);
            this.grpLevelStat.Controls.Add(this.lblLevelComparator);
            this.grpLevelStat.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpLevelStat.Location = new System.Drawing.Point(9, 40);
            this.grpLevelStat.Name = "grpLevelStat";
            this.grpLevelStat.Size = new System.Drawing.Size(236, 122);
            this.grpLevelStat.TabIndex = 28;
            this.grpLevelStat.TabStop = false;
            this.grpLevelStat.Text = "Level or Stat is...";
            // 
            // cmbLevelStat
            // 
            this.cmbLevelStat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbLevelStat.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbLevelStat.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbLevelStat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbLevelStat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLevelStat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbLevelStat.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbLevelStat.FormattingEnabled = true;
            this.cmbLevelStat.Items.AddRange(new object[] {
            "Level",
            "Attack",
            "Defense",
            "Speed",
            "Ability Power",
            "Magic Resist"});
            this.cmbLevelStat.Location = new System.Drawing.Point(79, 23);
            this.cmbLevelStat.Name = "cmbLevelStat";
            this.cmbLevelStat.Size = new System.Drawing.Size(151, 21);
            this.cmbLevelStat.TabIndex = 7;
            // 
            // lblLevelOrStat
            // 
            this.lblLevelOrStat.AutoSize = true;
            this.lblLevelOrStat.Location = new System.Drawing.Point(7, 25);
            this.lblLevelOrStat.Name = "lblLevelOrStat";
            this.lblLevelOrStat.Size = new System.Drawing.Size(70, 13);
            this.lblLevelOrStat.TabIndex = 6;
            this.lblLevelOrStat.Text = "Level or Stat:";
            // 
            // scrlLevel
            // 
            this.scrlLevel.Location = new System.Drawing.Point(79, 84);
            this.scrlLevel.Maximum = 32000;
            this.scrlLevel.Name = "scrlLevel";
            this.scrlLevel.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrlLevel.Size = new System.Drawing.Size(151, 17);
            this.scrlLevel.TabIndex = 5;
            this.scrlLevel.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrlLevel_Scroll);
            // 
            // lblLvlStatValue
            // 
            this.lblLvlStatValue.AutoSize = true;
            this.lblLvlStatValue.Location = new System.Drawing.Point(10, 89);
            this.lblLvlStatValue.Name = "lblLvlStatValue";
            this.lblLvlStatValue.Size = new System.Drawing.Size(46, 13);
            this.lblLvlStatValue.TabIndex = 4;
            this.lblLvlStatValue.Text = "Value: 0";
            // 
            // cmbLevelComparator
            // 
            this.cmbLevelComparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbLevelComparator.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbLevelComparator.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbLevelComparator.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbLevelComparator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLevelComparator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbLevelComparator.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbLevelComparator.FormattingEnabled = true;
            this.cmbLevelComparator.Location = new System.Drawing.Point(79, 53);
            this.cmbLevelComparator.Name = "cmbLevelComparator";
            this.cmbLevelComparator.Size = new System.Drawing.Size(151, 21);
            this.cmbLevelComparator.TabIndex = 3;
            // 
            // lblLevelComparator
            // 
            this.lblLevelComparator.AutoSize = true;
            this.lblLevelComparator.Location = new System.Drawing.Point(7, 55);
            this.lblLevelComparator.Name = "lblLevelComparator";
            this.lblLevelComparator.Size = new System.Drawing.Size(64, 13);
            this.lblLevelComparator.TabIndex = 2;
            this.lblLevelComparator.Text = "Comparator:";
            // 
            // grpTime
            // 
            this.grpTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.grpTime.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.grpTime.Controls.Add(this.lblEndRange);
            this.grpTime.Controls.Add(this.lblStartRange);
            this.grpTime.Controls.Add(this.cmbTime2);
            this.grpTime.Controls.Add(this.cmbTime1);
            this.grpTime.Controls.Add(this.lblAnd);
            this.grpTime.ForeColor = System.Drawing.Color.Gainsboro;
            this.grpTime.Location = new System.Drawing.Point(10, 40);
            this.grpTime.Name = "grpTime";
            this.grpTime.Size = new System.Drawing.Size(236, 105);
            this.grpTime.TabIndex = 30;
            this.grpTime.TabStop = false;
            this.grpTime.Text = "Time is between:";
            this.grpTime.Visible = false;
            // 
            // lblEndRange
            // 
            this.lblEndRange.AutoSize = true;
            this.lblEndRange.Location = new System.Drawing.Point(9, 73);
            this.lblEndRange.Name = "lblEndRange";
            this.lblEndRange.Size = new System.Drawing.Size(64, 13);
            this.lblEndRange.TabIndex = 6;
            this.lblEndRange.Text = "End Range:";
            // 
            // lblStartRange
            // 
            this.lblStartRange.AutoSize = true;
            this.lblStartRange.Location = new System.Drawing.Point(6, 21);
            this.lblStartRange.Name = "lblStartRange";
            this.lblStartRange.Size = new System.Drawing.Size(67, 13);
            this.lblStartRange.TabIndex = 5;
            this.lblStartRange.Text = "Start Range:";
            // 
            // cmbTime2
            // 
            this.cmbTime2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTime2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTime2.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTime2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTime2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTime2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTime2.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTime2.FormattingEnabled = true;
            this.cmbTime2.Location = new System.Drawing.Point(92, 70);
            this.cmbTime2.Name = "cmbTime2";
            this.cmbTime2.Size = new System.Drawing.Size(138, 21);
            this.cmbTime2.TabIndex = 4;
            // 
            // cmbTime1
            // 
            this.cmbTime1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbTime1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbTime1.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbTime1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTime1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTime1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTime1.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbTime1.FormattingEnabled = true;
            this.cmbTime1.Location = new System.Drawing.Point(92, 18);
            this.cmbTime1.Name = "cmbTime1";
            this.cmbTime1.Size = new System.Drawing.Size(138, 21);
            this.cmbTime1.TabIndex = 3;
            // 
            // lblAnd
            // 
            this.lblAnd.AutoSize = true;
            this.lblAnd.Location = new System.Drawing.Point(100, 49);
            this.lblAnd.Name = "lblAnd";
            this.lblAnd.Size = new System.Drawing.Size(26, 13);
            this.lblAnd.TabIndex = 2;
            this.lblAnd.Text = "And";
            // 
            // EventCommand_ConditionalBranch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.grpConditional);
            this.Name = "EventCommand_ConditionalBranch";
            this.Size = new System.Drawing.Size(262, 208);
            this.grpConditional.ResumeLayout(false);
            this.grpConditional.PerformLayout();
            this.grpGender.ResumeLayout(false);
            this.grpGender.PerformLayout();
            this.grpPowerIs.ResumeLayout(false);
            this.grpPowerIs.PerformLayout();
            this.grpSelfSwitch.ResumeLayout(false);
            this.grpSelfSwitch.PerformLayout();
            this.grpSpell.ResumeLayout(false);
            this.grpSpell.PerformLayout();
            this.grpClass.ResumeLayout(false);
            this.grpClass.PerformLayout();
            this.grpHasItem.ResumeLayout(false);
            this.grpHasItem.PerformLayout();
            this.grpSwitch.ResumeLayout(false);
            this.grpSwitch.PerformLayout();
            this.grpPlayerVariable.ResumeLayout(false);
            this.grpPlayerVariable.PerformLayout();
            this.grpQuestCompleted.ResumeLayout(false);
            this.grpQuestCompleted.PerformLayout();
            this.grpQuestInProgress.ResumeLayout(false);
            this.grpQuestInProgress.PerformLayout();
            this.grpStartQuest.ResumeLayout(false);
            this.grpStartQuest.PerformLayout();
            this.grpLevelStat.ResumeLayout(false);
            this.grpLevelStat.PerformLayout();
            this.grpTime.ResumeLayout(false);
            this.grpTime.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkGroupBox grpConditional;
        private DarkButton btnCancel;
        private DarkButton btnSave;
        private DarkGroupBox grpSwitch;
        private DarkComboBox cmbSwitchVal;
        private System.Windows.Forms.Label lblSwitchIs;
        private DarkComboBox cmbSwitch;
        private System.Windows.Forms.Label lblSwitch;
        private DarkComboBox cmbConditionType;
        private System.Windows.Forms.Label lblType;
        private DarkGroupBox grpPlayerVariable;
        private DarkComboBox cmbVariableMod;
        private System.Windows.Forms.Label lblComparator;
        private DarkComboBox cmbVariable;
        private System.Windows.Forms.Label lblVariable;
        private DarkTextBox txtVariableVal;
        private System.Windows.Forms.Label lblVariableValue;
        private DarkGroupBox grpHasItem;
        private DarkScrollBar scrlItemQuantity;
        private DarkComboBox cmbItem;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.Label lblItemQuantity;
        private DarkGroupBox grpSpell;
        private DarkComboBox cmbSpell;
        private System.Windows.Forms.Label lblSpell;
        private DarkGroupBox grpClass;
        private DarkComboBox cmbClass;
        private System.Windows.Forms.Label lblClass;
        private DarkGroupBox grpLevelStat;
        private DarkScrollBar scrlLevel;
        private System.Windows.Forms.Label lblLvlStatValue;
        private DarkComboBox cmbLevelComparator;
        private System.Windows.Forms.Label lblLevelComparator;
        private DarkGroupBox grpSelfSwitch;
        private DarkComboBox cmbSelfSwitchVal;
        private System.Windows.Forms.Label lblSelfSwitchIs;
        private DarkComboBox cmbSelfSwitch;
        private System.Windows.Forms.Label lblSelfSwitch;
        private DarkGroupBox grpPowerIs;
        private DarkComboBox cmbPower;
        private System.Windows.Forms.Label lblPower;
        private DarkGroupBox grpTime;
        private DarkComboBox cmbTime2;
        private DarkComboBox cmbTime1;
        private System.Windows.Forms.Label lblAnd;
        private System.Windows.Forms.Label lblEndRange;
        private System.Windows.Forms.Label lblStartRange;
        private DarkGroupBox grpQuestInProgress;
        private System.Windows.Forms.Label lblQuestTask;
        private DarkComboBox cmbQuestTask;
        private DarkComboBox cmbTaskModifier;
        private System.Windows.Forms.Label lblQuestIs;
        private System.Windows.Forms.Label lblQuestProgress;
        private DarkComboBox cmbQuestInProgress;
        private DarkGroupBox grpStartQuest;
        private System.Windows.Forms.Label lblStartQuest;
        private DarkComboBox cmbStartQuest;
        private DarkGroupBox grpQuestCompleted;
        private System.Windows.Forms.Label lblQuestCompleted;
        private DarkComboBox cmbCompletedQuest;
        private DarkComboBox cmbLevelStat;
        private System.Windows.Forms.Label lblLevelOrStat;
        private DarkGroupBox grpGender;
        private DarkComboBox cmbGender;
        private System.Windows.Forms.Label lblGender;
    }
}
